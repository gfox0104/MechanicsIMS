using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using MJC.config;

namespace MJC.model
{
    public struct VendorData
    {
        public int id { get; set; }
        public string number { get; set; }
        public string name { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string phone { get; set; }

        public VendorData(int _id, string _name, string _number, string _city, string _state, string _phone)
        {
            id = _id;
            number = _number;
            name = _name;
            city = _city;
            state = _state;
            phone = _phone;
        }
    }

    public class VendorsModel : DbConnection
    {
        public List<VendorData> VendorDataList { get; private set; }

        public bool LoadVendorData(string filter, bool archived)
        {
            VendorDataList = new List<VendorData>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataReader reader;

                    command.CommandText = @"select id, vendorName, vendorNumber, city, state, businessPhone
                                            from dbo.Vendors";

                    if (archived) command.CommandText += " where archived = 1";

                    if (filter != "")
                    {
                        command.CommandText = @"select id, vendorName, vendorNumber, city, state, businessPhone
                                                from dbo.Vendors
                                                where vendorName like @filter or city like @filter or state like @filter or businessPhone like @filter";
                        command.Parameters.Add("@filter", System.Data.SqlDbType.VarChar).Value = "%" + filter + "%";
                    }

                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        VendorDataList.Add(
                            new VendorData((int)reader[0], reader[1].ToString(), reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[5].ToString())
                        );
                    }
                    reader.Close();
                }
            }

            return true;
        }

        public bool AddVendor(string vendor_name, string vendor_number, string address1, string address2, string city, string state, string zipcode, string business_phone, string fax)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "INSERT INTO dbo.Vendors (archived, vendorName, address1, address2, city, state, zipcode, businessPhone, fax, createdAt, createdBy, updatedAt, updatedBy, vendorNumber) VALUES (@Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7, @Value8, @Value9, @Value10, @Value11, @Value12, @Value13, @Value14)";
                    command.Parameters.AddWithValue("@Value1", true);
                    command.Parameters.AddWithValue("@Value2", vendor_name);
                    command.Parameters.AddWithValue("@Value3", address1);
                    command.Parameters.AddWithValue("@Value4", address2);
                    command.Parameters.AddWithValue("@Value5", city);
                    command.Parameters.AddWithValue("@Value6", state);
                    command.Parameters.AddWithValue("@Value7", zipcode);
                    command.Parameters.AddWithValue("@Value8", business_phone);
                    command.Parameters.AddWithValue("@Value9", fax);
                    command.Parameters.AddWithValue("@Value10", DateTime.Now);
                    command.Parameters.AddWithValue("@Value11", 1);
                    command.Parameters.AddWithValue("@Value12", DateTime.Now);
                    command.Parameters.AddWithValue("@Value13", 1);
                    command.Parameters.AddWithValue("@Value14", vendor_number);

                    command.ExecuteNonQuery();

                    MessageBox.Show("New Vendor has been added successfully.");
                }

                return true;
            }
        }

        public bool UpdateVendor(string vendor_name, string vendor_number, string address1, string address2, string city, string state, string zipcode, string business_phone, string fax, int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE dbo.Vendors SET vendorName = @Value1, vendorNumber=@Value10, address1 = @Value2, address2 = @Value3, city = @Value4, state = @Value5, zipcode = @Value6, businessPhone = @Value7, fax = @Value8 WHERE id = @Value9";
                    command.Parameters.AddWithValue("@Value1", vendor_name);
                    command.Parameters.AddWithValue("@Value2", address1);
                    command.Parameters.AddWithValue("@Value3", address2);
                    command.Parameters.AddWithValue("@Value4", city);
                    command.Parameters.AddWithValue("@Value5", state);
                    command.Parameters.AddWithValue("@Value6", zipcode);
                    command.Parameters.AddWithValue("@Value7", business_phone);
                    command.Parameters.AddWithValue("@Value8", fax);
                    command.Parameters.AddWithValue("@Value9", id);
                    command.Parameters.AddWithValue("@Value10", vendor_number);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The Vendor has been updated successfully.");
                }

                return true;
            }
        }

        public bool DeleteVendor(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "DELETE FROM dbo.Vendors WHERE id = @Value1";
                    command.Parameters.AddWithValue("@Value1", id);

                    command.ExecuteNonQuery();

                    MessageBox.Show("The Vendor was deleted.");
                }

                return true;
            }
        }

        public List<dynamic> GetVendorData(int id)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    SqlDataReader reader;
                    List<dynamic> returnList = new List<dynamic>();

                    command.Connection = connection;

                    command.CommandText = @"select * from dbo.Vendors where id = @id";
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

        public List<KeyValuePair<int, string>> GetVendorItems()
        {
            List<KeyValuePair<int, string>> SKUList = new List<KeyValuePair<int, string>>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataReader reader;

                    command.CommandText = @"select id, vendorName
                                            from dbo.Vendors";
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

        public List<string> GetVendorNameList()
        {
            List<string> vendors = new List<string>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataReader reader;

                    command.CommandText = @"SELECT vendorName
                                            from dbo.Vendors";
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        vendors.Add(reader[0].ToString());
                    }
                    reader.Close();
                }
            }
            return vendors;
        }

        public List<KeyValuePair<int, string>> GetVendorList()
        {
            List<KeyValuePair<int, string>> VendorList = new List<KeyValuePair<int, string>>();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    SqlDataReader reader;

                    command.CommandText = @"select id, vendorName
                                            from dbo.Vendors";
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        VendorList.Add(
                            new KeyValuePair<int, string>((int)reader[0], reader[1].ToString())
                        );
                    }
                    reader.Close();
                }
            }
            return VendorList;
        }
    }
}
