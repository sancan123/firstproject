
using System;
using CLDC_DataCore;
using System.Collections.Generic;
using System.Threading;
using CLDC_DataCore.Const;
using CLDC_DataCore.Struct;
using CLDC_Comm.Enum;
using CLDC_DataCore.Model.DnbModel.DnbInfo;


namespace CLDC_VerifyAdapter
{
    /// <summary>
    /// 通讯协议检查试验
    /// 只记录当前方向结论
    /// </summary>
    class ConnProtocol : VerifyBase
    {
        //检测系统参照规程
        float[] arrCreepTimes = new float[0];

        public ConnProtocol(object plan)
            : base(plan)
        {
            ResultNames = new string[] { "标识名称", "标识编码", "标识长度", "数据格式", "读数据", "试验类型", "结论" };
        }

        #region 解析并校验检定参数
        /// <summary>
        /// 解析并校验检定参数
        /// </summary>
        /// <returns></returns>
        protected override bool CheckPara()
        {
            string[] arrayPara = VerifyPara.Split('|');
            if (arrayPara.Length < 7)
            {
                MessageController.Instance.AddMessage(string.Format("通讯协议检查参数错误,参数长度应不小于6,参数:{0}", VerifyPara), 6, 2);
                return false;
            }
            paraStruct.ConnProtocolItem = arrayPara[0];
            paraStruct.ItemCode = arrayPara[1];
            int lengthTemp = 0;
            if (int.TryParse(arrayPara[2], out lengthTemp))
            {
                paraStruct.DataLen = lengthTemp;
            }
            else
            {
                MessageController.Instance.AddMessage(string.Format("通讯协议检查参数错误,第三个参数数据标识长度解析错误,参数:{0}", VerifyPara), 6, 2);
                return false;
            }
            if (int.TryParse(arrayPara[3], out lengthTemp))
            {
                paraStruct.PointIndex = lengthTemp;
            }
            else
            {
                MessageController.Instance.AddMessage(string.Format("通讯协议检查参数错误,第四个参数数据标识小数位数解析错误,参数:{0}", VerifyPara), 6, 2);
                return false;
            }
            paraStruct.StrDataType = arrayPara[4];
            if (arrayPara[5] == "读")
            {
                paraStruct.OperType = StMeterOperType.读;
            }
            else if (arrayPara[5] == "写")
            {
                paraStruct.OperType = StMeterOperType.写;
            }
            else
            {
                MessageController.Instance.AddMessage(string.Format("通讯协议检查参数错误,第六个参数应为'读'或'写',参数:{0}", VerifyPara), 6, 2);
                return false;
            }
            paraStruct.WriteContent = arrayPara[6];
            return true;
        }
        #endregion

        #region 初始化设备参数:InitEquipment
        /// <summary>
        /// 初始化设备参数,计算每一块表需要检定的圈数
        /// </summary>
        /// <param name="_curPoint"></param>
        /// <param name="PL"></param>
        /// <param name="_Pulselap"></param>
        /// <returns></returns>
        private bool InitEquipment()
        {
            if (GlobalUnit.IsDemo) return true;
            //MessageController.Instance.AddMessage("设置表开关!");
            //Helper.EquipHelper.Instance.SetMeterOnOff(Helper.MeterDataHelper.Instance.GetYaoJian());

            if (Stop) return false;                   //假如当前停止检定，则退出
            MessageController.Instance.AddMessage("停止当前设置的功能!");
            Helper.EquipHelper.Instance.SetCurFunctionOnOrOff(true,0);
            if (Stop) return false;                   //假如当前停止检定，则退出
            MessageController.Instance.AddMessage("开始控制源输出!");
            bool result = PowerOn();
            if (!result)
            {
                MessageController.Instance.AddMessage("控制源输出失败", 6, 2);
                return false;
            }
            return true;
        }
        #endregion

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
            MessageController.Instance.AddMessage(string.Format("开始控制功率源输出:{0}V {1}A", GlobalUnit.U, 0));
            bool ret = Helper.EquipHelper.Instance.PowerOn(GlobalUnit.U,
                                  0,
                                  (int)ele, (int)PowerFangXiang,
                                  FangXiangStr + "1.0",
                                  true, false);
            MessageController.Instance.AddMessage(string.Format("控制功率源输出:{0}V {1}A {2}", GlobalUnit.U, 0, ret ? "成功" : "失败"), 6, ret ? 0 : 2);
            return ret;
        }

