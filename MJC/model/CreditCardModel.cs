using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MJC.config;
using System.Windows.Forms;
using System.Reflection.Emit;
using System.Xml.Linq;
using System.Data;
using Microsoft.VisualBasic;

namespace MJC.model
{
    public class CreditCardModel : DbConnection
    {
        public struct CreditCardData
        {

            public int Id { get; set; }
            public int CustomerId { get; set; }
            public string CardNumber { get; set; }
            public string CardType { get; set; }
            public DateTime Expires { get; set; }
            public string SecurityCode { get; set; }
            public int Priority {  get; set; }

            public CreditCardData(int _id, int _customerId, string _cardNumber, string _cardType, DateTime _expires, string _securityCode, int _priority)
            {
                Id = _id;
                CustomerId = _customerId;
                CardNumber = _cardNumber;
                CardType = _cardType;
                Expires = _expires;
                SecurityCode = _securityCode;
                Priority = _priority;
            }

        }

        public List<CreditCardData> LoadCreditCardData(int customerId)
        {
            List<CreditCardData> creditCardDataList = new List<CreditCardData>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataAdapter sda;
                    DataSet ds;

                    command.CommandText = @"SELECT * FROM CustomerCreditCards WHERE customerId = @Value1";
                    command.Parameters.AddWithValue("@Value1", customerId);

                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int id = int.Parse(row["id"].ToString());
                        string cardNumber = row["cardNumber"].ToString();
                        string cardType = row["cardType"].ToString();
                        DateTime expires = DateTime.Parse(row["expires"].ToString());
                        string securityCode = "";
                        string[] str = cardNumber.Split(" ");
                        securityCode = str[str.Length - 1];
                        int priority = int.Parse(row["Priority"].ToString());

                        creditCardDataList.Add(new CreditCardData { Id = id, CustomerId = customerId, CardNumber = cardNumber, CardType = cardType, Expires = expires, SecurityCode = securityCode, Priority = priority });
                    }
                }
            }

            return creditCardDataList;
        }

        public bool DeleteCreditCard(int _id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    //Get Total Number of Customers
                    command.CommandText = "DELETE FROM dbo.CustomerCreditCards WHERE id = @id";
                    command.Parameters.AddWithValue("@id", _id);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The CreditCard was deleted.");
                }

                return true;
            }
        }

        public CreditCardData GetCreditCardData(int _id)
        {
            CreditCardData creditCardData = new CreditCardData();
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"select * from dbo.CustomerCreditCards where id = @id";
                    command.Parameters.AddWithValue("@id", _id);

                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        int creditCardId = (int)reader.GetValue(0);
                        string cardNumber = reader.IsDBNull(3) ? "" : reader.GetString(3);
                        string cardType = reader.IsDBNull(4) ? "" : reader.GetString(4);
                        DateTime expires = reader.IsDBNull(5) ? DateTime.Now : reader.GetDateTime(5);
                        int priority = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);

                        creditCardData = new CreditCardData
                        {
                            Id = creditCardId,
                            CardNumber = cardNumber,
                            CardType = cardType,
                            Expires = expires,
                            Priority = priority
                        };
                    }

                    return creditCardData;
                }
            }
        }

        public bool AddCreditCard(int customerId , string cardNumber, string cardType, DateTime expires, int priority, bool active, int createdBy, int updatedBy)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "INSERT INTO dbo.CustomerCreditCards (customerId, cardNumber, cardType, expires, priority, active, createdAt, createdBy, updatedAt, updatedBy) VALUES (@customerId, @cardNumber, @cardType, @expires, @priority, @active, @createdAt, @createdBy, @updatedAt, @updatedBy)";
                    command.Parameters.AddWithValue("@customerId", customerId);
                    command.Parameters.AddWithValue("@cardNumber", cardNumber);
                    command.Parameters.AddWithValue("@cardType", cardType);
                    command.Parameters.AddWithValue("@expires", expires);
                    command.Parameters.AddWithValue("@priority", priority);
                    command.Parameters.AddWithValue("@active", Convert.ToInt16(active));

                    command.Parameters.AddWithValue("@createdAt", DateTime.Now);
                    command.Parameters.AddWithValue("@createdBy", createdBy);
                    command.Parameters.AddWithValue("@updatedAt", DateTime.Now);
                    command.Parameters.AddWithValue("@updatedBy", updatedBy);

                    command.ExecuteNonQuery();

                    MessageBox.Show("New CreditCard is added successfully.");
                }

                return true;
            }
        }

        public bool UpdateCreditCard(string cardNumber, string cardType, DateTime expires, int priority, bool active, int createdBy, int updatedBy, int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE dbo.CustomerCreditCards SET cardNumber = @cardNumber, cardType = @cardType, expires = @expires, priority = @priority, updatedAt = @updatedAt, updatedBy = @updatedBy WHERE id = @id";
                    command.Parameters.AddWithValue("@cardNumber", cardNumber);
                    command.Parameters.AddWithValue("@cardType", cardType);
                    command.Parameters.AddWithValue("@expires", expires);
                    command.Parameters.AddWithValue("@priority", priority);
                    command.Parameters.AddWithValue("@active", Convert.ToInt16(active));

                    command.Parameters.AddWithValue("@createdAt", DateTime.Now);
                    command.Parameters.AddWithValue("@createdBy", createdBy);
                    command.Parameters.AddWithValue("@updatedAt", DateTime.Now);
                    command.Parameters.AddWithValue("@updatedBy", updatedBy);

                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();

                    MessageBox.Show("The CreditCard updated successfully.");
                }

                return true;
            }
        }
    }
}
