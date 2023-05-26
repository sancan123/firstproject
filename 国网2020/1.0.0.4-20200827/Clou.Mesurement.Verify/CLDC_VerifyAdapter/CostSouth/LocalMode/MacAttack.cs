using CLDC_DataCore;
using CLDC_DataCore.Function;
using System;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// MAC挂起测试
    /// </summary>
    public class MacAttack : VerifyBase
    {
        protected override string ItemKey
        {
            // get { throw new System.NotImplementedException(); }
            get { return null; }
        }
        protected override string ResultKey
        {
            //get { throw new System.NotImplementedException(); }
            get { return null; }
        }

        public MacAttack(object plan)
            : base(plan)
        {
        }

        protected override bool CheckPara()
        {
            //return base.CheckPara();
            ResultNames = new string[] { "设置时间为23∶59∶55", "195次Mac攻击", "设置时间为23∶55∶00",
                                         "10次Mac攻击", "设置前时间5","设置后时间5","零点前不可设置日期时间",
                                         "设置前时间6","设置后时间6","零点后可设置日期时间", "结论" };
            return true;
        }

        public override void Verify()
        {
            base.Verify();
            if (Stop) return;
            PowerOn();
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strData = new string[BwCount];
            string[] strDate = new string[BwCount];
            string[] strCode = new string[BwCount];
            string[] str_OutData = new string[BwCount];
            string[] str_OutMac = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 6];
            int[] iFlag = new int[BwCount];
            int[] iCount195 = new int[BwCount];
            int[] iCount10 = new int[BwCount];
            bool[] result = new bool[BwCount];
            string[] str_Apdu = new string[BwCount];

            //Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);

            try
            {
                //准备工作
                ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置时效为30分钟,请稍候....");
                Common.Memset(ref strCode, "070001FF");
                Common.Memset(ref strData, "0030");
                Common.Memset(ref str_Apdu, "04D6812B06");
                bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, str_Apdu, strCode, strData);

                

                //1--------------
                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置时间为23:59:55秒,请稍候....");
                Common.Memset(ref strData, "04000102" + "235955");
                Common.Memset(ref strCode, "04000102");

                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    blnRet[i, 0] = result[i];
                    ResultDictionary["设置时间为23∶59∶55"][i] = result[i] == true ? "通过" : "失败";
                }
                UploadTestResult("设置时间为23∶59∶55");

                if (Stop) return;
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 25);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);


                MessageController.Instance.AddMessage("准备进行195次Mac攻击,请稍候....");
                for (int i = 0; i < 195; i++)
                {

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行第" + (i + 1) + "次Mac攻击....");
                    result = MeterProtocolAdapter.Instance.SouthParameterElseUpdateByMac(iFlag, strRand2, strData, strCode);
                    for (int j = 0; j < BwCount; j++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(j).YaoJianYn) continue;
                        if (!result[j])
                        {
                            iCount195[j]++;
                            blnRet[j, 1] = true;
                        }
                    }
                }

                ConvertTestResult("195次Mac攻击", iCount195);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置时间为23:55:00秒,请稍候....");
                Common.Memset(ref strData, "04000102" + "235500");
                Common.Memset(ref strCode, "04000102");

                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    blnRet[i, 2] = result[i];
                    ResultDictionary["设置时间为23∶55∶00"][i] = result[i] == true ? "通过" : "失败";
                }
                UploadTestResult("设置时间为23∶55∶00");

                MessageController.Instance.AddMessage("准备进行10次Mac攻击,请稍候....");

                for (int i = 0; i < 10; i++)
                {

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行第" + (i + 1) + "次Mac攻击....");
                    result = MeterProtocolAdapter.Instance.SouthParameterElseUpdateByMac(iFlag, strRand2, strData, strCode);
                    for (int j = 0; j < BwCount; j++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(j).YaoJianYn) continue;
                        if (!result[j])
                        {
                            iCount10[j]++;
                            blnRet[j, 3] = true;
                        }
                    }
                }

                ConvertTestResult("10次Mac攻击", iCount10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电表时间,请稍候....");
                DateTime[] dateTimeQ = MeterProtocolAdapter.Instance.ReadDateTime();
                if (Stop) return;
                MessageController.Instance.AddMessage("正在发送远程设置参数【设置时间】,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    strCode[i] = "0400010C";
                    strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                    strData[i] += DateTime.Now.ToString("HHmmss");
                }
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电表时间,请稍候....");
                DateTime[] dateTimeH = MeterProtocolAdapter.Instance.ReadDateTime();

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    blnRet[i, 4] = !result[i];
                    ResultDictionary["零点前不可设置日期时间"][i] = !result[i] ? "通过" : "不通过";
                    ResultDictionary["设置前时间5"][i] = dateTimeQ[i].ToString();
                    ResultDictionary["设置后时间5"][i] = dateTimeH[i].ToString();
                }
                UploadTestResult("设置前时间5");
                UploadTestResult("设置后时间5");
                UploadTestResult("零点前不可设置日期时间");


                if (Stop) return;
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 360);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthFindCard(1);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电表时间,请稍候....");
                dateTimeQ = MeterProtocolAdapter.Instance.ReadDateTime();

                if (Stop) return;
                MessageController.Instance.AddMessage("正在发送远程设置参数【设置时间】,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    strCode[i] = "0400010C";
                    strDate[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                    strDate[i] += DateTime.Now.ToString("HHmmss");
                }
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strDate, strCode);
                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取电表时间,请稍候....");
                dateTimeH = MeterProtocolAdapter.Instance.ReadDateTime();
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    blnRet[i, 5] = result[i];
                    ResultDictionary["零点后可设置日期时间"][i] = result[i] ? "通过" : "不通过";
                    ResultDictionary["设置前时间6"][i] = dateTimeQ[i].ToString();
                    ResultDictionary["设置后时间6"][i] = dateTimeH[i].ToString();
                }
                UploadTestResult("设置前时间6");
                UploadTestResult("设置后时间6");
                UploadTestResult("零点后可设置日期时间");

                MessageController.Instance.AddMessage("正在处理结果,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3] && blnRet[i, 4] && blnRet[i, 5])
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
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}
