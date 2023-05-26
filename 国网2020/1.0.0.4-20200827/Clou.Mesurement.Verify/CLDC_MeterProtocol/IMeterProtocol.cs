using System;

namespace CLDC_MeterProtocol
{
    /// <summary>
    /// 电能表多功能协议接口
    /// </summary>
    public interface IMeterProtocol
    {
        /// <summary>
        /// 设置通讯端口名称
        /// </summary>
        /// <param name="szPortName">通讯端口名称</param>
        void SetPortName(string szPortName);
        /// <summary>
        /// 设置脉冲端子
        /// </summary>
        /// <param name="ecp_PulseType">端子输出脉冲类型</param>
        /// <returns>设置是否成功</returns>
        bool SetPulseCom(byte ecp_PulseType);
        /// <summary>
        /// 设置表地址
        /// </summary>
        /// <param name="szMeterAddress">电表地址</param>
        void SetMeterAddress(string szMeterAddress);

        /// <summary>
        /// 设置表MAC地址
        /// </summary>
        /// <param name="szMeterAddress">电表地址</param>
        void SetMeterAddress_MAC(string szMeterAddress_MAC);

        bool ConnectBlueTooth(string strAddress_MAC);
        /// <summary>
        /// 设置电能表协议
        /// </summary>
        /// <param name="protocol">电能表协议</param>
        /// <returns>设置电能表协议是否成功</returns>
        bool SetProtocol(CLDC_DataCore.Model.DgnProtocol.DgnProtocolInfo protocol);

        /// <summary>
        /// 读取电量
        /// </summary>
        /// <param name="energyType">功率类型，0=P+ 1=P- 2=Q+ 3=Q- 4=Q1 5=Q2 6=Q3 6=Q4</param>
        /// <param name="tariffType">费率类型，0-4,顺序根据协议内费率顺序转换,对于Q1 Q2 Q3 Q4本参数无效</param>
        /// <returns>返回电量</returns>
        float ReadEnergy(byte energyType, byte tariffType);


        /// <summary>
        /// 读取电量(所有费率读取)
        /// </summary>
        /// <param name="energyType">功率类型，0=P+ 1=P- 2=Q+ 3=Q- 4=Q1 5=Q2 6=Q3 6=Q4</param>
        /// <returns>返回电量,当energyType小于4时，返回一个长度为5的数组，反之返回一个长度为1的数组</returns>
        float[] ReadEnergys(byte energyType, int int_FreezeTimes);
        /// <summary>
        /// 清空需量
        /// </summary>
        /// <param name="str_Endata">密文</param>
        /// <returns>是否成功</returns>
        bool ClearDemand(string str_Endata);

        /// <summary>
        /// 清空需量
        /// </summary>
        /// <returns>是否成功</returns>
        bool ClearDemand();

        /// <summary>
        /// 读取需量(分费率读取)
        /// </summary>
        /// <param name="energyType">功率类型，0=P+ 1=P- 2=Q+ 3=Q- 4=Q1 5=Q2 6=Q3 6=Q4</param>
        /// <param name="tariffType">费率类型，0=总，1=峰，2=平，3=谷，4=尖</param>
        /// <returns>返回需量</returns>
        float ReadDemand(byte energyType, byte tariffType);


        

        /// <summary>
        /// 读取需量(所有费率读取)
        /// </summary>
        /// <param name="ept_DirectType">功率类型，0=P+ 1=P- 2=Q+ 3=Q- 4=Q1 5=Q2 6=Q3 6=Q4</param>
        /// <returns>返回需量</returns>
        float[] ReadDemands(byte energyType, int int_FreezeTimes);
        /// <summary>
        /// 读日期时间
        /// </summary>
        /// <returns>读取到的电表时间</returns>
        DateTime ReadDateTime();


        /// <summary>
        /// 读地址
        /// </summary>
        /// <returns>返回地址</returns>
        string ReadAddress();



        /// <summary>
        /// 读取数据（数据型，数据项）
        /// </summary>
        /// <param name="str_ID">标识符,2个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="int_Dot">小数位</param>
        /// <returns>返回数据</returns>
        float ReadData(string str_ID, int int_Len, int int_Dot);


