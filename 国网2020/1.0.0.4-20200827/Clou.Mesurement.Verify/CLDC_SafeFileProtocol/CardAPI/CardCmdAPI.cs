using System.Runtime.InteropServices;
using System.Text;

namespace CLDC_SafeFileProtocol.CardAPI
{
    /// <summary>
    /// 操作卡片指令API
    /// </summary>
    public class CardCmdAPI
    {
        #region 读卡器指令
        //2.1.	SelDir
        //函数名称    int SelDir(char* selMF, char* selDF)
        //函数功能 
        //出参 selMF  
        //    selDF 
        //返回值	
        //※参数详细信息请参照「接口参数说明」。

        //常用错误码
        //错误码 解释
        //1021	参数错误
        /// <summary>
        /// 输出选择目录指令
        /// </summary>
        /// <param name="selMF">选择主目录(7字节)</param>
        /// <param name="selDF">选择应用目录(7字节)</param>
        /// <returns>0：成功，其他：失败</returns>
        [DllImport(@"Encryption\DLL_SERVER_SOUTH\Card\CSGFormalTest_EN_Cmd.dll", EntryPoint = "SelDir", CharSet = CharSet.Ansi)]
        public static extern int CMD_SelDir(StringBuilder selMF, StringBuilder selDF);
        //2.2.	ReadParamPresetCardNum
        //函数名称    int ReadParamPresetCardNum(char* cardNum)
        //函数功能 
        //出参 cardNum 
        //返回值	
        //※参数详细信息请参照「接口参数说明」。

        //常用错误码
        //错误码 解释
        //1021	参数错误
        /// <summary>
        /// 输出读参数预置卡卡号指令
        /// </summary>
        /// <param name="cardNum">读参数预置卡卡号指令(5字节)</param>
        /// <returns>0：成功，其他：失败</returns>
        [DllImport(@"Encryption\DLL_SERVER_SOUTH\Card\CSGFormalTest_EN_Cmd.dll", EntryPoint = "ReadParamPresetCardNum", CharSet = CharSet.Ansi)]
        public static extern int CMD_ReadParamPresetCardNum(StringBuilder cardNum);
        //2.3.	ReadCardRand
        //函数名称    int ReadCardRand(char* cardRand)
        //函数功能 
        //出参 cardRand 
        //返回值	
        //※参数详细信息请参照「接口参数说明」。

        //常用错误码
        //错误码 解释
        //1021	参数错误
        /// <summary>
        /// 输出取卡片随机数指令
        /// </summary>
        /// <param name="cardRand">取卡片随机数指令(5字节)</param>
        /// <returns>0：成功，其他：失败</returns>
        [DllImport(@"Encryption\DLL_SERVER_SOUTH\Card\CSGFormalTest_EN_Cmd.dll", EntryPoint = "ReadCardRand", CharSet = CharSet.Ansi)]
        public static extern int CMD_ReadCardRand(StringBuilder cardRand);
        //2.4.	ReadParamPresetCard
        //函数名称    int ReadParamPresetCard(char* fileParam, char* fileMoney, char* filePrice1, char* filePrice2)
        //函数功能 
        //出参 fileParam
        //    fileMoney 
        //    filePrice1 
        //    filePrice2 
        //返回值	
        //※参数详细信息请参照「接口参数说明」。

        //常用错误码
        //错误码 解释
        //1021	参数长度错误
        /// <summary>
        /// 输出读参数预置卡指令
        /// </summary>
        /// <param name="fileParam">读参数信息文件指令(5字节)</param>
        /// <param name="fileMoney">读购电信息文件指令(5字节)</param>
        /// <param name="filePrice1">读当前套电价文件指令(5字节)</param>
        /// <param name="filePrice2">读备用套电价文件指令(5字节)</param>
        /// <returns>0：成功，其他：失败</returns>
        [DllImport(@"Encryption\DLL_SERVER_SOUTH\Card\CSGFormalTest_EN_Cmd.dll", EntryPoint = "ReadParamPresetCard", CharSet = CharSet.Ansi)]
        public static extern int CMD_ReadParamPresetCard(StringBuilder fileParam, StringBuilder fileMoney, StringBuilder filePrice1, StringBuilder filePrice2);
        //2.5.	ParamPresetCardAuth
        //函数名称    int ParamPresetCardAuth(char* cardNum, char* randFromCard, char* data_Auth1, char* cmd_Auth1, char* cmd_Auth2)
        //函数功能 
        //入参 cardNum     
        //    randFromCard 
        //出参 data_Auth1   
        //    cmd_Auth1 
        //    cmd_Auth2 
        //返回值	
        //※参数详细信息请参照「接口参数说明」。

