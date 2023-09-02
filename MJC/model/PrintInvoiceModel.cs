using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using MJC.config;


namespace MJC.model
{
    public struct PrintSoldToInfo
    {
        public string customerName { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipcode { get; set; }
        public string businessPhone { get; set; }
        public PrintSoldToInfo(string _customerName, string _address1, string _address2, string _city, string _state, string _zipcode, string _businessPhone)
        {
            customerName = _customerName;
            address1 = _address1;
            address2 = _address2;
            city = _city;
            state = _state;
            zipcode = _zipcode;
            businessPhone = _businessPhone;
        }
    }

    public struct PrintShipToInfo
    {
        public string name { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipcode { get; set; }
        public string businessPhone { get; set; }
        public PrintShipToInfo(string _name, string _address1, string _address2, string _city, string _state, string _zipcode, string _businessPhone)
        {
            name = _name;
            address1 = _address1;
            address2 = _address2;
            city = _city;
            state = _state;
            zipcode = _zipcode;
            businessPhone = _businessPhone;
        }
    }

    public struct PrintOrderInfo
    {
        public string customerName { get; set; }
        public DateTime? date  { get; set; }
        public string poNumber { get; set; }
        public string shipVia { get; set; }
        public string terms { get; set; }

        public string invoiceNumber { get; set; }

        public PrintOrderInfo(string _customerName, DateTime? _date, string _poNumber, string _shipVia, string _terms, string _invoiceNumber)
        {
            customerName = _customerName;
            date = _date;
            poNumber = _poNumber;
            shipVia = _shipVia;
            terms = _terms;
            invoiceNumber = _invoiceNumber;
        }
    }

    public struct PrintOrderItemInfo
    {
        public int? Qty { get; set; }
        public string PartNo { get; set; }
        public double? List { get; set; }
        public double? Net { get; set; }
        public double? Core { get; set; }
        public double? Labor { get; set; }
        public double? Extended { get; set; }
        //public string sku { get; set; }
        //public string categoryName { get; set; }
        //public string description { get; set; }

        public PrintOrderItemInfo(int _qty, string? _partNo, double? _list, double? _net, double? _core, double? _labor, double? _extended)
        {
            Qty = _qty;
            PartNo = _partNo;
            List = _list;
            Net = _net;
            Core = _core;
            Labor = _labor;
            Extended = _extended;
            //sku = _sku;
            //categoryName = _categoryName;
            //description = _description;
        }
    }

    public class PrintInvoiceModel : DbConnection
    {
        public PrintSoldToInfo GetSoldToInfo(int customerId)
        {
            PrintSoldToInfo soldToInfo = new PrintSoldToInfo();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataReader reader;

                    command.CommandText = @"SELECT displayName, address1, address2, city, state, zipcode, businessPhone FROM Customers WHERE id = @Value1";
                    command.Parameters.AddWithValue("@Value1", customerId);

                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        soldToInfo = new PrintSoldToInfo { customerName = reader[0].ToString(), address1 = reader[1].ToString(), address2 = reader[2].ToString(), city = reader[3].ToString(), state = reader[4].ToString(), zipcode = reader[5].ToString(), businessPhone = reader[6].ToString() };
                    }
                    reader.Close();
                }
            }
            return soldToInfo;
        }

        public PrintShipToInfo GetPrintShipToInfo(int customerId)
        {
            PrintShipToInfo PrintShipToInfo = new PrintShipToInfo();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataReader reader;

                    command.CommandText = @"SELECT name, address1, address2, city, state, zipcode FROM CustomerShipTos WHERE customerId = @Value1";
                    command.Parameters.AddWithValue("@Value1", customerId);

                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        PrintShipToInfo = new PrintShipToInfo { name = reader[0].ToString(), address1 = reader[1].ToString(), address2 = reader[2].ToString(), city = reader[3].ToString(), state = reader[4].ToString(), zipcode = reader[5].ToString(), businessPhone = "(740) - 374 2306 " };
                    }
                    reader.Close();
                }
            }
            return PrintShipToInfo;
        }

        public PrintOrderInfo GetPrintOrderInfo(int orderId)
        {
            PrintOrderInfo PrintOrderInfo = new PrintOrderInfo();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataReader reader;

                    command.CommandText = @"SELECT customerName, invoiceDate, poNumber, shipVia, terms, invoiceNumber FROM Orders WHERE id = @Value1";
                    command.Parameters.AddWithValue("@Value1", orderId);

                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        DateTime? date = null;
                        if (!reader.IsDBNull(1))
                            date = DateTime.Parse(reader[1].ToString());
                        PrintOrderInfo = new PrintOrderInfo { customerName = reader[0].ToString(), date = date, poNumber = reader[2].ToString(), shipVia = reader[3].ToString(), terms = reader[4].ToString(), invoiceNumber = reader[5].ToString() };
                    }
                    reader.Close();
                }
            }
            return PrintOrderInfo;
        }

        public List<PrintOrderItemInfo> GetPrintOrderItemInfo(int orderId)
        {
            List<PrintOrderItemInfo> printOrderItemInfoList = new List<PrintOrderItemInfo>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataAdapter sda;
                    DataSet ds;

                    command.CommandText = @"SELECT categoryName, tblSKU.* FROM Categories RIGHT JOIN (SELECT category, tblOrderSKU.* FROM SKU INNER JOIN (SELECT skuId, sku, description, message, quantity, coreCharge, lineTotal, orderItemType FROM OrderItems WHERE orderId = @Value1) AS tblOrderSKU ON tblOrderSKU.skuId = SKU.id) AS tblSKU ON tblSKU.category = Categories.id";
                    command.Parameters.AddWithValue("@Value1", orderId);

                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        string categoryName = row["categoryName"].ToString();
                        string sku = row["sku"].ToString();
                        int? qty = null;
                        if (!row.IsNull("quantity"))
                            qty = int.Parse(row["quantity"].ToString());
                        string desc = row["description"].ToString();
                        string message = row["message"].ToString();
                        double? coreCharge = null;
                        if (!row.IsNull("coreCharge"))
                            coreCharge = double.Parse(row["coreCharge"].ToString());
                        double? lineTotal = null;
                        if (!row.IsNull("lineTotal"))
                            lineTotal = double.Parse(row["lineTotal"].ToString());
                        int? orderItemType = null;
                        if (!row.IsNull("orderItemType"))
                            orderItemType = int.Parse(row["orderItemType"].ToString());
                        string partNo = "";
                        switch (orderItemType)
                        {
                            case 1:
                                partNo = sku + "\n" + categoryName + " " + desc;
                                break;
                            case 2:
                                partNo = message;
                                break;
                            case 3:
                                partNo = sku + "\n" + desc;
                                break;
                            case 4:
                                partNo = sku + "\n" + desc;
                                break;
                            default:
                                partNo = sku + "\n" + categoryName + " " + desc;
                                break;
                        }
                        printOrderItemInfoList.Add(new PrintOrderItemInfo { Qty = qty, PartNo = partNo, Core = coreCharge, Extended = lineTotal });
                    }
                }
            }

            return printOrderItemInfoList;
        }
    }
}
