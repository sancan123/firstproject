namespace CLDC_SafeFileProtocol.Protocols
{
    class PacketSend : CLDC_DataCore.SocketModule.Packet.SendPacket
    {
        public byte[] SendData { get; set; }

        public override int WaiteTime()
        {
            return 200;
        }
        public override byte[] GetPacketData()
        {
            return SendData;
        }
    }
}
