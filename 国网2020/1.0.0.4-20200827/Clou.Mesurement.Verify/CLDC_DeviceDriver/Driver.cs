using System;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using CLDC_DeviceDriver.Drivers.Clou.DllPackage;

namespace CLDC_DeviceDriver
{

    public delegate void MsgCallBack(string szMessage);
    #region 数据结构
    /// <summary>
    /// 读取的电源信息
    /// </summary>
    public struct stStdInfo
    {
        public CLDC_Comm.Enum.Cus_Clfs Clfs;  //	 接线方式	
        public byte Flip_ABC;     //   	'相位开关控制	
        public float Freq;//	'频率	
        public byte Scale_Ua;// 	'Ua档位 	
        public byte Scale_Ub;// 	'Ub档位 	
        public byte Scale_Uc;// 	'Uc档位 	
        public byte Scale_Ia;// 	'Ia档位 	
        public byte Scale_Ib;// 	'Ib档位 	
        public byte Scale_Ic;// 	'Ic档位 	
        public float Ua;//	'UA 
        public float Ia;//	'Ia 	
        public float Ub;//	'UB  	
        public float Ib;// Ib 	
        public float Uc;// 	'UC 	
        public float Ic;// 'Ic 	
        public float Phi_Ua;// 	'Ua相位 	
        public float Phi_Ia;// 	'Ia相位 	
        public float Phi_Ub;//	'UB相位 	
        public float Phi_Ib;// 	'Ib相位 	
        public float Phi_Uc;// 	'UC相位 	
        public float Phi_Ic;// 	'Ic相位 	
        public float Pa;// 	'A相有功功率 	
        public float Pb;// 	'B相有功功率	
        public float Pc;// 	'C相有功功率	
        public float Qa;//	'A相无功功率	
        public float Qb;//	'B相无功功率	
        public float Qc;//	'C相无功功率	
        public float Sa;//	'A相视在功率	
        public float Sb;// 	'B相视在功率	
        public float Sc;// 	'C相视在功率	
        public float P;//	总有功功率	
        public float Q;//	总无功功率	
        public float S;//	总视在功功率	
        public float COS;//	有功功率因数	
        public float SIN;//无功功率因数	
    }

   
   








    /// <summary>
    /// 读取的误差数据
    /// </summary>
    public struct stError
    {
        /// <summary>
        /// 误差值
        /// </summary>
        public string szError;

        /// <summary>
        /// 标识当前属于第几次误差
        /// </summary>
        public int Index;

        /// <summary>
        /// 表位号
        /// </summary>
        public int MeterIndex;
        /// <summary>
        /// 状态类型
        /// </summary>
        public int MeterConst;
        /// <summary>
        /// 电压回路状态,0x00表示直接接入式电表电压回路选择，0x01表示互感器接入式电表电压回路选择，0x02表示本表位无电表接入
        /// </summary>
        public Cus_BothVRoadType vType;
        /// <summary>
        /// 通讯口状态,0x00表示选择第一路普通485通讯；0x01表示选择第二路普通485通讯；0x02表示选择红外通讯；
        /// </summary>
        public int ConnType;

        /*
         * 状态类型分为四种：接线故障状态（Bit0）、预付费跳闸状态（Bit1）、报警信号状态（Bit2）、对标状态（Bit3）的参数
         * 分别由一个字节中的Bit0、Bit1、Bit2、Bit3标示，为1则表示该表位有故障/跳闸/报警/对标完成，为0则表示正常/正常/正常/未完成对标。
        */
        /// <summary>
        /// 接线故障状态,为true则表示该表位有故障,false为正常
        /// </summary>
        public bool statusTypeIsOnErr_Jxgz;

        /// <summary>
        /// 预付费跳闸状态,为true则表示该表位跳闸,为false正常
        /// </summary>
        public bool statusTypeIsOnErr_Yfftz;