        //常用错误码
        //错误码 解释
        //200	加载动态库失败
        //300	连接加密机失败
        //400	断开加密机失败
        //500	取随机数失败(加密机)
        //600	加密数据失败
        //1021	参数错误
        /// <summary>
        /// 输出参数预置卡身份认证指令
        /// </summary>
        /// <param name="cardNum">卡片序列号(8字节)</param>
        /// <param name="randFromCard">随机数(8字节)</param>
        /// <param name="data_Auth1">参数预置卡身份认证数据(8字节)</param>
        /// <param name="cmd_Auth1">参数预置卡身份认证1(13字节)</param>
        /// <param name="cmd_Auth2">参数预置卡身份认证2(13字节)</param>
        /// <returns>0：成功，其他：失败</returns>
        [DllImport(@"Encryption\DLL_SERVER_SOUTH\Card\CSGFormalTest_EN_Cmd.dll", EntryPoint = "ParamPresetCardAuth", CharSet = CharSet.Ansi)]
        public static extern int CMD_ParamPresetCardAuth(string cardNum, string randFromCard, StringBuilder data_Auth1, StringBuilder cmd_Auth1, StringBuilder cmd_Auth2);
        //2.6.	MakeParamPresetCard
        //函数名称    int MakeParamPresetCard(char* fileParam, char* fileMoney, char* filePrice1, char* filePrice2, char* cmd_fileParam, char* cmd_fileMoney, char* cmd_filePrice1, char* cmd_filePrice2)
        //函数功能 
        //入参 fileParam   
        //    fileMoney 
        //	filePrice1 
        //    filePrice2 
        //出参 cmd_fileParam   
        //    cmd_fileMoney 
        //	cmd_filePrice1 
        //    cmd_filePrice2 
        //返回值	
        //※参数详细信息请参照「接口参数说明」。

        //常用错误码
        //错误码 解释
        //1021	参数错误
        /// <summary>
        /// 输出写参数预置卡指令
        /// </summary>
        /// <param name="fileParam">参数信息文件(32字节)</param>
        /// <param name="fileMoney">购电信息文件(购电金额，购电次数)(8字节)</param>
        /// <param name="filePrice1">当前套电价文件(199字节)</param>
        /// <param name="filePrice2">备用套电价文件(199字节)</param>
        /// <param name="cmd_fileParam">写参数信息文件指令(37字节)</param>
        /// <param name="cmd_fileMoney">写购电信息文件指令(购电金额，购电次数)(13字节)</param>
        /// <param name="cmd_filePrice1">写当前套电价文件指令(204字节)</param>
        /// <param name="cmd_filePrice2">写备用套电价文件指令(204字节)</param>
        /// <returns>0：成功，其他：失败</returns>
        [DllImport(@"Encryption\DLL_SERVER_SOUTH\Card\CSGFormalTest_EN_Cmd.dll", EntryPoint = "MakeParamPresetCard", CharSet = CharSet.Ansi)]
        public static extern int CMD_MakeParamPresetCard(string fileParam, string fileMoney, string filePrice1, string filePrice2, StringBuilder cmd_fileParam, StringBuilder cmd_fileMoney, StringBuilder cmd_filePrice1, StringBuilder cmd_filePrice2);
        //2.7.	ReadUserCardNum
        //函数名称    int ReadUserCardNum(char* cardNum)
        //函数功能 
        //出参 cardNum 
        //返回值	
        //※参数详细信息请参照「接口参数说明」。

