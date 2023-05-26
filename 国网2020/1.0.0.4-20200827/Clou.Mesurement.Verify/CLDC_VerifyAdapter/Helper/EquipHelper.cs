
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using System.Threading;
using CLDC_Comm;
using CLDC_DataCore.Struct;
using CLDC_Comm.Enum;
using CLDC_Comm.BaseClass;
using CLDC_DataCore.Const;
using System.Text;
using CLDC_DataCore.Function;


namespace CLDC_VerifyAdapter.Helper
{
    /// <summary>
    /// 设备控制单元
    /// </summary>
    public class EquipHelper : SingletonBase<EquipHelper>
    {
        #region----------公有变量----------

        /// <summary>
        /// 当前CT档位
        /// </summary>
        public int m_intCurrentCT = 0;
        /// <summary>
        /// 多功能控制器
        /// </summary>
        //public IAmMeterController m_IMeterControler = null;
        /// <summary>
        /// 当前是否是有功
        /// </summary>

        public bool IsYouGong = true;
        /// <summary>
        /// 是否已经联机
        /// </summary>

        public bool isConnected = false;

        ///// <summary>
        ///// 当前是否已经有源输出
        ///// </summary>
        //public bool isPowerOn = false;

        /// <summary>
        /// 最后一次电源 操作类型
        /// </summary>
        public Cus_PowerWorkFlow powerWorkFlow = Cus_PowerWorkFlow.None;

        ///// <summary>
        ///// 当前功能是否已经启动
        ///// </summary>
        //private bool isControlTask = false;
        #endregion

        #region----------私有变量----------
        //private Adapter m_adapter = null;
        /// <summary>
        /// 设备接口
        /// </summary>
        private CLDC_DeviceDriver.Driver m_DeviceDriver = null;
        /// <summary>
        /// 表位数量
        /// </summary>
        private int m_BwCount = 24;
        /// <summary>
        /// 操作重试次数.每表位对应一个
        /// </summary>
        protected int[] m_RetryTimes;
        //操作失败重试次数
        public int ReTryTimes = 1;
        /// <summary>
        /// 上一次控源信息
        /// </summary>
        public CLDC_DataCore.Struct.StPower LastPowerPara;

        /// <summary>
        /// 上一次源输出时间
        /// </summary>
        public DateTime LastPowerOnTime = DateTime.Now;
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public EquipHelper()
        {
        }



        /// <summary>
        /// 初始化设备控制器
        /// </summary>
        /// <param name="bwCount"></param>
        /// <returns></returns>
        public bool Initialize(int bwCount, string[] deviceArray)
        {
            m_BwCount = bwCount;
            //CLDC_DataCore.Function.TopWaiting.ShowWaiting("初始化设备控制器...");
            try
            {
                m_DeviceDriver = new CLDC_DeviceDriver.Driver(m_BwCount, deviceArray);
                m_DeviceDriver.CallBack -= new CLDC_DeviceDriver.MsgCallBack(equipMessage);
                m_DeviceDriver.CallBack += new CLDC_DeviceDriver.MsgCallBack(equipMessage);

            }
            catch (Exception ex)
            {
                MessageController.Instance.AddMessage(string.Format("初始化设备发生错误!\r\n{0}\r\n具体情况请查看日志文件", ex.Message), 7, 2);
                DataLoger.Log(string.Format("初始化设备控制器发生错误!\r\n{0}\r\n具体情况请查看日志文件", ex.ToString()));
                return false;
            }
            //CLDC_DataCore.Function.TopWaiting.HideWaiting();

            return true;
        }

        #region -------统一台体接口---------

        #region ----------联机/脱机操作----------
        /// <summary>
        /// 联机操作
        /// </summary>
        /// <returns>联机是否成功</returns>
        public bool Link()
        {
            isConnected = m_DeviceDriver.Link();
            //if (isConnected) break;
            //Thread.Sleep(300);
            return isConnected;
        }
        /// <summary>
        /// 脱机操作
        /// </summary>
        /// <returns>脱机是否成功</returns>
        public bool UnLink()
        {
            return true;
        }

        #endregion

        #region 载波
        public void Init2041()
        {
            GlobalUnit.g_CommunType = Cus_CommunType.通讯载波;
            if (CLDC_DataCore.Const.GlobalUnit.CarrierInfo.CarrierName != "中电华瑞2016")
            {
                m_DeviceDriver.PacketToCarrierInit(1, GlobalUnit.Flag_IsZD2016, int.Parse(GlobalUnit.CarrierInfo.Comm));  //  初始化1
                Thread.Sleep(200);
            }
            Thread.Sleep(200);
            m_DeviceDriver.PacketToCarrierInit(2, GlobalUnit.Flag_IsZD2016, int.Parse(GlobalUnit.CarrierInfo.Comm));
            Thread.Sleep(200);
            m_DeviceDriver.PacketToCarrierInit(3, GlobalUnit.Flag_IsZD2016, int.Parse(GlobalUnit.CarrierInfo.Comm));
            Thread.Sleep(200);
            if (CLDC_DataCore.Const.GlobalUnit.CarrierInfo.CarrierName != "中电华瑞2016")
            {
                m_DeviceDriver.PacketToCarrierCtr(1, new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00 }, int.Parse(GlobalUnit.CarrierInfo.Comm));
                Thread.Sleep(200);
            }
            else
            {
                byte[] byteAll = new byte[] { 0x40, 0x04, 0xDD, 0x01, 0x04, 0x02, 0xE8, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27 };//
                m_DeviceDriver.PacketToCarrierAddAddr(0, byteAll, GlobalUnit.Flag_IsZD2016, int.Parse(GlobalUnit.CarrierInfo.Comm));//添加2016宽带 主节点
            }
        }
        /// <summary>
        /// 添加载波从节点
        /// </summary>
        /// <param name="int_Fn"></param>
        /// <param name="str_addr"></param>
        public void AddCarrierNode(int int_Fn, string str_addr)
        {
            byte[] byt_addr = new byte[6];
            int strL = str_addr.Length;
            byte[] by_KD2016 = new byte[] { 0x40, 0x04, 0x09, 0x02, 0x04, 0x02, 0xE8, 0x01 };
            byte[] byteAll = new byte[14];
            for (int i = 0; i < 6; i++)
            {
                byt_addr[i] = byte.Parse(str_addr.Substring(strL - 2 * (i + 1), 2), System.Globalization.NumberStyles.HexNumber);

            }
            if (CLDC_DataCore.Const.GlobalUnit.CarrierInfo.CarrierName == "中电华瑞2016")
            {
                //68 13 00 40 04 09 02 04 02 E8 01 00 23 19 00 00 00 7A 16
                by_KD2016.CopyTo(byteAll, 0);
                byt_addr.CopyTo(byteAll, 8);
                m_DeviceDriver.PacketToCarrierAddAddr(int_Fn, byteAll, GlobalUnit.Flag_IsZD2016, int.Parse(GlobalUnit.CarrierInfo.Comm)); //
                return;
            }
            m_DeviceDriver.PacketToCarrierAddAddr(int_Fn, byt_addr, GlobalUnit.Flag_IsZD2016, int.Parse(GlobalUnit.CarrierInfo.Comm));//
        }
        /// <summary>
        /// 暂停路由
        /// </summary>
        public void PauseRouter()
        {

            m_DeviceDriver.PacketToCarrierInitA(0x12, 2, int.Parse(GlobalUnit.CarrierInfo.Comm));
            Thread.Sleep(200);
        }
        /// <summary>
        /// 打包645成376.2
        /// </summary>
        /// <param name="int_Fn">Fn</param>
        /// <param name="Data">电表地址，反转</param>
        public void PacketTo3762(byte[] Frame645, byte byt_DLTType, ref byte[] RFrame645, bool state, int int_BwIndex)
        {
            RFrame645 = null;
            m_DeviceDriver.PacketTo3762Frame(Frame645, byt_DLTType, ref RFrame645, state, int_BwIndex, int.Parse(GlobalUnit.CarrierInfo.Comm));
        }


        /// <summary>
        /// 启动任务
        /// </summary>
        public void StarCarrier()
        {


            byte[] Data = new byte[] { 0x40, 0x02, 0xDD, 0x08, 0x02, 0x02, 0xE8 };
            m_DeviceDriver.PacketToCarrierAddAddr(1, Data, GlobalUnit.Flag_IsZD2016, int.Parse(GlobalUnit.CarrierInfo.Comm));//启动载波2016宽带 任务 
            return;


        }
        #endregion



        #region

        //public bool DoMeterReadPara()
        //{
        //    //StPlan_Dgn ReadParaPlan = new StPlan_Dgn();
        //    //ReadParaPlan.DgnPrjID = "";
        //    //ReadParaPlan.DgnPrjName = "读取参数";

        //    //ReadParaPlan.OutPramerter.GLFX = Cus_PowerFangXiang.正向有功;
        //    //ReadParaPlan.OutPramerter.GLYS = "1.0";
        //    //ReadParaPlan.OutPramerter.xIb = "0";
        //    //ReadParaPlan.OutPramerter.xU = 1;
        //    //ReadParaPlan.OutPramerter.YJ = Cus_PowerYuanJian.H;


        //    //CLDC_VerifyAdapter.Multi.Dgn_ReadPara readPara = new CLDC_VerifyAdapter.Multi.Dgn_ReadPara(ReadParaPlan);
        //    //readPara.Verify();
        //    return true;
        //}


        #endregion
        #region 检定参数初始化

        /// <summary>
        /// 初始化日计时误差参数，不包括升源
        /// </summary>
        /// <returns>初始化是否成功</returns>
        public bool InitPara_InitTimeAccuracy(float[] MeterFre, float[] bcs, int[] quans)
        {
            bool ret = false;
            for (int i = 0; i < ReTryTimes; i++)
            {
                ret = m_DeviceDriver.InitTimeAccuracy(Helper.MeterDataHelper.Instance.GetYaoJian(), MeterDataHelper.Instance.GetIm(), MeterFre, bcs, quans);
                if (ret) break;
                Thread.Sleep(300);
            }
            return ret;
        }

        /// <summary>
        /// 初始化需量周期误差，不包括升源
        /// </summary>
        /// <param name="demandPeriod">，实际没有用，应该是滑差时间</param>
        /// <param name="slipTimes">滑差次数</param>
        /// <returns>初始化是否成功</returns>
        public bool InitPara_InitDemandPeriod(int demandPeriod, int slipTimes)
        {
            bool ret = false;
            CLDC_VerifyAdapter.Helper.EquipHelper.Instance.SetTimeMaiCon(true);

            for (int i = 0; i < ReTryTimes; i++)
            {
                ret = m_DeviceDriver.InitDemandPeriod(Helper.MeterDataHelper.Instance.GetYaoJian(), MeterDataHelper.Instance.GetIm(), CLDC_Comm.Utils.ArrayHelper.MakeArray(demandPeriod, this.m_BwCount), CLDC_Comm.Utils.ArrayHelper.MakeArray(slipTimes, this.m_BwCount));
                if (ret) break;
                Thread.Sleep(300);
            }
            return ret;
        }
        #endregion

        #region ---------源控制---------

        #region ---------源控制结构体---------
        public struct stPowerPara
        {
            /// <summary>
            /// 测量方式
            /// </summary>
            public CLDC_Comm.Enum.Cus_Clfs clfs;
            /// <summary>
            /// 标定电压(V)
            /// </summary>
            public float sng_Ub;
            /// <summary>
            /// 标定电流(A)
            /// </summary>
            public float sng_Ib;
            /// <summary>
            /// 最大电流(A)(用于防止电压过高损坏台体)
            /// </summary>
            public float sng_IMax;
            /// <summary>
            /// A相电压(V)
            /// </summary>
            public float sng_xUb_A;
            /// <summary>
            /// B相电压(V)
            /// </summary>
            public float sng_xUb_B;
            /// <summary>
            /// C相电压(V)
            /// </summary>
            public float sng_xUb_C;
            /// <summary>
            /// A相电流(A)
            /// </summary>
            public float sng_xIb_A;
            /// <summary>
            /// B相电(A)
            /// </summary>
            public float sng_xIb_B;
            /// <summary>
            /// C相电流(A)
            /// </summary>
            public float sng_xIb_C;
            /// <summary>
            /// 元件
            /// </summary>
            public Cus_PowerYuanJian element;
            /// <summary>
            /// A相电压角度
            /// </summary>
            public float sng_UaPhi;
            /// <summary>
            /// B相电压角度
            /// </summary>
            public float sng_UbPhi;
            /// <summary>
            /// C相电压角度
            /// </summary>
            public float sng_UcPhi;
            /// <summary>
            /// A相电流角度
            /// </summary>
            public float sng_IaPhi;
            /// <summary>
            /// B相电流角度
            /// </summary>
            public float sng_IbPhi;
            /// <summary>
            /// C相电流角度
            /// </summary>
            public float sng_IcPhi;
            /// <summary>
            /// 频率HZ
            /// </summary>
            public float sng_Freq;
            /// <summary>
            /// 是否逆相序
            /// </summary>
            public bool bln_IsNxx;
            /// <summary>
            /// 是否是对标
            /// </summary>
            public bool bln_DuiBiao;
            /// <summary>
            /// 是否是潜动
            /// </summary>
            public bool bln_IsQiangDong;
        }
        #endregion

