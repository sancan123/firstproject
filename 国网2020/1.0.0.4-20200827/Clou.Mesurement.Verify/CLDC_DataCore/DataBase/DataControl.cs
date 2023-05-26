using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OleDb;
using System.Windows.Forms;

namespace CLDC_DataCore.DataBase
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class DataControl
    {
        /// <summary>
        /// ACCESS数据库连接字符串常数
        /// </summary>
        const string CONST_ACCESS = "Provider=Microsoft.Jet.OLEDB.4.0;Persist Security Info=True;Jet OLEDB:DataBase Password=;Data Source=";
        /// <summary>
        /// 数据库连接对象
        /// </summary>
        protected OleDbConnection _Con;
        /// <summary>
        /// 数据库记录集对象
        /// </summary>
        private OleDbDataReader _Reader;
        /// <summary>
        /// Access数据库路径
        /// </summary>
        private string _sTrAccessPath = "";
        /// <summary>
        /// SQL服务器IP地址
        /// </summary>
        private string _Ip = "";
        /// <summary>
        /// SQL用户名
        /// </summary>
        private string _User = "";
        /// <summary>
        /// SQL密码
        /// </summary>
        private string _Pwd = "";
        /// <summary>
        /// SQL数据库名称
        /// </summary>
        const string _DbName = "CLOUMETERDATASERVER";//CLOUSERVERDATACENTER

        private bool _Connection = false;

        /// <summary>
        /// 检查数据是否连接（只读）
        /// </summary>
        public bool Connection
        {
            get
            {
                return _Connection;
            }
        }

        /// <summary>
        /// 无参数构造，默认ACCESS路径为程序目录下DATABASE文件夹,正式库
        /// </summary>
        /// 
        public DataControl()
        {
            _sTrAccessPath = System.Windows.Forms.Application.StartupPath + "\\Database\\MeterData.mdb";//ClouDnbInfo
            if (!System.IO.File.Exists(_sTrAccessPath))
            {
                CreateMdb DataMdb = new CreateMdb(_sTrAccessPath);
                DataMdb.CreateDataDb();
                DataMdb = null;
            }
            OpenDB();
        }
        /// <summary>
        /// 一个参数构造，设置ACCESS路径,
        /// </summary>
        /// <param name="Tmpor">true正式库，false临时库</param>
        public DataControl(bool Tmpor)
        {
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(string.Format(@"{0}..\",System.Windows.Forms.Application.StartupPath ));

            _sTrAccessPath = dir.Parent.FullName + "\\Database\\MeterDataTmp.mdb";
            OpenDB();
        }
        /// <summary>
        /// 一个参数构造，设置ACCESS路径,
        /// </summary>
        /// <param name="SavePath"></param>
        /// <param name="Absor">true绝对路径，false程序下相对路径“\\...”</param>
        public DataControl(string SavePath,bool Absor)
        {
            if (false == Absor)
            {
                _sTrAccessPath = System.Windows.Forms.Application.StartupPath + SavePath;
            }
            else
            {
                _sTrAccessPath = SavePath;
            }
            OpenDB();
        }
        /// <summary>
        /// 三个参数构造，设置服务器IP地址
        /// </summary>
        /// <param name="Ip"></param>
        /// <param name="User"></param>
        /// <param name="Pwd"></param>
        public DataControl(string Ip, string User, string Pwd)
        {
            _Ip = Ip;
            _User = User;
            _Pwd = Pwd;
            OpenDB();

        }
        /// <summary>
        /// 
        /// </summary>
        ~DataControl()
        {
            //CloseDB();//句柄已经释放了，这句不要放这里，fjk屏蔽
        }


        /// <summary>
        /// 
        /// </summary>
        public OleDbConnection Con
        {
            get
            {
                return _Con;
            }
        }


        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <returns></returns>
        private void OpenDB()
        {
            string ConString = "";
            if (_sTrAccessPath == "")
            {
                _Ip = _Ip.Replace("1433", "");
                _Ip = _Ip.Replace(",", "");
                _Ip = _Ip.Replace("，", "").Trim();
                ConString = @"Provider=SQLOLEDB.1;Data Source=" + _Ip
                + ",1433;User ID=" + _User
                + ";Password=" + _Pwd
                 + ";Initial Catalog="
                + _DbName + ";Persist Security Info=True;";
            }
            else
            {
                ConString = CONST_ACCESS + _sTrAccessPath;
            }
            if (_Con != null)
            {
                _Con.Close();
                _Con.Dispose();
                OleDbConnection.ReleaseObjectPool();
            }
            _Con = new OleDbConnection(ConString);
            try
            {
                _Con.Open();
                _Connection = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                _Con.Dispose();
                OleDbConnection.ReleaseObjectPool();
            }
        }
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void CloseDB()
        {
            try
            {
                if (this._Con.State != System.Data.ConnectionState.Closed)
                {
                    this._Con.Close();
                    OleDbConnection.ReleaseObjectPool();
                }
            }
            catch (Exception ex)
            {
                Const.GlobalUnit.Logger.Error("关闭数据源时出错:" + ex);
            }
        }
        /// <summary>
        /// 执行SQL语句 返回影响的记录数
        /// </summary>
        /// <param name="SQLString"></param>
        /// <returns></returns>
        public  int ExecuteSql(string SQLString)
        {
            System.Data.OleDb.OleDbCommand Cmd = new System.Data.OleDb.OleDbCommand();
            Cmd.Connection = _Con;
            string sql = "";
            try
            {
                if (_sTrAccessPath == "")
                    sql = SQLString.Replace("#", "'");//如果是SQL数据库则需要将#替换为'
                else
                    sql = SQLString;
                Cmd.CommandText = sql;
                int i = Cmd.ExecuteNonQuery();
                Cmd = null;
                return i;
            }
            catch (OleDbException e)
            {
                Const.GlobalUnit.Logger.Fatal("向数据库插入数据失败:" + sql, e);
                return 0;
            }
            catch (Exception ex)
            {
                Const.GlobalUnit.Logger.Fatal("向数据库插入数据失败:", ex);
                return 0;
            }
        }
        /// <summary>
        /// 执行SQL语句 返回reader
        /// </summary>
        /// <param name="SQLString"></param>
        /// <returns></returns>
        public OleDbDataReader ExecuteReaderSql(string SQLString)
        {
            System.Data.OleDb.OleDbCommand Cmd = new System.Data.OleDb.OleDbCommand();
            Cmd.Connection = _Con;
            string sql = "";
            try
            {
                if (_sTrAccessPath == "")
                    sql = SQLString.Replace("#", "'");//如果是SQL数据库则需要将#替换为'
                else
                    sql = SQLString;
                Cmd.CommandText = sql;
               _Reader  = Cmd.ExecuteReader ();
                Cmd = null;
                return _Reader;
            }
            catch (OleDbException e)
            {
                Const.GlobalUnit.Logger.Fatal("向数据库插入数据失败:" + sql, e);
                return null ;
            }
            catch (Exception ex)
            {
                Const.GlobalUnit.Logger.Fatal("向数据库插入数据失败:", ex);
                return null ;
            }
        }
        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>		
        public bool ExecuteSqlTran(List<string> SQLStringList)
        {
            if (0 == SQLStringList.Count) return false;
            OleDbCommand Cmd = new OleDbCommand();    //声明一个命令对象
            OleDbTransaction _Tx = _Con.BeginTransaction();     //实例化一个事务对象
            Cmd.Connection = _Con;
            Cmd.Transaction = _Tx;    //引用事务
            try
            {
                for (int i = 0; i < SQLStringList.Count; i++)
                {
                    string sql = SQLStringList[i];
                    if (sql.Trim().Length > 3)
                    {
                        Cmd.CommandText = sql;
                        Cmd.ExecuteNonQuery();
                    }
                }
                _Tx.Commit();
                _Tx.Dispose();
                Cmd.Dispose();
                return true;
            }
            catch (OleDbException e)
            {
                e.ToString();
                _Tx.Rollback();
                _Tx.Dispose();
                Cmd.Dispose();
                return false;
            }
        }
        /// <summary>
        /// 将数据写入数据库
        /// </summary>
        /// <param name="InsertSQLString">写入数据库的SQL语句</param>
        /// <param name="ErrString">写入数据库的SQL语句</param>
        /// <returns></returns>
        public bool SaveData(List<string> InsertSQLString, out string ErrString)
        {
            Const.GlobalUnit.Logger.Debug("BeginCalling SaveData");
            if (_Con.State == System.Data.ConnectionState.Closed) _Con.Open();
                OleDbCommand _Com = new OleDbCommand();    //声明一个命令对象
                OleDbTransaction _Tx = _Con.BeginTransaction();     //实例化一个事务对象
                _Com.Connection = _Con;
                _Com.Transaction = _Tx;    //引用事务
                int int_i = 0;
                try
                {
                    DateTime startTime = DateTime.Now;
                    for (int_i = 0; int_i < InsertSQLString.Count; int_i++)
                    {
                        string _SqlString = "";
                        if (_sTrAccessPath == "")
                            _SqlString = InsertSQLString[int_i].Replace("#", "'");//如果是SQL数据库则需要将#替换为'
                        else
                            _SqlString = InsertSQLString[int_i].ToString();
                        _Com.CommandText = _SqlString;//插入数据
                        if (_Com.ExecuteNonQuery() < 1 && _SqlString.ToUpper().IndexOf("DELETE") == -1)
                        {
                        Const.GlobalUnit.Logger.Fatal("插入失败");
                        } //执行
                    }
                    _Tx.Commit();  //提交事务
                    _Tx.Dispose();
                    _Com.Dispose();
                    ErrString = "";
                    return true;
                }
                catch (OleDbException e)
                {
                    _Tx.Rollback();   //回滚事务
                    _Tx.Dispose();
                    _Com.Dispose();
                    ErrString = e.Message;
                Const.GlobalUnit.Logger.Fatal("向数据库插入数据失败:" + InsertSQLString[int_i], e);
                    return false;
                }
                catch (Exception ex)
                {
                    _Tx.Rollback();
                    _Tx.Dispose();
                    _Com.Dispose();
                    ErrString = ex.Message;
                Const.GlobalUnit.Logger.Fatal("向数据库插入数据失败:", ex);
                    return false;
                }

                finally
                {

                }
            
        }
        /// <summary>
        /// 读取最大自动编号并且+1
        /// </summary>
        /// <param name="ErrString">输出错误编号</param>
        /// <returns></returns>
        public long ReadMaxAutoID2(out string ErrString)
        {
            OleDbCommand _Com = _Con.CreateCommand();

            _Com.CommandText = "Select Lng_AutoID AS ID From CreateIDTable where sTrTmpValue='maxID'";
            _Reader = _Com.ExecuteReader();
            _Reader.Read();
            long _Return = long.Parse(_Reader["ID"].ToString()) + 1;
            _Reader.Close();
            _Reader = null;

            _Com.CommandText = "update CreateIDTable set Lng_AutoID='" + _Return + "' where  sTrTmpValue='maxID'";
            _Com.ExecuteNonQuery();
            _Com = null;
            ErrString = "";
            return _Return;
        }

        /// <summary>
        /// 读取最大自动编号并且+1
        /// </summary>
        /// <param name="ErrString">输出错误编号</param>
        /// <returns></returns>
        public long ReadMaxAutoID(out string ErrString)
        {
            if (_Con.State == System.Data.ConnectionState.Closed) _Con.Open();
            OleDbCommand _Com = _Con.CreateCommand();
            _Com.CommandText = "Insert into CreateIDTable(sTrTmpValue) Values ('')";
            _Com.ExecuteNonQuery();
            _Com.CommandText = "Select MAX(Lng_AutoID) AS ID From CreateIDTable";
            _Reader = _Com.ExecuteReader();
            _Reader.Read();
            long _Return = long.Parse(_Reader["ID"].ToString());
            _Reader.Close();
            _Reader = null;
            _Com = null;
            ErrString = "";
            return _Return;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>


        public string GetData(string sql)
        {
            string str = "";
            try
            {
                string connectionString ="" ;
               
                    if (_Con.State == System.Data.ConnectionState.Closed) _Con.Open();
                    DbCommand command = new OleDbCommand();
                    command = new OleDbCommand(sql, _Con as OleDbConnection);
                    DbDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        str = (string)reader[0];
                    }
                
            }
            catch (Exception e)
            {
               
            }
            return str;

        }
    }
}
