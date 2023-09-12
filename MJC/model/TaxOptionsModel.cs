using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJC.model
{
    class TaxData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Selected { get; set; }

    }

    class TaxOptions
    {
        static public List<TaxData> TaxData = new List<TaxData>()
            {
                new TaxData() { Id = "1", Name = "Yes", Selected = "Yes" },
                new TaxData() { Id = "2", Name= "No", Selected = "Yes"}
            };
    };
}
