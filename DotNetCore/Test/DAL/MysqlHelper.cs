using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace DAL
{
    public class MysqlHelper
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString = ConfigurationManager.ConnectionStrings["DBKey"].ToString();
        /// <summary>
        ///增删改(无参)
        /// </summary>
        /// <param name="sqlText"></param>
        /// <returns></returns>
        public int ExcuteNonQuery(string sqlText)
        {
            int result;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand())
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
            using (MySqlConnection conn = new MySqlConnection())
            {
                conn.Open();
                MySqlDataAdapter dap = new MySqlDataAdapter(sql, conn);
                dap.SelectCommand.CommandType = CommandType.StoredProcedure;
                dap.SelectCommand.Parameters.AddRange(paras);
                dap.Fill(dt);
                conn.Close();
            }
            return dt;
        }
    }
}
