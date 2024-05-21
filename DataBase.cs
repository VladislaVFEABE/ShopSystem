using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop
{
    class DataBase
    {
        private SqlConnection sql;

        public DataBase()
        {
            sql = new SqlConnection(@"Data Source=DESKTOP-EG8M5CN\DESKTOP;Initial Catalog=ShopSystem;Integrated Security=True");
        }

        public void openConnection()
        {
            if (sql.State == System.Data.ConnectionState.Closed)
            {
                sql.Open();
            }
        }

        public void closeConnection()
        {
            if (sql.State == System.Data.ConnectionState.Open)
            {
                sql.Close();
            }
        }

        public SqlConnection GetConnection()
        {
            return sql;
        }
    }
}
