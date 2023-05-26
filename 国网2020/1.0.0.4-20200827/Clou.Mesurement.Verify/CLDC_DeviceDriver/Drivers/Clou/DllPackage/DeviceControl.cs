using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CLDC_DeviceDriver.Drivers.Clou.DllPackage
{
    public class DeviceControl : ABase
    {
        public override string DisPlayName
        {
            get { return "南网各个厂家设备控制"; }
        }

        public DeviceControl(string fileName, string className, bool isLocalAssemly)
            : base(fileName, className, isLocalAssemly)
        {
        }


        public void ShowDriverConfig()
        {
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = null;
                MethodInvoke("ShowDriverConfig", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        API.DeviceControlAPI_GeNing.ShowDriverConfig();
                        break;
                    default:
                        break;
                }
            }
        }


        /// <summary>
        ///  功率源联机
        /// </summary>
        /// <returns></returns>
        public int ConnectPower()
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = null;
                return MethodInvoke("ConnectPower", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        result = API.DeviceControlAPI_GeNing.ConnectPower();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }


        /// <summary>
        /// 升源
        /// </summary>
        /// 1：表位数组[是否挂表]
        /// 2：接线方式:0--三相四线有功,1--三相四线无功,2--三相三线有功,3--三相三线无功,4--二元件跨相90°,5--二元件跨相60°,6--三元件跨相90°,7--单相有功,8--单相无功
        /// 3：功率方向:0-正向 1-反向
        /// 4：功率因数：带感性（L）、容性（C）标记。例如(1.0,0.5L,0.8C)
        /// 5：A相电压幅值
        /// 6：B相电压幅值
        /// 7：C相电压幅值
        /// 8：A相电流幅值
        /// 9:B相电流幅值
        /// 10:C相电流幅值
        /// 11:元件（HABC）：0-ABC,1-A,2-B,3-C
        /// 12:频率
        /// 13:相序:true-正相序；false-逆相序
        /// </param>
        /// <returns></returns>
        public int PowerOn(bool[] MeterPosition, int TestMode, int PowerDirection, string PowerFactor, float VoltageUa, float VoltageUb, float VoltageUc, float CurrentIa, float CurrentIb,
                            float CurrentIc, float sng_UaPhi, float sng_UbPhi, float sng_UcPhi, float sng_IaPhi
           , float sng_IbPhi, float sng_IcPhi, int Element, float Frequency, bool PhaseSequence)
        {

            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] {  TestMode, PowerDirection, PowerFactor, VoltageUa, VoltageUb, VoltageUc, CurrentIa, CurrentIb, CurrentIc, sng_UaPhi,  sng_UbPhi,  sng_UcPhi,  sng_IaPhi
            ,  sng_IbPhi,  sng_IcPhi, Element, Frequency, PhaseSequence };
                result = MethodInvoke("PowerOn", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        result = API.DeviceControlAPI_GeNing.PowerOn(MeterPosition, TestMode, PowerDirection, PowerFactor, VoltageUa, VoltageUb, VoltageUc, CurrentIa, CurrentIb, CurrentIc, Element, Frequency, PhaseSequence);
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 关源
        /// </summary>
        /// <returns></returns>
        public int PowerOff()
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = null;
                result = MethodInvoke("PowerOff", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        result = API.DeviceControlAPI_GeNing.PowerOff();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 控制表位和误差板
        /// </summary>
        /// <param name="meterIndex">表位号</param>
        /// <param name="TypeNo">误差板功能：1进入跳闸检测、 2进入合闸检测、3读取外置继电器状态(使用出参)、4控制开路检测功能断开继电器命令、5控制开路检测功能启用继电器命令、6切换到220V输出外置式跳闸、7切换到开关量输出跳闸</param>
        /// <param name="meterControlType">负荷开关控制方式：0:A 无源无极性控制开关信号、1:B交流电压控制信号</param>
        /// <param name="Flag">跳合闸信号：表示是否检测到跳合闸信号：0 检测到，1 未检测到</param>
        /// <returns></returns>
        public int RelayControl(int meterIndex, int TypeNo, int meterControlType, ref int Flag)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { meterIndex, TypeNo, meterControlType, Flag };
                result = MethodInvoke("RelayControl", paras);
                if (paras[3] is int)
                {
                    Flag = (int)paras[3];
                }
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        result = API.DeviceControlAPI_GeNing.RelayControl(meterIndex, TypeNo, meterControlType, ref Flag);
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 控制表位继电器是否旁路
        /// </summary>
        /// <param name="MeterPosition">表位数组[是否挂表]</param>
        /// <param name="ControlType">旁路继电器状态:0：旁路 1：正常</param>
        /// <returns></returns>
        public int SetLoadRelayControl(bool[] MeterPosition, int ControlType)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { MeterPosition, ControlType };
                result = MethodInvoke("SetLoadRelayControl", paras);

            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        result = API.DeviceControlAPI_GeNing.SetLoadRelayControl(MeterPosition, ControlType);
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 485的连接设备、初始化
        /// </summary>
        /// <param name="meterIndex">表位号</param>
        /// <param name="setting">波特率，格式2400,,e,8,1</param>
        /// <returns></returns>
        public int Init485(int meterIndex, string setting)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { meterIndex, setting };
                result = MethodInvoke("Init485", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        result = API.DeviceControlAPI_GeNing.Init485(meterIndex, setting);
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 发送485报文
        /// </summary>
        /// <param name="meterIndex">表位号</param>
        /// <param name="setting">波特率，格式2400,,e,8,1</param>
        /// <param name="sendFrame">发送报文</param>
        /// <param name="outTime">超时时间</param>
        /// <param name="RecvFrame">返回报文</param>
        /// <returns></returns>
        public int SendToMeter(int meterIndex, string setting, string sendFrame, int outTime, ref string RecvFrame)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { meterIndex, setting, sendFrame, outTime, RecvFrame };
                result = MethodInvoke("SendToMeter", paras);
                if (paras[4] is string)
                {
                    RecvFrame = paras[4].ToString();
                }
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        result = API.DeviceControlAPI_GeNing.SendToMeter(meterIndex, setting, sendFrame, outTime, ref RecvFrame);
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 发送485报文
        /// </summary>
        /// <param name="meterIndex">表位号</param>
        /// <param name="setting">波特率，格式2400,,e,8,1</param>
        /// <param name="sendFrame">发送报文</param>
        /// <param name="outTime">超时时间</param>
        /// <param name="RecvFrame">返回报文</param>
        /// <returns></returns>
        public int SendToMeterByBlue(int meterIndex, string setting, string sendFrame, int outTime, ref string RecvFrame)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { meterIndex, setting, sendFrame, outTime, RecvFrame };
                result = MethodInvoke("SendToMeterByBlue", paras);
                if (paras[4] is string)
                {
                    RecvFrame = paras[4].ToString();
                }
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        result = API.DeviceControlAPI_GeNing.SendToMeter(meterIndex, setting, sendFrame, outTime, ref RecvFrame);
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        /// <summary>
        ///  标准表联机
        /// </summary>
        /// <returns></returns>
        public int ConnectRefMeter()
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = null;
                result = MethodInvoke("ConnectRefMeter", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 3.5.2.	读标准表实时数据
        /// </summary>
        /// 1：ABC电压[0~2]  V
        /// 2：ABC电流[3~5]  A
        /// 3：ABC电压相位[6~8]  °
        /// 4：ABC电流相位[9~11]  °
        /// 5：ABC相角[12~14]  °
        /// 6：功率相角[15]  °
        /// 7：ABC相有功功率[16~18] W
        /// 8：总有功功率[19]  W
        /// 9: ABC无功功率[20~22]   Var
        /// 10:总无功功率[23]    Var
        /// 11:ABC视在功率[24~26]   VA
        /// 12:总视在功率[27]    VA
        /// 13:ABC有功功率因数[28~30]
        /// 14:总有功功率因数[31]
        /// 15:总无功功率因数[32]
        /// 16:频率[33]    Hz
        /// </param>
        /// <returns></returns>
        public int ReadInstMetricAll(out float[] instValue)
        {

            float[] instValueTmp = new float[34];
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { instValueTmp };
                result = MethodInvoke("ReadInstMetricAll", paras);
                if (paras.Length > 0)
                {
                    object obj = paras[0];

                    instValueTmp = (float[])obj;
                }
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        result = API.DeviceControlAPI_GeNing.ReadInstMetricAll(out instValueTmp);
                    break;
                    default:
                        break;
               }
           }
            instValue = instValueTmp;
            return result;
        }

        /// <summary>
        /// 红外的连接设备、初始化
        /// </summary>
        /// <param name="meterIndex">表位号</param>
        /// <param name="setting">波特率，格式2400,,e,8,1</param>
        /// <returns></returns>
        public int InitInfrared(int meterIndex, string setting)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { meterIndex, setting };
                result = MethodInvoke("InitInfrared", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        result = API.DeviceControlAPI_GeNing.InitInfrared(meterIndex, setting);
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 发送485报文(红外)
        /// </summary>
        /// <param name="meterIndex">表位号</param>
        /// <param name="setting">波特率，格式2400,,e,8,1</param>
        /// <param name="sendFrame">发送报文</param>
        /// <param name="outTime">超时时间</param>
        /// <param name="RecvFrame">返回报文</param>
        /// <returns></returns>
        public int SendInfraredToMeter(int meterIndex, string setting, string sendFrame, int outTime, ref string RecvFrame)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { meterIndex, setting, sendFrame, outTime, RecvFrame };
                result = MethodInvoke("SendInfraredToMeter", paras);
                if (paras[4] is string)
                {
                    RecvFrame = paras[4].ToString();
                }
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        result = API.DeviceControlAPI_GeNing.SendInfraredToMeter(meterIndex, setting, sendFrame, outTime, ref RecvFrame);
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        /// <summary>
        ///  误差板联机
        /// </summary>
        /// <returns></returns>
        public int ConnectWcB()
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = null;
                result = MethodInvoke("ConnectWcB", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// CL191B联机
        /// </summary>
        /// <returns></returns>
        public int Connect191B()
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = null;
                result = MethodInvoke("Connect191B", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 191B设置脉冲通道
        /// </summary>
        /// <returns></returns>
        public int SetTimePulse(bool isTime)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { isTime };
                result = MethodInvoke("SetTimePulse", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                   result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// 设置误差板设置脉冲通道和类型 
        /// </summary>
        /// <param name="Id">误差板编号</param>     
        /// <param name="wcchannelno">脉冲通道,0=P+,1=P-,2=Q+,3=Q-,4=需量,5=时钟,0x06：耐压实验 0x07：多功能脉冲计数试验</param>
        /// <param name="pulsetype">通道类型,0=脉冲盒,1=光电头</param>
        /// <param name="gygy">脉冲类型,0=共阳,1=共阴</param>
        /// <param name="EnergyChangeType">电能走字选择位，1为显示脉冲间隔时间，0为显示脉冲计数</param>
        /// <param name="dgnwcchannelno">多功能误差通道号,1=日计时，2=需量脉冲</param>
        /// <param name="checktype">检定类型0x00电能误差、0x01需量误差、0x02日计时误差、0x03走字计数、0x04对标、0x05预付费功能检定。0x06：耐压实验 0x07：多功能脉冲计数试验</param>
        public int SetPulseChannelAndType(bool[] MeterPosition, int wcchannelno, int pulsetype, int gygy, int EnergyChangeType, int dgnwcchannelno, int checktype)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { MeterPosition, wcchannelno, pulsetype, gygy, EnergyChangeType, dgnwcchannelno, checktype };
                result = MethodInvoke("SetPulseChannelAndType", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                 result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
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
        public int SetClockFrequency(bool[] MeterPosition, int stdMeterTimeFreq, int meterTimeFreq, int meterPulseNum)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { MeterPosition, stdMeterTimeFreq, meterTimeFreq, meterPulseNum };
                result = MethodInvoke("SetClockFrequency", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //               result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 启动检定
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="verifyType">检定类型 0=电能误差,1=需量周期,2=日计时误差,3=计数,4=对标,5=预付费功能检定,6=耐压实验,7=多功能脉冲计数试验</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int StartTest(bool[] MeterPosition, byte verifyType)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { MeterPosition, verifyType };
                result = MethodInvoke("StartTest", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //          result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 停止检定
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="verifyType">检定类型 0=电能误差,1=需量周期,2=日计时误差,3=计数,4=对标,5=预付费功能检定,6=耐压实验,7=多功能脉冲计数试验</param>
        /// <param name="FrameAry">输出帧</param>
        /// <returns></returns>
        public int StopTest(bool[] MeterPosition, byte verifyType)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { MeterPosition, verifyType };
                result = MethodInvoke("StopTest", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //          result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// 设置标准表常数
        /// </summary>
        /// <param name="pulseConst">标准表常数</param>
        /// <returns></returns>

        public int SetStdPulseConst(int pulseConst)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { pulseConst };
                result = MethodInvoke("SetStdPulseConst", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //        result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// 设置标准表参数
        /// </summary>
        /// <param name="wiringMode"></param>
        /// <param name="powerMode"></param>
        /// <param name="calcType"></param>
        /// <returns></returns>
        public int SetStdParams(int meterConst, int circle, int currentType, int wiringMode)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { meterConst, circle, currentType, wiringMode };
                result = MethodInvoke("SetStdParams", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //          result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// 设置电能误差检定时脉冲参数
        /// </summary>
        /// <param name="id">误差板编号</param>
        /// <param name="stdMeterConst">标准表脉冲常数</param>
        /// <param name="stdPulseFreq">标准脉冲频率</param>
        /// <param name="stdMeterConstShorttime">标准脉冲缩放倍数</param>
        /// <param name="meterConst">被检表脉冲常数</param>
        /// <param name="circles">圈数</param>
        /// <param name="meterConstZooms">被检脉冲缩放倍数</param>
        /// <param name="FrameAry"></param>
        /// <returns></returns>
        public int SetEnergePulseParams(bool[] MeterPosition, int stdMeterConst, int stdPulseFreq, int stdMeterConstShorttime, int[] meterConst, int[] circles, int meterConstZooms)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { MeterPosition, stdMeterConst, stdPulseFreq, stdMeterConstShorttime, meterConst, circles, meterConstZooms };
                result = MethodInvoke("SetEnergePulseParams", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //     result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 读取误差
        /// </summary>
        /// <param name="meterIndex"></param>
        /// <param name="Type"></param>
        /// <param name="Index"></param>
        /// <param name="Num"></param>
        /// <param name="Data"></param>
        /// <returns></returns>


        public int ReadCurrentData(int meterIndex, ref byte Type, ref int Index, ref int Num, ref string Data)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { meterIndex, Type, Index, Num, Data };
                result = MethodInvoke("ReadCurrentData", paras);
                if (paras[1] is byte)
                {
                    Type = byte.Parse(paras[1].ToString());
                }
                if (paras[2] is int)
                {
                    Index = int.Parse(paras[2].ToString());
                }
                if (paras[3] is int)
                {
                    Num = int.Parse(paras[3].ToString());
                }
                if (paras[4] is string)
                {
                    Data = paras[4].ToString();
                }

            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //   result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// 供电类型
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="isMeterTypeHGQ"></param>
        /// <returns></returns>
        public int SetPowerSupplyType(int elementType, bool isMeterTypeHGQ)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                int[] switchOpen = { 0, 1, 2, 4, 5, 6, 9 };
                int[] switchClose = { 3, 7, 8 };
                object[] paras = new object[] { elementType, isMeterTypeHGQ, switchOpen, switchClose };

                result = MethodInvoke("SetPowerSupplyType", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //          result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;

        }

        /// <summary>
        /// 继电器控制
        /// </summary>
        /// <param name="switchOpen">需闭合继电器</param>
        /// <param name="switchClose">需断开继电器</param>
        /// <returns></returns>
        public int SetRelay(int[] switchOpen, int[] switchClose)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { 3, true, switchOpen, switchClose };

                result = MethodInvoke("SetPowerSupplyType", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //          result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;

        }


        /// <summary>
        /// 初始化命令
        /// </summary>
        /// <param name="int_Fn">Fn</param>
        public int PacketToCarrierInit(int int_Fn, bool state, int num)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { int_Fn, state, num };
                result = MethodInvoke("PacketToCarrierInit", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //          result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 控制命令
        /// </summary>
        /// <param name="int_Fn">Fn</param>
        /// <param name="Data">数据域</param>
        public int PacketToCarrierCtr(int int_Fn, byte[] Data, int num)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { int_Fn, Data, num };
                result = MethodInvoke("PacketToCarrierCtr", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //          result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 不带数据域，返回确认/否认帧的命令
        /// </summary>
        /// <param name="byt_AFN">AFN</param>
        /// <param name="int_Fn">Fn</param>
        public int PacketToCarrierInitA(byte byt_AFN, int int_Fn, int num)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { byt_AFN, int_Fn, num };
                result = MethodInvoke("PacketToCarrierInitA", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //          result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        /// <summary>
        ///载波板联机
        /// </summary>
        /// <returns></returns>
        public int ConnectCarrier()
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = null;
                result = MethodInvoke("ConnectCarrier", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        ///2029D联机
        /// </summary>
        /// <returns></returns>
        public int Connect2029D()
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = null;
                result = MethodInvoke("Connect2029D", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        /// <summary>
        ///2029D联机
        /// </summary>
        /// <returns></returns>
        public int Connect2029B()
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = null;
                result = MethodInvoke("Connect2029B", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        #region 李鑫 20200618
        /// <summary>
        /// 温控板联机
        /// </summary>
        /// <returns></returns>
        public int ConnectTemperatureBoard()
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = null;
                result = MethodInvoke("ConnectTemperatureBoard", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// 读实时测量数据
        /// </summary>
        /// <param name="Index">表位号</param>
        /// <param name="instValue">输出测量数据</param>
        /// <returns></returns>
        public int ReadTemperature(int Index, out float[] instValue)
        {
            int result = 1;
            float[] tmpinstValue = null;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { Index, tmpinstValue };
                result = MethodInvoke("ReadTemperature", paras);

                if (paras[1] is float[])
                {
                    tmpinstValue = (float[])paras[1];
                }

            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            instValue = tmpinstValue;
            return result;
        }

        /// <summary>
        /// 设置温度
        /// </summary>
        /// <param name="Index">表位号</param>
        /// <param name="Flags">需要温控标志位</param>
        /// <param name="Temperatures">控制温度</param>
        /// <returns></returns>
        public int SetTemperature(int Index, bool[] Flags, float[] Temperatures)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { Index, Flags, Temperatures };
                result = MethodInvoke("SetTemperature", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 开风扇
        /// </summary>
        /// <param name="Flag">控制标志位</param>
        /// <returns></returns>
        public int OpenFan(bool Flag)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { Flag };
                result = MethodInvoke("OpenFan", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 开锁
        /// </summary>
        /// <returns></returns>
        public int OpenLock()
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = null;
                result = MethodInvoke("OpenLock", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        #endregion
        /// <summary>
        /// 添加载波从节点
        /// </summary>
        /// <param name="int_Fn">Fn</param>
        /// <param name="Data">电表地址，反转</param>
        public int PacketToCarrierAddAddr(int int_Fn, byte[] Data, bool state, int num)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { int_Fn, Data, (byte)1, state, num };
                result = MethodInvoke("PacketToCarrierAddAddr", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// 打包645成376.2
        /// </summary>
        /// <param name="int_Fn">Fn</param>
        /// <param name="Data">电表地址，反转</param>
        public int Packet645To3762Frame(byte[] Frame645, byte byt_DLTType, ref byte[] RFrame645, bool state, int int_BwIndex, int num)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                RFrame645 = null;
                object[] paras = new object[] { Frame645, byt_DLTType, RFrame645, state, int_BwIndex,num };
                result = MethodInvoke("Packet645To3762Frame", paras);

                if (paras[2] is byte[])
                {
                    RFrame645 = (byte[])paras[2];
                }
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        //获取标准表常数
        public int GetBzMeterConst(float MaxU, float MaxI, ref string BZConst)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                // RFrame645 = null;
                object[] paras = new object[] { MaxU, MaxI, BZConst };
                result = MethodInvoke("GetBzMeterConst", paras);

                if (paras[2] is string)
                {
                    BZConst = paras[2].ToString();
                }
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// 设置尖顶波-1 平顶波-2
        /// </summary>
        /// <returns></returns>
        public int SetJd_Pd(int ua, int ia, int ub, int ib, int uc, int ic)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                // RFrame645 = null;
                object[] paras = new object[] { ua, ia, ub, ib, uc, ic };
                result = MethodInvoke("SetJd_Pd", paras);


            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
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

        public int SettingWaveformSelection(int ua, int ia, int ub, int ib, int uc, int ic)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                // RFrame645 = null;
                object[] paras = new object[] { ua, ia, ub, ib, uc, ic };
                result = MethodInvoke("SettingWaveformSelection", paras);


            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 设置谐波总开关
        /// </summary>
        /// <param name="FrameAry"></param>
        /// <returns></returns>

        public int SetHarmonicSwitch(bool bSwitch)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                // RFrame645 = null;
                object[] paras = new object[] { bSwitch };
                result = MethodInvoke("SetHarmonicSwitch", paras);


            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// 设置谐波参数
        /// </summary>
        /// <param name="Phase">数组各元素：0=A相电压，1=B相电压，2=C相电压，3=A相电流，4=B相电流，5=C相电流</param>
        /// <param name="int_XTSwitch">各相开关，数组元素值：0=不加谐波，1=加谐波，</param>
        /// <param name="sng_Value">各次谐波含量</param>
        /// <param name="sng_Phase">各次谐波相位</param>
        /// <param name="frameAry">合成上行报文</param>
        /// <returns></returns>
        public int SetHarmonic(int Phase, int[] int_XTSwitch, Single[] sng_Value, Single[] sng_Phase)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                // RFrame645 = null;
                object[] paras = new object[] { Phase, int_XTSwitch, sng_Value, sng_Phase };
                result = MethodInvoke("SetHarmonic", paras);


            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// 读GPS时间
        /// </summary>
        /// <param name="strTime"></param>
        /// <returns></returns>
        public int ReadGPSTime(ref string strTime)
        {
            int result = 1;
            string strRevTime = "";
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { strRevTime };
                result = MethodInvoke("ReadGPSTime", paras);
                if (paras.Length > 0)
                {
                    object obj = paras[0];

                    strRevTime = obj.ToString();
                    strTime = strRevTime;
                }

            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// 读取脉冲和脉冲间隔时间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="CheckType"></param>
        /// <param name="ErrNum"></param>
        /// <param name="ErrData"></param>
        /// <param name="TQtime"></param>
        /// <returns></returns>
        public int ReadQueryCurrentError(int id, int CheckType, out int ErrNum, out string ErrData, out string TQtime)
        {
            int result = 1;
            ErrNum = 0;
            ErrData = string.Empty;
            TQtime = string.Empty;

            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { id, CheckType, ErrNum, ErrData, TQtime };
                result = MethodInvoke("ReadQueryCurrentError", paras);

                if (paras[2] is int)
                {
                    ErrNum = int.Parse(paras[2].ToString());
                }
                if (paras[3] is string)
                {
                    ErrData = paras[3].ToString();
                }
                if (paras[4] is string)
                {
                    TQtime = paras[4].ToString();
                }

            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //   result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// 设置标准表挡位
        /// </summary>
        /// <param name="Ua"></param>
        /// <param name="Ub"></param>
        /// <param name="Uc"></param>
        /// <param name="Ia"></param>
        /// <param name="Ib"></param>
        /// <param name="Ic"></param>
        /// <returns></returns>
        public int SetRange(float Ua, float Ub, float Uc, float Ia, float Ib, float Ic)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                // RFrame645 = null;
                object[] paras = new object[] { Ua, Ub, Uc, Ia, Ib, Ic };
                result = MethodInvoke("SetRange", paras);


            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        } 

         /// <summary>
        /// 设置标准表界面 1：谐波柱图界面2：谐波列表界面3：波形界面4：清除设置界面
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="FrameAry">输出报文</param>
        /// <returns></returns>
        public int SetDisplayForm(int formType)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                // RFrame645 = null;
                object[] paras = new object[] { formType };
                result = MethodInvoke("SetDisplayForm", paras);


            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
         /// <summary>
        /// 读取各相电压电流谐波幅值（分两帧读取数据）
        /// </summary>
        /// <param name="phase">相别，0是C相电压，1是B相电压，2是A相电压，3是C相电流，4是B相电流，5是A相电流</param>
        /// <param name="harmonicArry"></param>
        /// <returns></returns>
        public int ReadHarmonicArry(int phase, out float[] harmonicArry)
        {
            int result = 1;
            harmonicArry =new float[65];
          

            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { phase, harmonicArry };
                result = MethodInvoke("ReadHarmonicArry", paras);

                if (paras[1] is float[])
                {
                    harmonicArry =(float[])(paras[1]);
                }
              

            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //   result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }


        /// <summary>
        /// 设置标准表接线方式
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="FrameAry">输出报文</param>
        /// <returns></returns>
        public int SetWiringMode(int range, int wiringMode)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                // RFrame645 = null;
                object[] paras = new object[] { range, wiringMode };
                result = MethodInvoke("SetWiringMode", paras);


            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// 设置标准表接线方式
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="FrameAry">输出报文</param>
        /// <returns></returns>
        public int PowerOffOnlyCurrent()
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                // RFrame645 = null;
                object[] paras = null;
                result = MethodInvoke("PowerOffOnlyCurrent", paras);


            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 设置闭环反馈
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="FrameAry">输出报文</param>
        /// <returns></returns>
        public int SetLoopFeed(bool state)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                // RFrame645 = null;
                object[] paras = new object[] { state };
                result = MethodInvoke("SetLoopFeed", paras);


            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        
        public int SetDropWave(float[] Wave)
        {
            int result = 1;



            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { Wave };
                result = MethodInvoke("SetDropWave", paras);



            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //   result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        public int SetDropTime(int[] Time)
        {
            int result = 1;



            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { Time };
                result = MethodInvoke("SetDropTime", paras);



            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //   result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        public int SetDropSwitch(bool[] Switch)
        {
            int result = 1;



            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { Switch };
                result = MethodInvoke("SetDropSwitch", paras);



            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //   result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        public int SetCL309UIRange(float Ua, float Ub, float Uc, float Ia, float Ib, float Ic)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                // RFrame645 = null;
                object[] paras = new object[] { Ua, Ub, Uc, Ia, Ib, Ic };
                result = MethodInvoke("SetCL309UIRange", paras);


            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        /// <summary>
        /// 启动标准表
        /// </summary>
        /// <param name="calcType">0 停止计算  </param>  
        ///                        1 开始计算电能误差  
        ///                        2 开始计算电能走字</param>
        /// <returns></returns>
        public int SetErrCalcType(int calcType)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { calcType };
                result = MethodInvoke("SetErrCalcType", paras);
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //        result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }


        /// <summary>
        /// 控制类型 0x00：默认界面 
        /// 0x01: 功率测量界面
        /// 0x02: 伏安测量界面
        /// 0x03: 电能误差与标准差界面
        /// 0x05: 电能量走字界面
        /// 0x09: 谐波测量界面
        /// 0x10: 稳定度测量界面
        /// 0xFE: 清除界面设置(返回默认界面)
        /// </summary>
        /// <param name="funcType"></param>
        /// <returns></returns>
        public int FuncMstate(int funcType)
        {
            int result = 1;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                // RFrame645 = null;
                object[] paras = new object[] { funcType };
                result = MethodInvoke("FuncMstate", paras);


            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //                    result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        public int ReadTestEnergy(out float testEnergy, out long PulseNum)
        {
            int result = 1;
            testEnergy = 0.0f;
            PulseNum = 0;
            if (GlobalUnit.DeviceDllType == Cus_SouthDeviceDllType.DotNet平台开发)
            {
                object[] paras = new object[] { testEnergy, PulseNum };
                result = MethodInvoke("ReadTestEnergy", paras);
                if (paras[0] is float)
                {
                    testEnergy = float.Parse(paras[0].ToString());
                }
                if (paras[1] is long)
                {
                    PulseNum = long.Parse(paras[1].ToString());
                }
            }
            else
            {
                switch (GlobalUnit.DeviceManufacturers)
                {
                    case Cus_DeviceManufacturers.格宁:
                        //        result = API.DeviceControlAPI_GeNing.ConnectRefMeter();
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
    }
}
