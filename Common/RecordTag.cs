using System;

namespace Common
{
    [Serializable]
    public class RecordTag
    {
        public int TagId { get; set; }
        public long Data { get; set; }
        public System.DateTime PlcTS { get; set; }
    }
}
