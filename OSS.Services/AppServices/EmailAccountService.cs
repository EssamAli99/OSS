using Microsoft.EntityFrameworkCore;
using OSS.Data.Entities;
using OSS.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSS.Services.AppServices
{
    public partial class EmailAccountService : IEmailAccountService
    {
        #region Fields

        private readonly ApplicationDbContext _ctx;

        #endregion

        #region Ctor

        public EmailAccountService(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        #endregion

        #region Methods

        private async Task<EmailAccount> GetEntity(string encryptedId)
        {
            //TODO: add data protection encrypt and decrypt
            int Id = Int32.Parse(encryptedId);
            return await _ctx.EmailAccount.FindAsync(Id);
        }

        public virtual async Task InsertAsync(EmailAccountModel model)
        {
            if (model == null)  throw new ArgumentNullException(nameof(model));

            await _ctx.EmailAccount.AddAsync(new EmailAccount
            {
                Email = model.Email.Trim(),
                DisplayName = model.DisplayName.Trim(),
                Host = model.Host.Trim(),
                Username = model.Username.Trim(),
                Password = model.Password.Trim(),
            });
            await _ctx.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(EmailAccountModel model)
        {
            if (model == null)  throw new ArgumentNullException(nameof(model));
            var entity = await GetEntity(model.EncrypedId);
            if ( entity != null)
            {
                entity.Email = model.Email.Trim();
                entity.DisplayName = model.DisplayName.Trim();
                entity.Host = model.Host.Trim();
                entity.Username = model.Username.Trim();
                entity.Password = model.Password.Trim();

                _ctx.EmailAccount.Update(entity);
                await _ctx.SaveChangesAsync();
            }
        }

        public virtual async Task DeleteAsync(EmailAccountModel model)
        {
            if (model == null)  throw new ArgumentNullException(nameof(model));

            if ((await GetAllAsync()).Count == 1)
                throw new Exception("You cannot delete this email account. At least one account is required.");

            var entity = await GetEntity(model.EncrypedId);
            if (entity != null)
            {
                _ctx.EmailAccount.Remove(entity);
                await _ctx.SaveChangesAsync();
            }
        }

        public virtual async Task<EmailAccountModel> GetByIdAsync(string encryptedId)
        {
            var entity = await GetEntity(encryptedId);
            if (entity != null)
            {
                return new EmailAccountModel
                {
                    DisplayName = entity.DisplayName,
                    Email = entity.Email,
                    EnableSsl = entity.EnableSsl,
                    EncrypedId = entity.Id.ToString(),
                    Host = entity.Host,
                    ModelMode = ModelActions.List,
                    Password = entity.Password,
                    Port = entity.Port,
                    UseDefaultCredentials = entity.UseDefaultCredentials,
                    Username = entity.Username
                };
            }
            return null;
        }

        public virtual async Task<IList<EmailAccountModel>> GetAllAsync()
        {
            return await _ctx.EmailAccount.AsNoTracking()
                .Select(x=> new EmailAccountModel
                {
                    DisplayName = x.DisplayName,
                    Email = x.Email,
                    EnableSsl = x.EnableSsl,
                    EncrypedId = x.Id.ToString(),
                    Host = x.Host,
                    ModelMode = ModelActions.List,
                    Password = x.Password,
                    Port = x.Port,
                    UseDefaultCredentials = x.UseDefaultCredentials,
                    Username = x.Username
                })
                .ToListAsync();
        }

        public async Task<EmailAccountModel> GetDefaultEmailAsync()
        {
            return await _ctx.EmailAccount.AsNoTracking().OrderBy(x => x.Id)
                .Select(x => new EmailAccountModel
                {
                    DisplayName = x.DisplayName,
                    Email = x.Email,
                    EnableSsl = x.EnableSsl,
                    EncrypedId = x.Id.ToString(),
                    Host = x.Host,
                    ModelMode = ModelActions.List,
                    Password = x.Password,
                    Port = x.Port,
                    UseDefaultCredentials = x.UseDefaultCredentials,
                    Username = x.Username
                })
                .FirstOrDefaultAsync();
        }

        #endregion
    }
}
