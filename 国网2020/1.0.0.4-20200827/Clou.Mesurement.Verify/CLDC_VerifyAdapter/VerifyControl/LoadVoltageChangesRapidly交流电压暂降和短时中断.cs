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

namespace CLDC_VerifyAdapter
{
    class LoadVoltageChangesRapidly: VerifyBase
    {
       
       #region ----------构造函数----------

        public LoadVoltageChangesRapidly(object plan)
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
            ResultNames = new string[] { "测试时间", "测试前电量", "测试后电量", "结论", "不合格原因" };
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
           float JdBs = float.Parse(array[0].ToString().Replace("%","")) /100;
           float Ton = float.Parse(array[2].ToString());
           float Toff = float.Parse(array[1].ToString());
           float RunTime = float.Parse(array[3].ToString());
          
        
           MessageController.Instance.AddMessage("正在读取起码");
           float[] flt_QM = MeterProtocolAdapter.Instance.ReadData("00010000", 4, 2);
           for (int k = 0; k < BwCount; k++)
           {
               if (!Helper.MeterDataHelper.Instance.Meter(k).YaoJianYn)
               {

                   continue;
               }


               ResultDictionary["测试前电量"][k] = flt_QM[k].ToString("0.0000");
           }
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "测试前电量", ResultDictionary["测试前电量"]);
          

          
           DateTime startTime = DateTime.Now;

           

           int ms = (int)(Toff * 1000);

           float[] wave = new float[6];
           int[] Time = new int[6];
           bool[] Switch = new bool[6];
           wave[0] = GlobalUnit.U * (1 - JdBs);
           wave[1] = GlobalUnit.U * (1 - JdBs);
           wave[2] = GlobalUnit.U * (1 - JdBs);
           wave[3] = 0;
           wave[4] = 0;
           wave[5] = 0;
           Time[0] = ms;
           Time[1] = ms;
           Time[2] = ms;
           Time[3] = 0;
           Time[4] = 0;
           Time[5] = 0;
           Switch[0] = true;
           Switch[1] = true;
           Switch[2] = true;
           Switch[3] = false;
           Switch[4] = false;
           Switch[5] = false;
           if (JdBs == 1)
           {
               for (int i = 0; i < RunTime; i++)
               {
                   if (Stop) break;
                   Countdown(Ton - 1, "开通持续时间");

                   if (Stop) break;
                   bool ret = EquipHelper.Instance.SetDropWave(wave);
                   bool ret1 = EquipHelper.Instance.SetDropTime(Time);
                   bool ret2 = EquipHelper.Instance.SetDropSwitch(Switch);
                   if (ret != true || ret1 != true || ret2 != true)
                   {
                       break;
                       Stop = true;
                   }
                   if (Stop) break;

                   Countdown(Toff, "断开持续时间");
                   if (Stop) break;
               }
           }
           else
           {
               for (int k = 0; k < 3; k++)
               {
                   if (k == 0)
                   {
                       MessageController.Instance.AddMessage("开始测试A相");
                       wave[1] = GlobalUnit.U;
                       wave[2] = GlobalUnit.U;
                   }
                   else if (k == 1)
                   {
                       MessageController.Instance.AddMessage("开始测试B相");
                       wave[0] = GlobalUnit.U ;
                       wave[2] = GlobalUnit.U;
                   }
                   else
                   {
                       MessageController.Instance.AddMessage("开始测试C相");
                       wave[0] = GlobalUnit.U;
                       wave[1] = GlobalUnit.U;
                   }
                   for (int i = 0; i < RunTime; i++)
                   {
                       if (Stop) break;
                       Countdown(Ton - 1, "开通持续时间");

                       if (Stop) break;
                       bool ret = EquipHelper.Instance.SetDropWave(wave);
                       bool ret1 = EquipHelper.Instance.SetDropTime(Time);
                       bool ret2 = EquipHelper.Instance.SetDropSwitch(Switch);
                       if (ret != true || ret1 != true || ret2 != true)
                       {
                           break;
                           Stop = true;
                       }
                       if (Stop) break;

                       Countdown(Toff, "断开持续时间");
                       if (Stop) break;
                   }
                  
               }

           }
          


           PowerOn();

           if (Stop) return;
           int num = 0;
           string data = string.Empty;
           string time = string.Empty;
           string[] arrData = new string[BwCount];
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


               ResultDictionary["测试后电量"][k] = flt_ZM[k].ToString("0.0000");
           }
           MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "测试后电量", ResultDictionary["测试后电量"]);

           if (Stop) return;
            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                if (flt_ZM[i]== flt_QM[i])
                {
                   
                        ResultDictionary["结论"][i] = "合格";
                   
                }
                else 
                {
                    ResultDictionary["结论"][i] = "不合格";
                    ResultDictionary["不合格原因"][i] = "电量出错";
                }
               


            }
       
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);
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

