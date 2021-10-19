using Microsoft.EntityFrameworkCore;
using OSS.Data.Entities;
using OSS.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.Services.AppServices
{
    /// <summary>
    /// Queued email service
    /// </summary>
    public partial class QueuedEmailService : IQueuedEmailService
    {
        #region Fields

        private readonly ApplicationDbContext _ctx;

        #endregion

        #region Ctor

        public QueuedEmailService(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>        
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertQueuedEmailAsync(QueuedEmailModel model)
        {
            if (model == null) throw new NullReferenceException("queued email model is null");
            await _ctx.QueuedEmail.AddAsync(new QueuedEmail
            {
                AttachedDownloadId = model.AttachedDownloadId,
                AttachmentFileName = model.AttachmentFileName,
                AttachmentFilePath = model.AttachmentFilePath,
                Bcc = model.Bcc,
                Body = model.Body,
                CC = model.CC,
                CreatedOnUtc = DateTime.UtcNow,
                DontSendBeforeDateUtc = model.DontSendBeforeDateUtc,
                EmailAccountId = model.EmailAccountId,
                From = model.From,
                FromName = model.FromName,
                PriorityId = model.PriorityId,
                ReplyTo = model.ReplyTo,
                ReplyToName = model.ReplyToName,
                Subject = model.Subject,
                To = model.To,
                ToName = model.ToName
            });
            await _ctx.SaveChangesAsync();
        }

        private async Task<QueuedEmail> GetEntity(string encryptedId)
        {
            //TODO: add data protection encrypt and decrypt
            int Id = Int32.Parse(encryptedId);
            return await _ctx.QueuedEmail.FindAsync(Id);
        }

        /// <summary>
        /// Updates a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateQueuedEmailAsync(QueuedEmailModel model)
        {
            var entity = await GetEntity(model.EncrypedId);
            if (entity == null) throw new NullReferenceException("queued email entity is null");

            entity.AttachedDownloadId = model.AttachedDownloadId;
            entity.AttachmentFileName = model.AttachmentFileName;
            entity.AttachmentFilePath = model.AttachmentFilePath;
            entity.Bcc = model.Bcc;
            entity.Body = model.Body;
            entity.CC = model.CC;
            entity.CreatedOnUtc = DateTime.UtcNow;
            entity.DontSendBeforeDateUtc = model.DontSendBeforeDateUtc;
            entity.EmailAccountId = model.EmailAccountId;
            entity.From = model.From;
            entity.FromName = model.FromName;
            entity.PriorityId = model.PriorityId;
            entity.ReplyTo = model.ReplyTo;
            entity.ReplyToName = model.ReplyToName;
            entity.Subject = model.Subject;
            entity.To = model.To;
            entity.ToName = model.ToName;
            entity.SentOnUtc = DateTime.UtcNow;
            entity.SentTries = model.SentTries;

            await _ctx.SaveChangesAsync();
        }

        /// <summary>
        /// Deleted a queued email
        /// </summary>
        /// <param name="queuedEmail">Queued email</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteQueuedEmailAsync(QueuedEmailModel model)
        {
            var entity = await GetEntity(model.EncrypedId);
            if (entity != null)
            {
                _ctx.QueuedEmail.Remove(entity);
                await _ctx.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Gets a queued email by identifier
        /// </summary>
        /// <param name="queuedEmailId">Queued email identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email
        /// </returns>
        public virtual async Task<QueuedEmailModel> GetQueuedEmailByIdAsync(string queuedEmailId)
        {
            if (int.TryParse(queuedEmailId, out int id))
            {
                var entity = await _ctx.QueuedEmail.FindAsync(id);
                if (entity != null)
                {
                    return new QueuedEmailModel
                    {
                        AttachedDownloadId = entity.AttachedDownloadId,
                        AttachmentFileName = entity.AttachmentFileName,
                        AttachmentFilePath = entity.AttachmentFilePath,
                        Bcc = entity.Bcc,
                        Body = entity.Body,
                        CC = entity.CC,
                        CreatedOnUtc = DateTime.UtcNow,
                        DontSendBeforeDateUtc = entity.DontSendBeforeDateUtc,
                        EmailAccountId = entity.EmailAccountId,
                        From = entity.From,
                        FromName = entity.FromName,
                        Priority = (QueuedEmailPriority)entity.PriorityId,
                        PriorityId = entity.PriorityId,
                        ReplyTo = entity.ReplyTo,
                        ReplyToName = entity.ReplyToName,
                        Subject = entity.Subject,
                        To = entity.To,
                        ToName = entity.ToName,
                        EncrypedId = entity.Id.ToString(),
                        ModelMode = ModelActions.Edit,
                        SentOnUtc = entity.SentOnUtc,
                        SentTries = entity.SentTries
                    };
                }
            }
            return null;
        }

        /// <summary>
        /// Get queued emails by identifiers
        /// </summary>
        /// <param name="queuedEmailIds">queued email identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued emails
        /// </returns>
        public virtual async Task<IList<QueuedEmailModel>> GetQueuedEmailsByIdsAsync(string[] queuedEmailIds)
        {
            int[] ids = queuedEmailIds.Select(x => int.Parse(x)).ToArray();
            return await _ctx.QueuedEmail.AsNoTracking().Where(x => ids.Contains(x.Id))
                .Select(entity => new QueuedEmailModel
                {
                    AttachedDownloadId = entity.AttachedDownloadId,
                    AttachmentFileName = entity.AttachmentFileName,
                    AttachmentFilePath = entity.AttachmentFilePath,
                    Bcc = entity.Bcc,
                    Body = entity.Body,
                    CC = entity.CC,
                    CreatedOnUtc = DateTime.UtcNow,
                    DontSendBeforeDateUtc = entity.DontSendBeforeDateUtc,
                    EmailAccountId = entity.EmailAccountId,
                    From = entity.From,
                    FromName = entity.FromName,
                    Priority = (QueuedEmailPriority)entity.PriorityId,
                    PriorityId = entity.PriorityId,
                    ReplyTo = entity.ReplyTo,
                    ReplyToName = entity.ReplyToName,
                    Subject = entity.Subject,
                    To = entity.To,
                    ToName = entity.ToName,
                    EncrypedId = entity.Id.ToString(),
                    ModelMode = ModelActions.Edit,
                    SentOnUtc = entity.SentOnUtc,
                    SentTries = entity.SentTries
                }).ToListAsync();
        }

        /// <summary>
        /// Gets all queued emails
        /// </summary>
        /// <param name="fromEmail">From Email</param>
        /// <param name="toEmail">To Email</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="loadNotSentItemsOnly">A value indicating whether to load only not sent emails</param>
        /// <param name="loadOnlyItemsToBeSent">A value indicating whether to load only emails for ready to be sent</param>
        /// <param name="maxSendTries">Maximum send tries</param>
        /// <param name="loadNewest">A value indicating whether we should sort queued email descending; otherwise, ascending.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the email item list
        /// </returns>
        public virtual async Task<IPagedList<QueuedEmailModel>> SearchEmailsAsync(string fromEmail,
            string toEmail, DateTime? createdFromUtc, DateTime? createdToUtc,
            bool loadNotSentItemsOnly, bool loadOnlyItemsToBeSent, int maxSendTries,
            bool loadNewest, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            fromEmail = (fromEmail ?? string.Empty).Trim();
            toEmail = (toEmail ?? string.Empty).Trim();

            var query = _ctx.QueuedEmail.AsNoTracking();
            if (!string.IsNullOrEmpty(fromEmail))
                query = query.Where(qe => qe.From.Contains(fromEmail));
            if (!string.IsNullOrEmpty(toEmail))
                query = query.Where(qe => qe.To.Contains(toEmail));
            if (createdFromUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc >= createdFromUtc);
            if (createdToUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc <= createdToUtc);
            if (loadNotSentItemsOnly)
                query = query.Where(qe => !qe.SentOnUtc.HasValue);
            if (loadOnlyItemsToBeSent)
            {
                var nowUtc = DateTime.UtcNow;
                query = query.Where(qe => !qe.DontSendBeforeDateUtc.HasValue || qe.DontSendBeforeDateUtc.Value <= nowUtc);
            }

            query = query.Where(qe => qe.SentTries < maxSendTries);
            query = loadNewest ?
                //load the newest records
                query.OrderByDescending(qe => qe.CreatedOnUtc) :
                //load by priority
                query.OrderByDescending(qe => qe.PriorityId).ThenBy(qe => qe.CreatedOnUtc);

            var queuedEmails = await query.Skip(pageIndex * pageSize).Take(pageSize)
                .Select(entity => new QueuedEmailModel
                {
                    AttachedDownloadId = entity.AttachedDownloadId,
                    AttachmentFileName = entity.AttachmentFileName,
                    AttachmentFilePath = entity.AttachmentFilePath,
                    Bcc = entity.Bcc,
                    Body = entity.Body,
                    CC = entity.CC,
                    CreatedOnUtc = DateTime.UtcNow,
                    DontSendBeforeDateUtc = entity.DontSendBeforeDateUtc,
                    EmailAccountId = entity.EmailAccountId,
                    From = entity.From,
                    FromName = entity.FromName,
                    Priority = (QueuedEmailPriority)entity.PriorityId,
                    PriorityId = entity.PriorityId,
                    ReplyTo = entity.ReplyTo,
                    ReplyToName = entity.ReplyToName,
                    Subject = entity.Subject,
                    To = entity.To,
                    ToName = entity.ToName,
                    EncrypedId = entity.Id.ToString(),
                    ModelMode = ModelActions.Edit,
                    SentOnUtc = entity.SentOnUtc,
                    SentTries = entity.SentTries
                })
                .ToListAsync();

            return new PagedList<QueuedEmailModel>(queuedEmails, pageIndex, pageSize);
        }

        /// <summary>
        /// Deletes already sent emails
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of deleted emails
        /// </returns>
        public virtual async Task<int> DeleteAlreadySentEmailsAsync(DateTime? createdFromUtc, DateTime? createdToUtc)
        {
            var query = _ctx.QueuedEmail.AsNoTracking();

            // only sent emails
            query = query.Where(qe => qe.SentOnUtc.HasValue);

            if (createdFromUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc >= createdFromUtc);
            if (createdToUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc <= createdToUtc);

            var emails = await query.ToArrayAsync();

            _ctx.QueuedEmail.RemoveRange(emails);

            return emails.Length;
        }

        #endregion
    }
}
