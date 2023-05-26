using System.Configuration;
using System.Data;
using System.Data.OleDb;

namespace Web.DataBase
{
    public class AccessHelper
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string Connstr = "";
        /// <summary>
        /// 查询方法
        /// </summary>
        /// <returns></returns>
        public static DataTable Query(string sqlText)
        {
            //OleDbConnection
            return new DataTable();
        }
        /// <summary>
        /// 增删改方法
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int ExcuteSql(string sql)
        {
            return 0;
        }
    }
}