        /// <summary>
        /// 求数据中的最大数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static T Max<T>(params T[] values) where T : IComparable
        {
            List<T> va = new List<T>();
            va.AddRange(values);
            va.Sort();
            return va[va.Count - 1];
        }

        /// <summary>
        /// 升源
        /// </summary>
        /// <param name="tagPara">控源参数</param>
        /// <returns>结果</returns>
        public bool PowerOn(stPowerPara tagPara, string strGlys, int glfx)
        {
            //进行参数检查
            float maxU = Max(tagPara.sng_xUb_A, tagPara.sng_xUb_B, tagPara.sng_xUb_C);
            float maxI = Max(tagPara.sng_xIb_A, tagPara.sng_xIb_B, tagPara.sng_xIb_C);
            if (CLDC_Comm.LoginSettingData.LoginSetting != null)
            {
                if (maxU > CLDC_Comm.LoginSettingData.LoginSetting.MaxDianYa)
                {
                    MessageController.Instance.AddMessage("当前检定项目的最大电压:" + maxU + "超过保护电压:" + CLDC_Comm.LoginSettingData.LoginSetting.MaxDianYa + "已停止升源");
                    return false;
                }

                if (maxI > CLDC_Comm.LoginSettingData.LoginSetting.MaxDianLiu)
                {

                    MessageController.Instance.AddMessage("当前检定项目的最大电流:" + maxU + "超过保护电流:" + CLDC_Comm.LoginSettingData.LoginSetting.MaxDianLiu + "已停止升源", 7, 2);
                    return false;
                }
            }

            bool result = false;


            for (int i = 0; i < ReTryTimes; i++)
            {
                result = m_DeviceDriver.PowerOn(tagPara.clfs, (Cus_PowerFangXiang)glfx, strGlys, tagPara.sng_Ub, tagPara.sng_Ib, tagPara.sng_IMax,
                       tagPara.sng_xUb_A, tagPara.sng_xUb_B, tagPara.sng_xUb_C,
                       tagPara.sng_xIb_A, tagPara.sng_xIb_B, tagPara.sng_xIb_C,
                       tagPara.element, tagPara.sng_UaPhi, tagPara.sng_UbPhi,
                       tagPara.sng_UcPhi, tagPara.sng_IaPhi, tagPara.sng_IbPhi,
                       tagPara.sng_IcPhi, tagPara.sng_Freq, tagPara.bln_DuiBiao, tagPara.bln_IsQiangDong, tagPara.bln_IsNxx);

                if (result)
                {
                    UpdateLastPowerInfo(tagPara);
                    break;
                }
                Thread.Sleep(300);
            }
            return result;
        }

        /// <summary>
        /// 更新最后一次控源信息
        /// </summary>
        /// <param name="para"></param>
        private void UpdateLastPowerInfo(stPowerPara para)
        {
            LastPowerOnTime = DateTime.Now;//更新源输出时间
            LastPowerPara.Clfs = para.clfs;
            LastPowerPara.Ia = para.sng_xIb_A;
            LastPowerPara.Ib = para.sng_xIb_B;
            LastPowerPara.Ic = para.sng_xIb_C;
            LastPowerPara.Ua = para.sng_xUb_A;
            LastPowerPara.Ub = para.sng_xUb_B;
            LastPowerPara.Uc = para.sng_xUb_C;
            powerWorkFlow = Cus_PowerWorkFlow.PowerOn;
        }

        /// <summary>
        /// 只输出电压[合元,功率因素1.0]
        /// </summary>
        /// <param name="U">电压值/V</param>
        /// <returns>结果</returns>
        public bool PowerOn(float U, int glfx)
        {
            return PowerOn(U, 0, (int)(Cus_PowerYuanJian.H), glfx, "1.0", true, false);
        }
        /// <summary>
        /// 通用控源
        /// 测量方式:GlobalUnit.Clfs
        /// </summary>
        /// <param name="Ub">电压V</param>
        /// <param name="Ib">电流A</param>
        /// <param name="ele">元件</param>
        /// <param name="glys">功率因素,如果是反向请在前加负号</param>
        /// <param name="bYouGong">是否是有功</param>
        /// <returns>操作结果</returns>
        public bool PowerOn(float Ub, float Ib, int ele, int glfx, string glys, bool bYouGong, bool isDuiBiao)
        {
            bool result = PowerOn(Ub, Ub, Ub, Ib, Ib, Ib, ele, GlobalUnit.PL, glys, bYouGong, isDuiBiao, false, glfx);
            Thread.Sleep(5 * 1000);
            return result;
        }
        /// <summary>
        /// 特殊检定升源
        /// </summary>
        /// <param name="Ua">A相电压V</param>
        /// <param name="Ub">B相电压V</param>
        /// <param name="Uc">C相电压V</param>
        /// <param name="Ia">A相电流A</param>
        /// <param name="Ib">B相电流A</param>
        /// <param name="Ic">C相电流A</param>
        /// <param name="glys">功率因素，反向请加-</param>
        /// <param name="bYouGong">是否是有功输出</param>
        /// <param name="nxx">是否逆相序</param>
        /// <returns></returns>
        public bool PowerOn(float Ua, float Ub, float Uc, float Ia, float Ib, float Ic, int yuanjian, float feq, string glys, bool bYouGong, bool isDuiBiao, bool nxx, int glfx)
        {
            stPowerPara tagPara = getPowerPara(Ua, Ub, Uc, Ia, Ib, Ic, (Cus_PowerYuanJian)yuanjian, glys, glfx, nxx, feq);
            tagPara.bln_DuiBiao = isDuiBiao;
            int intCurrentCt = 0;
            //if (!GlobalUnit.IsDan)
            //{
            //    if (tagPara.sng_xIb_A == 0 && tagPara.sng_xIb_B == 0 && tagPara.sng_xIb_C == 0)
            //    {
            //        intCurrentCt = m_intCurrentCT;//只升电压，不换当CT
            //    }
            //    else if (tagPara.sng_xIb_A > 2 || tagPara.sng_xIb_B > 2 || tagPara.sng_xIb_C > 2)
            //    {
            //        intCurrentCt = 100;
            //    }
            //    else
            //    {
            //        intCurrentCt = 2;
            //    }

            //    if (m_intCurrentCT != intCurrentCt)
            //    {
            //        stPowerPara powerPara = new stPowerPara();
            //        powerPara = tagPara;
            //        powerPara.sng_xIb_A = 0;
            //        powerPara.sng_xIb_B = 0;
            //        powerPara.sng_xIb_C = 0;
            //        PowerOn(powerPara, glys, glfx);
            //        //Thread.Sleep(3000);
            //        m_intCurrentCT = intCurrentCt;
            //    }
            //}
            if ((Cus_PowerFangXiang)glfx == Cus_PowerFangXiang.反向有功 || (Cus_PowerFangXiang)glfx == Cus_PowerFangXiang.反向无功)
            {
                if (glys.IndexOf('-') == -1)
                    glys = "-" + glys;
            }
            return PowerOn(tagPara, glys, glfx);
        }

        /// <summary>
        /// 关源
        /// </summary>
        /// <returns>结果</returns>
        public bool PowerOff()
        {


            bool ret = false;
            for (int i = 0; i < ReTryTimes; i++)
            {
                ret = m_DeviceDriver.PowerOff();
                if (ret) break;
                Thread.Sleep(300);
            }
            //关源后把当前源信息清空
            this.powerWorkFlow = Cus_PowerWorkFlow.PowerOff;
            stPowerPara para = new stPowerPara();
            this.UpdateLastPowerInfo(para);
            return ret;
        }

        private stPowerPara getPowerPara(float ua, float ub, float uc, float ia, float ib, float ic,
            Cus_PowerYuanJian ele, string glys, int glfx, bool bNxx, float feq)
        {
            stPowerPara tagPara = new stPowerPara();
            tagPara.clfs = GlobalUnit.Clfs;
            tagPara.sng_Ub = GlobalUnit.U;                //Ub
            //tagPara.sng_Ib = GlobalUnit.Ib;     //Ib
            //tagPara.sng_IMax = GlobalUnit.Imax;
            tagPara.element = ele;
            //U
            tagPara.sng_xUb_A = ua;// / GlobalUnit.U;
            tagPara.sng_xUb_B = ub;// / GlobalUnit.U;
            tagPara.sng_xUb_C = uc;// / GlobalUnit.U;
            //I
            tagPara.sng_xIb_A = ia;// GlobalUnit.Ib;
            tagPara.sng_xIb_B = ib;// / GlobalUnit.Ib;
            tagPara.sng_xIb_C = ic;// / GlobalUnit.Ib;
            //相序
            tagPara.bln_IsNxx = bNxx;
            //频率
            tagPara.sng_Freq = feq;

            #region 去掉不需要的
            if (ele == Cus_PowerYuanJian.H)
            {
                if (tagPara.clfs == Cus_Clfs.单相)
                {
                    tagPara.sng_xUb_B = 0;
                    tagPara.sng_xUb_C = 0;
                    //I
                    tagPara.sng_xIb_B = 0;
                    tagPara.sng_xIb_C = 0;
                }
                else if (tagPara.clfs == Cus_Clfs.三相三线)
                {
                    tagPara.sng_xUb_B = 0;
                    tagPara.sng_xIb_B = 0;
                }
            }
            else if (ele == Cus_PowerYuanJian.A)
            {
                //I
                tagPara.sng_xIb_B = 0;
                tagPara.sng_xIb_C = 0;
            }
            else if (ele == Cus_PowerYuanJian.B)
            {
                //I
                tagPara.sng_xIb_A = 0;
                tagPara.sng_xIb_C = 0;
            }
            else if (ele == Cus_PowerYuanJian.C)
            {
                //I
                tagPara.sng_xIb_A = 0;
                tagPara.sng_xIb_B = 0;
            }
            #endregion

            #region 角度转换
            //角度转换
       //     int clfs = getClfs(GlobalUnit.Clfs, IsYouGong);
         //   int element = (int)ele - 1;
            Cus_PowerPhase xx = Cus_PowerPhase.正相序;
            if (bNxx)
            {
                 xx = Cus_PowerPhase.逆相序;
            }
            else
            {
                 xx = Cus_PowerPhase.正相序;
            }
            Single[] arrPhi = CLDC_DataCore.Function.Common.GetPhiGlys(tagPara.clfs, glfx, (Cus_PowerYuanJian)ele, glys, xx);
            //Single[] arrPhi2 = Comm.Function.Common.GetPhiGlys(clfs, glys, element, bNxx, IsYouGong); 
            tagPara.sng_UaPhi = arrPhi[0];
            tagPara.sng_UbPhi = arrPhi[1];
            tagPara.sng_UcPhi = arrPhi[2];
            tagPara.sng_IaPhi = arrPhi[3];
            tagPara.sng_IbPhi = arrPhi[4];
            tagPara.sng_IcPhi = arrPhi[5];
            #endregion

            if (tagPara.clfs == Cus_Clfs.三相三线)
            {
                tagPara.sng_xUb_B = 0;
                //tagPara.sng_IbPhi = 0;
                //tagPara.sng_UbPhi = 0;
            }

            return tagPara;
        }

        /// <summary>
        /// 转换当前要升源的测量方式
        /// 中的测试方式定义与检定器定义不一致。
        /// </summary>
        /// <param name="isYouGong">是否是有功</param>
        /// <returns>测量方式</returns>
        private int getClfs(Cus_Clfs Clfs, bool isYouGong)
        {
            /*   三相四线有功 = 0,
         三相四线无功 = 1,
         三相三线有功 = 2,
         三相三线无功 = 3,
         二元件跨相90 = 4,
         二元件跨相60 = 5,
         三元件跨相90 = 6,
             
        三相四线=0,
        三相三线=1,
        二元件跨相90=2,
        二元件跨相60=3,
        三元件跨相90=4,
        单相=5
             
             */
            IsYouGong = isYouGong;
            int clfs = (int)Clfs;
            if (clfs == 5)                            //单相台统一划分为三相四线
            {
                clfs = 0;
            }
            clfs += 2;                              //先保证后面对齐
            if (clfs < 4)                             //处理前面没有对齐部分
            {
                if (clfs == 3)
                {
                    if (IsYouGong)
                    {
                        clfs--;
                    }
                }
                else
                {
                    clfs--;
                    if (IsYouGong)
                    {
                        clfs--;
                    }
                }
            }
            return clfs;
        }
        #endregion

