
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_DataCore.Struct;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;
using CLDC_DataCore.Function;
using CLDC_VerifyAdapter.Multi;
using CLDC_VerifyAdapter.Helper;

namespace CLDC_VerifyAdapter.Multicore
{
    /// <summary>
    /// 测量监测
    /// </summary>
    public class CheckMeterInfo : DgnBase
    {
       private StPlan_ConnProtocol paraStruct = new StPlan_ConnProtocol();
                     
        protected override string ItemKey
        {
            get { throw new NotImplementedException(); }
        }

        protected override string ResultKey
        {
            get { throw new NotImplementedException(); }
        }

        public CheckMeterInfo(object plan)
            : base(plan)
        {
            //ResultNames = new string[] { "标识名称", "标识编码", "标识长度", "数据格式", "标准值", "读(写)数据", "试验类型", "结论" };
            ResultNames = new string[] {"测试时间", "标识名称", "标识编码", "标识长度", "数据格式", "读(写)数据", "试验类型", "结论" ,"不合格原因"};
        }

       #region 解析并校验检定参数
        /// <summary>
        /// 解析并校验检定参数
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            string[] arrayPara = VerifyPara.Split('|');
          
            paraStruct.ItemCode = arrayPara[0];
            int lengthTemp = 0;
            if (int.TryParse(arrayPara[1], out lengthTemp))
            {
                paraStruct.DataLen = lengthTemp;
            }
            else
            {
                MessageController.Instance.AddMessage(string.Format("数据标识长度解析错误,参数:{0}", VerifyPara), 6, 2);
                return false;
            }
            if (int.TryParse(arrayPara[2], out lengthTemp))
            {
                paraStruct.PointIndex = lengthTemp;
            }
            else
            {
                MessageController.Instance.AddMessage(string.Format("数据标识小数位数解析错误,参数:{0}", VerifyPara), 6, 2);
                return false;
            }
            paraStruct.StrDataType = arrayPara[3];
           
                paraStruct.OperType = StMeterOperType.读;
                
          
           
         

            if (string.IsNullOrEmpty(arrayPara[4]) == false)
            {
                paraStruct.DiDesc = arrayPara[4];
            }

            if (string.IsNullOrEmpty(arrayPara[5] )==false)
            {
                paraStruct.BzValue =float.Parse( arrayPara[5]);
            }
            if (arrayPara[6].ToString() == "是")
            {
                paraStruct.IsXieBo = true;
            }
            else
            {
                paraStruct.IsXieBo = false;
            }
            if (paraStruct.IsXieBo)
            {
                if (int.TryParse(arrayPara[7], out lengthTemp))
                {
                    paraStruct.XBcount = lengthTemp;
                    if (paraStruct.XBcount <= 1)
                    {
                        MessageController.Instance.AddMessage(string.Format("谐波次数需大于1,参数:{0}", VerifyPara), 6, 2);
                        return false;
                    }
                    else
                    {
                        paraStruct.XBcount = lengthTemp;
                        paraStruct.XBContent = 0.1f;
                        paraStruct.XBPhase = 0;
                    }
                   
                }
                else
                {
                    MessageController.Instance.AddMessage(string.Format("谐波次数解析错误,参数:{0}", VerifyPara), 6, 2);
                    return false;
                }
               
            }
           
            return true;
        }
        #endregion
        


            #region ----------开始检定----------
        /// <summary>
        /// 开始检定
        /// </summary>
        public override void Verify()
        {
            if (Stop) return;
            PowerOn();
            base.Verify();

            if (GlobalUnit.g_CommunType == Cus_CommunType.通讯蓝牙)
            {
                MeterProtocolAdapter.Instance.SouthCheckBlueToothIdentity();
            }



            bool f = false;//设置的值是否需要和标准比对，如升电流就要比对
            //ChangLocalPreparatoryWork(out iFlag, out strRand1, out strRand2, out strEsamNo);
            string[] revCode = new string[BwCount];
           
            //谐波情况
            if (paraStruct.IsXieBo)
            {
                EquipHelper.HarmonicPhasePara[] arrPara = new EquipHelper.HarmonicPhasePara[6];
                for (int i = 0; i < arrPara.Length; i++)
                {
                    arrPara[i] = new EquipHelper.HarmonicPhasePara();
                    arrPara[i].Initialize();
                }

                if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.单相)
                {
                    int arryIndex = 0;//相别
                    if (paraStruct.DiDesc.ToUpper().ToUpper().Contains("A相电压"))
                    {
                        arryIndex = 0;
                    }
                    else if (paraStruct.DiDesc.ToUpper().ToUpper().Contains("A相电流"))
                    {
                        arryIndex = 3;
                    }
                    arrPara[arryIndex].Content[0] = 1f; ;
                    arrPara[arryIndex].Content[paraStruct.XBcount - 1] = 0.05F;
                    // arrPara[h].Phase[paraStruct.XBcount - 1] = paraStruct.XBPhase;
                    arrPara[arryIndex].TimeSwitch[0] = true;
                    arrPara[arryIndex].TimeSwitch[paraStruct.XBcount] = true;
                    arrPara[arryIndex].IsOpen = true;
                }
                else
                {
                    for (int g = 0; g < 3; g++)
                    {


                        arrPara[g].Content[0] = 1f;
                        arrPara[g].Content[paraStruct.XBcount - 1] = 0.1f;
                        arrPara[g].Phase[paraStruct.XBcount - 1] = paraStruct.XBPhase;
                        arrPara[g].TimeSwitch[0] = true;
                        arrPara[g].TimeSwitch[paraStruct.XBcount - 1] = true;
                        arrPara[g].IsOpen = true;

                    }
                    for (int h = 3; h < 6; h++)
                    {
                       
                            arrPara[h].Content[0] = 1f; ;
                            arrPara[h].Content[paraStruct.XBcount - 1] = 0.1f;
                            arrPara[h].Phase[paraStruct.XBcount - 1] = paraStruct.XBPhase;
                            arrPara[h].TimeSwitch[0] = true;
                            arrPara[h].TimeSwitch[paraStruct.XBcount - 1] = true;
                            arrPara[h].IsOpen = true;
                        
                    }

                }
                EquipHelper.Instance.SetHarmonic(arrPara[0], arrPara[1], arrPara[2], arrPara[3], arrPara[4], arrPara[5]);

                EquipHelper.Instance.SetHarmonicSwitch(true);
            }



