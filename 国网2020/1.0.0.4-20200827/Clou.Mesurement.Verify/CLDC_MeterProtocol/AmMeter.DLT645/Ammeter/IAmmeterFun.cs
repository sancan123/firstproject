/*-----------------------------------------------------------------------------------
 * Copyright(C) 2011 深圳市科陆软件有限公司电测事业部
 * 文件名: IAmmeterFun.cs
 * 文件功能描述: 电能表功能接口
 * 创建标识: ShiHe 20110316
 * 修改标识:
 * 修改描述:
 *-----------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;
using CLDC.CLAT.Framework.Struct;
using CLDC_MeterProtocol.Ammeter.DLT645.Comm.Struct;
using CLDC_MeterProtocol.Ammeter.DLT6452007;// 陈大伟 2011-8-15

namespace CLDC_MeterProtocol.Ammeter
{
    /// <summary>
    /// 电能表功能接口
    /// </summary>
    public interface IAmmeterFun
    {
        /// <summary>
        /// 密码
        /// </summary>
        string PassWord { get; set; }
        /// <summary>
        /// 操作者代码
        /// </summary>
        string HandleCode { get; set; }

        /// <summary>
        /// 加密
        /// </summary>
        IEncrypt DataEncrypt { get; set; }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="dataCode">数据标识</param>
        /// <returns>帧长度</returns>
        int ReadData(out byte[] frameData, string commAddr, string dataCode);

        /// <summary>
        /// 写电能表数据
        /// </summary>
        /// <param name="cellID">单元编号</param>
        /// <param name="meterID">表位号</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="passWord">密码</param>
        /// <param name="handleCode">操作者代码</param>
        /// <param name="dateTime">时间</param>
        /// <param name="medType">通讯媒介</param>
        /// <param name="dataState">数据状态</param>
        /// <returns>True-设置成功；False-设置失败</returns>        
        int WriteData(out byte[] frameData, string commAddr, string dataCode, string strWriteData, int dataSort);
        /// <summary>
        /// 读取通信地址指令
        /// </summary>
        /// <param name="frameData">帧信息</param>
        /// <returns>帧长度</returns>
        int GetCommAddr(out byte[] frameData);
        /// <summary>
        /// 读取各费率电能及总电能指令
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="enyAspect">电能方向[0-组合 1-正向 2-反向]</param>
        /// <param name="enyType">电能类型[0-视在 1-有功 2-无功]</param>
        /// <returns>帧长度</returns>
        int GetFullEnergy(out byte[] frameData, string commAddr, byte enyAspect, byte enyType);
        /// <summary>
        /// 读取需量寄存器
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="DmdAspect">电能方向[0-组合 1-正向 2-反向]</param>
        /// <param name="DmdType">电能类型[0-视在 1-有功 2-无功 3-无功2]</param>
        /// <returns></returns>
        int GetFunllDemand(out byte[] frameData, string commAddr, byte DmdAspect, byte DmdType);
        /// <summary>
        /// 读取各相电压指令
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>
        int GetFullUValue(out byte[] frameData, string commAddr);
        /// <summary>
        /// 读取各相电流指令
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>
        int GetFullIValue(out byte[] frameData, string commAddr);
        /// <summary>
        /// 读取剩余金额指令
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>
        int GetSpareMoney(out byte[] frameData, string commAddr);

        /// <summary>
        /// 读当前阶梯电价 0280000B XXXX.XXXX  元/kWh
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>
        int GetCurrPrice(out byte[] frameData, string commAddr);

        /// <summary>
        /// 读取剩余电量指令
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>
        int GetSpareEnergy(out byte[] frameData, string commAddr);
        /// <summary>
        /// 获取电能表运行状态
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>
        int GetRunState(out byte[] frameData, string commAddr);
        //runCode：运行状态字（0-数据块）
        int GetRunState(out byte[] frameData, string commAddr, byte runCode);
        /// <summary>
        /// 设置时间指令
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="ctyTime">时间</param>
        /// <returns>帧长度</returns>
        int SetTime(out byte[] frameData, string commAddr, DateTime ctyTime, int dataSort);
        /// <summary>
        /// 设置日期和星期指令
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="ctyDate">日期及星期</param>
        /// <returns>帧长度</returns>
        int SetDateAndWeek(out byte[] frameData, string commAddr, DateTime ctyDate, int dataSort);
        /// <summary>
        /// 校准准备指令
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>
        int GetAdjustPrepare(out byte[] frameData, string commAddr);
        /// <summary>
        /// 校准电能表指令
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="adjParam">校准参数</param>
        /// <returns>帧长度</returns>
        int GetAdjustMeter(out byte[] frameData, string commAddr, StAdjustParam adjParam);
        /// <summary>
        /// 控制电能表状态
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="ctlType">状态[0-跳闸 1-合闸 2-报警 3-报警解除 4-保电 5-保电解除]</param>
        /// <param name="alyTime">有效截止时间</param>
        /// <returns>帧长度</returns>
        int ControlState(out byte[] frameData, string commAddr, byte ctlType, DateTime alyTime);
        /// <summary>
        /// 最大需量清零
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns></returns>
        int ControlMaxDemandClear(out byte[] frameData, string commAddr);
        /// <summary>
        /// 控制电表清零
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <returns>帧长度</returns>
        int ControlMeterClear(out byte[] frameData, string commAddr);
        /// <summary>
        /// 控制多功能端子输出
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="pulseType">脉冲类型：00-时钟秒脉冲，01-需量周期，02-时段投切</param>
        /// <returns></returns>
        int ControlMultPulse(out byte[] frameData, string commAddr, byte pulseType);
        /// <summary>
        /// 身份认证指令
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="cryptograph1">密文1</param>
        /// <param name="sticNumber1">随机数1(主站)</param>
        /// <param name="sptGene">分散因子</param>
        /// <returns>帧长度</returns>
        int StatusAtic(out byte[] frameData, string commAddr, string cryptograph1, string sticNumber1, string sptGene);
        /// <summary>
        /// 解析随机数2和EASM序列号
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="sticNumber2">随机数2(电能表)</param>
        /// <param name="salNumber">ESAM序列号</param>
        /// <returns>True-解析成功；False-解析失败</returns>
        bool ParseSticNumAndEsam(byte[] frameData, ref string sticNumber2, ref string salNumber);

        /// <summary>
        /// 组帧(智能表状态查询)
        /// </summary>
        /// <returns>智能表状态查询命令帧</returns>
        byte[] WeaveFrame_CheckupState(string commAddr);

        /// <summary>
        /// 组帧(密钥下装)
        /// </summary>
        /// <param name="keyNo">下装密钥类型</param>
        /// <param name="keyInfo">密钥信息</param>
        /// <param name="macData">MAC</param>
        /// <param name="keyData">密钥数据</param>
        /// <returns>密钥下装命令帧</returns>
        byte[] WeaveFrame_UpdateKey(int keyNo, string commAddr, string keyInfo, string macData, string keyData);

        /// <summary>
        /// 更新密钥
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="keyType">密钥类型[1-控制命令密钥 2-参数密钥 3-远程身份认证密钥 4-主控密钥]</param>
        /// <param name="keyInfo">密钥信息</param>
        /// <param name="macAddr">MAC地址</param>
        /// <param name="keyData">密钥数据</param>
        /// <returns>帧长度</returns>
        int UpdateKey(out byte[] frameData, string commAddr, byte keyType, string keyInfo, string macAddr, string keyData);
        /// <summary>
        /// 密钥清零
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="macAddr">MAC地址</param>
        /// <param name="keyData">密钥数据</param>
        /// <returns>帧长度</returns>
        int ClearKey(out byte[] frameData, string commAddr, string macAddr, string keyData);
        /// <summary>跳合闸延时时间
        /// </summary>
        /// <param name="frameData"></param>
        /// <param name="commAddr"></param>
        /// <param name="cltype">0跳阐1合阐</param>
        /// <param name="times">延时时间</param>
        /// <returns></returns>
        int SetPullSwitchTime(out byte[] frameData, string commAddr, byte cltype, byte times, int dataSort);




        /// <summary>
        /// 拆帧(密钥下装)
        /// </summary>
        /// <param name="keyNo">下装密钥类型</param>
        /// <param name="cmdFrame">待解析的命令帧</param>
        /// <param name="frameText">返回的报文字符串</param>
        /// <param name="errText">错误信息字提示信息</param>
        /// <returns>[true-成功, false-失败]</returns>
        bool ParseFrame_UpdateKey(int keyNo, byte[] cmdFrame, out string frameText, out string errText);
        /// <summary>
        /// 回抄ESAM模块中文件数据
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /// <param name="fileType">文件类型[0-密钥文件 2-参数信息文件 5-密钥信息文件 6-运行信息文件 7-控制命令文件]</param>
        /// <returns>帧长度</returns>
        int GetEsamFile(out byte[] frameData, string commAddr, byte fileType);

        //2011-8-15 陈大伟
        /// <summary>回抄ESAM模块中文件数据
        /// </summary>
        /////// <param name="frameData">帧数据</param>
        /// <param name="commAddr">通讯地址</param>
        /////// <param name="fileType">文件类型[0-密钥文件 2-参数信息文件 5-密钥信息文件 6-运行信息文件 7-控制命令文件]</param>
        /// <returns>发送帧</returns>
        byte[] GetEsamFileData(string commAddr);
        /// <summary>
        /// 解析ESAM模块中运行信息文件的数据
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="fileData">文件数据</param>
        /// <param name="macAddr">MAC地址</param>
        /// <returns>True-解析成功；False-解析失败</returns>
        bool ParseEsamRunFile(byte[] frameData, ref StEsamRunFile fileData, ref string macAddr);

        /// <summary>
        /// 解析ESAM模块中运行信息文件的数据
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="fileData">文件数据</param>
        /// <param name="macAddr">MAC地址</param>
        /// <returns>True-解析成功；False-解析失败</returns>
        bool ParseEsamRunFileDataN(byte[] frameData, ref StEsamRunFile fileData, ref string macAddr);

        /// <summary>
        /// 解析ESAM模块中运行信息文件的数据
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="strData">密钥信息数据</param>
        /// <param name="macAddr">MAC地址</param>
        /// <returns>True-解析成功；False-解析失败</returns>
        bool ParseEsamRunFileData(byte[] frameData, ref string strData, ref string macAddr);
        /// <summary>
        /// 解析读取数据返回的数据域
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="errCode">错误编码</param>
        /// <returns>解析失败，返回空数组</returns>
        byte[] ParseDataRegion(byte[] frameData, ref int errCode);
        /// <summary>
        /// 解析电能表运行状态
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="runState">运行状态</param>
        /// <param name="errCode">错误编码</param>
        /// <returns>True-解析成功；False-解析失败</returns>
        bool ParseRunState(byte[] frameData, ref StMeterRunState runState, ref int errCode);
        //runCode：运行状态字（0-数据块）
        bool ParseRunState(byte[] frameData, byte runCode, ref StMeterRunState runState, ref int errCode);
        /// <summary>
        /// 解析通讯地址
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="errCode">错误编码</param>
        /// <returns>通讯地址</returns>
        string ParseCommAddr(byte[] frameData, ref int errCode);
        /// <summary>
        /// 解析剩余金额
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="errCode">错误编码</param>
        /// <returns>剩余金额（单位：元）</returns>
        float ParseSpareMoney(byte[] frameData, ref int errCode);


        /// <summary>
        /// 拆帧(智能表状态查询 ESAM:intType=1 密钥信息4 intType=2 客户编号6 intType=3 MAC4 intType=4 购电次数（ESAM内）4 intType=5 MAC4  intType=6 剩余金额（ESAM内）4 陈大伟 2011-8-14)
        /// </summary>
        /// <param name="intType">intType=1 密钥信息4 intType=2 客户编号6 intType=3 MAC4 intType=4 购电次数（ESAM内）4 intType=5 MAC4  intType=6 剩余金额（ESAM内）4</param>
        /// <param name="cmdFrame">待解析的命令帧</param>
        /// <param name="stateCode">返回的数据值</param>
        /// <param name="frameText">返回的报文字符串</param>
        /// <param name="errText">错误信息字提示信息</param>
        /// <returns>[true-成功, false-失败]</returns>
        bool ParseFrame_CheckupState(int intType, byte[] cmdFrame, ref string strRetDat, out string frameText,
            out string errText);
        /// <summary>
        /// 解析阶梯电价  陈大伟 2011-8-14
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="errCode">错误编码</param>
        /// <returns>当前阶梯电价 XXXX.XXXX 元/kWh</returns>
        float CurrJtdjMoney(byte[] frameData, ref int errCode);
        /// <summary>
        /// 解析剩余电量
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="errCode">错误编码</param>
        /// <returns>剩余电量（单位：kWh）</returns>
        float ParseSpareEnergy(byte[] frameData, ref int errCode);
        /// <summary>
        /// 解析各费率电能及总电能
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="errCode">错误编码</param>
        /// <returns>解析失败，返回空列表；下标为0表示总电能</returns>
        List<float> ParseFullEnergy(byte[] frameData, ref int errCode);
        /// <summary>
        /// 解析各费率需量及总需量
        /// </summary>
        /// <param name="frameData"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        List<float> ParseFullDemand(byte[] frameData, ref int errCode);
        /// <summary>
        /// 解析设参结果
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="errCode">错误编码</param>
        /// <returns>True-设参成功；False-设参失败</returns>
        bool ParseSetResult(byte[] frameData, ref int errCode);

    }
}
