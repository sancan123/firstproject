using System;
using CLDC_DataCore;
using System.Collections.Generic;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_DataCore.Struct;
using CLDC_DataCore.Function;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.Helper;
using CLDC_DeviceDriver;
using CLDC_VerifyAdapter.VerifyService;

namespace CLDC_VerifyAdapter.Function.SignalOutPut
{
    class HarmonicPulseOut: VerifyBase
    {
        public HarmonicPulseOut
            (object plan)
            : base(plan)
        {
          
        }
        string StrDI = "";
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
            string[] _Prams = VerifyPara.Split('|');
            if (_Prams.Length < 8) return false;
            StPlan_ZouZi curPoint = new StPlan_ZouZi();
            curPoint.FeiLv = (Cus_FeiLv)Enum.Parse(typeof(Cus_FeiLv), _Prams[5]);
            curPoint.FeiLvString = _Prams[5];
            curPoint.Glys = _Prams[2];
            curPoint.PowerFangXiang = (Cus_PowerFangXiang)Enum.Parse(typeof(Cus_PowerFangXiang), _Prams[0]);
            curPoint.PowerYj = (Cus_PowerYuanJian)Enum.Parse(typeof(Cus_PowerYuanJian), _Prams[1]);
            curPoint.xIb = _Prams[3];
            curPoint.ZouZiMethod = (Cus_ZouZiMethod)Enum.Parse(typeof(Cus_ZouZiMethod), _Prams[4]);
            string dufen = _Prams[6] + "度";
            curPoint.UseMinutes = float.Parse(_Prams[6]);
            if (_Prams[7].Trim() != "0")
            {
                curPoint.UseMinutes = float.Parse(_Prams[7]);
                dufen = _Prams[7] + "分";
            }
            curPoint.ZouZiPrj = new List<StPlan_ZouZi.StPrjFellv>() { 
                new StPlan_ZouZi.StPrjFellv()
                {
                    FeiLv= (Cus_FeiLv)Enum.Parse(typeof(Cus_FeiLv), _Prams[5]),
                    StartTime="",
                    ZouZiTime=dufen
                }
            };
            curPoint.ZuHeWc = "0";
          
            CurPlan = curPoint;

            StPlan_ZouZi _curPoint = (StPlan_ZouZi)CurPlan;

            StrDI = GetStrDI(curPoint.PowerFangXiang, curPoint.PowerYj);
            ResultNames = new string[] { "起码", "止码", "表码差", "表脉冲", "标准表脉冲", "误差", "结论", "不合格原因" };
            return true;
        }
        public string GetStrDI(Cus_PowerFangXiang FX, Cus_PowerYuanJian YJ)
        {
            if (FX == Cus_PowerFangXiang.正向有功 && YJ == Cus_PowerYuanJian.H)
            {
                StrDI = "00830000";
            }
            else if (FX == Cus_PowerFangXiang.反向有功 && YJ == Cus_PowerYuanJian.H)
            {
                StrDI = "00840000";
            }
            else if (FX == Cus_PowerFangXiang.正向有功 && YJ == Cus_PowerYuanJian.A)
            {
                StrDI = "00970000";
            }
            else if (FX == Cus_PowerFangXiang.反向有功 && YJ == Cus_PowerYuanJian.A)
            {
                StrDI = "00980000";
            }
            else if (FX == Cus_PowerFangXiang.正向有功 && YJ == Cus_PowerYuanJian.B)
            {
                StrDI = "00AB0000";
            }
            else if (FX == Cus_PowerFangXiang.反向有功 && YJ == Cus_PowerYuanJian.B)
            {
                StrDI = "00AC0000";
            }
            else if (FX == Cus_PowerFangXiang.正向有功 && YJ == Cus_PowerYuanJian.C)
            {
                StrDI = "00BF0000";
            }
            else if (FX == Cus_PowerFangXiang.反向有功 && YJ == Cus_PowerYuanJian.C)
            {
                StrDI = "00C00000";
            }

            return StrDI;
        }
        
               
        public override void Verify()
        {
            base.Verify();
        
           bool[] Result = new bool[BwCount];
           string[] Fail = new string[BwCount];
           MeterBasicInfo _FirstMeter = Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter);       //第一块表基本功信息
           StPlan_ZouZi _curPoint = (StPlan_ZouZi)CurPlan;
           this.PowerFangXiang = _curPoint.PowerFangXiang;
           //把方案时间分转化为秒
           int _MaxTestTime = (int)(_curPoint.UseMinutes * 60);
           Cus_ZouZiMethod _ZZMethod = _curPoint.ZouZiMethod;
           //设置误差计算器参数
           string[] arrData = new string[0];    //数据数组
           string strDesc = string.Empty;       //描述信息

