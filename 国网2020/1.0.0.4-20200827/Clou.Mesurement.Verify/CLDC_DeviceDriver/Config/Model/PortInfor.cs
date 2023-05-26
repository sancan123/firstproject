using System.Collections.Generic;

namespace CLDC_DeviceDriver.Config.Model
{
    /// 端口信息
    /// <summary>
    /// 端口信息
    /// </summary>
    public class PortInfor
    {
        /// 序号
        /// <summary>
        /// 序号
        /// </summary>
        public int Index { get; set; }
        private List<ItemInfor> inforList = new List<ItemInfor>();
        /// 端口参数列表
        /// <summary>
        /// 端口参数列表
        /// </summary>
        public List<ItemInfor> InforList
        {
            get
            {
                if (inforList == null)
                {
                    inforList = new List<ItemInfor>();
                }
                return inforList;
            }
            internal set
            {
                inforList = value;
            }
        }
        /// 端口号
        /// <summary>
        /// 端口号
        /// </summary>
        public int Number
        {
            get 
            {
                return int.Parse(inforList[0].Content);
            }
        }
        /// 服务器名
        /// <summary>
        /// 服务器名
        /// </summary>
        public string Server
        {
            get
            {
                return inforList[1].Content;
            }
        }
        /// 串口参数
        /// <summary>
        /// 串口参数
        /// </summary>
        public string Parameter
        {
            get
            {
                return inforList[2].Content;
            }
        }
        /// 接收最大等待时间，单位ms
        /// <summary>
        /// 接收最大等待时间，单位ms
        /// </summary>
        public int MaxWaitTme
        {
            get 
            {
                return int.Parse(inforList[3].Content);
            }
        }
        /// 字节间最大时间，单位ms
        /// <summary>
        /// 字节间最大时间，单位ms
        /// </summary>
        public int WaitSencondsPerByte
        {
            get
            {
                return int.Parse(inforList[4].Content);
            }
        }
    }
}
