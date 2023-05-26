using System;
using System.Collections.Generic;
using System.Text;

using CLDC_DataCore.Struct;
using CLDC_Comm;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using System.Threading;

using System.Windows.Forms;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.SecondStage
{
    class UplinkModuleInterchange: VerifyBase
    {
        #region--------------私有变量-----------------
        private const Cus_MeterResultPrjID CST_CMP_CURITEMID = Cus_MeterResultPrjID.载波试验;               //当前检定点编号
        private int[] m_int_CheckNums;                                                                      //召测次数集合
        private int[] m_int_SuccessNums;                                                                    //成功次数集合
        private int m_int_VerifyNum = 1;                                                                    //发送次数
        private double m_dbl_SuccessRate = 1.0;                                                             //合格成功率
        /// <summary>
        /// 返回值，10、16等进制，数字等普通值
        /// </summary>
        private string[] str_RevD;
        private string[] str_RevDnumber;
        /// <summary>
        /// 返回值，10、16等进制，数字等普通值  载波抄读
        /// </summary>
        private string[] str_RevD1;
        /// <summary>
        /// 返回值，10、16等进制，数字等普通值  无线抄读
        /// </summary>
        private string[] str_RevD2;
        /// <summary>
        /// 转换编码的字符，ASCII等
        /// </summary>
        private string[] str_RevDConvert;
        private static bool Is_SM = GlobalUnit.CarrierInfo.IsCheck_SM;
        #endregion------------------------------------

        #region--------------公共属性-----------------
        /// <summary>
        /// 项目名
        /// </summary>
        public string CodeName
        {
            get
            {
                return ((StPlan_Carrier)CurPlan).str_Name;
            }
        }
        /// <summary>
        /// 标识符
        /// </summary>
        public string Code
        {
            get
            {
                return ((StPlan_Carrier)CurPlan).str_Code;
            }
        }

        /// <summary>
        /// 成功率
        /// </summary>
        public double SuccessRate
        {
            get
            {
                return ((StPlan_Carrier)CurPlan).flt_Success;
            }
        }

        /// <summary>
        /// 发送次数
        /// </summary>
        public int VerifyNum
        {
            get
            {
                return ((StPlan_Carrier)CurPlan).int_Times;
            }

            set
            {

            }
        }
        /// <summary>
        /// 模块互换
        /// </summary>
        public bool ModuleSwaps
        {
            get
            {
                return ((StPlan_Carrier)CurPlan).b_ModuleSwaps;
            }
        }
        #endregion------------------------------------

        #region--------------构造函数-----------------
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public UplinkModuleInterchange(object plan)
            : base(plan)
        {
            ResultNames = new string[] {"载波项目名称", "第一次", "第二次", "第三次", "第四次", "第五次", "第六次", "第七次", "第八次", "第九次", "第十次", "结论","不合格原因" };
        }


        #endregion------------------------------------

        #region-------基类方法重写 --------
        /// <summary>
        /// 总结论主键
        /// </summary>
        protected override string ResultKey
        {
            get
            {
                return string.Format("{0}", (int)Cus_MeterResultPrjID.载波试验);
            }
        }

        /// <summary>
        /// 重写数据主键
        /// </summary>
        protected override string ItemKey
        {
            get
            {
                return String.Format("{0}{1}{2}"                                          //Key:参见数据结构设计附2
                        , ResultKey
                        , Code
                        , ((StPlan_Carrier)CurPlan).str_Type);
            }
        }
        #endregion

        #region--------------公共函数-----------------
        /// <summary>
        /// 载波试验检定
        /// </summary>
        public override void Verify()
        {
            CLDC_DataCore.Const.GlobalUnit.Flag_IsCarrier = true;

            if (GlobalUnit.CarrierInfo.CarrierName == "中电华瑞2016")
            {
                GlobalUnit.Flag_IsZD2016 = true;
            }
            else
            {
                GlobalUnit.Flag_IsZD2016 = false;
            }
            string[] arrStrResultKey = new string[BwCount];
            //【基类检定】
            //【初始化参数】
            //【初始化设备】
            try
            {
                GlobalUnit.g_MsgControl.OutMessage("正在进行载波试验检定...");
                base.Verify();
                if (Stop) return ;   
                str_RevD = new string[BwCount];
                str_RevD2 = new string[BwCount];
                str_RevD1 = new string[BwCount];

                str_RevDnumber = new string[BwCount];
                str_RevDConvert = new string[BwCount];
                string[] strModel = new string[BwCount];

                //【消息处理】
                //GlobalUnit.g_MsgControl.OutMessage(Cus_MessageType.清空消息队列);
                GlobalUnit.g_MsgControl.OutMessage("正在进行载波试验检定...", false);
                if (Stop) return;
                Helper.EquipHelper.Instance.PowerOn(CLDC_DataCore.Const.GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);//只输出电压
                GlobalUnit.g_MsgControl.OutMessage("等待升源稳定...", false);
                Thread.Sleep(5000);
            

          
                if (Stop) return ;   
                //【载波检定】
                CarrierCheck();


                Helper.EquipHelper.Instance.PowerOn(CLDC_DataCore.Const.GlobalUnit.U, (int)Cus_PowerFangXiang.正向有功);//只输出电压
                GlobalUnit.g_MsgControl.OutMessage("等待升源稳定...", false);
                Thread.Sleep(5000);

                if (GlobalUnit.CarrierInfo.CarrierName == "中电华瑞2016")
                {
                    //恢复主动上报模式字
                    GlobalUnit.g_MsgControl.OutMessage("正在恢复主动上报模式字...", false);
                    MeterProtocolAdapter.Instance.WriteArrData("04001104", 8, strModel);
                }

                if (Stop) return;
                //【消息处理】
                GlobalUnit.g_MsgControl.OutMessage("载波试验检定完毕。", false);

                //【表位结果处理】
           
            //    CLDC_DataCore.Const.GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_CARRIER_WAVE, (string[])arrStrResultKey);

            }
            catch (Exception e)
            {
                GlobalUnit.g_MsgControl.OutMessage(e.Message, false);
          //      CLDC_DataCore.Function.LogInfoHelper.Write(e);
         //       CLDC_DataCore.Const.GlobalUnit.Flag_IsCarrier = false;

            }
            CLDC_DataCore.Const.GlobalUnit.Flag_IsCarrier = false;

        }

        /// <summary>
        /// 转换状态字，将0装换成1，1转换成0
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public string[] ConvertStatus(string[] status)
        {
            string[] statusTmp = new string[status.Length];
            for (int i = 0; i < status.Length; i++)
            {
                if (!string.IsNullOrEmpty(status[i]))
                {
                    string strTmp = "";
                    for (int j = 0; j < status[i].Length / 2; j++)
                    {
                        strTmp += (255 - Convert.ToInt32(status[i].Substring(j * 2, 2), 16)).ToString("X2");
                    }
                    statusTmp[i] = strTmp;
                }
                else
                {
                    statusTmp[i] = status[i];
                }
            }
            return statusTmp;
        }


        #endregion------------------------------------

        #region--------------私有函数-----------------
 /// <summary>
 /// 载波检定
 /// </summary>
 /// <returns></returns>
        private bool CarrierCheck()
        {

            if (Stop) return false;   
            string[,]result = new string[BwCount,10];
            if (Stop) return false;   
            SwitchCarrierOr485(Cus_CommunType.通讯载波);
            MeterBasicInfo curMeter;

            Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U
                                                       , GlobalUnit.Ib
                                                       , (int)Cus_PowerYuanJian.H
                                                       , (int)Cus_PowerFangXiang.正向有功
                                                       , "1.0"
                                                       , true
                                                       , false);
             if (MessageBox.Show("请互换载波模块后继续"
                               , "确定提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                   {
                       return false;
                  }
             for (int j = 0; j < 10; j++)
             {
                 MessageController.Instance.AddMessage("正在读取数据第"+ (j+1).ToString() +"次...");
                 ShowWaitMessage("{0}秒，请等待", 10000);
            
                 for (int iBw = 0; iBw < BwCount; iBw++)
                 {
                     GlobalUnit.Carrier_Cur_BwIndex = iBw;
                     if (CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData.CheckState == Cus_CheckStaute.停止检定)
                     {
                         break;
                     }
                     //【获取指定表位电表信息】
                     curMeter = Helper.MeterDataHelper.Instance.Meter(iBw);
                     //【判断是否要检】
                     if (!curMeter.YaoJianYn)
                     {
                         continue;
                     }
                     MessageController.Instance.AddMessage("正在载波试验第" + (iBw + 1) + "表位...");
                    
                     float flt_RevD = -1;

                     float flt_RevD2 = -1;
                     try
                     {
                         #region
                         if (Stop) return false;                                             
                             //【读召测数据】
                             Thread.Sleep(1000);
                             flt_RevD = CLDC_VerifyAdapter.MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2, iBw);
                             str_RevD[iBw] = flt_RevD.ToString();
                             result[iBw, j] = flt_RevD.ToString();                      
                             if (Stop) return false;
                        
                         #endregion
                     }
                     catch (Exception e)
                     {
                         GlobalUnit.g_MsgControl.OutMessage(e.Message, false);
                     }
                     GlobalUnit.g_MsgControl.OutMessage();
                 }
             }
            SwitchCarrierOr485(Cus_CommunType.通讯485);
           
            for (int Num = 0; Num < BwCount; Num++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(Num).YaoJianYn)
                {
                    continue;
                }

                 ResultDictionary["载波项目名称"][Num] = "当前正向有功总电能";
                 ResultDictionary["第一次"][Num] = result[Num, 0];
                 ResultDictionary["第二次"][Num] = result[Num, 1];
                 ResultDictionary["第三次"][Num] = result[Num, 2];
                 ResultDictionary["第四次"][Num] = result[Num, 3];
                 ResultDictionary["第五次"][Num] = result[Num, 4];
                 ResultDictionary["第六次"][Num] = result[Num, 5];
                 ResultDictionary["第七次"][Num] = result[Num, 6];
                 ResultDictionary["第八次"][Num] = result[Num, 7];
                 ResultDictionary["第九次"][Num] = result[Num, 8];
                 ResultDictionary["第十次"][Num] = result[Num, 9];
                            
            }
            for (int Num = 0; Num < BwCount; Num++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(Num).YaoJianYn)
                {
                    continue;
                }
                for (int j = 0; j < 10; j++)
                {

                    if (string.IsNullOrEmpty(result[Num, j]) || result[Num, j] == "-1")
                    {
                        ResultDictionary["结论"][Num] = "不合格";
                        ResultDictionary["不合格原因"][Num] = j.ToString() + "," + ResultDictionary["不合格原因"][Num];
                    }
                    else
                    {
                        ResultDictionary["结论"][Num] = "合格";
                    }
                }
                if (ResultDictionary["结论"][Num] == "不合格")
                {
                    ResultDictionary["不合格原因"][Num] = ResultDictionary["不合格原因"][Num] + "次读取不了数据";
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "载波项目名称", ResultDictionary["载波项目名称"]);
         
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第一次", ResultDictionary["第一次"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第二次", ResultDictionary["第二次"]);
         
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第三次", ResultDictionary["第三次"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第四次", ResultDictionary["第四次"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第五次", ResultDictionary["第五次"]);
       
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第六次", ResultDictionary["第六次"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第七次", ResultDictionary["第七次"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第八次", ResultDictionary["第八次"]);
         
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第九次", ResultDictionary["第九次"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "第十次", ResultDictionary["第十次"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", ResultDictionary["不合格原因"]);
           
            return true;
        }
        #endregion------------------------------------
    }
}
