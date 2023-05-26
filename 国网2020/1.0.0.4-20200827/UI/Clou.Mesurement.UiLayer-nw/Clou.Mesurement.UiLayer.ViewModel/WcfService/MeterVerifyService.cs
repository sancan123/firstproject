using System;
using Mesurement.UiLayer.VerifyService;
using Mesurement.UiLayer.Utility.Log;
using Mesurement.UiLayer.Utility;
using Mesurement.UiLayer.DAL;
using Mesurement.UiLayer.ViewModel.Device;

namespace Mesurement.UiLayer.ViewModel.WcfService
{
    /// <summary>
    /// 检表服务信息上报
    /// </summary>
    public class MeterVerifyService : IVerifyMessage
    {
        public bool OutMessage(int connectionId, int messageSourse, int messageType, string message)
        {
            EnumLogSource logSouce = EnumLogSource.检定业务日志;
            EnumLevel level = EnumLevel.Error;
            Enum.TryParse(messageType.ToString(), out level);
            if (Enum.TryParse(messageSourse.ToString(), out logSouce))
            {
                LogManager.AddMessage(message, logSouce, level);
            }
            else
            {
                LogManager.AddMessage(string.Format("检定模块未知来源的数据:{0}", message), EnumLogSource.检定业务日志, EnumLevel.Warning);
            }
            return true;
        }
        /// 更新检定结论
        /// <summary>
        /// 更新检定结论
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="itemKey">检定点编号</param>
        /// <param name="dataName">检定结论名称</param>
        /// <param name="dataValue">检定结论的值</param>
        /// <returns></returns>
        public bool OutVerifyData(int connectionId, string itemKey, string dataName, string[] dataValue)
        {
            EquipmentData.CheckResults.UpdateCheckResult(itemKey, dataName, dataValue);
            return true;
        }

        /// <summary>
        /// 获取检定数据
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="AVR_PROJECT_NAME">项目名称</param>
        /// <param name="arrayResult">检定数据</param>
        /// <returns></returns>

        public bool OutDataValue(int connectionId, string AVR_PROJECT_NAME, out string[] arrayResult)
        {
            EquipmentData.CheckResults.GetDataValue(AVR_PROJECT_NAME, out arrayResult);
            return true;
        }

        public bool OutOneMeterData(int connectionId, string itemKey, string dataName, int meterIndex, string dataValue)
        {
            return true;
        }
        /// <summary>
        /// 外发监视数据
        /// </summary>
        /// <param name="connectionID"></param>
        /// <param name="monitorType">监视类型：enum_MonitorType</param>
        /// <param name="analogArray"></param>
        /// <returns></returns>
        public bool OutMonitorInfo(int connectionId, int monitorType, string formatData)
        {
            try
            {
                if (!string.IsNullOrEmpty(formatData))
                {
                    EnumMonitorType enumType = (EnumMonitorType)monitorType;
                    switch (enumType)
                    {
                        case EnumMonitorType.MeterStandard:
                            #region  标准表信息
                            string[] analogArray = formatData.Split('|');
                            if (analogArray.Length > 0)
                            {
                                float[] floatArray = new float[analogArray.Length];
                                for (int i = 0; i < analogArray.Length; i++)
                                {
                                    float temp = 0;
                                    float.TryParse(analogArray[i], out temp);
                                    floatArray[i] = temp;
                                }
                                TaskManager.AddUIAction(() =>
                                {
                                    try
                                    {
                                        //EquipmentData.StdInfo;
                                        EquipmentData.StdInfo.Ua = floatArray[0];
                                        EquipmentData.StdInfo.Ub = floatArray[1];
                                        EquipmentData.StdInfo.Uc = floatArray[2];
                                        EquipmentData.StdInfo.Ia = floatArray[3];
                                        EquipmentData.StdInfo.Ib = floatArray[4];
                                        EquipmentData.StdInfo.Ic = floatArray[5];
                                        EquipmentData.StdInfo.PhaseUa = floatArray[6];
                                        EquipmentData.StdInfo.PhaseUb = floatArray[7];
                                        EquipmentData.StdInfo.PhaseUc = floatArray[8];
                                        EquipmentData.StdInfo.PhaseIa = floatArray[9];
                                        EquipmentData.StdInfo.PhaseIb = floatArray[10];
                                        EquipmentData.StdInfo.PhaseIc = floatArray[11];
                                        EquipmentData.StdInfo.PhaseA = floatArray[12];
                                        EquipmentData.StdInfo.PhaseB = floatArray[13];
                                        EquipmentData.StdInfo.PhaseC = floatArray[14];
                                        EquipmentData.StdInfo.Pa = floatArray[16];
                                        EquipmentData.StdInfo.Pb = floatArray[17];
                                        EquipmentData.StdInfo.Pc = floatArray[18];
                                        EquipmentData.StdInfo.P = floatArray[19];
                                        EquipmentData.StdInfo.Qa = floatArray[20];
                                        EquipmentData.StdInfo.Qb = floatArray[21];
                                        EquipmentData.StdInfo.Qa = floatArray[22];
                                        EquipmentData.StdInfo.Q = floatArray[23];
                                        EquipmentData.StdInfo.Sa = floatArray[24];
                                        EquipmentData.StdInfo.Sb = floatArray[25];
                                        EquipmentData.StdInfo.Sc = floatArray[26];
                                        EquipmentData.StdInfo.S = floatArray[27];
                                        EquipmentData.StdInfo.PF = floatArray[31];
                                        EquipmentData.StdInfo.Freq = floatArray[33];
                                    }
                                    catch
                                    {
                                        LogManager.AddMessage("标准表数据格式错误!", EnumLogSource.设备操作日志, EnumLevel.Warning);
                                    }
                                });
                            }
                            #endregion
                            break;
                        case EnumMonitorType.PressStatus:
                            #region 表上下位状态
                            string[] pressedArray = formatData.Split('|');
                            for (int i = 0; i < pressedArray.Length; i++)
                            {
                                if (i < EquipmentData.DeviceManager.MeterUnits.Count)
                                {
                                    if (pressedArray[i] != null && pressedArray[i].Length == 3)
                                    {
                                        EquipmentData.DeviceManager.MeterUnits[i].ConvertUnitStatus(pressedArray[i]);
                                    }
                                }
                            }
                            #endregion
                            break;
                        case EnumMonitorType.ErrorBoard:
                            #region 误差板屏显
                            string[] errorArray = formatData.Split('|');
                            for (int i = 0; i < errorArray.Length; i++)
                            {
                                if (i < EquipmentData.DeviceManager.MeterUnits.Count)
                                {
                                    string[] arrayTemp = errorArray[i].Split(',');
                                    EquipmentData.DeviceManager.MeterUnits[i].TextScreen = arrayTemp[0];
                                    if (arrayTemp.Length > 1)
                                    {
                                        if (arrayTemp[1] == "1")
                                        {
                                            EquipmentData.DeviceManager.MeterUnits[i].IsOverFlow = true;
                                        }
                                        else
                                        {
                                            EquipmentData.DeviceManager.MeterUnits[i].IsOverFlow = false;
                                        }
                                    }
                                    else
                                    {
                                        EquipmentData.DeviceManager.MeterUnits[i].IsOverFlow = false;
                                    }
                                }
                            }
                            #endregion
                            break;
                        case EnumMonitorType.Frame:
                            #region 帧数据收发
                            string[] arrayFrame = formatData.Split('|');
                            if (arrayFrame.Length == 2)
                            {
                                PortUnit port = ServersViewModel.Instance.FindPort(arrayFrame[0]);
                                if (port != null)
                                {
                                    if (arrayFrame[1] == "0")
                                    {
                                        if (port.NewSend)
                                        {
                                            port.NewSend = false;
                                        }
                                        port.NewSend = true;
                                    }
                                    if (arrayFrame[1] == "1")
                                    {
                                        if (port.NewReceived)
                                        {
                                            port.NewReceived = false;
                                        }
                                        port.NewReceived = true;
                                    }
                                }
                            }
                            #endregion
                            break;
                    }
                }
            }
            catch
            {
                LogManager.AddMessage(string.Format("不识别的监视器数据类型:{0},数据:{1}", monitorType, formatData), EnumLogSource.用户操作日志, EnumLevel.Warning);
            }
            return true;
        }

