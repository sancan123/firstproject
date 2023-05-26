using System;

namespace CLDC_DataCore.Const
{

    public class Variable
    {
        /// <summary>
        /// Grid表格常态颜色
        /// </summary>
        public static System.Drawing.Color Color_Grid_Normal = System.Drawing.Color.FromArgb(250, 250, 250);

        /// <summary>
        /// Grid表格间隔行颜色
        /// </summary>
        public static System.Drawing.Color Color_Grid_Alter = System.Drawing.Color.FromArgb(235, 250, 235);

        /// <summary>
        /// 固定行（列）颜色
        /// </summary>
        public static System.Drawing.Color Color_Grid_Frone = System.Drawing.Color.FromArgb(225, 225, 225);
        /// <summary>
        ///不合格颜色
        /// </summary>
        public static System.Drawing.Color Color_Grid_BuHeGe = System.Drawing.Color.Red;



        /// <summary>
        /// 合格文本内容
        /// </summary>
        public const string CTG_HeGe = "合格";
        /// <summary>
        /// 未出结论前的默认显示
        /// </summary>
        public const string CTG_DEFAULTRESULT = "--";
        /// <summary>
        /// 不合格文本内容
        /// </summary>
        public const string CTG_BuHeGe = "不合格";

        /// <summary>
        /// 合格标记。
        /// </summary>
        public const string CMG_HeGe = "√";

        /// <summary>
        /// 不合格标志
        /// </summary>
        public const string CMG_BuHeGe = "×";
        /// <summary>
        /// 服务器断开连接消息提示文本
        /// </summary>
        public const string CTG_SERVERUNCONNECT = "服务器断开连接";
        /// <summary>
        /// 项目检定完毕提示文本
        /// </summary>
        public const string CTG_VERIFYOVER = "所有项目检定完毕";
        /// <summary>
        /// 启动网络服务成功文本
        /// </summary>
        public const string CTG_CONNECTSERVERSUCCESS = "启动网络服务成功";
        /// <summary>
        /// 主控文本标识
        /// </summary>
        public const string CTG_CONTROLMODEL_CONTROL = "主控";
        /// <summary>
        /// 被控文本标识
        /// </summary>
        public const string CTG_CONTROLMODEL_BECONTROL = "被控";
        /// <summary>
        /// 没有出误差默认值
        /// </summary>
        public const float WUCHA_INVIADE = -999F;

        /// <summary>
        /// 台体类型,1-单相台,0-三相台
        /// </summary>
        public const string CTC_DESKTYPE = "DESKTYPE";
        #region ----------------误差检定相关设置-------------
        /// <summary>
        /// 每个误差点取几次误差参与计算
        /// </summary>
        public const string CTC_WC_TIMES_BASICERROR = "TIMES_BASICERROR";
        /// <summary>
        /// 标准偏差取几次误差参与计算
        /// </summary>
        public const string CTC_WC_TIMES_WINDAGE = "TIMES_WINDAGE";
        /// <summary>
        /// 每个点误差最大处理次数
        /// </summary>
        public const string CTC_WC_MAXTIMES = "WC_MAXTIMES";
        /// <summary>
        /// 每个点最大检定时间
        /// </summary>
        public const string CTC_WC_MAXSECONDS = "WC_MAXSECONDS";
        /// <summary>
        /// 跳差倍数判定
        /// </summary>
        public const string CTC_WC_JUMP = "WC_JUMP";
        /// <summary>
        /// IN电流
        /// </summary>
        public const string CTC_WC_IN = "WC_IN";
        /// <summary>
        /// 平均值保留小数位
        /// </summary>
        public const string CTC_WC_AVGPRECISION = "AVGPRECISION";
        /// <summary>
        /// 误差限字典文件
        /// </summary>
        public const string CONST_WCLIMIT = "\\Const\\WcLimit.Mdb";

        /// <summary>
        /// 标准脉冲分频系数，如果是科陆标准表则为1
        /// </summary>
        public const string CTC_DRIVERF = "DRIVERF";
        #endregion

        #region 多功能检定配置
        /// <summary>
        /// 多功能应用层发送数据后最大等待时间
        /// </summary>
        public const string CTC_DGN_MAXWAITDATABACKTIME = "MAXWAITDATABACKTIME";
        /// <summary>
        /// 多功能检定源稳定操作时间
        /// </summary>
        public const String CTC_DGN_POWERON_ATTERTIME = "POWERON_ATTERTIME";
        /// <summary>
        /// 当要对表进行写操作时发出提示
        /// </summary>
        public const string CTC_DGN_WRITEMETERALARM = "WRITEMETERALARM";
        /// <summary>
        /// 日计时误差检定类型:快速模式|标准模式
        /// </summary>
        public const string CTC_DGN_RJSVERIFYTYPE = "RJSVERIFYTYPE";

        /// <summary>
        /// 
        /// </summary>
        public const string CTC_DGN_READDATAFROMRS485 = "READDATAFROMRS485";
        #endregion

        #region 相关配置文件

        /// <summary>
        /// 载波方案配置文件
        /// </summary>
        public const string CONST_CARRIER = "\\Const\\CarrierProtocol.xml";

        #endregion 

    }
}
