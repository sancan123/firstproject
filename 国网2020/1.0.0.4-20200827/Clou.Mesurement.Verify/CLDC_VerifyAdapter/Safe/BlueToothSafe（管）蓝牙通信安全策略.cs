using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_VerifyAdapter.EventLog;
using CLDC_Comm.Enum;
namespace CLDC_VerifyAdapter.Safe
{
    /// <summary>
    /// （管）蓝牙通信安全策略
    /// </summary>
    class BlueToothSafe : VerifyBase
    {
        public BlueToothSafe(object plan)
            : base(plan) 
        {
            
        }

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
            ResultNames = new string[] { "测试时间",  "结论", "不合格原因" };
            return true;
        }

        /// 重写基类测试方法
        /// </summary>
        /// <param name="ItemNumber">检定方案序号</param>
        public override void Verify()
        {

            if (Stop) return;
            base.Verify();
            PowerOn();

            if (Stop) return;
            MessageController.Instance.AddMessage("正在准备蓝牙操作");
            System.Windows.Forms.MessageBox.Show("请手动插上蓝牙模块，完成后点击确定。");
            float f = 1.3F;
            if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.三相四线)
            {
                bool bPowerOn = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U * f, GlobalUnit.U * f, GlobalUnit.U * f, 0, 0, 0, 0, 50, "1.0", true, false, false, 0);
            }
            else if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.三相三线)
            {
                bool bPowerOn = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U * f, GlobalUnit.U, GlobalUnit.U * f, 0, 0, 0, 0, 50, "1.0", true, false, false, 0);
            }
            else if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.单相)
            {
                CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, 0, GlobalUnit.Imax * f, 0, 0, (int)Cus_PowerYuanJian.H, 50, "1.0", false, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);

            }
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60);
            PowerOn();
            string[] strID = new string[BwCount];
            string[] strData = new string[BwCount];
            string[] strSetData = new string[BwCount];
            int[] iFlag = new int[BwCount];
            string[] strShowData = new string[BwCount];
            string[] strCode = new string[BwCount];
            string[] strRand1 = new string[BwCount];//随机数
            string[] strRand2 = new string[BwCount];//随机
            string[] strEsamNo = new string[BwCount];//Esam序列号
            bool[] result = new bool[BwCount];
            string[] strEnerZQ = new string[BwCount];//
            
            GlobalUnit.g_CommunType = CLDC_Comm.Enum.Cus_CommunType.通讯蓝牙;
           
            string[] address_MAC = new string[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                address_MAC[i] = Helper.MeterDataHelper.Instance.Meter(i).Mb_chrAddr_MAC;
            }

            //读数据
            MessageController.Instance.AddMessage("正在进行蓝牙连接...");
            bool[] bResult = MeterProtocolAdapter.Instance.ConnectBlueTooth(address_MAC);

            MessageController.Instance.AddMessage("正在读取当前正向有功总电能,请稍候....");
            float[] fEnerZQ = MeterProtocolAdapter.Instance.ReadData("00010000", 4,2);

           

            //写数据
            if (Stop) return;
            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();
           
            if (Stop) return;
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
              
            //设置1次校时
            for (int it = 0; it < 1; it++)
            {
                string datetime = DateTime.Now.AddMinutes(it+1).ToString("yyMMddHHmmss");//每次对时加1小时
                for (int i = 0; i < BwCount; i++)
                {
                    strCode[i] = "0400010C";
                    strSetData[i] = datetime.Substring(0, 6) + "0" + (int)DateTime.Now.DayOfWeek;
                    strSetData[i] += datetime.Substring(6, 6);
                    strShowData[i] = datetime;
                    strData[i] = strCode[i] + strSetData[i];
                }
                if (Stop) return;
                MessageController.Instance.AddMessage("正在第" + (it + 1) + "次校时....");
                 result = MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
                ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);
            }
            
            //for (int i = 0; i < BwCount; i++)
            //{
            //    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
               
            //     strEnerZQ[i] = fEnerZQ[i].ToString();
               
            //    if (result[i] == false)
            //    {
            //        ResultDictionary["蓝牙认证写数据是否成功"][i] = "失败";
            //    }
            //    else
            //    {
            //        ResultDictionary["蓝牙认证写数据是否成功"][i] = "成功";
            //    }
            //}

            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "蓝牙无认证读数据值", strEnerZQ);
            //MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "蓝牙认证写数据是否成功", ResultDictionary["蓝牙认证写数据是否成功"]);


            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (fEnerZQ[i] == -1 || fEnerZQ[i] == 0)
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "蓝牙无认证读数据值不对";
                    continue;
                }
                if (result[i] ==false)
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "蓝牙认证后写数据失败";
                    continue;
                }
                else
                {
                    ResultDictionary["结论"][i] = Variable.CTG_HeGe;
                   
                }
            }
         
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);

            UploadTestResult("结论");

            //
            GlobalUnit.g_CommunType = CLDC_Comm.Enum.Cus_CommunType.通讯485;
          
        }
    }
}

