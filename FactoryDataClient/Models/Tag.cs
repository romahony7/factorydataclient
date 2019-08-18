using System.ComponentModel.DataAnnotations;

namespace FactoryDataClient.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Tag Name")]
        public string Name { get; set; }

        public TagType TagType { get; set; }

        [Required]
        [Display(Name = "Tag Type")]
        public int TagTypeId { get; set; }

        public Plc Plc { get; set; }

        [Required]
        [Display(Name = "Controller Name")]
        public int PlcId { get; set; }

        [Required]
        [Display(Name = "Tag Active")]
        public bool IsActive { get; set; }


    }
}