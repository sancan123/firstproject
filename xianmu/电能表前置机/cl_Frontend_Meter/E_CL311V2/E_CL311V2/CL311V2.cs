using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using E_CLSocketModule.Enum;
using E_CLSocketModule.Struct;
using E_CLSocketModule;
using E_CLSocketModule.SocketModule.Packet;
using E_CL311V2.Device;

namespace E_CL311V2
{
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
    ComVisible(true)]
    public interface IClass_Interface
    {
        /// <summary>
        /// 初始化设备通讯参数
        /// </summary>
        /// <param name="ComNumber">端口号</param>
        /// <param name="MaxWaitTme">最长等待时间</param>
        /// <param name="WaitSencondsPerByte">帧字节间隔时间</param>
        /// <param name="IP">Ip地址</param>
        /// <param name="RemotePort">远程端口</param>
        /// <param name="LocalStartPort">本地端口</param>
        /// <returns>是否注册成功</returns>
        [DispId(1)]
        int InitSetting(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte, string IP, int RemotePort, int LocalStartPort, string HaveProtocol);
        /// <summary>
        /// 注册Com 口
        /// </summary>
        /// <param name="ComNumber"></param>
        /// <param name="strSetting"></param>
        /// <param name="maxWaittime"></param>
        /// <returns></returns>
        [DispId(2)]
        int InitSettingCom(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte);
        /// <summary>
        /// 连机
        /// </summary>
        /// <returns></returns>
        [DispId(3)]
        int Connect(out string[] FrameAry);
        /// <summary>
        /// 断开连机
        /// </summary>
        /// <returns></returns>
        [DispId(4)]
        int DisConnect(out string[] FrameAry);
        /// <summary>
        /// 读实时测量数据
        /// </summary>
        /// <param name="instValue"></param>
        /// <returns></returns>
        [DispId(5)]
        int ReadInstMetricAll(out float[] instValue, out string[] FrameAry);
        /// <summary>
        /// 读标准表常数
        /// </summary>
        /// <param name="pulseConst"></param>
        /// <returns></returns>
        [DispId(6)]
        int ReadStdPulseConst(out int pulseConst, out string[] FrameAry);
        /// <summary>
        /// 读取电能量
        /// </summary>
        /// <param name="energy">返回电能量</param>
        /// <returns></returns>
        [DispId(7)]
        int ReadEnergy(out float energy, out string[] FrameAry);
        /// <summary>
        /// 读电能量累计脉冲数
        /// </summary>
        /// <param name="pulses">读电能量累计脉冲数</param>
        /// <returns></returns>
        [DispId(8)]
        int ReadTotalPulses(out long pulses, out string[] FrameAry);
        /// <summary>
        /// 读电能走字数据
        /// </summary>
        /// <param name="testEnergy">返回走字电能量</param>
        /// <returns></returns>
        [DispId(9)]
        int ReadTestEnergy(out float testEnergy, out string[] FrameAry);
        /// <summary>
        /// 读仪器版本号
        /// </summary>
        /// <param name="version">版本号</param>
        /// <returns></returns>
        [DispId(10)]
        int ReadVersion(out string version, out string[] FrameAry);
        /// <summary>
        /// 读各项电压电流谐波幅值
        /// </summary>
        /// <param name="phase"></param>
        /// <param name="harmonicArry">65个长度 </param>
        /// <returns></returns>
        [DispId(11)]
        int ReadHarmonicArry(int phase, out float[] harmonicArry, out string[] FrameAry);
        /// <summary>
        /// 读各项电压电流波形数据
        /// </summary>
        /// <param name="phase"></param>
        /// <param name="waveformArry"> 256个长度</param>
        /// <returns></returns>
        [DispId(12)]
        int ReadWaveformArry(int phase, out float[] waveformArry, out string[] FrameAry);
        /// <summary>
        /// 设置接线方式
        /// </summary>
        /// <param name="wiringMode">接线类型</param>
        /// <returns></returns>
        [DispId(13)]
        int SetWiringMode(int wiringMode, out string[] FrameAry);
        /// <summary>
        /// 设置标准表常数
        /// </summary>
        /// <param name="pulseConst">标准表常数</param>
        /// <returns></returns>
        [DispId(14)]
        int SetStdPulseConst(int pulseConst, out string[] FrameAry);
        /// <summary>
        /// 设置电能指示
        /// </summary>
        /// <param name="powerMode">1：总有功电能 2：总无功电能</param>
        /// <returns></returns>
        [DispId(15)]
        int SetPowerMode(int powerMode, out string[] FrameAry);
        /// <summary>
        /// 设置电能误差计算启动开关
        /// </summary>
        /// <param name="calcType">0 停止计算  </param>  
        ///                        1 开始计算电能误差  
        ///                        2 开始计算电能走字
        /// <returns></returns>
        [DispId(16)]
        int SetErrCalcType(int calcType, out string[] FrameAry);
        /// <summary>
        /// 设置电能参数
        /// </summary>
        /// <param name="wiringMode">接线方式</param>
        /// <param name="powerMode">电能方式</param>
        /// <param name="calcType">电能误差计算开关</param>
        /// <returns></returns>
        [DispId(17)]
        int SetStdParams(int wiringMode, int powerMode, int calcType, out string[] FrameAry);
        /// <summary>
        /// 设置档位
        /// </summary>
        /// <param name="UaRange">Ua档位</param>
        /// <param name="UbRange">Ub档位</param>
        /// <param name="UcRange">Uc档位</param>
        /// <param name="IaRange">Ia档位</param>
        /// <param name="IbRange">Ib档位</param>
        /// <param name="IcRange">Ic档位</param>
        /// <returns></returns>
        [DispId(18)]
        int SetRange(float UaRange, float UbRange, float UcRange, float IaRange, float IbRange, float IcRange, out string[] FrameAry);
        /// <summary>
        /// 设置显示界面
        /// </summary>
        /// <param name="formType">界面类型</param>
        /// <returns></returns>
        [DispId(19)]
        int SetDisplayForm(int formType, out string[] FrameAry);
        /// <summary>
        /// 设置电能误差检定参数
        /// </summary>
        /// <param name="pulseNum"></param>
        /// <param name="testConst"></param>
        /// <returns></returns>
        [DispId(20)]
        int SetCalcParams(int pulseNum, int testConst, out string[] FrameAry);
        /// <summary>
        /// 读取误差和脉冲数
        /// </summary>
        /// <param name="pulses">输出8路脉冲</param>
        /// <param name="errors">输出8录误差</param>
        /// <returns></returns>
        [DispId(21)]
        int ReadError(out int[] pulses, out float[] errors, out string[] FrameAry);
        /// <summary>
        /// 读取最近一次误差及误差次数
        /// </summary>
        /// <param name="num"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        [DispId(22)]
        int ReadLastError(out int num, out float error, out string[] FrameAry);
        /// <summary>
        /// 启动标准表
        /// </summary>
        /// <returns></returns>
        [DispId(23)]
        int StartStdMeter(out string[] FrameAry);
        /// <summary>
        /// 退出界面
        /// </summary>
        /// <returns></returns>
        [DispId(24)]
        int ExitForm(out string[] FrameAry);
        /// <summary>
        /// 控制电流量限
        /// </summary>
        /// <param name="currentType"> 0:25A、1:100A</param>
        /// <returns></returns>
        [DispId(25)]
        int SetCurrentMeasure(int currentType, out string[] FrameAry);
        /// <summary>
        /// 设置标准表参数
        /// </summary>
        /// <param name="meterConst"></param>
        /// <param name="circle"></param>
        /// <param name="currentMeasure"></param>
        /// <param name="wiringMode"></param>
        /// <returns></returns>
        [DispId(26)]
        int SetStdParms(int meterConst, int circle, int currentType, int wiringMode, out string[] FrameAry);

        /// <summary>
        /// 设置获取请求报文标志
        /// </summary>
        /// <param name="Flag">True:发送报文,并传出报文,false:不发送,只传出报文</param>
        /// <returns></returns>
        [DispId(27)]
        int SetSendFlag(bool Flag);
        /// <summary>
        /// 解析下行报文
        /// </summary>
        /// <param name="MothedName">函数名(有出参FrameAry的函数的名称)</param>
        /// <param name="ReFrameAry">下行报文</param>
        /// <param name="ReAry">解析后的数据</param>
        /// <returns></returns>
        [DispId(28)]
        int UnPacket(string MothedName, byte[] ReFrameAry, out string[] ReAry);
    }
    [Guid("74F48DC5-BBCD-423a-875E-67CCD38E46CA"),
    InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
    ComVisible(true)]
    public interface IClass_Events
    {
        [DispId(80)]
        void MsgCallBack(string szMessage);
    }

    [Guid("1173E98F-F769-4cda-9F24-81D5AE158734"),
    ProgId("CLOU.CL311V2"),
    ClassInterface(ClassInterfaceType.None),
    ComDefaultInterface(typeof(IClass_Interface)),
    ComSourceInterfaces(typeof(IClass_Events)),
    ComVisible(true)]
    public class CL311V2 : IClass_Interface
    {
        //public delegate void MsgCallBackDelegate(string szMessage);
        //public event MsgCallBackDelegate MsgCallBack;

        /// <summary>
        /// 重试次数
        /// </summary>
        public static int RETRYTIEMS = 1;
        /// <summary>
        /// 控制端口
        /// </summary>
        private readonly StPortInfo _MeterStd = null;

        /// <summary>
        /// 
        /// </summary>
        private readonly DriverBase driverBase = null;
        /// <summary>
        /// 发送标志位
        /// </summary>
        private bool sendFlag = true;

        /// <summary>
        /// 构造方法
        /// </summary>
        public CL311V2()
        {
            _MeterStd = new StPortInfo();
            driverBase = new DriverBase();

        }

        #region IClass_Interface 成员
        /// <summary>
        /// 初始化2018端口
        /// </summary>
        /// <param name="ComNumber">端口号</param>
        /// <param name="MaxWaitTime">最长等待时间</param>
        /// <param name="WaitSencondsPerByte"></param>
        /// <param name="IP">2018Ip 地址</param>
        /// <param name="RemotePort">远程端口号</param>
        /// <param name="LocalStartPort">本地端口号</param>
        /// <returns>0 成功 1 失败</returns>
        public int InitSetting(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte, string IP, int RemotePort, int LocalStartPort, string HaveProtocol)
        {
            _MeterStd.m_Exist = 1;
            _MeterStd.m_IP = IP;
            _MeterStd.m_Port = ComNumber;
            _MeterStd.m_Port_Type =  Cus_EmComType.UDP;
            _MeterStd.m_Port_Setting = "9600,n,8,1";
            try
            {
                driverBase.RegisterPort(ComNumber, _MeterStd.m_Port_Setting, _MeterStd.m_IP, RemotePort, LocalStartPort, HaveProtocol, MaxWaitTime, WaitSencondsPerByte);
            }
            catch (Exception)
            {

                return 1;
            }
            return 0;
        }
        /// <summary>
        /// 初始化Com 口
        /// </summary>
        /// <param name="ComNumber">端口号</param>
        /// <param name="MaxWaitTime">最长等待时间</param>
        /// <param name="WaitSencondsPerByte">帧字节等待时间</param>
        /// <returns>0 成功 1 失败</returns>
        public int InitSettingCom(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte)
        {
            _MeterStd.m_Exist = 1;
            _MeterStd.m_IP = "";
            _MeterStd.m_Port = ComNumber;
            _MeterStd.m_Port_Type = Cus_EmComType.COM;
            _MeterStd.m_Port_Setting = "9600,n,8,1";
            try
            {
                driverBase.RegisterPort(ComNumber, "9600,n,8,1", MaxWaitTime, WaitSencondsPerByte);
            }
            catch (Exception)
            {

                return 1;
            }

            return 0;
        }
        /// <summary>
        /// 连机
        /// </summary>
        /// <returns> 0 成功 1 设备返回失败 2 发送失败</returns>
        public int Connect(out string[] FrameAry)
        {
            CL311_RequestLinkPacket rc = new CL311_RequestLinkPacket
            {
                IsLink = true
            };
            Cl311_RequestLinkReplyPacket resv = new Cl311_RequestLinkReplyPacket();
            FrameAry = new string[1];
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(_MeterStd, rc, resv))
                    {
                        return resv.ReciveResult == RecvResult.OK ? 0 : 1;
                    }
                    else
                    {
                        return 2;
                    }
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
        /// 脱机
        /// </summary>
        /// <returns></returns>
        public int DisConnect(out string[] FrameAry)
        {
            CL311_RequestLinkPacket rc = new CL311_RequestLinkPacket
            {
                IsLink = false
            };
            Cl311_RequestLinkReplyPacket resv = new Cl311_RequestLinkReplyPacket();
            FrameAry = new string[1];
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(_MeterStd, rc, resv))
                    {
                        return resv.ReciveResult == RecvResult.OK ? 0 : 1;
                    }
                    else
                    {
                        return 2;
                    }
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

        #endregion

        #region IClass_Interface 成员

        /// <summary>
        /// 读取设备当前瞬时所有信息
        /// </summary>
        /// <param name="instValue">返回检定数据</param>
        /// <returns></returns>
        public int ReadInstMetricAll(out float[] instValue, out string[] FrameAry)
        {
            //stStdInfo stdInfo = new stStdInfo();
            instValue = new float[34];
            FrameAry = new string[1];
            CL311_RequestReadStdParamPacket rc = new CL311_RequestReadStdParamPacket();
            CL311_RequestReadStdInfoReplayPacket recv = new CL311_RequestReadStdInfoReplayPacket();

            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(_MeterStd, rc, recv))
                    {
                        stStdInfo stdInfo = recv.PowerInfo;
                        //电压
                        instValue[0] = stdInfo.Ua;
                        instValue[1] = stdInfo.Ub;
                        instValue[2] = stdInfo.Uc;
                        //电流
                        instValue[3] = stdInfo.Ia;
                        instValue[4] = stdInfo.Ib;
                        instValue[5] = stdInfo.Ic;
                        //电压相位
                        instValue[6] = stdInfo.Phi_Ua;
                        instValue[7] = stdInfo.Phi_Ub;
                        instValue[8] = stdInfo.Phi_Uc;
                        //电流相位
                        instValue[9] = stdInfo.Phi_Ia;
                        instValue[10] = stdInfo.Phi_Ib;
                        instValue[11] = stdInfo.Phi_Ic;
                        //相角
                        instValue[12] = -1f;// stdInfo.PhiAngle_A;
                        instValue[13] = -1f;// stdInfo.PhiAngle_B;
                        instValue[14] = -1f;// stdInfo.PhiAngle_C;
                        //功率相角
                        instValue[15] = -1f; //stdInfo.SAngle;
                        //有功功率
                        instValue[16] = stdInfo.Pa;
                        instValue[17] = stdInfo.Pb;
                        instValue[18] = stdInfo.Pc;
                        instValue[19] = stdInfo.P;
                        //无功功率
                        instValue[20] = stdInfo.Qa;
                        instValue[21] = stdInfo.Qb;
                        instValue[22] = stdInfo.Qc;
                        instValue[23] = stdInfo.Q;
                        //视在功率
                        instValue[24] = stdInfo.Sa;
                        instValue[25] = stdInfo.Sb;
                        instValue[26] = stdInfo.Sc;
                        instValue[27] = stdInfo.S;
                        //功率因数
                        instValue[28] = -1f;//stdInfo.PowerFactor_A;
                        instValue[29] = -1f;//stdInfo.PowerFactor_B;
                        instValue[30] = -1f;//stdInfo.PowerFactor_C;
                        //总有功功率因数
                        instValue[31] = stdInfo.COS;
                        //总无功功率因数
                        instValue[32] = stdInfo.SIN;
                        //频率
                        instValue[33] = stdInfo.Freq;

                        return 0;
                    }
                    else
                    {
                        return 2;
                    }
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
        /// 读取标准表脉冲常数
        /// </summary>
        /// <param name="pulseConst"></param>
        /// <returns></returns>
        public int ReadStdPulseConst(out int pulseConst, out string[] FrameAry)
        {
            pulseConst = -1;
            FrameAry = new string[1];
            CL311_RequestReadStdMeterConstOrPulsePacket rc = new CL311_RequestReadStdMeterConstOrPulsePacket(true);
            Cl311_RequestReadStdMeterConstOrPulseReplayPacket recv = new Cl311_RequestReadStdMeterConstOrPulseReplayPacket();

            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(_MeterStd, rc, recv))
                    {
                        if (recv.ReciveResult == RecvResult.OK)
                        {
                            pulseConst = recv.Data;
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    }

                    return 2;
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
        /// 读标准表电能
        /// </summary>
        /// <param name="energy"></param>
        /// <returns></returns>
        public int ReadEnergy(out float energy, out string[] FrameAry)
        {
            energy = -1f;
            FrameAry = new string[1];
            return 3;
        }
        /// <summary>
        /// 读电能量累计脉冲数
        /// </summary>
        /// <param name="pulses"></param>
        /// <returns></returns>
        public int ReadTotalPulses(out long pulses, out string[] FrameAry)
        {
            pulses = 0;
            CL311_RequestReadStdMeterConstOrPulsePacket rc = new CL311_RequestReadStdMeterConstOrPulsePacket(false);
            Cl311_RequestReadStdMeterConstOrPulseReplayPacket recv = new Cl311_RequestReadStdMeterConstOrPulseReplayPacket();
            FrameAry = new string[1];
            try
            {
                if (sendFlag)
                {
                    if (SendPacketWithRetry(_MeterStd, rc, recv))
                    {
                        if (recv.ReciveResult == RecvResult.OK)
                        {
                            pulses = recv.Data;
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    }

                    return 2;
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
        /// 读取测试电能量
        /// </summary>
        /// <param name="testEnergy"></param>
        /// <returns></returns>
        public int ReadTestEnergy(out float testEnergy, out string[] FrameAry)
        {
            testEnergy = -1f;
            FrameAry = new string[1];
            return 3;
        }

        /// <summary>
        /// 读取版本号
        /// </summary>
        /// <param name="version">版本号</param>
        /// <returns></returns>
        public int ReadVersion(out string version, out string[] FrameAry)
        {
            CL311_RequestReadVersionPacket rc = new CL311_RequestReadVersionPacket();

            CL311_RequestReadVersionReplayPacket recv = new CL311_RequestReadVersionReplayPacket();

            version = string.Empty;
            FrameAry = new string[1];

            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(_MeterStd, rc, recv))
                    {
                        if (recv.ReciveResult == RecvResult.OK)
                        {
                            version = recv.Version;
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    return 2;
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
        /// 读取电压电流谐波幅值
        /// </summary>
        /// <param name="phase"></param>
        /// <param name="harmonicArry"></param>
        /// <returns></returns>
        public int ReadHarmonicArry(int phase, out float[] harmonicArry, out string[] FrameAry)
        {
            byte bPhase = 0x00;
            harmonicArry = new float[16];
            FrameAry = new string[1];
            //0：Uc
            //1：Ub
            //2：Ua
            //3：Ic
            //4：Ib
            //5：Ia
            switch (phase)
            {
                case 0:
                    bPhase = 0x00;//uc
                    break;
                case 1:
                    bPhase = 0xC0;//ub
                    break;
                case 2:
                    bPhase = 0x80;//ua
                    break;
                case 3:
                    bPhase = 0x20;//ic
                    break;
                case 4:
                    bPhase = 0xE0;//ib
                    break;
                case 5:
                    bPhase = 0xA0;//iA
                    break;
                default:
                    break;
            }

            CL311_RequestReadHarmonicDataPacket rc = new CL311_RequestReadHarmonicDataPacket(bPhase);

            CL311_RequestReadHarmonicDataReplayPacket recv = new CL311_RequestReadHarmonicDataReplayPacket();

            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(_MeterStd, rc, recv))
                    {
                        if (recv.ReciveResult == RecvResult.OK)
                        {
                            harmonicArry = recv.Data;
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    }

                    return 2;

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
        /// 读取电压电流波形数据
        /// </summary>
        /// <param name="phase">相别</param>
        /// <param name="waveformArry"></param>
        /// <returns></returns>
        public int ReadWaveformArry(int phase, out float[] waveformArry, out string[] FrameAry)
        {
            waveformArry = new float[256];
            FrameAry = new string[1];
            return 3;
        }
        /// <summary>
        /// 设置接线方式
        /// </summary>
        /// <param name="wiringMode"></param>
        /// <returns></returns>
        public int SetWiringMode(int wiringMode, out string[] FrameAry)
        {
            FrameAry = new string[1];
            //0：三相四线PQ 或 单相
            //1：三相三线PQ
            //2：二元件跨相90
            //3：二元件跨相60
            //4：三元件跨相90

            //注：	本命令适用画面：功率测量、伏安测量、相频测量

            //0	YPQ	
            //1	△PQ	
            //2	△Q90	
            //3	△Q3	
            //4	△Q60°	
            byte byteMode;
            if (wiringMode >= 0 && wiringMode < 6)
            {
                byteMode = Convert.ToByte(wiringMode);
            }
            else
            {
                return 4;
            }

            CL311_RequestOtherWiringModePacket rc = new CL311_RequestOtherWiringModePacket(byteMode);

            CL311_ReplyOkPacket recv = new CL311_ReplyOkPacket();

            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(_MeterStd, rc, recv))
                    {
                        if (recv.ReciveResult == RecvResult.OK)
                        {
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    return 2;
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
        /// 设置标准表常数
        /// </summary>
        /// <param name="pulseConst"></param>
        /// <returns></returns>
        public int SetStdPulseConst(int pulseConst, out string[] FrameAry)
        {
            CL311_RequestSetStdMeterConstPacket rc = new CL311_RequestSetStdMeterConstPacket();
            CL311_ReplyOkPacket recv = new CL311_ReplyOkPacket();
            rc.SetPara(pulseConst, true);

            FrameAry = new string[1];

            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(_MeterStd, rc, recv))
                    {
                        if (recv.ReciveResult == RecvResult.OK)
                        {
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    }

                    return 2;
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
        /// 设置电能指示
        /// </summary>
        /// <param name="powerMode"></param>
        /// <returns></returns>
        public int SetPowerMode(int powerMode, out string[] FrameAry)
        {
            FrameAry = new string[1];
            return 3;
        }
        /// <summary>
        /// 设置电能误差启动开关
        /// </summary>
        /// <param name="calcType"></param>
        /// <returns></returns>
        public int SetErrCalcType(int calcType, out string[] FrameAry)
        {
            FrameAry = new string[1];
            return 3;
        }
        /// <summary>
        /// 设置电能参数
        /// </summary>
        /// <param name="wiringMode"></param>
        /// <param name="powerMode"></param>
        /// <param name="calcType"></param>
        /// <returns></returns>
        public int SetStdParams(int wiringMode, int powerMode, int calcType, out string[] FrameAry)
        {
            FrameAry = new string[1];
            return 3;
        }
        /// <summary>
        /// 设置档位
        /// </summary>
        /// <param name="UaRange"></param>
        /// <param name="UbRange"></param>
        /// <param name="UcRange"></param>
        /// <param name="IaRange"></param>
        /// <param name="IbRange"></param>
        /// <param name="IcRange"></param>
        /// <returns></returns>
        public int SetRange(float UaRange, float UbRange, float UcRange, float IaRange, float IbRange, float IcRange, out string[] FrameAry)
        {
            //分两帧发送，
            FrameAry = new string[2];
            CL311_RequestSetUScalePacket rc = new CL311_RequestSetUScalePacket();

            CL311_ReplyOkPacket recv = new CL311_ReplyOkPacket();
            try
            {

                bool bUsend = false;
                rc.SetPara(UaRange, UbRange, UcRange, true);
                rc.IsU = true;
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(_MeterStd, rc, recv))
                    {
                        if (recv.ReciveResult == RecvResult.OK)
                        {
                            bUsend = true;
                        }

                    }
                    else
                    {
                        return 2;
                    }
                }


                //发送电流档位
                rc.IsU = false;
                rc.SetPara(IaRange, IbRange, IcRange, true);
                FrameAry[1] = BytesToString(rc.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(_MeterStd, rc, recv))
                    {
                        if (recv.ReciveResult == RecvResult.OK && bUsend)//两包都发送成功，才算成功。
                        {
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        return 2;
                    }
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
        /// PC控制LCD进入各画面状态
        /// </summary>
        /// <param name="formType"></param>
        /// <returns></returns>
        public int SetDisplayForm(int formType, out string[] FrameAry)
        {
            FrameAry = new string[1];
            CL311_RequestSetStdMeterDisplayMode rc = new CL311_RequestSetStdMeterDisplayMode();
            CL311_ReplyOkPacket recv = new CL311_ReplyOkPacket();

            //画面序号0-----5
            //0：功率测量		
            //1：伏安测量
            //2：相频测量
            //3：谐波测量
            //4：波形测量
            //5：校电能表
            if (formType >= 0 && formType < 6)
            {
                rc.DisplayType = Convert.ToByte(formType);
            }
            else
            {
                return 4;
            }

            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (sendFlag)
                {

                    if (SendPacketWithRetry(_MeterStd, rc, recv))
                    {
                        if (recv.ReciveResult == RecvResult.OK)
                        {
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    }

                    return 2;
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
        /// 退出界面
        /// </summary>
        /// <returns></returns>
        public int ExitForm(out string[] FrameAry)
        {
            CL311_RequestReadDataOnlyCmdCodePacket rc = new CL311_RequestReadDataOnlyCmdCodePacket(0x63);

            CL311_ReplyOkPacket recv = new CL311_ReplyOkPacket();
            FrameAry = new string[1];
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(_MeterStd, rc, recv))
                    {
                        if (recv.ReciveResult == RecvResult.OK)
                        {
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    }

                    return 2;
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
        /// 设置校验常数和校验圈数
        /// </summary>
        /// <param name="pulseNum"></param>
        /// <param name="testConst"></param>
        /// <returns></returns>
        public int SetCalcParams(int pulseNum, int testConst, out string[] FrameAry)
        {
            FrameAry = new string[1];
            CL311_RequestSet8PassawayConstandCirclePacket rc = new CL311_RequestSet8PassawayConstandCirclePacket();
            CL311_ReplyOkPacket recv = new CL311_ReplyOkPacket();
            rc.MeterConst = pulseNum;
            rc.Circle = testConst;

            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(_MeterStd, rc, recv))
                    {
                        if (recv.ReciveResult == RecvResult.OK)
                        {
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }

                    }
                    return 2;
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
        /// 读取8录脉冲数 和 8路误差
        /// </summary>
        /// <param name="pulses"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public int ReadError(out int[] pulses, out float[] errors, out string[] FrameAry)
        {
            pulses = new int[8];
            errors = new float[8];
            CL311_RequestReadVerifyDataPacket rc = new CL311_RequestReadVerifyDataPacket();
            CL311_RequestReadVerifyDataReplayPacket recv = new CL311_RequestReadVerifyDataReplayPacket();
            FrameAry = new string[1];

            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(_MeterStd, rc, recv))
                    {
                        if (recv.ReciveResult == RecvResult.OK)
                        {
                            pulses = recv.Pulses;
                            errors = recv.Errors;
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    return 2;
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
        // 读最后一次误差
        public int ReadLastError(out int num, out float error, out string[] FrameAry)
        {
            num = -1;
            error = -1f;
            FrameAry = new string[1];
            return 3;
        }
        /// <summary>
        /// 启动标准表
        /// </summary>
        /// <returns></returns>
        public int StartStdMeter(out string[] FrameAry)
        {
            CL311_RequsetStartStdMeterPacket rc = new CL311_RequsetStartStdMeterPacket();
            CL311_ReplyOkPacket recv = new CL311_ReplyOkPacket();

            FrameAry = new string[1];

            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(_MeterStd, rc, recv))
                    {
                        if (recv.ReciveResult == RecvResult.OK)
                        {
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    }

                    return 2;
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
        /// 设置电流量限
        /// </summary>
        /// <param name="currentType"></param>
        /// <returns></returns>
        public int SetCurrentMeasure(int currentType, out string[] FrameAry)
        {
            CL311_RequestReadCurrentMeasurePacket rc = new CL311_RequestReadCurrentMeasurePacket();

            CL311_ReplyOkPacket recv = new CL311_ReplyOkPacket();

            FrameAry = new string[1];

            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(_MeterStd, rc, recv))
                    {
                        if (recv.ReciveResult == RecvResult.OK)
                        {
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    }

                    return 2;
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
        /// 设置标准表参数
        /// </summary>
        /// <param name="meterConst">表常数</param>
        /// <param name="circle">圈数</param>
        /// <param name="currentMeasure">表量限</param>
        /// <param name="wiringMode">接线方式</param>
        /// <returns></returns>
        public int SetStdParms(int meterConst, int circle, int currentType, int wiringMode, out string[] FrameAry)
        {
            FrameAry = new string[1];
            CL311_RequestSetMeterParaPacket rc = new CL311_RequestSetMeterParaPacket();

            CL311_ReplyOkPacket recv = new CL311_ReplyOkPacket();
            //表量限

            //0	25A
            //1	100A


            //接线方式
            //0	PT4    三相四线有功电能表
            //1	QT4    三相四线真无功电能表
            //2	P32     三相三线有功电能表
            //3	Q32    三相三线真无功电能表
            //4	Q90    二元件90°无功电能表
            //5	Q60    二元件60°无功电能表
            //6	Q33    三元件无功电能表


            rc.SetPara(meterConst, circle, Convert.ToByte(currentType), Convert.ToByte(wiringMode));

            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(_MeterStd, rc, recv))
                    {
                        if (recv.ReciveResult == RecvResult.OK)
                        {
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    }

                    return 2;
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

        #endregion

        #region IClass_Interface 成员
        //设置标志位
        public int SetSendFlag(bool Flag)
        {
            this.sendFlag = Flag;
            return 0;
        }
        //解析下行报文
        public int UnPacket(string MothedName, byte[] ReFrameAry, out string[] ReAry)
        {
            MothedName = MothedName.Replace(" ", "");
            ReAry = new string[1];
            int ReValue = 3;
            switch (MothedName)
            {
                case "Connect":
                    {
                        // 连机
                        // int Connect(out string[] FrameAry);
                        try
                        {
                            Cl311_RequestLinkReplyPacket recv = new Cl311_RequestLinkReplyPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            return 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    //break;
                case "ReadInstMetricAll":
                    {
                        // 读实时测量数据 int ReadInstMetricAll(out float[] instValue, out string[] FrameAry);
                        try
                        {
                            CL311_RequestReadStdInfoReplayPacket recv = new CL311_RequestReadStdInfoReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            stStdInfo stdInfo = recv.PowerInfo;
                            ReAry = new string[34];
                            //电压
                            ReAry[0] = stdInfo.Ua.ToString();
                            ReAry[1] = stdInfo.Ub.ToString();
                            ReAry[2] = stdInfo.Uc.ToString();
                            //电流
                            ReAry[3] = stdInfo.Ia.ToString();
                            ReAry[4] = stdInfo.Ib.ToString();
                            ReAry[5] = stdInfo.Ic.ToString();
                            //电压相位
                            ReAry[6] = stdInfo.Phi_Ua.ToString();
                            ReAry[7] = stdInfo.Phi_Ub.ToString();
                            ReAry[8] = stdInfo.Phi_Uc.ToString();
                            //电流相位
                            ReAry[9] = stdInfo.Phi_Ia.ToString();
                            ReAry[10] = stdInfo.Phi_Ib.ToString();
                            ReAry[11] = stdInfo.Phi_Ic.ToString();
                            //相角
                            ReAry[12] = "-1";// stdInfo.PhiAngle_A;
                            ReAry[13] = "-1";// stdInfo.PhiAngle_B;
                            ReAry[14] = "-1";// stdInfo.PhiAngle_C;
                            //功率相角
                            ReAry[15] = "-1"; //stdInfo.SAngle;
                            //有功功率
                            ReAry[16] = stdInfo.Pa.ToString();
                            ReAry[17] = stdInfo.Pb.ToString();
                            ReAry[18] = stdInfo.Pc.ToString();
                            ReAry[19] = stdInfo.P.ToString();
                            //无功功率
                            ReAry[20] = stdInfo.Qa.ToString();
                            ReAry[21] = stdInfo.Qb.ToString();
                            ReAry[22] = stdInfo.Qc.ToString();
                            ReAry[23] = stdInfo.Q.ToString();
                            //视在功率
                            ReAry[24] = stdInfo.Sa.ToString();
                            ReAry[25] = stdInfo.Sb.ToString();
                            ReAry[26] = stdInfo.Sc.ToString();
                            ReAry[27] = stdInfo.S.ToString();
                            //功率因数
                            ReAry[28] = "-1";//stdInfo.PowerFactor_A;
                            ReAry[29] = "-1";//stdInfo.PowerFactor_B;
                            ReAry[30] = "-1";//stdInfo.PowerFactor_C;
                            //总有功功率因数
                            ReAry[31] = stdInfo.COS.ToString();
                            //总无功功率因数
                            ReAry[32] = stdInfo.SIN.ToString();
                            //频率
                            ReAry[33] = stdInfo.Freq.ToString();

                            return 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    //break;
                case "ReadStdPulseConst":
                    {
                        //读标准表常数int ReadStdPulseConst(out int pulseConst, out string[] FrameAry);
                        try
                        {
                            Cl311_RequestReadStdMeterConstOrPulseReplayPacket recv = new Cl311_RequestReadStdMeterConstOrPulseReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            if (recv.ReciveResult == RecvResult.OK)
                            {
                                ReAry[0] = recv.Data.ToString();
                                return 0;
                            }
                            else
                            {
                                return 1;
                            }
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    //break;
                case "ReadEnergy":
                    {
                        //读取电能量int ReadEnergy(out float energy, out string[] FrameAry);

                        return 3;
                    }
                    //break;
                case "ReadTotalPulses":
                    {
                        //读电能量累计脉冲数 int ReadTotalPulses(out long pulses, out string[] FrameAry);

                        try
                        {
                            Cl311_RequestReadStdMeterConstOrPulseReplayPacket recv = new Cl311_RequestReadStdMeterConstOrPulseReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            if (recv.ReciveResult == RecvResult.OK)
                            {
                                ReAry[0] = recv.Data.ToString();
                                return 0;
                            }
                            else
                            {
                                return 1;
                            }
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    //break;
                case "ReadTestEnergy":
                    {
                        //读电能走字数据int ReadTestEnergy(out float testEnergy, out string[] FrameAry);
                        return 3;
                    }
                    //break;
                case "ReadVersion":
                    {
                        //读仪器版本号 int ReadVersion(out string version, out string[] FrameAry);
                        try
                        {
                            CL311_RequestReadVersionReplayPacket recv = new CL311_RequestReadVersionReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            if (recv.ReciveResult == RecvResult.OK)
                            {
                                ReAry[0] = recv.Version;
                                return 0;
                            }
                            else
                            {
                                return 1;
                            }

                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    //break;
                case "ReadHarmonicArry":
                    {
                        //读各项电压电流谐波幅值int ReadHarmonicArry(int phase, out float[] harmonicArry, out string[] FrameAry);

                        try
                        {
                            CL311_RequestReadHarmonicDataReplayPacket recv = new CL311_RequestReadHarmonicDataReplayPacket();

                            recv.ParsePacket(ReFrameAry);
                            if (recv.ReciveResult == RecvResult.OK)
                            {
                                ReAry[0] = recv.Data.ToString();
                                return 0;
                            }
                            else
                            {
                                return 1;
                            }
                        }
                        catch (Exception)
                        {

                            return -1;
                        }

                    }
                    //break;
                case "SetWiringMode":
                    {
                        //设置接线方式int SetWiringMode(int wiringMode, out string[] FrameAry);
                        try
                        {
                            CL311_ReplyOkPacket recv = new CL311_ReplyOkPacket();

                            recv.ParsePacket(ReFrameAry);

                            ReAry[0] = recv.ReciveResult.ToString();
                            return 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    //break;
                case "SetStdPulseConst":
                    {
                        //设置标准表常数 int SetStdPulseConst(int pulseConst, out string[] FrameAry);
                        try
                        {
                            CL311_ReplyOkPacket recv = new CL311_ReplyOkPacket();
                            recv.ParsePacket(ReFrameAry);

                            ReAry[0] = recv.ReciveResult.ToString();
                            return 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    //break;
                case "SetPowerMode":
                    {
                        //设置电能指示 int SetPowerMode(int powerMode, out string[] FrameAry);
                        return 3;
                    }
                    //break;
                case "SetErrCalcType":
                    {
                        //设置电能误差计算启动开关 int SetErrCalcType(int calcType, out string[] FrameAry);
                        return 3;
                    }
                    //break;
                case "SetStdParams":
                    {
                        //设置电能参数 int SetStdParams(int wiringMode, int powerMode, int calcType, out string[] FrameAry);
                        return 3;
                    }
                    //break;
                case "SetRange":
                    {
                        //设置档位int SetRange(float UaRange, float UbRange, float UcRange, float IaRange, float IbRange, float IcRange, out string[] FrameAry);
                        try
                        {
                            CL311_ReplyOkPacket recv = new CL311_ReplyOkPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();

                            return 0;

                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    //break;
                case "SetDisplayForm":
                    {
                        //设置显示界面        int SetDisplayForm(int formType, out string[] FrameAry);
                        try
                        {
                            CL311_ReplyOkPacket recv = new CL311_ReplyOkPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            return 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    //break;
                case "ExitForm":
                    {
                        //退出显示界面
                        try
                        {
                            CL311_ReplyOkPacket recv = new CL311_ReplyOkPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            return 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    //break;
                case "SetCalcParams":
                    {
                        //设置电能误差检定参数        int SetCalcParams(int pulseNum, int testConst, out string[] FrameAry);
                        try
                        {
                            CL311_ReplyOkPacket recv = new CL311_ReplyOkPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            return 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    //break;
                case "ReadError":
                    {
                        //读取误差和脉冲数int ReadError(out int[] pulses, out float[] errors, out string[] FrameAry);
                        try
                        {
                            CL311_RequestReadVerifyDataReplayPacket recv = new CL311_RequestReadVerifyDataReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            if (recv.ReciveResult == RecvResult.OK)
                            {
                                ReAry = new string[2];
                                ReAry[0] = recv.Pulses.ToString();
                                ReAry[1] = recv.Errors.ToString();
                                return 0;

                            }
                            else
                            {
                                return 1;
                            }

                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    //break;
                case "ReadLastError":
                    {
                        //读取最近一次误差及误差次数 int ReadLastError(out int num, out float error, out string[] FrameAry);
                        return 3;
                    }
                    //break;
                case "StartStdMeter":
                    {
                        //启动标准表        int StartStdMeter(out string[] FrameAry);
                        try
                        {
                            CL311_ReplyOkPacket recv = new CL311_ReplyOkPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            return 0;

                        }
                        catch (Exception)
                        {

                            return -1;
                        }

                    }
                    //break;
                case "SetCurrentMeasure":
                    {
                        //控制电流量限 int SetCurrentMeasure(int currentType, out string[] FrameAry);
                        try
                        {
                            CL311_ReplyOkPacket recv = new CL311_ReplyOkPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            return 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    //break;
                case "SetStdParms":
                    {
                        //设置标准表参数int SetStdParms(int meterConst, int circle, int currentType, int wiringMode, out string[] FrameAry);
                        try
                        {
                            CL311_ReplyOkPacket recv = new CL311_ReplyOkPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            return 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    //break;
                default:
                    break;
            }

            return ReValue;

        }

        #endregion

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="stPort">端口号</param>
        /// <param name="sp">发送包</param>
        /// <param name="rp">接收包</param>
        /// <returns></returns>
        private bool SendPacketWithRetry(StPortInfo stPort, SendPacket sp, RecvPacket rp)
        {
            for (int i = 0; i < RETRYTIEMS; i++)
            {
                if (driverBase.SendData(stPort, sp, rp) == true)
                {
                    return true;
                }
                Thread.Sleep(300);
            }
            return false;
        }


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





    }
}
