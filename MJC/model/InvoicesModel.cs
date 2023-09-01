using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using MJC.config;

namespace MJC.model
{
    public struct InvoiceData
    {
        public int id { get; set; }
        public int customerId { get; set; }
        public string invoiceNumber { get; set; }
        public DateTime? date { get; set; }
        public string description { get; set; }
        public double? total { get; set; }
        public double? balance { get; set; }

        public InvoiceData(int _id, int _customerId, string _invoiceNumber, DateTime? _date, string _description, double? _total, double? _balance)
        {
            id = _id;
            customerId = _customerId;
            invoiceNumber = _invoiceNumber;
            date = _date;
            description = _description;
            total = _total;
            balance = _balance;
        }
    }

    public struct HistoricalInvoiceData
    {
        public int id { get; set; }
        public int customerId { get; set; }
        public string invoiceNumber { get; set; }
        public DateTime? date { get; set; }
        public DateTime? datePaid { get; set; }
        public int? days { get; set; }
        public string description { get; set; }

        public HistoricalInvoiceData(int _id, int _customerId, string _invoiceNumber, DateTime? _date, DateTime? _datePaid, int? _days, string _description)
        {
            id = _id;
            customerId = _customerId;
            invoiceNumber = _invoiceNumber;
            date = _date;
            datePaid = _datePaid;
            days = _days;
            description = _description;
        }
    }

    public struct InvoiceAgingData
    {
        public string agingName { get; set; }
        public int agingPeriod { get; set; }

        public InvoiceAgingData(string _agingName, int _agingPeriod)
        {
            agingName = _agingName;
            agingPeriod = _agingPeriod;
        }
    }

    public class InvoicesModel : DbConnection
    {
        public List<HistoricalInvoiceData> HistoricalInvoiceDataList { get; private set; }