        #endregion

        #region ----------开始检定----------
        /// <summary>
        /// 开始检定
        /// </summary>
        public override void Verify()
        {
            for (int i = 0; i < BwCount; i++)
            {
                MeterBasicInfo meterTemp = Helper.MeterDataHelper.Instance.Meter(i);
                if (meterTemp.YaoJianYn)
                {
                    ResultDictionary["标识名称"][i] = paraStruct.ConnProtocolItem;
                    ResultDictionary["标识编码"][i] = paraStruct.ItemCode;
                    ResultDictionary["标识长度"][i] = paraStruct.DataLen.ToString();
                    ResultDictionary["数据格式"][i] = paraStruct.StrDataType;
                    ResultDictionary["试验类型"][i] = paraStruct.OperType.ToString();
                }
            }
            UploadTestResult("标识名称");
            UploadTestResult("标识编码");
            UploadTestResult("标识长度");
            UploadTestResult("数据格式");
            UploadTestResult("试验类型");

            base.Verify();                                                                  //调用基类的检定方法，执行默认操作

            Helper.LogHelper.Instance.WriteInfo("初始化设备参数...");
            Adapter.Instance.UpdateMeterProtocol();
            //初始化设备
            bool resultInitEquip = InitEquipment();
            if (!resultInitEquip)
            {
                MessageController.Instance.AddMessage("初始化基本误差设备参数失败", 6, 2);
                return;
            }
            if (Stop) return;                   //假如当前停止检定，则退出

            if (GlobalUnit.IsDemo) return;
            if (Stop) return;                   //假如当前停止检定，则退出
            if (paraStruct.OperType == StMeterOperType.读)
            {
                string[] readdata = new string[BwCount];
                if (paraStruct.PointIndex > 0)
                {
                    float[] tmpdata = MeterProtocolAdapter.Instance.ReadData(paraStruct.ItemCode, paraStruct.DataLen, paraStruct.PointIndex);
                    readdata = CLDC_DataCore.Function.ConvertArray.ConvertFloat2Str(tmpdata);
                }
                else
                {
                    readdata = MeterProtocolAdapter.Instance.ReadData(paraStruct.ItemCode, paraStruct.DataLen);
                }
                //结论数据
                for (int i = 0; i < BwCount; i++)
                {
                    if (Stop) return;                   //假如当前停止检定，则退出
                    MeterBasicInfo _Meter = Helper.MeterDataHelper.Instance.Meter(i);
                    if (!_Meter.YaoJianYn) continue;
                    ResultDictionary["读数据"][i] = readdata[i];
                    ResultDictionary["结论"][i] = string.IsNullOrEmpty(readdata[i]) ? "不合格" : "合格";
                }
                UploadTestResult("读数据");
                UploadTestResult("结论");
            }
            else
            {
                string writedata = FormatWriteData(paraStruct.WriteContent, paraStruct.StrDataType, paraStruct.DataLen, paraStruct.PointIndex);
                bool[] bResult = MeterProtocolAdapter.Instance.WriteData(paraStruct.ItemCode, paraStruct.DataLen, writedata);
                ConvertTestResult("结论", bResult);
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

        private StPlan_ConnProtocol paraStruct = new StPlan_ConnProtocol();

        protected override string ItemKey
        {
            get { throw new NotImplementedException(); }
        }

        protected override string ResultKey
        {
            get { throw new NotImplementedException(); }
        }
    }
}
