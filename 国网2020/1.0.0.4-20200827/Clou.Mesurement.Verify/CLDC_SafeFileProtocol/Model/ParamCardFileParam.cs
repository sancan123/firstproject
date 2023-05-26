using System;

namespace CLDC_SafeFileProtocol.Model
{
    public class ParamCardFileParam
    {
        private byte _standby1 = 0x00;
        public string standby1
        {
            get { return "00"; }
        }
        private byte _updateFlag = 0x00;
        /// <summary>
        /// 参数更新标志位，bit0=1 更新当前套费率；bit1=1 更新备用套费率;bit2=1 更新当前套阶梯;bit3=1 更新备用套阶梯;bit7=1 更新其它参数.
        /// </summary>
        public string updateFlag
        {
            get
            {
                return Convert.ToString(_updateFlag, 16).PadLeft(2, '0');
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 2)
                {
                    _updateFlag = byte.Parse(value, System.Globalization.NumberStyles.HexNumber);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        private byte[] _standby2 = new byte[4];
        public string standby2
        {
            get
            {
                return "00000000";
            }
        }
        private byte[] _RateChangeTime = new byte[5];
        public string RateChangeTime
        {
            get
            {
                return BitConverter.ToString(_RateChangeTime).Replace("-", "");
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length==10)
                {
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _RateChangeTime = tdc.stringToByte(value);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        private byte _standby3 = 0x00;
        public string standby3
        {
            get
            {
                return "00";
            }
        }
        private byte[] _warningMoney1 = new byte[4];
        /// <summary>
        /// xxxxxx.xx
        /// </summary>
        public string warningMoney1
        {
            get
            {
                string tmp = BitConverter.ToString(_warningMoney1).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 2) + "." + tmp.Substring(tmp.Length - 2);
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _warningMoney1 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        private byte[] _warningMoney2 = new byte[4];
        /// <summary>
        /// xxxxxx.xx
        /// </summary>
        public string warningMoney2
        {
            get
            {
                string tmp = BitConverter.ToString(_warningMoney2).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 2) + "." + tmp.Substring(tmp.Length - 2);
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _warningMoney2 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        private byte[] _currentRate = new byte[3];
        public string currentRate
        {
            get
            {
                return BitConverter.ToString(_currentRate).Replace("-", "");
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length == 6)
                {
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _currentRate = tdc.stringToByte(value);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        private byte[] _voltageRate = new byte[3];
        public string voltageRate
        {
            get
            {
                return BitConverter.ToString(_voltageRate).Replace("-", "");
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length == 6)
                {
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _voltageRate = tdc.stringToByte(value);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }

        public byte[] GetData()
        {
            byte[] tmp = new byte[26];
            tmp[0] = _standby1;
            tmp[1] = _updateFlag;
            Array.Copy(_standby2, 0, tmp, 2, 4);
            Array.Copy(_RateChangeTime, 0, tmp, 6, 5);
            tmp[11] = _standby3;
            Array.Copy(_warningMoney1, 0, tmp, 12, 4);
            Array.Copy(_warningMoney2, 0, tmp, 16, 4);
            Array.Copy(_currentRate, 0, tmp, 20, 3);
            Array.Copy(_voltageRate, 0, tmp, 23, 3);
            return tmp;
        }

    }
}
