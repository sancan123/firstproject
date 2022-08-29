/*
 2012/2/19 modifyed by niaoked
 * 初始化端口由端口自己完成。
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Win32;



namespace E_CLSocketModule.SocketModule.Sock
{
    /// <summary>
    /// UDP端口
    /// </summary>
    internal class UDPClient : IConnection
    {
        private int UdpBindPort;
        private UdpClient Client;
        private UdpClient settingClient;
        private string szBlt = "1200,e,8,1";
        private IPEndPoint Point = new IPEndPoint(IPAddress.Parse("193.168.18.1"), 10003);
        private IPEndPoint localPoint = null;
        private string m_2018IpAddress = string.Empty;
        private string m_bHaveProtocol = "2018-负控";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Ip"></param>
        /// <param name="BindPort">com</param>
        /// <param name="RemotePort">10003,10004</param>
        /// <param name="BasePort">本地起始端口</param>
        public UDPClient(string Ip, int BindPort, int RemotePort, int BasePort, string bHaveProtocol)
        {
            m_bHaveProtocol = bHaveProtocol;
            m_2018IpAddress = Ip;
            Point.Address = IPAddress.Parse(Ip);
            Point.Port = RemotePort;

            try
            {
                if (m_bHaveProtocol == "2018-负控")
                {
                    UdpBindPort = BindPort;
                    localPoint = new IPEndPoint(IPAddress.Parse(GetHostIp()), LocalPortTo2011Port(BindPort, BasePort));
                    Client = new UdpClient();
                    Client.Client.Bind(this.localPoint);
                    Client.Connect(Point);
                    byte[] byt_Data = new byte[0];

                    string str_Data = "<cl2018 ,comserver ,hello ,py ,>";
                    byt_Data = ASCIIEncoding.ASCII.GetBytes(str_Data);
                    int sendlen = Client.Send(byt_Data, byt_Data.Length);
                    Thread.Sleep(50);
                    str_Data = "<cl2018 ,comserver ,close ,py ,pcom" + BindPort.ToString() + " ,>";
                    byt_Data = ASCIIEncoding.ASCII.GetBytes(str_Data);
                    sendlen = Client.Send(byt_Data, byt_Data.Length);
                    Thread.Sleep(50);
                    str_Data = "<cl2018 ,comserver ,open ,py ,pcom" + BindPort.ToString() + " ,pdir1 ,>";
                    byt_Data = ASCIIEncoding.ASCII.GetBytes(str_Data);
                    sendlen = Client.Send(byt_Data, byt_Data.Length);
                }
                else if (m_bHaveProtocol == "2018-电能")
                {
                    UdpBindPort = LocalPortTo2011Port(BindPort, BasePort);//转换端口成2018端口
                    localPoint = new IPEndPoint(IPAddress.Parse(GetHostIp()), LocalPortTo2011Port(BindPort, BasePort));
                    Client = new UdpClient();
                    Client.Client.Bind(this.localPoint);
                    Client.Connect(Point);
                    localPoint = new IPEndPoint(IPAddress.Parse(GetHostIp()), UdpBindPort);
                }
                else
                {
                    UdpBindPort = LocalPortToCTS9032Port(BindPort, BasePort);//转换端口成2018端口
                    localPoint = new IPEndPoint(IPAddress.Parse(GetHostIp()), LocalPortToCTS9032Port(BindPort, BasePort));
                    Client = new UdpClient();
                    Client.Client.Bind(this.localPoint);
                    Point.Port = BindPort + RemotePort - 1;
                    Client.Connect(Point);
                    localPoint = new IPEndPoint(IPAddress.Parse(GetHostIp()), UdpBindPort);
                }
            }
            catch (Exception)
            {
            }

        }

        public static uint IPToUint(string ipAddress)
        {
            string[] strs = ipAddress.Trim().Split('.');
            byte[] buf = new byte[4];

            for (int i = 0; i < strs.Length; i++)
            {
                buf[i] = byte.Parse(strs[i]);
            }
            Array.Reverse(buf);

            return BitConverter.ToUInt32(buf, 0);
        }

        public static string GetSubnetworkIP(string targetIP)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\services\Tcpip\Parameters\Interfaces", RegistryKeyPermissionCheck.ReadSubTree, System.Security.AccessControl.RegistryRights.ReadKey);
            uint iTarget = IPToUint(targetIP);
            foreach (string keyName in key.GetSubKeyNames())
            {
                try
                {
                    RegistryKey tmpKey = key.OpenSubKey(keyName);
                    string[] ip = tmpKey.GetValue("IPAddress") as string[];
                    if (ip == null)
                    {
                        continue;
                    }
                    string[] subnet = tmpKey.GetValue("SubnetMask") as string[];
                    for (int i = 0; i < ip.Length; i++)
                    {
                        IPAddress local = IPAddress.Parse(ip[i]);
                        if (local.IsIPv6SiteLocal)
                            continue;

                        uint iIP = IPToUint(ip[i]);
                        uint iSub = IPToUint(subnet[i]);

                        if ((iIP & iSub) == (iTarget & iSub))
                        {
                            return ip[i];
                        }
                    }
                }
                catch
                {
                }
            }
            return "127.0.0.1";
            //throw new Exception("未在本计算机上找到与目标地址：" + targetIP + " 相匹配的网段IP地址，无法发送数据,请配置应用的IP地址");
        }


        public string GetHostIp()
        {
            string Tip;
            Tip = Dns.GetHostName();
            IPHostEntry Tipentry = Dns.GetHostEntry(Tip);
            string[] str2018Ips = m_2018IpAddress.Split('.');
            bool bChaZhao = false;
            string strResult = string.Empty;
            for (int i = 0; i < Tipentry.AddressList.Length; i++)
            {
                Tip = Tipentry.AddressList[i].ToString();
                string[] Tipg;
                Tipg = Tip.Split('.');
                if (Tipg.Length == 4)
                {
                    if (str2018Ips.Length == 4)
                    {
                        if (Tipg[0] == str2018Ips[0]
                            && Tipg[1] == str2018Ips[1]
                            && Tipg[2] == str2018Ips[2])
                        {
                            strResult = Tip;
                            break;
                        }


                    }
                    else
                    {
                        if (!bChaZhao)
                        {
                            bChaZhao = true;
                            strResult = Tip;
                        }
                    }

                }
            }
            if (strResult == "")
                strResult = "127.0.0.1";
            return strResult;

        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="vData"></param>
        /// <param name="IsReturn"></param>
        /// <param name="WaiteTime"></param>
        /// <returns></returns>
        public bool SendData(ref byte[] vData, bool IsReturn, int WaiteTime, int MaxByte)
        {
            try
            {
                if (Client.Client == null)
                {
                    Client = new UdpClient();
                    Client.Client.Bind(this.localPoint);
                }
                Client.Connect(Point);
            }
            catch { return false; }
            if (m_bHaveProtocol == "2018-负控")
            {
                string strSendData = BitConverter.ToString(vData).Replace("-", "");

                string str_Data = "";

                str_Data = "<cl2018 ,comserver ,ask ,pn ,pcom" + UdpBindPort.ToString() + " ,data" + strSendData + " ,>";

                str_Data = "<cl2018 ,comserver ,ask ,py ,pcom" + UdpBindPort.ToString() + " ,pnum" + MaxByte.ToString() + " ,pwait100 ,data" + strSendData + " ,>";

                str_Data = "<cl2018 ,comserver ,ask ,pn ,pcom" + UdpBindPort.ToString() + " ,data" + strSendData + " ,>";

                vData = ASCIIEncoding.ASCII.GetBytes(str_Data);
            }
            Client.Send(vData, vData.Length);
            Console.WriteLine(UdpBindPort.ToString());
            Console.WriteLine("┏SendData:{0}", BitConverter.ToString(vData));
            if (!IsReturn)
            {
                Console.WriteLine("┗本包不需要回复");
                Client.Close();
                return true;
            }
            //Thread.Sleep(WaiteTime);
            byte[] BytReceived = new byte[0];
            bool IsReveive = false;     //标志是否返回
            List<byte> RevItems = new List<byte>();     //接收的数据集合
            DateTime Dt;            //等待时间变量
            Dt = DateTime.Now;
            while (TimeSub(DateTime.Now, Dt) < MaxWaitSeconds)
            {
                Thread.Sleep(1);
                try
                {
                    if (Client.Available > 0)
                    {
                        BytReceived = Client.Receive(ref Point);
                        IsReveive = true;
                        break;
                    }
                }
                catch
                {
                    Client.Close();
                    return false;
                }
            }

            if (!IsReveive)
            {
                vData = new byte[0];
            }
            else
            {
                RevItems.AddRange(BytReceived);
                Dt = DateTime.Now;
                while (TimeSub(DateTime.Now, Dt) < WaitSecondsPerByte)
                {
                    if (Client.Available > 0)
                    {
                        BytReceived = Client.Receive(ref Point);
                        RevItems.AddRange(BytReceived);
                        Dt = DateTime.Now;
                    }
                }
                vData = RevItems.ToArray();
            }
            if (m_bHaveProtocol == "2018-负控")
            {
                string strRec = Encoding.Default.GetString(vData).Trim('<').Trim('>');

                string[] arrRec = strRec.Split(',');
                if (arrRec.Length > 3)
                {
                    switch (arrRec[2])
                    {
                        case "ask":
                            if (arrRec[5] == "pa0")
                            {
                                string strRecInfo = arrRec[6].Replace("data", "");
                                vData = ConvertStringToBytes(strRecInfo);
                            }
                            break;
                    }
                }
            }
            Console.WriteLine("┗RecvData:{0}", BitConverter.ToString(vData));
            Client.Close();
            return true;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="vData"></param>
        /// <param name="IsReturn"></param>
        /// <param name="WaiteTime"></param>
        /// <returns></returns>
        public bool SendData(ref byte[] vData, bool IsReturn, int WaiteTime, int MaxByte, int MaxWaitSeconds)
        {
            try
            {
                try
                {
                    if (Client.Client == null)
                    {
                        Client = new UdpClient();
                        Client.Client.Bind(this.localPoint);
                    }
                    Client.Connect(Point);
                }
                catch { return false; }
                if (m_bHaveProtocol == "2018-负控")
                {
                    string strSendData = BitConverter.ToString(vData).Replace("-", "");

                    //string str_Data = "<cl2018 ,comserver ,ask ,py ,pcom" + UdpBindPort.ToString() + " ,pnum" + MaxByte.ToString() + " ,pwait100 ,data" + strSendData + " ,>";

                    //str_Data = "<cl2018 ,comserver ,ask ,py ,pcom" + UdpBindPort.ToString() + " ,pnum" + "255" + " ,pwait100 ,data" + strSendData + " ,>";

                    string str_Data = "<cl2018 ,comserver ,ask ,pn ,pcom" + UdpBindPort.ToString() + " ,data" + strSendData + " ,>";

                    vData = ASCIIEncoding.ASCII.GetBytes(str_Data);
                }
                //System.IO.File.AppendAllText("test", localPoint.Address + "," + localPoint.Port + Environment.NewLine);
                //System.IO.File.AppendAllText("test", localPoint.Address + "," + localPoint.Port + Environment.NewLine);
                //System.IO.File.AppendAllText("test", Point.Address + "," + Point.Port + Environment.NewLine);
                //System.IO.File.AppendAllText("test", Point.Address + "," + Point.Port + Environment.NewLine);
                Client.Send(vData, vData.Length);
                Console.WriteLine(UdpBindPort.ToString());
                Console.WriteLine("┏SendData:{0}", BitConverter.ToString(vData));
                if (!IsReturn)
                {
                    Console.WriteLine("┗本包不需要回复");
                    Client.Close();
                    return true;
                }
                //Thread.Sleep(WaiteTime);
                byte[] BytReceived = new byte[0];
                bool IsReveive = false;     //标志是否返回
                List<byte> RevItems = new List<byte>();     //接收的数据集合
                DateTime Dt;            //等待时间变量
                Dt = DateTime.Now;
                while (TimeSub(DateTime.Now, Dt) < MaxWaitSeconds)
                {
                    Thread.Sleep(1);
                    try
                    {
                        if (Client.Available > 0)
                        {
                            BytReceived = Client.Receive(ref Point);
                            IsReveive = true;
                            break;
                        }
                    }
                    catch
                    {
                        Client.Close();
                        return false;
                    }
                }

                if (!IsReveive)
                {
                    vData = new byte[0];
                }
                else
                {
                    RevItems.AddRange(BytReceived);
                    Dt = DateTime.Now;
                    while (TimeSub(DateTime.Now, Dt) < WaitSecondsPerByte)
                    {
                        if (Client.Available > 0)
                        {
                            BytReceived = Client.Receive(ref Point);
                            RevItems.AddRange(BytReceived);
                            Dt = DateTime.Now;
                        }
                    }
                    vData = RevItems.ToArray();
                }
                if (m_bHaveProtocol == "2018-负控")
                {
                    string strRec = Encoding.Default.GetString(vData).Trim('<').Trim('>');

                    string[] arrRec = strRec.Split(',');
                    if (arrRec.Length > 3)
                    {
                        switch (arrRec[2])
                        {
                            case "ask":
                                if (arrRec[5] == "pa0")
                                {
                                    string strRecInfo = arrRec[6].Replace("data", "");
                                    vData = ConvertStringToBytes(strRecInfo);
                                }
                                break;
                        }
                    }
                }
                Console.WriteLine("┗RecvData:{0}", BitConverter.ToString(vData));
                Client.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private byte[] ConvertStringToBytes(string p_str_Context)
        {
            int int_ByteCount = p_str_Context.Length / 2;
            byte[] byt_Return = new byte[int_ByteCount];

            for (int i = 0; i < int_ByteCount; i++)
            {
                byt_Return[i] = Convert.ToByte(p_str_Context.Substring(i * 2, 2), 16);
            }

            return byt_Return;
        }
        private long TimeSub(DateTime Time1, DateTime Time2)
        {
            TimeSpan tsSub = Time1.Subtract(Time2);
            return tsSub.Hours * 60 * 60 * 1000 + tsSub.Minutes * 60 * 1000 + tsSub.Seconds * 1000 + tsSub.Milliseconds;
        }

        /// <summary>
        /// 本地通道转换成2018端口:20000 + 2 * (port - 1);
        /// 数据端口，设置端口在数据端口的基础上+1
        /// </summary>
        /// <param name="port"></param>
        /// <param name="BasePort"></param>
        /// <returns></returns>
        private int LocalPortTo2011Port(int port, int BasePort)
        {
            return BasePort + 2 * (port - 1);
        }

        /// <summary>
        /// 本地通道转换成CTS9032端口:20000 + (port - 1);
        /// 数据端口，设置端口在数据端口的基础上+1
        /// </summary>
        /// <param name="port"></param>
        /// <param name="BasePort"></param>
        /// <returns></returns>
        private int LocalPortToCTS9032Port(int port, int BasePort)
        {
            return BasePort + (port - 1);
        }

        #region IConnection 成员

        public string ConnectName
        {
            get
            {
                return Point.ToString();
            }
            set
            {
            }
        }

        public int MaxWaitSeconds
        {
            get;
            set;
        }

        public int WaitSecondsPerByte
        {
            get;
            set;
        }

        public bool Open()
        {
            return true;
        }

        public bool Close()
        {
            return true;
        }

        /// <summary>
        /// 更新232串口波特率
        /// </summary>
        /// <param name="szSetting"></param>
        /// <returns></returns>
        public bool UpdateBltSetting(string szSetting)
        {
            //if (szBlt == szSetting) return true;//与上次相同，则不用初始化
            szBlt = szSetting;
            int settingPort = UdpBindPort + 1;

            try
            {
                try
                {
                    if (m_bHaveProtocol == "2018-负控")
                    {
                        if (Client.Client == null)
                        {
                            Client = new UdpClient();
                            Client.Client.Bind(this.localPoint);
                            Client.Connect(Point);
                        }

                    }
                    else
                    {
                        settingClient = new UdpClient(settingPort);
                        settingClient.Connect(Point);
                    }
                }
                catch { }

                string str_Data = "";
                byte[] byt_Data = new byte[0];
                int sendlen = 0;
                if (m_bHaveProtocol == "2018-负控")
                {//todo:2018带协议

                    str_Data = "<cl2018 ,comserver ,init ,py ,pcom" + UdpBindPort.ToString() + " ,p" + szBlt.Replace(',', '-') + " ,>";

                    byt_Data = ASCIIEncoding.ASCII.GetBytes(str_Data);
                    sendlen = Client.Send(byt_Data, byt_Data.Length);
                    Client.Close();
                    Thread.Sleep(50);
                }
                else if (m_bHaveProtocol == "2018-电能")
                {
                    //str_Data = "reset";
                    //byt_Data = ASCIIEncoding.ASCII.GetBytes(str_Data);
                    //sendlen = settingClient.Send(byt_Data, byt_Data.Length);

                    System.Threading.Thread.Sleep(20);
                    str_Data = "init " + szBlt.Replace(',', ' ');
                    byt_Data = ASCIIEncoding.ASCII.GetBytes(str_Data);
                    sendlen = settingClient.Send(byt_Data, byt_Data.Length);
                    System.Threading.Thread.Sleep(20);
                    settingClient.Close();
                }
                else
                {

                }


                return sendlen == byt_Data.Length;
            }
            catch { }
            finally
            {
            }
            return false;
        }

        #endregion
    }
}
