namespace CLDC_SafeFileProtocol.Protocols
{
    class PacketRecv : CLDC_DataCore.SocketModule.Packet.RecvPacket
    {
        public byte[] RecvData { get; set; }
        public override bool ParsePacket(byte[] buf)
        {
            RecvData = buf;
            return true;
        }
    }
}
