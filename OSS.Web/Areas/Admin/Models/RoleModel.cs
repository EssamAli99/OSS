using OSS.Services.Models;
using System.Collections.Generic;

namespace OSS.Web.Areas.Admin.Models
{
    public class RoleModel : BaseModel
    {
        public RoleModel()
        {
            AppPages = new List<AppPageModel>();
        }
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<AppPageModel> AppPages { get; set; }
    }
}
