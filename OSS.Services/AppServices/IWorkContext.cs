using Microsoft.AspNetCore.Identity;

namespace OSS.Services.AppServices
{
    public interface IWorkContext
    {
        IdentityUser CurrentUser { get; set; }
        int WorkingLanguageId { get; set; }
        bool IsAdmin { get; set; }
    }
}
