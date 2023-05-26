
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

namespace CLDC_VerifyAdapter.SecondStage
{
    #region 
    enum TestType1
    {
        默认 = 0,
        /// <summary>
        /// 总与分同时做是指当有总费率及其它分费率在一起时。先读取总费率的起码，然后走分费率。最后再读取总
        /// 费率的止码。
        /// </summary>
        总与分费率同时做 = 1,
        /// <summary>
        /// 此种走字方式是指只走总时读取所有分费率起码，总走完后再读取所有分费率止码。
        /// 此种方式应用于总走字时间较长，同时跨几个分费率的情况
        /// </summary>
        自动检定总时段内的所有分费率 = 2
    }

    /// <summary>
    /// 读取电量方式
    /// </summary>
    public enum ReadEnergyType
    {
        使用485自动读取,
        //读取误差板脉冲数字,
        手动输入
    }
    #endregion

    /// <summary>
    /// 热插拔试验
    /// </summary>
    class ConstantVerifyHot : VerifyBase
    {
        #region ----------变量声明----------
        /// <summary>
        /// 当前操作是否全部完成
        /// </summary>
        private bool CurActionOver = false;

        //标准表累积电量
        private float _StarandMeterDl = 0F;

        #endregion

        public CLDC_Encryption.CLEncryption.Interface.IAmMeterEncryption EncryptionTool;


        public CLDC_Encryption.CLEncryption.EncryptionFactory encryptionFactory;
        /// <summary>
        /// 清理节点数据[重写]
        /// </summary>
        protected override void ClearItemData()
        {
            CLDC_DataCore.Model.DnbModel.DnbInfo.MeterBasicInfo meter = null;
            for (int i = 0; i < BwCount; i++)
            {
                meter = Helper.MeterDataHelper.Instance.Meter(i);
                if (meter != null)
                {
                    if (meter.MeterZZErrors.ContainsKey(ItemKey))
                    {
                        meter.MeterZZErrors.Remove(ItemKey);
                    }
                }
            }
        }

        #region------------属性------------
        /// <summary>
        /// 是否是机械表
        /// </summary>
        private bool IsJiXieBiao
        {
            get
            {
                MeterBasicInfo _m = Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter);
                return !CLDC_DataCore.Function.Common.IsEmpty(_m.Mb_chrAddr);
            }
        }


        protected override string ItemKey
        {
            get
            {
                return ((StPlan_ZouZi)CurPlan).itemKey;
            }
        }

        protected override string ResultKey
        {
            get { return string.Format("{0}", (int)Cus_MeterResultPrjID.走字试验); }
        }

        #endregion

        public ConstantVerifyHot(object plan)
            : base(plan)
        {
          
        }

