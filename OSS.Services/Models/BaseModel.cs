namespace OSS.Services.Models
{
    public abstract class BaseModel
    {
        public int Id { get; set; }
        public ModelActions ModelMode { get; set; }
    }
}
