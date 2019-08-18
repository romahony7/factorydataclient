using System;
using System.Collections.Generic;

namespace Common
{
    [Serializable]
    public class TagReadResultsMsg
    {
        public List<RecordTag> List { get; set; }
    }
}
