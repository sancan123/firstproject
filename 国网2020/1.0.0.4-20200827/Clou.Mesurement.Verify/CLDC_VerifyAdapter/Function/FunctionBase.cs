
using System.Collections.Generic;
using System.Threading;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using CLDC_DataCore.Struct;

namespace CLDC_VerifyAdapter.Function
{
    /// <summary>
    /// 功能检定基类
    /// </summary>
    public class FunctionBase : VerifyBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public FunctionBase(object plan)
            : base(plan)
        {

        }

        /// <summary>
        /// 功能项目ID
        /// </summary>
        protected override string ItemKey
        {
            get
            {
                return CurPlan.FunctionPrjID;
            }
        }

        /// <summary>
        /// 结论数据结点
        /// </summary>
        protected override string ResultKey
        {
            get
            {
                return string.Format("{0}",(int)Cus_MeterResultPrjID.智能表功能试验);   
            }
        }

        /// <summary>
        /// 当前多功能方案
        /// </summary>
        protected new StPlan_Function CurPlan
        {
            get { return (CLDC_DataCore.Struct.StPlan_Function)base.CurPlan; }
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
                if (meter != null)
                {
                    if (meter.MeterFunctions.ContainsKey(ItemKey))
                    {
                        meter.MeterFunctions.Remove(ItemKey);
                    }
                }
            }
            
            
        }

        /// <summary>
        /// 清理节点数据[重写]
        /// </summary>
        protected override void ClearItemData(string strKey)
        {
            CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo meter = null;
            for (int i = 0; i < BwCount; i++)
            {
                meter = Helper.MeterDataHelper.Instance.Meter(i);
                if (meter != null)
                {
                    if (meter.MeterFunctions.ContainsKey(strKey))
                    {
                        meter.MeterFunctions.Remove(strKey);
                    }
                }
            }
        }

        /// <summary>
        /// 按方案参数进行控源操作
        /// </summary>
        /// <returns></returns>
        protected override bool PowerOn()
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

        /// <summary>
        /// 获取功能方案参数
        /// </summary>
        protected string[] PrjPara
        {
            get { return CurPlan.PrjParm.Split('|'); }
        }

        /// <summary>
        /// 处理数据结果,适用于只带单一结论结点型测试
        /// </summary>
        protected void ControlResult(bool[] result)
        {            
            //处理结果
            for (int k = 0; k < BwCount; k++)
            {
                ControlResult(k,result[k]);   //处理一块表的检定结论
                
            }
        }

        /// <summary>
        /// 处理一块表的结论
        /// </summary>
        /// <param name="Index">表位序号：从0开始</param>
        protected void ControlResult(int Index,bool result)
        {
            CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo curMeter;
            string strItemName = ((CLDC_DataCore.Struct.StPlan_Function)CurPlan).FunctionPrjName;

            curMeter = Helper.MeterDataHelper.Instance.Meter(Index);
            if (!curMeter.YaoJianYn)
                return;
            //挂结论
            CLDC_DataCore.Model.DnbModel.DnbInfo.MeterFunction _FunctionResult;
            if (!curMeter.MeterFunctions.ContainsKey(ItemKey))
            {
                _FunctionResult = new CLDC_DataCore.Model.DnbModel.DnbInfo.MeterFunction();
                _FunctionResult.Mf_PrjID = ItemKey;
                _FunctionResult.Mf_PrjName = strItemName;
                curMeter.MeterFunctions.Add(ItemKey, _FunctionResult);
            }
            else
            {
                _FunctionResult = curMeter.MeterFunctions[ItemKey];
            }
            _FunctionResult.Mf_chrValue = (result ? Variable.CTG_HeGe : Variable.CTG_BuHeGe);
            if (result)
            {
                //只外发成功的结论
                
                Thread.Sleep(10);
                
            }
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
            return sourceOrder;
        }


        /// <summary>
        /// 获取功能协议配置参数
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="PramKey"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>

        protected int getType(int Index, string PramKey, int DefaultValue)
        {
            Dictionary<string, string> DgnPram = Helper.MeterDataHelper.Instance.Meter(Index).DgnProtocol.DgnPras;
            if (!DgnPram.ContainsKey(PramKey))
            {
                return DefaultValue;
            }
            string[] Arr_Pram = DgnPram[PramKey].Split('|');
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
        /// 获取一块表功能协议节点值
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="PramKey"></param>
        /// <returns></returns>
        protected string[] getType(int Index, string PramKey)
        {
            Dictionary<string, string> DgnPram = Helper.MeterDataHelper.Instance.Meter(Index).DgnProtocol.DgnPras;
            if (!DgnPram.ContainsKey(PramKey))
            {
                return new string[0];
            }
            return DgnPram[PramKey].Split('|');
        }


        public static void ShowWirteMeterWwaring()
        {
            if (GlobalUnit.g_SystemConfig.SystemMode.getValue(Variable.CTC_DGN_WRITEMETERALARM).Trim() == "是")
                System.Windows.Forms.MessageBox.Show("请确认打开电表编程键", "提示", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

        }
    }
}
