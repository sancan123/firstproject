using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Mesurement.UiLayer.Utility.Log;
using Mesurement.UiLayer.ViewModel.WcfService;
using System.Collections.Generic;
using System;
using Mesurement.UiLayer.ViewModel.Time;

namespace Mesurement.UiLayer.ViewModel.CheckController
{
    /// 检定控制器视图
    /// <summary>
    /// 检定控制器视图
    /// </summary>
    public class ControllerViewModel : ViewModelBase
    {
        /// <summary>
        /// 允许检定
        /// </summary>
        public bool IsEnable
        {
            get
            {
                return index >= 0 && EquipmentData.DeviceManager.IsReady;
            }
        }

        private int index;
        /// <summary>
        /// 当前检定点序号,逻辑里面的核心
        /// </summary>
        public int Index
        {
            get
            {
                if (index >= CheckCount)
                {
                    index = CheckCount - 1;
                }
                return index;
            }
            set
            {
                //将执行完毕的检定点设置为非检定状态
                if (EquipmentData.CheckResults.ResultCollection.Count > Index && Index >= 0)
                {
                    EquipmentData.CheckResults.ResultCollection[Index].IsCurrent = false;
                    EquipmentData.CheckResults.ResultCollection[Index].IsChecking = false;
                }
                SetPropertyValue(value, ref index, "StringCheckIndex");
                if (EquipmentData.CheckResults.ResultCollection.Count > Index && Index >= 0)
                {
                    EquipmentData.CheckResults.ResultCollection[Index].IsCurrent = true;
                    EquipmentData.CheckResults.CheckNodeCurrent = EquipmentData.CheckResults.ResultCollection[Index];
                }
                OnPropertyChanged("CheckCount");
                #region 加载检定参数
                if (Index >= 0 && Index < EquipmentData.Schema.ParaValues.Count)
                {
                    DynamicViewModel viewModel = EquipmentData.Schema.ParaValues[Index];
                    EquipmentData.Schema.ParaNo = viewModel.GetProperty("PARA_NO") as string;
                    string paraValue = viewModel.GetProperty("PARA_VALUE") as string;
                    var temp = from item in EquipmentData.Schema.ParaInfo.CheckParas select item.ParaDisplayName;
                    if (paraValue == null)
                    {
                        paraValue = "";
                    }
                    string[] tempValues = paraValue.Split('|');
                    stringPara = "检定参数";
                    List<string> listTemp = new List<string>();
                    for (int i = 0; i < temp.Count(); i++)
                    {
                        if (tempValues.Length > i)
                        {
                            listTemp.Add(string.Format("{0}:{1}", temp.ElementAt(i), tempValues[i]));
                        }
                    }
                    if (listTemp.Count > 0)
                    {
                        StringPara = "参数:" + string.Join(",", listTemp);
                    }
                    else
                    {
                        StringPara = "无参数";
                    }

                    CheckingName = EquipmentData.CheckResults.ResultCollection[Index].Name;
                }
                #endregion
                OnPropertyChanged("IsEnable");
                EquipmentData.LastCheckInfo.SaveCurrentCheckInfo();
                #region 时间统计
                if (index >= 0 && index < TimeMonitor.Instance.TimeCollection.ItemsSource.Count)
                {
                    TimeMonitor.Instance.TimeCollection.SelectedItem = TimeMonitor.Instance.TimeCollection.ItemsSource[index];
                }
                #endregion
            }
        }

        /// 检定点数量
        /// <summary>
        /// 检定点数量
        /// </summary>
        public int CheckCount
        {
            get
            {
                return EquipmentData.CheckResults.ResultCollection.Count;
            }
        }
        /// 检定点序号字符串
        /// <summary>
        /// 检定点序号字符串
        /// </summary>
        public string StringCheckIndex
        {
            get
            {
                if (Index == -1)
                {
                    return "参数录入";
                }
                else if (Index == -3)
                {
                    return "审核存盘";
                }
                return string.Format("({0}/{1})", index + 1, CheckCount);
            }
        }
        /// 当前检定点编号
        /// <summary>
        /// 当前检定点编号
        /// </summary>
        public string CurrentKey
        {
            get
            {
                if (Index >= 0 && Index < EquipmentData.Schema.ParaValues.Count)
                {
                    DynamicViewModel viewModel = EquipmentData.Schema.ParaValues[Index];
                    EquipmentData.Schema.ParaNo = viewModel.GetProperty("PARA_NO") as string;
                    return viewModel.GetProperty("PARA_KEY") as string;
                }
                else
                {
                    return "";
                }
            }
        }

