using System.ServiceModel;

namespace CLDC_Interfaces
{
    [ServiceContract]
    public interface IVerifyForMis
    {
        /// <summary>
        /// 刷新数据（空闲状态下刷新）
        /// </summary>
        [OperationContract]
        void RefreshData();
    }
}