        /// <summary>
        /// 报警信号状态,为true则表示该表位报警,false为正常
        /// </summary>
        public bool statusTypeIsOnErr_Bjxh;

        /// <summary>
        /// 对标状态,为true则表示该表位对标完成,false为未完成对标
        /// </summary>
        public bool statusTypeIsOnOver_Db;
        /// <summary>
        /// 温度过高故障状态（false：正常；true：故障）。温度过高时，会自动短接隔离继电器
        /// </summary>
        public bool statusTypeIsOnErr_Temp;
        /// <summary>
        /// 光电信号状态（false：未挂表；true：已挂表）
        /// </summary>
        public bool statusTypeIsOn_HaveMeter;

        /// <summary>
        /// 表位上限限位状态（false：未就位；true：就位）
        /// </summary>
        public bool statusTypeIsOn_PressUpLimit;

        /// <summary>
        /// 表位下限限位状态（false：未就位；true：就位）
        /// </summary>
        public bool statusTypeIsOn_PressDownLimt;
        /// <summary>
        /// true :读到数据 FALSE ：没有读到
        /// </summary>
        public bool statusReadFlog;


        public string QdTime; 
    }


    /// <summary>
    /// 台体驱动类型
    /// </summary>
    //public enum eDriverType
    //{
    //    Driver_Clou_Common,

    //    Driver_Clou_2036,

    //    Driver_Clou_V90,

    //    Driver_Clou_NMZ1,

    //    Driver_Geny_Standard
    //}
    #endregion

    public class Driver : Drivers.IDriver
    {


        private static Drivers.IDriver iDriver;


        object objPowerOnLock = new object();


        public Driver(int bws, string[] arrayDevice)
        {
            iDriver = new Drivers.Clou.Driver(bws, arrayDevice);
        }


        #region IDriver 成员
        /// <summary>
        /// 停止当前操作
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            if (GlobalUnit.IsDemo) return true;
            return iDriver.Stop();
        }

        /// <summary>
        /// 联机操作
        /// </summary>
        /// <returns>联机是否成功</returns>
        public bool Link()
        {
            lock (this)
            {
                return iDriver.Link();
            }
        }

        /// <summary>
        /// 关源操作
        /// </summary>
        /// <returns>关源是否成功</returns>
        public bool PowerOff()
        {
            if (GlobalUnit.IsDemo) return true;
            lock (objPowerOnLock)
            {
                return iDriver.PowerOff();
            }
        }


        public MsgCallBack CallBack
        {
            get
            {
                if (iDriver != null)
                    return iDriver.CallBack;
                return null;
            }
            set
            {
                if (iDriver != null)
                    iDriver.CallBack = value;
                else
                    throw new Exception("还没有初始化设备组件或是初始化设备组件失败");
            }
        }

