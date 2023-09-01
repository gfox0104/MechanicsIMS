using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using Intuit.Ipp.Data;
using MJC.config;

namespace MJC.model
{
    public struct CustomerCreditCard
    {
        public int id { get; set; }
        public int customerId { get; set; }
        public string cardNumber { get; set; }
        public string cardType { get; set; }
        public DateTime expires { get; set; }
        public int priority { get; set; }

        public CustomerCreditCard(int _id, int _customerId, string _cardNumber, string _cardType, DateTime _expires, int _priority)
        {
            id = _id;
            customerId = _customerId;
            cardNumber = _cardNumber;
            cardType = _cardType;
            expires = _expires;
            priority = _priority;
        }
    }

    public class CustomerCreditCardModel : DbConnection
    {
        public List<CustomerCreditCard> GetCreditCardsByCustomerId(int customerId)
        {
            List<CustomerCreditCard> creditCards = new List<CustomerCreditCard>();
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"select id, customerId, cardNumber, cardType, expires, priority from dbo.CustomerCreditCards where customerId = @Value1";
                    command.Parameters.AddWithValue("@Value1", customerId);

                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        int id = (int)reader.GetValue(0);
                        string cardNumber = reader.GetString(2);
                        string cardType = reader.GetString(3);
                        DateTime expires = reader.GetDateTime(4);
                        int prority = (int)reader.GetValue(5);

                        creditCards.Add( new CustomerCreditCard
                        {
                            id = id,
                            customerId = customerId,
                            cardNumber = cardNumber,
                            cardType = cardType,
                            expires = expires,
                            priority = prority
                        });

                        return creditCards;
                    }

                    return creditCards;
                }
            }
        }
    }
}
