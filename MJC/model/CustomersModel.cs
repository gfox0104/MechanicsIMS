using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using MJC.config;
using Newtonsoft.Json;

namespace MJC.model
{
    public struct CustomerData
    {
        public int Id { get; set; }
        public string Num { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Phone { get; set; }
        public string QboId { get; set; }
        public string Email { get; set; }

        public CustomerData(int _id, string _num, string _name, string _address, string _city, string _state, string _phone, string _qboId, string _email)
        {
            Id = _id;
            Num = _num;
            Name = _name;
            Address = _address;
            City = _city;
            State = _state;
            Phone = _phone;
            QboId = _qboId;
            Email = _email;
        }
    }

    public class CustomersModel : DbConnection
    {
        public int NumCustomers { get; private set; }
        public List<CustomerData> CustomerDataList { get; private set; }

        public bool LoadCustomerData(string filter, bool archived)
        {
            CustomerDataList = new List<CustomerData>();

            using (var connection = GetConnection())
            {
                connection.Open();
                string whereClause = " WHERE 1=1";
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataReader reader;

                    if (archived) whereClause += " AND archived = 1";

                    command.CommandText = @"select id, customerNumber, displayName, address1, city, state, zipcode, qboId, email
                                            from dbo.Customers" + whereClause;

                    if (filter != "")
                    {
                        command.CommandText = @"select id, customerNumber, displayName, address1, city, state, zipcode, qboId, email
                                                from dbo.Customers
                                                where customerNumber like @filter or displayName like @filter or address1 or city like @filter or state like @filter or zipcode like @filter";
                        command.Parameters.Add("@filter", System.Data.SqlDbType.VarChar).Value = "%" + filter + "%";
                    }


                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        CustomerDataList.Add(
                            new CustomerData((int)reader[0], reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString())
                        );
                    }
                    reader.Close();
                }
            }

            return true;
        }

