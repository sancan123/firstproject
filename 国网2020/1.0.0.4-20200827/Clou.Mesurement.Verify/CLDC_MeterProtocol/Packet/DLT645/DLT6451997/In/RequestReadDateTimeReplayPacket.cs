using System;
using System.Collections.Generic;
using System.Text;
using MeterProtocol.Packet.DLT645;

namespace MeterProtocol.Packet.DLT645.DLT6451997.In
{
    class RequestReadDateTimeReplayPacket : DL645RecvPacket
    {
        /// <summary>
        /// 读取到的电表时间
        /// </summary>
        public DateTime MeterTime { get; set; }
        /// <summary>
        /// 日期格式
        /// </summary>
        public string DateTimeFormat { get; set; }
        protected override bool ParseBody(byte[] buf)
        {
            Array.Reverse(buf);//反转数据
            string str_Tmp = BitConverter.ToString(buf, 0, buf.Length - 2).Replace("-", "");
            str_Tmp = str_Tmp.TrimStart(new char[] { 'A' });        //去掉AA
            str_Tmp = str_Tmp.TrimEnd(new char[] { 'A' });
            str_Tmp = str_Tmp.Substring(6, 6) + str_Tmp.Substring(0, 6);
            string[] str_Para = new string[] { "YY", "MM", "DD ", "HH", "FF", "SS" };
            for (int int_Inc = 0; int_Inc < 6; int_Inc++)
            {
                int int_Index = DateTimeFormat.IndexOf(str_Para[int_Inc]);
                str_Para[int_Inc] = str_Tmp.Substring(int_Index, 2);
            }
            string str_DateTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}"
                    , str_Para[0]
                    , str_Para[1]
                    , str_Para[2]
                    , str_Para[3]
                    , str_Para[4]
                    , str_Para[5]
                    );
            return true;
        }
    }
}
