namespace CLDC_SafeFileProtocol
{
    /// <summary>
    /// 读写卡器接口
    /// </summary>
    public interface ICardControlProtocol
    {
        /// <summary>
        /// 设置数据端口名称
        /// </summary>
        /// <param name="szPortName">数据端口名称</param>
        void SetPortName(string szPortName);
        /// <summary>
        /// 设置数据端口波特率
        /// </summary>
        /// <param name="szSetting">数据端口波特率</param>
        void SetPortSetting(string szSetting);
        /// <summary>
        /// 初始化复位
        /// </summary>
        /// <returns></returns>
        int ResetDevice(int meterIndex);

        /// <summary>
        /// 读用户卡卡号
        /// </summary>
        /// <param name="cardNum"></param>
        /// <returns></returns>
        int ReadUserCardNum(out string cardNum);
        /// <summary>
        /// 读参数预置卡卡号
        /// </summary>
        /// <param name="cardNum"></param>
        /// <returns></returns>
        int ReadParamPresetCardNum(out string cardNum);
        /// <summary>
        /// 读用户卡数据
        /// </summary>
        /// <param name="fileParamData">参数文件数据域,数据项逗号","分隔</param>
        /// <param name="fileMoneyData"></param>
        /// <param name="filePrice1Data"></param>
        /// <param name="filePrice2Data"></param>
        /// <param name="fileReplyData"></param>
        /// <param name="enfileControlData"></param>
        /// <returns></returns>
        int ReadUserCard(out string fileParamData, out string fileMoneyData, out string filePrice1Data, out string filePrice2Data, out string fileReplyData, out string enfileControlData);
        /// <summary>
        /// 读参数预置卡
        /// </summary>
        /// <param name="fileParamData">参数文件数据域</param>
        /// <param name="fileMoneyData"></param>
        /// <param name="filePrice1Data"></param>
        /// <param name="filePrice2Data"></param>
        /// <returns></returns>
        int ReadParamPresetCard(out string fileParamData, out string fileMoneyData, out string filePrice1Data, out string filePrice2Data);

        /// <summary>
        /// 写用户卡
        /// </summary>
        /// <param name="fileParam">参数文件</param>
        /// <param name="fileMoney"></param>
        /// <param name="filePrice1"></param>
        /// <param name="filePrice2"></param>
        /// <param name="fileReply"></param>
        /// <param name="fileControl"></param>
        /// <returns>0</returns>
        int WriteUserCard(string fileParam, string fileMoney, string filePrice1, string filePrice2, string fileReply,string fileControl);
        /// <summary>
        /// 写参数预置卡
        /// </summary>
        /// <param name="fileParam"></param>
        /// <param name="fileMoney"></param>
        /// <param name="filePrice1"></param>
        /// <param name="filePrice2"></param>
        /// <returns></returns>
        int WriteParamPresetCard(string fileParam, string fileMoney, string filePrice1, string filePrice2);

        /// <summary>
        /// 带MAC读用户卡
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="fileParam"></param>
        /// <param name="fileMoney"></param>
        /// <param name="filePrice1"></param>
        /// <param name="filePrice2"></param>
        /// <param name="fileControl"></param>
        /// <returns></returns>
        int ReadUserCardMAC(string rand, out string fileParam, out string fileMoney, out string filePrice1, out string filePrice2, out string fileControl);

        /// <summary>
        /// 单独写返写信息文件
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="fileReply"></param>
        /// <returns></returns>
        int WriteUserCardReply(string fileReply);

        /// <summary>
        /// 读交互终端到卡片的信息    07A002FF
        /// </summary>
        /// <param name="strRnd"></param>
        /// <param name="CardNo"></param>
        /// <returns></returns>
        int ReadTerminalToCardInfo(string CardNo, string strRand, out string parainfo);

        /// <summary>
        /// 单独写返写备用套电价文件
        /// </summary>
        /// <param name="fileprice2Reply"></param>
        /// <returns></returns>
        int WriteUserCardReplyPrice2(string fileprice2Reply);

        /// <summary>
        /// 读交互终端到卡片的当前套电价信息          07A003FF
        /// </summary>
        /// <param name="CardNo"></param>
        /// <param name="strRand"></param>
        /// <param name="price2"></param>
        /// <returns></returns>
        int ReadTerminalToCardPrice1(string CardNo, string strRand, out string price1);

        /// <summary>
        /// 读交互终端到卡片的备用套电价信息          07A004FF
        /// </summary>
        /// <param name="CardNo"></param>
        /// <param name="strRand"></param>
        /// <param name="price2"></param>
        /// <returns></returns>
        int ReadTerminalToCardPrice2(string CardNo, string strRand, out string price2);

    }
}
