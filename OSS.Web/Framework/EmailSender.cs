using MimeKit;
using MimeKit.Text;
using OSS.Services.AppServices;
using OSS.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.Web.Framework
{
    public partial class EmailSender : IEmailSender
    {
        #region Fields

        private readonly IOSSFileProvider _FileProvider;
        private readonly IEmailAccountService _EmailAccountService;
        private readonly ISmtpBuilder _SmtpBuilder;

        #endregion

        #region Ctor

        public EmailSender(IOSSFileProvider fileProvider, IEmailAccountService emailService, ISmtpBuilder smtpBuilder)
        {
            _FileProvider = fileProvider;
            _EmailAccountService = emailService;
            _SmtpBuilder = smtpBuilder;
        }

        #endregion

        #region Utilities

        ///// <summary>
        ///// Create an file attachment for the specific download object from DB
        ///// </summary>
        ///// <param name="download">Attachment download (another attachment)</param>
        ///// <returns>A leaf-node MIME part that contains an attachment.</returns>
        //protected MimePart CreateMimeAttachment(Download download)
        //{
        //    if (download is null)
        //        throw new ArgumentNullException(nameof(download));

        //    var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : download.Id.ToString();

        //    return CreateMimeAttachment($"{fileName}{download.Extension}", download.DownloadBinary, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow);
        //}

        /// <summary>
        /// Create an file attachment for the specific file path
        /// </summary>
        /// <param name="filePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a leaf-node MIME part that contains an attachment.
        /// </returns>
        protected async Task<MimePart> CreateMimeAttachmentAsync(string filePath, string attachmentFileName = null)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (string.IsNullOrWhiteSpace(attachmentFileName))
                attachmentFileName = Path.GetFileName(filePath);

            return CreateMimeAttachment(
                    attachmentFileName,
                    await _FileProvider.ReadAllBytesAsync(filePath),
                    _FileProvider.GetCreationTime(filePath),
                    _FileProvider.GetLastWriteTime(filePath),
                    _FileProvider.GetLastAccessTime(filePath));
        }

        /// <summary>
        /// Create an file attachment for the binary data
        /// </summary>
        /// <param name="attachmentFileName">Attachment file name</param>
        /// <param name="binaryContent">The array of unsigned bytes from which to create the attachment stream.</param>
        /// <param name="cDate">Creation date and time for the specified file or directory</param>
        /// <param name="mDate">Date and time that the specified file or directory was last written to</param>
        /// <param name="rDate">Date and time that the specified file or directory was last access to.</param>
        /// <returns>A leaf-node MIME part that contains an attachment.</returns>
        protected MimePart CreateMimeAttachment(string attachmentFileName, byte[] binaryContent, DateTime cDate, DateTime mDate, DateTime rDate)
        {
            if (!ContentType.TryParse(MimeTypes.GetMimeType(attachmentFileName), out var mimeContentType))
                mimeContentType = new ContentType("application", "octet-stream");

            return new MimePart(mimeContentType)
            {
                FileName = attachmentFileName,
                Content = new MimeContent(new MemoryStream(binaryContent)),
                ContentDisposition = new ContentDisposition
                {
                    CreationDate = cDate,
                    ModificationDate = mDate,
                    ReadDate = rDate
                }
            };
        }

        #endregion

        #region Methods
        public virtual async Task SendEmailAsync(string toAddress, string subject, string body)
        {
            var emailAccount = await _EmailAccountService.GetDefaultEmailAsync();
            await SendEmailAsync(emailAccount, subject, body, emailAccount.Email, emailAccount.DisplayName, toAddress, toAddress);
        }
        public virtual async Task SendEmailAsync(EmailAccountModel emailAccount, string subject, string body,
            string fromAddress, string fromName, string toAddress, string toName,
            string replyTo = null, string replyToName = null,
            IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
            string attachmentFilePath = null, string attachmentFileName = null,
            IDictionary<string, string> headers = null)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(fromName, fromAddress));
            message.To.Add(new MailboxAddress(toName, toAddress));

            if (!string.IsNullOrEmpty(replyTo))
            {
                message.ReplyTo.Add(new MailboxAddress(replyToName, replyTo));
            }

            //BCC
            if (bcc != null)
            {
                foreach (var address in bcc.Where(bccValue => !string.IsNullOrWhiteSpace(bccValue)))
                {
                    message.Bcc.Add(new MailboxAddress("", address.Trim()));
                }
            }

            //CC
            if (cc != null)
            {
                foreach (var address in cc.Where(ccValue => !string.IsNullOrWhiteSpace(ccValue)))
                {
                    message.Cc.Add(new MailboxAddress("", address.Trim()));
                }
            }

            //content
            message.Subject = subject;

            //headers
            if (headers != null)
                foreach (var header in headers)
                {
                    message.Headers.Add(header.Key, header.Value);
                }

            var multipart = new Multipart("mixed")
            {
                new TextPart(TextFormat.Html) { Text = body }
            };

            //create the file attachment for this e-mail message
            if (!string.IsNullOrEmpty(attachmentFilePath) && _FileProvider.FileExists(attachmentFilePath))
            {
                multipart.Add(await CreateMimeAttachmentAsync(attachmentFilePath, attachmentFileName));
            }

            message.Body = multipart;

            //send email
            using var smtpClient = await _SmtpBuilder.BuildAsync(emailAccount);
            await smtpClient.SendAsync(message);
            await smtpClient.DisconnectAsync(true);
        }
        #endregion
    }
}
