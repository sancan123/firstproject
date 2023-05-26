using System;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_SafeFileProtocol.Protocols;
using System.Windows.Forms;
using CLDC_DataCore.Function;
using CLDC_SafeFileProtocol;

namespace CLDC_VerifyAdapter.CostSouth.LocalMode
{
    /// <summary>
    /// 用户卡补卡
    /// </summary>
    public class UserCardRepairCard : VerifyBase
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

        public UserCardRepairCard(object plan)
            : base(plan)
        {

        }

            //本地开户状态位1|未开户不可补卡1|本地开户状态位2|卡1开户并购电2|
            //卡2客户编号3|卡2表号3|表内客户编号3|表内表号3|表号不一致不可补卡3|
            //卡2客户编号4|卡2表号4|表内客户编号4|表内表号4|客户编号不一致不可补卡4|
            //卡2购电次数5|表内购电次数5|卡购电次数比表内大于等于2不可补卡5|
        //囤积金额限值6|剩余金额6|充值100超囤积不可补卡6|
            //卡2购电次数7|表内购电次数7|卡2购电金额7|剩余金额补卡前-补卡后7|购电次数相等可补卡不可购电7|
            //购电次数购电前-购电后8|剩余金额购电前-购电后8|卡2购电成功8|
            //异常插卡总次数前-后9|卡1失效9|
            //卡3购电次数10|卡3购电金额10|购电次数补卡前-补卡后10|剩余金额补卡前-补卡后10|卡3补卡并购电10|
            //异常插卡总次数前-后11|卡2失效11|
            //卡2购电次数12|卡2购电金额12|表内购电次数12|剩余金额补卡前-补卡后12|卡购电次数比表内小可补卡不充值12|

        protected override bool CheckPara()
        {

            ResultNames = new string[] { "本地开户状态位1","未开户不可补卡1","本地开户状态位2","卡1开户并购电2",
                                        "卡2客户编号3","卡2表号3","表内客户编号3","表内表号3","表号不一致不可补卡3",
                                        "卡2客户编号4","卡2表号4","表内客户编号4","表内表号4","客户编号不一致不可补卡4",
                                        "卡2购电次数5","表内购电次数5","卡购电次数比表内大于等于2不可补卡5",
                                        "囤积金额限值6","剩余金额6","充值100超囤积不可补卡6",
                                        "卡2购电次数7","表内购电次数7","卡2购电金额7","剩余金额补卡前一补卡后7","购电次数相等可补卡不可购电7",
                                        "购电次数购电前一购电后8","剩余金额购电前一购电后8","卡2购电成功8",
                                        "异常插卡总次数前一后9","卡1失效9",
                                        "卡3购电次数10","卡3购电金额10","购电次数补卡前一补卡后10","剩余金额补卡前一补卡后10","卡3补卡并购电10",
                                        "异常插卡总次数前一后11","卡2失效11",
                                        "卡2购电次数12","卡2购电金额12","表内购电次数12","剩余金额补卡前一补卡后12","卡购电次数比表内小可补卡不充值12",
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
            string[] strRevData = new string[BwCount];
            string[] strOutMac1 = new string[BwCount];
            string[] strOutMac2 = new string[BwCount];
            string[] strRevCode = new string[BwCount];
            string[] strApdu = new string[BwCount];
            string[] strSyMoneyQ = new string[BwCount]; //剩余金额
            string[] strGdCountQ = new string[BwCount]; //购电次数
            string[] strSyMoney = new string[BwCount]; //当前剩余金额
            string[] strGdMoney = new string[BwCount];
            string[] strKhID = new string[BwCount]; //当前客户编号
            string[] GdkKhID = new string[BwCount];
            string[] strGdCount = new string[BwCount]; //购电次数
            string[] GdkGdCount = new string[BwCount];
            string[] GdkMeterNo = new string[BwCount];
            string[] strMeterNo = new string[BwCount];
            string[] paraFile = new string[12]; //参数信息文件
            string[] walletFile = new string[2];//钱包文件
            string[] priceFile1 = new string[51];//当前套电价文件
            string[] priceFile2 = new string[51];//备用套电价文件
            string[] ControlFilePlain = new string[1]; //合闸明文
            string[] strData = new string[BwCount];
            string strParaFile = ""; //参数信息文件
            string strwalletFile = "";//钱包文件
            string strpriceFile1 = "";//当前套电价文件
            string strpriceFile2 = "";//备用套电价文件
            string strControlFilePlain = ""; //合闸明文
            string[] strParaFileArr = new string[BwCount];  //参数信息文件
            string[] strwalletFileArr = new string[BwCount];//钱包文件
            string[] strpriceFile1Arr = new string[BwCount];//当前套电价文件
            string[] strpriceFile2Arr = new string[BwCount];//备用套电价文件
            string[] strfileReplyArr = new string[BwCount];
            string[] strControlFilePlainArr = new string[BwCount];  //合闸明文
            string[] strBuyMoney = new string[BwCount];
            bool[] result = new bool[BwCount];
            string[] MyStatus = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 12];
            int[] iFlag = new int[BwCount];
            bool[] BlnIniRet = new bool[BwCount];
            string[] strErrCountQ = new string[BwCount];
            string[] strErrCountH = new string[BwCount];
            string[] status = new string[BwCount];
            string[] status3 = new string[BwCount];
            bool[] rstTmp = new bool[BwCount];
            string[] FkStatus = new string[BwCount];
            string[] DataTmp = new string[BwCount];
            string[] BuyMoney = new string[BwCount];
            string[] BuyCount = new string[BwCount];
            string[] outData = new string[BwCount];
            int iSelectBwCount = 0;
            string[] strErrInfo = new string[BwCount];


            //Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);

            #region 准备步骤
            //准备
            //准备工作
            ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发送钱包初始化命令,请稍候....");
            Common.Memset(ref strData, "00002710");
            result = MeterProtocolAdapter.Instance.SouthInitPurse(iFlag, strRand2, strData);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在密钥密钥更新,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthKeyUpdateV2("01", 17, strRand2, strEsamNo);
            Common.Memset(ref iFlag, 1);

            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            string strDataTmp = "0400010C" + DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
            strDataTmp += DateTime.Now.ToString("HHmmss");
            Common.Memset(ref strRevCode, "0400010C");
            Common.Memset(ref strData, strDataTmp);
            bool[] blnSetDateRet = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            if (Stop) return;
            Common.Memset(ref strRevCode, "04001004");
            Common.Memset(ref strData, "04001004" + "00000000");
            MessageController.Instance.AddMessage("正在设置囤积金额限值,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            #endregion

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCount, out strSyMoneyQ);


            //1------------------------------------
            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            if (Stop) return;
            System.Threading.Thread.Sleep(500);

            MessageController.Instance.AddMessage("用户卡1对表计补卡操作,请稍候....");

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;
                paraFile[0] = "00";            //保留
                paraFile[1] = "8F";            //参数更新标志位
                paraFile[2] = "00000000";      //保留
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                paraFile[4] = "00";            //保留
                paraFile[5] = "00005000";      //报警金额2
                paraFile[6] = "00004000";      //报警金额1
                paraFile[7] = "000001";        //电流互感器变比
                paraFile[8] = "000001";        //电压互感器变比
                paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');  //表号
                paraFile[10] = "112233445566"; //客户编号
                paraFile[11] = "03";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                walletFile[0] = "00000000";    //购电金额
                walletFile[1] = "00000000";    //购电次数

                for (int j = 0; j < 12; j++)   //费率1-12
                {
                    priceFile1[j] = "00010000";
                    priceFile2[j] = "00010000";
                }
                for (int j = 12; j < 18; j++)  //第1阶梯表阶梯值1-6
                {
                    priceFile1[j] = "00000100";
                    priceFile2[j] = "00000100";
                }
                for (int j = 18; j < 25; j++)  //第1阶梯表阶梯电价1-7
                {
                    priceFile1[j] = "00010000";
                    priceFile2[j] = "00010000";
                }
                for (int j = 25; j < 31; j++)  //年第1-6结算日
                {
                    priceFile1[j] = "010101";
                    priceFile2[j] = "010101";
                }
                for (int j = 31; j < 37; j++)  //第2阶梯表阶梯值1-6
                {
                    priceFile1[j] = "00000100";
                    priceFile2[j] = "00000100";
                }
                for (int j = 37; j < 44; j++)  //第2阶梯表阶梯电价1-7
                {
                    priceFile1[j] = "00010000";
                    priceFile2[j] = "00010000";
                }
                for (int j = 44; j < 50; j++) //年第1-6结算日
                {
                    priceFile1[j] = "010101";
                    priceFile2[j] = "010101";
                }
                priceFile1[50] = "0000000000"; //保留
                priceFile2[50] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");  //两套阶梯切换时间

                ControlFilePlain[0] = "1C00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");

