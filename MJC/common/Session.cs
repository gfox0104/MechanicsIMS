using MJC.forms.taxcode;
using MJC.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJC.common
{
    internal class Session
    {
        // This class allows all forms to share models and other data
        // without having to pass them around as parameters.
        // 
        // We can also add and update locally without querying the database continually
        // instead we will update data from the database each time we are entering a
        // function but not redrawing the entire form.

        public static bool LoggedIn {get;set;}

        public static SystemSettingsModel SettingsModelObj = new SystemSettingsModel();
        public static SalesTaxCodeModel SalesTaxModelObj = new SalesTaxCodeModel();
        public static CustomersModel CustomersModelObj = new CustomersModel();
        public static SKUModel SKUModelObj = new SKUModel();
        public static OrderItemsModel OrderItemModelObj = new OrderItemsModel();
        public static InvoicesModel InvoicesModelObj = new InvoicesModel();
        public static PaymentDetailModel paymentDetailModelObj = new PaymentDetailModel();
        public static AccountingModel accountingModelObj = new AccountingModel();
        public static VendorsModel VendorsModelObj = new VendorsModel();
        public static VendorCostsModel VendorCostModelObj = new VendorCostsModel();
        public static PriceTiersModel PriceTiersModelObj = new PriceTiersModel();
        public static CategoriesModel CategoriesModelObj = new CategoriesModel();
        public static SKUPricesModel SKUPricesModelObj = new SKUPricesModel();
        public static SalesTaxCodeModel SalesTaxCodeModelObj = new SalesTaxCodeModel();
        public static SubAssemblyModel SubAssemblyModelObj = new SubAssemblyModel();
        public static SKUSerialLotsModel SKUSerialLotsModelObj = new SKUSerialLotsModel();
        public static SKUQtyDiscountModel SKUQtyDiscountModelObj = new SKUQtyDiscountModel();
        public static SKUCostQtyModel SKUCostModelObj = new SKUCostQtyModel();
        public static SKUCostQtyModel SKUCostQtyModelObj = new SKUCostQtyModel();
        public static SalesCostCodeModel SalesCostCodesModelObj = new SalesCostCodeModel();
        public static PaymentHistoryModel PymtHistoryModelObj = new PaymentHistoryModel();
        public static CustomersModel CustomerModelObj = new CustomersModel();
        public static PaymentDetailModel PymtDetailModelObj = new PaymentDetailModel();
        public static OrderModel OrderModelObj = new OrderModel();
    }

}
