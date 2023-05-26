using Mesurement.UiLayer.DAL;
using Mesurement.UiLayer.Utility.Log;
using Mesurement.UiLayer.ViewModel.CodeTree;
using Mesurement.UiLayer.ViewModel.Model;
using System.Collections.Generic;
using System.Linq;

namespace Mesurement.UiLayer.ViewModel.PortConfig
{
    /// <summary>
    /// 服务器端口配置
    /// </summary>
    public class PortConfigViewModel : ViewModelBase
    {
        #region 各种设备端口的数据模型
        private AsyncObservableCollection<string> comParams = new AsyncObservableCollection<string>();
        /// <summary>
        /// 串口参数
        /// </summary>
        public AsyncObservableCollection<string> ComParams
        {
            get { return comParams; }
            set { SetPropertyValue(value, ref comParams, "ComParams"); }
        }

        #region 串口服务器列表
        private AsyncObservableCollection<ServerItem> servers = new AsyncObservableCollection<ServerItem>();
        /// <summary>
        /// 串口服务器列表
        /// </summary>
        public AsyncObservableCollection<ServerItem> Servers
        {
            get { return servers; }
            set { SetPropertyValue(value, ref servers, "Servers"); }
        }
        #endregion
        #region 设备列表

        private AsyncObservableCollection<DeviceGroup> groups = new AsyncObservableCollection<DeviceGroup>();
        /// <summary>
        /// 所有设备列表
        /// </summary>
        public AsyncObservableCollection<DeviceGroup> Groups
        {
            get { return groups; }
            set { SetPropertyValue(value, ref groups, "Groups"); }
        }

        #endregion
        #region RS485配置参数
        private AsyncObservableCollection<RS485Item> rs485Group = new AsyncObservableCollection<RS485Item>();
        /// <summary>
        /// 485端口列表
        /// </summary>
        public AsyncObservableCollection<RS485Item> Rs485Group
        {
            get { return rs485Group; }
            set { SetPropertyValue(value, ref rs485Group, "Rs485Group"); }
        }

