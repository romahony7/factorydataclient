using FactoryDataClient.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace FactoryDataClient.Dtos
{
    public class SubscriptionTagRecordDto
    {
        public int Id { get; set; }

        public Tag Tag { get; set; }

        [Required]
        public int TagId { get; set; }

        [Required]
        public int Data { get; set; }

        [Required]
        public DateTime PlcTS { get; set; }

        [Required]
        public DateTime RecordTS { get; set; }
    }
}