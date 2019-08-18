using System.ComponentModel.DataAnnotations;

namespace FactoryDataClient.ViewModels
{
    public class PlcFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Controller Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "IP Address")]
        public string IPAddress { get; set; }

        [Required]
        [Display(Name = "Disable Subscriptions")]
        public bool DisableSubscriptions { get; set; }

        [Required]
        [Display(Name = "PollRate Override")]
        public int PollRateOverride { get; set; }

        [Required]
        [Display(Name = "Processor Slot")]
        public int ProcessorSlot { get; set; }

        [Required]
        [Display(Name = "Port")]
        public int Port { get; set; }

        [Required]
        [Display(Name = "Event PollRate")]
        public int EventPollRate { get; set; }

        [Required]
        [Display(Name = "Subscription PollRate")]
        public int SubscriptionPollRate { get; set; }

        [Required]
        [Display(Name = "Transaction PollRate")]
        public int TransactionPollRate { get; set; }
    }


}