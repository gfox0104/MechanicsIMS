using MJC.config;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Security.Cryptography;

namespace MJC.model
{
    public struct SubAssembly
    {
        public int id { get; set; }
        public string targetSKU { get; set; }
        public string subAssemblySKU { get; set; }
        public string categoryName { get; set; }
        public string description { get; set; }
        public int? qty { get; set; }
        public int targetSKUId { get; set; }
        public int subAssemblySkuId { get; set; }
        public int categoryId { get; set; }
        public bool status { get; set; }
        public bool invoicePrint { get; set; }

        public SubAssembly(int _id, string _targetSKU, int _targetSKUId, string _subAssemblySKU, int _categoryId, string _categoryName, bool _status, bool _invoicePrint, string _description, int? _qty, int _subAssemblySkuId)
        {
            id = _id;
            targetSKU = _targetSKU;
            subAssemblySKU = _subAssemblySKU;
            categoryName = _categoryName;
            status = _status;
            invoicePrint = _invoicePrint;
            description = _description;
            qty = _qty;
            targetSKUId = _targetSKUId;
            subAssemblySkuId = _subAssemblySkuId;
            categoryId = _categoryId;
        }
    }

    public class SubAssemblyModel : DbConnection
    {
        public List<SubAssembly> SubAssemblyList { get; set; }

        public List<SubAssembly> LoadSubAssemblies()
        {
            //SubAssemblyList = new List<SubAssembly>();
            List<SubAssembly> SubAssemblyList = new List<SubAssembly>();

            using (var connection = GetConnection())
            {
                connection.Open();
                SqlDataAdapter sda;
                DataSet ds;

                using (var command = new SqlCommand())
                {
                    command.CommandText = @"SELECT SKU.sku AS targetSKU, tblSubAssemblies.* FROM SKU INNER JOIN (SELECT SKU.sku, tblSKU.* FROM SKU INNER JOIN (SELECT Categories.categoryName, tblSubAssemblies.* FROM Categories INNER JOIN (SELECT * FROM SubAssemblies) AS tblSubAssemblies ON tblSubAssemblies.categoryId = categories.id) AS tblSKU ON tblSKU.subAssemblySkuId = SKU.id) AS tblSubAssemblies ON tblSubAssemblies.targetSkuId = SKU.id";

                    command.Connection = connection;
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int id = int.Parse(row["id"].ToString());
                        int targetSkuId = int.Parse(row["targetSkuId"].ToString());
                        string targetSKU = row["targetSKU"].ToString();
                        string sku = row["sku"].ToString();
                        int subAssemblySkuId = int.Parse(row["subAssemblySkuId"].ToString());
                        int categoryId = int.Parse(row["categoryId"].ToString());
                        string categoryName = row["categoryName"].ToString();
                        bool status = false;
                        if (!row.IsNull("status"))
                            status = row.Field<Boolean>("status");
                        bool invoicePrint = false;
                        if (!row.IsNull("invoicePrint"))
                            invoicePrint = row.Field<Boolean>("invoicePrint");
                        string description = row["description"].ToString();
                        int? qty = null;
                        if (!row.IsNull("quantity"))
                            qty = int.Parse(row["quantity"].ToString());

                        SubAssemblyList.Add(new SubAssembly
                        {
                            id = id,
                            targetSKU = targetSKU,
                            targetSKUId = targetSkuId,
                            subAssemblySKU = sku,
                            subAssemblySkuId = subAssemblySkuId,
                            categoryId = categoryId,
                            categoryName = categoryName,
                            status = status,
                            invoicePrint = invoicePrint,
                            description = description,
                            qty = qty,

                        });
                    }

                }
            }

            return SubAssemblyList;
        }