        /// 手动结束检定标记
        /// <summary>
        /// 手动结束检定标记
        /// </summary>
        private bool flagHandStop = false;

        private EnumCheckMode checkMode = EnumCheckMode.连续模式;
        /// 检定模式
        /// <summary>
        /// 检定模式
        /// </summary>
        public EnumCheckMode CheckMode
        {
            get { return checkMode; }
            set { SetPropertyValue(value, ref checkMode, "CheckMode"); }
        }

        private bool isChecking;
        /// 是否正在检定
        /// <summary>
        /// 是否正在检定
        /// </summary>
        public bool IsChecking
        {
            get { return isChecking; }
            set
            {
                if (value != isChecking)
                {
                    isChecking = value;
                    if (isChecking && !isBusy)
                    {
                        Task.Factory.StartNew(() => VerifyProcess());
                    }
                    else
                    {
                        EquipmentData.CheckResults.ResultCollection[Index].IsChecking = false;
                        LogManager.AddMessage("停止检定!", EnumLogSource.检定业务日志, EnumLevel.InformationSpeech);
                        currentStepWaitHandle.Set();
                    }
                }
                OnPropertyChanged("IsChecking");
            }
        }

        private bool newArrived;
        /// 新消息到来
        /// <summary>
        /// 新消息到来
        /// </summary>
        public bool NewArrived
        {
            get { return newArrived; }
            set
            {
                SetPropertyValue(value, ref newArrived, "NewArrived");
            }
        }

        #region 检定过程控制
        /// 停止检定
        /// <summary>
        /// 停止检定
        /// </summary>
        public void StopVerify()
        {
            flagHandStop = true;
            //手动停止，时间计数无效
            TimeMonitor.Instance.ActiveCurrentItem(Index);
            TimeMonitor.Instance.FinishCurrentItem(Index);

            WcfHelper.Instance.StopVerify(CurrentKey);
            LogManager.AddMessage("请等待,正在停止检定台...", EnumLogSource.检定业务日志);
        }
        /// 单步检定
        /// <summary>
        /// 单步检定
        /// </summary>
        public void StepVerify()
        {
            CheckMode = EnumCheckMode.单步模式;
            IsChecking = true;
        }
        /// 连续检定
        /// <summary>
        /// 连续检定
        /// </summary>
        public void RunningVerify()
        {
            CheckMode = EnumCheckMode.连续模式;
            IsChecking = true;
        }
        /// 循环检定
        /// <summary>
        /// 循环检定
        /// </summary>
        public void LoopVerify()
        {
            IsChecking = true;
            CheckMode = EnumCheckMode.循环模式;
        }
        /// 当前检定项检定完毕
        /// <summary>
        /// 当前检定项检定完毕
        /// </summary>
        public void FinishCurrentStep()
        {
            string checkName = EquipmentData.CheckResults.ResultCollection[Index].Name;
            LogManager.AddMessage(string.Format("收到检定项:{0} 执行完毕通知.", checkName), EnumLogSource.检定业务日志);
            if (isBusy)
            {
                currentStepWaitHandle.Set();
            }
            else
            {
                IsChecking = false;
            }
        }
        #endregion

        #region 检定线程
        private AutoResetEvent currentStepWaitHandle = new AutoResetEvent(false);
        /// <summary>
        /// 检定执行过程
        /// </summary>
        private void VerifyProcess()
        {
            isBusy = true;
            while (IsChecking)
            {
                try
                {
                    if (Index < 0 || Index >= CheckCount)
                    {
                        IsChecking = false;
                        break;
                    }
                    if (EquipmentData.CheckResults.ResultCollection[Index].IsSelected)
                    {
                        //如果调用检定服务失败,退出循环
                        if (!InvokeVerifyService())
                        {
                            break;
                        }

                        #region 等待当前检定项结束
                        currentStepWaitHandle.Reset();
                        currentStepWaitHandle.WaitOne();
                        #endregion
                    }

                    #region 检定器将要执行的动作
                    //如果手动终止
                    if (flagHandStop)
                    {
                        LogManager.AddMessage("当前检定项被手动终止!", EnumLogSource.检定业务日志);
                        flagHandStop = false;
                        IsChecking = false;
                        break;
                    }
                    //统计检定项的时间
                    TimeMonitor.Instance.FinishCurrentItem(Index);

                    //根据检定模式判断将要执行的检定动作
                    switch (CheckMode)
                    {
                        case EnumCheckMode.单步模式:
                            IsChecking = false;
                            break;
                        case EnumCheckMode.连续模式:
                            //判断是否要执行下一个检定项
                            if (IsChecking && Index < CheckCount - 1 && Index >= 0)
                            {
                                //如果当前点是最后一个检定点
                                Index = Index + 1;
                            }
                            else
                            {
                                IsChecking = false;
                                LogManager.AddMessage(string.Format("检定项执行完毕,{0}.", EquipmentData.CheckResults.ResultCollection[Index].Name), EnumLogSource.检定业务日志);
                            }
                            break;
                        case EnumCheckMode.循环模式:
                            //啥都不要干,进入下一个循环
                            break;
                    }
                    #endregion
                }
                catch (Exception e)
                {
                    LogManager.AddMessage(string.Format("调用检定开始服务异常:{0}", e.Message), EnumLogSource.检定业务日志, EnumLevel.Error);
                    IsChecking = false;
                }
            }
            isBusy = false;
        }

