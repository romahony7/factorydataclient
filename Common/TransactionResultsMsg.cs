using System;
using System.Collections.Generic;

namespace Common
{
    [Serializable]
    public class TransactionResultsMsg
    {
        public List<TransactionRecord> Records { get; set; }
    }
}