        //常用错误码
        //错误码 解释
        //1021	参数错误
        /// <summary>
        /// 输出读用户卡卡号指令
        /// </summary>
        /// <param name="cardNum">读用户卡卡号指令(5字节)</param>
        /// <returns>0：成功，其他：失败</returns>
        [DllImport(@"Encryption\DLL_SERVER_SOUTH\Card\CSGFormalTest_EN_Cmd.dll", EntryPoint = "ReadUserCardNum", CharSet = CharSet.Ansi)]
        public static extern int CMD_ReadUserCardNum(StringBuilder cardNum);
        //2.8.	ReadUserCard
        //函数名称    int ReadUserCard(char* fileParam, char* fileMoney, char* filePrice1, char* filePrice2, char* fileReply, char* enfileControl)
        //函数功能 
        //出参 fileParam
        //    fileMoney
        //    filePrice1
        //    filePrice2
        //    fileReply
        //    enfileControl
        //返回值	
        //※参数详细信息请参照「接口参数说明」。

        //常用错误码
        //错误码 解释
        //1021	参数错误
        /// <summary>
        /// 输出读用户卡指令
        /// </summary>
        /// <param name="fileParam">读参数信息文件指令(5字节)</param>
        /// <param name="fileMoney">读购电信息文件指令(5字节)</param>
        /// <param name="filePrice1">读当前套电价文件指令(5字节)</param>
        /// <param name="filePrice2">读备用套电价文件指令(5字节)</param>
        /// <param name="fileReply">读返写信息文件指令(5字节)</param>
        /// <param name="enfileControl">读合闸命令文件密文指令(5字节)</param>
        /// <returns>0：成功，其他：失败</returns>
        [DllImport(@"Encryption\DLL_SERVER_SOUTH\Card\CSGFormalTest_EN_Cmd.dll", EntryPoint = "ReadUserCard", CharSet = CharSet.Ansi)]
        public static extern int CMD_ReadUserCard(StringBuilder fileParam, StringBuilder fileMoney, StringBuilder filePrice1, StringBuilder filePrice2, StringBuilder fileReply, StringBuilder enfileControl);
        //2.9.	UserCardAuth
        //函数名称    int UserCardAuth(char* cardNum, char* randFromCard, char* data_Auth1, char* cmd_Auth1, char* cmd_Auth2)
        //函数功能 
        //入参 cardNum 
        //    randFromCard 
        //出参 data_Auth1
        //    cmd_Auth1
        //    cmd_Auth2
        //返回值	
        //※参数详细信息请参照「接口参数说明」。

        //常用错误码
        //错误码 解释
        //200	加载动态库失败
        //300	连接加密机失败
        //400	断开加密机失败
        //500	取随机数失败(加密机)
        //600	加密数据失败
        //601	加密数据失败
        //1021	参数错误
        /// <summary>
        /// 输出用户卡身份认证指令
        /// </summary>
        /// <param name="cardNum">卡片序列号(8字节)</param>
        /// <param name="randFromCard">随机数(8字节)</param>
        /// <param name="data_Auth1">用户卡身份认证数据(8字节)</param>
        /// <param name="cmd_Auth1">用户卡身份认证指令1(13字节)</param>
        /// <param name="cmd_Auth2">用户卡身份认证指令2(13字节)</param>
        /// <returns>0：成功，其他：失败</returns>
        [DllImport(@"Encryption\DLL_SERVER_SOUTH\Card\CSGFormalTest_EN_Cmd.dll", EntryPoint = "UserCardAuth", CharSet = CharSet.Ansi)]
        public static extern int CMD_UserCardAuth(string cardNum, string randFromCard, StringBuilder data_Auth1, StringBuilder cmd_Auth1, StringBuilder cmd_Auth2);
        //2.10.	MakeUserCard
        //函数名称    int MakeUserCard(char* cardNum, char* randFromCard, char* fileParam, char* fileMoney, char* filePrice1, char* filePrice2, char* controlFilePlain, char* cmd_fileParam, char* cmd_fileMoney, char* cmd_filePrice1, char* cmd_filePrice2, char* cmd_EnControlFilePlain)
        //函数功能 
        //入参 cardNum
        //    randFromCard
        //    fileParam
        //    fileMoney
        //    filePrice1
        //    filePrice2
        //    controlFilePlain
        //出参 cmd_fileParam
        //    cmd_fileMoney
        //    cmd_filePrice1
        //    cmd_filePrice2
        //    cmd_EnControlFilePlain
        //返回值	
        //※参数详细信息请参照「接口参数说明」。

