using System;
using System.Collections.Generic;
using System.Threading;
using CLDC_DataCore.Struct;
using CLDC_DataCore.Model.DnbModel.DnbInfo;

namespace CLDC_DataCore.Model.DnbModel
{
    /// <summary>
    /// 可以被序列化，网络传输模型问题，断电损坏数据问题，模型改动反序列化问题
    /// 改为临时数据库
    /// </summary>
    [Serializable()]
    public class DnbGroupInfo : CLDC_Comm.SerializationBytes
    {

        #region 电能表总模型 、以及同步处理相关 public List<MeterBasicInfo> MeterGroup
        private object ObjMeterGroupLock = new object();


        private List<MeterBasicInfo> m_MeterGroup = new List<MeterBasicInfo>();

        /// <summary>
        /// 获取MeterGroup的使用权、使用完毕以后必须调用MeterGroupExit()
        /// 1、MeterGroupEnter() 和 MeterGroupExit() 必须成对使用、不能凭空调用MeterGroupExit()
        /// 2、MeterGroupExit() 必须在调用MeterGroupEnter() 的同一个线程
        /// </summary>
        public void MeterGroupEnter()
        {
            Monitor.Enter(MeterGroup);
        }

        /// <summary>
        /// 释放 MeterGroup 的使用权、使用完毕以后必须调用MeterGroupExit()
        /// 1、MeterGroupEnter() 和 MeterGroupExit() 必须成对使用、不能凭空调用MeterGroupExit()
        /// 2、MeterGroupExit() 必须在调用MeterGroupEnter() 的同一个线程
        /// </summary>
        public void MeterGroupExit()
        {
            Monitor.Exit(MeterGroup);
        }

        /// <summary>
        /// 表信息组，信息组中一个元素为一只表
        /// </summary>
        public List<MeterBasicInfo> MeterGroup
        {
            get
            {
                if (ObjMeterGroupLock == null)
                {
                    ObjMeterGroupLock = new object();
                }
                lock (ObjMeterGroupLock)
                {
                    return m_MeterGroup;
                }
            }
            set
            {
                if (ObjMeterGroupLock == null)
                {
                    ObjMeterGroupLock = new object();
                }
                lock (ObjMeterGroupLock)
                {
                    m_MeterGroup = value;
                }
            }
        }
        #endregion

        #region -------------------方案相关----------------------------------------------
        /// <summary>
        /// 电能表检定方案
        /// </summary>
        private List<object> m_CheckPlan = new List<object>();

        /// <summary>
        /// 所使用的方案的名称
        /// </summary>
        private string m_FaName = "";

        /// <summary>
        /// 参照圈数
        /// </summary>
        private int m_CzQs = 1;

        /// <summary>
        /// 参照电流倍数
        /// </summary>
        private string m_CzIb = "1.0Ib";

        /// <summary>
        /// 最小圈数，下标0有功，下标1无功
        /// </summary>
        public int[] MinConst = new int[2];

        /// <summary>
        /// 误差上限比率（默认100%）
        /// </summary>
        private float m_WcxUp = 1F;
        /// <summary>
        /// 误差下限比率（默认100%）
        /// </summary>
        private float m_WcxDown = 1F;

        /// <summary>
        /// 统一误差上限比率（只读）
        /// </summary>
        public float WcxUpPercent
        {
            get
            {
                return m_WcxUp;
            }
        }
        /// <summary>
        /// 统一误差下限比率（只读）
        /// </summary>
        public float WcxDownPercent
        {
            get
            {
                return m_WcxDown;
            }
        }
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="Up"></param>
        /// <param name="Down"></param>
        public void SetWcxPercent(float Up, float Down)
        {
            m_WcxUp = Up;
            m_WcxDown = Down;
        }
        /// <summary>
        /// 误差限参照
        /// </summary>
        private string m_CzWcLimit = "规程误差限";

