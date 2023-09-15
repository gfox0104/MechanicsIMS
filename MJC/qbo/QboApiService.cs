using System.Data.SqlClient;
using System.Text.Json;
using QboLib;
using QuickBooksSharp;
using QuickBooksSharp.Entities;
using MJC.config;
using System.Data;
using MJC.model;
using MJC.common;

namespace MJC.qbo
{
    public class QboApiService : DbConnection
    {
        private string accessToken { get; set; }

        private long realmId { get; set; }
        private CustomersModel customerModelObj = new CustomersModel();
        private AccountingModel accountingModelObj = new AccountingModel();
        private OrderModel orderModelObj = new OrderModel();
        private OrderItemsModel orderItemModelObj = new OrderItemsModel();
        private PaymentDetailModel PymtDetailModelObj = new PaymentDetailModel();
        private SKUModel skuModelObj = new SKUModel();

        public QboApiService()
        {
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string tokenFilePath = Path.Combine(directory, "MechanicsIMS", "Tokens.json");

            try
            {
                QboAuthTokens? Tokens = null;
                Tokens = System.Text.Json.JsonSerializer.Deserialize<QboAuthTokens>(File.ReadAllText(tokenFilePath), new JsonSerializerOptions()
                {
                    ReadCommentHandling = JsonCommentHandling.Skip
                }) ?? new();

                if (Session.SettingsModelObj.Settings.accessToken == null)
                {

                    this.accessToken = Tokens.AccessToken;
                    this.realmId = long.Parse(Tokens.RealmId);
                }
                else
                {
                    this.accessToken = Session.SettingsModelObj.Settings.accessToken;
                    // this.refreshToken = Session.SettingsModelObj.Settings.refreshToken;
                    this.realmId = long.Parse(Tokens.RealmId);
                }
            }
            catch(Exception e)
            {
                throw new Exception("TOKENS");
            }
        }

        public void RefreshToken()
        {
        }

        async public void CreateAccounting(string accountName, string acctNum, AccountTypeEnum acctType, string subAcctType)
        {
            DataService dataService = new DataService(this.accessToken, this.realmId, useSandbox: true);

            try
            {
                var result = await dataService.PostAsync(new Account
                {
                    Name = accountName,
                    AcctNum = acctNum,
                    AccountSubType = subAcctType,
                    AccountType = acctType
                });
            }
            catch (Exception exc)
            {
                Sentry.SentrySdk.CaptureException(exc);
                if (exc.Message.Contains("KEY"))
                {
                    Messages.ShowError("There was a problem updating the SKU.");
                }

                throw;
            }
        }

        async public void TestFunc()
        {
            DataService dataService = new DataService(this.accessToken, this.realmId, useSandbox: true);

            try
            {
                var result = await dataService.QueryAsync<Payment>("select * from Payment");
                Payment[] Accounts = result.Response.Entities;
            }
            catch (Exception exc)
            {
                Sentry.SentrySdk.CaptureException(exc);
                if (exc.Message.Contains("KEY"))
                {
                    Messages.ShowError("There was a problem updating the SKU.");
                }

                throw;
            }
        }
        
        async public void DeletePayment(string qboPaymentId, string syncToken, string customerId, string customerName, int paymentId)
        {
            DataService dataService = new DataService(this.accessToken, this.realmId, useSandbox: true);

            try
            {
                var result = await dataService.PostAsync(new Payment
                {
                    SyncToken = syncToken,
                    Id = qboPaymentId,
                    CustomerRef = new ReferenceType
                    {
                        value = customerId,
                        name = customerName
                    }
                });
            }
            catch (Exception exc)
            {
                Sentry.SentrySdk.CaptureException(exc);
                if (exc.Message.Contains("KEY"))
                {
                    Messages.ShowError("There was a problem updating the SKU.");
                }

                throw;
            }
        }

        async public Task<bool> CreatePayment(int customerId, string customerName, string qboCustomerId, DateTime dateReceived, double totalAmt, int orderId)
        {
            DataService dataService = new DataService(this.accessToken, this.realmId, useSandbox: true);

            try
            {
                var result = await dataService.PostAsync(new Payment
                {
                    TotalAmt = Convert.ToDecimal(totalAmt),
                    CustomerRef = new ReferenceType
                    {
                        value = qboCustomerId,
                        name = customerName
                    },
                    CurrencyRef = new ReferenceType
                    {
                        value = "USD",
                        name = "US Dollar"
                    }
                });

                Payment payment = result.Response;
                string qboPaymentId = payment.Id;
                string syncToken = payment.SyncToken;
                int paymentId = PymtDetailModelObj.CreatePayment(customerId, dateReceived, totalAmt, syncToken, qboPaymentId);
                int orderPaymentId = PymtDetailModelObj.CreateOrderPayment(orderId, paymentId, 1, 1);

                return true;
            }
            catch (Exception exc)
            {
                Sentry.SentrySdk.CaptureException(exc);
                if (exc.Message.Contains("KEY"))
                {
                    Messages.ShowError("There was a problem updating the SKU.");
                }

                throw;
            }
        }

