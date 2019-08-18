using System;
using System.ComponentModel.DataAnnotations;

namespace FactoryDataClient.Dtos
{
    public class PlcDto
    {

        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string IPAddress { get; set; }

        [Required]
        public Boolean DisableSubscriptions { get; set; }

        [Required]
        public int PollRateOverride { get; set; }

        [Required]
        public int ProcessorSlot { get; set; }

        [Required]
        public int Port { get; set; }

        [Required]
        public int EventPollRate { get; set; }

        [Required]
        public int SubscriptionPollRate { get; set; }

        [Required]
        public int TransactionPollRate { get; set; }
    }
}