using System;
using CLDC_DataCore.Struct;
using System.Text;
using System.Threading;
using CLDC_Comm.Enum;
using CLDC_DataCore.SocketModule.Packet;
using CLDC_DataCore.SocketModule;
using CLDC_DataCore.Const;

namespace CLDC_DeviceDriver.Drivers.Clou.Packets.Out
{

    #region 多路误差总线控制线程

    public class ReadWcbManager
    {
        #region
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
        public StPortInfo[] portNum { get; set; }

        /// <summary>
        /// 传入参数列表，指令不同参数不同，说明：
        /// 0x14 启动遥信输出：//TODO:
        /// </summary>
        public object[] Params { private get; set; }
        #endregion

        #region 数据结构
        /// <summary>
        /// 所有误差板数据
        /// </summary>
        public stError[] tagError;
        /// <summary>
        /// 每个表位的返回结果，只返回成功失败的命令用
        /// </summary>
        public RecvResult[] tagResults;

        #endregion
        /// <summary>
        /// 当前试验类型
        /// </summary>
        public enmTaskType m_curTaskType { get; set; }

        /// <summary>
        /// 工作线程数组
        /// </summary>
        private ReadWcbThread[] workThreads = new ReadWcbThread[0];

        /// <summary>
        /// 启动线程
        /// </summary>
        /// <returns>启动线程是否成功</returns>
        public bool Start( bool state)
        {
            //初始化结构
            int sLen = BwStatus.Length;
            tagError = new stError[sLen];


            for (int i = 0; i < BwStatus.Length; i++)
            {
                ///假如停止试验,则跳出
                //if (runFlag) return;

                //rc.BwStatus = bSelectBw;
                ///假如不需要检表,则跳出
                if (!BwStatus[i]) continue;

                tagError[i].szError = "";
                if (GlobalUnit.g_Dev_CommunType == Cus_CommunType.南网通讯DLL)
                {
                     int result = -1;
                    if(state == false)
                    {
                        int Flag = 1;
                        int meterControlType = GlobalUnit.g_CUS.DnbData.MeterGroup[i].Mb_intFKType;
                        result = DeviceControl.Instance.Dev_DeviceControl[0].RelayControl(i + 1, 3, meterControlType, ref Flag);
                        if (result == 0)
                        {
                            tagError[i].statusTypeIsOnErr_Yfftz = Flag == 0 ? true : false;
                        }
                        else
                        {
                            tagError[i].statusTypeIsOnErr_Yfftz = false;
                        }
                    }
                    else
                    {
                        byte Type = (byte)GlobalUnit.g_CurTestType; 
                        int Index=0;
                        int Num=0; 
                        string Data="";

                        result = DeviceControl.Instance.Dev_DeviceControl[0].ReadCurrentData(i + 1, ref Type, ref  Index, ref Num, ref Data);
                        if (result == 0)
                        {
                            tagError[i].szError = Data;
                            tagError[i].Index = Num;
                            tagError[i].MeterConst = Type;
                            tagError[i].MeterIndex = Index;
                        }
                      
                    }

                 
                }
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
        /// <summary>
        /// 所有线程完毕后，处理数据
        /// </summary>
        public void WaitAllThreaWorkDone()
        {
            bool isAllThreaWorkDone = false;
            while (!isAllThreaWorkDone)
            {
                isAllThreaWorkDone = IsWorkDone();
            }
            for (int i = 0; i < 1; i++)
            {
                int startpos = i * WcbPerChannelBwCount;
                {
                    Array.Copy(workThreads[i].tagError, startpos, tagError, startpos, WcbPerChannelBwCount);
                }
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
        #region
        /// <summary>
        /// 当前线程
        /// </summary>
        Thread workThread = null;
        /// <summary>
        /// 当前线程执行的方法
        /// </summary>
        ThreadStart StartMethod = null;
        /// <summary>
        /// 运行标志
        /// </summary>
        private bool runFlag = false;

        /// <summary>
        /// 工作完成标志
        /// </summary>
        private bool workOverFlag = false;

        /// <summary>
        /// 每个线程个数,一路误差总线上的表位数
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
        public StPortInfo PortNum { get; set; }

        /// <summary>
        /// 当前试验类型
        /// </summary>
        public enmTaskType m_curTaskType { get; set; }

        /// <summary>
        /// 线程ID
        /// </summary>
        public int ThreadID { get; set; }
        #endregion

        #region 数据结构，与ReadWcbManager一致
        /// <summary>
        /// 该端口下的误差板数据
        /// </summary>
        public stError[] tagError;
        /// <summary>
        /// 每个表位的返回结果，只返回成功失败的命令用
        /// </summary>
        public RecvResult[] tagResults;

        #endregion

        #region 线程处理
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
            StartWork();
        }
        #endregion

        #region 各种试验类型
        /// <summary>
        /// 读取误差，各种状态，报警等
        /// </summary>
        private void StartWork()
        {
            //初始化标志
            runFlag = false;
            workOverFlag = false;
            //调用方法
            try
            {
                tagError = new stError[bSelectBw.Length];
                for (int i = 0; i < bSelectBw.Length; i++)
                {
                    if (!bSelectBw[i]) continue;

                    tagError[i].szError = "";
                    if (GlobalUnit.g_Dev_CommunType == Cus_CommunType.南网通讯DLL)
                    {
                        int Flag = 1;
                        int meterControlType = GlobalUnit.g_CUS.DnbData.MeterGroup[i].Mb_intFKType;
                        int result = DeviceControl.Instance.Dev_DeviceControl[0].RelayControl(i + 1, 3,meterControlType, ref Flag);
                        if (result == 0)
                        {
                            tagError[i].statusTypeIsOnErr_Yfftz = Flag == 0 ? true : false;
                        }
                        else
                        {
                            tagError[i].statusTypeIsOnErr_Yfftz = false;
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

        #endregion

    }
    #endregion
}
