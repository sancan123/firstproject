using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace cl_Frontend_Meter
{
    public class CL2018 : CLBase
    {

        public delegate void EMoniBack(object sender, string Tdata, int Index);
        public event EMoniBack ClEventMoni;


        public Thread TThread;
        public Socket TupdOne = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public Socket TupdOneInitPort = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private IPEndPoint Tlocat;
        private IPEndPoint Tremoto;
        private string g_Chen;
        public int CommPort;
        public bool IsLog = false;
        public bool IsAnalysis = false;

        public string L_2018ReturnCmd;
        public bool bol_StopComm = false;//停止通讯
        public string G_BTL;
        public string ZbAddress = "000000000010";

        public bool RunFalg = false;//运行模式，false为采集器模式，ture为集中器模式


        //int BiaoType = 1;

        int index = 0;

        System.Windows.Forms.Timer tm = new System.Windows.Forms.Timer();//定时器，定时对2018进行初始化
        DateTime timLastReceive = new DateTime();//最近一次接受数据数据时间

        /// <summary>
        /// 2018版本号
        /// </summary>
        int _2018Ver = 0;
        public void AriseEventMn(string Data)
        {
            if (ClEventMoni != null)
            {
                ClEventMoni(this, Data, index);
            }
        }


        public CL2018(int MyId, int comm, string TRemoIp, int RomComm, int StartComm,int ver,string btl)
        {
            tm.Enabled = true;
            tm.Interval = 1000;
            tm.Tick += tm_Tick;

            G_BTL = "2400-e-8-1";
            //G_BTL = "9600-e-8-1";

            //dic_Meter = dic_Tmp;
            index = MyId;
            CommPort = comm;
            _2018Ver = ver;
            G_BTL = btl;

            string Tip; int Ti;
            Tip = Dns.GetHostName();
            IPHostEntry Tentryip = Dns.GetHostEntry(Tip);
            for (Ti = 0; Ti < Tentryip.AddressList.Length; Ti++)
            {
                if (Tentryip.AddressList[Ti].ToString().Length > 8)
                    if (Tentryip.AddressList[Ti].ToString().Substring(0, 8) == TRemoIp.Substring(0, 8))
                        break;
            }
            if (Ti == Tentryip.AddressList.Length)
                Tip = "127.0.0.1";
            else
                Tip = Tentryip.AddressList[Ti].ToString();
            if (Tip.Length < 7)
                Tip = "127.0.0.1";
            if (_2018Ver == 0)
                Tlocat = new IPEndPoint(IPAddress.Parse(Tip), StartComm + (comm - 1) * 2);
            else
                Tlocat = new IPEndPoint(IPAddress.Parse(Tip), StartComm + (comm - 1) * 2);
            Tremoto = new IPEndPoint(IPAddress.Parse(TRemoIp), RomComm);
            IPEndPoint Tlocat2 = new IPEndPoint(IPAddress.Parse(Tip), StartComm + (comm - 1) * 2 + 1);
            try
            {
                TupdOne.Bind(Tlocat); ;
                if (_2018Ver != 0)
                    TupdOneInitPort.Bind(Tlocat2);
            }
            catch
            {
                AriseEventMn("联机不上！");
            }
            Open();

            TThread = new Thread(AcceptC);
            TThread.Start();


            L_2018ReturnCmd = "";
            return;
        }

        void tm_Tick(object sender, EventArgs e)
        {
            if ((DateTime.Now.Ticks / 10000000 - timLastReceive.Ticks / 10000000) > 120)
            {
                timLastReceive = DateTime.Now;
                //Open();
            }
        }


        string strCache = "";

        /// <summary>
        /// 存储完整帧的帧队列
        /// </summary>
        public Queue<string> ListCache = new Queue<string>();

        public string GetOneFrameAndFillListCache(string frmStr)
        {
            #region 645解析报文
            //frmStr = frmStr.ToUpper();
            //if (frmStr.IndexOf("68") < 0)
            //    return "";
            //else
            //{
            //    frmStr = frmStr.Substring(frmStr.IndexOf("68"));
            //    if (frmStr.Length >= 20)
            //    {
            //        if (frmStr.Substring(14, 2) == "68")
            //        {
            //            int ilen = Convert.ToInt16(frmStr.Substring(18, 2), 16);
            //            if (frmStr.Length >= 24 + ilen * 2)
            //            {
            //                if (frmStr.Substring(22 + ilen * 2, 2) == "16")
            //                {
            //                    ListCache.Enqueue(frmStr.Substring(0, ilen * 2 + 24));
            //                    frmStr = frmStr.Remove(0, frmStr.Substring(0, ilen * 2 + 24).Length);
            //                    frmStr = GetOneFrameAndFillListCache(frmStr);
            //                }
            //                else
            //                {
            //                    frmStr = frmStr.Remove(0, 2);
            //                    frmStr = GetOneFrameAndFillListCache(frmStr);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            frmStr = frmStr.Remove(0, 2);
            //            frmStr = GetOneFrameAndFillListCache(frmStr);
            //        }
            //    }
            //}

            //return frmStr;
            #endregion
            #region 698.45

            frmStr = frmStr.ToUpper();
            if (frmStr.IndexOf("68") < 0)
                return "";
            else
            {
                frmStr = frmStr.Substring(frmStr.IndexOf("68"));
                if (frmStr.Length >= 20)
                {
                    int ilen = Convert.ToInt16(frmStr.Substring(2, 2), 16) + Convert.ToInt16(frmStr.Substring(4, 2), 16) * 256;
                    if (frmStr.Length >= 4 + ilen * 2)
                    {
                        if (frmStr.Substring(2 + ilen * 2, 2) == "16")
                        {
                            ListCache.Enqueue(frmStr.Substring(0, ilen * 2 + 4));
                            frmStr = frmStr.Remove(0, frmStr.Substring(0, ilen * 2 + 4).Length);
                            frmStr = GetOneFrameAndFillListCache(frmStr);
                        }
                        else
                        {
                            frmStr = frmStr.Remove(0, 2);
                            frmStr = GetOneFrameAndFillListCache(frmStr);
                        }
                    }
                }
            }

            return frmStr;
            #endregion
        }
        private void AcceptC()
        {
            byte[] Tbyte;
            int Tlen = 0;
            while (true)
            {


                Tbyte = new byte[1024];

                try
                {
                    Tlen = TupdOne.Receive(Tbyte);
                }
                catch
                {
                    //AriseEventMn("通信错误！");
                    //if (IsLog)
                    //    WriteText("AcceptC:通信错误！");
                    //break;
                }

                string input = "";
                string Tstr = "";
                Tstr = "";

                if (_2018Ver == 0)
                {
                    int Ti;
                    Tstr = System.Text.Encoding.ASCII.GetString(Tbyte);

                    Ti = Tstr.IndexOf(",>");
                    if (Ti != 0)
                    {

                        Tstr = Tstr.Substring(0, Ti + 1);
                        try
                        {
                            switch (g_GetItem(Tstr, 3, ","))
                            {
                                case "resetinit":
                                    AriseEventMn("初始化成功！");
                                    break;
                                case "open":
                                    AriseEventMn("打开端口成功！");
                                    break;
                                case "close":
                                    AriseEventMn("关闭端口成功！");
                                    break;
                                case "hello":
                                    AriseEventMn("连接成功！");
                                    break;
                                case "ask":
                                    if (g_GetItem(Tstr, 2, ",") == "cl2018")
                                    {
                                        Tstr = g_GetItem(Tstr, 7, ",");
                                        Tstr = Tstr.Substring(4, Tstr.Length - 4);
                                    }
                                    break;
                            }

                        }
                        catch { break; }
                    }
                }
                else
                {

                    for (int i = 0; i < Tlen; i++)
                    {
                        Tstr += Convert.ToString(Tbyte[i], 16).PadLeft(2, '0');
                    }
                }

                input = strCache + Tstr;

                if (input.Length > 0)
                {
                    strCache = GetOneFrameAndFillListCache(input);
                }

                if (ListCache.Count > 0)
                {
                    for (int i = ListCache.Count - 1; i >= 0; i--)
                    {
                        g_Chen = ListCache.Dequeue();
                        if (!bol_StopComm)
                        {
                            Program.queueReceiveData.Enqueue(new EventsReceiveData() { bw = index, data = g_Chen, lastcmd = "cmd=1002" });
                            AriseEventMn("收：" + g_Chen);
                        }
                        //SCoreSock();
                    }

                }

            }

        }

        public bool Open()
        {
            string Tcmd;
            byte[] Tcl;
            try
            {
                if (_2018Ver == 0)
                {
                    Tcmd = "<cl2018 ,comserver ,hello ,py ,>";
                    Tcl = System.Text.Encoding.ASCII.GetBytes(Tcmd);
                    TupdOne.SendTo(Tcl, Tremoto);
                    Delay(100);

                    Tcmd = "<cl2018 ,comserver ,close ,py ,pcom" + string.Format("{0:0}", CommPort) + " ,>";
                    Tcl = System.Text.Encoding.ASCII.GetBytes(Tcmd);
                    TupdOne.SendTo(Tcl, Tremoto);
                    Delay(100);

                    Tcmd = "<cl2018 ,comserver ,open ,py ,pcom" + string.Format("{0:0}", CommPort) + " ,pdir1 ,>";
                    Tcl = System.Text.Encoding.ASCII.GetBytes(Tcmd);
                    TupdOne.SendTo(Tcl, Tremoto);
                    Delay(100);

                    Tcmd = "<cl2018 ,comserver ,init ,py ,pcom" + string.Format("{0:0}", CommPort) + " ,p" + G_BTL + " ,>";

                    Tcl = System.Text.Encoding.ASCII.GetBytes(Tcmd);
                    TupdOne.SendTo(Tcl, Tremoto);
                    Delay(100);
                }
                else
                {
                    string msg = "";
                    //msg = "reset";
                    //Tcl = System.Text.Encoding.ASCII.GetBytes(msg);
                    //TupdOneInitPort.SendTo(Tcl, Tremoto);
                    //Delay(10);

                    msg = "init " + G_BTL.Replace(',', ' '); ;
                    Tcl = System.Text.Encoding.ASCII.GetBytes(msg);
                    TupdOneInitPort.SendTo(Tcl, Tremoto);

                    TupdOne.SendTo(new byte[] { 0, 1 }, Tremoto);
                }
                return true;

            }
            catch
            {
                return false;
            }
        }

        public void SendData(string str)
        {

            byte[] Tcl = ChangeTtoByte(str);
            byte[] Tcl2 = new byte[0];
            int ilen = (Tcl.Length % 5000 > 0) ? (Tcl.Length / 5000 + 1) : (Tcl.Length / 5000);
            for (int i = 1; i <= ilen; i++)
            {
                Tcl2 = new byte[i == ilen ? (Tcl.Length - (i - 1) * 5000) : 5000];
                for (int j = 0; j < Tcl2.Length; j++)
                {
                    Tcl2[j] = Tcl[(i - 1) * 5000 + j];
                }
                TupdOne.SendTo(Tcl2, Tremoto);
            }

        }

        public bool Colse()
        {
            //string Tcmd;
            //byte[] Tcl;

            try
            {
                //Tcmd = "<cl2018 ,comserver ,close ,py ,pcom" + string.Format("{0:0}", CommPort) + " ,>";
                //Tcl = System.Text.Encoding.ASCII.GetBytes(Tcmd);
                ////Tudp.Send(Tcl, Tcl.Length);
                //TupdOne.SendTo(Tcl, Tremoto);
                //Delay(10);
                //State = CST_CLOSE;
                //Ttimer2.Enabled = false;
                return true;

            }
            catch
            {
                return false;
            }
        }
    }
}
