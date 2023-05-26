using CLDC_DataCore.SocketModule.Packet;
using CLDC_DataCore.SocketModule;
using CLDC_DataCore.Struct;
namespace CLDC_DeviceDriver.Drivers
{
    abstract class DriverBase
    {
        
        /// <summary>
        /// 消息回调函数
        /// </summary>
        public MsgCallBack msgCallBack;

        /// <summary>
        /// 表位数
        /// </summary>
        public int g_Bws = 0;

        /// <summary>
        /// 当前工作流状态
        /// </summary>
        public WorkFlow currentWorkFlow = WorkFlow.None;
        private int bws;

        public DriverBase(int bws)
        {
            this.g_Bws = bws;
            this.InitSetting();
            
        }

        public DriverBase(int bws, string[] arrayDevice)
        {
            // TODO: Complete member initialization
            this.g_Bws = bws;
        }

        /// <summary>
        /// 初始化设置类
        /// </summary>
        protected abstract void InitSetting();




    }
}
