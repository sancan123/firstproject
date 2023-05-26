using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using Npgsql;
using System.Data.OleDb;
using System.Data.SQLite;
using Oracle.ManagedDataAccess.Client;
//using Microsoft

namespace Common
{
    public class DbConnectionFactory
    {
        /// <summary>
        /// 数据库连接字符串缓存
        /// </summary>
        private static ConcurrentDictionary<string, string> connStrDict = new ConcurrentDictionary<string, string>();
        //private static IConfiguration? Configuration { get; }

        private static string GetConnString(string dbKey)
        {
            string connString = string.Empty;
            if (connStrDict.Keys.Contains(dbKey))
            {
                connString = connStrDict[dbKey];
            }
            else
            {
                //connString = Configuration[$"ConnectionStrings:{dbKey}"];
                connStrDict.TryAdd(dbKey, connString);
            }
            return connString;
        }


        public static IDbConnection GetConnection(string dbKey, DBType dbType = DBType.SqlServer)
        {
            IDbConnection connObj = null;
            switch (dbType)
            {
                case DBType.SqlServer:
                    connObj = new SqlConnection(GetConnString(dbKey));
                    break;
                case DBType.MySql:
                    connObj = new MySqlConnection(GetConnString(dbKey));
                    break;
                case DBType.Access:
                    connObj = new OleDbConnection(GetConnString(dbKey));
                    break;
                case DBType.SqlLite:
                    connObj = new SQLiteConnection(GetConnString(dbKey));
                    break;
                case DBType.Oracle:
                    connObj = new OracleConnection(GetConnString(dbKey));
                    break;
                case DBType.PostgreSQL:
                    new NpgsqlConnection(GetConnString(dbKey));
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
        public static IDbConnection GetConnectionByConnString(string connString, DBType dbType = DBType.SqlServer)
        {
            IDbConnection connObj = null;
            switch (dbType)
            {
                case DBType.SqlServer:
                    connObj = new SqlConnection(connString);
                    break;
                case DBType.MySql:
                    connObj = new MySqlConnection(connString);
                    break;
                case DBType.Access:
                    connObj = new OleDbConnection(connString);
                    break;
                case DBType.SqlLite:
                    connObj = new SQLiteConnection(connString);
                    break;
                case DBType.Oracle:
                    connObj = new OracleConnection(connString);
                    break;
                case DBType.PostgreSQL:
                    new NpgsqlConnection(GetConnString(connString));
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
