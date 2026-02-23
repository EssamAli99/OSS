#nullable disable
namespace OSS.Data.Entities
{
    public class LocaleStringResource : BaseEntity
    {
        public int LanguageId { get; set; }
        public string ResourceName { get; set; } = string.Empty;
        public string ResourceValue { get; set; } = string.Empty;
        public virtual Language Language { get; set; }
    }
}
