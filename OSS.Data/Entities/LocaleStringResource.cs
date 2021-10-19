namespace OSS.Data.Entities
{
    public partial class LocaleStringResource : BaseEntity
    {
        public int LanguageId { get; set; }
        public string ResourceName { get; set; }
        public string ResourceValue { get; set; }
        public virtual Language Language { get; set; }
    }
}
