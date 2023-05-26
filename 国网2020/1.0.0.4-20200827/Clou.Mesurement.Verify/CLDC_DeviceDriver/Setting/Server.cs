using System.Drawing;

namespace CLDC_DeviceDriver.Setting
{
    /// <summary>
    /// 串口服务器基本信息类
    /// </summary>
    public class Server:ItemInfor
    {
        /// <summary>
        /// 服务器构造函数，用于初始化配置
        /// </summary>
        /// <param name="stringKey">关键字</param>
        /// <param name="stringValue">IP地址</param>
        /// <param name="stringName">名称</param>
        /// <param name="stringSettingBase">本机端口号</param>
        /// <param name="stringSettingExpand">远程端口号</param>
        /// <param name="stringDescription">描述</param>
        /// <param name="colorItem">颜色</param>
        /// <param name="boolExist">是否显示</param>
        public Server(string stringKey,string stringValue,string stringName,string stringSettingBase,string stringSettingExpand,string stringDescription,Color colorItem,bool boolExist)
        {
            Key = stringKey;
            KeyValue = stringValue;
            Name = stringName;
            SettingBase = stringSettingBase;
            SettingExpand = stringSettingExpand;
            Description = stringDescription;
            ColorItem = colorItem;
            Exist = boolExist;
        }

        /// <summary>
        /// 构造函数，用于从文件加载
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="stringKey">关键字</param>
        public Server(string filePath,string stringKey)
        {
            Key = stringKey;
            string stringTemp = string.Empty;
            stringTemp = ReadSingle(filePath, "Setting", Key);
            if (stringTemp != string.Empty)
                KeyValue = stringTemp;

            stringTemp = ReadSingle(filePath, "Setting", Key + "Name");
            if (stringTemp != string.Empty)
                Name = stringTemp;

            stringTemp = ReadSingle(filePath, "Setting", Key + "Com");
            if (stringTemp != string.Empty)
                SettingBase = stringTemp;

            stringTemp = ReadSingle(filePath, "Setting", Key + "Setting");
            if (stringTemp != string.Empty)
                SettingExpand = stringTemp;

            stringTemp = ReadSingle(filePath, "Setting", Key + "Description");
            if (stringTemp != string.Empty)
                Description = stringTemp;

            stringTemp = ReadSingle(filePath, "Setting", Key + "Color");
            if (stringTemp != string.Empty)
                ColorItem = GetColor(stringTemp);

            stringTemp = ReadSingle(filePath, "Setting", Key + "Exist");
            if (stringTemp != string.Empty)
                stringTemp = ReadSingle(filePath, "Setting", Key + "Exist");
            Exist = GetBool(stringTemp);
        }
    }
}
