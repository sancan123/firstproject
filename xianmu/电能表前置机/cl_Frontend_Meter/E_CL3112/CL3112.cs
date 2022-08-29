using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using E_CLSocketModule.Enum;
using E_CLSocketModule.Struct;
using E_CLSocketModule;
using E_CLSocketModule.SocketModule.Packet;
using E_CL3112.Device;

namespace E_CL3112
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
        /// <param name="FrameAry">输出连机报文</param>
        /// <returns></returns>
        [DispId(3)]
        int Connect(out string[] FrameAry);
        /// <summary>
        /// 断开连机
        /// </summary>
        /// <param name="FrameAry">输出断开连机报文</param>
        /// <returns></returns>
        [DispId(4)]
        int DisConnect(out string[] FrameAry);
        /// <summary>
        /// 读实时测量数据
        /// </summary>
        /// <param name="instValue">输出测量数据</param>
        /// <param name="FrameAry">读实时测量数据报文</param>
        /// <returns></returns>
        [DispId(5)]
        int ReadInstMetricAll(out float[] instValue, out string[] FrameAry);
        /// <summary>
        /// 读标准表常数
        /// </summary>
        /// <param name="pulseConst"></param>
        /// <param name="FrameAry">读标准表常数</param>
        /// <returns></returns>
        [DispId(6)]
        int ReadStdPulseConst(out int pulseConst, out string[] FrameAry);
        /// <summary>
        /// 读取电能量
        /// </summary>
        /// <param name="energy">返回电能量</param>
        /// <param name="FrameAry">读取电能量输出报文</param>
        /// <returns></returns>
        [DispId(7)]
        int ReadEnergy(out float energy, out string[] FrameAry);
        /// <summary>
        /// 读电能量累计脉冲数
        /// </summary>
        /// <param name="pulses">读电能量累计脉冲数</param>
        /// <param name="FrameAry">读电能量累计脉冲数输出报文</param>
        /// <returns></returns>
        [DispId(8)]
        int ReadTotalPulses(out long pulses, out string[] FrameAry);
        /// <summary>
        /// 读电能走字数据
        /// </summary>
        /// <param name="testEnergy">返回走字电能量</param>
        /// <param name="FrameAry">读电能走字数据输出报文</param>
        /// <returns></returns>
        [DispId(9)]
        int ReadTestEnergy(out float testEnergy, out string[] FrameAry);
        /// <summary>
        /// 读仪器版本号
        /// </summary>
        /// <param name="version">版本号</param>
        /// <param name="FrameAry">读仪器版本号输出报文</param>
        /// <returns></returns>
        [DispId(10)]
        int ReadVersion(out string version, out string[] FrameAry);
        /// <summary>
        /// 读各项电压电流谐波幅值
        /// </summary>
        /// <param name="phase"></param>
        /// <param name="harmonicArry">65个长度 </param>
        /// <param name="FrameAry">读各项电压电流谐波幅值输出报文</param>
        /// <returns></returns>
        [DispId(11)]
        int ReadHarmonicArry(int phase, out float[] harmonicArry, out string[] FrameAry);
        /// <summary>
        /// 读各项电压电流波形数据
        /// </summary>
        /// <param name="phase"></param>
        /// <param name="waveformArry"> 256个长度</param>
        /// <param name="FrameAry">读各项电压电流波形数据输出报文</param>
        /// <returns></returns>
        [DispId(12)]
        int ReadWaveformArry(int phase, out float[] waveformArry, out string[] FrameAry);
        /// <summary>
        /// 设置接线方式
        /// </summary>
        /// <param name="wiringMode">接线类型</param>
        /// <param name="FrameAry">设置接线方式输出报文</param>
        /// <returns></returns>
        [DispId(13)]
        int SetWiringMode(int wiringMode, out string[] FrameAry);
        /// <summary>
        /// 设置标准表常数
        /// </summary>
        /// <param name="pulseConst">标准表常数</param>
        /// <param name="FrameAry">设置标准表常数输出报文</param>
        /// <returns></returns>
        [DispId(14)]
        int SetStdPulseConst(int pulseConst, out string[] FrameAry);
        /// <summary>
        /// 设置电能指示
        /// </summary>
        /// <param name="powerMode">1：总有功电能 2：总无功电能</param>
        /// <param name="FrameAry">设置电能指示输出报文</param>
        /// <returns></returns>
        [DispId(15)]
        int SetPowerMode(int powerMode, out string[] FrameAry);
        /// <summary>
        /// 设置电能误差计算启动开关
        /// </summary>
        /// <param name="calcType">0 停止计算  </param>  
        ///                        1 开始计算电能误差  
        ///                        2 开始计算电能走字
        /// <param name="FrameAry">设置电能误差计算启动开关输出报文</param>
        /// <returns></returns>
        [DispId(16)]
        int SetErrCalcType(int calcType, out string[] FrameAry);
        /// <summary>
        /// 设置电能参数
        /// </summary>
        /// <param name="wiringMode">接线方式</param>
        /// <param name="powerMode">电能方式</param>
        /// <param name="calcType">电能误差计算开关</param>
        /// <param name="FrameAry">设置电能参数输出报文</param>
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
        /// <param name="FrameAry">设置档位输出报文</param>
        /// <returns></returns>
        [DispId(18)]
        int SetRange(int UaRange, int UbRange, int UcRange, int IaRange, int IbRange, int IcRange, out string[] FrameAry);
        /// <summary>
        /// 设置显示界面
        /// </summary>
        /// <param name="formType">界面类型</param>
        /// <param name="FrameAry">设置显示界面输出报文</param>
        /// <returns></returns>
        [DispId(19)]
        int SetDisplayForm(int formType, out string[] FrameAry);
        /// <summary>
        /// 设置电能误差检定参数
        /// </summary>
        /// <param name="pulseNum"></param>
        /// <param name="testConst"></param>
        /// <param name="FrameAry">设置电能误差检定参数输出报文</param>
        /// <returns></returns>
        [DispId(20)]
        int SetCalcParams(int pulseNum, int testConst, out string[] FrameAry);
        /// <summary>
        /// 读取电能误差
        /// </summary>
        /// <param name="error"></param>
        /// <param name="FrameAry">读取电能误差输出报文</param>
        /// <returns></returns>
        [DispId(21)]
        int ReadError(out float error, out string[] FrameAry);
        /// <summary>
        /// 读取最近一次误差及误差次数
        /// </summary>
        /// <param name="num"></param>
        /// <param name="error"></param>
        /// <param name="FrameAry">读取最近一次误差及误差次数输出报文</param>
        /// <returns></returns>
        [DispId(22)]
        int ReadLastError(out int num, out float error, out string[] FrameAry);
        /// <summary>
        /// 设置获取请求报文标志
        /// </summary>
        /// <param name="Flag">True:发送报文,并传出报文,false:不发送,只传出报文</param>
        /// <returns></returns>
        [DispId(23)]
        int SetSendFlag(bool Flag);
        /// <summary>
        /// 解析下行报文
        /// </summary>
        /// <param name="MothedName">函数名(有出参FrameAry的函数的名称)</param>
        /// <param name="ReFrameAry">下行报文</param>
        /// <param name="ReAry">解析后的数据</param>
        /// <returns></returns>
        [DispId(24)]
        int UnPacket(string MothedName, byte[] ReFrameAry, out string[] ReAry);
    }

    [Guid("6922994B-4D1A-4375-8699-61C042B5FE13"),
    InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
    ComVisible(true)]
    public interface IClass_Events
    {
        [DispId(80)]
        void MsgCallBack(string szMessage);
    }

    [Guid("D09C2799-FEC8-4e35-BB2E-A59255875188"),
    ProgId("CLOU.CL3112"),
    ClassInterface(ClassInterfaceType.None),
    ComDefaultInterface(typeof(IClass_Interface)),
    ComSourceInterfaces(typeof(IClass_Events)),
    ComVisible(true)]
    public class CL3112 : IClass_Interface
    {
        public delegate void MsgCallBackDelegate(string szMessage);
        //public event MsgCallBackDelegate MsgCallBack;

        /// <summary>
        /// 重试次数
        /// </summary>
        public static int RETRYTIEMS = 1;
        /// <summary>
        /// 控制端口
        /// </summary>
        private StPortInfo m_MeterStd = null;

        /// <summary>
        /// 
        /// </summary>
        private DriverBase driverBase = null;
        /// <summary>
        /// 是否发送报文标志位
        /// </summary>
        private bool sendFlag = true;

        public CL3112()
        {
            m_MeterStd = new StPortInfo();
            driverBase = new DriverBase();

        }
        #region IClass_Interface 成员
        /// <summary>
        /// 初始化2018端口
        /// </summary>
        /// <param name="ComNumber"></param>
        /// <param name="MaxWaitTime"></param>
        /// <param name="WaitSencondsPerByte"></param>
        /// <param name="IP"></param>
        /// <param name="RemotePort"></param>
        /// <param name="LocalStartPort"></param>
        /// <returns></returns>
        public int InitSetting(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte, string IP, int RemotePort, int LocalStartPort, string HaveProtocol)
        {
            m_MeterStd.m_Exist = 1;
            m_MeterStd.m_IP = IP;
            m_MeterStd.m_Port = ComNumber;
            m_MeterStd.m_Port_Type = Cus_EmComType.UDP;
            m_MeterStd.m_Port_Setting = "57600,n,8,1";
            try
            {
                driverBase.RegisterPort(ComNumber, m_MeterStd.m_Port_Setting, m_MeterStd.m_IP, RemotePort, LocalStartPort, HaveProtocol, MaxWaitTime, WaitSencondsPerByte);
            }
            catch (Exception)
            {

                return 1;
            }
            return 0;
        }
        /// <summary>
        /// 初始化COM口
        /// </summary>
        /// <param name="ComNumber">端口号</param>
        /// <param name="MaxWaitTime">最长等待回复时间</param>
        /// <param name="WaitSencondsPerByte">单帧字节等待时间</param>
        /// <returns></returns>
        public int InitSettingCom(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte)
        {
            m_MeterStd.m_Exist = 1;
            m_MeterStd.m_IP = "";
            m_MeterStd.m_Port = ComNumber;
            m_MeterStd.m_Port_Type = Cus_EmComType.COM;
            m_MeterStd.m_Port_Setting = "57600,n,8,1";
            try
            {
                driverBase.RegisterPort(ComNumber, "57600,n,8,1", MaxWaitTime, WaitSencondsPerByte);
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
        /// <returns></returns>
        public int Connect(out string[] FrameAry)
        {
            CL3112RequestReadStdInfoPacket rc = new CL3112RequestReadStdInfoPacket();
            CL3112RequestReadStdInfoReplayPacket recv = new CL3112RequestReadStdInfoReplayPacket();
            FrameAry = new string[1];
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(m_MeterStd, rc, recv))
                    {
                        bool linkOk = recv.ReciveResult == RecvResult.OK;
                        return linkOk ? 0 : 1;
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
            CL3112RequestReadStdInfoPacket rc = new CL3112RequestReadStdInfoPacket();
            CL3112RequestReadStdInfoReplayPacket recv = new CL3112RequestReadStdInfoReplayPacket();
            FrameAry = new string[1];
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(m_MeterStd, rc, recv))
                    {
                        bool linkOk = recv.ReciveResult == RecvResult.OK;
                        return linkOk ? 0 : 1;
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
        /// 读取瞬时测量数据
        /// </summary>
        /// <param name="instValue"></param>
        /// <returns></returns>
        public int ReadInstMetricAll(out float[] instValue, out string[] FrameAry)
        {
            CL3112RequestReadStdInfoPacket rc = new CL3112RequestReadStdInfoPacket();
            CL3112RequestReadStdInfoReplayPacket rcback = new CL3112RequestReadStdInfoReplayPacket();
            instValue = new float[34];
            FrameAry = new string[1];
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (sendFlag)
                {
                    if (SendPacketWithRetry(m_MeterStd, rc, rcback))
                    {

                        stStdInfo stdInfo = rcback.PowerInfo;
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
                        instValue[12] = stdInfo.PhiAngle_A;
                        instValue[13] = stdInfo.PhiAngle_B;
                        instValue[14] = stdInfo.PhiAngle_C;
                        //功率相角
                        instValue[15] = stdInfo.SAngle;
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
                        instValue[28] = stdInfo.PowerFactor_A;
                        instValue[29] = stdInfo.PowerFactor_B;
                        instValue[30] = stdInfo.PowerFactor_C;
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
                        return 1;
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
            
            pulseConst = 0;
            FrameAry = new string[1];

            return 0;


        }
        /// <summary>
        /// 读取标准表电能
        /// </summary>
        /// <param name="energy">标准表电能</param>
        /// <returns></returns>
        public int ReadEnergy(out float energy, out string[] FrameAry)
        {
            
            energy = 0.0f;
            FrameAry = new string[1];


            return 0;


        }
        /// <summary>
        /// 读取电能累计脉冲数
        /// </summary>
        /// <param name="pulses"></param>
        /// <returns></returns>
        public int ReadTotalPulses(out long pulses, out string[] FrameAry)
        {
            
            pulses = 0;
            FrameAry = new string[1];
            return 0;

        }
        /// <summary>
        /// 读取电能走字数据
        /// </summary>
        /// <param name="testEnergy"></param>
        /// <returns></returns>
        public int ReadTestEnergy(out float testEnergy, out string[] FrameAry)
        {
            
            testEnergy = 0.0F;
            FrameAry = new string[1];
            return 0;


        }
        /// <summary>
        /// 读取版本号
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public int ReadVersion(out string version, out string[] FrameAry)
        {
            version = "null";
            FrameAry = new string[1];
            return 3;
        }
        /// <summary>
        /// 读取各相电压电流谐波幅值（分两帧读取数据）
        /// </summary>
        /// <param name="phase">相别</param>
        /// <param name="harmonicArry"></param>
        /// <returns></returns>
        public int ReadHarmonicArry(int phase, out float[] harmonicArry, out string[] FrameAry)
        {
            harmonicArry = new float[65];
            
            FrameAry = new string[2];

            return 0;


        }
        /// <summary>
        /// 读取各项电压电流波形数据（分三帧读取数据）
        /// </summary>
        /// <param name="phase">相别</param>
        /// <param name="waveformArry">256个点的数据</param>
        /// <returns></returns>
        public int ReadWaveformArry(int phase, out float[] waveformArry, out string[] FrameAry)
        {            
            waveformArry = new float[256];
            //int iCountNum = 256;
            //int iOneFrame = 100;
            //ushort iStart = 0;
            FrameAry = new string[4];

            return 0;
        }
        /// <summary>
        /// 设置接线方式
        /// </summary>
        /// <param name="wiringMode"></param>
        /// <returns></returns>
        public int SetWiringMode(int wiringMode, out string[] FrameAry)
        {
            FrameAry = new string[1];
            return 0;

        }
        /// <summary>
        /// 设置标准表常数
        /// </summary>
        /// <param name="pulseConst">标准表常数</param>
        /// <returns></returns>
        public int SetStdPulseConst(int pulseConst, out string[] FrameAry)
        {
            FrameAry = new string[1];
            return 0;
        }
        /// <summary>
        /// 设置电能指示
        /// </summary>
        /// <param name="powerMode">1:总有功|2：总无功3</param>
        /// <returns></returns>
        public int SetPowerMode(int powerMode, out string[] FrameAry)
        {            
            FrameAry = new string[1];

            return 0;

        }
        /// <summary>
        /// 设置电能计算误差启动开关
        /// </summary>
        /// <param name="calcType"> 0，停止；1，开始计算电能误差；2，开始计算电能走字</param>
        /// <returns></returns>
        public int SetErrCalcType(int calcType, out string[] FrameAry)
        {            
            
            FrameAry = new string[1];
            return 0;
        }
        /// <summary>
        /// 设置标准表参数
        /// </summary>
        /// <param name="wiringMode">接线方式</param>
        /// <param name="powerMode">供电模式</param>
        /// <param name="calcType">计算误差开关</param>
        /// <returns></returns>
        public int SetStdParams(int wiringMode, int powerMode, int calcType, out string[] FrameAry)
        {            
            FrameAry = new string[1];
            return 0;

        }
        /// <summary>
        /// 设置档位
        /// </summary>
        /// <param name="UaRange">UA档位</param>
        /// <param name="UbRange">UB档位</param>
        /// <param name="UcRange">UC档位</param>
        /// <param name="IaRange">IA档位</param>
        /// <param name="IbRange">IB档位</param>
        /// <param name="IcRange">IC档位</param>
        /// <param name="FrameAry">输出报文</param>
        /// <returns></returns>
        public int SetRange(int UaRange, int UbRange, int UcRange, int IaRange, int IbRange, int IcRange, out string[] FrameAry)
        {
            FrameAry = new string[1];
            return 0;

        }
        /// <summary>
        /// 设置标准表界面 1：谐波柱图界面2：谐波列表界面3：波形界面4：清除设置界面
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="FrameAry">输出报文</param>
        /// <returns></returns>
        public int SetDisplayForm(int formType, out string[] FrameAry)
        {
            
            FrameAry = new string[1];
            return 0;


        }
        /// <summary>
        /// 5.1.16	设置电能误差检定参数（CL1115副表检定控制前）
        /// </summary>
        /// <param name="pulseNum">校验圈数</param>
        /// <param name="testConst">被检表常数</param>
        /// <returns></returns>
        public int SetCalcParams(int pulseNum, int testConst, out string[] FrameAry)
        {
           
            FrameAry = new string[1];
            return 0;


        }
        /// <summary>
        /// 读取电能误差（仅CL1115主副表版本）
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public int ReadError(out float error, out string[] FrameAry)
        {
            
            error = -1f;
            FrameAry = new string[1];
            return 0;



        }
        /// <summary>
        /// 读取最近一次电能误差及误差计算次数
        /// </summary>
        /// <param name="num"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public int ReadLastError(out int num, out float error, out string[] FrameAry)
        {
            
            num = -1;
            error = -1f;
            FrameAry = new string[1];
            return 0;


        }

        #endregion

        #region IClass_Interface 成员


        public int SetSendFlag(bool Flag)
        {
            this.sendFlag = Flag;
            return 0;
        }
        /// <summary>
        /// 解析下行报文
        /// </summary>
        /// <param name="MothedName">对应方法名称</param>
        /// <param name="ReFrameAry">下行报文</param>
        /// <param name="ReAry">解析相应的数据</param>
        /// <returns></returns>
        public int UnPacket(string MothedName, byte[] ReFrameAry, out string[] ReAry)
        {
            MothedName = MothedName.Replace(" ", "").ToUpper();
            int iRsValue = 0;
            ReAry = new string[1];
            switch (MothedName)
            {
                case "CONNECT":
                    {
                        //连机
                        try
                        {
                            CL3112RequestReadStdInfoReplayPacket recv = new CL3112RequestReadStdInfoReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                        }
                        catch (Exception)
                        {

                            return -1;
                        }

                    }
                    break;
                case "DISCONNECT":
                    {
                        //断开连机 int DisConnect(out string[] FrameAry);
                        try
                        {
                            CL3112RequestReadStdInfoReplayPacket recv = new CL3112RequestReadStdInfoReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                        }
                        catch (Exception)
                        {
                            return -1;
                        }

                    }
                    break;
                case "READINSTMETRICALL":
                    {
                        //读实时测量数据 int ReadInstMetricAll(out float[] instValue, out string[] FrameAry);
                        try
                        {
                            CL3112RequestReadStdInfoReplayPacket rcback = new CL3112RequestReadStdInfoReplayPacket();
                            rcback.ParsePacket(ReFrameAry);
                            if (rcback.ReciveResult == RecvResult.OK)
                            {
                                stStdInfo stdInfo = rcback.PowerInfo;
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
                                ReAry[12] = stdInfo.PhiAngle_A.ToString();
                                ReAry[13] = stdInfo.PhiAngle_B.ToString();
                                ReAry[14] = stdInfo.PhiAngle_C.ToString();
                                //功率相角
                                ReAry[15] = stdInfo.SAngle.ToString();
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
                                ReAry[28] = stdInfo.PowerFactor_A.ToString();
                                ReAry[29] = stdInfo.PowerFactor_B.ToString();
                                ReAry[30] = stdInfo.PowerFactor_C.ToString();
                                //总有功功率因数
                                ReAry[31] = stdInfo.COS.ToString();
                                //总无功功率因数
                                ReAry[32] = stdInfo.SIN.ToString();
                                //频率
                                ReAry[33] = stdInfo.Freq.ToString();
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
                    break;
                case "READSTDPULSECONST":
                    {
                        //读标准表常数

                    }
                    break;
                case "READENERGY":
                    {
                        //读取电能量
                    }
                    break;
                case "READTOTALPULSES":
                    {
                        //读电能量累计脉冲数
                    }
                    break;
                case "READTESTENERGY":
                    {
                        //读电能走字数据
                    }
                    break;
                case "READVERSION":
                    {
                        //读仪器版本号 int ReadVersion(out string version, out string[] FrameAry);
                        return 3;
                    }
                    //break;
                case "READHARMONICARRY":
                    {
                        //读各项电压电流谐波幅值
                    }
                    break;
                case "READWAVEFORMARRY":
                    {
                        //读各项电压电流波形数据
                    }
                    break;
                case "SETWIRINGMODE":
                    {
                        //设置接线方式
                    }
                    break;
                case "SETSTDPULSECONST":
                    {
                        //设置标准表常数
                    }
                    break;
                case "SETPOWERMODE":
                    {
                        //设置电能指示

                    }
                    break;
                case "SETERRCALCTYPE":
                    {
                        //设置电能误差计算启动开关 
                    }
                    break;
                case "SETSTDPARAMS":
                    {
                        //设置电能参数
                    }
                    break;
                case "SETRANGE":
                    {
                        //设置档位

                    }
                    break;
                case "SETDISPLAYFORM":
                    {
                        //设置显示界面
                    }
                    break;
                case "SETCALCPARAMS":
                    {
                        //设置电能误差检定参数
                    }
                    break;
                case "READERROR":
                    {
                        //读取电能误差
                    }
                    break;
                case "READLASTERROR":
                    {
                        //读取最近一次误差及误差次数

                    }
                    break;
                default:
                    break;

            }

            return iRsValue;

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
