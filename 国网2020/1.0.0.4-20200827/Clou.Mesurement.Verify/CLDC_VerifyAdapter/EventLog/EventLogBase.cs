
using System.Collections.Generic;
using System.Threading;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter.Helper;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.EventLog
{
    /// <summary>
    /// 事件记录检定基类
    /// </summary>
    public class EventLogBase : VerifyBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public EventLogBase(object plan)
            : base(plan)
        {

        }

      

        /// <summary>
        /// 事件记录项目ID
        /// </summary>
        protected override string ItemKey
        {
            get
            {
                return CurPlan.EventLogPrjID;
            }
        }

        /// <summary>
        /// 结论数据结点
        /// </summary>
        protected override string ResultKey
        {
            get
            {
                return string.Format("{0}",(int)CLDC_Comm.Enum.Cus_MeterResultPrjID.事件记录功能试验);   
            }
        }

        /// <summary>
        /// 当前事件记录方案
        /// </summary>
        protected new CLDC_DataCore.Struct.StPlan_EventLog CurPlan
        {
            get { return (CLDC_DataCore.Struct.StPlan_EventLog)base.CurPlan; }
        }
    //    public string[] strEventTimes = new string[Adapter.Instance.BwCount];
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
                    if (meter.MeterEventLogs.ContainsKey(ItemKey))
                    {
                        meter.MeterEventLogs.Remove(ItemKey);
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
                    if (meter.MeterEventLogs.ContainsKey(strKey))
                    {
                        meter.MeterEventLogs.Remove(strKey);
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
            bool isYouGong = true;//(CurPlan.OutPramerter.GLFX == Cus_PowerFangXiang.正向有功 || CurPlan.OutPramerter.GLFX == Cus_PowerFangXiang.反向有功);
            float xIb = 0F;
            //float.TryParse( CurPlan.OutPramerter.xIb.Replace("Ib", ""),out xIb); 
            //电流倍数与功率因素方案内容与文档内容不符合，默认按无电流，1.0输出
            bool ret = EquipHelper.Instance.PowerOn(GlobalUnit.U
                                                      , xIb * GlobalUnit.Ib
                                                      , (int)Cus_PowerYuanJian.H
                                                      , (int)Cus_PowerFangXiang.正向有功
                                                      , "1.0"
                                                      , isYouGong
                                                      , false);
            if (ret) Thread.Sleep(3000);
            return ret;
        }

        /// <summary>
        /// 获取多功能方案参数
        /// </summary>
        protected string[] PrjPara
        {
            get { return CurPlan.PrjParm.Split('|'); }
        }

        protected void ControlResult(bool[] result)
        {
        }


        /// <summary>
        /// 处理数据结果,适用于只带单一结论结点型测试
        /// </summary>
        protected void ControlResult(bool[] result, string[] arrBeforeLogInfo, string[] arrAfterLogInfo)
        {            
            //处理结果
            for (int k = 0; k < BwCount; k++)
            {
                ControlResult(k, result[k], arrBeforeLogInfo[k], arrAfterLogInfo[k]);   //处理一块表的检定结论
                
            }
        }

        /// <summary>
        /// 处理一块表的结论
        /// </summary>
        /// <param name="Index">表位序号：从0开始</param>
        protected void ControlResult(int Index, bool result, string arrBeforeLogInfo, string arrAfterLogInfo)
        {
            ResultNames = new string[] { "事件产生前事件次数", "事件产生后事件次数", "事件记录发生时刻", "事件记录结束时刻","结论" };
            CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo curMeter;
          //  string strItemName = ((CLDC_DataCore.Struct.StPlan_EventLog)CurPlan).EventLogPrjName;

            curMeter = Helper.MeterDataHelper.Instance.Meter(Index);
            if (!curMeter.YaoJianYn)
                return;
            string[] startTime = null;
            string[] endTime = null;
            if (result == false)
            {
                startTime = arrBeforeLogInfo.Split('|');
                endTime = arrAfterLogInfo.Split('|');
                if (startTime.Length > 1 && endTime.Length > 1)
                {
                    ResultDictionary["事件产生前事件次数"][Index] = startTime[0].ToString();
                    ResultDictionary["事件产生后事件次数"][Index] = endTime[0].ToString();
                    ResultDictionary["事件记录发生时刻"][Index] = startTime[1].ToString();
                    ResultDictionary["事件记录结束时刻"][Index] = endTime[1].ToString();
                }
            }
            else
            {
                startTime = arrBeforeLogInfo.Split('|');
                endTime = arrAfterLogInfo.Split('|');
                if (startTime.Length > 2 && endTime.Length > 2)
                {
                    ResultDictionary["事件产生前事件次数"][Index] = startTime[0].ToString();
                    ResultDictionary["事件产生后事件次数"][Index] = endTime[0].ToString();
                    ResultDictionary["事件记录发生时刻"][Index] = endTime[1].ToString();
                    ResultDictionary["事件记录结束时刻"][Index] = endTime[2].ToString();
                }
               

            }

            ResultDictionary["结论"][Index] = (result == true) ? Variable.CTG_HeGe : Variable.CTG_BuHeGe;
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生前事件次数", ResultDictionary["事件产生前事件次数"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件产生后事件次数", ResultDictionary["事件产生后事件次数"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件记录发生时刻", ResultDictionary["事件记录发生时刻"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "事件记录结束时刻", ResultDictionary["事件记录结束时刻"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
           
     
        //    if (curMeter.MeterFunctions.ContainsKey(ItemKey))
          //  {

          //  }
           // curMeter.MeterFunctions;
            //挂结论
    
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
        /// 获取事件记录协议配置参数
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
        /// 获取一块表事件记录协议节点值
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

        /// <summary>
        /// 读取表号
        /// </summary>
        public void ReadMeterNo()
        {
            GlobalUnit.g_MsgControl.OutMessage("正在进行【读取表号】操作...", false);

            string[] meterno = MeterProtocolAdapter.Instance.ReadData("04000402", 6);


            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //假如当前停止检定，则退出
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                curMeter._Mb_MeterNo = meterno[i];

            }
        }
    }
}
