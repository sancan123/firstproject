using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace cl_Frontend_Meter
{
    static class Program
    {
        public static Queue<EventsReceiveData> queueReceiveData = new Queue<EventsReceiveData>();//接收指令集合

        public static string UpSideAgreement = "698.45";
        public static string UpSideBaudRate = "2400,e,8,1";
        public static string Element = "合元";

        public static Reg reg = new Reg();
        public static bool bolReg = false;//是否注册
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            reg.sName = "CL3000SH";
            //if (reg.BoolCode(reg.CreateCode(), CLBase.g_GetINI(Application.StartupPath + "\\System\\VirtualMeter.ini", "System", "Registered", "698.45").Replace("-", "")) != "czx")
            //{
            //    MessageBox.Show("软件非法，请先注册", "提示！");

            //    FormReg frm = new FormReg();
            //    frm.ShowDialog();
            //}
            //else
                bolReg = true;
            
            if (bolReg)
                Application.Run(new FormLogin());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log("未处理的异常" + DateTime.Now);
            Log(e.ExceptionObject.ToString());
            Log(e.IsTerminating.ToString());
            MessageBox.Show("检测到未处理异常\r\n请查看文件:" + LogFile, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        static string LogFile = Application.StartupPath + "\\cldatamanager.txt";

        public static void Log(string message)
        {
            System.IO.File.AppendAllText(LogFile, message + Environment.NewLine);
        }
    }
}