        public bool AddSubAssembly(int? targetSkuId, int? subAssemblySkuId, int? categoryId, bool? status, bool? invoicePrint, string description, int? qty)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "INSERT INTO dbo.SubAssemblies (targetSkuId, subAssemblySkuId, categoryId, status, invoicePrint, description, quantity, createdBy, updatedBy, active) VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7, @Value8, @Value9, @Value10)";
                    if (targetSkuId != null)
                        command.Parameters.AddWithValue("@Value1", targetSkuId);
                    else command.Parameters.AddWithValue("@Value1", DBNull.Value);
                    if (subAssemblySkuId != null)
                        command.Parameters.AddWithValue("@Value2", subAssemblySkuId);
                    else command.Parameters.AddWithValue("@Value2", DBNull.Value);
                    if (categoryId != null)
                        command.Parameters.AddWithValue("@Value3", categoryId);
                    else command.Parameters.AddWithValue("@Value3", DBNull.Value);
                    if (status != null)
                        command.Parameters.AddWithValue("@Value4", Convert.ToInt32(status));
                    else command.Parameters.AddWithValue("@Value4", DBNull.Value);
                    if (invoicePrint != null)
                        command.Parameters.AddWithValue("@Value5", Convert.ToInt32(invoicePrint));
                    else command.Parameters.AddWithValue("@Value5", DBNull.Value);
                    command.Parameters.AddWithValue("@Value6", description);
                    if (qty != null)
                        command.Parameters.AddWithValue("@Value7", qty);
                    else command.Parameters.AddWithValue("@Value7", DBNull.Value);

                    command.Parameters.AddWithValue("@Value8", 1);
                    command.Parameters.AddWithValue("@Value9", 1); 
                    command.Parameters.AddWithValue("@Value10", 1);

                    command.ExecuteNonQuery();