        /// <summary>
        /// 升源操作
        /// </summary>
        /// <param name="clfs">测量方式</param>
        /// <param name="sng_Ub">额定电压</param>
        /// <param name="sng_Ib">额定电流</param>
        /// <param name="sng_IMax">最大电流</param>
        /// <param name="sng_xUb_A">输出额定电压倍数A</param>
        /// <param name="sng_xUb_B">输出额定电压倍数B</param>
        /// <param name="sng_xUb_C">输出额定电压倍数C</param>
        /// <param name="sng_xIb_A">输出额定电流倍数A</param>
        /// <param name="sng_xIb_B">输出额定电流倍数B</param>
        /// <param name="sng_xIb_C">输出额定电流倍数C</param>
        /// <param name="element">元件</param>
        /// <param name="sng_UaPhi">A相电压角度</param>
        /// <param name="sng_UbPhi">B相电压角度</param>
        /// <param name="sng_UcPhi">C相电压角度</param>
        /// <param name="sng_IaPhi">A相电流角度</param>
        /// <param name="sng_IbPhi">B相电流角度</param>
        /// <param name="sng_IcPhi">C相电流角度</param>
        /// <param name="sng_Freq">频率</param>
        /// <param name="bln_IsNxx">是否逆向序</param>
        /// <returns></returns>
        public bool PowerOn(CLDC_Comm.Enum.Cus_Clfs clfs, Cus_PowerFangXiang glfx, string strGlys, float sng_Ub, float sng_Ib, float sng_IMax,
                        float sng_xUb_A, float sng_xUb_B, float sng_xUb_C, float sng_xIb_A,
                        float sng_xIb_B, float sng_xIb_C, Cus_PowerYuanJian element, float sng_UaPhi,
                        float sng_UbPhi, float sng_UcPhi, float sng_IaPhi, float sng_IbPhi,
                        float sng_IcPhi, float sng_Freq, bool IsDuiBiao, bool IsQianDong, bool bln_IsNxx)
        {
            if (GlobalUnit.IsDemo) return true;
            lock (objPowerOnLock)
            {

                return iDriver.PowerOn(clfs, glfx, strGlys
                , sng_Ub, sng_Ib, sng_IMax
                    , sng_xUb_A, sng_xUb_B
                        , sng_xUb_C, sng_xIb_A
                            , sng_xIb_B, sng_xIb_C
                                , element
                    , sng_UaPhi, sng_UbPhi, sng_UcPhi, sng_IaPhi
                    , sng_IbPhi, sng_IcPhi, sng_Freq
                    , IsDuiBiao, IsQianDong, bln_IsNxx);
            }
        }


        /// <summary>
        /// 读取标准表信息
        /// </summary>
        /// <returns></returns>
        public stStdInfo ReadStdInfo()
        {
            return iDriver.ReadStdInfo();
        }
        /// <summary>
        /// 初始化启动项目
        /// </summary>
        /// <param name="clfs">测量方式</param>
        /// <param name="glfx">功率方向</param>
        /// <param name="im">脉冲类型</param>
        /// <param name="IsOnOff">表位开关</param>
        /// <param name="startTimes">各表位起动时间【单位秒】</param>
        /// <returns>初始化是否成功</returns>
        public bool InitStartUp(CLDC_Comm.Enum.Cus_Clfs clfs, Cus_PowerFangXiang glfx, CLDC_Comm.Enum.Cus_GyGyType[] im, bool[] IsOnOff, int[] startTimes, int[] meterconst)
        {
            if (GlobalUnit.IsDemo) return true;
            return iDriver.InitStartUp(clfs, glfx, im, IsOnOff, startTimes, meterconst);

        }


        /// <summary>
        /// 读取所有误差版的数据，表位=返回数组下标+1
        /// </summary>
        /// <param name="IsOnOff"></param>
        /// <param name="errTimes"></param>
        /// <returns></returns>
        public stError[] ReadWcb(bool[] IsOnOff, bool state)
        {

            return iDriver.ReadWcb(IsOnOff, state);

        }

        public stError ReadWc(int bw)
        {

            return iDriver.ReadWc(bw);

        }

        //zhengrubin-20190920
        /// <summary>
        /// 初始化日计时误差、不包含升源操作
        /// </summary>
        /// <param name="IsOnOff">指示具体表位是否需要试验该项目，表位号=下标+1</param>
        /// <param name="im">脉冲类型</param>
        /// <param name="MeterFre">各表位时钟周期</param>
        /// <returns></returns>
        public bool InitTimeAccuracy(bool[] IsOnOff, CLDC_Comm.Enum.Cus_GyGyType[] im, float[] MeterFre, float[] bcs, int[] quans)
        {
            if (GlobalUnit.IsDemo) return true;
            return iDriver.InitTimeAccuracy(IsOnOff, im, MeterFre, bcs, quans);

        }

