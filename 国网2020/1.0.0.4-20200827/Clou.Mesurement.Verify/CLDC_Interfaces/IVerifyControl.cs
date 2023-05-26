using System.ServiceModel;
using System.Xml;

namespace CLDC_Interfaces
{
    [ServiceContract]
    public interface IVerifyControl
    {
        /// <summary>
        /// 请求ID
        /// </summary>
        /// <returns>标识ID</returns>
        [OperationContract]
        int RequestID();
        /// <summary>
        /// 登入接口
        /// </summary>
        /// <param name="connectionID">凭证ID</param>
        /// <returns>成功、失败</returns>
        [OperationContract]
        bool Login(int connectionID);
        /// <summary>
        /// 设置表位数
        /// </summary>
        /// <param name="connectionID">连接密码</param>
        /// <param name="equipmentID">台体编号</param>
        /// <param name="meterSum">表位数</param>
        /// <param name="isDan">台体为单相台</param>
        /// <param name="isDan">演示模式</param>
        /// <returns></returns>
        [OperationContract]
        bool InitialEquipment(int connectionID, string equipmentID, int meterCount, bool isDan, bool isDemo, string[] EquipmentInfos);

        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <param name="connectionID"></param>
        /// <param name="deviceParams">设备ID|[设备名]|数量|序号|IP或“COM”|端口号|驱动文件全名|完整类名</param>
        /// <returns></returns>
        [OperationContract]
        int InitDevice(int connectionID, string[] deviceParams);

        /// <summary>
        /// 设置被检设备信息
        /// </summary>
        /// <param name="meterInfos">被检设备信息数组，数组长度代表装置表位数</param>
        /// <returns></returns>
        [OperationContract]
        bool SetMerter(int connectionID, int meterCount, MeterInfo[] meterInfos);
      
     
        /// <summary>
        /// 同步被检设备检定标志
        /// </summary>
        /// <param name="checkFlag">检定标志数组</param>
        /// <returns></returns>
        [OperationContract]
        bool UpdateCheckFlag(int connectionID, bool[] checkFlag);
        /// <summary>
        /// 开始检定itemKey指定的项目
        /// </summary>
        /// <param name="connectionID">凭证ID</param>
        /// <param name="itemName">检定项名称</param>
        /// <param name="itemKey">项目号</param>
        /// <param name="className">方法名称</param>
        /// <param name="formatPara">实验参数</param>
        /// <param name="formatParaValue">实验参数值</param>
        /// <param name="option">其它操作</param>
        /// <returns>成功、失败</returns>
        [OperationContract]
        bool Start(int connectionID, string itemName, string itemKey, string className, string formatPara, string formatParaValue, string option);
        /// <summary>
        /// 停止检定
        /// </summary>
        /// <param name="connectionID">凭证ID</param>
        /// <param name="itemKey">检定点编号</param>
        /// <returns>成功、失败</returns>
        [OperationContract]
        bool Stop(int connectionID,string itemKey);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionID"></param>
        /// <param name="MethodName"></param>
        /// <param name="paramArry"></param>
        /// <returns></returns>
        [OperationContract]
        int DeviceControl(int connectionID, string MethodName, object[] paramArry);

        /// <summary>
        /// 下发表协议
        /// </summary>
        /// <param name="connectionID"></param>
        /// <param name="nodeProtocols"></param>
        /// <returns></returns>
        [OperationContract]
        int LoadMeterProtocols(int connectionID, XmlElement nodeProtocols);
        /// <summary>
        /// 下发检定参数
        /// </summary>
        /// <param name="connectionID"></param>
        /// <param name="configId">配置信息编号</param>
        /// <param name="strConfig">配置信息内容</param>
        /// <returns></returns>
        [OperationContract]
        int InitialCheckParam(int connectionID, string configId,string strConfig);
    }
}
