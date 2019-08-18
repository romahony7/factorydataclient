using System.ComponentModel.DataAnnotations;

namespace FactoryDataClient.Models
{
    public class TagType
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }
    }
}