using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_MeterProtocol.Ammeter.DLT645.Comm.Struct
{
    /// <summary>
    /// 运行状态
    /// </summary>
    public struct StRunState
    {
        #region ---- 运行状态字1 ----
        //需量积算方式[0-滑差 1-区间]
        //时钟电池[0-正常 1-欠压]
        //停电抄表电池
        //有功功率方向
        //无功功率方向
        #endregion ---- 运行状态字1 ----
        #region ---- 运行状态字2 ----
        //A相有功功率方向
        //B相有功功率方向
        //C相有功功率方向
        //A相无功功率方向
        //B相无功功率方向
        //C相无功功率方向
        #endregion ---- 运行状态字2 ----
        #region ---- 运行状态字3 ----
        //当前运行时段
        /// <summary>
        /// 供电方式[0-主电源 1-辅助电源 2-电池供电]
        /// </summary>
        public byte SupplyType;
        /// <summary>
        /// 编程允许[0-禁止 1-许可]
        /// </summary>
        public byte ProgLicense;
        /// <summary>
        /// 继电器状态[0-通 1-断]
        /// </summary>
        public byte RelayState;
        /// <summary>
        /// 继电器命令状态[0-通 1-断]
        /// </summary>
        public byte RelayCmdState;
        /// <summary>
        /// 预跳闸报警状态[0-无 1-有]
        /// </summary>
        public byte AlarmState;
        //电能表类型
        //当前运行分时费率
        //当前阶梯
        #endregion ---- 运行状态字3 ----
        #region ---- 运行状态字4（A相故障） ----
        //失压
        //欠压
        //过压
        //失流
        //过流
        //过载
        //潮流方向
        //断相
        //电流
        #endregion ---- 运行状态字4 ----
        #region ---- 运行状态字5（B相故障） ----
        //失压
        //欠压
        //过压
        //失流
        //过流
        //过载
        //潮流方向
        //断相
        //电流
        #endregion ---- 运行状态字5 ----
        #region ---- 运行状态字6（C相故障） ----
        //失压
        //欠压
        //过压
        //失流
        //过流
        //过载
        //潮流方向
        //断相
        //电流
        #endregion ---- 运行状态字6 ----
        #region ---- 运行状态字7 ----
        //电压逆相序
        //电流逆相序
        //电压不平衡
        //电流不平衡
        //辅助电源失电
        //掉电
        //需量超限
        //总功率因数超下限
        //电流严重不平衡
        #endregion ---- 运行状态字7 ----
    }
}
