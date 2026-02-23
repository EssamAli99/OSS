
namespace OSS.Data.Entities
{
    public class Language : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string LanguageCulture { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
