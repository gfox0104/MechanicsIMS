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
            connectionString = @"Server=tcp:mndSQL10.everleap.com; Initial Catalog=DB_7153_mjcdev; User ID=DB_7153_mjcdev_user; Password = Drew-Cubicle5-Guru; Integrated Security = False";
        }

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
