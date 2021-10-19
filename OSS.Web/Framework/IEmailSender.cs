﻿using OSS.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OSS.Web.Framework
{
    public partial interface IEmailSender
    {
        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="emailAccount">Email account to use</param>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="fromAddress">From address</param>
        /// <param name="fromName">From display name</param>
        /// <param name="toAddress">To address</param>
        /// <param name="toName">To display name</param>
        /// <param name="replyToAddress">ReplyTo address</param>
        /// <param name="replyToName">ReplyTo display name</param>
        /// <param name="bcc">BCC addresses list</param>
        /// <param name="cc">CC addresses list</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <param name="attachedDownloadId">Attachment download ID (another attachment)</param>
        /// <param name="headers">Headers</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SendEmailAsync(EmailAccountModel emailAccount, string subject, string body,
            string fromAddress, string fromName, string toAddress, string toName,
             string replyToAddress = null, string replyToName = null,
            IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
            string attachmentFilePath = null, string attachmentFileName = null,
            IDictionary<string, string> headers = null);

        /// <summary>
        /// Send Email with fewer parameters using the default email account
        /// </summary>
        /// <param name="toAddress">To email address</param>
        /// <param name="subject">email subject</param>
        /// <param name="body">email body</param>
        /// <returns></returns>
        Task SendEmailAsync(string toAddress, string subject, string body);
    }
}