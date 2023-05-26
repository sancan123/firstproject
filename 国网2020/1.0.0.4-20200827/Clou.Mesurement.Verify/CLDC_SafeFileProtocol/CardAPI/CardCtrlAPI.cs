using System.Runtime.InteropServices;
using System.Text;

namespace CLDC_SafeFileProtocol.CardAPI
{
    /// <summary>
    /// 直接操作卡片API
    /// </summary>
    public class CardCtrlAPI
    {
        #region 读卡器
        /*******************************************************************************
* 函数名称  : OpenDevice
* 功能概要  : 打开读卡器
* 参数输入  : 无
* 参数输出  : 无
*
* 返回值   : 0    成功
*            其他 失败
*******************************************************************************/
        /// <summary>
        /// 打开读卡器
        /// </summary>
        /// <returns>0    成功 ,其他 失败</returns>
        [DllImport(@"Encryption\DLL_SERVER_SOUTH\Card\CSGFormalTest_EN_Send.dll", EntryPoint = "OpenDevice", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int WINAPI_OpenDevice();

        /*******************************************************************************
        * 函数名称  : CloseDevice
        * 功能概要  : 关闭读卡器
        * 参数输入  : 无
        * 参数输出  : 无
        *
        * 返回值   : 0    成功
        *            其他 失败
        *******************************************************************************/
        /// <summary>
        /// 关闭读卡器
        /// </summary>
        /// <returns>0    成功</returns>
        [DllImport(@"Encryption\DLL_SERVER_SOUTH\Card\CSGFormalTest_EN_Send.dll", EntryPoint = "CloseDevice", CharSet = CharSet.Ansi)]
        public static extern int WINAPI_CloseDevice();

        /*******************************************************************************
        *函数名称	:	ReadParamPresetCard
        *功能概要 	:	读参数预置卡	
        *参数输入	:	无
        *参数输出   :	fileParam	参数信息文件(32字节)
                    :	fileMoney	购电信息文件(8字节)
                    :	filePrice1	当前套电价文件(199字节)
                    :	filePrice2	备用套电价文件(199字节)
                    :	cardNum		卡片序列号(8字节)      
        *函数返回值 :	0 成功
                        其他失败
        *******************************************************************************/
        //extern "C" int WINAPI ReadParamPresetCard (char *fileParam, char *fileMoney, char *filePrice1, char *filePrice2, char*cardNum);
        /// <summary>
        /// 读参数预置卡
        /// </summary>
        /// <param name="fileParam">参数信息文件(32字节)</param>
        /// <param name="fileMoney">购电信息文件(8字节)</param>
        /// <param name="filePrice1">当前套电价文件(199字节)</param>
        /// <param name="filePrice2">备用套电价文件(199字节)</param>
        /// <param name="cardNum">卡片序列号(8字节)</param>
        /// <returns>0 成功</returns>
        [DllImport(@"Encryption\DLL_SERVER_SOUTH\Card\CSGFormalTest_EN_Send.dll", EntryPoint = "ReadParamPresetCard", CharSet = CharSet.Ansi)]
        public static extern int WINAPI_ReadParamPresetCard(StringBuilder fileParam, StringBuilder fileMoney, StringBuilder filePrice1, StringBuilder filePrice2, StringBuilder cardNum);

        /*******************************************************************************
        *函数名称	:	MakeParamPresetCard
        *功能概要 	:	写参数预置卡
        *参数输入   :	fileParam	参数信息文件(32字节)
                    :	fileMoney	购电信息文件(8字节)
                    :	filePrice1	当前套电价文件(199字节)
                    :	filePrice2	备用套电价文件(199字节)
        *参数输出	:	无
        *函数返回值 :	0 成功
                        其他失败
        *******************************************************************************/
        /// <summary>
        /// 写参数预置卡
        /// </summary>
        /// <param name="fileParam">参数信息文件(32字节)</param>
        /// <param name="fileMoney">购电信息文件(8字节)</param>
        /// <param name="filePrice1">当前套电价文件(199字节)</param>
        /// <param name="filePrice2">备用套电价文件(199字节)</param>
        /// <returns>0 成功</returns>
        [DllImport(@"Encryption\DLL_SERVER_SOUTH\Card\CSGFormalTest_EN_Send.dll", EntryPoint = "MakeParamPresetCard", CharSet = CharSet.Ansi)]
        public static extern int WINAPI_MakeParamPresetCard(string fileParam, string fileMoney, string filePrice1, string filePrice2);

        /*******************************************************************************
        *函数名称	:	ReadUserCard
        *功能概要 	:	读用户卡数据
        *参数输入	:	无	
        *参数输出   :	fileParam	  参数信息文件(45字节)
                    :	fileMoney 	  购电信息文件(8字节)
                    :	filePrice1	  当前套电价文件(199字节)
                    :	filePrice2	  备用套电价文件(199字节)
                    :	fileReply	  返写信息文件(50字节)
                    :	enfileControl 合闸命令文件密文(16字节)
                    :	cardNum		  卡片序列号(8字节)
        *函数返回值 :	0 成功
                        其他失败
        *******************************************************************************/
        /// <summary>
        /// 读用户卡数据
        /// </summary>
        /// <param name="fileParam">参数信息文件(45字节)</param>
        /// <param name="fileMoney">购电信息文件(8字节)</param>
        /// <param name="filePrice1">当前套电价文件(199字节)</param>
        /// <param name="filePrice2">备用套电价文件(199字节)</param>
        /// <param name="fileReply">返写信息文件(50字节)</param>
        /// <param name="enfileControl">合闸命令文件密文(16字节)</param>
        /// <param name="cardNum">卡片序列号(8字节)</param>
        /// <returns>0 成功</returns>
        [DllImport(@"Encryption\DLL_SERVER_SOUTH\Card\CSGFormalTest_EN_Send.dll", EntryPoint = "ReadUserCard", CharSet = CharSet.Ansi)]
        public static extern int WINAPI_ReadUserCard(StringBuilder fileParam, StringBuilder fileMoney, StringBuilder filePrice1, StringBuilder filePrice2, StringBuilder fileReply, StringBuilder enfileControl, StringBuilder cardNum);

        /*******************************************************************************
        *函数名称	:	MakeUserCard
        *功能概要 	:	写用户卡	
        *参数输入   :	fileParam	参数信息文件(45字节)
                    :	fileMoney	购电信息文件(8字节)
                    :	filePrice1	当前套电价文件(199字节)
                    :	filePrice2	备用套电价文件(199字节)
                    :	fileControl	合闸命令文件明文(8字节)
        *参数输出	:	无
        *函数返回值 :	0 成功
                        其他失败
        *******************************************************************************/
        /// <summary>
        /// 写用户卡
        /// </summary>
        /// <param name="fileParam">参数信息文件(45字节)</param>
        /// <param name="fileMoney">购电信息文件(8字节)</param>
        /// <param name="filePrice1">当前套电价文件(199字节)</param>
        /// <param name="filePrice2">备用套电价文件(199字节)</param>
        /// <param name="fileControl">合闸命令文件明文(8字节)</param>
        /// <returns>0 成功</returns>
        [DllImport(@"Encryption\DLL_SERVER_SOUTH\Card\CSGFormalTest_EN_Send.dll", EntryPoint = "MakeUserCard", CharSet = CharSet.Ansi)]
        public static extern int WINAPI_MakeUserCard(string fileParam, string fileMoney, string filePrice1, string filePrice2, string fileControl);

        #endregion


    }
}
