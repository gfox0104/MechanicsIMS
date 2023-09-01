using MJC.config;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using MJC.forms.price;
using MJC.model.MJC.model;

namespace MJC.model
{
    public class QuickCalcPriceModel : DbConnection
    {
        public List<SKUPrice> calcPriceList = new List<SKUPrice>();

        public bool LoadCalcPrice(int skuId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                SqlDataAdapter sda;
                DataSet ds;

                using (var command = new SqlCommand())
                {
                    command.CommandText = @"SELECT CategoryPriceTiers.id AS categoryPriceTierId, CategoryPriceTiers.margin, tblSKU.* FROM CategoryPriceTiers RIGHT JOIN (SELECT inventoryValue, category, tblPriceTier.* FROM SKU INNER JOIN (SELECT PriceTiers.id AS priceTierId, PriceTiers.priceTierCode, PriceTiers.profitMargin, tblSKUPrice.price, tblSKUPrice.skuId FROM PriceTiers INNER JOIN (SELECT * FROM SKUPrices WHERE skuId = @Value1) AS tblSKUPrice ON tblSKUPrice.priceTierId = PriceTiers.id) AS tblPriceTier ON tblPriceTier.skuId = SKU.id) AS tblSKU ON tblSKU.category = CategoryPriceTiers.categoryId AND CategoryPriceTiers.priceTierId = tblSKU.priceTierId";
                    command.Parameters.AddWithValue("@Value1", skuId);

                    command.Connection = connection;
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int priceTierId = int.Parse(row["priceTierId"].ToString());
                        string priceTierCode = row["priceTierCode"].ToString();
                        double margin = 0;
                        if (!row.IsNull("margin"))
                            margin = double.Parse(row["margin"].ToString());
                        double profitMargin = double.Parse(row["profitMargin"].ToString());
                        double price = double.Parse(row["price"].ToString());
                        double inventoryValue = double.Parse(row["inventoryValue"].ToString());
                        int? categoryPriceTierId = null;
                        if(!row.IsNull("categoryPriceTierId"))
                            categoryPriceTierId = int.Parse(row["categoryPriceTierId"].ToString());

                        calcPriceList.Add(new SKUPrice
                        {
                            priceTierId = priceTierId,
                            priceTier = priceTierCode,
                            inventoryValue = inventoryValue,
                            margin = margin,
                            profitMargin = profitMargin,
                            price = price,
                            categoryPriceTierId = categoryPriceTierId
                        });
                    }

                }
            }

            return true;
        }

        public dynamic GetQuickCalcPriceInfo(int skuId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                SqlDataAdapter sda;
                DataSet ds;

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT Categories.categoryName, Categories.calculateAs, tblSKU.* FROM Categories INNER JOIN (SELECT * FROM SKU WHERE id = @Value1) AS tblSKU ON tblSKU.category = Categories.id;";
                    command.Parameters.AddWithValue("@Value1", skuId);

                    command.ExecuteNonQuery();
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        string categoryName = row["categoryName"].ToString();
                        int calculateAs = int.Parse(row["calculateAs"].ToString());

                        var quickCalcPriceInfo = new
                        {
                           categoryName = categoryName,
                           calculateAs = calculateAs,
                        };

                        return quickCalcPriceInfo;
                    }

                    return null;
                }
            }
        }
    }
}
