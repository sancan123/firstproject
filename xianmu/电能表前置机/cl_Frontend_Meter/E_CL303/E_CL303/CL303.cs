using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using E_CLSocketModule.Enum;
using E_CLSocketModule.SocketModule.Packet;
using System.Threading;
using E_CLSocketModule.Struct;
using E_CLSocketModule;

namespace E_CL303
{
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
    ComVisible(true)]
    public interface IClass_Interface
    {
        /// <summary>
        /// 注册2018端口号
        /// </summary>
        /// <param name="ComNumber">端口号</param>
        /// <param name="MaxWaitTime">最大等待时间</param>
        /// <param name="IP">Ip地址</param>
        /// <param name="RemotePort">远程端口号</param>
        /// <param name="LocalStartPort">本地端口号</param>
        /// <returns></returns>
        [DispId(1)]
        int InitSetting(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte, string IP, int RemotePort, int LocalStartPort, string HaveProtocol);
        /// <summary>
        /// 注册Com 口
        /// </summary>
        /// <param name="ComNumber">Com 口号</param>
        /// <param name="MaxWaitTime">最大等待时间</param>
        /// <param name="WaitSencondsPerByte">字节最大等待时间</param>
        /// <returns></returns>
        [DispId(2)]
        int InitSettingCom(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte);
        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="FrameAry">输出上行报文</param>
        /// <returns></returns>
        [DispId(3)]
        int Connect(out string[] FrameAry);
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="FrameAry">输出上行报文</param>
        /// <returns></returns>
        [DispId(4)]
        int DisConnect(out string[] FrameAry);
        /// <summary>
        /// 升源
        /// </summary>
        /// <param name="clfs">接线方式</param>
        /// <param name="Ub">电压</param>
        /// <param name="Ib">电流</param>
        /// <param name="ele">功率元件</param>
        /// <param name="glfx">功率方向</param>
        /// <param name="strGlys">功率因数</param>
        /// <param name="FrameAry">输出上行报文</param>
        /// <returns></returns>
        [DispId(5)]
        int PowerOn(int clfs, byte byt_xwkz, int glfx, string strGlys, float sng_Ub_A, float sng_Ub_B, float sng_Ub_C, float sng_Ib_A, float sng_Ib_B, float sng_Ib_C, int element, float sng_Freq, bool bln_IsNxx, out string[] FrameAry);
        /// <summary>
        /// 自由升源
        /// </summary>
        /// <param name="sng_xUb_A">A相电压</param>
        /// <param name="sng_xUb_B">B相电压</param>
        /// <param name="sng_xUb_C">C相电压</param>
        /// <param name="sng_xIb_A">A相电流</param>
        /// <param name="sng_xIb_B">B相电流</param>
        /// <param name="sng_xIb_C">C相电流</param>
        /// <param name="sng_UaPhi">A相电压角度</param>
        /// <param name="sng_UbPhi">B相电压角度</param>
        /// <param name="sng_UcPhi">C相电压角度</param>
        /// <param name="sng_IaPhi">A相电流角度</param>
        /// <param name="sng_IbPhi">B相电流角度</param>
        /// <param name="sng_IcPhi">C相电流角度</param>
        /// <param name="sng_Freq">频率</param>
        /// <param name="FrameAry">输出上行报文</param>
        /// <returns></returns>
        [DispId(6)]
        int PowerOnFree(byte byt_xwkz, double Ua, double Ub, double Uc, double Ia, double Ib, double Ic, double PhiUa, double PhiUb, double PhiUc, double PhiIa, double PhiIb, double PhiIc, float Hz, bool b_xiebo, out string[] FrameAry);
        /// <summary>
        /// 降源
        /// </summary>
        /// <param name="FrameAry">输出上行报文</param>
        /// <returns></returns>
        [DispId(7)]
        int PowerOff(out string[] FrameAry);
        /// <summary>
        /// 设置谐波参数
        /// </summary>
        /// <param name="Phase">数组各元素：0=A相电压，1=B相电压，2=C相电压，3=A相电流，4=B相电流，5=C相电流</param>
        /// <param name="int_XTSwitch">各相开关，数组元素值：0=不加谐波，1=加谐波，</param>
        /// <param name="sng_Value">各次谐波含量</param>
        /// <param name="sng_Phase">各次谐波相位</param>
        /// <param name="frameAry">合成上行报文</param>
        /// <returns></returns>
        [DispId(9)]
        //bool SetHarmonic(int[][] int_XTSwitch, Single[][] sng_Value, Single[][] sng_Phase);
        int SetHarmonic(int Phase, int[] int_XTSwitch, Single[] sng_Value, Single[] sng_Phase, out string[] frameAry);
        /// <summary>
        /// <summary>
        /// 设置发送标志
        /// </summary>
        /// <param name="Flag">是否发送数据标志</param>
        /// <returns></returns>
        [DispId(8)]
        int SetSendFlag(bool Flag);
        /// <summary>
        /// 解析下行报文
        /// </summary>
        /// <param name="MothedName">方法名称</param>
        /// <param name="ReFrameAry">下行报文</param>
        /// <param name="ReAry">输出解析的数据</param>
        /// <returns></returns>
        [DispId(9)]
        int UnPacket(string MothedName, byte[] ReFrameAry, out string[] ReAry);

    }


    [Guid("A4FC3E59-6216-4b51-9AEF-0E0AA0CBAA15"),
    InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
    ComVisible(true)]
    public interface IClass_Events
    {
        [DispId(80)]
        void MsgCallBack(string szMessage);
    }



    [Guid("D9807A3F-6375-4333-9CB3-B46811FA6C74"),
    ProgId("CLOU.CL303"),
    ClassInterface(ClassInterfaceType.None),
    ComDefaultInterface(typeof(IClass_Interface)),
    ComSourceInterfaces(typeof(IClass_Events)),
    ComVisible(true)]
    public class CL303 : IClass_Interface
    {
        //public delegate void MsgCallBackDelegate(string szMessage);
        //public event MsgCallBackDelegate MsgCallBack;
        /// <summary>
        /// 重试次数
        /// </summary>
        public static int RETRYTIEMS = 1;
        /// <summary>
        /// 源控制端口
        /// </summary>
        public StPortInfo m_PowerPort = null;

        DriverBase driverBase = null;
        //发送标志
        private bool sendFlag = true;



        public CL303()
        {
            m_PowerPort = new StPortInfo();
            driverBase = new DriverBase();
        }

        public int InitSetting(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte, string IP, int RemotePort, int LocalStartPort, string HaveProtocol)
        {
            m_PowerPort.m_Exist = 1;
            m_PowerPort.m_IP = IP;
            m_PowerPort.m_Port = ComNumber;
            m_PowerPort.m_Port_Type = Cus_EmComType.UDP;
            m_PowerPort.m_Port_Setting = "9600,n,8,1";
            driverBase.RegisterPort(ComNumber, m_PowerPort.m_Port_Setting, m_PowerPort.m_IP, RemotePort, LocalStartPort, HaveProtocol, MaxWaitTime, WaitSencondsPerByte);
            return 0;
        }
        /// <summary>
        /// 注册Com 口
        /// </summary>
        /// <param name="ComNumber">端口号</param>
        /// <param name="MaxWaitTime"></param>
        /// <param name="WaitSencondsPerByte"></param>
        /// <returns></returns>
        public int InitSettingCom(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte)
        {
            m_PowerPort = new StPortInfo();
            m_PowerPort.m_Exist = 1;
            m_PowerPort.m_IP = "";
            m_PowerPort.m_Port = ComNumber;
            m_PowerPort.m_Port_Type = Cus_EmComType.COM;
            m_PowerPort.m_Port_Setting = "9600,n,8,1";
            driverBase.RegisterPort(ComNumber, "9600,n,8,1", MaxWaitTime, WaitSencondsPerByte);
            return 0;
        }
        public int Connect(out string[] FrameAry)
        {
            FrameAry = new string[1];
            CL303_RequestReadVersionPacket sp = new CL303_RequestReadVersionPacket();
            CL303_RequestReadVersionReplayPacket rp = new CL303_RequestReadVersionReplayPacket();
            try
            {
                FrameAry[0] = BytesToString(sp.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(m_PowerPort, sp, rp))
                    {
                        if (rp.Version != null && rp.Version.Length > 0)
                        {
                            return 0;
                        }
                    }

                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }

        }
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int DisConnect(out string[] FrameAry)
        {
            FrameAry = new string[1];

            return 0;
        }
        /// <summary>
        /// 升源
        /// </summary>
        /// <param name="clfs">接线方式</param>
        /// <param name="Ub">电压</param>
        /// <param name="Ib">电流</param>
        /// <param name="ele">功率元件</param>
        /// <param name="glfx">功率方向</param>
        /// <param name="strGlys">功率因数</param>
        /// <param name="FrameAry">输出上行报文</param>
        /// <returns></returns>
        public int PowerOn(int clfs, byte byt_xwkz, int glfx, string strGlys, float sng_Ub_A, float sng_Ub_B, float sng_Ub_C, float sng_Ib_A, float sng_Ib_B, float sng_Ib_C, int element, float sng_Freq, bool bln_IsNxx, out string[] FrameAry)
        {

            FrameAry = new string[1];

            int myclfs = getClfs((Cus_EmClfs)clfs, (Cus_EmPowerFangXiang)glfx);

            try
            {

                CL303_RequestSetPowerOnPacket0x30 rcpower = new CL303_RequestSetPowerOnPacket0x30();
                CL303_RequestSetPowerOnPacket0x30.UPara upra = new CL303_RequestSetPowerOnPacket0x30.UPara();
                upra.Ua = sng_Ub_A;
                upra.Ub = sng_Ub_B;
                upra.Uc = sng_Ub_C;
                upra.Ia = sng_Ib_A;
                upra.Ib = sng_Ib_B;
                upra.Ic = sng_Ib_C;
                CL303_RequestSetPowerOnPacket0x30.PhiPara ppra = new CL303_RequestSetPowerOnPacket0x30.PhiPara();
                //ppra.PhiIa = sng_IaPhi;
                //ppra.PhiIb = sng_IbPhi;
                //ppra.PhiIc = sng_IcPhi;
                //ppra.PhiUa = sng_UaPhi;
                //ppra.PhiUb = sng_UbPhi;
                //ppra.PhiUc = sng_UcPhi;
                byte byt_XWKG = 63;
                //Cus_EmPowerYuanJiang phase;
                switch ((Cus_EmPowerYuanJiang)element)
                {
                    case Cus_EmPowerYuanJiang.A:
                        //phase = Cus_EmPowerYuanJiang.A;
                        byt_XWKG &= 0xf;

                        break;
                    case Cus_EmPowerYuanJiang.B:
                        //phase = Cus_EmPowerYuanJiang.B;
                        byt_XWKG &= 0x17;
                        break;
                    case Cus_EmPowerYuanJiang.C:
                        //phase = Cus_EmPowerYuanJiang.C;
                        byt_XWKG &= 0x27;
                        break;
                    default:
                        //phase = Cus_EmPowerYuanJiang.H;

                        break;
                }

                byte clfswiringMode = 0;
                switch ((Cus_EmClfs)clfs)
                {
                    case Cus_EmClfs.PT4:
                        if ((Cus_EmPowerFangXiang)glfx == Cus_EmPowerFangXiang.ZXP || (Cus_EmPowerFangXiang)glfx == Cus_EmPowerFangXiang.FXP)
                        {
                            clfswiringMode = 0;
                        }
                        else
                        {
                            clfswiringMode = 1;
                        }
                        break;
                    case Cus_EmClfs.PT3:
                        if ((Cus_EmPowerFangXiang)glfx == Cus_EmPowerFangXiang.ZXP || (Cus_EmPowerFangXiang)glfx == Cus_EmPowerFangXiang.FXP)
                        {
                            clfswiringMode = 2;
                        }
                        else
                        {
                            clfswiringMode = 3;
                        }

                        break;
                    case Cus_EmClfs.P:
                        clfswiringMode = 7;
                        break;

                }


                //float phi = 0;

                //switch (strGlys)
                //{
                //    case "1.0":
                //        phi = 0;
                //        break;
                //    case "0.5L":
                //        phi = 60;
                //        break;
                //    case "0.5C":
                //        phi = 300;
                //        break;
                //    case "0.8L":
                //        phi = 36.8F;
                //        break;
                //    case "0.8C":
                //        phi = 323.2F;
                //        break;
                //    case "0.25L":
                //        phi = 75.5F;
                //        break;
                //    case "0.25C":
                //        phi = 284.5F;
                //        break;

                //}

                ////if (loadPhase == LoadPhase.None)
                ////{
                ////为了电量寄存器计算 所以在合元时我们通过计算得到功率因数
                //phi = GetPhiGlys((int)clfs, strGlys);     //根据测试方式和功率因数计算角度


                //}
                //else
                //{
                //    phi = GetPhiGlys((int)clfs, StrGlys, loadPhase);//根据测试方式、功率因数和合分元计算角度
                //}

                rcpower.SetPara(clfswiringMode, upra, ppra);
                //rcpower.m_xwkz = byt_XWKG;
                byte btmp = 0;
                for (int kk = 0; kk < 6; kk++)
                {
                    if ((rcpower.m_xwkz >> kk) % 2 == 1 && (byt_xwkz >> kk) % 2 == 1)
                        btmp += Convert.ToByte(Math.Pow(2, kk));
                }
                rcpower.m_xwkz = btmp;
                rcpower.SetAcSourcePowerFactor(strGlys, (byte)myclfs, true, element, bln_IsNxx);
                FrameAry[0] = BytesToString(rcpower.GetPacketData());

                if (!doResult(rcpower, m_PowerPort, "sy"))
                {
                    return 1;
                }

                return 0;


                #region 作废

                //CL303_RequestSetPowerOnPacket0x35 rcpower = new CL303_RequestSetPowerOnPacket0x35();

                //byte byt_XWKG = 63;
                //Cus_PowerYuanJiang phase;
                //switch ((Cus_PowerYuanJiang)element)
                //{
                //    case Cus_PowerYuanJiang.A:
                //        phase = Cus_PowerYuanJiang.A;
                //        byt_XWKG &= 0xf;
                //        break;
                //    case Cus_PowerYuanJiang.B:
                //        phase = Cus_PowerYuanJiang.B;
                //        byt_XWKG &= 0x17;
                //        break;
                //    case Cus_PowerYuanJiang.C:
                //        phase = Cus_PowerYuanJiang.C;
                //        byt_XWKG &= 0x27;
                //        break;
                //    default:
                //        phase = Cus_PowerYuanJiang.H;

                //        break;
                //}
                //rcpower.m_YuanJian = phase;
                //byte clfswiringMode = 0;
                //switch ((Cus_Clfs)clfs)
                //{
                //    case Cus_Clfs.PT4:
                //        if ((Cus_PowerFangXiang)glfx == Cus_PowerFangXiang.ZXP || (Cus_PowerFangXiang)glfx == Cus_PowerFangXiang.FXP)
                //        {
                //            clfswiringMode = 0;
                //        }
                //        else
                //        {
                //            clfswiringMode = 1;
                //        }
                //        break;
                //    case Cus_Clfs.PT3:
                //        if ((Cus_PowerFangXiang)glfx == Cus_PowerFangXiang.ZXP || (Cus_PowerFangXiang)glfx == Cus_PowerFangXiang.FXP)
                //        {
                //            clfswiringMode = 2;
                //        }
                //        else
                //        {
                //            clfswiringMode = 3;
                //        }

                //        break;
                //    case Cus_Clfs.P:
                //        clfswiringMode = 7;
                //        break;

                //}

                //float phi = 0;

                //switch (strGlys)
                //{
                //    case "1.0":
                //        phi = 0;
                //        break;
                //    case "0.5L":
                //        phi = 60;
                //        break;
                //    case "0.5C":
                //        phi = 300;
                //        break;
                //    case "0.8L":
                //        phi = 36.8F;
                //        break;
                //    case "0.8C":
                //        phi = 323.2F;
                //        break;
                //    case "0.25L":
                //        phi = 75.5F;
                //        break;
                //    case "0.25C":
                //        phi = 284.5F;
                //        break;

                //}
                ////为了电量寄存器计算 所以在合元时我们通过计算得到功率因数
                //phi = GetPhiGlys((int)clfs, strGlys);     //根据测试方式和功率因数计算角度

                //rcpower.SetPara(clfswiringMode, byt_XWKG, false, Ub, Ib, false, phi);
                ////
                //FrameAry[0] = BytesToString(rcpower.GetPacketData());
                ////
                //if (!doResult(rcpower, m_PowerPort, "sy"))
                //{
                //    return 1;
                //}
                //return 0;

                #endregion
            }
            catch (Exception)
            {

                return -1;
            }

        }

        /// <summary>
        /// 转换当前要升源的测量方式
        /// 中的测试方式定义与检定器定义不一致。
        /// </summary>
        /// <param name="Clfs">测量方式0-7</param>
        /// <param name="pd">功率方向</param>
        /// <returns>测量方式</returns>
        private byte getClfs(Cus_EmClfs Clfs, Cus_EmPowerFangXiang pd)
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
            bool IsYouGong = ((pd == Cus_EmPowerFangXiang.ZXP) || (pd == Cus_EmPowerFangXiang.FXP));
            byte clfs = (byte)Clfs;
            //if (clfs == 5)                            //单相台统一划分为三相四线
            //{
            //    clfs = 0;
            //}
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


        /// <summary>
        /// 自由升源
        /// </summary>
        /// <param name="clfs">测量方式</param>
        /// <param name="glfx">功率方向</param>
        /// <param name="strGlys">功率因数</param>
        /// <param name="sng_xUb_A"></param>
        /// <param name="sng_xUb_B"></param>
        /// <param name="sng_xUb_C"></param>
        /// <param name="sng_xIb_A"></param>
        /// <param name="sng_xIb_B"></param>
        /// <param name="sng_xIb_C"></param>
        /// <param name="element"></param>
        /// <param name="sng_UaPhi"></param>
        /// <param name="sng_UbPhi"></param>
        /// <param name="sng_UcPhi"></param>
        /// <param name="sng_IaPhi"></param>
        /// <param name="sng_IbPhi"></param>
        /// <param name="sng_IcPhi"></param>
        /// <param name="sng_Freq"></param>
        /// <param name="bln_IsNxx"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        //public int PowerOnFree(int clfs
        //    , int glfx
        //    , string strGlys
        //    , float sng_xUb_A, float sng_xUb_B, float sng_xUb_C
        //    , float sng_xIb_A, float sng_xIb_B, float sng_xIb_C
        //    , int element
        //    , float sng_UaPhi, float sng_UbPhi, float sng_UcPhi
        //    , float sng_IaPhi, float sng_IbPhi, float sng_IcPhi
        //    , float sng_Freq
        //    , bool bln_IsNxx,out string[] FrameAry)
        public int PowerOnFree(byte byt_xwkz, double Ua, double Ub, double Uc,
                                double Ia, double Ib, double Ic,
                                double PhiUa, double PhiUb, double PhiUc,
                                double PhiIa, double PhiIb, double PhiIc,
                                float Hz, bool b_xiebo, out string[] FrameAry)
        {
            FrameAry = new string[1];
            try
            {
                CL303_RequestSetPowerOnPacket0x30 rcpower = new CL303_RequestSetPowerOnPacket0x30();
                CL303_RequestSetPowerOnPacket0x30.UPara upra = new CL303_RequestSetPowerOnPacket0x30.UPara();
                upra.Ua = Convert.ToSingle(Ua);
                upra.Ub = Convert.ToSingle(Ub);
                upra.Uc = Convert.ToSingle(Uc);
                upra.Ia = Convert.ToSingle(Ia);
                upra.Ib = Convert.ToSingle(Ib);
                upra.Ic = Convert.ToSingle(Ic);
                CL303_RequestSetPowerOnPacket0x30.PhiPara ppra = new CL303_RequestSetPowerOnPacket0x30.PhiPara();
                ppra.PhiUa = Convert.ToSingle(PhiUa);
                ppra.PhiUb = Convert.ToSingle(PhiUb);
                ppra.PhiUc = Convert.ToSingle(PhiUc);
                ppra.PhiIa = Convert.ToSingle(PhiIa);
                ppra.PhiIb = Convert.ToSingle(PhiIb);
                ppra.PhiIc = Convert.ToSingle(PhiIc);

                rcpower.OpenXieBo = b_xiebo;
                //byte byt_XWKG = 63;
                //}
                //else
                //{
                //    phi = GetPhiGlys((int)clfs, StrGlys, loadPhase);//根据测试方式、功率因数和合分元计算角度
                //}
                rcpower.SetPara(1, upra, ppra);
                rcpower.Freq = Hz;
                byte btmp = 0;
                for (int kk = 0; kk < 6; kk++)
                {
                    if ((rcpower.m_xwkz >> kk) % 2 == 1 && (byt_xwkz >> kk) % 2 == 1)
                        btmp += Convert.ToByte(Math.Pow(2, kk));
                }
                rcpower.m_xwkz = btmp;
                FrameAry[0] = BytesToString(rcpower.GetPacketData());

                if (!doResult(rcpower, m_PowerPort, "sy"))
                {
                    return 1;
                }

                return 0;
            }
            catch (Exception)
            {

                return -1;
            }

        }

        /// <summary>
        /// 设置谐波参数
        /// </summary>
        /// <param name="Phase">相别A相电压 = 0,B相电压 = 1,C相电压 = 2,A相电流 = 3,B相电流 = 4,C相电流 = 5</param>
        /// <param name="int_XTSwitch">各相各次开关,数组元素值：0=不加谐波，1=加谐波，各次（0-31）</param>
        /// <param name="sng_Value">幅值（最高到32次）</param>
        /// <param name="sng_Phase">相位（最高到32次）</param>
        /// <param name="frameAry">输出上行报文</param>
        /// <returns></returns>
        public int SetHarmonic(int Phase, int[] int_XTSwitch, Single[] sng_Value, Single[] sng_Phase, out string[] FrameAry)
        {
            FrameAry = new string[1];
            try
            {
                Cl303_RequestSetXieBoPacket rcpower = new Cl303_RequestSetXieBoPacket();

                rcpower.YuanJian = Convert.ToByte(Phase);//相别

                rcpower.YuanJian = Convert.ToByte(48 + Phase);
                rcpower.SetPara(sng_Value, sng_Phase);
                FrameAry[0] = BytesToString(rcpower.GetPacketData());

                if (!doResult(rcpower, m_PowerPort, "sy"))
                {
                    return 1;
                }

                return 0;
            }
            catch (Exception)
            {

                return -1;
            }
        }

        /// <summary>
        /// 关源
        /// </summary>
        /// <param name="FrameAry">输出上行报文</param>
        /// <returns></returns>
        public int PowerOff(out string[] FrameAry)
        {
            FrameAry = new string[1];
            try
            {

                CL303_RequestSetPowerOffPacket rc = new CL303_RequestSetPowerOffPacket();
                CLNormalRequestResultReplayPacket recv = new CLNormalRequestResultReplayPacket();
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (!SendPacketWithRetry(m_PowerPort, rc, recv))
                {
                    return 1;
                }

                return 0;
            }
            catch (Exception)
            {

                return -1;
            }

        }
        /// <summary>
        /// 设置标志位
        /// </summary>
        /// <param name="Flag">标志</param>
        /// <returns></returns>
        public int SetSendFlag(bool Flag)
        {
            sendFlag = Flag;
            return 0;
        }
        /// <summary>
        /// 解析下行报文
        /// </summary>
        /// <param name="MothedName">方法名称</param>
        /// <param name="ReFrameAry">下行报文</param>
        /// <param name="ReAry">返回解析后的数据</param>
        /// <returns></returns>
        public int UnPacket(string MothedName, byte[] ReFrameAry, out string[] ReAry)
        {
            MothedName = MothedName.Replace(" ", "");
            ReAry = new string[1];
            int ReValue = 3;
            try
            {
                switch (MothedName)
                {
                    case "Connect":
                        {
                            // 连接设备 int Connect(out string[] FrameAry);
                            CL303_RequestReadVersionReplayPacket recv = new CL303_RequestReadVersionReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.Version;
                            if (string.IsNullOrEmpty(recv.Version))
                            {
                                ReValue = 1;
                            }
                            ReValue = 0;
                        }
                        break;
                    case "DisConnect":
                        {
                            // 断开连接 int DisConnect(out string[] FrameAry);
                            ReValue = 3;
                        }
                        break;
                    case "PowerOn":
                        {
                            // 升源 int PowerOn(int clfs, float Ub, float Ib, int ele,int glfx, string strGlys,out string[] FrameAry);
                            CLNormalRequestResultReplayPacket recv = new CLNormalRequestResultReplayPacket();
                            recv.ParsePacket(ReFrameAry);

                            ReAry[0] = recv.ReplayResult.ToString();
                            if (recv.ReplayResult != CLNormalRequestResultReplayPacket.ReplayCode.Ok)
                            {
                                ReValue = 1;
                            }
                            ReValue = 0;
                        }
                        break;
                    case "PowerOnFree":
                        {
                            // 自由升源
                            //int PowerOnFree(int clfs
                            //    , int glfx
                            //    , string strGlys
                            //    , float sng_xUb_A, float sng_xUb_B, float sng_xUb_C
                            //    , float sng_xIb_A, float sng_xIb_B, float sng_xIb_C
                            //    , int element
                            //    , float sng_UaPhi, float sng_UbPhi, float sng_UcPhi
                            //    , float sng_IaPhi, float sng_IbPhi, float sng_IcPhi
                            //    , float sng_Freq
                            //    , bool bln_IsNxx, out string[] FrameAry);
                            CLNormalRequestResultReplayPacket recv = new CLNormalRequestResultReplayPacket();
                            recv.ParsePacket(ReFrameAry);

                            ReAry[0] = recv.ReplayResult.ToString();
                            if (recv.ReplayResult != CLNormalRequestResultReplayPacket.ReplayCode.Ok)
                            {
                                ReValue = 1;
                            }
                            ReValue = 0;
                        }
                        break;
                    case "PowerOff":
                        {
                            // 降源    int PowerOff(out string []FrameAry);
                            CLNormalRequestResultReplayPacket recv = new CLNormalRequestResultReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReplayResult.ToString();
                            if (recv.ReplayResult != CLNormalRequestResultReplayPacket.ReplayCode.Ok)
                            {
                                ReValue = 1;
                            }
                            ReValue = 0;
                        }
                        break;
                    default:
                        {
                            ReValue = 3;
                        }
                        break;

                }
            }
            catch (Exception)
            {
                return -1;
            }
            return ReValue;
        }


        #region private


        /// <summary>
        /// 字节数组转字符串
        /// </summary>
        /// <param name="bytesData"></param>
        /// <returns></returns>
        private string BytesToString(byte[] bytesData)
        {
            string strRevalue = string.Empty;
            if (bytesData == null || bytesData.Length < 1)
                return strRevalue;

            strRevalue = BitConverter.ToString(bytesData).Replace("-", "");

            return strRevalue;
        }

        /// <summary>
        /// 计算角度
        /// </summary>
        /// <param name="int_Clfs">测量方式</param>
        /// <param name="str_Glys">功率因数</param>
        /// <returns></returns>
        private Single GetPhiGlys(int int_Clfs, string str_Glys)
        {
            string str_CL = str_Glys.ToUpper().Substring(str_Glys.Length - 1, 1);
            Double dbl_XS = 0;
            if (str_CL == "C" || str_CL == "L")
                dbl_XS = Convert.ToDouble(str_Glys.Substring(0, str_Glys.Length - 1));
            else
                dbl_XS = Convert.ToDouble(str_Glys);
            Double dbl_Phase;
            if (int_Clfs == 1 || int_Clfs == 3 || int_Clfs == 6)
                dbl_Phase = Math.Asin(Math.Abs(dbl_XS));                              //无功计算
            else
                dbl_Phase = Math.Acos(Math.Abs(dbl_XS));                              //有功计算

            dbl_Phase = dbl_Phase * 180 / Math.PI;      //角度换算
            if (dbl_XS < 0) dbl_Phase = 180 + dbl_Phase;         //反向
            if (str_CL == "C") dbl_Phase = 360 - dbl_Phase;
            if (dbl_Phase < 0) dbl_Phase = 360 + dbl_Phase;
            dbl_Phase %= 360;
            return Convert.ToSingle(dbl_Phase);
        }
        /// <summary>
        /// 发送返回CLNormalRequestResultReplayPacket的数据包并返回结果
        /// </summary>
        /// <param name="rac"></param>
        /// <param name="stPort"></param>
        /// <param name="ename"></param>
        /// <returns></returns>
        private bool doResult(SendPacket rac, StPortInfo stPort, string ename)
        {
            bool result = true;

            RecvPacket rcback = new CLNormalRequestResultReplayPacket();
            result = this.SendPacketWithRetry(stPort, rac, rcback);
            if (result)
            {
                if (((CLNormalRequestResultReplayPacket)rcback).ReplayResult
                    != CLNormalRequestResultReplayPacket.ReplayCode.Ok)
                {
                    result = false;

                }

            }
            else
            { }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stPort"></param>
        /// <param name="UDPorCOM">true UDP,false COM</param>
        /// <param name="sp"></param>
        /// <param name="rp"></param>
        /// <returns></returns>
        private bool SendPacketWithRetry(StPortInfo stPort, SendPacket sp, RecvPacket rp)
        {

            {
                for (int i = 0; i < RETRYTIEMS; i++)
                {
                    if (driverBase.SendData(stPort, sp, rp) == true)
                    {
                        return true;
                    }
                    Thread.Sleep(300);
                }
            }

            return false;
        }
        #endregion private
    }
}
