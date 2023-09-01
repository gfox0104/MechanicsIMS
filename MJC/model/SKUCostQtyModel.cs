using MJC.config;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using static System.Formats.Asn1.AsnWriter;
using Antlr4.Runtime.Tree;

namespace MJC.model
{
    public struct SKUCostQty
    {
        public int id { get; set; }
        public int skuId { get; set; }
        public DateTime? costDate { get; set; }
        public string method { get; set; }
        public int qty { get; set; }
        public decimal? cost { get; set; }
        public decimal? core { get; set; }

        public SKUCostQty(int _id, int _skuId, DateTime? _costDate, string _method, int _qty, decimal? _cost, decimal? _core)
        {
            id = _id;
            skuId = _skuId;
            costDate = _costDate;
            method = _method;
            qty = _qty;
            cost = _core;
            core = _core;
        }
    }

    public class SKUCostQtyModel : DbConnection
    {
        public List<SKUCostQty> SkuCostQtyList { get; set; }

        public bool LoadSKUCostQty()
        {
            SkuCostQtyList = new List<SKUCostQty>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataAdapter sda;
                    DataSet ds;

                    command.CommandText = @"SELECT * FROM SKUCostQtys";

                    command.ExecuteNonQuery();
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int id = int.Parse(row["id"].ToString());
                        int skuId = int.Parse(row["skuId"].ToString());
                        DateTime? costDate = null;
                        if (!row.IsNull("costDate"))
                            costDate = row.Field<DateTime>("costDate");
                        string method = row["method"].ToString();
                        int qty = int.Parse(row["qty"].ToString());
                        decimal? cost = null;
                        if (!row.IsNull("cost"))
                            cost = decimal.Parse(row["cost"].ToString());
                        decimal? core = null;
                        if(!row.IsNull("core")) 
                            core = decimal.Parse(row["core"].ToString());

                        SkuCostQtyList.Add(new SKUCostQty
                        {
                            id = id,
                            skuId = skuId,
                            costDate = costDate,
                            method = method,
                            qty = qty,
                            cost = cost,
                            core = core
                        });
                    }
                }
            }
            return true;
        }

        public bool AddSKUCostQty(int skuId, DateTime costDate, string method, int? qty, decimal? cost, decimal? core)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "INSERT INTO dbo.SKUCostQtys (skuId, costDate, method, qty, cost, core, createdBy, updatedBy, active) VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7, @Value8, @Value9)";
                    command.Parameters.AddWithValue("@Value1", skuId);
                    command.Parameters.AddWithValue("@Value2", costDate);
                    command.Parameters.AddWithValue("@Value3", method);
                    if(qty != null)
                        command.Parameters.AddWithValue("@Value4", qty);
                    else command.Parameters.AddWithValue("@Value4", DBNull.Value);
                    if(cost != null)
                        command.Parameters.AddWithValue("@Value5", cost);
                    else command.Parameters.AddWithValue("@Value5", DBNull.Value);
                    if(core != null)
                        command.Parameters.AddWithValue("@Value6", core);
                    else command.Parameters.AddWithValue("@Value6", DBNull.Value);
                    command.Parameters.AddWithValue("@Value7", 1);
                    command.Parameters.AddWithValue("@Value8", 1);
                    command.Parameters.AddWithValue("@Value9", 1);

                    command.ExecuteNonQuery();

                    MessageBox.Show("New SKU Cost Qty is added successfully.");
                }
                return true;
            }
        }

        public bool UpdateSKUCostQty(int skuId, DateTime costDate, string method, int? qty, decimal? cost, decimal? core, int skuCostQtyId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE dbo.SKUCostQtys SET skuId = @Value1, costDate = @Value2, method = @Value3, qty = @Value4, cost = @Value5, core = @Value6, createdAt = @Value7, updatedAt = @Value8 WHERE id = @Value9";
                    command.Parameters.AddWithValue("@Value1", skuId);
                    command.Parameters.AddWithValue("@Value2", costDate);
                    command.Parameters.AddWithValue("@Value3", method);
                    if (qty != null)
                        command.Parameters.AddWithValue("@Value4", qty);
                    else command.Parameters.AddWithValue("@Value4", DBNull.Value);
                    if (cost != null)
                        command.Parameters.AddWithValue("@Value5", cost);
                    else command.Parameters.AddWithValue("@Value5", DBNull.Value);
                    if (core != null)
                        command.Parameters.AddWithValue("@Value6", core);
                    else command.Parameters.AddWithValue("@Value6", DBNull.Value);
                    command.Parameters.AddWithValue("@Value7", 1);
                    command.Parameters.AddWithValue("@Value8", 1);
                    command.Parameters.AddWithValue("@Value9", skuCostQtyId);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The SKU Cost Qty updated successfully.");
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

                    command.CommandText = "DELETE FROM dbo.SKUCostQtys WHERE id = @Value1";
                    command.Parameters.AddWithValue("@Value1", id);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The SKUCostQty was deleted.");
                }

                return true;
            }
        }
    }
}
