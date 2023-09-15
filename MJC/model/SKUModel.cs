using System;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Security.Cryptography;
using System.Xml.Linq;
using Antlr4.Runtime.Tree;
using MJC.common;
using MJC.config;

namespace MJC.model
{
    public struct SKUDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int QuantityAvailable { get; set; }
        public int QuantityOnHand { get; set; }

        public SKUDetail(int id, string name, string category, string description, int quantity, int quantityAvailable, int quantityOnHand)
        {
            Id = id;
            Name = name;
            Category = category;
            Description = description;
            QuantityAvailable = quantityAvailable;
            Quantity = quantity;
            QuantityOnHand = quantityOnHand;
        }
    }

    public struct SKUProfile
    {
        public int SkuId { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public string InvoiceNumber { get; set; }
        public int? Qty { get; set; }
        public double? Price { get; set; }

        public SKUProfile(int skuId, DateTime? invoiceDate, string customerName, string invoiceNumber, int? qty, double? price)
        {
            SkuId = skuId;
            InvoiceDate = invoiceDate;
            InvoiceNumber = invoiceNumber;
            CustomerName = customerName;
            InvoiceNumber = invoiceNumber;
            Qty = qty;
            Price = price;
        }
    }

    public struct SKUAllocation
    {
        public DateTime? DateShipped { get; set; }
        public string CustomerName { get; set; }
        public int? HeldStatus { get; set; }
        public string ProcessedBy { get; set; }
        public int? Qty { get; set; }
        public double? Price { get; set; }
        public double? SubTotal { get; set; }
        public int CustomerId { get; set; }

        public SKUAllocation(DateTime? dateShipped, string customerName, int? heldStatus, string processedBy, int? qty, double? price, double? subTotal, int customerId)
        {
            DateShipped = dateShipped;
            CustomerName = customerName;
            HeldStatus = heldStatus;
            ProcessedBy = processedBy;
            Qty = qty;
            Price = price;
            SubTotal = subTotal;
            CustomerId = customerId;
        }
    }

    public struct SKURecorder
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public string Description { get; set; }
        public int Qty { get; set; }
        public int QtyAvailable { get; set; }
        public int QtyCritical { get; set; }
        public int QtyRecorder { get; set; }
        public string QboSkuId { get; set; }

        public SKURecorder(int id, string sku, string desc, int qty, int qtyAvailable, int qtyCritical, int qtyRecorder, string qboSkuId)
        {
            Id = id;
            Sku = sku;
            Description = desc;
            Qty = qty;
            QtyAvailable = qtyAvailable;
            QtyCritical = qtyCritical;
            QtyRecorder = qtyRecorder;
            QboSkuId = qboSkuId;
        }
    }

    public struct SKUOrderItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Qty { get; set; }
        public int? CostCode { get; set; }
        public string PriceTier { get; set; }
        public int PriceTierId { get; set; }
        public double? Price { get; set; }
        public string QboSkuId { get; set; }

        public SKUOrderItem(int id, string name, string description, int qty, double? price, int? costCode, string priceTier, int priceTierId, string qboSkuId)
        {
            Id = id;
            Name = name;
            Description = description;
            Qty = qty;
            Price = price;
            CostCode = costCode;
            PriceTier = priceTier;
            PriceTierId = priceTierId;
            QboSkuId = qboSkuId;
        }
    }

    public struct SKUTax
    {
        public bool Value { get; set; }
        public string DisplayName { get; set; }

        public SKUTax(bool value, string displayName) { Value = value; DisplayName = displayName; }
    }

    public class SKUModel : DbConnection
    {

        public SKUModel() { }

        public List<SKUDetail> SKUDataList { get; private set; }
        public List<SKUProfile> SKUProfileList { get; private set; }
        public List<SKUAllocation> SKUAllocationList { get; private set; }

        private SKUPricesModel SKUPricesModelObj = new SKUPricesModel();
        private CategoryPriceTierModel CategoryPriceModelObj = new CategoryPriceTierModel();
        private PriceTiersModel PriceTiersModelObj = new PriceTiersModel();

        public List<SKUOrderItem> LoadSkuOrderItems()
        {
            List<SKUOrderItem> skuList = new List<SKUOrderItem>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataAdapter sda;
                    DataSet ds;

                    command.CommandText = @"SELECT PriceTiers.priceTierCode, tblPriceTier.* FROM (SELECT dbo.SKU.id, dbo.SKU.sku, dbo.SKU.description, dbo.SKU.quantity, dbo.SKU.costCode, SKUPrices.priceTierId, SKUPrices.price, dbo.SKU.qboSkuId FROM dbo.SKU INNER JOIN SKUPrices ON SKUPrices.skuId = SKU.id) AS tblPriceTier INNER JOIN PriceTiers ON PriceTiers.id = tblPriceTier.priceTierId";

                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int id = int.Parse(row["id"].ToString());
                        string name = row["sku"].ToString();
                        string description = row["description"].ToString();
                        int? qty = null;
                        if (!row.IsNull("quantity"))
                            qty = int.Parse(row["quantity"].ToString());
                        int? sc = null;
                        if (!row.IsNull("costCode"))
                            sc = int.Parse(row["costCode"].ToString());
                        double? price = null;
                        if (!row.IsNull("price"))
                            price = double.Parse(row["price"].ToString());
                        string priceTier = row["priceTierCode"].ToString();
                        int priceTierId = int.Parse(row["priceTierId"].ToString());
                        string qboSkuId = row["qboSkuId"].ToString();

                        skuList.Add(new SKUOrderItem { Id = id, Name = name, Description = description, CostCode = sc, Price = price, PriceTier = priceTier, QboSkuId = qboSkuId, PriceTierId = priceTierId });
                    }
                }
            }

            return skuList;
        }

        public bool LoadSKUData(string filter, bool archived)
        {
            SKUDataList = new List<SKUDetail>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataReader reader;

                    command.CommandText = @"select S_Table.id, sku, C_Table.categoryName, description, quantity, qtyAvailable, qtyAllocated from dbo.SKU as S_Table LEFT join dbo.Categories C_Table on category = C_Table.id";

                    if (archived) command.CommandText += " where S_Table.archived = 1";

                    if (filter != "")
                    {
                        command.CommandText = @"select S_Table.id, sku, C_Table.categoryName, description, quantity, qtyAvailable, qtyAllocated
                                                from dbo.SKU as S_Table
                                                LEFT join dbo.Categories C_Table on category = C_Table.id 
                                                where S_Table.description like @filter 
                                                or S_Table.sku like @filter
                                                or C_Table.categoryName like @filter";
                        command.Parameters.Add("@filter", System.Data.SqlDbType.VarChar).Value = "%" + filter + "%";
                    }

                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int.TryParse(reader[0].ToString(), out int id);
                        string name = reader[1].ToString();
                        string desc = reader[2].ToString();
                        string category = reader[3].ToString();
                        int quantity = (int)reader[4];
                        int quantityAvailable = (int)reader[5];
                        int quantityAllocated = (int)reader[6];

                        SKUDetail skuItem = new SKUDetail(id, name, desc, category, quantity, quantityAvailable, quantityAllocated);
                        SKUDataList.Add(skuItem);
                    }
                    reader.Close();
                }
            }

            return true;
        }

        public bool AddSKU(string sku__name,
            int category,
            string description,
            string measurement_unit,
            int weight,
            int cost_code,
            int asset_acct,
            bool taxable,
            bool maintain_qty,
            bool allow_discount,
            bool commissionable,
            int order_from,
            DateTime? last_sold,
            string manufacturer,
            string? location,
            int quantity,
            int qty_allocated,
            int qty_available,
            int critical_qty,
            int reorder_qty,
            int sold_this_month,
            int sold_ytd,
            bool freeze_prices,
            double core_cost,
            double inv_value,
            string memo,
            Dictionary<int, double> priceTierDict,
            bool billAslabor,
            string syncToken,
            string qboSkuId,
            bool hidden,
            bool editingQuantity
            )
        {

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    //Get Total Number of Customers
                    command.CommandText = "INSERT INTO dbo.SKU (active, sku, category, description, measurementUnit, weight, costCode, assetAccount, taxable, manageStock, allowDiscounts, commissionable, orderFrom, lastSold, manufacturer, location, quantity, qtyAllocated, qtyAvailable, qtyCritical, qtyReorder, soldMonthToDate, soldYearToDate, freezePrices, coreCost, inventoryValue, createdAt, createdBy, updatedAt, updatedBy, subassemblyStatus, subassemblyPrint, memo, billAsLabor, syncToken, qboSkuId, hidden, editingQuantity) OUTPUT INSERTED.ID VALUES (@active, @Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7, @Value8, @Value9, @Value10, @Value11, @Value12, @Value13, @Value14, @Value15, @Value16, @Value17, @Value18, @Value19, @Value20, @Value21, @Value22, @Value23, @Value24, @Value25, @Value26, @Value27, @Value28, @Value29, @Value30, @Value31, @memo,  @billAsLabor, @syncToken, @qboSkuId, @hidden,  @editingQuantity)";
                    command.Parameters.AddWithValue("@active", true);
                    command.Parameters.AddWithValue("@Value1", sku__name);
                    command.Parameters.AddWithValue("@Value2", category);
                    if(!string.IsNullOrEmpty(description))
                        command.Parameters.AddWithValue("@Value3", description);
                    else command.Parameters.AddWithValue("@Value3", DBNull.Value);
                    command.Parameters.AddWithValue("@Value4", measurement_unit);
                    command.Parameters.AddWithValue("@Value5", weight);
                    command.Parameters.AddWithValue("@Value6", cost_code);
                    command.Parameters.AddWithValue("@Value7", asset_acct);
                    command.Parameters.AddWithValue("@Value8", taxable);
                    command.Parameters.AddWithValue("@Value9", maintain_qty);
                    command.Parameters.AddWithValue("@Value10", allow_discount);
                    command.Parameters.AddWithValue("@Value11", commissionable);
                    command.Parameters.AddWithValue("@Value12", order_from);
                    if(last_sold != null)
                        command.Parameters.AddWithValue("@Value13", last_sold);
                    else command.Parameters.AddWithValue("@Value13", DBNull.Value);
                    command.Parameters.AddWithValue("@Value14", manufacturer);
                    if(!string.IsNullOrEmpty(location))
                        command.Parameters.AddWithValue("@Value15", location);
                    else command.Parameters.AddWithValue("@Value15", DBNull.Value);
                    command.Parameters.AddWithValue("@Value16", quantity);
                    command.Parameters.AddWithValue("@Value17", qty_allocated);
                    command.Parameters.AddWithValue("@Value18", qty_available);
                    command.Parameters.AddWithValue("@Value19", critical_qty);
                    command.Parameters.AddWithValue("@Value20", reorder_qty);
                    command.Parameters.AddWithValue("@Value21", sold_this_month);
                    command.Parameters.AddWithValue("@Value22", sold_ytd);
                    command.Parameters.AddWithValue("@Value23", freeze_prices);
                    command.Parameters.AddWithValue("@Value24", core_cost);
                    command.Parameters.AddWithValue("@Value25", inv_value);
                    command.Parameters.AddWithValue("@Value26", DateTime.Now);
                    command.Parameters.AddWithValue("@Value27", 1);
                    command.Parameters.AddWithValue("@Value28", DateTime.Now);
                    command.Parameters.AddWithValue("@Value29", 1);
                    command.Parameters.AddWithValue("@Value30", false);
                    command.Parameters.AddWithValue("@Value31", false);
                    if(!string.IsNullOrEmpty(memo))
                        command.Parameters.AddWithValue("@memo", memo);
                    else command.Parameters.AddWithValue("@memo", DBNull.Value);
                    command.Parameters.AddWithValue("@billAsLabor", billAslabor);
                    command.Parameters.AddWithValue("@syncToken", syncToken);
                    command.Parameters.AddWithValue("@qboSkuId", qboSkuId);
                    command.Parameters.AddWithValue("@hidden", hidden);
                    command.Parameters.AddWithValue("@editingQuantity", editingQuantity);

                    int newId = (int)command.ExecuteScalar();

                    foreach (KeyValuePair<int, double> pair in priceTierDict)
                    {
                        int key = pair.Key;
                        double value = pair.Value;

                        SKUPricesModelObj.AddSKUPrice(newId, key, value);
                    }

                    //MessageBox.Show("New SKU is added successfully.");
                }

                return true;
            }
        }

        public bool UpdateSKU(string sku__name,
            int category,
            string description,
            string measurement_unit,
            int weight,
            int cost_code,
            int asset_acct,
            bool taxable,
            bool maintain_qty,
            bool allow_discount,
            bool commissionable,
            int order_from,
            DateTime last_sold,
            string manufacturer,
            string location,
            int quantity,
            int qty_allocated,
            int qty_available,
            int critical_qty,
            int reorder_qty,
            int sold_this_month,
            int sold_ytd,
            bool freeze_prices,
            double core_cost,
            double inv_value,
            string memo,
            Dictionary<int, double> priceTierDict,
            bool billAsLabor,
            bool editingQuantity,
            int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE dbo.SKU SET sku = @Value1, category = @Value2, description = @Value3, measurementUnit = @Value4, weight = @Value5, costCode = @Value6, assetAccount = @Value7, taxable = @Value8, manageStock = @Value9, allowDiscounts = @Value10, commissionable = @Value11, orderFrom = @Value12, lastSold = @Value13, manufacturer = @Value14, location = @Value15, quantity = @Value16, qtyAllocated = @Value17, qtyAvailable = @Value18, qtyCritical = @Value19, qtyReorder = @Value20, soldMonthToDate = @Value21, soldYearToDate = @Value22, freezePrices = @Value23, coreCost = @Value24, inventoryValue = @Value25, memo = @memo, billAsLabor = @billAsLabor, editingQuantity = @editingQuantity WHERE id = @Value26";
                    command.Parameters.AddWithValue("@Value1", sku__name);
                    command.Parameters.AddWithValue("@Value2", category);
                    command.Parameters.AddWithValue("@Value3", description);
                    command.Parameters.AddWithValue("@Value4", measurement_unit);
                    command.Parameters.AddWithValue("@Value5", weight);
                    command.Parameters.AddWithValue("@Value6", cost_code);
                    command.Parameters.AddWithValue("@Value7", asset_acct);
                    command.Parameters.AddWithValue("@Value8", taxable);
                    command.Parameters.AddWithValue("@Value9", maintain_qty);
                    command.Parameters.AddWithValue("@Value10", allow_discount);
                    command.Parameters.AddWithValue("@Value11", commissionable);
                    command.Parameters.AddWithValue("@Value12", order_from);
                    command.Parameters.AddWithValue("@Value13", last_sold);
                    command.Parameters.AddWithValue("@Value14", manufacturer);
                    command.Parameters.AddWithValue("@Value15", location);
                    command.Parameters.AddWithValue("@Value16", quantity);
                    command.Parameters.AddWithValue("@Value17", qty_allocated);
                    command.Parameters.AddWithValue("@Value18", qty_available);
                    command.Parameters.AddWithValue("@Value19", critical_qty);
                    command.Parameters.AddWithValue("@Value20", reorder_qty);
                    command.Parameters.AddWithValue("@Value21", sold_this_month);
                    command.Parameters.AddWithValue("@Value22", sold_ytd);
                    command.Parameters.AddWithValue("@Value23", freeze_prices);
                    command.Parameters.AddWithValue("@Value24", core_cost);
                    command.Parameters.AddWithValue("@Value25", inv_value);
                    command.Parameters.AddWithValue("@memo", memo);
                    command.Parameters.AddWithValue("@billAsLabor", billAsLabor);
                    command.Parameters.AddWithValue("@editingQuantity", editingQuantity);
                    command.Parameters.AddWithValue("@Value26", id);

                    command.ExecuteNonQuery();

                    foreach (KeyValuePair<int, double> pair in priceTierDict)
                    {
                        int key = pair.Key;
                        double value = pair.Value;
                        bool active = true;
                        int createdBy = 1;
                        int updatedBy = 1;

                        if (!SKUPricesModelObj.UpdateSKUPrice(id, key, value, active, createdBy, updatedBy)) SKUPricesModelObj.AddSKUPrice(id, key, value);
                    }

                    Messages.ShowInformation("The SKU updated successfully.");
                }

                return true;
            }
        }

        public bool UpdateSKUMemo(string sku__memo, int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE dbo.SKU SET memo = @Value1 WHERE id = @Value2";
                    command.Parameters.AddWithValue("@Value1", sku__memo);
                    command.Parameters.AddWithValue("@Value2", id);

                    command.ExecuteNonQuery();
                }

                return true;
            }
        }

        public bool UpdateSKUArchived(bool archived, int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE dbo.SKU SET archived = @Value1 WHERE id = @Value2";
                    command.Parameters.AddWithValue("@Value1", archived);
                    command.Parameters.AddWithValue("@Value2", id);

                    command.ExecuteNonQuery();
                }

                return true;
            }
        }

        public bool DeleteSKU(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "DELETE FROM dbo.SKU WHERE id = @Value1";
                    command.Parameters.AddWithValue("@Value1", id);

                    command.ExecuteNonQuery();

                    Messages.ShowInformation("The SKU was deleted.");
                }

                return true;
            }
        }
        public double GetInventoryValue(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    SqlDataReader reader;
                    double inventoryValue = 0;

                    command.Connection = connection;

                    command.CommandText = @"select inventoryValue from dbo.SKU where id = @id";
                    command.Parameters.AddWithValue("@id", id);

                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        inventoryValue = Convert.ToDouble(result);
                    }

                    return inventoryValue;
                }
            }
        }

        public List<dynamic> GetSKUData(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    SqlDataReader reader;
                    List<dynamic> returnList = new List<dynamic>();

                    command.Connection = connection;

                    command.CommandText = @"select * from dbo.SKU where id = @id";
                    command.Parameters.AddWithValue("@id", id);

                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new ExpandoObject() as IDictionary<string, object>;

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row.Add(reader.GetName(i), reader[i]);
                        }

                        returnList.Add(row);
                    }
                    reader.Close();

                    return returnList;
                }
            }
        }

        public int GetQuantity(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"select quantity from dbo.SKU where id = @id";
                    command.Parameters.AddWithValue("@id", id);

                    // Execute the command and retrieve the quantity value.
                    object result = command.ExecuteScalar();

                    // Convert the result to an int before returning it.
                    if (result == null || result == DBNull.Value)
                    {
                        return 0;
                    }
                    else
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }
        }
        public int GetQuantityAvailable(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"select qtyAvailable from dbo.SKU where id = @id";
                    command.Parameters.AddWithValue("@id", id);

                    // Execute the command and retrieve the quantity value.
                    object result = command.ExecuteScalar();

                    // Convert the result to an int before returning it.
                    if (result == null || result == DBNull.Value)
                    {
                        return 0;
                    }
                    else
                    {
                        return Convert.ToInt32(result);
                    }
                }
            }
        }

        public bool UpdateQuantity(int quantity, int availableQuantity, int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE dbo.SKU SET quantity = @Value1, qtyAvailable = @Value3 WHERE id = @Value2";
                    command.Parameters.AddWithValue("@Value1", quantity);
                    command.Parameters.AddWithValue("@Value2", id);
                    command.Parameters.AddWithValue("@Value3", availableQuantity);

                    command.ExecuteNonQuery();
                }

                return true;
            }
        }

        public bool UpdateQuantityAvailable(int quantity, int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE dbo.SKU SET qtyAvailable = @Value1 WHERE id = @Value2";
                    command.Parameters.AddWithValue("@Value1", quantity);
                    command.Parameters.AddWithValue("@Value2", id);

                    command.ExecuteNonQuery();
                }

                return true;
            }
        }

        public List<KeyValuePair<int, string>> GetSKUItems()
        {
            List<KeyValuePair<int, string>> SKUList = new List<KeyValuePair<int, string>>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataReader reader;

                    command.CommandText = @"select id, sku
                                            from dbo.sku";
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        SKUList.Add(
                            new KeyValuePair<int, string>((int)reader[0], reader[1].ToString())
                        );
                    }
                    reader.Close();
                }
            }
            return SKUList;
        }

        public bool LoadSkuProfile(int skuId)
        {
            SKUProfileList = new List<SKUProfile>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataAdapter sda;
                    DataSet ds;

                    command.CommandText = @"SELECT displayName, tblSKU.* FROM Customers RIGHT JOIN (SELECT SKUPrices.price, tblOrder.* FROM SKUPrices RIGHT JOIN (SELECT invoiceDate, customerId, invoiceNumber, tblOrderItem.* FROM Orders RIGHT JOIN (SELECT OrderItems.quantity, orderId, OrderItems.skuId, tblSKU.sku FROM OrderItems INNER JOIN (SELECT * FROM SKU WHERE id = @Value1) AS tblSKU ON tblSKU.id = OrderItems.skuId) AS tblOrderItem ON tblOrderItem.orderId = Orders.id) AS tblOrder ON tblOrder.skuId = SKUPrices.skuId) AS tblSKU ON tblSKU.customerId = Customers.id";
                    command.Parameters.AddWithValue("@Value1", skuId);

                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        DateTime? invoiceDate = null;
                        if (!row.IsNull("invoiceDate"))
                            invoiceDate = row.Field<DateTime>("invoiceDate");
                        string customerName = row["displayName"].ToString();
                        string invoiceNumber = row["invoiceNumber"].ToString();
                        int? qty = null;
                        if (!row.IsNull("quantity"))
                            qty = int.Parse(row["quantity"].ToString());
                        double? price = null;
                        if (!row.IsNull("price"))
                            price = double.Parse(row["price"].ToString());

                        SKUProfileList.Add(new SKUProfile(skuId, invoiceDate, customerName, invoiceNumber, qty, price));
                    }
                }
            }

            return true;
        }

        public bool LoadSkuAllocations(int skuId)
        {
            SKUAllocationList = new List<SKUAllocation>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataAdapter sda;
                    DataSet ds;

                    command.CommandText = @"SELECT displayName, tblOrder.*FROM Customers INNER JOIN(SELECT dateShipped, customerId, status, processedBy, tblOrderItem.* FROM Orders INNER JOIN(SELECT skuId, orderId, quantity, unitPrice, lineTotal FROM OrderItems WHERE skuId = @Value1) AS tblOrderItem ON tblOrderItem.orderId = Orders.id) AS tblOrder ON Customers.id = tblOrder.customerId";
                    command.Parameters.AddWithValue("@Value1", skuId);

                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {

                        DateTime? dateShipped = null;
                        if (!row.IsNull("dateShipped"))
                            dateShipped = row.Field<DateTime>("dateShipped");
                        string customerName = row["displayName"].ToString();
                        int? heldStatus = null;
                        if (!row.IsNull("status"))
                            heldStatus = int.Parse(row["status"].ToString());
                        string processedBy = row["processedBy"].ToString();
                        int? qty = null;
                        if (!row.IsNull("quantity"))
                            qty = int.Parse(row["quantity"].ToString());
                        double? price = null;
                        if (!row.IsNull("unitPrice"))
                            price = double.Parse(row["unitPrice"].ToString());
                        double? subTotal = null;
                        if (!row.IsNull("lineTotal"))
                            subTotal = double.Parse(row["lineTotal"].ToString());
                        int customerId = int.Parse(row["customerId"].ToString());

                        SKUAllocationList.Add(new SKUAllocation(dateShipped, customerName, heldStatus, processedBy, qty, price, subTotal, customerId));
                    }
                }
            }

            return true;
        }

        public dynamic LoadSkuQty(int skuId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT quantity, qtyAllocated, qtyAvailable, qtyReorder FROM SKU WHERE id = @Value1";
                    command.Parameters.AddWithValue("@Value1", skuId);

                    var reader = command.ExecuteReader();
                    if (reader.Read()) // check if there are any rows returned
                    {
                        int qty = (int)reader.GetValue(0);
                        int qtyAllocated = (int)reader.GetValue(1);
                        int qtyAvailable = (int)reader.GetValue(2);

                        var qtyInfo = new
                        {
                            qty = qty,
                            qtyAllocated = qtyAllocated,
                            qtyAvailable = qtyAvailable
                        };

                        return qtyInfo;
                    }
                    return null;
                }
            }
        }

        //public List<string> GetSkuNameList()
        //{
        //    List<string> skuList = new List<string>();

        //    using (var connection = GetConnection())
        //    {
        //        connection.Open();
        //        using (var command = new SqlCommand())
        //        {
        //            command.Connection = connection;
        //            command.CommandText = @"SELECT sku FROM SKU";

        //            var reader = command.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                string skuName = reader.GetValue(0).ToString();

        //                skuList.Add(skuName);

        //            }
        //            return skuList;
        //        }
        //    }
        //}

        public bool InsertSKUCostQty(bool active, int skuId, DateTime costDate, int qty, double cost, Dictionary<int, double> priceTierDict, int createdBy, int updatedBy)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"INSERT INTO SKUCostQtys(active, skuId, costDate, qty, cost, createdBy, updatedBy) VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7)";
                    command.Parameters.AddWithValue("@Value1", Convert.ToInt32(active));
                    command.Parameters.AddWithValue("@Value2", skuId);
                    command.Parameters.AddWithValue("@Value3", costDate);
                    command.Parameters.AddWithValue("@Value4", qty);
                    command.Parameters.AddWithValue("@Value5", cost);
                    command.Parameters.AddWithValue("@Value6", createdBy);
                    command.Parameters.AddWithValue("@Value7", updatedBy);

                    command.ExecuteNonQuery();

                }

                foreach (KeyValuePair<int, double> pair in priceTierDict)
                {
                    int key = pair.Key;
                    double value = pair.Value;

                    if (!SKUPricesModelObj.UpdateSKUPrice(skuId, key, value, active, createdBy, updatedBy)) SKUPricesModelObj.AddSKUPrice(skuId, key, value);
                }

                return true;
            }
        }

        public bool UpdateSKUQty(int qty, int skuId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE SKU SET quantity = @Value1, qtyAvailable = @Value1 WHERE id = @Value2";
                    command.Parameters.AddWithValue("@Value1", qty);
                    command.Parameters.AddWithValue("@Value2", skuId);

                    command.ExecuteNonQuery();

                }

                return true;
            }
        }

        public List<SKURecorder> LoadSKURecorderList()
        {
            using (var connection = GetConnection())
            {
                List<SKURecorder> skuRecorderList = new List<SKURecorder>();
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT id, sku, description, quantity, qtyAvailable, qtyCritical, qtyReorder, qboSkuId FROM SKU";

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = (int)reader.GetValue(0);
                        string sku = reader.GetValue(1).ToString();
                        string description = reader.GetValue(2).ToString();
                        int qty = (int)reader.GetValue(3);
                        int qtyAvailable = (int)reader.GetValue(4);
                        int qtyCritical = (int)reader.GetValue(5);
                        int qtyReorder = (int)reader.GetValue(6);
                        string qboSkuId = reader.GetValue(7).ToString();

                        if (qtyAvailable <= qtyCritical)
                        {
                            skuRecorderList.Add(new SKURecorder(id, sku, description, qty, qtyAvailable, qtyCritical, qtyReorder, qboSkuId));
                        }
                    }
                    return skuRecorderList;
                }
            }
        }

        public void SetPrice(int categoryId, int calculateAs)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                SqlDataAdapter sda;
                DataSet ds;
                PriceTiersModelObj.LoadPriceTierData();
                List<PriceTierData> PriceTiersList = PriceTiersModelObj.PriceTierDataList;
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT * FROM SKU WHERE category = @Value1";
                    command.Parameters.AddWithValue("@Value1", categoryId);
                    command.ExecuteNonQuery();
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int skuId = Convert.ToInt32(row["id"]);
                        double inventoryValue = Convert.ToDouble(row["inventoryValue"]);
                        bool freeezePrice = Convert.ToBoolean(row["freezePrices"]);
                        bool active = true;
                        int createdBy = 1;
                        int updatedBy = 1;

                        if (!freeezePrice)
                        {
                            foreach (PriceTierData priceTierData in PriceTiersList)
                            {
                                int priceTierId = priceTierData.Id;
                                double priceTierProfitMargin = priceTierData.ProfitMargin;
                                double price = 0;
                                double categoryMargin = CategoryPriceModelObj.GetCategoryMarginByPriceTierId(categoryId, priceTierId);
                                if (calculateAs == 1)
                                {
                                    price = (inventoryValue + categoryMargin) * (1 + priceTierProfitMargin);
                                }
                                else if (calculateAs == 2)
                                {
                                    price = (inventoryValue * (1 + categoryMargin)) * (1 + priceTierProfitMargin);
                                }

                                bool isExistPrice = SKUPricesModelObj.IsExistSkuPriceByPriceTierId(skuId, priceTierId);

                                if (!isExistPrice)
                                    SKUPricesModelObj.AddSKUPrice(skuId, priceTierId, price);
                                else SKUPricesModelObj.UpdateSKUPrice(skuId, priceTierId, price, active, createdBy, updatedBy);
                            }
                        }
                    }
                }

                MessageBox.Show("Reset all prices for SKUs successfully.");
            }
        }

        public List<int[]> LoadQtyHistoryData(int skuId)
        {
            List<int[]> TotalQtyByYearList = new List<int[]>();

            using (var connection = GetConnection())
            {
                connection.Open();
                List<int> yearList = new List<int>();
                yearList = GetQtyYearList(skuId);

                foreach (int year in yearList)
                {
                    int[] totalQtyByMonth = new int[13];
                    List<KeyValuePair<int, int>> totalQtyList = GetTotalQtyByYear(year, skuId);
                    foreach (KeyValuePair<int, int> totalQty in totalQtyList)
                    {
                        totalQtyByMonth[totalQty.Key] = totalQty.Value;
                    }
                    TotalQtyByYearList.Add(totalQtyByMonth);
                }
            }
            return TotalQtyByYearList;
        }

        public List<int> GetQtyYearList(int skuId)
        {
            List<int> yearList = new List<int>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataAdapter sda;
                    DataSet ds;

                    command.CommandText = @"SELECT YEAR(costDate) AS year FROM SKUCostQtys WHERE skuId = @Value1 GROUP BY YEAR(costDate) ORDER BY year DESC";
                    command.Parameters.AddWithValue("@Value1", skuId);

                    command.ExecuteNonQuery();
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int year = int.Parse(row["year"].ToString());
                        yearList.Add(year);
                    }
                }
            }
            return yearList;
        }

        public List<KeyValuePair<int, int>> GetTotalQtyByYear(int year, int skuId)
        {
            List<KeyValuePair<int, int>> totalQtyListByMonth = new List<KeyValuePair<int, int>>();
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataAdapter sda;
                    DataSet ds;

                    command.CommandText = @"SELECT SUM(qty) AS total, MONTH(costDate) AS month FROM SKUCostQtys WHERE YEAR(costDate) = @Value1 AND skuId = @Value2 GROUP BY  MONTH(costDate)";
                    command.Parameters.AddWithValue("@Value1", year);
                    command.Parameters.AddWithValue("@Value2", skuId);

                    command.ExecuteNonQuery();
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    int totalQtyByYear = 0;
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int totalQty = int.Parse(row["total"].ToString());
                        int month = int.Parse(row["month"].ToString()) - 1;

                        totalQtyByYear += totalQty;
                        totalQtyListByMonth.Add(new KeyValuePair<int, int>(month, totalQty));
                    }
                    totalQtyListByMonth.Add(new KeyValuePair<int, int>(12, totalQtyByYear));
                }
            }

            return totalQtyListByMonth;
        }

        public string GetSkuNameById(int skuId)
        {
            string skuName = "";

            using (var connection = GetConnection())
            {
                connection.Open();
                SqlDataAdapter sda;
                DataSet ds;

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT sku FROM SKU WHERE id=@Value1";
                    command.Parameters.AddWithValue("@Value1", skuId);

                    command.ExecuteNonQuery();
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    DataRow row = ds.Tables[0].Rows[0];

                    skuName = row["sku"].ToString();

                    return skuName;
                }
            }
        }

        public SKUDetail GetSKUById(int skuId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                SqlDataAdapter sda;
                DataSet ds;

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"select S_Table.id, sku, C_Table.categoryName, description, qtyAvailable, qtyReorder from (SELECT * FROM SKU WHERE id = @Value1) as S_Table inner join dbo.Categories C_Table on category = C_Table.id;";

                    command.Parameters.AddWithValue("@Value1", skuId);

                    command.ExecuteNonQuery();
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    DataRow row = ds.Tables[0].Rows[0];

                    string skuName = row["sku"].ToString();
                    string categoryName = row["categoryName"].ToString();
                    string description = row["description"].ToString();
                    int qtyAvailable = int.Parse(row["qtyAvailable"].ToString());
                    int qtyRecorder = int.Parse(row["qtyReorder"].ToString());

                    SKUDetail skuDetail = new SKUDetail { Id = skuId, Name = skuName, Category = categoryName, Description = description, QuantityOnHand = qtyRecorder };

                    return skuDetail;
                }
            }
        }

        public dynamic LoadSkuInfoByName(string skuName)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataAdapter sda;
                    DataSet ds;

                    command.CommandText = @"SELECT categoryName, tblSKU.* FROM Categories INNER JOIN (SELECT id, sku, category, description, quantity, inventoryValue, qtyAvailable FROM SKU WHERE sku = @Value1) AS tblSKU ON tblSKU.category = Categories.id;";
                    command.Parameters.AddWithValue("@Value1", skuName);

                    command.ExecuteNonQuery();
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables[0].Rows[0];

                        skuName = row["sku"].ToString();
                        int skuId = int.Parse(row["id"].ToString());
                        int categoryId = int.Parse(row["category"].ToString());
                        string categoryName = row["categoryName"].ToString();
                        string description = row["description"].ToString();
                        int? qty = null;
                        if (row.IsNull("quantity"))
                            qty = int.Parse(row["quantity"].ToString());
                        int? qtyAvailable = null;
                        if (row.IsNull("qtyAvailable"))
                            qtyAvailable = int.Parse(row["qtyAvailable"].ToString());
                        int? cost = null;
                        if (row.IsNull("inventoryValue"))
                            cost = int.Parse(row["inventoryValue"].ToString());

                        var reader = command.ExecuteReader();

                        var skuInfo = new
                        {
                            skuId = skuId,
                            desc = description,
                            categoryName = categoryName,
                            categoryId = categoryId,
                            qty = qty,
                            cost = cost,
                            qtyAvailable = qtyAvailable
                        };

                        return skuInfo;
                    }
                    return null;
                }
            }
        }

        public dynamic LoadSkuInfoById(int skuId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataAdapter sda;
                    DataSet ds;

                    command.CommandText = @"SELECT categoryName, tblSKU.* FROM Categories INNER JOIN (SELECT id, sku, category, description, quantity, inventoryValue, qtyAvailable FROM SKU WHERE id = @Value1) AS tblSKU ON tblSKU.category = Categories.id;";
                    command.Parameters.AddWithValue("@Value1", skuId);

                    //command.ExecuteNonQuery();
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow row = ds.Tables[0].Rows[0];

                        string skuName = row["sku"].ToString();
                        //int skuId = int.Parse(row["id"].ToString());
                        int categoryId = int.Parse(row["category"].ToString());
                        string categoryName = row["categoryName"].ToString();
                        string description = row["description"].ToString();
                        int? qty = null;
                        if (!row.IsNull("quantity"))
                            qty = int.Parse(row["quantity"].ToString());
                        int? qtyAvailable = null;
                        if (!row.IsNull("qtyAvailable"))
                            qtyAvailable = int.Parse(row["qtyAvailable"].ToString());
                        int? cost = null;
                        if (row.IsNull("inventoryValue"))
                            cost = int.Parse(row["inventoryValue"].ToString());

                        var reader = command.ExecuteReader();

                        var skuInfo = new
                        {
                            skuId = skuId,
                            desc = description,
                            categoryName = categoryName,
                            categoryId = categoryId,
                            qty = qty,
                            cost = cost,
                            qtyAvailable = qtyAvailable
                        };

                        return skuInfo;
                    }
                    return null;
                }
            }
        }

        public List<KeyValuePair<int, string>> GetSKUNameList()
        {
            List<KeyValuePair<int, string>> PriceTierList = new List<KeyValuePair<int, string>>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataReader reader;

                    command.CommandText = @"select id, sku
                                            from dbo.SKU";
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        PriceTierList.Add(
                            new KeyValuePair<int, string>((int)reader[0], reader[1].ToString())
                        );
                    }
                    reader.Close();
                }
            }
            return PriceTierList;
        }
    }
}