        public bool VerifyFinished(int connectionId)
        {
            EquipmentData.Controller.FinishCurrentStep();
            return true;
        }
        /// 帧消息
        /// <summary>
        /// 帧消息
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="frameString">帧日志</param>
        /// <returns></returns>
        public bool OutFrame(int connectionId, string frameString)
        {
            if (string.IsNullOrEmpty(frameString))
            {
                return false;
            }
            string[] arrayFrame = frameString.Split('^');
            if (arrayFrame.Length == 11)
            {
                DynamicModel model = new DynamicModel();
                model.SetProperty("chrPortNo", arrayFrame[0]);
                model.SetProperty("chrEquipName", arrayFrame[1]);
                //这里可能会有延时
                //model.SetProperty("chrItemName", arrayFrame[2]);
                model.SetProperty("chrItemName", EquipmentData.Controller.CheckingName);
                model.SetProperty("chrMessage", arrayFrame[3]);
                model.SetProperty("chrSFrame", arrayFrame[4]);
                model.SetProperty("chrSMeaning", arrayFrame[5]);
                model.SetProperty("chrSTime", arrayFrame[6]);
                model.SetProperty("chrRFrame", arrayFrame[7]);
                model.SetProperty("chrRMeaning", arrayFrame[8]);
                model.SetProperty("chrRTime", arrayFrame[9]);
                model.SetProperty("chrOther", arrayFrame[10]);
                #region 添加到数据库
                //表报文日志存储到表报文库里面
                //if (!string.IsNullOrEmpty(arrayFrame[1]) && arrayFrame[1].Contains("MeterProtocol"))
                //{
                //    DALManager.LogDal.Insert("FrameMeterLog", model);
                //}
                ////标准表的不存储到数据库
                //else 
                if (!arrayFrame[1].Contains("311"))
                {
                    DALManager.LogDal.Insert("FrameDeviceLog", model);
                }
                #endregion
                if (arrayFrame[1].Contains("MeterProtocol"))
                {
                    if(EquipmentData.CheckResults.ResultCollection.Count>EquipmentData.Controller.Index && EquipmentData.Controller.Index>=0)
                    {
                        CheckInfo.CheckNodeViewModel nodeCurrent = EquipmentData.CheckResults.ResultCollection[EquipmentData.Controller.Index];
                        nodeCurrent.LiveFrames.AddFrame(model);
                    }
                }
            }
            return true;
        }
        private static int idTemp = 0;
        /// 客户端请求创立连接
        /// <summary>
        /// 客户端请求创立连接
        /// </summary>
        /// <returns></returns>
        public int RequestId()
        {
            Random random = new Random();
            if (idTemp == 0)
            {
                idTemp = random.Next(23, 8888888);
            }
            TaskManager.AddWcfAction(() => WcfHelper.Instance.InitialControlClient());
            return idTemp;
        }

        public bool NotifyIsChecking(int connectionId, string checkState)
        {
            //if (connectionId == idTemp)
            {
                EquipmentData.Controller.NotifyIsChecking(checkState);
                return true;
            }
            //return false;
        }
    }
}
