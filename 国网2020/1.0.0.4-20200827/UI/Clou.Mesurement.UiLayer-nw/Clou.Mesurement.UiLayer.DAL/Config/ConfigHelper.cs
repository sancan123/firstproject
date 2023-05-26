using Mesurement.UiLayer.Utility.Log;
using System;
using System.Collections.Generic;

namespace Mesurement.UiLayer.DAL.Config
{
    /// <summary>
    /// 配置信息管理
    /// </summary>
    public class ConfigHelper
    {
        private static ConfigHelper instance = null;

        public static ConfigHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConfigHelper();
                }
                return instance;
            }
        }

        /// 配置信息列表
        /// <summary>
        /// 配置信息列表
        /// </summary>
        private Dictionary<EnumConfigId, List<string>> configDictionary = new Dictionary<EnumConfigId, List<string>>();

        /// 从数据库加载所有配置信息
        /// <summary>
        /// 从数据库加载所有配置信息
        /// </summary>
        public void LoadAllConfig()
        {
            configDictionary.Clear();
            List<DynamicModel> models = DALManager.ApplicationDbDal.GetList(EnumAppDbTable.CONFIG_PARA_VALUE.ToString());
            for (int i = 0; i < models.Count; i++)
            {
                string configNo = models[i].GetProperty("CONFIG_NO") as string;
                string stringValue = models[i].GetProperty("CONFIG_VALUE") as string;
                if (configNo != null)
                {
                    configNo = configNo.TrimStart('0');
                    EnumConfigId configId = EnumConfigId.未知编号配置;
                    Enum.TryParse(configNo, out configId);
                    if (configDictionary.ContainsKey(configId))
                    {
                        configDictionary[configId].Add(stringValue);
                    }
                    else
                    {
                        configDictionary.Add(configId, new List<string> { stringValue });
                    }
                }
            }
            List<DynamicModel> modelsFormat = DALManager.ApplicationDbDal.GetList(EnumAppDbTable.CONFIG_PARA_FORMAT.ToString());
            for (int i = 0; i < modelsFormat.Count; i++)
            {
                string configNo = modelsFormat[i].GetProperty("CONFIG_NO") as string;
                string defaultValue = modelsFormat[i].GetProperty("CONFIG_DEFAULT_VALUE") as string;
                if (configNo != null)
                {
                    configNo = configNo.TrimStart('0');
                    EnumConfigId configId = EnumConfigId.未知编号配置;
                    Enum.TryParse(configNo, out configId);
                    if (!dictionaryFormat.ContainsKey(configId))
                    {
                        dictionaryFormat.Add(configId, defaultValue);
                    }
                }
            }
        }

        public List<string> GetConfig(EnumConfigId configId)
        {
            if (configDictionary.ContainsKey(configId))
            {
                return configDictionary[configId];
            }
            else
            {
                return new List<string>();
            }
        }

        public string GetConfigString(EnumConfigId configId)
        {
            if (configDictionary.ContainsKey(configId))
            {
                List<string> valueList = configDictionary[configId];
                if (valueList.Count > 0)
                {
                    return valueList[0];
                }
            }
            LogManager.AddMessage(string.Format("配置信息:{0}获取失败!", configId), EnumLogSource.用户操作日志, EnumLevel.Error);
            return "";
        }

        #region 获取值
        /// <summary>
        /// 获取配置值,如果获取失败,取默认值
        /// </summary>
        /// <param name="configId">配置编号</param>
        /// <param name="indexTemp">值序号</param>
        /// <returns></returns>
        public string GetConfigString(EnumConfigId configId, int indexTemp)
        {
            string stringTemp = GetConfigString(configId);
            try
            {
                return stringTemp.Split('|')[indexTemp];
            }
            catch
            { 
                return GetDefaultValue(configId, indexTemp);
            }
        }
        /// <summary>
        /// 获取配置值,如果获取失败,取默认值
        /// </summary>
        /// <param name="configId">配置编号</param>
        /// <param name="indexTemp">值序号</param>
        /// <returns></returns>
        public bool GetConfigBool(EnumConfigId configId, int indexTemp,bool initialValue)
        {
            string stringTemp = GetConfigString(configId);
            try
            {
                string boolString = stringTemp.Split('|')[indexTemp];
                if (boolString == "是")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                string boolString = GetDefaultValue(configId, indexTemp);
                if (boolString == "是")
                {
                    return true;
                }
                else if (boolString == "否")
                {
                    return false;
                }
                else
                {
                    return initialValue;
                }
            }
        }
        /// <summary>
        /// 获取配置值,如果获取失败,取默认值
        /// </summary>
        /// <param name="configId">配置编号</param>
        /// <param name="indexTemp">值序号</param>
        /// <returns></returns>
        public int GetConfigInt(EnumConfigId configId, int indexTemp, int initialValue)
        {
            string stringTemp = GetConfigString(configId);
            try
            {
                string intString = stringTemp.Split('|')[indexTemp];
                return int.Parse(intString);
            }
            catch
            {
                string intString = GetDefaultValue(configId, indexTemp);
                int boolTemp = 0;
                if (!int.TryParse(intString, out boolTemp))
                {
                    return initialValue;
                }
                return boolTemp;
            }
        }
        /// <summary>
        /// 获取配置值,如果获取失败,取默认值
        /// </summary>
        /// <param name="configId">配置编号</param>
        /// <param name="indexTemp">值序号</param>
        /// <returns></returns>
        public float GetConfigFloat(EnumConfigId configId, int indexTemp, float initialValue)
        {
            string stringTemp = GetConfigString(configId);
            try
            {
                string floatString = stringTemp.Split('|')[indexTemp];
                return float.Parse(floatString);
            }
            catch
            {
                string floatString = GetDefaultValue(configId, indexTemp);
                float floatTemp = 0;
                if (!float.TryParse(floatString, out floatTemp))
                {
                    return initialValue;
                }
                return floatTemp;
            }
        }
        #endregion

        private void SaveConfigValue(EnumConfigId configId, string configValue)
        {
            string sql = string.Format("update config_para_value set config_value = '{0}' where config_no='{1}'", configValue, ((int)configId).ToString().PadLeft(5, '0'));
            DALManager.ApplicationDbDal.ExecuteOperation(new List<string> { sql });
        }
        /// <summary>
        /// 保存配置值
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="indexTemp"></param>
        /// <param name="objTemp"></param>
        private void SaveConfigValue(EnumConfigId configId, int indexTemp,object objTemp)
        {
            string temp = GetConfigString(configId);
            string[] arrayTemp = temp.Split('|');
            if (arrayTemp.Length>indexTemp)
            {
                string valueTemp=objTemp==null?"": objTemp.ToString();
                if (objTemp is bool)
                {
                    if ((bool)objTemp)
                    {
                        valueTemp = "是";
                    }
                    else
                    {
                        valueTemp = "否";
                    }
                }
                arrayTemp[indexTemp] = valueTemp;
                temp = string.Join("|", arrayTemp);
                SaveConfigValue(EnumConfigId.运行环境, temp);
            }
        }

        #region 获取默认值
        private Dictionary<EnumConfigId, string> dictionaryFormat = new Dictionary<EnumConfigId, string>();
        /// <summary>
        /// 获取默认值
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="indexTemp"></param>
        /// <returns></returns>
        private string GetDefaultValue(EnumConfigId configId, int indexTemp)
        {
            string strResult = "";
            if (dictionaryFormat.ContainsKey(configId))
            {
                string valueTemp = dictionaryFormat[configId];
                if (valueTemp != null)
                {
                    string[] arrayDefault = valueTemp.Split('|');
                    if (arrayDefault.Length > indexTemp)
                    {
                        return arrayDefault[indexTemp];
                    }
                }
            }
            return strResult;
        }
        #endregion

        #region 软件运行环境
        public string Language
        {
            get
            {
                return GetConfigString(EnumConfigId.运行环境, 0);
            }
            set
            {
                SaveConfigValue(EnumConfigId.运行环境, 0, value);
            }
        }
        /// <summary>
        /// 开启语音
        /// </summary>
        public bool OpenVoice
        {
            get
            {
                return GetConfigBool(EnumConfigId.运行环境, 1, true);
            }
            set
            {
                SaveConfigValue(EnumConfigId.运行环境, 1, value);
            }
        }
        #endregion

        #region 出厂编号配置
        /// <summary>
        /// 是否从条码号中截取出厂编号
        /// </summary>
        public bool IsAssertNoFromBarCode
        {
            get
            {
                return GetConfigBool(EnumConfigId.出厂编号配置, 0,true);
            }
            set
            {
                SaveConfigValue(EnumConfigId.出厂编号配置, 0, value);
            }
        }
        /// <summary>
        /// 出厂编号前缀
        /// </summary>
        public string AssertNoStartStr
        {
            get
            {
                return GetConfigString(EnumConfigId.出厂编号配置, 1);
            }
            set
            {
                SaveConfigValue(EnumConfigId.出厂编号配置, 1, value);
            }
        }
        /// <summary>
        /// 出厂编号后缀
        /// </summary>
        public string AssertNoEndStr
        {
            get
            {
                return GetConfigString(EnumConfigId.出厂编号配置, 2);
            }
            set
            {
                SaveConfigValue(EnumConfigId.出厂编号配置, 2, value);
            }
        }
        /// <summary>
        /// 出厂编号起始序号
        /// </summary>
        public int AssertNoStartIndex
        {
            get
            {
                return GetConfigInt(EnumConfigId.出厂编号配置, 3, 8);
            }
            set
            {
                SaveConfigValue(EnumConfigId.出厂编号配置, 3, value);
            }
        }
        /// <summary>
        /// 出厂编号长度
        /// </summary>
        public int AssertNoLength
        {
            get
            {
                return GetConfigInt(EnumConfigId.出厂编号配置, 4, 12);
            }
            set
            {
                SaveConfigValue(EnumConfigId.出厂编号配置, 4, value);
            }
        }
        #endregion

        #region 台体信息配置
        public string EquipmentNo
        {
            get
            {
                return GetConfigString(EnumConfigId.台体基本信息,0);
            }
            set
            {
                SaveConfigValue(EnumConfigId.台体基本信息, 0, value);
            }
        }
        public int MeterCount
        {
            get
            {
                return GetConfigInt(EnumConfigId.台体基本信息,2,24);
            }
            set
            {
                SaveConfigValue(EnumConfigId.台体基本信息, 2, value);
            }
        }
        public string EquipmentType
        {
            get
            {
                return GetConfigString(EnumConfigId.台体基本信息, 1);
            }
            set
            {
                SaveConfigValue(EnumConfigId.台体基本信息, 1, value);
            }
        }

        public string SouthManufacturers
        {
            get
            {
                return GetConfigString(EnumConfigId.台体基本信息, 3);
            }
            set
            {
                SaveConfigValue(EnumConfigId.台体基本信息, 3, value);
            }
        }

        #endregion

        #region 检定服务地址配置
        /// <summary>
        /// 界面服务地址
        /// </summary>
        public string ServiceAddressUI
        {
            get
            {
                return GetConfigString(EnumConfigId.服务地址配置,0);
            }
            set
            {
                SaveConfigValue(EnumConfigId.服务地址配置, 0,value);
            }
        }
        /// <summary>
        /// 检定服务地址
        /// </summary>
        public string ServiceAddressVerify
        {
            get
            {
                return GetConfigString(EnumConfigId.服务地址配置, 1);
            }
            set
            {
                SaveConfigValue(EnumConfigId.服务地址配置, 1, value);
            }
        }
        #endregion

        #region 营销接口配置
        /// <summary>
        /// 营销系统数据库路径
        /// </summary>
        public string MisConnString
        {
            get
            {
                try
                {
                    string temp = GetConfigString(EnumConfigId.平台数据库配置);
                    string[] arrayService = temp.Split('|');
                    string conString = string.Format("data source={0};persist security info=True;user id={1};password={2}", arrayService[0], arrayService[1], arrayService[2]);
                    return conString;
                }
                catch
                {
                    return "";
                }
            }
        }
        /// <summary>
        /// 营销系统Webservice地址
        /// </summary>
        public string MisServiceAddress
        {
            get
            {
                return GetConfigString(EnumConfigId.WebService配置,0); 
            }
            set
            {
                SaveConfigValue(EnumConfigId.WebService配置, 0,value); 
            }
        }
        /// <summary>
        /// 本地结论服务地址
        /// </summary>
        public string LocalMisServiceAddress
        {
            get
            {
                return GetConfigString(EnumConfigId.检定结论服务地址, 0);
            }
            set
            {
                SaveConfigValue(EnumConfigId.检定结论服务地址, 0, value);
            }
        }
        /// <summary>
        /// 是否开通本地结论服务
        /// </summary>
        public bool IsOpenLocalMisService
        {
            get
            {
                return GetConfigBool(EnumConfigId.检定结论服务地址, 1,true);
            }
            set
            {
                SaveConfigValue(EnumConfigId.检定结论服务地址, 1, value);
            }
        }
        #endregion

        #region 检定控制配置
        //TODO:检定控制配置
        public string SpsConsumptionDSN
        {
            get
            {
                return GetConfigString(EnumConfigId.设备特殊配置, 0);
            }
            set
            {
                SaveConfigValue(EnumConfigId.设备特殊配置, 0, value);
            }
        }
        public string SpsConsumptionNo 
        {
            get
            {
                return GetConfigString(EnumConfigId.设备特殊配置, 1);
            }
            set
            {
                SaveConfigValue(EnumConfigId.设备特殊配置, 1, value);
            }
        }
        public bool SpsIsLinkErrBoard
        {
            get
            {
                return GetConfigBool(EnumConfigId.设备特殊配置, 2, true);
            }
            set
            {
                SaveConfigValue(EnumConfigId.设备特殊配置, 2, value);
            }
        }
        public bool SpsIsRecvErrBoardCmd
        {
            get
            {
                return GetConfigBool(EnumConfigId.设备特殊配置, 3, true);
            }
            set
            {
                SaveConfigValue(EnumConfigId.设备特殊配置, 3, value);
            }
        }
        public bool SpsIsProtectWithstand
        {
            get
            {
                return GetConfigBool(EnumConfigId.设备特殊配置, 4, true);
            }
            set
            {
                SaveConfigValue(EnumConfigId.设备特殊配置, 4, value);
            }
        }
        public bool SpsIsHaveInfraredProtect
        {
            get
            {
                return GetConfigBool(EnumConfigId.设备特殊配置, 5, true);
            }
            set
            {
                SaveConfigValue(EnumConfigId.设备特殊配置, 5, value);
            }
        }
        public string SpsInfraredProtectIDs
        {
            get
            {
                return GetConfigString(EnumConfigId.设备特殊配置, 6);
            }
            set
            {
                SaveConfigValue(EnumConfigId.设备特殊配置, 6, value);
            }
        }
        public bool SpsIsHavePoleProtect
        {
            get
            {
                return GetConfigBool(EnumConfigId.设备特殊配置, 7, false);
            }
            set
            {
                SaveConfigValue(EnumConfigId.设备特殊配置, 7, value);
            }
        }
        public int SpsPoleAngularVelocity
        {
            get
            {
                return GetConfigInt(EnumConfigId.设备特殊配置, 8, 5);
            }
            set
            {
                SaveConfigValue(EnumConfigId.设备特殊配置, 8, value);
            }
        }
        public int SpsPoleTravel
        {
            get
            {
                return GetConfigInt(EnumConfigId.设备特殊配置, 9, 90);
            }
            set
            {
                SaveConfigValue(EnumConfigId.设备特殊配置, 9, value);
            }
        }
        public bool SpsIsDefaultForeward
        {
            get
            {
                return GetConfigBool(EnumConfigId.设备特殊配置, 10, true);
            }
            set
            {
                SaveConfigValue(EnumConfigId.设备特殊配置, 10, value);
            }
        }
        #endregion
    }
}
