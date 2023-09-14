using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MJC.model
{
    public class OrderItem : IEquatable<OrderItem>, IComparable<OrderItem>
    {
        private int _id;
        private int _orderId;
        private int _skuId;
        private string _sku;
        private int? _quantity;
        private string _description;
        private bool? _tax;
        private double? _unitPrice;
        private double? _lineTotal;
        private string _sc;
        private string _qboSkuId;
        private string _qboItemId;
        private int? _priceTier;
        private string _priceTierCode;
        public string message;
        public bool? _billAsLabor;

        public int Id 
        {
            get => _id;
            set {
                _id = value;
                OnPropertyChanged();
            }
        }

        public int OrderId
        {
            get => _orderId;
            set
            {
                _orderId = value;
                OnPropertyChanged();
            }
        }
        public int SkuId
        {
            get => _skuId;
            set
            {
                _skuId = value;
                OnPropertyChanged();
            }
        }
        public string QboItemId
        {
            get => _qboItemId;
            set
            {
                _qboItemId = value;
                OnPropertyChanged();
            }
        }
        public string QboSkuId
        {
            get => _qboSkuId;
            set
            {
                _qboSkuId = value;
                OnPropertyChanged();
            }
        }
        public string Sku
        {
            get => _sku;
            set
            {
                _sku = value;
                OnPropertyChanged();
            }
        }

        public int? Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
            }
        }
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public bool? Tax
        {
            get => _tax;
            set
            {
                _tax = value;
                OnPropertyChanged();
            }
        }

        public string PriceTierCode
        {
            get => _priceTierCode;
            set
            {
                _priceTierCode = value;
                OnPropertyChanged();
            }
        }

        public double? UnitPrice
        {
            get => _unitPrice;
            set
            {
                _unitPrice = value;
                OnPropertyChanged();
            }
        }
        public double? LineTotal
        {
            get => _lineTotal;
            set
            {
                _lineTotal = value;
                OnPropertyChanged();
            }
        }
        public string SC
        {
            get => _sc;
            set
            {
                _sc = value;
                OnPropertyChanged();
            }
        }


        public int? PriceTier
        {
            get => _priceTier;
            set
            {
                _priceTier = value;
                OnPropertyChanged();
            }
        }

        public bool? BillAsLabor
        {
            get => _billAsLabor;
            set
            {
                _billAsLabor = value;
                OnPropertyChanged();
            }
        }

        //public string _TaxString { get { return "Yes"; } } 

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            OrderItem objAsPart = obj as OrderItem;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }
        public int SortByNameAscending(string name1, string name2)
        {

            return name1.CompareTo(name2);
        }

        // Default comparer for Part type.
        public int CompareTo(OrderItem comparePart)
        {
            // A null value means that this object is greater.
            if (comparePart == null)
                return 1;

            else
                return this.SkuId.CompareTo(comparePart.SkuId);
        }
        public override int GetHashCode()
        {
            return SkuId;
        }
        public bool Equals(OrderItem other)
        {
            if (other == null) return false;
            return (this.Sku.Equals(other.Sku));
        }
    }
}
