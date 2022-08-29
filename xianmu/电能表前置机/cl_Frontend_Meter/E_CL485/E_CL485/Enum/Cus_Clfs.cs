using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CLOU.Enum
{
    /// <summary>
    /// 测量方式
    /// 
    ///   三相四线 = 0,
    ///   三相三线 = 1,
    ///   二元件跨相90 = 2,
    ///   二元件跨相60 = 3,
    ///   三元件跨相90 = 4,
    ///   单相 = 5,
    ///   
    /// </summary>
    [ComVisible(true)]
    public enum Cus_Clfs
    {
        /// <summary>
        /// 测量方式-三相四线
        /// </summary>
        PT4 = 0,
        /// <summary>
        /// 测量方式-三相三线
        /// </summary>
        PT3 = 1,        
        /// <summary>
        /// 测量方式-二元件跨相90
        /// </summary>
        EK90 = 2,
        /// <summary>
        /// 测量方式-二元件跨相60
        /// </summary>
        EK60 = 3,
        /// <summary>
        /// 测量方式-三元件跨相90
        /// </summary>
        SK90 = 4,
        /// <summary>
        /// 测量方式-单相
        /// </summary>
        P = 5
    }


    /// <summary>
    /// 电能误差通道号
    /// </summary>
    public enum Cus_MeterWcChannelNo
    {
        /// <summary>
        /// 
        /// </summary>
        正向有功 = 0,
        /// <summary>
        /// 
        /// </summary>
        正向无功 = 2,
        /// <summary>
        /// 
        /// </summary>
        反向有功 = 1,
        /// <summary>
        /// 
        /// </summary>
        反向无功 = 3,
        /// <summary>
        /// 
        /// </summary>
        需量 = 4,
        /// <summary>
        /// 
        /// </summary>
        时钟 = 5
    }

    /// <summary>
    /// 多功能误差通道号
    /// </summary>
    public enum Cus_DgnWcChannelNo
    {
        /// <summary>
        /// 
        /// </summary>
        电能误差 = 0,
        /// <summary>
        /// 
        /// </summary>
        日计时脉冲 = 1,
        /// <summary>
        /// 
        /// </summary>
        需量脉冲 = 2
    }

    /// <summary>
    /// 检定类型
    /// </summary>
    public enum Cus_CheckType
    {
        /// <summary>
        /// 
        /// </summary>
        电能误差 = 0,
        /// <summary>
        /// 
        /// </summary>
        需量误差 = 1,
        /// <summary>
        /// 
        /// </summary>
        日计时误差 = 2,
        /// <summary>
        /// 
        /// </summary>
        脉冲计数 = 3,
        /// <summary>
        /// 
        /// </summary>
        对标 = 4,
        /// <summary>
        /// 
        /// </summary>
        预付费功能检定 = 5,
        /// <summary>
        /// 
        /// </summary>
        耐压实验 = 6,
        /// <summary>
        /// 
        /// </summary>
        多功能脉冲计数试验 = 7

    }

    /// <summary>
    /// 光电头选择位
    /// </summary>
    public enum Cus_PulseType
    {
        /// <summary>
        /// 
        /// </summary>
        脉冲盒 = 0,
        /// <summary>
        /// 
        /// </summary>
        光电头 = 1
    }


    /// <summary>
    /// 电流的输出回路
    /// </summary>
    public enum Cus_BothIRoadType
    {
        /// <summary>
        /// 第一个电流回路
        /// </summary>
        第一个电流回路 = 0,
        /// <summary>
        /// 第二个电流回路
        /// </summary>
        第二个电流回路 = 1,
    }
    /// <summary>
    /// 电表电压回路选择
    /// </summary>
    public enum Cus_BothVRoadType
    {
        /// <summary>
        /// 直接接入式
        /// </summary>
        直接接入式 = 0,
        /// <summary>
        /// 互感器接入式
        /// </summary>
        互感器接入式 = 1,
        /// <summary>
        /// 本表位无电表接入
        /// </summary>
        本表位无电表接入 = 2
    }
}