        public List<InvoiceData> LoadInvoiceData(int customerId = 0, int status = 0)
        {
            List<InvoiceData> InvoiceDataList = new List<InvoiceData>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataReader reader;
                    command.CommandText = @"SELECT id, customerId, invoiceNumber, invoiceDate, invoiceDesc, invoiceTotal, invoiceBalance from Orders";
                    if (customerId != 0)
                    {
                        command.CommandText = @"SELECT id, customerId, invoiceNumber, invoiceDate, invoiceDesc, invoiceTotal, invoiceBalance from Orders WHERE customerId = @Value1";

                        command.Parameters.AddWithValue("@Value1", customerId);
                    }

                    if (status != 0)
                    {
                        command.CommandText = @"SELECT id, customerId, invoiceNumber, invoiceDate, invoiceDesc, invoiceTotal, invoiceBalance from Orders WHERE status = @Value1";

                        command.Parameters.AddWithValue("@Value1", customerId);
                    }

                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = (int)reader[0];
                        //int newCustomerid = (int)reader[1];
                        string invoiceNumber = reader[2].ToString();
                        DateTime? date = null;
                        if (!reader.IsDBNull(3))
                            date = DateTime.Parse(reader[3].ToString());
                        string desc = reader[4].ToString();
                        double? total = null;
                        if (!reader.IsDBNull(5))
                            total = double.Parse(reader[5].ToString());
                        double? balance = null;
                        if (!reader.IsDBNull(6))
                            balance = double.Parse(reader[6].ToString());

                        InvoiceDataList.Add(
                            new InvoiceData(id, customerId, invoiceNumber, date, desc, total, balance)
                        );
                    }
                    reader.Close();
                }
            }

            return InvoiceDataList;
        }

        public bool LoadHistoricalInvoiceData(int? customerId = null)
        {
            HistoricalInvoiceDataList = new List<HistoricalInvoiceData>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataReader reader;
                    command.CommandText = @"SELECT id, customerId, invoiceNumber, invoiceDate, paidDate, invoiceDesc from Orders";
                    if (customerId != null)
                    {
                        command.CommandText = @"SELECT id, customerId, invoiceNumber, invoiceDate, paidDate, invoiceDesc from Orders WHERE customerId = @Value1 AND status = @Value2";

                        command.Parameters.AddWithValue("@Value1", customerId);
                        command.Parameters.AddWithValue("@Value2", 4);
                    }

                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = (int)reader[0];
                        int newCustomerId = (int)reader[1];
                        string invoiceNumber = reader[2].ToString();
                        DateTime.TryParse(reader[3].ToString(), out DateTime date);
                        DateTime.TryParse(reader[4].ToString(), out DateTime paidDate);
                        TimeSpan difference = paidDate.Subtract(date);
                        int differenceInDays = difference.Days;
                        string desc = reader[5].ToString();

                        HistoricalInvoiceDataList.Add(
                            new HistoricalInvoiceData(id, newCustomerId, invoiceNumber, date, paidDate, differenceInDays, desc)
                        );
                    }
                    reader.Close();
                }
            }

            return true;
        }

        public dynamic GetInvoiceDataById(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    SqlDataReader reader;
                    List<dynamic> returnList = new List<dynamic>();
                    command.Connection = connection;
                    command.CommandText = @"SELECT coreCharge, tblOrder.* FROM OrderItems RIGHT JOIN (SELECT customerNumber, displayName, address1, city, state, zipcode, businessPhone, tblOrder.* FROM Customers INNER JOIN (SELECT id as orderId, invoiceNumber, customerId, invoiceDate, invoiceDesc, terms, paidDate, lastPayment, interestRate, interestApplied, discountAllowed, dateShipped, poNumber, shipVia, fob, salesman, invoiceTotal, discountableAmount, totalPaid, invoiceBalance from Orders WHERE id = @Value1) AS tblOrder ON tblOrder.customerId = Customers.id) AS tblOrder ON tblOrder.orderId = OrderItems.orderId";
                    command.Parameters.AddWithValue("@Value1", id);

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
                    return returnList;
                }
            }
        }

        public bool UpdateInvoice(string invoiceNumber, DateTime invoiceDate, string description, string terms, DateTime paymentDate, DateTime lastPayment, double interestRate, bool interestApplied, double discountAllowed, double coreCharge, DateTime dateShipped, string poNumber, string shipVia, string fob, string salesman, int orderId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE dbo.Orders SET invoiceNumber = @Value1, invoiceDate = @Value2, invoiceDesc = @Value3, terms = @Value4, paidDate = @Value5, lastPayment = @Value6, interestRate = @Value7, interestApplied = @Value8, discountAllowed = @Value9, dateShipped = @Value10, poNumber = @Value11, shipVia = @Value12, fob = @Value13, salesman = @Value14 WHERE id = @Value15";
                    command.Parameters.AddWithValue("@Value1", invoiceNumber);
                    command.Parameters.AddWithValue("@Value2", invoiceDate);
                    command.Parameters.AddWithValue("@Value3", description);
                    command.Parameters.AddWithValue("@Value4", terms);
                    command.Parameters.AddWithValue("@Value5", paymentDate);
                    command.Parameters.AddWithValue("@Value6", lastPayment);
                    command.Parameters.AddWithValue("@Value7", interestRate);
                    command.Parameters.AddWithValue("@Value8", interestApplied);
                    command.Parameters.AddWithValue("@Value9", discountAllowed);
                    command.Parameters.AddWithValue("@Value10", dateShipped);
                    command.Parameters.AddWithValue("@Value11", poNumber);
                    command.Parameters.AddWithValue("@Value12", shipVia);
                    command.Parameters.AddWithValue("@Value13", fob);
                    command.Parameters.AddWithValue("@Value14", salesman);
                    command.Parameters.AddWithValue("@Value15", orderId);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The Invoice updated successfully.");
                }

                return true;
            }
        }



        public bool UpdateOrderStatus(int status, int orderId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE dbo.Orders SET status = @Value1 WHERE id = @Value2";
                    command.Parameters.AddWithValue("@Value1", status);
                    command.Parameters.AddWithValue("@Value2", orderId);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The Order Status updated successfully.");
                }

                return true;
            }
        }

        public List<InvoiceAgingData> InvoiceAgingDataList { get; private set; }

        public bool LoadInvoiceAgingData(int customerId)
        {
            InvoiceAgingDataList = new List<InvoiceAgingData>();
            int agingCurrent = 0, aging30 = 0, aging60 = 0, aging90 = 0, aging120 = 0, agingOver120 = 0;
            int totalAging = 0;

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataAdapter sda;
                    DataSet ds;

                    command.CommandText = @"SELECT invoiceDate, paidDate FROM Orders WHERE customerId = @Value1";
                    command.Parameters.AddWithValue("@Value1", customerId);

                    command.ExecuteNonQuery();
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        DateTime invoiceDate = row.Field<DateTime>("invoiceDate");
                        DateTime paidDate;
                        if (row.IsNull("paidDate"))
                            agingCurrent += 1;
                        else
                        {
                            paidDate = row.Field<DateTime>("paidDate");
                            TimeSpan difference = paidDate - invoiceDate;
                            int differenceInDays = (int)difference.TotalDays;
                            if (1 < differenceInDays && differenceInDays <= 30)
                            {
                                aging30 += 1;
                            }
                            else if (30 < differenceInDays && differenceInDays <= 60)
                            {
                                aging60 += 1;
                            }
                            else if (60 < differenceInDays && differenceInDays <= 90)
                            {
                                aging90 += 1;
                            }
                            else if (90 < differenceInDays && differenceInDays <= 120)
                            {
                                aging120 += 1;
                            }
                            else if (differenceInDays > 120)
                            {
                                agingOver120 += 1;
                            }
                        }
                    }
                }
            }

            totalAging = agingCurrent + aging30 + aging60 + aging90 + aging120 + agingOver120;
            InvoiceAgingDataList.Add(new InvoiceAgingData("Current...", agingCurrent));
            InvoiceAgingDataList.Add(new InvoiceAgingData("1 to 30 Days", aging30));
            InvoiceAgingDataList.Add(new InvoiceAgingData("31 to 60 Days", aging60));
            InvoiceAgingDataList.Add(new InvoiceAgingData("61 to 90 Days", aging90));
            InvoiceAgingDataList.Add(new InvoiceAgingData("91 to 120 Days", aging120));
            InvoiceAgingDataList.Add(new InvoiceAgingData("Over 121 Days", agingOver120));
            InvoiceAgingDataList.Add(new InvoiceAgingData("Total Due", totalAging));

            return true;
        }

        public List<double[]> LoadPurchaseHistoryData(int customerId)
        {
            List<DateTime> PaidDateList = new List<DateTime>();
            List<double[]> TotalPaidByYearList = new List<double[]>();

            using (var connection = GetConnection())
            {
                connection.Open();
                List<int> yearList = new List<int>();
                yearList = GetInvoiceYearList(customerId);

                foreach (int year in yearList)
                {
                    double[] totalPaidByMonth = new double[13];
                    List<KeyValuePair<int, double>> totalPaidList = GetTotalPaidByYear(year, customerId);
                    foreach (KeyValuePair<int, double> totalPaid in totalPaidList)
                    {
                        totalPaidByMonth[totalPaid.Key] = totalPaid.Value;
                    }
                    TotalPaidByYearList.Add(totalPaidByMonth);
                }
            }
            return TotalPaidByYearList;
        }

        public List<int> GetInvoiceYearList(int customerId)
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

                    command.CommandText = @"SELECT YEAR(paidDate) AS year FROM Orders WHERE customerId = @Value1 AND invoiceBalance > 0 GROUP BY YEAR(paidDate) ORDER BY year DESC";
                    command.Parameters.AddWithValue("@Value1", customerId);

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

        public List<KeyValuePair<int, double>> GetTotalPaidByYear(int year, int customerId)
        {
            List<KeyValuePair<int, double>> totalPaidListByMonth = new List<KeyValuePair<int, double>>();
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataAdapter sda;
                    DataSet ds;

                    command.CommandText = @"SELECT SUM(totalPaid) AS total, MONTH(paidDate) AS month FROM Orders WHERE YEAR(paidDate) = @Value1 AND customerId = @Value2 GROUP BY  MONTH(paidDate)";
                    command.Parameters.AddWithValue("@Value1", year);
                    command.Parameters.AddWithValue("@Value2", customerId);

                    command.ExecuteNonQuery();
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    double totalPaidByYear = 0;
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        double totalPaid = double.Parse(row["total"].ToString());
                        int month = int.Parse(row["month"].ToString()) - 1;

                        totalPaidByYear += totalPaid;
                        totalPaidListByMonth.Add(new KeyValuePair<int, double>(month, totalPaid));
                    }
                    totalPaidListByMonth.Add(new KeyValuePair<int, double>(12, totalPaidByYear));
                }
            }

            return totalPaidListByMonth;
        }
    }
}