        /// <summary>
        /// 设置谐波
        /// </summary>
        /// <returns></returns>
        #region ----------谐波参数----------
        public struct HarmonicPhasePara
        {
            /// <summary>
            /// 开关
            /// </summary>
            public bool IsOpen;
            /// <summary>
            /// 各次开关
            /// </summary>
            public bool[] TimeSwitch;
            /// <summary>
            /// 含量
            /// </summary>
            public float[] Content;
            /// <summary>
            /// 相角
            /// </summary>
            public float[] Phase;

            /// <summary>
            /// 初始化结构体
            /// </summary>
            public void Initialize()
            {
                TimeSwitch = new bool[64];
                Content = new float[64];
                Phase = new float[64];
            }
        }
        #endregion

        /// <summary>
        /// 设置谐波含量
        /// </summary>
        /// <param name="Ua">A相</param>
        /// <param name="Ub"></param>
        /// <param name="Uc"></param>
        /// <param name="Ia"></param>
        /// <param name="Ib"></param>
        /// <param name="Ic"></param>
        /// <returns></returns>
        public bool SetHarmonic(HarmonicPhasePara Ua, HarmonicPhasePara Ub, HarmonicPhasePara Uc,
                                HarmonicPhasePara Ia, HarmonicPhasePara Ib, HarmonicPhasePara Ic)
        {
            //参数转换
            //相开关
            bool[] bSwitch = new bool[6] { Ua.IsOpen, Ub.IsOpen, Uc.IsOpen, Ia.IsOpen, Ib.IsOpen, Ic.IsOpen };
            int[] xSwitch = new int[6];
            //次开关
            bool[][] bSwitch2 = new bool[6][] { Ua.TimeSwitch, Ub.TimeSwitch, Uc.TimeSwitch, Ia.TimeSwitch, Ib.TimeSwitch, Ic.TimeSwitch };

            int len = CLDC_Comm.Utils.ArrayHelper.Max(Ua.TimeSwitch.Length, Ub.TimeSwitch.Length, Uc.TimeSwitch.Length, Ia.TimeSwitch.Length, Ib.TimeSwitch.Length, Ic.TimeSwitch.Length);
            int[][] xWticth2 = new int[6][];
            for (int i = 0; i < xWticth2.Length; i++)
            {
                xWticth2[i] = new int[len];
            }
            //含量和相角
            float[][] fContent = new float[][] { Ua.Content, Ub.Content, Uc.Content, Ia.Content, Ib.Content, Ic.Content };
            float[][] fPhase = new float[][] { Ua.Phase, Ub.Phase, Uc.Phase, Ia.Phase, Ib.Phase, Ic.Phase };
            //组合
            for (int i = 0; i < xSwitch.Length; i++)
            {
                xSwitch[i] = (bSwitch[i] ? 1 : 0);
                for (int j = 0; j < bSwitch2[i].Length; j++)
                {
                    xWticth2[i][j] = bSwitch2[i][j] ? 1 : 0;
                }
            }
            return m_DeviceDriver.SetHarmonic(xWticth2, fContent, fPhase);
            //return true;
        }

        public bool SetHarmonicSwitch(bool bSwitch)
        {
            return m_DeviceDriver.SetHarmonicSwitch(bSwitch);
        }

        /// <summary>
        /// 设置尖顶波-1 平顶波-2
        /// </summary>
        /// <returns></returns>
        public bool SetJd_Pd(int ua, int ia, int ub, int ib, int uc, int ic)
        {

            return m_DeviceDriver.SetJd_Pd(ua, ia, ub, ib, uc, ic);
        }
        /// <summary>
        /// 设置波形
        /// </summary>
        /// <param name="ua"></param>
        /// <param name="ub"></param>
        /// <param name="uc"></param>
        /// <param name="ia"></param>
        /// <param name="ib"></param>
        /// <param name="ic"></param>
        /// <returns></returns>
        public bool SettingWaveformSelection(int ua, int ia, int ub, int ib, int uc, int ic)
        {

            return m_DeviceDriver.SettingWaveformSelection(ua, ia, ub, ib, uc, ic);
        }







        #region 控制回路
        /// <summary>
        /// 供电类型，耐压供电=1、载波供电=2、普通供电=3、一回路=4、二回路=5、耐压保护=6、
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public bool SetPowerSupplyType(int elementType)
        {

            return SetPowerSupplyType(elementType, GlobalUnit.HGQ);
        }   
        /// <summary>
        /// 供电类型
        /// </summary>
        /// <param name="elementType">耐压供电=1、载波供电=2、普通供电=3、一回路=4、二回路=5</param>
        /// <param name="isMeterTypeHGQ">互感true，直接false</param>
        /// <returns></returns>
        public bool SetPowerSupplyType(int elementType, bool isMeterTypeHGQ)
        {
            // <param name="meterType">false直接式，true互感式</param>
            return m_DeviceDriver.SetPowerSupplyType(elementType, isMeterTypeHGQ);
        }

        #endregion

        #region ---------读取检定数据----------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state">false:读取状态；true：读取误差</param>
        /// <returns></returns>
        public CLDC_DeviceDriver.stError[] ReadWcb(bool state)
        {
            CLDC_DeviceDriver.stError[] stStatus = null;
            if (state == false)
            {
                stStatus = m_DeviceDriver.ReadWcb(Helper.MeterDataHelper.Instance.GetYaoJian(), false);
                ConvertStatus(stStatus, 0);
            }
            else
            {
                stStatus = m_DeviceDriver.ReadWcb(Helper.MeterDataHelper.Instance.GetYaoJian(), true);
            }

            return stStatus;
        }

        public CLDC_DeviceDriver.stError ReadWc(int bw)
        {
            CLDC_DeviceDriver.stError stStatus = new CLDC_DeviceDriver.stError();
            stStatus = m_DeviceDriver.ReadWc(bw);
             return stStatus;
        }






        private string[] ConvertStatus(CLDC_DeviceDriver.stError[] stStatus, int type)
        {
            string[] states = new string[GlobalUnit.g_CUS.DnbData._Bws];
            string[] QuarantineStatus = new string[GlobalUnit.g_CUS.DnbData._Bws];
            for (int i = 0; i < stStatus.Length; i++)
            {
                if (stStatus[i].statusReadFlog)
                {
                    #region 限位、挂表
                    if (stStatus[i].statusTypeIsOn_PressDownLimt == true)//MeterPositionPressStatus.已压接)
                    {
                        states[i] = "1";
                    }
                    else
                    {
                        states[i] = "0";
                    }
                    if (stStatus[i].statusTypeIsOn_PressUpLimit == true)//MeterPositionPressStatus.已压接)
                    {
                        states[i] += "1";
                    }
                    else
                    {
                        states[i] += "0";
                    }
                    if (stStatus[i].statusTypeIsOn_HaveMeter == true)//MeterPositionPressStatus.已压接)
                    {
                        states[i] += "1";
                    }
                    else
                    {
                        states[i] += "0";
                    }
                    #endregion
                    #region 隔离、故障
                    if (stStatus[i].statusTypeIsOnErr_Jxgz == true)//MeterPositionPressStatus.已压接)
                    {
                        QuarantineStatus[i] = "1";
                    }
                    else
                    {
                        QuarantineStatus[i] = "0";
                    }
                    if (stStatus[i].statusTypeIsOnErr_Temp == true)//MeterPositionPressStatus.已压接)
                    {
                        QuarantineStatus[i] += "1";
                    }
                    else
                    {
                        QuarantineStatus[i] += "0";
                    }
                    #endregion
                }
                else
                {
                    states[i] = "222";
                    QuarantineStatus[i] = "22";
                }
            }
            try
            {
                //发送表位状态信息
                MessageController.Instance.AddMonitorMessage(EnumMonitorType.PressStatus, string.Join("|", states));
            }
            catch (Exception ex)
            {
                CLDC_DataCore.Function.ErrorLog.Write(ex);
            }
            if (type == 0)
            {
                return states;
            }
            else
            {
                return QuarantineStatus;
            }
        }
        /// <summary>
        /// 读取源信息
        /// </summary>
        /// <returns></returns>
        public CLDC_DataCore.Struct.StPower ReadPowerInfo()
        {
            CLDC_DataCore.Struct.StPower tagPower = new StPower();
            CLDC_DeviceDriver.stStdInfo tagPowerInfo = m_DeviceDriver.ReadStdInfo();
            tagPower.Ua = tagPowerInfo.Ua;
            tagPower.Ub = tagPowerInfo.Ub;
            tagPower.Uc = tagPowerInfo.Uc;
            tagPower.Ia = tagPowerInfo.Ia;
            tagPower.Ib = tagPowerInfo.Ib;
            tagPower.Ic = tagPowerInfo.Ic;
            tagPower.Phi_Ia = tagPowerInfo.Phi_Ia;
            tagPower.Phi_Ib = tagPowerInfo.Phi_Ib;
            tagPower.Phi_Ic = tagPowerInfo.Phi_Ic;
            tagPower.Phi_Ua = tagPowerInfo.Phi_Ua;
            tagPower.Phi_Ub = tagPowerInfo.Phi_Ub;
            tagPower.Phi_Uc = tagPowerInfo.Phi_Uc;
            tagPower.Q = tagPowerInfo.Q;
            tagPower.Qa = tagPowerInfo.Qa;
            tagPower.Qb = tagPowerInfo.Qb;
            tagPower.Qc = tagPowerInfo.Qc;
            tagPower.S = tagPowerInfo.S;
            tagPower.Sa = tagPowerInfo.Sa;
            tagPower.Sb = tagPowerInfo.Sb;
            tagPower.Sc = tagPowerInfo.Sc;
            tagPower.Scale_Ia = tagPowerInfo.Scale_Ia;
            tagPower.Scale_Ib = tagPowerInfo.Scale_Ib;
            tagPower.Scale_Ic = tagPowerInfo.Scale_Ic;
            tagPower.Scale_Ua = tagPowerInfo.Scale_Ua;
            tagPower.Scale_Ub = tagPowerInfo.Scale_Ub;
            tagPower.Scale_Uc = tagPowerInfo.Scale_Uc;
            tagPower.Freq = tagPowerInfo.Freq;
            tagPower.Clfs = tagPowerInfo.Clfs;
            tagPower.COS = tagPowerInfo.COS;
            tagPower.SIN = tagPowerInfo.SIN;
            tagPower.Flip_ABC = tagPowerInfo.Flip_ABC;
            tagPower.P = tagPowerInfo.P;
            tagPower.Pa = tagPowerInfo.Pa;
            tagPower.Pb = tagPowerInfo.Pb;
            tagPower.Pc = tagPowerInfo.Pc;
            return tagPower;
        }
        //读取标准表信息
        /// <summary>
        /// 切换时钟脉冲
        /// </summary>
        /// <param name="isTime">是否切换为时钟脉冲</param>
        public bool SetTimeMaiCon(bool isTime)
        {
            return m_DeviceDriver.SetTimeMaiCon(isTime);
        }
        #endregion



