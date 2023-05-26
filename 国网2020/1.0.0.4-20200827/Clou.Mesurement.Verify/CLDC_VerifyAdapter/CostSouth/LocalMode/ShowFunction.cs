using System;
using System.Collections.Generic;
using System.Text;
using CLDC_DataCore;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 显示功能
    /// </summary>
   public class ShowFunction:VerifyBase
    {
               protected override string ItemKey
        {
            get { throw new NotImplementedException(); }
        }

        protected override string ResultKey
        {
            get { throw new NotImplementedException(); }
        }

        //拉闸显示|合闸显示|远程报警显示|远程报警解除显示

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "拉闸显示1", "合闸显示1","远程报警显示2","远程报警解除显示3","结论" };
            return true;
        }

        public ShowFunction(object plan)
            : base(plan)
        {
        }

        public override void Verify()
        {
            base.Verify();
            PowerOn();
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strData = new string[BwCount];//明文
            string[] strID = new string[BwCount];
            string[] strDataTmp = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 3];
            int[] iFlag = new int[BwCount];
            bool[] result = new bool[BwCount];

            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strID[i] = "0400010C";
                strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strData[i] += DateTime.Now.ToString("HHmmss");
            }
            bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);

            //1-----------------
            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置跳闸延时时间为10分钟,请稍候....");
            Common.Memset(ref strData, "04001401" + "0010");
            Common.Memset(ref strID,"04001401");
            
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);

            if (Stop) return;
            Common.Memset(ref strData, "1A00" + System.DateTime.Now.AddMinutes(10).ToString("yyMMddHHmmss"));
            MessageController.Instance.AddMessage("正在通过远程发送跳闸命令,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                blnRet[i, 0] = result[i];
                ResultDictionary["拉闸显示1"][i] = result[i] ? "成功" : "失败";
                ResultDictionary["合闸显示1"][i] = result[i] ? "成功" : "失败";
            }
            UploadTestResult("拉闸显示1");
            UploadTestResult("合闸显示1");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送远程报警命令,请稍候....");
            Common.Memset(ref strData,"2A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
            result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                blnRet[i, 1] = result[i];
                ResultDictionary["远程报警显示2"][i] = result[i] ? "成功" : "失败";
            }
            UploadTestResult("远程报警显示2");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送远程报警解除命令,请稍候....");
            Common.Memset(ref strData, "2B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
            result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                blnRet[i, 2] = result[i];
                ResultDictionary["远程报警解除显示3"][i] = result[i] ? "成功" : "失败";
            }
            UploadTestResult("远程报警解除显示3");

            //处理结果
            MessageController.Instance.AddMessage("正在处理结果,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i,2])
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }
                }
            }
            UploadTestResult("结论");

        }
    }
}
