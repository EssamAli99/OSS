using Microsoft.AspNetCore.Identity;

namespace OSS.Web.Framework
{
    public interface IWorkContext
    {
        IdentityUser CurrentUser { get; set; }

        int WorkingLanguageId { get; set; }
        void SetCookie(string key, string value, int? expireTime);

        bool IsAdmin { get; set; }
    }
}
