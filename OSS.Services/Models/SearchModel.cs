using System.Collections.Generic;

namespace OSS.Services.Models
{
    public class SearchModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string OrderDirection { get; set; } = "desc";
        public string OrderBy { get; set; } = "Id";
        public Dictionary<string, string> WhereFieldsWithAND { get; set; } = null;
        public Dictionary<string, string> WhereFieldsWithOR { get; set; } = null;
    }
}