        async public void CreateProduct()
        {
            DataService dataService = new DataService(this.accessToken, this.realmId, useSandbox: true);

            try
            {
                var result = await dataService.PostAsync(new Item
                {

                });
            }
            catch (Exception exc)
            {
                Sentry.SentrySdk.CaptureException(exc);
                if (exc.Message.Contains("KEY"))
                {
                    Messages.ShowError("There was a problem updating the SKU.");
                }

                throw;
            }
        }
        
        async public Task<bool> UpdateInvoice(CustomerData customer, List<OrderItem> itemList, dynamic order)
        {
            DataService dataService = new DataService(this.accessToken, this.realmId, useSandbox: true);

            try
            {
                int itemCount = itemList.Count;
                Line[] LineList = new Line[itemCount];

                int index = 0;
                foreach (OrderItem item in itemList)
                {
                    SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail { ItemRef = new ReferenceType { value = item.QboSkuId.ToString(), name = item.Sku }, Qty = item.Quantity, UnitPrice = Convert.ToDecimal(item.UnitPrice), TaxCodeRef = new ReferenceType { value = "Tax" } };

                    Line salesItemLine = new Line();
                    Line newSalesItemLine = new Line();
                    if (item.QboItemId != null)
                    {
                        salesItemLine = new Line
                        {
                            Id = item.QboItemId.ToString(),
                            Description = item.Description,
                            DetailType = LineDetailTypeEnum.SalesItemLineDetail,
                            SalesItemLineDetail = salesItemLineDetail,
                            LineNum = (uint)(index + 1),
                            Amount = item.Quantity * Convert.ToDecimal(item.UnitPrice),
                        };
                        LineList[index] = salesItemLine;
                    } 
                    //else
                    //{
                    //    newSalesItemLine = new Line
                    //    {
                    //        Description = item.Description,
                    //        DetailType = LineDetailTypeEnum.SalesItemLineDetail,
                    //        SalesItemLineDetail = salesItemLineDetail,
                    //        LineNum = (uint)(index + 1),
                    //        Amount = item.Quantity * Convert.ToDecimal(item.UnitPrice)
                    //    };
                    //    LineList[index] = newSalesItemLine;
                    //}
                    
                    index++;
                }

                string qboOrderId = order.qboOrderId;
                int orderId = order.orderId;
                string syncToken = order.syncToken;
                string qboCustomerId = customer.QboId;
                string customerName = customer.Name;
                string invoiceNumber = "";

                if (!DBNull.Value.Equals(order.invoiceNumber) || !Convert.IsDBNull(order.invoiceNumber))
                {
                    invoiceNumber = order.invoiceNumber;
                }
                var result = await dataService.PostAsync(new Invoice
                {
                    Id = qboOrderId.ToString(),
                    CustomerRef = new ReferenceType
                    {
                        value = qboCustomerId,
                        name = customerName
                    },
                    CurrencyRef = new ReferenceType
                    {
                        value = "USD",
                        name = "Dollar"
                    },
                    DocNumber = invoiceNumber,
                    Line = LineList,
                    SyncToken = syncToken,
                    sparse = true
                });

                Invoice invoice = result.Response;
                Line[] items = invoice.Line;

                var refreshData = orderModelObj.UpdateOrder(customer.Id, customer.Name, "", invoice.SyncToken, invoice.Id, 1, 1, orderId);
                index = 0;
                for(int i = 0; i< itemList.Count; i++)
                {
                    OrderItem item = itemList[i];
                    int skuId = item.SkuId;
                    int? qty = item.Quantity;
                    string description = item.Description;
                    bool? tax = item.Tax;
                    int? priceTier = item.PriceTier;
                    double? unitPrice = item.UnitPrice;
                    double? lineTotal = item.LineTotal;
                    string salesCode = item.SC;
                    string message = item.message;
                    string sku = item.Sku;
                    string tempSkuId = items[index].SalesItemLineDetail.ItemRef?.value ?? "";
                    int qboSkuId = int.Parse(tempSkuId);
                    string qboItemId = items[index].Id;
                    int lineNum = (int)items[index].LineNum;
                    int createdBy = 1;
                    int updatedBy = 1;
                    int orderItemId = item.Id;
                    index += 1;

                    if (orderItemId > 0)
                    {  
                        orderItemModelObj.UpdateOrderItem(skuId, qty, description, tax, priceTier, unitPrice, lineTotal, salesCode, sku, qboItemId, lineNum, createdBy, updatedBy, orderItemId);
                        orderItemModelObj.UpdateOrderItemMessageById(message, orderId);
                    } else
                    {
                        orderItemModelObj.CreateOrderItem(orderId, skuId, qty, description, message, tax, priceTier, unitPrice, lineTotal, salesCode, sku, qboSkuId, qboItemId, lineNum, createdBy, updatedBy);
                        orderItemModelObj.UpdateOrderItemMessageById(message, orderId);
                    }
                }

                return true;
            }
            catch (Exception exc)
            {
                Sentry.SentrySdk.CaptureException(exc);
                if (exc.Message.Contains("KEY"))
                {
                    Messages.ShowError("There was a problem updating the SKU.");
                }

                throw;
            }
        }

