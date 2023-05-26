//using System.Collections.Concurrent;
//using System.Data;
//using MySql.Data.MySqlClient;
//using System.Data.SqlClient;


using Microsoft.Extensions.Configuration;
 using MySql.Data.MySqlClient;
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Data;
  using System.Data.SqlClient;
  using System.Linq;
  using System.Threading.Tasks;

namespace ShoppingMallSys.DataBase
{
    public class DbConnectionFactory
    {
        /// <summary>
         /// 数据库连接字符串缓存
         /// </summary>
         private static ConcurrentDictionary<string, string> connStrDict = new ConcurrentDictionary<string, string>();
         private static IConfiguration ?Configuration { get; }

        private static string GetConnString(string dbKey)
         {
             string connString = string.Empty;
             if (connStrDict.Keys.Contains(dbKey))
             {
                 connString = connStrDict[dbKey];
             }
             else
             {
                 connString = Configuration[$"ConnectionStrings:{dbKey}"];
                 connStrDict.TryAdd(dbKey, connString);
             }
             return connString;
         }


        public static IDbConnection GetConnection(string dbKey, DbType dbType = DbType.SqlServer)
         {
             IDbConnection connObj = null;
             switch (dbType)
             {
                 case DbType.SqlServer:
                     connObj = new SqlConnection(GetConnString(dbKey));
                     break;
                 case DbType.MySql:
                    //connObj = new MySqlConnection(GetConnString(dbKey));
                     break;
                 case DbType.Access:
                     //connObj = new OleDbConnection(GetConnString(dbKey));
                     break;
                 case DbType.SqlLite:
                     break;
                 case DbType.Oracle:
                     break;
             }
 
             if (connObj.State != ConnectionState.Open)
             {
                 connObj.Open();
             }

             return connObj;
        }


         /// <summary>
         /// 获取数据连接
         /// </summary>
         /// <param name="connectionString"></param>
         /// <param name="dbType"></param>
         /// <returns></returns>
         public static IDbConnection GetConnectionByConnString(string connString, DbType dbType = DbType.SqlServer)
         {
             IDbConnection connObj = null;
             switch (dbType)
             {
                 case DbType.SqlServer:
                     connObj = new SqlConnection(connString);
                     break;
                 case DbType.MySql:
                     //connObj = new MySqlConnection(connString);
                     break;
                 case DbType.Access:
                     //connObj = new OleDbConnection(connString);
                     break;
                 case DbType.SqlLite:
                     break;
                 case DbType.Oracle:
                     break;
             }
 
             if (connObj.State != ConnectionState.Open)
             {
                 connObj.Open();
             }

             return connObj;
         }
    }
}
