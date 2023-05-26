using System.Xml.Serialization;
using System.IO;
using CLDC_DataCore.Struct;
using CLDC_DeviceDriver.PortFactory;
namespace CLDC_MeterProtocol.Settings
{
    /// <summary>
    /// 通讯端口类型
    /// </summary>
    public enum PortType
    {

        CL20181 = 0,
        COMM = 1
    }

    public class DgnConfigManager:CLDC_Comm.BaseClass.SingletonBase<DgnConfigManager> 
    {
        private DgnPortInfo[] dicPortConfigs =new DgnPortInfo[0];
        private const string _configFilePath = "\\System\\Rs485PortConfig.xml";
        private string configFilePath = System.Windows.Forms.Application.StartupPath + _configFilePath;

        private StPortInfo[] m_arrRs485Port = null;
        private StPortInfo[] m_CarrierPort = null;
        private StPortInfo[] m_InfraredPort = null;
        /// <summary>
        /// 设置通道数
        /// </summary>
        public void SetChannelCount(int channelCount)
        {
            dicPortConfigs = new DgnPortInfo[channelCount];
            for (int i = 0; i < dicPortConfigs.Length;i++ )
            {
                dicPortConfigs[i] = new DgnPortInfo();
            }
        }
        /// <summary>
        /// 设置通道数
        /// </summary>
        public void SetChannelCount(int channelCount, int intBaseCom, string IP, string RemotePort, int LocalBasePort)
        {
            dicPortConfigs = new DgnPortInfo[channelCount];
            for (int i = 0; i < dicPortConfigs.Length; i++)
            {
                dicPortConfigs[i] = new DgnPortInfo()
                {
                    PortNumber = intBaseCom + i,
                    PortType = PortType.CL20181,
                    Setting = IP + "|" + RemotePort + "|" + LocalBasePort,

                };
            }
        }
        public int GetChannelCount()
        {
            return m_arrRs485Port.Length;
            //return dicPortConfigs.Length;
        }


        public StPortInfo GetConfig(int index)
        {
            return m_arrRs485Port[index];
            //return dicPortConfigs[index];
        }
        /// <summary>
        /// 获取当前载波端口
        /// </summary>
        /// <returns></returns>
        public StPortInfo getCarrierPort(int int_BwIndex)
        {
            int priCarrPort = 0;
            StPortInfo stPort = new StPortInfo();
            if (null != CLDC_DataCore.Const.GlobalUnit.CarrierInfos && CLDC_DataCore.Const.GlobalUnit.CarrierInfos.Length > int_BwIndex && null != CLDC_DataCore.Const.GlobalUnit.CarrierInfos[int_BwIndex])
            {
                if (int.TryParse(CLDC_DataCore.Const.GlobalUnit.CarrierInfos[int_BwIndex].ComPort.Replace("COM", ""), out priCarrPort))
                {

                    stPort.m_Port = priCarrPort;
                    stPort.m_IP = "193.168.18.1";
                //    stPort.m_IP = CLDC_DataCore.Const.GlobalUnit.CarrierInfos[int_BwIndex]
                   
                }
            }
          //  

           // stPort = CLDC_DataCore.Const.GlobalUnit.CarrierInfos[int_BwIndex];

       //     foreach (StPortInfo item in m_CarrierPort)
        //    {
        //        if (item.m_Port == priCarrPort)
        //        {
        //            stPort = item;
        //            break;
        //        }
       //     }
            return stPort;
        }

        /// <summary>
        /// 获取当前红外设备端口
        /// </summary>
        /// <returns></returns>
        public StPortInfo getInfraredPort()
        {

            return m_InfraredPort[0];
        }
        /// <summary>
        /// 加载配置
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            #region 加载485端口配置
            DeviceParaUnit[] paraUnitsRs485 = Rs485Setting.GetRs485Ports();
            m_arrRs485Port = new StPortInfo[paraUnitsRs485.Length];
            for (int i = 0; i < m_arrRs485Port.Length; i++)
            {
                m_arrRs485Port[i] = new StPortInfo
                {
                    m_Port = paraUnitsRs485[i].PortNo,
                    m_Port_IsUDPorCom = paraUnitsRs485[i].Address == "COM" ? false : true,
                    m_IP = paraUnitsRs485[i].Address,
                    m_Port_Setting = paraUnitsRs485[i].Baudrate,
                    m_Exist = 1
                };
            }
            #endregion

            #region 加载红外端口配置
            //DeviceParaUnit[] paraUnitsInfrared = Rs485Setting.GetInfraredPorts();
            //m_InfraredPort = new StPortInfo[paraUnitsInfrared.Length];
            //for (int i = 0; i < paraUnitsInfrared.Length; i++)
            //{
            //    m_InfraredPort[i] = new StPortInfo
            //    {
            //        m_Port = paraUnitsInfrared[i].PortNo,
            //        m_Port_IsUDPorCom = paraUnitsInfrared[i].Address == "COM" ? false : true,
            //        m_IP = paraUnitsInfrared[i].Address,
            //        m_Port_Setting = paraUnitsInfrared[i].Baudrate,
            //        m_Exist = 1
            //    };
            //}
            #endregion
            return true;

            
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            using (FileStream fs = new FileStream(configFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                XmlSerializer serializer = new XmlSerializer(dicPortConfigs.GetType());
                serializer.Serialize(fs, dicPortConfigs);
                return true;
            }
        }

        private void DefaultConfigs()
        {
            SetChannelCount(24);
            Save();
        }
    }
}
