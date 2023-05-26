#ifndef _CSGFORMALTEST_EN_DLL_H_
#define _CSGFORMALTEST_EN_DLL_H_

//��������
//���ܻ���س�������
#define THES_HSM_PARA_ERROR 100       //���ܼ���������
#define THES_CARD_TYPE_ERROR 101      //��Ƭ���ʹ���
#define THES_IN_OUTPUT_PARA_ERROR 103 //�������ָ��Ϊ��
#define THES_INPUT_PARA_LEN_ERROR 104 //�����������ʧ��
#define THES_LOAD_DLL_ERROR 200       //���ض�̬��ʧ��
#define THES_CONNECT_HSM_ERROR 300    //���Ӽ��ܻ�ʧ��
#define THES_DISCONNECT_HSM_ERROR 400 //�Ͽ����ܻ�ʧ��
#define THES_JMJ_GENRAND_ERROR 500    //ȡ�����ʧ��(���ܻ�)
#define THES_JMJ_ENDATA_ERROR 600	  //��������ʧ��
#define THES_JMJ_CALMAC_ERROR 700     //����MACʧ��
#define THES_JMJ_EXPORTKEY_ERROR 800  //������Կʧ��


//��������س�������
#define  OK                      0      // ����
#define  ERROR_UNKNOWN           -1     // δ֪�쳣
#define  ERROR_ELECTRIFY         1000   // �ϵ�ʧ��
#define  ERROR_RESET             1001   // ��λʧ��
#define  ERROR_UNPUBLISH         1002   // δ����
#define  ERROR_IC_PUBLISHED      1003   // оƬ�ѷ���
#define  ERROR_MATERIAL          1004   // ���ϴ���
//���������1021�Ժ������ۼӣ�1005-1020ΪԤ����
#define  ERROR_PARAM             1021   // ��������
#define  ERROR_READER_UNOPENED   1022   // ������δ��
#define  ERROR_AUTH_IDENTITY     1023   // �����֤ʧ��
#define  ERROR_AUTH_OUTER        1024   // �ⲿ��֤ʧ��
#define  ERROR_SELECTFILE_MF     1025   // ѡ�������ļ�ʧ��
#define  ERROR_SELECTFILE_DF     1026   // ѡ��Ŀ¼�ļ�ʧ��
#define  ERROR_GET_RAND          1027   // ȡ�����ʧ��(��Ƭ)
#define  ERROR_READ_SN           1028   // ����Ƭ���к�ʧ��
#define  ERROR_READFILE_PARAM    1029   // ��������Ϣ�ļ�ʧ��
#define  ERROR_READFILE_PURSE    1030   // ��������Ϣ�ļ�ʧ��
#define  ERROR_READFILE_PRICE1   1031   // ����ǰ�׵���ļ�ʧ��
#define  ERROR_READFILE_PRICE2   1032   // �������׵���ļ�ʧ��
#define  ERROR_READFILE_REPLY    1033   // ����д��Ϣ�ļ�ʧ��
#define  ERROR_LOADKEY_NO1       1034   // ������Կ1ʧ��
#define  ERROR_LOADKEY_NO2       1035   // ������Կ2ʧ��
#define  ERROR_LOADKEY_NO3       1036   // ������Կ3ʧ��
#define  ERROR_LOADKEY_NO4       1037   // ������Կ4ʧ��
#define  ERROR_LOADKEY_NO5       1038   // ������Կ5ʧ��
#define  ERROR_WRITEFILE_PARAM   1039   // д������Ϣ�ļ�ʧ��
#define  ERROR_WRITEFILE_PURSE   1040   // д������Ϣ�ļ�ʧ��
#define  ERROR_WRITEFILE_PRICE1  1041   // д��ǰ�׵���ļ�ʧ��
#define  ERROR_WRITEFILE_PRICE2  1042   // д�����׵���ļ�ʧ��
#define  ERROR_WRITEFILE_REPLY   1043   // д��д��Ϣ�ļ�ʧ��
#define  ERROR_WRITE_SWITCH      1044   // д��բ�����ļ�ʧ��
#define  ERROR_READFILE_SWITCH   1045   // ����բ�����ļ�ʧ��
#define  ERROR_LOADKEY	         1046   // ������Կʧ��

