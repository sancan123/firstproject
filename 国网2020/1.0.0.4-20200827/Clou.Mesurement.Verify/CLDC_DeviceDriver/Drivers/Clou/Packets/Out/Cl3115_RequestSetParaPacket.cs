using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 设置标准表参数
    /// </summary>
    internal class CL3115_RequestSetParaPacket : CL3115SendPacket
    {
        private byte _YouGongSetData;
        private byte _ClfsSetData;
        /// <summary>
        /// 
        /// </summary>
        public CL3115_RequestSetParaPacket()
        {
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_Clfs">测量方式</param>        
        public void SetPara(CLDC_Comm.Enum.Cus_Clfs _Clfs,CLDC_Comm.Enum.Cus_PowerFangXiang glfx, bool bAuto)
        {
             
            if (glfx == CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功 || glfx == CLDC_Comm.Enum.Cus_PowerFangXiang.反向有功)
                _YouGongSetData = 0x00;
            else
                _YouGongSetData = 0x40;                   
            

            if (CLDC_Comm.GlobalUnit.IsDan)
            {
                if (bAuto)
                    _ClfsSetData = 0x08;
                else
                    _ClfsSetData = 0x88;
            }
            else
            {
                if (bAuto)
                {
                    switch (_Clfs)
                    {
                        case CLDC_Comm.Enum.Cus_Clfs.三相四线:                        
                            _ClfsSetData = 0x08;
                            break;
                        case CLDC_Comm.Enum.Cus_Clfs.三相三线:                        
                            _ClfsSetData = 0x48;
                            break;
                        case CLDC_Comm.Enum.Cus_Clfs.三元件跨相90:
                            _ClfsSetData = 0x44;
                            break;
                        case CLDC_Comm.Enum.Cus_Clfs.二元件跨相90:
                            _ClfsSetData = 0x42;
                            break;
                        case CLDC_Comm.Enum.Cus_Clfs.二元件跨相60:
                            _ClfsSetData = 0x41;
                            break;
                        default:
                            _ClfsSetData = 0x08;
                            break;
                    }
                }
                else
                {
                    switch (_Clfs)
                    {
                        case CLDC_Comm.Enum.Cus_Clfs.三相四线:                        
                            _ClfsSetData = 0x88;
                            break;
                        case CLDC_Comm.Enum.Cus_Clfs.三相三线:
                            _ClfsSetData = 0xC8;
                            break;
                        case CLDC_Comm.Enum.Cus_Clfs.三元件跨相90:
                            _ClfsSetData = 0xC4;
                            break;
                        case CLDC_Comm.Enum.Cus_Clfs.二元件跨相90:
                            _ClfsSetData = 0xC2;
                            break;
                        case CLDC_Comm.Enum.Cus_Clfs.二元件跨相60:
                            _ClfsSetData = 0xC1;
                            break;
                        default:
                            _ClfsSetData = 0x88;
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
            buf.Put(0x09);
            buf.Put(0x20);
            buf.Put(_ClfsSetData);
            buf.Put(0x11);
            buf.Put(_YouGongSetData);
            buf.Put(0x00);
            buf.Put(0x01);
            return buf.ToByteArray();
        }
    }
}
