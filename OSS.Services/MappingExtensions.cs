#nullable disable
using OSS.Data.Entities;
using OSS.Services.Models;

namespace OSS.Services
{
    /// <summary>
    /// Fix-6: Manual mapping extension methods replace AutoMapper.
    /// One ToModel / ToEntity pair per entity — explicit, zero-reflection, and testable.
    /// </summary>
    public static class MappingExtensions
    {
        // -- TestTable ----------------------------------------------------------
        public static TestTableModel ToModel(this TestTable entity) => new()
        {
            Id = entity.Id,
            Text1 = entity.Text1,
            Text2 = entity.Text2,
        };

        public static TestTable ToEntity(this TestTableModel model) => new()
        {
            Id = model.Id,
            Text1 = model.Text1,
            Text2 = model.Text2,
        };

        public static TestTable ApplyTo(this TestTableModel model, TestTable entity)
        {
            entity.Id = model.Id;
            entity.Text1 = model.Text1;
            entity.Text2 = model.Text2;
            return entity;
        }

        // -- Log ---------------------------------------------------------------
        public static LogModel ToModel(this Log entity) => new()
        {
            Id = entity.Id,
            LogLevelId = entity.LogLevelId,
            ShortMessage = entity.ShortMessage,
            FullMessage = entity.FullMessage,
            IpAddress = entity.IpAddress,
            UserId = entity.UserId,
            PageUrl = entity.PageUrl,
            ReferrerUrl = entity.ReferrerUrl,
            CreatedOnUtc = entity.CreatedOnUtc,
            CreatedOn = entity.CreatedOnUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
        };

        // -- AppPage -----------------------------------------------------------
        public static AppPageModel ToModel(this AppPage entity) => new()
        {
            Id = entity.Id,
            SystemName = entity.SystemName,
            Title = entity.Title,
            ControllerName = entity.ControllerName,
            ActionName = entity.ActionName,
            IconClass = entity.IconClass,
            PermissionNames = entity.PermissionNames,
            PageOrder = entity.PageOrder,
            AppPageId = entity.AppPageId,
            AreaName = entity.AreaName,
        };

        // -- EmailAccount ------------------------------------------------------
        public static EmailAccountModel ToModel(this EmailAccount entity) => new()
        {
            Id = entity.Id,
            Email = entity.Email,
            DisplayName = entity.DisplayName,
            Host = entity.Host,
            Port = entity.Port,
            Username = entity.Username,
            Password = entity.Password,
            EnableSsl = entity.EnableSsl,
            UseDefaultCredentials = entity.UseDefaultCredentials,
        };

        public static EmailAccount ToEntity(this EmailAccountModel model) => new()
        {
            Id = model.Id,
            Email = model.Email,
            DisplayName = model.DisplayName,
            Host = model.Host,
            Port = model.Port,
            Username = model.Username,
            Password = model.Password,
            EnableSsl = model.EnableSsl,
            UseDefaultCredentials = model.UseDefaultCredentials,
        };

        public static EmailAccount ApplyTo(this EmailAccountModel model, EmailAccount entity)
        {
            entity.Id = model.Id;
            entity.Email = model.Email;
            entity.DisplayName = model.DisplayName;
            entity.Host = model.Host;
            entity.Port = model.Port;
            entity.Username = model.Username;
            entity.Password = model.Password;
            entity.EnableSsl = model.EnableSsl;
            entity.UseDefaultCredentials = model.UseDefaultCredentials;
            return entity;
        }

        // -- Language ----------------------------------------------------------
        public static LanguageModel ToModel(this Language entity) => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            LanguageCulture = entity.LanguageCulture,
            DisplayOrder = entity.DisplayOrder,
        };

        public static Language ToEntity(this LanguageModel model) => new()
        {
            Id = model.Id,
            Name = model.Name,
            LanguageCulture = model.LanguageCulture,
            DisplayOrder = model.DisplayOrder,
        };

        // -- QueuedEmail -------------------------------------------------------
        public static QueuedEmailModel ToModel(this QueuedEmail entity) => new()
        {
            Id = entity.Id,
            PriorityId = entity.PriorityId,
            From = entity.From,
            FromName = entity.FromName,
            To = entity.To,
            ToName = entity.ToName,
            ReplyTo = entity.ReplyTo,
            ReplyToName = entity.ReplyToName,
            CC = entity.CC,
            Bcc = entity.Bcc,
            Subject = entity.Subject,
            Body = entity.Body,
            AttachmentFilePath = entity.AttachmentFilePath,
            AttachmentFileName = entity.AttachmentFileName,
            AttachedDownloadId = entity.AttachedDownloadId,
            CreatedOnUtc = entity.CreatedOnUtc,
            DontSendBeforeDateUtc = entity.DontSendBeforeDateUtc,
            SentTries = entity.SentTries,
            SentOnUtc = entity.SentOnUtc,
            EmailAccountId = entity.EmailAccountId,
        };
    }
}
