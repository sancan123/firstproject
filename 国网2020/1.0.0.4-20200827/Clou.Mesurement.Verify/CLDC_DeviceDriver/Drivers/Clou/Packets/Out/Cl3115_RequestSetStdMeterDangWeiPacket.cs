using System;
using System.Collections.Generic;
using System.Text;
using CLDC_Comm.Enum;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 设置档位
    /// </summary>
    internal class CL3115_RequestSetStdMeterDangWeiPacket : CL3115SendPacket
    {
        /// <summary>
        /// C相电压档位
        /// </summary>
        private Cus_StdMeterVDangWei ucUcRange;
        /// <summary>
        /// B相电压档位
        /// </summary>
        private Cus_StdMeterVDangWei ucUbRange;
        /// <summary>
        /// A相电压档位
        /// </summary>
        private Cus_StdMeterVDangWei ucUaRange;
        /// <summary>
        /// C相电流档位
        /// </summary>
        private Cus_StdMeterIDangWei ucIcRange;
        /// <summary>
        /// B相电流档位
        /// </summary>
        private Cus_StdMeterIDangWei ucIbRange;
        /// <summary>
        /// C相电流档位
        /// </summary>
        private Cus_StdMeterIDangWei ucIaRange;

        /// <summary>
        /// 通一设置档位,默认需要回复
        /// </summary>
        /// <param name="uRange">电压档位</param>
        /// <param name="iRange">电流档位</param>
        public CL3115_RequestSetStdMeterDangWeiPacket(Cus_StdMeterVDangWei uRange, Cus_StdMeterIDangWei iRange)
            : base()
        {
            ucUaRange = uRange;
            ucUbRange = uRange;
            ucUcRange = uRange;
            ucIaRange = iRange;
            ucIbRange = iRange;
            ucIcRange = iRange;
        }
        /// <summary>
        /// 通一设置档位
        /// </summary>
        /// <param name="uRange">电压档位</param>
        /// <param name="iRange">电流档位</param>
        /// <param name="needReplay">是否需要回复</param>
        public CL3115_RequestSetStdMeterDangWeiPacket(Cus_StdMeterVDangWei uRange, Cus_StdMeterIDangWei iRange, bool needReplay)
            : base(needReplay)
        {
            ucUaRange = uRange;
            ucUbRange = uRange;
            ucUcRange = uRange;
            ucIaRange = iRange;
            ucIbRange = iRange;
            ucIcRange = iRange;
        }

        /// <summary>
        /// 设置档位
        /// </summary>
        /// <param name="uaRange">A相电压档位</param>
        /// <param name="ubRange">B相电压档位</param>
        /// <param name="ucRange">C相电压档位</param>
        /// <param name="iaRange">A相电流档位</param>
        /// <param name="ibRange">B相电流档位</param>
        /// <param name="icRange">C相电流档位</param>
        public CL3115_RequestSetStdMeterDangWeiPacket(Cus_StdMeterVDangWei uaRange, Cus_StdMeterVDangWei ubRange, Cus_StdMeterVDangWei ucRange, Cus_StdMeterIDangWei iaRange, Cus_StdMeterIDangWei ibRange, Cus_StdMeterIDangWei icRange)
            : base()
        {
            ucUaRange = uaRange;
            ucUbRange = ubRange;
            ucUcRange = ucRange;
            ucIaRange = iaRange;
            ucIbRange = ibRange;
            ucIcRange = icRange;
        }
        /// <summary>
        /// 设置档位
        /// </summary>
        /// <param name="uaRange">A相电压档位</param>
        /// <param name="ubRange">B相电压档位</param>
        /// <param name="ucRange">C相电压档位</param>
        /// <param name="iaRange">A相电流档位</param>
        /// <param name="ibRange">B相电流档位</param>
        /// <param name="icRange">C相电流档位</param>
        /// <param name="needReplay">是否需要回复</param>
        public CL3115_RequestSetStdMeterDangWeiPacket(Cus_StdMeterVDangWei uaRange, Cus_StdMeterVDangWei ubRange, Cus_StdMeterVDangWei ucRange, Cus_StdMeterIDangWei iaRange, Cus_StdMeterIDangWei ibRange, Cus_StdMeterIDangWei icRange, bool needReplay)
            : base(needReplay)
        {
                ucUaRange=uaRange;
                ucUbRange = ubRange;
                ucUcRange=ucRange;
                ucIaRange=iaRange;
                ucIbRange=ibRange;
                ucIcRange=icRange;
        }

        /*
         * 81 30 PCID 0F A3 02 02 3F ucUcRange ucUbRange ucUaRange ucIcRange ucIbRange ucIaRange CS
         */
        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);          //命令 
            buf.Put(0x02);
            buf.Put(0x02);
            buf.Put(0x3F);
            buf.Put((byte)ucUcRange);
            buf.Put((byte)ucUbRange);
            buf.Put((byte)ucUaRange);
            buf.Put((byte)ucIcRange);
            buf.Put((byte)ucIbRange);
            buf.Put((byte)ucIaRange);
            return buf.ToByteArray();
        }
    }
}