        #region 读取起码、止码、写表时间
        /// <summary>
        /// 读取电表能信息 
        /// </summary>
        /// <param name="isStartEnergy"></param>
        /// <returns></returns>
        private bool ReadMeterEnergys(bool isStartEnergy)
        {
            int YaoJianCount = 0;
            int noPassCount = 0;
            StPlan_ZouZi _curPoint = (StPlan_ZouZi)CurPlan;
            MessageController.Instance.AddMessage(string.Format("正在读取{0}电量...", isStartEnergy ? "起始" : "结束"));
          
            MeterBasicInfo curMeter;
            try
            {
                noPassCount = 0;
                YaoJianCount = 0;
                //等待 3秒，因为切换电源后，可能会电路断电，电表重启时，需要一定时间
                Thread.Sleep(3 * 1000);
                float[] energys = MeterProtocolAdapter.Instance.ReadEnergy((byte)(this.PowerFangXiang - 1), (byte)((StPlan_ZouZi)CurPlan).FeiLv);
                for (int i = 0; i < BwCount; i++)
                {
                    curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                    if (curMeter.YaoJianYn == false)
                    {
                        continue;
                    }
                    if (Stop)
                    {
                        break;
                    }

                    YaoJianCount++;
                    if (energys[i] < 0)
                    {
                        energys[i] = MeterProtocolAdapter.Instance.ReadEnergy((byte)(this.PowerFangXiang - 1), (byte)((StPlan_ZouZi)CurPlan).FeiLv, i);
                    }
                    if (energys[i] < 0)
                    {
                        noPassCount++;
                        MessageController.Instance.AddMessage("表位号" + (i + 1) + "没有读到电量值(" + energys[i] + ")");
                        continue;
                    }
                }

                if (noPassCount >= YaoJianCount)
                {
                    //提示手动输入电量
                    //WaitInputDl(PowerFangXiang, _curPoint.FeiLv, isStartEnergy);
                }
                if (isStartEnergy)
                {
                    arrayQima = energys;
                }
                else
                {
                    arrayZima = energys;
                }
                string resultName = isStartEnergy ? "起码" : "止码";
                ConvertTestResult(resultName, energys);
                UploadTestResult(resultName);
                return true;
            }
            catch (System.Exception e)
            {
                MessageController.Instance.AddMessage(e.Message, 6, 2);
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// 写表时间
        /// </summary>
        /// <param name="dateTime">日期格式 yymmddhhffss</param>
        /// <returns>操作是否成功</returns>
        private bool WriteDateTime(string dateTime)
        {
            //string showMsg = GlobalUnit.GetConfig(CLDC_DataCore.Const.Variable.CTC_DGN_WRITEMETERALARM, "是");
            //if (showMsg.Equals("是", StringComparison.OrdinalIgnoreCase))
            //{
            //    System.Windows.Forms.MessageBox.Show("请确认打开被检表的编程的开关", "提示", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            //}
          
            bool bResult = true;
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机
            string[] strEsamNo = new string[BwCount];//Esam序列号
            string[] strPutApdu = new string[BwCount];
            string[] strID = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strSetData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            bool[] result = new bool[BwCount];
            string[] strCode = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 6];
            string[] strMeterTime = new string[BwCount];
            string[] strShowData = new string[BwCount];

            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);

         //   if (Stop) return;
            MessageController.Instance.AddMessage("正在设置时间,请稍候....");
            for (int i = 0; i < BwCount; i++)
            {
                strCode[i] = "0400010C";
                strSetData[i] = DateTime.Now.ToString("yyMMdd") + "0" + (int)DateTime.Now.DayOfWeek;
                strSetData[i] += DateTime.Now.ToString("HHmmss");
                strShowData[i] = DateTime.Now.ToString("yyMMddHHmmss");
                strData[i] = strCode[i] + strSetData[i];
            }
            bool[] bln_Rst = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
   //        MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "设置值", strShowData);
            for (int i = 0; i < bln_Rst.Length; i++)
            {
                if (!GlobalUnit.g_CUS.DnbData.MeterGroup[i].YaoJianYn) continue;
                if (bln_Rst[i] == false)
                {
                    bResult = false;
                 //   strMessageText += (i + 1).ToString() + "号,";
                }
            }

            if (!bResult)
            {
            //    strMessageText = strMessageText.Trim(',');
             //   strMessageText += "表位修改时间失败，试验停止";
         //       MessageController.Instance.AddMessage(strMessageText, 6, 2);
                Stop = true;
            }
            return bResult;
        }

        /// <summary>
        /// 等待手动录入电量
        /// </summary>
        /// <param name="p">要输入的功率方向</param>
        /// <param name="f">要输入的费率</param>
        //private void WaitInputDl(Cus_PowerFangXiang p, Cus_FeiLv f, bool isQiMa)
        //{
        //    String strDes = String.Format("请输入{0} {1}电量", p.ToString(), f.ToString());

        //    CurActionOver = false;
        //    //手动录入电量则不做组合误差
        //    MessageController.Instance.AddMessage(strDes);
        //    while (!CurActionOver)
        //    {
        //        Thread.Sleep(GlobalUnit.g_ThreadWaitTime);
        //        if (Stop) return;
        //    }
        //}
        #endregion
        private float[] arrayQima = new float[GlobalUnit.g_CUS.DnbData._Bws];
        private float[] arrayZima = new float[GlobalUnit.g_CUS.DnbData._Bws];
        /// <summary>
        /// 走字试验[默认模式]
        /// </summary>
        /// <param name="ItemNumber"></param>
        public override void Verify()
        {
            GlobalUnit.g_CurTestType = 3;
            //基类确定检定ID
            base.Verify();
            #region 初始化工作
            MeterBasicInfo _FirstMeter = Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter);       //第一块表基本功信息
            Cus_ZouZiMethod _ZZMethod;                                              //走字试验方法：标准表法或是头表法
            StPlan_ZouZi _curPoint = (StPlan_ZouZi)CurPlan;
            this.PowerFangXiang = _curPoint.PowerFangXiang;
            //把方案时间分转化为秒
            int _MaxTestTime = (int)(_curPoint.UseMinutes * 60);
            _ZZMethod = _curPoint.ZouZiMethod;
            //设置误差计算器参数
            string[] arrData = new string[0];    //数据数组
            string strDesc = string.Empty;       //描述信息

