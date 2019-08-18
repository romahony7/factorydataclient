using Common;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DataAccess

{
    public class DBUtils
    {
        

        public static SqlConnection GetDBConnection(string server,string catalog, string sid,string pwd)
        {
            string datasource = server;
            string database = catalog;
            string username = sid;
            string password = pwd;

            return DBSQLServerUtils.GetDBConnection(datasource, database, username, password);
        }

        public static int SqlInsert(TransactionUDT udt, string tag)
        {
            int rowCount;

            SqlConnection connection = GetDBConnection(udt.DBSource, udt.DBName, udt.DBSid, udt.DBPwd);

            try
            {
                connection.Open();
            }
            catch (Exception)
            {
                throw ;
            }
            try
            {
                // Insert statement.
                string sql = "Insert into "+udt.TableName+" (TagName,"+udt.ColumnName+",PlcTS) "
                                                 + " values (@tag,@value,@timestamp) ";

                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = sql;

                // Add parameter 
                cmd.Parameters.Add("@tag", SqlDbType.VarChar).Value = tag;
                cmd.Parameters.Add("@value", SqlDbType.BigInt).Value = udt.Data;
                cmd.Parameters.Add("@timestamp", SqlDbType.DateTime).Value = udt.TS;

                // Execute Command (for Delete,Insert or Update).
                rowCount = cmd.ExecuteNonQuery();

            }
            catch (Exception)
            {
                throw ;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
                connection = null;
            }

            return rowCount;
        }

        public static int SqlUpdate(TransactionUDT udt, string tag)
        {
            int rowCount;

            SqlConnection connection = GetDBConnection(udt.DBSource, udt.DBName, udt.DBSid, udt.DBPwd);

            try
            {
                connection.Open();
            }
            catch (Exception)
            {
                throw;
            }
            try
            {
                // Update statement.
                string sql = "Update "+udt.TableName+" set "+udt.ColumnName+" = @value,PlcTs = @timestamp,RecordTS = @time where TagName = @tag";
                
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = sql;

                // Add parameter 
                cmd.Parameters.Add("@tag", SqlDbType.VarChar).Value = tag;
                cmd.Parameters.Add("@value", SqlDbType.BigInt).Value = udt.Data;
                cmd.Parameters.Add("@timestamp", SqlDbType.DateTime).Value = udt.TS;
                cmd.Parameters.Add("@time", SqlDbType.DateTime).Value = DateTime.Now;

                // Execute Command (for Delete,Insert or Update).
                rowCount = cmd.ExecuteNonQuery();

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
                connection.Dispose();
                connection = null;
            }

            return rowCount;
        }
    }

}