        /// <summary>
        /// 初始化基本误差项目、不包含升源操作
        /// </summary>
        /// <param name="clfs">测量方式</param>
        /// <param name="glfx">功率方向</param>
        /// <param name="bcs">表常数</param>
        /// <param name="quans">检定圈数</param>
        /// <param name="wccs">计算误差脉冲个数</param>
        /// <param name="im">脉冲通道</param>
        /// <param name="IsOnOff">表位开关</param>
        /// <returns>初始化基本误差是否成功</returns>
        public bool InitError(CLDC_Comm.Enum.Cus_Clfs clfs, Cus_PowerFangXiang glfx, int[] bcs, int[] quans, int wccs, CLDC_Comm.Enum.Cus_GyGyType[] im, bool[] IsOnOff)
        {
            if (GlobalUnit.IsDemo) return true;
            return iDriver.InitError(clfs, glfx, bcs, quans, wccs, im, IsOnOff);

        }


        /// <summary>
        /// 初始化潜动试验项目
        /// </summary>
        /// <param name="clfs">测量方式</param>
        /// <param name="glfx">功率方向</param>
        /// <param name="im">脉冲类型</param>
        /// <param name="IsOnOff">表位开关</param>
        /// <param name="startTimes">各表位起动时间【单位秒】</param>
        /// <returns></returns>
        public bool InitCreeping(CLDC_Comm.Enum.Cus_Clfs clfs, Cus_PowerFangXiang glfx, CLDC_Comm.Enum.Cus_GyGyType[] im, bool[] IsOnOff, int[] startTimes)
        {
            if (GlobalUnit.IsDemo) return true;
            return iDriver.InitCreeping(clfs, glfx, im, IsOnOff, startTimes);

        }
        /// <summary>
        /// 启动/停止当前设置的功能
        /// </summary>
        /// <param name="IsOnOff"></param>
        /// <returns></returns>
        public bool SetCurFunctionOnOrOff(bool IsOnOff, byte state)
        {
            //启动或停止当前功能
            if (GlobalUnit.IsDemo) return true;
            return iDriver.SetCurFunctionOnOrOff(IsOnOff, state);
        }

