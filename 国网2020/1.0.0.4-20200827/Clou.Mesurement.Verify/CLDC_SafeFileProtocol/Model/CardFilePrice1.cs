using System;
using System.Collections.Generic;

namespace CLDC_SafeFileProtocol.Model
{
    public class CardFilePrice1
    {

        private byte[] _rate1 = new byte[4];
        private byte[] _rate2 = new byte[4];
        private byte[] _rate3 = new byte[4];
        private byte[] _rate4 = new byte[4];
        private byte[] _rate5 = new byte[4];
        private byte[] _rate6 = new byte[4];
        private byte[] _rate7 = new byte[4];
        private byte[] _rate8 = new byte[4];
        private byte[] _rate9 = new byte[4];
        private byte[] _rate10 = new byte[4];
        private byte[] _rate11 = new byte[4];
        private byte[] _rate12 = new byte[4];
        
        private byte[] _step1Value1 = new byte[4];
        private byte[] _step1Value2 = new byte[4];
        private byte[] _step1Value3 = new byte[4];
        private byte[] _step1Value4 = new byte[4];
        private byte[] _step1Value5 = new byte[4];
        private byte[] _step1Value6 = new byte[4];
        private byte[] _step1Price1 = new byte[4];
        private byte[] _step1Price2 = new byte[4];
        private byte[] _step1Price3 = new byte[4];
        private byte[] _step1Price4 = new byte[4];
        private byte[] _step1Price5 = new byte[4];
        private byte[] _step1Price6 = new byte[4];
        private byte[] _step1Price7 = new byte[4];
        private byte[] _priceDay1 = new byte[3];
        private byte[] _priceDay2 = new byte[3];
        private byte[] _priceDay3 = new byte[3];
        private byte[] _priceDay4 = new byte[3];
        private byte[] _priceDay5 = new byte[3];
        private byte[] _priceDay6 = new byte[3];

        private byte[] _step2Value1 = new byte[4];
        private byte[] _step2Value2 = new byte[4];
        private byte[] _step2Value3 = new byte[4];
        private byte[] _step2Value4 = new byte[4];
        private byte[] _step2Value5 = new byte[4];
        private byte[] _step2Value6 = new byte[4];
        private byte[] _step2Price1 = new byte[4];
        private byte[] _step2Price2 = new byte[4];
        private byte[] _step2Price3 = new byte[4];
        private byte[] _step2Price4 = new byte[4];
        private byte[] _step2Price5 = new byte[4];
        private byte[] _step2Price6 = new byte[4];
        private byte[] _step2Price7 = new byte[4];
        private byte[] _price2Day1 = new byte[3];
        private byte[] _price2Day2 = new byte[3];
        private byte[] _price2Day3 = new byte[3];
        private byte[] _price2Day4 = new byte[3];
        private byte[] _price2Day5 = new byte[3];
        private byte[] _price2Day6 = new byte[3];

        private byte[] _standby1 = new byte[5];