        async public Task<bool> CreateInvoice(CustomerData customer, string invoiceNumber, List<OrderItem> itemList)
        {
            DataService dataService = new DataService(this.accessToken, this.realmId, useSandbox: false);
            
            try
            {
                int itemCount = itemList.Count;
                Line[] LineList = new Line[itemCount];

                int index = 0;
                foreach (OrderItem item in itemList)
                {
                    var sku = item.Sku;
                    
                    string qboItemId = "0";
                    if (!string.IsNullOrEmpty(item.QboItemId))
                        qboItemId = item.QboItemId.ToString();


                    SalesItemLineDetail salesItemLineDetail = new SalesItemLineDetail { 
                        ItemRef = new ReferenceType { 
                            value = item.QboSkuId, 
                            name = item.Sku,
                            type = null
                        },
                        Qty = item.Quantity,  
                        UnitPrice = Convert.ToDecimal(item.UnitPrice), 
                        TaxCodeRef = new ReferenceType { value = "Tax" } };

                    //SubTotalLineDetail subTotalLineDetail = new SubTotalLineDetail { ServiceDate = DateTime.Now, ItemRef = new ReferenceType { name = "test_subTotalLine", value = "15" } };
                 
                    Line salesItemLine = new Line
                    {
                        Id = qboItemId,
                        Description = item.Description,
                        DetailType = LineDetailTypeEnum.SalesItemLineDetail,
                        SalesItemLineDetail = salesItemLineDetail,
                        LineNum = (uint)(index + 1),
                        Amount = item.Quantity * Convert.ToDecimal(item.UnitPrice)
                    };
                    LineList[index] = salesItemLine;
                    index++;
                }

                var result = await dataService.PostAsync(new Invoice
                {
                    CustomerRef = new ReferenceType
                    {
                        value = customer.QboId,
                        name = customer.Name
                    },
                    CurrencyRef = new ReferenceType
                    {
                        value = "USD",
                        name = "Dollar"
                    },
                    DocNumber = invoiceNumber,
                    BillEmail = new EmailAddress
                    {
                        Address = customer.Email
                    },
                    DueDate = DateTime.Now,
                    Line = LineList
                }) ;

                Invoice invoice = result.Response;
                DateTime invoiceDate = DateTime.Now;
                double invoiceTotal = Convert.ToDouble(invoice.Balance);
                string invoiceDesc = "";

                int orderId = orderModelObj.CreateOrder(customer.Id, customer.Name, "", invoiceNumber, invoiceDate, invoiceDesc, invoiceTotal, invoice.SyncToken, invoice.Id, 1, 1);
                
                Line[] items = invoice.Line;
                index = 0;

                int count = items.Length - 1;
                for (int i = 0; i < count; i++)
                {
                    Line item = items[i];
                    string qboItemId = item.Id;
                    decimal qty = item.SalesItemLineDetail?.Qty ?? 0;

                    string description = item.Description;
                    int? priceTier = null;
                    bool tax = false;
                    string? taxValue = item.SalesItemLineDetail?.TaxCodeRef?.value;
                    if(taxValue != null)
                    {
                        if (taxValue.Equals("TAX"))
                            tax = true;
                    }
                    double unitPrice = Convert.ToDouble(item.SalesItemLineDetail?.UnitPrice ?? 0);
                    double? lineTotal = null;
                    string salesCode = "";
                    //string? sku = item.SalesItemLineDetail?.ItemRef?.name;
                    string sku = itemList[i].Sku;
                    var message = itemList[i].message;
                    string tempSkuId = item.SalesItemLineDetail?.ItemRef?.value ?? "";
                    int qboSkuId = 0;
                    if (!string.IsNullOrEmpty(tempSkuId))
                        qboSkuId = int.Parse(tempSkuId);
                    int skuId = itemList[index].SkuId;
                    int orderItemId = itemList[index].Id;
                    int lineNum = (int)item.LineNum;
                    int createdBy = 1;
                    int updatedBy = 1;
                  
                    orderItemModelObj.CreateOrderItem(orderId, skuId, qty, description, message, tax, priceTier, unitPrice, lineTotal, salesCode, sku, qboSkuId, qboItemId, lineNum, createdBy, updatedBy);
                    //orderItemModelObj.UpdateOrderItemMessageById(message, orderId); 
                    
                    index++;
                }

                return true;
            }
            catch (Exception exc)
            {
                Sentry.SentrySdk.CaptureException(exc);

                throw;
            }
        }

