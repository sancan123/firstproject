using Mesurement.UiLayer.Utility.Log;
using Mesurement.UiLayer.ViewModel.WcfService;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;

namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// Interaction logic for ViewProtocolConfig.xaml
    /// </summary>
    public partial class ViewMeterZaiBO
    {
        public ViewMeterZaiBO()
        {
            InitializeComponent();
            Name = "协议配置";
            LoadXmlFromFile();
        }
        /// <summary>
        /// 要显示的xml文件
        /// </summary>
        private XmlDataProvider providerProtocol
        { get { return Resources["ProviderCarrierProtocol"] as XmlDataProvider; } }
        /// <summary>
        /// 表协议配置文件路径
        /// </summary>
        private string filePath = string.Format(@"{0}\Verify\xml\CarrierProtocol.xml", Directory.GetCurrentDirectory());
        /// <summary>
        /// 从文件加载
        /// </summary>
        private void LoadXmlFromFile()
        {
            providerProtocol.Source = new Uri(filePath, UriKind.Absolute);
        }
        /// <summary>
        /// 保存xml文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Save(object sender, System.Windows.RoutedEventArgs e)
        {
            
            providerProtocol.Document.Save(filePath);
            LogManager.AddMessage("载波配置信息保存成功!!", EnumLogSource.用户操作日志, EnumLevel.Tip);
            //向检定服务下发表通信协议
          //  WcfHelper.Instance.LoadMeterProtocols();
        }
    
      
       
        private ContextMenu menuTemp
        { get { return Resources["MenuCarrierProtocol"] as ContextMenu; } }
        /// <summary>
        /// 删除当前协义
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_DeleteCarrierProtocol(object sender, System.Windows.RoutedEventArgs e)
        {
            FrameworkElement elementTemp = ContextMenuService.GetPlacementTarget(menuTemp) as FrameworkElement;
            if (elementTemp != null && elementTemp.DataContext is XmlNode)
            {
                XmlNode nodeProtocol = elementTemp.DataContext as XmlNode;
                if (nodeProtocol.ParentNode != null && nodeProtocol.Name == "R")
                {
                    nodeProtocol.ParentNode.RemoveChild(nodeProtocol);
                }
            }
        }
        /// <summary>
        /// 添加新协议
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_AddCarrierProtocol(object sender, System.Windows.RoutedEventArgs e)
        {
            XmlDocument docTemp = providerProtocol.Document;
            XmlNodeList nodeListTemp = docTemp.SelectNodes("CarrierProtocol/Protocols");
            if (nodeListTemp != null)
            {
                XmlNode nodeParent = nodeListTemp[0];
                XmlNodeList nodeList = docTemp.SelectNodes("CarrierProtocol/DefaultProtocols/R");
                foreach (XmlNode nodeTemp in nodeList)
                {
                    try
                    {
                        if (nodeTemp.Attributes["Name"].Value == "中电华瑞2016-Default")
                        {
                            XmlNode nodeNew = nodeTemp.Clone();
                            nodeNew.Attributes["Name"].Value = "中电华瑞2016_Copy";
                            nodeParent.AppendChild(nodeNew);
                            break;
                        }
                    }
                    catch
                    { }
                }
            }
        }
    }
}
