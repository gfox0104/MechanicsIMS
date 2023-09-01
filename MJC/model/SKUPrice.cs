using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJC.model
{
    using Intuit.Ipp.Data;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;

    namespace MJC.model
    {
        public class SKUPrice
        {
            public int skuPriceId { get; set; }
            public int priceTierId { get; set; }
            public string priceTier { get; set; }
            public double inventoryValue { get; set; }

            private double _margin;
            private double _price;

            public double margin
            {
                get => _margin;
                set
                {
                    if (_margin != value)
                    {
                        _margin = value;
                        //_price = 1231231;
                        OnPropertyChanged();
                    }
                }
            }

            public double profitMargin { get; set; }
            public double price
            {
                get => _price;
                set
                {
                    if (_price != value)
                    {
                        _price = value;

                        OnPropertyChanged();
                    }
                }
            }
            public int? categoryPriceTierId { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

}
