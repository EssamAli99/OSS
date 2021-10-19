using OSS.Data.Entities;
using OSS.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSS.Services.DomainServices
{
    public interface IPersonService
    {
        PersonModel PrepareMode(string encryptedId);
        IList<PersonModel> PrepareModePagedList(IEnumerable<KeyValuePair<string, string>> param);
        Task<IEnumerable<PersonModel>> PrepareModeListAsync(Func<Person, bool>? where);
        Task<PersonModel> Save(PersonModel model);
        Task Delete(string encryptedId);
        Task<int> GetTotal(Func<PersonModel, bool>? where);
    }
}
