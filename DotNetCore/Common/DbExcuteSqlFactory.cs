using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using Npgsql;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace Common
{
    public class DbExcuteSqlFactory
    {
        public DBType _DBType;
        public DbExcuteSqlFactory(DBType dbtype) { 
        
          this._DBType =dbtype;        
        }
        public IDbConnection dbConnection;
        public IDbCommand dbCommand;
        public IDbDataAdapter dbDataAdapter;

        public DataTable Query(string sql,DBType dbType)
        {
            DataTable dt= new DataTable();
            switch (dbType)
            {
                case DBType.SqlServer:
                    //dt = new SqlConnection(connString);
                    break;
                case DBType.MySql:
                    //connObj = new MySqlConnection(connString);
                    break;
                case DBType.Access:
                    //connObj = new OleDbConnection(connString);
                    break;
                case DBType.SqlLite:
                    //connObj = new SQLiteConnection(connString);
                    break;
                case DBType.Oracle:
                    //dt = OracleHelper.;
                    break;
                case DBType.PostgreSQL:
                    //new NpgsqlConnection(GetConnString(connString));
                    break;
            }
            return dt;
        }
        public int ExcuteNonQuery(string sql, DBType dbType)
        {
            int result= 0;
            switch (dbType)
            {
                case DBType.SqlServer:
                    //dt = new SqlConnection(connString);
                    break;
                case DBType.MySql:
                    //connObj = new MySqlConnection(connString);
                    break;
                case DBType.Access:
                    //connObj = new OleDbConnection(connString);
                    break;
                case DBType.SqlLite:
                    //connObj = new SQLiteConnection(connString);
                    break;
                case DBType.Oracle:
                    //dt = OracleHelper.;
                    break;
                case DBType.PostgreSQL:
                    //new NpgsqlConnection(GetConnString(connString));
                    break;
            }
            return 0;
        }
    }
}
