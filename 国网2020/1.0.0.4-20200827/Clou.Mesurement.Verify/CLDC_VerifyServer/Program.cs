using System;
using CLDC_Interfaces;
using CLDC_Interfaces.WcfDynamicHost;
using CLDC_DataCore;
using CLDC_VerifyAdapter.VerifyService;
using System.Diagnostics;

namespace CLDC_VerifyServer
{
    class Program
    {
        static void Main(string[] args)
        {
            WcfServiceHost sv = new WcfServiceHost();
            try
            {
                sv.StartService(Properties.Settings.Default.VerifyIP, typeof(VerifyControl), typeof(IVerifyControl));
                Console.WriteLine("启动检定服务成功。");
            }
            catch (Exception ex)
            {
                Console.WriteLine("启动检定服务失败：" + ex.Message);
            }
            try
            {
                CLDC_VerifyClient.VerifyClient.Instance.RequestId();
            }
            catch (Exception ex)
            {
                Console.WriteLine("连接检定控制系统失败：" + ex.Message);
            }
            //string cmd = Console.ReadLine();
            //if ("exit".Equals(cmd))
            {
                System.Windows.Forms.Application.Run(new FormAboutBox());
                try
                {
                    sv.StopService();
                    MessageController.Instance.AddMessage("停止检定服务成功。");
                    Console.WriteLine("停止检定服务成功。");
                }
                catch (Exception ex)
                {
                    MessageController.Instance.AddMessage("停止检定服务失败：" + ex.Message);
                    Console.WriteLine("停止检定服务失败：" + ex.Message);
                }
            }

            #region 结束线程
            CLDC_Comm.Log.Log4netHelper.UnLoadLogCommpent();
            //检定线程
            VerifyProcess.Instance.Exit();
            VerifyControl.cm = null;
            GC.Collect();
            //自杀
            Process.GetCurrentProcess().Kill();
            #endregion
        }

    }
}
