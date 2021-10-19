namespace OSS.Services.Models
{
    public abstract class BaseModel
    {
        public string EncrypedId { get; set; }
        public ModelActions ModelMode { get; set; }
    }
}
