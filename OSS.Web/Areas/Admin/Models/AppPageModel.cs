using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace OSS.Web.Areas.Admin.Models
{
    public class AppPageModel
    {
        public AppPageModel()
        {
            Permissions = new List<SelectListItem>();
        }
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ControllerName { get; set; } = string.Empty;
        public List<SelectListItem> Permissions { get; set; }
    }
}
