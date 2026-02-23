#nullable disable
using Microsoft.EntityFrameworkCore;
using OSS.Data.Entities;
using OSS.Services.Models;

namespace OSS.Services.AppServices
{
    public class EmailAccountService : IEmailAccountService
    {


        private readonly IRepository<EmailAccount> _repository;





        public EmailAccountService(IRepository<EmailAccount> ctx)
        {
            _repository = ctx;
        }





        private async Task<EmailAccount> GetEntity(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public virtual async Task InsertAsync(EmailAccountModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            await _repository.InsertAsync(new EmailAccount
            {
                Email = model.Email.Trim(),
                DisplayName = model.DisplayName.Trim(),
                Host = model.Host.Trim(),
                Username = model.Username.Trim(),
                Password = model.Password.Trim(),
            });
        }

        public virtual async Task UpdateAsync(EmailAccountModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            var entity = await GetEntity(model.Id);
            if (entity != null)
            {
                entity.Email = model.Email.Trim();
                entity.DisplayName = model.DisplayName.Trim();
                entity.Host = model.Host.Trim();
                entity.Username = model.Username.Trim();
                entity.Password = model.Password.Trim();

                await _repository.UpdateAsync(entity);
            }
        }

        public virtual async Task DeleteAsync(EmailAccountModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            if ((await GetAllAsync()).Count == 1)
                throw new Exception("You cannot delete this email account. At least one account is required.");

            var entity = await GetEntity(model.Id);
            if (entity != null)
            {
                await _repository.DeleteAsync(entity);
            }
        }

        public virtual async Task<EmailAccountModel> GetByIdAsync(int id)
        {
            var entity = await GetEntity(id);
            if (entity != null)
            {
                return new EmailAccountModel
                {
                    DisplayName = entity.DisplayName,
                    Email = entity.Email,
                    EnableSsl = entity.EnableSsl,
                    Id = entity.Id,
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
            return await _repository.TableNoTracking
                .Select(x => new EmailAccountModel
                {
                    DisplayName = x.DisplayName,
                    Email = x.Email,
                    EnableSsl = x.EnableSsl,
                    Id = x.Id,
                    Host = x.Host,
                    ModelMode = ModelActions.List,
                    Password = x.Password,
                    Port = x.Port,
                    UseDefaultCredentials = x.UseDefaultCredentials,
                    Username = x.Username
                }).ToListAsync();
        }

        public async Task<EmailAccountModel> GetDefaultEmailAsync()
        {
            return await _repository.TableNoTracking.OrderBy(x => x.Id)
                .Select(x => new EmailAccountModel
                {
                    DisplayName = x.DisplayName,
                    Email = x.Email,
                    EnableSsl = x.EnableSsl,
                    Id = x.Id,
                    Host = x.Host,
                    ModelMode = ModelActions.List,
                    Password = x.Password,
                    Port = x.Port,
                    UseDefaultCredentials = x.UseDefaultCredentials,
                    Username = x.Username
                }).FirstOrDefaultAsync();
        }


    }
}
