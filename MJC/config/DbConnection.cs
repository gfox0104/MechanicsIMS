using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MJC.config
{
    public abstract class DbConnection
    {
        private readonly string connectionString;

        public DbConnection()
        {
            connectionString = @"Server=tcp:s11.everleap.com;Initial Catalog=DB_7153_mjcdev;User ID=DB_7153_mjcdev_user;Password=Drew-Cubicle5-Guru;Integrated Security=False";
            //connectionString = @"Server=tcp:s10.everleap.com;Initial Catalog=DB_7153_mjcprod;User ID=DB_7153_mjcprod_user;Password=!n8B6%x&NSCfbyh4ReLG;Integrated Security=False";
            //connectionString = @"Data Source = DESKTOP-G6592L2\SQL_SERVER; user id=sa; password=qwe234ASD@#$; Initial Catalog = DB_7153_mjcdev;";
        }

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
