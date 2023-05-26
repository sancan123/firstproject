using System;

namespace CLDC_SafeFileProtocol.Model
{
    public class UserCardFileMoney
    {

        private byte[] _buyMoney = new byte[4];
        public string buyMoney
        {
            get
            {
                string tmp = BitConverter.ToString(_buyMoney).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 2) + "." + tmp.Substring(tmp.Length - 2);
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _buyMoney = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        private byte[] _buyCount = new byte[4];
        public string buyCount
        {
            get
            {
                return BitConverter.ToString(_buyCount).Replace("-", "");
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _buyCount = tdc.stringToByte(value);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }

        public byte[] GetData()
        {
            byte[] tmp = new byte[8];
            Array.Copy(_buyMoney, 0, tmp, 0, 4);
            Array.Copy(_buyCount, 0, tmp, 4, 4);
            return tmp;
        }

    }
}
