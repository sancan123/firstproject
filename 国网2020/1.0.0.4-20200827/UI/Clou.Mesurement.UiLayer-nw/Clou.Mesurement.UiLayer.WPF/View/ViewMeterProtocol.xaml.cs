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
    public partial class ViewMeterProtocol
    {
        public ViewMeterProtocol()
        {
            InitializeComponent();
            Name = "协议配置";
            LoadXmlFromFile();
        }
        /// <summary>
        /// 要显示的xml文件
        /// </summary>
        private XmlDataProvider providerProtocol
        { get { return Resources["ProviderMeterProtocol"] as XmlDataProvider; } }
        /// <summary>
        /// 表协议配置文件路径
        /// </summary>
        private string filePath = string.Format(@"{0}\xml\DgnProtocol.xml", Directory.GetCurrentDirectory());
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
            LogManager.AddMessage("表协议配置信息保存成功!!", EnumLogSource.用户操作日志, EnumLevel.Tip);
            //向检定服务下发表通信协议
            WcfHelper.Instance.LoadMeterProtocols();
        }
        /// <summary>
        /// 移除节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Remove(object sender, System.Windows.RoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;
            if (button != null && button.Name == "buttonRemove")
            {
                XmlNode nodeCurrent = button.DataContext as XmlNode;
                if (nodeCurrent != null && nodeCurrent.ParentNode != null)
                {
                    nodeCurrent.ParentNode.RemoveChild(nodeCurrent);
                }
            }
        }
        /// <summary>
        /// 添加三类数据标识
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_AddLevel3(object sender, System.Windows.RoutedEventArgs e)
        {
            //XmlNode nodeTemp = dataGridLevel3.DataContext as XmlNode;
            //if (nodeTemp == null)
            //{
            //    return;
            //}
            //XmlNode nodeParent = nodeTemp.SelectSingleNode("DataItemsLevel3");
            //if (nodeParent != null)
            //{
            //    AddDataItem(nodeParent);
            //}
        }
        private void AddDataItem(XmlNode nodeParent)
        {
            if (nodeParent.ChildNodes.Count > 0)
            {
                nodeParent.AppendChild(nodeParent.LastChild.Clone());
            }
            else
            {
                XmlDocument docTemp = providerProtocol.Document;
                XmlElement dataItem = docTemp.CreateElement("DataItem");
                XmlElement idNode = docTemp.CreateElement("ID");
                idNode.InnerText = "01";
                dataItem.AppendChild(idNode);
                XmlElement addrNode = docTemp.CreateElement("Address");
                addrNode.InnerText = "000000";
                dataItem.AppendChild(addrNode);
                XmlElement descNode = docTemp.CreateElement("Description");
                descNode.InnerText = "请输入标识的说明";
                dataItem.AppendChild(descNode);

                nodeParent.AppendChild(dataItem);
            }
        }
        /// <summary>
        /// 添加一类数据标识
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_AddLevel1(object sender, System.Windows.RoutedEventArgs e)
        {
            //XmlNode nodeTemp = dataGridLevel1.DataContext as XmlNode;
            //if (nodeTemp == null)
            //{
            //    return;
            //}
            //XmlNode nodeParent = nodeTemp.SelectSingleNode("DataItemsLevel1");
            //if (nodeParent != null)
            //{
            //    AddDataItem(nodeParent);
            //}
        }
        private ContextMenu menuTemp
        { get { return Resources["MenuProtocol"] as ContextMenu; } }
        /// <summary>
        /// 删除当前协义
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_DeleteProtocol(object sender, System.Windows.RoutedEventArgs e)
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
        private void Click_AddProtocol(object sender, System.Windows.RoutedEventArgs e)
        {
            XmlDocument docTemp = providerProtocol.Document;
            XmlNodeList nodeListTemp = docTemp.SelectNodes("DgnProtocol/Protocols");
            if (nodeListTemp != null)
            {
                XmlNode nodeParent = nodeListTemp[0];
                XmlNodeList nodeList = docTemp.SelectNodes("DgnProtocol/DefaultProtocols/R");
                foreach (XmlNode nodeTemp in nodeList)
                {
                    try
                    {
                        if (nodeTemp.Attributes["Name"].Value == "DLT645-2007-Default")
                        {
                            XmlNode nodeNew = nodeTemp.Clone();
                            nodeNew.Attributes["Name"].Value = "DLT645-2007_Copy";
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
