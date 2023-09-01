using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MJC.config;
using MJC.forms.category;

namespace MJC.model
{
    internal class CategoryPriceTierModel : DbConnection
    {
        public double GetCategoryMarginByPriceTierId(int categoryId, int priceTierId)
        {
            List<KeyValuePair<int, double>> priceTierList = new List<KeyValuePair<int, double>>();

            using (var connection = GetConnection())
            {
                connection.Open();
                SqlDataAdapter sda;
                DataSet ds;
                double categoryMargin = 0;

                using (var command = new SqlCommand())
                {
                    SqlDataReader reader;

                    command.Connection = connection;
                    
                    command.CommandText = @"select margin 
                                            from dbo.CategoryPriceTiers
                                            where categoryId = @Value1 AND priceTierId = @Value2";
                    command.Parameters.AddWithValue("@Value1", categoryId);
                    command.Parameters.AddWithValue("@Value2", priceTierId);

                    reader = command.ExecuteReader();

                    
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        categoryMargin = double.Parse(reader[0].ToString());
                    }
                }

                return categoryMargin;
            }
        }

        public bool AddCategoryPriceTier(int _categoryId, int _priceTierId, double _margin)
        {

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    //Get Total Number of Customers
                    command.CommandText = "INSERT INTO dbo.CategoryPriceTiers (active, categoryId, priceTierId, margin, createdAt, createdBy, updatedAt, updatedBy) VALUES (@active, @Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7)";
                    command.Parameters.AddWithValue("@active", true);
                    command.Parameters.AddWithValue("@Value1", _categoryId);
                    command.Parameters.AddWithValue("@Value2", _priceTierId);
                    command.Parameters.AddWithValue("@Value3", _margin);
                    command.Parameters.AddWithValue("@Value4", DateTime.Now);
                    command.Parameters.AddWithValue("@Value5", 1);
                    command.Parameters.AddWithValue("@Value6", DateTime.Now);
                    command.Parameters.AddWithValue("@Value7", 1);
                    command.ExecuteNonQuery();
                }
                return true;
            }
        }

        public bool UpdateCategoryPriceTier(int _categoryId, int _priceTierId, double _margin)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    
                    command.CommandText = "Update dbo.CategoryPriceTiers SET margin = @Value1  WHERE categoryId = @Value2 AND priceTierId = @Value3";
                    command.Parameters.AddWithValue("@Value1", _margin);
                    command.Parameters.AddWithValue("@Value2", _categoryId);
                    command.Parameters.AddWithValue("@Value3", _priceTierId);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
        }
    }
}
