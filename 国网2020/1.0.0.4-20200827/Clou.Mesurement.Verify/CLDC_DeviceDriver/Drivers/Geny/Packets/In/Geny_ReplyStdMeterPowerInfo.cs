using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.In
{
    class Geny_ReplyStdMeterPowerInfo : Geny_ReplyStdMeterPacket
    {

        private stStdInfo stdInfo = new stStdInfo();

        public stStdInfo StdInfo
        {
            get
            {
                return stdInfo;
            }
        }

        protected override void ParseData(string s)
        {
            try
            {
                this.ParseDataImpl(s);
                this.ReciveResult = CLDC_Comm.SocketModule.Packet.RecvResult.OK;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                this.ReciveResult = CLDC_Comm.SocketModule.Packet.RecvResult.DataError;
            }
        }

        private void ParseDataImpl(string s)
        {
            string retData = this.resultData;
            string[] arrData = retData.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (arrData.Length < 4)
            {
                this.ReciveResult = CLDC_Comm.SocketModule.Packet.RecvResult.DataError;
            }
            float[] fAData = DataFormart.ParseStringToFloat(arrData[1].Replace("A:", ""));
            float[] fBData = DataFormart.ParseStringToFloat(arrData[2].Replace("B:", ""));
            float[] fCData = DataFormart.ParseStringToFloat(arrData[3].Replace("C:", ""));

            // a 相值
            stdInfo.Ua = fAData[0];
            stdInfo.Ia = fAData[2];
            stdInfo.Pa = fAData[4];
            stdInfo.Qa = fAData[5];
            stdInfo.Sa = (float)Math.Sqrt(stdInfo.Pa * stdInfo.Pa + stdInfo.Qa * stdInfo.Qa);
            stdInfo.Phi_Ia = fAData[7];

            if (CLDC_Comm.GlobalUnit.IsDan == false)
            {
                // b相值
                stdInfo.Ub = fBData[0];
                stdInfo.Ib = fBData[2];
                stdInfo.Pb = fBData[4];
                stdInfo.Qb = fBData[5];
                stdInfo.Sb = (float)Math.Sqrt(stdInfo.Pb * stdInfo.Pb + stdInfo.Qb * stdInfo.Qb);
                stdInfo.Phi_Ib = fBData[7];

                //c 相值 
                stdInfo.Uc = fCData[0];
                stdInfo.Ic = fCData[2];
                stdInfo.Pc = fCData[4];
                stdInfo.Qc = fCData[5];
                stdInfo.Sc = (float)Math.Sqrt(stdInfo.Pc * stdInfo.Pc + stdInfo.Qc * stdInfo.Qc);
                stdInfo.Phi_Ic = fCData[7];
            }
            //解析 各种 累加值
            string[] strSumDatas = arrData[4].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < 5 && i < strSumDatas.Length; i++)
            {
                string str = strSumDatas[i];
                string[] tmpStrs = str.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                float value = float.Parse(tmpStrs[1]);
                if (tmpStrs[0].Equals("FEQ"))
                {
                    stdInfo.Freq = value;
                }
                else if (tmpStrs[0].Equals("PSUM"))
                {
                    stdInfo.P = value;
                    if (CLDC_Comm.GlobalUnit.IsDan)
                        stdInfo.P = stdInfo.P / 3;
                }
                else if (tmpStrs[0].Equals("QSUM"))
                {
                    stdInfo.Q = value;
                    if (CLDC_Comm.GlobalUnit.IsDan)
                        stdInfo.Q = stdInfo.Q / 3;
                }
                else if (tmpStrs[0].Equals("SSUM"))
                {
                    stdInfo.S = value;
                    if (CLDC_Comm.GlobalUnit.IsDan)
                        stdInfo.S = stdInfo.S / 3;
                }
                else if (tmpStrs[0].Equals("COSSUM"))
                {
                    stdInfo.COS = value;
                    if (CLDC_Comm.GlobalUnit.IsDan)
                        stdInfo.COS = stdInfo.COS / 3;
                }
                else
                {
                }
            }

        }
    }
}
