using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out;
using CLDC_DeviceDriver.Drivers.Clou.V80.Packets.In;

namespace CLDC_DeviceDriver.Drivers.Clou.V80
{
    /// <summary>
    /// CL2018误差板数据多线程获取误差数据
    /// 调用方法:
    /// ReadWcbManager readWcb=new ReadWcbManager();
    /// readWcb.WcbChannelCount=4;
    /// readWcb.WcbPerChannelBwCount=6;
    /// readWcb.bSelectBw=bSelectBw;
    /// readWcb.portNum=portNum;
    /// readWcb.m_curTaskType=m_curTaskType;
    /// readWcb.Start();
    /// WaitAllThreaWorkDone();
    /// tagError=readWcb.tagError;
    /// </summary>
    public class ReadWcbManager
    {
        /// <summary>
        /// 最大线程数量
        /// </summary>
        public int WcbChannelCount { get; set; }

        /// <summary>
        /// 每个线程最大任务数
        /// </summary>
        public int WcbPerChannelBwCount { get; set; }

        private bool[] bwStatus;
        /// <summary>
        /// 所有表位状态
        /// </summary>
        public bool[] BwStatus 
        {
            get
            {
                return bwStatus;
            }

            set
            {
                bwStatus = value;
            }
        }

        /// <summary>
        /// 误差板端口系列
        /// </summary>
        public int[] portNum { get; set; }

        /// <summary>
        /// 所有误差板数据
        /// </summary>
        public stError[] tagError;

        /// <summary>
        /// 当前试验类型
        /// </summary>
        public CLDC_DeviceDriver.Drivers.Clou.V80.enmTaskType m_curTaskType { get; set; }

        /// <summary>
        /// 工作线程数组
        /// </summary>
        private ReadWcbThread[] workThreads = new ReadWcbThread[0];

        /// <summary>
        /// 启动线程
        /// </summary>
        /// <returns>启动线程是否成功</returns>
        public bool Start()
        {
            //结束上一次的线程
            workThreads = new ReadWcbThread[WcbChannelCount];
            tagError = new stError[BwStatus.Length];
            for (int i = 0; i < WcbChannelCount; i++)
            {
                ReadWcbThread newThread = new ReadWcbThread()
                {
                    ThreadID = i+1,                      //线程编号,用于线程自己推导起始位置
                    ThreadPerCount = WcbPerChannelBwCount,
                    bSelectBw = BwStatus,
                    PortNum = portNum[i],
                    m_curTaskType = m_curTaskType
                };

                newThread.bSelectBw = BwStatus;
                workThreads[i] = newThread;
                newThread.Start();
            }
            return true;
        }

        /// <summary>
        /// 停止所有工作线程
        /// </summary>
        public void Stop()
        {
            //首先发出停止指令
            foreach (ReadWcbThread workthread in workThreads)
            {
                workthread.Stop();
            }
        }

        public void WaitAllThreaWorkDone()
        {
            bool isAllThreaWorkDone = false;
            while (!isAllThreaWorkDone)
            {
                isAllThreaWorkDone = IsWorkDone();
            }
            for (int i = 0; i < workThreads.Length; i++)
            {
                int startpos = i * WcbPerChannelBwCount;
                Array.Copy(workThreads[i].tagError, startpos, tagError, startpos, WcbPerChannelBwCount);
            }
        }

        /// <summary>
        /// 等待所有线程工作完成
        /// </summary>
        public bool IsWorkDone()
        {
            bool isAllThreaWorkDone = true;

            foreach (ReadWcbThread workthread in workThreads)
            {
                isAllThreaWorkDone = workthread.IsWorkFinish();
                if (!isAllThreaWorkDone) break;
            }
            return isAllThreaWorkDone;
        }
        
    }

    public class ReadWcbThread
    {
        /// <summary>
        /// 当前线程
        /// </summary>
        Thread workThread = null;

        /// <summary>
        /// 运行标志
        /// </summary>
        private bool runFlag = false;

        /// <summary>
        /// 工作完成标志
        /// </summary>
        private bool workOverFlag = false;

        /// <summary>
        /// 每个线程个数
        /// </summary>
        public int ThreadPerCount { get; set; }