        //常用错误码
        //错误码 解释
        //103	输入参数指针为空
        //200	加载动态库失败
        //300	连接加密机失败
        //400	断开加密机失败
        //602	加密闸命令文件失败
        //700	计算参数信息文件MAC失败
        //701	计算钱包文件MAC失败
        //702	计算电价文件1MAC失败
        //703	计算电价文件2MAC失败
        //704	计算闸命令文件密文文件MAC失败
        //1021	参数长度错误
        /// <summary>
        /// 输出写用户卡指令
        /// </summary>
        /// <param name="cardNum">卡片序列号(8字节)</param>
        /// <param name="randFromCard">随机数(8字节)</param>
        /// <param name="fileParam">参数信息文件内容(45字节)</param>
        /// <param name="fileMoney">购电信息文件(购电金额，购电次数)(8字节)</param>
        /// <param name="filePrice1">当前套电价文件(199字节)</param>
        /// <param name="filePrice2">备用套电价文件(199字节)</param>
        /// <param name="fileReply">返写</param>
        /// <param name="controlFilePlain">合闸控制命令明文(8字节)</param>
        /// <param name="cmd_fileParam">写参数信息文件内容指令(54字节)</param>
        /// <param name="cmd_fileMoney">写购电信息文件指令(购电金额，购电次数)(17字节)</param>
        /// <param name="cmd_filePrice1">写当前套电价文件指令(208字节)</param>
        /// <param name="cmd_filePrice2">写备用套电价文件指令(208字节)</param>
        /// <param name="cmd_fileReply">返写指令</param>
        /// <param name="cmd_EnControlFilePlain">写合闸控制命令指令(25字节)</param>
        /// <returns>0：成功，其他：失败</returns>
        [DllImport(@"Encryption\DLL_SERVER_SOUTH\Card\CSGFormalTest_EN_Cmd.dll", EntryPoint = "MakeUserCard", CharSet = CharSet.Ansi)]
        public static extern int CMD_MakeUserCard(string cardNum,string randFromCard, string fileParam, string fileMoney, string filePrice1, string filePrice2, string fileReply,string controlFilePlain, StringBuilder cmd_fileParam, StringBuilder cmd_fileMoney, StringBuilder cmd_filePrice1, StringBuilder cmd_filePrice2, StringBuilder cmd_fileReply,StringBuilder cmd_EnControlFilePlain);
        //2.11.	UserCard_KeyUpdata
        //函数名称    int UserCard_KeyUpdata(char* cardNum, char* divNum, char* cmd_key1, char* cmd_key2, char* cmd_key3, char* cmd_key4, char* cmd_key5, char* cmd_key6, char* cmd_key7, char* cmd_key8, char* cmd_key9, char* cmd_key10, char* cmd_key11, char* cmd_key12)
        //函数功能 
        //入参 cardNum
        //    divNum 
        //出参 cmd_key1
        //    cmd_key2
        //    cmd_key3
        //    cmd_key4
        //    cmd_key5
        //    cmd_key6
        //    cmd_key7
        //    cmd_key8
        //    cmd_key9
        //    cmd_key10
        //    cmd_key11
        //    cmd_key12
        //返回值	
        //※参数详细信息请参照「接口参数说明」。

