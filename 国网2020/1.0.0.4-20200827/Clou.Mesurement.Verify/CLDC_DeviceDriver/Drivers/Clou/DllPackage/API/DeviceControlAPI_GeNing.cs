using CLDC_DataCore.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CLDC_DeviceDriver.Drivers.Clou.DllPackage.API
{
   public class DeviceControlAPI_GeNing
    {
       
       [DllImport(@"DllFile\DeviceManufacturers\GeNing\DeviceControl.dll", EntryPoint = "ShowDriverConfig", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       public static extern void ShowDriverConfig();

       [DllImport(@"DllFile\DeviceManufacturers\GeNing\DeviceControl.dll", EntryPoint = "ConnectPower", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       public static extern int ConnectPower();

       [DllImport(@"DllFile\DeviceManufacturers\GeNing\DeviceControl.dll", EntryPoint = "PowerOn", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       public static extern int PowerOn(bool[] MeterPosition, int TestMode, int PowerDirection, string PowerFactor, float VoltageUa, float VoltageUb, float VoltageUc, float CurrentIa, float CurrentIb,
                           float CurrentIc, int Element, float Frequency, bool PhaseSequence);

       [DllImport(@"DllFile\DeviceManufacturers\GeNing\DeviceControl.dll", EntryPoint = "PowerOff", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       public static extern int PowerOff();

       [DllImport(@"DllFile\DeviceManufacturers\GeNing\DeviceControl.dll", EntryPoint = "RelayControl", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       public static extern int RelayControl(int meterIndex, int TypeNo,int meterControlType, ref int Flag);

       [DllImport(@"DllFile\DeviceManufacturers\GeNing\DeviceControl.dll", EntryPoint = "SetLoadRelayControl", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       public static extern int SetLoadRelayControl(bool[] MeterPosition, int ControlType);

       [DllImport(@"DllFile\DeviceManufacturers\GeNing\DeviceControl.dll", EntryPoint = "Init485", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       public static extern int Init485(int meterIndex, string setting);

       [DllImport(@"DllFile\DeviceManufacturers\GeNing\DeviceControl.dll", EntryPoint = "SendToMeter", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       public static extern int SendToMeter(int meterIndex, string setting, string sendFrame, int outTime, ref string RecvFrame);

       [DllImport(@"DllFile\DeviceManufacturers\GeNing\DeviceControl.dll", EntryPoint = "ConnectRefMeter", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       public static extern int ConnectRefMeter();

       [DllImport(@"DllFile\DeviceManufacturers\GeNing\DeviceControl.dll", EntryPoint = "ReadInstMetricAll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       public static extern int ReadInstMetricAll(out float[] instValue);

       [DllImport(@"DllFile\DeviceManufacturers\GeNing\DeviceControl.dll", EntryPoint = "InitInfrared", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       public static extern int InitInfrared(int meterIndex, string setting);

       [DllImport(@"DllFile\DeviceManufacturers\GeNing\DeviceControl.dll", EntryPoint = "SendInfraredToMeter", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
       public static extern int SendInfraredToMeter(int meterIndex, string setting, string sendFrame, int outTime, ref string RecvFrame);

    }
}
