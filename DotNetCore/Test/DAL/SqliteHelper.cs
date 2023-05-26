using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace DAL
{
    public class SqliteHelper
    {

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString = "";
        /// <summary>
        ///增删改(无参)
        /// </summary>
        /// <param name="sqlText"></param>
        /// <returns></returns>
        public int ExcuteNonQuery(string sqlText)
        {
            int result;
            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = sqlText;
                    result = cmd.ExecuteNonQuery();
                }
            }
            return result;
        }

        public static DataTable GetDataTable(string sql, SqlParameter[] paras)
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection conn = new SQLiteConnection())
            {
                conn.Open();
                SQLiteDataAdapter dap = new SQLiteDataAdapter(sql, conn);
                dap.SelectCommand.CommandType = CommandType.StoredProcedure;
                dap.SelectCommand.Parameters.AddRange(paras);
                dap.Fill(dt);
                conn.Close();
            }
            return dt;
        }
    }
}
