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
// �����ز���
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

#define ERR_OPENPORT	202		//���豸����
#define ERR_CLOSEPORT	203		//�ر��豸����
#define ERR_GETRAND		306		//ȡ���������

#define ERR_KEYOUT1		700		//��Կ���������޷�ɢ
#define ERR_KEYOUT2		701		//��Կ��������ESAM���к�����ɢ���ӣ���Կ��ɢ
#define ERR_KEYOUT3		702		//��Կ��������ESAM���к�����ɢ���ӣ���Կ����
#define ERR_KEYOUT4		703		//��Կ�������󣬱������ɢ���ӣ���Կ��ɢ
#define ERR_KEYOUT5		704		//��Կ�������󣬱������ɢ���ӣ���Կ����

#define ERR_MACCHECK	810		//MACУ�����
#define ERR_ENCRYPT1	900		//���ݼ��ܴ���
#define ERR_ENCRYPT3	902		//MAC�������

#define ERR_AUTH		1100	//��֤���󣬺�����֤ʱ�ȶ�����
#define ERR_SIGNATURE	1200	//����ǩ������

#define ERR_PARAM1		1501	//����1����
#define ERR_PARAM2		1502	//����2����
#define ERR_PARAM3		1503	//����3����
#define ERR_PARAM4		1504	//����4����
#define ERR_PARAM5		1505	//����5����
#define ERR_PARAM6		1506	//����6����
#define ERR_PARAM7		1507	//����7����
#define ERR_PARAM8		1508	//����8����

#define ERR_NOT_SUPPORT		1601	//��֧��

#endif //_MASTER_STATION_HSM_H_