        #endregion
        #endregion
        public PortConfigViewModel()
        {
            LoadFromConfig();
        }
        /// <summary>
        /// 从配置加载端口信息
        /// </summary>
        public void LoadFromConfig()
        {
            Servers.Clear();
            Groups.Clear();
            ComParams.Clear();
            Rs485Group.Clear();
            RelayGroups.Clear();
            #region 初始化串口参数
            Dictionary<string, string> dictionaryComPars = CodeDictionary.GetLayer2("SerialPortPara");
            for (int i = 0; i < dictionaryComPars.Count; i++)
            {
                ComParams.Add(dictionaryComPars.Keys.ElementAt(i));
            }
            #endregion
            #region 初始化服务器
            Dictionary<string, string> dictionaryServers = CodeDictionary.GetLayer2("PortServerType");
            for (int i = 0; i < dictionaryServers.Count; i++)
            {
                if (Servers.Count < 4)
                {
                    ServerItem server = new ServerItem()
                    {
                        Name = string.Format("串口服务器{0}", i + 1),
                        Address = dictionaryServers.Keys.ElementAt(i)
                    };
                    if (server.Address == "COM")
                    {
                        server.Address = "193.168.18.1_10003_20000";
                    }
                    server.FlagChanged = false;
                    Servers.Add(server);
                }
            }
            while (Servers.Count < 4)
            {
                Servers.Add(new ServerItem()
                {
                    Name = string.Format("串口服务器{0}", Servers.Count+1),
                    Address = "193.168.18.1_10003_20000",
                });
            }
            Servers.Add(new ServerItem()
            {
                Name = "COM",
                Address = "COM",
                FlagChanged=false
            });
            #endregion
            #region 加载设备端口
            Dictionary<string, string> dictionaryConfig = CodeDictionary.GetLayer2("SystemConfig");
            string devideId = dictionaryConfig["设备配置"];
            List<DynamicModel> models = DALManager.ApplicationDbDal.GetList(EnumAppDbTable.CONFIG_PARA_VALUE.ToString(), string.Format("CONFIG_NO like '{0}%'", devideId));
            Dictionary<string, string> dictonaryTemp = CodeDictionary.GetLayer2("Config_Device");
            foreach (KeyValuePair<string, string> pair in dictonaryTemp)
            {
                if (pair.Key=="RS485")
                {
                    #region rs485
                    string paraNoTemp = string.Format("{0}{1}", devideId, pair.Value.PadLeft(3, '0'));
                    var items = models.Where(item => item.GetProperty("CONFIG_NO") as string == paraNoTemp);
                    if (items != null)
                    {
                        foreach (DynamicModel model in items)
                        {
                            string configValue = model.GetProperty("CONFIG_VALUE") as string;
                            string[] arrayTemp = configValue.Split('|');
                            if (arrayTemp.Length < 8)
                            {
                                continue;
                            }
                            string comAddress = arrayTemp[5];
                            ServerItem serverTemp = Servers.FirstOrDefault(item => item.Address == comAddress);
                            if (serverTemp == null)
                            {
                                serverTemp = Servers[0];
                            }
                            RS485Item rs485Item = new RS485Item()
                            {
                                ID = model.GetProperty("ID").ToString(),
                                Name = arrayTemp[0],
                                ParaNo = paraNoTemp,
                                Server = serverTemp,
                                PortCount = arrayTemp[1],
                                StartPort = arrayTemp[2],
                                IntervalValue = arrayTemp[3],
                                MaxTimePerFrame = arrayTemp[6],
                                MaxTimePerByte = arrayTemp[7],
                                DllType = arrayTemp[8],
                                FlagChanged = false
                            };
                            Rs485Group.Add(rs485Item);
                        }
                    }
                    #endregion
                }
                else if (pair.Key == "继电器配置")
                {
                    #region 继电器
                    string paraNoTemp = string.Format("{0}{1}", devideId, pair.Value.PadLeft(3, '0'));
                    Dictionary<string, string> dictionaryRelay = CodeDictionary.GetLayer2("RelayConfig");
                    foreach (KeyValuePair<string, string> pairRelay in dictionaryRelay)
                    {
                        RelayGroup relayGroup = new RelayGroup()
                        {
                            ParaNo = paraNoTemp,
                            Name = pairRelay.Key,
                            GroupId=pairRelay.Value
                        };
                        RelayGroups.Add(relayGroup);
                    }
                    var items = models.Where(item => item.GetProperty("CONFIG_NO") as string == paraNoTemp);
                    foreach (RelayGroup relayGroup in RelayGroups)
                    {
                        DynamicModel modelRelay = items.FirstOrDefault(item => item.GetProperty("CONFIG_VALUE") as string != null && (item.GetProperty("CONFIG_VALUE") as string).StartsWith(relayGroup.GroupId));
                        if (modelRelay != null)
                        {
                            relayGroup.ID = modelRelay.GetProperty("ID").ToString();
                            relayGroup.LoadRelayConfig(modelRelay.GetProperty("CONFIG_VALUE") as string);
                            relayGroup.FlagChanged = false;
                        }
                    }
                    #endregion
                }
                else if (pair.Key != "特殊配置")
                {
                    #region 台体设备
                    DeviceGroup groupTemp = new DeviceGroup();
                    groupTemp.ParaNo = string.Format("{0}{1}", devideId, pair.Value.PadLeft(3, '0'));
                    groupTemp.Name = pair.Key;
                    var items = models.Where(item => item.GetProperty("CONFIG_NO") as string == groupTemp.ParaNo);
                    if (items != null)
                    {
                        foreach (DynamicModel model in items)
                        {
                            DeviceItem deviceTemp = new DeviceItem();
                            deviceTemp.ID = model.GetProperty("ID").ToString();
                            string configValue = model.GetProperty("CONFIG_VALUE") as string;
                            string[] arrayTemp = configValue.Split('|');
                            if (arrayTemp.Length < 8)
                            {
                                continue;
                            }
                            string comAddress = arrayTemp[1];
                            ServerItem serverTemp = Servers.FirstOrDefault(item => item.Address == comAddress);
                            if (serverTemp == null)
                            {
                                deviceTemp.Server = Servers[0];
                            }
                            else
                            {
                                deviceTemp.Server = serverTemp;
                            }
                            deviceTemp.PortNum = arrayTemp[2];
                            deviceTemp.ComParam = arrayTemp[5];
                            deviceTemp.MaxTimePerByte = arrayTemp[7];
                            deviceTemp.MaxTimePerFrame = arrayTemp[6];
                            deviceTemp.DeviceName = arrayTemp[3];
                            deviceTemp.ClassName = arrayTemp[4];
                            deviceTemp.DllType = arrayTemp[8];
                            deviceTemp.FlagChanged = false;
                            groupTemp.DeviceItems.Add(deviceTemp);
                        }
                    }
                    groupTemp.IsExist = groupTemp.DeviceItems.Count > 0;
                    Groups.Add(groupTemp);
                }
                #endregion
            }
            #endregion
            Groups.Sort(item => item.ParaNo);
            Groups.Sort(item => !item.IsExist);
        }
        /// <summary>
        /// 保存端口配置信息
        /// </summary>
        public void SavePortConfig()
        {
            SaveDeviceConfig();
            SaveServerConfig();
            SaveRs485();
            SaveRelays();
        }
        /// <summary>
        /// 保存设备端口信息
        /// </summary>
        private void SaveDeviceConfig()
        {
            for (int i = 0; i < Groups.Count; i++)
            {
                for (int j = 0; j < Groups[i].DeviceItems.Count; j++)
                {
                    DeviceItem itemTemp = Groups[i].DeviceItems[j];
                    if (Groups[i].IsExist)
                    {
                        #region 如果组别存在
                        if (!itemTemp.FlagChanged)
                        {
                            continue;
                        }
                        DynamicModel modelTemp = GetModel(Groups[i], itemTemp);
                        if (itemTemp.ID == "0")
                        {
                            #region 插入新信息
                            int insertTemp = DALManager.ApplicationDbDal.Insert("CONFIG_PARA_VALUE", modelTemp);
                            if (insertTemp == 1)
                            {
                                DynamicModel modelInsert = DALManager.ApplicationDbDal.GetByID("CONFIG_PARA_VALUE", string.Format("config_no='{0}' order by id desc", Groups[i].ParaNo));
                                if (modelInsert != null)
                                {
                                    itemTemp.ID = modelInsert.GetProperty("ID").ToString();
                                }
                                itemTemp.FlagChanged = false;
                                LogManager.AddMessage(string.Format("添加设备:{0} 的端口信息:{1} 成功", Groups[i].Name, modelInsert.GetProperty("CONFIG_VALUE")), EnumLogSource.数据库存取日志);
                            }
                            else
                            {
                                LogManager.AddMessage(string.Format("添加设备:{0} 的端口信息失败", Groups[i].Name, modelTemp.GetProperty("CONFIG_VALUE")), EnumLogSource.数据库存取日志, EnumLevel.Warning);
                            }
                            #endregion
                        }
                        else
                        {
                            #region 更新当前端口信息
                            int updateTemp = DALManager.ApplicationDbDal.Update("CONFIG_PARA_VALUE", string.Format("ID={0}", itemTemp.ID), modelTemp, new List<string>() { "CONFIG_VALUE" });
                            if (updateTemp == 1)
                            {
                                itemTemp.FlagChanged = false;
                                LogManager.AddMessage(string.Format("更新设备:{0} 的端口信息:{1} 成功", Groups[i].Name, modelTemp.GetProperty("CONFIG_VALUE")), EnumLogSource.数据库存取日志);
                            }
                            else
                            {
                                LogManager.AddMessage(string.Format("更新设备:{0} 的端口信息失败", Groups[i].Name, modelTemp.GetProperty("CONFIG_VALUE")), EnumLogSource.数据库存取日志, EnumLevel.Warning);
                            }
                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        #region 删除组别下的端口信息
                        if (itemTemp.ID != "0")
                        {
                            int deleteCount = DALManager.ApplicationDbDal.Delete("CONFIG_PARA_VALUE", string.Format("ID={0}", itemTemp.ID));
                            if (deleteCount == 1)
                            {
                                LogManager.AddMessage(string.Format("删除设备:{0} 的端口信息成功", Groups[i].Name), EnumLogSource.数据库存取日志);
                            }
                            else
                            {
                                LogManager.AddMessage(string.Format("删除设备:{0} 的端口信息失败", Groups[i].Name), EnumLogSource.数据库存取日志, EnumLevel.Warning);
                            }
                        }
                        #endregion
                    }
                }
                if (!Groups[i].IsExist)
                {
                    Groups[i].DeviceItems.Clear();
                }
            }
        }

        /// <summary>
        /// 删除端口信息
        /// </summary>
        /// <param name="deviceItem"></param>
        public void DeleteItem(DeviceItem deviceItem)
        {
            for (int i = 0; i < Groups.Count; i++)
            {
                if (Groups[i].DeviceItems.Contains(deviceItem))
                {
                    #region 删除组别下的端口信息
                    if (deviceItem.ID != "0")
                    {
                        int deleteCount = DALManager.ApplicationDbDal.Delete("CONFIG_PARA_VALUE", string.Format("ID={0}", deviceItem.ID));
                        if (deleteCount == 1)
                        {
                            LogManager.AddMessage(string.Format("删除设备:{0} 的端口信息成功", Groups[i].Name), EnumLogSource.数据库存取日志);
                        }
                        else
                        {
                            LogManager.AddMessage(string.Format("删除设备:{0} 的端口信息失败", Groups[i].Name), EnumLogSource.数据库存取日志, EnumLevel.Warning);
                        }
                    }
                    #endregion
                    Groups[i].DeviceItems.Remove(deviceItem);
                }
            }
        }
        /// <summary>
        /// 删除485端口
        /// </summary>
        /// <param name="rs485Temp"></param>
        public void DeleteRs485(RS485Item rs485Temp)
        {
            if (rs485Temp.ID != "0")
            {
                int deleteCount = DALManager.ApplicationDbDal.Delete("CONFIG_PARA_VALUE", string.Format("ID={0}", rs485Temp.ID));
                if (deleteCount == 1)
                {
                    LogManager.AddMessage("删除485端口信息成功", EnumLogSource.数据库存取日志);
                }
                else
                {
                    LogManager.AddMessage("删除485端口信息失败", EnumLogSource.数据库存取日志, EnumLevel.Warning);
                }
            }
            Rs485Group.Remove(rs485Temp);
        }
        /// <summary>
        /// 获取设备端口的模型
        /// </summary>
        /// <param name="groupTemp"></param>
        /// <param name="itemTemp"></param>
        /// <returns></returns>
        private DynamicModel GetModel(DeviceGroup groupTemp, DeviceItem itemTemp)
        {
            DynamicModel model = new DynamicModel();
            model.SetProperty("ID", itemTemp.ID);
            model.SetProperty("CONFIG_NO", groupTemp.ParaNo);
            string valueTemp = string.Format("{0}|{1}|{2}|{7}|{8}|{3}|{4}|{5}|{6}", groupTemp.Name, itemTemp.Server.Address, itemTemp.PortNum, itemTemp.ComParam, itemTemp.MaxTimePerFrame, itemTemp.MaxTimePerByte,itemTemp.DllType,itemTemp.DeviceName,itemTemp.ClassName);
            model.SetProperty("CONFIG_VALUE", valueTemp);
            return model;
        }
        /// <summary>
        /// 保存服务器的配置
        /// </summary>
        private void SaveServerConfig()
        {
            CodeTreeNode nodeServer = CodeTreeViewModel.Instance.GetCodeByEnName("PortServerType",2);
            if (nodeServer != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (nodeServer.Children.Count <= i)
                    {
                        nodeServer.AddCode();
                    }
                    Servers[i].FlagChanged = false;
                    if (nodeServer.Children[i].CODE_NAME != Servers[i].Address)
                    {
                        nodeServer.Children[i].CODE_NAME = Servers[i].Address;
                        nodeServer.Children[i].CODE_VALUE = (i + 1).ToString().PadLeft(2, '0');
                        nodeServer.Children[i].VALID_FLAG = true;
                    }
                }
                if(nodeServer.Children.Count<5)
                {
                    if (nodeServer.Children.Count <= 5)
                    {
                        nodeServer.AddCode();
                    }
                    if (nodeServer.Children[4].CODE_NAME != "COM")
                    {
                        nodeServer.Children[4].CODE_NAME = "COM";
                        nodeServer.Children[4].CODE_VALUE = "05";
                    }
                }
                nodeServer.SaveCode();
            }
        }
        /// <summary>
        /// 保存485信息
        /// </summary>
        private void SaveRs485()
        {
            for (int i = 0; i < Rs485Group.Count; i++)
            {
                if (Rs485Group[i].FlagChanged)
                {
                    DynamicModel modelTemp = new DynamicModel();
                    modelTemp.SetProperty("CONFIG_NO", Rs485Group[i].ParaNo);
                    modelTemp.SetProperty("CONFIG_VALUE", Rs485Group[i].ToString());
                    if (Rs485Group[i].ID == "0")
                    {
                        #region 插入新信息
                        int insertTemp = DALManager.ApplicationDbDal.Insert("CONFIG_PARA_VALUE", modelTemp);
                        if (insertTemp == 1)
                        {
                            DynamicModel modelInsert = DALManager.ApplicationDbDal.GetByID("CONFIG_PARA_VALUE", string.Format("config_no='{0}' order by id desc", Rs485Group[i].ParaNo));
                            if (modelInsert != null)
                            {
                                Rs485Group[i].ID = modelInsert.GetProperty("ID").ToString();
                            }
                            Rs485Group[i].FlagChanged = false;
                            LogManager.AddMessage(string.Format("添加485端口信息:{0} 成功", modelInsert.GetProperty("CONFIG_VALUE")), EnumLogSource.数据库存取日志);
                        }
                        else
                        {
                            LogManager.AddMessage("添加485端口信息失败", EnumLogSource.数据库存取日志, EnumLevel.Warning);
                        }
                        #endregion
                    }
                    else
                    {
                        #region 更新当前端口信息
                        int updateTemp = DALManager.ApplicationDbDal.Update("CONFIG_PARA_VALUE", string.Format("ID={0}", Rs485Group[i].ID), modelTemp, new List<string>() { "CONFIG_VALUE" });
                        if (updateTemp == 1)
                        {
                            Rs485Group[i].FlagChanged = false;
                            LogManager.AddMessage(string.Format("更新485端口信息:{0} 成功", modelTemp.GetProperty("CONFIG_VALUE")), EnumLogSource.数据库存取日志);
                        }
                        else
                        {
                            LogManager.AddMessage("更新485端口信息失败", EnumLogSource.数据库存取日志, EnumLevel.Warning);
                        }
                        #endregion
                    }
                }
            }
        }

