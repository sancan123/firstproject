using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.In
{

    /// <summary>
    /// 读取误差板的 返回值包基类
    /// </summary>
    class Geny_ReplyReadWCBDataPacket : GenyRecvPacket
    {
        static char[] chars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        /// <summary>
        /// 误差次数
        /// </summary>
        public byte ErrorTimes
        {
            get;
            set;
        }

        /// <summary>
        /// 误差数据
        /// </summary>
        public string Data
        {
            get;
            set;
        }

        public WorkFlow WorkFlow
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workFlow"></param>
        public Geny_ReplyReadWCBDataPacket(WorkFlow workFlow)
        {
            this.WorkFlow = workFlow;
        }

        protected override void ParseData(string s)
        {
            switch (WorkFlow)
            {
                case WorkFlow.对色标:
                    {
                        if (!string.IsNullOrEmpty(s) && (s.IndexOf("on") != -1 || s.IndexOf("off") != -1))
                            Data = "1";
                        else
                            Data = "0";
                    }
                    break;
                case WorkFlow.启动:
                case WorkFlow.潜动:
                    {
                        if (!string.IsNullOrEmpty(s) && s.IndexOf("pass") != -1)
                            Data = "1";
                        else
                            Data = "0";
                    }
                    break;
                case WorkFlow.基本误差:
                    {
                        s = GetSubStringWithDigit(s);
                        ErrorTimes = byte.Parse(s[0].ToString());
                        Data = s.Substring(1);
                    }
                    break;
            }

            ReciveResult = CLDC_Comm.SocketModule.Packet.RecvResult.OK;
        }

        /// <summary>
        /// 将内部数转换成 误差类
        /// </summary>
        /// <returns></returns>
        public stError ToSTError()
        {
            stError error = new stError();
            error.Index = ErrorTimes;
            error.szError = Data;

            return error;
        }

        /// <summary>
        /// 获取 从数字开关的 字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetSubStringWithDigit(string value)
        {
            value = value.Trim();
            int i = value.IndexOfAny(chars);

            //除去 头部可能的非数字字符
            if (i > 0)
            {
                value = value.Substring(i);
            }

            i = value.LastIndexOfAny(chars);

            if (i < value.Length - 1)
            {
                value = value.Substring(0, i + 1);
            }

            i = value.IndexOfAny(new char[] { '-', '+' });
            if (i > 0)
            {
                value = value.Substring(i + 1);
            }

            foreach (char a in value)
            {
                if (char.IsDigit(a) == false && a !='.')
                {
                    return "";
                }
            }

            return value;
        }

        public static byte GetErrorTimes(string s)
        {
            byte errTimes = 0;

            int index = s.IndexOfAny(new char[] { '-', '+' });
            if (index>=0&& char.IsDigit(s[index - 1]))
            {
                errTimes = byte.Parse(s[index - 1].ToString());
            }

            return errTimes;
        }
    }
}