           m_CheckOver = false;
           //获取走字的电流
           float testI = CLDC_DataCore.Function.Number.GetCurrentByIb(_curPoint.xIb, Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).Mb_chrIb, Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).Mb_BlnHgq);
           if (Stop)
           {
               return;
           }
           bool bPowerOn = PowerOn();
            MessageController.Instance.AddMessage("正在读取谐波电能");
            
            float[] energys = MeterProtocolAdapter.Instance.ReadData(StrDI, 4, 2);
          
         
            for (int i = 0; i < BwCount; i++)
            {
                if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    //"功率元件","功率方向","电流倍数","功率因数","误差下限","误差上限","误差圈数"
                    ResultDictionary["起码"][i] = energys[i].ToString();
                 
                   
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "起码", ResultDictionary["起码"]);
            if (Stop)
            {
                return;
            }
            Thread.Sleep(3000);

            if (EquipHelper.Instance.InitPara_Constant(_curPoint.PowerFangXiang, null) == false)
            {
                //Stop = true;
                MessageController.Instance.AddMessage("启动误差板走字指令失败", 6, 2);
                //return;
            }
            bool YouGong = true;
            if (_curPoint.PowerFangXiang == Cus_PowerFangXiang.正向有功 || _curPoint.PowerFangXiang == Cus_PowerFangXiang.反向有功)
            {
                YouGong = true;
            }
            else
            {
                YouGong = false;
            }



            if (Stop)
            {
                return;
            }
            if (EquipHelper.Instance.PowerOn(GlobalUnit.U, testI, (int)_curPoint.PowerYj, (int)_curPoint.PowerFangXiang, _curPoint.Glys, YouGong, false) == false)
            {
                //Stop = true;
                //return;
            }
            if (Stop)
            {
                return;
            }
       
            string stroutmessage = string.Empty;        //外发消息
            DateTime startTime = DateTime.Now.AddSeconds(-5);   //检定开始时间,减掉源等待时间
          float  _StarandMeterDl = 0;                        //标准表电量
            DateTime lastTime = DateTime.Now.AddSeconds(-5);
            //_ZZMethod = Cus_ZouZiMethod.计读脉冲法;

            while (true)
            {
                Thread.Sleep(1000);
                if (Stop) return;
                int _PastTime = DateTimes.DateDiff(startTime);
                //处理跨天
                if (_PastTime < 0) _PastTime += 43200;      //如果当前PC为12小时制
                if (_PastTime < 0) _PastTime += 43200;      //24小时制
                if (_PastTime < 0)
                {
                    //处理人为修改时间
                    //跨度超过24小时，肯定是系统日期被修改
                    MessageController.Instance.AddMessage("系统时间被人为修改超过24小时，检定停止");
                    Stop = true;
                    return;
                }

          
                    #region
                    if (!GlobalUnit.IsDemo)
                    {
                        //记录标准表电量
                        float pSum = 0;
                        if (YouGong)
                        {
                            //if (GlobalUnit.g_StrandMeterP[0] > 0)
                            pSum = Math.Abs(GlobalUnit.g_StrandMeterP[0]);
                        }
                        else
                        {
                            //if (GlobalUnit.g_StrandMeterP[1] > 0)
                            pSum = Math.Abs(GlobalUnit.g_StrandMeterP[1]);
                        }

                        float pastSecond = (float)(DateTime.Now - lastTime).TotalMilliseconds;
                        lastTime = DateTime.Now;
                        _StarandMeterDl += pastSecond * pSum / 3600 / 1000 / 1000;
                        //同步记录（读）脉冲数
                        if (arrData.Length < BwCount)
                        {
                            arrData = new string[BwCount];
                        }
                        stError[] errors = EquipHelper.Instance.ReadWcb(true);
                        for (int i = 0; i < errors.Length; i++)
                        {
                            arrData[i] = errors[i].szError;
                        }
                          
                        if (IsYouGong)
                        {
                            //if (GlobalUnit.g_StrandMeterP[0] > 0)
                            pSum = Math.Abs(GlobalUnit.g_StrandMeterP[0]);
                        }
                        else
                        {
                            //if (GlobalUnit.g_StrandMeterP[1] > 0)
                            pSum = Math.Abs(GlobalUnit.g_StrandMeterP[1]);
                        }
                        //再算一次电量
                        pastSecond = (int)(DateTime.Now - lastTime).TotalSeconds;
                        //lastTime = DateTime.Now;
                        _StarandMeterDl = pastSecond * pSum / 3600 / 1000;
                    }
                    else
                    {
                        //模拟电量
                        _StarandMeterDl = _PastTime * GlobalUnit.U * testI / 3600000F;
                        //同步模拟脉冲数
                    }
                    //如果电量达到设定，停止
                    if (_StarandMeterDl >= _curPoint.UseMinutes - 0.02)
                    {
                        m_CheckOver = true;
                    }
                    //如果脉冲数达到设定，也停止
                    float flt_C = 0;
                    if (arrData != null && arrData.Length > 0)
                    {
                        float.TryParse(arrData[GlobalUnit.FirstYaoJianMeter], out flt_C);
                    }
                    flt_C = flt_C / Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).GetBcs()[0];
                    if (flt_C >= _curPoint.UseMinutes - 0.002)
                    {
                        m_CheckOver = true;
                    }
                    //外发检定消息
                    GlobalUnit.g_CUS.DnbData.NowMinute = _StarandMeterDl;
                    stroutmessage = string.Format("方案设置走字电量：{0}度，已经走字：{1}度", _curPoint.UseMinutes, _StarandMeterDl.ToString("F5"));
                    #endregion
         
               
                //缓存数据
                for (int i = 0; i < BwCount; i++)
                {
                    MeterBasicInfo _meterInfo = Helper.MeterDataHelper.Instance.Meter(i);
                    if (!_meterInfo.YaoJianYn)
                    {
                        continue;
                    }

                    //"表脉冲", "标准表脉冲"
                    if (arrData != null && arrData.Length > i)
                    {
                        ResultDictionary["表脉冲"][i] = arrData[i];
                    }
                    ResultDictionary["标准表脉冲"][i] = ((_StarandMeterDl * _meterInfo.GetBcs()[0])).ToString("F2");
                }
                UploadTestResult("表脉冲");
                UploadTestResult("标准表脉冲");
              
            
             
                if (m_CheckOver)
                {
                    break;
                }
            }

            if (Stop)
            {
                return;
            }
            EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, (int)_curPoint.PowerYj, (int)_curPoint.PowerFangXiang, _curPoint.Glys, YouGong, false);

            if (Stop)
            {
                return;
            }
            if (!GlobalUnit.IsDemo)
            {
                if (arrData.Length < BwCount)
                {
                    arrData = new string[BwCount];
                }
                stError[] errors = EquipHelper.Instance.ReadWcb(true);
                for (int i = 0; i < errors.Length; i++)
                {
                    arrData[i] = errors[i].szError;
                }
            }

           
                MessageController.Instance.AddMessage("正在读取止码");
                float[] energysZ = MeterProtocolAdapter.Instance.ReadData(StrDI, 4, 2);
                for (int i = 0; i < BwCount; i++)
                {
                    if (Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                    {
                        //"功率元件","功率方向","电流倍数","功率因数","误差下限","误差上限","误差圈数"
                        ResultDictionary["止码"][i] = energysZ[i].ToString();


                    }
                }
                MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "止码", ResultDictionary["止码"]);
                for (int i = 0; i < BwCount; i++)
                {
                    MeterBasicInfo _meterInfo = Helper.MeterDataHelper.Instance.Meter(i);
                    if (!_meterInfo.YaoJianYn)
                    {
                        continue;
                    }
                    //"表脉冲", "标准表脉冲"
                    if (arrData != null && arrData.Length > i)
                    {
                        ResultDictionary["表脉冲"][i] = arrData[i];
                    }
                    float flt_QZC = energysZ[i] - energys[i];
                    ResultDictionary["表码差"][i] = ((flt_QZC * _meterInfo.GetBcs()[0])).ToString("F2");
                }
                UploadTestResult("表脉冲");
                UploadTestResult("表码差");
            
            
            
            


            MeterBasicInfo _Meter = null;                                           //当前表检定基本信息
            MeterZZError _ZZError = null;                                           //检定结论
            bool isAllHeGe = true;                                                 //是否所有表都合格
            CLDC_DataCore.Const.GlobalUnit.g_MsgControl.OutMessage("正在计算走字结果");
            CLDC_DataCore.Struct.StWuChaDeal zzWCPata = new CLDC_DataCore.Struct.StWuChaDeal();
            for (int r = 0; r < BwCount; r++)
            {
                float fDiff = 0.0f;
              int  dotLeng = 5;
              int intPower = (int)Math.Pow(10, dotLeng);
             float starandDl = _StarandMeterDl;
              starandDl = (int)(Math.Round(starandDl * intPower));
              starandDl /= intPower;
              fDiff = energysZ[r] - energys[r];
              float err = fDiff - starandDl;
              ResultDictionary["误差"][r] = err.ToString();

              if (energysZ[r].ToString() == "-1" || energys[r].ToString() == "-1" || energysZ[r].ToString() == energys[r].ToString() || err > 0.1)
              {
                  ResultDictionary["结论"][r] = "不合格";
              }
              else
              {
                  ResultDictionary["结论"][r] = "合格";
              }

                                                                                    
            }
          
            UploadTestResult("误差");
            UploadTestResult("结论");


        }

       

    }
}

