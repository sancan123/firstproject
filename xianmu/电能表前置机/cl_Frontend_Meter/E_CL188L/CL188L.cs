using E_CL188L.Device;
using E_CLSocketModule;
using E_CLSocketModule.Enum;
using E_CLSocketModule.SocketModule.Packet;
using E_CLSocketModule.Struct;
using System;
using System.Runtime.InteropServices;

namespace E_CL188L
{
    [Guid("73DCFB98-6D31-40D1-AFF2-E8DC3C83CF12"),
    ProgId("CLOU.CL188L"),
    ClassInterface(ClassInterfaceType.None),
    ComVisible(true)]
    public class CL188L
    {
        public int int_Id = 0;

        public bool[] SelectStatus { get; set; }

        /// <summary>
        /// 当前通道号
        /// </summary>
        public int ChannelNo { get; set; }

        /// <summary>
        /// 通道数
        /// </summary>
        public int ChannelNum { get; set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        public static int RETRYTIEMS = 1;
        /// <summary>
        /// 源控制端口
        /// </summary>
        private StPortInfo _Port = null;
        readonly DriverBase driverBase = null;
        /// <summary>
        /// 发送标志
        /// </summary>
        private bool SendFlag = true;

        public CL188L()
        {
            _Port = new StPortInfo();
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
            _Port = new StPortInfo
            {
                m_Exist = 1,
                m_IP = IP,
                m_Port = ComNumber,
                m_Port_Type = Cus_EmComType.UDP,
                m_Port_Setting = "38400,n,8,1"
            };
            try
            {
                driverBase.RegisterPort(ComNumber, _Port.m_Port_Setting, _Port.m_IP, RemotePort, LocalStartPort, HaveProtocol, MaxWaitTime, WaitSencondsPerByte);
            }
            catch (Exception)
            {

                return -1;
            }
            return 0;
        }
        /// <summary>
        /// 初始化COM口
        /// </summary>
        /// <param name="ComNumber"></param>
        /// <param name="MaxWaitTime"></param>
        /// <param name="WaitSencondsPerByte"></param>
        /// <returns></returns>
        public int InitSettingCom(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte)
        {
            _Port.m_Exist = 1;
            _Port.m_IP = "";
            _Port.m_Port = ComNumber;
            _Port.m_Port_Type = Cus_EmComType.COM;
            _Port.m_Port_Setting = "38400,n,8,1";
            try
            {
                driverBase.RegisterPort(ComNumber, "38400,n,8,1", MaxWaitTime, WaitSencondsPerByte);
            }
            catch (Exception)
            {
                return -1;
            }

            return 0;
        }
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int Connect(int Id, out string[] FrameAry)
        {
            FrameAry = new string[1];
            CL188L_RequestLinkPacket rc2 = new CL188L_RequestLinkPacket
            {
                ChannelNo = ChannelNo,
                ChannelNum = ChannelNum,
                BwStatus = SelectStatus
            };
            CL188L_RequestLinkReplayPacket recv2 = new CL188L_RequestLinkReplayPacket();
            try
            {
                rc2.Pos = Id;
                FrameAry[0] = BytesToString(rc2.GetPacketData());
                int intRst = 1;
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc2, recv2))
                    {
                        intRst = recv2.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }
                return intRst;
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
            FrameAry[0] = "null";
            return 0;
        }
        /// <summary>
        /// 设置标志位
        /// </summary>
        /// <param name="Flag"></param>
        /// <returns></returns>
        public int SetSendFlag(bool Flag)
        {
            SendFlag = Flag;
            return SendFlag == Flag ? 0 : 1;
        }

