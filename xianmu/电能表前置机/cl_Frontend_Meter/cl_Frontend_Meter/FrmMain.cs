using E_CL188L;
using E_CL188M;
using E_CL191B;
using E_CL2029D;
using E_CL303;
using E_CL309;
using E_CL3112;
using E_CL3115;
using E_CL311V2;
using E_CL485;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace cl_Frontend_Meter
{
    public partial class FrmMain : Form
    {
        /// <summary>
        /// 表位数
        /// </summary>
        int TableCount = 40;


        RichTextBox[] TbackStr;



        /// <summary>
        /// 是否显示到当前最后一行
        /// </summary>
        readonly bool blScroll = true;

        public Dictionary<int, CL188M> lst_CL188m = new Dictionary<int, CL188M>();
        public Dictionary<int, CL188L> lst_CL188l = new Dictionary<int, CL188L>();
        public Dictionary<int, CL_Rs485> lst_CL_Rs485 = new Dictionary<int, CL_Rs485>();
        public Dictionary<int, CL303> lst_CL303 = new Dictionary<int, CL303>();
        public Dictionary<int, CL3115> lst_CL3115 = new Dictionary<int, CL3115>();
        public Dictionary<int, CL3112> lst_CL3112 = new Dictionary<int, CL3112>();
        public Dictionary<int, CL191B> lst_CL191B = new Dictionary<int, CL191B>();
        public Dictionary<int, CL2018> lst_CL2018 = new Dictionary<int, CL2018>();
        public Dictionary<int, CL309> lst_CL309 = new Dictionary<int, CL309>();
        public Dictionary<int, CL311V2> lst_CL311V2 = new Dictionary<int, CL311V2>();
        public Dictionary<int, CL2029D> lst_CL2029D = new Dictionary<int, CL2029D>();

        int int_MaxWaitTime = 30000;
        public FrmMain()
        {
            InitializeComponent();
        }

        IPEndPoint RemoteTlocat;
        readonly Queue<EventsSendData> queueSendData = new Queue<EventsSendData>();//设置指令集合
        private void FrmMain_Load(object sender, EventArgs e)
        {
            notifyIcon1.Visible = true;
            GetDeviceini();
            TbackStr = new RichTextBox[TableCount + 1];

            RichTextBox txTmp = new RichTextBox
            {
                Text = "",
                Name = "log_Main",
                Location = new Point(3, 3),
                Multiline = true,
                Dock = DockStyle.Fill
            };
            txTmp.MouseDoubleClick += new MouseEventHandler(TxTmp_MouseDoubleClick);
            TbackStr[0] = txTmp;

            TabPage tbTmp = new TabPage() { Text = "与主程序通信", Name = "Main" };
            tbTmp.Controls.Add(txTmp);

            tabTableLog.Controls.Clear();
            tabTableLog.Controls.Add(tbTmp);


            chk_IsLog.Checked = false;
            RemoteTlocat = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10006);

            #region 继电器1
            //继电器闭合ID
            int[] arr0 = new int[0];
            string str0 = File.ReadInIString(Application.StartupPath + "\\System\\Driver继电器配置.ini", "Relay", "CLSupplyOpen", "1");
            if (!string.IsNullOrEmpty(str0.Trim()))
            {
                string[] arr = str0.Split(',');
                arr0 = new int[arr.Length];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr0[i] = int.Parse(arr[i]);
                }
            }


            //继电器断开ID
            int[] arr1 = new int[0];
            string str1 = File.ReadInIString(Application.StartupPath + "\\System\\Driver继电器配置.ini", "Relay", "CLSupplyClose", "2");
            if (!string.IsNullOrEmpty(str1.Trim()))
            {
                string[] arr = str1.Split(',');
                arr1 = new int[arr.Length];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr1[i] = int.Parse(arr[i]);
                }
            }

            if (lst_CL2029D.Count > 0 && (arr0.Length > 0 || arr1.Length > 0))
            {
                lst_CL2029D[1].SetPowerSupplyType(3, false, arr0, arr1);
            }
            #endregion 继电器1

            #region 继电器2
            //继电器闭合ID
            int[] arr10 = new int[0];
            string str10 = File.ReadInIString(Application.StartupPath + "\\System\\Driver继电器配置.ini", "Relay", "CLSupplyOpen2", "1");
            if (!string.IsNullOrEmpty(str10.Trim()))
            {
                string[] arr = str10.Split(',');
                arr10 = new int[arr.Length];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr10[i] = int.Parse(arr[i]);
                }
            }


            //继电器断开ID
            int[] arr11 = new int[0];
            string str11 = File.ReadInIString(Application.StartupPath + "\\System\\Driver继电器配置.ini", "Relay", "CLSupplyClose2", "2");
            if (!string.IsNullOrEmpty(str11.Trim()))
            {
                string[] arr = str11.Split(',');
                arr11 = new int[arr.Length];
                for (int i = 0; i < arr.Length; i++)
                {
                    arr11[i] = int.Parse(arr[i]);
                }
            }

            if (lst_CL2029D.Count > 1 && (arr10.Length > 0 || arr11.Length > 0))
            {
                lst_CL2029D[2].SetPowerSupplyType(3, false, arr10, arr11);
            }
            #endregion 继电器2



            // 初始化继电器
            if (lst_CL188m.Count > 0)
            {

                foreach (KeyValuePair<int, CL188M> kvp in lst_CL188m)
                {
                    kvp.Value.B_SelectStatus = new bool[lst_CL_Rs485.Count];
                    kvp.Value.Int_ChannelNo = 0;
                    kvp.Value.Int_ChannelNum = lst_CL188m.Count;

                    for (int i = 0; i < lst_CL_Rs485.Count; i++)
                    {
                        kvp.Value.B_SelectStatus[i] = true;
                    }
                    kvp.Value.InitSwitch();
                    break;
                }

            }


            IPEndPoint local = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 10004);
            receiveUdpClient = new UdpClient(local);
            Thread t = new Thread(AcceptC);
            t.Start();
            Thread t2 = new Thread(AcceptC2);
            t2.Start();
            Thread t3 = new Thread(AcceptC3);
            t3.Start();
            this.Visible = false;

        }

        readonly object obj = new object();
        readonly object obj2 = new object();

        /// <summary>
        /// 定时轮询接收到的数据，将其发给主程序
        /// </summary>
        private void AcceptC2()
        {
            while (true)
            {
                Thread.Sleep(5);
                lock (obj2)
                {
                    if (Program.queueReceiveData.Count > 0)
                    {
                        string str = "";
                        EventsReceiveData cmd = Program.queueReceiveData.Dequeue();
                        if (cmd == null)
                        { }
                        else if (cmd.lastcmd == "cmd=1002")
                        {
                            str = "cmd=1002,data=" + cmd.bw + ";" + cmd.data + "\r\n";
                        }
                        else if (cmd.lastcmd == "cmd=1001")
                        {
                            str = "cmd=1001,data=" + cmd.bw + ";" + cmd.data + "\r\n";
                        }
                        else if (cmd.lastcmd == "cmd=0301")
                        {
                            str = "cmd=0301,ret=0,data=" + cmd.data + "\r\n";
                        }
                        else if (cmd.lastcmd == "cmd=0413")
                        {
                            str = "cmd=0413,ret=0,data=" + cmd.data + "\r\n";
                        }
                        else
                        {
                            str = cmd.lastcmd + ",ret=0,data=null" + "\r\n";
                        }

                        ChangeControlInvoke(this, () =>
                        {

                            int lenth = TbackStr[0].Text.Length;

                            string msg = $"[{DateTime.Now}]To PC:{str}";
                            TbackStr[0].AppendText(msg);
                            TbackStr[0].Select(lenth, msg.Length);
                            TbackStr[0].SelectionColor = Color.Blue;
                            TbackStr[0].AppendText("\r\n");
                            TbackStr[0].ScrollToCaret();

                            if (chk_IsLog.Checked)
                                CLBase.WriteLog("测试日志", msg);
                        });

                        receiveUdpClient.Send(System.Text.Encoding.ASCII.GetBytes(str), System.Text.Encoding.ASCII.GetBytes(str).Length, RemoteTlocat);
                    }
                }
            }
        }

        /// <summary>
        /// 定时轮询主程序的数据队列，将其发送出去
        /// </summary>
        private void AcceptC3()
        {
            string[] outarry = new string[0];
            EventsSendData cmd = null;
            while (true)
            {
                Thread.Sleep(5);
                lock (obj)
                {
                    if (queueSendData.Count > 0)
                    {
                        cmd = queueSendData.Dequeue();
                        if (cmd.cmd == "cmd=1002")
                        {
                            int bw = int.Parse(cmd.data.Split(';')[0].Substring(6, cmd.data.Split(';')[0].Length - 6));
                            string data = cmd.data.Split(';')[1];
                            lst_CL2018[bw].SendData(data);
                        }
                        else if (cmd.cmd == "cmd=1001")
                        {
                            string[] stmp = cmd.data.Split(';');
                            Thread t = new Thread(SetCmd1001);
                            t.Start(stmp);
                        }
                        else if (cmd.cmd == "cmd=0101")
                        {
                            lock (obj2)
                            {
                                Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0101" });
                            }
                        }
                        else if (cmd.cmd == "cmd=0102")
                        {
                            int bw = int.Parse(cmd.data.Split(';')[0]);
                            string data = cmd.data.Split(';')[1];
                            if (bw >= 1 && bw <= 16)
                            {
                                foreach (CL_Rs485 cl485 in lst_CL_Rs485.Values)
                                {
                                    if (cl485.m_Rs485Port[0].m_Port == bw)
                                    {
                                        cl485.m_Rs485Port[0].m_Port_Setting = data;
                                    }
                                    break;
                                }
                            }
                            else if (bw >= 17 && bw <= 32)
                            {
                                foreach (CL2018 cl2018 in lst_CL2018.Values)
                                {
                                    if (cl2018.CommPort == bw)
                                    {
                                        cl2018.G_BTL = data;
                                        cl2018.bol_StopComm = false;
                                        cl2018.Open();
                                        break;
                                    }
                                }
                            }
                            lock (obj2)
                            {
                                Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0102" });
                            }
                        }
                        else if (cmd.cmd == "cmd=0103")
                        {
                            int bw = int.Parse(cmd.data.Split(';')[0]);
                            string data = cmd.data.Split(';')[1];
                            if (bw > 17 && bw <= 32)
                            {
                                foreach (CL2018 cl2018 in lst_CL2018.Values)
                                {
                                    if (cl2018.CommPort == bw)
                                    {
                                        cl2018.bol_StopComm = true;
                                        break;
                                    }
                                }
                            }
                            lock (obj2)
                            {
                                Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0103" });
                            }
                        }
                        else if (cmd.cmd == "cmd=0104")//暂时不对这条指令进行解析，先满足面向对象协议
                        {
                            string[] stmp = cmd.data.Split(';');
                            Thread t = new Thread(SetCmd0104);
                            t.Start(stmp);
                        }
                        else if (cmd.cmd == "cmd=0201")
                        {
                            string[] stmp = cmd.data.Split(';');
                            Thread t = new Thread(SetCmd0201);
                            t.Start(stmp);
                        }
                        else if (cmd.cmd == "cmd=0202")
                        {
                            string[] stmp = cmd.data.Split(';');
                            Thread t = new Thread(SetCmd0202);
                            t.Start(stmp);
                        }
                        else if (cmd.cmd == "cmd=0203")
                        {
                            string[] stmp = cmd.data.Split(';');
                            Thread t = new Thread(SetCmd0203);
                            t.Start(stmp);
                        }
                        else if (cmd.cmd == "cmd=0204")
                        {
                            string[] stmp = cmd.data.Split(';');
                            Thread t = new Thread(SetCmd0204);
                            t.Start(stmp);
                        }
                        else if (cmd.cmd == "cmd=0301")
                        {
                            string[] stmp = cmd.data.Split(';');
                            Thread t = new Thread(SetCmd0301);
                            t.Start(stmp);
                        }
                        else if (cmd.cmd == "cmd=0401")
                        {
                            string[] stmp = cmd.data.Split(';');
                            Thread t = new Thread(SetCmd0401);
                            t.Start(stmp);
                        }
                        else if (cmd.cmd == "cmd=0402")//台体没有这种指令,后续扩展
                        {
                        }
                        else if (cmd.cmd == "cmd=0403")//设置台体遥信状态
                        {
                            string[] stmp = cmd.data.Split(';');
                            Thread t = new Thread(SetCmd0403);
                            t.Start(stmp);
                        }
                        else if (cmd.cmd == "cmd=0404")//台体不支持脉冲,后续扩展
                        {

                        }
                        else if (cmd.cmd == "cmd=0405")//台体不支持脉冲,后续扩展
                        {

                        }
                        else if (cmd.cmd == "cmd=0406")//台体脉冲设置
                        {
                            string[] stmp = cmd.data.Split(';');
                            Thread t = new Thread(SetCmd0406);
                            t.Start(stmp);
                        }
                        else if (cmd.cmd == "cmd=0407")
                        {
                            string[] stmp = cmd.data.Split(';');
                            Thread t = new Thread(SetCmd0407);
                            t.Start(stmp);
                        }
                        else if (cmd.cmd == "cmd=0408")
                        {
                            string[] stmp = cmd.data.Split(';');
                            Thread t = new Thread(SetCmd0408);
                            t.Start(stmp);
                        }
                        else if (cmd.cmd == "cmd=0409")
                        {
                            string[] stmp = cmd.data.Split(';');
                            Thread t = new Thread(SetCmd0409);
                            t.Start(stmp);
                        }
                        else if (cmd.cmd == "cmd=040A")
                        {
                            string[] stmp = cmd.data.Split(';');
                            Thread t = new Thread(SetCmd040A);
                            t.Start(stmp);
                        }
                        else if (cmd.cmd == "cmd=040B")
                        {
                            string[] stmp = cmd.data.Split(';');
                            Thread t = new Thread(SetCmd040B);
                            t.Start(stmp);
                        }
                        else if (cmd.cmd == "cmd=040C")
                        {

                        }
                        else if (cmd.cmd == "cmd=040D")
                        {
                            string[] stmp = cmd.data.Split(';');
                            Thread t = new Thread(SetCmd040D);
                            t.Start(stmp);
                        }
                        else if (cmd.cmd == "cmd=0411")
                        {
                            string[] stmp = cmd.data.Split(';');
                            Thread t = new Thread(SetCmd0411);
                            t.Start(stmp);
                        }
                        else if (cmd.cmd == "cmd=0412")
                        {
                            string[] stmp = cmd.data.Split(';');
                            Thread t = new Thread(SetCmd0412);
                            t.Start(stmp);
                        }
                        else if (cmd.cmd == "cmd=0413")
                        {
                            string[] stmp = cmd.data.Split(';');
                            Thread t = new Thread(SetCmd0413);
                            t.Start(stmp);
                        }
                        else
                        {

                        }
                    }
                }
            }
        }

        private void SetCmd1001(object ob)
        {
            int id = int.Parse(((string[])ob)[0]);
            string data = ((string[])ob)[2];
            CL_Rs485 rs485 = lst_CL_Rs485[id];

            if (chk_IsLog.Checked)
                CLBase.WriteLog($"表位{id}", $"发送{data}");

            int ret = rs485.SendData(CLBase.ConvertStringToBytes(data), out byte[] outFrame, int_MaxWaitTime);
            string data2 = CLBase.ConvertBytesToString(outFrame);
            if (ret == 0)
            {
                lock (obj2)
                {
                    Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=1001", data = data2, bw = id });
                }
                if (chk_IsLog.Checked)
                    CLBase.WriteLog($"表位{id}" , $"接收{data2}");
            }
            else//临时测试
            {
                if (chk_IsLog.Checked)
                    CLBase.WriteLog($"表位{id}", "返回失败");
            }
        }

        private void SetCmd0411(object ob)
        {
            //string[] FrameAry = new string[0];
            int id = int.Parse(((string[])(ob))[0]);

            if (lst_CL191B.Count > 0)
                lst_CL191B[1].SetTimePulse(true, out _);

            bool[] p_bol_Positions = new bool[lst_CL_Rs485.Count];
            p_bol_Positions[id - 1] = true;
            int bwid = GetId(id);

            int ret;
            if (lst_CL188m.ContainsKey(bwid + 1))
            {
                CL188M cl188m = lst_CL188m[bwid + 1];
                cl188m.B_SelectStatus = p_bol_Positions;
                cl188m.Int_ChannelNo = bwid;
                cl188m.Int_ChannelNum = lst_CL188m.Count;
                cl188m.SetPulseChannelAndType(0, 5, 0, 0, 1, 2, out _);
            }

            if (lst_CL188m.ContainsKey(bwid + 1))
            {
                CL188M cl188m = lst_CL188m[bwid + 1];
                cl188m.B_SelectStatus = p_bol_Positions;
                cl188m.Int_ChannelNo = bwid;
                cl188m.Int_ChannelNum = lst_CL188m.Count;
                cl188m.SetClockFrequency(0, 500000, 1, 10, out _);

            }

            if (lst_CL188m.ContainsKey(bwid + 1))
            {
                CL188M cl188m = lst_CL188m[bwid + 1];
                cl188m.B_SelectStatus = p_bol_Positions;
                cl188m.Int_ChannelNo = bwid;
                cl188m.Int_ChannelNum = lst_CL188m.Count;
                ret = cl188m.StartTest(0, 2, out _);
                if (ret == 0)
                {
                    lock (obj2)
                    {
                        Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0411" });
                    }
                }
            }

            if (lst_CL188l.ContainsKey(bwid + 1))
            {
                CL188L cl188m = lst_CL188l[bwid + 1];
                cl188m.SelectStatus = p_bol_Positions;
                cl188m.ChannelNo = bwid;
                cl188m.ChannelNum = lst_CL188l.Count;
                cl188m.SetPulseChannelAndType(0, 5, 0, 0, 1, 2, out _);
            }

            if (lst_CL188l.ContainsKey(bwid + 1))
            {
                CL188L cl188m = lst_CL188l[bwid + 1];
                cl188m.SelectStatus = p_bol_Positions;
                cl188m.ChannelNo = bwid;
                cl188m.ChannelNum = lst_CL188l.Count;
                cl188m.SetClockFrequency(0, 500000, 1, 10, out _);

            }

            if (lst_CL188l.ContainsKey(bwid + 1))
            {
                CL188L cl188m = lst_CL188l[bwid + 1];
                cl188m.SelectStatus = p_bol_Positions;
                cl188m.ChannelNo = bwid;
                cl188m.ChannelNum = lst_CL188l.Count;
                ret = cl188m.StartTest(0, 2, out _);
                if (ret == 0)
                {
                    lock (obj2)
                    {
                        Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0411" });
                    }
                }
            }

            if (lst_CL188m.Count == 0 && lst_CL188l.Count == 0)
            {
                Thread.Sleep(100);
                lock (obj2)
                {
                    Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0411" });
                }
            }
        }

        private void SetCmd0412(object ob)
        {
            //string[] FrameAry = new string[0];
            int id = int.Parse(((string[])(ob))[0]);


            bool[] p_bol_Positions = new bool[lst_CL_Rs485.Count];
            p_bol_Positions[id - 1] = true;
            int bwid = GetId(id);
            if (lst_CL188m.ContainsKey(bwid + 1))
            {
                CL188M cl188m = lst_CL188m[bwid + 1];
                cl188m.B_SelectStatus = p_bol_Positions;
                cl188m.Int_ChannelNo = bwid;
                cl188m.Int_ChannelNum = lst_CL188m.Count;
                //int ret = cl188m.Connect(0, out FrameAry);
                int ret = cl188m.StopTest(id, 2, out _);
                if (ret == 0)
                    Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0412" });
            }

            if (lst_CL188l.ContainsKey(bwid + 1))
            {
                CL188L cl188m = lst_CL188l[bwid + 1];
                cl188m.SelectStatus = p_bol_Positions;
                cl188m.ChannelNo = bwid;
                cl188m.ChannelNum = lst_CL188l.Count;
                //int ret = cl188m.Connect(0, out FrameAry);
                int ret = cl188m.StopTest(id, 2, out _);
                if (ret == 0)
                    Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0412" });
            }

            if (lst_CL188m.Count == 0 && lst_CL188l.Count == 0)
            {
                Thread.Sleep(100);
                lock (obj2)
                {
                    Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0412" });
                }
            }
        }

        private void SetCmd0413(object ob)
        {
            int id = int.Parse(((string[])(ob))[0]);

            //byte verificationType = 0;
            //bool[] statusType = new bool[0];
            //byte CurrentContour = 0;
            //byte VoltageContour = 0;
            //byte CommunicationType = 0;
            //bool[] workType = new bool[0];
            //byte expandStatus = 0;

            bool[] p_bol_Positions = new bool[lst_CL_Rs485.Count];
            p_bol_Positions[id - 1] = true;
            int bwid = GetId(id);
            //string[] FrameAry = new string[0];
            //int MeterIndex = 0;
            //int ErrorNum = 0;
            //string wcData;
            if (lst_CL188m.ContainsKey(bwid + 1))
            {
                CL188M cl188m = lst_CL188m[bwid + 1];
                cl188m.B_SelectStatus = p_bol_Positions;
                cl188m.Int_ChannelNo = bwid;
                cl188m.Int_ChannelNum = lst_CL188m.Count;
                int ret = cl188m.ReadCurrentData(bwid + 1, 2, out _, out _, out _, out string wcData, out bool[] _, out byte _, out byte _, out byte _, out bool[] _, out byte _, out _);
                if (ret == 0)
                {
                    lock (obj2)
                    {
                        Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0413", bw = id, data = wcData });
                    }
                }
            }

            if (lst_CL188l.ContainsKey(bwid + 1))
            {
                CL188L cl188m = lst_CL188l[bwid + 1];
                cl188m.SelectStatus = p_bol_Positions;
                cl188m.ChannelNo = bwid;
                cl188m.ChannelNum = lst_CL188l.Count;
                int ret = cl188m.ReadCurrentData(bwid + 1, 2, out _, out _, out _, out string wcData, out bool[] _, out byte _, out byte _, out byte _, out bool[] _, out byte _, out _);
                if (ret == 0)
                {
                    lock (obj2)
                    {
                        Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0413", bw = id, data = wcData });
                    }
                }
            }

        }

        private void SetCmd040D(object ob)
        {
            //string[] FrameAry = new string[0];
            int id = int.Parse(((string[])ob)[0]);
            CL188M cl188m = lst_CL188m[id];
            int value = int.Parse(((string[])ob)[1]);

            lst_CL2018[id].bol_StopComm = value == 0;
            int ret = cl188m.Connect(id, out _);
            if (ret == 0)
            {
                lock (obj2)
                {
                    Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=040D" });
                }
            }

        }

        private void SetCmd0409(object ob)
        {
            //string[] FrameAry = new string[0];
            int id = int.Parse(((string[])(ob))[0]);


            bool[] p_bol_Positions = new bool[lst_CL_Rs485.Count];
            p_bol_Positions[id - 1] = true;
            int bwid = GetId(id);
            if (lst_CL188m.ContainsKey(bwid + 1))
            {
                CL188M cl188m = lst_CL188m[bwid + 1];
                cl188m.B_SelectStatus = p_bol_Positions;
                cl188m.Int_ChannelNo = bwid;
                cl188m.Int_ChannelNum = lst_CL188m.Count;
                int ret = cl188m.SetBwVolCutIsolation(0, 0, out _);
                if (ret == 0)
                {
                    lock (obj2)
                    {
                        Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0409" });
                    }
                }
            }

            if (lst_CL188l.ContainsKey(bwid + 1))
            {
                CL188L cl188m = lst_CL188l[bwid + 1];
                cl188m.SelectStatus = p_bol_Positions;
                cl188m.ChannelNo = bwid;
                cl188m.ChannelNum = lst_CL188l.Count;
                int ret = cl188m.SetBwVolCutIsolation(0, 0, out _);
                if (ret == 0)
                {
                    lock (obj2)
                    {
                        Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0409" });
                    }
                }
            }

            if (lst_CL188m.Count == 0 && lst_CL188l.Count == 0)
            {
                Thread.Sleep(100);
                lock (obj2)
                {
                    Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0409" });
                }
            }
        }

        private void SetCmd040A(object ob)
        {
            //string[] FrameAry = new string[0];
            int id = int.Parse(((string[])(ob))[0]);


            bool[] p_bol_Positions = new bool[lst_CL_Rs485.Count];
            p_bol_Positions[id - 1] = true;
            int bwid = GetId(id);
            if (lst_CL188m.ContainsKey(bwid + 1))
            {
                CL188M cl188m = lst_CL188m[bwid + 1];
                cl188m.B_SelectStatus = p_bol_Positions;
                cl188m.Int_ChannelNo = bwid;
                cl188m.Int_ChannelNum = lst_CL188m.Count;
                int ret = cl188m.SetBwVolCutIsolation(0, 1, out _);
                if (ret == 0)
                {
                    lock (obj2)
                    {
                        Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=040A" });
                    }
                }
            }

            if (lst_CL188l.ContainsKey(bwid + 1))
            {
                CL188L cl188m = lst_CL188l[bwid + 1];
                cl188m.SelectStatus = p_bol_Positions;
                cl188m.ChannelNo = bwid;
                cl188m.ChannelNum = lst_CL188l.Count;
                int ret = cl188m.SetBwVolCutIsolation(0, 1, out _);
                if (ret == 0)
                {
                    lock (obj2)
                    {
                        Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=040A" });
                    }
                }
            }

            if (lst_CL188m.Count == 0 && lst_CL188l.Count == 0)
            {
                Thread.Sleep(100);
                lock (obj2)
                {
                    Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=040A" });
                }
            }

        }

        private void SetCmd040B(object ob)
        {
            //string[] FrameAry = new string[0];
            int id = int.Parse(((string[])(ob))[0]);


            bool[] p_bol_Positions = new bool[lst_CL_Rs485.Count];
            p_bol_Positions[id - 1] = true;
            int bwid = GetId(id);
            if (lst_CL188m.ContainsKey(bwid + 1))
            {
                CL188M cl188m = lst_CL188m[bwid + 1];
                cl188m.B_SelectStatus = p_bol_Positions;
                cl188m.Int_ChannelNo = bwid;
                cl188m.Int_ChannelNum = lst_CL188m.Count;
                int ret = cl188m.SetBwVolCutIsolation(0, 2, out _);
                if (ret == 0)
                {
                    lock (obj2)
                    {
                        Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0409" });
                    }
                }
            }

            if (lst_CL188l.ContainsKey(bwid + 1))
            {
                CL188L cl188m = lst_CL188l[bwid + 1];
                cl188m.SelectStatus = p_bol_Positions;
                cl188m.ChannelNo = bwid;
                cl188m.ChannelNum = lst_CL188l.Count;
                int ret = cl188m.SetBwVolCutIsolation(0, 2, out _);
                if (ret == 0)
                {
                    lock (obj2)
                    {
                        Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0409" });
                    }
                }
            }

            if (lst_CL188m.Count == 0 && lst_CL188l.Count == 0)
            {
                Thread.Sleep(100);
                lock (obj2)
                {
                    Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0409" });
                }
            }

        }


        private void SetCmd0408(object ob)
        {
            //string[] FrameAry = new string[0];
            int id = int.Parse(((string[])(ob))[0]);
            CL188M cl188m = lst_CL188m[id];
            int ret = cl188m.StopTest(id, 4, out _);
            if (ret == 0)
            {
                lock (obj2)
                {
                    Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0408" });
                }
            }

        }

        private void SetCmd0407(object ob)
        {
            //string[] FrameAry = new string[0];
            //int id = int.Parse(((string[])(ob))[0]);

            //CL188M cl188m = lst_CL188m[id];
            int ret = 0;
            if (ret == 0)
            {
                lock (obj2)
                {
                    Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0407" });
                }
            }

        }

        private void SetCmd0406(object ob)
        {
            //string[] FrameAry = new string[0];
            //int id = int.Parse(((string[])(ob))[0]);
            //int p_int_RemoteCount = Convert.ToInt32(((string[])(ob))[1]);
            //int time = Convert.ToInt32(((string[])(ob))[2]);
            //int plusecount = Convert.ToInt32(((string[])(ob))[3]);
            //int zkb = Convert.ToInt32(((string[])(ob))[4]);
            //float pl = Convert.ToSingle(plusecount) / time / 60;
            //CL188M cl188m = lst_CL188m[id];
            int ret = 0;
            if (ret == 0)
            {
                lock (obj2)
                {
                    Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0406" });
                }
            }

        }

        private void SetCmd0403(object ob)
        {
            //string[] FrameAry = new string[0];
            //int id = int.Parse(((string[])(ob))[0]);
            //int std = Convert.ToInt32(((string[])(ob))[1], 16);
            //CL188M cl188m = lst_CL188m[id];
            int ret = 0;
            if (ret == 0)
            {
                lock (obj2)
                {
                    Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0403" });
                }
            }

        }

        private void SetCmd0401(object ob)
        {
            try
            {
                string[] FrameAry = new string[0];
                int id = int.Parse(((string[])(ob))[0]);
                int std = int.Parse(((string[])(ob))[1]);

                bool[] p_bol_Positions = new bool[lst_CL_Rs485.Count];
                p_bol_Positions[id - 1] = true;
                int bwid = GetId(id);
                if (lst_CL188m.ContainsKey(bwid + 1))
                {
                    CL188M cl188m = lst_CL188m[bwid + 1];
                    cl188m.B_SelectStatus = p_bol_Positions;
                    cl188m.Int_ChannelNo = bwid;
                    cl188m.Int_ChannelNum = lst_CL188m.Count;
                    //int ret = cl188m.Connect(0, out FrameAry);
                   int rets = cl188m.InitSwitch2();
                    int ret = cl188m.SetBwVolCutIsolation(0, std == 1 ? 0 : 1, out FrameAry);
                    if (ret == 0)
                    {
                        lock (obj2)
                        {
                            Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0401" });
                        }
                    }
                }

                if (lst_CL188l.ContainsKey(bwid + 1))
                {
                    CL188L cl188m = lst_CL188l[bwid + 1];
                    cl188m.SelectStatus = p_bol_Positions;
                    cl188m.ChannelNo = bwid;
                    cl188m.ChannelNum = lst_CL188l.Count;
                    //int ret = cl188m.Connect(0, out FrameAry);
                    int ret = cl188m.SetBwVolCutIsolation(0, std == 1 ? 0 : 1, out FrameAry);
                    if (ret == 0)
                    {
                        lock (obj2)
                        {
                            Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0401" });
                        }
                    }
                    else if (ret == 2)
                    {
                        Thread.Sleep(100);
                        lock (obj2)
                        {
                            Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0401" });
                        }
                    }
                }


                if (lst_CL188m.Count == 0 && lst_CL188l.Count == 0)
                {
                    Thread.Sleep(100);
                    lock (obj2)
                    {
                        Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0401" });
                    }
                }
            }
            catch
            {

            }
        }

        public int GetId(int id)
        {
            int channel;
            if (lst_CL188m.Count > 0)
                channel = lst_CL_Rs485.Count / lst_CL188m.Count;
            else if (lst_CL188l.Count > 0)
                channel = lst_CL_Rs485.Count / lst_CL188l.Count;
            else
                channel = 1;

            int id188 = (id - 1) / channel;

            return id188;
        }

        private void SetCmd0301(object ob)
        {
            //string[] FrameAry = new string[0];
            //float[] instValue = new float[0];
            foreach (CL3115 cl3115 in lst_CL3115.Values)
            {
                int ret = cl3115.ReadInstMetricAll(out float[] instValue, out _);
                if (ret == 0)
                {
                    lock (obj2)
                    {
                        Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0301", data = instValue[0] + ";" + instValue[6] + ";" + instValue[1] + ";" + instValue[7] + ";" + instValue[2] + ";" + instValue[8] + ";" + instValue[3] + ";" + instValue[9] + ";" + instValue[4] + ";" + instValue[10] + ";" + instValue[5] + ";" + instValue[11] + ";" + instValue[19] + ";" + instValue[23] + ";" + instValue[31] + ";" + instValue[33] });
                    }
                }
            }
            foreach (CL3112 cl3112 in lst_CL3112.Values)
            {
                int ret = cl3112.ReadInstMetricAll(out float[] instValue, out _);
                if (ret == 0)
                {
                    lock (obj2)
                    {
                        Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0301", data = instValue[0] + ";" + instValue[6] + ";" + instValue[1] + ";" + instValue[7] + ";" + instValue[2] + ";" + instValue[8] + ";" + instValue[3] + ";" + instValue[9] + ";" + instValue[4] + ";" + instValue[10] + ";" + instValue[5] + ";" + instValue[11] + ";" + instValue[19] + ";" + instValue[23] + ";" + instValue[31] + ";" + instValue[33] });
                    }
                }
            }
            foreach (CL311V2 cl311V2 in lst_CL311V2.Values)
            {
                int ret = cl311V2.ReadInstMetricAll(out float[] instValue, out _);
                if (ret == 0)
                {
                    lock (obj2)
                    {
                        Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0301", data = instValue[0] + ";" + instValue[6] + ";" + instValue[1] + ";" + instValue[7] + ";" + instValue[2] + ";" + instValue[8] + ";" + instValue[3] + ";" + instValue[9] + ";" + instValue[4] + ";" + instValue[10] + ";" + instValue[5] + ";" + instValue[11] + ";" + instValue[19] + ";" + instValue[23] + ";" + instValue[31] + ";" + instValue[33] });
                    }
                }
            }
        }

        private void SetCmd0201(object ob)
        {
            //string[] FrameAry = new string[0];
            foreach (CL303 cl303 in lst_CL303.Values)
            {
                int clfs = 0;
                if (int.Parse(((string[])(ob))[0]) == 0)
                    clfs = 0;
                else if (int.Parse(((string[])(ob))[0]) == 2)
                    clfs = 1;
                else if (int.Parse(((string[])(ob))[0]) == 7)
                    clfs = 5;
                double glys = Math.Cos(double.Parse(((string[])(ob))[4]) / 180 * Math.PI);
                string strglys = glys > 0 ? glys + "L" : glys + "C";
                float u = float.Parse(((string[])(ob))[2]);
                float i = float.Parse(((string[])(ob))[3]);
                float Hz = float.Parse(((string[])(ob))[5]);
                int ret = cl303.PowerOn(clfs, byte.Parse(((string[])(ob))[1]), 1, strglys, u, u, u, i, i, i, 1, Hz, false, out _);
                Thread.Sleep(6000);
                if (ret == 0)
                {
                    lock (obj2)
                    {
                        Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0201" });
                    }
                }
            }
            foreach (CL309 cl309 in lst_CL309.Values)
            {
                //int clfs = 0;
                //if (int.Parse(((string[])(ob))[0]) == 0)
                //    clfs = 0;
                //else if (int.Parse(((string[])(ob))[0]) == 2)
                //    clfs = 1;
                //else if (int.Parse(((string[])(ob))[0]) == 7)
                //    clfs = 5;
                //string strglys = Convert.ToString(Math.Cos(int.Parse(((string[])(ob))[0])));
                //float u = float.Parse(((string[])(ob))[2]);
                //float i = float.Parse(((string[])(ob))[3]);
                //float Hz = float.Parse(((string[])(ob))[5]);
                //int ret = cl309.PowerOn(clfs, byte.Parse(((string[])(ob))[1]), 1, strglys, u, u, u, i, i, i, 1, Hz, false, out FrameAry);



                int clfs = 0;
                if (int.Parse(((string[])(ob))[0]) == 0)
                    clfs = 0;
                else if (int.Parse(((string[])(ob))[0]) == 1)
                    clfs = 1;
                else if (int.Parse(((string[])(ob))[0]) == 2)
                    clfs = 2;
                else if (int.Parse(((string[])(ob))[0]) == 3)
                    clfs = 3;
                else
                    clfs = 5;
                float Hz = float.Parse(((string[])(ob))[5]);
                float ua = float.Parse(((string[])(ob))[2]);
                float ub = float.Parse(((string[])(ob))[2]);
                float uc = float.Parse(((string[])(ob))[2]);
                float ia = float.Parse(((string[])(ob))[3]);
                float ib = float.Parse(((string[])(ob))[3]);
                float ic = float.Parse(((string[])(ob))[3]);
                float ua_phi = 0;
                float ub_phi = 120;
                float uc_phi = 240;
                float ia_phi = 0 + float.Parse(((string[])(ob))[4]);
                float ib_phi = 120 + float.Parse(((string[])(ob))[4]);
                float ic_phi = 240 + float.Parse(((string[])(ob))[4]);
                if (clfs == 0 || clfs == 2)
                {
                    ia_phi = 0 + float.Parse(((string[])(ob))[4]);
                    ib_phi = 120 + float.Parse(((string[])(ob))[4]);
                    ic_phi = 240 + float.Parse(((string[])(ob))[4]);
                }
                else if (clfs == 1 || clfs == 3)
                {
                    ia_phi = 0 + 90 - float.Parse(((string[])(ob))[4]);
                    ib_phi = 120 + 90 - float.Parse(((string[])(ob))[4]);
                    ic_phi = 240 + 90 - float.Parse(((string[])(ob))[4]);
                }
                else
                {
                    ia_phi = 0 + float.Parse(((string[])(ob))[4]);
                    ib_phi = 120 + float.Parse(((string[])(ob))[4]);
                    ic_phi = 240 + float.Parse(((string[])(ob))[4]);
                }

                if (clfs == 2 || clfs == 3)
                {
                    ub = 0; ib = 0;
                    ua_phi = (ua_phi - 30) % 360;
                    uc_phi = (uc_phi - 330) % 360;
                }
                else if (clfs == 5)
                {
                    ub = 0; ib = 0;
                    uc = 0; ic = 0;
                }
                if (Program.Element == "A元")
                {
                    ub = 0; ib = 0;
                    uc = 0; ic = 0;
                }
                else if (Program.Element == "B元")
                {
                    ub = ua; ib = ia;
                    ua = 0; ia = 0;
                    uc = 0; ic = 0;
                    ub_phi = ua_phi;
                    ib_phi = uc_phi;
                }
                else if (Program.Element == "C元")
                {
                    uc = ua; ic = ia;
                    ub = 0; ib = 0;
                    ua = 0; ia = 0;
                    uc_phi = ua_phi;
                    ic_phi = uc_phi;
                }

                int ret = cl309.PowerOnFree(byte.Parse(((string[])(ob))[1]), ua, ub, uc, ia, ib, ic, ua_phi, ub_phi, uc_phi, ia_phi, ib_phi, ic_phi, Hz, false, out _);
                Thread.Sleep(12000);
                if (ret == 0)
                {
                    lock (obj2)
                    {
                        Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0201" });
                    }
                }
            }
        }

        private void SetCmd0202(object ob)
        {
            //string[] FrameAry = new string[0];
            foreach (CL303 cl303 in lst_CL303.Values)
            {
                //int clfs = 0;
                //if (int.Parse(((string[])(ob))[0]) == 0)
                //    clfs = 0;
                //else if (int.Parse(((string[])(ob))[0]) == 2)
                //    clfs = 1;
                //else if (int.Parse(((string[])(ob))[0]) == 7)
                //    clfs = 5;
                float Hz = float.Parse(((string[])(ob))[2]);
                float ua = float.Parse(((string[])(ob))[3]);
                float ub = float.Parse(((string[])(ob))[5]);
                float uc = float.Parse(((string[])(ob))[7]);
                float ia = float.Parse(((string[])(ob))[9]);
                float ib = float.Parse(((string[])(ob))[11]);
                float ic = float.Parse(((string[])(ob))[13]);
                float ua_phi = float.Parse(((string[])(ob))[4]);
                float ub_phi = float.Parse(((string[])(ob))[6]);
                float uc_phi = float.Parse(((string[])(ob))[8]);
                float ia_phi = float.Parse(((string[])(ob))[10]);
                float ib_phi = float.Parse(((string[])(ob))[12]);
                float ic_phi = float.Parse(((string[])(ob))[14]);

                int ret = cl303.PowerOnFree(byte.Parse(((string[])(ob))[1]), ua, ub, uc, ia, ib, ic, ua_phi, ub_phi, uc_phi, ia_phi, ib_phi, ic_phi, Hz, false, out _);
                Thread.Sleep(6000);
                if (ret == 0)
                {
                    lock (obj2)
                    {
                        Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0202" });
                    }
                }
            }
            foreach (CL309 cl309 in lst_CL309.Values)
            {
                int clfs = 0;
                if (int.Parse(((string[])(ob))[0]) == 0)
                    clfs = 0;
                else if (int.Parse(((string[])(ob))[0]) == 2)
                    clfs = 1;
                else if (int.Parse(((string[])(ob))[0]) == 7)
                    clfs = 5;
                float Hz = float.Parse(((string[])(ob))[2]);
                float ua = float.Parse(((string[])(ob))[3]);
                float ub = float.Parse(((string[])(ob))[5]);
                float uc = float.Parse(((string[])(ob))[7]);
                float ia = float.Parse(((string[])(ob))[9]);
                float ib = float.Parse(((string[])(ob))[11]);
                float ic = float.Parse(((string[])(ob))[13]);
                float ua_phi = float.Parse(((string[])(ob))[4]);
                float ub_phi = float.Parse(((string[])(ob))[6]);
                float uc_phi = float.Parse(((string[])(ob))[8]);
                float ia_phi = float.Parse(((string[])(ob))[10]);
                float ib_phi = float.Parse(((string[])(ob))[12]);
                float ic_phi = float.Parse(((string[])(ob))[14]);

                if (clfs == 1)
                {
                    ub = 0; ib = 0;
                }
                else if (clfs == 5)
                {
                    ub = 0; ib = 0;
                    uc = 0; ic = 0;
                }
                if (Program.Element == "A元")
                {
                    ub = 0; ib = 0;
                    uc = 0; ic = 0;
                }
                else if (Program.Element == "B元")
                {
                    ub = ua; ib = ia;
                    ua = 0; ia = 0;
                    uc = 0; ic = 0;
                    ub_phi = ua_phi;
                    ib_phi = uc_phi;
                }
                else if (Program.Element == "C元")
                {
                    uc = ua; ic = ia;
                    ub = 0; ib = 0;
                    ua = 0; ia = 0;
                    uc_phi = ua_phi;
                    ic_phi = uc_phi;
                }
                int ret = cl309.PowerOnFree(byte.Parse(((string[])(ob))[1]), ua, ub, uc, ia, ib, ic, ua_phi, ub_phi, uc_phi, ia_phi, ib_phi, ic_phi, Hz, false, out _);
                Thread.Sleep(12000);
                if (ret == 0)
                {
                    lock (obj2)
                    {
                        Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0202" });
                    }
                }
            }
        }

        private void SetCmd0203(object ob)
        {
            //string[] FrameAry = new string[0];
            foreach (CL303 cl303 in lst_CL303.Values)
            {
                int ret = cl303.PowerOff(out _);
                if (ret == 0)
                    Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0203" });
            }
            foreach (CL309 cl309 in lst_CL309.Values)
            {
                int ret = cl309.PowerOff(out _);
                if (ret == 0)
                    Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0203" });
            }
        }

        private void SetCmd0104(object ob)
        {
            //string[] FrameAry = new string[0];
            if (int.Parse(((string[])(ob))[2]) == 0) return;
            int id = int.Parse(((string[])(ob))[2]);
            //if (int.Parse(((string[])(ob))[0]) == 0)//上行
            {
                CL_Rs485 rs485 = lst_CL_Rs485[id];
                rs485.m_Rs485Port[0].m_Port_Setting = ((string[])(ob))[3].ToLower().Replace("-", ",");
            }
            Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0104" });

        }

        private void SetCmd0204(object ob)
        {
            //string[] FrameAry = new string[0];
            foreach (CL303 cl303 in lst_CL303.Values)
            {
                int[] int_XTSwitch = new int[32];//各相各次开关
                float[] sng_Value = new float[32];//幅值
                float[] sng_Phase = new float[32];//相位
                //int clfs = 0;
                sng_Value[0] = 1;

                string[] stmp = (((string[])(ob))[1]).Split('-');

                int_XTSwitch[Convert.ToInt16(stmp[0])] = 1;
                sng_Value[0] = 100;
                sng_Value[Convert.ToInt16(stmp[0])] = Convert.ToSingle(stmp[1]) * 100;
                sng_Phase[Convert.ToInt16(stmp[0])] = Convert.ToSingle(stmp[2]);
                int ret = cl303.SetHarmonic(int.Parse(((string[])(ob))[0]) + 1, int_XTSwitch, sng_Value, sng_Phase, out _);
                if (ret == 0)
                    Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0204" });
            }
            foreach (CL309 cl309 in lst_CL309.Values)
            {
                int[] int_XTSwitch = new int[32];//各相各次开关
                float[] sng_Value = new float[32];//幅值
                float[] sng_Phase = new float[32];//相位
                //int clfs = 0;
                sng_Value[0] = 1;

                string[] stmp = (((string[])(ob))[1]).Split('-');

                int_XTSwitch[Convert.ToInt16(stmp[0])] = 1;
                sng_Value[Convert.ToInt16(stmp[0])] = Convert.ToSingle(stmp[1]);
                sng_Phase[Convert.ToInt16(stmp[0])] = Convert.ToSingle(stmp[2]);
                int ret = cl309.SetHarmonic(int.Parse(((string[])(ob))[0]), int_XTSwitch, sng_Value, sng_Phase, out _);
                if (ret == 0)
                    Program.queueReceiveData.Enqueue(new EventsReceiveData() { lastcmd = "cmd=0204" });
            }
        }

        /// <summary>接收用</summary>
        private UdpClient receiveUdpClient;

        /// <summary>
        /// 接收来自主程序的数据
        /// </summary>
        private void AcceptC()
        {
            while (true)
            {
                try
                {
                    IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                    byte[] bytes = receiveUdpClient.Receive(ref remote);
                    RemoteTlocat = remote;

                    string str = System.Text.Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                    if (str == "") return;

                    ChangeControlInvoke(this, () =>
                    {
                        if (TbackStr[0].Lines.Length > 200)
                            TbackStr[0].Clear();

                        int lenth = TbackStr[0].Text.Length;

                        string msg = $"[{DateTime.Now}]From PC:{str}";
                        TbackStr[0].AppendText(msg);
                        TbackStr[0].Select(lenth, msg.Length);
                        TbackStr[0].SelectionColor = Color.Red;
                        TbackStr[0].AppendText("\r\n");
                        TbackStr[0].ScrollToCaret();
                        if (chk_IsLog.Checked)
                            CLBase.WriteLog("测试日志", msg);
                        try
                        {
                            lock (obj)
                            {
                                queueSendData.Enqueue(new EventsSendData() { cmd = str.Substring(0, 8), data = str.Substring(14) });
                            }
                        }
                        catch
                        {
                        }
                    });
                }
                catch
                {
                }

            }
        }

        private void TxTmp_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            (sender as RichTextBox).Text = "";
        }

        private delegate void InvokeHandler();//
        /// <summary>
        /// 变更显示，可能在其他线程里面控制界面显示
        /// </summary>
        /// <param name="control"></param>
        /// <param name="handler"></param>
        private void ChangeControlInvoke(Control control, InvokeHandler handler)//
        {
            if (control.InvokeRequired)
            {
                control.Invoke(handler);
            }
            else
            {
                handler();
            }
        }

        private void Showtxt(object sender, string Tstr, int Index)
        {
            ChangeControlInvoke(this, () =>
            {
                if (TbackStr == null) return;
                if (TbackStr[Index - 1].Text.Length > 5000)
                {
                    TbackStr[Index - 1].Text = "";
                }
                int ilen = TbackStr[Index - 1].SelectionStart;
                TbackStr[Index - 1].AppendText(Tstr + "\r\n");

                if (blScroll)
                    TbackStr[Index - 1].ScrollToCaret();
                else
                    TbackStr[Index - 1].SelectionStart = ilen;
            });
        }

        /// <summary>
        /// 获取表位配置信息
        /// </summary>
        public void GetDeviceini()
        {
            TableCount = (int.Parse)(CLBase.g_GetINI(Application.StartupPath + "\\System\\VirtualMeter.ini", "System", "TableCount", "16"));
            textBox1.Text = (CLBase.g_GetINI(Application.StartupPath + "\\System\\VirtualMeter.ini", "System", "BwCount", "16"));
            for (int i = 1; i <= TableCount; i++)
            {

                string[] strTmp = CLBase.g_GetINI(Application.StartupPath + "\\System\\VirtualMeter.ini", "System", "Table" + i, "16").Split('|');

                dgv_Config.Rows.Add();
                dgv_Config.Rows[i - 1].Cells[0].Value = strTmp[0];
                dgv_Config.Rows[i - 1].Cells[1].Value = strTmp[1];
                dgv_Config.Rows[i - 1].Cells[2].Value = strTmp[2];
                dgv_Config.Rows[i - 1].Cells[3].Value = strTmp[3];
                dgv_Config.Rows[i - 1].Cells[4].Value = strTmp[4];
                dgv_Config.Rows[i - 1].Cells[5].Value = strTmp[5];
                dgv_Config.Rows[i - 1].Cells[6].Value = strTmp[6];
                dgv_Config.Rows[i - 1].Cells[7].Value = strTmp[7];

                if (strTmp[0] == "cl303")
                {
                    CL303 cl303 = new CL303();
                    if (strTmp[5] == "串口")
                    {
                        cl303.InitSettingCom(int.Parse(strTmp[4]), int.Parse(strTmp[6]), int.Parse(strTmp[7]));
                    }
                    else
                    {
                        cl303.InitSetting(int.Parse(strTmp[4]), int.Parse(strTmp[6]), int.Parse(strTmp[7]), strTmp[1], int.Parse(strTmp[3]), int.Parse(strTmp[2]), strTmp[5]);
                    }
                    lst_CL303.Add(lst_CL303.Count + 1, cl303);
                }
                else if (strTmp[0] == "cl309")
                {
                    CL309 cl309 = new CL309();
                    if (strTmp[5] == "串口")
                    {
                        cl309.InitSettingCom(int.Parse(strTmp[4]), int.Parse(strTmp[6]), int.Parse(strTmp[7]));
                    }
                    else
                    {
                        cl309.InitSetting(int.Parse(strTmp[4]), int.Parse(strTmp[6]), int.Parse(strTmp[7]), strTmp[1], int.Parse(strTmp[3]), int.Parse(strTmp[2]), strTmp[5]);
                    }
                    lst_CL309.Add(lst_CL309.Count + 1, cl309);
                }
                else if (strTmp[0] == "cl3115")
                {
                    CL3115 cl3115 = new CL3115();
                    if (strTmp[5] == "串口")
                    {
                        cl3115.InitSettingCom(int.Parse(strTmp[4]), int.Parse(strTmp[6]), int.Parse(strTmp[7]));
                    }
                    else
                    {
                        cl3115.InitSetting(int.Parse(strTmp[4]), int.Parse(strTmp[6]), int.Parse(strTmp[7]), strTmp[1], int.Parse(strTmp[3]), int.Parse(strTmp[2]), strTmp[5]);
                    }
                    lst_CL3115.Add(lst_CL3115.Count + 1, cl3115);
                }
                else if (strTmp[0] == "cl3112")
                {
                    CL3112 cl3112 = new CL3112();
                    if (strTmp[5] == "串口")
                        cl3112.InitSettingCom(int.Parse(strTmp[4]), int.Parse(strTmp[6]), int.Parse(strTmp[7]));
                    else
                        cl3112.InitSetting(int.Parse(strTmp[4]), int.Parse(strTmp[6]), int.Parse(strTmp[7]), strTmp[1], int.Parse(strTmp[3]), int.Parse(strTmp[2]), strTmp[5]);

                    lst_CL3112.Add(lst_CL3112.Count + 1, cl3112);
                }
                else if (strTmp[0] == "cl311v2")
                {
                    CL311V2 cl311V2 = new CL311V2();
                    if (strTmp[5] == "串口")
                    {
                        cl311V2.InitSettingCom(int.Parse(strTmp[4]), int.Parse(strTmp[6]), int.Parse(strTmp[7]));
                    }
                    else
                    {
                        cl311V2.InitSetting(int.Parse(strTmp[4]), int.Parse(strTmp[6]), int.Parse(strTmp[7]), strTmp[1], int.Parse(strTmp[3]), int.Parse(strTmp[2]), strTmp[5]);
                    }
                    lst_CL311V2.Add(lst_CL311V2.Count + 1, cl311V2);
                }
                else if (strTmp[0] == "cl2029D")
                {
                    CL2029D cl2029D = new CL2029D();
                    if (strTmp[5] == "串口")
                        cl2029D.InitSettingCom(int.Parse(strTmp[4]), int.Parse(strTmp[6]), int.Parse(strTmp[7]));
                    else
                        cl2029D.InitSetting(int.Parse(strTmp[4]), int.Parse(strTmp[6]), int.Parse(strTmp[7]), strTmp[1], int.Parse(strTmp[3]), int.Parse(strTmp[2]), strTmp[5]);
                    lst_CL2029D.Add(lst_CL2029D.Count + 1, cl2029D);
                }
                else if (strTmp[0] == "cl188m")
                {
                    CL188M cl188m = new CL188M();
                    if (strTmp[5] == "串口")
                        cl188m.InitSettingCom(int.Parse(strTmp[4]), int.Parse(strTmp[6]), int.Parse(strTmp[7]));
                    else
                        cl188m.InitSetting(int.Parse(strTmp[4]), int.Parse(strTmp[6]), int.Parse(strTmp[7]), strTmp[1], int.Parse(strTmp[3]), int.Parse(strTmp[2]), strTmp[5]);
                    lst_CL188m.Add(lst_CL188m.Count + 1, cl188m);
                }
                else if (strTmp[0] == "cl188l")
                {
                    CL188L cl188l = new CL188L();
                    if (strTmp[5] == "串口")
                        cl188l.InitSettingCom(int.Parse(strTmp[4]), int.Parse(strTmp[6]), int.Parse(strTmp[7]));
                    else
                        cl188l.InitSetting(int.Parse(strTmp[4]), int.Parse(strTmp[6]), int.Parse(strTmp[7]), strTmp[1], int.Parse(strTmp[3]), int.Parse(strTmp[2]), strTmp[5]);
                    lst_CL188l.Add(lst_CL188l.Count + 1, cl188l);
                }
                else if (strTmp[0] == "cl191B")
                {
                    CL191B cl191b = new CL191B();
                    if (strTmp[5] == "串口")
                        cl191b.InitSettingCom(int.Parse(strTmp[4]), int.Parse(strTmp[6]), int.Parse(strTmp[7]));
                    else
                        cl191b.InitSetting(int.Parse(strTmp[4]), int.Parse(strTmp[6]), int.Parse(strTmp[7]), strTmp[1], int.Parse(strTmp[3]), int.Parse(strTmp[2]), strTmp[5]);
                    lst_CL191B.Add(lst_CL191B.Count + 1, cl191b);
                }
                else if (strTmp[0] == "上行485")
                {
                    CL_Rs485 clrs485 = new CL_Rs485();
                    int_MaxWaitTime = int.Parse(strTmp[6]);
                    if (strTmp[5] == "串口")
                        clrs485.InitSettingCom(int.Parse(strTmp[4]), Program.UpSideBaudRate, int.Parse(strTmp[6]), int.Parse(strTmp[7]));
                    else
                        clrs485.InitSetting(int.Parse(strTmp[4]), Program.UpSideBaudRate, int.Parse(strTmp[6]), int.Parse(strTmp[7]), strTmp[1], int.Parse(strTmp[3]), int.Parse(strTmp[2]), strTmp[5]);
                    lst_CL_Rs485.Add(lst_CL_Rs485.Count + 1, clrs485);
                }
                else if (strTmp[0] == "下行485")
                {
                    CL2018 cl_2018 = new CL2018(lst_CL2018.Count + 1, int.Parse(strTmp[4]), strTmp[1], int.Parse(strTmp[3]), int.Parse(strTmp[2]), strTmp[5] == "2018-负控" ? 0 : 1, Program.UpSideBaudRate);
                    //this.cl2018[i - 1] = new CL2018(i, (int.Parse)(strTmp[0]), strTmp[1], (int.Parse)(strTmp[2]), (int.Parse)(strTmp[3]), dic_Meter, _2018Ver);
                    //this.cl2018[i - 1].ClEventMoni += new CL2018.EMoniBack(this.showtxt);
                    cl_2018.ClEventMoni += new CL2018.EMoniBack(this.Showtxt);
                    lst_CL2018.Add(lst_CL2018.Count + 1, cl_2018);
                }
            }
        }


        /// <summary>
        /// 主窗体关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process[] myProcess = Process.GetProcessesByName("CLOU_TerminalUI");
            if (myProcess.Length > 0)
            {
                MessageBox.Show("主控程序还没有关闭，模拟表程序不要关闭！", "提示", MessageBoxButtons.OK);
                e.Cancel = true;
                notifyIcon1.Visible = true;
                return;
            }
            else
                System.Diagnostics.Process.GetCurrentProcess().Kill();

        }

        private void Btn_SetTableCount_Click(object sender, EventArgs e)
        {
            dgv_Config.Rows.Clear();
            for (int i = 1; i <= Convert.ToInt16(textBox1.Text); i++)
            {
                dgv_Config.Rows.Add();
                dgv_Config.Rows[i - 1].Cells[0].Value = i.ToString();
                dgv_Config.Rows[i - 1].Cells[1].Value = i.ToString();
                dgv_Config.Rows[i - 1].Cells[2].Value = "193.168.18.1";
                dgv_Config.Rows[i - 1].Cells[3].Value = "10004";
                dgv_Config.Rows[i - 1].Cells[4].Value = "20000";
            }
        }

        bool boolfalg = true;
        private void Dgv_Config_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (boolfalg)
            {
                boolfalg = false;
                for (int i = 0; i < dgv_Config.Rows.Count; i++)
                {
                    if (i > e.RowIndex)
                    {
                        if (e.ColumnIndex == 4)//端口号,累加，其余参数保持一致
                            dgv_Config.Rows[i].Cells[e.ColumnIndex].Value = (Convert.ToInt16((dgv_Config.Rows[i - 1].Cells[e.ColumnIndex].Value)) + 1).ToString();
                        else
                            dgv_Config.Rows[i].Cells[e.ColumnIndex].Value = dgv_Config.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    }
                }
            }
            boolfalg = true;
        }

        private void Btn_SaveData_Click(object sender, EventArgs e)
        {
            CLBase.g_WriteINI(Application.StartupPath + "\\System\\VirtualMeter.ini", "System", "BwCount", textBox1.Text);
            CLBase.g_WriteINI(Application.StartupPath + "\\System\\VirtualMeter.ini", "System", "TableCount", dgv_Config.Rows.Count.ToString());
            for (int i = 1; i <= dgv_Config.Rows.Count; i++)
            {
                CLBase.g_WriteINI(Application.StartupPath + "\\System\\VirtualMeter.ini", "System", "Table" + i, dgv_Config.Rows[i - 1].Cells[0].Value.ToString() + "|" + dgv_Config.Rows[i - 1].Cells[1].Value.ToString() + "|" + dgv_Config.Rows[i - 1].Cells[2].Value.ToString() + "|" + dgv_Config.Rows[i - 1].Cells[3].Value.ToString() + "|" + dgv_Config.Rows[i - 1].Cells[4].Value.ToString() + "|" + dgv_Config.Rows[i - 1].Cells[5].Value.ToString() + "|" + dgv_Config.Rows[i - 1].Cells[6].Value.ToString() + "|" + dgv_Config.Rows[i - 1].Cells[7].Value.ToString());
            }
            lab_Prompting.Text = "参数在下一次软件重启生效！";
        }


        private void ToolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            //隐藏托盘程序中的图标
            notifyIcon1.Visible = false;
            //关闭系统
            this.Close();

        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
                notifyIcon1.Visible = true;
            }
            else
            {
                //notifyIcon1.Visible = false;
            }
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (dgv_Config.SelectedRows.Count < 1) return;
            DataGridViewRow dv = dgv_Config.SelectedRows[0];
            dgv_Config.Rows.Remove(dv);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            dgv_Config.Rows.Add();
            int iCount = dgv_Config.Rows.Count;
            dgv_Config.Rows[iCount - 1].Cells[0].Value = "cl303";
            dgv_Config.Rows[iCount - 1].Cells[1].Value = "193.168.18.1";
            dgv_Config.Rows[iCount - 1].Cells[2].Value = "20000";
            dgv_Config.Rows[iCount - 1].Cells[3].Value = "10003";
            dgv_Config.Rows[iCount - 1].Cells[4].Value = "1";
        }

    }
}
