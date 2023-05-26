using System;
using System.Collections.Generic;
using System.Text;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;
using CLDC_DataCore.Struct;
using CLDC_DataCore;
namespace CLDC_VerifyAdapter
{
    /// <summary>
    /// 负荷记录
    /// </summary>
    class LoadRecordVerify : VerifyBase
    {
        private StPlan_LoadRecord _CurrentPlan = new StPlan_LoadRecord();
        private StRunningE _stRunE = new StRunningE();

        public LoadRecordVerify(object plan)
            : base(plan) 
        {
            _CurrentPlan.danWei = "分钟";
            _CurrentPlan.MarginTime = 1;
            if (GlobalUnit.Clfs == Cus_Clfs.单相)
            {
                _CurrentPlan.ModeByte = "00011111";
            }
            else
            {
                _CurrentPlan.ModeByte = "00111111";
            }
            _CurrentPlan.OverTime = 2;

            _stRunE.PowerFX = Cus_PowerFangXiang.正向有功;
            _stRunE.RunningTime ="1";
            _stRunE.Glys="1.0";
            _stRunE.xIB = "Imax";

        }

        private MeterLoadRecord _Result = null;
        private MeterBasicInfo curMeter;

        /// <summary>
        /// 事件记录项目ID
        /// </summary>
        protected override string ItemKey
        {
            get
            {
                return "001";
            }
        }

        /// <summary>
        /// 结论数据结点
        /// </summary>
        protected override string ResultKey
        {
            get
            {
                return string.Format("{0}", (int)CLDC_Comm.Enum.Cus_MeterResultPrjID.负荷记录试验);
            }
        }

        protected override bool CheckPara()
        {
            ResultNames = new string[] { "第一类（电压、电流、记录时间）","第二类（有、无功率、记录时间）","第三类（功率因数、记录时间）",
                                        "第四类（有、无功总电能、记录时间）","第五类（四象限无功总电能、记录时间）","第六类（有、无功需量、记录时间）",
                                         "结论"};
            return true;
        }


