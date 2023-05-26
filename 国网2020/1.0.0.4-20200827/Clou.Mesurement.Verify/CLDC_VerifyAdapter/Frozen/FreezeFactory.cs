
using System;
using CLDC_Comm.Enum;

namespace CLDC_VerifyAdapter.Frozen
{
    /// <summary>
    /// 冻结检定工厂
    /// </summary>
    public class FreezeFactory
    {
        /// <summary>
        /// 创建一个冻结控制器工厂
        /// </summary>
        /// <param name="VerifyType">冻结试验类型</param>
        /// <returns></returns>
        public FreezeBase CreateFreezeControler(CLDC_DataCore.Struct.StPlan_Freeze tagPlan)
        {
            CLDC_Comm.Enum.Cus_FreezeItem VerifyType;
            FreezeBase curControler = null;
            VerifyType = (Cus_FreezeItem)Enum.Parse(typeof(Cus_FreezeItem), tagPlan.FreezePrjID);
            switch (VerifyType)
            {
                case CLDC_Comm.Enum.Cus_FreezeItem.定时冻结:   
                    {
                        curControler = new Freeze_Timing(tagPlan);
                        break;
                    }
                case CLDC_Comm.Enum.Cus_FreezeItem.瞬时冻结:
                    {
                        curControler = new Freeze_Instant(tagPlan);
                        break;
                    }
                case CLDC_Comm.Enum.Cus_FreezeItem.约定冻结:
                    {
                        curControler = new Freeze_Appoint(tagPlan);
                        break;
                    }
                case CLDC_Comm.Enum.Cus_FreezeItem.日冻结:
                    {
                        curControler = new Freeze_Day(tagPlan);
                        break;
                    }
                case CLDC_Comm.Enum.Cus_FreezeItem.整点冻结:
                    {
                       curControler = new Freeze_Whole_Point(tagPlan);
                        break;
                    }
                default:
                    {
                        curControler = new FreezeBase(tagPlan);
                        break;
                    }
            }
            return curControler;
        }
    }
}