        /// <summary>
        /// 读取数据（字符型，数据项）
        /// </summary>
        /// <param name="str_ID">标识符,两个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="bln_Reverse">解释方式，true=高低位对调，false=高低位正常</param>
        /// <param name="str_Value">返回数据</param>
        /// <returns></returns>
        string ReadData(string str_ID, int int_Len, string strItem);
        /// <summary>
        /// 读取数据（字符型，数据项）
        /// </summary>
        /// <param name="str_ID">标识符,2个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <returns></returns>
        string ReadData(string str_ID, int int_Len);

        /// <summary>
        /// 读取数据（字符型，数据项）
        /// </summary>
        /// <param name="str_ID">标识符,2个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="revData">返回报文</param>
        /// <returns></returns>
        string ReadData(string str_ID, int int_Len ,out string revData);
        /// <summary>
        /// 读取数据（字符型）
        /// </summary>
        /// <param name="sendData">发送侦</param>
        /// <returns></returns>
        string ReadData(string sendData);


        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="str_ID">标识符</param>
        /// <param name="byt_Value">写入数据</param>
        /// <returns>是否成功</returns>
        bool WriteData( string str_ID, byte[] byt_Value);

        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="str_ID">标识符</param>
        /// <param name="byt_Value">写入数据</param>
        /// <returns>是否成功</returns>
        bool WriteDataByMac(string str_ID, byte[] byt_Value);

        /// <summary>
        /// 写数据(字符型，数据项)
        /// </summary>
        /// <param name="str_ID">标识符,两个字节</param>
        /// <param name="int_Len">数据长度(块中每项字节数)</param>
        /// <param name="str_Value">写入数据</param>
        /// <returns>是否成功</returns>
        bool WriteData( string str_ID, int int_Len, string str_Value);




        /// <summary>
        /// 写数据(字符型，数据块)
        /// </summary>
        /// <param name="str_ID">标识符</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="str_Value">写入数据</param>
        /// <returns>是否成功</returns>
        bool WriteData( string str_ID, int int_Len, string[] str_Value);

        /// <summary>
        /// 写日期时间
        /// </summary>
        /// <param name="str_DateTime">日期时间</param>
        /// <returns>是否成功</returns>
        bool WriteDateTime(string str_DateTime);
        bool WriteRatesPrice(string str_ID, byte[] byt_Value);

        /// <summary>
        /// 清空电量
        /// </summary>
        /// <param name="str_Endata">密文</param>
        /// <returns>是否成功</returns>
        bool ClearEnergy(string str_Endata);

        /// <summary>
        /// 密钥下装指令
        /// </summary>
        /// <param name="byt_Addr">地址</param>
        /// <param name="byt_Cmd">命令字</param>
        /// <param name="byt_Data">数据域</param>
        /// <param name="bln_Sequela">是否有后续帧</param>
        /// <param name="byt_RevDataF">返回帧数据域</param>
        /// <returns></returns>
        bool UpdateRemoteEncryptionCommand(byte byt_Cmd, byte[] byt_Data, ref bool bln_Sequela, ref byte[] byt_RevDataF);

        /// <summary>
        /// 密钥下装指令 交互终端专用
        /// </summary>
        /// <param name="byt_Addr">地址</param>
        /// <param name="byt_Cmd">命令字</param>
        /// <param name="byt_Data">数据域</param>
        /// <param name="bln_Sequela">是否有后续帧</param>
        /// <param name="byt_RevDataF">返回帧数据域</param>
        /// <returns></returns>
        bool UpdateRemoteEncryptionCommandByTerminal(byte byt_Cmd, byte[] byt_Data, ref bool bln_Sequela, ref byte[] byt_RevDataF);


        /// <summary>
        /// 读取冻结模式字
        /// </summary>
        /// <param name="int_type">读取类型0=块读，1=分项读</param>
        /// <param name="int_PatternType">模式字类型1=定时冻结，2=瞬时冻结，3=日冻结，4=约定冻结，5=整点冻结</param>
        /// <param name="str_PatternWord">返回冻结模式字</param>
        /// <returns></returns>
        bool ReadPatternWord(int int_type, int int_PatternType, ref string str_PatternWord);


