#ifndef _CSGFORMALTEST_EN_DLL_H_
#define _CSGFORMALTEST_EN_DLL_H_

//常量声明
//加密机相关常量声明
#define THES_HSM_PARA_ERROR 100       //机密级参数错误
#define THES_CARD_TYPE_ERROR 101      //卡片类型错误
#define THES_IN_OUTPUT_PARA_ERROR 103 //输入参数指针为空
#define THES_INPUT_PARA_LEN_ERROR 104 //输入参数长度失败
#define THES_LOAD_DLL_ERROR 200       //加载动态库失败
#define THES_CONNECT_HSM_ERROR 300    //连接加密机失败
#define THES_DISCONNECT_HSM_ERROR 400 //断开加密机失败
#define THES_JMJ_GENRAND_ERROR 500    //取随机数失败(加密机)
#define THES_JMJ_ENDATA_ERROR 600	  //加密数据失败
#define THES_JMJ_CALMAC_ERROR 700     //计算MAC失败
#define THES_JMJ_EXPORTKEY_ERROR 800  //导出密钥失败


//读卡器相关常量声明
#define  OK                      0      // 正常
#define  ERROR_UNKNOWN           -1     // 未知异常
#define  ERROR_ELECTRIFY         1000   // 上电失败
#define  ERROR_RESET             1001   // 复位失败
#define  ERROR_UNPUBLISH         1002   // 未发行
#define  ERROR_IC_PUBLISHED      1003   // 芯片已发行
#define  ERROR_MATERIAL          1004   // 来料错误
//其他错误从1021以后自行累加（1005-1020为预留）
#define  ERROR_PARAM             1021   // 参数错误
#define  ERROR_READER_UNOPENED   1022   // 读卡器未打开
#define  ERROR_AUTH_IDENTITY     1023   // 身份认证失败
#define  ERROR_AUTH_OUTER        1024   // 外部认证失败
#define  ERROR_SELECTFILE_MF     1025   // 选择主控文件失败
#define  ERROR_SELECTFILE_DF     1026   // 选择目录文件失败
#define  ERROR_GET_RAND          1027   // 取随机数失败(卡片)
#define  ERROR_READ_SN           1028   // 读卡片序列号失败
#define  ERROR_READFILE_PARAM    1029   // 读参数信息文件失败
#define  ERROR_READFILE_PURSE    1030   // 读购电信息文件失败
#define  ERROR_READFILE_PRICE1   1031   // 读当前套电价文件失败
#define  ERROR_READFILE_PRICE2   1032   // 读备用套电价文件失败
#define  ERROR_READFILE_REPLY    1033   // 读返写信息文件失败
#define  ERROR_LOADKEY_NO1       1034   // 更新密钥1失败
#define  ERROR_LOADKEY_NO2       1035   // 更新密钥2失败
#define  ERROR_LOADKEY_NO3       1036   // 更新密钥3失败
#define  ERROR_LOADKEY_NO4       1037   // 更新密钥4失败
#define  ERROR_LOADKEY_NO5       1038   // 更新密钥5失败
#define  ERROR_WRITEFILE_PARAM   1039   // 写参数信息文件失败
#define  ERROR_WRITEFILE_PURSE   1040   // 写购电信息文件失败
#define  ERROR_WRITEFILE_PRICE1  1041   // 写当前套电价文件失败
#define  ERROR_WRITEFILE_PRICE2  1042   // 写备用套电价文件失败
#define  ERROR_WRITEFILE_REPLY   1043   // 写返写信息文件失败
#define  ERROR_WRITE_SWITCH      1044   // 写合闸命令文件失败
#define  ERROR_READFILE_SWITCH   1045   // 读合闸命令文件失败
#define  ERROR_LOADKEY	         1046   // 更新密钥失败

/*******************************************************************************
* 函数名称  : OpenDevice
* 功能概要  : 打开读卡器
* 参数输入  : 无
* 参数输出  : 无
*
* 返回值   : 0    成功
*            其他 失败
*******************************************************************************/
extern "C" int WINAPI OpenDevice();

/*******************************************************************************
* 函数名称  : CloseDevice
* 功能概要  : 关闭读卡器
* 参数输入  : 无
* 参数输出  : 无
*
* 返回值   : 0    成功
*            其他 失败
*******************************************************************************/
extern "C" int WINAPI CloseDevice();

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
extern "C" int WINAPI ReadParamPresetCard (LPTSTR fileParam, LPTSTR fileMoney, LPTSTR filePrice1, LPTSTR filePrice2, LPTSTR cardNum);

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
extern "C" int WINAPI MakeParamPresetCard (char*fileParam, char *fileMoney, char *filePrice1, char *filePrice2);

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
extern "C" int WINAPI ReadUserCard(char *fileParam, char *fileMoney, char *filePrice1, char *filePrice2, char *fileReply, char *enfileControl, char*cardNum);

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
 extern "C" int WINAPI MakeUserCard (char*fileParam, char *fileMoney, char *filePrice1, char *filePrice2, char *fileControl);


#endif