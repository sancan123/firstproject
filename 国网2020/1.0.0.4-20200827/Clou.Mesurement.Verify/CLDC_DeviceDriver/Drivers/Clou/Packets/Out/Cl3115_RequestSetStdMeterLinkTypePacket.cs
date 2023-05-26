using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 设置接线方式
    /// </summary>
    internal class CL3115_RequestSetStdMeterLinkTypePacket : CL3115SendPacket
    {
        private byte _SetData;
        /// <summary>
        /// 
        /// </summary>
        public CL3115_RequestSetStdMeterLinkTypePacket()
        {
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_Clfs">测量方式</param>
        /// <param name="bAuto">自动，手动</param>
        public void SetPara(CLDC_Comm.Enum.Cus_Clfs _Clfs, bool bAuto)
        {
            if (CLDC_Comm.GlobalUnit.IsDan)
            {
                if (bAuto)
                    _SetData = 0x08;
                else
                    _SetData = 0x88;
            }
            else
            {
                if (bAuto)
                {
                    switch (_Clfs)
                    {
                        case CLDC_Comm.Enum.Cus_Clfs.三相四线:                        
                            _SetData = 0x08;
                            break;
                        case CLDC_Comm.Enum.Cus_Clfs.三相三线:                        
                            _SetData = 0x48;
                            break;
                        case CLDC_Comm.Enum.Cus_Clfs.三元件跨相90:
                            _SetData = 0x44;
                            break;
                        case CLDC_Comm.Enum.Cus_Clfs.二元件跨相90:
                            _SetData = 0x42;
                            break;
                        case CLDC_Comm.Enum.Cus_Clfs.二元件跨相60:
                            _SetData = 0x41;
                            break;
                        default:
                            _SetData = 0x08;
                            break;
                    }
                }
                else
                {
                    switch (_Clfs)
                    {
                        case CLDC_Comm.Enum.Cus_Clfs.三相四线:                        
                            _SetData = 0x88;
                            break;
                        case CLDC_Comm.Enum.Cus_Clfs.三相三线:                        
                            _SetData = 0xC8;
                            break;
                        case CLDC_Comm.Enum.Cus_Clfs.三元件跨相90:
                            _SetData = 0xC4;
                            break;
                        case CLDC_Comm.Enum.Cus_Clfs.二元件跨相90:
                            _SetData = 0xC2;
                            break;
                        case CLDC_Comm.Enum.Cus_Clfs.二元件跨相60:
                            _SetData = 0xC1;
                            break;
                        default:
                            _SetData = 0x88;
                            break;
                    }
                }
            }
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.Put(0xA3);
            buf.Put(0x00);
            buf.Put(0x01);
            buf.Put(0x20);
            buf.Put(_SetData);
            return buf.ToByteArray();
        }
    }
}