/*******************************************************************************
* ��������  : OpenDevice
* ���ܸ�Ҫ  : �򿪶�����
* ��������  : ��
* �������  : ��
*
* ����ֵ   : 0    �ɹ�
*            ���� ʧ��
*******************************************************************************/
extern "C" int WINAPI OpenDevice();

/*******************************************************************************
* ��������  : CloseDevice
* ���ܸ�Ҫ  : �رն�����
* ��������  : ��
* �������  : ��
*
* ����ֵ   : 0    �ɹ�
*            ���� ʧ��
*******************************************************************************/
extern "C" int WINAPI CloseDevice();

/*******************************************************************************
*��������	:	ReadParamPresetCard
*���ܸ�Ҫ 	:	������Ԥ�ÿ�	
*��������	:	��
*�������   :	fileParam	������Ϣ�ļ�(32�ֽ�)
			:	fileMoney	������Ϣ�ļ�(8�ֽ�)
			:	filePrice1	��ǰ�׵���ļ�(199�ֽ�)
			:	filePrice2	�����׵���ļ�(199�ֽ�)
			:	cardNum		��Ƭ���к�(8�ֽ�)      
*��������ֵ :	0 �ɹ�
				����ʧ��
*******************************************************************************/
//extern "C" int WINAPI ReadParamPresetCard (char *fileParam, char *fileMoney, char *filePrice1, char *filePrice2, char*cardNum);
extern "C" int WINAPI ReadParamPresetCard (LPTSTR fileParam, LPTSTR fileMoney, LPTSTR filePrice1, LPTSTR filePrice2, LPTSTR cardNum);

/*******************************************************************************
*��������	:	MakeParamPresetCard
*���ܸ�Ҫ 	:	д����Ԥ�ÿ�
*��������   :	fileParam	������Ϣ�ļ�(32�ֽ�)
			:	fileMoney	������Ϣ�ļ�(8�ֽ�)
			:	filePrice1	��ǰ�׵���ļ�(199�ֽ�)
			:	filePrice2	�����׵���ļ�(199�ֽ�)
*�������	:	��
*��������ֵ :	0 �ɹ�
				����ʧ��
*******************************************************************************/
extern "C" int WINAPI MakeParamPresetCard (char*fileParam, char *fileMoney, char *filePrice1, char *filePrice2);

/*******************************************************************************
*��������	:	ReadUserCard
*���ܸ�Ҫ 	:	���û�������
*��������	:	��	
*�������   :	fileParam	  ������Ϣ�ļ�(45�ֽ�)
			:	fileMoney 	  ������Ϣ�ļ�(8�ֽ�)
			:	filePrice1	  ��ǰ�׵���ļ�(199�ֽ�)
			:	filePrice2	  �����׵���ļ�(199�ֽ�)
			:	fileReply	  ��д��Ϣ�ļ�(50�ֽ�)
			:	enfileControl ��բ�����ļ�����(16�ֽ�)
			:	cardNum		  ��Ƭ���к�(8�ֽ�)
*��������ֵ :	0 �ɹ�
				����ʧ��
*******************************************************************************/
extern "C" int WINAPI ReadUserCard(char *fileParam, char *fileMoney, char *filePrice1, char *filePrice2, char *fileReply, char *enfileControl, char*cardNum);

/*******************************************************************************
*��������	:	MakeUserCard
*���ܸ�Ҫ 	:	д�û���	
*��������   :	fileParam	������Ϣ�ļ�(45�ֽ�)
			:	fileMoney	������Ϣ�ļ�(8�ֽ�)
			:	filePrice1	��ǰ�׵���ļ�(199�ֽ�)
			:	filePrice2	�����׵���ļ�(199�ֽ�)
			:	fileControl	��բ�����ļ�����(8�ֽ�)
*�������	:	��
*��������ֵ :	0 �ɹ�
				����ʧ��
*******************************************************************************/
 extern "C" int WINAPI MakeUserCard (char*fileParam, char *fileMoney, char *filePrice1, char *filePrice2, char *fileControl);


#endif