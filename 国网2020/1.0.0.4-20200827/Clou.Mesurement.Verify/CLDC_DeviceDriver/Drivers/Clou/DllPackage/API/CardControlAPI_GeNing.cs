using CLDC_DataCore.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CLDC_DeviceDriver.Drivers.Clou.DllPackage.API
{
   public class CardControlAPI_GeNing
    {
       [DllImport(@"DllFile\DeviceManufacturers\GeNing\CardReaderControl.dll", EntryPoint = "ShowCardReaderConfig", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       public static extern void ShowCardReaderConfig();

       [DllImport(@"DllFile\DeviceManufacturers\GeNing\CardReaderControl.dll", EntryPoint = "InitCard", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       public static extern int InitCard();

       [DllImport(@"DllFile\DeviceManufacturers\GeNing\CardReaderControl.dll", EntryPoint = "ResetCard", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       public static extern int ResetCard();

       [DllImport(@"DllFile\DeviceManufacturers\GeNing\CardReaderControl.dll", EntryPoint = "SwitchCardState", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       public static extern int SwitchCardState(int workFlag);

       [DllImport(@"DllFile\DeviceManufacturers\GeNing\CardReaderControl.dll", EntryPoint = "SendToCard", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       public static extern int SendToCard(int meterIndex, string sendFrame, int outTime, ref string recvFrame);

    }
}
