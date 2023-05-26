
using System;
using CLDC_DataCore;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;

namespace CLDC_VerifyAdapter.Multi
{
    /// <summary>
    /// 通讯测试类
    /// </summary>
    class Dgn_ReadPara : DgnBase
    {
        public Dgn_ReadPara(object plan)
            : base(plan)
        { }


        /// <summary>
        /// 通讯测试
        /// </summary>
        public override void Verify()
        {
            string strItemName, strMessage;
            strItemName = "";
            //更新一下电能表数据
            Helper.MeterDataHelper.Instance.Init();
            //更新多功能协议
            Adapter.Instance.UpdateMeterProtocol();
            bool bPowerOn = PowerOn();
            Check.Require(bPowerOn, "控制源输出失败");
            MessageController.Instance.AddMessage("正在进行" + CurPlan.ToString());

            string[] arrStrResultKey = new string[BwCount];
            string[] strReadData = null;
            CLDC_Comm.Enum.Cus_ReadParaType readType;
            for (int i = 1; i <= 1; i++)
            {
                readType = (Cus_ReadParaType)i;
                switch (readType)
                {
                    case Cus_ReadParaType.通信地址:
                        strItemName = "通讯地址";                        
                        MessageController.Instance.AddMessage("正在读取【地址】");
                        strReadData = MeterProtocolAdapter.Instance.ReadAddress();                          
                        if(!CheckReadPara(strReadData))
                            strReadData = MeterProtocolAdapter.Instance.ReadAddress();   
                        break;
                    //case Cus_ReadParaType.资产编号:
                    //    strReadData = MeterProtocolAdapter.Instance.ReadData("04000403", 32);
                    //    if (!CheckReadPara(strReadData))
                    //        strReadData = MeterProtocolAdapter.Instance.ReadData("04000403", 32);
                    //    break;
                    case Cus_ReadParaType.有功常数:
                        strItemName = "有功常数";
                        MessageController.Instance.AddMessage("正在读取【有功常数】");
                        strReadData = MeterProtocolAdapter.Instance.ReadData("04000409", 3);
                        if (!CheckReadPara(strReadData))
                            strReadData = MeterProtocolAdapter.Instance.ReadData("04000409", 3);
                        break;                    
                    case Cus_ReadParaType.有功等级:
                        strItemName = "有功等级";
                        MessageController.Instance.AddMessage("正在读取【有功等级】");
                        strReadData = MeterProtocolAdapter.Instance.ReadData("04000407", 4);
                        if (!CheckReadPara(strReadData))
                            strReadData = MeterProtocolAdapter.Instance.ReadData("04000407", 4);
                        break;
                    case Cus_ReadParaType.电表型号:
                        strItemName = "电表型号";
                        MessageController.Instance.AddMessage("正在读取【电表型号】");
                        strReadData = MeterProtocolAdapter.Instance.ReadData("0400040B", 10);
                        if (!CheckReadPara(strReadData))
                            strReadData = MeterProtocolAdapter.Instance.ReadData("0400040B", 10);
                        break;
                    case Cus_ReadParaType.生产日期:
                        strItemName = "生产日期";
                        MessageController.Instance.AddMessage("正在读取【生产日期】");
                        strReadData = MeterProtocolAdapter.Instance.ReadData("0400040C", 10);
                        if (!CheckReadPara(strReadData))
                            strReadData = MeterProtocolAdapter.Instance.ReadData("0400040C", 10);
                        break;
                }
                SetMeterInfo(readType, strReadData);
                if (Stop)
                    return;
                strMessage = string.Format("读取{0}完毕", strItemName);
                MessageController.Instance.AddMessage(strMessage);
                for (int j = 0; j < BwCount; j++)
                {
                        arrStrResultKey[j] = ItemKey;
                    
                }
            }
            GlobalUnit.ReadingPara = false;
            GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrStrResultKey);
        }
        private bool CheckReadPara(string[] strData)
        {
            bool bResult = true;
            for (int i = 0; i < BwCount; i++)
            {
                if (strData[i] == "" || strData[i] == null)
                {
                    bResult = false;
                    break;
                }
            }
            return bResult;
        }
        private void SetMeterInfo(CLDC_Comm.Enum.Cus_ReadParaType readType, string[] strData)
        {
            bool bResult = true;
            string strMessageText = "";
            for (int i = 0; i < BwCount; i++)
            {                
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                
                switch (readType)
                {
                    case Cus_ReadParaType.通信地址:
                        curMeter.Mb_chrAddr = strData[i];

                        if (curMeter.Mb_chrAddr.Length < 1)
                        {
                            bResult = false;
                            strMessageText += (i + 1).ToString() + "号,";
                        }
                        //curMeter.Mb_ChrJlbh = strData[i];
                        //curMeter.Mb_ChrCcbh = strData[i];
                        //if (curMeter.Mb_ChrCcbh !=null)
                        //    if (curMeter.Mb_ChrCcbh.Length > 10)
                        //        curMeter.Mb_ChrCcbh = curMeter.Mb_ChrCcbh.Substring(curMeter.Mb_ChrCcbh.Length - 10, 10);
                        break;
                    //case Cus_ReadParaType.资产编号:
                    //    if (strData[i].Length != 0)
                    //    {
                    //        strTmp = GetASCII(strData[i]);
                    //        if (strTmp.Length > 22)
                    //            curMeter.Mb_ChrTxm = strTmp.Substring(strTmp.Length - 22, 22);
                    //        else
                    //            curMeter.Mb_ChrTxm = "";
                    //    }
                    //    else
                    //        curMeter.Mb_ChrTxm = "";
                    //    break;
                    case Cus_ReadParaType.有功常数:
                        if (strData[i] != null)
                        {
                            if (strData[i].Length != 0)
                                curMeter.Mb_chrBcs = Convert.ToString(Convert.ToInt32(strData[i]));
                            else
                                curMeter.Mb_chrBcs = "";
                        }
                        break;
                    //case Cus_ReadParaType.无功常数:
                    //    if (strData[i].Length != 0)
                    //        curMeter.Mb_chrBcs += "(" + Convert.ToString(Convert.ToInt32(strData[i])) + ")";
                    //    else
                    //        curMeter.Mb_chrBcs = "";
                    //    break;
                    case Cus_ReadParaType.有功等级:
                        if (strData[i].Length != 0)
                            curMeter.Mb_chrBdj = GetASCII(strData[i]);
                        else
                            curMeter.Mb_chrBdj = "";
                        break;
                    //case Cus_ReadParaType.无功等级:
                    //    if (strData[i].Length != 0)
                    //        curMeter.Mb_chrBdj += "(" + GetASCII(strData[i]) + ")";
                    //    else
                    //        curMeter.Mb_chrBdj = "";
                    //    break;
                    case Cus_ReadParaType.电表型号:
                        if (strData[i].Length != 0)
                            curMeter.Mb_Bxh = GetASCII(strData[i]);
                        else
                            curMeter.Mb_Bxh = "";
                        break;
                    case Cus_ReadParaType.生产日期:
                        if (strData[i].Length != 0)
                            curMeter.Mb_chrCcrq = GetASCII(strData[i]);
                        break;
                }
            }
            if (readType == Cus_ReadParaType.通信地址)
            {
                Adapter.Instance.UpdateMeterProtocol();
                if (!bResult)
                {
                    strMessageText = strMessageText.Trim(',');
                    strMessageText += "表位探测表地址失败，请检查";
                    MessageController.Instance.AddMessage(strMessageText);
                    //if (GlobalUnit.NetState == CLDC_Comm.Enum.Cus_NetState.DisConnected)
                    MessageController.Instance.AddMessage(strMessageText);
                    Stop = true;
                }
            }
        }
        private string GetASCII(string strData)
        {            
            string str;
            string strASCII = "";
            int intAsc = 0;
            for (int i = 0; i < strData.Length / 2; i++)
            {
                str = strData.Substring(i * 2, 2);
                intAsc = Convert.ToInt32(str, 16);
                if (intAsc == 0) break;
                char c = (char)intAsc;
                strASCII += c;
            }
            return strASCII;
        }
    }
}
/*===========================================================================================================*/