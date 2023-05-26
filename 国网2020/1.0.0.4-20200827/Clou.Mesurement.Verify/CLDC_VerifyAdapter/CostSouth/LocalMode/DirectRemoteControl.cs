using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;
using CLDC_DataCore.Const;
using System.Windows.Forms;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 直接合闸测试1C
    /// </summary>
    public class DirectRemoteControl: VerifyBase
    {
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

       public DirectRemoteControl(object plan)
           : base(plan)
        {
        }

        //继电器命令状态位测试前-测试后|合闸信号输出|命令下发时间|上1次合闸记录|合闸次数前-后|直接合闸命令


       protected override bool CheckPara()
       {
           ResultNames = new string[] { "继电器状态位前一后", "合闸信号输出（外置）","表内电流前一后（内置）", "命令下发时间", "电能表功率前一后（内置）",
                                         "上1次合闸记录", "合闸次数前一后", "直接合闸命令", "结论" };
           return true;
       }

       public override void Verify()
       {


           if (!GlobalUnit.IsNZLoadRelayControl)
           {
               if (GlobalUnit.IsDan)
               {
                   Helper.EquipHelper.Instance.SetRelayControl(6);
               }
               else
               {
                   Helper.EquipHelper.Instance.SetRelayControl(7);
               }
           }

           base.Verify();

           if (Stop) return;
           PowerOn();

           string[] strRand1 = new string[BwCount];//随机数
           string[] strRand2 = new string[BwCount];//随机数
           string[] strEsamNo = new string[BwCount];//Esam序列号
           string[] strData = new string[BwCount];//明文
           string strDateTime = "";
           int[] iFlag = new int[BwCount];
           string[] strCode = new string[BwCount];
           string[] status3 = new string[BwCount];
           string[] statusTmp = new string[BwCount];
           string[] statusTmp1 = new string[BwCount];
           string[] strHzDate = new string[BwCount];
           bool[,] blnRet = new bool[BwCount, 6];
           bool[] result = new bool[BwCount];
           string[] strFhkg = new string[BwCount];
           bool[] blnFhkg = new bool[BwCount];
           string[] strDataPut = new string[BwCount];
           bool[] blnYaojianMeter = new bool[BwCount];
           string[] strRevCode = new string[BwCount];

           ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

           if (Stop) return;
           MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
           Common.Memset(ref strData, "00002710");
           bool[] blnQbRet = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
           ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

           if (Stop) return;
           MessageController.Instance.AddMessage("正在进行设置透支金额限值0元,请稍候....");
           Common.Memset(ref strRevCode, "04001003");
           Common.Memset(ref strData, "04001003" + "00000000");
           result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

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
               strCode[i] = "0400010C";
               strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
               strData[i] += DateTime.Now.ToString("HHmmss");
           }
           bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);

           if (Stop) return;
           MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
           Common.Memset(ref strData, "3B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
           bln_Rst = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

           if (Stop) return;
           MessageController.Instance.AddMessage("正在设置跳闸延时时间为0分钟,请稍候....");
           Common.Memset(ref strCode, "04001401");
           Common.Memset(ref strDataPut, "04001401" + "0000");
           result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strDataPut, strCode);

           if (Stop) return;
           int[] iHeZhaCount3Q = ReadMeterHeZhaCount();


           if (Stop) return;
           MessageController.Instance.AddMessage("正在通过远程发送跳闸命令,请稍候....");
           strDateTime = System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
           Common.Memset(ref strData, "1A00" + strDateTime);
           bool[] blnTzRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
           ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

           if (GlobalUnit.IsNZLoadRelayControl && GlobalUnit.IsDan)
           {
               #region 内置

               if (Stop) return;
               Helper.EquipHelper.Instance.SetLoadRelayControl(GlobalUnit.blnYaoJianMeter, 1);
               if (Stop) return;
               Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, 1, "1.0", true, false);
               ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 4);
               //if (Stop) return;
               //MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
               //status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
               //for (int i = 0; i < BwCount; i++)
               //{
               //    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
               //    if (!string.IsNullOrEmpty(status3[i]))
               //    {
               //        if ((Convert.ToInt32(status3[i], 16) & 0x0040) == 0x0040)
               //        {
               //            statusTmp[i] = "断";
               //            blnRet[i, 0] = true;
               //        }
               //        else
               //        {
               //            statusTmp[i] = "通";
               //        }
               //    }
               //    else
               //    {
               //        statusTmp[i] = "异常";
               //    }
               //}

               if (Stop) return;
               MessageController.Instance.AddMessage("正在读取表内电流,请稍候....");
               string[] strMeterIbQ = MeterProtocolAdapter.Instance.ReadData("02020100", 3);
               strMeterIbQ = Common.StringConverToDecimaByIb(strMeterIbQ);
               for (int i = 0; i < BwCount; i++)
               {
                   if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                   if (!string.IsNullOrEmpty(strMeterIbQ[i]))
                   {
                       ResultDictionary["表内电流前一后（内置）"][i] = strMeterIbQ[i];
                   }
                   else
                   {
                       ResultDictionary["表内电流前一后（内置）"][i] = "异常";
                   }
               }

               if (Stop) return;
               MessageController.Instance.AddMessage("正在读取表内功率,请稍候....");
               string[] strMeterGLQ = MeterProtocolAdapter.Instance.ReadData("02030000", 3);
               strMeterGLQ = Common.StringConverToDecimaByGL(strMeterGLQ);
               for (int i = 0; i < BwCount; i++)
               {
                   if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                   if (!string.IsNullOrEmpty(strMeterGLQ[i]))
                   {
                       ResultDictionary["电能表功率前一后（内置）"][i] = strMeterGLQ[i];
                   }
                   else
                   {
                       ResultDictionary["电能表功率前一后（内置）"][i] = "异常";
                   }
               }

               if (Stop) return;
               Helper.EquipHelper.Instance.PowerOff();


               int iCount = BwCount / 6;
               for (int i = 0; i < iCount; i++)
               {
                   bool BlnBj = false;
                   for (int j = 0; j < 6; j++)
                   {
                       int iBw = (6 * i) + j;
                       if (Helper.MeterDataHelper.Instance.Meter(iBw).YaoJianYn)
                       {
                           BlnBj = true;
                           break;
                       }
                   }

                   if (BlnBj)
                   {
                       MessageBox.Show("正在准备检测继电器状态,请在台体上没有电流的情况下准备好第" + (i * 6 + 1) + "-" + ((i + 1) * 6) + "表位中要检表位的检测环境后按确定。");
                       if (Stop) return;
                       PowerOn();
                       ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 4);
                   }

                   for (int j = 0; j < 6; j++)
                   {
                       int iBw = (6 * i) + j;
                       if (!Helper.MeterDataHelper.Instance.Meter(iBw).YaoJianYn) continue;
                       MessageController.Instance.AddMessage("正在读取第" + (iBw + 1) + "表位状态字3,请稍候....");

                       if (Stop) return;
                       status3[iBw] = MeterProtocolAdapter.Instance.ReadDataByPos("04000503", 2, iBw);
                       if (!string.IsNullOrEmpty(status3[iBw]))
                       {
                           if ((Convert.ToInt32(status3[iBw], 16) & 0x0010) == 0x0010)
                           {
                               statusTmp1[iBw] = "断";
                               blnRet[iBw, 1] = true;
                           }
                           else
                           {
                               statusTmp1[iBw] = "通";
                           }
                       }
                       else
                       {
                           statusTmp1[iBw] = "异常";
                       }
                   }

                   if (BlnBj)
                   {
                       if (Stop) return;
                       Helper.EquipHelper.Instance.PowerOff();
                       MessageBox.Show("表继电器状态检测完成,请把表重新挂回台体后按确定。");
                   }
               }


               if (Stop) return;
               Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, 1, "1.0", true, false);

               if (Stop) return;
               MessageController.Instance.AddMessage("正在进行身份认证,请稍候....");
               result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

               if (Stop) return;
               MessageController.Instance.AddMessage("正在通过远程发送直接合闸命令,请稍候....");
               strDateTime = System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
               Common.Memset(ref strData, "1C00" + strDateTime);
               Common.Memset(ref strHzDate, DateTime.Now.ToString("yyMMddHHmmss"));
               bool[] blnHzRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
               ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

               if (Stop) return;
               MessageController.Instance.AddMessage("正在读取合闸事件记录,请稍候....");
               string[] strHzsjjl = MeterProtocolAdapter.Instance.ReadData("1E00FF01", 34);
               MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次合闸记录", strHzsjjl);
               MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "命令下发时间", strHzDate);

               if (Stop) return;
               MessageController.Instance.AddMessage("正在读取表内电流,请稍候....");
               string[] strMeterIbH = MeterProtocolAdapter.Instance.ReadData("02020100", 3);
               strMeterIbH = Common.StringConverToDecimaByIb(strMeterIbH);
               for (int i = 0; i < BwCount; i++)
               {
                   if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                   if (!string.IsNullOrEmpty(strMeterIbH[i]))
                   {
                       ResultDictionary["表内电流前一后（内置）"][i] += "-" + strMeterIbH[i];
                   }
                   else
                   {
                       ResultDictionary["表内电流前一后（内置）"][i] += "-异常";
                   }
               }

               if (Stop) return;
               MessageController.Instance.AddMessage("正在读取表内功率,请稍候....");
               string[] strMeterGLH = MeterProtocolAdapter.Instance.ReadData("02030000", 3);
               strMeterGLH = Common.StringConverToDecimaByGL(strMeterGLH);
               for (int i = 0; i < BwCount; i++)
               {
                   if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                   if (!string.IsNullOrEmpty(strMeterGLH[i]))
                   {
                       ResultDictionary["电能表功率前一后（内置）"][i] += "-" + strMeterGLH[i];
                   }
                   else
                   {
                       ResultDictionary["电能表功率前一后（内置）"][i] += "-异常";
                   }
               }
               UploadTestResult("电能表功率前一后（内置）");

               if (Stop) return;
               int[] iHeZhaCount3H = ReadMeterHeZhaCount();

               if (Stop) return;
               MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
               status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
               for (int i = 0; i < BwCount; i++)
               {
                   if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                   if (!string.IsNullOrEmpty(status3[i]))
                   {
                       //if ((Convert.ToInt32(status3[i], 16) & 0x0040) == 0x0040)
                       //{
                       //    statusTmp[i] += "-断";
                       //}
                       //else
                       //{
                       //    statusTmp[i] += "-通";
                       //    blnRet[i, 2] = true;
                       //}
                       if ((Convert.ToInt32(status3[i], 16) & 0x0010) == 0x0010)
                       {
                           statusTmp1[i] += "-断";
                       }
                       else
                       {
                           statusTmp1[i] += "-通";
                           blnRet[i, 3] = true;
                       }
                   }
                   else
                   {
                       statusTmp[i] += "-异常";
                       statusTmp1[i] += "-异常";
                   }

                   ResultDictionary["直接合闸命令"][i] = blnHzRet[i] ? "成功" : "失败";
                   ResultDictionary["合闸次数前一后"][i] = iHeZhaCount3Q[i].ToString() + "-" + iHeZhaCount3H[i].ToString();
               }
               UploadTestResult("合闸次数前一后");
               UploadTestResult("表内电流前一后（内置）");
               UploadTestResult("直接合闸命令");

               //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "继电器命令状态位前一后", statusTmp);
               MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "继电器状态位前一后", statusTmp1);
               Common.Memset(ref strData, "该项不启用");
               MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "合闸信号输出（外置）", strData);
               //继电器命令状态位测试前-测试后|合闸信号输出|命令下发时间|上1次合闸记录|合闸次数前-后|直接合闸命令

               if (Stop) return;
               Helper.EquipHelper.Instance.SetLoadRelayControl(GlobalUnit.blnYaoJianMeter, 0);

               for (int i = 0; i < BwCount; i++)
               {
                   if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || strHzsjjl[i].ToString().Length < 68 || string.IsNullOrEmpty(iHeZhaCount3H[i].ToString()) || string.IsNullOrEmpty(iHeZhaCount3Q[i].ToString())
                     || string.IsNullOrEmpty(strMeterIbQ[i]) || string.IsNullOrEmpty(strMeterIbH[i]) || string.IsNullOrEmpty(strMeterGLQ[i]) || string.IsNullOrEmpty(strMeterGLH[i])) continue;
                   string strDateTmp = strHzsjjl[i].Substring(strHzsjjl[i].Length - 12, 12);
                   if (string.IsNullOrEmpty(strDateTmp) || strDateTmp == "000000000000") continue;
                   int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strHzDate[i]), DateTimes.FormatStringToDateTime(strDateTmp));

                   if (iErr < 180 && ResultDictionary["直接合闸命令"][i].Equals("成功") && !string.IsNullOrEmpty(iHeZhaCount3H[i].ToString()) && !string.IsNullOrEmpty(iHeZhaCount3Q[i].ToString()) && Convert.ToInt32(iHeZhaCount3H[i]) == Convert.ToInt32(iHeZhaCount3Q[i]) + 1
                       && strMeterIbQ[i] == "0" && strMeterIbH[i] != "0" && (strMeterGLQ[i] == "0" || strMeterGLQ[i] == "-0") && strMeterGLH[i] != "0" && strMeterGLH[i] != "-0")
                   {
                       blnRet[i, 4] = true;
                   }
               }

               //判断结论
               for (int i = 0; i < BwCount; i++)
               {
                   if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                   {
                       if (blnRet[i, 1] && blnRet[i, 3] && blnRet[i, 4])
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



               #endregion

           }
           else
           {
               #region 外置
               if (Stop) return;
               MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
               status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
               for (int i = 0; i < BwCount; i++)
               {
                   if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                   if (!string.IsNullOrEmpty(status3[i]))
                   {
                       //if ((Convert.ToInt32(status3[i], 16) & 0x0040) == 0x0040)
                       //{
                       //    statusTmp[i] = "断";
                       //    blnRet[i, 0] = true;
                       //}
                       //else
                       //{
                       //    statusTmp[i] = "通";
                       //}
                       if ((Convert.ToInt32(status3[i], 16) & 0x0010) == 0x0010)
                       {
                           statusTmp1[i] = "断";
                           blnRet[i, 1] = true;
                       }
                       else
                       {
                           statusTmp1[i] = "通";
                       }
                   }
                   else
                   {
                       //statusTmp[i] = "异常";
                       statusTmp1[i] = "异常";
                   }
               }
               if (Stop) return;
               bool blnResult = Helper.EquipHelper.Instance.SetRelayControl(2);

               if (Stop) return;
               MessageController.Instance.AddMessage("正在通过远程发送直接合闸命令,请稍候....");
               strDateTime = System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
               Common.Memset(ref strData, "1C00" + strDateTime);
               Common.Memset(ref strHzDate, DateTime.Now.ToString("yyMMddHHmmss"));
               bool[] blnHzRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
               ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

               Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, 1, "1.0", true, false);

               if (Stop) return;
               MessageController.Instance.AddMessage("正在读取合闸事件记录,请稍候....");
               string[] strHzsjjl = MeterProtocolAdapter.Instance.ReadData("1E00FF01", 34);
               MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "上1次合闸记录", strHzsjjl);
               MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "命令下发时间", strHzDate);

               //清除表位清除预付费跳闸状态
               //Helper.EquipHelper.Instance.ClearBwStatus(Helper.MeterDataHelper.Instance.GetYaoJian(), 2);

               if (Stop) return;
               MessageController.Instance.AddMessage("正在读取负荷控制开关输出信号,请稍候....");
               CLDC_DeviceDriver.stError[] stStatus = Helper.EquipHelper.Instance.ReadWcb(false);

               if (Stop) return;
               int[] iHeZhaCount3H = ReadMeterHeZhaCount();

               if (Stop) return;
               MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
               status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
               for (int i = 0; i < BwCount; i++)
               {
                   if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                   if (!string.IsNullOrEmpty(status3[i]))
                   {
                       //if ((Convert.ToInt32(status3[i], 16) & 0x0040) == 0x0040)
                       //{
                       //    statusTmp[i] += "-断";
                       //}
                       //else
                       //{
                       //    statusTmp[i] += "-通";
                       //    blnRet[i, 2] = true;
                       //}
                       if ((Convert.ToInt32(status3[i], 16) & 0x0010) == 0x0010)
                       {
                           statusTmp1[i] += "-断";
                       }
                       else
                       {
                           statusTmp1[i] += "-通";
                           blnRet[i, 3] = true;
                       }
                   }
                   else
                   {
                       statusTmp[i] += "-异常";
                       statusTmp1[i] += "-异常";
                   }
                   blnFhkg[i] = stStatus[i].statusTypeIsOnErr_Yfftz;
                   {
                       strFhkg[i] = blnFhkg[i] ? "是" : "否";
                   }
                   ResultDictionary["合闸信号输出（外置）"][i] = strFhkg[i];

                   ResultDictionary["直接合闸命令"][i] = blnHzRet[i] ? "成功" : "失败";
                   ResultDictionary["合闸次数前一后"][i] = iHeZhaCount3Q[i].ToString() + "-" + iHeZhaCount3H[i].ToString();
               }
               UploadTestResult("合闸次数前一后");
               UploadTestResult("合闸信号输出（外置）");
               UploadTestResult("直接合闸命令");

               //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "继电器命令状态位前一后", statusTmp);
               MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "继电器状态位前一后", statusTmp1);

               Common.Memset(ref strData, "该项不启用");
               MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "电能表功率前一后（内置）", strData);
               MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内电流前一后（内置）", strData);

               //继电器命令状态位测试前-测试后|合闸信号输出|命令下发时间|上1次合闸记录|合闸次数前-后|直接合闸命令


               for (int i = 0; i < BwCount; i++)
               {
                   if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || strHzsjjl[i].ToString().Length < 68 || string.IsNullOrEmpty(iHeZhaCount3H[i].ToString()) || string.IsNullOrEmpty(iHeZhaCount3Q[i].ToString())) continue;
                   string strDateTmp = strHzsjjl[i].Substring(strHzsjjl[i].Length - 12, 12);
                   if (string.IsNullOrEmpty(strDateTmp) || strDateTmp == "000000000000") continue;
                   int iErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strHzDate[i]), DateTimes.FormatStringToDateTime(strDateTmp));


                   if (iErr < 180 && ResultDictionary["直接合闸命令"][i].Equals("成功") && !string.IsNullOrEmpty(iHeZhaCount3H[i].ToString()) && !string.IsNullOrEmpty(iHeZhaCount3Q[i].ToString()) && Convert.ToInt32(iHeZhaCount3H[i]) == Convert.ToInt32(iHeZhaCount3Q[i]) + 1
                       && ResultDictionary["合闸信号输出（外置）"][i] == "是")
                   {
                       blnRet[i, 4] = true;
                   }
               }

               //判断结论
               for (int i = 0; i < BwCount; i++)
               {
                   if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                   {
                       if (blnRet[i, 1] && blnRet[i, 3] && blnRet[i, 4])
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

               #endregion
           }

       }

       /// <summary>
       /// 读取合闸次数
       /// </summary>
       /// <returns></returns>
       private int[] ReadMeterHeZhaCount()
       {
           //if (Stop) return;
           MessageController.Instance.AddMessage("正在读取合闸次数....");
           string[] strlaZhaCount = MeterProtocolAdapter.Instance.ReadData("1E000001", 3);
           int[] iLaZhaCount = new int[BwCount];

           for (int i = 0; i < BwCount; i++)
           {
               if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn || string.IsNullOrEmpty(strlaZhaCount[i])) continue;
               string strTmp = strlaZhaCount[i];
               if (strTmp != "000000")
               {
                   strTmp = strTmp.TrimStart('0');
                   iLaZhaCount[i] = Convert.ToInt32(strTmp);
               }
               else
               {
                   iLaZhaCount[i] = 0;
               }
           }
           return iLaZhaCount;
       }

    }
}
