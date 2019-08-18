using FactoryDataClient.Models;

namespace FactoryDataClient.ViewModels
{
    public class TagDetailViewModel
    {
        public Tag Tag { get; set; }

        public TagType TagType { get; set; }

        public Plc Plc { get; set; }
    }
}