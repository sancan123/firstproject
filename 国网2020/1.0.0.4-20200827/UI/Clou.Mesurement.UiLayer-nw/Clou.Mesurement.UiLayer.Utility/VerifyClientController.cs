using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;

namespace Mesurement.UiLayer.Utility
{
    /// <summary>
    /// 检定客户端控制器
    /// </summary>
    public class VerifyClientController
    {
        /// <summary>
        /// 检定客户端名称
        /// </summary>
        private static string clientName = "CLDC_VerifyServer";
        private static string clientDebugName = "CLDC_VerifyServer.vshost";
        /// <summary>
        /// 判断检定客户端正在运行
        /// </summary>
        public static bool IsRunning
        {
            get
            {
                Process[] processes = Process.GetProcesses();
                Process processTemp = processes.FirstOrDefault(item => item.ProcessName == clientDebugName);
                if (processTemp != null)
                {
                    return true;
                }
                processTemp = processes.FirstOrDefault(item => item.ProcessName == clientName);
                if (processTemp == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        /// <summary>
        /// 运行检定客户端
        /// </summary>
        /// <returns>运行成功:true,运行失败:false</returns>
        public static bool RunVerifyClient()
        {
            try
            {
                    Process.Start(string.Format(@"{0}\Verify\{1}", Directory.GetCurrentDirectory(), clientName));
            }
            catch (Exception e)
            {
                Log.LogManager.AddMessage(string.Format("启动检定客户端程序失败:{0}", e.Message), Log.EnumLogSource.用户操作日志, Log.EnumLevel.Error);
                return false;
            }
            return false;
        }
        /// <summary>
        /// 关闭检定客户端
        /// </summary>
        /// <returns>成功:true,失败:false</returns>
        public static bool CloseVerifyClient()
        {
            try
            {
                Process[] processes = Process.GetProcesses();
                Process processTemp = processes.FirstOrDefault(item => item.ProcessName == clientName);
                if (processTemp != null)
                {
                    processTemp.Kill();
                }
            }
            catch (Exception e)
            {
                Log.LogManager.AddMessage(string.Format("关闭检定客户端程序失败:{0}", e.Message), Log.EnumLogSource.用户操作日志, Log.EnumLevel.Error);
                return false;
            }
            return true;
        }
    }
}
