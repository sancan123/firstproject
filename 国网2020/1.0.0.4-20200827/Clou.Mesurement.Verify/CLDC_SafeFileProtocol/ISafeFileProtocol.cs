namespace CLDC_SafeFileProtocol
{
    public interface ISafeFileProtocol
    {
        /// <summary>
        /// 用户卡 参数信息文件(45字节)
        /// </summary>
        /// <param name="Params">数组长度12，依次为：00，参数更新标志位（1字节），00000000，两套分时费率切换时间（5字节，年月日时分），00，报警金额1（4字节，XXXXXX.XX），报警金额2，电流互感器变比，电压互感器变比，表号，客户编号，用户卡类型</param>
        /// <param name="OutFile">文件格式</param>
        /// <returns>0 成功</returns>
        int GetUserCardFileParam(string[] Params,out string OutFile);
        /// <summary>
        /// 用户卡 购电信息文件(8字节)
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>
        int GetUserCardFileMoney(string[] Params, out string OutFile);
        /// <summary>
        /// 用户卡 当前套电价文件(199字节)
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>
        int GetUserCardFilePrice1(string[] Params, out string OutFile);
        /// <summary>
        /// 用户卡 备用套电价文件(199字节)
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>
        int GetUserCardFilePrice2(string[] Params, out string OutFile);
        /// <summary>
        /// 用户卡 合闸命令文件明文(8字节)
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>
        int GetUserCardFileControl(string[] Params, out string OutFile);


        /// <summary>
        /// 参数预置卡 参数信息文件(32字节)
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>
        int GetParamCardFileParam(string[] Params, out string OutFile);
        /// <summary>
        /// 参数预置卡 购电信息文件(8字节)
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>
        int GetParamCardFileMoney(string[] Params, out string OutFile);
        /// <summary>
        /// 参数预置卡 当前套电价文件(199字节)
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>
        int GetParamCardFilePrice1(string[] Params, out string OutFile);
        /// <summary>
        /// 参数预置卡 备用套电价文件(199字节)
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>
        int GetParamCardFilePrice2(string[] Params, out string OutFile);


    }
}