        /// <summary>
        /// 初始化走字项目，不包含升源操作
        /// </summary>
        /// <param name="IsOnOff">指示具体表位是否需要试验该项目，表位号=下标+1</param>
        /// <param name="glfx">功率方向</param>
        /// <returns></returns>
        public bool InitZZ(bool[] IsOnOff, Cus_PowerFangXiang glfx, CLDC_Comm.Enum.Cus_GyGyType[] im, int[] impluseCount)
        {
            if (GlobalUnit.IsDemo) return true;
            return iDriver.InitZZ(IsOnOff, glfx, im, impluseCount);

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
        public bool FuncMstate(int funcType)
        {
            return iDriver.FuncMstate(funcType);
        }
        /// <summary>
        /// 供电类型，耐压供电=1、载波供电=2、普通供电=3
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="meterType">false直接式，true互感式</param>
        /// <returns></returns>
        public bool SetPowerSupplyType(int elementType, bool isMeterTypeHGQ)
        {
            if (GlobalUnit.IsDemo) return true;
            return iDriver.SetPowerSupplyType(elementType, isMeterTypeHGQ);
        }
        #region 李鑫 20200618
        public bool SetRelay(int[] switchOpen, int[] switchClose)
        {
            if (GlobalUnit.IsDemo) return true;
            return iDriver.SetRelay(switchOpen, switchClose);
        }

        /// <summary>
        /// 读实时测量数据
        /// </summary>
        /// <param name="Index">表位号</param>
        /// <param name="instValue">输出测量数据</param>
        /// <returns></returns>
        public bool ReadTemperature(int Index, out float[] instValue)
        {
            instValue = null;
            if (GlobalUnit.IsDemo) return true;
            return iDriver.ReadTemperature(Index,out instValue);
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
            if (GlobalUnit.IsDemo) return true;
            return iDriver.SetTemperature(Index, Flags, Temperatures);
        }

        /// <summary>
        /// 开风扇
        /// </summary>
        /// <param name="Flag">控制标志位</param>
        /// <returns></returns>
        public bool OpenFan(bool Flag)
        {
            if (GlobalUnit.IsDemo) return true;
            return iDriver.OpenFan(Flag);
        }
        /// <summary>
        /// 开锁
        /// </summary>
        /// <returns></returns>
        public bool OpenLock()
        {
            if (GlobalUnit.IsDemo) return true;
            return iDriver.OpenLock();
        }
        #endregion
        /// <summary>
        /// 初始化命令
        /// </summary>
        /// <param name="int_Fn">Fn</param>
        public bool PacketToCarrierInit(int int_Fn, bool state, int num)
        {

            return iDriver.PacketToCarrierInit(int_Fn, state,num);
        }

        /// <summary>
        /// 控制命令
        /// </summary>
        /// <param name="int_Fn">Fn</param>
        /// <param name="Data">数据域</param>
        public bool PacketToCarrierCtr(int int_Fn, byte[] Data, int num)
        {
            //  if (GlobalUnit.IsDemo) return;
            return iDriver.PacketToCarrierCtr(int_Fn, Data,num);
        }


        /// <summary>
        /// 控制命令
        /// </summary>
        /// <param name="int_Fn">Fn</param>
        /// <param name="Data">数据域</param>
        public bool PacketToCarrierAddAddr(int int_Fn, byte[] Data, bool state, int num)
        {
            //  if (GlobalUnit.IsDemo) return;
            return iDriver.PacketToCarrierAddAddr(int_Fn, Data,state,num);
        }

        /// <summary>
        /// 不带数据域，返回确认/否认帧的命令
        /// </summary>
        /// <param name="int_Fn">Fn</param>
        public bool PacketToCarrierInitA(byte byt_AFN, int int_Fn, int num)
        {

            return iDriver.PacketToCarrierInitA(byt_AFN, int_Fn,num);
        }
        /// <summary>
        /// 打包645成376.2
        /// </summary>
        /// <param name="int_Fn">Fn</param>
        /// <param name="Data">电表地址，反转</param>
        public bool PacketTo3762Frame(byte[] Frame645, byte byt_DLTType, ref byte[] RFrame645, bool state, int int_BwIndex, int num)
        {
            RFrame645 = null;
            return iDriver.PacketTo3762Frame(Frame645, byt_DLTType, ref RFrame645, state, int_BwIndex,num);
        }

        /// <summary>
        /// 打包645成376.2
        /// </summary>
        /// <param name="int_Fn">Fn</param>
        /// <param name="Data">电表地址，反转</param>
        public static bool Packet645To3762Frame(byte[] Frame645, byte byt_DLTType, ref byte[] RFrame645, bool state, int int_BwIndex, int num)
        {
            RFrame645 = null;
            return iDriver.PacketTo3762Frame(Frame645, byt_DLTType, ref RFrame645, state, int_BwIndex,num);
        }

        /// <summary>
        /// 切换时钟脉冲
        /// </summary>
        /// <param name="isTime">是否切换为时钟脉冲</param>
        public bool SetTimeMaiCon(bool isTime)
        {
            if (GlobalUnit.IsDemo) return true;
            return iDriver.SetTimeMaiCon(isTime);
        }


        /// <summary>
        /// 初始化需量周期项目、不包含升源操作
        /// </summary>
        /// <param name="IsOnOff"></param>
        /// <param name="xlzqSeconds">需量周期</param>
        /// <param name="hccs">滑差次数</param>
        /// <returns></returns>
        public bool InitDemandPeriod(bool[] IsOnOff, CLDC_Comm.Enum.Cus_GyGyType[] im, int[] xlzqSeconds, int[] hccs)
        {
            if (GlobalUnit.IsDemo) return true;
            return iDriver.InitDemandPeriod(IsOnOff, im, xlzqSeconds, hccs);

        }


        /// <summary>
        /// 读取标准时间
        /// </summary>
        /// <returns></returns>
        public DateTime ReadGPSTime()
        {
            if (GlobalUnit.IsDemo) return DateTime.Now;
            return iDriver.ReadGPSTime();
        }


        /// <summary>
        /// 读取功耗
        /// </summary>
        /// <param name="int_BwIndex">功耗板ID，一般等于表位号</param>
        /// <param name="byt_Chancel">通道号，1=A相电压,2=A相电流,3=B相电压,4=B相电流,5=C相电压,6=C相电流</param>
        /// <param name="flt_PD">传出，float[4]{电压有效值,电流有效值,基波有功功率,基波无功功率}</param>
        public bool ReadPowerDissipation(int int_BwIndex, byte byt_Chancel, out float[] flt_PD)
        {

            return iDriver.ReadPowerDissipation(int_BwIndex, byt_Chancel, out flt_PD);
        }

        /// <summary>
        /// 读取误差板的功耗数据
        /// </summary>
        /// <param name="blnBwIndex">要读的表位</param>
        /// <param name="flt_PD">出数据结构</param>
        public void ReadErrPltGHPram(bool[] blnBwIndex, out CLDC_DataCore.Struct.stGHPram[] flt_PD)
        {

            iDriver.ReadErrPltGHPram(blnBwIndex, out flt_PD);
        }

        /// <summary>
        /// 设置谐波参数
        /// </summary>
        /// <param name="int_XSwitch">各相开关，数组元素值：0=不加谐波，1=加谐波，数组各元素：0=A相电压，1=B相电压，2=C相电压，3=A相电流，4=B相电流，5=C相电流</param>
        /// <param name="int_XTSwitch">各相各次开关,数组元素值：0=不加谐波，1=加谐波,数组各元素：各相（0-5），各次（0-64）</param>
        /// <param name="sng_Value">各次谐波含量</param>
        /// <param name="sng_Phase">各次谐波相位</param>
        /// <returns>设置谐波参数是否成功</returns>
        public bool SetHarmonic(int[][] int_XTSwitch, float[][] sng_Value, float[][] sng_Phase)
        {
            if (CLDC_DataCore.Const.GlobalUnit.g_SystemConfig.SystemMode.getValue("ISDEMO") == "演示模式") return true;
            return iDriver.SetHarmonic(int_XTSwitch, sng_Value, sng_Phase);
        }

        public bool SetHarmonicSwitch(bool bSwitch)
        {
            if (CLDC_DataCore.Const.GlobalUnit.g_SystemConfig.SystemMode.getValue("ISDEMO") == "演示模式") return true;
            return iDriver.SetHarmonicSwitch(bSwitch);
        }

        /// <summary>
        /// 设置尖顶波-1 平顶波-2
        /// </summary>
        /// <returns></returns>
        public bool SetJd_Pd(int ua, int ia, int ub, int ib, int uc, int ic)
        {
            if (CLDC_DataCore.Const.GlobalUnit.g_SystemConfig.SystemMode.getValue("ISDEMO") == "演示模式") return true;
            return iDriver.SetJd_Pd(ua, ia, ub, ib, uc, ic);
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
            if (CLDC_DataCore.Const.GlobalUnit.g_SystemConfig.SystemMode.getValue("ISDEMO") == "演示模式") return true;
            return iDriver.SettingWaveformSelection(ua, ia, ub, ib, uc, ic);
        }
        public bool ReadQueryCurrentErrorControl(int id, int CheckType, out int ErrNum, out string ErrData, out string TQtime)
        {
          //  if (CLDC_DataCore.Const.GlobalUnit.g_SystemConfig.SystemMode.getValue("ISDEMO") == "演示模式") return true;
            return iDriver.ReadQueryCurrentErrorControl( id,  CheckType, out  ErrNum, out  ErrData, out  TQtime);
        }
        /// <summary>
        /// 设置标准表界面 1：谐波柱图界面2：谐波列表界面3：波形界面4：清除设置界面
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="FrameAry">输出报文</param>
        /// <returns></returns>
        public bool SetDisplayFormControl(int formType)
        {
            return iDriver.SetDisplayFormControl(formType);
        }

          /// <summary>
        /// 读取各相电压电流谐波幅值（分两帧读取数据）
        /// </summary>
        /// <param name="phase">相别，0是C相电压，1是B相电压，2是A相电压，3是C相电流，4是B相电流，5是A相电流</param>
        /// <param name="harmonicArry"></param>
        /// <returns></returns>
        public bool ReadHarmonicArryControl(int phase, out float[] harmonicArry)
        {
            return iDriver.ReadHarmonicArryControl( phase, out  harmonicArry);
        }
        #region 电压暂降，电流负载快速变化

        /// <summary>
        /// 设置暂降电压电流阀值
        /// </summary>
        /// <param name="Wave">float[6]类型 </param>
        /// Wave[0] ua  ;Wave[1] ub;Wave[2] uc;Wave[3] ia;Wave[4] ib;Wave[5] ic
        /// <returns></returns>
        public bool SetDropWave(float[] Wave)
        {
            return iDriver.SetDropWave(Wave);
        }

        /// <summary>
        /// 设置暂降电压电流时间
        /// </summary>
        /// <param name="Time">int[2]</param>
        /// Ua,Ub,Uc,Ia,Ib,Ic
        /// /// <returns></returns>
        public bool SetDropTime(int[] Time)
        {
            return iDriver.SetDropTime(Time);
        }

        /// <summary>
        /// 设置暂降电压电流开关
        /// </summary>
        /// <param name="Switch">bool[6]</param>
        ///  bool[0] ua  ;bool[1] ub;bool[2] uc;bool[3] ia;bool[4] ib;bool[5] ic
        /// <returns></returns>
        public bool SetDropSwitch(bool[] Switch)
        {
            return iDriver.SetDropSwitch(Switch);
        }

        #endregion
        #region 远程上电
        /// <summary>
        /// 远程控制供电，远程上电
        /// </summary>
        /// <param name="OnOrOff">true 上电，false 断电</param>
        public void RemoteControlOnOrOff(bool OnOrOff)
        {
            iDriver.RemoteControlOnOrOff(OnOrOff);
        }
        #endregion

        #endregion

        #region 南网统一接口
        /// <summary>
        /// 控制表位和误差板
        /// </summary>
        /// <param name="TypeNo">功能号：1进入跳闸检测、2进入合闸检测、3读取外置继电器状态(使用出参)、4控制开路检测功能断开继电器命令、5控制开路检测功能启用继电器命令、6切换到220V输出外置式跳闸、7切换到开关量输出跳闸</param>
        /// <returns></returns>
        public bool SetRelayControl(int TypeNo)
        {
            return iDriver.SetRelayControl(TypeNo);
        }

        /// <summary>
        /// 控制表位继电器是否旁路
        /// </summary>
        /// <param name="ControlType">旁路继电器状态:1：旁路 0：正常</param>
        /// <returns></returns>
        public bool SetLoadRelayControl(bool[] MeterPosition, int ControlType)
        {
            return iDriver.SetLoadRelayControl(MeterPosition, ControlType);
        }

        /// <summary>
        /// 检定装置参数配置界面
        /// </summary>
        public void ShowDriverConfig()
        {
            iDriver.ShowDriverConfig();
        }


        /// <summary>
        /// 读写卡器参数配置
        /// </summary>
        public void ShowCardReaderConfig()
        {
            iDriver.ShowCardReaderConfig();
        }

        #endregion


        public bool SetErrCalcType(int calcType)
        {
            return iDriver.SetErrCalcType(calcType);
        }

        public bool ReadTestEnergy(out float testEnergy, out long PulseNum)
        {
            return iDriver.ReadTestEnergy(out testEnergy, out PulseNum);
        }
    }
}