        /// <summary>
        /// 读取特殊电量
        /// </summary>
        /// <param name="int_type">读取类型0=块读，1=分项读</param>
        /// <param name="int_DLType">读取类型 1=剩余电量，2=透支电量，3=(上1次)定时冻结正向有功电能,4=(上1次)日冻结正向有功电能
        /// 5=(上1次)整点冻结正向有功总电能,6=(上1次)瞬时冻结正向有功电能</param>
        /// <param name="int_Times">第几次</param>
        /// <param name="str_PatternWord">返回特殊电量</param>
        /// <returns></returns>
        bool ReadSpecialEnergy(int int_type, int int_DLType, int int_Times, ref float[] flt_CurDL);
        /// <summary>
        /// 设置切换时间
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="int_SwitchType">切换时间类型 1=两套时区表切换时间，2=两套日时段表切换时间，3=两套费率电价切换时间，4=两套梯度切换时间,5=整点冻结起始时间 6=时间间隔 </param>
        /// <param name="str_Time">时间[YYMMDDhhmm]</param>
        /// <returns></returns>
        bool WriteFreezeInterval(int int_PatternType, string str_DateTime);

        /// <summary>
        /// 冻结命令
        /// </summary>
        /// <param name="str_DateHour">冻结时间，MMDDhhmm(月.日.时.分)</param>
        /// <returns></returns>
        bool FreezeCmd(string str_DateHour);
        /// <summary>
        /// 读取数据（数据型，数据块）
        /// </summary>
        /// <param name="str_ID">标识符,2个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="int_Dot">小数位</param>
        /// <returns>返回数据</returns>
        float[] ReadDataBlock(string str_ID, int int_Len, int int_Dot);

        /// <summary>
        /// 广播校时
        /// </summary>
        /// <param name="broadCaseTime">广播校准时间</param>
        /// <returns>广播校时是否成功</returns>
        bool BroadcastTime(DateTime broadCaseTime);

        /// <summary>
        /// 点对点广播校时
        /// </summary>
        /// <param name="broadCaseTime">广播校准时间</param>
        /// <returns>广播校时是否成功</returns>
        bool BroadcastTimeByPoint(DateTime broadCaseTime);
        /// <summary>
        /// 设置电表拉闸心跳帧
        /// </summary>
        /// <returns></returns>
        bool SetBreakRelayTime(int Time);

        //模式字类型1=定时冻结，2=瞬时冻结，4=约定冻结，5=整点冻结，3=日冻结
        bool WritePatternWord(int int_PatternType, string data);

        /// <summary>
        /// 读取上一次冻结时间
        /// </summary>
        /// <param name="int_Type">类型0=块读(hhmmNN)，1=块读(NNhhmm)</param>
        /// <param name="int_FreezeType">类型 1=整点冻结时间，2=日冻结时间，3=定时冻结时间,4=整点冻结起始时间</param>
        /// <param name="str_PTime">返回时间</param>
        /// <returns></returns>
        bool ReadFreezeTime(int int_FreezeType, ref string str_FreezeTime);

        /// <summary>
        /// 读取冻结时间间隔
        /// </summary>
        /// <param name="int_Type">类型0=块读(hhmmNN)，1=块读(NNhhmm)</param>
        /// <param name="str_PTime">返回时段</param>
        /// <returns></returns>
        float[] ReadFreezeInterval(int int_Type, ref string str_FTime);

        /// <summary>
        /// 清空电量
        /// </summary>
        /// <returns>是否成功</returns>
        bool ClearEnergy();


        /// <summary>
        /// 清空事件记录
        /// </summary>
        /// <param name="str_ID">事件清零内容</param>
        /// <returns>是否成功</returns>
        bool ClearEventLog(string str_ID);

        /// <summary>
        /// 清空事件记录
        /// </summary>
        /// <param name="str_ID">事件清零内容</param>
        /// <param name="strEndata">密文</param>
        /// <returns></returns>
        bool ClearEventLog(string str_ID, string strEndata);
    }
}
