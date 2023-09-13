using System.Data;
using System.Data.SqlClient;
using MJC.config;

namespace MJC.model
{
    internal class SKUPricesModel : DbConnection
    {
        private PriceTiersModel PriceTiersModelObj = new PriceTiersModel();
        private CategoryPriceTierModel CategoryPriceModelObj = new CategoryPriceTierModel();
        private CategoriesModel CategoriesModelObj = new CategoriesModel();

        public List<KeyValuePair<int, double>> LoadPriceTierDataBySKUId(int skuId)
        {
            List<KeyValuePair<int, double>> priceTierList = new List<KeyValuePair<int, double>>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    SqlDataReader reader;

                    command.Connection = connection;
                    // Get PriceTierList by SKUId
                    command.CommandText = @"select priceTierId, price 
                                            from dbo.SKUPrices
                                            where skuId = @Value1";
                    command.Parameters.AddWithValue("@Value1", skuId);

                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int.TryParse(reader[0].ToString(), out int priceTierId);
                        double.TryParse(reader[1].ToString(), out double price);

                        priceTierList.Add(new KeyValuePair<int, double>(priceTierId, price));
                    }
                }

                return priceTierList;
            }
        }

        public bool IsExistSkuPriceByPriceTierId(int skuId, int priceTierId)
        {
            List<KeyValuePair<int, double>> priceTierList = new List<KeyValuePair<int, double>>();

            using (var connection = GetConnection())
            {
                connection.Open();
                SqlDataAdapter sda;
                DataSet ds;

                using (var command = new SqlCommand())
                {
                    SqlDataReader reader;

                    command.Connection = connection;

                    command.CommandText = @"select priceTierId, price 
                                            from dbo.SKUPrices
                                            where skuId = @Value1 AND priceTierId = @Value2";
                    command.Parameters.AddWithValue("@Value1", skuId);
                    command.Parameters.AddWithValue("Value2", priceTierId);

                    command.ExecuteNonQuery();
                    sda = new SqlDataAdapter(command);
                    ds = new DataSet();
                    sda.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    //reader = command.ExecuteReader();

                    //while (reader.Read())
                    //{
                    //    int.TryParse(reader[0].ToString(), out int priceTierId);
                    //    double.TryParse(reader[1].ToString(), out double price);

                    //    priceTierList.Add(new KeyValuePair<int, double>(priceTierId, price));
                    //}
                }

                //return priceTierList;
            }
        }

        public bool AddSKUPrice(int skuId, int priceTierId, double price)
        {

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "INSERT INTO dbo.SKUPrices (active, skuId, priceTierId, price, createdAt, createdBy, updatedAt, updatedBy) VALUES (@active, @Value1, @Value2, @Value3, @Value4, @Value5, @Value6, @Value7)";
                    command.Parameters.AddWithValue("@active", true);
                    command.Parameters.AddWithValue("@Value1", skuId);
                    command.Parameters.AddWithValue("@Value2", priceTierId);
                    command.Parameters.AddWithValue("@Value3", price);
                    command.Parameters.AddWithValue("@Value4", DateTime.Now);
                    command.Parameters.AddWithValue("@Value5", 1);
                    command.Parameters.AddWithValue("@Value6", DateTime.Now);
                    command.Parameters.AddWithValue("@Value7", 1);
                    command.ExecuteNonQuery();
                }
                return true;
            }
        }

        public bool UpdateSKUPrice(int skuId, int priceTierId, double price, bool active, int createdBy, int updatedBy)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    
                    command.CommandText = "Update dbo.SKUPrices SET price = @Value1, active = @Value2, createdBy = @Value3, updatedBy = @Value4  WHERE skuId = @Value5 AND priceTierId = @Value6";
                    command.Parameters.AddWithValue("@Value1", price);
                    command.Parameters.AddWithValue("@Value2", Convert.ToInt32(active));
                    command.Parameters.AddWithValue("@Value3", createdBy);
                    command.Parameters.AddWithValue("@Value4", updatedBy);
                    command.Parameters.AddWithValue("@Value5", skuId);
                    command.Parameters.AddWithValue("@Value6", priceTierId);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0) return true;
                    else return false;
                }
            }
        }

        public double CalculateSKUPrice(double inventoryValue, double categoryMargin, double profitMargin, int priceTierId, int categoryId)
        {
            PriceTiersModelObj.LoadPriceTierData();
            List<PriceTierData> PriceTiersList = PriceTiersModelObj.PriceTierDataList;
            CategoryData category = CategoriesModelObj.LoadCategoryById(categoryId);
            int calculateAs = category.calculateAs;
            double price = 0;
            
            if (calculateAs == 1)
            {
                price = (inventoryValue + categoryMargin) * (1 + profitMargin);
            }
            else if (calculateAs == 2)
            {
                price = (inventoryValue) * (1 + profitMargin) * (1 + profitMargin);
            }
            return price;
        }
    }
}
