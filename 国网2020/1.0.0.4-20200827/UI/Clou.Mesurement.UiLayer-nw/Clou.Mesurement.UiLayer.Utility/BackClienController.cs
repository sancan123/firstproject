using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Mesurement.UiLayer.Utility
{
   public class BackClienController
    {
       private static string clientName = "Backup";

       /// <summary>
       /// 判断数据备份的EXE有没有在运行
       /// </summary>
       public static bool IsRunningBackUp
       {
           get
           {
               Process[] processes = Process.GetProcesses();
               Process processTemp = processes.FirstOrDefault(item => item.ProcessName == clientName);
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
       /// 关闭数据备份客户端
       /// </summary>
       /// <returns>成功:true,失败:false</returns>
       public static bool CloseBackUpClient()
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
               return true;
           }
           return true;
       }

       /// <summary>
       /// 运行数据备份客户端
       /// </summary>
       /// <returns>运行成功:true,运行失败:false</returns>
       public static bool RunBackUpClient()
       {
           try
           {
               string strFileName = string.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), clientName);
               Process.Start(strFileName);
           }
           catch (Exception e)
           {
               Log.LogManager.AddMessage(string.Format("启动数据备份BackUp.exe失败:{0}", e.Message), Log.EnumLogSource.用户操作日志, Log.EnumLevel.Error);
               return false;
           }
           return true;
       }
    }
}
