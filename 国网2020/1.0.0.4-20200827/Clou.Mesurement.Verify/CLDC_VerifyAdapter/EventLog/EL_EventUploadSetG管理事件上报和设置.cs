using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;
using CLDC_Comm.Enum;
namespace CLDC_VerifyAdapter.EventLog
{
    /// <summary>
    /// 事件上报和设置
    /// </summary>
    class EL_EventUploadSetG:EventLogBase
    {
        public EL_EventUploadSetG(object plan)
            : base(plan) 
        {
            
        }

        protected override string ResultKey
        {

            //get { throw new NotImplementedException(); }
            get { return null; }
        }

        protected override string ItemKey
        {
            //get { throw new NotImplementedException(); }
            get { return null; }
        }

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "测试时间", "开状态下的主动上报状态字", "开状态下复位主动上报状态字后的状态字", "关状态下的主动上报状态字", "结论", "不合格原因" };
            return true;
        }

        /// 重写基类测试方法
        /// </summary>
        /// <param name="ItemNumber">检定方案序号</param>
        public override void Verify()
        {
           
             if (Stop) return;
            base.Verify();
            PowerOn();
            string[] strID = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strSetData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            string[] strShowData = new string[BwCount];
            string[] strCode = new string[BwCount];
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机
            string[] strEsamNo = new string[BwCount];//Esam序列号


            if (Stop) return;
            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            //身份认证

            string[] strDataCode = new string[BwCount];
          
   

            //1： 打开允许上报开关，04001101 ，"FF"
            //2：设置主动上报状态字，让校时开,04001104,0010000000000000
            //3：普通校时，产生上报 
            //4：读取主动上报状态字，04001501
            //5：复位上报状态字04001503
            //6：读取开下的复位后主动上报状态字 04001501
            //7：关闭主动上报状态字
            //8：普通校时，产生上报 
            //9：读取主动上报状态字，04001501

            bool[] bResult;
      
          

            if (Stop) return;
            //复位上报状态字，复位前要先读取04001501
            MeterProtocolAdapter.Instance.ReadData("04001501", 14);
            bResult = MeterProtocolAdapter.Instance.WriteData("04001503", 12, CLDC_VerifyAdapter.Helper.EquipHelper.Instance.RevString("000000000000000000000000", 2));
           
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            ////主动上报状态字复位时间
            //Common.Memset(ref strDataCode, "04001405");
            //Common.Memset(ref strData, "04001405" + "30");// 1 分钟
            //bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

          
            ////打开允许上报开关
            Common.Memset(ref strDataCode, "04001101");
            Common.Memset(ref strData, "04001101" + "FF");//电表运行特征字 1 
            bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

         
          
            //设置主动上报状态字，让校时开
            Common.Memset(ref strDataCode, "04001104");
            Common.Memset(ref strData, "04001104"+"0001000000000000");//电表运行特征字 1  设置校时为开
            bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
          //  string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("16000001", 3);
            if (Stop) return;
            //设置1次编程
            for (int it = 0; it < 1; it++)
            {
                if (Stop) return;
                MessageController.Instance.AddMessage("正在让电能表编程以便形成编程记录");
                //  DateTime time = DateTime.Now
                MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);
                MessageController.Instance.AddMessage("正在下发身份认证失效命令,请稍候....");
                if (Stop) return;
                MeterProtocolAdapter.Instance.SouthCmdNoData("070002FF");
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 3);
            }
         //   string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("16000001", 3);
            //读取开下的主动上报状态字
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取开下的主动上报状态字");
            string[] strStatusOpen1 = MeterProtocolAdapter.Instance.ReadData("04001501", 14);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开状态下的主动上报状态字", strStatusOpen1);

          
            if (Stop) return;
            //复位上报状态字，复位前要先读取04001501
            bResult = MeterProtocolAdapter.Instance.WriteData("04001503", 12, CLDC_VerifyAdapter.Helper.EquipHelper.Instance.RevString("000000000000000000000000", 2));
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取开下的复位后主动上报状态字");
            string[] strStatusOpenReset = MeterProtocolAdapter.Instance.ReadData("04001501", 14);


            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "开状态下复位主动上报状态字后的状态字", strStatusOpenReset);
           
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
     
             //关闭主动上报模式字
             Common.Memset(ref strDataCode, "04001104");
             Common.Memset(ref strData, "04001104"+"0000000000000000");//电表运行特征字 1 
             bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);

             if (Stop) return;
             //复位上报状态字，复位前要先读取04001501
             MeterProtocolAdapter.Instance.ReadData("04001501", 14);
             bResult = MeterProtocolAdapter.Instance.WriteData("04001503", 12, CLDC_VerifyAdapter.Helper.EquipHelper.Instance.RevString("000000000000000000000000", 2));
             ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);
         
             //设置1次编程
             for (int it = 0; it < 1; it++)
             {
                 if (Stop) return;
                 MessageController.Instance.AddMessage("正在让电能表编程以便形成编程记录");
                 //  DateTime time = DateTime.Now
                 MeterProtocolAdapter.Instance.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));
                 ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);
                 MessageController.Instance.AddMessage("正在下发身份认证失效命令,请稍候....");
                 if (Stop) return;
                 MeterProtocolAdapter.Instance.SouthCmdNoData("070002FF");
                 ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 3);
             }


             //读取关状态下的主动上报状态字
             if (Stop) return;
             MessageController.Instance.AddMessage("正在读取关状态下的主动上报状态字");
             string[] strStatusClose = MeterProtocolAdapter.Instance.ReadData("04001501", 14);
             MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "关状态下的主动上报状态字", strStatusClose);

             iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

             //设置主动上报状态字，恢复默认值
             Common.Memset(ref strDataCode, "04001104");
             Common.Memset(ref strData, "04001104" + "00000000008104BB");//电表运行特征字 1
             bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
           
           
            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                   
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strStatusOpen1[i] == "" || strStatusOpenReset[i] == "" || strStatusClose[i] == "")
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回状态值为空";
                    continue;
                }
                else
                {
                    string strOpenTime = strStatusOpen1[i].Substring(0, 2);//次数
                    string strOpenSB = strStatusOpen1[i].Substring(6, 2);//上报状态字


                    string strOpenResetTime = strStatusOpenReset[i].Substring(0, 2);//次数
                    string strOpenResetSB = strStatusOpenReset[i].Substring(6, 2);//上报状态字

                    string strOpenCloseTime = strStatusClose[i].Substring(0, 2);//次数
                    string strOpenCloseSB = strStatusClose[i].Substring(6, 2);//上报状态字
                    //打开校时状态
                    if (strOpenTime != "01")
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                        reasonS[i] = "状态字打开情况下，上报状态字次数不对，应该为1;";
                        continue;
                    }
                    if (strOpenSB != "01")
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                        reasonS[i] = "状态字打开情况下，上报状态字不对，应该为01;";
                        continue;
                    }
                    //打开校时，复位后状态
                    if (strOpenResetTime != "AA")
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                        reasonS[i] = "状态字复位情况下，上报状态字次数不对;";
                        continue;
                    }
                    if (strOpenResetSB != "00")
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                        reasonS[i] = "状态字复位情况下，上报状态字不对，应该为00;";
                        continue;
                    }
                    //关校时状态
                    if (strOpenCloseTime != "AA")
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                        reasonS[i] = " 状态字关情况下，上报状态字次数不对";
                        continue;
                    }
                    if (strOpenCloseSB != "00")
                    {
                        ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                        reasonS[i] = " 状态字关情况下，上报状态字不对，应该为00;";
                        continue;
                    }
                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                }
               
            }

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

            ////主动上报状态字复位时间
            //Common.Memset(ref strDataCode, "04001405");
            //Common.Memset(ref strData, "04001405" + "30");// 1 分钟
            //bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);


            ////打开允许上报开关
            Common.Memset(ref strDataCode, "04001101");
            Common.Memset(ref strData, "04001101" + "00");//电表运行特征字 1 
            bResult = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strDataCode);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);

            UploadTestResult("结论");
        }
    }
}

