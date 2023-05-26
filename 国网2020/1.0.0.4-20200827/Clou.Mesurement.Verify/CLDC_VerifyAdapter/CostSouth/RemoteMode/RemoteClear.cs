using System;
using System.Collections.Generic;
using System.Text;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_SafeFileProtocol;
using System.Windows.Forms;
using CLDC_DataCore.Function;
using CLDC_SafeFileProtocol.Protocols;
using CLDC_DataCore.Const;

namespace CLDC_VerifyAdapter.CostSouth.RemoteMode
{
    /// <summary>
    /// 远程清零
    /// </summary>
    public class RemoteClear : VerifyBase
    {
        protected override string ItemKey
        {
            get { throw new NotImplementedException(); }
        }

        protected override string ResultKey
        {
            get { throw new NotImplementedException(); }
        }

        //远程模式不可钱包初始化1
        //远程报警状态清零前-清零后2|异常插卡总次数清零前-清零后2|合闸状态清零前-清零后2|保电状态清零前-清零后2
        //上1次跳闸时刻清零前-清零后2|上1次合闸时刻清零前-清零后2|上1次编程事件清零前-清零后2|校时总次数清零前-清零后2|密钥更新总次数清零前-清零后2
        //当前正向有功总电能清零前-清零后2|上1结算日正向有功总电能清零前-清零后2|清零总次数清零前-清零后2|远程清零命令下发时刻2|上1次清零发生时刻2|远程模式远程清零2
        //保电下清零后状态3|非保电下清零后状态3|远程清零命令3|远程清零不清保电3

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "远程模式不可钱包初始化1", 
                                         "非法插卡总次数清零前一清零后2","异常插卡总次数清零前一清零后2","远程报警状态清零前一清零后2","继电器状态位清零前一清零后2","保电状态清零前一清零后2",
                                         "上1次跳闸时刻清零前一清零后2","上1次编程事件清零前一清零后2","校时总次数清零前一清零后2","密钥更新总次数清零前一清零后2",
                                         "当前正向有功总电能清零前一清零后2","上1结算日正向有功总电能清零前一清零后2","清零总次数清零前一清零后2","远程清零命令下发时刻2","上1次清零发生时刻2","远程模式远程清零2",
                                         "保电下清零后状态3","非保电下清零后状态3","远程清零命令3","远程清零不清保电3",
                                         "结论" };
            return true;
        }

        public RemoteClear(object plan)
            : base(plan)
        {

        }


        public override void Verify()
        {
            base.Verify();
            if (Stop) return;
            PowerOn();
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机数
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strRevData = new string[BwCount];
            string[] strOutMac1 = new string[BwCount];
            string[] strOutMac2 = new string[BwCount];
            string[] strRevCode = new string[BwCount];
            string[] strSyMoneyQ = new string[BwCount]; //钱包初始化剩余金额
            string[] strSyMoney = new string[BwCount]; //当前剩余金额
            string[] strKhID = new string[BwCount]; //当前客户编号
            string[] strGdkKhID = new string[BwCount];
            string[] strGdCount = new string[BwCount]; //购电次数
            string[] strData = new string[BwCount];
            bool[] rstTmp = new bool[BwCount];
            bool[,] blnRet = new bool[BwCount, 3];
            int[] iFlag = new int[BwCount];
            bool[] WriteUserResult = new bool[BwCount];
            bool[] BlnIniRet = new bool[BwCount];
            bool[] blnsfRet = new bool[BwCount];
            string[] MyStatus = new string[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] DataTmp = new string[BwCount];
            string[] BuyMoney = new string[BwCount];
            string[] BuyCount = new string[BwCount];
            string[] outData = new string[BwCount];
            bool[] result = new bool[BwCount];
            string[] firstStatus3 = new string[BwCount];
            string[] status3 = new string[BwCount];
            string[] strCode = new string[BwCount];
            string[] strErrInfo = new string[BwCount];
            string[] strShowData = new string[BwCount];

            #region 准备步骤
            //准备
            ChangRemotePreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在进行密钥更新,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            Common.Memset(ref iFlag, 1);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间为2015年6月30日23:58:00,请稍候....");
            string strDatetime = "0400010C" + "150630" + "02" + "235800";
            Common.Memset(ref strRevCode, "0400010C");
            Common.Memset(ref strData, strDatetime);
            bool[] blnSetDateRetZb = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);


            MessageController.Instance.AddMessage("正在走字5分钟以便形成记录,请稍候....");
            bool blnSetRet = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib * 5, 1, 1, "1.0", true, false);
            bool blnIsBreak = false;
            while (!blnIsBreak) //走字5分钟
            {
                if (Stop) return;
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 300);
                blnIsBreak = true;
            }
            PowerOn();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strCode[i] = "0400010C";
                strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strData[i] += DateTime.Now.ToString("HHmmss");
            }
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);

            MessageBox.Show("请插入非法卡，以便形成记录。");
            if (Stop) return;
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在开始寻卡,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthFindCard(0);

            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthFindCard(1);

            #endregion

            if (GlobalUnit.IsNZLoadRelayControl && GlobalUnit.IsDan)
            {
                #region 内置

                try
                {
                    //1-----------   第一步
                    //私钥下进行钱包初始化
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
                    Common.Memset(ref strData, "00002710");
                    result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        blnRet[i, 0] = result[i];
                        ResultDictionary["远程模式不可钱包初始化1"][i] = !blnRet[i, 0] ? "通过" : "不通过";
                    }
                    UploadTestResult("远程模式不可钱包初始化1");

                    //2----------------------第二步
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行设置合闸允许金额限值0元,请稍候....");
                    Common.Memset(ref strRevCode, "04001005");
                    Common.Memset(ref strData, "04001005" + "00000000");
                    result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
                    Common.Memset(ref strData, "3B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                    bool[] resultBdjc = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在发送远程报警命令,请稍候....");
                    string strDateTime = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
                    Common.Memset(ref strData, "2A00" + strDateTime);
                    bool[] resultBJ = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

                    if (Stop) return;
                    //拉闸延时 
                    MessageController.Instance.AddMessage("跳闸延时时间,请稍候....");
                    for (int i = 0; i < BwCount; i++)
                    {
                        strCode[i] = "04001401";
                        strData[i] = "04001401" + "0000";
                    }
                    bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
                    System.Threading.Thread.Sleep(200);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发拉闸命令,请稍候....");
                    Common.Memset(ref strData, "1A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                    bool[] resultLZ = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                    //清零前
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                    status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        if (!string.IsNullOrEmpty(status3[i]))
                        {

                            if ((Convert.ToInt32(status3[i], 16) & 0x1000) != 0x1000)
                            {
                                ResultDictionary["保电状态清零前一清零后2"][i] = "非保电";
                            }
                            else
                            {
                                ResultDictionary["保电状态清零前一清零后2"][i] = "保电";
                            }
                            if ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080)
                            {
                                ResultDictionary["远程报警状态清零前一清零后2"][i] = "报警";
                            }
                            else
                            {
                                ResultDictionary["远程报警状态清零前一清零后2"][i] = "无报警";
                            }
                        }
                        else
                        {
                            ResultDictionary["继电器状态位清零前一清零后2"][i] = "异常";
                            ResultDictionary["保电状态清零前一清零后2"][i] = "异常";
                            ResultDictionary["远程报警状态清零前一清零后2"][i] = "异常";
                        }
                    }


                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取非法刷卡次数,请稍候....");
                    string[] strFFCountQ = MeterProtocolAdapter.Instance.ReadData("03301400", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
                    string[] strYcCountQ = MeterProtocolAdapter.Instance.ReadData("03301300", 3);


                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1次跳闸时刻,请稍候....");
                    string[] strTzDateQ = MeterProtocolAdapter.Instance.ReadData("1D000101", 6);

                    //if (Stop) return;
                    //MessageController.Instance.AddMessage("正在读取上1次合闸时刻,请稍候....");
                    //string[] strHzDateQ = MeterProtocolAdapter.Instance.ReadData("1E000101", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1次编程事件,请稍候....");
                    string[] strBcsjQ = MeterProtocolAdapter.Instance.ReadData("03300001", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取校时总次数,请稍候....");
                    string[] strXsCountQ = MeterProtocolAdapter.Instance.ReadData("03300400", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取密钥更新总次数,请稍候....");
                    string[] strUpdateKeyCountQ = MeterProtocolAdapter.Instance.ReadData("03301200", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取当前正向有功总电量,请稍候....");
                    string[] strMeterEnergyQ = MeterProtocolAdapter.Instance.ReadData("00010000", 4);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1结算日正向有功总电能,请稍候....");
                    string[] strJsrMeterEnergyQ = MeterProtocolAdapter.Instance.ReadData("00010001", 4);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取清零总次数,请稍候....");
                    string[] strQlCountQ = MeterProtocolAdapter.Instance.ReadData("03300100", 3);


                    //清零前
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在发送电表清零命令,请稍候....");
                    bool[] blnMeterClearRet = MeterProtocolAdapter.Instance.SouthDataClear1(iFlag, strRand2);
                    string strDate = DateTime.Now.ToString("yyMMddHHmmss");
                    if (Stop) return;
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);





                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取非法刷卡次数,请稍候....");
                    string[] strFFCountH = MeterProtocolAdapter.Instance.ReadData("03301400", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1次跳闸时刻,请稍候....");
                    string[] strTzDateH = MeterProtocolAdapter.Instance.ReadData("1D000101", 6);

                    //if (Stop) return;
                    //MessageController.Instance.AddMessage("正在读取上1次合闸时刻,请稍候....");
                    //string[] strHzDateH = MeterProtocolAdapter.Instance.ReadData("1E000101", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1次编程事件,请稍候....");
                    string[] strBcsjH = MeterProtocolAdapter.Instance.ReadData("03300001", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取校时总次数,请稍候....");
                    string[] strXsCountH = MeterProtocolAdapter.Instance.ReadData("03300400", 3);


                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取密钥更新总次数,请稍候....");
                    string[] strUpdateKeyCountH = MeterProtocolAdapter.Instance.ReadData("03301200", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取当前正向有功总电量,请稍候....");
                    string[] strMeterEnergyH = MeterProtocolAdapter.Instance.ReadData("00010000", 4);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1结算日正向有功总电能,请稍候....");
                    string[] strJsrMeterEnergyH = MeterProtocolAdapter.Instance.ReadData("00010001", 4);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取清零总次数,请稍候....");
                    string[] strQlCountH = MeterProtocolAdapter.Instance.ReadData("03300100", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1次清零发生时刻,请稍候....");
                    string[] strQlDate = MeterProtocolAdapter.Instance.ReadData("03300101", 6);

                    //异常插卡总次数  异常插卡总次数清零前-清零后2

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
                    string[] strYcCountH = MeterProtocolAdapter.Instance.ReadData("03301300", 3);
                    if (Stop) return;
                    blnSetRet = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0.05f, 1, 1, "1.0", true, false);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                    status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                    if (Stop) return;
                    PowerOn();


                    //清零后
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        if (!string.IsNullOrEmpty(status3[i]))
                        {
                            if ((Convert.ToInt32(status3[i], 16) & 0x1000) != 0x1000)
                            {
                                ResultDictionary["保电状态清零前一清零后2"][i] += "-非保电";
                                ResultDictionary["非保电下清零后状态3"][i] = "非保电";
                            }
                            else
                            {
                                ResultDictionary["保电状态清零前一清零后2"][i] += "-保电";
                                ResultDictionary["非保电下清零后状态3"][i] = "保电";
                            }
                            if ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080)
                            {
                                ResultDictionary["远程报警状态清零前一清零后2"][i] += "-报警";
                            }
                            else
                            {
                                ResultDictionary["远程报警状态清零前一清零后2"][i] += "-无报警";
                            }
                        }
                        else
                        {
                            ResultDictionary["继电器状态位清零前一清零后2"][i] += "-异常";
                            ResultDictionary["保电状态清零前一清零后2"][i] += "-异常";
                            ResultDictionary["远程报警状态清零前一清零后2"][i] += "-异常";
                            ResultDictionary["非保电下清零后状态3"][i] = "异常";
                        }
                    }


                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (strDate != "00".PadLeft(12, '0') && strQlDate[i] != "00".PadLeft(12, '0') && !string.IsNullOrEmpty(strQlCountH[i]) && !string.IsNullOrEmpty(strQlCountQ[i]))
                            {
                                int iDateErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strDate), DateTimes.FormatStringToDateTime(strQlDate[i]));
                                if (ResultDictionary["远程报警状态清零前一清零后2"][i] == "报警-无报警"
                                   && ResultDictionary["保电状态清零前一清零后2"][i] == "非保电-非保电" && strTzDateH[i] == "00".PadLeft(12, '0')
                                   && strBcsjH[i] == "00".PadLeft(12, '0') && strXsCountH[i] == "00".PadLeft(6, '0')
                                   && strUpdateKeyCountH[i] == "000000" && strMeterEnergyH[i] == "00".PadLeft(8, '0') && strJsrMeterEnergyH[i] == "00".PadLeft(8, '0')
                                  && strFFCountH[i] == "000000" && strYcCountH[i] == "000000"
                                   && Convert.ToInt32(strQlCountH[i]) == Convert.ToInt32(strQlCountQ[i]) + 1 && iDateErr <= 300)
                                {
                                    blnRet[i, 1] = true;
                                }
                            }
                            ResultDictionary["非法插卡总次数清零前一清零后2"][i] = strFFCountQ[i] + "-" + strFFCountH[i];
                            ResultDictionary["异常插卡总次数清零前一清零后2"][i] = strYcCountQ[i] + "-" + strYcCountH[i];
                            ResultDictionary["上1次跳闸时刻清零前一清零后2"][i] = strTzDateQ[i] + "-" + strTzDateH[i];
                            //ResultDictionary["上1次合闸时刻清零前一清零后2"][i] = strHzDateQ[i] + "-" + strHzDateH[i];
                            ResultDictionary["上1次编程事件清零前一清零后2"][i] = strBcsjQ[i] + "-" + strBcsjH[i];
                            ResultDictionary["校时总次数清零前一清零后2"][i] = strXsCountQ[i] + "-" + strXsCountH[i];
                            ResultDictionary["密钥更新总次数清零前一清零后2"][i] = strUpdateKeyCountQ[i] + "-" + strUpdateKeyCountH[i];
                            ResultDictionary["当前正向有功总电能清零前一清零后2"][i] = strMeterEnergyQ[i] + "-" + strMeterEnergyH[i];
                            ResultDictionary["上1结算日正向有功总电能清零前一清零后2"][i] = strJsrMeterEnergyQ[i] + "-" + strJsrMeterEnergyH[i];
                            ResultDictionary["清零总次数清零前一清零后2"][i] = strQlCountQ[i] + "-" + strQlCountH[i];
                            ResultDictionary["远程清零命令下发时刻2"][i] = strDate;
                            ResultDictionary["上1次清零发生时刻2"][i] = strQlDate[i];
                            ResultDictionary["远程模式远程清零2"][i] = blnRet[i, 1] ? "通过" : "不通过";

                        }
                    }

                    Common.Memset(ref strShowData, "该项不启用");
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "继电器状态位清零前一清零后2", strShowData);

                    UploadTestResult("远程报警状态清零前一清零后2");
                    UploadTestResult("保电状态清零前一清零后2");
                    UploadTestResult("上1次跳闸时刻清零前一清零后2");
                    //UploadTestResult("上1次合闸时刻清零前一清零后2");
                    UploadTestResult("上1次编程事件清零前一清零后2");
                    UploadTestResult("上1结算日正向有功总电能清零前一清零后2");
                    UploadTestResult("校时总次数清零前一清零后2");
                    UploadTestResult("非法插卡总次数清零前一清零后2");
                    UploadTestResult("异常插卡总次数清零前一清零后2");
                    UploadTestResult("密钥更新总次数清零前一清零后2");
                    UploadTestResult("当前正向有功总电能清零前一清零后2");
                    UploadTestResult("清零总次数清零前一清零后2");
                    UploadTestResult("远程清零命令下发时刻2");
                    UploadTestResult("上1次清零发生时刻2");
                    UploadTestResult("远程模式远程清零2");


                    //3---------------
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发保电命令,请稍候....");
                    Common.Memset(ref strData, "3A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                    result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);


                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行密钥恢复,请稍候....");
                    result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                    Common.Memset(ref iFlag, 0);



                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在发送电表清零命令,请稍候....");
                    blnMeterClearRet = MeterProtocolAdapter.Instance.SouthDataClear1(iFlag, strRand2);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                    status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        if (!string.IsNullOrEmpty(status3[i]))
                        {
                            if ((Convert.ToInt32(status3[i], 16) & 0x1000) == 0x1000)
                            {
                                ResultDictionary["保电下清零后状态3"][i] = "保电";

                            }
                            else
                            {
                                ResultDictionary["保电下清零后状态3"][i] = "非保电";
                            }
                        }
                        else
                        {
                            ResultDictionary["保电下清零后状态3"][i] = "异常";
                        }
                        if (ResultDictionary["保电下清零后状态3"][i] == "保电" && ResultDictionary["非保电下清零后状态3"][i] == "非保电" && blnMeterClearRet[i])
                        {
                            blnRet[i, 2] = true;
                        }
                        ResultDictionary["远程清零命令3"][i] = blnMeterClearRet[i] ? "正常应答" : "异常应答";
                        ResultDictionary["远程清零不清保电3"][i] = blnRet[i, 2] ? "通过" : "不通过";
                    }

                    UploadTestResult("保电下清零后状态3");
                    UploadTestResult("非保电下清零后状态3");
                    UploadTestResult("远程清零命令3");
                    UploadTestResult("远程清零不清保电3");

                    //---------4--------------------------
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
                    Common.Memset(ref strData, "3B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                    bool[] blnSetJcbdRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                    //判断结论
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (!blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2])
                            {
                                ResultDictionary["结论"][i] = "合格";
                            }
                            else
                            {
                                ResultDictionary["结论"][i] = "不合格";
                            }
                        }
                    }
                    //通知界面
                    UploadTestResult("结论");

                }
                catch (Exception ex)
                {

                    throw;
                }
                #endregion
            }
            else
            {
                #region 外置 

                try
                {
                    //1-----------   第一步
                    //私钥下进行钱包初始化
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
                    Common.Memset(ref strData, "00002710");
                    result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        blnRet[i, 0] = result[i];
                        ResultDictionary["远程模式不可钱包初始化1"][i] = !blnRet[i, 0] ? "通过" : "不通过";
                    }
                    UploadTestResult("远程模式不可钱包初始化1");

                    //2----------------------第二步
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行设置合闸允许金额限值0元,请稍候....");
                    Common.Memset(ref strRevCode, "04001005");
                    Common.Memset(ref strData, "04001005" + "00000000");
                    result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
                    Common.Memset(ref strData, "3B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                    bool[] resultBdjc = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在发送远程报警命令,请稍候....");
                    string strDateTime = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");
                    Common.Memset(ref strData, "2A00" + strDateTime);
                    bool[] resultBJ = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);

                    if (Stop) return;
                    //拉闸延时 
                    MessageController.Instance.AddMessage("跳闸延时时间,请稍候....");
                    for (int i = 0; i < BwCount; i++)
                    {
                        strCode[i] = "04001401";
                        strData[i] = "04001401" + "0000";
                    }
                    bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
                    System.Threading.Thread.Sleep(200);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发拉闸命令,请稍候....");
                    Common.Memset(ref strData, "1A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                    bool[] resultLZ = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                    //清零前
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                    status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        if (!string.IsNullOrEmpty(status3[i]))
                        {

                            if ((Convert.ToInt32(status3[i], 16) & 0x0010) != 0x0010)
                            {
                                ResultDictionary["继电器状态位清零前一清零后2"][i] = "通";
                            }
                            else
                            {
                                ResultDictionary["继电器状态位清零前一清零后2"][i] = "断";
                            }

                            if ((Convert.ToInt32(status3[i], 16) & 0x1000) != 0x1000)
                            {
                                ResultDictionary["保电状态清零前一清零后2"][i] = "非保电";
                            }
                            else
                            {
                                ResultDictionary["保电状态清零前一清零后2"][i] = "保电";
                            }
                            if ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080)
                            {
                                ResultDictionary["远程报警状态清零前一清零后2"][i] = "报警";
                            }
                            else
                            {
                                ResultDictionary["远程报警状态清零前一清零后2"][i] = "无报警";
                            }
                        }
                        else
                        {
                            ResultDictionary["继电器状态位清零前一清零后2"][i] = "异常";
                            ResultDictionary["保电状态清零前一清零后2"][i] = "异常";
                            ResultDictionary["远程报警状态清零前一清零后2"][i] = "异常";
                        }
                    }


                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取非法刷卡次数,请稍候....");
                    string[] strFFCountQ = MeterProtocolAdapter.Instance.ReadData("03301400", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
                    string[] strYcCountQ = MeterProtocolAdapter.Instance.ReadData("03301300", 3);


                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1次跳闸时刻,请稍候....");
                    string[] strTzDateQ = MeterProtocolAdapter.Instance.ReadData("1D000101", 6);

                    //if (Stop) return;
                    //MessageController.Instance.AddMessage("正在读取上1次合闸时刻,请稍候....");
                    //string[] strHzDateQ = MeterProtocolAdapter.Instance.ReadData("1E000101", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1次编程事件,请稍候....");
                    string[] strBcsjQ = MeterProtocolAdapter.Instance.ReadData("03300001", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取校时总次数,请稍候....");
                    string[] strXsCountQ = MeterProtocolAdapter.Instance.ReadData("03300400", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取密钥更新总次数,请稍候....");
                    string[] strUpdateKeyCountQ = MeterProtocolAdapter.Instance.ReadData("03301200", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取当前正向有功总电量,请稍候....");
                    string[] strMeterEnergyQ = MeterProtocolAdapter.Instance.ReadData("00010000", 4);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1结算日正向有功总电能,请稍候....");
                    string[] strJsrMeterEnergyQ = MeterProtocolAdapter.Instance.ReadData("00010001", 4);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取清零总次数,请稍候....");
                    string[] strQlCountQ = MeterProtocolAdapter.Instance.ReadData("03300100", 3);


                    //清零前
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在发送电表清零命令,请稍候....");
                    bool[] blnMeterClearRet = MeterProtocolAdapter.Instance.SouthDataClear1(iFlag, strRand2);
                    string strDate = DateTime.Now.ToString("yyMMddHHmmss");
                    if (Stop) return;
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);





                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取非法刷卡次数,请稍候....");
                    string[] strFFCountH = MeterProtocolAdapter.Instance.ReadData("03301400", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1次跳闸时刻,请稍候....");
                    string[] strTzDateH = MeterProtocolAdapter.Instance.ReadData("1D000101", 6);

                    //if (Stop) return;
                    //MessageController.Instance.AddMessage("正在读取上1次合闸时刻,请稍候....");
                    //string[] strHzDateH = MeterProtocolAdapter.Instance.ReadData("1E000101", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1次编程事件,请稍候....");
                    string[] strBcsjH = MeterProtocolAdapter.Instance.ReadData("03300001", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取校时总次数,请稍候....");
                    string[] strXsCountH = MeterProtocolAdapter.Instance.ReadData("03300400", 3);


                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取密钥更新总次数,请稍候....");
                    string[] strUpdateKeyCountH = MeterProtocolAdapter.Instance.ReadData("03301200", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取当前正向有功总电量,请稍候....");
                    string[] strMeterEnergyH = MeterProtocolAdapter.Instance.ReadData("00010000", 4);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1结算日正向有功总电能,请稍候....");
                    string[] strJsrMeterEnergyH = MeterProtocolAdapter.Instance.ReadData("00010001", 4);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取清零总次数,请稍候....");
                    string[] strQlCountH = MeterProtocolAdapter.Instance.ReadData("03300100", 3);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取上1次清零发生时刻,请稍候....");
                    string[] strQlDate = MeterProtocolAdapter.Instance.ReadData("03300101", 6);

                    //异常插卡总次数  异常插卡总次数清零前-清零后2

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
                    string[] strYcCountH = MeterProtocolAdapter.Instance.ReadData("03301300", 3);
                    if (Stop) return;
                    blnSetRet = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0.05f, 1, 1, "1.0", true, false);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                    status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                    if (Stop) return;
                    PowerOn();


                    //清零后
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        if (!string.IsNullOrEmpty(status3[i]))
                        {
                            if ((Convert.ToInt32(status3[i], 16) & 0x0010) != 0x0010)
                            {
                                ResultDictionary["继电器状态位清零前一清零后2"][i] += "-通";
                            }
                            else
                            {
                                ResultDictionary["继电器状态位清零前一清零后2"][i] += "-断";
                            }
                            if ((Convert.ToInt32(status3[i], 16) & 0x1000) != 0x1000)
                            {
                                ResultDictionary["保电状态清零前一清零后2"][i] += "-非保电";
                                ResultDictionary["非保电下清零后状态3"][i] = "非保电";
                            }
                            else
                            {
                                ResultDictionary["保电状态清零前一清零后2"][i] += "-保电";
                                ResultDictionary["非保电下清零后状态3"][i] = "保电";
                            }
                            if ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080)
                            {
                                ResultDictionary["远程报警状态清零前一清零后2"][i] += "-报警";
                            }
                            else
                            {
                                ResultDictionary["远程报警状态清零前一清零后2"][i] += "-无报警";
                            }
                        }
                        else
                        {
                            ResultDictionary["继电器状态位清零前一清零后2"][i] += "-异常";
                            ResultDictionary["保电状态清零前一清零后2"][i] += "-异常";
                            ResultDictionary["远程报警状态清零前一清零后2"][i] += "-异常";
                            ResultDictionary["非保电下清零后状态3"][i] = "异常";
                        }
                    }


                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (strDate != "00".PadLeft(12, '0') && strQlDate[i] != "00".PadLeft(12, '0') && !string.IsNullOrEmpty(strQlCountH[i]) && !string.IsNullOrEmpty(strQlCountQ[i]))
                            {
                                int iDateErr = DateTimes.DateDiff(DateTimes.FormatStringToDateTime(strDate), DateTimes.FormatStringToDateTime(strQlDate[i]));
                                if (ResultDictionary["远程报警状态清零前一清零后2"][i] == "报警-无报警" && ResultDictionary["继电器状态位清零前一清零后2"][i] == "断-通"
                                   && ResultDictionary["保电状态清零前一清零后2"][i] == "非保电-非保电" && strTzDateH[i] == "00".PadLeft(12, '0')
                                   && strBcsjH[i] == "00".PadLeft(12, '0') && strXsCountH[i] == "00".PadLeft(6, '0')
                                   && strUpdateKeyCountH[i] == "000000" && strMeterEnergyH[i] == "00".PadLeft(8, '0') && strJsrMeterEnergyH[i] == "00".PadLeft(8, '0')
                                  && strFFCountH[i] == "000000" && strYcCountH[i] == "000000"
                                   && Convert.ToInt32(strQlCountH[i]) == Convert.ToInt32(strQlCountQ[i]) + 1 && iDateErr <= 300)
                                {
                                    blnRet[i, 1] = true;
                                }
                            }
                            ResultDictionary["非法插卡总次数清零前一清零后2"][i] = strFFCountQ[i] + "-" + strFFCountH[i];
                            ResultDictionary["异常插卡总次数清零前一清零后2"][i] = strYcCountQ[i] + "-" + strYcCountH[i];
                            ResultDictionary["上1次跳闸时刻清零前一清零后2"][i] = strTzDateQ[i] + "-" + strTzDateH[i];
                            //ResultDictionary["上1次合闸时刻清零前一清零后2"][i] = strHzDateQ[i] + "-" + strHzDateH[i];
                            ResultDictionary["上1次编程事件清零前一清零后2"][i] = strBcsjQ[i] + "-" + strBcsjH[i];
                            ResultDictionary["校时总次数清零前一清零后2"][i] = strXsCountQ[i] + "-" + strXsCountH[i];
                            ResultDictionary["密钥更新总次数清零前一清零后2"][i] = strUpdateKeyCountQ[i] + "-" + strUpdateKeyCountH[i];
                            ResultDictionary["当前正向有功总电能清零前一清零后2"][i] = strMeterEnergyQ[i] + "-" + strMeterEnergyH[i];
                            ResultDictionary["上1结算日正向有功总电能清零前一清零后2"][i] = strJsrMeterEnergyQ[i] + "-" + strJsrMeterEnergyH[i];
                            ResultDictionary["清零总次数清零前一清零后2"][i] = strQlCountQ[i] + "-" + strQlCountH[i];
                            ResultDictionary["远程清零命令下发时刻2"][i] = strDate;
                            ResultDictionary["上1次清零发生时刻2"][i] = strQlDate[i];
                            ResultDictionary["远程模式远程清零2"][i] = blnRet[i, 1] ? "通过" : "不通过";

                        }
                    }

                    UploadTestResult("远程报警状态清零前一清零后2");
                    UploadTestResult("继电器状态位清零前一清零后2");
                    UploadTestResult("保电状态清零前一清零后2");
                    UploadTestResult("上1次跳闸时刻清零前一清零后2");
                    //UploadTestResult("上1次合闸时刻清零前一清零后2");
                    UploadTestResult("上1次编程事件清零前一清零后2");
                    UploadTestResult("上1结算日正向有功总电能清零前一清零后2");
                    UploadTestResult("校时总次数清零前一清零后2");
                    UploadTestResult("非法插卡总次数清零前一清零后2");
                    UploadTestResult("异常插卡总次数清零前一清零后2");
                    UploadTestResult("密钥更新总次数清零前一清零后2");
                    UploadTestResult("当前正向有功总电能清零前一清零后2");
                    UploadTestResult("清零总次数清零前一清零后2");
                    UploadTestResult("远程清零命令下发时刻2");
                    UploadTestResult("上1次清零发生时刻2");
                    UploadTestResult("远程模式远程清零2");


                    //3---------------
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发保电命令,请稍候....");
                    Common.Memset(ref strData, "3A00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                    result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);


                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行密钥恢复,请稍候....");
                    result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("00", 17, strRand2, strEsamNo);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                    Common.Memset(ref iFlag, 0);



                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在发送电表清零命令,请稍候....");
                    blnMeterClearRet = MeterProtocolAdapter.Instance.SouthDataClear1(iFlag, strRand2);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                    status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        if (!string.IsNullOrEmpty(status3[i]))
                        {
                            if ((Convert.ToInt32(status3[i], 16) & 0x1000) == 0x1000)
                            {
                                ResultDictionary["保电下清零后状态3"][i] = "保电";

                            }
                            else
                            {
                                ResultDictionary["保电下清零后状态3"][i] = "非保电";
                            }
                        }
                        else
                        {
                            ResultDictionary["保电下清零后状态3"][i] = "异常";
                        }
                        if (ResultDictionary["保电下清零后状态3"][i] == "保电" && ResultDictionary["非保电下清零后状态3"][i] == "非保电" && blnMeterClearRet[i])
                        {
                            blnRet[i, 2] = true;
                        }
                        ResultDictionary["远程清零命令3"][i] = blnMeterClearRet[i] ? "正常应答" : "异常应答";
                        ResultDictionary["远程清零不清保电3"][i] = blnRet[i, 2] ? "通过" : "不通过";
                    }

                    UploadTestResult("保电下清零后状态3");
                    UploadTestResult("非保电下清零后状态3");
                    UploadTestResult("远程清零命令3");
                    UploadTestResult("远程清零不清保电3");

                    //-----------------------------------
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
                    Common.Memset(ref strData, "3B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                    bool[] blnSetJcbdRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                    //判断结论
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (!blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2])
                            {
                                ResultDictionary["结论"][i] = "合格";
                            }
                            else
                            {
                                ResultDictionary["结论"][i] = "不合格";
                            }
                        }
                    }
                    //通知界面
                    UploadTestResult("结论");

                }
                catch (Exception ex)
                {

                    throw;
                }
                #endregion
            }
        }
    }
}
