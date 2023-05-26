using System;
using System.Windows;
using Mesurement.UiLayer.ViewModel.Model;
using Mesurement.UiLayer.ViewModel.Schema;
using System.Windows.Controls;
using Mesurement.UiLayer.WPF.UiGeneral;

namespace Mesurement.UiLayer.WPF.Schema
{
    /// 设置检定参数值的表格控件
    /// <summary>
    /// 设置检定参数值的表格控件
    /// </summary>
    public class DataGridParaValue : DataGrid, IDisposable
    {
        public override void EndInit()
        {
            base.EndInit();
            try
            {
                paraInfo.PropertyChanged += ParaInfo_PropertyChanged;
            }
            catch
            { }
        }
        ParaInfoViewModel paraInfo
        {
            get
            {
                if (DataContext as SchemaViewModel == null)
                {
                    return null;
                }
                else
                {
                    return (DataContext as SchemaViewModel).ParaInfo;
                }
            }
        }
        void ParaInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (paraInfo == null)
            { return; }
            if (e.PropertyName == "LoadFlag" && paraInfo.LoadFlag)
            {
                AsyncObservableCollection<CheckParaViewModel> CheckParas = paraInfo.CheckParas;

                while (Columns.Count > 3)
                {
                    Columns.RemoveAt(3);
                }


                if (CheckParas.Count == 0)
                {
                    DataGridTextColumn column = Application.Current.Resources["dataGridColumnNoParameter"] as DataGridTextColumn;
                    Columns.Add(column);
                }
                else
                {
                    for (int i = 0; i < CheckParas.Count; i++)
                    {
                        CheckParaViewModel checkPara = CheckParas[i] as CheckParaViewModel;
                        if (checkPara != null)
                        {
                            DataGridColumn column = ControlFactory.CreateColumn(checkPara.ParaDisplayName, checkPara.ParaEnumType,checkPara.ParaDisplayName);
                            if (column != null)
                            {
                                Columns.Add(column);
                            }
                        }
                    }
                }
                paraInfo.LoadFlag = false;
            }
        }

        public void Dispose()
        {
            paraInfo.PropertyChanged -= ParaInfo_PropertyChanged;
        }
    }
}
