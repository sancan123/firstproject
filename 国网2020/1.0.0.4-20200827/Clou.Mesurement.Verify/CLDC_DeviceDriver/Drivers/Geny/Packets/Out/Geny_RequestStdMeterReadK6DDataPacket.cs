using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{


    /// <summary>
    /// 读取 标准表 数据包
    /// </summary>
    class Geny_RequestStdMeterReadK6DDataPacket : Geny_RequestStdMeterPacket
    {
        /// <summary>
        /// 数据类型
        /// 请使用枚举值
        /// HX 表示谐波
        /// DATA 标准表数据
        /// </summary>
        public Geny_StandMeterDataType DataType
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="phase">如果是读取 data ,则phase 参数不使用</param>
        public Geny_RequestStdMeterReadK6DDataPacket(string stdmeterType, Geny_StandMeterDataType dataType)
            : base(stdmeterType)
        {
            this.DataType = dataType;
            if (this.DataType == Geny_StandMeterDataType.Non)
            {
                this.IsNeedReturn = false;
            }
            else
            {
                this.IsNeedReturn = true;
            }
        }


        /// <summary>
        /// 已重写，
        /// 返回数据
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {
            //byte[] data = Encoding.ASCII.GetBytes(DataType.ToString().PadRight(7, ' '));

            return Encoding.ASCII.GetBytes(DataType.ToString().PadRight(7, ' '));
        }
    }
}
