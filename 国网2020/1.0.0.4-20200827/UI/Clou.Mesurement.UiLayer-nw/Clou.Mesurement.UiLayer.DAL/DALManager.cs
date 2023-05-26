﻿using System.IO;

namespace Mesurement.UiLayer.DAL
{
    /// <summary>
    ///  数据存取层调用管理类,包含当前应用程序要用到的数据库存取类
    /// </summary>
    public class DALManager
    {
        private static GeneralDal applicationDbDal;
        /// <summary>
        /// 应用程序数据库存取类
        /// </summary>
        public static GeneralDal ApplicationDbDal
        {
            get
            {
                if (applicationDbDal == null)
                {
                    string connString = string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}\Database\ApplicationDB.mdb;jet oledb:database password=csg", Directory.GetCurrentDirectory());
                    applicationDbDal = new GeneralDal(connString); 
                }
                return applicationDbDal;
            }
        }
        private static GeneralDal meterTempDbDal;
        /// <summary>
        /// 临时表数据存取类
        /// </summary>
        public static GeneralDal MeterTempDbDal
        {
            get
            {
                if (meterTempDbDal == null)
                {
                    string connString = string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}\Database\MeterDataTmp.mdb", Directory.GetCurrentDirectory());
                    meterTempDbDal = new GeneralDal(connString);
                }
                return meterTempDbDal;
            }
        }
        private static GeneralDal meterDbDal;
        /// <summary>
        /// 正式表数据存取类
        /// </summary>
        public static GeneralDal MeterDbDal
        {
            get
            {
                if (meterDbDal == null)
                {
                    string connString = string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}\Database\MeterData.mdb", Directory.GetCurrentDirectory());
                    meterDbDal = new GeneralDal(connString);
                }
                return meterDbDal;
            }
        }
        private static GeneralDal logDal;
        /// <summary>
        /// 日志数据
        /// </summary>
        public static GeneralDal LogDal
        {
            get
            {
                if (logDal == null)
                {
                    string connString = string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}\Database\Log.mdb", Directory.GetCurrentDirectory());
                    logDal = new GeneralDal(connString);
                }
                return logDal;
            }
        }
        private static GeneralDal protocolDal;
        /// <summary>
        /// 协议标识数据库
        /// </summary>
        public static GeneralDal ProtocolDal
        {
            get
            {
                if (protocolDal == null)
                {
                    string connString = string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}\Database\ClouProtocol.mdb", Directory.GetCurrentDirectory());
                    protocolDal = new GeneralDal(connString);
                }
                return protocolDal;
            }
        }
        private static GeneralDal wcLimitDal;
        /// <summary>
        /// 误差限数据库
        /// </summary>
        public static GeneralDal WcLimitDal
        {
            get
            {
                if (wcLimitDal == null)
                {
                    string connString = string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}\Database\WcLimit.Mdb", Directory.GetCurrentDirectory());
                    wcLimitDal = new GeneralDal(connString);
                }
                return wcLimitDal;
            }
        }
    }
}
