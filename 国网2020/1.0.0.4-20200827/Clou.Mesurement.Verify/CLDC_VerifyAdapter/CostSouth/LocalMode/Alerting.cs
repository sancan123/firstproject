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
using System.Globalization;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 报警功能
    /// </summary>
    public class Alerting : VerifyBase
    {
        protected override string ItemKey
        {
            get { throw new NotImplementedException(); }
        }

        protected override string ResultKey
        {
            get { throw new NotImplementedException(); }
        }

        //剩余金额1|报警金额1(步骤1)|预跳闸报警状态位1|液晶显示1|剩余金额小于报警金额1报警操作1|
        //剩余金额2|报警金额2(步骤2)|跳闸次数前-后2|跳闸信号输出2|液晶显示2|剩余金额小于报警金额2报警操作2|
        //合闸次数前-后3|合闸信号输出3|液晶显示3|合闸复电3|
        //保电状态4|剩余金额4|报警金额2(步骤4)|跳闸次数前-后4|跳闸信号输出4|保电状态不跳闸4|

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "剩余金额1","报警金额1（步骤1）","预跳闸报警状态位1","剩余金额小于报警金额1报警操作1",
                                        "剩余金额2","报警金额2（步骤2）","跳闸次数前一后2","继电器状态位2","剩余金额小于报警金额2报警操作2",
                                        "合闸次数前一后3","继电器状态位3","合闸复电3",
                                        "保电状态4","剩余金额4","报警金额2（步骤4）","跳闸次数前一后4","继电器状态位4","保电状态不跳闸4",
                                         "结论" };
            return true;
        }

        public Alerting(object plan)
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
            string[] paraFile = new string[12]; //参数信息文件
            string[] walletFile = new string[2];//钱包文件
            string[] priceFile1 = new string[51];//当前套电价文件
            string[] priceFile2 = new string[51];//备用套电价文件
            string[] ControlFilePlain = new string[1]; //合闸明文
            string[] strData = new string[BwCount];
            string[] strParaFileArr = new string[BwCount];  //参数信息文件
            string[] strwalletFileArr = new string[BwCount];//钱包文件
            string[] strpriceFile1Arr = new string[BwCount];//当前套电价文件
            string[] strpriceFile2Arr = new string[BwCount];//备用套电价文件
            string[] strfileReplyArr = new string[BwCount];
            string[] strControlFilePlainArr = new string[BwCount];  //合闸明文
            bool[] rstTmp = new bool[BwCount];
            bool[,] blnRet = new bool[BwCount, 5];
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
            string[] status3 = new string[BwCount];
            string[] strBj1 = new string[BwCount];
            string[] strBj2 = new string[BwCount];
            string[] strRevMac = new string[BwCount];
            string[] strID = new string[BwCount];
            string[] strPutApdu = new string[BwCount];
            string[] strErrInfo = new string[BwCount];
            string[] strShowData = new string[BwCount];

            // Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);


                #region 准备步骤
                //准备工作
                ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置透支金额限值0元,请稍候....");
                Common.Memset(ref strRevCode, "04001003");
                Common.Memset(ref strData, "04001003" + "00000000");
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
                Common.Memset(ref strData, "000186A0");
                bool[] blnQbRet = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                string[] strTjMoney = new string[BwCount];
                MessageController.Instance.AddMessage("正在设置囤积金额限值为0,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    strRevCode[i] = "04001004";
                    strTjMoney[i] = "00000000";
                    strData[i] = strRevCode[i] + strTjMoney[i];
                }
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在密钥更新,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                Common.Memset(ref iFlag, 1);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行远程开户,请稍候....");
                Common.Memset(ref strData, "000186A0" + "00000001" + "112233445566");
                result = MeterProtocolAdapter.Instance.SouthIncreasePurse(0, iFlag, strRand2, strData, out strErrInfo);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置时间,请稍候....");
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    strID[i] = "0400010C";
                    strData[i] = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                    strData[i] += DateTime.Now.ToString("HHmmss");
                }
                bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                //设置备用套费率
                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置备用套费率,请稍候....");
                Common.Memset(ref strID, "040502FF");
                Common.Memset(ref strData, "00000000" + "00000000" + "00000000" + "00000000");
                Common.Memset(ref strPutApdu, "04D6840414");
                result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置两套费率电价切换时间,请稍候....");
                Common.Memset(ref strID, "04000108");
                Common.Memset(ref strData, DateTime.Now.AddMinutes(-5).ToString("yyMMddHHmm"));
                Common.Memset(ref strPutApdu, "04D6810A09");
                result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置备用套阶梯值,请稍候....");
                Common.Memset(ref strID, "04060AFF");

                Common.Memset(ref strData, "00000010" + "00000020" + "00000030" + "00000040" + "00000050" + "00000060"
                           + "00010000" + "00020000" + "00030000" + "00040000" + "00050000" + "00060000" + "00070000"
                           + "000000" + "000000" + "000000" + "000000" + "000000" + "000000");
                Common.Memset(ref strPutApdu, "04D684344A");
                result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置两套阶梯切换时间,请稍候....");
                Common.Memset(ref strID, "04000109");
                Common.Memset(ref strData, DateTime.Now.AddMinutes(-5).ToString("yyMMddHHmm"));
                Common.Memset(ref strPutApdu, "04D684C009");
                result = MeterProtocolAdapter.Instance.SouthPrice2Update(iFlag, strPutApdu, strRand2, strID, strData);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置电流变比,请稍候....");
                Common.Memset(ref strID, "04000306");
                Common.Memset(ref strData, "000010");
                Common.Memset(ref strPutApdu, "04D6811807");
                result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置电压变比,请稍候....");
                Common.Memset(ref strID, "04000307");
                Common.Memset(ref strData, "000010");
                Common.Memset(ref strPutApdu, "04D6811B07");
                result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);


                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置报警金额1,请稍候....");
                Common.Memset(ref strID, "04001001");
                Common.Memset(ref strData, "00193000");
                Common.Memset(ref strPutApdu, "04D6811008");
                result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置报警金额2,请稍候....");
                Common.Memset(ref strID, "04001002");
                Common.Memset(ref strData, "00188000");
                Common.Memset(ref strPutApdu, "04D6811408");
                result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置备用套阶梯时区表,请稍候....");
                Common.Memset(ref strID, "04070001");
                Common.Memset(ref strData, "04070001" + "010101" + "050102" + "110102" + "110102");
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在进行设置两套阶梯时区切换时间,请稍候....");
                Common.Memset(ref strID, "0400010A");
                Common.Memset(ref strData, "0400010A" + DateTime.Now.AddMinutes(-5).ToString("yyMMddHHmm"));
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strID);

                if (Stop) return;
                MessageController.Instance.AddMessage("正在设置时间到1月2日....");
                Common.Memset(ref strRevCode, "04000101");
                string strDatetime = DateTime.Now.ToString("yy") + "-01-02";
                DateTime dt = DateTime.Parse(strDatetime, DateTimeFormatInfo.CurrentInfo);
                Common.Memset(ref strData, "04000101" + DateTime.Now.ToString("yy") + "0102" + "0" + ((int)dt.DayOfWeek).ToString());
                result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);



                #endregion

                if(GlobalUnit.IsNZLoadRelayControl && GlobalUnit.IsDan)
                {
                    #region 内置

                    #region 步骤1
                    //1-----------

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                    result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
                    Common.Memset(ref strData, "3B00" + System.DateTime.Now.AddDays(5).ToString("yyMMddHHmmss"));
                    result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);


                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行设置透支金额限值100元,请稍候....");
                    Common.Memset(ref strRevCode, "04001003");
                    Common.Memset(ref strData, "04001003" + "00010000");
                    result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                    MessageController.Instance.AddMessage("正在走字,请稍候....");
                    bool blnIsBreak = false;
                    bool blnSetRet = false;
                    int iTime = 0;

                        blnSetRet = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib * 5, 1, 1, "1.0", true, false);

                        iTime = Convert.ToInt32((35000 * 3600) / (GlobalUnit.U * GlobalUnit.Ib * 5) / 100);


                        while (!blnIsBreak)
                        {
                            if (Stop) return;
                            bool bValue = ShowWaitTimeMessage("正在等待{0}秒,请稍候....", 1000 * iTime);
                            if (bValue)
                                break;
                        }

                    if (Stop) return;
                    PowerOn();
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取剩余金额,请稍候....");
                    strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额1", Common.StringConverToDecima(strSyMoney));
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取报警金额1,请稍候....");
                    string[] strBjMoney1 = MeterProtocolAdapter.Instance.ReadData("04001001", 4);
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "报警金额1（步骤1）", Common.StringConverToDecima(strBjMoney1));


                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                    status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (!string.IsNullOrEmpty(status3[i]))
                            {
                                if ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080)
                                {
                                    ResultDictionary["预跳闸报警状态位1"][i] = "报警";
                                    ResultDictionary["剩余金额小于报警金额1报警操作1"][i] = "通过";
                                    blnRet[i, 0] = true;
                                }
                                else
                                {
                                    ResultDictionary["预跳闸报警状态位1"][i] = "无报警";
                                    ResultDictionary["剩余金额小于报警金额1报警操作1"][i] = "不通过";
                                }
                            }
                            else
                            {
                                ResultDictionary["预跳闸报警状态位1"][i] = "异常";
                                ResultDictionary["剩余金额小于报警金额1报警操作1"][i] = "不通过";
                            }
                        }
                    }
                    UploadTestResult("预跳闸报警状态位1");
                    UploadTestResult("剩余金额小于报警金额1报警操作1");

                    #endregion

                    #region 步骤2

                    if (Stop) return;
                    Helper.EquipHelper.Instance.SetLoadRelayControl(GlobalUnit.blnYaoJianMeter, 1);

                    if (Stop) return;
                    int[] iLaZhaCount2Q = ReadMeterLaZhaCount();

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                    result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取剩余金额,请稍候....");
                    strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);


                    strData = calMoney(strSyMoney, 1);


                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行设置报警金额2,请稍候....");
                    Common.Memset(ref strID, "04001002");
                    //Common.Memset(ref strData, "00192500");
                    Common.Memset(ref strPutApdu, "04D6811408");
                    result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取报警金额2,请稍候....");
                    string[] strBjMoney2 = MeterProtocolAdapter.Instance.ReadData("04001002", 4);
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "报警金额2（步骤2）", Common.StringConverToDecima(strBjMoney2));


                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在走字2分钟,请稍候....");
                    Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib * 5, 1, 1, "1.0", true, false);
                    blnIsBreak = false;

                    while (!blnIsBreak) //走字10分钟
                    {
                        if (Stop) return;
                        ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60 * 2);
                        blnIsBreak = true;
                    }

                    if (Stop) return;
                    PowerOn();

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取剩余金额,请稍候....");
                    strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额2", Common.StringConverToDecima(strSyMoney));


                    if (Stop) return;
                    int[] iLaZhaCount2H = ReadMeterLaZhaCount();

                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                        if (Convert.ToInt32(iLaZhaCount2H[i]) == Convert.ToInt32(iLaZhaCount2Q[i]) + 1)
                        {

                            ResultDictionary["剩余金额小于报警金额2报警操作2"][i] = "通过";
                            blnRet[i, 1] = true;
                        }
                        else
                        {
                            ResultDictionary["剩余金额小于报警金额2报警操作2"][i] = "不通过";
                        }
                        ResultDictionary["跳闸次数前一后2"][i] = iLaZhaCount2Q[i].ToString() + "-" + iLaZhaCount2H[i].ToString();
                    }

                    Common.Memset(ref strShowData, "该项不启用");
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "继电器状态位2", strShowData);

                    UploadTestResult("跳闸次数前一后2");
                    UploadTestResult("剩余金额小于报警金额2报警操作2");

                    #endregion

                    #region 步骤3

                    if (Stop) return;
                    int[] iHeZhaCount3Q = ReadMeterHeZhaCount();

                    //3--------------------
                    MessageBox.Show("下一试验流程需要手动按键合闸恢复后按确定");

                    blnSetRet = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, 1, "1.0", true, false);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);


                    if (Stop) return;
                    PowerOn();

                    if (Stop) return;
                    int[] iHeZhaCount3H = ReadMeterHeZhaCount();


                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (!string.IsNullOrEmpty(status3[i]))
                            {
                                if (Convert.ToInt32(iHeZhaCount3H[i]) == Convert.ToInt32(iHeZhaCount3Q[i]) + 1)
                                {
                                    ResultDictionary["合闸复电3"][i] = "通过";
                                    blnRet[i, 2] = true;
                                }
                                else
                                {
                                    ResultDictionary["合闸复电3"][i] = "不通过";
                                }
                            }
                            ResultDictionary["合闸次数前一后3"][i] = iHeZhaCount3Q[i].ToString() + "-" + iHeZhaCount3H[i];
                        }
                    }
                    UploadTestResult("合闸次数前一后3");
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "继电器状态位3", strShowData);
                    UploadTestResult("合闸复电3");



                    #endregion

                    #region 步骤4
                    //4-----------------


                    if (Stop) return;
                    int[] iLaZhaCount4Q = ReadMeterLaZhaCount();

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                    result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);


                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发保电命令,请稍候....");
                    Common.Memset(ref  strData, "3A00" + System.DateTime.Now.AddDays(5).ToString("yyMMddHHmmss"));
                    result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                    string[] strRedMeterKhID = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行远程充值,请稍候....");
                    //购电金额+购电次数+客户编号

                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        BuyCount[i] = "00000002";
                        BuyMoney[i] = "000005DC";
                        strData[i] = BuyMoney[i] + BuyCount[i] + strRedMeterKhID[i];
                    }
                    result = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在走字5分钟,请稍候....");
                    Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib * 5, 1, 1, "1.0", true, false);
                    blnIsBreak = false;

                    while (!blnIsBreak) //走字10分钟
                    {
                        if (Stop) return;
                        ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60 *5);
                        blnIsBreak = true;
                    }

                    if (Stop) return;
                    PowerOn();

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取剩余金额,请稍候....");
                    Common.Memset(ref strRevCode, "DF01000200000004");
                    result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out BuyCount, out strSyMoney);
                    if (Stop) return;
                    strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额4", Common.StringConverToDecima(strSyMoney));
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取报警金额2,请稍候....");
                    strBjMoney2 = MeterProtocolAdapter.Instance.ReadData("04001002", 4);
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "报警金额2（步骤4）", Common.StringConverToDecima(strBjMoney2));
                    if (Stop) return;
                    int[] iLaZhaCount4H = ReadMeterLaZhaCount();

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                    status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);


                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {

                            if (!string.IsNullOrEmpty(status3[i]))
                            {
                                if ((Convert.ToInt32(status3[i], 16) & 0x1000) == 0x1000)
                                {
                                    ResultDictionary["保电状态4"][i] = "保电";
                                }
                                else
                                {
                                    ResultDictionary["保电状态4"][i] = "非保电";
                                }
                                if (iLaZhaCount4H[i] == iLaZhaCount4Q[i])
                                {
                                    blnRet[i, 3] = true;
                                }
                            }
                            else
                            {
                                ResultDictionary["继电器状态位4"][i] = "异常";
                                ResultDictionary["保电状态4"][i] = "异常";
                            }
                            ResultDictionary["跳闸次数前一后4"][i] = iLaZhaCount4Q[i].ToString() + "-" + iLaZhaCount4H[i].ToString();
                            ResultDictionary["保电状态不跳闸4"][i] = blnRet[i, 3] ? "通过" : "不通过";
                        }
                    }
                    UploadTestResult("跳闸次数前一后4");
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "继电器状态位4", strShowData);
                    UploadTestResult("保电状态4");
                    UploadTestResult("保电状态不跳闸4");

                    //保电状态4|剩余金额4|报警金额2(步骤4)|跳闸次数前-后|跳闸信号输出4|保电状态不跳闸4|
                    #endregion

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                    result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行设置报警金额1,请稍候....");
                    Common.Memset(ref strID, "04001001");
                    Common.Memset(ref strData, "00002000");
                    Common.Memset(ref strPutApdu, "04D6811008");
                    result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行设置报警金额2,请稍候....");
                    Common.Memset(ref strID, "04001002");
                    Common.Memset(ref strData, ("00001000"));
                    Common.Memset(ref strPutApdu, "04D6811408");
                    result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
                    Common.Memset(ref strData, "3B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                    bool[] blnSetJcbdRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);


                    //-------恢复---
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行设置电流变比,请稍候....");
                    Common.Memset(ref strID, "04000306");
                    Common.Memset(ref strData, "000001");
                    Common.Memset(ref strPutApdu, "04D6811807");
                    result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行设置电压变比,请稍候....");
                    Common.Memset(ref strID, "04000307");
                    Common.Memset(ref strData, "000001");
                    Common.Memset(ref strPutApdu, "04D6811B07");
                    result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);


                    #region 处理结果
                    MessageController.Instance.AddMessage("正在处理结果");
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3])
                        {
                            ResultDictionary["结论"][i] = "合格";
                        }
                        else
                        {
                            ResultDictionary["结论"][i] = "不合格";
                        }
                    }
                    UploadTestResult("结论");
                    #endregion
                    #endregion
                }
                else
                {
                    #region 外置
                    #region 步骤1
                    //1-----------

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                    result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
                    Common.Memset(ref strData, "3B00" + System.DateTime.Now.AddDays(5).ToString("yyMMddHHmmss"));
                    result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);


                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行设置透支金额限值100元,请稍候....");
                    Common.Memset(ref strRevCode, "04001003");
                    Common.Memset(ref strData, "04001003" + "00010000");
                    result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

                    MessageController.Instance.AddMessage("正在走字,请稍候....");
                    bool blnIsBreak = false;
                    bool blnSetRet = false;
                    int iTime = 0;
                    if (GlobalUnit.IsDan)
                    {
                        blnSetRet = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib * 4, 1, 1, "1.0", true, false);

                        iTime = Convert.ToInt32((35000 * 3600) / (GlobalUnit.U * GlobalUnit.Ib * 4) / 100);
                    }
                    else
                    {
                        blnSetRet = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib * 5, 1, 1, "1.0", true, false);

                        iTime = Convert.ToInt32((35000 * 3600) / (GlobalUnit.U * GlobalUnit.Ib * 5 * 3) / 100);
                    }



                    while (!blnIsBreak)
                    {
                        if (Stop) return;
                        bool bValue = ShowWaitTimeMessage("正在等待{0}秒,请稍候....", 1000 * iTime);
                        if (bValue)
                            break;
                    }

                    if (Stop) return;
                    PowerOn();
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取剩余金额,请稍候....");
                    strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额1", Common.StringConverToDecima(strSyMoney));
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取报警金额1,请稍候....");
                    string[] strBjMoney1 = MeterProtocolAdapter.Instance.ReadData("04001001", 4);
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "报警金额1（步骤1）", Common.StringConverToDecima(strBjMoney1));


                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                    status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (!string.IsNullOrEmpty(status3[i]))
                            {
                                if ((Convert.ToInt32(status3[i], 16) & 0x0080) == 0x0080)
                                {
                                    ResultDictionary["预跳闸报警状态位1"][i] = "报警";
                                    ResultDictionary["剩余金额小于报警金额1报警操作1"][i] = "通过";
                                    blnRet[i, 0] = true;
                                }
                                else
                                {
                                    ResultDictionary["预跳闸报警状态位1"][i] = "无报警";
                                    ResultDictionary["剩余金额小于报警金额1报警操作1"][i] = "不通过";
                                }
                            }
                            else
                            {
                                ResultDictionary["预跳闸报警状态位1"][i] = "异常";
                                ResultDictionary["剩余金额小于报警金额1报警操作1"][i] = "不通过";
                            }
                        }
                    }
                    UploadTestResult("预跳闸报警状态位1");
                    UploadTestResult("剩余金额小于报警金额1报警操作1");

                    #endregion

                    #region 步骤2


                    if (Stop) return;
                    int[] iLaZhaCount2Q = ReadMeterLaZhaCount();


                    //2-----------------
                    MessageController.Instance.AddMessage("正在走字,请稍候....");


                    blnIsBreak = false;

                    int iWalkTime = 0;

                    if (GlobalUnit.IsDan)
                    {
                        blnSetRet = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib * 4, 1, 1, "1.0", true, false);

                        iWalkTime = Convert.ToInt32((20000 * 3600) / (GlobalUnit.U * GlobalUnit.Ib * 4) / 100);
                    }
                    else
                    {
                        blnSetRet = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib * 5, 1, 1, "1.0", true, false);

                        iWalkTime = Convert.ToInt32((20000 * 3600) / (GlobalUnit.U * GlobalUnit.Ib * 5 * 3) / 100);
                    }
                    while (true)
                    {
                        if (Stop) return;
                        bool bValue = ShowWaitTimeMessage("正在等待{0}秒,请稍候....", 1000 * iWalkTime);
                        if (bValue)
                            break;
                    }
                    PowerOn();
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取剩余金额,请稍候....");
                    strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);

                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额2", Common.StringConverToDecima(strSyMoney));
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取报警金额2,请稍候....");
                    string[] strBjMoney2 = MeterProtocolAdapter.Instance.ReadData("04001002", 4);
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "报警金额2（步骤2）", Common.StringConverToDecima(strBjMoney2));


                    if (Stop) return;
                    int[] iLaZhaCount2H = ReadMeterLaZhaCount();



                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                    status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        if (!string.IsNullOrEmpty(status3[i]))
                        {
                            if ((Convert.ToInt32(status3[i], 16) & 0x0010) == 0x0010)
                            {

                                ResultDictionary["继电器状态位2"][i] = "拉闸";
                                ResultDictionary["剩余金额小于报警金额2报警操作2"][i] = "通过";
                                blnRet[i, 1] = true;
                            }
                            else
                            {
                                ResultDictionary["继电器状态位2"][i] = "合闸";
                                ResultDictionary["剩余金额小于报警金额2报警操作2"][i] = "不通过";
                            }
                        }
                        else
                        {
                            ResultDictionary["继电器状态位2"][i] = "异常";
                            ResultDictionary["剩余金额小于报警金额2报警操作2"][i] = "不通过";
                        }
                        ResultDictionary["跳闸次数前一后2"][i] = iLaZhaCount2Q[i].ToString() + "-" + iLaZhaCount2H[i].ToString();
                    }
                    UploadTestResult("跳闸次数前一后2");
                    UploadTestResult("继电器状态位2");
                    UploadTestResult("剩余金额小于报警金额2报警操作2");

                    #endregion

                    #region 步骤3

                    if (Stop) return;
                    int[] iHeZhaCount3Q = ReadMeterHeZhaCount();

                    //3--------------------
                    MessageBox.Show("下一试验流程需要手动按键合闸恢复后按确定");

                    blnSetRet = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, 1, "1.0", true, false);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                    if (Stop) return;
                    int[] iHeZhaCount3H = ReadMeterHeZhaCount();

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                    status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                    if (Stop) return;
                    PowerOn();

                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {
                            if (!string.IsNullOrEmpty(status3[i]))
                            {
                                if ((Convert.ToInt32(status3[i], 16) & 0x0010) != 0x0010 && Convert.ToInt32(iHeZhaCount3H[i]) == Convert.ToInt32(iHeZhaCount3Q[i]) + 1)
                                {
                                        ResultDictionary["继电器状态位3"][i] = "合闸";
                                        ResultDictionary["合闸复电3"][i] = "通过";
                                        blnRet[i, 2] = true;
                                }
                                else
                                {
                                    ResultDictionary["继电器状态位3"][i] = "拉闸";
                                    ResultDictionary["合闸复电3"][i] = "不通过";
                                }
                            }
                            else
                            {
                                ResultDictionary["继电器状态位3"][i] = "异常";
                                ResultDictionary["合闸复电3"][i] = "不通过";
                            }
                            ResultDictionary["合闸次数前一后3"][i] = iHeZhaCount3Q[i].ToString() + "-" + iHeZhaCount3H[i];
                        }
                    }
                    UploadTestResult("合闸次数前一后3");
                    UploadTestResult("继电器状态位3");
                    UploadTestResult("合闸复电3");



                    #endregion

                    #region 步骤4
                    //4-----------------


                    if (Stop) return;
                    int[] iLaZhaCount4Q = ReadMeterLaZhaCount();

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                    result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);


                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发保电命令,请稍候....");
                    Common.Memset(ref  strData, "3A00" + System.DateTime.Now.AddDays(5).ToString("yyMMddHHmmss"));
                    result = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
                    string[] strRedMeterKhID = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行远程充值,请稍候....");
                    //购电金额+购电次数+客户编号

                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        BuyCount[i] = "00000002";
                        BuyMoney[i] = "00001388";
                        strData[i] = BuyMoney[i] + BuyCount[i] + strRedMeterKhID[i];
                    }
                    result = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

                    MessageController.Instance.AddMessage("正在走字,请稍候....");
                    if (GlobalUnit.IsDan)
                    {
                        blnSetRet = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib * 4, 1, 1, "1.0", true, false);

                        iWalkTime = Convert.ToInt32((20000 * 3600) / (GlobalUnit.U * GlobalUnit.Ib * 4) / 100);
                    }
                    else
                    {
                        blnSetRet = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib * 5, 1, 1, "1.0", true, false);

                        iWalkTime = Convert.ToInt32((20000 * 3600) / (GlobalUnit.U * GlobalUnit.Ib * 5 * 3) / 100);
                    }

                    blnIsBreak = false;
                    while (!blnIsBreak)
                    {
                        if (Stop) return;
                        bool bValue = ShowWaitTimeMessage("正在等待{0}秒,请稍候....", 1000 * iWalkTime);
                        if (bValue)
                            break;
                    }
                    PowerOn();
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取剩余金额,请稍候....");
                    Common.Memset(ref strRevCode, "DF01000200000004");
                    result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out BuyCount, out strSyMoney);
                    if (Stop) return;
                    strSyMoney = MeterProtocolAdapter.Instance.ReadData("00900200", 4);
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额4", Common.StringConverToDecima(strSyMoney));
                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取报警金额2,请稍候....");
                    strBjMoney2 = MeterProtocolAdapter.Instance.ReadData("04001002", 4);
                    MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "报警金额2（步骤4）", Common.StringConverToDecima(strBjMoney2));
                    if (Stop) return;
                    blnSetRet = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Ib, 1, 1, "1.0", true, false);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
                    if (Stop) return;
                    int[] iLaZhaCount4H = ReadMeterLaZhaCount();

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在读取状态字3,请稍候....");
                    status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
                    if (Stop) return;
                    PowerOn();


                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                        {

                            if (!string.IsNullOrEmpty(status3[i]))
                            {
                                if ((Convert.ToInt32(status3[i], 16) & 0x1000) == 0x1000)
                                {
                                    ResultDictionary["保电状态4"][i] = "保电";
                                }
                                else
                                {
                                    ResultDictionary["保电状态4"][i] = "非保电";
                                }
                                if ((Convert.ToInt32(status3[i], 16) & 0x0010) != 0x0010)
                                {
                                    ResultDictionary["继电器状态位4"][i] = "合闸";
                                    blnRet[i, 3] = true;
                                }
                                else
                                {
                                    ResultDictionary["继电器状态位4"][i] = "拉闸";

                                }
                            }
                            else
                            {
                                ResultDictionary["继电器状态位4"][i] = "异常";
                                ResultDictionary["保电状态4"][i] = "异常";
                            }
                            ResultDictionary["跳闸次数前一后4"][i] = iLaZhaCount4Q[i].ToString() + "-" + iLaZhaCount4H[i].ToString();
                            ResultDictionary["保电状态不跳闸4"][i] = blnRet[i, 3] ? "通过" : "不通过";
                        }
                    }
                    UploadTestResult("跳闸次数前一后4");
                    UploadTestResult("继电器状态位4");
                    UploadTestResult("保电状态4");
                    UploadTestResult("保电状态不跳闸4");

                    //保电状态4|剩余金额4|报警金额2(步骤4)|跳闸次数前-后|跳闸信号输出4|保电状态不跳闸4|
                    #endregion

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在身份认证,请稍候....");
                    result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行设置报警金额1,请稍候....");
                    Common.Memset(ref strID, "04001001");
                    Common.Memset(ref strData, "00002000");
                    Common.Memset(ref strPutApdu, "04D6811008");
                    result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在进行设置报警金额2,请稍候....");
                    Common.Memset(ref strID, "04001002");
                    Common.Memset(ref strData, ("00001000"));
                    Common.Memset(ref strPutApdu, "04D6811408");
                    result = MeterProtocolAdapter.Instance.SouthParameterUpdate(iFlag, strRand2, strPutApdu, strID, strData);

                    if (Stop) return;
                    MessageController.Instance.AddMessage("正在下发保电解除命令,请稍候....");
                    Common.Memset(ref strData, "3B00" + DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss"));
                    bool[] blnSetJcbdRet = MeterProtocolAdapter.Instance.SouthUserControl(iFlag, strRand2, strEsamNo, strData);
                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 15);

                    #region 处理结果
                    MessageController.Instance.AddMessage("正在处理结果");
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3])
                        {
                            ResultDictionary["结论"][i] = "合格";
                        }
                        else
                        {
                            ResultDictionary["结论"][i] = "不合格";
                        }
                    }
                    UploadTestResult("结论");
                    #endregion
                    #endregion
                }


        }

        /// <summary>
        /// 读取拉闸次数
        /// </summary>
        /// <returns></returns>
        private int[] ReadMeterLaZhaCount()
        {
            //if (Stop) return;
            MessageController.Instance.AddMessage("正在读取拉闸次数....");
            string[] strlaZhaCount = MeterProtocolAdapter.Instance.ReadData("1D000001", 3);
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

        /// <summary>
        /// 根据参数计算报警金额
        /// </summary>
        /// <param name="strData"></param>
        /// <param name="iAddMoney"></param>
        /// <returns></returns>
        public static string[] calMoney(string[] strData, int iReduceMoney)
        {
            string[] strDecimalism = new string[strData.Length];
            for (int i = 0; i < strData.Length; i++)
            {
                if (!string.IsNullOrEmpty(strData[i]))
                {
                    Decimal dMoney = Math.Round((Convert.ToDecimal(strData[i]) / 100), 2) - iReduceMoney;
                    if (dMoney.ToString().Contains("."))
                    {
                        string[] moneyArr = dMoney.ToString().Split('.');
                        strDecimalism[i] = moneyArr[0].PadLeft(6, '0') + moneyArr[1].PadRight(2, '0');
                    }
                    else
                    {
                        strDecimalism[i] = dMoney.ToString().PadLeft(6, '0') + "00";
                    }
                }
                else
                {
                    strDecimalism[i] = "";
                }
            }
            return strDecimalism;
        }

    }
}
