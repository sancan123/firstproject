
using CLDC_DataCore;
using CLDC_DataCore.Const;
using CLDC_VerifyAdapter.VerifyService;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CLDC_VerifyAdapter.Multi
{
    /// <summary>
    /// 通讯测试类
    /// </summary>
    class Dgn_CommTest : DgnBase
    {
        public Dgn_CommTest(object plan)
            : base(plan)
        { }

        private static UdpClient udpcSend = null;
        Socket client;
        static IPEndPoint localIpep = null;
        /// <summary>
        /// 通讯测试
        /// </summary>
        public override void Verify()
        {
            ResultNames = new string[] { "检定数据", "结论" };
            base.Verify();

          //  localIpep = new IPEndPoint(IPAddress.Parse("192.168.8.106"), 8080); // 本机IP和监听端口号
          //  udpcSend = new UdpClient(localIpep);


          //  string message = "{'TestType':'Durability','CustomerID':'SCM','ProductModel':'DNB-01','ProductSN':'00237654893761','ProductType':'3','Time':'2020-06-18 14:14:07','Voltage':{'Ua':'380.3','Ub':'380.4','Uc':'380.5'},'Current':{'Ia':'2.3','Ib':'2.4','Ic':'2.5'},'PowerError':{'ActivePE':'12','ReactivePE':'2'},'PF':'240','Power':{'ActivePower':'120','ReactivePower':'23'},'RefEnergy':{'RefActiveEnergy':'10','RefReactiveEnergy':'12'},'IUTEnergy':{'IUTActiveEnergy':'20','IUTReactiveEnergy':'30'},'Temperature':'110'}";
          ////  DatagramPacket dp = new DatagramPacket(str.getBytes(), 0, str.length(), InetAddress.getByName("localhost"), 9999);
          //  byte[] sendbytes = Encoding.ASCII.GetBytes(message);
          //  IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Parse("120.79.170.121"), 50001); // 发送到的IP地址和端口号
          //  udpcSend.Send(sendbytes, sendbytes.Length, remoteIpep);




            bool bPowerOn = PowerOn();

            string[] str_Data = new string[BwCount];
            string[] strERand2 = new string[BwCount];
            string[] strEsamNo = new string[BwCount];
            string[] strRand1 = new string[BwCount];
            string[] strRand2 = new string[BwCount];
            string[] strERand1 = new string[BwCount];
            int[] iFlag = new int[BwCount];



            string[] arrStrResultKey = new string[BwCount];
            MessageController.Instance.AddMessage("正在进行通信测试...");
            string[] address = MeterProtocolAdapter.Instance.ReadAddress();
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "检定数据", address);

            MessageController.Instance.AddMessage("正在处理结果...");
            for (int i = 0; i < BwCount; i++)
            {
                if (Stop) return;                   //假如当前停止检定，则退出
                if (!Helper.MeterDataHelper.Instance.Meter(i).YaoJianYn)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(address[i]))
                {
                    Helper.MeterDataHelper.Instance.Meter(i).Mb_chrAddr = address[i];
                }
                ResultDictionary["结论"][i] = (string.IsNullOrEmpty(address[i]) == false) ? Variable.CTG_HeGe : Variable.CTG_BuHeGe;
            }
            MessageController.Instance.UploadCheckResult(VerifyProcess.Instance.CurrentKey, "结论", ResultDictionary["结论"]);

            Adapter.Instance.UpdateMeterProtocol();
            ReadMeterNo();
        }
        private static void SendMessage(object obj)
        {
            try
            {
              
                //udpcSend.Close();
            }
            catch { }
        }




    }
}
/*===========================================================================================================*/
