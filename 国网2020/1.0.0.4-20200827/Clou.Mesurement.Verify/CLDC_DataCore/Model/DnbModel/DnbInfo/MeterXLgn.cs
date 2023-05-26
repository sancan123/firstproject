using System;
namespace CLDC_DataCore.Model.DnbModel.DnbInfo
{
    /// <summary>
    /// 需量功能
    /// </summary>
    [Serializable()]
    public class MeterXLgn : MeterErrorBase
    {
        
        /// <summary>
        /// 项目名称		
        /// </summary>
        public string Mx_chrProjectName = "";
        /// <summary>
        /// 组别
        /// </summary>
        public string Mx_chrGrpType = "";
        /// <summary>
        /// 项目号
        /// </summary>
        public string Mx_chrListNo = "";
        /// <summary>
        /// 小项目号
        /// </summary>
        public short Mx_intItemType = 0;
        /// <summary>
        /// 分项结论		
        /// </summary>
        public string Mx_chrItemJL = "";
        /// <summary>
        /// 方向和角度
        /// </summary>
        public string Mx_chrGK = "";
        /// <summary>
        /// 清零方法
        /// </summary>
        public string Mx_chrQLFF = "";
        /// <summary>
        /// 编程键		
        /// </summary>
        public string Mx_chrBCJ = "";
        /// <summary>
        /// 需量周期数据结论
        /// </summary>
        public string Mx_chrZqDatJL = "";
        /// <summary>
        /// 编程键影响结论
        /// </summary>
        public string Mx_chrBCJJL = "";
        /// <summary>
        /// 转存结论		
        /// </summary>
        public string Mx_chrZCJL = "";
        /// <summary>
        /// 清零记录结论
        /// </summary>
        public string Mx_chrQLJL = "";
        /// <summary>
        /// 当前记录结论
        /// </summary>
        public string Mx_chrDQJL = "";
        /// <summary>
        /// 上一次结算日P+		
        /// </summary>
        public string Mx_chrPzJS = "";
        /// <summary>
        /// 上一次结算日P-
        /// </summary>
        public string Mx_chrPfJS = "";
        /// <summary>
        /// 上一次结算日组合无功1
        /// </summary>
        public string Mx_chrZhQ1JS = "";
        /// <summary>
        /// 上一次结算日组合无功2		
        /// </summary>
        public string Mx_chrZhQ2JS = "";
        /// <summary>
        /// 上一次结算日第一象限无功需量
        /// </summary>
        public string Mx_chrXX1JS = "";
        /// <summary>
        /// 上一次结算日第二象限无功需量
        /// </summary>
        public string Mx_chrXX2JS = "";

        /// <summary>
        /// 上一次结算日第三象限无功需量
        /// </summary>
        public string Mx_chrXX3JS = "";
        /// <summary>
        /// 上一次结算日第四象限无功需量		
        /// </summary>
        public string Mx_chrXX4JS = "";
        /// <summary>
        /// P+
        /// </summary>
        public string Mx_chrPz = "";
        /// <summary>
        /// P-
        /// </summary>
        public string Mx_chrPf = "";
        /// <summary>
        /// 组合无功1
        /// </summary>
        public string Mx_chrZhQ1 = "";
        /// <summary>
        /// 组合无功2
        /// </summary>
        public string Mx_chrZhQ2 = "";
        /// <summary>
        /// 第一象限无功需量
        /// </summary>
        public string Mx_chrXX1 = "";
        /// <summary>
        /// 第二象限无功需量		
        /// </summary>
        public string Mx_chrXX2 = "";
        /// <summary>
        /// 第三象限无功需量
        /// </summary>
        public string Mx_chrXX3 = "";
        /// <summary>
        /// 第四象限无功需量
        /// </summary>
        public string Mx_chrXX4 = "";
        /// <summary>
        /// 备份1
        /// </summary>
        public string Mx_chrOther1 = "";
        /// <summary>
        /// 备份2
        /// </summary>
        public string Mx_chrOther2 = "";
        /// <summary>
        /// 备份3
        /// </summary>
        public string Mx_chrOther3 = "";
        /// <summary>
        /// 备份4
        /// </summary>
        public string Mx_chrOther4 = "";
        /// <summary>
        /// 备份5
        /// </summary>
        public string Mx_chrOther5 = "";
    }
}
