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
using System.Security.Cryptography;

namespace MJC.model
{
    public struct SKUSerialLot
    {
        public int id { get; set; }
        public int skuId { get; set; }
        public string serialNumber { get; set; }
        public string lotNumber { get; set; }
        public DateTime? dateReceived { get; set; }
        public decimal? cost { get; set; }
        public string invoiceId { get; set; }
        public string purchaseFrom { get; set; }

        public SKUSerialLot(int _id, int _skuId, string _serialNumber, string _lotNumber, DateTime? _dateReceived, decimal? _cost, string _invoiceId, string _purchaseFrom)
        {
            id = _id;
            skuId = _skuId;
            serialNumber = _serialNumber;
            lotNumber = _lotNumber;
            dateReceived = _dateReceived;
            cost = _cost;
            invoiceId = _invoiceId;
            purchaseFrom = _purchaseFrom;
        }
    }

    public class SKUSerialLotsModel : DbConnection
    {
        public List<SKUSerialLot> SKUSerialLotsList { get; set; }

        public bool LoadSKUSerialLot()
        {
            SKUSerialLotsList = new List<SKUSerialLot>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataAdapter sda;
                    DataSet ds;

                    command.CommandText = @"SELECT * FROM SKUSerialLots";

                    command.ExecuteNonQuery();
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {

                        int id = int.Parse(row["id"].ToString());
                        int skuId = int.Parse(row["skuId"].ToString());
                        string serialNumber = row["serialNumber"].ToString();
                        string lotNumber = row["lotNumber"].ToString();
                        DateTime? dateReceived = null;
                        if (!row.IsNull("dateReceived"))
                            dateReceived = row.Field<DateTime>("dateReceived");
                        decimal? cost = null;
                        if (!row.IsNull("cost"))
                            cost = decimal.Parse(row["cost"].ToString());
                        string invoiceId = row["invoiceId"].ToString();
                        string purchaseFrom = row["purchasedFrom"].ToString();

                        SKUSerialLotsList.Add(new SKUSerialLot
                        {
                            id = id,
                            skuId = skuId,
                            serialNumber = serialNumber,
                            lotNumber = lotNumber,
                            dateReceived = dateReceived,
                            cost = cost,
                            invoiceId = invoiceId,
                            purchaseFrom = purchaseFrom
                        });
                    }
                }
            }
            return true;
        }

        public bool AddSKUSerialLot(int skuId, string serialNumber, string lotNumber, DateTime dateReceived, decimal? cost, string invoiceId, string purchaseFrom)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "INSERT INTO dbo.SKUSerialLots (skuId, serialNumber, lotNumber, dateReceived, cost, invoiceId, purchasedFrom, createdBy, updatedBy, active) VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7, @Value8, @Value9, @Value10)";
                    command.Parameters.AddWithValue("@Value1", skuId);
                    command.Parameters.AddWithValue("@Value2", serialNumber);
                    command.Parameters.AddWithValue("@Value3", lotNumber);
                    command.Parameters.AddWithValue("@Value4", dateReceived);
                    if(cost != null)
                        command.Parameters.AddWithValue("@Value5", cost);
                    else command.Parameters.AddWithValue("@Value5", DBNull.Value);
                    command.Parameters.AddWithValue("@Value6", invoiceId);
                    command.Parameters.AddWithValue("@Value7", purchaseFrom);
                    command.Parameters.AddWithValue("@Value8", 1);
                    command.Parameters.AddWithValue("@Value9", 1);
                    command.Parameters.AddWithValue("@Value10", 1);

                    command.ExecuteNonQuery();

                    MessageBox.Show("New SKU SerialLots is added successfully.");
                }
                return true;
            }
        }

        public bool UpdateSKUSerialLot(int skuId, string serialNumber, string lotNumber, DateTime dateReceived, decimal? cost, string invoiceId, string purchaseFrom, int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE dbo.SKUSerialLots SET skuId = @Value1, serialNumber = @Value2, lotNumber = @Value3, dateReceived = @Value4, cost = @Value5, invoiceId = @Value6, purchasedFrom = @Value7, createdAt = @Value8, updatedAt = @Value9 WHERE id = @Value10";
                    command.Parameters.AddWithValue("@Value1", skuId);
                    command.Parameters.AddWithValue("@Value2", serialNumber);
                    command.Parameters.AddWithValue("@Value3", lotNumber);
                    command.Parameters.AddWithValue("@Value4", dateReceived);
                    if (cost != null)
                        command.Parameters.AddWithValue("@Value5", cost);
                    else command.Parameters.AddWithValue("@Value5", DBNull.Value);
                    command.Parameters.AddWithValue("@Value6", invoiceId);
                    command.Parameters.AddWithValue("@Value7", purchaseFrom);
                    command.Parameters.AddWithValue("@Value8", 1);
                    command.Parameters.AddWithValue("@Value9", 1);
                    command.Parameters.AddWithValue("@Value10", id);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The SKUSerialLots updated successfully.");
                }

                return true;
            }
        }

        public bool DeleteSKUSerialLots(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "DELETE FROM dbo.SKUSerialLots WHERE id = @Value1";
                    command.Parameters.AddWithValue("@Value1", id);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The SKUSerialLots was deleted.");
                }

                return true;
            }
        }
    }
}