        public string rate1
        {
            get
            {
                string tmp = BitConverter.ToString(_rate1).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _rate1 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string rate2
        {
            get
            {
                string tmp = BitConverter.ToString(_rate2).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _rate2 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string rate3
        {
            get
            {
                string tmp = BitConverter.ToString(_rate3).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _rate3 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string rate4
        {
            get
            {
                string tmp = BitConverter.ToString(_rate4).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _rate4 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string rate5
        {
            get
            {
                string tmp = BitConverter.ToString(_rate5).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _rate5 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string rate6
        {
            get
            {
                string tmp = BitConverter.ToString(_rate6).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _rate6 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string rate7
        {
            get
            {
                string tmp = BitConverter.ToString(_rate7).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _rate7 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string rate8
        {
            get
            {
                string tmp = BitConverter.ToString(_rate8).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _rate8 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string rate9
        {
            get
            {
                string tmp = BitConverter.ToString(_rate9).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _rate9 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string rate10
        {
            get
            {
                string tmp = BitConverter.ToString(_rate10).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _rate10 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string rate11
        {
            get
            {
                string tmp = BitConverter.ToString(_rate11).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _rate11 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string rate12
        {
            get
            {
                string tmp = BitConverter.ToString(_rate12).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _rate12 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string priceDay1
        {
            get
            {
                return BitConverter.ToString(_priceDay1).Replace("-", "");
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length == 6)
                {
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _priceDay1 = tdc.stringToByte(value);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string priceDay2
        {
            get
            {
                return BitConverter.ToString(_priceDay2).Replace("-", "");
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length == 6)
                {
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _priceDay2 = tdc.stringToByte(value);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string priceDay3
        {
            get
            {
                return BitConverter.ToString(_priceDay3).Replace("-", "");
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length == 6)
                {
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _priceDay3 = tdc.stringToByte(value);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string priceDay4
        {
            get
            {
                return BitConverter.ToString(_priceDay4).Replace("-", "");
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length == 6)
                {
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _priceDay4 = tdc.stringToByte(value);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string priceDay5
        {
            get
            {
                return BitConverter.ToString(_priceDay5).Replace("-", "");
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length == 6)
                {
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _priceDay5 = tdc.stringToByte(value);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string priceDay6
        {
            get
            {
                return BitConverter.ToString(_priceDay6).Replace("-", "");
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length == 6)
                {
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _priceDay6 = tdc.stringToByte(value);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step1Value1
        {
            get
            {
                string tmp = BitConverter.ToString(_step1Value1).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 2) + "." + tmp.Substring(tmp.Length - 2);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step1Value1 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step1Value2
        {
            get
            {
                string tmp = BitConverter.ToString(_step1Value2).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 2) + "." + tmp.Substring(tmp.Length - 2);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step1Value2 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step1Value3
        {
            get
            {
                string tmp = BitConverter.ToString(_step1Value3).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 2) + "." + tmp.Substring(tmp.Length - 2);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step1Value3 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step1Value4
        {
            get
            {
                string tmp = BitConverter.ToString(_step1Value4).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 2) + "." + tmp.Substring(tmp.Length - 2);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step1Value4 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step1Value5
        {
            get
            {
                string tmp = BitConverter.ToString(_step1Value5).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 2) + "." + tmp.Substring(tmp.Length - 2);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step1Value5 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step1Value6
        {
            get
            {
                string tmp = BitConverter.ToString(_step1Value6).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 2) + "." + tmp.Substring(tmp.Length - 2);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step1Value6 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step1Price1
        {
            get
            {
                string tmp = BitConverter.ToString(_step1Price1).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step1Price1 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step1Price2
        {
            get
            {
                string tmp = BitConverter.ToString(_step1Price2).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step1Price2 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step1Price3
        {
            get
            {
                string tmp = BitConverter.ToString(_step1Price3).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step1Price3 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step1Price4
        {
            get
            {
                string tmp = BitConverter.ToString(_step1Price4).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step1Price4 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step1Price5
        {
            get
            {
                string tmp = BitConverter.ToString(_step1Price5).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step1Price5 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step1Price6
        {
            get
            {
                string tmp = BitConverter.ToString(_step1Price6).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step1Price6 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step1Price7
        {
            get
            {
                string tmp = BitConverter.ToString(_step1Price7).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step1Price7 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step2Value1
        {
            get
            {
                string tmp = BitConverter.ToString(_step2Value1).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 2) + "." + tmp.Substring(tmp.Length - 2);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step2Value1 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step2Value2
        {
            get
            {
                string tmp = BitConverter.ToString(_step2Value2).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 2) + "." + tmp.Substring(tmp.Length - 2);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step2Value2 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step2Value3
        {
            get
            {
                string tmp = BitConverter.ToString(_step2Value3).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 2) + "." + tmp.Substring(tmp.Length - 2);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step2Value3 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step2Value4
        {
            get
            {
                string tmp = BitConverter.ToString(_step2Value4).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 2) + "." + tmp.Substring(tmp.Length - 2);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step2Value4 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step2Value5
        {
            get
            {
                string tmp = BitConverter.ToString(_step2Value5).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 2) + "." + tmp.Substring(tmp.Length - 2);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step2Value5 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step2Value6
        {
            get
            {
                string tmp = BitConverter.ToString(_step2Value6).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 2) + "." + tmp.Substring(tmp.Length - 2);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step2Value6 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step2Price1
        {
            get
            {
                string tmp = BitConverter.ToString(_step2Price1).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step2Price1 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step2Price2
        {
            get
            {
                string tmp = BitConverter.ToString(_step2Price2).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step2Price2 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step2Price3
        {
            get
            {
                string tmp = BitConverter.ToString(_step2Price3).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step2Price3 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step2Price4
        {
            get
            {
                string tmp = BitConverter.ToString(_step2Price4).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step2Price4 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step2Price5
        {
            get
            {
                string tmp = BitConverter.ToString(_step2Price5).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step2Price5 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step2Price6
        {
            get
            {
                string tmp = BitConverter.ToString(_step2Price6).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step2Price6 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string step2Price7
        {
            get
            {
                string tmp = BitConverter.ToString(_step2Price7).Replace("-", "");
                return tmp.Substring(0, tmp.Length - 4) + "." + tmp.Substring(tmp.Length - 4);
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length <= 9)
                {
                    string tmp = value.Replace(".", "");
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _step2Price7 = tdc.stringToByte(tmp);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string price2Day1
        {
            get
            {
                return BitConverter.ToString(_priceDay1).Replace("-", "");
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length == 6)
                {
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _priceDay1 = tdc.stringToByte(value);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string price2Day2
        {
            get
            {
                return BitConverter.ToString(_priceDay2).Replace("-", "");
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length == 6)
                {
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _priceDay2 = tdc.stringToByte(value);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string price2Day3
        {
            get
            {
                return BitConverter.ToString(_priceDay3).Replace("-", "");
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length == 6)
                {
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _priceDay3 = tdc.stringToByte(value);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string price2Day4
        {
            get
            {
                return BitConverter.ToString(_priceDay4).Replace("-", "");
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length == 6)
                {
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _priceDay4 = tdc.stringToByte(value);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string price2Day5
        {
            get
            {
                return BitConverter.ToString(_priceDay5).Replace("-", "");
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length == 6)
                {
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _priceDay5 = tdc.stringToByte(value);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string price2Day6
        {
            get
            {
                return BitConverter.ToString(_priceDay6).Replace("-", "");
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length == 6)
                {
                    Tools.DataConvert tdc = new Tools.DataConvert();
                    _priceDay6 = tdc.stringToByte(value);
                }
                else
                {
                    throw new Exception("数据格式错误!");
                }
            }
        }
        public string standby1
        {
            get
            {
                return BitConverter.ToString(_standby1).Replace("-", "");
            }
        }

        public byte[] GetData()
        {
            List<byte> lst_tmp = new List<byte>();
            lst_tmp.AddRange(_rate1);
            lst_tmp.AddRange(_rate2);
            lst_tmp.AddRange(_rate3);
            lst_tmp.AddRange(_rate4);
            lst_tmp.AddRange(_rate5);
            lst_tmp.AddRange(_rate6);
            lst_tmp.AddRange(_rate7);
            lst_tmp.AddRange(_rate8);
            lst_tmp.AddRange(_rate9);
            lst_tmp.AddRange(_rate10);
            lst_tmp.AddRange(_rate11);
            lst_tmp.AddRange(_rate12);
            
            lst_tmp.AddRange(_step1Value1);
            lst_tmp.AddRange(_step1Value2);
            lst_tmp.AddRange(_step1Value3);
            lst_tmp.AddRange(_step1Value4);
            lst_tmp.AddRange(_step1Value5);
            lst_tmp.AddRange(_step1Value6);
            lst_tmp.AddRange(_step1Price1);
            lst_tmp.AddRange(_step1Price2);
            lst_tmp.AddRange(_step1Price3);
            lst_tmp.AddRange(_step1Price4);
            lst_tmp.AddRange(_step1Price5);
            lst_tmp.AddRange(_step1Price6);
            lst_tmp.AddRange(_step1Price7);
            lst_tmp.AddRange(_priceDay1);
            lst_tmp.AddRange(_priceDay2);
            lst_tmp.AddRange(_priceDay3);
            lst_tmp.AddRange(_priceDay4);
            lst_tmp.AddRange(_priceDay5);
            lst_tmp.AddRange(_priceDay6);

            lst_tmp.AddRange(_step2Value1);
            lst_tmp.AddRange(_step2Value2);
            lst_tmp.AddRange(_step2Value3);
            lst_tmp.AddRange(_step2Value4);
            lst_tmp.AddRange(_step2Value5);
            lst_tmp.AddRange(_step2Value6);
            lst_tmp.AddRange(_step2Price1);
            lst_tmp.AddRange(_step2Price2);
            lst_tmp.AddRange(_step2Price3);
            lst_tmp.AddRange(_step2Price4);
            lst_tmp.AddRange(_step2Price5);
            lst_tmp.AddRange(_step2Price6);
            lst_tmp.AddRange(_step2Price7);
            lst_tmp.AddRange(_price2Day1);
            lst_tmp.AddRange(_price2Day2);
            lst_tmp.AddRange(_price2Day3);
            lst_tmp.AddRange(_price2Day4);
            lst_tmp.AddRange(_price2Day5);
            lst_tmp.AddRange(_price2Day6);

            lst_tmp.AddRange(_standby1);
            return lst_tmp.ToArray();
        }
    }
}
