using System;
using System.Collections.Generic;
using System.Text;
using CLDC_Comm.Enum;
using CLDC_DataCore.Const;
using System.Threading;
using CLDC_Comm;
using CLDC_DataCore.Struct;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Function;

namespace CLDC_VerifyAdapter.Multi
{
    /// <summary>
    /// 测量及监测试验
    /// </summary>
    class Dgn_MeasureAndMonitor : DgnBase
    {
        public Dgn_MeasureAndMonitor(object plan)
            : base(plan)
        {
            //根据技术条件Q/GDW 1354-2013 4.12条
            LoadScheme();
        }


        private Dictionary<int, string> testScheme = new Dictionary<int, string>();

        private void LoadScheme()
        {
            for (int i = 1; i < 13; i++)
            {
                switch (i)
                {
                    case 1:
                        testScheme.Add(i, "Un|120");
                        break;
                    case 2:
                        testScheme.Add(i, "Un|100");
                        break;
                    case 3:
                        testScheme.Add(i, "Un|60");
                        break;
                    case 4:
                        testScheme.Add(i, "Imax|120");
                        break;
                    case 5:
                        testScheme.Add(i, "Ib|100");
                        break;
                    case 6:
                        testScheme.Add(i, "Ib|5");
                        break;
                    case 7:
                        testScheme.Add(i, "P|120|120|1.0|Imax");
                        break;
                    case 8:
                        testScheme.Add(i, "P|100|100|1.0|Ib");
                        break;
                    case 9:
                        testScheme.Add(i, "P|100|0.4|1.0|Ib");
                        break;
                    case 10:
                        testScheme.Add(i, "C|0.5L");
                        break;
                    case 11:
                        testScheme.Add(i, "Hz|48");
                        break;
                    case 12:
                        testScheme.Add(i, "Hz|52");
                        break;
                    default:
                        break;

                }
            }
        }

