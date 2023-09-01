using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MJC.config;

namespace MJC.model
{
    public struct SKUQtyDiscount
    {
        public int id { get; set; }
        public int skuId { get; set; }
        public int? fromQty { get; set; }
        public int? toQty { get; set; }
        public int? discountType { get; set; }
        public int? priceTier { get; set; }
        public decimal? discount0 { get; set; }
        public decimal? discount1 { get; set; }

        public SKUQtyDiscount(int _id, int _skuId, int? _fromQty, int? _toQty, int? _discountType, int? _priceTier, decimal? _discount0, decimal? _discount1)
        {
            id = _id;
            skuId = _skuId;
            fromQty = _fromQty;
            toQty = _toQty;
            discountType = _discountType;
            priceTier = _priceTier;
            discount0 = _discount0;
            discount1 = _discount1;
        }
    }

    public class SKUQtyDiscountModel : DbConnection
    {
        public List<SKUQtyDiscount> SkuQtyDiscountList { get; set; }

        public bool LoadSKUQtyDiscount()
        {
            SkuQtyDiscountList = new List<SKUQtyDiscount>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataAdapter sda;
                    DataSet ds;

                    command.CommandText = @"SELECT * FROM SKUQuantityDiscounts";

                    command.ExecuteNonQuery();
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int id = int.Parse(row["id"].ToString());
                        int skuId = int.Parse(row["skuId"].ToString());
                        int? fromQty = null;
                        if (!row.IsNull("fromQty"))
                            fromQty = int.Parse(row["fromQty"].ToString());
                        int? toQty = null;
                        if (!row.IsNull("toQty"))
                            toQty = int.Parse(row["toQty"].ToString());
                        int? discountType = null;
                        if (!row.IsNull("discountType"))
                            discountType = int.Parse(row["discountType"].ToString());
                        int? priceTier = null;
                        if (!row.IsNull("priceTier"))
                            priceTier = int.Parse(row["priceTier"].ToString());
                        decimal? discount0 = null;
                        if (!row.IsNull("discount0"))
                            discount0 = decimal.Parse(row["discount0"].ToString());
                        decimal? discount1 = null;
                        if (!row.IsNull("discount1"))
                            discount1 = decimal.Parse(row["discount1"].ToString());

                        SkuQtyDiscountList.Add(new SKUQtyDiscount
                        {
                            id = id,
                            skuId = skuId,
                            fromQty = fromQty,
                            toQty = toQty,
                            discountType = discountType,
                            priceTier = priceTier,
                            discount0 = discount0,
                            discount1 = discount1,
                        });
                    }
                }
            }
            return true;
        }


        public bool AddSkuQtyDiscount(int skuId, int? fromQty, int? toQty, int? priceTier, int? discountType, decimal? discount0, decimal? discount1)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "INSERT INTO dbo.SKUQuantityDiscounts (skuId, fromQty, toQty, priceTier, discountType, discount0, discount1, createdBy, updatedBy, active) VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7, @Value8, @Value9, @Value10)";
                    command.Parameters.AddWithValue("@Value1", skuId);
                    if (fromQty != null)
                        command.Parameters.AddWithValue("@Value2", fromQty);
                    else command.Parameters.AddWithValue("@Value2", DBNull.Value);
                    if (toQty != null)
                        command.Parameters.AddWithValue("@Value3", toQty);
                    else command.Parameters.AddWithValue("@Value3", DBNull.Value);
                    if (priceTier != null)
                        command.Parameters.AddWithValue("@Value4", priceTier);
                    else command.Parameters.AddWithValue("@Value4", DBNull.Value);
                    if (discountType != null)
                        command.Parameters.AddWithValue("@Value5", discountType);
                    else command.Parameters.AddWithValue("@Value5", DBNull.Value);
                    if (discount0 != null)
                        command.Parameters.AddWithValue("@Value6", discount0);
                    else command.Parameters.AddWithValue("@Value6", DBNull.Value);
                    if (discount1 != null)
                        command.Parameters.AddWithValue("@Value7", discount1);
                    else command.Parameters.AddWithValue("@Value7", DBNull.Value);
                    command.Parameters.AddWithValue("@Value8", 1);
                    command.Parameters.AddWithValue("@Value9", 1);
                    command.Parameters.AddWithValue("@Value10", 1);

                    command.ExecuteNonQuery();

                    MessageBox.Show("New SKU Qty Discount is added successfully.");
                }
                return true;
            }
        }

        public bool UpdateSkuQtyDiscount(int skuId, int? fromQty, int? toQty, int? priceTier, int? discountType, decimal? discount0, decimal? discount1, int skuQtyDiscountId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE dbo.SKUQuantityDiscounts SET skuId = @Value1, fromQty = @Value2, toQty = @Value3, priceTier = @Value4, discountType = @Value5, discount0 = @Value6, discount1 = @Value7, createdAt = @Value8, updatedAt = @Value9 WHERE id = @Value10";
                    command.Parameters.AddWithValue("@Value1", skuId);
                    if (fromQty != null)
                        command.Parameters.AddWithValue("@Value2", fromQty);
                    else command.Parameters.AddWithValue("@Value2", DBNull.Value);
                    if (toQty != null)
                        command.Parameters.AddWithValue("@Value3", toQty);
                    else command.Parameters.AddWithValue("@Value3", DBNull.Value);
                    if (priceTier != null)
                        command.Parameters.AddWithValue("@Value4", priceTier);
                    else command.Parameters.AddWithValue("@Value4", DBNull.Value);
                    if (discountType != null)
                        command.Parameters.AddWithValue("@Value5", discountType);
                    else command.Parameters.AddWithValue("@Value5", DBNull.Value);
                    if (discount0 != null)
                        command.Parameters.AddWithValue("@Value6", discount0);
                    else command.Parameters.AddWithValue("@Value6", DBNull.Value);
                    if (discount1 != null)
                        command.Parameters.AddWithValue("@Value7", discount1);
                    else command.Parameters.AddWithValue("@Value7", DBNull.Value);
                    command.Parameters.AddWithValue("@Value8", 1);
                    command.Parameters.AddWithValue("@Value9", 1);
                    command.Parameters.AddWithValue("@Value10", skuQtyDiscountId);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The SKU Qty Discount updated successfully.");
                }

                return true;
            }
        }

        public bool DeleteSKUCostQty(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "DELETE FROM dbo.SKUQuantityDiscounts WHERE id = @Value1";
                    command.Parameters.AddWithValue("@Value1", id);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The SKU Qty Discount was deleted.");
                }

                return true;
            }
        }
    }
}
