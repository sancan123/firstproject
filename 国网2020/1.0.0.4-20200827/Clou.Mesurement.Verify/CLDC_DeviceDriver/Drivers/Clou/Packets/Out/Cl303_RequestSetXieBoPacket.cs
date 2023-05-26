using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 设置谐波请求包[0x32]
    /// </summary>
    internal class Cl303_RequestSetXieBoPacket : CL303SendPacket
    {
        /// <summary>
        /// 是否加谐波
        /// </summary>
        public bool AddXieBo { get; set; }
        /// <summary>
        /// 当前加谐波的元件
        ///  A相电压 = 0,
        ///B相电压 = 1,
        ///C相电压 = 2,
        ///A相电流 = 3,
        ///B相电流 = 4,
        ///C相电流 = 5
        /// </summary>
        public byte YuanJian { get; set; }
        float[] content = new float[64];
        float[] phase = new float[64];
        public Cl303_RequestSetXieBoPacket()
        {
            AddXieBo = true;
        }

        public void SetPara(float[] contents, float[] phases)
        {
            content = contents;
            phase = phases;
        }

        protected override byte[] GetBody()
        {
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();
            buf.SetLength(424);
            buf.Position = 0;       
            buf.Put(0x32);                      //CMD
            buf.Put(YuanJian);                  //当前相位
            buf.PutUShort(0xFFFF);              //波开关
            buf.Put(0xFF);                      //含量状态
            for (int i = 0; i < 21; i++)
            {
                byte[] bytData = To10Bit(content[i]);
                buf.Put(bytData);//含量
                bytData = To10Bit(phase[i]);
                buf.Put(bytData);//相位
            }
            return buf.ToByteArray();
        }

        /// <summary>
        /// 转换成10个Bit的值
        /// </summary>
        /// <param name="sValue">转换值</param>
        /// <returns></returns>
        private byte[] To10Bit(Single sng_Value)
        {
            string sData = Convert.ToString(sng_Value);
            if (sData.IndexOf('.') <= 0) sData += ".";
            sData += "0000000000";
            sData = sData.Substring(0, 9);
            byte[] bPara = ASCIIEncoding.ASCII.GetBytes(sData);
            Array.Resize(ref bPara, bPara.Length + 1);
            bPara[9] = 48;          //Convert.ToByte((sng_Value - Math.Floor(sng_Value)) == 0 ? 46 : 48);
            return bPara;
        }

    }
}
