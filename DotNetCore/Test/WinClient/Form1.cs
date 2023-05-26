using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WinClient
{
    public partial class Form1 : Form
    {
        private Guid guid;
        public Form1()
        {
            InitializeComponent();
        }
        Socket socketSend;
        private void bt_connect_Click(object sender, EventArgs e)
        {
            try
            {
                //创建客户端Socket，获得远程ip和端口号
                socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(txt_ip.Text);
                IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32(txt_port.Text));

                socketSend.Connect(point);
                ShowMsg("连接成功!");
                //开启新的线程，不停的接收服务器发来的消息
                Thread c_thread = new Thread(Received);
                c_thread.IsBackground = true;
                c_thread.Start();
            }
            catch (Exception)
            {
                ShowMsg("IP或者端口号错误...");
            }
        }



        void ShowMsg(string str)
        {
            textBox1.AppendText(str + "\r\n");
        }
        /// <summary>
        /// 接收服务端返回的消息
        /// </summary>
        void Received()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 1024 * 3];
                    //实际接收到的有效字节数
                    int len = socketSend.Receive(buffer);
                    if (len == 0)
                    {
                        break;
                    }
                    string str = Encoding.UTF8.GetString(buffer, 0, len);
                    ShowMsg(socketSend.RemoteEndPoint + ":" + str);
                }
                catch { }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            guid = Guid.NewGuid();
        }

        private void bt_send_Click(object sender, EventArgs e)
        {
            try
            {
                string msg = txt_msg.Text.Trim();
                byte[] buffer = new byte[1024 * 1024 * 3];
                buffer = Encoding.UTF8.GetBytes(msg);
                socketSend.Send(buffer);
            }
            catch { }
        }
    }
}
