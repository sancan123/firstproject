using System;

namespace CLDC_DeviceDriver.Drivers
{

    interface IDriver
    {
        #region 公共
        /// <summary>
        /// 
        /// </summary>
        MsgCallBack CallBack { get;set;}


        /// <summary>
        /// 立即停止
        /// </summary>
        /// <returns></returns>
        bool Stop();

        /// <summary>
        /// 联机操作
        /// </summary>
        /// <returns></returns>
        bool Link();

        
        #endregion

       
        
        /// <summary>
        /// 
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
        bool PowerOn(CLDC_Comm.Enum.Cus_Clfs clfs,CLDC_Comm.Enum.Cus_PowerFangXiang glfx,string strGlys,Single sng_Ub,Single sng_Ib,Single sng_IMax
            ,Single sng_xUb_A,Single sng_xUb_B,Single sng_xUb_C
            ,Single sng_xIb_A,Single sng_xIb_B,Single sng_xIb_C,
            CLDC_Comm.Enum.Cus_PowerYuanJian  element,
            Single sng_UaPhi,Single sng_UbPhi,Single sng_UcPhi,Single sng_IaPhi,Single sng_IbPhi,Single sng_IcPhi,
            Single sng_Freq,bool IsDuiBiao,bool IsQianDong,bool bln_IsNxx);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool PowerOff();

        //zhengrubin-20190920
        /// <summary>
        /// 初始化日计时误差、不包含升源操作
        /// </summary>
        /// <param name="IsOnOff">指示具体表位是否需要试验该项目，表位号=下标+1</param>
        /// <param name="im">脉冲类型</param>
        /// <param name="MeterFre">各表位时钟周期</param>
        /// <param name="bcs">表常数</param>
        /// <param name="quans">校验圈数</param>
        /// <returns></returns>
        bool InitTimeAccuracy(bool[] IsOnOff, CLDC_Comm.Enum.Cus_GyGyType[] im, float[] MeterFre, float[] bcs, int[] quans);

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
        bool InitError(CLDC_Comm.Enum.Cus_Clfs clfs, CLDC_Comm.Enum.Cus_PowerFangXiang glfx
            , int[] bcs, int[] quans, int wccs, CLDC_Comm.Enum.Cus_GyGyType[] im, bool[] IsOnOff);

        /// <summary>
        /// 初始化启动项目
        /// </summary>
        /// <param name="clfs">测量方式</param>
        /// <param name="glfx">功率方向</param>
        /// <param name="im">脉冲类型</param>
        /// <param name="IsOnOff">表位开关</param>
        /// <param name="startTimes">各表位起动时间【单位秒】</param>
        /// <returns>初始化是否成功</returns>
        bool InitStartUp(CLDC_Comm.Enum.Cus_Clfs clfs, CLDC_Comm.Enum.Cus_PowerFangXiang glfx, CLDC_Comm.Enum.Cus_GyGyType[] im, bool[] IsOnOff, int[] startTimes,int[] meterconst);


        /// <summary>
        /// 初始化潜动项目
        /// </summary>
        /// <param name="clfs">测量方式</param>
        /// <param name="glfx">功率方向</param>
        /// <param name="im">脉冲类型</param>
        /// <param name="IsOnOff">表位开关</param>
        /// <param name="startTimes">各表位起动时间【单位秒】</param>
        /// <returns></returns>
        bool InitCreeping(CLDC_Comm.Enum.Cus_Clfs clfs, CLDC_Comm.Enum.Cus_PowerFangXiang glfx, CLDC_Comm.Enum.Cus_GyGyType[] im, bool[] IsOnOff, int[] startTimes);
        /// <summary>
        /// 读取误差
        /// </summary>
        /// <returns></returns>
        stError[] ReadWcb(bool[] IsOnOff,bool state);


        stError ReadWc(int bw);
        /// <summary>
        /// 初始化走字项目，不包含升源操作
        /// </summary>
        /// <param name="IsOnOff">指示具体表位是否需要试验该项目，表位号=下标+1</param>
        /// <param name="glfx">功率方向</param>
        /// <returns></returns>
        bool InitZZ(bool[] IsOnOff, CLDC_Comm.Enum.Cus_PowerFangXiang glfx, CLDC_Comm.Enum.Cus_GyGyType[] im, int[] impluseCount);

