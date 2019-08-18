using System;

namespace Common
{
    [Serializable]
    public class PlcConfigMsg
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public bool DisableSubscriptions { get; set; }
        public int PollRateOverride { get; set; }
        public int ProcessorSlot { get; set; }
        public int Port { get; set; }
        public int EventPollRate { get; set; }
        public int SubscriptionPollRate { get; set; }
        public int TransactionPollRate { get; set; }
    }
}
