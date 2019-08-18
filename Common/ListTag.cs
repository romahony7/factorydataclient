using System;

namespace Common
{
    [Serializable]
    public class ListTag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TagTypeId { get; set; }
        public int PlcId { get; set; }
        public bool IsActive { get; set; }
    }
}
