using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using Intuit.Ipp.OAuth2PlatformClient;
using System.Reflection.Emit;
using MJC.config;
using static System.Windows.Forms.AxHost;
using System;
using MJC.forms.vendor;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace MJC.model
{
    public struct VendorCost
    {
        public int id { get; set; }
        public int skuId { get; set; }
        public int vendorId { get; set; }
        public string vendor { get; set; }
        public string manufacturer { get; set; }
        public string memo { get; set; }
        public int packageQuantity { get; set; }
        public decimal cost { get; set; }
        public  decimal  core { get; set; }

        public VendorCost(int _id, int _skuId, int _vendorId, string _vendor, string _manuf, string _memo, int _pkgQty, decimal _cost, decimal _core)
        {
            id = _id;
            skuId = _skuId;
            vendorId = _vendorId;
            vendor = _vendor;
            manufacturer = _manuf;
            memo = _memo;
            packageQuantity = _pkgQty;
            cost = _core;
            core = _core;
        }
    }

    public class VendorCostsModel : DbConnection
    {
        public List<VendorCost> VendorCostList { get; set; }

        public bool LoadVendorCosts()
        {
            VendorCostList = new List<VendorCost>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataAdapter sda;
                    DataSet ds;

                    command.CommandText = @"SELECT * FROM SKUVendorCosts";

                    command.ExecuteNonQuery();
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int id = int.Parse(row["id"].ToString());
                        int skuId = int.Parse(row["skuId"].ToString());
                        int vendorId = int.Parse(row["vendorId"].ToString());
                        string vendor = row["vendor"].ToString();
                        string manuf = row["manufacturer"].ToString();
                        string memo = row["memo"].ToString();
                        int packageQty = int.Parse(row["packageQuantity"].ToString());
                        decimal cost = decimal.Parse(row["cost"].ToString());
                        decimal core = decimal.Parse(row["core"].ToString());

                        VendorCostList.Add(new VendorCost
                        {
                            id = id,
                            skuId = skuId,
                            vendorId = vendorId,
                            vendor = vendor,
                            manufacturer = manuf,
                            memo = memo,
                            packageQuantity = packageQty,
                            cost = cost,
                            core = core
                        });
                    }
                }
            }
            return true;
        }

        public bool AddVendorCost(int skuId, int vendorId, string vendorName, string manuf, string memo, int pkgQty, decimal cost, decimal core)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "INSERT INTO dbo.SKUVendorCosts (skuId, vendorId, vendor, manufacturer, memo, packageQuantity, cost, core, createdBy, updatedBy, active) VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7, @Value8, @Value9, @Value10, @Value11)";
                    command.Parameters.AddWithValue("@Value1", skuId);
                    command.Parameters.AddWithValue("@Value2", vendorId);
                    command.Parameters.AddWithValue("@Value3", vendorName);
                    command.Parameters.AddWithValue("@Value4", manuf);
                    command.Parameters.AddWithValue("@Value5", memo);
                    command.Parameters.AddWithValue("@Value6", pkgQty);
                    command.Parameters.AddWithValue("@Value7", cost);
                    command.Parameters.AddWithValue("@Value8", core);
                    command.Parameters.AddWithValue("@Value9", 1);
                    command.Parameters.AddWithValue("@Value10", 1); 
                    command.Parameters.AddWithValue("@Value11", 1);

                    command.ExecuteNonQuery();

                    MessageBox.Show("New Vendor Cost is added successfully.");
                }
                return true;
            }
        }

        public bool UpdateVendorCost(int skuId, int vendorId, string vendorName, string manuf, string memo, int pkgQty, decimal cost, decimal core, int vendorCostId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE dbo.SKUVendorCosts SET skuId = @Value1, vendorId = @Value2, vendor = @Value3, manufacturer = @Value4, memo = @Value5, packageQuantity = @Value6, cost = @Value7, core = @Value8  WHERE id = @Value9";
                    command.Parameters.AddWithValue("@Value1", skuId);
                    command.Parameters.AddWithValue("@Value2", vendorId);
                    command.Parameters.AddWithValue("@Value3", vendorName);
                    command.Parameters.AddWithValue("@Value4", manuf);
                    command.Parameters.AddWithValue("@Value5", memo);
                    command.Parameters.AddWithValue("@Value6", pkgQty);
                    command.Parameters.AddWithValue("@Value7", cost);
                    command.Parameters.AddWithValue("@Value8", core);
                    command.Parameters.AddWithValue("@Value9", vendorCostId);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The VendorCost updated successfully.");
                }

                return true;
            }
        }

        public bool DeleteVendorCost(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "DELETE FROM dbo.SKUVendorCosts WHERE id = @Value1";
                    command.Parameters.AddWithValue("@Value1", id);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The VendorCost was deleted.");
                }

                return true;
            }
        }
    }
}
    