using System;

namespace Common
{
    [Serializable]
    public class TransactionRecord
    {
        public string TagName { get; set; }

        public TransactionUDT Udt { get; set; }

    }
}
