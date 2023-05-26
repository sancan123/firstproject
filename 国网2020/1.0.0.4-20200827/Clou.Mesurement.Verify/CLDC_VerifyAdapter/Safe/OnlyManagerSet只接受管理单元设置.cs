using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;
namespace CLDC_VerifyAdapter.Safe
{
    /// <summary>
    /// 只接受管理单元设置
    /// </summary>
    class OnlyManagerSet : VerifyBase
    {
        public OnlyManagerSet(object plan)
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
            ResultNames = new string[] { "事件产生前事件次数", "事件产生后事件次数", "上1次事件记录发生时刻", "结论", "不合格原因" };
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

            if (Stop) return;
            MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();


           
            if (Stop) return;
            MessageController.Instance.AddMessage("正在模拟无管理芯情况");
            System.Windows.Forms.MessageBox.Show("请手动插上工装模块，完成后点击确定。");

            //需要工装模块，变波特率
            GlobalUnit.IsGZMK = true;
            //设置是否要加密，发明文还是密文
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
             
                    Helper.MeterDataHelper.Instance.Meter(i).DgnProtocol.IsSouthEncryption = false;
                
            }
            //设置过载阀值   
            if (Stop) return;
            MessageController.Instance.AddMessage("正在设置过载阀值 ");
            bool[] bResult = MeterProtocolAdapter.Instance.WriteData("04090B01", 3, CLDC_VerifyAdapter.Helper.EquipHelper.Instance.RevString("001000", 2));
            string[] str04090D01 = MeterProtocolAdapter.Instance.ReadData("04090B01", 3);
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;              
                    Helper.MeterDataHelper.Instance.Meter(i).DgnProtocol.IsSouthEncryption = true;
            }
            //变回原来波特率
            GlobalUnit.IsGZMK = false;
            System.Windows.Forms.MessageBox.Show("请手动插拔掉工装模块并接上管理芯模块，完成后点击确定。");

            //恢复原来阀值
            iFlag = MeterProtocolAdapter.Instance.SouthCheckIdentity(out strRand1, out strRand2, out strEsamNo);
            Common.Memset(ref strCode, "04090B01");
            Common.Memset(ref strData, "04090B01" + "012000");//Imax 0.1倍
            MeterProtocolAdapter.Instance.SouthParameterElseUpdate(iFlag, strRand2, strData, strCode);
            
            MessageController.Instance.AddMessage("正在处理结果");
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (bResult[i] )
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "没管理芯情况，不能设置阀值";
                    continue;
                }
                if (str04090D01[i]=="")
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                    reasonS[i] = "没管理芯情况，工装模块通讯不上";
                    continue;
                }
               
                else
                {
                    ResultDictionary["结论"][i] = Variable.CTG_BuHeGe;
                     
                }
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", reasonS);

            UploadTestResult("结论");
        }
    }
}