            switch( paraStruct.DiDesc.ToUpper())
            {
                case "A相电压":
                    Helper.EquipHelper.Instance.PowerOn(paraStruct.BzValue, 0, 0, 0, 0, 0, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);
                    f = true;
                    break;
                case "B相电压":
                    Helper.EquipHelper.Instance.PowerOn(0, paraStruct.BzValue, 0, 0, 0, 0, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);
                    f = true;
                    break;
                case "C相电压":
                    Helper.EquipHelper.Instance.PowerOn(0, 0, paraStruct.BzValue, 0, 0, 0, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);
                    f = true;
                    break;
                case "A相电流" :
                    Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, 0, paraStruct.BzValue, 0, 0, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);
                   f = true;
                    break;
                case "B相电流":
                    Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, 0, 0, paraStruct.BzValue, 0, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);
                    f = true;
                    break;
                case "C相电流":
                    Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, 0, 0, 0, paraStruct.BzValue, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);
                    f = true;
                    break;
                case "功率因数":
                    Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, 0, 0, 0, 0, (int)Cus_PowerYuanJian.H, 50, paraStruct.BzValue.ToString(), true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);
                     f = true;
                    break;
                case "频率":
                    Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, 0, 0, 0, 0, (int)Cus_PowerYuanJian.H, paraStruct.BzValue, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);
                    f = true;
                    break;
                   
                default :
                   
                    // if (GlobalUnit.Clfs == CLDC_Comm.Enum.Cus_Clfs.单相)
                    //{
                    //    //CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, 0, 0, GlobalUnit.Itr, 0, 0, (int)Cus_PowerYuanJian.H, 50, "1.0", false, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);
                    //    //Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.Itr*2, 1, 1, "1.0", true, false);
                    //    Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, GlobalUnit.Itr * 2, GlobalUnit.Itr * 2, GlobalUnit.Itr * 2, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);

                    //}else
                    // {
                         Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U, GlobalUnit.U, GlobalUnit.U, 5, 5, 5, (int)Cus_PowerYuanJian.H, 50, "1.0", true, false, false, (int)CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功);
                   
                     //}
                    f = false;

                    ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 60);
                    break;
                  
            }
              ShowWaitMessage("正在等待{0}秒,请稍候....", 1000 * 10);

            for (int i = 0; i < BwCount; i++)
            {
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;

                ResultDictionary["标识名称"][i] = paraStruct.DiDesc;
                ResultDictionary["标识编码"][i] = paraStruct.ItemCode;
                ResultDictionary["标识长度"][i] = paraStruct.DataLen.ToString();
                ResultDictionary["数据格式"][i] = paraStruct.StrDataType;
                ResultDictionary["试验类型"][i] = paraStruct.OperType.ToString();
            }
            UploadTestResult("标识名称");
            UploadTestResult("标识编码");
            UploadTestResult("标识长度");
            UploadTestResult("数据格式");
            UploadTestResult("试验类型");

                                                                         


            if (Stop) return;   //假如当前停止检定，则退出
            if (paraStruct.OperType == StMeterOperType.读)
            {
                string[] readdata = new string[BwCount];
                if (paraStruct.PointIndex > 0)
                {
                    float[] tmpdata = MeterProtocolAdapter.Instance.ReadData(paraStruct.ItemCode, paraStruct.DataLen, paraStruct.PointIndex);
                    for (int i = 0; i < BwCount; i++)
                    {
                        if (Stop) return;                   //假如当前停止检定，则退出
                        if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                        readdata[i] = tmpdata[i].ToString();
                    }
                }
                else
                {
                    readdata = MeterProtocolAdapter.Instance.ReadDataWithCode(paraStruct.ItemCode, paraStruct.DataLen, out revCode);
                }
                //结论数据
                for (int i = 0; i < BwCount; i++)
                {
                    if (Stop) return;                   //假如当前停止检定，则退出
                    if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn) continue;
                    
                    ResultDictionary["读(写)数据"][i] = readdata[i];
                  //  ResultDictionary["标准值"][i] = paraStruct.BzValue.ToString();
                    if(string.IsNullOrEmpty(readdata[i])==false)
                    {
                        if (f)//读取的值需要和标准比较的情况
                        {

                      
                        float values= float.Parse(readdata[i]);
                        float per=(Math.Abs(paraStruct.BzValue-values)*100/paraStruct.BzValue);
                        if (per >= 5.0 || values<0)
                        {
                              ResultDictionary["结论"][i] ="不合格" ;
                              ResultDictionary["不合格原因"][i] = "读取"+paraStruct.ItemCode + "值不正确";
                            
                        }
                        else
                        {
                            ResultDictionary["结论"][i] = "合格";
                        }
                        }
                        else
                        {
                            if (float.Parse(readdata[i]) == -1.0)
                            {
                                ResultDictionary["结论"][i] = "不合格";
                                ResultDictionary["不合格原因"][i] = "读取" + paraStruct.ItemCode + "值不正确，返回："+readdata[i];
                            }
                            else
                            {
                                ResultDictionary["结论"][i] = "合格";
                            }
                            
                        }
                    }
                  
                }
                UploadTestResult("不合格原因");
                UploadTestResult("读(写)数据");
                UploadTestResult("结论");
            }
           
            
        }
        #endregion

        /// <summary>
        /// 格式化读字符串
        /// </summary>
        /// <param name="data"></param>
        /// <param name="strformat"></param>
        /// <returns></returns>
        private string FormatReadData(string data, string strformat)
        {
            if (data == "" || data == null) return "";
            string formatdata = data;
            bool blnIsNum = true;           //判断读取的数据是不是数字
            List<char> splitChar = new List<char>(new char[] { '.', '-', '#', '|', '@' });
            for (int i = 0; i < strformat.Length; i++)
            {
                if (Stop) return "";                   //假如当前停止检定，则退出
                if (splitChar.Contains(strformat[i]))
                {
                    formatdata = formatdata.Insert(i, strformat[i].ToString());
                    blnIsNum = false;
                }
                else if (strformat[i] == 'N')
                {
                    blnIsNum = false;
                }
            }
            if (blnIsNum) formatdata = float.Parse(formatdata).ToString();
            return formatdata;
        }

        /// <summary>
        /// 格式化写字符串
        /// </summary>
        /// <param name="data"></param>
        /// <param name="strformat"></param>
        /// <param name="len"></param>
        /// <param name="pointindex"></param>
        /// <returns></returns>
        private string FormatWriteData(string data, string strformat, int len, int pointindex)
        {
            string formatdata = "";
            try
            {
                if (data == "" || data == null) return "";
                formatdata = data;
                bool blnIsNum = true;           //判断读取的数据是不是数字
                List<char> splitChar = new List<char>(new char[] { '.', 'N' });
                for (int i = 0; i < strformat.Length; i++)
                {
                    if (!splitChar.Contains(strformat[i]))
                    {
                        blnIsNum = false;
                        break;
                    }
                }
                if (pointindex != 0)
                {
                    if (blnIsNum)
                    {
                        int left = len * 2 - pointindex;
                        int right = pointindex;
                        formatdata = float.Parse(formatdata).ToString();
                        string[] newdata = formatdata.Split('.');
                        if (newdata.Length == 1)
                        {
                            if (newdata[0].Length <= left)
                            {
                                newdata[0] = newdata[0].PadLeft(left, '0');
                            }
                            else
                            {
                                newdata[0] = newdata[0].Substring(0, left);
                            }
                            formatdata = newdata[0] + "".PadRight(right, '0');
                        }
                        else
                        {
                            if (newdata[0].Length <= left)
                            {
                                newdata[0] = newdata[0].PadLeft(left, '0');
                            }
                            else
                            {
                                newdata[0] = newdata[0].Substring(0, left);
                            }
                            if (newdata[1].Length <= right)
                            {
                                newdata[1] = newdata[1].PadRight(right, '0');
                            }
                            else
                            {
                                newdata[1] = newdata[1].Substring(0, right);
                            }
                            formatdata = newdata[0] + newdata[1];
                        }
                    }
                    else
                    {
                        formatdata = formatdata.Replace(".", "");
                        formatdata = formatdata.Replace("-", "");
                        if (formatdata.Length <= len * 2)
                        {
                            formatdata = formatdata.PadRight(len * 2, '0');
                        }
                        else
                        {
                            formatdata = formatdata.Substring(0, len * 2);
                        }
                    }
                }
                else
                {
                    if (formatdata.Length <= len * 2)
                    {
                        formatdata = formatdata.PadLeft(len * 2, '0');
                    }
                    else
                    {
                        formatdata = formatdata.Substring(0, len * 2);
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.LogHelper.Instance.WriteInfo(ex.StackTrace);
            }
            return formatdata;
        }

        
        
    }
}
