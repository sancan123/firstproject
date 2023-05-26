
using System;
using CLDC_Comm.Enum;

namespace CLDC_VerifyAdapter.Function
{
    /// <summary>
    /// 功能检定工厂
    /// </summary>
    public class FunctionFactory
    {

        /// <summary>
        /// 创建一个功能控制器工厂
        /// </summary>
        /// <param name="VerifyType">功能试验类型</param>
        /// <returns></returns>
        public FunctionBase CreateFunctionControler(CLDC_DataCore.Struct.StPlan_Function tagPlan)
        {
            FunctionBase curControler = null;
            CLDC_Comm.Enum.Cus_FunctionItem VerifyType = (Cus_FunctionItem)Enum.Parse(typeof(Cus_FunctionItem), tagPlan.FunctionPrjID);

            switch (VerifyType)
            {
                case Cus_FunctionItem.计量功能:
                    {
                        curControler = new FC_Computation(tagPlan);
                        break;
                    }
                case Cus_FunctionItem.计时功能:
                    {
                        curControler = new FC_Timing(tagPlan);
                        break;
                    }
                case Cus_FunctionItem.费率时段功能:
                    {
                        curControler = new FC_RatePeriodFunction(tagPlan);
                        break;
                    }
                case Cus_FunctionItem.脉冲输出功能:
                    {
                        curControler = new FC_PulseOutPut(tagPlan);
                        break;
                    }
                case Cus_FunctionItem.显示功能:
                    {
                        curControler = new FC_Show(tagPlan);
                        break;
                    }
                case Cus_FunctionItem.最大需量功能:
                    {
                        curControler = new FC_MaxDemandFunction(tagPlan);
                        break;
                    }                
                default:
                    {
                        curControler = new FunctionBase(tagPlan);
                        break;
                    }
            }
            
            return curControler;
        }
    }
}
