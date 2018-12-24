using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace SHJXMESWCFServer
{
    public static class DataHelper
    {
        public   enum RetrunType {strings,dataTable,integer }
        public static object ExecuteSql(string sql,SqlConnection sqlconn,RetrunType returnType)
        {
            try
            {
                DataTable dataTable = new DataTable();
                SqlCommand sqlCommand = new SqlCommand(sql, sqlconn);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlconn)
                {
                    sqlconn.Open();
                    sqlDataAdapter.Fill(dataTable);
                    switch (returnType)
                    {
                        case RetrunType.strings:
                            return Newtonsoft.Json.JsonConvert.SerializeObject(dataTable);
                           
                        case RetrunType.dataTable:
                            return dataTable;
                        case RetrunType.integer:
                            return int.Parse(sqlCommand.ExecuteScalar().ToString());
                        default:
                          return null;
                    }
                    

                }
            }
            catch (Exception ex)
            {

                return "发生错误：" + ex.Message;
            }
          
        }
       
        public static int ExecuteSqlprocedure(string SqlProName,SqlConnection sqlconn)
        {
            try
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = SqlProName;
                sqlCommand.Connection = sqlconn;
                return (int)sqlCommand.ExecuteScalar();
            }
            catch (Exception)
            {

                return 0;
            }
           

        }

        public static SqlConnection CreateSqlConn(string sqlConfig)
        {
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[sqlConfig].ConnectionString);
            return connection;
        }


    }
}