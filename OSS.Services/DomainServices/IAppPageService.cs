using OSS.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OSS.Services.DomainServices
{
    public interface IAppPageService
    {
        Task<List<AppPageModel>> GetAppPagesAsync(string allowedPaged);
        Task<List<AppPageModel>> GetAppPagesAsync();
    }
}