                    MessageBox.Show("New Sub Assembly is added successfully.");
                }
                return true;
            }
        }

        public bool UpdateSubAssembly(int? targetSkuId, int? subAssemblySkuId, int? categoryId, bool? status, bool? invoicePrint, string description, int? qty, int subAssemblyId)
       {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE dbo.SubAssemblies SET targetSkuId = @Value1, subAssemblySkuId = @Value2, categoryId = @Value3, status = @Value4, invoicePrint = @Value5, description = @Value6, quantity = @Value7, createdAt = @Value8, updatedAt = @Value9 WHERE id = @Value10";
                    
                    if (targetSkuId != null)
                        command.Parameters.AddWithValue("@Value1", targetSkuId);
                    else command.Parameters.AddWithValue("@Value1", DBNull.Value);
                    if (subAssemblySkuId != null)
                        command.Parameters.AddWithValue("@Value2", subAssemblySkuId);
                    else command.Parameters.AddWithValue("@Value2", DBNull.Value);
                    if (categoryId != null)
                        command.Parameters.AddWithValue("@Value3", categoryId);
                    else command.Parameters.AddWithValue("@Value3", DBNull.Value);
                    if (status != null)
                        command.Parameters.AddWithValue("@Value4", status);
                    else command.Parameters.AddWithValue("@Value4", DBNull.Value);
                    if (invoicePrint != null)
                        command.Parameters.AddWithValue("@Value5", invoicePrint);
                    else command.Parameters.AddWithValue("@Value5", DBNull.Value);
                    command.Parameters.AddWithValue("@Value6", description);
                    if (qty != null)
                        command.Parameters.AddWithValue("@Value7", qty);
                    else command.Parameters.AddWithValue("@Value7", DBNull.Value);

                    command.Parameters.AddWithValue("@Value8", 1);
                    command.Parameters.AddWithValue("@Value9", 1);
                    command.Parameters.AddWithValue("@Value10", subAssemblyId);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The Sub Assembly updated successfully.");
                }

                return true;
            }
        }

        public bool DeleteSubAssembly(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "DELETE FROM dbo.SubAssemblies WHERE id = @Value1";
                    command.Parameters.AddWithValue("@Value1", id);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The Sub Assebmly was deleted.");
                }

                return true;
            }
        }

        public SubAssembly GetSubAssemblyId (int subAssebmlyId) {

            SubAssembly subAssembly = new SubAssembly();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataAdapter sda;
                    DataSet ds;

                    command.CommandText = @"SELECT SKU.sku AS targetSKU, tblSubAssemblies.* FROM SKU INNER JOIN (SELECT SKU.sku, tblSKU.* FROM SKU INNER JOIN (SELECT Categories.categoryName, tblSubAssemblies.* FROM Categories INNER JOIN (SELECT * FROM SubAssemblies WHERE id = @Value1) AS tblSubAssemblies ON tblSubAssemblies.categoryId = categories.id) AS tblSKU ON tblSKU.subAssemblySkuId = SKU.id) AS tblSubAssemblies ON tblSubAssemblies.targetSkuId = SKU.id";

                    command.Parameters.AddWithValue("@Value1", subAssebmlyId);

                    command.ExecuteNonQuery();
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    DataRow row = ds.Tables[0].Rows[0];
                    int id = int.Parse(row["id"].ToString());
                    int targetSkuId = int.Parse(row["targetSkuId"].ToString());
                    string targetSKU = row["targetSKU"].ToString();
                    string sku = row["sku"].ToString();
                    int subAssemblySkuId = int.Parse(row["subAssemblySkuId"].ToString());
                    int categoryId = int.Parse(row["categoryId"].ToString());
                    string categoryName = row["categoryName"].ToString();
                    bool status = false;
                    if (!row.IsNull("status"))
                        status = row.Field<Boolean>("status");
                    bool invoicePrint = false;
                    if (!row.IsNull("invoicePrint"))
                        invoicePrint = row.Field<Boolean>("invoicePrint");
                    string description = row["description"].ToString();
                    int? qty = null;
                    if (!row.IsNull("quantity"))
                        qty = int.Parse(row["quantity"].ToString());

                    subAssembly = new SubAssembly
                    {
                        id = id,
                        targetSKU = targetSKU,
                        targetSKUId = targetSkuId,
                        subAssemblySKU = sku,
                        subAssemblySkuId = subAssemblySkuId,
                        categoryId = categoryId,
                        categoryName = categoryName,
                        status = status,
                        invoicePrint = invoicePrint,
                        description = description,
                        qty = qty,

                    };
                    return subAssembly;
                }
            }
        }

        public dynamic GetSubAssebmlyMessageById(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT id, message FROM SubAssemblies WHERE id = @Value1";
                    command.Parameters.AddWithValue("@Value1", id);

                    var reader = command.ExecuteReader();
                    if (reader.Read()) // check if there are any rows returned
                    {
                        //id = (int)reader.GetValue(0);
                        string message = reader.IsDBNull(1) ? "" : reader.GetString(1);

                        var orderItemMessage = new
                        {
                            id = id,
                            message = message,
                        };

                        return orderItemMessage;
                    }
                    return null;
                }
            }
        }

        public bool UpdateSubAssemblyMessageById(string message, int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE SubAssemblies SET message = @Value1 WHERE id = @Value2";
                    command.Parameters.AddWithValue("@Value1", message);
                    command.Parameters.AddWithValue("@Value2", id);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The Sub Assembly Message updated successfully.");

                    return true;
                }
            }
        }

        public bool UpdatePrintInvoice(int printInvoiceState, int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE SubAssemblies SET invoicePrint = @Value1 WHERE targetSkuId = @Value2";
                    command.Parameters.AddWithValue("@Value1", printInvoiceState);
                    command.Parameters.AddWithValue("@Value2", id);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The Sub Assembly PrintInvoice updated successfully.");

                    return true;
                }
            }
        }

        public bool UpdateTargetSKUCost(int skuId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE SKU SET inventoryValue = (SELECT COUNT(inventoryValue) FROM SKU INNER JOIN (SELECT subAssemblySkuId FROM SubAssemblies WHERE targetSkuId = @Value1) AS tblSKU ON tblSKU.subAssemblySkuId = SKU.id) WHERE SKU.id = @Value2";
                    command.Parameters.AddWithValue("@Value1", skuId);
                    command.Parameters.AddWithValue("@Value2", skuId);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The SKU Inventory Cost updated successfully.");

                    return true;
                }
            }
        }
    }
}
