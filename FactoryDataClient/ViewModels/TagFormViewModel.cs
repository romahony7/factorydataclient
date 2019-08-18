using FactoryDataClient.Models;
using System.Collections.Generic;

namespace FactoryDataClient.ViewModels
{
    public class TagFormViewModel
    {
        public Tag Tag { get; set; }

        public IEnumerable<TagType> TagTypes { get; set; }

        public IEnumerable<Plc> Plcs { get; set; }


    }
}
