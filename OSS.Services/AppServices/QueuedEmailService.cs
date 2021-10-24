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

        private readonly IRepository<QueuedEmail> _repository;

        #endregion

        #region Ctor

        public QueuedEmailService(IRepository<QueuedEmail> ctx)
        {
            _repository = ctx;
        }

        #endregion

        #region Methods

        public virtual async Task InsertQueuedEmailAsync(QueuedEmailModel model)
        {
            if (model == null) throw new NullReferenceException("queued email model is null");
            await _repository.InsertAsync(new QueuedEmail
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
        }

        private async Task<QueuedEmail> GetEntity(string encryptedId)
        {
            //TODO: add data protection encrypt and decrypt
            int Id = Int32.Parse(encryptedId);
            return await _repository.GetByIdAsync(Id);
        }

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

            await _repository.UpdateAsync(entity);
        }
        public virtual async Task DeleteQueuedEmailAsync(QueuedEmailModel model)
        {
            var entity = await GetEntity(model.EncrypedId);
            if (entity != null)
            {
                await _repository.DeleteAsync(entity);
            }
        }

        public virtual async Task<QueuedEmailModel> GetQueuedEmailByIdAsync(string queuedEmailId)
        {
            if (int.TryParse(queuedEmailId, out int id))
            {
                var entity = await _repository.GetByIdAsync(id);
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

        public virtual async Task<IList<QueuedEmailModel>> GetQueuedEmailsByIdsAsync(string[] queuedEmailIds)
        {
            int[] ids = queuedEmailIds.Select(x => int.Parse(x)).ToArray();
            return await _repository.TableNoTracking.Where(x => ids.Contains(x.Id))
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

        public virtual async Task<IPagedList<QueuedEmailModel>> SearchEmailsAsync(string fromEmail,
            string toEmail, DateTime? createdFromUtc, DateTime? createdToUtc,
            bool loadNotSentItemsOnly, bool loadOnlyItemsToBeSent, int maxSendTries,
            bool loadNewest, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            fromEmail = (fromEmail ?? string.Empty).Trim();
            toEmail = (toEmail ?? string.Empty).Trim();

            var query = _repository.TableNoTracking;
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
                }).ToListAsync();

            return new PagedList<QueuedEmailModel>(queuedEmails, pageIndex, pageSize);
        }

        public virtual async Task<int> DeleteAlreadySentEmailsAsync(DateTime? createdFromUtc, DateTime? createdToUtc)
        {
            var query = _repository.TableNoTracking;

            // only sent emails
            query = query.Where(qe => qe.SentOnUtc.HasValue);

            if (createdFromUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc >= createdFromUtc);
            if (createdToUtc.HasValue)
                query = query.Where(qe => qe.CreatedOnUtc <= createdToUtc);

            var emails = await query.ToArrayAsync();

            await _repository.DeleteAsync(emails);

            return emails.Length;
        }

        #endregion
    }
}
