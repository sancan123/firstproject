using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.In
{

    /// <summary>
    /// 误差板，数据块读回复包
    /// 包括 6 个表位的数据
    /// </summary>
    class Geny_RequestReadWcbBlockReplyPacket : GenyRecvPacket
    {
        public stError[] Errors { get; private set; }

        /// <summary>
        /// 该值确定要解析的数据类型
        /// </summary>
        public WorkFlow WorkFlow
        {
            get;
            set;
        }

        public Geny_RequestReadWcbBlockReplyPacket(WorkFlow workFlow)
            : base()
        {
            this.WorkFlow = workFlow;
        }

        string[] SpiltString(string value)
        {
            try
            {
                List<string> ss = new List<string>();
                for (int i = 0; i < value.Length && value.Length - i >= 10; i += 10)
                {
                    ss.Add(value.Substring(i, 10));
                }
                return ss.ToArray();
            }
            catch (Exception ex)
            {
                int i = 3243;
                return null;
            }
        }

        protected override void ParseData(string s)
        {
            try
            {
                this.ParseDataImpl(s);
            }
            catch (Exception ex)
            {
                this.Errors = new stError[6];
            }
        }

        private void ParseDataImpl(string s)
        {
            string[] arrData = SpiltString(s);
            stError[] tagError = new stError[6];
            for (int i = 0; i < 6; i++)
            {
                tagError[i].MeterIndex = (byte)arrData[i][0];
                tagError[i].Index = 2;
                if (Char.IsDigit(arrData[i][1]) == true)
                {
                    tagError[i].Index = byte.Parse(arrData[i][1].ToString());
                }
                if (WorkFlow == WorkFlow.对色标)
                {
                    if (arrData[i].IndexOf("off", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        tagError[i].szError = "1";
                    }
                    else
                    {
                        tagError[i].szError = "0";
                    }
                    //如果台体不支持对色标，则默认返回成功，以便应用程序继续运行
                    if (Geny.Setting.Settings.SiBiaoType == GenySeBiaoType.NotSupport)
                    {
                        tagError[i].szError = "1";
                    }
                }
                else if (WorkFlow == WorkFlow.潜动 || WorkFlow == WorkFlow.启动)
                {
                    if (arrData[i].IndexOf("pass", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        tagError[i].szError = "1";
                    }
                    else if (arrData[i].IndexOf("fail", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        tagError[i].szError = "0";
                    }
                    else
                    {
                        tagError[i].szError = "busy";
                    }
                }
                else if (WorkFlow == WorkFlow.基本误差)
                {
                    tagError[i].Index = Geny_ReplyReadWCBDataPacket.GetErrorTimes(arrData[i]);
                    tagError[i].szError = Geny_ReplyReadWCBDataPacket.GetSubStringWithDigit(arrData[i]);
                }
            }
            this.Errors = tagError;
            this.ReciveResult = CLDC_Comm.SocketModule.Packet.RecvResult.OK;
        }
    }
}