        /// <summary>
        /// 供电类型，耐压供电=1、载波供电=2、普通供电=3
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="meterType">false直接式，true互感式</param>
        /// <returns></returns>
        bool SetPowerSupplyType(int elementType, bool isMeterTypeHGQ);
        #region 李鑫 20200618
        bool SetRelay(int[] switchOpen, int[] switchClose);

        /// <summary>
        /// 读实时测量数据
        /// </summary>
        /// <param name="Index">表位号</param>
        /// <param name="instValue">输出测量数据</param>
        /// <returns></returns>
        bool ReadTemperature(int Index, out float[] instValue);

        /// <summary>
        /// 设置温度
        /// </summary>
        /// <param name="Index">表位号</param>
        /// <param name="Flags">需要温控标志位</param>
        /// <param name="Temperatures">控制温度</param>
        /// <returns></returns>
        bool SetTemperature(int Index, bool[] Flags, float[] Temperatures);

        /// <summary>
        /// 开风扇
        /// </summary>
        /// <param name="Flag">控制标志位</param>
        /// <returns></returns>
        bool OpenFan(bool Flag);

        /// <summary>
        /// 开锁
        /// </summary>
        /// <returns></returns>
        bool OpenLock();
        #endregion
        /// <summary>
        /// 控制表位和误差板
        /// </summary>
        /// <param name="TypeNo">功能号：1进入跳闸检测、2进入合闸检测、3读取外置继电器状态(使用出参)、4控制开路检测功能断开继电器命令、5控制开路检测功能启用继电器命令、6切换到220V输出外置式跳闸、7切换到开关量输出跳闸</param>
        /// <returns></returns>
        bool SetRelayControl(int TypeNo);

        /// <summary>
        /// 控制表位继电器是否旁路
        /// </summary>
        /// <param name="MeterPosition">要检表位</param>
        /// <param name="ControlType">旁路继电器状态:1：旁路 0：正常</param>
        /// <returns></returns>
        bool SetLoadRelayControl(bool[] MeterPosition, int ControlType);

        /// <summary>
        /// 检定装置参数配置界面
        /// </summary>
        void ShowDriverConfig();

        /// <summary>
        /// 读写卡器参数配置
        /// </summary>
        void ShowCardReaderConfig();

        /// <summary>
        /// 初始化命令
        /// </summary>
        /// <param name="int_Fn">Fn</param>
        bool PacketToCarrierInit(int int_Fn, bool state, int num);


        /// <summary>
        /// 控制命令
        /// </summary>
        /// <param name="int_Fn">Fn</param>
        /// <param name="Data">数据域</param>
        bool PacketToCarrierCtr(int int_Fn, byte[] Data, int num);

        /// <summary>
        /// 添加载波从节点
        /// </summary>
        /// <param name="int_Fn">Fn</param>
        /// <param name="Data">电表地址，反转</param>
        bool PacketToCarrierAddAddr(int int_Fn, byte[] Data, bool state, int num);


        /// <summary>
        /// 不带数据域，返回确认/否认帧的命令
        /// </summary>
        /// <param name="byt_AFN">AFN</param>
        /// <param name="int_Fn">Fn</param>
        bool PacketToCarrierInitA(byte byt_AFN, int int_Fn, int num);
        /// <summary>
        /// 打包645成376.2
        /// </summary>
        /// <param name="int_Fn">Fn</param>
        /// <param name="Data">电表地址，反转</param>
        bool PacketTo3762Frame(byte[] Frame645, byte byt_DLTType, ref byte[] RFrame645, bool state, int int_BwIndex, int num);
        
        /// <summary>
        /// 切换时钟脉冲
        /// </summary>
        /// <param name="isTime">是否切换为时钟脉冲</param>
        bool SetTimeMaiCon(bool isTime);



        /// <summary>
        /// 初始化需量周期项目、不包含升源操作
        /// </summary>
        /// <param name="IsOnOff"></param>
        /// <param name="xlzqSeconds">需量周期</param>
        /// <param name="hccs">滑差次数</param>
        /// <returns></returns>
        bool InitDemandPeriod(bool[] IsOnOff, CLDC_Comm.Enum.Cus_GyGyType[] im, int[] xlzqSeconds, int[] hccs);