        public DataTable LoadCustomerTable()
        {
            DataTable dataTable = new DataTable();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.CommandText = "SELECT id, customerNumber, displayName, address1, city, state, zipcode FROM dbo.Customers";
                    command.Connection = connection;
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        public bool AddCustomer(string displayName, string givenName, string middleName, string familyName, string title, string suffix, string businessPhone, string homePhone, string fax, string address1, string address2, string city, string state, string zipCode, string email, DateTime date_opened, string salesman, bool resale, string stmtCustomerNumber, string stmtName, int? priceTierId, string terms, string limit, string memo, bool taxable, bool send_stm, string core_tracking, decimal? coreBalance, string acct_type, bool print_core_tot, bool porequired, int? creditCodeId, decimal? interestRate, decimal? accountBalance, int? ytdPurchases, decimal? ytdInterest, DateTime last_date_purch, string? qboId, string? syncToken, string customerNumber)
        {
            
            //MessageBox.Show(price)
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "INSERT INTO dbo.Customers (active, displayName, givenName, middleName, familyName, title, suffix, businessPhone, homePhone, fax, address1, address2, city, state, zipcode, email, dateOpened, salesman, resale, taxable, sendStatements, statementCustomerNumber, statementName, priceTierId, terms, limit, coreTracking, coreBalance, printCoreTotal, accountType, poRequired, creditCodeId, interestRate, accountBalance, yearToDatePurchases, yearToDateInterest, dateLastPurchased, memo, qboId, archived, createdAt, createdBy, updatedAt, updatedBy, syncToken, customerNumber) VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7, @Value8, @Value9, @Value10, @Value11, @Value12, @Value13, @Value14, @Value15, @Value16, @Value17, @Value18, @Value19, @Value20, @Value21, @Value22, @Value23, @Value24, @Value25, @Value26, @Value27, @Value28, @Value29, @Value30, @Value31, @Value32, @Value33, @Value34, @Value35, @Value36, @Value37, @Value38, @Value39, @Value40, @Value41, @Value42, @Value43, @Value44, @Value45, @Value46)";
                    command.Parameters.AddWithValue("@Value1", 1);
                    command.Parameters.AddWithValue("@Value2", displayName);
                    command.Parameters.AddWithValue("@Value3", givenName);
                    command.Parameters.AddWithValue("@Value4", middleName);
                    command.Parameters.AddWithValue("@Value5", familyName);
                    command.Parameters.AddWithValue("@Value6", title);
                    command.Parameters.AddWithValue("@Value7", suffix);
                    command.Parameters.AddWithValue("@Value8", businessPhone);
                    command.Parameters.AddWithValue("@Value9", homePhone);
                    command.Parameters.AddWithValue("@Value10", fax);
                    command.Parameters.AddWithValue("@Value11", address1);
                    command.Parameters.AddWithValue("@Value12", address2);
                    command.Parameters.AddWithValue("@Value13", city);
                    command.Parameters.AddWithValue("@Value14", state);
                    command.Parameters.AddWithValue("@Value15", zipCode);
                    command.Parameters.AddWithValue("@Value16", email);
                    command.Parameters.AddWithValue("@Value17", date_opened);
                    command.Parameters.AddWithValue("@Value18", salesman);
                    command.Parameters.AddWithValue("@Value19", resale);
                    command.Parameters.AddWithValue("@Value20", taxable);
                    command.Parameters.AddWithValue("@Value21", send_stm);
                    command.Parameters.AddWithValue("@Value22", stmtCustomerNumber);
                    command.Parameters.AddWithValue("@Value23", stmtName);
                    if(priceTierId != null)
                        command.Parameters.AddWithValue("@Value24", priceTierId);
                    else command.Parameters.AddWithValue("@Value24", DBNull.Value);
                    command.Parameters.AddWithValue("@Value25", terms);
                    command.Parameters.AddWithValue("@Value26", limit);
                    command.Parameters.AddWithValue("@Value27", core_tracking);
                    if(coreBalance != null)
                        command.Parameters.AddWithValue("@Value28", coreBalance);
                    else command.Parameters.AddWithValue("@Value28", DBNull.Value);
                    command.Parameters.AddWithValue("@Value29", print_core_tot);
                    command.Parameters.AddWithValue("@Value30", acct_type);
                    command.Parameters.AddWithValue("@Value31", porequired);
                    if(creditCodeId != null)
                        command.Parameters.AddWithValue("@Value32", creditCodeId);
                    else command.Parameters.AddWithValue("@Value32", DBNull.Value);
                    if(interestRate != null)
                        command.Parameters.AddWithValue("@Value33", interestRate);
                    else command.Parameters.AddWithValue("@Value33", DBNull.Value);
                    if (accountBalance != null)
                        command.Parameters.AddWithValue("@Value34", accountBalance);
                    else command.Parameters.AddWithValue("@Value34", DBNull.Value);
                    if(ytdPurchases != null)
                        command.Parameters.AddWithValue("@Value35", ytdPurchases);
                    else command.Parameters.AddWithValue("@Value35", DBNull.Value);
                    if(ytdInterest != null)
                        command.Parameters.AddWithValue("@Value36", ytdInterest);
                    else command.Parameters.AddWithValue("@Value36", DBNull.Value);
                    command.Parameters.AddWithValue("@Value37", last_date_purch);
                    command.Parameters.AddWithValue("@Value38", memo);
                    command.Parameters.AddWithValue("@Value39", qboId);
                    command.Parameters.AddWithValue("@Value40", 0);
                    command.Parameters.AddWithValue("@Value41", DateTime.Now);
                    command.Parameters.AddWithValue("@Value42", 1);
                    command.Parameters.AddWithValue("@Value43", DateTime.Now);
                    command.Parameters.AddWithValue("@Value44", 1);
                    command.Parameters.AddWithValue("@Value45", syncToken);
                    command.Parameters.AddWithValue("@Value46", customerNumber);

                    command.ExecuteNonQuery();

                }

                return true;
            }
        }

        public bool UpdateCustomer(string displayName, string givenName, string middleName, string familyName, string title, string suffix, string businessPhone, string homePhone, string fax, string address1, string address2, string city, string state, string zipCode, string email, DateTime date_opened, string salesman, bool resale, string stmtCustomerNumber, string stmtName, int? priceTierId, string terms, string limit, string memo, bool taxable, bool send_stm, string core_tracking, decimal? coreBalance, string acct_type, bool print_core_tot, bool porequired, int? creditCodeId, decimal? interestRate, decimal? accountBalance, int? ytdPurchases, decimal? ytdInterest, DateTime last_date_purch, string? qboId, string? syncToken, int customerId, string customerNumber)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE dbo.Customers SET active = @Value1, displayName = @Value2, givenName = @Value3, middleName = @Value4, familyName = @Value5, title = @Value6, suffix = @Value7, businessPhone = @Value8, homePhone = @Value9, fax = @Value10, address1 = @Value11, address2 = @Value12, city = @Value13, state = @Value14, zipcode = @Value15, email = @Value16, dateOpened = @Value17, salesman = @Value18, resale = @Value19, taxable = @Value20, sendStatements = @Value21, statementCustomerNumber = @Value22, statementName = @Value23, priceTierId = @Value24, terms = @Value25, limit = @Value26, coreTracking = @Value27, coreBalance = @Value28, printCoreTotal = @Value29, accountType = @Value30, poRequired = @Value31, creditCodeId = @Value32, interestRate = @Value33, accountBalance = @Value34, yearToDatePurchases = @Value35, yearToDateInterest = @Value36, dateLastPurchased = @Value37, memo = @Value38, qboId = @Value39, archived = @Value40, createdAt = @Value41, createdBy = @Value42, updatedAt = @Value43, updatedBy = @Value44, syncToken = @Value45, customerNumber = @Value46 WHERE id = @Value47";
                    command.Parameters.AddWithValue("@Value1", 1);
                    command.Parameters.AddWithValue("@Value2", displayName);
                    command.Parameters.AddWithValue("@Value3", givenName);
                    command.Parameters.AddWithValue("@Value4", middleName);
                    command.Parameters.AddWithValue("@Value5", familyName);
                    command.Parameters.AddWithValue("@Value6", title);
                    command.Parameters.AddWithValue("@Value7", suffix);
                    command.Parameters.AddWithValue("@Value8", businessPhone);
                    command.Parameters.AddWithValue("@Value9", homePhone);
                    command.Parameters.AddWithValue("@Value10", fax);
                    command.Parameters.AddWithValue("@Value11", address1);
                    command.Parameters.AddWithValue("@Value12", address2);
                    command.Parameters.AddWithValue("@Value13", city);
                    command.Parameters.AddWithValue("@Value14", state);
                    command.Parameters.AddWithValue("@Value15", zipCode);
                    command.Parameters.AddWithValue("@Value16", email);
                    command.Parameters.AddWithValue("@Value17", date_opened);
                    command.Parameters.AddWithValue("@Value18", salesman);
                    command.Parameters.AddWithValue("@Value19", resale);
                    command.Parameters.AddWithValue("@Value20", taxable);
                    command.Parameters.AddWithValue("@Value21", send_stm);
                    command.Parameters.AddWithValue("@Value22", stmtCustomerNumber);
                    command.Parameters.AddWithValue("@Value23", stmtName);
                    if (priceTierId != null)
                        command.Parameters.AddWithValue("@Value24", priceTierId);
                    else command.Parameters.AddWithValue("@Value24", DBNull.Value);
                    command.Parameters.AddWithValue("@Value25", terms);
                    command.Parameters.AddWithValue("@Value26", limit);
                    command.Parameters.AddWithValue("@Value27", core_tracking);
                    if (coreBalance != null)
                        command.Parameters.AddWithValue("@Value28", coreBalance);
                    else command.Parameters.AddWithValue("@Value28", DBNull.Value);
                    command.Parameters.AddWithValue("@Value29", print_core_tot);
                    command.Parameters.AddWithValue("@Value30", acct_type);
                    command.Parameters.AddWithValue("@Value31", porequired);
                    if (creditCodeId != null)
                        command.Parameters.AddWithValue("@Value32", creditCodeId);
                    else command.Parameters.AddWithValue("@Value32", DBNull.Value);
                    if (interestRate != null)
                        command.Parameters.AddWithValue("@Value33", interestRate);
                    else command.Parameters.AddWithValue("@Value33", DBNull.Value);
                    if (accountBalance != null)
                        command.Parameters.AddWithValue("@Value34", accountBalance);
                    else command.Parameters.AddWithValue("@Value34", DBNull.Value);
                    if (ytdPurchases != null)
                        command.Parameters.AddWithValue("@Value35", ytdPurchases);
                    else command.Parameters.AddWithValue("@Value35", DBNull.Value);
                    if (ytdInterest != null)
                        command.Parameters.AddWithValue("@Value36", ytdInterest);
                    else command.Parameters.AddWithValue("@Value36", DBNull.Value);
                    command.Parameters.AddWithValue("@Value37", last_date_purch);
                    command.Parameters.AddWithValue("@Value38", memo);
                    command.Parameters.AddWithValue("@Value39", qboId);
                    command.Parameters.AddWithValue("@Value40", 0);
                    command.Parameters.AddWithValue("@Value41", DateTime.Now);
                    command.Parameters.AddWithValue("@Value42", 1);
                    command.Parameters.AddWithValue("@Value43", DateTime.Now);
                    command.Parameters.AddWithValue("@Value44", 1);
                    command.Parameters.AddWithValue("@Value45", syncToken);
                    command.Parameters.AddWithValue("@Value46", customerNumber);
                    command.Parameters.AddWithValue("@Value47", customerId);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The Customer updated successfully.");
                }

                return true;
            }
        }

        public bool DeleteCustomer(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "DELETE FROM dbo.Customers WHERE id = @Value1";
                    command.Parameters.AddWithValue("@Value1", id);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The Customer was deleted.");
                }

                return true;
            }
        }

        public List<KeyValuePair<int, string>> GetCustomerNumberList()
        {
            List<KeyValuePair<int, string>> PriceTierList = new List<KeyValuePair<int, string>>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataReader reader;

                    command.CommandText = @"select id, customerNumber
                                            from dbo.Customers";
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

        public dynamic GetCustomerData(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"select id, customerNumber, address1, displayName, terms, poRequired, city, state, zipcode, accountBalance, yearToDatePurchases, yearToDateInterest from dbo.Customers where id = @id";
                    command.Parameters.AddWithValue("@id", id);

                    var reader = command.ExecuteReader();
                    if (reader.Read()) // check if there are any rows returned
                    {
                        // retrieve values from the reader
                        int customerId = (int)reader.GetValue(0);
                        string customerNumber = reader.IsDBNull(1) ? "" : reader.GetString(1);
                        string address1 = reader.IsDBNull(2) ? "" : reader.GetString(2);
                        string customerName = reader.IsDBNull(3) ? "" : reader.GetString(3);
                        string terms = reader.IsDBNull(4) ? "" : reader.GetString(4);
                        string poRequired = reader.IsDBNull(5) ? "" : reader.GetValue(5).ToString();
                        string city = reader.IsDBNull(6) ? "" : reader.GetString(6);
                        string state = reader.IsDBNull(7) ? "" : reader.GetString(7);
                        string zipcode = reader.IsDBNull(8) ? "" : reader.GetString(8);
                        string accountBalance = reader.IsDBNull(9) ? "" : reader.GetValue(9).ToString();
                        string yearToDatePurchases = reader.IsDBNull(10) ? "" : reader.GetValue(10).ToString();
                        string yearToDateInterest = reader.IsDBNull(11) ? "" : reader.GetValue(11).ToString();

                        // create an object to hold the customer data
                        var customer = new
                        {
                            id = customerId,
                            customerNumber = customerNumber,
                            customerName = customerName,
                            address1 = address1,
                            terms = terms,
                            poRequired = poRequired,
                            city = city,
                            state = state,
                            zipcode = zipcode,
                            accountBalance = accountBalance,
                            yearToDatePurchases = yearToDatePurchases,
                            yearToDateInterest = yearToDateInterest
                        };

                        return customer;
                    }

                    return null;
                }
            }
        }

        public bool UpdateCustomerArchived(bool archived, int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE dbo.Customers SET archived = @Value1 WHERE id = @Value2";
                    command.Parameters.AddWithValue("@Value1", archived);
                    command.Parameters.AddWithValue("@Value2", id);

                    command.ExecuteNonQuery();
                }

                return true;
            }
        }

        public dynamic GetCustomerDataById(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    SqlDataReader reader;
                    List<dynamic> returnList = new List<dynamic>();
                    command.Connection = connection;
                    command.CommandText = @"select * from dbo.Customers where id = @id";
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

                    // no rows returned
                    if (returnList.Count > 0)
                        return returnList[0];
                    else return null;
                }
            }
        }

        public bool GetCustomerArchived()
        {
            CustomerDataList = new List<CustomerData>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataReader reader;

                    command.CommandText = @"select id, customerNumber, customerName, address1, city, state, zipcode, email
                                            from dbo.Customers where archived = @Value1";
                    command.Parameters.AddWithValue("@Value1", 1);

                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        CustomerDataList.Add(
                            new CustomerData((int)reader[0], reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString(), reader[6].ToString(), reader[7].ToString(), reader[8].ToString())
                        );
                    }
                    reader.Close();
                }
            }

            return true;
        }

        public List<KeyValuePair<int, string>> GetCustomerNameList()
        {
            List<KeyValuePair<int, string>> CustomerNameList = new List<KeyValuePair<int, string>>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataReader reader;

                    command.CommandText = @"select id, displayName
                                            from dbo.Customers";
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        CustomerNameList.Add(
                            new KeyValuePair<int, string>((int)reader[0], reader[1].ToString())
                        );
                    }
                    reader.Close();
                }
            }
            return CustomerNameList;
        }
    }
}