        /// <summary>
        /// 检定业务主方法体
        /// </summary>
        public override void Verify()
        {
            base.Verify();


            string[] arrStrResultKey = new string[BwCount];
            string[] arrStrResultKeyTmp = new string[BwCount];

            for (int c = 0; c < BwCount; c++)
            {
                MeterBasicInfo curMeter = Helper.MeterDataHelper.Instance.Meter(c);
                for (int i = 1; i < 13; i++)
                {
                    string strKeyTmp = ItemKey + string.Format("{0:D2}", i);
                    if (curMeter.MeterDgns.ContainsKey(strKeyTmp))
                    {
                        curMeter.MeterDgns.Remove(strKeyTmp);
                    }
                }
                arrStrResultKey[c] = ItemKey;
            }
            GlobalUnit.g_MsgControl.OutMessage();

            for (int i = 1; i < 13; i++)
            {

                string strItemKeyId = ItemKey + string.Format("{0:D2}", i);
                //升源输出
                if (Stop) return;
                
                PowerOn(testScheme[i]);
                //等待源输出稳定
                if (Stop) return;
                GlobalUnit.g_MsgControl.OutMessage("等待功率源稳定....", false);
                Thread.Sleep((_DgnWaitTime+12) * 1000);
                //首先读取电表的值
                GlobalUnit.g_MsgControl.OutMessage("读取电表的参数值....", false);
                string[] strReadData;
                //switch(testScheme[i]
                if (Stop) return;
               bool bValue=  ReadMeterData(testScheme[i], out strReadData);
               if (!bValue)
               {
                   continue;
               }
               if (Stop) return;
                //再次读取标准表的值
               GlobalUnit.g_MsgControl.OutMessage("读取标准表数据....", false);
               CLDC_DataCore.Struct.StPower tagPower = new CLDC_DataCore.Struct.StPower();
               tagPower = CLDC_VerifyAdapter.Helper.EquipHelper.Instance.ReadPowerInfo();
                //计算相应的误差
               GlobalUnit.g_MsgControl.OutMessage("开始计算误差数据....", false);
                MeterBasicInfo _MeterInfo;
               for (int j = 0; j < BwCount; j++)
               {
                   arrStrResultKeyTmp[j] = strItemKeyId;
                     _MeterInfo = Helper.MeterDataHelper.Instance.Meter(j);                                              //表基本信息
                    if (!_MeterInfo.YaoJianYn)
                    {
                        continue;
                    }

                   string strValue=  strReadData[j];

                    MeterDgn _Error =null;
                   
                    if (_MeterInfo.MeterDgns.ContainsKey(strItemKeyId))
                        _Error = _MeterInfo.MeterDgns[strItemKeyId];
                    else
                    {
                        _Error = new MeterDgn();
                        _Error.Md_PrjID = strItemKeyId;
                        _Error.Md_PrjName = Cus_DgnItem.测量及监测误差.ToString() +"_" + testScheme[i];
                        _MeterInfo.MeterDgns.Add(strItemKeyId, _Error);
                    }
                    _Error.Md_chrValue = "";
                   //模型误差值
                   List<double> listWcz = new List<double>();
                   //字符串误差值
                   string strWcz = string.Empty;
                   //获取相应的读取标准值 并且计算误差值
                   string strStdValue = GetStdAndWcData(testScheme[i], strValue, tagPower, out listWcz, out strWcz);

                   if (i == 10)
                   {
                       double dCos = Math.Atan(Math.Sqrt(1 - Convert.ToSingle(strValue) * Convert.ToSingle(strValue)) / Convert.ToSingle(strValue)); //Math.Sin(Convert.ToSingle(strReadMeter));
                       dCos = dCos * 180 / 3.14159f;

                       strValue = dCos.ToString("F2");
                   }

                   _Error.Md_chrValue = strValue + "|" + strStdValue + "|" + strWcz;

                   //获取电表的等级判定结论
                    string[] _DJ = Number.getDj(_MeterInfo.Mb_chrBdj);
                    float _MeterLevel = float.Parse(_DJ[base.IsYouGong ? 0 : 1]);                   //当前表的等级
                    string strResult = string.Empty;
                    for (int k= 0; k < listWcz.Count; k++)
                    { 
                        if(strResult.Equals(Variable.CTG_BuHeGe))
                        {
                            break;
                        }
                        if (double.IsNaN(listWcz[k]))
                            listWcz[k] = 0;
                        strResult =Math.Abs((listWcz[k]*100))<1?Variable.CTG_HeGe:Variable.CTG_BuHeGe;
                    }
                    _Error.AVR_CONCLUSION = strResult;
                    _Error.Md_chrValue = strValue + "|" + strStdValue + "|" + strWcz+"|"+strResult;
                   //刷新数据
                    GlobalUnit.g_MsgControl.OutMessage();

                    Thread.Sleep(200);

               }

               //GlobalUnit.g_DataControl.OutUpdateData(999, arrStrResultKey, objResultValue, Cus_DBTableName.METER_COMMUNICATION, false);
               //存储一个点的数据
            //   CLDC_DataCore.Const.GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrStrResultKeyTmp);

               if (Stop) return;
                //通知刷型数据。
               GlobalUnit.g_MsgControl.OutMessage("开始关源....", false);
               //关源
               Helper.EquipHelper.Instance.PowerOff();

               for (int o = 0; o < 5; o++)
               {
                   Thread.Sleep(1000);
               }
               if (Stop) return;
               
            }
            //存储相应的数据到多功能模块

            GlobalUnit.g_MsgControl.OutMessage("开始存储数据....", false);
            for (int p = 0; p < BwCount; p++)
            {//判定总结论
                if (Helper.MeterDataHelper.Instance.Meter(p).YaoJianYn == false) continue;
                if (Helper.MeterDataHelper.Instance.Meter(p).MeterDgns.ContainsKey(ItemKey))
                {
                    Helper.MeterDataHelper.Instance.Meter(p).MeterDgns[ItemKey].Md_chrValue = CailabrateResult(Helper.MeterDataHelper.Instance.Meter(p));
                }
                else
                {
                    MeterDgn _Error = new MeterDgn();
                    Helper.MeterDataHelper.Instance.Meter(p).MeterDgns.Add(ItemKey, _Error);
                    _Error.Md_PrjID = ItemKey;
                    _Error.Md_PrjName = Cus_DgnItem.测量及监测误差.ToString();
                    _Error.Md_chrValue = CailabrateResult(Helper.MeterDataHelper.Instance.Meter(p));
                    _Error.AVR_CONCLUSION = _Error.Md_chrValue;
                }
            }

       //     CLDC_DataCore.Const.GlobalUnit.g_CUS.SaveTempDB(Cus_DBTableName.METER_COMMUNICATION, arrStrResultKey);
        //    GlobalUnit.g_RealTimeDataControl.OutUpdateRealTimeData(ItemKey, Cus_MeterDataType.多功能数据);

        //    GlobalUnit.g_MsgControl.OutMessage(CurPlan.ToString() + "检定完毕");
            Thread.Sleep(200);

        }

