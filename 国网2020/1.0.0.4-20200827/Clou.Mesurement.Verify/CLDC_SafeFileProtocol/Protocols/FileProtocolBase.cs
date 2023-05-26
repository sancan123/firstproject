using System;

namespace CLDC_SafeFileProtocol.Protocols
{
    public class FileProtocolBase : ISafeFileProtocol
    {
        public virtual int GetParamCardFileMoney(string[] Params, out string OutFile)
        {
            throw new NotImplementedException();
        }

        public virtual int GetParamCardFileParam(string[] Params, out string OutFile)
        {
            throw new NotImplementedException();
        }

        public virtual int GetParamCardFilePrice1(string[] Params, out string OutFile)
        {
            throw new NotImplementedException();
        }

        public virtual int GetParamCardFilePrice2(string[] Params, out string OutFile)
        {
            throw new NotImplementedException();
        }

        public virtual int GetUserCardFileControl(string[] Params, out string OutFile)
        {
            throw new NotImplementedException();
        }

        public virtual int GetUserCardFileMoney(string[] Params, out string OutFile)
        {
            throw new NotImplementedException();
        }

        public virtual int GetUserCardFileParam(string[] Params, out string OutFile)
        {
            throw new NotImplementedException();
        }

        public virtual int GetUserCardFilePrice1(string[] Params, out string OutFile)
        {
            throw new NotImplementedException();
        }

        public virtual int GetUserCardFilePrice2(string[] Params, out string OutFile)
        {
            throw new NotImplementedException();
        }

        protected byte[] OrgFrame(byte byt_Cmd, byte[] byt_Data)
        {

            int byt_Len = byt_Data.Length;
            byte[] byt_Frame = new byte[byt_Len + 6];  //68H(1)+命令码(1)+Len(2)+Data(Len)+ChkSum(1)+16H(1)  
            
            byt_Frame[0] = 0x68;
            byt_Frame[1] = byt_Cmd;
            string str_Len = Convert.ToString(byt_Len, 16).PadLeft(4, '0');
            byt_Frame[2] = byte.Parse(str_Len.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byt_Frame[3] = byte.Parse(str_Len.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            for (int int_Inc = 0; int_Inc < byt_Len; int_Inc++)
            {
                byt_Frame[4 + int_Inc] = Convert.ToByte((byt_Data[int_Inc]) % 256);
            }
            for (int int_Inc = 0; int_Inc < byt_Len + 3; int_Inc++)
            {
                byt_Frame[4 + byt_Len] += Convert.ToByte(byt_Frame[1 + int_Inc] % 256);
            }
            byt_Frame[5 + byt_Len] = 0x16;
            //Console.WriteLine(BitConverter.ToString(byt_Frame));
            return byt_Frame;
        }

    }
}
