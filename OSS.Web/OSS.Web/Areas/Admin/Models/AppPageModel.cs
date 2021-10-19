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
        public string Title { get; set; }
        public string ControllerName { get; set; }
        public List<SelectListItem> Permissions { get; set; }
    }
}
