using System.Data.SqlClient;
using MJC.config;

namespace MJC.model
{
    public struct PaymentHistoryData
    {
        public int id { get; set; }
        public string checkNumber { get; set; }
        public DateTime datePaid { get; set; }
        public double amount { get; set; }
        public bool reserved { get; set; }
        public string syncToken { get; set; }
        public string qboPaymentId { get; set; }

        public PaymentHistoryData(int _id, string _checkNumber, DateTime _datePaid, double _amount, bool _reserved, string _syncToken, string _qboPaymentId)
        {
            id = _id;
            checkNumber = _checkNumber;
            datePaid = _datePaid;
            amount = _amount;
            reserved = _reserved;
            syncToken = _syncToken;
            qboPaymentId = _qboPaymentId;
        }
    }

    public class PaymentHistoryModel : DbConnection
    {
        public List<PaymentHistoryData> PaymentHistoryDataList { get; private set; }

        public bool LoadPaymentHistoryData(int filter)
        {
            PaymentHistoryDataList = new List<PaymentHistoryData>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataReader reader;

                    command.CommandText = @"select id, checkNumber, datePaid, amount, reversed, syncToken, qboPaymentId from dbo.Payments";

                    if (filter != 0)
                    {
                        command.CommandText = @"select id, checkNumber, datePaid, amount, reversed, syncToken, qboPaymentId from dbo.Payments where customerId = @filter";
                        command.Parameters.Add("@filter", System.Data.SqlDbType.Int).Value = filter;
                    }

                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        PaymentHistoryDataList.Add(new PaymentHistoryData((int)reader[0], reader[1].ToString(), DateTime.Parse(reader[2].ToString()), double.Parse(reader[3].ToString()), bool.Parse(reader[4].ToString()), reader[5].ToString(), reader[6].ToString()));
                    }

                    reader.Close();
                }
            }
            return true;
        }

        public bool DeletePaymentHistory(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "DELETE FROM dbo.Payments WHERE id = @Id";
                    command.Parameters.AddWithValue("@Id", id);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The Payment History was deleted.");
                }

                return true;
            }
        }

        public bool UpdateReverse(bool reverse, int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE dbo.Payments SET reversed = @Value1 WHERE id = @Value2";
                    command.Parameters.AddWithValue("@Value1", Convert.ToInt32(reverse));
                    command.Parameters.AddWithValue("@Value2", id);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The Payment updated successfully.");
                }

                return true;
            }
        }
    }
}
