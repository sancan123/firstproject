using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DAL
{
    public class DBHelper
    {
        //数据库连接字符串
        public string ConnectionString = "";

        /// <summary>
        ///增删改(无参)
        /// </summary>
        /// <param name="sqlText"></param>
        /// <returns></returns>
        public int ExcuteNonQuery(string sqlText)
        {
            int result;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = sqlText;
                    result = cmd.ExecuteNonQuery();
                }
            }
            return result;
        }

        /// <summary>
        /// 查询(无参)
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql, SqlParameter[] paras)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection())
            {
                conn.Open();
                SqlDataAdapter dap = new SqlDataAdapter(sql, conn);
                dap.SelectCommand.CommandType = CommandType.StoredProcedure;
                dap.SelectCommand.Parameters.AddRange(paras);
                dap.Fill(dt);
                conn.Close();
            }
            return dt;
        }
        public void PrerepareCommand(SqlConnection conn)
        {

        }
    }
}
