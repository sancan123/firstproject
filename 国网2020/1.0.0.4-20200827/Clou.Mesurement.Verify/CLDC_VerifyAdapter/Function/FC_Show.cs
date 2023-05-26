using System;
using CLDC_DataCore;
using System.Collections.Generic;
using CLDC_DataCore.Const;
using CLDC_DataCore.Model.DnbModel.DnbInfo;

namespace CLDC_VerifyAdapter.Function
{
    ///<summary>
    /// FileName:FC_Show.cs
    /// machinename:2014-0325-1259
    /// Author:kaury
    /// DateTime:2014/7/18 17:48:02
    /// Corporation:
    /// Description:
    ///</summary>
    public class FC_Show : FunctionBase
    {
        public FC_Show(object plan)
            : base(plan) 
        {
            
        }
        List<string[]> lst_IDs = new List<string[]>();//每屏所有表
        /// <summary>
        /// 
        /// </summary>
        public override void Verify()
        {
            base.Verify();
            //读出电表屏显的数据标识列表 与 方案配置的数据标识列表 做相等对比（顺序一致、标识一致）
            #region 自动循环显示
            int AutoScreenCountStd = 0;
            string[] AutoScreenIDsStd;
            ResultNames = new string[] { "屏显数量结论", "屏显顺序结论", "结论" };
            try
            {
                string[] AutoScreenStd = CurPlan.PrjParm.Split(',');
                AutoScreenCountStd = AutoScreenStd.Length;
                AutoScreenIDsStd = new string[AutoScreenCountStd];
                for (int i = 0; i < AutoScreenCountStd; i++)
                {
                    string[] tmp = AutoScreenStd[i].Split('|');
                    AutoScreenIDsStd[i] = tmp[2];
                }

            }
            catch (Exception)
            {
                MessageController.Instance.AddMessage("自动循环显示数据方案配置错误！");
                return;
            }
            PowerOn();

            MessageController.Instance.AddMessage("读取自动循环显示屏数");
            string[] AutoScreenCount = MeterProtocolAdapter.Instance.ReadData("04000301", 1);

            MessageController.Instance.AddMessage("读取自动循环显示屏显示数据项");
            int[] intAutoScreenCount = CLDC_DataCore.Function.ConvertArray.ConvertString2Int(AutoScreenCount);
            int maxAuto = getMax(intAutoScreenCount);
            
            for (int i = 1; i <= maxAuto; i++)
            {
                MessageController.Instance.AddMessage("读取自动循环显示第" + i + "/" + maxAuto + "屏显示数据项");
                string[] sZdxx = MeterProtocolAdapter.Instance.ReadData("040401" + Convert.ToString(i, 16).PadLeft(2, '0'), 5);
                for (int j = 0; j < sZdxx.Length;j++ )
                {
                    if (sZdxx[j] != null && sZdxx[j].Length == 10)
                    {
                        sZdxx[j] = sZdxx[j].Substring(2, 8);
                    }
                }
                lst_IDs.Add(sZdxx);
            }
            MessageController.Instance.AddMessage("正在处理数据...");
            bool[] Rst = new bool[BwCount];
            string[] RstDisReason = new string[BwCount];
            MeterBasicInfo mb = new MeterBasicInfo();
            string[] arrStrResultKey = new string[BwCount];
            object[] arrObjResultValue = new object[BwCount];
            for (int i = 0; i < BwCount; i++)
            {
                mb = Helper.MeterDataHelper.Instance.Meter(i);
                if (!mb.YaoJianYn) continue;
                string[] OneMeterIDs = getIDsFromLst_IDs(i);
                if (intAutoScreenCount[i] == AutoScreenCountStd)
                {
                    bool bYz = true ;
                    for (int j = 0; j < AutoScreenCountStd; j++)
                    {
                        if(AutoScreenIDsStd[j] !=OneMeterIDs[j])
                        {
                            bYz = false;
                            break;
                        }
                    }
                    if (bYz)//AutoScreenIDsStd.Equals(OneMeterIDs)
                    {
                        Rst[i] = true;
                        RstDisReason[i] = "";
                    }
                    else
                    {
                        Rst[i] = false;
                        RstDisReason[i] = "自动循环显示屏显示数据项或顺序与配置不匹配";
                        ResultDictionary["屏显顺序结论"][i] = "不合格";
                    }
                    ResultDictionary["屏显数量结论"][i] = "合格";
                    ResultDictionary["屏显顺序结论"][i] = "合格";
                }
                else
                {
                    Rst[i] = false;
                    RstDisReason[i] = "自动循环显示屏数" + intAutoScreenCount[i] + "不等于配置屏数" + AutoScreenCountStd;
                    ResultDictionary["屏显数量结论"][i] = "不合格";
                    ResultDictionary["屏显顺序结论"][i] = "不合格";
                }

                string result = Rst[i] ? Variable.CTG_HeGe : Variable.CTG_BuHeGe;
                ResultDictionary["结论"][i] = result;
            }
            UploadTestResult("屏显数量结论");
            UploadTestResult("屏显顺序结论");
            UploadTestResult("结论");

            #endregion 自动循环显示

            #region 按键循环显示
            //MessageController.Instance.AddMessage("读取按键循环显示屏数", false);
            //string[] BtnScreenCount = MeterProtocolAdapter.Instance.ReadData("04000305", 1);
            #endregion 按键循环显示
        }

        private string StrtoString(string[] OneMeterIDs)
        {
            string _Ids = "";
            int _IdsCount = OneMeterIDs.Length;
            for (int i = 0; i < _IdsCount; i++)
            {
                if (i == _IdsCount - 1)
                {
                    _Ids = _Ids + OneMeterIDs[i];
                }
                else
                {
                    _Ids = _Ids + OneMeterIDs[i] + "|";
                }
            }
            return _Ids;
        }

        private string[] getIDsFromLst_IDs(int bw)
        {
            List<string> _lstIDs = new List<string>();
            int lstCount = lst_IDs.Count;
            for (int i = 0; i < lstCount; i++)
            {
                _lstIDs.Add((lst_IDs[i])[bw]);
            }
            return _lstIDs.ToArray();
        }

        private int getMax(int[] intAutoScreenCount)
        {
            int max = 0;
            foreach (int item in intAutoScreenCount)
            {
                if (item > max)
                {
                    max = item;
                }
            }
            return max;
        }
        protected override string ItemKey
        {
            get
            {
                return base.ItemKey;
            }
        }
        protected override string ResultKey
        {
            get
            {
                return base.ResultKey;
            }
        }
    }
}
