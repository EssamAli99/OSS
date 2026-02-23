namespace OSS.Services.Models
{
    public class LanguageModel : BaseModel
    {
        public string? Name { get; set; }
        public string? LanguageCulture { get; set; }
        public int DisplayOrder { get; set; }
    }
}
