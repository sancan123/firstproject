using System.ServiceModel;
using System.Collections.Generic;

namespace Mesurement.UiLayer.ViewModel.WcfService
{
    [ServiceContract]
    public interface IMisData
    {
        /// <summary>
        /// 客户端发起连接请求
        /// </summary>
        /// <returns>获取到的连接ID</returns>
        //[OperationContract]
        int RequestId();
        /// <summary>
        /// 获取表唯一编号
        /// </summary>
        /// <param name="connectionId">连接编号</param>
        /// <param name="isHistory">是否为历史记录</param>
        /// <param name="barCode">条形码</param>
        /// <returns>条码号对应的唯一编号字典:key:唯一编号,value:</returns>
        //[OperationContract]
        Dictionary<string, string> GetMeterPk(int connectionId, bool isHistory, string barCode);
        /// <summary>
        /// 获取一块表的检定数据
        /// </summary>
        /// <param name="connectionId">连接编号</param>
        /// <param name="isHistory">是否为历史数据</param>
        /// <param name="meterPk">条码号</param>
        /// <returns></returns>
        //[OperationContract]
        Dictionary<string, Dictionary<string, string>> GetOneMeterResult(int connectionId, bool isHistory, string meterPk);

        [OperationContract]
        int GetCostConsByBarcode(int codeType, string codeValue, string checkDate, ref string xmlValue);
    }
}
