using Mesurement.UiLayer.DAL;
using Mesurement.UiLayer.Utility.Log;
using Mesurement.UiLayer.ViewModel;
using System;
using System.IO;
using System.Windows.Data;
using System.Xml;
namespace Mesurement.UiLayer.WPF.View
{
    /// <summary>
    /// Interaction logic for ViewProtocolConfig.xaml
    /// </summary>
    public partial class ViewProtocolConfig
    {
        public ViewProtocolConfig()
        {
            InitializeComponent();
            Name = "数据标识"; 
            LoadXmlFromFile();
        }
        /// <summary>
        /// 要显示的xml文件
        /// </summary>
        private XmlDataProvider dataFlagDict
        { get { return Resources["DataFlagDict"] as XmlDataProvider; } }
        /// <summary>
        /// 添加的数据标识的内容
        /// </summary>
        private DataFlagItem flagItemTemp
        {
            get { return Resources["DataItemTemp"] as DataFlagItem; }
        }
        /// <summary>
        /// 表协议配置文件路径
        /// </summary>
        private string filePath = string.Format(@"{0}\xml\DataFlagDict.xml", Directory.GetCurrentDirectory());
        /// <summary>
        /// 从文件加载
        /// </summary>
        private void LoadXmlFromFile()
        {
            dataFlagDict.Source = new Uri(filePath, UriKind.Absolute);
        }
        /// <summary>
        /// 添加标识事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_Add(object sender, System.Windows.RoutedEventArgs e)
        {
            XmlDocument docTemp = dataFlagDict.Document;
            XmlNode nodeParent = docTemp.DocumentElement;
            for (int i = 0; i < nodeParent.ChildNodes.Count; i++)
            {
                try
                {
                    if (nodeParent.ChildNodes[i].Attributes["DataFlagName"].Value == flagItemTemp.Name)
                    {
                        LogManager.AddMessage("该标识名称已经存在,请更改标识名称!!", EnumLogSource.用户操作日志, EnumLevel.Tip);
                        return;
                    }
                }
                catch
                { }
            }
            XmlElement dataItem = docTemp.CreateElement("R");
            XmlAttribute attributeName = docTemp.CreateAttribute("DataFlagName");
            dataItem.Attributes.Append(attributeName);
            dataItem.Attributes["DataFlagName"].Value = flagItemTemp.Name;
            XmlAttribute attributeDataFlag = docTemp.CreateAttribute("DataFlag");
            dataItem.Attributes.Append(attributeDataFlag);
            dataItem.Attributes["DataFlag"].Value = flagItemTemp.DataFlag;
            XmlAttribute attributeLength = docTemp.CreateAttribute("DataLength");
            dataItem.Attributes.Append(attributeLength);
            dataItem.Attributes["DataLength"].Value = flagItemTemp.Length;
            XmlAttribute attributeDotNumber = docTemp.CreateAttribute("DataSmallNumber");
            dataItem.Attributes.Append(attributeDotNumber);
            dataItem.Attributes["DataSmallNumber"].Value = flagItemTemp.DotLength;
            XmlAttribute attributeFormat = docTemp.CreateAttribute("DataFormat");
            dataItem.Attributes.Append(attributeFormat);
            dataItem.Attributes["DataFormat"].Value = flagItemTemp.DataFormat;
            XmlAttribute attributeDefault = docTemp.CreateAttribute("Default");
            dataItem.Attributes.Append(attributeDefault);
            dataItem.Attributes["Default"].Value = flagItemTemp.DataFormat;

            nodeParent.AppendChild(dataItem);
            LogManager.AddMessage("标识名称已添加,点击保存按钮后生效", EnumLogSource.用户操作日志, EnumLevel.Tip);
        }
        /// <summary>
        /// 保存标识到xml文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Click_Save(object sender, System.Windows.RoutedEventArgs e)
        {
            dataFlagDict.Document.Save(filePath);
            CodeDictionary.LoadDataFlagNames();
        }
    }
    /// <summary>
    /// 数据标识项
    /// </summary>
    public class DataFlagItem : ViewModelBase
    {
        private string name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { SetPropertyValue(value, ref name, "Name"); }
        }
        private string dataFlag;
        /// <summary>
        /// 数据标识
        /// </summary>
        public string DataFlag
        {
            get { return dataFlag; }
            set { SetPropertyValue(value, ref dataFlag, "DataFlag"); }
        }
        private string length;
        /// <summary>
        /// 数据长度
        /// </summary>
        public string Length
        {
            get { return length; }
            set { SetPropertyValue(value, ref length, "Length"); }
        }
        private string dotLength;
        /// <summary>
        /// 小数位数
        /// </summary>
        public string DotLength
        {
            get { return dotLength; }
            set 
            { 
                SetPropertyValue(value, ref dotLength, "DotLength"); 
            }
        }
        private string dataFormat;
        /// <summary>
        /// 数据格式
        /// </summary>
        public string DataFormat
        {
            get { return dataFormat; }
            set { SetPropertyValue(value, ref dataFormat, "DataFormat"); }
        }
        
    }
}
