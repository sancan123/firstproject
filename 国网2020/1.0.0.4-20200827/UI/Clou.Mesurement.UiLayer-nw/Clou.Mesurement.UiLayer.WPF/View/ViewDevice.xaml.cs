using Mesurement.UiLayer.Utility.Log;
using Mesurement.UiLayer.ViewModel;
using Mesurement.UiLayer.ViewModel.Const;
using Mesurement.UiLayer.ViewModel.Device;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Text.RegularExpressions;
using System.IO;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// ViewService.xaml 的交互逻辑
    /// </summary>
    public partial class ViewDevice
    {
        private ObservableCollection<MeterRow> meterRows = new ObservableCollection<MeterRow>();
        public ViewDevice()
        {
            InitializeComponent();
            Name = "设备操作";
            DataContext = EquipmentData.DeviceManager;
            LoadParameterData();
            
        }

        private void Viewbox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Control controlCurrent = sender as Control;
                MeterUnitViewModel meterUnit = controlCurrent.DataContext as MeterUnitViewModel;
                if (meterUnit != null)
                {
                    meterUnit.IsSelected = !meterUnit.IsSelected;
                }
            }
        }
        public override void Dispose()
        {
            //slider.ValueChanged -= slider_ValueChanged;
            base.Dispose();
        }


        private void BtnSetAlertingMoney1_Click(object sender, RoutedEventArgs e)
        {
            SetAlertingMoney1();
        }

        private void SetAlertingMoney1()
        {
            string AlertingMoney1 = this.AlertingMoney1.Text;
            LogManager.AddMessage("正在准备设置报警金额1为：" + AlertingMoney1, EnumLogSource.检定业务日志);
            EquipmentData.DeviceManager.SetAlertingMoney1(AlertingMoney1);
        }

        private void BtnSetAlertingMoney2_Click(object sender, RoutedEventArgs e)
        {
            SetAlertingMoney2();
        }

        private void SetAlertingMoney2()
        {
            string AlertingMoney2 = this.AlertingMoney2.Text;
            LogManager.AddMessage("正在准备设置报警金额2为：" + AlertingMoney2, EnumLogSource.检定业务日志);
            EquipmentData.DeviceManager.SetAlertingMoney2(AlertingMoney2);
        }

        private void SetCurrentScale_Click(object sender, RoutedEventArgs e)
        {
            SetCurrentScale();
        }

        private void SetCurrentScale()
        {
            string CurrentScale = this.CurrentScale.Text;
            LogManager.AddMessage("正在准备设置电流互感器变比为：" + CurrentScale, EnumLogSource.检定业务日志);
            EquipmentData.DeviceManager.SetCurrentScale(CurrentScale);
        }

        private void SetIdentityTime_Click(object sender, RoutedEventArgs e)
        {
            SetIdentityTime();
        }

        private void SetIdentityTime()
        {
            string IdentityTime = this.IdentityTime.Text;
            LogManager.AddMessage("正在准备设置身份认证时效为：" + IdentityTime, EnumLogSource.检定业务日志);
            EquipmentData.DeviceManager.SetIdentityTime(IdentityTime);
        }

        private void SetVoltageScale_Click(object sender, RoutedEventArgs e)
        {
            SetVoltageScale();
        }

        private void SetVoltageScale()
        {
            string VoltageScale = this.VoltageScale.Text;
            LogManager.AddMessage("正在准备设置电压互感器变比为：" + VoltageScale, EnumLogSource.检定业务日志);
            EquipmentData.DeviceManager.SetVoltageScale(VoltageScale);
        }


        private void AllWrite_Click(object sender, RoutedEventArgs e)
        {
            if (chk_BjMoney1.IsChecked == true)
            {
                SetAlertingMoney1();
            }
            if (chk_BjMoney2.IsChecked == true)
            {
                SetAlertingMoney2();
            }
            if (chk_CurrentScale.IsChecked == true)
            {
                SetCurrentScale();
            }
            if (chk_VoltageScale.IsChecked == true)
            {
                SetVoltageScale();
            }
            if (chk_IdentityTime.IsChecked == true)
            {
                SetIdentityTime();
            }
        }

        public class MeterRow : ViewModelBase
        {
            private int columnCount;

            public int ColumnCount
            {
                get { return columnCount; }
                set { SetPropertyValue(value, ref columnCount, "ColumnCount"); }
            }

            private int rowIndex;

            public int RowIndex
            {
                get { return rowIndex; }
                set
                {
                    SetPropertyValue(value, ref rowIndex, "RowIndex");
                    OnPropertyChanged("Text");
                }
            }

            public string Text
            {
                get
                {
                    int startIndex = rowIndex * columnCount + 1;
                    int endIndex = (rowIndex + 1) * columnCount;
                    if (endIndex > EquipmentData.DeviceManager.MeterUnits.Count)
                    {
                        endIndex = EquipmentData.DeviceManager.MeterUnits.Count;
                    }
                    return string.Format("表位{0}-{1}", startIndex, endIndex);
                }

            }


            private bool isSelected;

            public bool IsSelected
            {
                get { return isSelected; }
                set
                {
                    SetPropertyValue(value, ref isSelected, "IsSelected");
                    for (int i = ColumnCount * rowIndex; i < columnCount * (rowIndex + 1); i++)
                    {
                        if (i < EquipmentData.DeviceManager.MeterUnits.Count)
                        {
                            EquipmentData.DeviceManager.MeterUnits[i].IsSelected = value;
                        }
                    }
                }
            }
        }

        private void WriteFl_Click(object sender, RoutedEventArgs e)
        {
            WriteFl();
        }

        private void WriteFl()
        {
            string strFl = this.Fl1.Text + "," + this.Fl2.Text + "," + this.Fl3.Text + "," + this.Fl4.Text;
            EquipmentData.DeviceManager.SetFl(strFl);
        }

        private void WriteJtBy1_Click(object sender, RoutedEventArgs e)
        {
            WriteJtBy1();
        }

        private void WriteJtBy1()
        {
            string strPara = this.Jtz1by1.Text + "," + this.Jtz2by1.Text + "," + this.Jtz3by1.Text + "," + this.Jtz4by1.Text + "," + this.Jtz5by1.Text + "," + this.Jtz6by1.Text + ",";
            strPara += this.Jtdj1By1.Text + "," + this.Jtdj2By1.Text + "," + this.Jtdj3By1.Text + "," + this.Jtdj4By1.Text + "," + this.Jtdj5By1.Text + "," + this.Jtdj6By1.Text + "," + this.Jtdj7By1.Text + ",";
            strPara += this.Jsr1by1.Text+ this.Jsr2by1.Text + this.Jsr3by1.Text + this.Jsr4by1.Text + this.Jsr5by1.Text + this.Jsr6by1.Text;

            EquipmentData.DeviceManager.SetJtBy1(strPara);
        }

        private void WriteJtBy2_Click(object sender, RoutedEventArgs e)
        {
            WriteJtBy2();
        }

        private void WriteJtBy2()
        {
            string strPara = this.Jtz1by2.Text + "," + this.Jtz2by2.Text + "," + this.Jtz3by2.Text + "," + this.Jtz4by2.Text + "," + this.Jtz5by2.Text + "," + this.Jtz6by2.Text + ",";
            strPara += this.Jtdj1By2.Text + "," + this.Jtdj2By2.Text + "," + this.Jtdj3By2.Text + "," + this.Jtdj4By2.Text + "," + this.Jtdj5By2.Text + "," + this.Jtdj6By2.Text + "," + this.Jtdj7By2.Text + ",";
            strPara += this.Jsr1by2.Text + this.Jsr2by2.Text + this.Jsr3by2.Text + this.Jsr4by2.Text + this.Jsr5by2.Text + this.Jsr6by2.Text;

            EquipmentData.DeviceManager.SetJtBy2(strPara);
        }

        private void SetInitPurse_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("请确保表计在本地模式、测试密钥状态。", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string strMoney = this.InitPurse.Text;
                EquipmentData.DeviceManager.SetInitPurse(strMoney);
            }
        }


        private void WriteChangFlTime_Click(object sender, RoutedEventArgs e)
        {
            WriteChangFlTime();
        }

        private void WriteChangFlTime()
        {
            string strChangFlTime = this.ChangFlTime.Text;
            EquipmentData.DeviceManager.SetChangFlTime(strChangFlTime);
        }

        private void WriteChangJtTime_Click(object sender, RoutedEventArgs e)
        {
            WriteChangJtTime();
        }

        private void WriteChangJtTime()
        {
            string strChangJtTime = this.ChangJtTime.Text;
            EquipmentData.DeviceManager.SetChangJtTime(strChangJtTime);
        }

        private void LostTextFocusBy8(object sender, RoutedEventArgs e)
        {
            //string strInput = (sender as TextBox).Text;
            //if (!Regex.IsMatch(strInput, @"^\d{8}$"))
            //{
            //    System.Windows.MessageBox.Show("输入格式有误，请输入8位整数。");
            //    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
            //    {
            //        (sender as TextBox).Focus();
            //    }));
            //}
        }
        private void LostTextFocusBy10(object sender, RoutedEventArgs e)
        {
            //string strInput = (sender as TextBox).Text;
            //if (!Regex.IsMatch(strInput, @"^\d{10}$"))
            //{
            //    System.Windows.MessageBox.Show("输入格式有误，请输入10位整数。");
            //    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
            //    {
            //        (sender as TextBox).Focus();
            //    }));
            //}
        }

        private void LostTextFocusBy6(object sender, RoutedEventArgs e)
        {
            //string strInput = (sender as TextBox).Text;
            //if (!Regex.IsMatch(strInput, @"^\d{6}$"))
            //{
            //    System.Windows.MessageBox.Show("输入格式有误，请输入6位整数。");
            //    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
            //    {
            //        (sender as TextBox).Focus();
            //    }));
            //}
        }

        private void LostTextFocusBy4(object sender, RoutedEventArgs e)
        {
            //string strInput = (sender as TextBox).Text;
            //if (!Regex.IsMatch(strInput, @"^\d{4}$"))
            //{
            //    System.Windows.MessageBox.Show("输入格式有误，请输入4位整数。");
            //    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
            //    {
            //        (sender as TextBox).Focus();
            //    }));
            //}
        }

        private void CheckValue(object sender, RoutedEventArgs e)
        {
            string strInput = (sender as TextBox).Text;

            if (!CheckNnmber(strInput))
            {
                System.Windows.MessageBox.Show("输入格式有误，请最多输入6位整数、两位小数。");
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
                {
                    (sender as TextBox).Focus();
                }));
            }
        }

        public bool CheckNnmber(string Number)
        {
            if (Number.Length <= 9)
            {
                if (Number.Contains("."))
                {
                    string[] tmp = Number.Split('.');
                    if (tmp[0].Length <= 6 && tmp[1].Length <= 2)
                    {
                        for (int i = 0; i < tmp[0].Length; i++)
                        {
                            if (!Char.IsNumber(tmp[0], i))
                            {
                                return false;
                            }
                        }
                        for (int i = 0; i < tmp[1].Length; i++)
                        {
                            if (!Char.IsNumber(tmp[1], i))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }
                else
                {
                    for (int i = 0; i < Number.Length; i++)
                    {
                        if (!Char.IsNumber(Number, i))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        private void CheckValue4(object sender, RoutedEventArgs e)
        {
            //string strInput = (sender as TextBox).Text;
            //bool IsOk = false;
            //if (strInput.Length <= 4)
            //{
            //    for (int i = 0; i < strInput.Length; i++)
            //    {
            //        if (!Char.IsNumber(strInput, i))
            //        {
            //            break;
            //        }
            //        IsOk = true;
            //    }
            //}

            //if (!IsOk)
            //{

            //    System.Windows.MessageBox.Show("输入格式有误，请最多输入4位整数");
            //    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
            //    {
            //        (sender as TextBox).Focus();
            //    }));
            //}
        }

        private void CheckValue6(object sender, RoutedEventArgs e)
        {
            //string strInput = (sender as TextBox).Text;
            //bool IsOk = false;
            //if (strInput.Length <= 6)
            //{
            //    for (int i = 0; i < strInput.Length; i++)
            //    {
            //        if (!Char.IsNumber(strInput, i))
            //        {
            //            break;
            //        }
            //        IsOk= true;
            //    }
            //}

            //if (!IsOk)
            //{

            //    System.Windows.MessageBox.Show("输入格式有误，请最多输入6位整数");
            //    this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
            //    {
            //        (sender as TextBox).Focus();
            //    }));
            //}
        }


        private void AllWriteDj_Click(object sender, RoutedEventArgs e)
        {
            if (chk_WriteFl.IsChecked == true)
            {
                WriteFl();
            }
            if (chk_WriteJtb1.IsChecked == true)
            {
                WriteJtBy1();
            }
            if (chk_WriteJtb2.IsChecked == true)
            {
                WriteJtBy2();
            }
            if (chk_WriteChangTime.IsChecked == true)
            {
                WriteChangFlTime();
                WriteChangJtTime();
            }
        }

        private void SaveParameterData_Click(object sender, RoutedEventArgs e)
        {
            
            string AlertingMoney1 = this.AlertingMoney1.Text;
            string AlertingMoney2 = this.AlertingMoney2.Text;
            string CurrentScale = this.CurrentScale.Text;
            string VoltageScale = this.VoltageScale.Text;
            string IdentityTime = this.IdentityTime.Text;
            string InitPurse = this.InitPurse.Text;

            string strPath = Directory.GetCurrentDirectory();
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("ParameterData", "AlertingMoney1", AlertingMoney1, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("ParameterData", "AlertingMoney2", AlertingMoney2, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("ParameterData", "CurrentScale", CurrentScale, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("ParameterData", "VoltageScale", VoltageScale, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("ParameterData", "IdentityTime", IdentityTime, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("ParameterData", "InitPurse", InitPurse, strPath + "\\ParameterData.ini");

            LogManager.AddMessage("保存成功", EnumLogSource.用户操作日志, EnumLevel.ErrorSpeech);
        }

        private void SavePriceData_Click(object sender, RoutedEventArgs e)
        {
            string strPath = Directory.GetCurrentDirectory();
            //备用套 费率1-4
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Fl1", this.Fl1.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Fl2", this.Fl2.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Fl3", this.Fl3.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Fl4", this.Fl4.Text, strPath + "\\ParameterData.ini");

            //备用套第1阶梯表 阶梯值1-6
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtz1by1", this.Jtz1by1.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtz2by1", this.Jtz2by1.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtz3by1", this.Jtz3by1.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtz4by1", this.Jtz4by1.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtz5by1", this.Jtz5by1.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtz6by1", this.Jtz6by1.Text, strPath + "\\ParameterData.ini");

            //备用套第1阶梯表 阶梯电价1-7
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtdj1By1", this.Jtdj1By1.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtdj2By1", this.Jtdj2By1.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtdj3By1", this.Jtdj3By1.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtdj4By1", this.Jtdj4By1.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtdj5By1", this.Jtdj5By1.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtdj6By1", this.Jtdj6By1.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtdj7By1", this.Jtdj7By1.Text, strPath + "\\ParameterData.ini");

            //备用套第1阶梯表 结算日1-6
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jsr1by1", this.Jsr1by1.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jsr2by1", this.Jsr2by1.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jsr3by1", this.Jsr3by1.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jsr4by1", this.Jsr4by1.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jsr5by1", this.Jsr5by1.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jsr6by1", this.Jsr6by1.Text, strPath + "\\ParameterData.ini");

            //备用套第2阶梯表 阶梯值1-6
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtz1by2", this.Jtz1by2.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtz2by2", this.Jtz2by2.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtz3by2", this.Jtz3by2.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtz4by2", this.Jtz4by2.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtz5by2", this.Jtz5by2.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtz6by2", this.Jtz6by2.Text, strPath + "\\ParameterData.ini");

            //备用套第2阶梯表 阶梯电价1-7
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtdj1By2", this.Jtdj1By2.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtdj2By2", this.Jtdj2By2.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtdj3By2", this.Jtdj3By2.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtdj4By2", this.Jtdj4By2.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtdj5By2", this.Jtdj5By2.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtdj6By2", this.Jtdj6By2.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jtdj7By2", this.Jtdj7By2.Text, strPath + "\\ParameterData.ini");

            //备用套第2阶梯表 结算日1-6
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jsr1by2", this.Jsr1by2.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jsr2by2", this.Jsr2by2.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jsr3by2", this.Jsr3by2.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jsr4by2", this.Jsr4by2.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jsr5by2", this.Jsr5by2.Text, strPath + "\\ParameterData.ini");
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "Jsr6by2", this.Jsr6by2.Text, strPath + "\\ParameterData.ini");

            //两套费率电价切换时间
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "ChangFlTime", this.ChangFlTime.Text, strPath + "\\ParameterData.ini");
            //两套阶梯切换时间
            Mesurement.UiLayer.ViewModel.Const.OperateFile.WriteIni("PriceData", "ChangJtTime", this.ChangJtTime.Text, strPath + "\\ParameterData.ini");

            LogManager.AddMessage("保存成功", EnumLogSource.用户操作日志, EnumLevel.ErrorSpeech);
        }

        public void LoadParameterData()
        {

          this.AlertingMoney1.Text =  Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "ParameterData", "AlertingMoney1", "0");
          this.AlertingMoney2.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "ParameterData", "AlertingMoney2", "0");
          this.CurrentScale.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "ParameterData", "CurrentScale", "1");
          this.VoltageScale.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "ParameterData", "VoltageScale", "1");
          this.IdentityTime.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "ParameterData", "IdentityTime", "30");
          this.InitPurse.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "ParameterData", "InitPurse", "50");

          //备用套 费率1-4
          this.Fl1.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Fl1", "1");
          this.Fl2.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Fl2", "2");
          this.Fl3.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Fl3", "3");
          this.Fl4.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Fl4", "4");

          //备用套第1阶梯表 阶梯值1-6
          this.Jtz1by1.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtz1by1", "1");
          this.Jtz2by1.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtz2by1", "2");
          this.Jtz3by1.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtz3by1", "3");
          this.Jtz4by1.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtz4by1", "4");
          this.Jtz5by1.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtz5by1", "5");
          this.Jtz6by1.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtz6by1", "6");

          //备用套第1阶梯表 阶梯电价1-7
          this.Jtdj1By1.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtdj1By1", "1");
          this.Jtdj2By1.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtdj2By1", "2");
          this.Jtdj3By1.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtdj3By1", "3");
          this.Jtdj4By1.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtdj4By1", "4");
          this.Jtdj5By1.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtdj5By1", "5");
          this.Jtdj6By1.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtdj6By1", "6");
          this.Jtdj7By1.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtdj7By1", "7");

          //备用套第1阶梯表 结算日1-6
          this.Jsr1by1.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jsr1by1", "000000");
          this.Jsr2by1.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jsr2by1", "000000");
          this.Jsr3by1.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jsr3by1", "000000");
          this.Jsr4by1.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jsr4by1", "000000");
          this.Jsr5by1.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jsr5by1", "000000");
          this.Jsr6by1.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jsr6by1", "000000");

          //备用套第2阶梯表 阶梯值1-6
          this.Jtz1by2.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtz1by2", "1");
          this.Jtz2by2.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtz2by2", "2");
          this.Jtz3by2.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtz3by2", "3");
          this.Jtz4by2.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtz4by2", "4");
          this.Jtz5by2.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtz5by2", "5");
          this.Jtz6by2.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtz6by2", "6");

          //备用套第2阶梯表 阶梯电价1-7
          this.Jtdj1By2.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtdj1By2", "1");
          this.Jtdj2By2.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtdj2By2", "2");
          this.Jtdj3By2.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtdj3By2", "3");
          this.Jtdj4By2.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtdj4By2", "4");
          this.Jtdj5By2.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtdj5By2", "5");
          this.Jtdj6By2.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtdj6By2", "6");
          this.Jtdj7By2.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jtdj7By2", "7");

          //备用套第2阶梯表 结算日1-6
          this.Jsr1by2.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jsr1by2", "000000");
          this.Jsr2by2.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jsr2by2", "000000");
          this.Jsr3by2.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jsr3by2", "000000");
          this.Jsr4by2.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jsr4by2", "000000");
          this.Jsr5by2.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jsr5by2", "000000");
          this.Jsr6by2.Text = Mesurement.UiLayer.ViewModel.Const.OperateFile.ReadInIString(Mesurement.UiLayer.ViewModel.Const.OperateFile.GetPhyPath("ParameterData.ini"), "PriceData", "Jsr6by2", "000000");


        }

        private void AllReader_Click(object sender, RoutedEventArgs e)
        {
            if (chk_BjMoney1.IsChecked == true)
            {
                EquipmentData.DeviceManager.ReadAlertingMoney1();
            }
            if (chk_BjMoney2.IsChecked == true)
            {
                EquipmentData.DeviceManager.ReadAlertingMoney2();
            }
            if (chk_CurrentScale.IsChecked == true)
            {
                EquipmentData.DeviceManager.ReadCurrentScale();
            }
            if (chk_VoltageScale.IsChecked == true)
            {
                EquipmentData.DeviceManager.ReadVoltageScale();
            }
            if (chk_IdentityTime.IsChecked == true)
            {
                EquipmentData.DeviceManager.ReadIdentityTime();
            }
        }

        private void AllReaderDj_Click(object sender, RoutedEventArgs e)
        {
            if (chk_WriteFl.IsChecked == true)
            {
                EquipmentData.DeviceManager.ReaderFl();
            }
            if (chk_WriteJtb1.IsChecked == true)
            {
                EquipmentData.DeviceManager.ReaderJtBy1();
            }
            if (chk_WriteJtb2.IsChecked == true)
            {
                EquipmentData.DeviceManager.ReaderJtBy2();
            }
            if (chk_WriteChangTime.IsChecked == true)
            {
                EquipmentData.DeviceManager.ReaderChangFlTime();
                EquipmentData.DeviceManager.ReaderChangJtTime();
            }
        }
    }
}
