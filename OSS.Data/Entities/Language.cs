
namespace OSS.Data.Entities
{
    public partial class Language : BaseEntity
    {
        public string Name { get; set; }
        public string LanguageCulture { get; set; }
        public int DisplayOrder { get; set; }
    }
}
