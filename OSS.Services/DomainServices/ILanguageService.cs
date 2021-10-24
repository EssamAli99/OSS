using OSS.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OSS.Services.DomainServices
{
    public interface ILanguageService
    {
        Task<List<LanguageModel>> GetLanguagesAsync();
    }
}
