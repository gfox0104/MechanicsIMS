using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MJC.config;

namespace MJC.model
{
    public class OrderModel : DbConnection
    {
        public dynamic GetOrderById(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    SqlDataReader reader;
                    List<dynamic> returnList = new List<dynamic>();
                    command.Connection = connection;
                    command.CommandText = @"SELECT id as orderId, invoiceNumber, customerId, invoiceDate, invoiceDesc, terms, paidDate, lastPayment, interestRate, interestApplied, discountAllowed, dateShipped, poNumber, shipVia, fob, salesman, invoiceTotal, discountableAmount, totalPaid, invoiceBalance, qboOrderId, syncToken from Orders WHERE id = @Value1";
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
                    return returnList[0];
                }
            }
        }

        public dynamic GetNextOrderId()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    List<dynamic> returnList = new List<dynamic>();
                    command.Connection = connection;
                    command.CommandText = @"SELECT TOP 1 id from Orders ORDER BY id  DESC";

                    var id = 0;
                    
                    var value = command.ExecuteScalar();
                    if (value != null) id = (int)value;

                    // no rows returned
                    return id + 1;
                }
            }
        }

        public int CreateOrder(int customerId, string customerName, string terms, string invoiceNumber, DateTime invoiceDate, string invoiceDesc, double invoiceTotal, string syncToken, string qboOrderId, int createdBy = 1, int updatedBy = 1)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                int orderId = 0;
                string creditCode = "";

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"INSERT INTO dbo.Orders(customerId, customerName, creditCode, terms, invoiceNumber, invoiceDate, invoiceDesc, invoiceTotal, syncToken, qboOrderId, createdBy, updatedBy, active) OUTPUT INSERTED.id VALUES(@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7, @Value8, @Value9, @Value10, @Value11, @Value12, @Value13)";
                    command.Parameters.AddWithValue("@Value1", customerId);
                    command.Parameters.AddWithValue("@Value2", customerName);
                    if (!string.IsNullOrEmpty(creditCode))
                        command.Parameters.AddWithValue("@Value3", creditCode);
                    else command.Parameters.AddWithValue("@Value3", DBNull.Value);
                    command.Parameters.AddWithValue("@Value4", terms);
                    command.Parameters.AddWithValue("@Value5", invoiceNumber);
                    command.Parameters.AddWithValue("@Value6", invoiceDate);
                    command.Parameters.AddWithValue("@Value7", invoiceDesc);
                    command.Parameters.AddWithValue("@Value8", invoiceTotal);
                    command.Parameters.AddWithValue("@Value9", syncToken);
                    command.Parameters.AddWithValue("@Value10", qboOrderId);
                    command.Parameters.AddWithValue("@Value11", createdBy);
                    command.Parameters.AddWithValue("@Value12", updatedBy);
                    command.Parameters.AddWithValue("@Value13", 1);

                    orderId = (int)command.ExecuteScalar();
                }

                return orderId;
            }
        }

        public int UpdateOrder(int customerId, string customerName, string terms, string syncToken, string qboOrderId, int createdBy, int updatedBy, int orderId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                string creditCode = "";

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE Orders SET status = @Value1 WHERE id = @Value2";

                    command.CommandText = @"UPDATE ORDERS SET customerId = @Value1, customerName = @Value2, creditCode = @Value3, terms = @Value4, syncToken = @Value5, qboOrderId = @Value6, createdBy = @Value7, updatedBy = @Value8, active = @value9 WHERE id = @Value10";
                    
                    command.Parameters.AddWithValue("@Value1", customerId);
                    command.Parameters.AddWithValue("@Value2", customerName);
                    if (!string.IsNullOrEmpty(creditCode))
                        command.Parameters.AddWithValue("@Value3", creditCode);
                    else command.Parameters.AddWithValue("@Value3", DBNull.Value);
                    command.Parameters.AddWithValue("@Value4", terms);
                    command.Parameters.AddWithValue("@Value5", syncToken);
                    command.Parameters.AddWithValue("@Value6", qboOrderId);
                    command.Parameters.AddWithValue("@Value7", createdBy);
                    command.Parameters.AddWithValue("@Value8", updatedBy);
                    command.Parameters.AddWithValue("@Value9", 1);
                    command.Parameters.AddWithValue("@Value10", orderId);

                    command.ExecuteScalar();
                    //MessageBox.Show("The Order is updated successfully.");
                }

                return orderId;
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
                    command.CommandText = @"UPDATE Orders SET status = @Value1 WHERE id = @Value2";
                    command.Parameters.AddWithValue("@Value1", status);
                    command.Parameters.AddWithValue("@Value2", orderId);

                    command.ExecuteNonQuery();

                    //MessageBox.Show("The Order Status updated successfully.");
                    return true;
                }
            }
        }

        public bool DeleteOrder(int orderId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = @"DELETE FROM OrderItems WHERE id = @Value1";
                    command.Parameters.AddWithValue("@Value1", orderId);
                    command.ExecuteNonQuery();

                    command.CommandText = @"DELETE FROM Orders WHERE id = @Value1";
                    command.ExecuteNonQuery();

                    //MessageBox.Show("The Order Status updated successfully.");
                    return true;
                }
            }
        }
    }
}
