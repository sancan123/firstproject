using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CLDC_DataCore;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Function;
using CLDC_DataCore.Const;
using CLDC_Comm.Enum;

namespace CLDC_VerifyAdapter.SecondStage
{
    class PowerConsumptionInfluence : VerifyBase
    {

        private string Communication;
        #region ----------构造函数----------

        public PowerConsumptionInfluence(object plan)
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
            Communication = "载波通讯状态";
            ResultNames = new string[] { "电压A有功", "电压B有功", "电压C有功", "电压A视在", "电压B视在", "电压C视在", "电流A有功", "电流B有功", "电流C有功", "电流A视在", "电流B视在", "电流C视在", "结论", "不合格原因" };
            return true;
        }

        #endregion
        public override void Verify()
        {
            base.Verify();
            if (Communication.IndexOf("485通讯状态") != -1)
                SwitchCarrierOr485(Cus_CommunType.通讯485);
            else if (Communication.IndexOf("载波通讯状态") != -1)
                SwitchCarrierOr485(Cus_CommunType.通讯载波);
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(StartCommunication));
            MessageController.Instance.AddMessage("升源");
            PowerOn();
            ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 5);


            #region 读取功耗

            #endregion





        }
        public void StartCommunication(object obj)
        {
            while (true)
            {
                if (Communication.IndexOf("载波通讯状态") != -1)
                {
                    for (int iBw = 0; iBw < BwCount; iBw++)
                    {
                        GlobalUnit.Carrier_Cur_BwIndex = iBw;
                        if (Stop) return;
                         //【获取指定表位电表信息】
                        MeterBasicInfo curMeter = Helper.MeterDataHelper.Instance.Meter(iBw);
                        //【判断是否要检】
                        if (!curMeter.YaoJianYn)
                        {
                            continue;
                        }
                       

                        GlobalUnit.g_MsgControl.OutMessage("正在载波试验第" + (iBw + 1) + "表位...", false);
                        float flt_RevD = -1;
                      
                        try
                        {
                            #region
                            if (Stop) return;
                            for (int i = 0; i < 3; i++)
                            {
                                if (Stop) return;
                                //【读召测数据】


                                flt_RevD = CLDC_VerifyAdapter.MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2, iBw);


                            }
                            #endregion
                        }
                        catch (Exception e)
                        {
                            GlobalUnit.g_MsgControl.OutMessage(e.Message, false);
                         
                        }
                    }

                    if (Stop) return;
                }

                else if (Communication.IndexOf("485通讯状态") != -1)
                {
                    CLDC_VerifyAdapter.MeterProtocolAdapter.Instance.ReadAddress();
                    if (Stop) return;
                }

               
            }
        }

    }
}