        #region 继电器相关
        private AsyncObservableCollection<RelayGroup> relayGroups = new AsyncObservableCollection<RelayGroup>();
        /// <summary>
        /// 继电器组集合
        /// </summary>
        public AsyncObservableCollection<RelayGroup> RelayGroups
        {
            get { return relayGroups; }
            set { SetPropertyValue(value, ref relayGroups, "RelayGroups"); }
        }
        /// <summary>
        /// 保存继电器配置
        /// </summary>
        private void SaveRelays()
        {
            for (int i = 0; i < RelayGroups.Count; i++)
            {
                if (RelayGroups[i].FlagChanged)
                {
                    DynamicModel modelTemp = new DynamicModel();
                    modelTemp.SetProperty("CONFIG_NO", RelayGroups[i].ParaNo);
                    modelTemp.SetProperty("CONFIG_VALUE", RelayGroups[i].ToString());
                    if (RelayGroups[i].ID == "0")
                    {
                        #region 插入新信息
                        int insertTemp = DALManager.ApplicationDbDal.Insert("CONFIG_PARA_VALUE", modelTemp);
                        if (insertTemp == 1)
                        {
                            DynamicModel modelInsert = DALManager.ApplicationDbDal.GetByID("CONFIG_PARA_VALUE", string.Format("config_no='{0}' order by id desc", RelayGroups[i].ParaNo));
                            if (modelInsert != null)
                            {
                                RelayGroups[i].ID = modelInsert.GetProperty("ID").ToString();
                            }
                            RelayGroups[i].FlagChanged = false;
                            LogManager.AddMessage(string.Format("添加继电器:{0}配置信息:{1} 成功", RelayGroups[i].Name, modelInsert.GetProperty("CONFIG_VALUE")), EnumLogSource.数据库存取日志);
                        }
                        else
                        {
                            LogManager.AddMessage(string.Format("添加继电器:{0}配置信息:{1} 失败", RelayGroups[i].Name), EnumLogSource.数据库存取日志, EnumLevel.Warning);
                        }
                        #endregion
                    }
                    else
                    {
                        #region 更新当前端口信息
                        int updateTemp = DALManager.ApplicationDbDal.Update("CONFIG_PARA_VALUE", string.Format("ID={0}", RelayGroups[i].ID), modelTemp, new List<string>() { "CONFIG_VALUE" });
                        if (updateTemp == 1)
                        {
                            RelayGroups[i].FlagChanged = false;
                            LogManager.AddMessage(string.Format("更新继电器:{0} 信息:{1} 成功", RelayGroups[i], modelTemp.GetProperty("CONFIG_VALUE")), EnumLogSource.数据库存取日志);
                        }
                        else
                        {
                            LogManager.AddMessage(string.Format("更新继电器:{0}信息失败", RelayGroups[i].Name), EnumLogSource.数据库存取日志, EnumLevel.Warning);
                        }
                        #endregion
                    }
                }
            }
        }
        #endregion
    }
}