                ISafeFileProtocol isafe = new SouthSafeFile();
                int iresult = isafe.GetUserCardFileParam(paraFile, out strParaFile);
                iresult = isafe.GetUserCardFileMoney(walletFile, out strwalletFile);
                iresult = isafe.GetUserCardFilePrice1(priceFile1, out strpriceFile1);
                iresult = isafe.GetUserCardFilePrice2(priceFile2, out strpriceFile2);
                iresult = isafe.GetUserCardFileControl(ControlFilePlain, out strControlFilePlain);

                strParaFileArr[i] = strParaFile;
                strwalletFileArr[i] = strwalletFile;
                strpriceFile1Arr[i] = strpriceFile1;
                strpriceFile2Arr[i] = strpriceFile2;
                strfileReplyArr[i] = "00".PadLeft(100, '0');
                strControlFilePlainArr[i] = strControlFilePlain;
            }
            if (Stop) return;
            MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
            bool[] WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteUserCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr, strfileReplyArr, strControlFilePlainArr);
            //MessageBox.Show("请把用户卡插入表后按确定");

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在开始寻卡,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthFindCard(0); ;

            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在结束寻卡,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthFindCard(1);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                    {
                        status3[i] = "未开户";
                    }
                    else
                    {
                        status3[i] = "开户";
                    }
                }
                else
                {
                    status3[i] = "异常";
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "本地开户状态位1", status3);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取剩余金额及购电次数....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCount, out strSyMoney);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
            status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status[i]))
                {
                    if (strSyMoneyQ[i] == strSyMoney[i] && (Convert.ToInt32(status[i], 16) & 0x0001) != 0x0001)
                    {
                        blnRet[i, 0] = true;
                    }
                }
                ResultDictionary["未开户不可补卡1"][i] = blnRet[i, 0] ? "通过" : "不通过";
            }
            UploadTestResult("未开户不可补卡1");

            //2-------------


            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("用户卡1对表计开户,请稍候....");

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');

                BuyMoney[i] = "00000000";
                walletFile[0] = BuyMoney[i];    //购电金额
                walletFile[1] = "00000000";    //购电次数
                paraFile[11] = "01";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                priceFile2[50] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");  //两套阶梯切换时间

                ControlFilePlain[0] = "1C00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");

                ISafeFileProtocol isafe = new SouthSafeFile();
                int iresult = isafe.GetUserCardFileParam(paraFile, out strParaFile);
                iresult = isafe.GetUserCardFileMoney(walletFile, out strwalletFile);
                iresult = isafe.GetUserCardFilePrice1(priceFile1, out strpriceFile1);
                iresult = isafe.GetUserCardFilePrice2(priceFile2, out strpriceFile2);
                iresult = isafe.GetUserCardFileControl(ControlFilePlain, out strControlFilePlain);

                strParaFileArr[i] = strParaFile;
                strwalletFileArr[i] = strwalletFile;
                strpriceFile1Arr[i] = strpriceFile1;
                strpriceFile2Arr[i] = strpriceFile2;
                strfileReplyArr[i] = "00".PadLeft(100, '0');
                strControlFilePlainArr[i] = strControlFilePlain;
            }
            if (Stop) return;
            MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
            WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteUserCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr, strfileReplyArr, strControlFilePlainArr);
            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);
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

            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("用户卡1对表计充值100元,请稍候....");

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;

                BuyMoney[i] = "00002710";
                walletFile[0] = BuyMoney[i];    //购电金额
                walletFile[1] = "00000001";    //购电次数
                paraFile[11] = "02";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');

                ISafeFileProtocol isafe = new SouthSafeFile();
                int iresult = isafe.GetUserCardFileParam(paraFile, out strParaFile);
                iresult = isafe.GetUserCardFileMoney(walletFile, out strwalletFile);
                iresult = isafe.GetUserCardFilePrice1(priceFile1, out strpriceFile1);
                iresult = isafe.GetUserCardFilePrice2(priceFile2, out strpriceFile2);
                iresult = isafe.GetUserCardFileControl(ControlFilePlain, out strControlFilePlain);

                strParaFileArr[i] = strParaFile;
                strwalletFileArr[i] = strwalletFile;
                strpriceFile1Arr[i] = strpriceFile1;
                strpriceFile2Arr[i] = strpriceFile2;
                strfileReplyArr[i] = "00".PadLeft(100, '0');
                strControlFilePlainArr[i] = strControlFilePlain;
            }
            if (Stop) return;
            MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
            WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteUserCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr, strfileReplyArr, strControlFilePlainArr);


            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);
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

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
            string[] strRedMeterKhID = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);

            //远程充值第一次
            for (int i = 0; i < BwCount; i++)
            {
                BuyCount[i] = "00000002";
                BuyMoney[i] = "00002710";
                strData[i] = BuyMoney[i] + BuyCount[i] + strRedMeterKhID[i];
            }
            result = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);
            //远程充值第二次
            for (int i = 0; i < BwCount; i++)
            {
                BuyCount[i] = "00000003";
                BuyMoney[i] = "00002710";
                strData[i] = BuyMoney[i] + BuyCount[i] + strRedMeterKhID[i];
            }
            result = MeterProtocolAdapter.Instance.SouthIncreasePurse(1, iFlag, strRand2, strData, out strErrInfo);


            if (Stop) return;
            MessageController.Instance.AddMessage("正在回抄表内剩余金额,请稍候....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCount, out strSyMoney);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表运行状态字3,请稍候....");
            status3 = MeterProtocolAdapter.Instance.ReadData("04000503", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status3[i]))
                {
                    if ((Convert.ToInt32(status3[i], 16) & 0x4000) == 0x4000)
                    {
                        status3[i] = "未开户";
                    }
                    else
                    {
                        status3[i] = "开户";
                    }
                }
                else
                {
                    status3[i] = "异常";
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "本地开户状态位2", status3);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
            status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn ) continue;
                if (!string.IsNullOrEmpty(status[i]))
                {
                    if (strSyMoney[i] == "00009C40" && (Convert.ToInt32(status[i], 16) & 0x0001) == 0x0001)
                    {
                        blnRet[i, 1] = true;
                    }
                }
                ResultDictionary["卡1开户并购电2"][i] = blnRet[i, 1] ? "成功" : "失败";
            }
            UploadTestResult("卡1开户并购电2");

            MessageBox.Show("下一试验流程需要更换用户卡,请把用户卡2插入表后按确定");


            //3---------------------

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("用户卡2对表计补卡，卡内表号与表内不一致、卡内客户编号与表内一致,请稍候....");

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间

                GdkMeterNo[i] = "000000112233";
                GdkKhID[i] = strKhID[i];
                paraFile[9] = GdkMeterNo[i];
                GdkKhID[i] = "112233445566";
                paraFile[10] = GdkKhID[i];
                paraFile[11] = "03";

                BuyMoney[i] = "00002710";
                walletFile[0] = BuyMoney[i];    //购电金额

                priceFile2[50] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");  //两套阶梯切换时间

                ControlFilePlain[0] = "1C00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");


                ISafeFileProtocol isafe = new SouthSafeFile();
                int iresult = isafe.GetUserCardFileParam(paraFile, out strParaFile);
                iresult = isafe.GetUserCardFileMoney(walletFile, out strwalletFile);
                iresult = isafe.GetUserCardFilePrice1(priceFile1, out strpriceFile1);
                iresult = isafe.GetUserCardFilePrice2(priceFile2, out strpriceFile2);
                iresult = isafe.GetUserCardFileControl(ControlFilePlain, out strControlFilePlain);

                strParaFileArr[i] = strParaFile;
                strwalletFileArr[i] = strwalletFile;
                strpriceFile1Arr[i] = strpriceFile1;
                strpriceFile2Arr[i] = strpriceFile2;
                strfileReplyArr[i] = "00".PadLeft(100, '0');
                strControlFilePlainArr[i] = strControlFilePlain;
            }
            if (Stop) return;
            MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
            WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteUserCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr, strfileReplyArr, strControlFilePlainArr);
            //MessageBox.Show("请把用户卡插入表后按确定");

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);    
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);
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

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡2客户编号3", GdkKhID);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡2表号3", GdkMeterNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
            strRedMeterKhID = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内客户编号3", strRedMeterKhID);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表表号,请稍候....");
            strMeterNo = MeterProtocolAdapter.Instance.ReadData("04000402", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内表号3", strMeterNo);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
            status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status[i]))
                {
                    if (strSyMoney[i] == "00009C40" && (Convert.ToInt32(status[i], 16) & 0x0001) != 0x0001)
                    {
                        blnRet[i, 2] = true;
                    }
                }
                ResultDictionary["表号不一致不可补卡3"][i] = blnRet[i, 2] ? "通过" : "不通过";
            }
            UploadTestResult("表号不一致不可补卡3");

            //4-----------------------------------

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("用户卡2对表计补卡，卡内表号与表内一致、卡内客户编号与表内不一致,请稍候....");

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间

                GdkMeterNo[i] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                GdkKhID[i] = "000000112233";
                paraFile[9] = GdkMeterNo[i];
                paraFile[10] = GdkKhID[i];

                BuyMoney[i] = "00002710";
                walletFile[0] = BuyMoney[i];    //购电金额

                priceFile2[50] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");  //两套阶梯切换时间

                ControlFilePlain[0] = "1C00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");


                ISafeFileProtocol isafe = new SouthSafeFile();
                int iresult = isafe.GetUserCardFileParam(paraFile, out strParaFile);
                iresult = isafe.GetUserCardFileMoney(walletFile, out strwalletFile);
                iresult = isafe.GetUserCardFilePrice1(priceFile1, out strpriceFile1);
                iresult = isafe.GetUserCardFilePrice2(priceFile2, out strpriceFile2);
                iresult = isafe.GetUserCardFileControl(ControlFilePlain, out strControlFilePlain);

                strParaFileArr[i] = strParaFile;
                strwalletFileArr[i] = strwalletFile;
                strpriceFile1Arr[i] = strpriceFile1;
                strpriceFile2Arr[i] = strpriceFile2;
                strfileReplyArr[i] = "00".PadLeft(100, '0');
                strControlFilePlainArr[i] = strControlFilePlain;
            }
            if (Stop) return;
            MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
            WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteUserCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr, strfileReplyArr, strControlFilePlainArr);
            //MessageBox.Show("请把用户卡插入表后按确定");

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

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

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡2客户编号4", GdkKhID);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡2表号4", GdkMeterNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表客户编号,请稍候....");
            strRedMeterKhID = MeterProtocolAdapter.Instance.ReadData("0400040E", 6);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内客户编号4", strRedMeterKhID);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取电能表表号,请稍候....");
            strMeterNo = MeterProtocolAdapter.Instance.ReadData("04000402", 6);
           
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内表号4", strMeterNo);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
            status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status[i]))
                {
                    if (strSyMoney[i] == "00009C40" && (Convert.ToInt32(status[i], 16) & 0x0001) != 0x0001)
                    {
                        blnRet[i, 3] = true;
                    }
                }
                ResultDictionary["客户编号不一致不可补卡4"][i] = blnRet[i, 3] ? "通过" : "不通过";
            }
            UploadTestResult("客户编号不一致不可补卡4");


         

            //5------------------------------------

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("用户卡2对表计补卡，卡内购电次数=表内购电次数+2，购电金额100,请稍候....");



            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间

                GdkMeterNo[i] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                GdkKhID[i] = "112233445566";
                paraFile[9] = GdkMeterNo[i];
                paraFile[10] = GdkKhID[i];

                BuyMoney[i] = "00002710";
                walletFile[0] = BuyMoney[i];    //购电金额
                BuyCount[i] = "00000005";
                walletFile[1] = BuyCount[i];   //购电次数

                priceFile2[50] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");  //两套阶梯切换时间

                ControlFilePlain[0] = "1C00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");


                ISafeFileProtocol isafe = new SouthSafeFile();
                int iresult = isafe.GetUserCardFileParam(paraFile, out strParaFile);
                iresult = isafe.GetUserCardFileMoney(walletFile, out strwalletFile);
                iresult = isafe.GetUserCardFilePrice1(priceFile1, out strpriceFile1);
                iresult = isafe.GetUserCardFilePrice2(priceFile2, out strpriceFile2);
                iresult = isafe.GetUserCardFileControl(ControlFilePlain, out strControlFilePlain);

                strParaFileArr[i] = strParaFile;
                strwalletFileArr[i] = strwalletFile;
                strpriceFile1Arr[i] = strpriceFile1;
                strpriceFile2Arr[i] = strpriceFile2;
                strfileReplyArr[i] = "00".PadLeft(100, '0');
                strControlFilePlainArr[i] = strControlFilePlain;
            }

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
            WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteUserCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr, strfileReplyArr, strControlFilePlainArr);
            //MessageBox.Show("请把用户卡插入表后按确定");

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

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

            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountQ, out strSyMoney);

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡2购电次数5",Common.StringConverToIntger( BuyCount));
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内购电次数5", Common.StringConverToIntger(strGdCountQ));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
            status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn ) continue;
                if (!string.IsNullOrEmpty(status[i]))
                {
                    if (strSyMoney[i] == "00009C40" && (Convert.ToInt32(status[i], 16) & 0x0001) != 0x0001)
                    {
                        blnRet[i, 4] = true;
                    }
                }
                ResultDictionary["卡购电次数比表内大于等于2不可补卡5"][i] = blnRet[i, 4] ? "通过" : "不通过";
            }

            UploadTestResult("卡购电次数比表内大于等于2不可补卡5");



            //6----------------------------------------------


            if (Stop) return;
            Common.Memset(ref strRevCode, "04001004");
            Common.Memset(ref strData, "04001004" + "00049900");
            MessageController.Instance.AddMessage("正在设置囤积金额限值,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("设囤积金额限值499，用户卡2通过交互终端对表计补卡，卡内购电次数=表内购电次数+1，购电金额100,请稍候....");



            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间

                GdkMeterNo[i] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                GdkKhID[i] = "112233445566";
                paraFile[9] = GdkMeterNo[i];
                paraFile[10] = GdkKhID[i];

                BuyMoney[i] = "00002710";
                walletFile[0] = BuyMoney[i];    //购电金额
                BuyCount[i] = "00000004";
                walletFile[1] = BuyCount[i];   //购电次数

                priceFile2[50] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");  //两套阶梯切换时间

                ControlFilePlain[0] = "1C00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");


                ISafeFileProtocol isafe = new SouthSafeFile();
                int iresult = isafe.GetUserCardFileParam(paraFile, out strParaFile);
                iresult = isafe.GetUserCardFileMoney(walletFile, out strwalletFile);
                iresult = isafe.GetUserCardFilePrice1(priceFile1, out strpriceFile1);
                iresult = isafe.GetUserCardFilePrice2(priceFile2, out strpriceFile2);
                iresult = isafe.GetUserCardFileControl(ControlFilePlain, out strControlFilePlain);

                strParaFileArr[i] = strParaFile;
                strwalletFileArr[i] = strwalletFile;
                strpriceFile1Arr[i] = strpriceFile1;
                strpriceFile2Arr[i] = strpriceFile2;
                strfileReplyArr[i] = "00".PadLeft(100, '0');
                strControlFilePlainArr[i] = strControlFilePlain;
            }

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
            WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteUserCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr, strfileReplyArr, strControlFilePlainArr);
            //MessageBox.Show("请把用户卡插入表后按确定");

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

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
           
            string[] strSyMoney6 = new string[BwCount];
            Common.Memset(ref strSyMoney6, "");
            MessageController.Instance.AddMessage("正在回抄表内金额购电次数,请稍候....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountQ, out strSyMoney6);


            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "剩余金额6", Common.HexConverToDecimalism(strSyMoney6));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取囤积金额限值,请稍候....");
            string[] strTjMoneyXz = MeterProtocolAdapter.Instance.ReadData("04001004", 4);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "囤积金额限值6", Common.StringConverToDecima(strTjMoneyXz));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
            status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn ) continue;
                if (!string.IsNullOrEmpty(status[i]))
                {
                    if (strSyMoney6[i] == "00009C40" && (Convert.ToInt32(status[i], 16) & 0x0001) != 0x0001)
                    {
                        blnRet[i, 6] = true;
                    }
                }
                ResultDictionary["充值100超囤积不可补卡6"][i] = blnRet[i, 6] ? "通过" : "不通过";
            }

            UploadTestResult("充值100超囤积不可补卡6");



            //--|||

            //7-----------------------------------

            if (Stop) return;
            Common.Memset(ref strRevCode, "04001004");
            Common.Memset(ref strData, "04001004" + "00000000");
            MessageController.Instance.AddMessage("正在设置囤积金额限值,请稍候....");
            result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strRevCode);

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("用户卡2对表计补卡，并充值100元，购电次数=表内购电次数,请稍候....");

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');

                GdkKhID[i] = "112233445566";
                paraFile[10] = GdkKhID[i];

                BuyMoney[i] = "00002710";
                BuyCount[i] = "00000003";
                walletFile[0] = BuyMoney[i];    //购电金额
                walletFile[1] = BuyCount[i];    //购电次数

                priceFile2[50] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");  //两套阶梯切换时间

                ControlFilePlain[0] = "1C00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");

                ISafeFileProtocol isafe = new SouthSafeFile();
                int iresult = isafe.GetUserCardFileParam(paraFile, out strParaFile);
                iresult = isafe.GetUserCardFileMoney(walletFile, out strwalletFile);
                iresult = isafe.GetUserCardFilePrice1(priceFile1, out strpriceFile1);
                iresult = isafe.GetUserCardFilePrice2(priceFile2, out strpriceFile2);
                iresult = isafe.GetUserCardFileControl(ControlFilePlain, out strControlFilePlain);

                strParaFileArr[i] = strParaFile;
                strwalletFileArr[i] = strwalletFile;
                strpriceFile1Arr[i] = strpriceFile1;
                strpriceFile2Arr[i] = strpriceFile2;
                strfileReplyArr[i] = "00".PadLeft(100, '0');
                strControlFilePlainArr[i] = strControlFilePlain;
            }
            if (Stop) return;
            MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
            WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteUserCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr, strfileReplyArr, strControlFilePlainArr);
            //MessageBox.Show("请把用户卡插入表后按确定");

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);
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


            if (Stop) return;
            MessageController.Instance.AddMessage("正在回抄表内金额购电次数,请稍候....");
            Common.Memset(ref strRevCode, "DF01000200000004");

            string []strSyMoney7 = new string[BwCount];

            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountQ, out strSyMoney7);

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡2购电次数7", Common.StringConverToIntger(BuyCount));
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内购电次数7",Common.StringConverToIntger( strGdCountQ));

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡2购电金额7", Common.HexConverToDecimalism(BuyMoney));

           

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
            status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status[i]))
                {
                    if (strSyMoney7[i] == "00009C40" && (Convert.ToInt32(status[i], 16) & 0x0001) == 0x0001)
                    {
                        blnRet[i, 7] = true;
                    }
                }
                ResultDictionary["购电次数相等可补卡不可购电7"][i] = blnRet[i, 7] ? "通过" : "不通过";
                
                strSyMoney7[i] = Common.HexConverToDecimalism(strSyMoney6[i]) + "-" + Common.HexConverToDecimalism(strSyMoney7[i]);
                ResultDictionary["剩余金额补卡前一补卡后7"][i] = strSyMoney7[i];
            }
            UploadTestResult("购电次数相等可补卡不可购电7");
            UploadTestResult("剩余金额补卡前一补卡后7");




            //8-----------------------------------

            if (Stop) return;
            MessageController.Instance.AddMessage("正在回抄表内金额购电次数,请稍候....");
            Common.Memset(ref  strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountQ, out strSyMoneyQ);

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("用户卡2对表计购电，充值购电金额100元，购电次数=表内购电次数+1的操作,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;

                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                paraFile[11] = "02";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                GdkGdCount[i] = "00000004";
                walletFile[0] = "00002710";    //购电金额
                walletFile[1] = GdkGdCount[i];    //购电次数

                priceFile2[50] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");  //两套阶梯切换时间

                ControlFilePlain[0] = "1C00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");


                ISafeFileProtocol isafe = new SouthSafeFile();
                int iresult = isafe.GetUserCardFileParam(paraFile, out strParaFile);
                iresult = isafe.GetUserCardFileMoney(walletFile, out strwalletFile);
                iresult = isafe.GetUserCardFilePrice1(priceFile1, out strpriceFile1);
                iresult = isafe.GetUserCardFilePrice2(priceFile2, out strpriceFile2);
                iresult = isafe.GetUserCardFileControl(ControlFilePlain, out strControlFilePlain);

                strParaFileArr[i] = strParaFile;
                strwalletFileArr[i] = strwalletFile;
                strpriceFile1Arr[i] = strpriceFile1;
                strpriceFile2Arr[i] = strpriceFile2;
                strfileReplyArr[i] = "00".PadLeft(100, '0');
                strControlFilePlainArr[i] = strControlFilePlain;
            }
            if (Stop) return;
            MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
            WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteUserCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr, strfileReplyArr, strControlFilePlainArr);
            //MessageBox.Show("请把用户卡插入表后按确定");

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

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


            if (Stop) return;
            MessageController.Instance.AddMessage("正在回抄表内金额,请稍候....");
            Common.Memset(ref strRevCode, "DF01000200000004");

            string []strGdCount8 = new string [BwCount];
            string [] strSyMoney8 = new string[BwCount];

            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCount8, out strSyMoney8);



            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
            status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status[i]))
                {
                    if (strSyMoney8[i] == "0000C350" && strGdCount8[i] == "00000004")
                    {
                        blnRet[i, 8] = true;
                    }
                }
                ResultDictionary["卡2购电成功8"][i] = blnRet[i, 8] ? "通过" : "不通过";
                ResultDictionary["购电次数购电前一购电后8"][i] = strGdCountQ[i] + "-" + strGdCount8[i];
                ResultDictionary["剩余金额购电前一购电后8"][i] = Common.HexConverToDecimalism(strSyMoneyQ[i]) + "-" + Common.HexConverToDecimalism(strSyMoney8[i]);
            
           }
            UploadTestResult("卡2购电成功8");
            UploadTestResult("购电次数购电前一购电后8");
            UploadTestResult("剩余金额购电前一购电后8");

            //--|||

            //9---------------------

            MessageBox.Show("下一试验流程需要更换用户卡,请把用户卡1插入表后按确定");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
            string[] strFfckCountQ = MeterProtocolAdapter.Instance.ReadData("03301300", 3);

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("用户卡1对表计充值100元,请稍候....");

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');

                BuyMoney[i] = "00002710";
                GdkGdCount[i] = "00000005";
                walletFile[0] = BuyMoney[i];    //购电金额
                walletFile[1] = GdkGdCount[i];    //购电次
                paraFile[11] = "02";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                priceFile2[50] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");  //两套阶梯切换时间

                ControlFilePlain[0] = "1C00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");


                ISafeFileProtocol isafe = new SouthSafeFile();
                int iresult = isafe.GetUserCardFileParam(paraFile, out strParaFile);
                iresult = isafe.GetUserCardFileMoney(walletFile, out strwalletFile);
                iresult = isafe.GetUserCardFilePrice1(priceFile1, out strpriceFile1);
                iresult = isafe.GetUserCardFilePrice2(priceFile2, out strpriceFile2);
                iresult = isafe.GetUserCardFileControl(ControlFilePlain, out strControlFilePlain);

                strParaFileArr[i] = strParaFile;
                strwalletFileArr[i] = strwalletFile;
                strpriceFile1Arr[i] = strpriceFile1;
                strpriceFile2Arr[i] = strpriceFile2;
                strfileReplyArr[i] = "00".PadLeft(100, '0');
                strControlFilePlainArr[i] = strControlFilePlain;
            }
            if (Stop) return;
            MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
            WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteUserCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr, strfileReplyArr, strControlFilePlainArr);
            //MessageBox.Show("请把用户卡插入表后按确定");

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

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

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
            string[] strFfckCountH = MeterProtocolAdapter.Instance.ReadData("03301300", 3);

            MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
            status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn )continue;
                if (!string.IsNullOrEmpty(strFfckCountQ[i]) && !string.IsNullOrEmpty(strFfckCountH[i]) && !string.IsNullOrEmpty(status[i]))
                {
                    if ((Convert.ToInt32(status[i], 16) & 0x0001) != 0x0001 && Convert.ToInt32(strFfckCountQ[i]) + 1 == Convert.ToInt32(strFfckCountH[i]))
                    {
                        blnRet[i, 9] = true;
                    }
                }

                ResultDictionary["卡1失效9"][i] = blnRet[i, 9] ? "通过" : "不通过";

                ResultDictionary["异常插卡总次数前一后9"][i] = strFfckCountQ[i] + "-" + strFfckCountH[i];
            }
            UploadTestResult("异常插卡总次数前一后9");
            UploadTestResult("卡1失效9");

            //--||


            //10----------------------

            //MessageBox.Show("下一试验流程需要更换用户卡,请把用户卡3插入表后按确定");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在回抄插卡后表内金额及购电次数,请稍候....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            string[] strGdCountQ11 = new string[BwCount];
            string[] strSyMoneyQ11 = new string[BwCount];
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountQ11, out strSyMoneyQ11);


            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("用户卡3对表计补卡，充值100元,购电次数等于表内购电次数+1的补卡操作,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;

                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                paraFile[11] = "03";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                GdkGdCount[i] = "00000005";
                BuyMoney[i] = "00002710";
                walletFile[0] = BuyMoney[i];    //购电金额
                walletFile[1] = GdkGdCount[i];    //购电次数

                priceFile2[50] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");  //两套阶梯切换时间

                ControlFilePlain[0] = "1C00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");


                ISafeFileProtocol isafe = new SouthSafeFile();
                int iresult = isafe.GetUserCardFileParam(paraFile, out strParaFile);
                iresult = isafe.GetUserCardFileMoney(walletFile, out strwalletFile);
                iresult = isafe.GetUserCardFilePrice1(priceFile1, out strpriceFile1);
                iresult = isafe.GetUserCardFilePrice2(priceFile2, out strpriceFile2);
                iresult = isafe.GetUserCardFileControl(ControlFilePlain, out strControlFilePlain);

                strParaFileArr[i] = strParaFile;
                strwalletFileArr[i] = strwalletFile;
                strpriceFile1Arr[i] = strpriceFile1;
                strpriceFile2Arr[i] = strpriceFile2;
                strfileReplyArr[i] = "00".PadLeft(100, '0');
                strControlFilePlainArr[i] = strControlFilePlain;
            }
            if (Stop) return;
            MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
            WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteUserCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr, strfileReplyArr, strControlFilePlainArr);
            //MessageBox.Show("请把用户卡插入表后按确定");

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

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

            MessageController.Instance.AddMessage("正在身份认证,请稍候....");
            MeterProtocolAdapter.Instance.SouthIdentity(iFlag, out strRand1, out strRand2, out strEsamNo);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在回抄插卡后表内金额及购电次数,请稍候....");
            Common.Memset(ref strRevCode, "DF01000200000004");
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCount, out strSyMoney);

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡3购电次数10", Common.StringConverToIntger(GdkGdCount));
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡3购电金额10", Common.HexConverToDecimalism(BuyMoney));


            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
            status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status[i]))
                {
                    if (strSyMoney[i] == "0000EA60" && strGdCount[i] == "00000005")
                    {
                        blnRet[i, 10] = true;
                    }
                }
                ResultDictionary["卡3补卡并购电10"][i] = blnRet[i, 10] ? "成功" : "失败";
                ResultDictionary["购电次数补卡前一补卡后10"][i] = strGdCountQ11[i] + "-" + strGdCount[i];
                ResultDictionary["剩余金额补卡前一补卡后10"][i] = Common.HexConverToDecimalism(strSyMoneyQ11[i]) + "-" + Common.HexConverToDecimalism(strSyMoney[i]);
            }
            UploadTestResult("卡3补卡并购电10");
            UploadTestResult("购电次数补卡前一补卡后10");
            UploadTestResult("剩余金额补卡前一补卡后10");

            //--||||



            //11-----------------
            MessageBox.Show("下一试验流程需要更换用户卡,请把用户卡2插入表后按确定");

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
            strFfckCountQ = MeterProtocolAdapter.Instance.ReadData("03301300", 3);

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("再次插用户卡2,充值100元,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;

                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间
                paraFile[9] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                paraFile[11] = "02";           //用户卡类型 01=开户卡 02=购电卡 03=补卡

                GdkGdCount[i] = "00000006";
                walletFile[0] = "00002710";    //购电金额
                walletFile[1] = GdkGdCount[i];    //购电次数

                priceFile2[50] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");  //两套阶梯切换时间

                ControlFilePlain[0] = "1C00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");


                ISafeFileProtocol isafe = new SouthSafeFile();
                int iresult = isafe.GetUserCardFileParam(paraFile, out strParaFile);
                iresult = isafe.GetUserCardFileMoney(walletFile, out strwalletFile);
                iresult = isafe.GetUserCardFilePrice1(priceFile1, out strpriceFile1);
                iresult = isafe.GetUserCardFilePrice2(priceFile2, out strpriceFile2);
                iresult = isafe.GetUserCardFileControl(ControlFilePlain, out strControlFilePlain);

                strParaFileArr[i] = strParaFile;
                strwalletFileArr[i] = strwalletFile;
                strpriceFile1Arr[i] = strpriceFile1;
                strpriceFile2Arr[i] = strpriceFile2;
                strfileReplyArr[i] = "00".PadLeft(100, '0');
                strControlFilePlainArr[i] = strControlFilePlain;
            }
            if (Stop) return;
            MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
            WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteUserCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr, strfileReplyArr, strControlFilePlainArr);
            //MessageBox.Show("请把用户卡插入表后按确定");

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

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

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取异常插卡总次数,请稍候....");
            strFfckCountH = MeterProtocolAdapter.Instance.ReadData("03301300", 3);

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
            status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status[i]))
                {
                    if ((Convert.ToInt32(status[i], 16) & 0x0001) != 0x0001)
                    {
                        blnRet[i, 11] = true;
                    }
                }
                ResultDictionary["卡2失效11"][i] = blnRet[i, 11] ? "通过" : "不通过";
                ResultDictionary["异常插卡总次数前一后11"][i] = strFfckCountQ[i] + "-" + strFfckCountH[i];
            }
            UploadTestResult("异常插卡总次数前一后11");
            UploadTestResult("卡2失效11");

            

            //12-----------------------------------

           // MessageBox.Show("下一试验流程需要更换用户卡,请把用户卡插入表后按确定");
            string[] strSyMoneyQ12 = new string[BwCount];
            //
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCount, out strSyMoneyQ12);

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(true);
            if (Stop) return;
            MessageController.Instance.AddMessage("正在复位卡片,请稍候....");
            BlnIniRet = MeterProtocolAdapter.Instance.SouthResetCard();
            MessageController.Instance.AddMessage("用户卡2对表计补卡，卡内购电次数=表内购电次数-1，购电金额100,请稍候....");



            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (Stop) return;
                paraFile[3] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");    //两套分时费率切换时间

                GdkMeterNo[i] = Helper.MeterDataHelper.Instance.Meter(i)._Mb_MeterNo.PadLeft(12, '0');
                GdkKhID[i] = "112233445566";
                paraFile[9] = GdkMeterNo[i];
                paraFile[10] = GdkKhID[i];
                paraFile[11] = "03";

                BuyMoney[i] = "00002710";
                walletFile[0] = BuyMoney[i];    //购电金额
                BuyCount[i] = "00000004";
                walletFile[1] = BuyCount[i];   //购电次数

                priceFile2[50] = DateTime.Now.AddMinutes(5).ToString("yyMMddHHmm");  //两套阶梯切换时间

                ControlFilePlain[0] = "1C00" + System.DateTime.Now.AddMinutes(5).ToString("yyMMddHHmmss");


                ISafeFileProtocol isafe = new SouthSafeFile();
                int iresult = isafe.GetUserCardFileParam(paraFile, out strParaFile);
                iresult = isafe.GetUserCardFileMoney(walletFile, out strwalletFile);
                iresult = isafe.GetUserCardFilePrice1(priceFile1, out strpriceFile1);
                iresult = isafe.GetUserCardFilePrice2(priceFile2, out strpriceFile2);
                iresult = isafe.GetUserCardFileControl(ControlFilePlain, out strControlFilePlain);

                strParaFileArr[i] = strParaFile;
                strwalletFileArr[i] = strwalletFile;
                strpriceFile1Arr[i] = strpriceFile1;
                strpriceFile2Arr[i] = strpriceFile2;
                strfileReplyArr[i] = "00".PadLeft(100, '0');
                strControlFilePlainArr[i] = strControlFilePlain;
            }

            if (Stop) return;
            MessageController.Instance.AddMessage("正在发行卡片,请稍候....");
            WriteUserResult = MeterProtocolAdapter.Instance.SouthWriteUserCard(strParaFileArr, strwalletFileArr, strpriceFile1Arr, strpriceFile2Arr, strfileReplyArr, strControlFilePlainArr);
            //MessageBox.Show("请把用户卡插入表后按确定");

            if (Stop) return;
            Helper.EquipHelper.Instance.RemoteControlOnOrOff(false);
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 2);

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

            //
            result = MeterProtocolAdapter.Instance.SouthMacCheck(iFlag, strRand2, strRevCode, out strOutMac1, out strOutMac2, out strGdCountQ, out strSyMoney);

            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡2购电次数12", Common.StringConverToIntger(BuyCount));
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "表内购电次数12", Common.StringConverToIntger(strGdCountQ));
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "卡2购电金额12", Common.HexConverToDecimalism(BuyMoney));

            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取插卡状态字,请稍候....");
            status = MeterProtocolAdapter.Instance.ReadData("04001502", 2);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (!string.IsNullOrEmpty(status[i]))
                {
                    if (strSyMoney[i] == "0000EA60")
                    {
                        blnRet[i, 5] = true;
                    }
                }
                ResultDictionary["剩余金额补卡前一补卡后12"][i] = Common.HexConverToDecimalism(strSyMoneyQ12[i]) + "-" + Common.HexConverToDecimalism(strSyMoney[i]);
                ResultDictionary["卡购电次数比表内小可补卡不充值12"][i] = blnRet[i, 5] ? "通过" : "不通过";
            }
            UploadTestResult("剩余金额补卡前一补卡后12");
            UploadTestResult("卡购电次数比表内小可补卡不充值12");

            //卡2购电次数12|表内购电次数12|剩余金额补卡前-补卡后12|卡购电次数比表内小可补卡不充值12|

            //处理结果
            MessageController.Instance.AddMessage("正在处理结论,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3] && blnRet[i, 4]
                        && blnRet[i, 5] && blnRet[i, 6] && blnRet[i, 7] && blnRet[i, 8] && blnRet[i, 9]
                        && blnRet[i, 10] && blnRet[i, 11])
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
    }
}
