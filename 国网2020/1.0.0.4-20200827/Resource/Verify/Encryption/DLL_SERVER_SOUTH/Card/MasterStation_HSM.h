#ifndef _MASTER_STATION_HSM_H_
#define _MASTER_STATION_HSM_H_

#ifdef MASTERSTATION_EXPORTS
#define MASTERSTATION_API __declspec(dllexport)
#else
#define MASTERSTATION_API __declspec(dllimport)
#endif

#ifdef __cplusplus
extern "C" {
#endif

MASTERSTATION_API int OpenDevice(const char* szType,const char *cHostIp, unsigned int uiPort, unsigned int timeout);
MASTERSTATION_API int CloseDevice();	

///////////////////////////////////////////////////////////////////////////////////////////
// 电表相关部分
///////////////////////////////////////////////////////////////////////////////////////////

MASTERSTATION_API int IdentityAuthentication(int Flag, char *PutDiv,char *OutRand,char* OutEndata);
MASTERSTATION_API int UserControl(int Flag,char *PutRand,char *PutDiv,char *PutEsamNo,char *PutData, char *OutEndata);
MASTERSTATION_API int ParameterUpdate (int Flag,char *PutRand,char *PutDiv,char  *PutApdu,char  *PutData, char *OutEndata);
MASTERSTATION_API int Price1Update(int Flag,char *PutRand,char *PutDiv,char  *PutApdu,char  *PutData, char *OutData);
MASTERSTATION_API int Price2Update(int Flag,char *PutRand,char *PutDiv,char  *PutApdu,char  *PutData, char *OutData);
MASTERSTATION_API int ParameterElseUpdate (int Flag,char *PutRand,char *PutDiv,char *PutApdu,char *PutData, char *OutEndata);
MASTERSTATION_API int IncreasePurse(int Flag,char *PutRand,char*PutDiv,char*PutData,char *OutData);
MASTERSTATION_API int InitPurse(int Flag,char *PutRand,char*PutDiv,char*PutData,char *OutData);
MASTERSTATION_API int KeyUpdateV2 (int PutKeySum ,char *PutKeystate,char *PutKeyid,char *PutRand,char *PutDiv,char *PutEsamNo,char *OutData);
MASTERSTATION_API int DataClear1(int Flag,char *PutRand,char *PutDiv,char *PutData,char *Outdata);
MASTERSTATION_API int InfraredRand(char *OutRand1);
MASTERSTATION_API int InfraredAuth(int Flag,char *PutDiv,char *PutEsamNo,char *PutRand1,char *PutRand1Endata,char *PutRand2,char *OutRand2Endata);
MASTERSTATION_API int MacCheck(int Flag, char *PutRand, char *PutDiv, char *PutApdu,char *PutData, char *PutMac);
MASTERSTATION_API int EncMacWrite(int Flag ,char *PutRand,char *PutDiv,char *PutEsamNo,char *PutFileID,char *PutDataBegin,char *PutData,char *OutData);
MASTERSTATION_API int MacWrite(int Flag ,char *PutRand,char *PutDiv,char *PutEsamNo,char *PutFileID,char *PutDataBegin,char *PutData,char *OutData);
MASTERSTATION_API int EncForCompare(char *Flag,char *PutDiv,char *PutData,char *OutData);
MASTERSTATION_API int DecreasePurse(int Flag,char *PutRand,char *PutDiv, char *PutData, char *OutEndata);
MASTERSTATION_API int SwitchChargeMode(int Flag,char *PutRand,char*PutDiv,char*PutData,char *OutData);

#ifdef __cplusplus
}
#endif

#define ERR_OPENPORT	202		//打开设备错误
#define ERR_CLOSEPORT	203		//关闭设备错误
#define ERR_GETRAND		306		//取随机数错误

#define ERR_KEYOUT1		700		//密钥导出错误，无分散
#define ERR_KEYOUT2		701		//密钥导出错误，ESAM序列号做分散因子，密钥分散
#define ERR_KEYOUT3		702		//密钥导出错误，ESAM序列号做分散因子，密钥导出
#define ERR_KEYOUT4		703		//密钥导出错误，表号做分散因子，密钥分散
#define ERR_KEYOUT5		704		//密钥导出错误，表号做分散因子，密钥导出

#define ERR_MACCHECK	810		//MAC校验错误
#define ERR_ENCRYPT1	900		//数据加密错误
#define ERR_ENCRYPT3	902		//MAC计算错误

#define ERR_AUTH		1100	//认证错误，红外认证时比对密文
#define ERR_SIGNATURE	1200	//计算签名错误

#define ERR_PARAM1		1501	//参数1错误
#define ERR_PARAM2		1502	//参数2错误
#define ERR_PARAM3		1503	//参数3错误
#define ERR_PARAM4		1504	//参数4错误
#define ERR_PARAM5		1505	//参数5错误
#define ERR_PARAM6		1506	//参数6错误
#define ERR_PARAM7		1507	//参数7错误
#define ERR_PARAM8		1508	//参数8错误

#define ERR_NOT_SUPPORT		1601	//不支持

#endif //_MASTER_STATION_HSM_H_