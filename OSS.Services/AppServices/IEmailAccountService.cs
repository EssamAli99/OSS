using OSS.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OSS.Services.AppServices
{
    public partial interface IEmailAccountService
    {
        /// <summary>
        /// Inserts an email account
        /// </summary>
        /// <param name="model">Email account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertAsync(EmailAccountModel model);

        /// <summary>
        /// Updates an email account
        /// </summary>
        /// <param name="model">Email account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateAsync(EmailAccountModel model);

        /// <summary>
        /// Deletes an email account
        /// </summary>
        /// <param name="model">Email account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteAsync(EmailAccountModel model);

        /// <summary>
        /// Gets an email account by identifier
        /// </summary>
        /// <param name="encryptedId">The email account identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the email account
        /// </returns>
        Task<EmailAccountModel> GetByIdAsync(string encryptedId);
        Task<EmailAccountModel> GetDefaultEmailAsync();

        /// <summary>
        /// Gets all email accounts
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the email accounts list
        /// </returns>
        Task<IList<EmailAccountModel>> GetAllAsync();
    }

}
