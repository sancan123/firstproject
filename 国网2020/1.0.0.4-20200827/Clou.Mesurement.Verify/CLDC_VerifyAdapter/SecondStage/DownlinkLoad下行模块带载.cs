﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CLDC_DataCore;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_VerifyAdapter.VerifyService;
using CLDC_DataCore.Const;

namespace CLDC_VerifyAdapter.SecondStage
{
    /// <summary>
    /// 下行模块带载
    /// </summary>
    class DownlinkLoad :VerifyBase
    {


           #region ----------构造函数----------

        public DownlinkLoad(object plan)
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
            ResultNames = new string[] { "检定数据", "结论" };
            return true;
        }

        #endregion                
        public override void Verify()
        {
            base.Verify();
           bool bPowerOn = PowerOn();


           System.Windows.Forms.MessageBox.Show("请测量下行模块带载后点击确定。");

            Form_Ariticial fs = new Form_Ariticial("下行模块接口带载实验");
            fs.ShowDialog();
            MessageController.Instance.AddMessage("正在处理结果...");
            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //假如当前停止检定，则退出
                MeterBasicInfo _meter = null;
                _meter = Helper.MeterDataHelper.Instance.Meter(i);
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    continue;
                }

                ResultDictionary["检定数据"][i] = GlobalUnit.ManualShuju[i];
                    ResultDictionary["结论"][i] = GlobalUnit.ManualResult[i];
              




              //  ResultDictionary["结论"][i] = (string.IsNullOrEmpty(address[i]) == false) ? Variable.CTG_HeGe : Variable.CTG_BuHeGe;
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "检定数据", ResultDictionary["检定数据"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
        



        }



    }
}