        /// <summary>
        /// 读取标准时间
        /// </summary>
        /// <returns></returns>
        DateTime ReadGPSTime();





        /// <summary>
        /// 读取误差板的功耗数据
        /// </summary>
        /// <param name="blnBwIndex">要读的表位</param>
        /// <param name="flt_PD">出数据结构</param>
        void ReadErrPltGHPram(bool[] blnBwIndex, out CLDC_DataCore.Struct.stGHPram[] flt_PD);

      


        #region 功耗
        /// <summary>
        /// 读取功耗
        /// </summary>
        /// <param name="int_BwIndex">功耗板ID，一般等于表位号</param>
        /// <param name="byt_Chancel">通道号，1=A相电压,2=A相电流,3=B相电压,4=B相电流,5=C相电压,6=C相电流</param>
        /// <param name="flt_PD">传出，float[4]{电压有效值,电流有效值,基波有功功率,基波无功功率}</param>
        bool ReadPowerDissipation(int int_BwIndex, byte byt_Chancel, out float[] flt_PD);


        /// <summary>
        /// 设置谐波参数
        /// </summary>
        /// <param name="int_XSwitch">各相开关，数组元素值：0=不加谐波，1=加谐波，数组各元素：0=A相电压，1=B相电压，2=C相电压，3=A相电流，4=B相电流，5=C相电流</param>
        /// <param name="int_XTSwitch">各相各次开关,数组元素值：0=不加谐波，1=加谐波,数组各元素：各相（0-5），各次（0-64）</param>
        /// <param name="sng_Value">各次谐波含量</param>
        /// <param name="sng_Phase">各次谐波相位</param>
        /// <returns>设置谐波参数是否成功</returns>
        bool SetHarmonic(int[][] int_XTSwitch, Single[][] sng_Value, Single[][] sng_Phase);

        /// <summary>
        /// 设置尖顶波-1 平顶波-2
        /// </summary>
        /// <returns></returns>
        bool SetHarmonicSwitch(bool bSwitch);
        bool SetJd_Pd(int ua, int ia, int ub, int ib, int uc, int ic);
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
        bool SettingWaveformSelection(int ua, int ia, int ub, int ib, int uc, int ic);
        #endregion 







        void RemoteControlOnOrOff(bool OnOrOff);
        
        stStdInfo ReadStdInfo();

        #region 功能接口
        /// <summary>
        /// 启动/停止当前设置的功能
        /// </summary>
        /// <param name="IsOnOff"></param>
        /// <returns></returns>
        bool SetCurFunctionOnOrOff(bool IsOnOff, byte state);
        #endregion



        bool ReadQueryCurrentErrorControl(int id, int CheckType, out int ErrNum, out string ErrData, out string TQtime);


        bool SetDisplayFormControl(int formType);

        bool ReadHarmonicArryControl(int phase, out float[] harmonicArry);
        #region 电压暂降，电流负载快速变化
        /// <summary>
        /// 设置暂降电压电流阀值
        /// </summary>
        /// <param name="Wave">float[6]类型 </param>
        /// Wave[0] ua  ;Wave[1] ub;Wave[2] uc;Wave[3] ia;Wave[4] ib;Wave[5] ic
        /// <returns></returns>
        bool SetDropWave(float[] Wave);

        /// <summary>
        /// 设置暂降电压电流时间
        /// </summary>
        /// <param name="Time">int[2]</param>
        /// Ua,Ub,Uc,Ia,Ib,Ic
        /// /// <returns></returns>
        bool SetDropTime(int[] Time);


        /// <summary>
        /// 设置暂降电压电流开关
        /// </summary>
        /// <param name="Switch">bool[6]</param>
        ///  bool[0] ua  ;bool[1] ub;bool[2] uc;bool[3] ia;bool[4] ib;bool[5] ic
        /// <returns></returns>
        bool SetDropSwitch(bool[] Switch);


        #endregion

        bool SetErrCalcType(int calcType);

        bool FuncMstate(int funcType);

        bool ReadTestEnergy(out float testEnergy, out long PulseNum);
    }
}
