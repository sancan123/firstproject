
using System;
using CLDC_Comm.Enum;


namespace CLDC_VerifyAdapter.Multi
{
    /// <summary>
    /// 多功能检定工厂
    /// </summary>
    public class DgnFactory
    {

        /// <summary>
        /// 创建一个多功能控制器工厂
        /// </summary>
        /// <param name="VerifyType">多功能试验类型</param>
        /// <returns></returns>
        public DgnBase CreateDgnControler(CLDC_DataCore.Struct.StPlan_Dgn tagPlan)
        {
            DgnBase curControler = null;
            CLDC_Comm.Enum.Cus_DgnItem VerifyType = (Cus_DgnItem)Enum.Parse(typeof(Cus_DgnItem), tagPlan.DgnPrjID);

            switch (VerifyType)
            {
                case CLDC_Comm.Enum.Cus_DgnItem.通信测试:    //OK
                    {
                        curControler = new Dgn_CommTest(tagPlan);
                        break;
                    }
                case Cus_DgnItem.GPS对时:               //OK
                    {
                        //GPS对时为时间误差
                        curControler = new Dgn_GPS(tagPlan);
                        break;
                    }
                case Cus_DgnItem.费率时段检查:
                    {
                  //      curControler = new Dgn_ReadPeriod(tagPlan);
                        break;
                    }
                case CLDC_Comm.Enum.Cus_DgnItem.日计时误差:  //OK
                    {
                        curControler = new Dgn_ClockError(tagPlan);
                        break;
                    }
                case CLDC_Comm.Enum.Cus_DgnItem.时段投切:    //OK
                case Cus_DgnItem.反向有功时段投切:
                case Cus_DgnItem.正向无功时段投切:
                case Cus_DgnItem.反向无功时段投切:
                    {
                        curControler = new Dgn_PeriodChange(tagPlan);
                        break;
                    }
                case Cus_DgnItem.费率时段示值误差:
                case Cus_DgnItem.反向有功费率时段示值误差:
                case Cus_DgnItem.正向无功费率时段示值误差:
                case Cus_DgnItem.反向无功费率时段示值误差:
                    {
                        curControler = new Dgn_RatePeriod(tagPlan);
                        break;
                    }
                case Cus_DgnItem.计度器示值组合误差:
                case Cus_DgnItem.反向有功计度器示值组合误差:
                case Cus_DgnItem.正向无功计度器示值组合误差:
                case Cus_DgnItem.反向无功计度器示值组合误差:
                    {
                        curControler = new Dgn_Register(tagPlan);
                        break;
                    }
                case Cus_DgnItem.闰年判断功能:      //ok
                    {
                        curControler = new Dgn_LeapYear(tagPlan);
                        break;
                    }
                case Cus_DgnItem.电量寄存器检查:    //UI还有点问题
                    {
                        curControler = new Dgn_CheckRam_Energy(tagPlan);
                        break;
                    }
                case Cus_DgnItem.需量寄存器检查:    //OK
                    {
                   //     curControler = new Dgn_CheckRam_Demand(tagPlan);
                        break;
                    }
                case Cus_DgnItem.瞬时寄存器检查:    //OK
                    {
                  //      curControler = new Dgn_CheckRam_Instant(tagPlan);
                        break;
                    }
                case Cus_DgnItem.状态寄存器检查:    //OK
                    {
                 //       curControler = new Dgn_CheckRam_State(tagPlan);
                        break;
                    }
                case Cus_DgnItem.失压寄存器检查:    //OK
                    {
                   //     curControler = new Dgn_CheckRam_Voltage(tagPlan);
                        break;
                    }
                case Cus_DgnItem.事件记录检查:  //OK
                    {
                        curControler = new Dgn_CheckRam_Log(tagPlan);
                        break;
                    }
                case Cus_DgnItem.需量清空:      //ok
                    {
                        curControler = new Dgn_ClearDemand(tagPlan);
                        break;
                    }
                case Cus_DgnItem.电压逐渐变化://硬件接口不支持
                    {
                 //       Dgn_UFallOrStep ctl = new Dgn_UFallOrStep(tagPlan);
                //        ctl.VolType = CLDC_Comm.Enum.Cus_VolFallOffType.电压逐渐变化;
                 //       ctl.Item = Cus_DgnItem.电压逐渐变化;
                  //      ctl.TestTime = 60000;
                 //       curControler = ctl;
                        break;
                    }
                case Cus_DgnItem.电压跌落:
                    {
                        //curControler = new Dgn_UFallOrStep(
                        //    Cus_DgnItem.电压跌落
                        //    , enmVolFallOff.电压跌落和短时中断
                        //    , 30);

                  //      Dgn_UFallOrStep ctl = new Dgn_UFallOrStep(tagPlan);
                  //      ctl.Item = Cus_DgnItem.电压跌落;
                  //      ctl.VolType = CLDC_Comm.Enum.Cus_VolFallOffType.电压跌落和短时中断;
                  //      ctl.TestTime = 1000 * 2 * 60;
                   //     curControler = ctl;

                        break;
                    }
                case Cus_DgnItem.电压短时中断:
                    {
                  //      Dgn_VoltageMomentaryBreakOff ctl = new Dgn_VoltageMomentaryBreakOff(tagPlan);
                //        ctl.Item = Cus_DgnItem.电压短时中断;
                 //       curControler = ctl;
                        break;
                    }

                case Cus_DgnItem.最大需量01Ib:  //OK
                    {
                        Dgn_MaxDemand ctl = new Dgn_MaxDemand(tagPlan);
                        ctl.XIB = 0.1F;
                        curControler = ctl;
                        break;
                    }
                case Cus_DgnItem.最大需量10Ib://OK
                    {

                        Dgn_MaxDemand ctl = new Dgn_MaxDemand(tagPlan);
                        ctl.XIB = 1F;
                        curControler = ctl;
                        break;
                    }
                case Cus_DgnItem.最大需量Imax://OK
                    {
                        Dgn_MaxDemand ctl = new Dgn_MaxDemand(tagPlan);
                        curControler = ctl;
                        ctl.XIB = CLDC_DataCore.Const.GlobalUnit.Imax / CLDC_DataCore.Const.GlobalUnit.Ib;
                        break;
                    }
                case Cus_DgnItem.读取电量:  //ok
                    {
                        curControler = new Dgn_ReadEnerfy(tagPlan);
                        break;
                    }
                //后边新加的五个项目
                case Cus_DgnItem.时间误差:  //ok
                    {
                 //       curControler = new Dgn_TimeError(tagPlan);
                        break;
                    }
                case Cus_DgnItem.校对电量://==
                    {
                 //       curControler = new Dgn_CheckEnerfy(tagPlan);
                        break;
                    }
                case Cus_DgnItem.校对需量:  //OK
                    {
                  //      curControler = new Dgn_CheckRam_Demand(tagPlan);
                        break;
                    }
                case Cus_DgnItem.检查电表运行状态://OK
                    {
                   //     curControler = new Dgn_CheckRam_State(tagPlan);

                        break;
                    }
                case Cus_DgnItem.预付费检测://==
                    {
                        //curControler = new Dgn_Check_PrePay();
                        break;
                    }









                default:
                    {
                        curControler = new DgnBase(VerifyType);
                        break;
                    }
            }
            return curControler;
        }
    }
}
