
using System.Collections.Generic;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_DataCore.Struct;
using CLDC_Comm.Enum;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.Frozen
{
    /// <summary>
    /// 冻结检定基类
    /// </summary>
    public class FreezeBase : VerifyBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public FreezeBase(object plan)
            : base(plan)
        {
            //bool rr = Adapter.Ini485Adpater();
        }

        public bool bLocalMeter = false;
        /// <summary>
        /// 冻结项目ID
        /// </summary>
        protected override string ItemKey
        {
            get
            {
                return VerifyProcess.Instance.CurrentKey;
            }
        }

        /// <summary>
        /// 结论数据结点
        /// </summary>
        protected override string ResultKey
        {
            get
            {
                return string.Format("{0}", (int)Cus_MeterResultPrjID.冻结功能试验);
            }
        }


        /// <summary>
        /// 当前多功能方案
        /// </summary>
        protected new StPlan_Freeze CurPlan
        {
            get { return (StPlan_Freeze)base.CurPlan; }
        }

        /// <summary>
        /// 清理节点数据[重写]
        /// </summary>
        protected override void ClearItemData()
        {
            CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo meter = null;
            for (int i = 0; i < BwCount; i++)
            {
                meter = Helper.MeterDataHelper.Instance.Meter(i);
                if (!meter.YaoJianYn) continue;
                if (meter != null)
                {
                    if (meter.MeterFreezes.ContainsKey(ItemKey))
                    {
                        meter.MeterFreezes.Remove(ItemKey);
                    }
                }
            }
            
            
        }


        /// <summary>
        /// 升源
        /// </summary>
        /// <param name="bCheck">是否检测当前源状态</param>
        /// <returns></returns>
        protected bool PowerOn(bool bCheck)
        {
            float xIb = 0F;
            //float.TryParse( CurPlan.OutPramerter.xIb.Replace("Ib", ""),out xIb); 
            //电流倍数与功率因素方案内容与文档内容不符合，默认按无电流，1.0输出
            bool ret = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U
                                                      , xIb * GlobalUnit.Ib
                                                      , (int)Cus_PowerYuanJian.H
                                                      , (int)Cus_PowerFangXiang.正向有功
                                                      , "1.0"
                                                      , true
                                                      , false);
            if (ret) Thread.Sleep(3000);
            return ret;
        }

        protected override bool PowerOn()
        {
            //bool isYouGong = (CurPlan.OutPramerter.GLFX == Cus_PowerFangXiang.正向有功 || CurPlan.OutPramerter.GLFX == Cus_PowerFangXiang.反向有功);
           // float xIb = 2F;
           //// float.TryParse(CurPlan.OutPramerter.xIb.Replace("Ib", ""), out xIb);
           // //电流倍数与功率因素方案内容与文档内容不符合，默认按无电流，1.0输出
           // bool ret = Helper.EquipHelper.Instance.PowerOn(CurPlan.OutPramerter.xU * GlobalUnit.U
           //                                           , xIb * GlobalUnit.Ib
           //                                           , (int)CurPlan.OutPramerter.YJ
           //                                           , (int)CurPlan.OutPramerter.GLFX
           //                                           , FangXiangStr + CurPlan.OutPramerter.GLYS
           //                                           , isYouGong
           //                                           , false);

            bool ret = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Imax, 1, 1, "1.0", true, false);

            if (ret) Thread.Sleep(5000);
            return ret;
        }

        /// <summary>
        /// 获取冻结方案参数
        /// </summary>
        protected string[] PrjPara
        {
            get { return ((StPlan_Freeze)CurPlan).PrjParm.Split('|'); }
        }

        /// <summary>
        /// 处理数据结果,适用于只带单一结论结点型测试
        /// </summary>
        protected void ControlResult(bool[] result)
        {
            //防止切点时发一意外
            if (!(CurPlan is StPlan_Freeze)) return;
            //处理结果
            for (int k = 0; k < BwCount; k++)
            {
                ControlResult(k, result[k]);   //处理一块表的检定结论
                
            }
        }
        /// <summary>
        /// 获得bool数组中的统计合格数据
        /// </summary>
        /// <param name="bValue"></param>
        /// <returns></returns>
        protected bool GetArrValue(bool[] bValue)
        {
            bool bResult = true;
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData.MeterGroup.Count; i++)
            {
                if (GlobalUnit.g_CUS.DnbData.MeterGroup[i].YaoJianYn)
                {
                    if (!bValue[i])
                    {
                        bResult = false;
                        return bResult;
                    }
                }
            }
            return bResult;
        }

        /// <summary>
        ///  获得bool数组中的统计合格数据
        /// </summary>
        /// <param name="bValue"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected bool GetArrValue(bool[] bValue, ref bool[] result)
        {
            bool bResult = true;
            for (int i = 0; i < GlobalUnit.g_CUS.DnbData.MeterGroup.Count; i++)
            {
                if (GlobalUnit.g_CUS.DnbData.MeterGroup[i].YaoJianYn)
                {
                    result[i] &= bValue[i];
                    if (!bValue[i])
                    {
                        bResult = false;
                    }
                }
            }
            return bResult;
        }
        /// <summary>
        /// 处理一块表的结论
        /// </summary>
        /// <param name="Index">表位序号：从0开始</param>
        protected void ControlResult(int Index, bool result)
        {
            CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo curMeter;
            string strItemName = ((StPlan_Freeze)CurPlan).FreezePrjName;

            curMeter = Helper.MeterDataHelper.Instance.Meter(Index);
            if (!curMeter.YaoJianYn)
                return;
            //挂结论
            CLDC_DataCore.Model.DnbModel.DnbInfo.MeterFreeze _FreezeResult;
            if (!curMeter.MeterFreezes.ContainsKey(ItemKey))
            {
                _FreezeResult = new CLDC_DataCore.Model.DnbModel.DnbInfo.MeterFreeze();
                _FreezeResult.Md_PrjID = ItemKey;
                _FreezeResult.Md_PrjName = strItemName;
                curMeter.MeterFreezes.Add(ItemKey, _FreezeResult);
            }
            else
            {
                _FreezeResult = curMeter.MeterFreezes[ItemKey];
            }
            _FreezeResult.Md_chrValue = (result ? Variable.CTG_HeGe : Variable.CTG_BuHeGe);
            
            //只外发成功的结论
            
            Thread.Sleep(10);
            
        }

        /// <summary>
        /// 费率名称转化到费率编号
        /// </summary>
        /// <param name="FeiLvName"></param>
        /// <param name="bwIndex"></param>
        /// <returns>被检表的第几个费率</returns>
        protected int SwitchFeiLvNameToOrder(string FeiLvName, int bwIndex)
        {
            if (FeiLvName == "总") return 0;
            if (FeiLvName.Length > 1) return 0;
            string[] arrSourceFL = new string[] { "总", "尖", "峰", "平", "谷" };
            int sourceOrder = 0;
            for (int j = 0; j < arrSourceFL.Length; j++)
            {
                sourceOrder = j;
                if (arrSourceFL[j].Equals(FeiLvName))
                {
                    break;
                }
            }
            //  sourceOrder++;
            return sourceOrder;

            /*
           int returnOrder = 0;
            string meterFeiLvOrder = Meter(bwIndex).FreezeProtocol.TariffOrderType;
            for (int k = 0; k < meterFeiLvOrder.Length; k++)
            {
                returnOrder = k;
                if (meterFeiLvOrder.Substring(k, 1) == sourceOrder.ToString())
                {
                    break;
                }
            }
            return returnOrder;
        */

        }


        /// <summary>
        /// 获取冻结协议配置参数
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="PramKey"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>

        protected int getType(int Index, string PramKey, int DefaultValue)
        {
            Dictionary<string, string> FreezePram = Helper.MeterDataHelper.Instance.Meter(Index).DgnProtocol.DgnPras;
            if (!FreezePram.ContainsKey(PramKey))
            {
                return DefaultValue;
            }
            string[] Arr_Pram = FreezePram[PramKey].Split('|');
            if (Arr_Pram.Length == 2)
            {
                return int.Parse(Arr_Pram[0]);
            }
            else
            {
                return DefaultValue;
            }
        }
        /// <summary>
        /// 获取一块表冻结协议节点值
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="PramKey"></param>
        /// <returns></returns>
        protected string[] getType(int Index, string PramKey)
        {
            Dictionary<string, string> FreezePram = Helper.MeterDataHelper.Instance.Meter(Index).DgnProtocol.DgnPras;
            if (!FreezePram.ContainsKey(PramKey))
            {
                return new string[0];
            }
            return FreezePram[PramKey].Split('|');
        }
        public bool GetlocalMeter()
        {
            bLocalMeter = false;
            CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo MeterInfo = null;
            int int_Index = GlobalUnit.g_CUS.DnbData.GetFirstYaoJianMeterBwh();
            if (int_Index == -1) return false;
            MeterInfo = GlobalUnit.g_CUS.DnbData.GetMeterBasicInfoByBwh(int_Index + 1);

            if (MeterInfo.Mb_intFKType == 1)//"本地表")
            {
                bLocalMeter = true;
            }

            return bLocalMeter;
        }
    }
}