        //正在忙碌
        private bool isBusy = false;

        private string stringPara;
        /// <summary>
        /// 检定参数字符串
        /// </summary>
        public string StringPara
        {
            get { return stringPara; }
            set { SetPropertyValue(value, ref stringPara, "StringPara"); }
        }
        #endregion

        private string checkingName;
        /// <summary>
        /// 当前检定项的名称
        /// </summary>
        public string CheckingName
        {
            get { return checkingName; }
            set { SetPropertyValue(value, ref checkingName, "CheckingName"); }
        }

        /// <summary>
        /// 检定客户端发起的正在检定
        /// </summary>
        public void NotifyIsChecking(string checkState)
        {
            if (checkState == "1")
            {
                isChecking = true;
            }
            else
            {
                //isChecking = false;
            }
            OnPropertyChanged("IsChecking");
        }

        /// <summary>
        /// 调用检定服务
        /// </summary>
        private bool InvokeVerifyService()
        {
            CheckInfo.CheckNodeViewModel nodeTemp = EquipmentData.CheckResults.ResultCollection[Index];
            //解析检定参数
            DynamicViewModel viewModel = EquipmentData.Schema.ParaValues[Index];
            if (viewModel != null)
            {
                EquipmentData.Schema.ParaNo = viewModel.GetProperty("PARA_NO") as string;
                string className = EquipmentData.Schema.ParaInfo.ClassName;
                var temp = from item in EquipmentData.Schema.ParaInfo.CheckParas select item.ParaDisplayName;
                string paraFormat = string.Join("|", temp);
                nodeTemp.IsChecking = true;
                string key = viewModel.GetProperty("PARA_KEY") as string;
                string paraValue = viewModel.GetProperty("PARA_VALUE") as string;
                //清除当前实时报文
                EquipmentData.CheckResults.ResultCollection[index].LiveFrames.ClearFrames();
                //如果是单步模式或者当前为最后一个检定点,则检定完毕以后关源
                bool resultStart = false;
                if (CheckMode == EnumCheckMode.单步模式 || (CheckMode == EnumCheckMode.连续模式 && Index == CheckCount - 1))
                {
                    resultStart = WcfHelper.Instance.StartVerify(key, className, paraFormat, paraValue, "1");
                }
                else
                {
                    resultStart = WcfHelper.Instance.StartVerify(key, className, paraFormat, paraValue, "0");
                }
                if (resultStart)
                {
                    TimeMonitor.Instance.StartCurrentItem(Index);
                    //清空当前的检定
                    if (CheckMode != EnumCheckMode.循环模式)
                    {
                        EquipmentData.CheckResults.ResetCurrentResult();
                    }
                }
                else
                {
                    IsChecking = false;
                    LogManager.AddMessage("开始检定服务调用失败!", EnumLogSource.检定业务日志, EnumLevel.ErrorSpeech);
                    return false;
                }
            }
            else
            {
                IsChecking = false;
                LogManager.AddMessage("检定过程出现异常,索引超出范围", EnumLogSource.检定业务日志, EnumLevel.ErrorSpeech);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取误差限
        /// </summary>
        /// <param name="itemKey"></param>
        /// <returns></returns>
        private string GetWcx(string itemKey)
        {
            string valueLimit = "";
            #region 获取表等级
            #endregion
            return valueLimit;
        }
    }
}