        private DateTime m_dtStartTime;
        /// <summary>
        /// 重写基类测试方法
        /// </summary>
        /// <param name="ItemNumber">检定方案序号</param>
        public override void Verify()
        {

            base.Verify();
            if (Stop) return;                   //假如当前停止检定，则退出
           
            //初始化设备
            if (!InitEquipment())
            {
                return;
            }

            if (Stop) return;                   //假如当前停止检定，则退出

            bool bWriteMeter = false;
            bool[] arrWriteMeter = new bool[BwCount];
            string[] arrInterval1 = new string[BwCount];
            string[] arrInterval2 = new string[BwCount];
            string[] arrInterval3 = new string[BwCount];
            string[] arrInterval4 = new string[BwCount];
            string[] arrInterval5 = new string[BwCount];
            string[] arrInterval6 = new string[BwCount];
            string[] arrPattern = new string[BwCount];
            string[] arrStartTime = new string[BwCount];
            bool[,] blnRet = new bool[BwCount, 6];
                      
                        
            string strPattern = "";
            int intSumTestNum = 1;
            int intCurrentTestNum = 1;                 
            int TotalItem=0;
            string strCurItem = "";

            bool[] arrResultInfo = null;
            bool[] arrResultInfoTotal = new bool[BwCount];      

            string[] strResultKey = new string[BwCount];
            object[] objResultValue = new object[BwCount];


            if (Stop) return;                   //假如当前停止检定，则退出
            bool[] arrResult = new bool[BwCount];
            bool bResult;

            for (int i = 0; i < BwCount; i++)
                arrResult[i] = true;

            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();

            strPattern = Convert.ToString(Convert.ToInt32(_CurrentPlan.ModeByte, 2), 16);

            intSumTestNum = _CurrentPlan.OverTime / _CurrentPlan.MarginTime;            
            
          //  Identity();
            if (Stop) return;
            MessageController.Instance.AddMessage("开始恢复表时间......");

            DateTime readTime = DateTime.Now;
          //  if (GlobalUnit.g_SystemConfig.methodAndBasis.getValue(Variable.CTC_TM_GPSGETT) != "取电脑时间")
          //  {
                MessageController.Instance.AddMessage("开始读取GPS时间...");
                readTime = Helper.EquipHelper.Instance.ReadGpsTime();  //读取GPS时间
         //   }

            bool[] result = MeterProtocolAdapter.Instance.WriteDateTime(readTime.ToString("yyMMddHHmmss"));


            MessageController.Instance.AddMessage("读取负荷记录模式字");
            arrPattern = MeterProtocolAdapter.Instance.ReadData("04000901", 1);

            //if (Stop) return;
            //MessageController.Instance.AddMessage("读取负荷记录起始时间");
            //arrStartTime = MeterProtocolAdapter.Instance.ReadData("04000A01", 4);

            if (Stop) return;
            if (_CurrentPlan.ModeByte[7] == '1')
            {
                TotalItem++;
                MessageController.Instance.AddMessage("读取第1类负荷记录间隔时间");
                arrInterval1 = MeterProtocolAdapter.Instance.ReadData("04000A02", 2);
            }
            if (Stop) return;
            if (_CurrentPlan.ModeByte[6] == '1')
            {
                TotalItem++;

                MessageController.Instance.AddMessage("读取第2类负荷记录间隔时间");
                arrInterval2 = MeterProtocolAdapter.Instance.ReadData("04000A03", 2);
            }
            if (Stop) return;
            if (_CurrentPlan.ModeByte[5] == '1')
            {
                TotalItem++;

                MessageController.Instance.AddMessage("读取第3类负荷记录间隔时间");
                arrInterval3 = MeterProtocolAdapter.Instance.ReadData("04000A04", 2);
            }
            if (Stop) return;
            if (_CurrentPlan.ModeByte[4] == '1')
            {
                TotalItem++;

                MessageController.Instance.AddMessage("读取第4类负荷记录间隔时间");
                arrInterval4 = MeterProtocolAdapter.Instance.ReadData("04000A05", 2);
            }
            if (Stop) return;
            if (_CurrentPlan.ModeByte[3] == '1')
            {
                TotalItem++;

                MessageController.Instance.AddMessage("读取第5类负荷记录间隔时间");
                arrInterval5 = MeterProtocolAdapter.Instance.ReadData("04000A06", 2);
            }
            if (Stop) return;
            if (_CurrentPlan.ModeByte[2] == '1')
            {
                TotalItem++;

                MessageController.Instance.AddMessage("读取第6类负荷记录间隔时间");
                arrInterval6 = MeterProtocolAdapter.Instance.ReadData("04000A07", 2);
            }
            if (Stop) return; 
            //读取表号
            
       //     Identity();
            if (Stop) return;
            MessageController.Instance.AddMessage("设置负荷记录模式字");
            arrWriteMeter = MeterProtocolAdapter.Instance.WriteData("04000901", 1, strPattern.PadLeft(2,'0'));
            if (Stop) return;
            if (_CurrentPlan.ModeByte[7] == '1')
            {
           //     Identity();
                if (Stop) return;
                MessageController.Instance.AddMessage("设置第1类负荷记录间隔时间");
                arrWriteMeter = MeterProtocolAdapter.Instance.WriteData("04000A02", 2, _CurrentPlan.MarginTime.ToString("D4"));

                //CLDC_DataCore.Const.GlobalUnit.g_MsgControl.OutMessage("读取第1类负荷记录间隔时间");
                //arrInterval1 = MeterProtocolAdapter.Instance.ReadData("04000A02", 2);
            }
            if (Stop) return;
            if (_CurrentPlan.ModeByte[6] == '1')
            {
          //      Identity();
                if (Stop) return;
                MessageController.Instance.AddMessage("设置第2类负荷记录间隔时间");
                arrWriteMeter = MeterProtocolAdapter.Instance.WriteData("04000A03", 2, _CurrentPlan.MarginTime.ToString("D4"));
            }
            if (Stop) return;
            if (_CurrentPlan.ModeByte[5] == '1')
            {
           //     Identity();
                if (Stop) return;
                MessageController.Instance.AddMessage("设置第3类负荷记录间隔时间");
                arrWriteMeter = MeterProtocolAdapter.Instance.WriteData("04000A04", 2, _CurrentPlan.MarginTime.ToString("D4"));
            }
            if (Stop) return;
            if (_CurrentPlan.ModeByte[4] == '1')
            {
          //      Identity();
                if (Stop) return;
                MessageController.Instance.AddMessage("设置第4类负荷记录间隔时间");
                arrWriteMeter = MeterProtocolAdapter.Instance.WriteData("04000A05", 2, _CurrentPlan.MarginTime.ToString("D4"));
            }
            if (Stop) return;
            if (_CurrentPlan.ModeByte[3] == '1')
            {
            //    Identity();
                if (Stop) return;
                MessageController.Instance.AddMessage("设置第5类负荷记录间隔时间");
                arrWriteMeter = MeterProtocolAdapter.Instance.WriteData("04000A06", 2, _CurrentPlan.MarginTime.ToString("D4"));
            }
            if (Stop) return;
            if (_CurrentPlan.ModeByte[2] == '1')
            {
           //     Identity();
                if (Stop) return;
                MessageController.Instance.AddMessage("设置第6类负荷记录间隔时间");
                arrWriteMeter = MeterProtocolAdapter.Instance.WriteData("04000A07", 2, _CurrentPlan.MarginTime.ToString("D4"));

                //string[] setsix = MeterProtocolAdapter.Instance.ReadData("04000103", 2);
                arrWriteMeter = MeterProtocolAdapter.Instance.WriteData("04000103", 2, _CurrentPlan.MarginTime.ToString("D2"));
            }
            if (Stop) return;

            //for (intCurrentTestNum = 1; intCurrentTestNum <= intSumTestNum; intCurrentTestNum++)
            {
                if (Stop) return;

                //try
                //{
                //    if (_CurrentPlan.RunningEPrj.Count == TotalItem)
                //    {
                //        bResult = Walk(_CurrentPlan.RunningEPrj[intCurrentTestNum - 1]);

                //    }
                //    else
                //    {
                //        bResult = Walk(_CurrentPlan.RunningEPrj[0]);

                //    }
                //}
                //catch
                //{
                //    bResult = Walk(_CurrentPlan.RunningEPrj[0]);
                //}
                bResult = Walk(_stRunE);
                if (Stop) return;
                int checktime = 0;
                for (int intInc = 1; intInc <= 8; intInc++)
                {
                    if (Stop) return;
                    if (_CurrentPlan.ModeByte[8 - intInc] == '0') continue;
                    //    string loadrecord_type = GlobalUnit.GetConfig(Variable.LOADRECORD_TYPE, "07协议");
                    //    if (loadrecord_type == "07协议")
                    //   {
                    arrResultInfo = ReadLoadRecordInfo(intInc, intCurrentTestNum);
                    //    }
                    //    else
                    //    {
                    //        arrResultInfo = ReadLoadRecordInfoType2(intInc, intCurrentTestNum);

                    ///}
                    if (checktime == 0)
                    {
                        arrResultInfoTotal = arrResultInfo;
                        checktime++;
                    }
                    else
                    {
                        for (int i = 0; i < arrResultInfo.Length; i++)
                        {
                            arrResultInfoTotal[i] = arrResultInfoTotal[i] && arrResultInfo[i];
                        }
                    }
                }


                if (Stop) return;
                //    Identity();
                if (Stop) return;
                MessageController.Instance.AddMessage("恢复负荷记录模式字");
                arrWriteMeter = MeterProtocolAdapter.Instance.WriteArrData("04000901", 1, arrPattern);
                if (Stop) return;
                //    Identity();
                //if (Stop) return;
                //MessageController.Instance.AddMessage("恢复负荷记录起始时间");
                //arrWriteMeter = MeterProtocolAdapter.Instance.WriteArrData("04000A01", 4, arrStartTime);

                if (Stop) return;
                if (_CurrentPlan.ModeByte[7] == '1')
                {
                    //        Identity();
                    if (Stop) return;
                    MessageController.Instance.AddMessage("恢复第1类负荷记录间隔时间");
                    arrWriteMeter = MeterProtocolAdapter.Instance.WriteArrData("04000A02", 2, arrInterval1);
                }
                if (Stop) return;
                if (_CurrentPlan.ModeByte[6] == '1')
                {
                    //       Identity();
                    if (Stop) return;
                    MessageController.Instance.AddMessage("恢复第2类负荷记录间隔时间");
                    arrWriteMeter = MeterProtocolAdapter.Instance.WriteArrData("04000A03", 2, arrInterval2);
                }
                if (Stop) return;
                if (_CurrentPlan.ModeByte[5] == '1')
                {
                    //      Identity();
                    if (Stop) return;
                    MessageController.Instance.AddMessage("恢复第3类负荷记录间隔时间");
                    arrWriteMeter = MeterProtocolAdapter.Instance.WriteArrData("04000A04", 2, arrInterval3);
                }
                if (Stop) return;
                if (_CurrentPlan.ModeByte[4] == '1')
                {
                    //      Identity();
                    if (Stop) return;
                    MessageController.Instance.AddMessage("恢复第4类负荷记录间隔时间");
                    arrWriteMeter = MeterProtocolAdapter.Instance.WriteArrData("04000A05", 2, arrInterval4);
                }
                if (Stop) return;
                if (_CurrentPlan.ModeByte[3] == '1')
                {
                    //    Identity();
                    if (Stop) return;
                    MessageController.Instance.AddMessage("恢复第5类负荷记录间隔时间");
                    arrWriteMeter = MeterProtocolAdapter.Instance.WriteArrData("04000A06", 2, arrInterval5);
                }
                if (Stop) return;
                if (_CurrentPlan.ModeByte[2] == '1')
                {
                    //       Identity();
                    if (Stop) return;
                    MessageController.Instance.AddMessage("恢复第6类负荷记录间隔时间");
                    arrWriteMeter = MeterProtocolAdapter.Instance.WriteArrData("04000A07", 2, arrInterval6);
                    MeterProtocolAdapter.Instance.WriteData("04000103", 2, "15");
                }

                if (Stop) return;
                for (int i = 0; i < BwCount; i++)
                {
                    if (Stop) return;                   //假如当前停止检定，则退出
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    curMeter = Helper.MeterDataHelper.Instance.Meter(i);

                    if (curMeter.MeterLoadRecords.ContainsKey("0010101") )
                    {
                        ResultDictionary["第一类（电压、电流、记录时间）"][i] = curMeter.MeterLoadRecords["0010101"].Ml_chrValue;
                        blnRet[i, 0] = curMeter.MeterLoadRecords["0010101"].Ml_chrValue.Contains("不合格") ? false : true;
                    }
                    if (curMeter.MeterLoadRecords.ContainsKey("0010201"))
                    {
                        ResultDictionary["第二类（有、无功率、记录时间）"][i] = curMeter.MeterLoadRecords["0010201"].Ml_chrValue;
                        blnRet[i, 1] = curMeter.MeterLoadRecords["0010201"].Ml_chrValue.Contains("不合格") ? false : true;
                    }
                    if (curMeter.MeterLoadRecords.ContainsKey("0010301"))
                    {
                        ResultDictionary["第三类（功率因数、记录时间）"][i] = curMeter.MeterLoadRecords["0010301"].Ml_chrValue;
                        blnRet[i, 2] = curMeter.MeterLoadRecords["0010301"].Ml_chrValue.Contains("不合格") ? false : true;
                    }
                    if (curMeter.MeterLoadRecords.ContainsKey("0010401"))
                    {
                        ResultDictionary["第四类（有、无功总电能、记录时间）"][i] = curMeter.MeterLoadRecords["0010401"].Ml_chrValue;
                        blnRet[i, 3] = curMeter.MeterLoadRecords["0010401"].Ml_chrValue.Contains("不合格") ? false : true;
                    }
                    if (curMeter.MeterLoadRecords.ContainsKey("0010501"))
                    {
                        ResultDictionary["第五类（四象限无功总电能、记录时间）"][i] = curMeter.MeterLoadRecords["0010501"].Ml_chrValue;
                        blnRet[i, 4] = curMeter.MeterLoadRecords["0010501"].Ml_chrValue.Contains("不合格") ? false : true;
                    }
                    if (curMeter.MeterLoadRecords.ContainsKey("0010601"))
                    {
                        ResultDictionary["第六类（有、无功需量、记录时间）"][i] = curMeter.MeterLoadRecords["0010601"].Ml_chrValue;
                        blnRet[i, 5] = curMeter.MeterLoadRecords["0010601"].Ml_chrValue.Contains("不合格") ? false : true;
                    }

                    ResultDictionary["结论"][i] = "不合格";

                    if (GlobalUnit.Clfs == Cus_Clfs.单相)
                    {
                        if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3] && blnRet[i, 4])
                        {
                            ResultDictionary["结论"][i] = "合格";
                        }
                    }
                    else
                    {
                        if (blnRet[i, 0] && blnRet[i, 1] && blnRet[i, 2] && blnRet[i, 3] && blnRet[i, 4] && blnRet[i, 5])
                        {
                            ResultDictionary["结论"][i] = "合格";
                        }
                    }

                    UploadTestResult("第一类（电压、电流、记录时间）");
                    UploadTestResult("第二类（有、无功率、记录时间）");
                    UploadTestResult("第三类（功率因数、记录时间）");
                    UploadTestResult("第四类（有、无功总电能、记录时间）");
                    UploadTestResult("第五类（四象限无功总电能、记录时间）");
                    UploadTestResult("第六类（有、无功需量、记录时间）");
                    UploadTestResult("结论");

                }
            }
        }

        
        

        private bool Walk(CLDC_DataCore.Struct.StRunningE walk)
        {
            string strGlys = "";
            bool[] arrWriteMeter = new bool[BwCount];

            switch (walk.PowerFX)
            {
                case Cus_PowerFangXiang.正向有功:
                    PowerFangXiang = Cus_PowerFangXiang.正向有功;
                    strGlys = "1.0";
                    break;
                case Cus_PowerFangXiang.反向有功:
                    PowerFangXiang = Cus_PowerFangXiang.反向有功;
                    strGlys = "-1.0";
                    break;
                case Cus_PowerFangXiang.第一象限无功:
                    PowerFangXiang = Cus_PowerFangXiang.正向无功;
                    strGlys = "0.5L";
                    break;
                case Cus_PowerFangXiang.第二象限无功:
                    PowerFangXiang = Cus_PowerFangXiang.正向无功;
                    strGlys = "0.8C";
                    break;
                case Cus_PowerFangXiang.第三象限无功:
                    PowerFangXiang = Cus_PowerFangXiang.反向无功;
                    strGlys = "-0.8C";
                    break;
                case Cus_PowerFangXiang.第四象限无功:
                    PowerFangXiang = Cus_PowerFangXiang.反向无功;
                    strGlys = "-0.5L";
                    break;
            }
            if (Helper.EquipHelper.Instance.PowerOn(CLDC_DataCore.Const.GlobalUnit.U, CLDC_DataCore.Const.GlobalUnit.Ib, (int)Cus_PowerYuanJian.H, (int)PowerFangXiang, strGlys, true, false) == false)
            {
                MessageController.Instance.AddMessage("控制源输出失败");
                return false;
            }
            System.Threading.Thread.Sleep(10000);

            m_dtStartTime = DateTime.Now.AddMinutes(int.Parse(_CurrentPlan.MarginTime.ToString("D4")));
            string strStartTime = m_dtStartTime.ToString("MMddHHmm");
         //   Identity();
            //if (Stop) return true;
            //MessageController.Instance.AddMessage("设置负荷记录起始时间");
            //arrWriteMeter = MeterProtocolAdapter.Instance.WriteData("04000A01", 4, strStartTime);
            //string[] startTim = MeterProtocolAdapter.Instance.ReadData("04000A01",4);
            int _MaxStartTime = int.Parse(walk.RunningTime) * 60+5;
            m_StartTime = DateTime.Now;
            while (true)
            {
                //每一秒刷新一次数据
                long _PastTime = base.VerifyPassTime;
                System.Threading.Thread.Sleep(1000);

                float pastMinute = _PastTime / 60F;
                CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData.NowMinute = pastMinute;
                string strDes = string.Format("{0}运行时间", walk.PowerFX) + (_MaxStartTime / 60.0f).ToString("F2") + "分，已经经过" + pastMinute.ToString("F2") + "分";

                MessageController.Instance.AddMessage(strDes);

                if ((_PastTime >= _MaxStartTime) || Stop)
                {
                    CLDC_DataCore.Const.GlobalUnit.g_CUS.DnbData.NowMinute = _MaxStartTime / 60F;
                    break;
                }
                if (Stop) break;
            }
            if (Stop) return true;
            //Helper.EquipHelper.Instance.PowerOn(CLDC_DataCore.Const.GlobalUnit.U, Cus_PowerFangXiang.正向有功);
            System.Threading.Thread.Sleep(300);
            return true;
        }

        private bool[] ReadLoadRecordInfo(int intType, int intTestNum)
        {
            bool bReadTwo = false;
            bool[] bResult = new bool[BwCount];
            int intDotLen = 0;
            int intDotLen1 = 0;
            int intRecordLen = 0;            
            int intRecordLen1 = 0;
            int intCount = 0;
            string strDataFlag = "";
            string strDataFlag1 = "";            
            string strCurItem = "";
            string strRecord = "";
            string strRecord1 = "";
            string strMessage = "";            
            string[] arrRecordLoad = new string[BwCount];
            string[] arrRecordLoad1 = new string[BwCount];
            string strRecordTime = "";
            CLDC_Comm.Enum.Cus_Clfs Clfs = (Cus_Clfs)Helper.MeterDataHelper.Instance.Meter(Helper.MeterDataHelper.Instance.FirstYaoJianMeter).Mb_intClfs;
            switch (intType)
            {
                case 1: //第1类符合记录(电压、电流)数据块的长度
                    intRecordLen = 6;
                    intDotLen = 1;
                    intRecordLen1 = 9;
                    intDotLen1 = 3;
                    intCount = 3;
                    bReadTwo = true;
                    //strDataFlag = "061001FF";
                    //strDataFlag1 = "061002FF";
                    strDataFlag = "06010001";
                    strDataFlag1 = "061002FF";
                    if (Clfs == Cus_Clfs.单相)
                    {
                        intRecordLen = 2;
                        intRecordLen1 = 3;
                        intCount = 1;
                        //strDataFlag = "06010002";
                        //strDataFlag1 = "06100201";
                        strDataFlag = "06100101";
                        strDataFlag1 = "06100201";
                    }
                    break;
                case 2://第2类符合记录(有功功率、无功功率)数据块的长度
                    intRecordLen = 12;
                    intRecordLen1 = 12;
                    intDotLen = 4;
                    intDotLen1 = 4;
                    intCount = 4;
                    bReadTwo = true;
                    //strDataFlag = "061003FF";
                    //strDataFlag1 = "061004FF";
                    strDataFlag = "06020001";
                    strDataFlag1 = "061004FF";
                    if (Clfs == Cus_Clfs.单相)
                    {
                        intRecordLen = 3;
                        intCount = 1;
                        //strDataFlag = "06100300";
                        strDataFlag = "06100300";
                    
                        bReadTwo = false;
                    }
                    break;
                case 3://第3类符合记录(功率因数)数据块的长度
                    intRecordLen = 8;
                    intDotLen = 3;
                    intCount = 4;
                    //strDataFlag = "061005FF";
                    strDataFlag = "06030001";
                    if (Clfs == Cus_Clfs.单相)
                    {
                        intRecordLen = 2;
                        intCount = 1;
                        strDataFlag = "06100500";                        
                    }
                    break;
                case 4://第4类符合记录(有功无功电能)数据块的长度
                    intRecordLen = 16;
                    intDotLen = 2;
                    intCount = 4;
                    //strDataFlag = "061006FF";
                    strDataFlag = "06040001";

                    if (Clfs == Cus_Clfs.单相)
                    {
                        intRecordLen = 4;
                        intRecordLen1 = 4;
                        intDotLen1 = 2;
                        intCount = 1;
                        strDataFlag = "06100601";
                        strDataFlag1 = "06100602";
                        bReadTwo = true;
                       
                    }
                    break;
                case 5://第5类符合记录(无功四象限电能)数据块的长度
                    intRecordLen = 16;
                    intDotLen = 2;
                    intCount = 4;
                    strDataFlag = "06100701";
                    //strDataFlag = "06050001";
                    break;
                case 6://第6类符合记录(需量)数据块的长度
                    intRecordLen = 6;
                    intDotLen = 4;
                    intCount = 2;
                    //strDataFlag = "061008FF";
                    strDataFlag = "06060001";
                    break;                
            }
            
            if (Stop) return null;

            for (int intInc = 0; intInc < BwCount; intInc++)
                bResult[intInc] = true;

            if (Stop) return null;
            if (intTestNum % 10 == 0)
                strCurItem = ItemKey + intType.ToString("D2") + "10";
            else
                strCurItem = ItemKey + intType.ToString("D2") + (intTestNum % 10).ToString("D2");
            if (Stop) return null;
            //strMessage = string.Format("第【{0}】次读取第【{1}】类负荷记录", intTestNum.ToString("D2"), intType.ToString("D2"));
            strMessage = string.Format("读取第【{1}】类负荷记录", intTestNum.ToString("D2"), intType.ToString("D2"));
            MessageController.Instance.AddMessage(strMessage);

            if (Stop) return null;
            strRecord = m_dtStartTime.ToString("yyMMddHHmm") + "01";
            string[] tempvalue = MeterProtocolAdapter.Instance.ReadData("02800004", 6);
            string[] tempvalue2 = MeterProtocolAdapter.Instance.ReadData("02800005", 6);
            //arrRecordLoad = MeterProtocolAdapter.Instance.ReadLoadRecord(strDataFlag, 16, strRecord);
            arrRecordLoad = MeterProtocolAdapter.Instance.ReadLoadRecord(strDataFlag, 16, strRecord);

            if (Stop) return null;
            if (bReadTwo && Clfs == Cus_Clfs.单相)
                arrRecordLoad1 = MeterProtocolAdapter.Instance.ReadLoadRecord(strDataFlag1, 16, strRecord);
            if (Stop) return null;

            for (int j = 0; j < BwCount; j++)
            {                
                strRecord = "";
                curMeter = Helper.MeterDataHelper.Instance.Meter(j);
                if (!curMeter.YaoJianYn) continue;
                //结论
                if (!curMeter.MeterLoadRecords.ContainsKey(strCurItem))
                {
                    _Result = new CLDC_DataCore.Model.DnbModel.DnbInfo.MeterLoadRecord();                    
                    _Result.Ml_PrjID = strCurItem;
                    curMeter.MeterLoadRecords.Add(strCurItem, _Result);
                }
                else
                {
                    _Result = curMeter.MeterLoadRecords[strCurItem];
                }
                if (intTestNum % 10 == 0)
                    _Result.Ml_SubItemName = string.Format("第【{0}】次第【{1}】类负荷记录", "10", intType.ToString("D2"));
                else
                    _Result.Ml_SubItemName = string.Format("第【{0}】次第【{1}】类负荷记录", (intTestNum % 10).ToString("D2"), intType.ToString("D2"));

                #region 反转返回数据
                string CheckTime = "";
                string MyAnalyze="";

                if (Clfs!= Cus_Clfs.单相)
                {
                    MyAnalyze = ExAnalyze(arrRecordLoad[j], intType, ref CheckTime);
                }

                #endregion
               
               arrRecordLoad[j] = arrRecordLoad[j].PadLeft(intRecordLen * 2 + 10, '0');
                strRecordTime = arrRecordLoad[j].Substring(arrRecordLoad[j].Length - 10, 10);

                arrRecordLoad[j] = arrRecordLoad[j].Replace(strRecordTime, "");

                strRecord = "";
                strRecord1 = "";
             
                if (Clfs == Cus_Clfs.单相)
                {  
                    #region 新协议解析
                    for (int intInc = 0; intInc < intCount; intInc++)
                    {
                        int intLen = intRecordLen / intCount * 2;
                        string strDot = string.Format("F{0}", intDotLen);
                        if (arrRecordLoad[j].Length >= (intInc + 1) * intLen)
                            try
                            {
                                if (intType == 6)
                                {
                                    if (intInc == 1)
                                    {
                                        strRecord += (float.Parse(tempvalue2[j].Substring(0 * intLen, intLen)) / (Math.Pow(10, intDotLen))).ToString(strDot) + "|";

                                    }
                                    else
                                    {
                                        strRecord += (float.Parse(tempvalue[j].Substring(intInc * intLen, intLen)) / (Math.Pow(10, intDotLen))).ToString(strDot) + "|";
                                    }
                                }
                                else
                                {
                                    strRecord += (float.Parse(arrRecordLoad[j].Substring(intInc * intLen, intLen)) / (Math.Pow(10, intDotLen))).ToString(strDot) + "|";
                                }
                            }
                            catch (Exception ex)
                            {
                                strRecord += arrRecordLoad[j].Substring(intInc * intLen, intLen) + "|";
                            }
                        else
                            strRecord += "|";

                        if (bReadTwo)
                        {
                            intLen = intRecordLen1 / intCount * 2;
                            strDot = string.Format("F{0}", intDotLen1);
                            if (arrRecordLoad1[j].Length >= (intInc + 1) * intLen)
                                try
                                {
                                    strRecord1 += (float.Parse(arrRecordLoad1[j].Substring(intInc * intLen, intLen)) / (Math.Pow(10, intDotLen1))).ToString(strDot) + "|";
                                }
                                catch (Exception ex)
                                {
                                    strRecord1 += arrRecordLoad1[j].Substring(intInc * intLen, intLen) + "|";
                                }
                            else
                                strRecord1 += "|";

                        }
                    }


                    if (bReadTwo)
                    {
                        strRecord1 = strRecord1.Trim('|');
                        strRecord += strRecord1 + "," + strRecordTime;
                    }
                    else
                    {
                        strRecord = strRecord.Trim('|');
                        strRecord += "," + strRecordTime;
                    }

                    _Result.Ml_chrValue = strRecord;
                      #endregion
                }
                else
                {
                    _Result.Ml_chrValue = MyAnalyze + "," + CheckTime;
                    strRecordTime = CheckTime;
                }
             
              
                
             
                if (strRecord.Contains("FF"))
                {
                    bResult[j] = false;
                    _Result.Ml_chrValue = _Result.Ml_chrValue + "|不合格";
                }
                if (strRecordTime != m_dtStartTime.AddMinutes(-1).ToString("yyMMddHHmm") &&
                    strRecordTime != m_dtStartTime.AddMinutes(1).ToString("yyMMddHHmm") &&
                    strRecordTime != m_dtStartTime.ToString("yyMMddHHmm"))
                {
                    bResult[j] = false;
                    _Result.Ml_chrValue = _Result.Ml_chrValue + "|不合格|" + strRecordTime;
                }
                     
            }

            return bResult;
        }
        private bool[] ReadLoadRecordInfoType2(int intType, int intTestNum)
        {
            bool bReadTwo = false;
            bool[] bResult = new bool[BwCount];
            int intDotLen = 0;
            int intDotLen1 = 0;
            int intRecordLen = 0;
            int intRecordLen1 = 0;
            int intCount = 0;
            string strDataFlag = "";
            string strDataFlag1 = "";
            string strCurItem = "";
            string strRecord = "";
            string strRecord1 = "";
            string strMessage = "";
            string[] arrRecordLoad = new string[BwCount];
            string[] arrRecordLoad1 = new string[BwCount];
            string strRecordTime = "";
            CLDC_Comm.Enum.Cus_Clfs Clfs = (Cus_Clfs)Helper.MeterDataHelper.Instance.Meter(Helper.MeterDataHelper.Instance.FirstYaoJianMeter).Mb_intClfs;
            switch (intType)
            {
                case 1: //第1类符合记录(电压、电流)数据块的长度
                    intRecordLen = 6;
                    intDotLen = 1;
                    intRecordLen1 = 9;
                    intDotLen1 = 3;
                    intCount = 3;
                    bReadTwo = true;
                    strDataFlag = "061001FF";
                    strDataFlag1 = "061002FF";
                    if (Clfs == Cus_Clfs.单相)
                    {
                        intRecordLen = 2;
                        intRecordLen1 = 3;
                        intCount = 1;
                        strDataFlag = "06100101";
                        strDataFlag1 = "06100201";
                    }
                    break;
                case 2://第2类符合记录(有功功率、无功功率)数据块的长度
                    intRecordLen = 12;
                    intRecordLen1 = 12;
                    intDotLen = 4;
                    intDotLen1 = 4;
                    intCount = 4;
                    bReadTwo = true;
                    strDataFlag = "061003FF";
                    strDataFlag1 = "061004FF";
                    if (Clfs == Cus_Clfs.单相)
                    {
                        intRecordLen = 3;
                        intCount = 1;
                        strDataFlag = "06100300";
                        bReadTwo = false;
                    }
                    break;
                case 3://第3类符合记录(功率因数)数据块的长度
                    intRecordLen = 8;
                    intDotLen = 3;
                    intCount = 4;
                    strDataFlag = "061005FF";
                    if (Clfs == Cus_Clfs.单相)
                    {
                        intRecordLen = 2;
                        intCount = 1;
                        strDataFlag = "06100500";
                    }
                    break;
                case 4://第4类符合记录(有功无功电能)数据块的长度
                    intRecordLen = 16;
                    intDotLen = 2;
                    intCount = 4;
                    strDataFlag = "061006FF";
                    if (Clfs == Cus_Clfs.单相)
                    {
                        intRecordLen = 4;
                        intRecordLen1 = 4;
                        intDotLen1 = 2;
                        intCount = 1;
                        strDataFlag = "06100601";
                        strDataFlag1 = "06100602";
                        bReadTwo = true;
                    }
                    break;
                case 5://第5类符合记录(无功四象限电能)数据块的长度
                    intRecordLen = 16;
                    intDotLen = 2;
                    intCount = 4;
                    strDataFlag = "061007FF";
                    break;
                case 6://第6类符合记录(需量)数据块的长度
                    intRecordLen = 6;
                    intDotLen = 4;
                    intCount = 2;
                    strDataFlag = "061008FF";
                    break;
            }

            if (Stop) return null;

            for (int intInc = 0; intInc < BwCount; intInc++)
                bResult[intInc] = true;

            if (Stop) return null;
            if (intTestNum % 10 == 0)
                strCurItem = ItemKey + intType.ToString("D2") + "10";
            else
                strCurItem = ItemKey + intType.ToString("D2") + (intTestNum % 10).ToString("D2");
            if (Stop) return null;
            strMessage = string.Format("第【{0}】次读取第【{1}】类负荷记录", intTestNum.ToString("D2"), intType.ToString("D2"));
            MessageController.Instance.AddMessage(strMessage);

            if (Stop) return null;
            strRecord = m_dtStartTime.ToString("yyMMddHHmm") + "01";
            string[] tempvalue = MeterProtocolAdapter.Instance.ReadData("02800004", 6);
            string[] tempvalue2 = MeterProtocolAdapter.Instance.ReadData("02800005", 6);
            arrRecordLoad = MeterProtocolAdapter.Instance.ReadLoadRecord(strDataFlag, 16, strRecord);

            if (Stop) return null;
            if (bReadTwo)
                arrRecordLoad1 = MeterProtocolAdapter.Instance.ReadLoadRecord(strDataFlag1, 16, strRecord);

            if (Stop) return null;

            for (int j = 0; j < BwCount; j++)
            {
                strRecord = "";
                curMeter = Helper.MeterDataHelper.Instance.Meter(j);
                if (!curMeter.YaoJianYn) continue;
                //结论
                if (!curMeter.MeterLoadRecords.ContainsKey(strCurItem))
                {
                    _Result = new CLDC_DataCore.Model.DnbModel.DnbInfo.MeterLoadRecord();
                    _Result.Ml_PrjID = strCurItem;
                    curMeter.MeterLoadRecords.Add(strCurItem, _Result);
                }
                else
                {
                    _Result = curMeter.MeterLoadRecords[strCurItem];
                }
                if (intTestNum % 10 == 0)
                    _Result.Ml_SubItemName = string.Format("第【{0}】次第【{1}】类负荷记录", "10", intType.ToString("D2"));
                else
                    _Result.Ml_SubItemName = string.Format("第【{0}】次第【{1}】类负荷记录", (intTestNum % 10).ToString("D2"), intType.ToString("D2"));

                arrRecordLoad[j] = arrRecordLoad[j].PadLeft(intRecordLen * 2 + 10, '0');
                strRecordTime = arrRecordLoad[j].Substring(arrRecordLoad[j].Length - 10, 10);

                arrRecordLoad[j] = arrRecordLoad[j].Replace(strRecordTime, "");

                strRecord = "";
                strRecord1 = "";
                for (int intInc = 0; intInc < intCount; intInc++)
                {
                    int intLen = intRecordLen / intCount * 2;
                    string strDot = string.Format("F{0}", intDotLen);
                    if (arrRecordLoad[j].Length >= (intInc + 1) * intLen)
                        try
                        {
                            string ExValue="";
                           
                            if (intType == 6)
                            {
                                if (intInc == 1)
                                {
                                    ExValue = (float.Parse(tempvalue2[j].Substring(0 * intLen, intLen)) / (Math.Pow(10, intDotLen))).ToString(strDot);
                                    ExValue = ChangeFirstNumber(ExValue);
                                    strRecord += ExValue + ",";

                                }
                                else
                                {
                                    ExValue = (float.Parse(tempvalue[j].Substring(intInc * intLen, intLen)) / (Math.Pow(10, intDotLen))).ToString(strDot);
                                    ExValue = ChangeFirstNumber(ExValue);
                                    strRecord += (float.Parse(tempvalue[j].Substring(intInc * intLen, intLen)) / (Math.Pow(10, intDotLen))).ToString(strDot) + "|";
                                }
                            }
                            else
                            {
                                ExValue = (float.Parse(arrRecordLoad[j].Substring(intInc * intLen, intLen)) / (Math.Pow(10, intDotLen))).ToString(strDot);
                                ExValue = ChangeFirstNumber(ExValue);
                                strRecord += (float.Parse(arrRecordLoad[j].Substring(intInc * intLen, intLen)) / (Math.Pow(10, intDotLen))).ToString(strDot) + "|";
                            }
                        }
                        catch (Exception ex)
                        {
                            strRecord += arrRecordLoad[j].Substring(intInc * intLen, intLen) + "|";
                        }
                    else
                        strRecord += ",";

                    if (bReadTwo)
                    {
                        intLen = intRecordLen1 / intCount * 2;
                        strDot = string.Format("F{0}", intDotLen1);
                        if (arrRecordLoad1[j].Length >= (intInc + 1) * intLen)
                            try
                            {
                                strRecord1 += (float.Parse(arrRecordLoad1[j].Substring(intInc * intLen, intLen)) / (Math.Pow(10, intDotLen1))).ToString(strDot) + "|";
                            }
                            catch (Exception ex)
                            {
                                strRecord1 += arrRecordLoad1[j].Substring(intInc * intLen, intLen) + "|";
                            }
                        else
                            strRecord1 += ",";

                    }
                }


                if (bReadTwo)
                {
                    strRecord1 = strRecord1.Trim(',');
                    strRecord += strRecord1 + "," + strRecordTime;
                }
                else
                {
                    strRecord = strRecord.Trim(',');
                    strRecord += "," + strRecordTime;
                }

                _Result.Ml_chrValue = strRecord;
                if (strRecord.Contains("FF")) bResult[j] = false;
                if (strRecordTime != m_dtStartTime.AddMinutes(-1).ToString("yyMMddHHmm") &&
                    strRecordTime != m_dtStartTime.AddMinutes(1).ToString("yyMMddHHmm") &&
                    strRecordTime != m_dtStartTime.ToString("yyMMddHHmm"))
                    bResult[j] = false;

               
            }

            return bResult;
        }
        #region 解析报文
        public static string ExAnalyze(string Code, int type,ref string e)
        {
            if (Code.Trim() == string.Empty) return "";
            string temp = Code;
            StringBuilder x = new StringBuilder(temp.Length);
            for (int i = temp.Length - 1; i >= 0; i = i - 2)
            {
                x.Append(temp[i - 1]);
                x.Append(temp[i]);

            }
            temp = x.ToString();
            return Analyz(temp, type, ref e);

        }
        public static string Reverse(string needRe)
        {
            string temp = "";
            StringBuilder x = new StringBuilder(needRe.Length);
            for (int i = needRe.Length - 1; i >= 0; i = i - 2)
            {
                x.Append(needRe[i - 1]);
                x.Append(needRe[i]);

            }
            temp = x.ToString();
            return temp;
        }
        public static string ReverseOne(string needRe)
        {
            string temp = "";
            StringBuilder x = new StringBuilder(needRe.Length);
            for (int i = needRe.Length - 1; i >= 0; i--)
            {

                x.Append(needRe[i]);

            }
            temp = x.ToString();
            return temp;
        }
        public static string MySplit(string myword, int Num, int dot)
        {
            //string[] arr_temp = null;
            int SplitNum = myword.Length / Num;
            string[] arr_temp = new string[SplitNum];
            string total = "";
            StringBuilder s = new StringBuilder(3 + Math.Abs(dot));
            s.Append("#");
            s.Append("0");
            s.Append(".");
            for (int i = 0; i < Math.Abs(dot); i++)
            {
                s.Append("0");


            }
            string S_format = s.ToString();
            for (int i = 0; i < SplitNum; i++)
            {
                string exValue = (Convert.ToDouble(myword.Substring((Num * i), Num)) * Math.Pow(10, dot)).ToString(S_format);
                if (exValue != "" && exValue.Substring(0,1)=="8")
                {
                
                    StringBuilder tempBulider = new StringBuilder(exValue.Length);
                    tempBulider.Append("-");
                    for (int j = 1; j < exValue.Length; j++)
                    {
                        tempBulider.Append(exValue[j]);
                    }
                    exValue = tempBulider.ToString();
                }
                total = total + exValue + "|";
            }
            return total.Substring(0, total.Length - 1);

        }
        public static string ChangeFirstNumber(string exValue)
        {
            if (exValue != "" && exValue.Substring(0, 1) == "8")
            {

                StringBuilder tempBulider = new StringBuilder(exValue.Length);
                tempBulider.Append("-");
                for (int j = 1; j < exValue.Length; j++)
                {
                    tempBulider.Append(exValue[j]);
                }
                exValue = tempBulider.ToString();
            }
            return exValue;
        }
        public static string ReArr(string NeedToRe)
        {
            string[] ReArray = new string[NeedToRe.Split('|').Length];
            string result = string.Empty;
            for (int i = NeedToRe.Split('|').Length - 1; i >= 0; i--)
            {
                result = result + NeedToRe.Split('|')[i] + "|";
            }
            return result.Substring(0, result.Length - 1);
        }
        public static string Analyz(string code, int type,ref string CheckTime)
        {
            string result = "";
            string checkTime = "";
            string[] Arr_content = null;
            checkTime = code.Substring(6, 10);
            CheckTime = Reverse(checkTime);
            Arr_content = code.Replace("AA", "*_").Split('*');
            string Content = string.Empty;
            switch (type)
            {
                case 1:
                    Content = Arr_content[0].Substring(16, Arr_content[0].Length - 16);
                    Content = Reverse(Content);
                    string vol, cur, fre;
                    fre = (Convert.ToDouble(Content.Substring(0, 4)) * 0.01).ToString();
                    cur = Content.Substring(4, 18);
                    cur = MySplit(cur, 6, -3);
                    vol = Content.Substring(22, 12);
                    vol = MySplit(vol, 4, -1);
                    result = vol + ";" + cur + ";" + fre;
                    break;
                case 2:
                    //301700 760500 760500 770500 010000 000000 000000 010000
                    // 0.1730 0.0576 0.0576 0.0577 0.0001 0.0000 0.0000 0.0000
                    Content = Arr_content[1].Replace("_", "");
                    Content = Reverse(Content);
                    string Ptol, Pa, Pb, Pc, Qtol, Qa, Qb, Qc;
                    Ptol = Content.Substring(Content.Length / 2, Content.Length / 2);
                    Ptol = MySplit(Ptol, 6, -4);


                    Qtol = Content.Substring(0, Content.Length / 2);
                    Qtol = MySplit(Qtol, 6, -4);
                    result = ReArr(Ptol) + ";" + ReArr(Qtol);
                    break;
                case 3:
                    //9909990999099909
                    Content = Arr_content[2].Replace("_", "");
                    Content = Reverse(Content);
                    string factorTol = "";
                    factorTol = MySplit(Content, 4, -3);
                    result = ReArr(factorTol);
                    break;
                case 4:
                    Content = Arr_content[3].Replace("_", "");
                    Content = Reverse(Content);
                    string EnergyTol = "";
                    EnergyTol = MySplit(Content, 8, -2);
                    result = ReArr(EnergyTol);
                    break;
                case 5:
                    Content = Arr_content[4].Replace("_", "");
                    Content = Reverse(Content);
                    string QEnergyTol = "";
                    QEnergyTol = MySplit(Content, 8, -2);
                    result = ReArr(QEnergyTol);
                    break;
                case 6:
                    Content = Arr_content[5].Replace("_", "");
                    Content = Reverse(Content);
                    string DemandTol = "";
                    DemandTol = MySplit(Content, 6, -4);
                    result = ReArr(DemandTol);

                    break;
            }
            return result;
        }

        #endregion
        /// <summary>
        /// 初始化设备参数,计算每一块表需要检定的圈数
        /// </summary>
        /// <returns></returns>
        private bool InitEquipment()
        {
            if (Stop) return false;                   //假如当前停止检定，则退出            
            MessageController.Instance.AddMessage("开始升电压...");
            if (Stop) return false;                   //假如当前停止检定，则退出
            
            if (!PowerOn())
            {
                MessageController.Instance.AddMessage("升电压失败! ");
                return false;
            }
            if (Stop) return false;                   //假如当前停止检定，则退出


            MessageController.Instance.AddMessage("正在等待多功能操作电能表正常运行时间10秒。");
            System.Threading.Thread.Sleep(10000);
            
            return true;
        }

        #region 控制源输出
        /// <summary>
        /// 控制源输出
        /// </summary>
        /// <returns>控源结果</returns>
        protected override bool PowerOn()
        {
            int firstYaoJianMeterIndex = Helper.MeterDataHelper.Instance.FirstYaoJianMeter;
            MeterBasicInfo firstYaoJianMeter = Helper.MeterDataHelper.Instance.Meter(firstYaoJianMeterIndex);
            Cus_PowerYuanJian ele = Cus_PowerYuanJian.H;
            //如果是单相，只输出A元
            if (GlobalUnit.IsDan) ele = Cus_PowerYuanJian.A;
            MessageController.Instance.AddMessage(string.Format("开始控制功率源输出:{0}V {1}A", CLDC_DataCore.Const.GlobalUnit.U, 0));
            bool ret = Helper.EquipHelper.Instance.PowerOn(CLDC_DataCore.Const.GlobalUnit.U,0,(int)ele, (int)PowerFangXiang,FangXiangStr + "1.0",true, false);
            MessageController.Instance.AddMessage(string.Format("控制功率源输出:{0}V {1}A {2}", CLDC_DataCore.Const.GlobalUnit.U, 0, ret ? "成功" : "失败"));
            return ret;
        }

        #endregion

    }
}