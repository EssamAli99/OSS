using OSS.Services.Models;
using System.Collections.Generic;

namespace OSS.Services.DomainServices
{
    public interface IAppPageService
    {
        List<AppPageModel> GetAppPages(string allowedPaged);
        List<AppPageModel> GetAppPages();
    }
}
