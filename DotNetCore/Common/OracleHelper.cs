using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace Common
{
    public class OracleHelper
    {
      


        public OracleHelper() { 
        
        
        
        }

        public OracleHelper(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }
        public OracleHelper(string ip, int port, string dataSource, string userId, string pwd, string webUrl)
        {
            this.Ip = ip;
            this.Port = port;
            this.DataSource = dataSource;
            this.UserId = userId;
            this.Password = pwd;
            this.WebServiceURL = webUrl;
        }
        #region 属性
        /// <summary>
        /// 数据库IP地址
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 数据库端口号
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 数据源名称
        /// </summary>
        public string DataSource { get; set; }
        /// <summary>
        /// 数据库登陆用户
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 数据库登陆密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// WebServer路径
        /// </summary>
        public string WebServiceURL { get; set; }

        /// <summary>
        /// 获取当前联接字符串
        /// </summary>
        public string ConnectString
        {
            get
            {
                return string.Format("Data Source=(DESCRIPTION =(ADDRESS =(PROTOCOL = TCP)(HOST = {0})(PORT = {1}))(CONNECT_DATA = (SERVICE_NAME ={2})));User ID={3};Password={4};Persist Security Info=True",
                        Ip, Port, DataSource, UserId, Password);
            }
        }
        #endregion


        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="sqlList">多条SQL语句</param>
        public string Execute(List<string> sqlList)
        {
            string msg = string.Empty;
            using (OracleConnection conn = new OracleConnection(ConnectString))
            {

                conn.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = conn;
                    OracleTransaction tran = conn.BeginTransaction();
                    cmd.Transaction = tran;
                    try
                    {
                        foreach (string s in sqlList)
                        {
                            if (string.IsNullOrEmpty(s)) continue;
                            if (s.Length < 5) continue;

                            cmd.CommandText = s;
                            cmd.ExecuteNonQuery();

                        }
                        tran.Commit();
                        msg = "上传成功";
                    }
                    catch (System.Data.OracleClient.OracleException ex)
                    {
                        tran.Rollback();
                        msg = cmd.CommandText + "\r\n" + ex.Message;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                conn.Close();
            }
            return msg;
        }

        /// <summary>
        /// 执行查询语句，返回OracleDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <returns>OracleDataReader</returns>
        public DataTable ExecuteReader(string sql)
        {
            DataTable dt = new DataTable();
            using (OracleConnection conn = new OracleConnection(ConnectString))
            {
                try
                {
                    conn.Open();
                    OracleDataAdapter adp = new OracleDataAdapter(sql, conn);
                    adp.Fill(dt);
                    adp.Dispose();
                    conn.Close();
                }
                catch (System.Data.OracleClient.OracleException ex)
                {
                    conn.Close();
                }
            }
            return dt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql)
        {
            object o = null;
            using (OracleConnection conn = new OracleConnection(ConnectString))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    o = cmd.ExecuteScalar();
                    conn.Close();
                }
                conn.Close();
            }
            return o;

        }

        public static int ExecuteNonQuery(string sql)
        {
            int count = 0;
            using (OracleConnection conn = new OracleConnection(ConnectString))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    count = cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            return count;
        }

        /// <summary>
        ///执行存储过程（只带输入参数）
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="commandType"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, CommandType commandType, params OracleParameter[] paras)
        {
            int count = 0;
            using (OracleConnection conn = new OracleConnection(ConnectString))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.CommandType = commandType;
                    if (paras!=null)
                    {
                        cmd.Parameters.AddRange(paras);
                    }
                    count = cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            return count;
        }
    }
}
