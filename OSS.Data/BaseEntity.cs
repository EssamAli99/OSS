using System.ComponentModel.DataAnnotations;

namespace OSS.Data
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