            m_CheckOver = false;
            //获取走字的电流
            float testI = CLDC_DataCore.Function.Number.GetCurrentByIb(_curPoint.xIb, Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).Mb_chrIb, Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).Mb_BlnHgq);
            //初始化相关的电能表走字数据
            InitZZData(testI.ToString());
            if (Stop)
            {
                return;
            }
            //MessageController.Instance.AddMessage("设置表开关!");
            //Helper.EquipHelper.Instance.SetMeterOnOff(Helper.MeterDataHelper.Instance.GetYaoJian());
            #endregion

          

            #region //第一步升压,不升电流,因为电表只有在 升源后，才能进行通讯
            bool isSc = EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, (int)_curPoint.PowerYj, (int)_curPoint.PowerFangXiang, _curPoint.Glys, IsYouGong, false);
            if (isSc == false)
            {
                Stop = true;
                return;
            }
            if (Stop)
            {
                return;
            }
            #endregion

            #region //第二步 如果不是测量 总费率的话，需要 将电能表的当前时间，改成要测试的费率的对应时间
            if (_curPoint.FeiLv != Cus_FeiLv.总)
            {
                string time = _curPoint.StartTime;
                DateTime dt = DateTime.Now;
                int hh = int.Parse(_curPoint.StartTime.Substring(0, 2));
                int mm = int.Parse(_curPoint.StartTime.Substring(3, 2));
                dt = new DateTime(dt.Year, dt.Month, dt.Day, hh, mm, 0);
                if (this.WriteDateTime(dt.ToString("yyMMddHHmmss")) == false)
                {
                    return;
                }
            }
            #endregion

            #region // 第三步，如果不是 计读脉冲法 读取起始电量
            if (Stop)
            {
                return;
            }
            if (_ZZMethod != Cus_ZouZiMethod.计读脉冲法 && ReadMeterEnergys(true) == false)
            {
                MessageController.Instance.AddMessage("读取起始电量失败,本项检定将终止", 6, 2);
                //Stop = true;
                //return;
            }
            #endregion

            //EquipHelper.Instance.PowerOff();//以前的309会突然升到100A然后再升到指定的电流，所以这里不关源了
            if (Stop)
            {
                return;
            }
            Thread.Sleep(3000);

            #region //第四步，发送走字指令，开始走字
            if (EquipHelper.Instance.InitPara_Constant(_curPoint.PowerFangXiang, null) == false)
            {
                //Stop = true;
                MessageController.Instance.AddMessage("启动误差板走字指令失败", 6, 2);
                //return;
            }
            #endregion

            #region //第五步，升走字电流
            if (Stop)
            {
                return;
            }
            if (EquipHelper.Instance.PowerOn(GlobalUnit.U, testI, (int)_curPoint.PowerYj, (int)_curPoint.PowerFangXiang, _curPoint.Glys, IsYouGong, false) == false)
            {
                //Stop = true;
                //return;
            }

            #endregion

            #region 提示拔出通讯模块
            System.Windows.Forms.MessageBox.Show("请拔出通讯模块后点击确定。");
            #endregion

            #region //第七步，控制 执行步骤
            string stroutmessage = string.Empty;        //外发消息
            DateTime startTime = DateTime.Now.AddSeconds(-5);   //检定开始时间,减掉源等待时间
            _StarandMeterDl = 0;                        //标准表电量
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

                if (_ZZMethod == Cus_ZouZiMethod.计读脉冲法)
                {
                    #region
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
                    else
                    {
                  //      Helper.VerifyDemoHelper.Instance.GetPulseCount(ref arrData);
                    }
                    //等待所有表都跑完指定的脉冲
                    bool bOver = true;
                    for (int i = 0; i < BwCount; i++)
                    {
                        MeterBasicInfo _meterInfo = Helper.MeterDataHelper.Instance.Meter(i);
                        if (!_meterInfo.YaoJianYn)
                            continue;
                        if (arrData[i] == null || arrData[i].Length == 0 || int.Parse(arrData[i]) < (int)_curPoint.UseMinutes)
                        {
                            bOver = false;
                            break;
                        }

                    }
                    m_CheckOver = bOver;
                    GlobalUnit.g_CUS.DnbData.NowMinute = float.Parse(arrData[GlobalUnit.FirstYaoJianMeter]);
                    //外发检定消息
                    stroutmessage = string.Format("方案设置脉冲：{0}个，第一要检表位已经收到：{1}个", _curPoint.UseMinutes, arrData[GlobalUnit.FirstYaoJianMeter]);
                    #endregion
                }
                else if (_ZZMethod == Cus_ZouZiMethod.标准表法 || _ZZMethod == Cus_ZouZiMethod.校核常数)
                {
                    #region
                    if (!GlobalUnit.IsDemo)
                    {
                        //记录标准表电量
                        float pSum = 0;
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

                        float pastSecond = (float)(DateTime.Now - lastTime).TotalMilliseconds;
                        lastTime = DateTime.Now;
                        _StarandMeterDl += pastSecond * pSum / 3600 / 1000/1000;
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
                        //再算一次电量
                        pastSecond = (int)(DateTime.Now - lastTime).TotalMilliseconds;
                        lastTime = DateTime.Now;
                        _StarandMeterDl += pastSecond * pSum / 3600 / 1000/1000;
                    }
                    else
                    {
                        //模拟电量
                        _StarandMeterDl = _PastTime * GlobalUnit.U * testI / 3600000F;
                        //同步模拟脉冲数
                    }
                    //如果电量达到设定，停止
                    if (_StarandMeterDl >= _curPoint.UseMinutes - 0.002)
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
                }
                else
                {
                    #region
                    //走字试验法
                    if (_PastTime >= _MaxTestTime)
                    {
                        m_CheckOver = true;
                    }
                    GlobalUnit.g_CUS.DnbData.NowMinute = _PastTime / 60F;
                    stroutmessage = string.Format("方案设置走字时间：{0}分，已经走字：{1}分", _curPoint.UseMinutes, Math.Round(GlobalUnit.g_CUS.DnbData.NowMinute, 2));
                    #endregion
                }
                #region 更新数据
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
                MessageController.Instance.AddMonitorMessage(EnumMonitorType.ErrorBoard, string.Join(",0|", arrData));
                #endregion
                MessageController.Instance.AddMessage(stroutmessage);
                if (m_CheckOver)
                {
                    break;
                }
            }
            #endregion

            #region //第八步升压,不升电流,因为电表只有在 升源后，才能进行通讯
            isSc = EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, (int)_curPoint.PowerYj, (int)_curPoint.PowerFangXiang, _curPoint.Glys, IsYouGong, false);
            if (isSc == false)
            {
                Stop = true;
                return;
            }
            #endregion


            #region //第九步 读终止时的电量或脉冲数
            //if (_ZZMethod == Cus_ZouZiMethod.校核常数)
            {
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
                else
                {
              //      Helper.VerifyDemoHelper.Instance.GetPulseCount(ref arrData);
                }
            }
            if (Stop)
            {
                return;
            }
            if (_ZZMethod != Cus_ZouZiMethod.计读脉冲法)
            {
                if (ReadMeterEnergys(false) == false)
                {
                    MessageController.Instance.AddMessage("读取终止电表量失败", 6, 2);
                    Stop = true;
                    return;
                }
            }

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
                float flt_QZC = arrayZima[i] - arrayQima[i];
                ResultDictionary["表码差"][i] = ((flt_QZC * _meterInfo.GetBcs()[0])).ToString("F2");
            }
            UploadTestResult("表脉冲");
            UploadTestResult("表码差");
            MessageController.Instance.AddMonitorMessage(EnumMonitorType.ErrorBoard, string.Join(",0|", arrData));
            #endregion

            #region //第十步将 电表时间改成 计算机当前时间
            if (_curPoint.FeiLv != Cus_FeiLv.总)
            {
                MessageController.Instance.AddMessage("正在修改表时间为当前时间..");
                this.WriteDateTime(DateTime.Now.ToString("yyMMddHHmmss"));
            }
            #endregion

            #region ////第十一步降源,这一步，被检定基类处理
            //isSc = EquipHelper.Instance.PowerOn(0, 0, _curPoint.PowerYj, _curPoint.PowerFangXiang, _curPoint.Glys, IsYouGong, false);
            //if (isSc == false)
            //{
            //    Stop = true;
            //    return;
            //}
            #endregion

            #region //第十二步，计算误差
            try
            {
                ControlZZResult(_curPoint, _ZZMethod, arrData, ItemKey);
                ControlResult();
            }
            catch
            {
                Stop = true;
                MessageController.Instance.AddMessage("计算走字误差时出现错误", 6, 2);
            }
            #endregion
        }

        #region 内部方法
        /// <summary>
        /// 计算 走字结果
        /// </summary>
        /// <param name="_curPoint">当前检定点</param>
        /// <param name="_ZZMethod">走字方式</param>
        /// <param name="arrData"></param>
        /// <param name="strKey"></param>
        private void ControlZZResult(StPlan_ZouZi _curPoint, Cus_ZouZiMethod _ZZMethod,
            string[] arrData, string strKey)
        {
            MeterBasicInfo _Meter = null;                                           //当前表检定基本信息
            bool isAllHeGe = true;                                                 //是否所有表都合格
            MessageController.Instance.AddMessage("正在计算走字结果");
            CLDC_DataCore.Struct.StWuChaDeal zzWCPata = new CLDC_DataCore.Struct.StWuChaDeal();

            #region ---------- 计算头表电量 -----------
            if (_curPoint.ZouZiMethod == Cus_ZouZiMethod.基本走字法)
            {
                CLDC_Comm.Win32Api.POINT _TouBiao = GetTouBiao(ItemKey);

                _StarandMeterDl = Helper.MeterDataHelper.Instance.Meter(_TouBiao.X).MeterZZErrors[ItemKey].Mz_chrZiMa -
                                Helper.MeterDataHelper.Instance.Meter(_TouBiao.X).MeterZZErrors[ItemKey].Mz_chrQiMa;
                _StarandMeterDl += Helper.MeterDataHelper.Instance.Meter(_TouBiao.Y).MeterZZErrors[ItemKey].Mz_chrZiMa -
                                Helper.MeterDataHelper.Instance.Meter(_TouBiao.Y).MeterZZErrors[ItemKey].Mz_chrQiMa;
                _StarandMeterDl = _StarandMeterDl / 2F;
            }
            #endregion

            for (int r = 0; r < BwCount; r++)
            {
                _Meter = Helper.MeterDataHelper.Instance.Meter(r);
                if (_Meter == null || !_Meter.YaoJianYn)
                {
                    continue;
                }
                //新申明一个走字误差，用于保存计算返回结果
                MeterZZError curResult = null;
                float _MeterLevel = MeterLevel(_Meter);                   //当前表的等级
                //设置计算参数
                zzWCPata.IsBiaoZunBiao = false;
                zzWCPata.MaxError = _MeterLevel;
                CLDC_DataCore.WuChaDeal.WuChaContext m_Context = null;

                if (_ZZMethod == Cus_ZouZiMethod.计读脉冲法)
                {
                    zzWCPata.MinError = -1;
                    zzWCPata.MaxError = 1;
                    m_Context = new CLDC_DataCore.WuChaDeal.WuChaContext(CLDC_DataCore.WuChaDeal.WuChaType.走字误差之计读脉冲法, zzWCPata);
                    curResult = (MeterZZError)m_Context.GetResult(float.Parse(arrData[r]));
                }
                else
                {
                    if (_curPoint.FeiLv == Cus_FeiLv.总)
                    {
                        StPlan_WcPoint curWcPoint = new StPlan_WcPoint();
                        if (curWcPoint.LapCount == 0)
                        {
                            //通过返回点的检定圈数确定是否有此基本误差点
                            zzWCPata.MinError = -_MeterLevel * 1.0F;
                            zzWCPata.MaxError = _MeterLevel * 1.0F;
                        }
                        else
                        {
                            /*手动计算误差限*/
                            curWcPoint.SetWcx(GlobalUnit.g_CUS.DnbData.CzWcLimit,
                                                _Meter.GuiChengName, _Meter.Mb_chrBdj,
                                                _Meter.Mb_BlnHgq);
                            /*
                             2009-7-29
                             * 增加内控误差限
                             */
                            zzWCPata.MaxError = curWcPoint.ErrorShangXian * GlobalUnit.g_CUS.DnbData.WcxUpPercent;
                            zzWCPata.MinError = -(Math.Abs(curWcPoint.ErrorXiaXian * GlobalUnit.g_CUS.DnbData.WcxDownPercent));
                        }
                    }
                    else
                    {
                        zzWCPata.MinError = -2;
                        zzWCPata.MeterLevel = 2;
                    }
                    m_Context = new CLDC_DataCore.WuChaDeal.WuChaContext(CLDC_DataCore.WuChaDeal.WuChaType.走字误差之标准表法, zzWCPata);
                    //if (_ZZMethod == Cus_ZouZiMethod.标准表法)//&& 系统配置了标准表
                    //{
                    //    curResult = (MeterZZError)m_Context.GetResult(_curPoint.FeiLv == Cus_FeiLv.总 ? 1L : 0L, _ZZError.Mz_chrQiMa, _ZZError.Mz_chrZiMa, _StarandMeterDl, 0.05F);
                    //}
                    //else
                    {
                        float flt_C = 0;
                        float.TryParse(arrData[r], out flt_C);
                        flt_C = flt_C / _Meter.GetBcs()[0];
                        curResult = (MeterZZError)m_Context.GetResult(_curPoint.FeiLv == Cus_FeiLv.总 ? 1L : 0L, arrayQima[r], arrayZima[r], flt_C, 0.0F);           //要加上标准表的等级，这个地方的0应该替换掉，而且需要用系统设置的形式来配置标准表等级
                    }
                }

                if (arrayQima[r] == -1.0 || arrayZima[r] == -1.0)
                {
                    curResult.Mz_chrQiMa = arrayQima[r];
                    curResult.Mz_chrZiMa = arrayZima[r];
                    curResult.Mz_chrWc = "-999.00";
                    curResult.Mz_chrJL = "不合格";
                }
                ResultDictionary["表码差"][r] = curResult.Mz_chrQiZiMaC;
                ResultDictionary["误差"][r] = curResult.Mz_chrWc;
                ResultDictionary["结论"][r] = curResult.Mz_chrJL;
                if (ResultDictionary["结论"][r] == Variable.CTG_BuHeGe && isAllHeGe)
                {
                    isAllHeGe = false;
                }
            }
            UploadTestResult("表码差");
            UploadTestResult("误差");
            UploadTestResult("结论");
            /*
               01/22/2010 16-07-21  By Niaoked
               内容说明：增加不合格手动调整功能
           */
            //if (!isAllHeGe)
            //{
            //MessageController.Instance.AddMessage(
            //    string.Format("有电能表在{0}中不合格，请重新输入电表起码后重新计算结果后点击[录入完毕]按钮\r\n如果不需要调整，请直接点击[录入完毕]按钮", _curPoint.ToString()), false, Cus_MessageType.录入电量起码);
            //this.WaitInputDl(_curPoint.PowerFangXiang, _curPoint.FeiLv, true);
            //ControlZZResult(_curPoint, _ZZMethod, arrData, strKey);
            //}
            /* Modify End */
            Stop = true;
        }

        /// <summary>
        /// 计算总的结论
        /// </summary>
        protected void ControlResult()
        {
            Cus_MeterResultPrjID curItemID = Cus_MeterResultPrjID.走字试验;

            if (!(CurPlan is StPlan_ZouZi)) return;
            StPlan_ZouZi curPoint = (StPlan_ZouZi)CurPlan;

            //当前检定方向总结论节点名称
            string curItemKey = ((int)curItemID).ToString("D3") + ((int)PowerFangXiang).ToString();
            for (int bw = 0; bw < BwCount; bw++)
            {
                MeterBasicInfo curMeter = Helper.MeterDataHelper.Instance.Meter(bw);
                if (!curMeter.YaoJianYn) continue;
                MeterResult curResult = new MeterResult();
                if (curMeter.MeterResults.ContainsKey(curItemKey))
                    curMeter.MeterResults.Remove(curItemKey);
                curMeter.MeterResults.Add(curItemKey, curResult);
                curResult.Mr_chrRstId = curItemKey;
                curResult.Mr_chrRstName = curItemID.ToString() + PowerFangXiang.ToString();
                curResult.Mr_chrRstValue = Variable.CTG_HeGe;
                if (curMeter.MeterZZErrors[ItemKey].Mz_chrJL == Variable.CTG_BuHeGe)
                    curResult.Mr_chrRstValue = Variable.CTG_BuHeGe;
                else
                {
                    //检测当前方向下的其它点是否合格
                    if (!isTheSamePowerPDHeGe(curMeter))
                        curResult.Mr_chrRstValue = Variable.CTG_BuHeGe;
                }
            }
        }

        /// <summary>
        /// 是否相同方向下的所有当前检定项目都合格
        /// </summary>
        /// <param name="curMeter"></param>
        /// <returns></returns>
        private bool isTheSamePowerPDHeGe(MeterBasicInfo curMeter)
        {
            bool isAllItemOk = true;
            foreach (string strKey in curMeter.MeterZZErrors.Keys)
            {
                //当前功率方向
                if (curMeter.MeterZZErrors[strKey].Mz_chrJL == Variable.CTG_BuHeGe)
                {
                    Cus_PowerFangXiang thisPointFX = (Cus_PowerFangXiang)(int.Parse(curMeter.MeterZZErrors[strKey].Me_chrProjectNo.Substring(0, 1)));
                    StPlan_ZouZi curResultItem = (StPlan_ZouZi)CurPlan;
                    if (curResultItem.PowerFangXiang == thisPointFX)
                    {
                        isAllItemOk = false;
                        break;
                    }
                }
            }
            return isAllItemOk;
        }

        /// <summary>
        /// 检测方案是否合法
        /// 格式:功率方向|功率元件|功率因数|电流倍数|走字试验方法类型|费率|走字电量(度)|走字时间(分钟)
        /// 默认值:正向有功|H|1.0|3.0Ib|标准表法|总|1|0
        /// </summary>
        /// <returns></returns>
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

            bool Result = true;
            TestType1 testMethod = TestType1.默认;
            string[] powerDirect = new string[1];
            //取当前检定方案中的所有功率方向
            powerDirect[0] = ((int)((Cus_PowerFangXiang)Enum.Parse(typeof(Cus_PowerFangXiang), _Prams[0]))).ToString();
            //检测每一个检定方向下的费率
            for (int i = 0; i < powerDirect.Length; i++)
            {
                string[] feilv = new string[1];

                int zNum = 0;
                //取当前功率方向下的费率时段
                feilv[0] = ((int)((Cus_FeiLv)Enum.Parse(typeof(Cus_FeiLv), _Prams[5]))).ToString();
                //当走字方式为总与分同时做时，要求每一个方向只有一个总且第一个费率必须为总，
                if (testMethod == TestType1.总与分费率同时做)
                {
                    if (feilv[0] != ((int)Cus_FeiLv.总).ToString())
                    {
                        MessageController.Instance.AddMessage("当走字方式为[" + testMethod.ToString() + "]时，第一个走字试验方案必须为[总]", 6, 2);
                        Result = false;
                        break;
                    }

                    for (int k = 0; k < feilv.Length; k++)
                    {

                        if (feilv[k] == ((int)Cus_FeiLv.总).ToString())
                            zNum++;
                        if (zNum > 1)
                        {
                            MessageController.Instance.AddMessage("当走字方式为[" + testMethod.ToString() + "]时，每一个功率方向允许有且仅允许有一个总费率方向方案", 6, 2);
                            return false;
                        }
                    }
                }
                else if (testMethod == TestType1.自动检定总时段内的所有分费率)
                {
                    if (feilv[0] != ((int)Cus_FeiLv.总).ToString()) //第一个不为总
                    {
                        Result = false;
                        break;
                    }
                }
                else
                {
                    Result = true;
                }
            }
            StPlan_ZouZi _curPoint = (StPlan_ZouZi)CurPlan;
            if (_curPoint.ZouZiMethod == Cus_ZouZiMethod.基本走字法 && Helper.MeterDataHelper.Instance.YaoJianMeterCount < 3)
            {
                MessageController.Instance.AddMessage("基本走字法至少要求有三块以上被检表!", 6, 2);
                return false;
            }

            ResultNames = new string[] { "起码", "止码", "表码差", "表脉冲", "标准表脉冲", "误差", "结论", "不合格原因" };

            return Result;
        }

        /// <summary>
        /// 通过比较当前负载下的误差，取前二个误差最小的表作为头表
        /// </summary>
        /// <param name="ZZPrjID">走字方案PrjID</param>
        /// <returns>表一:Point.x;表二:Point.y</returns>
        private CLDC_Comm.Win32Api.POINT GetTouBiao(string ZZPrjID)
        {
            /*
             误差ID:误差类别+功率方向+元件+功率在素+电流倍数+谐波+相序
             * 走字ID：功率方向+元件+功率在素+电流倍数+费率
             */
            CLDC_Comm.Win32Api.POINT _TouBiao = new CLDC_Comm.Win32Api.POINT();
            string _Key = "1" + ZZPrjID.Substring(0, ZZPrjID.Length - 1);
            float[] wc = new float[BwCount];

            for (int k = 0; k < BwCount; k++)
            {
                //if (Helper.MeterDataHelper.Instance.Meter(k).MeterErrors.ContainsKey(_Key))
                if (!Helper.MeterDataHelper.Instance.Meter(k).YaoJianYn)
                {
                    //不检定的表放在最后
                    wc[k] = 999999 + k;
                    continue;
                }
                bool bFind = false;
                foreach (String strKey in Helper.MeterDataHelper.Instance.Meter(k).MeterErrors.Keys)
                {
                    MeterError mError = Helper.MeterDataHelper.Instance.Meter(k).MeterErrors[strKey];
                    if (mError.Me_chrProjectNo.Substring(0, _Key.Length) == _Key && mError.Me_chrWcJl == Variable.CTG_HeGe)
                    {
                        string[] _wc = mError.MeWc.Split('|');
                        //误差的后3位用来标明表位,用于数组排序后还能够正常标明表位
                        wc[k] = float.Parse(_wc[_wc.Length - 1] + (k + 1).ToString("D3"));
                        bFind = true;
                        break;
                    }
                }
                //没有找到此基本误差点
                if (!bFind)
                    wc[k] = float.Parse(999 + (k + 1).ToString("D3")); ;
            }
            //排顺序
            CLDC_DataCore.Function.Number.PopDesc(ref wc, true);
            string Meter1, Meter2;
            Meter1 = wc[0].ToString();
            Meter2 = wc[1].ToString();

            Meter1 = Meter1.Substring(Meter1.Length - 3, 3);
            Meter2 = Meter2.Substring(Meter2.Length - 3, 3);
            /*
            	10/09/2009 10-43-23  By Niaoked
            	内容说明：
            	如果二块头表的误差都大于999,则为不合表或是要检定的表不够
             * 
            */
            //if (wc[0] > 999F || wc[1] > 999F)
            // {
            //     _TouBiao.X = -1;
            //     _TouBiao.Y = -1;
            // }
            // else
            // {
            _TouBiao.X = int.Parse(Meter1) - 1;
            _TouBiao.Y = int.Parse(Meter2) - 1;
            // }
            return _TouBiao;
        }

        /// <summary>
        /// 挂检定数据节点
        /// </summary>
        /// <param name="curPoint">当前检定点方案</param>
        /// <param name="I">走字电流</param>
        /// <returns></returns>
        private void InitZZData(string I)
        {
            MeterBasicInfo curMeter = null;
            MeterZZError zzError = null;
            StPlan_ZouZi curPoint = (StPlan_ZouZi)CurPlan;
            for (int i = 0; i < BwCount; i++)
            {
                curMeter = Helper.MeterDataHelper.Instance.Meter(i);
                if (curMeter.MeterZZErrors.ContainsKey(ItemKey))
                    curMeter.MeterZZErrors.Remove(ItemKey);
                if (curMeter.YaoJianYn)
                {
                    zzError = new MeterZZError();
                    zzError.Mz_chrxIbString = curPoint.xIb;
                    zzError.Mz_chrJdfx = (Convert.ToInt32(curPoint.PowerFangXiang)).ToString();

                    switch (curPoint.FeiLv)
                    {
                        case Cus_FeiLv.尖:
                            zzError.Mz_chrFl = "尖";
                            break;
                        case Cus_FeiLv.峰:
                            zzError.Mz_chrFl = "峰";
                            break;
                        case Cus_FeiLv.平:
                            zzError.Mz_chrFl = "平";
                            break;
                        case Cus_FeiLv.谷:
                            zzError.Mz_chrFl = "谷";
                            break;
                        default:
                            zzError.Mz_chrFl = "总";
                            break;
                    }
                    zzError.Mz_chrGlys = curPoint.Glys;
                    zzError.Mz_chrxIb = I;
                    zzError.Mz_chrYj = (Convert.ToInt32(curPoint.PowerYj)).ToString();

                    zzError.Mz_chrWc = "--";
                    zzError.Mz_chrJL = Variable.CTG_DEFAULTRESULT;
                    zzError.AVR_STANDARD_METER_ENERGY = curPoint.UseMinutes.ToString();
                    zzError.Me_chrProjectNo = curPoint.PrjID;
                    zzError.Mz_chrStartTime = curPoint.StartTime;
                    curMeter.MeterZZErrors.Add(ItemKey, zzError);
                }
            }
            MessageController.Instance.AddMessage("清理上一次检定数据完毕...");
        }

        #endregion

      
    }
}