        async public Task<bool> CreateCustomer(string displayName, string givenName, string middleName, string familyName, string title, string suffix, string business_phone, string homePhone, string fax, string address1, string address2, string city, string state, string zipCode, string email, DateTime date_opened, string salesman, bool resale, string stmtCustomerNumber, string stmtName, int? priceTierId, string terms, string limit, string memo, bool taxable, bool send_stm, string core_tracking, decimal? coreBalance, string acct_type, bool print_core_tot, bool porequired, int? creditCodeId, decimal? interestRate, decimal? accountBalance, int? ytdPurchases, decimal? ytdInterest, DateTime last_date_purch, string customerNumber)
        {
            DataService dataService = new DataService(this.accessToken, this.realmId, useSandbox: true);

            try
            {
                var result = await dataService.PostAsync(new Customer
                {
                    DisplayName = displayName,
                    Suffix = suffix,
                    Title = title,
                    MiddleName = middleName,
                    FamilyName = familyName,
                    GivenName = givenName,
                    BillAddr = new PhysicalAddress
                    {
                        Line1 = address1,
                        Line2 = address2,
                        City = city,
                        County = state,
                        PostalCode = zipCode,
                    },
                    BusinessNumber = business_phone,
                    //CompanyName = "rihno docs",
                    Balance = accountBalance,
                });

                Customer customer = result.Response;
                string? qboId = "";
                string? syncToken = "";
                if (customer != null)
                {
                    qboId = customer.Id;
                    syncToken = customer.SyncToken;
                }
                return customerModelObj.AddCustomer(displayName, givenName, middleName, familyName, title, suffix, business_phone, homePhone, fax, address1, address2, city, state, zipCode, email, date_opened, salesman, resale, stmtCustomerNumber, stmtName, priceTierId, terms, limit, memo, taxable, send_stm, core_tracking, coreBalance, acct_type, print_core_tot, porequired, creditCodeId, interestRate, accountBalance, ytdPurchases, ytdInterest, last_date_purch, qboId, syncToken, customerNumber);
            }
            catch (Exception exc)
            {
                Sentry.SentrySdk.CaptureException(exc);
                if (exc.Message.Contains("KEY"))
                {
                    Messages.ShowError("There was a problem updating the SKU.");
                }

                throw;
            }

            return false;
        }

        async public void UpdateCustomer(string displayName, string givenName, string middleName, string familyName, string title, string suffix, string business_phone, string homePhone, string fax, string address1, string address2, string city, string state, string zipCode, string email, DateTime date_opened, string salesman, bool resale, string stmtCustomerNumber, string stmtName, int? priceTierId, string terms, string limit, string memo, bool taxable, bool send_stm, string core_tracking, decimal? coreBalance, string acct_type, bool print_core_tot, bool porequired, int? creditCodeId, decimal? interestRate, decimal? accountBalance, int? ytdPurchases, decimal? ytdInterest, DateTime last_date_purch, string qboId, string syncToken, int customerId, string customerNumber)
        {
            DataService dataService = new DataService(this.accessToken, this.realmId, useSandbox: true);
            string m_qboId = qboId;
            string m_syncToken = syncToken;
            try
            {
                var result = await dataService.PostAsync(new Customer
                {
                    Id = m_qboId,
                    DisplayName = displayName,
                    Suffix = suffix,
                    Title = title,
                    MiddleName = middleName,
                    FamilyName = familyName,
                    GivenName = givenName,
                    BillAddr = new PhysicalAddress
                    {
                        Line1 = address1,
                        Line2 = address2,
                        City = city,
                        County = state,
                        PostalCode = zipCode,
                    },
                    BusinessNumber = business_phone,
                    //CompanyName = "rihno docs",
                    Balance = accountBalance,
                    SyncToken = m_syncToken,
                    sparse = true
                });

                Customer customer = result.Response;
         
                if (customer != null)
                {
                    qboId = customer.Id;
                    syncToken = customer.SyncToken;
                }
                customerModelObj.UpdateCustomer(displayName, givenName, middleName, familyName, title, suffix, business_phone, homePhone, fax, address1, address2, city, state, zipCode, email, date_opened, salesman, resale, stmtCustomerNumber, stmtName, priceTierId, terms, limit, memo, taxable, send_stm, core_tracking, coreBalance, acct_type, print_core_tot, porequired, creditCodeId, interestRate, accountBalance, ytdPurchases, ytdInterest, last_date_purch, qboId, syncToken, customerId, customerNumber);
            }
            catch (Exception exc)
            {
                Sentry.SentrySdk.CaptureException(exc);
                if (exc.Message.Contains("KEY"))
                {
                    Messages.ShowError("There was a problem updating the SKU.");
                }

                throw;
            }
        }

        async public Task LoadCustomers()
        {
            DataService dataService= new DataService(this.accessToken, this.realmId, useSandbox: false);

            try
            {
                //var result = await dataService.QueryAsync<Customer>("select * from Customer");
                //var customers = result.Response.Entities;
                
                //if(customers != null)
                //{
                //    foreach (var customer in customers)
                //    {
                //        if (await DoesCustomerExist(customer))
                //        {
                //            UpdateCustomer(customer);
                //        }
                //        else
                //        {
                //            CreateNewCustomer(customer);
                //        }

                //    }
                //}

                Console.WriteLine("Customer is synchorized");
                //LoadInvoices();
                for (int i = 0; i < 94; i++)
                {
                    LoadSKU(i);
                }

            }
            catch (Exception exc)
            {
                Sentry.SentrySdk.CaptureException(exc);
                if (exc.Message.Contains("KEY"))
                {
                    Messages.ShowError("There was a problem updating the SKU.");
                }

                throw;
            }
        }

        async private Task<bool> DoesCustomerExist(Customer customer)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT * FROM dbo.Customers WHERE qboId=@qboId";
                    command.Parameters.AddWithValue("@qboId", customer.Id);

