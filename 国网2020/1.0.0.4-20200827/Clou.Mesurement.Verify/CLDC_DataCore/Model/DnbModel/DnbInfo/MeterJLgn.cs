using System;
namespace CLDC_DataCore.Model.DnbModel.DnbInfo
{
    /// <summary>
    /// 计量功能
    /// </summary>
    [Serializable()]
    public class MeterJLgn : MeterErrorBase
    {
        
        /// <summary>
        /// 项目名称
        /// </summary>
        public string Mjl_chrProjectName = "";
        /// <summary>
        /// 组别
        /// </summary>
        public string Mjl_chrGrpType = "";
        /// <summary>
        /// 项目号
        /// </summary>
        public string Mjl_chrListNo = "";
        /// <summary>
        /// 小项目号	
        /// </summary>
        public short Mjl_intItemType = 0;
        /// <summary>
        /// 分项结论
        /// </summary>
        public string Mjl_chrItemJL = "";
        /// <summary>
        /// 电能与有功组合方式结论 
        /// </summary>
        public string Mjl_chrYgZhzJL = "";
        /// <summary>
        /// 电能与无功组合方式1结论	
        /// </summary>
        public string Mjl_chrWgZhZ1JL = "";
        /// <summary>
        /// 电能与无功组合方式2结论
        /// </summary>
        public string Mjl_chrWgZhZ2JL = "";
        /// <summary>
        /// 转存结论
        /// </summary>
        public string Mjl_chrZcJL = "";
        /// <summary>
        /// 停电结论	
        /// </summary>
        public string Mjl_chrTdJL = "";
        /// <summary>
        /// 分相计量结论
        /// </summary>
        public string Mjl_chrFxZjJL = "";
        /// <summary>
        /// 上10次有功组合方式结论
        /// </summary>
        public string Mjl_chrYgZhz10JL = "";
        /// <summary>
        /// 上10次无功组合方式1结论	
        /// </summary>
        public string Mjl_chrWgZhz110JL = "";
        /// <summary>
        /// 上10次无功组合方式2结论
        /// </summary>
        public string Mjl_chrWgZhz210JL = "";
        /// <summary>
        /// 结算日编程结论
        /// </summary>
        public string Mjl_chrJsrBcJL = "";
        /// <summary>
        /// 有功组合状态字	
        /// </summary>
        public string Mjl_chrYgZhZ = "";
        /// <summary>
        /// 无功组合方式1
        /// </summary>
        public string Mjl_chrWgZhZ1 = "";
        /// <summary>
        /// 无功组合方式2
        /// </summary>
        public string Mjl_chrWgZhZ2 = "";
        /// <summary>
        /// 组合有功	
        /// </summary>
        public string Mjl_chrZhP = "";
        /// <summary>
        /// P+
        /// </summary>
        public string Mjl_chrPz = "";
        /// <summary>
        /// P-
        /// </summary>
        public string Mjl_chrPf = "";
        /// <summary>
        /// 组合无功1	
        /// </summary>
        public string Mjl_chrZhQ1 = "";
        /// <summary>
        /// 组合无功2
        /// </summary>
        public string Mjl_chrZhQ2 = "";
        /// <summary>
        /// 第一象限无功电能
        /// </summary>
        public string Mjl_chrXX1 = "";
        /// <summary>
        /// 第二象限无功电能	
        /// </summary>
        public string Mjl_chrXX2 = "";
        /// <summary>
        /// 第三象限无功电能
        /// </summary>
        public string Mjl_chrXX3 = "";
        /// <summary>
        /// 第四象限无功电能
        /// </summary>
        public string Mjl_chrXX4 = "";
        /// <summary>
        /// 备份1	
        /// </summary>
        public string Mjl_chrOther1 = "";
        /// <summary>
        /// 备份2
        /// </summary>
        public string Mjl_chrOther2 = "";
        /// <summary>
        /// 备份3
        /// </summary>
        public string Mjl_chrOther3 = "";
        /// <summary>
        /// 备份4
        /// </summary>
        public string Mjl_chrOther4 = "";
        /// <summary>
        /// 备份5
        /// </summary>
        public string Mjl_chrOther5 = "";
    }
}