        //常用错误码
        //错误码 解释
        //200	加载动态库失败
        //300	连接加密机失败
        //400	断开加密机失败
        //1021	参数错误
        //1046	更新密钥失败
        /// <summary>
        /// 输出更新用户卡密钥指令
        /// </summary>
        /// <param name="cardNum">卡片序列号(8字节)</param>
        /// <param name="divNum">表号(0000+表号)(8字节)</param>
        /// <param name="cmd_key1">写密钥1指令(37字节)</param>
        /// <param name="cmd_key2">写密钥2指令(37字节)</param>
        /// <param name="cmd_key3">写密钥3指令(37字节)</param>
        /// <param name="cmd_key4">写密钥4指令(37字节)</param>
        /// <param name="cmd_key5">写密钥5指令(37字节)</param>
        /// <param name="cmd_key6">写密钥6指令(37字节)</param>
        /// <param name="cmd_key7">写密钥7指令(37字节)</param>
        /// <param name="cmd_key8">写密钥8指令(37字节)</param>
        /// <param name="cmd_key9">写密钥9指令(37字节)</param>
        /// <param name="cmd_key10">写密钥10指令(37字节)</param>
        /// <param name="cmd_key11">写密钥11指令(37字节)</param>
        /// <param name="cmd_key12">写密钥12指令(37字节)</param>
        /// <returns>0：成功，其他：失败</returns>
        [DllImport(@"Encryption\DLL_SERVER_SOUTH\Card\CSGFormalTest_EN_Cmd.dll", EntryPoint = "UserCard_KeyUpdata", CharSet = CharSet.Ansi)]
        public static extern int CMD_UserCard_KeyUpdata(string cardNum, string divNum, StringBuilder cmd_key1, StringBuilder cmd_key2, StringBuilder cmd_key3, StringBuilder cmd_key4, StringBuilder cmd_key5, StringBuilder cmd_key6, StringBuilder cmd_key7, StringBuilder cmd_key8, StringBuilder cmd_key9, StringBuilder cmd_key10, StringBuilder cmd_key11, StringBuilder cmd_key12);




        /// <summary>
        /// 带MAC读用户卡 
        /// </summary>
        /// <param name="Rand">随机数(4字节)</param>
        /// <param name="cmd_fileParam">带MAC读参数信息文件指令（14字节）</param>
        /// <param name="cmd_fileMoney">带MAC读购电信息文件指令（14字节）</param>
        /// <param name="cmd_filePrice1">带MAC读当前套电价文件指令（14字节）</param>
        /// <param name="cmd_filePrice2">带MAC读备用套电价文件指令（14字节）</param>
        /// <param name="cmd_EnControlFilePlain">带MAC读合闸命令文件密文指令（14字节）</param>
        /// <returns></returns>
        [DllImport(@"Encryption\DLL_SERVER_SOUTH\Card\CSGFormalTest_EN_Cmd.dll", EntryPoint = "ReadUserCardMAC", CharSet = CharSet.Ansi)]
        public static extern int CMD_ReadUserCardMAC(string Rand, StringBuilder cmd_fileParam, StringBuilder cmd_fileMoney, StringBuilder cmd_filePrice1, StringBuilder cmd_filePrice2, StringBuilder cmd_EnControlFilePlain);



        //常用错误码
        //错误码 解释
        //103	输入参数指针为空
        //200	加载动态库失败
        //300	连接加密机失败
        //400	断开加密机失败
        //602	加密闸命令文件失败
        //700	计算参数信息文件MAC失败
        //701	计算钱包文件MAC失败
        //702	计算电价文件1MAC失败
        //703	计算电价文件2MAC失败
        //704	计算闸命令文件密文文件MAC失败
        //1021	参数长度错误
        /// <summary>
        /// 输出写用户卡指令
        /// </summary>
        /// <param name="cardNum">卡片序列号(8字节)</param>
        /// <param name="randFromCard">随机数(8字节)</param>
        /// <param name="fileReply">返写</param>
        /// <param name="cmd_fileReply">写返写信息文件内容指令(3B字节)</param>
        /// <returns>0：成功，其他：失败</returns>
        [DllImport(@"Encryption\DLL_SERVER_SOUTH\Card\CSGFormalTest_EN_Cmd.dll", EntryPoint = "WriteUserCardReply", CharSet = CharSet.Ansi)]
        public static extern int CMD_WriteUserCardReply(string cardNum, string randFromCard,string fileReply, StringBuilder cmd_fileReply);

        
        #endregion


    }
}