        /// <summary>
        /// 设置脉冲通道和类型
        /// </summary>
        /// <param name="Id">误差板编号</param>
        /// <param name="pram1">电能误差通道号0正有|1反有|2正无|3反无|4需量|5时钟</param>
        /// <param name="pram2">光电头选择位0脉冲盒|1光电头</param>
        /// <param name="pram3">共因0|共阳1</param>
        /// <param name="pram4">多功能误差通道号0电能|1日计时|2需量</param>
        /// <param name="pram5">检定类型0电能|1需量|2日计时|3脉冲|4对标|5预付费|6耐压|7多功能脉冲计数</param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int SetPulseChannelAndType(int Id, int pram1, int pram2, int pram3, int pram4, int pram5, out string[] FrameAry)
        {
            FrameAry = new string[1];
            CL188L_RequestSelectPulseChannelAndCheckTypePacket rc2 = new CL188L_RequestSelectPulseChannelAndCheckTypePacket();
            CL188L_RequestSelectPulseChannelAndCheckTypeReplayPacket recv2 = new CL188L_RequestSelectPulseChannelAndCheckTypeReplayPacket();
            try
            {
                rc2.Pos = Id;
                rc2.ChannelNo = ChannelNo;
                rc2.ChannelNum = ChannelNum;
                rc2.SetPara((Cus_EmMeterWcChannelNo)pram1, (Cus_EmPulseType)pram2, (Cus_EmGyGyType)pram3, (Cus_EmDgnWcChannelNo)pram4, (Cus_EmCheckType)pram5, ChannelNum);
                rc2.BwStatus = SelectStatus;
                FrameAry[0] = BytesToString(rc2.GetPacketData());
                int intRst = 1;
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc2, recv2))
                    {
                        intRst = recv2.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }
                return intRst;
            }
            catch (Exception)
            {
                return -1;
            }
        }
        /// <summary>
        /// 远程登录
        /// </summary>
        /// <param name="FrameAry">合成报文</param>
        /// <returns></returns>
        public int UpdateLogin(int id, out string[] FrameAry)
        {
            FrameAry = new string[1];
            CL188L_RequestUpdateLoginPacket rc = new CL188L_RequestUpdateLoginPacket();
            CL188L_RequestUpdateLoginReplayPacket recv = new CL188L_RequestUpdateLoginReplayPacket();
            int intRst = 1;
            try
            {
                rc.Pos = id;
                rc.ChannelNo = ChannelNo;
                rc.ChannelNum = ChannelNum;
                rc.BwStatus = SelectStatus;
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }
            return intRst;

        }
        /// <summary>
        /// 重启误差板
        /// </summary>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int ReBoot(int id, out string[] FrameAry)
        {
            FrameAry = new string[1];
            int intRst = 1;
            CL188L_RequestReBootPacket rc = new CL188L_RequestReBootPacket();
            CL188L_RequestReBootReplayPacket recv = new CL188L_RequestReBootReplayPacket();
            rc.Pos = id;
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            rc.BwStatus = SelectStatus;
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());

                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }

            }
            catch (Exception)
            {

                return -1;
            }

            return intRst;
        }
        /// <summary>
        /// 升级数据
        /// </summary>
        /// <param name="DataSerial">数据序号</param>
        /// <param name="bytesData">要升级的数据</param>
        /// <param name="FrameAry">要反会的组帧报文</param>
        /// <returns></returns>
        public int UpdateFirmware(int id, UInt16 DataSerial, byte[] bytesData, out string[] FrameAry)
        {
            FrameAry = new string[1];
            CL188L_RequestUpdateFirmwarePacket rc = new CL188L_RequestUpdateFirmwarePacket(DataSerial, bytesData);
            CL188L_RequestUpdateFirmwareReplayPacket recv = new CL188L_RequestUpdateFirmwareReplayPacket();
            rc.Pos = id;
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            rc.BwStatus = SelectStatus;
            int intRst = 1;
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }
            return intRst;

        }

        /// <summary>
        /// 远程登录2
        /// </summary>
        /// <param name="id"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int UpdateLogin2(int id, out string[] FrameAry)
        {
            FrameAry = new string[1];
            CL188L_RequestUpdateLogin2Packet rc = new CL188L_RequestUpdateLogin2Packet();
            CL188L_RequestUpdateLogin2ReplayPacket recv = new CL188L_RequestUpdateLogin2ReplayPacket();
            int intRst = 1;
            try
            {
                rc.Pos = id;
                rc.ChannelNo = ChannelNo;
                rc.ChannelNum = ChannelNum;
                rc.BwStatus = SelectStatus;
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }
            return intRst;
        }
        /// <summary>
        /// 重启误差板2
        /// </summary>
        /// <param name="id"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int ReBoot2(int id, out string[] FrameAry)
        {
            FrameAry = new string[1];
            int intRst = 1;
            CL188L_RequestReBoot2Packet rc = new CL188L_RequestReBoot2Packet();
            CL188L_RequestReBoot2ReplayPacket recv = new CL188L_RequestReBoot2ReplayPacket();
            rc.Pos = id;
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            rc.BwStatus = SelectStatus;
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());

                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }

            }
            catch (Exception)
            {

                return -1;
            }

            return intRst;
        }
        /// <summary>
        /// 升级数据2
        /// </summary>
        /// <param name="id"></param>
        /// <param name="DataSerial"></param>
        /// <param name="bytesData"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int UpdateFirmware2(int id, ushort DataSerial, byte[] bytesData, out string[] FrameAry)
        {
            FrameAry = new string[1];
            CL188L_RequestUpdateFirm2warePacket rc = new CL188L_RequestUpdateFirm2warePacket(DataSerial, bytesData);
            CL188L_RequestUpdateFirmware2ReplayPacket recv = new CL188L_RequestUpdateFirmware2ReplayPacket();
            rc.Pos = id;
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            rc.BwStatus = SelectStatus;
            int intRst = 1;
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }
            return intRst;
        }
        /// <summary>
        /// 读取版本号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Version"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int ReadVersion2(int id, out string Version, out string[] FrameAry)
        {
            FrameAry = new string[1];
            Version = string.Empty;
            int intRst = 1;

            CL1888M_RequestReadVersion2Packet rc = new CL1888M_RequestReadVersion2Packet();
            CL1888M_RequestReadVersion2ReplayPacket recv = new CL1888M_RequestReadVersion2ReplayPacket();
            rc.Pos = id;
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            rc.BwStatus = SelectStatus;
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                        Version = recv.strVersion;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }

            return intRst;
        }
        /// <summary>
        /// 读取功耗
        /// </summary>
        /// <param name="id"></param>
        /// <param name="AU_Ia_or_I"></param>
        /// <param name="BU_Ib_or_L1_U"></param>
        /// <param name="CU_Ic_or_L2_U"></param>
        /// <param name="AI_Ua"></param>
        /// <param name="BI_Ub"></param>
        /// <param name="CI_Uc"></param>
        /// <param name="AU_Phia_or_Phi"></param>
        /// <param name="BU_Phib"></param>
        /// <param name="CU_Phic"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int ReadPowerParams(int id, out float AU_Ia_or_I, out float BU_Ib_or_L1_U, out float CU_Ic_or_L2_U, out float AI_Ua, out float BI_Ub, out float CI_Uc, out float AU_Phia_or_Phi, out float BU_Phib, out float CU_Phic, out string[] FrameAry)
        {
            FrameAry = new string[1];
            int intRst = 1;
            AU_Ia_or_I = 0f;
            BU_Ib_or_L1_U = 0f;
            CU_Ic_or_L2_U = 0f;
            AI_Ua = 0f;
            BI_Ub = 0f;
            CI_Uc = 0f;
            AU_Phia_or_Phi = 0f;
            BU_Phib = 0f;
            CU_Phic = 0f;
            CL188L_RequestReadGHPramPacket rc = new CL188L_RequestReadGHPramPacket();
            CL188L_RequestReadBwGHPramReplyPacket recv = new CL188L_RequestReadBwGHPramReplyPacket();
            rc.Pos = id;
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            rc.BwStatus = SelectStatus;
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                        AU_Ia_or_I = recv.AU_Ia_or_I;
                        BU_Ib_or_L1_U = recv.BU_Ib_or_L1_U;
                        CU_Ic_or_L2_U = recv.CU_Ic_or_L2_U;
                        AI_Ua = recv.AI_Ua;
                        BI_Ub = recv.BI_Ub;
                        CI_Uc = recv.CI_Uc;
                        AU_Phia_or_Phi = recv.AU_Phia_or_Phi;
                        BU_Phib = recv.BU_Phib;
                        CU_Phic = recv.CU_Phic;

                    }
                }
                else
                {
                    intRst = 0;
                }

            }
            catch (Exception)
            {
                return -1;
            }
            return intRst;
        }
        /// <summary>
        /// 启动遥信输出命令
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="YXTestNo">要信路数</param>
        /// <param name="YxTestType">遥信输出方式</param>
        /// <param name="YxTestPulseNum">脉冲个数</param>
        /// <param name="yxTestPulseOutHz">脉冲输出频率</param>
        /// <param name="yxTestOutmultiple">输出占控比</param>
        /// <param name="p_str_OutFrame">输出帧</param>
        /// <returns></returns>
        public int StartRemoteSignals(int Id, int p_int_RemoteCount, int p_int_OutputType, int p_int_PulseCount, float p_flt_PulseOutHz, float p_flt_OutMultiple, out string[] p_str_OutFrame)
        {
            p_str_OutFrame = new string[1];
            int intRst = 1;
            CL188L_RequestStartYXOutPacket rc = new CL188L_RequestStartYXOutPacket
            {
                Pos = Id,
                ChannelNo = ChannelNo,
                ChannelNum = ChannelNum,
                BwStatus = SelectStatus
            };
            rc.SetPara(SelectStatus, p_int_RemoteCount, p_int_OutputType, p_int_PulseCount, p_flt_PulseOutHz, p_flt_OutMultiple);
            CL188L_RequestStartYXOutReplayPacket recv = new CL188L_RequestStartYXOutReplayPacket();
            try
            {
                p_str_OutFrame[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }
            return intRst;
        }

        /// <summary>
        /// 启动直流模拟量
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="Current">直流模拟量采集电流</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int StartDCAnalog(int id, int Current, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            CL188L_RequestStartZLMNTestFunctionPacket rc = new CL188L_RequestStartZLMNTestFunctionPacket();
            rc.SetParam(id, Current);
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            rc.BwStatus = SelectStatus;
            CL188L_RequestStartZLMNTestFunctionReplayPacket recv = new CL188L_RequestStartZLMNTestFunctionReplayPacket();
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }

            }
            catch (Exception)
            {

                return -1;
            }

            return intRst;
        }
        /// <summary>
        /// 停止遥信试验、直流模拟量采集试验
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="checkType">试验类型</param>
        /// <param name="chennNo">遥信路数</param>
        /// <param name="p_str_OutFrame">输出帧</param>
        /// <returns></returns>

        public int StopOutPut(int id, int p_int_CheckType, int p_int_RemoteCount, out string[] p_str_OutFrame)
        {
            p_str_OutFrame = new string[1];
            int intRst = 1;

            //188L控制2路脉冲时必须发3才控制2路脉冲
            //if (p_int_RemoteCount == 2) p_int_RemoteCount = 3;

            CL188L_RequestStopPCYXZLTestFunctionPacket rc = new CL188L_RequestStopPCYXZLTestFunctionPacket();
            CL188L_RequestStopPCYXZLTestFunctionReplayPacket recv = new CL188L_RequestStopPCYXZLTestFunctionReplayPacket();
            rc.Pos = id;
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            rc.BwStatus = SelectStatus;
            rc.SetParam(SelectStatus, p_int_CheckType, p_int_RemoteCount);

            try
            {
                p_str_OutFrame[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }

            return intRst;
        }

        /// <summary>
        /// 读取遥控信号
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="YkCount">遥控路数</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int ReadSignals(int id, int YkCount, out int PusleCount, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            PusleCount = 0;
            CL188L_RequestYaoKongPacket rc = new CL188L_RequestYaoKongPacket();
            CL188L_RequestYaoKongReplayPacket recv = new CL188L_RequestYaoKongReplayPacket();
            rc.SetPara(YkCount);
            rc.Pos = id;
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            rc.BwStatus = SelectStatus;
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                        PusleCount = recv.PusleCount;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }

            return intRst;
        }
        /// <summary>
        /// 跳闸选择命令
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="SwitchCommand">跳闸方式</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int SetTripRelayType(int id, byte SwitchCommand, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            CL188L_RequestChoiceSwitchPacket rc = new CL188L_RequestChoiceSwitchPacket(SwitchCommand)
            {
                Pos = id,
                ChannelNo = ChannelNo,
                ChannelNum = ChannelNum,
                BwStatus = SelectStatus
            };
            CL188L_RequestChoiceSwitchReplayPacket recv = new CL188L_RequestChoiceSwitchReplayPacket();
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }

            }
            catch (Exception)
            {

                return -1;
            }
            return intRst;
        }
        /// <summary>
        /// 负控继电器控制命令
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="bearRelayStatus">负控继电器状态</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int SetSecondaryRelayStatus(int id, byte bearRelayStatus, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];

            CL188L_RequestFuKJDQPacket rc = new CL188L_RequestFuKJDQPacket();
            CL188L_RequestFuKJDQReplayPacket recv = new CL188L_RequestFuKJDQReplayPacket();
            rc.FkTestStatus = bearRelayStatus;
            rc.Pos = id;
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            rc.BwStatus = SelectStatus;
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }

            }
            catch (Exception)
            {

                return -1;
            }

            return intRst;
        }
        /// <summary>
        /// 直流输出校准命令
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int DirectCurrentOutPutCorrect(int id, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];

            CL188L_RequestDirectCurrentOutputPacket rc = new CL188L_RequestDirectCurrentOutputPacket();
            CL188L_RequestDirectCurrentOutputReplayPacket recv = new CL188L_RequestDirectCurrentOutputReplayPacket();
            rc.Pos = id;
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            rc.BwStatus = SelectStatus;
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }

            }
            catch (Exception)
            {

                return -1;
            }

            return intRst;
        }
        /// <summary>
        /// 直流输出校准传递实际值
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="uiData">直流模拟量实际电流值</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int DirectCurrentOutPutRealityValueCorrect(int id, ushort uiData, out short correctParams, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            correctParams = 0;
            CL188L_RequestDirectCurrentOutputCalibrationPacket rc = new CL188L_RequestDirectCurrentOutputCalibrationPacket(uiData)
            {
                Pos = id,
                ChannelNo = ChannelNo,
                ChannelNum = ChannelNum,
                BwStatus = SelectStatus
            };
            CL188L_RequestDirectCurrentOutputCalibrationReplayPacket recv = new CL188L_RequestDirectCurrentOutputCalibrationReplayPacket();

            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                        correctParams = recv.correctParams;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }
            return intRst;
        }
        /// <summary>
        /// 压表电机检测就位延时调试命令
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="UpOrDown">0上限位|1下限位</param>
        /// <param name="Option">0递增延时时间|1递减延时时间</param>
        /// <param name="CalTime">需要递增或递减的延时时间(ms)</param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int SetMotorMutexAndSpeed(int id, int UpOrDown, int Option, int CalTime, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            CL188L_SetElectromotorTimePacket rc = new CL188L_SetElectromotorTimePacket(id, UpOrDown, Option, CalTime)
            {
                ChannelNo = ChannelNo,
                ChannelNum = ChannelNum,
                BwStatus = SelectStatus
            };
            CL188L_SetElectromotorTimePacketReplayPacket recv = new CL188L_SetElectromotorTimePacketReplayPacket();
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }
            return intRst;
        }
        /// <summary>
        /// 压表电机检测就位延时时间
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="UpDelaytime">上限位延时时间</param>
        /// <param name="DownDelaytime">下限位延时时间</param>
        /// <param name="FrameAry">输出参数</param>
        /// <returns></returns>
        public int ReadMotorSpeed(int id, out int UpDelaytime, out int DownDelaytime, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            UpDelaytime = 0;
            DownDelaytime = 0;
            CL188L_ReadElectromotorTimePacket rc = new CL188L_ReadElectromotorTimePacket
            {
                Pos = id,
                ChannelNo = ChannelNo,
                ChannelNum = ChannelNum,
                BwStatus = SelectStatus
            };
            CL188L_ReadElectromotorTimePacketReplayPacket recv = new CL188L_ReadElectromotorTimePacketReplayPacket();
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                        UpDelaytime = recv.UpDelayTime;
                        DownDelaytime = recv.DownDelayTime;

                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }

            return intRst;
        }
        /// <summary>
        /// 读取电流柱温度
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="readType">0=A,1=B,2=C</param>
        /// <param name="Temperature">温度</param>
        /// <param name="FrameAry">输出参数</param>
        /// <returns></returns>
        public int ReadTConnector(int id, int readType, out string[] Temperature, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            Temperature = new string[0];
            CL188L_RequestReadBwTemperaturePacket rc = new CL188L_RequestReadBwTemperaturePacket();
            CL188L_RequestReadBwTemperatureReplyPacket recv = new CL188L_RequestReadBwTemperatureReplyPacket();
            rc.Pos = id;
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            rc.BwStatus = SelectStatus;
            rc.m_intReadType = readType;
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                        Temperature = recv.Temperature;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }

            return intRst;
        }
        /// <summary>
        ///  继电器控制指令
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="RelayID">继电器ID</param>
        /// <param name="ControlType">0断开继电器|1闭合继电器</param>
        /// <param name="FrameAry">输出参数</param>
        /// <returns></returns>
        public int SetSwitchType(int id, byte RelayID, byte ControlType, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            CL188L_RequestSetSwitchPacket rc = new CL188L_RequestSetSwitchPacket(RelayID, ControlType);
            CL188L_RequestSetSwitchReplayPacket recv = new CL188L_RequestSetSwitchReplayPacket();
            rc.Pos = id;
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            rc.BwStatus = SelectStatus;
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }

            return intRst;
        }

        /// <summary>
        /// 查询继电器状态
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="RelayID">继电器ID</param>
        /// <param name="ContrlType">0断开|1闭合</param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int ReadRelayStatus(int id, byte RelayID, out byte ContrlType, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            ContrlType = 13;
            CL188L_RequestQuerySwitchStatusPacket rc = new CL188L_RequestQuerySwitchStatusPacket(RelayID)
            {
                Pos = id,
                ChannelNo = ChannelNo,
                ChannelNum = ChannelNum,
                BwStatus = SelectStatus
            };
            CL188L_RequestQuerySwitchStatusReplayPacket recv = new CL188L_RequestQuerySwitchStatusReplayPacket();

            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                        ContrlType = recv.SwitchStatus;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }
            return intRst;
        }
        /// <summary>
        /// 设置检定设备信息
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="MeterType">电能表类型</param>
        /// <param name="CTtype">CT类型</param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int SetEquipInformation(int id, byte MeterType, byte CTtype, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            CL188L_RequestSetEquipInformationPacket rc = new CL188L_RequestSetEquipInformationPacket(MeterType, CTtype)
            {
                Pos = id,
                ChannelNo = ChannelNo,
                ChannelNum = ChannelNum,
                BwStatus = SelectStatus
            };
            CL188L_RequestSetEquipInformationReplayPacket recv = new CL188L_RequestSetEquipInformationReplayPacket();
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {
                return -1;
            }
            return intRst;
        }
        /// <summary>
        /// 读取设备信息
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="MeterIndex">表位号</param>
        /// <param name="MeterType">电能表类型</param>
        /// <param name="CTbyte">CT类型</param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int ReadEquipInformation(int id, out int MeterIndex, out int MeterType, out int CTbyte, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            MeterIndex = 0;
            MeterType = 0;
            CTbyte = 0;
            CL188L_RequestReadEquipInformationPacket rc = new CL188L_RequestReadEquipInformationPacket();
            CL188L_RequestReadEquipInformationReplayPacket recv = new CL188L_RequestReadEquipInformationReplayPacket();
            rc.Pos = id;
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            rc.BwStatus = SelectStatus;
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                        MeterIndex = recv.MeterNumber;
                        MeterType = recv.MeterType;
                        CTbyte = recv.CtType;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }
            return intRst;
        }
        /// <summary>
        /// 读取脉冲通道及检定类型
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="pram1">误差通道</param>
        /// <param name="pram2">脉冲类型</param>
        /// <param name="pram3">脉冲极性选择</param>
        /// <param name="pram4">多功能 0误差通道号,1为日计时脉冲、2为需量脉冲。</param>
        /// <param name="pram5">检定类型：0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定。0x06：耐压实验 0x07：多功能脉冲计数试验</param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int ReadPulseChannelAndType(int id, out int pram1, out int pram2, out int pram3, out int pram4, out int pram5, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            pram1 = -1;
            pram2 = -1;
            pram3 = -1;
            pram4 = -1;
            pram5 = -1;
            CL188L_RequestQueryPusleChannelAndCheckTypePacket rc = new CL188L_RequestQueryPusleChannelAndCheckTypePacket();
            CL188L_RequestQueryPusleChannelAndCheckTypeReplayPacket recv = new CL188L_RequestQueryPusleChannelAndCheckTypeReplayPacket();
            rc.Pos = id;
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            rc.BwStatus = SelectStatus;
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                        pram1 = (byte)recv.wcChannelNo;
                        pram2 = (byte)recv.pulseType;
                        pram3 = (byte)recv.GyGy;
                        pram4 = (byte)recv.dgnWcChannelNo;
                        pram5 = (byte)recv.checkType;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }

            return intRst;
        }
        /// <summary>
        /// 设置光电头状态选择
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="selecttype">选择类型</param>
        /// <param name="p_str_OutFrame">输出帧</param>
        /// <returns></returns>
        public int SetSelectLightStatus(int Id, byte p_byt_SelectType, out string[] p_str_OutFrame)
        {
            int intRst = 1;
            p_str_OutFrame = new string[1];
            CL188L_RequestSelectLightStatusPacket rc = new CL188L_RequestSelectLightStatusPacket
            {
                Pos = Id
            };
            rc.SetPara(SelectStatus, (Cus_EmLightSelect)p_byt_SelectType, ChannelNo, ChannelNum);

            CL188L_RequestSelectLightStatusReplayPacket recv = new CL188L_RequestSelectLightStatusReplayPacket();
            try
            {
                p_str_OutFrame[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }
            return intRst;
        }

        /// <summary>
        /// 设置双回路命令
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="iroad">电流回路路数</param>
        /// <param name="vroad">电压回路路数</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int SetSelectCheckRoad(int id, byte iroad, byte vroad, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            CL188L_RequestSelectCheckRoadPacket rc = new CL188L_RequestSelectCheckRoadPacket(id, (Cus_EmBothIRoadType)iroad, (Cus_EmBothVRoadType)vroad);
            CL188L_RequestSelectCheckRoadReplayPacket recv = new CL188L_RequestSelectCheckRoadReplayPacket();

            rc.Pos = id;
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            rc.BwStatus = SelectStatus;
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }

            }
            catch (Exception)
            {

                return -1;
            }


            return intRst;
        }
        /// <summary>
        /// 启动检定
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="verifyType">检定类型</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int StartTest(int id, byte verifyType, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            CL188L_RequestStartPCFunctionPacket rc = new CL188L_RequestStartPCFunctionPacket();
            CL188L_RequestStartPCFunctionReplayPacket recv = new CL188L_RequestStartPCFunctionReplayPacket();
            rc.ChannelNum = ChannelNum;
            rc.ChannelNo = ChannelNo;
            rc.SetPara(id, (Cus_EmCheckType)verifyType);
            rc.BwStatus = SelectStatus;
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }

            }
            catch (Exception)
            {

                return -1;
            }

            return intRst;
        }
        /// <summary>
        /// 停止检定
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="verifyType">检定类型</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int StopTest(int id, byte verifyType, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            CL188L_RequestStopPCFunctionPacket rc = new CL188L_RequestStopPCFunctionPacket
            {
                ChannelNum = ChannelNum,
                ChannelNo = ChannelNo
            };
            rc.SetParam(id, (Cus_EmCheckType)verifyType);
            rc.BwStatus = SelectStatus;
            CL188L_RequestStopPCFunctionReplayPacket recv = new CL188L_RequestStopPCFunctionReplayPacket();

            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }
            return intRst;


        }

        /// <summary>
        /// 设置表位电压电流隔离
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="IsolationStatus">继电器状态</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int SetBwVolCutIsolation(int id, int p_int_IsolationStatus, out string[] p_str_OutFrame)
        {
            int intRst = 1;
            p_str_OutFrame = new string[1];
            CL188L_RequestSeperateBwControlPacket rc = new CL188L_RequestSeperateBwControlPacket
            {
                Pos = id,
                ChannelNo = ChannelNo,
                ChannelNum = ChannelNum
            };
            rc.SetPara(SelectStatus, p_int_IsolationStatus == 1, ChannelNo, ChannelNum);
            CL188L_RequestSeperateBwControlReplayPacket recv = new CL188L_RequestSeperateBwControlReplayPacket();
            try
            {
                p_str_OutFrame[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    rc.IsNeedReturn = false;
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }

            return intRst;
        }
        /// <summary>
        /// 读取表位电压电流隔离状态
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="IsolationStatus">继电器状态</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int ReadBwVolCutsolation(int id, out int IsolationStatus, out string[] FrameAry)
        {
            int intRst = 1;
            IsolationStatus = 0;
            FrameAry = new string[1];
            CL188L_RequestQuerySeperateBwControlPacket rc = new CL188L_RequestQuerySeperateBwControlPacket();
            CL188L_RequestQuerySeperateBwControlReplayPacket recv = new CL188L_RequestQuerySeperateBwControlReplayPacket();
            rc.Pos = id;
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                        IsolationStatus = recv.Seperate;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }

            return intRst;
        }
        /// <summary>
        /// 设置耐压继电器指令控制状态
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="highVoltageType">耐压继电器指令控制状态</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int SetACTVRelay(int id, byte highVoltageType, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            CL188L_SendCMDToInsulationRelays rc = new CL188L_SendCMDToInsulationRelays();
            CL188L_SendCMDToInsulationRelaysReplayPacket recv = new CL188L_SendCMDToInsulationRelaysReplayPacket();
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            rc.SetPara(id, highVoltageType);

            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {
                return -1;
            }
            return intRst;
        }
        /// <summary>
        /// 查询耐压继电器状态
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="highVoltageType">耐压继电器指令控制状态</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int ReadACTVRelay(int id, out byte highVoltageType, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            highVoltageType = 0;
            CL188L_RequestQueryCMDToInsulationRelaysPacket rc = new CL188L_RequestQueryCMDToInsulationRelaysPacket();
            CL188L_RequestQueryCMDToInsulationRelaysRePlayPacket recv = new CL188L_RequestQueryCMDToInsulationRelaysRePlayPacket();
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            rc.Pos = id;
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                        highVoltageType = recv.InsulationStatus;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }

            return intRst;
        }
        /// <summary>
        /// 设置CT档位指令
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="CtType">CT 类型</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int SetCTPosition(int id, byte CtType, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            CL188L_RequestCTPositionChannelControlPacket rc = new CL188L_RequestCTPositionChannelControlPacket(id, (Cus_EmIChannelType)CtType);
            CL188L_RequestCTPositionChannelControlReplayPacket recv = new CL188L_RequestCTPositionChannelControlReplayPacket();
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }
            return intRst;
        }
        /// <summary>
        /// 读取误差板当前误差
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="verifyType">误差板当前状态</param>
        /// <param name="verificationType">输出当前检定误差类型</param>
        /// <param name="MeterIndex">表位号</param>
        /// <param name="ErrorNum">误差次数</param>
        /// <param name="wcData">误差值</param>
        /// <param name="statusType">状态类型</param>
        /// <param name="CurrentContour">电流回路状态</param>
        /// <param name="VoltageContour">电压回路状态</param>
        /// <param name="CommunicationType">通讯口状态</param>
        /// <param name="workType">工作状态</param>
        /// <param name="expandStatus">扩展状态</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int ReadCurrentData(int id, byte wcbVerifyType, out byte verificationType, out int MeterIndex, out int ErrorNum, out string wcData, out bool[] statusType, out byte CurrentContour, out byte VoltageContour, out byte CommunicationType, out bool[] workType, out byte expandStatus, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            verificationType = 0;
            MeterIndex = 0;
            ErrorNum = 0;
            wcData = string.Empty;
            statusType = new bool[8];
            CurrentContour = 0;
            VoltageContour = 0;
            CommunicationType = 0;
            workType = new bool[8];
            expandStatus = 0;
            CL188L_RequestReadBwWcAndStatusPacket rc = new CL188L_RequestReadBwWcAndStatusPacket((Cus_EmWuchaType)wcbVerifyType);
            CL188L_RequestReadBwWcAndStatusReplyPacket recv = new CL188L_RequestReadBwWcAndStatusReplyPacket();
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            rc.Pos = id;
            rc.BwStatus = SelectStatus;

            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = 0;//recv.ReciveResult == RecvResult.OK ? 0 : 2;
                        verificationType = (byte)recv.CheckType;
                        MeterIndex = recv.CurBwNum;
                        ErrorNum = recv.WcNum;
                        wcData = recv.WcData;
                        //接线故障状态
                        statusType[0] = recv.StatusTypeIsOnErr_Jxgz;
                        //预付费状态
                        statusType[1] = recv.StatusTypeIsOnErr_Yfftz;
                        //报警信号状态
                        statusType[2] = recv.StatusTypeIsOnErr_Bjxh;
                        //对标状态
                        statusType[3] = recv.StatusTypeIsOnOver_Db;
                        //温度过高状态
                        statusType[4] = recv.StatusTypeIsOnErr_Temp;
                        //光电信号状态 有表还是没有表
                        statusType[5] = recv.StatusTypeIsOn_HaveMeter;
                        //表位上限位状态
                        statusType[6] = recv.StatusTypeIsOn_PressUpLimit;
                        //表位下限位状态
                        statusType[7] = recv.StatusTypeIsOn_PressDownLimt;
                        CurrentContour = (byte)recv.IType;
                        VoltageContour = (byte)recv.VType;
                        CommunicationType = Convert.ToByte(recv.ConnType);
                        //电能误差
                        workType[0] = recv.WorkStatusIsOn_Dn;
                        //需量周期
                        workType[1] = recv.WorkStatusIsOn_Xlzq;
                        //日计时
                        workType[2] = recv.WorkStatusIsOn_Rjs;
                        //多功能走字
                        workType[3] = recv.WorkStatusIsOn_Dnzz;
                        //对标
                        workType[4] = recv.WorkStatusIsOn_Db;
                        //预付费
                        workType[5] = recv.WorkStatusIsOn_Yff;
                        //耐压
                        workType[6] = recv.WorkStatusIsOn_Ny;
                        //多功能脉冲计数
                        workType[7] = recv.WorkStatusIsOn_Dgnmcjs;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }


            return intRst;
        }
        /// <summary>
        /// 读取最近10次误差
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="verifyType">检定误差类型</param>
        /// <param name="verificationType">输出检定误差类型</param>
        /// <param name="MeterIndex">表位号</param>
        /// <param name="ErrorNum">误差次数</param>
        /// <param name="wcDatas">误差数据</param>
        /// <param name="wcData">当前误差值</param>
        /// <param name="statusType">状态类型</param>
        /// <param name="CurrentContour">电流回路</param>
        /// <param name="VoltageContour">电压回路</param>
        /// <param name="CommunicationType">通讯状态</param>
        /// <param name="workType">工作状态</param>
        /// <param name="FrameAry">输出参数</param>
        /// <returns></returns>
        public int ReadFirstTenTimesData(int id, byte wcbVerifyType, out byte verificationType, out int MeterIndex, out int ErrorNum, out string[] wcDatas, out string wcData, out bool[] statusType, out byte CurrentContour, out byte VoltageContour, out byte CommunicationType, out bool[] workType, out string[] FrameAry)
        {
            int intRst = 1;
            verificationType = 0;
            MeterIndex = 0;
            ErrorNum = 0;
            wcDatas = new string[9];
            wcData = string.Empty;
            statusType = new bool[8];
            CurrentContour = 0;
            VoltageContour = 0;
            CommunicationType = 0;
            workType = new bool[8];
            FrameAry = new string[1];
            CL188L_RequestReadBwLast10WcAndStatusPacket rc = new CL188L_RequestReadBwLast10WcAndStatusPacket(id, (Cus_EmWuchaType)wcbVerifyType);
            CL188L_RequestReadBwLast10WcAndStatusReplyPacket recv = new CL188L_RequestReadBwLast10WcAndStatusReplyPacket();
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = 0;//recv.ReciveResult == RecvResult.OK ? 0 : 2;
                        verificationType = (byte)recv.CheckType;
                        MeterIndex = recv.CurBwNum;
                        ErrorNum = recv.WcNum;
                        wcDatas = recv.WcData;
                        wcData = recv.WcData[recv.WcData.Length - 1];
                        //接线故障状态
                        statusType[0] = recv.StatusTypeIsOnErr_Jxgz;
                        //预付费状态
                        statusType[1] = recv.StatusTypeIsOnErr_Yfftz;
                        //报警信号状态
                        statusType[2] = recv.StatusTypeIsOnErr_Bjxh;
                        //对标状态
                        statusType[3] = recv.StatusTypeIsOnOver_Db;
                        //温度过高状态
                        statusType[4] = recv.StatusTypeIsOnErr_Temp;
                        //光电信号状态 有表还是没有表
                        statusType[5] = recv.StatusTypeIsOn_HaveMeter;
                        //表位上限位状态
                        statusType[6] = recv.StatusTypeIsOn_PressUpLimit;
                        //表位下限位状态
                        statusType[7] = recv.StatusTypeIsOn_PressDownLimt;
                        CurrentContour = (byte)recv.CurrentRoadType;
                        VoltageContour = (byte)recv.VoltageRoadType;
                        CommunicationType = Convert.ToByte(recv.ConnType);
                        //电能误差
                        workType[0] = recv.WorkStatusIsOn_Dn;
                        //需量周期
                        workType[1] = recv.WorkStatusIsOn_Xlzq;
                        //日计时
                        workType[2] = recv.WorkStatusIsOn_Rjs;
                        //多功能走字
                        workType[3] = recv.WorkStatusIsOn_Dnzz;
                        //对标
                        workType[4] = recv.WorkStatusIsOn_Db;
                        //预付费
                        workType[5] = recv.WorkStatusIsOn_Yff;
                        //耐压
                        workType[6] = recv.WorkStatusIsOn_Ny;
                        //多功能脉冲计数
                        workType[7] = recv.WorkStatusIsOn_Dgnmcjs;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }

            return intRst;
        }
        /// <summary>
        /// 清除表位状态
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="statusType">状态类型</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int ReSetStatus(int id, byte statusType, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            CL188L_RequestClearBwStatusPacket rc = new CL188L_RequestClearBwStatusPacket(statusType)
            {
                ChannelNo = ChannelNo,
                ChannelNum = ChannelNum,
                Pos = id
            };
            CL188L_RequestClearBwStatusReplayPacket recv = new CL188L_RequestClearBwStatusReplayPacket();
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }

            }
            catch (Exception)
            {

                return -1;
            }

            return intRst;
        }
        /// <summary>
        /// 读取表位版本信息
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="MeterIndex">表位号</param>
        /// <param name="strVersion">版本号</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int ReadVersion(int id, out int MeterIndex, out string strVersion, out string[] FrameAry)
        {
            int intRst = 1;
            MeterIndex = 0;
            strVersion = string.Empty;
            FrameAry = new string[1];
            CL188L_RequestReadWcBoardVerPacket rc = new CL188L_RequestReadWcBoardVerPacket(id);
            CL188L_RequestReadWcBoardVerReplyPacket recv = new CL188L_RequestReadWcBoardVerReplyPacket();
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            rc.Pos = id;
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                        MeterIndex = recv.WcbIndex;
                        strVersion = recv.SoftVer;
                    }
                }
                else
                {
                    intRst = 0;
                }

            }
            catch (Exception)
            {
                return -1;
            }
            return intRst;
        }
        /// <summary>
        /// 读取打印信息
        /// </summary>
        /// <param name="id">指定模块表位号</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int ReadPrintInformation(int id, int Modid, out byte dataSerial, out byte dataLen, out byte[] dataAry, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            dataSerial = 0;
            dataLen = 0;
            dataAry = new byte[0];
            CL188L_RequestReadPrintInformationPacket rc = new CL188L_RequestReadPrintInformationPacket(Convert.ToByte(Modid))
            {
                ChannelNo = ChannelNo,
                ChannelNum = ChannelNum,
                Pos = id
            };
            CL188L_RequestReadPrintInformationReplayPacket recv = new CL188L_RequestReadPrintInformationReplayPacket();
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                        dataSerial = recv.Serial;
                        dataLen = recv.DataNum;
                        dataAry = recv.listData.ToArray();
                    }
                }
                else
                {
                    intRst = 0;
                }

            }
            catch (Exception)
            {

                return -1;
            }

            return intRst;
        }
        /// <summary>
        /// 读取隔离继电器过载动作可靠性检测时间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int ReadIsolationrelayOverloadReliabilityTime(int id, out int meterIndex, out int detectionTime, out string[] FrameAry)
        {
            int intRst = 1;
            meterIndex = 0;
            detectionTime = 0;
            FrameAry = new string[1];

            CL188L_RequestIsolationrelayOverloadReliabilityPacket rc = new CL188L_RequestIsolationrelayOverloadReliabilityPacket();
            CL188L_RequestIsolationrelayOverloadReliabilityReplayPacket recv = new CL188L_RequestIsolationrelayOverloadReliabilityReplayPacket();
            rc.ChannelNo = ChannelNo;
            rc.ChannelNum = ChannelNum;
            rc.Pos = id;

            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                        meterIndex = recv.MeterNumber;
                        detectionTime = recv.detectionTime;
                    }
                }
                else
                {
                    intRst = 0;
                }

            }
            catch (Exception)
            {

                return -1;
            }

            return intRst;
        }
        /// <summary>
        /// 设置隔离继电器过载动作可靠性检测性时间
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="Time">设置时间</param>
        /// <param name="FrameAry">出参帧</param>
        /// <returns></returns>
        public int SetIsolationrelayOverloadReliabilityTime(int id, byte Time, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            CL188L_RequestIsolationrelayOverloadReliabilityTimePacket rc = new CL188L_RequestIsolationrelayOverloadReliabilityTimePacket(Time);
            CL188L_RequestIsolationrelayOverloadReliabilityTimeReplayPacket recv = new CL188L_RequestIsolationrelayOverloadReliabilityTimeReplayPacket();
            rc.Pos = id;
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }

            }
            catch (Exception)
            {

                return -1;
            }
            return intRst;
        }
        /// <summary>
        ///  设置电能误差检定时脉冲参数命令
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="stdMeterConst">标准表常数</param>
        /// <param name="stdPulseFreq">标准脉冲频率</param>
        /// <param name="stdMeterConstShorttime">标准缩放倍数</param>
        /// <param name="meterConst">被检表常数</param>
        /// <param name="circles">圈数</param>
        /// <param name="meterConstZooms">被检表常数缩放倍数</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int SetEnergePulseParams(int id, int stdMeterConst, int stdPulseFreq, int stdMeterConstShorttime, int meterConst, int circles, int meterConstZooms, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            CL188L_RequestSetPulseParaPacket rc = new CL188L_RequestSetPulseParaPacket();
            rc.SetPara(id, stdMeterConst, stdPulseFreq, stdMeterConstShorttime, meterConst, circles, meterConstZooms);
            CL188L_RequestSetPulseParaReplayPacket recv = new CL188L_RequestSetPulseParaReplayPacket();
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }

            }
            catch (Exception)
            {

                return -1;
            }


            return intRst;
        }
        /// <summary>
        ///  设置电能误差检定时脉冲参数命令
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="stdMeterConst">标准表常数</param>
        /// <param name="stdPulseFreq">标准脉冲频率</param>
        /// <param name="stdMeterConstShorttime">标准缩放倍数</param>
        /// <param name="meterConst">被检表常数</param>
        /// <param name="circles">圈数</param>
        /// <param name="meterConstZooms">被检表常数缩放倍数</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int SetEnergePulseParams(bool[] bwstatus, int stdMeterConst, int stdPulseFreq, int stdMeterConstShorttime, int meterConst, int circles, int meterConstZooms, out string[] FrameAry, int iChannelNum, int iChannelNo)
        {
            int intRst = 1;
            FrameAry = new string[1];
            CL188L_RequestSetPulseParaPacket rc = new CL188L_RequestSetPulseParaPacket();
            rc.SetPara(bwstatus, stdMeterConst, stdPulseFreq, stdMeterConstShorttime, meterConst, circles, meterConstZooms, iChannelNum, iChannelNo);
            CL188L_RequestSetPulseParaReplayPacket recv = new CL188L_RequestSetPulseParaReplayPacket();
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }

            }
            catch (Exception)
            {

                return -1;
            }


            return intRst;
        }
        /// <summary>
        /// 4.50	查询电能误差检定时脉冲参数命令
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="stdMeterConst">标准表脉冲</param>
        /// <param name="stdMeterConstShorttime">标准脉冲缩放倍数</param>
        /// <param name="meterConst">被检表常数</param>
        /// <param name="circles">圈数</param>
        /// <param name="meterConstZooms">被检脉冲常数缩放倍数</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int ReadEnergePulseParams(int id, out int stdMeterConst, out int stdMeterConstShorttime, out int meterConst, out int circles, out int meterConstZooms, out string[] FrameAry)
        {
            int intRst = 1;
            stdMeterConst = 0;
            stdMeterConstShorttime = 0;
            meterConst = 0;
            circles = 0;
            meterConstZooms = 0;
            FrameAry = new string[1];
            CL188L_RequestQueryPulseParamPacket rc = new CL188L_RequestQueryPulseParamPacket
            {
                Pos = id
            };
            CL188L_RequestQueryPulseParamReplayPacket recv = new CL188L_RequestQueryPulseParamReplayPacket();

            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                        stdMeterConst = recv.stdMeterConst;
                        stdMeterConstShorttime = recv.stdMeterConstShortTime;
                        meterConst = recv.meterConst;
                        circles = recv.meterQuans;
                        meterConstZooms = recv.stdMeterConstShortTime;
                    }
                }
                else
                {
                    intRst = 0;
                }

            }
            catch (Exception)
            {

                return -1;
            }

            return intRst;
        }
        /// <summary>
        /// 设置时钟频率
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="stdMeterTimeFreq">标准表时钟频率</param>
        /// <param name="meterTimeFreq">被检表时钟频率</param>
        /// <param name="meterPulseNum">电表脉冲常数</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int SetClockFrequency(int id, int stdMeterTimeFreq, int meterTimeFreq, int meterPulseNum, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            CL188L_RequestSetTimePluseNumAndXLTimePacket rc = new CL188L_RequestSetTimePluseNumAndXLTimePacket();
            CL188L_RequestSetTimePluseNumAndXLTimeReplayPacket recv = new CL188L_RequestSetTimePluseNumAndXLTimeReplayPacket();
            rc.SetPara(id, stdMeterTimeFreq, meterTimeFreq, meterPulseNum);

            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }
            return intRst;
        }
        /// <summary>
        /// 读取时钟频率
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="stdMeterTimeFreq">标准表时钟频率</param>
        /// <param name="meterTimeFreq">被检表时钟频率</param>
        /// <param name="meterPulseNum">被检表脉冲常数</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int ReadClockFrequency(int id, out int stdMeterTimeFreq, out int meterTimeFreq, out int meterPulseNum, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            stdMeterTimeFreq = 0;
            meterTimeFreq = 0;
            meterPulseNum = 0;
            CL188L_RequestQueryTimePluseNumAndXLTimePacket rc = new CL188L_RequestQueryTimePluseNumAndXLTimePacket
            {
                Pos = id
            };
            CL188L_RequestQueryTimePusleNumAndXLTimeReplayPacket recv = new CL188L_RequestQueryTimePusleNumAndXLTimeReplayPacket();
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                        //
                        stdMeterTimeFreq = recv.stdMeterTimeFreq;
                        meterTimeFreq = recv.meterTimeFreq;
                        meterPulseNum = recv.meterPulseNum;
                    }
                }
                else
                {
                    intRst = 0;
                }

            }
            catch (Exception)
            {
                return -1;
            }

            return intRst;
        }
        /// <summary>
        /// 设置误差板耐压漏电流阀值
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="iCurrentLimit">漏电流阀值</param>
        /// <param name="FrameAry">输出参数</param>
        /// <returns></returns>
        public int SetACTVLeakCurrentThresholdValue(int id, int iCurrentLimit, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            CL188L_RequestSetWishStandCurrentLimitPacket rc = new CL188L_RequestSetWishStandCurrentLimitPacket();
            rc.SetPara(id, iCurrentLimit);
            CL188L_RequestSetWishStandCurrentLimitReplayPacket recv = new CL188L_RequestSetWishStandCurrentLimitReplayPacket();
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                        //
                    }
                }
                else
                {
                    intRst = 0;
                }
            }
            catch (Exception)
            {

                return -1;
            }


            return intRst;

        }
        /// <summary>
        /// 查询耐压漏电流阀值
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="iCurrentLimit">漏电流阀值</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int ReadACTVLeakCurrentThresholdValue(int id, out int iCurrentLimit, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            iCurrentLimit = 0;
            CL188L_RequestQueryWishStandCurrentLimitPacket rc = new CL188L_RequestQueryWishStandCurrentLimitPacket
            {
                Pos = id
            };
            CL188L_RequestQueryWishStandCurrentLimitReplayPacket recv = new CL188L_RequestQueryWishStandCurrentLimitReplayPacket();
            try
            {
                FrameAry[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                        iCurrentLimit = recv.CurrentLimit;
                    }
                }
                else
                {
                    intRst = 0;
                }

            }
            catch (Exception)
            {
                return -1;
            }

            return intRst;
        }

        /// <summary>
        /// 电机控制
        /// </summary>
        /// <param name="p_bol_Status">表位状态</param>
        /// <param name="p_int_MachineMode">电机控制模式</param>
        /// <param name="p_int_ChannelNo">当前误差板通道号</param>
        /// <param name="p_int_TotalChannelNo">误差板总通道数</param>
        /// <param name="p_str_OutFrame">发送的报文</param>
        /// <returns></returns>
        public int ControlMotor(bool[] p_bol_Status, int p_int_MachineMode, int p_int_ChannelNo, int p_int_TotalChannelNo, out string[] p_str_OutFrame)
        {
            int intRst = 1;
            p_str_OutFrame = new string[1];
            CL188L_RequestSetElectromotorPacket rc = new CL188L_RequestSetElectromotorPacket();
            rc.SetPara(p_bol_Status, p_int_MachineMode, p_int_ChannelNo, p_int_TotalChannelNo);
            CL188L_RequestSetElectromotorReplayPacket recv = new CL188L_RequestSetElectromotorReplayPacket();
            try
            {
                p_str_OutFrame[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }

            }
            catch (Exception)
            {

                return -1;
            }


            return intRst;
        }
        #endregion

        #region private
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="UDPorCOM">true UDP,false COM</param>
        /// <param name="sp"></param>
        /// <param name="rp"></param>
        /// <returns></returns>
        private bool SendPacketWithRetry(StPortInfo port, SendPacket sp, RecvPacket rp)
        {

            for (int i = 0; i < RETRYTIEMS; i++)
            {
                if (driverBase.SendData(port, sp, rp) == true)
                {
                    return true;
                }
                System.Threading.Thread.Sleep(300);
            }

            return false;
        }


        private string BytesToString(byte[] array)
        {
            if (array == null || array.Length < 1)
                return "";

            return BitConverter.ToString(array).Replace("-", "");
        }

        #endregion



        /// <summary>
        /// 解析下行报文
        /// </summary>
        /// <param name="MothedName">相应的方法名称</param>
        /// <param name="ReFrameAry">下行报文</param>
        /// <param name="ReAry">解析后的数据</param>
        /// <returns></returns>
        public int UnPacket(string MothedName, byte[] ReFrameAry, out string[] ReAry)
        {
            int iRsValue = 3;
            MothedName = MothedName.Replace(" ", "");

            ReAry = new string[1];

            switch (MothedName)
            {
                case "Connect":
                    {
                        //连接设备         int Connect(int Id, out string[] FrameAry);
                        try
                        {
                            CL188L_RequestLinkReplayPacket recv = new CL188L_RequestLinkReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }



                    }
                    break;
                case "DisConnect":
                    {
                        iRsValue = 3;
                    }
                    break;
                case "SetPulseChannelAndType":
                    {
                        try
                        {	       //设置脉冲通道和类型        int SetPulseChannelAndType(int Id, int pram1, int pram2, int pram3, int pram4, int pram5,out string[] FrameAry);
                            CL188L_RequestSelectPulseChannelAndCheckTypeReplayPacket recv = new CL188L_RequestSelectPulseChannelAndCheckTypeReplayPacket();

                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "UpdateLogin":
                    {
                        //升级登录指令 int UpdateLogin(int id,out string[] FrameAry);
                        try
                        {
                            CL188L_RequestUpdateLoginReplayPacket recv = new CL188L_RequestUpdateLoginReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "ReBoot":
                    {
                        //重启误差板 int ReBoot(int id,out string[] FrameAry);
                        try
                        {
                            CL188L_RequestReBootReplayPacket recv = new CL188L_RequestReBootReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "UpdateFirmware":
                    {
                        //升级数据 int UpdateFirmware(int id,UInt16 DataSerial,byte[] bytesData,out string[] FrameAry);
                        try
                        {
                            CL188L_RequestUpdateFirmwareReplayPacket recv = new CL188L_RequestUpdateFirmwareReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "UpdateLogin2":
                    {
                        //升级登录2 int UpdateLogin2(int id,out string[] FrameAry);
                        try
                        {
                            CL188L_RequestUpdateLogin2ReplayPacket recv = new CL188L_RequestUpdateLogin2ReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;

                case "ReBoot2":
                    {
                        //重启设备2 int ReBoot2(int id,out string[] FrameAry);
                        try
                        {
                            CL188L_RequestReBoot2ReplayPacket recv = new CL188L_RequestReBoot2ReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "UpdateFirmware2":
                    {
                        //升级数据2 int UpdateFirmware2(int id,UInt16 DataSerial, byte[] bytesData, out string[] FrameAry);
                        try
                        {
                            CL188L_RequestUpdateFirmware2ReplayPacket recv = new CL188L_RequestUpdateFirmware2ReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "ReadVersion2":
                    {
                        //读取版本2 int ReadVersion2(int id,out string Version,out string []FrameAry);
                        try
                        {
                            CL1888M_RequestReadVersion2ReplayPacket recv = new CL1888M_RequestReadVersion2ReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "ReadPowerParams":
                    {
                        //读取功耗参数
                        //int ReadPowerParams( int id,out float AU_Ia_or_I, out float BU_Ib_or_L1_U, out float CU_Ic_or_L2_U, 
                        //                 out float AI_Ua, out float BI_Ub,out float  CI_Uc,
                        //                 out float AU_Phia_or_Phi,out float BU_Phib, out float CU_Phic, 
                        //                 out string [] FrameAry);

                        try
                        {
                            CL188L_RequestReadBwGHPramReplyPacket recv = new CL188L_RequestReadBwGHPramReplyPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry = new string[9];
                            ReAry[0] = recv.AU_Ia_or_I.ToString();
                            ReAry[1] = recv.BU_Ib_or_L1_U.ToString();
                            ReAry[2] = recv.CU_Ic_or_L2_U.ToString();
                            ReAry[3] = recv.AI_Ua.ToString();
                            ReAry[4] = recv.BI_Ub.ToString();
                            ReAry[5] = recv.CI_Uc.ToString();
                            ReAry[6] = recv.AU_Phia_or_Phi.ToString();
                            ReAry[8] = recv.BU_Phib.ToString();
                            ReAry[9] = recv.CU_Phic.ToString();
                            iRsValue = 0;

                        }
                        catch (Exception)
                        {

                            return -1;
                        }


                    }
                    break;
                case "StartRemoteSignals":
                    {
                        //启动要信输出
                        //int StartRemoteSignals(int id, int YXTestNo, int YxTestType,
                        //                 int YxTestPulseNum, float yxTestPulseOutHz, float yxTestOutmultiple,
                        //                out string[] FrameAry);
                        try
                        {
                            CL188L_RequestStartYXOutReplayPacket recv = new CL188L_RequestStartYXOutReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "StartDCAnalog":
                    {
                        //启动直流模拟量输出        int StartDCAnalog(int id, int Current,out string[] FrameAry);
                        try
                        {
                            CL188L_RequestStartZLMNTestFunctionReplayPacket recv = new CL188L_RequestStartZLMNTestFunctionReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "StopOutPut":
                    {
                        // 停止遥信、或直流模拟量输出         int StopOutPut(int id, int checkType, int chennNo,out string[] FrameAry);
                        try
                        {
                            CL188L_RequestStopPCYXZLTestFunctionReplayPacket recv = new CL188L_RequestStopPCYXZLTestFunctionReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "ReadSignals":
                    {
                        //读取遥控信号 int ReadSignals(int id, int YkCount, out int PusleCount,out string[] FrameAry);
                        try
                        {
                            CL188L_RequestYaoKongReplayPacket recv = new CL188L_RequestYaoKongReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            if (recv.CanReturnError == false)
                            {
                                ReAry[0] = recv.PusleCount.ToString();
                                iRsValue = 0;
                            }
                            else
                            {
                                iRsValue = 1;
                            }
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "SetTripRelayType":
                    {
                        //设置跳闸继电器类型         int SetTripRelayType(int id,byte SwitchCommand, out string [] FrameAry);
                        try
                        {
                            CL188L_RequestChoiceSwitchReplayPacket recv = new CL188L_RequestChoiceSwitchReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "SetSecondaryRelayStatus":
                    {
                        //负控继电器控制命令         int SetSecondaryRelayStatus(int id, byte bearRelayStatus, out string[] FrameAry);
                        try
                        {
                            CL188L_RequestFuKJDQReplayPacket recv = new CL188L_RequestFuKJDQReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "DirectCurrentOutPutCorrect":
                    {
                        //直流输出校准         int DirectCurrentOutPutCorrect(int id,out string[] FrameAry);
                        try
                        {
                            CL188L_RequestDirectCurrentOutputReplayPacket recv = new CL188L_RequestDirectCurrentOutputReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "DirectCurrentOutPutRealityValueCorrect":
                    {
                        //直流输出实际电流校准         int DirectCurrentOutPutRealityValueCorrect(int id,UInt16 uiData, out short correctParams, out string[] FrameAry);
                        try
                        {
                            CL188L_RequestDirectCurrentOutputCalibrationReplayPacket recv = new CL188L_RequestDirectCurrentOutputCalibrationReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            if (recv.ReciveResult == RecvResult.OK)
                            {
                                ReAry[0] = recv.correctParams.ToString();
                                iRsValue = 0;
                            }
                            else
                            {
                                iRsValue = 1;
                            }
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "SetMotorMutexAndSpeed":
                    {
                        //设置互斥电机和速度  int SetMotorMutexAndSpeed(int id, int UpOrDown, int Option, int CalTime,out string[] FrameAry);
                        try
                        {
                            CL188L_SetElectromotorTimePacketReplayPacket recv = new CL188L_SetElectromotorTimePacketReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "ReadMotorSpeed":
                    {
                        //读取电机速度         int ReadMotorSpeed(int id, out int UpDelaytime, out int DownDelaytime,out string[] FrameAry);
                        try
                        {
                            CL188L_ReadElectromotorTimePacketReplayPacket recv = new CL188L_ReadElectromotorTimePacketReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            if (recv.ReciveResult == RecvResult.OK)
                            {
                                ReAry = new string[2];
                                ReAry[0] = recv.UpDelayTime.ToString();
                                ReAry[1] = recv.DownDelayTime.ToString();
                                iRsValue = 0;
                            }
                            else
                            {
                                iRsValue = 1;
                            }
                        }
                        catch (Exception)
                        {

                            return -1;
                        }

                    }
                    break;
                case "ReadTConnector":
                    {
                        //读取温度         int ReadTConnector(int id, out string []Temperature,out string[] FrameAry);
                        try
                        {
                            CL188L_RequestReadBwTemperatureReplyPacket recv = new CL188L_RequestReadBwTemperatureReplyPacket();
                            recv.ParsePacket(ReFrameAry);
                            if (recv.ReciveResult == RecvResult.OK)
                            {
                                ReAry = new string[4];
                                for (int i = 0; i < 4; i++)
                                {
                                    ReAry[i] = recv.Temperature[i];
                                }
                                iRsValue = 0;
                            }
                            else
                            {
                                iRsValue = 1;
                            }
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "SetSwitchType":
                    {
                        //设置继电器类型         int SetSwitchType(int id, byte RelayID, byte ControlType,out string[] FrameAry);
                        try
                        {
                            CL188L_RequestSetSwitchReplayPacket recv = new CL188L_RequestSetSwitchReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "ReadRelayStatus":
                    {
                        //读取继电器类型        int ReadRelayStatus(int id, byte RelayID, out byte ContrlType,out string[] FrameAry);
                        CL188L_RequestQuerySwitchStatusReplayPacket recv = new CL188L_RequestQuerySwitchStatusReplayPacket();
                        try
                        {
                            recv.ParsePacket(ReFrameAry);
                            if (recv.ReciveResult == RecvResult.OK)
                            {
                                ReAry[0] = recv.SwitchStatus.ToString();
                                iRsValue = 0;
                            }
                            else
                            {
                                iRsValue = 1;
                            }
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "SetEquipInformation":
                    {
                        //设置设备信息  int SetEquipInformation(int id, byte MeterType, byte CTtype,out string[] FrameAry);
                        try
                        {
                            CL188L_RequestSetEquipInformationReplayPacket recv = new CL188L_RequestSetEquipInformationReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "ReadEquipInformation":
                    {
                        //读取设备信息 int ReadEquipInformation( int id, out int MeterIndex, out int MeterType, out int CTbyte,out string[] FrameAry);
                        try
                        {
                            CL188L_RequestReadEquipInformationReplayPacket recv = new CL188L_RequestReadEquipInformationReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            if (recv.ReciveResult == RecvResult.OK)
                            {
                                ReAry = new string[3];
                                ReAry[0] = recv.MeterNumber.ToString();
                                ReAry[1] = recv.MeterType.ToString();
                                ReAry[2] = recv.CtType.ToString();

                                iRsValue = 0;
                            }
                            else
                            {
                                iRsValue = 1;
                            }
                        }
                        catch (Exception)
                        {

                            return -1;
                        }

                    }
                    break;
                case "ReadPulseChannelAndType":
                    {
                        //读取脉冲通道和类型  int ReadPulseChannelAndType(int id,out int pram1,out int pram2,out  int pram3,out int pram4,out int pram5,out string[] FrameAry);
                        try
                        {
                            CL188L_RequestQueryPusleChannelAndCheckTypeReplayPacket recv = new CL188L_RequestQueryPusleChannelAndCheckTypeReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            if (recv.ReciveResult == RecvResult.OK)
                            {
                                ReAry = new string[6];
                                ReAry[0] = recv.wcChannelNo.ToString();//误差通道
                                ReAry[1] = recv.pulseType.ToString();//脉冲类型
                                ReAry[2] = recv.GyGy.ToString();//共阴 共阳
                                ReAry[3] = recv.verificationPusleType.ToString();//被检脉冲
                                ReAry[4] = recv.dgnWcChannelNo.ToString();//多功能误差通达
                                ReAry[5] = recv.checkType.ToString();//检定类型
                                iRsValue = 0;
                            }
                            else
                            {
                                iRsValue = 1;
                            }
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "SetSelectLightStatus":
                    {
                        //设置光电头状态 int SetSelectLightStatus(int id, byte selecttype, out string[] FrameAry);
                        try
                        {
                            CL188L_RequestSelectLightStatusReplayPacket recv = new CL188L_RequestSelectLightStatusReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "SetSelectCheckRoad":
                    {
                        //设置双回路命令         int SetSelectCheckRoad(int id, byte iroad, byte vroad, out string[] FrameAry);
                        try
                        {
                            CL188L_RequestSelectCheckRoadReplayPacket recv = new CL188L_RequestSelectCheckRoadReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "StartTest":
                    {
                        //开始检定试验        int StartTest(int id, byte verifyType, out string[] FrameAry);
                        try
                        {
                            CL188L_RequestStartPCFunctionReplayPacket recv = new CL188L_RequestStartPCFunctionReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "StopTest":
                    {
                        //停止检定试验        int StopTest(int id, byte verifyType, out string[] FrameAry);
                        try
                        {
                            CL188L_RequestStopPCFunctionReplayPacket recv = new CL188L_RequestStopPCFunctionReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "SetBwVolCutIsolation":
                    {
                        //设置表位电压电流隔离命令int SetBwVolCutIsolation(int id, int IsolationStatus, out string[] FrameAry);
                        //CL188L_RequestSeperateBwControlReplayPacket
                        try
                        {
                            CL188L_RequestSeperateBwControlReplayPacket recv = new CL188L_RequestSeperateBwControlReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {
                            return -1;
                        }
                    }
                    break;
                case "ReadBwVolCutsolation":
                    {
                        // 读取表位隔离状态       int ReadBwVolCutsolation(int id, out int IsolationStatus, out string[] FrameAry);
                        try
                        {
                            CL188L_RequestQuerySeperateBwControlReplayPacket recv = new CL188L_RequestQuerySeperateBwControlReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            if (recv.ReciveResult == RecvResult.OK)
                            {
                                ReAry[0] = recv.Seperate.ToString();
                                iRsValue = 0;
                            }
                            else
                            {
                                iRsValue = 1;
                            }
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "SetACTVRelay":
                    {
                        //设置耐压继电器状态        int SetACTVRelay(int id, byte highVoltageType, out string[] FrameAry);
                        try
                        {
                            CL188L_SendCMDToInsulationRelaysReplayPacket recv = new CL188L_SendCMDToInsulationRelaysReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "ReadACTVRelay":
                    {
                        //读取耐压继电器状态        int ReadACTVRelay(int id, out byte highVoltageType, out string[] FrameAry);
                        try
                        {
                            CL188L_RequestQueryCMDToInsulationRelaysRePlayPacket recv = new CL188L_RequestQueryCMDToInsulationRelaysRePlayPacket();
                            recv.ParsePacket(ReFrameAry);
                            if (recv.ReciveResult == RecvResult.OK)
                            {
                                ReAry[0] = recv.InsulationStatus.ToString();
                                iRsValue = 0;
                            }
                            else
                            {
                                iRsValue = 1;
                            }
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "SetCTPosition":
                    {
                        //CT 档位控制器         int SetCTPosition(int id, byte CtType, out string[] FrameAry);
                        try
                        {
                            CL188L_RequestCTPositionChannelControlReplayPacket recv = new CL188L_RequestCTPositionChannelControlReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "ReadCurrentData":
                    {
                        //查询误差板当前误差及其状态
                        //int ReadCurrentData(int id, byte wcbVerifyType, out byte verificationType,
                        //    out int MeterIndex,out int ErrorNum,out string wcData, out bool[] statusType,out byte CurrentContour,
                        //    out byte VoltageContour,out byte CommunicationType,out bool[] workType,out byte expandStatus, out string[] FrameAry);
                        try
                        {
                            CL188L_RequestReadBwWcAndStatusReplyPacket recv = new CL188L_RequestReadBwWcAndStatusReplyPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry = new string[23];
                            ReAry[0] = recv.CheckType.ToString();//检定类型
                            ReAry[1] = recv.CurBwNum.ToString();//表位号
                            ReAry[2] = recv.WcNum.ToString();
                            ReAry[3] = recv.WcData;
                            //接线故障状态
                            ReAry[4] = recv.StatusTypeIsOnErr_Jxgz.ToString();
                            //预付费状态
                            ReAry[5] = recv.StatusTypeIsOnErr_Yfftz.ToString();
                            //报警信号状态
                            ReAry[6] = recv.StatusTypeIsOnErr_Bjxh.ToString();
                            //对标状态
                            ReAry[7] = recv.StatusTypeIsOnOver_Db.ToString();
                            //温度过高状态
                            ReAry[8] = recv.StatusTypeIsOnErr_Temp.ToString();
                            //光电信号状态 有表还是没有表
                            ReAry[9] = recv.StatusTypeIsOn_HaveMeter.ToString();
                            //表位上限位状态
                            ReAry[10] = recv.StatusTypeIsOn_PressUpLimit.ToString();
                            //表位下限位状态
                            ReAry[11] = recv.StatusTypeIsOn_PressDownLimt.ToString();
                            ReAry[12] = recv.IType.ToString();
                            ReAry[13] = recv.VType.ToString();
                            ReAry[14] = Convert.ToByte(recv.ConnType).ToString();
                            //电能误差
                            ReAry[15] = recv.WorkStatusIsOn_Dn.ToString();
                            //需量周期
                            ReAry[16] = recv.WorkStatusIsOn_Xlzq.ToString();
                            //日计时
                            ReAry[17] = recv.WorkStatusIsOn_Rjs.ToString();
                            //多功能走字
                            ReAry[18] = recv.WorkStatusIsOn_Dnzz.ToString();
                            //对标
                            ReAry[19] = recv.WorkStatusIsOn_Db.ToString();
                            //预付费
                            ReAry[20] = recv.WorkStatusIsOn_Yff.ToString();
                            //耐压
                            ReAry[21] = recv.WorkStatusIsOn_Ny.ToString();
                            //多功能脉冲计数
                            ReAry[22] = recv.WorkStatusIsOn_Dgnmcjs.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "ReadFirstTenTimesData":
                    {
                        //读取误差板最新10次误差及其状态
                        //int ReadFirstTenTimesData(int id, byte wcbVerifyType, out byte verificationType,
                        //    out int MeterIndex, out int ErrorNum, out string[] wcDatas, out string wcData, out bool[] statusType, out byte CurrentContour,
                        //    out byte VoltageContour, out byte CommunicationType, out bool [] workType, out string[] FrameAry);
                        try
                        {
                            CL188L_RequestReadBwLast10WcAndStatusReplyPacket recv = new CL188L_RequestReadBwLast10WcAndStatusReplyPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry = new string[33];
                            ReAry[0] = recv.CheckType.ToString();
                            ReAry[1] = recv.CurBwNum.ToString();
                            ReAry[2] = recv.WcNum.ToString();
                            for (int i = 0; i < 10; i++)
                            {
                                ReAry[3 + i] = recv.WcData[i];
                            }
                            ReAry[13] = recv.WcData[recv.WcData.Length - 1];
                            //接线故障状态
                            ReAry[14] = recv.StatusTypeIsOnErr_Jxgz.ToString();
                            //预付费状态
                            ReAry[15] = recv.StatusTypeIsOnErr_Yfftz.ToString();
                            //报警信号状态
                            ReAry[16] = recv.StatusTypeIsOnErr_Bjxh.ToString();
                            //对标状态
                            ReAry[17] = recv.StatusTypeIsOnOver_Db.ToString();
                            //温度过高状态
                            ReAry[18] = recv.StatusTypeIsOnErr_Temp.ToString();
                            //光电信号状态 有表还是没有表
                            ReAry[19] = recv.StatusTypeIsOn_HaveMeter.ToString();
                            //表位上限位状态
                            ReAry[20] = recv.StatusTypeIsOn_PressUpLimit.ToString();
                            //表位下限位状态
                            ReAry[21] = recv.StatusTypeIsOn_PressDownLimt.ToString();
                            ReAry[22] = recv.CurrentRoadType.ToString();
                            ReAry[23] = recv.VoltageRoadType.ToString();
                            ReAry[24] = Convert.ToByte(recv.ConnType).ToString();
                            //电能误差
                            ReAry[25] = recv.WorkStatusIsOn_Dn.ToString();
                            //需量周期
                            ReAry[26] = recv.WorkStatusIsOn_Xlzq.ToString();
                            //日计时
                            ReAry[27] = recv.WorkStatusIsOn_Rjs.ToString();
                            //多功能走字
                            ReAry[28] = recv.WorkStatusIsOn_Dnzz.ToString();
                            //对标
                            ReAry[29] = recv.WorkStatusIsOn_Db.ToString();
                            //预付费
                            ReAry[30] = recv.WorkStatusIsOn_Yff.ToString();
                            //耐压
                            ReAry[31] = recv.WorkStatusIsOn_Ny.ToString();
                            //多功能脉冲计数
                            ReAry[32] = recv.WorkStatusIsOn_Dgnmcjs.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "ReSetStatus":
                    {
                        //清除表位状态        int ReSetStatus(int id,byte statusType, out string[] FrameAry);
                        try
                        {
                            CL188L_RequestClearBwStatusReplayPacket recv = new CL188L_RequestClearBwStatusReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "ReadVersion":
                    {
                        //读取软件版本号         int ReadVersion(int id,out int MeterIndex, out string strVersion,out string [] FrameAry);
                        try
                        {
                            CL188L_RequestReadWcBoardVerReplyPacket recv = new CL188L_RequestReadWcBoardVerReplyPacket();
                            recv.ParsePacket(ReFrameAry);
                            if (recv.ReciveResult == RecvResult.OK)
                            {
                                ReAry[0] = recv.SoftVer;
                                iRsValue = 0;
                            }
                            else
                            {
                                iRsValue = 1;
                            }
                        }
                        catch (Exception)
                        {

                            return -1;
                        }

                    }
                    break;
                case "ReadPrintInformation":
                    {
                        //读取打印信息         int ReadPrintInformation(int id,int modid,out byte dataSerial,out byte dataLen,out byte []dataAry, out string[] FrameAry);
                        CL188L_RequestReadPrintInformationReplayPacket recv = new CL188L_RequestReadPrintInformationReplayPacket();
                        try
                        {
                            recv.ParsePacket(ReFrameAry);
                            if (recv.ReciveResult == RecvResult.OK)
                            {
                                ReAry = new string[3];
                                ReAry[0] = recv.Serial.ToString();
                                ReAry[1] = recv.DataNum.ToString();
                                ReAry[2] = BytesToString(recv.listData.ToArray());
                                iRsValue = 0;
                            }
                            else
                            {
                                iRsValue = 1;
                            }
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "ReadIsolationrelayOverloadReliabilityTime":
                    {
                        //读取隔离继电器过载动作可靠性检测时间         int ReadIsolationrelayOverloadReliabilityTime(int id, out int meterIndex, out int detectionTime, out string[] FrameAry);
                        try
                        {
                            CL188L_RequestIsolationrelayOverloadReliabilityReplayPacket recv = new CL188L_RequestIsolationrelayOverloadReliabilityReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            if (recv.ReciveResult == RecvResult.OK)
                            {
                                ReAry = new string[2];
                                ReAry[0] = recv.MeterNumber.ToString();
                                ReAry[1] = recv.detectionTime.ToString();
                                iRsValue = 0;
                            }
                            else
                            {
                                iRsValue = 1;
                            }
                        }
                        catch (Exception)
                        {
                            return -1;
                        }
                    }
                    break;
                case "SetIsolationrelayOverloadReliabilityTime":
                    {
                        //设置隔离继电器过载动作可靠性检测时间         int SetIsolationrelayOverloadReliabilityTime(int id, byte Time, out string[] FrameAry);
                        try
                        {
                            CL188L_RequestIsolationrelayOverloadReliabilityTimeReplayPacket recv = new CL188L_RequestIsolationrelayOverloadReliabilityTimeReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {
                            return -1;
                        }
                    }
                    break;
                case "SetEnergePulseParams":
                    {
                        //设置电能误差检定时脉冲参数        int SetEnergePulseParams(int id,int stdMeterConst, int stdPulseFreq, int stdMeterConstShorttime, int meterConst, int circles, int meterConstZooms,out string []FrameAry);
                        try
                        {
                            CL188L_RequestSetPulseParaReplayPacket recv = new CL188L_RequestSetPulseParaReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {
                            return -1;
                        }
                    }
                    break;
                case "ReadEnergePulseParams":
                    {
                        //读取电能差脉冲参数        int ReadEnergePulseParams(int id, out int stdMeterConst, out int stdMeterConstShorttime, out  int meterConst,out  int circles, out int meterConstZooms, out string[] FrameAry);
                        try
                        {
                            CL188L_RequestQueryPulseParamReplayPacket recv = new CL188L_RequestQueryPulseParamReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            if (recv.ReciveResult == RecvResult.OK)
                            {
                                ReAry = new string[5];
                                ReAry[0] = recv.stdMeterConst.ToString();
                                ReAry[1] = recv.stdMeterConstShortTime.ToString();
                                ReAry[2] = recv.meterConst.ToString();
                                ReAry[3] = recv.meterQuans.ToString();
                                ReAry[4] = recv.meterConstShortTime.ToString();
                                iRsValue = 0;
                            }
                            else
                            {
                                iRsValue = 1;
                            }
                        }
                        catch (Exception)
                        {
                            return -1;
                        }

                    }
                    break;
                case "SetClockFrequency":
                    {
                        //设置日计时检定时钟频率及需量周期检定时间         int SetClockFrequency(int id, int stdMeterTimeFreq, int meterTimeFreq, int meterPulseNum, out string[] FrameAry);
                        try
                        {
                            CL188L_RequestSetTimePluseNumAndXLTimeReplayPacket recv = new CL188L_RequestSetTimePluseNumAndXLTimeReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {
                            return -1;
                        }
                    }
                    break;
                case "ReadClockFrequency":
                    {
                        //读取日计时检定时钟频率及需量周期检定时间 int ReadClockFrequency(int id, out int stdMeterTimeFreq, out int meterTimeFreq, out int meterPulseNum, out string[] FrameAry);
                        try
                        {
                            CL188L_RequestQueryTimePusleNumAndXLTimeReplayPacket recv = new CL188L_RequestQueryTimePusleNumAndXLTimeReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry = new string[3];
                            ReAry[0] = recv.stdMeterTimeFreq.ToString();
                            ReAry[1] = recv.meterTimeFreq.ToString();
                            ReAry[2] = recv.meterPulseNum.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {
                            return -1;
                        }
                    }
                    break;
                case "SetACTVLeakCurrentThresholdValue":
                    {
                        //设置耐压试验漏电流阀值         int SetACTVLeakCurrentThresholdValue(int id,int iCurrentLimit, out string[] FrameAry);
                        try
                        {
                            CL188L_RequestSetWishStandCurrentLimitReplayPacket recv = new CL188L_RequestSetWishStandCurrentLimitReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;
                case "ReadACTVLeakCurrentThresholdValue":
                    {
                        //读取漏电流阀值        int ReadACTVLeakCurrentThresholdValue(int id, out int iCurrentLimit, out string[] FrameAry);
                        try
                        {
                            CL188L_RequestQueryWishStandCurrentLimitReplayPacket recv = new CL188L_RequestQueryWishStandCurrentLimitReplayPacket();
                            recv.ParsePacket(ReFrameAry);
                            if (recv.ReciveResult == RecvResult.OK)
                            {
                                ReAry = new string[2];
                                ReAry[0] = recv.meterNumber.ToString();
                                ReAry[1] = recv.CurrentLimit.ToString();
                                iRsValue = 0;
                            }
                            else
                            {
                                iRsValue = 1;
                            }
                        }
                        catch (Exception)
                        {
                            return -1;
                        }
                    }
                    break;
                case "ControlMotor":
                    {
                        //设置电机模式         int ControlMotor(int id, byte electricalMachineMode,out string []FrameAry);
                        try
                        {
                            CL188L_RequestSetElectromotorReplayPacket recv = new CL188L_RequestSetElectromotorReplayPacket();
                            recv.ParsePacket(ReFrameAry);

                            ReAry[0] = recv.ReciveResult.ToString();
                            iRsValue = 0;
                        }
                        catch (Exception)
                        {

                            return -1;
                        }
                    }
                    break;

                default:
                    break;
            }
            return iRsValue;
        }
    }
}
