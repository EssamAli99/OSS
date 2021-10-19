using OSS.Services.Models;
using System.Collections.Generic;

namespace OSS.Services.DomainServices
{
    public interface ILanguageService
    {
        List<LanguageModel> GetLanguages();
    }
}
