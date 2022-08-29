using E_CL321.Device;
using E_CLSocketModule;
using E_CLSocketModule.Enum;
using E_CLSocketModule.SocketModule.Packet;
using E_CLSocketModule.Struct;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace E_CL321
{
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
    ComVisible(true)]
    public interface IClass_Interface
    {
        ///// <summary>
        ///// 初始化2018端口
        ///// </summary>
        ///// <param name="ComNumber"></param>
        ///// <param name="MaxWaitTme"></param>
        ///// <param name="WaitSencondsPerByte"></param>
        ///// <param name="IP"></param>
        ///// <param name="RemotePort"></param>
        ///// <param name="LocalStartPort"></param>
        ///// <returns></returns>
        //[DispId(1)]
        //int InitSetting(int ComNumber, int MaxWaitTme, int WaitSencondsPerByte, string IP, int RemotePort, int LocalStartPort, bool bool_HaveProtocol);
        /// <summary>
        /// 初始化串口
        /// </summary>
        /// <param name="ComNumber"></param>
        /// <param name="MaxWaitTime"></param>
        /// <param name="WaitSencondsPerByte"></param>
        /// <returns></returns>
        [DispId(2)]
        int InitSettingCom(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte);
        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        [DispId(3)]
        int Connect(int Id, out string[] FrameAry);
        ///// <summary>
        ///// 断开连接
        ///// </summary>
        ///// <param name="FrameAry"></param>
        ///// <returns></returns>
        //[DispId(4)]
        //int DisConnect(out string[] FrameAry);
        ///// <summary>
        ///// 设置发送标志
        ///// </summary>
        ///// <param name="Flag"></param>
        ///// <returns></returns>
        //[DispId(5)]
        //int SetSendFlag(bool Flag);
        ///// <summary>
        ///// 设置脉冲通道和类型
        ///// </summary>
        ///// <param name="Id"></param>
        ///// <param name="pram1"></param>
        ///// <param name="pram2"></param>
        ///// <param name="pram3"></param>
        ///// <param name="pram4"></param>
        ///// <param name="pram5"></param>
        ///// <param name="FrameAry"></param>
        ///// <returns></returns>
        //[DispId(6)]
        //int SetPulseChannelAndType(int Id, int pram1, int pram2, int pram3, int pram4, int pram5,out string[] FrameAry);
        ///// <summary>
        ///// 升级登录指令
        ///// </summary>
        ///// <returns></returns>
        //[DispId(7)]
        //int UpdateLogin(int id, out string[] FrameAry);
        ///// <summary>
        ///// 重启误差板
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="FrameAry"></param>
        ///// <returns></returns>
        //[DispId(8)]
        //int ReBoot(int id, out string[] FrameAry);
        ///// <summary>
        ///// 升级数据
        ///// </summary>
        ///// <param name="DataSerial">数据序号</param>
        ///// <param name="bytesData">要升级的数据</param>
        ///// <param name="FrameAry">合成升级数据的报文</param>
        ///// <returns></returns>
        //[DispId(9)]
        //int UpdateFirmware(int id, UInt16 DataSerial, byte[] bytesData, out string[] FrameAry);
        ///// <summary>
        ///// 升级登录2
        ///// </summary>
        ///// <param name="FrameAry"></param>
        ///// <returns></returns>
        //[DispId(10)]
        //int UpdateLogin2(int id, out string[] FrameAry);
        ///// <summary>
        ///// 重启设备2
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="FrameAry"></param>
        ///// <returns></returns>
        //[DispId(11)]
        //int ReBoot2(int id, out string[] FrameAry);
        ///// <summary>
        ///// 升级数据2
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="DataSerial">数据序号</param>
        ///// <param name="bytesData">要升级的数据</param>
        ///// <param name="FrameAry">输出报文</param>
        ///// <returns></returns>
        //[DispId(12)]
        //int UpdateFirmware2(int id, UInt16 DataSerial, byte[] bytesData, out string[] FrameAry);
        ///// <summary>
        ///// 读取版本2
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="Version">版本号</param>
        ///// <param name="FrameAry">输出报文</param>
        ///// <returns></returns>
        //[DispId(13)]
        //int ReadVersion2(int id, out string Version, out string[] FrameAry);
        ///// <summary>
        ///// 读取功耗参数
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="AU_Ia_or_I">三相A相电压回路电流值|单相电压回路电流值</param>
        ///// <param name="BU_Ib_or_L1_U">三相B相电压回路电流值|电流1回路电压值</param>
        ///// <param name="CU_Ic_or_L2_U">三相C相电压回路电流值|电流2回路电压值</param>
        ///// <param name="AI_Ua">三相A相电流回路电压值|单相保留</param>
        ///// <param name="BI_Ub">三相B相电流回路电压值|单相保留</param>
        ///// <param name="CI_Uc">三相C相电流回路电压值|单相保留</param>
        ///// <param name="AU_Phia_or_Phi">三相A相电压回路相位角|单相电压回路相位角</param>
        ///// <param name="BU_Phib">三相B相电压回路相位角|单相保留</param>
        ///// <param name="CU_Phic">三相C相电压回路相位角|单相保留</param>
        ///// <param name="FrameAry">输出报文</param>
        ///// <returns></returns>
        //[DispId(14)]
        //int ReadPowerParams(int id, out float AU_Ia_or_I, out float BU_Ib_or_L1_U, out float CU_Ic_or_L2_U,
        //                                         out float AI_Ua, out float BI_Ub, out float CI_Uc,
        //                                         out float AU_Phia_or_Phi, out float BU_Phib, out float CU_Phic,
        //                                         out string[] FrameAry);
        ///// <summary>
        ///// 启动要信输出
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="YXTestNo">遥信路数</param>
        ///// <param name="YxTestType">遥信输出方式</param>
        ///// <param name="YxTestPulseNum">脉冲个数</param>
        ///// <param name="yxTestPulseOutHz">脉冲输出频率</param>
        ///// <param name="yxTestOutmultiple">输出占空比的1000000倍（4Bytes</param>
        ///// <param name="FrameAry">输出报文</param>
        ///// <returns></returns>
        //[DispId(15)]
        //int StartRemoteSignals(int id, int p_int_RemoteCount, int p_int_OutputType, int p_int_PulseCount, float p_flt_PulseOutHz, float p_flt_OutMultiple, out string[] p_str_OutFrame);
        ///// <summary>
        ///// 启动直流模拟量输出
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="Current">电流</param>
        ///// <param name="FrameAry">输出报文</param>
        ///// <returns></returns>
        //[DispId(16)]
        //int StartDCAnalog(int id, int Current, out string[] FrameAry);
        ///// <summary>
        ///// 停止遥信、或直流模拟量输出
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="checkType">0：遥信实验1：直流模拟量采集实验</param>
        ///// <param name="chennNo">表示第几路遥信,该字节只有当实验类型是遥信实验时候为有效值,其他值：无效参数</param>
        ///// <param name="FrameAry">输出报文</param>
        ///// <returns></returns>
        //[DispId(17)]
        //int StopOutPut(int id, int p_int_CheckType, int p_int_RemoteCount, out string[] p_str_OutFrame);
        ///// <summary>
        ///// 读取遥控信号
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="YkCount">遥控个数</param>
        ///// <param name="PusleCount">脉冲个数</param>
        ///// <param name="FrameAry">输出报文</param>
        ///// <returns></returns>
        //[DispId(18)]
        //int ReadSignals(int id, int YkCount, out int PusleCount, out string[] FrameAry);
        ///// <summary>
        ///// 设置跳闸继电器类型
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="SwitchCommand">0 停止跳闸|1启动跳闸试验</param>
        ///// <param name="FrameAry">输出报文</param>
        ///// <returns></returns>
        //[DispId(19)]
        //int SetTripRelayType(int id, byte SwitchCommand, out string[] FrameAry);
        ///// <summary>
        ///// 负控继电器控制命令
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="bearRelayType">负控继电器状态</param>
        ///// <param name="FrameAry">输出报文</param>
        ///// <returns></returns>
        //[DispId(20)]
        //int SetSecondaryRelayStatus(int id, byte bearRelayStatus, out string[] FrameAry);
        ///// <summary>
        ///// 直流输出校准
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="FrameAry">输出报文</param>
        ///// <returns></returns>
        //[DispId(21)]
        //int DirectCurrentOutPutCorrect(int id, out string[] FrameAry);
        ///// <summary>
        ///// 直流输出实际电流校准
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="uiData">电流值</param>
        ///// <param name="correctParams">输出校准参数</param>
        ///// <param name="FrameAry">输出报文</param>
        ///// <returns></returns>
        //[DispId(22)]
        //int DirectCurrentOutPutRealityValueCorrect(int id, UInt16 uiData, out short correctParams, out string[] FrameAry);
        ///// <summary>
        ///// 设置互斥电机和速度
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="UpOrDown">电机向上或向下</param>
        ///// <param name="Option">递增或者递减</param>
        ///// <param name="CalTime">延时时间</param>
        ///// <returns></returns>
        //[DispId(23)]
        //int SetMotorMutexAndSpeed(int id, int UpOrDown, int Option, int CalTime, out string[] FrameAry);

        ///// <summary>
        ///// 读取电机速度
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="UpDelaytime">上限为延时时间</param>
        ///// <param name="DownDelaytime">下限位延时时间</param>
        ///// <param name="FrameAry"></param>
        ///// <returns></returns>
        //[DispId(24)]
        //int ReadMotorSpeed(int id, out int UpDelaytime, out int DownDelaytime, out string[] FrameAry);
        ///// <summary>
        ///// 读取温度
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="readType">0=A,1=B,2=C</param>
        ///// <param name="Temperature">温度</param>
        ///// <param name="FrameAry">输出参数</param>
        ///// <returns></returns>
        //[DispId(25)]
        //int ReadTConnector(int id, int readType, out string[] Temperature, out string[] FrameAry);
        ///// <summary>
        ///// 设置继电器类型
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="RelayID">继电器ID</param>
        ///// <param name="ControlType">继电器类型0，断开继电器|1，闭合继电器</param>
        ///// <param name="FrameAry"></param>
        ///// <returns></returns>
        //[DispId(26)]
        //int SetSwitchType(int id, byte RelayID, byte ControlType, out string[] FrameAry);
        ///// <summary>
        ///// 读取继电器类型
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="RelayID">继电器ID</param>
        ///// <param name="ContrlType">继电器控制类型 0,断开继电器|1，闭合继电器</param>
        ///// <param name="FrameAry"></param>
        ///// <returns></returns>
        //[DispId(27)]
        //int ReadRelayStatus(int id, byte RelayID, out byte ContrlType, out string[] FrameAry);
        ///// <summary>
        ///// 设置设备信息
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="MeterType">电能表类型</param>
        ///// <param name="CTtype">CT类型</param>
        ///// <param name="FrameAry"></param>
        ///// <returns></returns>
        //[DispId(28)]
        //int SetEquipInformation(int id, byte MeterType, byte CTtype, out string[] FrameAry);
        ///// <summary>
        ///// 读取设备信息
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="MeterIndex">表位号</param>
        ///// <param name="MeterType">电表类型</param>
        ///// <param name="CTbyte">CT类型</param>
        ///// <param name="FrameAry">输出报文</param>
        ///// <returns></returns>
        //[DispId(29)]
        //int ReadEquipInformation(int id, out int MeterIndex, out int MeterType, out int CTbyte, out string[] FrameAry);
        ///// <summary>
        ///// 读取脉冲通道和类型
        ///// </summary>
        ///// <param name="Id">误差板编号</param>
        ///// <param name="pram1"></param>
        ///// <param name="pram2"></param>
        ///// <param name="pram3"></param>
        ///// <param name="pram4"></param>
        ///// <param name="pram5"></param>
        ///// <param name="FrameAry"></param>
        ///// <returns></returns>
        //[DispId(30)]
        //int ReadPulseChannelAndType(int id, out int pram1, out int pram2, out  int pram3, out int pram4, out int pram5, out string[] FrameAry);
        ///// <summary>
        ///// 设置光电头状态
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="selecttype">选择类型</param>
        ///// <param name="FrameAry">输出报文</param>
        ///// <returns></returns>
        //[DispId(31)]
        //int SetSelectLightStatus(int id, byte p_byt_SelectType, out string[] p_str_OutFrame);
        ///// <summary>
        ///// 设置双回路命令
        ///// </summary>
        ///// <param name="id">表位状态</param>
        ///// <param name="iroad">电流回路</param>
        ///// <param name="vroad">电压回路</param>
        ///// <param name="FrameAry">出参报文</param>
        ///// <returns></returns>
        //[DispId(32)]
        //int SetSelectCheckRoad(int id, byte iroad, byte vroad, out string[] FrameAry);
        ///// <summary>
        ///// 开始检定试验
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="verifyType">检定类型</param>
        ///// <param name="FrameAry"></param>
        ///// <returns></returns>
        //[DispId(33)]
        //int StartTest(int id, byte verifyType, out string[] FrameAry);

        ///// <summary>
        ///// 停止检定试验
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="verifyType">检定类型</param>
        ///// <param name="FrameAry">出参</param>
        ///// <returns></returns>
        //[DispId(34)]
        //int StopTest(int id, byte verifyType, out string[] FrameAry);

        ///// <summary>
        ///// 设置表位电压电流隔离命令
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="IsolationStatus">隔离状态</param>
        ///// <param name="FrameAry">出参帧</param>
        ///// <returns></returns>
        //[DispId(35)]
        //int SetBwVolCutIsolation(int id, int p_int_IsolationStatus, out string[] p_str_OutFrame);
        ///// <summary>
        ///// 读取表位隔离状态
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="IsolationStatus">隔离状态</param>
        ///// <param name="FrameAry">出参帧</param>
        ///// <returns></returns>
        //[DispId(36)]
        //int ReadBwVolCutsolation(int id, out int IsolationStatus, out string[] FrameAry);
        ///// <summary>
        ///// 设置耐压继电器状态
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="highVoltageType">耐压继电器状态</param>
        ///// <param name="FrameAry">出参帧</param>
        ///// <returns></returns>
        //[DispId(37)]
        //int SetACTVRelay(int id, byte highVoltageType, out string[] FrameAry);
        ///// <summary>
        ///// 读取耐压继电器状态
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="highVoltageType">输出耐压状态</param>
        ///// <param name="FrameAry">出参帧</param>
        ///// <returns></returns>
        //[DispId(38)]
        //int ReadACTVRelay(int id, out byte highVoltageType, out string[] FrameAry);
        ///// <summary>
        ///// CT 档位控制器
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="CtType">CT类型</param>
        ///// <param name="FrameAry">出参帧</param>
        ///// <returns></returns>
        //[DispId(39)]
        //int SetCTPosition(int id, byte CtType, out string[] FrameAry);
        ///// <summary>
        ///// 查询误差板当前误差及其状态
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="wcbVerifyType">检定类型</param>
        ///// <param name="verificationType">输出误差板检定类型</param>
        ///// <param name="MeterIndex">表位号</param>
        ///// <param name="ErrorNum">误差次数</param>
        ///// <param name="wcData">误差值</param>
        ///// <param name="statusType">状态</param>
        ///// <param name="CurrentContour"></param>
        ///// <param name="VoltageContour"></param>
        ///// <param name="CommunicationType"></param>
        ///// <param name="workType"></param>
        ///// <param name="expandStatus"></param>
        ///// <param name="FrameAry"></param>
        ///// <returns></returns>
        //[DispId(40)]
        //int ReadCurrentData(int id, byte wcbVerifyType, out byte verificationType,
        //    out int MeterIndex, out int ErrorNum, out string wcData, out bool[] statusType, out byte CurrentContour,
        //    out byte VoltageContour, out byte CommunicationType, out bool[] workType, out byte expandStatus, out string[] FrameAry);
        ///// <summary>
        ///// 读取误差板最新10次误差及其状态
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="verifyType">检定类型</param>
        ///// <param name="verificationType"></param>
        ///// <param name="MeterIndex"></param>
        ///// <param name="Errorcount"></param>
        ///// <param name="wcDatas"></param>
        ///// <param name="wcData"></param>
        ///// <param name="statusType"></param>
        ///// <param name="CurrentContour"></param>
        ///// <param name="VoltageContour"></param>
        ///// <param name="CommunicationType"></param>
        ///// <param name="workType"></param>
        ///// <param name="FrameAry"></param>
        ///// <returns></returns>
        //[DispId(41)]
        //int ReadFirstTenTimesData(int id, byte wcbVerifyType, out byte verificationType,
        //    out int MeterIndex, out int ErrorNum, out string[] wcDatas, out string wcData, out bool[] statusType, out byte CurrentContour,
        //    out byte VoltageContour, out byte CommunicationType, out bool[] workType, out string[] FrameAry);
        ///// <summary>
        ///// 清除表位状态
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="statusType"></param>
        ///// <param name="FrameAry"></param>
        ///// <returns></returns>
        //[DispId(42)]
        //int ReSetStatus(int id, byte statusType, out string[] FrameAry);
        ///// <summary>
        ///// 读取软件版本号
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="MeterIndex">表位号</param>
        ///// <param name="strVersion">版本号</param>
        ///// <param name="FrameAry">出参帧</param>
        ///// <returns></returns>
        //[DispId(43)]
        //int ReadVersion(int id, out int MeterIndex, out string strVersion, out string[] FrameAry);
        ///// <summary>
        ///// 读取打印信息
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="FrameAry">输出参数</param>
        ///// <returns></returns>
        //[DispId(44)]
        //int ReadPrintInformation(int id, int modid, out byte dataSerial, out byte dataLen, out byte[] dataAry, out string[] FrameAry);
        ///// <summary>
        ///// 读取隔离继电器过载动作可靠性检测时间
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="FrameAry">输出参数</param>
        ///// <returns></returns>
        //[DispId(45)]
        //int ReadIsolationrelayOverloadReliabilityTime(int id, out int meterIndex, out int detectionTime, out string[] FrameAry);
        ///// <summary>
        ///// 设置隔离继电器过载动作可靠性检测时间
        ///// </summary>
        ///// <param name="id">表位号</param>
        ///// <param name="Time">时间</param>
        ///// <param name="FrameAry">出参帧</param>
        ///// <returns></returns>
        //[DispId(46)]
        //int SetIsolationrelayOverloadReliabilityTime(int id, byte Time, out string[] FrameAry);

        ///// <summary>
        ///// 设置电能误差检定时脉冲参数
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="stdMeterConst">标准表脉冲常数</param>
        ///// <param name="stdPulseFreq">标准脉冲频率</param>
        ///// <param name="stdMeterConstShorttime">标准脉冲缩放倍数</param>
        ///// <param name="meterConst">被检表脉冲常数</param>
        ///// <param name="circles">圈数</param>
        ///// <param name="meterConstZooms">被检脉冲缩放倍数</param>
        ///// <param name="FrameAry"></param>
        ///// <returns></returns>
        //[DispId(47)]
        //int SetEnergePulseParams(int id, int stdMeterConst, int stdPulseFreq, int stdMeterConstShorttime, int meterConst, int circles, int meterConstZooms, out string[] FrameAry);

        ///// <summary>
        ///// 设置参数
        ///// </summary>
        ///// <param name="bwstatus">表位状态</param>
        ///// <param name="stdmeterconst">标准脉冲常数</param>
        ///// <param name="stdpulsefreq">标准脉冲频率</param>
        ///// <param name="stdmeterconstshorttime">标准脉冲常数缩放倍数</param>
        ///// <param name="meterconst">被检脉冲常数</param>
        ///// <param name="meterquans">校验圈数</param>
        ///// <param name="meterconstshorttime">被检脉冲常数缩放倍数</param>
        //[DispId(49)]
        //int SetEnergePulseParams(bool[] bwstatus, int stdMeterConst, int stdPulseFreq, int stdMeterConstShorttime, int meterConst, int circles, int meterConstZooms, out string[] FrameAry, int iChannelNum, int iChannelNo);

        ///// <summary>
        /////  读取电能差脉冲参数
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="stdMeterConst">标准表常数</param>
        ///// <param name="stdMeterConstShorttime">标准表脉冲常数缩放倍数</param>
        ///// <param name="meterConst">被检表脉冲常数</param>
        ///// <param name="circles">校验圈数</param>
        ///// <param name="meterConstZooms">被检脉冲常数缩放倍数</param>
        ///// <param name="FrameAry">出参帧</param>
        ///// <returns></returns>
        //[DispId(50)]
        //int ReadEnergePulseParams(int id, out int stdMeterConst, out int stdMeterConstShorttime, out  int meterConst, out  int circles, out int meterConstZooms, out string[] FrameAry);
        ///// <summary>
        ///// 设置日计时检定时钟频率及需量周期检定时间
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="stdMeterTimeFreq">标准时钟频率</param>
        ///// <param name="meterTimeFreq">需量时间周期</param>
        ///// <param name="meterPulseNum">被检脉冲个数</param>
        ///// <param name="FrameAry">出参帧</param>
        ///// <returns></returns>
        //[DispId(51)]
        //int SetClockFrequency(int id, int stdMeterTimeFreq, int meterTimeFreq, int meterPulseNum, out string[] FrameAry);
        ///// <summary>
        ///// 读取日计时检定时钟频率及需量周期检定时间
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="stdMeterTimeFreq">标准时钟频率</param>
        ///// <param name="meterTimeFreq">被检表时钟频率</param>
        ///// <param name="meterPulseNum">被检表脉冲数</param>
        ///// <param name="FrameAry">输出参数</param>
        ///// <returns></returns>
        //[DispId(52)]
        //int ReadClockFrequency(int id, out int stdMeterTimeFreq, out int meterTimeFreq, out int meterPulseNum, out string[] FrameAry);
        ///// <summary>
        ///// 设置耐压试验漏电流阀值
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="iCurrentLimit">电流阀值</param>
        ///// <param name="FrameAry">输出参数</param>
        ///// <returns></returns>
        //[DispId(53)]
        //int SetACTVLeakCurrentThresholdValue(int id, int iCurrentLimit, out string[] FrameAry);
        ///// <summary>
        ///// 读取漏电流阀值
        ///// </summary>
        ///// <param name="id">表位号</param>
        ///// <param name="iCurrentLimit">漏电流阀值</param>
        ///// <param name="FrameAry">输出参数</param>
        ///// <returns></returns>
        //[DispId(54)]
        //int ReadACTVLeakCurrentThresholdValue(int id, out int iCurrentLimit, out string[] FrameAry);
        ///// <summary>
        ///// 设置电机模式
        ///// </summary>
        ///// <param name="id">误差板编号</param>
        ///// <param name="electricalMachineMode">电机控制方式</param>
        ///// <returns></returns>
        //[DispId(55)]
        //int ControlMotor(bool[] p_bol_Status, int p_int_MachineMode, int p_int_ChannelNo, int p_int_TotalChannelNo, out string[] p_str_OutFrame);
        /// <summary>
        /// 解析下行报文
        /// </summary>
        /// <param name="MothedName">解析报文的方法</param>
        /// <param name="ReFrameAry">报文帧</param>
        /// <param name="ReAry">返回解析数据</param>
        /// <returns></returns>
        //[DispId(56)]
        //int UnPacket(string MothedName, byte[] ReFrameAry, out string[] ReAry);

    }

    [Guid("73DCFB98-6D51-40D1-AFF2-E8DC3C83CF1F"),
    ProgId("CLOU.E_CL321"),
    ClassInterface(ClassInterfaceType.None),
    ComDefaultInterface(typeof(IClass_Interface)),
    ComVisible(true)]
    public class CL321 : IClass_Interface
    {
        public int int_Id = 0;

        /// <summary>
        /// 重试次数
        /// </summary>
        public static int RETRYTIEMS = 1;
        /// <summary>
        /// 源控制端口
        /// </summary>
        private StPortInfo m_Port = null;

        DriverBase driverBase = null;
        /// <summary>
        /// 发送标志
        /// </summary>
        private bool SendFlag = true;

        public CL321()
        {
            m_Port = new StPortInfo();
            driverBase = new DriverBase();
        }

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
        public int InitSetting(int ComNumber, int MaxWaitTime, int WaitSencondsPerByte, string IP, int RemotePort, int LocalStartPort, string bool_HaveProtocol)
        {
            m_Port = new StPortInfo();
            m_Port.m_Exist = 1;
            m_Port.m_IP = IP;
            m_Port.m_Port = ComNumber;
            m_Port.m_Port_Type = Cus_EmComType.UDP;
            m_Port.m_Port_Setting = "19200,n,8,1";
            try
            {
                driverBase.RegisterPort(ComNumber, m_Port.m_Port_Setting, m_Port.m_IP, RemotePort, LocalStartPort, bool_HaveProtocol, MaxWaitTime, WaitSencondsPerByte);
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
            m_Port.m_Exist = 1;
            m_Port.m_IP = "";
            m_Port.m_Port = ComNumber;
            m_Port.m_Port_Type = Cus_EmComType.COM;
            m_Port.m_Port_Setting = "19200,n,8,1";
            try
            {
                driverBase.RegisterPort(ComNumber, "19200,n,8,1", MaxWaitTime, WaitSencondsPerByte);
            }
            catch (Exception)
            {
                return -1;
            }

            return 0;
        }
        /// <summary>
        /// 连接,点亮485
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int Connect(int Id, out string[] FrameAry)
        {
            FrameAry = new string[1];
            CL321_RequestWriteDataPacket rc2 = new CL321_RequestWriteDataPacket();

            CL321_RequestWriteDataReplayPacket recv2 = new CL321_RequestWriteDataReplayPacket();
            try
            {
                rc2.Pos = Id;
                rc2.Str_AddressFlag = "1203";
                rc2.Str_Data = "01";
                FrameAry[0] = BytesToString(rc2.GetPacketData());
                //FrameAry[0] = (rc2.GetWrite3201(Id, "1203", "01"));
                int intRst = 1;
                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rc2, recv2))
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
        /// 启动脉冲输出
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

            CL321_RequestWriteDataPacket rc = new CL321_RequestWriteDataPacket();
            rc.Pos = Id;
            CL321_RequestWriteDataReplayPacket recv = new CL321_RequestWriteDataReplayPacket();

            try
            {
                rc.Str_AddressFlag = "1208";

                ByteBuffer buf = new ByteBuffer();
                buf.Initialize();
                buf.PutInt((int)(p_flt_PulseOutHz * 100));
                buf.Put(50);
                buf.PutInt(p_int_PulseCount);

                rc.Str_Data = BytesToString(buf.ToByteArray());
                p_str_OutFrame[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }
                Thread.Sleep(20);
                rc = new CL321_RequestWriteDataPacket();
                rc.Pos = Id;
                recv = new CL321_RequestWriteDataReplayPacket();
                rc.Str_AddressFlag = "120A";

                buf = new ByteBuffer();
                buf.Initialize();
                buf.PutInt((int)(p_flt_PulseOutHz * 100));
                buf.Put(50);
                buf.PutInt(p_int_PulseCount);

                rc.Str_Data = BytesToString(buf.ToByteArray());
                p_str_OutFrame[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }
                Thread.Sleep(20);
            }
            catch (Exception)
            {

                return -1;
            }

            CL321_RequestStartFunPacket rcFun = new CL321_RequestStartFunPacket();
            rcFun.Pos = Id;
            CL321_RequestStartFunReplayPacket recvFun = new CL321_RequestStartFunReplayPacket();

            try
            {
                rcFun.Str_FunFlag = "0450";
                p_str_OutFrame[0] = BytesToString(rcFun.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rcFun, recvFun))
                    {
                        intRst = recvFun.ReciveResult == RecvResult.OK ? 0 : 2;
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
        /// 设置脉冲输出
        /// </summary>
        /// <param name="Id">表位号</param>
        /// <param name="p_int_RemoteCount">脉冲路数</param>
        /// <param name="p_int_PulseCount">脉冲个数</param>
        /// <param name="p_flt_PulseOutHz">频率</param>
        /// <param name="p_flt_OutMultiple">占空比</param>
        /// <param name="p_str_OutFrame"></param>
        /// <returns></returns>
        public int SetPulse(int Id, int p_int_RemoteCount, int p_int_PulseCount, float p_flt_PulseOutHz, float p_flt_OutMultiple, out string[] p_str_OutFrame)
        {
            p_str_OutFrame = new string[1];
            int intRst = 1;

            CL321_RequestWriteDataPacket rc = new CL321_RequestWriteDataPacket();
            rc.Pos = Id;
            CL321_RequestWriteDataReplayPacket recv = new CL321_RequestWriteDataReplayPacket();

            try
            {
                if (p_int_RemoteCount == 5)
                    rc.Str_AddressFlag = "1208";
                else
                    rc.Str_AddressFlag = "120A";

                ByteBuffer buf = new ByteBuffer();
                buf.Initialize();
                buf.PutInt((int)(p_flt_PulseOutHz * 100));
                buf.Put(50);
                buf.PutInt(p_int_PulseCount);

                rc.Str_Data = BytesToString(buf.ToByteArray());
                p_str_OutFrame[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rc, recv))
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
        /// 启动脉冲输出
        /// </summary>
        /// <param name="p_str_OutFrame"></param>
        /// <returns></returns>
        public int StartPluse(int Id, out string[] p_str_OutFrame)
        {
            p_str_OutFrame = new string[1];
            int intRst = 1;
            CL321_RequestStartFunPacket rcFun = new CL321_RequestStartFunPacket();
            rcFun.Pos = Id;
            CL321_RequestStartFunReplayPacket recvFun = new CL321_RequestStartFunReplayPacket();

            try
            {
                rcFun.Str_FunFlag = "0450";
                p_str_OutFrame[0] = BytesToString(rcFun.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rcFun, recvFun))
                    {
                        intRst = recvFun.ReciveResult == RecvResult.OK ? 0 : 2;
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
        /// 启动遥信输出
        /// </summary>
        /// <param name="id">误差板编号</param>        
        /// <param name="p_str_OutFrame">输出帧</param>
        /// <returns></returns>
        public int StartRemoteSignals(int Id, int int_RemoteCount, out string[] p_str_OutFrame)
        {
            p_str_OutFrame = new string[1];
            int intRst = 1;

            CL321_RequestWriteDataPacket rc = new CL321_RequestWriteDataPacket();
            rc.Pos = Id;
            CL321_RequestWriteDataReplayPacket recv = new CL321_RequestWriteDataReplayPacket();

            try
            {
                rc.Str_AddressFlag = "1200";

                rc.Str_Data = (int_RemoteCount).ToString("X").PadLeft(2, '0');
                p_str_OutFrame[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rc, recv))
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
        /// 停止遥信输出
        /// </summary>
        /// <param name="id">误差板编号</param>        
        /// <param name="p_str_OutFrame">输出帧</param>
        /// <returns></returns>
        public int StopOutPut(int Id, out string[] p_str_OutFrame)
        {
            p_str_OutFrame = new string[1];
            int intRst = 1;

            CL321_RequestWriteDataPacket rc = new CL321_RequestWriteDataPacket();
            rc.Pos = Id;
            CL321_RequestWriteDataReplayPacket recv = new CL321_RequestWriteDataReplayPacket();

            try
            {
                rc.Str_AddressFlag = "1200";

                rc.Str_Data = "00";
                p_str_OutFrame[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rc, recv))
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
        public int StartTest(int Id, byte verifyType, int intGlfx, out string[] p_str_OutFrame)
        {
            int intRst = 1;
            p_str_OutFrame = new string[1];

            CL321_RequestStartFunPacket rcFun = new CL321_RequestStartFunPacket();
            rcFun.Pos = Id;
            CL321_RequestStartFunReplayPacket recvFun = new CL321_RequestStartFunReplayPacket();

            try
            {
                if (intGlfx == 1 || intGlfx == 2)
                    rcFun.Str_FunFlag = verifyType.ToString("D2") + "01";
                else
                    rcFun.Str_FunFlag = verifyType.ToString("D2") + "02";
                p_str_OutFrame[0] = BytesToString(rcFun.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rcFun, recvFun))
                    {
                        intRst = recvFun.ReciveResult == RecvResult.OK ? 0 : 2;
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
        public int StopTest(int Id, byte verifyType, out string[] p_str_OutFrame)
        {
            int intRst = 1;
            p_str_OutFrame = new string[1];

            CL321_RequestStopFunPacket rcFun = new CL321_RequestStopFunPacket();
            rcFun.Pos = Id;
            CL321_RequestStopFunReplayPacket recvFun = new CL321_RequestStopFunReplayPacket();

            try
            {
                rcFun.Str_FunFlag = verifyType.ToString("D2") + "FF";
                p_str_OutFrame[0] = BytesToString(rcFun.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rcFun, recvFun))
                    {
                        intRst = recvFun.ReciveResult == RecvResult.OK ? 0 : 2;
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
        /// <param name="meterConst">被检表常数</param>
        /// <param name="circles">圈数</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int SetEnergePulseParams(int Id, int stdMeterConst, int meterConst, int circles, out string[] p_str_OutFrame)
        {
            p_str_OutFrame = new string[1];
            int intRst = 1;

            CL321_RequestWriteDataPacket rc = new CL321_RequestWriteDataPacket();
            rc.Pos = Id;
            CL321_RequestWriteDataReplayPacket recv = new CL321_RequestWriteDataReplayPacket();

            try
            {
                //标准表常数
                rc.Str_AddressFlag = "1002";
                ByteBuffer buf = new ByteBuffer();
                buf.Initialize();
                buf.PutInt(stdMeterConst);

                rc.Str_Data = BytesToString(buf.ToByteArray());
                p_str_OutFrame[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }
                //第一路 电表脉冲
                rc.Str_AddressFlag = "1003";
                buf = new ByteBuffer();
                buf.Initialize();
                buf.PutInt(meterConst);

                rc.Str_Data = BytesToString(buf.ToByteArray());
                p_str_OutFrame[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }

                //第二路 电表脉冲
                rc.Str_AddressFlag = "1010";
                buf = new ByteBuffer();
                buf.Initialize();
                buf.PutInt(meterConst);

                rc.Str_Data = BytesToString(buf.ToByteArray());
                p_str_OutFrame[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }

                //第二路 电表圈数
                rc.Str_AddressFlag = "1004";
                buf = new ByteBuffer();
                buf.Initialize();
                buf.PutInt(circles);

                rc.Str_Data = BytesToString(buf.ToByteArray());
                p_str_OutFrame[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }

                //第二路 电表圈数
                rc.Str_AddressFlag = "1011";
                buf = new ByteBuffer();
                buf.Initialize();
                buf.PutInt(circles);

                rc.Str_Data = BytesToString(buf.ToByteArray());
                p_str_OutFrame[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rc, recv))
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
        /// 设置时钟频率
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="stdMeterTimeFreq">标准表时钟频率</param>
        /// <param name="meterTimeFreq">被检表时钟频率</param>
        /// <param name="meterPulseNum">电表脉冲常数</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int SetClockFrequency(int Id, int stdMeterTimeFreq, int meterTimeFreq, int meterPulseNum, out string[] p_str_OutFrame)
        {
            p_str_OutFrame = new string[1];
            int intRst = 1;

            CL321_RequestWriteDataPacket rc = new CL321_RequestWriteDataPacket();
            rc.Pos = Id;
            CL321_RequestWriteDataReplayPacket recv = new CL321_RequestWriteDataReplayPacket();

            try
            {
                //标准表时钟频率
                rc.Str_AddressFlag = "1005";
                ByteBuffer buf = new ByteBuffer();
                buf.Initialize();
                buf.PutInt(stdMeterTimeFreq * 100);

                rc.Str_Data = BytesToString(buf.ToByteArray());
                p_str_OutFrame[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }
                //第一路 电表时钟频率
                rc.Str_AddressFlag = "1006";
                buf = new ByteBuffer();
                buf.Initialize();
                buf.PutInt(meterTimeFreq * 100);

                rc.Str_Data = BytesToString(buf.ToByteArray());
                p_str_OutFrame[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }

                //第二路 电表时钟频率
                rc.Str_AddressFlag = "1012";
                buf = new ByteBuffer();
                buf.Initialize();
                buf.PutInt(meterTimeFreq * 100);

                rc.Str_Data = BytesToString(buf.ToByteArray());
                p_str_OutFrame[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }

                //第一路 电表圈数
                rc.Str_AddressFlag = "1004";
                buf = new ByteBuffer();
                buf.Initialize();
                buf.PutInt(meterPulseNum);

                rc.Str_Data = BytesToString(buf.ToByteArray());
                p_str_OutFrame[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rc, recv))
                    {
                        intRst = recv.ReciveResult == RecvResult.OK ? 0 : 2;
                    }
                }
                else
                {
                    intRst = 0;
                }

                //第二路 电表圈数
                rc.Str_AddressFlag = "1011";
                buf = new ByteBuffer();
                buf.Initialize();
                buf.PutInt(meterPulseNum);

                rc.Str_Data = BytesToString(buf.ToByteArray());
                p_str_OutFrame[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rc, recv))
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
        public int SetBwVolCutIsolation(int Id, int p_int_IsolationStatus, out string[] p_str_OutFrame)
        {
            int intRst = 1;
            p_str_OutFrame = new string[1];
            CL321_RequestWriteDataPacket rc = new CL321_RequestWriteDataPacket();
            rc.Pos = Id;
            CL321_RequestWriteDataReplayPacket recv = new CL321_RequestWriteDataReplayPacket();

            try
            {
                //标准表时钟频率
                rc.Str_AddressFlag = "1202";
                if (p_int_IsolationStatus == 0)
                    rc.Str_Data = "222222";
                else if (p_int_IsolationStatus == 1)
                    rc.Str_Data = "000000";
                else
                    rc.Str_Data = "111111";
                p_str_OutFrame[0] = BytesToString(rc.GetPacketData());
                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rc, recv))
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
        /// <param name="intRoadNo">n路误差</param>
        /// <param name="MeterIndex">表位号</param>
        /// <param name="ErrorNum">误差次数</param>
        /// <param name="wcData">误差值</param>       
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int ReadCurrentData(int Id, byte wcbVerifyType, int intRoadNo, out int MeterIndex, out int ErrorNum, out string wcData, out string[] FrameAry)
        {
            int intRst = 1;
            FrameAry = new string[1];
            MeterIndex = 0;
            ErrorNum = 0;
            wcData = string.Empty;

            FrameAry = new string[1];
            CL321_RequestReadDataPacket rc2 = new CL321_RequestReadDataPacket();

            CL321_RequestReadDataReplayPacket recv2 = new CL321_RequestReadDataReplayPacket();
            try
            {
                rc2.Pos = Id;
                if (wcbVerifyType == 0)
                {//基本误差数据
                    if (intRoadNo == 1)
                        rc2.Str_AddressFlag = "1015";
                    else
                        rc2.Str_AddressFlag = "1017";
                }
                else
                {//日计时误差数据
                    if (intRoadNo == 1)
                        rc2.Str_AddressFlag = "1016";
                    else
                        rc2.Str_AddressFlag = "1018";
                }
                FrameAry[0] = BytesToString(rc2.GetPacketData());

                if (SendFlag)
                {
                    if (SendPacketWithRetry(m_Port, rc2, recv2))
                    {
                        intRst = recv2.ReciveResult == RecvResult.OK ? 0 : 2;

                        string[] arr = recv2.strReturnInfo.Split(':');

                        MeterIndex = Id;

                        if (arr.Length >= 2)
                        {
                            ErrorNum = int.Parse(arr[0]);
                            wcData = arr[1];
                        }
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

            return intRst;
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
                    System.Threading.Thread.Sleep(300);
                }
            }

            return false;
        }


        private string BytesToString(byte[] bytesData)
        {
            string strRevalue = string.Empty;
            if (bytesData == null || bytesData.Length < 1)
                return strRevalue;

            strRevalue = BitConverter.ToString(bytesData).Replace("-", "");
            //for (int i = 0; i < bytesData.Length; i++)
            //{
            //    byte byteTmp = bytesData[i];
            //    strRevalue += Convert.ToString(byteTmp, 16);
            //}

            return strRevalue;
        }
    }
}