                    var row = command.ExecuteScalar();
                    if (row != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        async private void CreateNewCustomer(Customer customer)
        {
            bool? active = customer.Active;
            string customerNumber = customer.DisplayName;
            string customerName = "";
            if (!string.IsNullOrEmpty(customer.CompanyName))
                customerName = customer.CompanyName;
            else customerName = customer.FullyQualifiedName;
            string? address1 = null;
            string? address2 = null;
            string? city = null;
            string? state = null;
            string? zipCode = null;
            if (customer.BillAddr != null)
            {
                address1 = customer.BillAddr.Line1;
                address2 = customer.BillAddr.Line2;
                city = customer.BillAddr.City;
                state = customer.BillAddr.CountrySubDivisionCode;
                zipCode = customer.BillAddr.PostalCode;
            }

            string businessPhone = customer.BusinessNumber;
            string? fax = null;
            if (customer.Fax != null)
                fax = customer.Fax.FreeFormNumber;
            string? homePhone = null;
            if (customer.PrimaryPhone != null)
                homePhone = customer.PrimaryPhone.FreeFormNumber;
            string? email = null;
            if (customer.PrimaryEmailAddr != null)
                email = customer.PrimaryEmailAddr.Address;
            DateTime dateOpened = DateTime.Now;
            decimal? balance = customer.Balance;
            DateTimeOffset? createdAt = customer.MetaData.CreateTime;
            int createdBy = 1; // TODO: Remove hard coding
            //if (customer.MetaData.CreatedByRef != null)
            //    createdBy = int.Parse(customer.MetaData.CreatedByRef.value);
            DateTimeOffset? updatedAt = customer.MetaData.LastUpdatedTime;
            int updatedBy = 1;
            //if (customer.MetaData.CreatedByRef != null)
            //    updatedBy = int.Parse(customer.MetaData.LastModifiedByRef.value);

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"INSERT INTO dbo.Customers(active, customerNumber, displayName, title, address1, address2, city, state, zipcode, businessPhone, fax, homePhone, email, dateOpened, accountBalance, createdAt, createdBy, updatedAt, updatedBy, qboId) OUTPUT INSERTED.id VALUES(@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7, @Value8, @Value9, @Value10, @Value11, @Value12, @Value13, @Value14, @Value15, @Value16, @Value17, @Value18, @Value19, @qboId)";

                    command.Parameters.AddWithValue("@Value1", active);
                    if (customerNumber != null)
                        command.Parameters.AddWithValue("@Value2", customerNumber);
                    else command.Parameters.AddWithValue("@Value2", DBNull.Value);
                    if (customerName != null)
                        command.Parameters.AddWithValue("@Value3", customerName);
                    else command.Parameters.AddWithValue("@Value3", DBNull.Value);

                    if (customerName != null)
                        command.Parameters.AddWithValue("@Value4", string.IsNullOrEmpty(customer.Title) ? "" : customer.Title);
                    else command.Parameters.AddWithValue("@Value4", DBNull.Value);

                    if (address1 != null)
                        command.Parameters.AddWithValue("@Value5", address1);
                    else command.Parameters.AddWithValue("@Value5", DBNull.Value);
                    if (address2 != null)
                        command.Parameters.AddWithValue("@Value6", address2);
                    else command.Parameters.AddWithValue("@Value6", DBNull.Value);
                    if (city != null)
                        command.Parameters.AddWithValue("@Value7", city);
                    else command.Parameters.AddWithValue("@Value7", DBNull.Value);
                    if (state != null)
                        command.Parameters.AddWithValue("@Value8", state);
                    else command.Parameters.AddWithValue("@Value8", DBNull.Value);
                    if (zipCode != null)
                        command.Parameters.AddWithValue("@Value9", zipCode);
                    else command.Parameters.AddWithValue("@Value9", DBNull.Value);
                    if (businessPhone != null)
                        command.Parameters.AddWithValue("@Value10", businessPhone);
                    else command.Parameters.AddWithValue("@Value10", DBNull.Value);
                    if (fax != null)
                        command.Parameters.AddWithValue("@Value11", fax);
                    else command.Parameters.AddWithValue("@Value11", DBNull.Value);
                    if (homePhone != null)
                        command.Parameters.AddWithValue("@Value12", homePhone);
                    else command.Parameters.AddWithValue("@Value12", DBNull.Value);
                    if (email != null)
                        command.Parameters.AddWithValue("@Value13", email);
                    else command.Parameters.AddWithValue("@Value13", DBNull.Value);
                    if (dateOpened != null)
                        command.Parameters.AddWithValue("@Value14", dateOpened);
                    else command.Parameters.AddWithValue("@Value14", DBNull.Value);
                    if (balance != null)
                        command.Parameters.AddWithValue("@Value15", balance);
                    else command.Parameters.AddWithValue("@Value15", DBNull.Value);
                    command.Parameters.AddWithValue("@Value16", createdAt);
                    command.Parameters.AddWithValue("@Value17", createdBy);
                    command.Parameters.AddWithValue("@Value18", updatedAt);
                    command.Parameters.AddWithValue("@Value19", updatedBy);
                    command.Parameters.AddWithValue("@qboId", customer.Id);

                    command.ExecuteScalar();
                }
            }
        }


