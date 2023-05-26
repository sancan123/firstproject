
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.Safe
{
    /// <summary>
    /// （计）正式密钥时钟设置
    /// </summary>
    class RightKetClockSet : VerifyBase
    {
        public RightKetClockSet(object plan)
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
            ResultNames = new string[] { "测试时间", "结论", "不合格原因" };
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
            bool[] f = new bool[BwCount];// 操作
            bool[] fKey = new bool[BwCount];// 操作

        
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取校时前总次数");
            string[] strLoseCountQ = MeterProtocolAdapter.Instance.ReadData("03300400", 3);
           



            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            f = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行密钥更新....");
            fKey = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            string UpdateKeyDate = DateTime.Now.ToString("yyMMddHHmmss");
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

          
            //设置1次校时
            for (int it = 0; it < 1; it++)
            {
                string datetime = DateTime.Now.AddMinutes(it + 1).ToString("yyMMddHHmmss");//每次对时加1小时
                for (int i = 0; i < BwCount; i++)
                {
                    strCode[i] = "0400010C";
                    strSetData[i] = datetime.Substring(0, 6) + "0" + (int)DateTime.Now.DayOfWeek;
                    strSetData[i] += datetime.Substring(6, 6);
                    strShowData[i] = datetime;
                    strData[i] = strCode[i] + strSetData[i];
                }
                if (Stop) return;
                MessageController.Instance.AddMessage("正在第" + (it + 1) + "次校时....");
                f = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);
            }
           

           
        
            if (Stop) return;
            CLDC_DataCore.Function.Common.Memset(ref iFlag, 1);
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
             MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行密钥恢复....");
            bool[] blnUpKeyRet = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
      
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取校时后总次数");
            string[] strLoseCountH = MeterProtocolAdapter.Instance.ReadData("03300400", 3);
            

            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (strLoseCountH[i] == "" || strLoseCountQ[i] == ""  )
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "返回日期或次数值为空";
                    continue;
                }
                if (strLoseCountQ[i].Contains("F") || strLoseCountH[i].Contains("F")   )
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "次数值带F值"; continue;
                }
                    if (fKey[i])
                {
                      ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "正式密钥更新不成功！";
                  
                }
                           if (f[i])
                {
                      ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "正式密钥下，校时不成功！";
                  
                }
                else if ((!string.IsNullOrEmpty(strLoseCountQ[i]) && !string.IsNullOrEmpty(strLoseCountH[i]) && Convert.ToInt32(strLoseCountQ[i]) + 1 == Convert.ToInt32(strLoseCountH[i]))&& f[i])
                {

                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                }
               
                else
                {
                  
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "正式密钥下，校时次数没增加！";
                  
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);

            UploadTestResult("结论");
        }
    }
}
