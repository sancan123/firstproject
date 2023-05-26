using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou._3000F.Packets.Out
{
    /// <summary>
    /// 设置功能参数
    /// </summary>
    class RequestSetVerifyPacket : ClouSendPacket_CLT11
    {
        private int tstType;
        private int errNum;
        private int pluse191;
        private int divideFreq;
        private int pluseTime;
        private int edayFreq = 500000;
        private int filter;
        /// <summary>
        /// 测试类型
        /// </summary>
        public int TstType
        {
            get { return tstType; }
            set { tstType = value; }
        }
        /// <summary>
        /// 误差次数
        /// </summary>
        public int ErrNum
        {
            get { return errNum; }
            set { errNum = value; }
        }
        /// <summary>
        /// 191通道
        /// </summary>
        public int Pluse191
        {
            get { return pluse191; }
            set { pluse191 = value; }
        }
        /// <summary>
        /// 脉冲分频系数
        /// </summary>
        public int DivideFreq
        {
            get { return divideFreq; }
            set { divideFreq = value; }
        }
        /// <summary>
        /// 估计被检脉冲间隔时间
        /// </summary>
        public int PluseTime
        {
            get { return pluseTime; }
            set { pluseTime = value; }
        }
        /// <summary>
        /// 标准时钟频率,默认500KHz
        /// </summary>
        public int EdayFreq
        {
            get { return edayFreq; }
            set { edayFreq = value; }
        }
        /// <summary>
        /// 设置脉冲记数状态下的滤波系数，最大系数为15，只用于脉冲计数功能
        /// </summary>
        public int Filter
        {
            get { return filter; }
            set { filter = value; }
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();

            buf.Put(0xa3);
            buf.Put(0x00);
            buf.Put(0x38);
            buf.Put(0x11);
            buf.PutInt_S(tstType);
            buf.PutInt_S(errNum);
            buf.Put(0x20);
            buf.PutInt_S(pluse191);
            buf.Put(0xa5);
            buf.PutInt_S(divideFreq);
            buf.PutInt_S(pluseTime);
            buf.PutInt_S(edayFreq);
            buf.PutInt_S(filter);

            return buf.ToByteArray();
        }

        public override string GetPacketName()
        {
            return "RequestSetVerifyPacket";
        }
    }
}