        async private void UpdateCustomer(Customer customer)
        {
            bool? active = customer.Active;
            string customerNumber = customer.DisplayName;
            string customerName = "";
            if (!string.IsNullOrEmpty(customer.CompanyName))
                customerName = customer.CompanyName;
            else customerName = customer.FullyQualifiedName;
            string? address1 = null;
            string? address2 = null;
            string? city = null;
            string? state = null;
            string? zipCode = null;
            if (customer.BillAddr != null)
            {
                address1 = customer.BillAddr.Line1;
                address2 = customer.BillAddr.Line2;
                city = customer.BillAddr.City;
                state = customer.BillAddr.CountrySubDivisionCode;
                zipCode = customer.BillAddr.PostalCode;
            }

            string businessPhone = customer.BusinessNumber;
            string? fax = null;
            if (customer.Fax != null)
                fax = customer.Fax.FreeFormNumber;
            string? homePhone = null;
            if (customer.PrimaryPhone != null)
                homePhone = customer.PrimaryPhone.FreeFormNumber;
            string? email = null;
            if (customer.PrimaryEmailAddr != null)
                email = customer.PrimaryEmailAddr.Address;
            DateTime dateOpened = DateTime.Now;
            decimal? balance = customer.Balance;
            DateTimeOffset? createdAt = customer.MetaData.CreateTime;
            int createdBy = 2;
            //if (customer.MetaData.CreatedByRef != null)
            //    createdBy = int.Parse(customer.MetaData.CreatedByRef.value);
            DateTimeOffset? updatedAt = customer.MetaData.LastUpdatedTime;
            int updatedBy = 2;
            //if (customer.MetaData.CreatedByRef != null)
            //    updatedBy = int.Parse(customer.MetaData.LastModifiedByRef.value);

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE dbo.Customers SET active=@Value1, customerNumber=@Value2, displayName=@Value3, title=@Value4, address1=@Value5, address2=@Value6, city=@Value7, state=@Value8, zipcode=@Value9, businessPhone=@Value10, fax=@Value11, homePhone=@Value12, email=@Value13, dateOpened=@Value14, accountBalance=@Value15, createdAt=@Value16, createdBy=@Value17, updatedAt=@Value18, updatedBy=@Value19  WHERE qboid=@Value20";

                    command.Parameters.AddWithValue("@Value1", active);
                    if (customerNumber != null)
                        command.Parameters.AddWithValue("@Value2", customerNumber);
                    else command.Parameters.AddWithValue("@Value2", DBNull.Value);
                    if (customerName != null)
                        command.Parameters.AddWithValue("@Value3", customerName);
                    else command.Parameters.AddWithValue("@Value3", DBNull.Value);

                    if (customerName != null)
                        command.Parameters.AddWithValue("@Value4", string.IsNullOrEmpty(customer.Title) ? "" : customer.Title);
                    else command.Parameters.AddWithValue("@Value4", DBNull.Value);

                    if (address1 != null)
                        command.Parameters.AddWithValue("@Value5", address1);
                    else command.Parameters.AddWithValue("@Value5", DBNull.Value);
                    if (address2 != null)
                        command.Parameters.AddWithValue("@Value6", address2);
                    else command.Parameters.AddWithValue("@Value6", DBNull.Value);
                    if (city != null)
                        command.Parameters.AddWithValue("@Value7", city);
                    else command.Parameters.AddWithValue("@Value7", DBNull.Value);
                    if (state != null)
                        command.Parameters.AddWithValue("@Value8", state);
                    else command.Parameters.AddWithValue("@Value8", DBNull.Value);
                    if (zipCode != null)
                        command.Parameters.AddWithValue("@Value9", zipCode);
                    else command.Parameters.AddWithValue("@Value9", DBNull.Value);
                    if (businessPhone != null)
                        command.Parameters.AddWithValue("@Value10", businessPhone);
                    else command.Parameters.AddWithValue("@Value10", DBNull.Value);
                    if (fax != null)
                        command.Parameters.AddWithValue("@Value11", fax);
                    else command.Parameters.AddWithValue("@Value11", DBNull.Value);
                    if (homePhone != null)
                        command.Parameters.AddWithValue("@Value12", homePhone);
                    else command.Parameters.AddWithValue("@Value12", DBNull.Value);
                    if (email != null)
                        command.Parameters.AddWithValue("@Value13", email);
                    else command.Parameters.AddWithValue("@Value13", DBNull.Value);
                    if (dateOpened != null)
                        command.Parameters.AddWithValue("@Value14", dateOpened);
                    else command.Parameters.AddWithValue("@Value14", DBNull.Value);
                    if (balance != null)
                        command.Parameters.AddWithValue("@Value15", balance);
                    else command.Parameters.AddWithValue("@Value15", DBNull.Value);
                    command.Parameters.AddWithValue("@Value16", createdAt);
                    command.Parameters.AddWithValue("@Value17", createdBy);
                    command.Parameters.AddWithValue("@Value18", updatedAt);
                    command.Parameters.AddWithValue("@Value19", updatedBy);
                    command.Parameters.AddWithValue("@Value20", customer.Id);

                    command.ExecuteScalar();
                }
            }
        }

        async public void LoadInvoices(int skuId = 0, int customerId1 = 0, double unitPrice = 0, int qty = 1)
        {
            DataService dataService = new DataService(this.accessToken, this.realmId, useSandbox: false);
            try
            {
                var result = await dataService.QueryAsync<Invoice>("select * from Invoice");
                var invoices = result.Response.Entities;
                if(invoices != null)
                {
                    foreach (var invoice in invoices)
                    {
                        string customerId = invoice.CustomerRef.value;
                        string customerName = invoice.CustomerRef.name;
                        string creditCode = "";
                        string terms = invoice.SalesTermRef?.value;
                        string zone = "";
                        string? poNumber = invoice.PONumber;
                        string? shipVia = null;
                        if (invoice.ShipMethodRef != null)
                            shipVia = invoice.ShipMethodRef.value;
                        string fob = invoice.FOB;
                        string salesman = "";
                        DateTime shipTo = DateTime.UtcNow;
                        var status = invoice.status;
                        string processedBy = "";
                        DateTime? dateShipped = invoice.ShipDate;
                        string invoiceNumber = invoice.DocNumber;
                        DateTime? invoiceDate = invoice.TxnDate;
                        string invoiceDesc = "";
                        DateTime? lastPayment = invoice.DueDate;
                        DateTime? paidDate = DateTime.UtcNow;
                        double interestRate = 0;
                        bool interestApplied = false;
                        double discountAllowed = 0;
                        decimal? invoiceTotal = invoice.TotalAmt;
                        double discountableAmount = 0;
                        double totalPaid = 0;
                        decimal? invoiceBalance = invoice.Balance;
                        bool trainingMode = false;
                        DateTimeOffset? createdAt = invoice.MetaData.CreateTime;
                        int createdBy = 2;
                        if (invoice.MetaData.CreatedByRef != null)
                            createdBy = int.Parse(invoice.MetaData.CreatedByRef.value);
                        DateTimeOffset? updatedAt = invoice.MetaData.LastUpdatedTime;
                        int updatedBy = 2;
                        if (invoice.MetaData.CreatedByRef != null)
                            updatedBy = int.Parse(invoice.MetaData.LastModifiedByRef.value);

                        using (var connection = GetConnection())
                        {
                            connection.Open();

                            using (var command = new SqlCommand())
                            {
                                command.Connection = connection;
                                command.CommandText = @"INSERT INTO dbo.Orders(active, customerId, customerName, creditCode, terms, zone, poNumber, shipVia, fob, salesman, shipTo, status, processedBy, dateShipped, invoiceNumber, invoiceDate, invoiceDesc, lastPayment, paidDate, interestRate, interestApplied, discountAllowed, invoiceTotal, discountableAmount, totalPaid, invoiceBalance, trainingMode, createdAt, createdBy, updatedAt, updatedBy) OUTPUT INSERTED.id VALUES(@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7, @Value8, @Value9, @Value10, @Value11, @Value12, @Value13, @Value14, @Value15, @Value16, @Value17, @Value18, @Value19, @Value20, @Value21, @Value22, @Value23, @Value24, @Value25, @Value26, @Value27, @Value28, @Value29, @Value30, @Value31)";
                                command.Parameters.AddWithValue("@Value1", 1);
                                command.Parameters.AddWithValue("@Value2", customerId);
                                command.Parameters.AddWithValue("@Value3", customerName);
                                command.Parameters.AddWithValue("@Value4", creditCode);
                                command.Parameters.AddWithValue("@Value5", terms ?? "No default terms");
                                command.Parameters.AddWithValue("@Value6", zone);
                                if (poNumber != null)
                                    command.Parameters.AddWithValue("@Value7", poNumber);
                                else command.Parameters.AddWithValue("@Value7", DBNull.Value);
                                if (shipVia != null)
                                    command.Parameters.AddWithValue("@Value8", shipVia);
                                else command.Parameters.AddWithValue("@Value8", DBNull.Value);
                                if (fob != null)
                                    command.Parameters.AddWithValue("@Value9", fob);
                                else command.Parameters.AddWithValue("@Value9", DBNull.Value);
                                if (salesman != null)
                                    command.Parameters.AddWithValue("@Value10", salesman);
                                else command.Parameters.AddWithValue("@Value10", DBNull.Value);
                                if (shipTo != null)
                                    command.Parameters.AddWithValue("@Value11", shipTo);
                                else command.Parameters.AddWithValue("@Value11", DBNull.Value);
                                if (status != null)
                                    command.Parameters.AddWithValue("@Value12", status);
                                else command.Parameters.AddWithValue("@Value12", DBNull.Value);
                                if (processedBy != null)
                                    command.Parameters.AddWithValue("@Value13", processedBy);
                                else command.Parameters.AddWithValue("@Value13", DBNull.Value);
                                if (dateShipped != null)
                                    command.Parameters.AddWithValue("@Value14", dateShipped);
                                else command.Parameters.AddWithValue("@Value14", DBNull.Value);

                                command.Parameters.AddWithValue("@Value15", invoiceNumber);
                                command.Parameters.AddWithValue("@Value16", invoiceDate);
                                command.Parameters.AddWithValue("@Value17", invoiceDesc);
                                command.Parameters.AddWithValue("@Value18", lastPayment);
                                command.Parameters.AddWithValue("@Value19", paidDate);
                                command.Parameters.AddWithValue("@Value20", interestRate);
                                command.Parameters.AddWithValue("@Value21", interestApplied);
                                command.Parameters.AddWithValue("@Value22", discountAllowed);
                                command.Parameters.AddWithValue("@Value23", invoiceTotal);
                                command.Parameters.AddWithValue("@Value24", discountableAmount);
                                command.Parameters.AddWithValue("@Value25", totalPaid);
                                command.Parameters.AddWithValue("@Value26", invoiceBalance);
                                command.Parameters.AddWithValue("@Value27", trainingMode);
                                command.Parameters.AddWithValue("@Value28", createdAt);
                                command.Parameters.AddWithValue("@Value29", createdBy);
                                command.Parameters.AddWithValue("@Value30", updatedAt);
                                command.Parameters.AddWithValue("@Value31", updatedBy);

                                command.ExecuteScalar();
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Sentry.SentrySdk.CaptureException(exc);
                if (exc.Message.Contains("KEY"))
                {
                    Messages.ShowError("There was a problem updating the SKU.");
                }

                throw;
            }

        }

        async public void LoadSKU(int index = 0)
        {
            DataService dataService = new DataService(this.accessToken, this.realmId, useSandbox: false);
            try
            {
                int startPosition = index * 100;
                string query = "select * from Item ORDERBY Id StartPosition " + startPosition + " MaxResults 100";
                var result = await dataService.QueryAsync<Item>(query);
                var items = result.Response.Entities;

                if(items != null)
                {
                    foreach (var item in items)
                    {
                        string itemId = item.Id;
                        string skuName = item.Name;
                        int category = 1;
                        string desc = item.Description;
                        string measurementUnit = "ea";
                        int weight = 0;
                        int costCode = 1;
                        int assetAccount = 1;
                        if (item.AssetAccountRef?.value != null)
                            assetAccount = int.Parse(item.AssetAccountRef.value);
                        bool taxable = item.Taxable ?? false;
                        bool maintain_qty = item.TrackQtyOnHand ?? false;
                        bool allow_discount = false;
                        bool commissionable = false;
                        int order_from = 1;
                        DateTime? last_sold = null;
                        string manufacturer = "";
                        string? location = null;
                        int quantity = Convert.ToInt32(item.QtyOnHand);
                        int qty_allocated = Convert.ToInt32(item.QtyOnSalesOrder);
                        int qty_available = Convert.ToInt32(item.QtyOnHand - item.QtyOnSalesOrder);
                        int critical_qty = 0;
                        int reorder_qty = Convert.ToInt32(item.QtyOnPurchaseOrder);
                        int sold_this_month = 12;
                        int sold_ytd = 2022;
                        bool freeze_prices = false;
                        double core_cost = Convert.ToDouble(item.UnitPrice);
                        double inv_value = Convert.ToDouble(item.PurchaseCost);
                        string? memo = null;
                        Dictionary<int, double> priceTierDict = new Dictionary<int, double>();
                        bool hidden = false;
                        bool billAslabor = false;
                        string? syncToken = item.SyncToken;
                        foreach(var priceTier in Session.PriceTiersModelObj.PriceTierDataList)
                        {
                            priceTierDict.Add(priceTier.Id, priceTier.ProfitMargin);
                        }

                        skuModelObj.AddSKU(skuName, category, desc, measurementUnit, weight, costCode, assetAccount, taxable, maintain_qty, allow_discount, commissionable, order_from, last_sold, manufacturer, location, quantity, qty_allocated, qty_available, critical_qty, reorder_qty, sold_this_month, sold_ytd, freeze_prices, core_cost, inv_value, memo, priceTierDict, billAslabor, syncToken, itemId, hidden, false);
                    }
                }
               
                //MessageBox.Show("Load QB data is finished");
            }
            catch (Exception exc)
            {
                Sentry.SentrySdk.CaptureException(exc);
                if (exc.Message.Contains("KEY"))
                {
                    Messages.ShowError("There was a problem updating the SKU.");
                }

                throw;
            }

        }

        public SqlCommand RunQuryNoParameters(string query)
        {
            using (var connection = GetConnection())
            {
                SqlCommand command = new SqlCommand();
                try
                {
                    connection.Open();
                    if (connection != null)
                    {
                        command = connection.CreateCommand();
                        command.CommandType = CommandType.Text;
                        command.CommandText = query;
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception exc)
                {
                    Sentry.SentrySdk.CaptureException(exc);
                    if (exc.Message.Contains("KEY"))
                    {
                        Messages.ShowError("There was a problem updating the SKU.");
                    }

                 
                    connection.Close();
                }
                return command;
            }
        }

        public void InitDatabase()
        {
            string query = "";
            query = "DELETE FROM CustomerCreditCards";
            RunQuryNoParameters(query);
            query = "DELETE FROM customerpriceLevels";
            RunQuryNoParameters(query);
            query = "DELETE FROM customershipTos";
            RunQuryNoParameters(query);
            query = "DELETE FROM OrderItems";
            RunQuryNoParameters(query);
            query = "DELETE FROM Orders";
            RunQuryNoParameters(query);
            query = "DELETE FROM Customers";
            RunQuryNoParameters(query);
        }
    }
}


