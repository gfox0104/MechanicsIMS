using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Antlr4.Runtime.Tree;
using MJC.config;

namespace MJC.model
{
    public struct PaymentDetailData
    {
        public int id { get; set; }
        public string invoiceNumber { get; set; }
        public double amount { get; set; }
        public double discount { get; set; }

        public PaymentDetailData(int _id, string _checkNumber, double _amount, double _discount)
        {
            id = _id;
            invoiceNumber = _checkNumber;
            amount = _amount;
            discount = _discount;
        }
    }

    public class PaymentDetailModel : DbConnection
    {
        public List<PaymentDetailData> PaymentDetailDataList { get; private set; }

        public bool LoadPaymentHistoryData(string filter)
        {
            PaymentDetailDataList = new List<PaymentDetailData>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataReader reader;

                    command.CommandText = @"select id, invoiceNumber, totalPaid, discountableAmount from dbo.Orders";

                    if (filter != "")
                    {
                        command.CommandText = @"select id, invoiceNumber, totalPaid, discountableAmount from dbo.Orders where customerId like @filter";
                        command.Parameters.Add("@filter", System.Data.SqlDbType.VarChar).Value = "%" + filter + "%";
                    }

                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int orderId = 0;
                        string invoiceNumber = "";
                        double totalPaid = 0;
                        double discountableAmount = 0;
                        if (reader[0] != DBNull.Value)
                            orderId = (int)reader[0];
                        if (reader[1] != DBNull.Value)
                            invoiceNumber = reader[1].ToString();
                        if (reader[2] != DBNull.Value)
                            totalPaid = double.Parse(reader[2].ToString());
                        if (reader[3] != DBNull.Value)
                            discountableAmount = double.Parse(reader[3].ToString());

                        PaymentDetailDataList.Add(new PaymentDetailData(orderId, invoiceNumber, totalPaid, discountableAmount));
                    }

                    reader.Close();
                }
            }
            return true;
        }

        public bool UpdatePayment(string checkNumber, double amount, double total, int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE dbo.Orders SET invoiceNumber = @Value1, totalPaid = @Value2, discountableAmount = @Value3 WHERE id = @Value4";
                    command.Parameters.AddWithValue("@Value1", checkNumber);
                    command.Parameters.AddWithValue("@Value2", amount);
                    command.Parameters.AddWithValue("@Value3", total);
                    command.Parameters.AddWithValue("@Value4", id);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The Order updated successfully.");
                }

                return true;
            }
        }

        public int CreatePayment(int customerId, DateTime datePaid, double amount, string syncToken, string qboPaymentId)
        {
            CustomerCreditCardModel customerCreditCardModelObj = new CustomerCreditCardModel();

            List<CustomerCreditCard> creditCards = customerCreditCardModelObj.GetCreditCardsByCustomerId(customerId);
            int creditCardId = 0;
            if(creditCards.Count > 0)
            {
                creditCardId = creditCards[0].id;
            }

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "INSERT INTO dbo.Payments (customerId, creditCardId, datePaid, amount, createdBy, updatedBy, reversed, active, syncToken, qboPaymentId)  OUTPUT INSERTED.id VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7, @Value8, @Value9, @Value10)";
                    command.Parameters.AddWithValue("@Value1", customerId);
                    if(creditCardId == 0)
                        command.Parameters.AddWithValue("@Value2", creditCardId);
                    else command.Parameters.AddWithValue("@Value2", DBNull.Value);
                    command.Parameters.AddWithValue("@Value3", datePaid);
                    command.Parameters.AddWithValue("@Value4", amount);
                    command.Parameters.AddWithValue("@Value5", 1);
                    command.Parameters.AddWithValue("@Value6", 1);
                    command.Parameters.AddWithValue("@Value7", 1);
                    command.Parameters.AddWithValue("@Value8", 1);
                    command.Parameters.AddWithValue("@Value9", syncToken);
                    command.Parameters.AddWithValue("@Value10", qboPaymentId);

                    int paymentId = (int)command.ExecuteScalar();

                    MessageBox.Show("Payment is added successfully.");
                    return paymentId;
                }
            }

        }

        public int CreateOrderPayment(int orderId, int paymentId, int createdBy, int updatedBy)
        {
            int creditCardId = 0;

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "INSERT INTO dbo.OrderPayments (orderId, paymentId, createdBy, updatedBy, active)  OUTPUT INSERTED.id VALUES (@Value1, @Value2, @Value3, @Value4, @Value5)";
                    command.Parameters.AddWithValue("@Value1", orderId);
                    command.Parameters.AddWithValue("@Value2", paymentId);
                    command.Parameters.AddWithValue("@Value3", createdBy);
                    command.Parameters.AddWithValue("@Value4", updatedBy);
                    command.Parameters.AddWithValue("@Value5", 1);

                    int orderPaymentId = (int)command.ExecuteNonQuery();

                    //MessageBox.Show("Order Payment is added successfully.");
                    return orderPaymentId;
                }
            }
        }

        public bool DeletePayment(int paymentId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "DELETE FROM dbo.OrderPayments WHERE id = @Value1";
                    command.Parameters.AddWithValue("@Value1", paymentId);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The Payment was deleted.");
                }

                return true;
            }
        }
    }
}
