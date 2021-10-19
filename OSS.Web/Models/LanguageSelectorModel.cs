using OSS.Services.Models;
using System.Collections.Generic;

namespace OSS.Web.Models
{
    public partial class LanguageSelectorModel 
    {
        public LanguageSelectorModel()
        {
            AvailableLanguages = new List<LanguageModel>();
        }

        public IList<LanguageModel> AvailableLanguages { get; set; }

        public int CurrentLanguageId { get; set; }

    }
}
