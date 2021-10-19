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
        public string Id { get; set; }
        public string Name { get; set; }
        public List<AppPageModel> AppPages { get; set; }
    }
}
