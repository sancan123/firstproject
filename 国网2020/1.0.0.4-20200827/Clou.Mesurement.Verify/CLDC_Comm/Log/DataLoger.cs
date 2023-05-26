using System;

namespace CLDC_Comm
{
    public  class DataLoger
    {
        private static string LogFile = System.Windows.Forms.Application.StartupPath + "\\Log\\cldatamanager.txt";
        public static void Log(string message)
        {
            System.IO.File.AppendAllText(LogFile, message + Environment.NewLine);
        }
    }
}
