using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Antlr4.Runtime.Tree;
using MJC.config;

namespace MJC.model
{
    public struct ProcessOrderItemsList
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string Sku { get; set; }
        public string Description { get; set; }
        public DateTime? PaidDate { get; set; }
        public double? Quantity { get; set; }
        public double? PricePaid { get; set; }
        public string SC { get; set; }

        public ProcessOrderItemsList(int _id, string _invoiceNumber, string _sku, string _description, DateTime? _paidDate, double? _quantity, double? _pricePaid, string _SC)
        {
            Id = _id;
            InvoiceNumber = _invoiceNumber;
            Sku = _sku;
            Description = _description;
            PaidDate = _paidDate;
            Quantity = _quantity;
            PricePaid = _pricePaid;
            SC = _SC;
        }
    }

    public struct Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? InvoiceDate { get; private set; }
        public double? InvoiceBalance { get; set; }
        public string HeldFor { get; set; }
        public string ProcBy { get; set; }


        public Order(int _id, int _customerId, string _name, string _desc, DateTime? _invoiceDate, double? _invoiceBalance, string _heldFor, string _procBy)
        {
            Id = _id;
            CustomerId = _customerId;
            Name = _name;
            Description = _desc;
            InvoiceDate = _invoiceDate;
            InvoiceBalance = _invoiceBalance;
            HeldFor = _heldFor;
            ProcBy = _procBy;
        }
    }

    public class OrderItemsModel : DbConnection
    {
        public List<OrderItem> OIList { get; private set; }

        public List<ProcessOrderItemsList> ProcessOIList { get; private set; }

        public List<OrderItem> GetOrderItemsListByCustomerId(int customerId, int orderId, string sort = "")
        {
            List<OrderItem> OrderItemData = new List<OrderItem>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataAdapter sda;
                    DataSet ds;

                    if (customerId != 0 && orderId != 0)
                    {
                        command.CommandText = @"SELECT OrderItems.*, SalesCostCodes.scCode FROM OrderItems INNER JOIN
                                            (SELECT id FROM Orders WHERE id = @Value1) AS tblOrder ON tblOrder.id = OrderItems.orderId 
                                            INNER JOIN SalesCostCodes On SalesCostCodes.id = OrderItems.salesCode" + sort;
                        command.Parameters.AddWithValue("@Value1", orderId);
                    }
                    else
                    {
                        command.CommandText = @"SELECT SKU.billAsLabor, tblOrderItem.* FROM SKU RIGHT JOIN (SELECT OrderItems.*, SalesCostCodes.scCode FROM OrderItems INNER JOIN
                                            (SELECT id FROM Orders WHERE customerId = @Value1) AS tblOrder ON tblOrder.id = OrderItems.orderId LEFT JOIN SalesCostCodes On SalesCostCodes.id = OrderItems.salesCode) AS tblOrderItem ON tblOrderItem.skuId = SKU.id" + sort;

                        command.Parameters.AddWithValue("@Value1", customerId);
                    }


                    command.ExecuteNonQuery();
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int orderItemId = int.Parse(row["id"].ToString());
                        int orderid = int.Parse(row["orderId"].ToString());
                        int skuId = int.Parse(row["skuId"].ToString());
                        string sku = row["sku"].ToString();
                        int? quantity = null;
                        if (!row.IsNull("quantity"))
                            quantity = int.Parse(row["quantity"].ToString());
                        string description = row["description"].ToString();
                        bool? tax = null;
                        if (!row.IsNull("tax"))
                            tax = row.Field<Boolean>("tax");
                        int? priceTier = null;
                        if (!row.IsNull("priceTier"))
                            priceTier = int.Parse(row["priceTier"].ToString());
                        double? unitPrice = null;
                        if (!row.IsNull("unitPrice"))
                            unitPrice = double.Parse(row["unitPrice"].ToString());
                        double? lineTotal = null;
                        if (!row.IsNull("lineTotal"))
                            lineTotal = double.Parse(row["lineTotal"].ToString());
                        string salesCode = row["scCode"].ToString();
                        string qboSkuId = row["qboSkuId"].ToString();
                        string qboOrderItemId = row["qboOrderItemId"].ToString();
                        bool? billAsLabor = null;
                        if (!row.IsNull("billAsLabor"))
                            billAsLabor = row.Field<Boolean>("billAsLabor");

                        OrderItemData.Add(new OrderItem
                        {
                            Id = orderItemId,
                            OrderId = orderid,
                            SkuId = skuId,
                            Sku = sku,
                            Quantity = quantity,
                            Description = description,
                            Tax = tax,
                            PriceTier = priceTier,
                            UnitPrice = unitPrice,
                            LineTotal = lineTotal,
                            SC = salesCode,
                            QboSkuId = qboSkuId,
                            QboItemId = qboOrderItemId,
                            BillAsLabor = billAsLabor
                        });
                    }
                }
            }

            return OrderItemData;
        }

        public bool LoadCustomerProfiler(string filter, int customerId)
        {
            ProcessOIList = new List<ProcessOrderItemsList>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataAdapter sda;
                    DataSet ds;

                    command.CommandText = @"SELECT * FROM OrderItems INNER JOIN (SELECT id, invoiceNumber, paidDate, totalPaid, customerId FROM Orders WHERE customerId=@Value1) AS tblOrder ON tblOrder.id = OrderItems.orderId";

                    command.Parameters.AddWithValue("@Value1", customerId);

                    if (filter != "")
                    {
                        command.CommandText = @"SELECT * FROM OrderItems INNER JOIN (SELECT id, invoiceNumber, paidDate, totalPaid, customerId FROM Orders WHERE customerId=@Value1) AS tblOrder ON tblOrder.id = OrderItems.orderId WHERE invoiceNumber LIKE @filter OR description LIKE @filter OR salesCode LIKE @filter OR sku LIKE @filter";
                        command.Parameters.Add("@filter", System.Data.SqlDbType.VarChar).Value = "%" + filter + "%";
                    }

                    command.ExecuteNonQuery();
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int id = int.Parse(row["id"].ToString());
                        string invoiceNumber = row["invoiceNumber"].ToString();
                        DateTime? paidDate = null;
                        if (!row.IsNull("paidDate"))
                            paidDate = row.Field<DateTime>("paidDate");
                        string sku = row["sku"].ToString();
                        double? quantity = null;
                        if (!row.IsNull("quantity"))
                            quantity = int.Parse(row["quantity"].ToString());
                        string description = row["description"].ToString();
                        double? totalPaid = null;
                        if (!row.IsNull("totalPaid"))
                            totalPaid = double.Parse(row["totalPaid"].ToString());
                        string salesCode = row["salesCode"].ToString();

                        ProcessOIList.Add(new ProcessOrderItemsList(id, invoiceNumber, sku, description, paidDate, quantity, totalPaid, salesCode));
                    }
                }
            }

            return true;
        }

        public bool DeleteOrder(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    DeleteOrderItem(id);
                    command.CommandText = "DELETE FROM dbo.Orders WHERE id = @Value1";
                    command.Parameters.AddWithValue("@Value1", id);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The Order was deleted.");
                }

                return true;
            }
        }

        public bool DeleteOrderItem(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "DELETE FROM dbo.OrderItems WHERE orderId = @Value1";
                    command.Parameters.AddWithValue("@Value1", id);

                    command.ExecuteNonQuery();
                }

                return true;
            }
        }

        public dynamic GetOrderItemMessageById(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT id, message FROM OrderItems WHERE id = @Value1";
                    command.Parameters.AddWithValue("@Value1", id);

                    var reader = command.ExecuteReader();
                    if (reader.Read()) // check if there are any rows returned
                    {
                        // retrieve values from the reader
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

        public bool UpdateOrderItemMessageById(string message, int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE OrderItems SET message=@Value1 WHERE id=@Value2";
                    command.Parameters.AddWithValue("@Value1", message);
                    command.Parameters.AddWithValue("@Value2", id);

                    command.ExecuteNonQuery();

                    return true;
                }
            }
        }

        public List<Order> LoadOrdersDataByStatus(int status = 1, int customerId = 0)
        {
            List<Order> OrderList = new List<Order>();

            using (var connection = GetConnection())
            {
                connection.Open();
                SqlDataAdapter sda;
                DataSet ds;

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    if (customerId > 0)
                    {
                        command.CommandText = @"SELECT id, customerId, customerName, invoiceDate, invoiceDesc, invoiceTotal, dateShipped, processedBy  FROM Orders WHERE status = @Value1 AND customerId = @Value2 ORDER BY invoiceDate DESC";
                        command.Parameters.AddWithValue("@Value1", status);
                        command.Parameters.AddWithValue("@Value2", customerId);
                    }
                    else
                    {
                        command.CommandText = @"SELECT id, customerId, customerName, invoiceDate, invoiceDesc, invoiceTotal, dateShipped, processedBy  FROM Orders WHERE status = @Value1 ORDER BY invoiceDate DESC";
                        command.Parameters.AddWithValue("@Value1", status);
                    }

                    command.ExecuteNonQuery();
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int orderId = Convert.ToInt32(row["id"]);
                        int newCustomerId = Convert.ToInt32(row["customerId"]);
                        string customerName = row["customerName"].ToString();
                        DateTime? invoiceDate = null;
                        if (!row.IsNull("invoiceDate"))
                            invoiceDate = row.Field<DateTime>("invoiceDate");
                        string desc = row["invoiceDesc"].ToString();
                        double? amount = null;
                        if (!row.IsNull("invoiceTotal"))
                            amount = double.Parse(row["invoiceTotal"].ToString());
                        string heldFor = "";
                        if (!row.IsNull("dateShipped"))
                            heldFor = "On Delivery";
                        else heldFor = "Processing";
                        string procBy = row["processedBy"].ToString();

                        OrderList.Add(
                            new Order(orderId, newCustomerId, customerName, desc, invoiceDate, amount, heldFor, procBy)
                        );
                    }
                }
            }
            return OrderList;
        }

        public int CreateOrderItem(int orderId, int skuId, decimal? qty, string description, string message, bool? tax, int? priceTier, double? unitPrice, double? lineTotal, string salesCode, string sku, int qboSkuId, string qboOrderItemId, int lineNum, int createdBy, int updatedBy)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                int orderItemId = 0;

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"INSERT INTO OrderItems(active, orderId, skuId, quantity, description, message, tax, priceTier, unitPrice, lineTotal, salesCode, sku, qboSkuId, qboOrderItemId, lineNum, createdBy, updatedBy) OUTPUT INSERTED.id VALUES(@Value1, @Value2, @Value3, @Value4, @Value5, @message, @Value6, @Value7, @Value8, @Value9, @Value10, @Value11, @Value12, @Value13, @Value14, @Value15, @Value16)";
                    command.Parameters.AddWithValue("@Value1", 1);
                    command.Parameters.AddWithValue("@Value2", orderId);
                    if(skuId != 0)
                        command.Parameters.AddWithValue("@Value3", skuId);
                    else command.Parameters.AddWithValue("@Value3", DBNull.Value);
                    if (qty != null)
                        command.Parameters.AddWithValue("@Value4", qty);
                    else command.Parameters.AddWithValue("@Value4", DBNull.Value);
                    if(!string.IsNullOrEmpty(description))
                        command.Parameters.AddWithValue("@Value5", description);
                    else command.Parameters.AddWithValue("@Value5", DBNull.Value);
                    if (tax != null)
                        command.Parameters.AddWithValue("@Value6", Convert.ToInt32(tax));
                    else command.Parameters.AddWithValue("@Value6", DBNull.Value);
                    if (priceTier != null)
                        command.Parameters.AddWithValue("@Value7", priceTier);
                    else command.Parameters.AddWithValue("@Value7", DBNull.Value);
                    if (unitPrice != null)
                        command.Parameters.AddWithValue("@Value8", unitPrice);
                    else command.Parameters.AddWithValue("@Value8", DBNull.Value);
                    if (lineTotal != null)
                        command.Parameters.AddWithValue("@Value9", lineTotal);
                    else command.Parameters.AddWithValue("@Value9", DBNull.Value);
                    command.Parameters.AddWithValue("@Value10", salesCode);
                    command.Parameters.AddWithValue("@Value11", sku);
                    if(qboSkuId != 0)
                        command.Parameters.AddWithValue("@Value12", qboSkuId);
                    else command.Parameters.AddWithValue("@Value12", DBNull.Value);
                    if (!string.IsNullOrEmpty(qboOrderItemId))
                        command.Parameters.AddWithValue("@Value13", qboOrderItemId);
                    else command.Parameters.AddWithValue("@Value13", DBNull.Value);
                    command.Parameters.AddWithValue("@Value14", lineNum);
                    command.Parameters.AddWithValue("@Value15", createdBy);
                    command.Parameters.AddWithValue("@Value16", updatedBy);
                    if (!string.IsNullOrEmpty(message))
                        command.Parameters.AddWithValue("@message", message);
                    else command.Parameters.AddWithValue("@message", DBNull.Value);

                    orderItemId = (int)command.ExecuteScalar();
 
                    return orderItemId;
                }
            }
        }

        public int UpdateOrderItem(int skuId, int? qty, string description, bool? tax, int? priceTier, double? unitPrice, double? lineTotal, string salesCode, string sku, string qboOrderItemId, int lineNum, int createdBy, int updatedBy, int orderItemId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE OrderItems SET skuId = @Value1, quantity = @Value2, description = @Value3, tax = @Value4, priceTier = @Value5, unitPrice = @Value6, lineTotal = @Value7, salesCode = @Value8, sku = @Value9, createdBy = @Value10, updatedBy = @Value11, qboOrderItemId = @Value12, lineNum = @Value13 WHERE id = @Value14";
                    command.Parameters.AddWithValue("@Value1", skuId);
                    if (qty != null)
                        command.Parameters.AddWithValue("@Value2", qty);
                    else command.Parameters.AddWithValue("@Value2", DBNull.Value);
                    command.Parameters.AddWithValue("@Value3", description);
                    if (tax != null)
                        command.Parameters.AddWithValue("@Value4", Convert.ToInt32(tax));
                    else command.Parameters.AddWithValue("@Value4", DBNull.Value);
                    if (priceTier != null)
                        command.Parameters.AddWithValue("@Value5", priceTier);
                    else command.Parameters.AddWithValue("@Value5", DBNull.Value);
                    if (unitPrice != null)
                        command.Parameters.AddWithValue("@Value6", unitPrice);
                    else command.Parameters.AddWithValue("@Value6", DBNull.Value);
                    if (lineTotal != null)
                        command.Parameters.AddWithValue("@Value7", lineTotal);
                    else command.Parameters.AddWithValue("@Value7", DBNull.Value);
                    command.Parameters.AddWithValue("@Value8", salesCode);
                    command.Parameters.AddWithValue("@Value9", sku);
                    command.Parameters.AddWithValue("@Value10", createdBy);
                    command.Parameters.AddWithValue("@Value11", updatedBy);
                    command.Parameters.AddWithValue("@Value12", qboOrderItemId);
                    command.Parameters.AddWithValue("@Value13", lineNum);
                    command.Parameters.AddWithValue("@Value14", orderItemId);

                    command.ExecuteNonQuery();

                    //MessageBox.Show("OrderItem is updated successfully.");

                    return orderItemId;
                }
            }
        }
    }
}