        /// <summary>
        /// 电能表检定方案（只读）,必须在调用CreateFA方法后才可使用
        /// </summary>
        public List<object> CheckPlan
        {
            get
            {
                return m_CheckPlan;
            }
        }
        /// <summary>
        /// 外部设置参照圈数和参照电流倍数
        /// </summary>
        /// <param name="Qs">圈数</param>
        /// <param name="xIb">电流倍数</param>
        public void SetCzQsIb(int Qs, string xIb)
        {
            if (Qs > 0) m_CzQs = Qs;
            if (xIb != string.Empty) m_CzIb = xIb;
        }
        /// <summary>
        /// 参照电流倍数（只读）
        /// </summary>
        public string CzIb
        {
            get
            {
                return m_CzIb;
            }
        }

        /// <summary>
        /// 参照圈数（只读）
        /// </summary>
        public int CzQs
        {
            get
            {
                return m_CzQs;
            }
        }
        /// <summary>
        /// 参照误差限（只读）
        /// </summary>
        public string CzWcLimit
        {
            get
            {
                return m_CzWcLimit;
            }
        }

        #endregion

        #region 字段、属性

        /// <summary>
        /// 台体编号
        /// </summary>
        public readonly int _TaiID = 0;

        /// <summary>
        /// 表位数
        /// </summary>
        public readonly int _Bws = 0;

        /// <summary>
        /// 当前试验时间，仅针对预热，启动，潜动，走字，多功能有效
        /// </summary>
        public float NowMinute = 0;

        /// <summary>
        /// 检定状态
        /// </summary>
        /// <remarks>
        /// 修改检定状态支持按位重叠。
        /// </remarks>
        public CLDC_Comm.Enum.Cus_CheckStaute CheckState = CLDC_Comm.Enum.Cus_CheckStaute.停止检定;

        /// <summary>
        /// 偏差检定次数，在偏差检定时窗体表格构建的时候会用到
        /// </summary>
        public int PcCheckNumic = 0;
        /// <summary>
        /// 误差检定次数，在误差检定时窗体表格构建的时候会用到
        /// </summary>
        public int WcCheckNumic = 0;

        /// <summary>
        /// 当前检定ID，-1表示参数录入，-2表示方案配置，-3表示审核存盘，0~N表示当前检定点
        /// </summary>
        private int m_ItemID = -1;

        /// <summary>
        /// 当前检定项目ID，-1表示参数录入，-2表示方案配置，-3表示审核存盘，0~N表示当前检定点
        /// </summary>
        public int ActiveItemID
        {
            set { m_ItemID = value; }
            get { return m_ItemID; }
        }


        /// <summary>
        /// 准确标志当前检定项目的下标
        /// 参数录入完毕按纽将其设置为 -1
        /// 创建检定方案按钮将其设置为 -2
        /// 其他时候：为实际检定方案进度的下标
        /// </summary>
        public int CheckProgressIndex = -1;
        #endregion

        #region 构造，方法
        /// <summary>
        /// 台体编号
        /// </summary>
        /// <param name="Bws">表位号</param>
        /// <param name="TaiID">台体编号</param>
        public DnbGroupInfo(int Bws, int TaiID)
        {
            _Bws = Bws;
            _TaiID = TaiID;
            this.Init();
        }

        /// <summary>
        /// 获取第一只要检表
        /// </summary>
        /// <returns></returns>
        public int GetFirstYaoJianMeterBwh()
        {
            if (this.MeterGroup.Count == 0) return -1;
            for (int i = 0; i < this.MeterGroup.Count; i++)
            {
                if (this.MeterGroup[i].YaoJianYn)
                {
                    return i;
                }

            }
            return -1;
        }

        /// <summary>
        /// 初始化表信息模型
        /// </summary>
        private void Init()
        {
            MeterGroup = new List<MeterBasicInfo>();
            for (int _I = 0; _I < _Bws; _I++)
            {
                MeterGroup.Add(new MeterBasicInfo(_I + 1));
            }
        }
        /// <summary>
        /// 根据表位号获取表基本信息
        /// </summary>
        /// <param name="intBno"></param>
        /// <returns></returns>
        public MeterBasicInfo GetMeterBasicInfoByBwh(int intBno)
        {
            foreach (MeterBasicInfo info in MeterGroup)
            {
                if (info.Mb_intBno == intBno)
                    return info;
            }
            return null;
        }

        #endregion

    }
}
