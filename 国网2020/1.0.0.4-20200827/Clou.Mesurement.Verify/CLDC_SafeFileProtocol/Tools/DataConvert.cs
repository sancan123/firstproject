namespace CLDC_SafeFileProtocol.Tools
{
    class DataConvert
    {
        /// <summary>
        /// HEX字符串转字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public byte[] stringToByte(string hexString)
        {
            int DLen = hexString.Length / 2;
            byte[] DataFrame = new byte[DLen];
            for (int i = DLen - 1; i >= 0; i--)
            {
                string bb = hexString.Substring(i * 2, 2);
                DataFrame[i] = byte.Parse(bb, System.Globalization.NumberStyles.HexNumber);
            }
            return DataFrame;

        }
        /// <summary>
        /// HEX字符串转字节数组,并反转
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public byte[] stringToByteRe(string hexString)
        {
            int DLen = hexString.Length / 2;
            byte[] DataFrame = new byte[DLen];
            for (int i = DLen - 1; i >= 0; i--)
            {
                string bb = hexString.Substring(i * 2, 2);
                DataFrame[DLen - 1 - i] = byte.Parse(bb, System.Globalization.NumberStyles.HexNumber);
            }
            return DataFrame;

        }
        /// <summary>
        /// 数组翻转
        /// </summary>
        /// <param name="dData"></param>
        /// <returns></returns>
        private byte[] bytesReserve(byte[] bData)
        {
            if (bData.Length <= 0 || bData == null)
            {
                return null;
            }
            byte[] tdata = new byte[bData.Length];
            for (int i = bData.Length - 1; i >= 0; i--)
            {
                tdata[bData.Length - 1 - i] = bData[i];
            }
            return tdata;
        }

    }
}