        private string CailabrateResult(MeterBasicInfo meterInfo)
        {
            MeterDgn _Error = null;
            string strValue = string.Empty;

            for (int i = 1; i < 13; i++)
            {
                string strKeyTmp = ItemKey + string.Format("{0:D2}", i);
                if (meterInfo.MeterDgns.ContainsKey(strKeyTmp))
                {
                    _Error = meterInfo.MeterDgns[strKeyTmp];
                    strValue = _Error.AVR_CONCLUSION;
                    if (strValue.Equals(Variable.CTG_BuHeGe))
                    {
                        break;
                    }
                }
            }

            return strValue;
        }

        private string GetStdAndWcData(string strParams, string strReadMeter, StPower tagParm,out List<double> dOutWcz,out string strOucWcz)
        {
            string strValue = string.Empty;
            dOutWcz = new List<double>();
            strOucWcz = string.Empty;
            string[] Parms = strParams.Split('|');
            if (Parms.Length <= 0)
            {
                return string.Empty;
            }

            switch (Parms[0])
            {
                case "Un":
                    {
                        #region 
                        string[] strUns = strReadMeter.Split(',');

                        //UA
                        float UaWc = (Convert.ToSingle(strUns[0]) - float.Parse(tagParm.Ua.ToString("#0.00"))) / (1.2F * GlobalUnit.U);

                        string strUaWc = UaWc.ToString("F2");
                        //UB
                        float UbWc = (Convert.ToSingle(strUns[1]) - float.Parse(tagParm.Ub.ToString("#0.00"))) / (1.2F * GlobalUnit.U);
                        string strUbWc = UaWc.ToString("F2");
                        //UC
                        float UcWc = (Convert.ToSingle(strUns[2]) - float.Parse(tagParm.Uc.ToString("#0.00"))) / (1.2F * GlobalUnit.U);
                        string strUcWc = UaWc.ToString("F2");
                        //添加出参
                        dOutWcz.Add(UaWc);
                        dOutWcz.Add(UbWc);
                        dOutWcz.Add(UcWc);
                        //误差值
                        strOucWcz = strUaWc + "," + strUbWc + "," + strUcWc;
                        //标准值
                        strValue = tagParm.Ua.ToString("F2") + "," + tagParm.Ub.ToString("F2") + "," + tagParm.Uc.ToString("F2");
                        #endregion
                    }
                    break;
                case "Imax":
                case "Ib":
                    {
                        #region
                        string[] strCurrents = strReadMeter.Split(',');

                        //UA
                        float CtaWc = (Convert.ToSingle(strCurrents[0]) - float.Parse(tagParm.Ia.ToString("#0.00"))) / (1.2F * GlobalUnit.Imax);
                        if (double.IsNaN(CtaWc))
                            CtaWc = 0;
                        string strCtaWc = CtaWc.ToString("F2");
                        //UB
                        float CtbWc = (Convert.ToSingle(strCurrents[1]) - float.Parse(tagParm.Ib.ToString("#0.00"))) / (1.2F * GlobalUnit.Imax);
                        if (double.IsNaN(CtaWc))
                        if (double.IsNaN(CtbWc))
                            CtbWc = 0;
                        string strCtbWc = CtbWc.ToString("F2");
                        //UC
                        float CtcWc = (Convert.ToSingle(strCurrents[2]) - float.Parse(tagParm.Ic.ToString("#0.00"))) / (1.2F * GlobalUnit.Imax);
                        if (double.IsNaN(CtaWc))
                        if (double.IsNaN(CtcWc))
                            CtcWc = 0;
                        string strCtcWc = CtcWc.ToString("F2");
                        //添加出参
                        dOutWcz.Add(CtaWc);
                        dOutWcz.Add(CtbWc);
                        dOutWcz.Add(CtcWc);
                        //误差值
                        strOucWcz = strCtaWc + "," + strCtbWc + "," + strCtcWc;

                        //标准值
                        strValue = tagParm.Ia.ToString("F2") + "," + tagParm.Ib.ToString("F2") + "," + tagParm.Ic.ToString("F2");
                        #endregion
                    }
                    break;
                case "P":
                    {
                        try
                        {
                            strValue = tagParm.P.ToString().Substring(0, tagParm.P.ToString().IndexOf('.') + 2);
                            float fPwc = (Convert.ToSingle(strReadMeter) - float.Parse(strValue)) / (GlobalUnit.U*1.2F*3*GlobalUnit.Imax*1.2F);
                            if (double.IsNaN(fPwc))
                                fPwc = 0;
                            dOutWcz.Add(fPwc);
                            strOucWcz = fPwc.ToString("F2");
                            strValue = tagParm.P.ToString().Substring(0, tagParm.P.ToString().IndexOf('.') + 2);
                        }
                        catch
                        {
                            float fPwc = (Convert.ToSingle(strReadMeter) - float.Parse(tagParm.P.ToString("#0.0"))) / (GlobalUnit.U * 1.2F * 3 * GlobalUnit.Imax * 1.2F);
                            if (double.IsNaN(fPwc))
                                fPwc = 0;
                            dOutWcz.Add(fPwc);
                            strOucWcz = fPwc.ToString("F2");
                            strValue = tagParm.P.ToString("F1");
                        }
                      
                    }
                    break;
                case "C":
                    {
                        double dCos = Math.Atan(Math.Sqrt(1 - Convert.ToSingle(strReadMeter) * Convert.ToSingle(strReadMeter)) / Convert.ToSingle(strReadMeter)); //Math.Sin(Convert.ToSingle(strReadMeter));
                        dCos = dCos * 180 / 3.14159f;
                        string Is_dan = CLDC_DataCore.Const.Variable.CTC_DESKTYPE;
                        float Fcos_StM;
                        if (Is_dan == "三相台")
                        {
                           // Fcos_StM = (tagParm.Phi_Ia + tagParm.Phi_Ib + tagParm.Phi_Ic) / 3;

                            Fcos_StM = (tagParm.P / tagParm.S);
                            double angle = Math.Acos(Fcos_StM) / 2 / Math.PI * 360;
                            Fcos_StM = float.Parse(angle.ToString());
                        }
                        else
                        {
                            Fcos_StM = (tagParm.P / tagParm.S);
                            double angle=Math.Acos(Fcos_StM)/2/Math.PI*360;
                            Fcos_StM = float.Parse( angle.ToString());
                        }
                        //if (Fcos_StM > 59 && Fcos_StM < 61)
                        //{
                        //    Fcos_StM = 60;
                        //}
                        float fCOSwc = (Convert.ToSingle(dCos.ToString()) - float.Parse(Fcos_StM.ToString("#0.00"))) / float.Parse(Fcos_StM.ToString("#0.00"));
                        if (double.IsNaN(fCOSwc))
                            fCOSwc = 0;
                        dOutWcz.Add(fCOSwc);
                        strOucWcz = fCOSwc.ToString("F2");

                        strValue = Fcos_StM.ToString("#0.00");
                    }
                    break;
                case "Hz":
                    {

                        float fPwc = (Convert.ToSingle(strReadMeter) - float.Parse(tagParm.Freq.ToString("#0.00"))) / 52.5F;
                        if (double.IsNaN(fPwc))
                            fPwc = 0;
                        dOutWcz.Add(fPwc);
                        strOucWcz = fPwc.ToString("F2");
                        strValue = tagParm.Freq.ToString("F2");

                      
                    }
                    break;
                default:
                    break;
            }

            return strValue;
        }


