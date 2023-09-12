using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using MJC.config;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace MJC.model
{
    public struct UserData
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public bool permissionOrders { get; set; }
        public bool permissionInventory { get; set; }
        public bool permissionReceivables { get; set; }
        public bool permissionSetting { get; set; }
        public bool permissionUsers { get; set; }
        public bool permissionQuickBooks { get; set; }

        public UserData(int _id, string _username, string _password, bool _permissionOrders, bool _permissionInventory, bool _permissionReceivables, bool _permissionSetting, bool _permissionUsers, bool _permissionQuickBooks)
        {
            id = _id;
            username = _username;
            password = _password;
            permissionOrders = _permissionOrders;
            permissionInventory = _permissionInventory;
            permissionReceivables = _permissionReceivables;
            permissionSetting = _permissionSetting;
            permissionUsers = _permissionUsers;
            permissionQuickBooks = _permissionQuickBooks;
        }
    }

    public class UserModel : DbConnection
    {
        public List<UserData> UserDataList { get; set; }

        public bool LoadUserData()
        {
            List<UserData> users = new List<UserData>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataAdapter sda;
                    DataSet ds;

                    command.CommandText = @"SELECT * FROM Accounts";

                    command.ExecuteNonQuery();
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int id = Convert.ToInt32(row["id"]);
                        string username = row["username"].ToString();
                        string password = row["password"].ToString();
                        bool pemsnOrder = row.Field<bool>("permissionOrders");
                        bool pemsnInventory = row.Field<bool>("permissionInventory");
                        bool pemsnReceivables = row.Field<bool>("permissionReceivables");
                        bool pemsnSetting = row.Field<bool>("permissionSettings");
                        bool pemsnUsers = row.Field<bool>("permissionUsers");
                        bool pemsnQuickBooks = row.Field<bool>("permissionQuickbooks");

                        users.Add(new UserData { id = id, username = username, password = password, permissionOrders = pemsnOrder, permissionInventory = pemsnInventory, permissionReceivables = pemsnReceivables, permissionSetting = pemsnSetting, permissionUsers = pemsnUsers, permissionQuickBooks = pemsnQuickBooks });
                    }

                    UserDataList = users;
                }

                return true;
            }
        }

        public bool DeleteUser(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "DELETE FROM dbo.Accounts WHERE id = @Value1";
                    command.Parameters.AddWithValue("@Value1", id);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The User was deleted.");
                }
                return true;
            }
        }

        public bool CreateUser(string username, string password, bool permissionOrders, bool permissionInventory, bool permissionReceivables, bool permissionSetting, bool permissionUsers, bool permissionQbLink, int createdBy, int updatedBy)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                int active = 1;
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"INSERT INTO Accounts(active, username, password, permissionOrders, permissionInventory, permissionReceivables, permissionSettings, permissionUsers, permissionQuickbooks, createdBy, updatedBy) VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7, @Value8, @Value9, @Value10, @Value11)";
                    command.Parameters.AddWithValue("@Value1", active);
                    command.Parameters.AddWithValue("@Value2", username);
                    command.Parameters.AddWithValue("@Value3", password);
                    command.Parameters.AddWithValue("@Value4", Convert.ToInt32(permissionOrders));
                    command.Parameters.AddWithValue("@Value5", Convert.ToInt32(permissionInventory));
                    command.Parameters.AddWithValue("@Value6", Convert.ToInt32(permissionReceivables));
                    command.Parameters.AddWithValue("@Value7", Convert.ToInt32(permissionSetting));
                    command.Parameters.AddWithValue("@Value8", Convert.ToInt32(permissionUsers));
                    command.Parameters.AddWithValue("@Value9", Convert.ToInt32(permissionQbLink));
                    command.Parameters.AddWithValue("@Value10", createdBy);
                    command.Parameters.AddWithValue("@Value11", updatedBy);

                    command.ExecuteNonQuery();

                    MessageBox.Show("User created successfully.");
                }

                return true;
            }
        }

        public bool UpdateUser(string username, string password, bool permissionOrders, bool permissionInventory, bool permissionReceivables, bool permissionSetting, bool permissionUsers, bool permissionQbLink, int createdBy, int updatedBy, int userId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                int active = 1;
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"Update Accounts SET active = @Value1, username = @Value2, password = @Value3, permissionOrders = @Value4, permissionInventory = @Value5, permissionReceivables = @Value6, permissionSettings = @Value7, permissionUsers = @Value8, permissionQuickbooks = @Value9, createdBy = @Value10, updatedBy = @Value11 WHERE id = @Value12";

                    command.Parameters.AddWithValue("@Value1", active);
                    command.Parameters.AddWithValue("@Value2", username);
                    command.Parameters.AddWithValue("@Value3", password);
                    command.Parameters.AddWithValue("@Value4", Convert.ToInt32(permissionOrders));
                    command.Parameters.AddWithValue("@Value5", Convert.ToInt32(permissionInventory));
                    command.Parameters.AddWithValue("@Value6", Convert.ToInt32(permissionReceivables));
                    command.Parameters.AddWithValue("@Value7", Convert.ToInt32(permissionSetting));
                    command.Parameters.AddWithValue("@Value8", Convert.ToInt32(permissionUsers));
                    command.Parameters.AddWithValue("@Value9", Convert.ToInt32(permissionQbLink));
                    command.Parameters.AddWithValue("@Value10", createdBy);
                    command.Parameters.AddWithValue("@Value11", updatedBy);
                    command.Parameters.AddWithValue("@Value12", userId);

                    command.ExecuteNonQuery();

                    MessageBox.Show("User updated successfully.");
                }

                return true;
            }
        }

        public bool Login(string username, string password)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT * FROM Accounts WHERE username=@Value1 and password=@Value2";
                    command.Parameters.AddWithValue("@Value1", username);
                    command.Parameters.AddWithValue("@Value2", password);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    if (reader.HasRows) return true;

                }
                return false;
            }
        }

        public UserData getUserDataById(string userName)
        {
            UserData userData = new UserData();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataReader reader;

                    command.CommandText = @"SELECT id, 
                                           username, 
                                           password, 
                                           permissionOrders, 
                                           permissionInventory, 
                                           permissionReceivables, 
                                           permissionSettings, 
                                           permissionUsers, 
                                           permissionQuickbooks
                                  FROM Accounts WHERE userName = @userName";
                    command.Parameters.AddWithValue("@userName", userName);

                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        userData = new UserData(
                            Convert.ToInt32(reader["id"]),
                            reader["username"].ToString(),
                            reader["password"].ToString(),
                            Convert.ToBoolean(reader["permissionOrders"]),
                            Convert.ToBoolean(reader["permissionInventory"]),
                            Convert.ToBoolean(reader["permissionReceivables"]),
                            Convert.ToBoolean(reader["permissionSettings"]),
                            Convert.ToBoolean(reader["permissionUsers"]),
                            Convert.ToBoolean(reader["permissionQuickbooks"])
                        );
                    }
                    reader.Close();
                }
            }

            return userData;
        }

    }

}
