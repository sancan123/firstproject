using CLDC_DataCore.Const;
using System;


namespace CLDC_MeterProtocol.Packet
{
    /// <summary>
    /// 电能表数据发送包基类
    /// </summary>
    public class MeterProtocolSendPacket : CLDC_DataCore.SocketModule.Packet.SendPacket
    {
        public byte[] SendData { get; set; }

        public override int WaiteTime()
        {
            return 1000;
        }
        public override byte[] GetPacketData()
        {
            return SendData;
        }
        /// <summary>
        /// 打包645成载波，TODO:这里固定成了07
        /// </summary>
        public void PacketTo3762(ref byte[] out_645Frame, int int_BwIndex)
        {
            int StartIndex = 0;
            int put_byt_Len = SendData.Length;
            for (int i = 0; i < put_byt_Len; i++)
            {
                if (SendData[i] == 0x68)
                {
                    StartIndex = i;
                    break;
                }
            }
            byte[] put_byt = new byte[put_byt_Len - StartIndex];
            Array.Copy(SendData, StartIndex, put_byt, 0, put_byt.Length);
            //CLDC_VerifyAdapter.Helper.EquipHelper.Instance.PacketTo3762(put_byt, (byte)2, ref out_645Frame, CLDC_DataCore.Const.GlobalUnit.Flag_IsZD2016 , int_BwIndex);
            CLDC_DeviceDriver.Driver.Packet645To3762Frame(put_byt, 0x02, ref out_645Frame, CLDC_DataCore.Const.GlobalUnit.Flag_IsZD2016, int_BwIndex, int.Parse(GlobalUnit.CarrierInfo.Comm));
        }
    }
}