        private bool ReadMeterData(string strParams,out string[] strOutValue)
        {
            strOutValue = new string[BwCount];
            string[] Parms = strParams.Split('|');
            if (Parms.Length <= 0)
            {
                return false;
            }

            switch (Parms[0])
            {
                case "Un":
                    strOutValue = MeterProtocolAdapter.Instance.ReadData("0201FF00", 8);
                    //strOutValue  = MeterProtocolAdapter.Instance.ReadData("02800002", 2);


                   // int iLen = strOutValue.Length/3;
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn == false)
                        {
                            continue;
                        }
                      strOutValue[i]=  DisPoseVoltageData(strOutValue[i]);
                    }

                    break;
                case "Imax":
                case "Ib":
                    strOutValue = MeterProtocolAdapter.Instance.ReadData("0202FF00", 10);

                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn == false)
                        {
                            continue;
                        }
                        strOutValue[i] = DisPoseCurrentData(strOutValue[i]);
                    }
                    break;
                case "P":
                    strOutValue = MeterProtocolAdapter.Instance.ReadData("02030000", 3);

                    for (int i = 0; i < BwCount; i++)
                    {
                         if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn == false)
                        {
                            continue;
                        }
                         strOutValue[i] = DisposePglData(strOutValue[i]);
                    }
                    break;
                case "C":
                    strOutValue = MeterProtocolAdapter.Instance.ReadData("02060000", 2);
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn == false)
                        {
                            continue;
                        }
                        strOutValue[i] = DisPoseGlys(strOutValue[i]);
                    }
                    break;
                case "Hz":
                     //strOutValue = MeterProtocolAdapter.Instance.ReadData("0201FF00", 8);
                    strOutValue  = MeterProtocolAdapter.Instance.ReadData("02800002", 2);


                   // int iLen = strOutValue.Length/3;
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn == false)
                        {
                            continue;
                        }
                        strOutValue[i] = DisPoseFrequence(strOutValue[i]);
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        //处理读取到的电压数据
        private string DisPoseVoltageData(string strData)
        {
            string strValue = string.Empty;
            strValue = "0.0,0.0,0.0";
            if (string.IsNullOrEmpty(strData))
                return strValue;
            int iCout = strData.Length / 3;

            for (int i = 0; i < 3; i++)
            {
                strData = strData.Insert((i + 1) * iCout + i, ",");
            }

            int iTmp = strData.Length / 3;

            for (int j = 0; j < 3; j++)
            {
                strData = strData.Insert((j + 1) * iTmp + j - 2, ".");
            }

            strData = strData.TrimEnd(',');
            string[] strValues = strData.Split(',');
            strValue = string.Empty;
            for (int i = 0; i < 3; i++)
            {
                double dvalue = 0f;
                if(strValues[2 - i].IndexOf('F') == -1)
                    dvalue = Convert.ToDouble(strValues[2 - i]);
                
                strValue += dvalue.ToString() + ",";
            }
            strValue = strValue.TrimEnd(',');
            return strValue;
        }

        private string DisPoseFrequence(string strData)
        {
            string strValue = string.Empty;
            try
            {
               

                double tempvalue = Convert.ToDouble(strData);
                tempvalue = tempvalue / 100;
                strValue = tempvalue.ToString();
                return strValue;
            }
            catch
            {
                return strValue;
            }
           
        }

        //处理读取到的电流数据
        private string DisPoseCurrentData(string strData)
        {
            string strValue = string.Empty;
            strValue = "0.0,0.0,0.0";
            if (string.IsNullOrEmpty(strData))
                return strValue;
            int iCout = strData.Length / 3;

            for (int i = 0; i < 3; i++)
            {
                strData = strData.Insert((i + 1) * iCout + i, ",");
            }

            int iTmp = strData.Length / 3;

            for (int j = 0; j < 3; j++)
            {
                strData = strData.Insert((j + 1) * iTmp + j - 4, ".");
            }

            strData = strData.TrimEnd(',');
            string[] strValues = strData.Split(',');
            strValue = string.Empty;
            for (int i = 0; i < 3; i++)
            {                
                double dvalue = 0f;
                if (strValues[2 - i].IndexOf('F') == -1)
                    dvalue = Convert.ToDouble(strValues[2 - i]);
                strValue += dvalue.ToString() + ",";
            }
            strValue = strValue.TrimEnd(',');
            return strValue;
        }
        //处理读取电表功率数据
        private string DisposePglData(string strData)
        {
            string strValue =string.Empty;
             strValue = "0.0";
            if (string.IsNullOrEmpty(strData))
                return strValue;
            double dValue = Convert.ToDouble(strData) / 10;

            strValue = dValue.ToString();


            return strValue;
        }

        private string DisPoseGlys(string strData)
        {
            string strValue = string.Empty;

            strValue = "0.0";
            if (string.IsNullOrEmpty(strData))
            {
                return strValue;
            }
            double dValue = Convert.ToDouble(strData) / 1000;

            strValue = dValue.ToString();


            return strValue;
        }

        private bool  PowerOn(string strParams)
        {
            m_CheckOver = false;

            float fVolt = CLDC_DataCore.Const.GlobalUnit.U;

            float fCurrent = 0f;

            string strGlys = "1.0";

            float Hz = 0F;

            string[] Parms = strParams.Split('|');
            if (Parms.Length <= 0)
            {
                return false;
            }

            switch (Parms[0])
            {
                case "Un":
                    fVolt = Convert.ToSingle(Parms[1]) / 100 * CLDC_DataCore.Const.GlobalUnit.U;
                    break;
                case "Imax":
                    fVolt = CLDC_DataCore.Const.GlobalUnit.U;
                    fCurrent = Convert.ToSingle(Parms[1]) / 100 * CLDC_DataCore.Const.GlobalUnit.Imax;
                    break;
                case "Ib":
                    fVolt = CLDC_DataCore.Const.GlobalUnit.U;
                    fCurrent = Convert.ToSingle(Parms[1]) / 100 * CLDC_DataCore.Const.GlobalUnit.Ib;
                    break;
                case "P":
                    fVolt = Convert.ToSingle(Parms[1]) / 100 * CLDC_DataCore.Const.GlobalUnit.U;
                    fCurrent = Convert.ToSingle(Parms[2]) / 100 * CLDC_DataCore.Const.GlobalUnit.Ib;
                    if (Parms[4].Equals("Imax"))
                    {
                        fCurrent = Convert.ToSingle(Parms[2]) / 100 * CLDC_DataCore.Const.GlobalUnit.Imax;
                    }
                    strGlys = Parms[3];
                    break;
                case "C":
                    strGlys = Parms[1];
                    fCurrent =  CLDC_DataCore.Const.GlobalUnit.Ib;
                    break;
                case "Hz":
                    Hz =float.Parse( Parms[1]);
                    fCurrent = CLDC_DataCore.Const.GlobalUnit.Ib;
                    break;
                default:
                    break;
            }
            if (Parms[0] == "Hz")
            {

                if (Helper.EquipHelper.Instance.PowerOn(fVolt, fVolt, fVolt, fCurrent, fCurrent, fCurrent, (int)Cus_PowerYuanJian.H, Hz, strGlys, true, false, false, (int)PowerFangXiang) == false)//
                {
                    GlobalUnit.g_MsgControl.OutMessage("控制源输出失败");
                    return false;
                }
                else
                {
                    return true;
                }
            }
          //   Helper.EquipHelper.Instance.PowerOn(CLDC_DataCore.Const.GlobalUnit.U, _xIb, _curPoint.PowerYuanJian, _curPoint.PowerFangXiang, FangXiangStr + _curPoint.PowerYinSu, IsYouGong, false);
            if (Helper.EquipHelper.Instance.PowerOn(fVolt, fCurrent, (int)Cus_PowerYuanJian.H, (int)PowerFangXiang, strGlys, true, false) == false)//
            {
                GlobalUnit.g_MsgControl.OutMessage("控制源输出失败");
                return false;
            }
            if (Parms[0] == "C")
            {
                Helper.EquipHelper.Instance.PowerOn(fVolt, fCurrent, (int)Cus_PowerYuanJian.H, (int)PowerFangXiang, strGlys, true, false);
            }
            return true;
        }
    }
}
