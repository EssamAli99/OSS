using MailKit.Net.Smtp;
using MailKit.Security;
using OSS.Services.AppServices;
using OSS.Services.Models;
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace OSS.Web.Framework
{
    public class SmtpBuilder : ISmtpBuilder
    {
        #region Fields

        private readonly IEmailAccountService _emailAccountService;

        #endregion

        #region Ctor

        public SmtpBuilder(IEmailAccountService emailAccountService)
        {
            _emailAccountService = emailAccountService;
        }

        #endregion

        #region Methods

        public virtual async Task<SmtpClient> BuildAsync(EmailAccountModel emailAccount = null)
        {
            if (emailAccount is null)
            {
                emailAccount = await _emailAccountService.GetDefaultEmailAsync()?? throw new Exception("Email account could not be loaded");
            }

            var client = new SmtpClient
            {
                ServerCertificateValidationCallback = ValidateServerCertificate
            };

            try
            {
                await client.ConnectAsync(
                    emailAccount.Host,
                    emailAccount.Port,
                    emailAccount.EnableSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable);

                if (emailAccount.UseDefaultCredentials)
                {
                    await client.AuthenticateAsync(CredentialCache.DefaultNetworkCredentials);
                }
                else if (!string.IsNullOrWhiteSpace(emailAccount.Username))
                {
                    await client.AuthenticateAsync(new NetworkCredential(emailAccount.Username, emailAccount.Password));
                }

                return client;
            }
            catch (Exception ex)
            {
                client.Dispose();
                throw ;
            }
        }

        public virtual bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //By default, server certificate verification is disabled.
            return true;
        }


        #endregion
    }
}