        private bool[] bSelectStatus;
        /// <summary>
        /// 所有表位状态
        /// </summary>
        public bool[] bSelectBw 
        {
            get
            {
                return bSelectStatus;
            }
            set
            {
                bSelectStatus = value;
            }
        }

        /// <summary>
        /// 误差板端口号
        /// </summary>
        public int PortNum { get; set; }

        /// <summary>
        /// 当前试验类型
        /// </summary>
        public CLDC_DeviceDriver.Drivers.Clou.V80.enmTaskType m_curTaskType { get; set; }

        /// <summary>
        /// 线程ID
        /// </summary>
        public int ThreadID{get;set;}

        /// <summary>
        /// 该端口下的误差板数据
        /// </summary>
        public stError[] tagError;

        /// <summary>
        /// 停止当前工作任务
        /// </summary>
        public void Stop()
        {
            runFlag = true;
            workThread.Abort();
        }

        /// <summary>
        /// 工作线程是否完成
        /// </summary>
        /// <returns></returns>
        public bool IsWorkFinish()
        {
            return workOverFlag;
        }

        /// <summary>
        /// 启动工作线程
        /// </summary>
        /// <param name="paras"></param>
        public void Start()
        {
            workThread = new Thread(StartWork);
            workThread.Start();
        }
        private void StartWork()
        {
            //初始化标志
            runFlag = false;
            workOverFlag = false;
            //调用方法
            try
            {
                //计算负载
                int startpos = (ThreadID - 1) * ThreadPerCount;
                int endpos = startpos + ThreadPerCount;
                CL188L_RequestReadBwWcAndStatusPacket rc = new CL188L_RequestReadBwWcAndStatusPacket((CLDC_Comm.Enum.Cus_WuchaType)CLDC_Comm.GlobalUnit.g_CurTestType);
                CL188L_RequestReadBwWcAndStatusReplyPacket rcback = new CL188L_RequestReadBwWcAndStatusReplyPacket();
                tagError = new stError[bSelectBw.Length];
                bool[] newSelectBw = new bool[bSelectBw.Length];
                for (int i = startpos; i < endpos; i++)
                {
                    ///假如停止试验,则跳出
                    if (runFlag) return;
                    ///假如不需要检表,则跳出
                    if (!bSelectBw[i]) continue;
                    //重新获取表位状态
                    rc.Pos = i+1;
                    rc.ChannelNo = ThreadID - 1;
                    //rc.BwStatus = SelectOneBwChannel(bSelectBw, i); 
                    rc.BwStatus = bSelectBw;
                    tagError[i].szError = "";
                    for (int j = 0; j < 3; i++)
                    {
                        if (SendData(PortNum, rc, rcback))
                        {
                            tagError[i].szError = rcback.wcData;
                            tagError[i].Index = rcback.wcNum;
                            tagError[i].MeterIndex = i;
                            if (m_curTaskType == CLDC_DeviceDriver.Drivers.Clou.V80.enmTaskType.需量周期)
                            {
                                tagError[i].Index = (tagError[i].Index + 1) / 10;
                            }
                            break;
                        }
                    }
                }
            }
            catch { }
            finally
            {
                //恢复标志
                runFlag = true;
                workOverFlag = true;
            }
        }

        /// <summary>
        /// 发送误差板端口数据
        /// </summary>
        /// <param name="port"></param>
        /// <param name="sendPacket"></param>
        /// <param name="recvPacket"></param>
        /// <returns></returns>
        private bool SendData(int port, CLDC_Comm.SocketModule.Packet.SendPacket sendPacket, CLDC_Comm.SocketModule.Packet.RecvPacket recvPacket)
        {
            string portName = GetPortNameByPortNumber(port);

            return CLDC_Comm.SocketModule.SockPool.Instance.Send(portName, sendPacket, recvPacket);
        }

        /// <summary>
        /// 根据端口号获取端口名
        /// </summary>
        /// <param name="port">端口号</param>
        /// <returns>端口名</returns>
        private string GetPortNameByPortNumber(int port)
        {
            return string.Format("Port_{0}", port);
        }

        /// <summary>
        /// 切换到指定表位通道
        /// </summary>
        /// <param name="bwdata"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool[] SelectOneBwChannel(bool[] bwdata, int index)
        {
            for (int i = 0; i < bwdata.Length; i++)
            {
                bwdata[i]=(i == index);
            }
            return bwdata;
        }

    }
}
