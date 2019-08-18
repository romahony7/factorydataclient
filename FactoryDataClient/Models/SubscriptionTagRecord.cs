using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FactoryDataClient.Models
{
    public class SubscriptionTagRecord
    {
        public int Id { get; set; }

        public Tag Tag { get; set; }

        [Required]
        public int TagId { get; set; }

        [Required]
        [Column(TypeName = "bigint")]
        [Display(Name = "Tag Data")]
        public int Data { get; set; }

        [Required]
        [Display(Name = "PLC Timestamp")]
        public DateTime PlcTS { get; set; }

        [Required]
        [Display(Name = "DB Record Timestamp")]
        public DateTime RecordTS { get; set; }
    }
}