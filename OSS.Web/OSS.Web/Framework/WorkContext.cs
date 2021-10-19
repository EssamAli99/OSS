using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;

namespace OSS.Web.Framework
{
    public class WorkContext : IWorkContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public WorkContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public IdentityUser CurrentUser { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int WorkingLanguageId
        {
            get
            {
                string strlangId = _httpContextAccessor.HttpContext.Request.Cookies[OSSConfig.PreferedLanguage];
                if (string.IsNullOrEmpty(strlangId))
                {
                    strlangId = "1"; //English
                    SetCookie(OSSConfig.PreferedLanguage, strlangId, 60 * 24 * 365);

                }
                return int.Parse(strlangId);
            }
            set
            {
                SetCookie(OSSConfig.PreferedLanguage, value.ToString(), 60 * 24 * 365);
            }
        }
        public void SetCookie(string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions
            {
                HttpOnly = false,
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Secure = false,
                Expires = expireTime.HasValue ? DateTime.Now.AddDays(365) : DateTime.Now.AddSeconds(3)
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append(key, value, option);
        }
        public bool IsAdmin { get; set; }
    }
}