        #region ----------检定控制---------
        /// <summary>
        /// 停止设备控制
        /// </summary>
        public void Stop()
        {
            if (GlobalUnit.IsDemo) return;

            string[] EbNo = new string[GlobalUnit.g_CUS.DnbData._Bws];
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData._Bws; i++)
            {
                EbNo[i] = "--" + (i + 1).ToString().PadLeft(2, '0') + "--";
            }
            MessageController.Instance.AddMonitorMessage(EnumMonitorType.ErrorBoard, string.Join("|", EbNo));
            LogHelper.Instance.WriteDebug("开始停止设备组件");
            m_DeviceDriver.Stop();
            LogHelper.Instance.WriteDebug("==================停止设备组件完成==================");
        }

        #endregion


        #region ----------私有方法----------
        private void equipMessage(string str)
        {
            MessageController.Instance.AddMessage(str);
            //MessageController.Instance.AddMessage(str, 6, 2);
        }
        #endregion

        #region 远程上电
        /// <summary>
        /// 远程控制供电，远程上电 true 上电，false 断电
        /// </summary>
        /// <param name="OnOrOff"></param>
        public void RemoteControlOnOrOff(bool OnOrOff)
        {
            //if (GlobalUnit.IsDemo)
            //{ }
            //else
            {
                m_DeviceDriver.RemoteControlOnOrOff(OnOrOff);
            }
        }
        #endregion

        #region 功耗
        /// <summary>
        /// 读取功耗
        /// </summary>
        /// <param name="int_BwIndex">功耗板ID，一般等于表位号</param>
        /// <param name="byt_Chancel">通道号，1=A相电压,2=A相电流,3=B相电压,4=B相电流,5=C相电压,6=C相电流</param>
        /// <param name="flt_PD">传出，float[4]{电压有效值,电流有效值,基波有功功率,基波无功功率}</param>
        public bool ReadPowerDissipation(int int_BwIndex, byte byt_Chancel, out float[] flt_PD)
        {
            return m_DeviceDriver.ReadPowerDissipation(int_BwIndex, byt_Chancel, out flt_PD);
        }

        #endregion

        #region 李鑫 20200618
        public bool SetRelay(int[] switchOpen, int[] switchClose)
        {
            // <param name="meterType">false直接式，true互感式</param>
            return m_DeviceDriver.SetRelay(switchOpen, switchClose);
        }
        /// <summary>
        /// 读实时测量数据
        /// </summary>
        /// <param name="Index">表位号</param>
        /// <param name="instValue">输出测量数据</param>
        /// <returns></returns>
        public bool ReadTemperature(int Index, out float[] instValue)
        {
            return m_DeviceDriver.ReadTemperature(Index, out instValue);
        }

        /// <summary>
        /// 设置温度
        /// </summary>
        /// <param name="Index">表位号</param>
        /// <param name="Flags">需要温控标志位</param>
        /// <param name="Temperatures">控制温度</param>
        /// <returns></returns>
        public bool SetTemperature(int Index, bool[] Flags, float[] Temperatures)
        {
            return m_DeviceDriver.SetTemperature(Index, Flags, Temperatures);
        }

        /// <summary>
        /// 开风扇
        /// </summary>
        /// <param name="Flag">控制标志位</param>
        /// <returns></returns>
        public bool OpenFan(bool Flag)
        {
            return m_DeviceDriver.OpenFan(Flag);
        }
        /// <summary>
        /// 开锁
        /// </summary>
        /// <returns></returns>
        public bool OpenLock()
        {
            return m_DeviceDriver.OpenLock();
        }
        #endregion

        #endregion

        #region 检定参数初始化

        /// <summary>
        /// 潜动参数初始化[不包括对标]
        /// </summary>
        /// <param name="pd">功能方向</param>
        /// <param name="Ib">潜动电流</param>
        /// <param name="isYouGong">是否是有功</param>
        /// <returns>是否成功</returns>
        public bool InitPara_Creep(Cus_PowerFangXiang pd, float Ib, bool isYouGong, int[] creepTimes)
        {
            bool[] isoff = new bool[0];
            bool ret = false;
          //  stPowerPara tagPara = getPowerPara(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, Ib, Ib, Ib, Cus_PowerYuanJian.H, "1.0", IsYouGong, false, 50F);
            for (int i = 0; i < ReTryTimes; i++)
            {
                ret = m_DeviceDriver.InitCreeping(GlobalUnit.Clfs, pd, Helper.MeterDataHelper.Instance.GetIm(), Helper.MeterDataHelper.Instance.GetYaoJian(), creepTimes);
                if (ret) break;
                Thread.Sleep(300);
            }
            return ret;
        }
        /// <summary>
        /// 基本误差参数初始化
        /// </summary>
        /// <returns></returns>
        public bool InitPara_BasicError(Cus_PowerFangXiang pd, int[] meterconst, int[] circleCount)
        {
            bool isP = (pd == Cus_PowerFangXiang.正向有功 || pd == Cus_PowerFangXiang.反向有功);

            if (GlobalUnit.clfs == Cus_Clfs.单相)
            {
                if (pd == Cus_PowerFangXiang.正向无功 || pd == Cus_PowerFangXiang.反向无功)
                {
                    MeterProtocolAdapter.Instance.SetPulseCom(3);
                }
            }  



            bool ret = false;
            for (int i = 0; i < ReTryTimes; i++)
            {
                ret = m_DeviceDriver.InitError(GlobalUnit.Clfs, pd, meterconst, circleCount, 2, Helper.MeterDataHelper.Instance.GetIm(), Helper.MeterDataHelper.Instance.GetYaoJian());
                if (ret && i >= 0) break;//通讯通道不可靠，广播形式，至少次
                Thread.Sleep(250);
            }
            return ret;
        }
        /// <summary>
        /// 启动参数初始化[不包括对标]
        /// </summary>
        /// <param name="pd">功率方向</param>
        /// <param name="Ib">起动电流值(A)</param>
        /// <param name="isYouGong">是否是有功</param>
        /// <returns>结果</returns>
        public bool InitPara_Start(Cus_PowerFangXiang pd, float Ib, bool isYouGong, int[] startTimes,int[] meterconst)
        {
            bool[] isoff = new bool[0];
            bool ret = false;
            if (GlobalUnit.clfs == Cus_Clfs.单相)
            {
                if (pd == Cus_PowerFangXiang.正向无功 || pd == Cus_PowerFangXiang.反向无功)
                {
                    MeterProtocolAdapter.Instance.SetPulseCom(3);
                }
            }  
          //  stPowerPara tagPara = getPowerPara(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, Ib, Ib, Ib, Cus_PowerYuanJian.H, "1.0", IsYouGong, false, 50F);
            for (int i = 0; i < ReTryTimes; i++)
            {
                ret = m_DeviceDriver.InitStartUp(GlobalUnit.Clfs, pd,
                           Helper.MeterDataHelper.Instance.GetIm(),
                           Helper.MeterDataHelper.Instance.GetYaoJian(), startTimes,meterconst);
                if (ret) break;
                Thread.Sleep(300);
            }
            return ret;
        }
        /// <summary>
        /// 启动/停止当前设置的功能
        /// </summary>
        /// <param name="isOn">True为停止,False为启动</param>
        /// <returns></returns>
        public bool SetCurFunctionOnOrOff(bool IsOnOff, byte state)
        {
            //启动或停止当前功能
            if (!m_DeviceDriver.SetCurFunctionOnOrOff(IsOnOff, state))
            {
                if (!m_DeviceDriver.SetCurFunctionOnOrOff(IsOnOff, state))
                {
                    string str = IsOnOff ? "停止" : "启动";
                  //  MessageController.Instance.AddMessage(str + "当前设置的功能出错");
                    return false;
                }
            }
            return true;

        }

        /// <summary>
        /// 走字试验参数初始化
        /// </summary>
        /// <returns></returns>
        public bool InitPara_Constant(Cus_PowerFangXiang pd, int[] impluseCount)
        {
            bool ret = false;
            for (int i = 0; i < ReTryTimes; i++)
            {
                ret = m_DeviceDriver.InitZZ(Helper.MeterDataHelper.Instance.GetYaoJian(), pd, Helper.MeterDataHelper.Instance.GetIm(), impluseCount);
                if (ret) break;
                Thread.Sleep(300);
            }
            return ret;
        }

        /// <summary>
        /// 读取GPS时间
        /// </summary>
        /// <returns></returns>
        public DateTime ReadGpsTime()
        {
            return m_DeviceDriver.ReadGPSTime();
        }


        /// <summary>
        /// 读取误差板的功耗数据
        /// </summary>
        /// <param name="blnBwIndex">要读的表位</param>
        /// <param name="flt_PD">出数据结构</param>
        public void ReadErrPltGHPram(bool[] blnBwIndex, out stGHPram[] flt_PD)
        {
            m_DeviceDriver.ReadErrPltGHPram(blnBwIndex, out flt_PD);
        }
        public bool ReadQueryCurrentErrorControl(int id, int CheckType, out int ErrNum, out string ErrData, out string TQtime)
        {
            return m_DeviceDriver.ReadQueryCurrentErrorControl(id, CheckType, out  ErrNum, out  ErrData, out  TQtime);
        }


        /// <summary>
        /// 设置标准表界面 1：谐波柱图界面2：谐波列表界面3：波形界面4：清除设置界面
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="FrameAry">输出报文</param>
        /// <returns></returns>
        public bool SetDisplayFormControl(int formType)
        {
            return m_DeviceDriver.SetDisplayFormControl(formType);
        }

        /// <summary>
        /// 读取各相电压电流谐波幅值（分两帧读取数据）
        /// </summary>
        /// <param name="phase">相别，0是C相电压，1是B相电压，2是A相电压，3是C相电流，4是B相电流，5是A相电流</param>
        /// <param name="harmonicArry"></param>
        /// <returns></returns>
        public bool ReadHarmonicArryControl(int phase, out float[] harmonicArry)
        {
            return m_DeviceDriver.ReadHarmonicArryControl(phase, out  harmonicArry);
        }


        #region    设置电压暂降，电流快速变化
        /// <summary>
        /// 设置暂降电压电流阀值
        /// </summary>
        /// <param name="Wave">float[6]类型 </param>
        /// Wave[0] ua  ;Wave[1] ub;Wave[2] uc;Wave[3] ia;Wave[4] ib;Wave[5] ic
        /// <returns></returns>
        public bool SetDropWave(float[] Wave)
        {
            return m_DeviceDriver.SetDropWave(Wave);
        }

        /// <summary>
        /// 设置暂降电压电流时间
        /// </summary>
        /// <param name="Time">int[2]</param>
        /// Ua,Ub,Uc,Ia,Ib,Ic
        /// /// <returns></returns>
        public bool SetDropTime(int[] Time)
        {
            return m_DeviceDriver.SetDropTime(Time);
        }

        /// <summary>
        /// 设置暂降电压电流开关
        /// </summary>
        /// <param name="Switch">bool[6]</param>
        ///  bool[0] ua  ;bool[1] ub;bool[2] uc;bool[3] ia;bool[4] ib;bool[5] ic
        /// <returns></returns>

        public bool SetDropSwitch(bool[] Switch)
        {
            return m_DeviceDriver.SetDropSwitch(Switch);
        }

        /// <summary>
        /// 设置标准表走字界面
        /// </summary>
        /// <param name="funcType">
        /// 控制类型 0x00：默认界面 
        /// 0x01: 功率测量界面
        /// 0x02: 伏安测量界面
        /// 0x03: 电能误差与标准差界面
        /// 0x05: 电能量走字界面
        /// 0x09: 谐波测量界面
        /// 0x10: 稳定度测量界面
        /// 0xFE: 清除界面设置(返回默认界面) </param>
        /// <returns></returns>
        public bool FuncMstate( int funcType)
        {
            return m_DeviceDriver.FuncMstate( funcType);
        }

        #endregion

        #endregion

        #region 加密机
        public void SouthLink(string szType, string cHostIp, int uiPort, int timeout)
        {
            string strMsg = "";
            int intRst = MeterProtocolAdapter.Instance.EncryptionTool.SouthOpenDevice(szType, cHostIp, uiPort, timeout, out strMsg);
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},信息:{2}.", "SouthOpenDevice", intRst, strMsg), 7, 0);
        }
        public void SouthCloseDevice()
        {
            string strMsg = "";
            int intRst = MeterProtocolAdapter.Instance.EncryptionTool.SouthCloseDevice(out strMsg);
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},信息:{2}.", "SouthCloseDevice", intRst, strMsg), 7, 0);
        }
        public void SouthIdentityAuthentication(int Flag, string PutDiv, ref string OutRand, ref string OutEndata)
        {
            OutRand = "";
            OutEndata = "";
            object[] paras = new object[] { Flag, PutDiv, OutRand, OutEndata };
            string strMsg = "";
            int intRst = MeterProtocolAdapter.Instance.EncryptionTool.SouthIdentityAuthentication(Flag, PutDiv, out OutRand, out OutEndata, out strMsg);
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{3},信息:{2}.", "SouthIdentityAuthentication", intRst, strMsg, "OutRand=" + OutRand + ",OutEndata=" + OutEndata), 7, 0);
        }
        public void SouthUserControl(int Flag, string PutRand, string PutDiv, string PutEsamNo,
string PutData, out string OutEndata)
        {
            OutEndata = "";
            string strMsg = "";
            int intRst = MeterProtocolAdapter.Instance.EncryptionTool.SouthUserControl(Flag, PutRand, PutDiv, PutEsamNo,
 PutData, out OutEndata, out strMsg);
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{3},信息:{2}.", "SouthUserControl", intRst, strMsg, "OutEndata=" + OutEndata), 7, 0);
        }
        public void SouthParameterUpdate(int Flag, string PutRand, string PutDiv, string PutApdu, string PutData, out string OutData)
        {
            OutData = "";
            string strMsg = "";
            int intRst = MeterProtocolAdapter.Instance.EncryptionTool.SouthParameterUpdate(Flag, PutRand, PutDiv, PutApdu, PutData, out OutData, out strMsg);
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{3},信息:{2}.", "SouthParameterUpdate", intRst, strMsg, "OutData=" + OutData), 7, 0);
        }
        public void SouthPrice1Update(int Flag, string PutRand, string PutDiv, string PutApdu, string PutData, out string OutData)
        {
            OutData = "";
            string strMsg = "";
            int intRst = MeterProtocolAdapter.Instance.EncryptionTool.SouthPrice1Update(Flag, PutRand, PutDiv, PutApdu, PutData, out OutData, out strMsg);
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{3},信息:{2}.", "SouthPrice1Update", intRst, strMsg, "OutData=" + OutData), 7, 0);
        }
        public void SouthPrice2Update(int Flag, string PutRand, string PutDiv, string PutApdu, string PutData, out string OutData)
        {
            OutData = "";
            string strMsg = "";
            int intRst = MeterProtocolAdapter.Instance.EncryptionTool.SouthPrice2Update(Flag, PutRand, PutDiv, PutApdu, PutData, out OutData, out strMsg);
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{3},信息:{2}.", "SouthPrice2Update", intRst, strMsg, "OutData=" + OutData), 7, 0);
        }
        public void SouthParameterElseUpdate(int Flag, string PutRand, string PutDiv, string PutApdu, string PutData, out string OutEndata)
        {
            OutEndata = "";
            string strMsg = "";
            int intRst = MeterProtocolAdapter.Instance.EncryptionTool.SouthParameterElseUpdate(Flag, PutRand, PutDiv, PutApdu, PutData, out OutEndata, out strMsg);
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{3},信息:{2}.", "SouthParameterElseUpdate", intRst, strMsg, "OutEndata=" + OutEndata), 7, 0);
        }
        public void SouthIncreasePurse(int Flag, string PutRand, string PutDiv, string PutData, out string OutData)
        {
            OutData = "";
            string strMsg = "";
            int intRst = MeterProtocolAdapter.Instance.EncryptionTool.SouthIncreasePurse(Flag, PutRand, PutDiv, PutData, out OutData, out strMsg);
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{3},信息:{2}.", "SouthIncreasePurse", intRst, strMsg, "OutData=" + OutData), 7, 0);
        }
        public void SouthInitPurse(int Flag, string PutRand, string PutDiv, string PutData, out string OutData)
        {
            OutData = "";
            string strMsg = "";
            int intRst = MeterProtocolAdapter.Instance.EncryptionTool.SouthInitPurse(Flag, PutRand, PutDiv, PutData, out OutData, out strMsg);
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{3},信息:{2}.", "SouthInitPurse", intRst, strMsg, "OutData=" + OutData), 7, 0);
        }
        public void SouthKeyUpdateV2(int PutKeySum, string PutKeyState, string PutKeyId, string PutRand, string PutDiv, string PutEsamNo, out string OutData)
        {
            OutData = "";
            string strMsg = "";
            int intRst = MeterProtocolAdapter.Instance.EncryptionTool.SouthKeyUpdateV2(PutKeySum, PutKeyState, PutKeyId, PutRand, PutDiv, PutEsamNo, out OutData, out strMsg);
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{3},信息:{2}.", "SouthKeyUpdateV2", intRst, strMsg, "OutData=" + OutData), 7, 0);
        }
        public void SouthDataClear1(int Flag, string PutRand, string PutDiv, string PutData, out string OutData)
        {
            OutData = "";
            string strMsg = "";
            int intRst = MeterProtocolAdapter.Instance.EncryptionTool.SouthDataClear1(Flag, PutRand, PutDiv, PutData, out OutData, out strMsg);
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{3},信息:{2}.", "SouthDataClear1", intRst, strMsg, "OutData=" + OutData), 7, 0);
        }
        public void SouthInfraredRand(out string OutRand1)
        {
            OutRand1 = "";
            string strMsg = "";
            int intRst = MeterProtocolAdapter.Instance.EncryptionTool.SouthInfraredRand(out OutRand1, out strMsg);
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{3},信息:{2}.", "SouthInfraredRand", intRst, strMsg, "OutRand1=" + OutRand1), 7, 0);
        }
        public void SouthInfraredAuth(int Flag, string PutDiv, string PutEsamNo, string PutRand1, string PutRand1Endata, string PutRand2, out string OutRand2Endata)
        {
            OutRand2Endata = "";
            string strMsg = "";
            int intRst = MeterProtocolAdapter.Instance.EncryptionTool.SouthInfraredAuth(Flag, PutDiv, PutEsamNo, PutRand1, PutRand1Endata, PutRand2, out OutRand2Endata, out strMsg);
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{3},信息:{2}.", "SouthInfraredAuth", intRst, strMsg, "OutRand2Endata=" + OutRand2Endata), 7, 0);
        }
        public void SouthMacCheck(int Flag, string PutRand, string PutDiv, string PutApdu, string PutData, string PutMac)
        {
            string strMsg = "";
            int intRst = MeterProtocolAdapter.Instance.EncryptionTool.SouthMacCheck(Flag, PutRand, PutDiv, PutApdu, PutData, PutMac, out strMsg);
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},信息:{2}.", "SouthMacCheck", intRst, strMsg), 7, 0);
        }
        public void SouthMacWrite(int Flag, string PutRand, string PutDiv, string PutEsamNo, string PutFileID, string PutDataBegin, string PutData, out string OutData)
        {
            OutData = "";
            string strMsg = "";
            int intRst = MeterProtocolAdapter.Instance.EncryptionTool.SouthMacWrite(Flag, PutRand, PutDiv, PutEsamNo, PutFileID, PutDataBegin, PutData, out OutData, out strMsg);
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{3},信息:{2}.", "SouthMacWrite", intRst, strMsg, "OutData=" + OutData), 7, 0);
        }
        public void SouthEncMacWrite(int Flag, string PutRand, string PutDiv, string PutEsamNo, string PutFileID, string PutDataBegin, string PutData, out string OutData)
        {
            OutData = "";
            string strMsg = "";
            int intRst = MeterProtocolAdapter.Instance.EncryptionTool.SouthEncMacWrite(Flag, PutRand, PutDiv, PutEsamNo, PutFileID, PutDataBegin, PutData, out OutData, out strMsg);
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{3},信息:{2}.", "SouthEncMacWrite", intRst, strMsg, "OutData=" + OutData), 7, 0);
        }
        public void SouthEncForCompare(string PutKeyid, string PutDiv, string PutData, out string OutData)
        {
            OutData = "";
            string strMsg = "";
            int intRst = MeterProtocolAdapter.Instance.EncryptionTool.SouthEncForCompare(PutKeyid, PutDiv, PutData, out OutData, out strMsg);
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{3},信息:{2}.", "SouthEncForCompare", intRst, strMsg, "OutData=" + OutData), 7, 0);
        }
        public void SouthDecreasePurse(int Flag, string PutRand, string PutDiv, string PutData, out string OutEndata)
        {
            OutEndata = "";
            string strMsg = "";
            int intRst = MeterProtocolAdapter.Instance.EncryptionTool.SouthDecreasePurse(Flag, PutRand, PutDiv, PutData, out OutEndata, out strMsg);
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{3},信息:{2}.", "SouthDecreasePurse", intRst, strMsg, "OutEndata=" + OutEndata), 7, 0);
        }
        public void SouthSwitchChargeMode(int Flag, string PutRand, string PutDiv, string PutData,
out string OutData)
        {
            OutData = "";
            string strMsg = "";
            int intRst = MeterProtocolAdapter.Instance.EncryptionTool.SouthSwitchChargeMode(Flag, PutRand, PutDiv, PutData, out 
  OutData, out strMsg);
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{3},信息:{2}.", "SouthSwitchChargeMode", intRst, strMsg, "OutData=" + OutData), 7, 0);
        }
        #endregion

        #region 读卡器
        bool IsCmd = true;
        int SelectBw = 0;

        public void WINAPI_OpenDevice()
        {
            for (int i = 0; i < m_BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    SelectBw = i;
                    break;
                }
            }

            int intRst = 1;
            if (IsCmd)
            {
                bool blnRst = MeterProtocolAdapter.Instance.SouthResetCard(SelectBw);
                intRst = blnRst ? 0 : 1;
            }
            else
            {
                intRst = CLDC_SafeFileProtocol.CardAPI.CardCtrlAPI.WINAPI_OpenDevice();
            }
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1}.", "WINAPI_OpenDevice", intRst), 7, 0);
        }
        public void WINAPI_ReadUserCardNum(out string cardNum)
        {
            int intRst = 1;
            cardNum = "";
            if (IsCmd)
            {

                bool blnRst = MeterProtocolAdapter.Instance.SouthReadUserCardNum(SelectBw, out cardNum);
                intRst = blnRst ? 0 : 1;
            }
            else
            {

            }
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{2}.", "ReadUserCardNum", intRst, cardNum), 7, 0);
        }
        public void WINAPI_ReadParamPresetCardNum(out string cardNum)
        {
            int intRst = 1;
            cardNum = "";
            if (IsCmd)
            {

                bool blnRst = MeterProtocolAdapter.Instance.SouthReadParamPresetCardNum(SelectBw, out cardNum);
                intRst = blnRst ? 0 : 1;
            }
            else
            {

            }
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{2}.", "ReadParamPresetCardNum", intRst, cardNum), 7, 0);
        }
        public void WINAPI_CloseDevice()
        {
            int intRst = 1;
            if (IsCmd)
            {
                intRst = 0;
            }
            else
            {
                intRst = CLDC_SafeFileProtocol.CardAPI.CardCtrlAPI.WINAPI_CloseDevice();
            }
            MessageController.Instance.AddMessage(string.Format("调用{0}返回{1}.", "WINAPI_CloseDevice", intRst), 7, 0);
        }
        public void WINAPI_ReadParamPresetCard(out string fileParam, out string fileMoney, out string filePrice1, out string filePrice2, out string cardNum)
        {
            fileParam = "";
            fileMoney = "";
            filePrice1 = "";
            filePrice2 = "";
            cardNum = "";
            try
            {
                int intRst = 1;
                if (IsCmd)
                {
                    bool blnRst = MeterProtocolAdapter.Instance.SouthReadParamPresetCard(SelectBw, out fileParam, out fileMoney, out filePrice1, out filePrice2);
                    intRst = blnRst ? 0 : 1;
                }
                else
                {
                    StringBuilder sfileParam = new StringBuilder(1024);
                    StringBuilder sfileMoney = new StringBuilder(1024);
                    StringBuilder sfilePrice1 = new StringBuilder(1024);
                    StringBuilder sfilePrice2 = new StringBuilder(1024);
                    StringBuilder scardNum = new StringBuilder(1024);

                    intRst = CLDC_SafeFileProtocol.CardAPI.CardCtrlAPI.WINAPI_ReadParamPresetCard(sfileParam, sfileMoney, sfilePrice1, sfilePrice2, scardNum);
                    if (intRst == 0)
                    {
                        fileParam = sfileParam.ToString().Replace("\0", "");
                        fileMoney = sfileMoney.ToString().Replace("\0", "");
                        filePrice1 = sfilePrice1.ToString().Replace("\0", "");
                        filePrice2 = sfilePrice2.ToString().Replace("\0", "");
                        cardNum = scardNum.ToString().Replace("\0", "");
                    }
                }
                MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{2}.", "WINAPI_ReadParamPresetCard", intRst, "fileParam=" + fileParam + ",fileMoney=" + fileMoney + ",filePrice1=" + filePrice1 + ",filePrice2=" + filePrice2 + ",cardNum=" + cardNum), 7, 0);
            }
            catch (Exception ex)
            {
                MessageController.Instance.AddMessage(string.Format("调用{0}异常{1}.", "WINAPI_ReadParamPresetCard", ex.Message), 7, 2);
            }
        }
        public void WINAPI_MakeParamPresetCard(string fileParam, string fileMoney, string filePrice1, string filePrice2)
        {
            try
            {
                int intRst = 1;
                if (IsCmd)
                {
                    bool blnRst = MeterProtocolAdapter.Instance.SouthWriteParamPresetCard(SelectBw, fileParam, fileMoney, filePrice1, filePrice2);
                    intRst = blnRst ? 0 : 1;
                }
                else
                {
                    intRst = CLDC_SafeFileProtocol.CardAPI.CardCtrlAPI.WINAPI_MakeParamPresetCard(fileParam, fileMoney, filePrice1, filePrice2);
                }
                MessageController.Instance.AddMessage(string.Format("调用{0}返回{1}.", "WINAPI_MakeParamPresetCard", intRst), 7, 0);
            }
            catch (Exception ex)
            {
                MessageController.Instance.AddMessage(string.Format("调用{0}异常{1}.", "WINAPI_MakeParamPresetCard", ex.Message), 7, 2);
            }
        }
        public void WINAPI_ReadUserCard(out string fileParam, out string fileMoney, out string filePrice1, out string filePrice2, out string fileReply, out string enfileControl, out string cardNum)
        {
            fileParam = "";
            fileMoney = "";
            filePrice1 = "";
            filePrice2 = "";
            fileReply = "";
            enfileControl = "";
            cardNum = "";
            int intRst = 1;
            try
            {
                if (IsCmd)
                {
                    bool blnRst = MeterProtocolAdapter.Instance.SouthReadUserCard(SelectBw, out fileParam, out fileMoney, out filePrice1, out filePrice2, out fileReply, out enfileControl);
                    intRst = blnRst ? 0 : 1;
                }
                else
                {
                    StringBuilder sfileParam = new StringBuilder(1024);
                    StringBuilder sfileMoney = new StringBuilder(1024);
                    StringBuilder sfilePrice1 = new StringBuilder(1024);
                    StringBuilder sfilePrice2 = new StringBuilder(1024);
                    StringBuilder sfileReply = new StringBuilder(1024);
                    StringBuilder senfileControl = new StringBuilder(1024);
                    StringBuilder scardNum = new StringBuilder(1024);

                    intRst = CLDC_SafeFileProtocol.CardAPI.CardCtrlAPI.WINAPI_ReadUserCard(sfileParam, sfileMoney, sfilePrice1, sfilePrice2, sfileReply, senfileControl, scardNum);
                    if (intRst == 0)
                    {
                        fileParam = sfileParam.ToString().Replace("\0", "");
                        fileMoney = sfileMoney.ToString().Replace("\0", "");
                        filePrice1 = sfilePrice1.ToString().Replace("\0", "");
                        filePrice2 = sfilePrice2.ToString().Replace("\0", "");
                        fileReply = sfileReply.ToString().Replace("\0", "");
                        enfileControl = senfileControl.ToString().Replace("\0", "");
                        cardNum = scardNum.ToString().Replace("\0", "");
                    }
                }
                MessageController.Instance.AddMessage(string.Format("调用{0}返回{1},出参：{2}.", "WINAPI_ReadUserCard", intRst, "fileParam=" + fileParam + ",fileMoney=" + fileMoney + ",filePrice1=" + filePrice1 + ",filePrice2=" + filePrice2 + ",fileReply=" + fileReply + ",enfileControl=" + enfileControl + ",cardNum=" + cardNum), 7, 0);
            }
            catch (Exception ex)
            {
                MessageController.Instance.AddMessage(string.Format("调用{0}异常{1}.", "WINAPI_ReadUserCard", ex.Message), 7, 2);
            }
        }
        public void WINAPI_MakeUserCard(string fileParam, string fileMoney, string filePrice1, string filePrice2, string fileControl)
        {
            try
            {
                int intRst = 1;
                if (IsCmd)
                {
                    bool blnRst = MeterProtocolAdapter.Instance.SouthWriteUserCard(SelectBw, fileParam, fileMoney, filePrice1, filePrice2, "00".PadLeft(100, '0'), fileControl);
                    intRst = blnRst ? 0 : 1;
                }
                else
                {
                    intRst = CLDC_SafeFileProtocol.CardAPI.CardCtrlAPI.WINAPI_MakeUserCard(fileParam, fileMoney, filePrice1, filePrice2, fileControl);
                }
                MessageController.Instance.AddMessage(string.Format("调用{0}返回{1}.", "WINAPI_MakeUserCard", intRst), 7, 0);
            }
            catch (Exception ex)
            {
                MessageController.Instance.AddMessage(string.Format("调用{0}异常{1}.", "WINAPI_MakeUserCard", ex.Message), 7, 2);
            }
        }
        #endregion

        #region 南网统一接口
        /// <summary>
        /// 控制表位和误差板 功能号：1进入跳闸检测、2进入合闸检测、3读取外置继电器状态(使用出参)、4控制开路检测功能断开继电器命令、5控制开路检测功能启用继电器命令、6切换到220V输出外置式跳闸、7切换到开关量输出跳闸
        /// </summary>
        /// <param name="TypeNo">功能号：1进入跳闸检测、2进入合闸检测、3读取外置继电器状态(使用出参)、4控制开路检测功能断开继电器命令、5控制开路检测功能启用继电器命令、6切换到220V输出外置式跳闸、7切换到开关量输出跳闸</param>
        /// <returns></returns>
        public bool SetRelayControl(int TypeNo)
        {
            //if (GlobalUnit.DeviceManufacturers == CLDC_Comm.Enum.Cus_DeviceManufacturers.科陆DLL)
            //{
            //    if (!GlobalUnit.IsDan)
            //    {
            //        if (TypeNo == 1)
            //        {
            //            TypeNo = 2;
            //        }
            //    }
            //}
            bool rst = m_DeviceDriver.SetRelayControl(TypeNo);
            string strmessage = rst ? "成功" : "失败";
            MessageController.Instance.AddMessage("控制表位和误差板接口：" + strmessage, 7, 0);
            return rst;
        }

        /// <summary>
        /// 控制表位继电器是否旁路   旁路继电器状态：1：旁路 0：正常
        /// </summary>
        /// <param name="ControlType">旁路继电器状态：1：旁路 0：正常</param>
        /// <returns></returns>
        public bool SetLoadRelayControl(bool[] MeterPosition, int ControlType)
        {

            bool rst = m_DeviceDriver.SetLoadRelayControl(MeterPosition, ControlType);
            string strmessage = rst ? "成功" : "失败";
            MessageController.Instance.AddMessage("调用控制表位旁路继电器接口：" + strmessage, 7, 0);
            return rst;
        }

        /// <summary>
        /// 检定装置参数配置界面
        /// </summary>
        public void ShowDriverConfig()
        {
            m_DeviceDriver.ShowDriverConfig();
            MessageController.Instance.AddMessage("调用ShowDriverConfig接口");
        }

        /// <summary>
        /// 读写卡器参数配置
        /// </summary>
        public void ShowCardReaderConfig()
        {
            m_DeviceDriver.ShowCardReaderConfig();
            MessageController.Instance.AddMessage("调用ShowCardReaderConfig接口");
        }

        #endregion

        #region 南网一类参数操作
        /// <summary>
        /// 读报警金额1
        /// </summary>
        public void ReadAlertingMoney1()
        {
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);
            string[] AlertingMoney1 = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            MessageController.Instance.AddMessage("正在进行读取报警金额1限值,请稍候....");
            AlertingMoney1 = MeterProtocolAdapter.Instance.ReadData("04001001", 4);
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                MessageController.Instance.AddMessage("第" + (i + 1) + "表位的报警金额1限值为：" + AlertingMoney1[i]);
            }
            PowerOff();
        }

        /// <summary>
        /// 设置报警金额1
        /// </summary>
        /// <param name="AlertingMoney1"></param>
        public void SetAlertingMoney1(string AlertingMoney1)
        {
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);
            string[] strRand1 = new string[m_BwCount];//随机数
            string[] strRand2 = new string[m_BwCount];//随机
            string[] strEsamNo = new string[m_BwCount];//Esam序列号
            string[] strPutApdu = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            string[] strData = new string[m_BwCount];
            int[] iFlag = new int[m_BwCount];
            bool[] Result = new bool[m_BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            MessageController.Instance.AddMessage("正在进行设置报警金额1限值为" + AlertingMoney1 + "元,请稍候....");
            if (AlertingMoney1.IndexOf('.') > 0)
            {
                string[] AlertingMoney = AlertingMoney1.Split('.');
                AlertingMoney1 = AlertingMoney[0].PadLeft(6, '0') + AlertingMoney[1].PadRight(2, '0');
            }
            else
            {
                AlertingMoney1 = AlertingMoney1.PadLeft(6, '0') + "00";
            }
            Common.Memset(ref strID, "04001001");
            Common.Memset(ref strData, AlertingMoney1);
            Common.Memset(ref strPutApdu, "04D6811008");
            Result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                string strMessage =  Result[i] ? "成功" : "失败";
                MessageController.Instance.AddMessage("设置第" + (i + 1) + "表位的报警金额1限值：" + strMessage, 6, Result[i] ? 0 : 1);
            }
            PowerOff();
        }

        /// <summary>
        /// 读报警金额2
        /// </summary>
        public void ReadAlertingMoney2()
        {
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);
            string[] AlertingMoney2 = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            MessageController.Instance.AddMessage("正在进行读取报警金额2限值,请稍候....");
            AlertingMoney2 = MeterProtocolAdapter.Instance.ReadData("04001002", 4);
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                MessageController.Instance.AddMessage("第" + (i + 1) + "表位的报警金额2限值为：" + AlertingMoney2[i]);
            }
            PowerOff();
        }

        /// <summary>
        /// 设置报警金额2
        /// </summary>
        /// <param name="AlertingMoney2"></param>
        public void SetAlertingMoney2(string AlertingMoney2)
        {
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);
            string[] strRand1 = new string[m_BwCount];//随机数
            string[] strRand2 = new string[m_BwCount];//随机
            string[] strEsamNo = new string[m_BwCount];//Esam序列号
            string[] strPutApdu = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            string[] strData = new string[m_BwCount];
            int[] iFlag = new int[m_BwCount];
            bool[] Result = new bool[m_BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            MessageController.Instance.AddMessage("正在进行设置报警金额2限值为" + AlertingMoney2 + "元,请稍候....");
            if (AlertingMoney2.IndexOf('.') > 0)
            {
                string[] AlertingMoney = AlertingMoney2.Split('.');
                AlertingMoney2 = AlertingMoney[0].PadLeft(6, '0') + AlertingMoney[1].PadRight(2, '0');
            }
            else
            {
                AlertingMoney2 = AlertingMoney2.PadLeft(6, '0') + "00";
            }
            Common.Memset(ref strID, "04001002");
            Common.Memset(ref strData, AlertingMoney2);
            Common.Memset(ref strPutApdu, "04D6811408");
            Result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                string strMessage = Result[i] ? "成功" : "失败";
                MessageController.Instance.AddMessage("设置第" + (i + 1) + "表位的报警金额2限值：" + strMessage, 6, Result[i] ? 0 : 1);
            }
            PowerOff();
        }

        /// <summary>
        /// 读电流互感器变比
        /// </summary>
        public void ReadCurrentScale()
        {
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);
            string[] CurrentScale = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            MessageController.Instance.AddMessage("正在进行读取电流互感器变比,请稍候....");
            CurrentScale = MeterProtocolAdapter.Instance.ReadData("04000306", 3);
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                MessageController.Instance.AddMessage("第" + (i + 1) + "表位的电流互感器变比为：" + CurrentScale[i]);
            }
            PowerOff();
        }

        /// <summary>
        /// 设置电流互感器变比
        /// </summary>
        /// <param name="CurrentScale"></param>
        public void SetCurrentScale(string CurrentScale)
        {
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);
            string[] strRand1 = new string[m_BwCount];//随机数
            string[] strRand2 = new string[m_BwCount];//随机
            string[] strEsamNo = new string[m_BwCount];//Esam序列号
            string[] strPutApdu = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            string[] strData = new string[m_BwCount];
            int[] iFlag = new int[m_BwCount];
            bool[] Result = new bool[m_BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            MessageController.Instance.AddMessage("正在进行设置电流互感器变比为" + CurrentScale + ",请稍候....");
            CurrentScale = CurrentScale.PadLeft(6, '0');
            Common.Memset(ref strID, "04000306");
            Common.Memset(ref strData, CurrentScale);
            Common.Memset(ref strPutApdu, "04D6811807");
            Result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                string strMessage = Result[i] ? "成功" : "失败";
                MessageController.Instance.AddMessage("设置第" + (i + 1) + "表位的电流互感器变比：" + strMessage, 6, Result[i] ? 0 : 1);
            }
            PowerOff();
        }

        /// <summary>
        /// 读电压互感器变比
        /// </summary>
        public void ReadVoltageScale()
        {
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);
            string[] VoltageScale = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            MessageController.Instance.AddMessage("正在进行读取电压互感器变比,请稍候....");
            VoltageScale = MeterProtocolAdapter.Instance.ReadData("04000307", 3);
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                MessageController.Instance.AddMessage("第" + (i + 1) + "表位的电压互感器变比为：" + VoltageScale[i]);
            }
            PowerOff();
        }

        /// <summary>
        /// 设置电压互感器变比
        /// </summary>
        /// <param name="VoltageScale"></param>
        public void SetVoltageScale(string VoltageScale)
        {
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);
            string[] strRand1 = new string[m_BwCount];//随机数
            string[] strRand2 = new string[m_BwCount];//随机
            string[] strEsamNo = new string[m_BwCount];//Esam序列号
            string[] strPutApdu = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            string[] strData = new string[m_BwCount];
            int[] iFlag = new int[m_BwCount];
            bool[] Result = new bool[m_BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            MessageController.Instance.AddMessage("正在进行设置电压互感器变比为" + VoltageScale + ",请稍候....");
            VoltageScale = VoltageScale.PadLeft(6, '0');
            Common.Memset(ref strID, "04000307");
            Common.Memset(ref strData, VoltageScale);
            Common.Memset(ref strPutApdu, "04D6811B07");
            Result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                string strMessage = Result[i] ? "成功" : "失败";
                MessageController.Instance.AddMessage("设置第" + (i + 1) + "表位的电压互感器变比：" + strMessage, 6, Result[i] ? 0 : 1);
            }
            PowerOff();
        }

        /// <summary>
        /// 读身份认证时效
        /// </summary>
        public void ReadIdentityTime()
        {
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);
            string[] IdentityTime = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            string[] strRand1 = new string[m_BwCount];//随机数
            string[] strRand2 = new string[m_BwCount];//随机
            string[] strEsamNo = new string[m_BwCount];//Esam序列号
            int[] iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            MessageController.Instance.AddMessage("正在进行读取身份认证时效,请稍候....");
            IdentityTime = MeterProtocolAdapter.Instance.ReadData("02800022", 2);
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                MessageController.Instance.AddMessage("第" + (i + 1) + "表位的身份认证时效值为：" + IdentityTime[i]);
            }
            PowerOff();
        }

        /// <summary>
        /// 设置身份认证时效
        /// </summary>
        /// <param name="VoltageScale"></param>
        public void SetIdentityTime(string IdentityTime)
        {
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);
            string[] strRand1 = new string[m_BwCount];//随机数
            string[] strRand2 = new string[m_BwCount];//随机
            string[] strEsamNo = new string[m_BwCount];//Esam序列号
            string[] strPutApdu = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            string[] strData = new string[m_BwCount];
            int[] iFlag = new int[m_BwCount];
            bool[] Result = new bool[m_BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            MessageController.Instance.AddMessage("正在进行设置身份认证时效为" + IdentityTime + ",请稍候....");
            IdentityTime = IdentityTime.PadLeft(4, '0');
            Common.Memset(ref strID, "070001FF");
            Common.Memset(ref strData, IdentityTime);
            Common.Memset(ref strPutApdu, "04D6812B06");
            Result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                string strMessage = Result[i] ? "成功" : "失败";
                MessageController.Instance.AddMessage("设置第" + (i + 1) + "表位的身份认证时效：" + strMessage, 6, Result[i] ? 0 : 1);
            }
            PowerOff();
        }

        /// <summary>
        /// 钱包初始化
        /// </summary>
        /// <param name="strMoney"></param>
        public void SetInitPurse(string strMoney)
        {
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);

            string[] strRand1 = new string[m_BwCount];//随机数
            string[] strRand2 = new string[m_BwCount];//随机
            string[] strEsamNo = new string[m_BwCount];//Esam序列号
            string[] strData = new string[m_BwCount];
            int[] iFlag = new int[m_BwCount];
            string[] strIniMoney = new string[m_BwCount];
            string strMoneyTmp = "";
            if (strMoney.Contains("."))
            {
                strMoneyTmp = strMoney.Split('.')[1];
                if (strMoneyTmp.Length == 2)
                {
                    strMoney = strMoney.Replace(".", "");
                }
                else if (strMoneyTmp.Length == 1)
                {
                    strMoney = strMoney.Replace(".", "") + "0";
                }
                int iMoneyTmp = Convert.ToInt32(strMoney);
                strMoneyTmp = (iMoneyTmp).ToString("X2").PadLeft(8, '0');
            }
            else
            {
                strMoneyTmp = (Convert.ToInt32(strMoney) * 100).ToString("X2").PadLeft(8, '0');
            }
            CLDC_DataCore.Function.Common.Memset(ref strIniMoney, strMoneyTmp);
            CLDC_DataCore.Function.Common.Memset(ref iFlag, 0);

            bool[] Result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            MessageController.Instance.AddMessage("正在进行钱包初始化,请稍候....");
            Result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strIniMoney);
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                string strMessage = Result[i] ? "成功" : "失败";
                MessageController.Instance.AddMessage("设置第" + (i + 1) + "表位的钱包初始化：" + strMessage, 6, Result[i] ? 0 : 1);
            }
            PowerOff();
        }

        /// <summary>
        /// 读费率
        /// </summary>
        public void ReaderFl()
        {
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);

            string[] strFl = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            MessageController.Instance.AddMessage("正在进行读取当前套费率值,请稍候....");
            strFl = MeterProtocolAdapter.Instance.ReadData("040501FF", 48);
            strFl = RevString(strFl, 8);
            

            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                MessageController.Instance.AddMessage("第" + (i + 1) + "表位的当前套费率值为：" + strFl[i]);
            }
            PowerOff();
        }

        /// <summary>
        /// 设置费率
        /// </summary>
        /// <param name="VoltageScale"></param>
        public void SetFl(string strFl)
        {
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);

            string[] strRand1 = new string[m_BwCount];//随机数
            string[] strRand2 = new string[m_BwCount];//随机
            string[] strEsamNo = new string[m_BwCount];//Esam序列号
            string[] strPutApdu = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            string[] strData = new string[m_BwCount];
            int[] iFlag = new int[m_BwCount];
            bool[] Result = new bool[m_BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            string[] strValueTmp = strFl.Split(',');
            strFl = FormatSetFl(strValueTmp[0], 4, 4);
            strFl += FormatSetFl(strValueTmp[1], 4, 4);
            strFl += FormatSetFl(strValueTmp[2], 4, 4);
            strFl += FormatSetFl(strValueTmp[3], 4, 4);

            MessageController.Instance.AddMessage("正在进行设置备用套费率值,请稍候....");
            Common.Memset(ref strID, "040502FF");
            Common.Memset(ref strData, strFl);
            Common.Memset(ref strPutApdu, "04D6840414");
            Result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                string strMessage = Result[i] ? "成功" : "失败";
                MessageController.Instance.AddMessage("设置第" + (i + 1) + "表位的费率值：" + strMessage, 6, Result[i] ? 0 : 1);
            }
            PowerOff();
        }

        /// <summary>
        /// 读第1阶梯表
        /// </summary>
        public void ReaderJtBy1()
        {
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);

            string[] JtBy1 = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            MessageController.Instance.AddMessage("正在进行读取当前套第1阶梯表,请稍候....");
            JtBy1 = MeterProtocolAdapter.Instance.ReadData("040606FF", 70);

            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (JtBy1[i].Length == 140)
                {
                    string strJt = JtBy1[i].Substring(36, 104);
                    string strJsr = JtBy1[i].Substring(0, 36);
                    strJt = RevString(strJt, 8);
                    strJsr = RevString(strJsr, 6);
                    JtBy1[i] = strJt + strJsr;
                }
            }

            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                MessageController.Instance.AddMessage("第" + (i + 1) + "表位的当前套第1阶梯表为：" + JtBy1[i]);
            }
            PowerOff();
        }

        /// <summary>
        /// 设置第1阶梯表
        /// </summary>
        /// <param name="VoltageScale"></param>
        public void SetJtBy1(string strPara)
        {
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);

            string[] strRand1 = new string[m_BwCount];//随机数
            string[] strRand2 = new string[m_BwCount];//随机
            string[] strEsamNo = new string[m_BwCount];//Esam序列号
            string[] strPutApdu = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            string[] strData = new string[m_BwCount];
            int[] iFlag = new int[m_BwCount];
            bool[] Result = new bool[m_BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            string[] strValueTmp = strPara.Split(',');
            strPara = FormatSetJtz(strValueTmp[0], 6, 2);
            strPara += FormatSetJtz(strValueTmp[1], 6, 2);
            strPara += FormatSetJtz(strValueTmp[2], 6, 2);
            strPara += FormatSetJtz(strValueTmp[3], 6, 2);
            strPara += FormatSetJtz(strValueTmp[4], 6, 2);
            strPara += FormatSetJtz(strValueTmp[5], 6, 2);
            strPara += FormatSetFl(strValueTmp[6], 4, 4);
            strPara += FormatSetFl(strValueTmp[7], 4, 4);
            strPara += FormatSetFl(strValueTmp[8], 4, 4);
            strPara += FormatSetFl(strValueTmp[9], 4, 4);
            strPara += FormatSetFl(strValueTmp[10], 4, 4);
            strPara += FormatSetFl(strValueTmp[11], 4, 4);
            strPara += FormatSetFl(strValueTmp[12], 4, 4);
            strPara += strValueTmp[13];

            MessageController.Instance.AddMessage("正在进行设置备用套第1阶梯表,请稍候....");
            Common.Memset(ref strID, "04060AFF");
            Common.Memset(ref strData, strPara);
            Common.Memset(ref strPutApdu, "04D684344A");
            Result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                string strMessage = Result[i] ? "成功" : "失败";
                MessageController.Instance.AddMessage("设置第" + (i + 1) + "表位的备用套第1阶梯表：" + strMessage, 6, Result[i] ? 0 : 1);
            }


            PowerOff();
        }

        /// <summary>
        /// 读第2阶梯表
        /// </summary>
        public void ReaderJtBy2()
        {
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);

            string[] JtBy2 = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            MessageController.Instance.AddMessage("正在进行读取当前套第2阶梯表,请稍候....");
            JtBy2 = MeterProtocolAdapter.Instance.ReadData("040607FF", 70);

            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (JtBy2[i].Length == 140)
                {
                    string strJt = JtBy2[i].Substring(36, 104);
                    string strJsr = JtBy2[i].Substring(0, 36);
                    strJt = RevString(strJt, 8);
                    strJsr = RevString(strJsr, 6);
                    JtBy2[i] = strJt + strJsr;
                }
            }

            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                MessageController.Instance.AddMessage("第" + (i + 1) + "表位的当前套第2阶梯表为：" + JtBy2[i]);
            }
            PowerOff();
        }

        /// <summary>
        /// 设置第2阶梯表
        /// </summary>
        /// <param name="VoltageScale"></param>
        public void SetJtBy2(string strPara)
        {
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);

            string[] strRand1 = new string[m_BwCount];//随机数
            string[] strRand2 = new string[m_BwCount];//随机
            string[] strEsamNo = new string[m_BwCount];//Esam序列号
            string[] strPutApdu = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            string[] strData = new string[m_BwCount];
            int[] iFlag = new int[m_BwCount];
            bool[] Result = new bool[m_BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            string[] strValueTmp = strPara.Split(',');
            strPara = FormatSetJtz(strValueTmp[0], 6, 2);
            strPara += FormatSetJtz(strValueTmp[1], 6, 2);
            strPara += FormatSetJtz(strValueTmp[2], 6, 2);
            strPara += FormatSetJtz(strValueTmp[3], 6, 2);
            strPara += FormatSetJtz(strValueTmp[4], 6, 2);
            strPara += FormatSetJtz(strValueTmp[5], 6, 2);
            strPara += FormatSetFl(strValueTmp[6], 4, 4);
            strPara += FormatSetFl(strValueTmp[7], 4, 4);
            strPara += FormatSetFl(strValueTmp[8], 4, 4);
            strPara += FormatSetFl(strValueTmp[9], 4, 4);
            strPara += FormatSetFl(strValueTmp[10], 4, 4);
            strPara += FormatSetFl(strValueTmp[11], 4, 4);
            strPara += FormatSetFl(strValueTmp[12], 4, 4);
            strPara += strValueTmp[13];

            MessageController.Instance.AddMessage("正在进行设置备用套第2阶梯表,请稍候....");
            Common.Memset(ref strID, "04060BFF");
            Common.Memset(ref strData, strPara);
            Common.Memset(ref strPutApdu, "04D6847A4A");
            Result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                string strMessage = Result[i] ? "成功" : "失败";
                MessageController.Instance.AddMessage("设置第" + (i + 1) + "表位的备用套第2阶梯表：" + strMessage, 6, Result[i] ? 0 : 1);
            }

            PowerOff();
        }

        /// <summary>
        /// 读两套费率电价切换时间
        /// </summary>
        public void ReaderChangFlTime()
        {
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);

            string[] ChangFlTime = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            MessageController.Instance.AddMessage("正在进行读取两套费率电价切换时间,请稍候....");
            ChangFlTime = MeterProtocolAdapter.Instance.ReadData("04000108", 10);
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                MessageController.Instance.AddMessage("第" + (i + 1) + "表位的两套费率电价切换时间为：" + ChangFlTime[i]);
            }
            PowerOff();
        }

        public void SetChangFlTime(string ChangTime)
        {
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);

            string[] strRand1 = new string[m_BwCount];//随机数
            string[] strRand2 = new string[m_BwCount];//随机
            string[] strEsamNo = new string[m_BwCount];//Esam序列号
            string[] strPutApdu = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            string[] strData = new string[m_BwCount];
            int[] iFlag = new int[m_BwCount];
            bool[] Result = new bool[m_BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            MessageController.Instance.AddMessage("正在进行设置两套费率电价切换时间,请稍候....");
            Common.Memset(ref strID, "04000108");
            Common.Memset(ref strData, ChangTime);
            Common.Memset(ref strPutApdu, "04D6810A09");
            Result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                string strMessage = Result[i] ? "成功" : "失败";
                MessageController.Instance.AddMessage("设置第" + (i + 1) + "表位的两套费率电价切换时间：" + strMessage, 6, Result[i] ? 0 : 1);
            }

            PowerOff();
        }

        /// <summary>
        /// 读两套阶梯切换时间
        /// </summary>
        public void ReaderChangJtTime()
        {
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);

            string[] ChangJtTime = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            MessageController.Instance.AddMessage("正在进行读取两套阶梯切换时间,请稍候....");
            ChangJtTime = MeterProtocolAdapter.Instance.ReadData("04000108", 10);
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                MessageController.Instance.AddMessage("第" + (i + 1) + "表位的两套阶梯切换时间为：" + ChangJtTime[i]);
            }
            PowerOff();
        }

        public void SetChangJtTime(string ChangTime)
        {
            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);

            string[] strRand1 = new string[m_BwCount];//随机数
            string[] strRand2 = new string[m_BwCount];//随机
            string[] strEsamNo = new string[m_BwCount];//Esam序列号
            string[] strPutApdu = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            string[] strData = new string[m_BwCount];
            int[] iFlag = new int[m_BwCount];
            bool[] Result = new bool[m_BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            MessageController.Instance.AddMessage("正在进行设置两套阶梯切换时间,请稍候....");
            Common.Memset(ref strID, "04000109");
            Common.Memset(ref strData, ChangTime);
            Common.Memset(ref strPutApdu, "04D684C009");
            Result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                string strMessage = Result[i] ? "成功" : "失败";
                MessageController.Instance.AddMessage("设置第" + (i + 1) + "表位的两套阶梯切换时间：" + strMessage, 6, Result[i] ? 0 : 1);
            }

            PowerOff();
        }


        public string FormatSetFl(string SetValue,int PadLeftCount,int PadRightCount)
        {
            string strRetValue = "";
            if (SetValue.IndexOf('.') > 0)
            {
                string[] strRetValueTmp = SetValue.Trim().Split('.');
                strRetValue = strRetValueTmp[0].PadLeft(PadLeftCount, '0') + strRetValueTmp[1].PadRight(PadRightCount, '0');
            }
            else
            {
                strRetValue = SetValue.PadLeft(4, '0') + "0000";
            }
            return strRetValue;
        }

        public string FormatSetJtz(string SetValue, int PadLeftCount, int PadRightCount)
        {
            string strRetValue = "";
            if (SetValue.IndexOf('.') > 0)
            {
                string[] strRetValueTmp = SetValue.Trim().Split('.');
                strRetValue = strRetValueTmp[0].PadLeft(PadLeftCount, '0') + strRetValueTmp[1].PadRight(PadRightCount, '0');
            }
            else
            {
                strRetValue = SetValue.PadLeft(6, '0') + "00";
            }
            return strRetValue;
        }

        /// <summary>
        /// 反转字符串
        /// </summary>
        /// <param name="bData"></param>
        /// <param name="SubCount"></param>
        /// <returns></returns>
        public string[] RevString(string[] bData, int SubCount)
        {

            if (bData.Length <= 0 || bData == null)
            {
                return null;
            }
            string[] bstring = new string[bData.Length];
            for (int j = 0; j < bData.Length; j++)
            {
                bstring[j] = "";
                if (string.IsNullOrEmpty(bData[j]) || bData[j].Length % SubCount != 0) continue;

                for (int i = 1; i <= bData[j].ToString().Length / SubCount; i++)
                {
                    bstring[j] += bData[j].ToString().Substring(bData[j].ToString().Length - i * SubCount, SubCount);
                }
            }
            return bstring;
        }

        public string RevString(string bData, int SubCount)
        {

            if (string.IsNullOrEmpty(bData))
            {
                return "";
            }
            if (bData.Length % SubCount != 0)
            {
                return bData;
            }
            string bstring = "";

            for (int i = 1; i <= bData.Length / SubCount; i++)
            {
                bstring += bData.Substring(bData.Length - i * SubCount, SubCount);
            }

            return bstring;
        }

        #endregion

        #region 南网快捷命令

        /// <summary>
        /// 对时
        /// </summary>
        public void Timing()
        {
            string[] strRand1 = new string[m_BwCount];//随机数
            string[] strRand2 = new string[m_BwCount];//随机
            string[] strEsamNo = new string[m_BwCount];//Esam序列号
            string[] strPutApdu = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            string[] strData = new string[m_BwCount];
            int[] iFlag = new int[m_BwCount];
            bool[] Result = new bool[m_BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            for (int i = 0; i < m_BwCount; i++)
            {
                strID[i] = "0400010C";
                strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strData[i] += DateTime.Now.ToString("HHmmss");
            }
            Result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID); 
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                string strMessage = Result[i] ? "成功" : "失败";
                MessageController.Instance.AddMessage("设置第" + (i + 1) + "表位的日期时间：" + strMessage, 6, Result[i] ? 0 : 1);
            }
        }
        /// <summary>
        /// 报警功能
        /// </summary>
        /// <param name="strControl">1个字节命令</param>
        public void WarnFun(string strControl)
        {
            string[] strRand1 = new string[m_BwCount];//随机数
            string[] strRand2 = new string[m_BwCount];//随机
            string[] strEsamNo = new string[m_BwCount];//Esam序列号
            string[] strPutApdu = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            string[] strData = new string[m_BwCount];
            int[] iFlag = new int[m_BwCount];
            bool[] Result = new bool[m_BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            MessageController.Instance.AddMessage("正在下发 报警命令,请稍候....");
            Common.Memset(ref strData, strControl + "00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
            Result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                string strMessage = Result[i] ? "成功" : "失败";
                MessageController.Instance.AddMessage("设置第" + (i + 1) + "表位的报警命令：" + strMessage, 6, Result[i] ? 0 : 1);
            }
        }
        /// <summary>
        /// 直接拉闸
        /// </summary>
        public void BreakRelay()
        {
            string[] strRand1 = new string[m_BwCount];//随机数
            string[] strRand2 = new string[m_BwCount];//随机
            string[] strEsamNo = new string[m_BwCount];//Esam序列号
            string[] strPutApdu = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            string[] strData = new string[m_BwCount];
            int[] iFlag = new int[m_BwCount];
            bool[] Result = new bool[m_BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            MessageController.Instance.AddMessage("正在下发跳闸命令,请稍候....");
            Common.Memset(ref strData, "1A00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
            Result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                string strMessage = Result[i] ? "成功" : "失败";
                MessageController.Instance.AddMessage("设置第" + (i + 1) + "表位的跳闸命令：" + strMessage, 6, Result[i] ? 0 : 1);
            }
        }

        /// <summary>
        /// 直接合闸
        /// </summary>
        public void DirectRemoteControl()
        {
            string[] strRand1 = new string[m_BwCount];//随机数
            string[] strRand2 = new string[m_BwCount];//随机
            string[] strEsamNo = new string[m_BwCount];//Esam序列号
            string[] strPutApdu = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            string[] strData = new string[m_BwCount];
            int[] iFlag = new int[m_BwCount];
            bool[] Result = new bool[m_BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            MessageController.Instance.AddMessage("正在通过远程发送直接合闸命令,请稍候....");
            string strDateTime = System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
            Common.Memset(ref strData, "1C00" + strDateTime);
            Result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                string strMessage = Result[i] ? "成功" : "失败";
                MessageController.Instance.AddMessage("设置第" + (i + 1) + "表位的直接合闸命令：" + strMessage, 6, Result[i] ? 0 : 1);
            }
        }

        /// <summary>
        /// 合闸允许
        /// </summary>
        public void CloseRelay()
        {
            string[] strRand1 = new string[m_BwCount];//随机数
            string[] strRand2 = new string[m_BwCount];//随机
            string[] strEsamNo = new string[m_BwCount];//Esam序列号
            string[] strPutApdu = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            string[] strData = new string[m_BwCount];
            int[] iFlag = new int[m_BwCount];
            bool[] Result = new bool[m_BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            MessageController.Instance.AddMessage("正在通过远程发送合闸允许命令,请稍候....");
            string strDateTime = System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
            Common.Memset(ref strData, "1B00" + strDateTime);
            Result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                string strMessage = Result[i] ? "成功" : "失败";
                MessageController.Instance.AddMessage("设置第" + (i + 1) + "表位的合闸允许命令：" + strMessage, 6, Result[i] ? 0 : 1);
            }
        }

        /// <summary>
        /// 保电
        /// </summary>
        public void EnPower()
        {
            string[] strRand1 = new string[m_BwCount];//随机数
            string[] strRand2 = new string[m_BwCount];//随机
            string[] strEsamNo = new string[m_BwCount];//Esam序列号
            string[] strPutApdu = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            string[] strData = new string[m_BwCount];
            int[] iFlag = new int[m_BwCount];
            bool[] Result = new bool[m_BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            MessageController.Instance.AddMessage("正在通过远程发送保电命令,请稍候....");
            string strDateTime = System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
            Common.Memset(ref strData, "3A00" + strDateTime);
            Result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                string strMessage = Result[i] ? "成功" : "失败";
                MessageController.Instance.AddMessage("设置第" + (i + 1) + "表位的保电命令：" + strMessage, 6, Result[i] ? 0 : 1);
            }
        }

        /// <summary>
        /// 保电解除
        /// </summary>
        public void RelieveEnPower()
        {
            string[] strRand1 = new string[m_BwCount];//随机数
            string[] strRand2 = new string[m_BwCount];//随机
            string[] strEsamNo = new string[m_BwCount];//Esam序列号
            string[] strPutApdu = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            string[] strData = new string[m_BwCount];
            int[] iFlag = new int[m_BwCount];
            bool[] Result = new bool[m_BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            MessageController.Instance.AddMessage("正在通过远程发送保电解除命令,请稍候....");
            string strDateTime = System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
            Common.Memset(ref strData, "3B00" + strDateTime);
            Result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                string strMessage = Result[i] ? "成功" : "失败";
                MessageController.Instance.AddMessage("设置第" + (i + 1) + "表位的保电解除命令：" + strMessage, 6, Result[i] ? 0 : 1);
            }
        }

        /// <summary>
        /// 切换到本地模式
        /// </summary>
        public void ChangeLocalMode()
        {
            string[] strRand1 = new string[m_BwCount];//随机数
            string[] strRand2 = new string[m_BwCount];//随机
            string[] strEsamNo = new string[m_BwCount];//Esam序列号
            string[] strPutApdu = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            string[] strData = new string[m_BwCount];
            int[] iFlag = new int[m_BwCount];
            bool[] Result = new bool[m_BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            MessageController.Instance.AddMessage("下发模式切为本地模式命令,请稍候....");
            Common.Memset(ref strData, "00" + "00000000" + "00000000");
            Result = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlag, strRand2, strData);

            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                string strMessage = Result[i] ? "成功" : "失败";
                MessageController.Instance.AddMessage("设置第" + (i + 1) + "表位的模式切为本地模式命令：" + strMessage, 6, Result[i] ? 0 : 1);
            }
        }

        /// <summary>
        /// 切换到远程模式
        /// </summary>
        public void ChangeRemoteModel()
        {
            string[] strRand1 = new string[m_BwCount];//随机数
            string[] strRand2 = new string[m_BwCount];//随机
            string[] strEsamNo = new string[m_BwCount];//Esam序列号
            string[] strPutApdu = new string[m_BwCount];
            string[] strID = new string[m_BwCount];
            string[] strData = new string[m_BwCount];
            int[] iFlag = new int[m_BwCount];
            bool[] Result = new bool[m_BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            MessageController.Instance.AddMessage("下发模式切为远程模式命令,请稍候....");
            Common.Memset(ref strData, "01" + "00000000" + "00000000");
            Result = MeterProtocolAdapter.Instance.SouthSwitchChargeMode(iFlag, strRand2, strData);

            for (int i = 0; i < m_BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                string strMessage = Result[i] ? "成功" : "失败";
                MessageController.Instance.AddMessage("设置第" + (i + 1) + "表位的模式切为远程模式命令：" + strMessage, 6, Result[i] ? 0 : 1);
            }
        }

        #endregion

        public bool SetErrCalcType(int calcType)
        {
            return m_DeviceDriver.SetErrCalcType(calcType);
        }

        public bool ReadTestEnergy(out float testEnergy, out long PulseNum)
        {
            return m_DeviceDriver.ReadTestEnergy(out  testEnergy, out  PulseNum);
        }
    }
}
