using CLDC_Comm.Enum;
using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_DataCore.Function;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Struct;
using CLDC_VerifyAdapter.Helper;
using CLDC_VerifyAdapter.VerifyService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CLDC_VerifyAdapter.Influence
{
    /// <summary>
    /// 负载电流快速改变试验
    /// </summary>
    class CurrentChange : VerifyBase
    {

        #region ----------构造函数----------

        public CurrentChange(object plan)
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
            ResultNames = new string[] { "测试时间", "起始电量", "结束电量", "脉冲数", "标准表电量", "误差", "结论", "不合格原因" };
            return true;
        }

        #endregion
        public override void Verify()
        {
            base.Verify();
            bool bPowerOn = PowerOn();
            bool[] Result = new bool[BwCount];
            string[] Fail = new string[BwCount];
            string glys = "1.0";
            float testI = 0;
            string[] array = VerifyPara.Split('|');
            float Ton = float.Parse(array[2].ToString());
            float Toff = float.Parse(array[3].ToString());
            float RunTime = float.Parse(array[4].ToString());
            if (!string.IsNullOrEmpty(array[0]))
            {
                glys = array[0];
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("请检查方案参数是否正确");
            }

            if (!string.IsNullOrEmpty(array[1]))
            {
                testI = Number.GetCurrentByIb(array[1], Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).Mb_chrIb, Helper.MeterDataHelper.Instance.Meter(GlobalUnit.FirstYaoJianMeter).Mb_BlnHgq);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("请检查方案参数是否正确");
            }

            MessageController.Instance.AddMessage("正在读取起码");
            float[] flt_QM = MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2);
            for (int k = 0; k < BwCount; k++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(k).YaoJianYn)
                {

                    continue;
                }


                ResultDictionary["起始电量"][k] = flt_QM[k].ToString("0.0000");
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "起始电量", ResultDictionary["起始电量"]);
            MessageController.Instance.AddMessage("启动误差板走字指令");

            if (EquipHelper.Instance.InitPara_Constant(Cus_PowerFangXiang.正向有功, null) == false)
            {
                //Stop = true;
                //  MessageController.Instance.AddMessage("启动误差板走字指令失败", 6, 2);
                //return;
            }

            if (EquipHelper.Instance.PowerOn(GlobalUnit.U, testI, 1, (int)Cus_PowerFangXiang.正向有功, glys, IsYouGong, false) == false)
            {
                //Stop = true;
                //return;
            }
            EquipHelper.Instance.SetErrCalcType(0);
            EquipHelper.Instance.SetErrCalcType(2);
            DateTime startTime = DateTime.Now;
            while (true)
            {
                if (Stop) break;
                Countdown(Ton - 1, "开通持续时间");

                if (Stop) break;
                int ms = (int)(Toff * 1000);

                float[] wave = new float[6];
                int[] Time = new int[6];
                bool[] Switch = new bool[6];
                wave[0] = 0;
                wave[1] = 0;
                wave[2] = 0;
                wave[3] = 0f;
                wave[4] = 0f;
                wave[5] = 0f;
                Time[0] = 0;
                Time[1] = 0;
                Time[2] = 0;
                Time[3] = ms;
                Time[4] = ms;
                Time[5] = ms;
                Switch[0] = false;
                Switch[1] = false;
                Switch[2] = false;
                Switch[3] = true;
                Switch[4] = true;
                Switch[5] = true;

                if (Stop) return;
                bool ret = EquipHelper.Instance.SetDropWave(wave);
                bool ret1 = EquipHelper.Instance.SetDropTime(Time);
                bool ret2 = EquipHelper.Instance.SetDropSwitch(Switch);
                if (ret != true || ret1 != true || ret2 != true)
                {
                    break; ;

                }
                if (Stop) break;
                //   EquipHelper.Instance.PowerShortDown(false, false, false, true, true, true, 0, 0, 0, 0f, 0f, 0f, 0, 0, 0, ms, ms, ms);

                if (Stop) break;
                Countdown(Toff, "断开持续时间");
                if (Stop) break;
                if (DateTime.Now.Subtract(startTime).TotalSeconds >= RunTime * 60 + 10)
                    break;
            }

            PowerOn();
            EquipHelper.Instance.SetErrCalcType(0);
            if (Stop) return;
            int num = 0;
            string data = string.Empty;
            string time = string.Empty;
            string[] arrData = new string[BwCount];
            for (int k = 0; k < BwCount; k++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(k).YaoJianYn)
                {

                    continue;
                }
                Helper.EquipHelper.Instance.ReadQueryCurrentErrorControl(k + 1, 3, out num, out data, out time);
                arrData[k] = data;
            }
            if (Stop) return;
            MessageController.Instance.AddMessage("正在读取止码");
            float[] flt_ZM = MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2);
            if (Stop) return;
            for (int k = 0; k < BwCount; k++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(k).YaoJianYn)
                {

                    continue;
                }

                ResultDictionary["脉冲数"][k] = arrData[k];
                ResultDictionary["结束电量"][k] = flt_ZM[k].ToString("0.0000");
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结束电量", ResultDictionary["结束电量"]);
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "脉冲数", ResultDictionary["脉冲数"]);


            float stmPower = 1;
            float testEn = 0.0f;
            long eun = 0;
            float c = 0.0f;

            if (Helper.EquipHelper.Instance.ReadTestEnergy(out testEn, out eun))
            {
                stmPower = testEn;

            }
            else
            {
                for (int i = 0; i < BwCount; i++)
                {
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    if (float.TryParse(arrData[i], out c))
                    {
                        if (c > 0)
                        {
                            stmPower = c / GetDJ(Helper.MeterDataHelper.Instance.Meter(i).Mb_chrBdj)[0];
                            break;
                        }
                    }
                }
            }
            float[] Wc = new float[BwCount];
            for (int i = 0; i < BwCount; i++)
            {

                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                ResultDictionary["标准表电量"][i] = stmPower.ToString();
                if (flt_ZM[i] - flt_QM[i] > 0 && stmPower > 0)
                {
                    Wc[i] = ((flt_ZM[i] - flt_QM[i]) - stmPower) / stmPower * 100;

                    if (Wc[i] <= 2.0)
                    {
                        ResultDictionary["误差"][i] = Wc[i].ToString("0.0000");
                        ResultDictionary["结论"][i] = "合格";
                    }
                    else
                    {
                        ResultDictionary["结论"][i] = "不合格";
                        ResultDictionary["不合格原因"][i] = "误差超差";
                    }
                }
                else if (flt_QM[i] - flt_ZM[i] >= 0)
                {
                    ResultDictionary["结论"][i] = "不合格";
                    ResultDictionary["不合格原因"][i] = "走字电量出错";
                }
                else if (stmPower == 0)
                {
                    ResultDictionary["结论"][i] = "不合格";
                    ResultDictionary["不合格原因"][i] = "标准表走字电量出错";
                }


            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "标准表电量", ResultDictionary["标准表电量"]);
            Countdown(2, "等待");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "误差", ResultDictionary["误差"]);
            Countdown(2, "等待");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
            Countdown(2, "等待");
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "不合格原因", ResultDictionary["不合格原因"]);



        }

      

        private float[] GetDJ(string DJ)
        {
            float[] value = new float[] { 0f, 0f };
            DJ = DJ.Replace("s", "");
            DJ = DJ.Replace("S", "");
            if (!DJ.Contains("("))
            {
                float.TryParse(DJ, out value[0]);
            }
            else
            {
                string temp = DJ;

                temp = temp.Substring(0, DJ.IndexOf("("));

                float.TryParse(temp, out value[0]);
                temp = DJ;
                temp = temp.Substring(DJ.IndexOf("(") + 1);
                temp = temp.Replace(")", "");
                float.TryParse(temp, out value[1]);
            }
            return value;
        }

    }
}
