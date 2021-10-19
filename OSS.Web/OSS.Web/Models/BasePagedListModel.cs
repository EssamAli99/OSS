using OSS.Services.Models;
using System.Collections.Generic;

namespace OSS.Web.Models
{
    public class BasePagedListModel<T> where T : BaseModel
    {
        public IEnumerable<T> data { get; set; }
        public string draw { get; set; }
        public int recordsFiltered { get; set; }
        public int recordsTotal { get; set; }
    }
}
