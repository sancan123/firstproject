namespace CLDC_MeterProtocol.Packet
{
    /// <summary>
    /// 电能表多功能通讯数据包接收类
    /// </summary>
    public class MeterProtocolRecvPacket : CLDC_DataCore.SocketModule.Packet.RecvPacket
    {
       public byte[] RecvData { get; set; }
        public override bool ParsePacket(byte[] buf)
        {
            RecvData = buf;
            return true;
        }
    }
}
