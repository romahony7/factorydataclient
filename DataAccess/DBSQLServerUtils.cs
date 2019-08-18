using System.Data.SqlClient;


namespace DataAccess
{
    class DBSQLServerUtils
    {

        public static SqlConnection GetDBConnection(string datasource, string database, string username, string password)
        {
            //
            // Data Source=FD-SQL;Initial Catalog=FactoryDataClient;Persist Security Info=True;User ID=sa;Password=***********
            //
            string connString = @"Data Source=" + datasource + ";Initial Catalog="
                        + database + ";Persist Security Info=True;User ID=" + username + ";Password=" + password;

            SqlConnection conn = new SqlConnection(connString);

            return conn;
        }


    }
}