using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 保电功能
    /// </summary>
    public class RemoteEnPower : VerifyBase
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

        public RemoteEnPower(object plan)
            : base(plan)
        {
        }

        //保电状态位前-后1|保电命令1|
        //保电状态位|远程跳闸命令2|继电器命令状态位前-后2|保电下不可跳闸2|
        //保电状态位前-后3|保电解除命令3


        protected override bool CheckPara()
        {

            ResultNames = new string[] {"保电状态位前一后1","保电命令1",
                                        "保电状态位2","远程跳闸命令2","继电器命令状态位前一后2","保电下不可跳闸2",
                                        "保电状态位前一后3","保电解除命令3", 
                                        "结论" };
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
            bool[,] blnResult = new bool[BwCount, 3];
            int[] iFlag = new int[BwCount];
            string[] str_ID = new string[BwCount];
            string[] str_Data = new string[BwCount];
            string[] strHzDate = new string[BwCount];
            bool[] result = new bool[BwCount]; 


            //Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);

            try
            {
                //准备工作
                ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                Common.Memset(ref iFlag, 1);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置时间,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    str_ID[i] = "0400010C";
                    str_Data[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                    str_Data[i] += DateTime.Now.ToString("HHmmss");
                }
                bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, str_Data, str_ID);

                if (Stop) return;
                Common.Memset(ref strData, "1B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                Common.Memset(ref strHzDate, DateTime.Now.ToString("yyMMddHHmmss"));
                MessageController.Instance.AddMessage("正在下发合闸命令,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
                Common.Memset(ref strData, "3B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                //1---------------
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                string[] status = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status[i]))
                    {
                        if ((Convert.ToInt32(status[i], 16) & 0x1000) == 0x1000)
                        {
                            ResultDictionary["保电状态位前一后1"][i] = "保电";
                        }
                        else
                        {
                            ResultDictionary["保电状态位前一后1"][i] = "非保电";
                        }
                    }
                    else
                    {
                        ResultDictionary["保电状态位前一后1"][i] = "异常";
                    }
                }

                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发保电命令,请稍候....");
                Common.Memset(ref strData, "3A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                bool[] Rest = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 *15);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status[i]))
                    {

                        if ((Convert.ToInt32(status[i], 16) & 0x1000) == 0x1000)
                        {
                            ResultDictionary["保电状态位前一后1"][i] += "-保电";
                            ResultDictionary["保电状态位2"][i] = "保电";
                        }
                        else
                        {
                            ResultDictionary["保电状态位前一后1"][i] += "-非保电";
                            ResultDictionary["保电状态位2"][i] = "非保电";
                        }
                    }
                    else
                    {
                        ResultDictionary["保电状态位前一后1"][i] += "-异常";
                    }
                    if (ResultDictionary["保电状态位前一后1"][i] == "非保电-保电")
                    {
                        blnResult[i, 0] = true;
                    }
                    ResultDictionary["保电命令1"][i] = blnResult[i, 0] ? "通过" : "不通过";
                }
                UploadTestResult("保电状态位前一后1");
                UploadTestResult("保电命令1");
                UploadTestResult("保电状态位2");

                //2--------------
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status[i]))
                    {
                        if ((Convert.ToInt32(status[i], 16) & 0x0040) != 0x0040)
                        {
                            ResultDictionary["继电器命令状态位前一后2"][i] = "通";
                        }
                        else
                        {
                            ResultDictionary["继电器命令状态位前一后2"][i] = "断";
                        }
                    }
                    else
                    {
                        ResultDictionary["继电器命令状态位前一后2"][i] = "异常";
                    }
                }

                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发拉闸命令,请稍候....");
                Common.Memset(ref strData, "1A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                Rest = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status[i]))
                    {
                        if ((Convert.ToInt32(status[i], 16) & 0x0040) != 0x0040)
                        {
                            ResultDictionary["继电器命令状态位前一后2"][i] += "-通";
                        }
                        else
                        {
                            ResultDictionary["继电器命令状态位前一后2"][i] += "-断";
                        }
                    }
                    else
                    {
                        ResultDictionary["继电器命令状态位前一后2"][i] += "-异常";
                    }
                }

                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        if (!Rest[i] && ResultDictionary["继电器命令状态位前一后2"][i] == "通-通")
                        {
                            blnResult[i, 1] = true;
                        }

                        ResultDictionary["远程跳闸命令2"][i] = !Rest[i] ? "异常应答" : "正常应答";
                        ResultDictionary["保电下不可跳闸2"][i] = blnResult[i, 1] ? "通过" : "不通过";
                    }

                }
                UploadTestResult("继电器命令状态位前一后2");
                UploadTestResult("远程跳闸命令2");
                UploadTestResult("保电下不可跳闸2");

                //3---------------
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status[i]))
                    {
                        if ((Convert.ToInt32(status[i], 16) & 0x1000) == 0x1000)
                        {
                            ResultDictionary["保电状态位前一后3"][i] = "保电";
                        }
                        else
                        {
                            ResultDictionary["保电状态位前一后3"][i] = "非保电";
                        }
                    }
                    else
                    {
                        ResultDictionary["保电状态位前一后3"][i] = "异常";
                    }
                }
                

                if (Stop) return;
                MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
                Common.Memset(ref strData, "3B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                Rest = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                status = MeterProtocolAdapter.Instance.ReadData("04000503", 2);

                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (!string.IsNullOrEmpty(status[i]))
                    {
                        if ((Convert.ToInt32(status[i], 16) & 0x1000) == 0x1000)
                        {
                            ResultDictionary["保电状态位前一后3"][i] += "-保电";
                        }
                        else
                        {
                            ResultDictionary["保电状态位前一后3"][i] += "-非保电";
                        }
                    }
                    else
                    {
                        ResultDictionary["保电状态位前一后3"][i] += "-异常";
                    }
                    if (ResultDictionary["保电状态位前一后3"][i] == "保电-非保电")
                    {
                        blnResult[i, 2] = true;
                    }

                    ResultDictionary["保电解除命令3"][i] = blnResult[i, 2] ? "通过" : "不通过";
                }
                UploadTestResult("保电状态位前一后3");
                UploadTestResult("保电解除命令3");

                MessageController.Instance.AddMessage("正在处理结果,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (blnResult[i, 0] && blnResult[i, 1] && blnResult[i, 2])
                    {
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                    }
                }

                UploadTestResult("结论");
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }
    }
}
