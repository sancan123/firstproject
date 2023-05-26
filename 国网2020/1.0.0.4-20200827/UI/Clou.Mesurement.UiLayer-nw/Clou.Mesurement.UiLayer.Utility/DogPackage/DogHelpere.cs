////////////////////////////////////////////////////////////////////
// Demo program for SuperDog licensing functions
//
// Copyright (C) 2013 SafeNet, Inc. All rights reserved.
//
// SuperDog(R) is a trademark of SafeNet, Inc.
//
////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using SuperDog;
using System.IO;
using System.Xml;
using System.Threading;

namespace Mesurement.UiLayer.Utility.DogPackage
{
    /// <summary>
    /// ��ȡ��������Ϣ,�������Ļ�ȡ����,�������볬�����е��ַ������жԱ�,���һ������Ϊ����������
    /// </summary>
    public class DogHelper
    {
        private static DogHelper instance = null;
        public static DogHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DogHelper();
                }
                return instance;
            }
        }

        #region �õ�������
        private string codeValue = "";
        /// <summary>
        /// ���ܹ���Ӧ������
        /// </summary>
        private string CodeValue
        {
            get
            {
                if (string.IsNullOrEmpty(codeValue))
                {
                    string fileName = string.Format(@"{0}\DogFile\{1}", Directory.GetCurrentDirectory(), "License.dat");
                    if (File.Exists(fileName))
                    {
                        codeValue = File.ReadAllText(fileName);
                    }
                }
                return codeValue;
            }
        }
        string scope = "<dogscope />";
        //����
        //CL3000S-H_V4
        private byte[] encryptArr = new byte[16]{0x1D, 0x46, 0x97, 0x6B, 0x01, 0xA6, 0xB6, 0x81, 0x06, 0xFE, 0x33, 0x49, 0xF6, 0x2A, 0xC0, 0xF3
 };
        #endregion

        #region �쳣�����
        /// <summary>
        /// ��ȡ״̬��Ӧ����Ϣ
        /// </summary>
        /// <param name="statusTemp"></param>
        /// <returns></returns>
        private string GetErrorMessage(DogStatus statusTemp)
        {
            switch (statusTemp)
            {
                case DogStatus.StatusOk:
                    return "�����ɹ�";
                case DogStatus.InvalidAddress:
                    return "���󳬳������ļ��ķ�Χ";
                case DogStatus.NotEnoughMemory:
                    return "ϵͳ�ڴ治��";
                case DogStatus.TooManyOpenFeatures:
                    return "�򿪵ĵ�¼�Ự��Ŀ����";
                case DogStatus.AccessDenied:
                    return "���ʱ��ܾ�";
                case DogStatus.DogNotFound:
                    return "δ�ҵ�����ĳ�����";
                case DogStatus.BufferTooShort:
                    return "����/���ܵ����ݳ���̫��";
                case DogStatus.InvalidHandle:
                    return "���뺯���ľ����Ч";
                case DogStatus.InvalidFile:
                    return "�޷�ʶ���ļ���ʶ��";
                case DogStatus.InvalidFormat:
                    return "��Ч��XML��ʽ";
                case DogStatus.KeyIdNotFound:
                    return "δ�ҵ��������ĳ�����";
                case DogStatus.InvalidUpdateData:
                    return "δ�ҵ������XML��ǣ����߶��������ݵ������Ѷ�ʧ����Ч";
                case DogStatus.UpdateNotSupported:
                    return "�ó�������֧����������";
                case DogStatus.InvalidUpdateCounter:
                    return "�������������ò���ȷ";
                case DogStatus.InvalidVendorCode:
                    return "����Ŀ����̴�����Ч";
                case DogStatus.InvalidTime:
                    return "�����ʱ��ֵ������֧�ֵ���ֵ��Χ";
                case DogStatus.UpdateNoAckSpace:
                    return "����Ҫ���ִ���ݣ����������ack_dataΪNULL";
                case DogStatus.TerminalServiceDetected:
                    return "�������ն˷�����������";
                case DogStatus.UnknownAlgorithm:
                    return "V2C�ļ���ʹ����δ֪�㷨";
                case DogStatus.InvalidSignature:
                    return "ǩ����֤ʧ��";
                case DogStatus.FeatureNotFound:
                    return "����������";
                case DogStatus.LocalCommErr:
                    return "API�ͳ��������л�����License Manager��ͨѶ����";
                case DogStatus.UnknownVcode:
                    return "API��ʶ�𿪷��̴���";
                case DogStatus.InvalidXmlSpec:
                    return "��Ч��XML��ʽ";
                case DogStatus.InvalidXmlScope:
                    return "��Ч��XML��Χ";
                case DogStatus.TooManyKeys:
                    return "��ǰ���ӵĳ�������Ŀ����";
                case DogStatus.BrokenSession:
                    return "�Ự���ж�";
                case DogStatus.FeatureExpired:
                    return "������ʧЧ";
                case DogStatus.TooOldLM:
                    return "�����������л����汾̫��";
                case DogStatus.DeviceError:
                    return "�볬����ͨѶ�г���USBͨ�Ŵ���";
                case DogStatus.TimeError:
                    return "ϵͳʱ���ѱ��۸�";
                case DogStatus.SecureChannelError:
                    return "��ȫͨ���з�����ͨ�Ŵ���";
                case DogStatus.EmptyScopeResults:
                    return "��������������ݱ��ƻ�";
                case DogStatus.UpdateTooOld:
                    return "�ļ��е���������������ֵС�ڳ������е���������������ֵ��������װV2C�ļ�";
                case DogStatus.UpdateTooNew:
                    return "�ļ��е���������������ֵ���ڳ������е���������������ֵ��������װV2C�ļ�";
                case DogStatus.NoApiDylib:
                    return "δ�ҵ�API�Ķ�̬��";
                case DogStatus.InvApiDylib:
                    return "API �Ķ�̬����Ч";
                case DogStatus.InvalidObject:
                    return "����ĳ�ʼ������ȷ";
                case DogStatus.InvalidParameter:
                    return "��Ч�ĺ�������";
                case DogStatus.AlreadyLoggedIn:
                    return "���ε�¼��ͬһ����";
                case DogStatus.AlreadyLoggedOut:
                    return "��ͬһ����ע������";
                case DogStatus.OperationFailed:
                    return "ϵͳ��ƽ̨��ʹ�ò���ȷ";
                case DogStatus.NetDllBroken:
                    return "���ܹ���̬���ļ���";
                case DogStatus.NotImplemented:
                    return "δʵʩ������Ĺ���";
                case DogStatus.InternalError:
                    return "API���ڲ�����";
                //case DogStatus.NextFreeValues:
                //    return "";
                default:
                    return "δ֪����";
            }
        }
        #endregion

        #region ��ʼ�����жϼ��ܹ�����
        private XmlDocument doc = new XmlDocument();
        private string dogId = "";
        /// <summary>
        /// �ļ����
        /// </summary>
        private int fileId = 0xfff4;
        /// <summary>
        /// ��ʼ�����ܻ���Ϣ:��ȡ���ܹ���ź��ļ����
        /// </summary>
        /// <param name="errorString"></param>
        /// <returns></returns>
        private bool Initialize(out string errorString)
        {
            Dog curDog = new Dog();
            errorString = "";
            DogStatus status;
            try
            {
                status = curDog.Login(CodeValue, scope);
                if (status != DogStatus.StatusOk)
                {
                    errorString = string.Format("��¼�����ܹ�ʧ��:{0}", GetErrorMessage(status));
                    return false;
                }
                string xmlString = "";
                status = curDog.GetSessionInfo(Dog.KeyInfo, ref xmlString);
                if (status != DogStatus.StatusOk)
                {
                    errorString = string.Format("��ȡ���ܹ�������Ϣʧ��:{0}", GetErrorMessage(status));
                    return false;
                }
                doc.LoadXml(xmlString);
                XmlNode nodeTemp = doc.DocumentElement;
                XmlNode nodeDogId = nodeTemp.SelectSingleNode(@"//dogid");
                dogId = nodeDogId.InnerText;
                XmlNode nodeFileId = nodeTemp.SelectSingleNode(@"//fileid");
                int.TryParse(nodeFileId.InnerText, out fileId);
                return true;
            }
            catch (Exception e)
            {
                errorString = string.Format("��ʼ�����ܹ�ʧ��:{0}", e.Message);
                return false;
            }
            finally
            {
                curDog.Logout();
            }
        }

        /// <summary>
        /// �жϼ��ܹ��ڲ���
        /// </summary>
        /// <returns></returns>
        private bool CheckKey(out string errorString)
        {
            errorString = "";
            DogStatus status;
            Dog curDog = new Dog(DogFeature.Default);
            try
            {
                #region ��¼
                status = curDog.Login(CodeValue, scope);
                if (status != DogStatus.StatusOk)
                {
                    errorString = string.Format("��¼�����ܹ�ʧ��:{0}", GetErrorMessage(status));
                    return false;
                }
                #endregion
                #region  ��ȡ���ܹ��ļ���Ϣ
                DogFile fileTemp1 = curDog.GetFile(fileId);
                int fileSize = 0x80;
                status = fileTemp1.FileSize(ref fileSize);
                if (status != DogStatus.StatusOk)
                {
                    errorString = string.Format("��¼�����ܹ�ʧ��:{0}", GetErrorMessage(status));
                    return false;
                }
                #endregion
                #region ��ȡ���ܹ��ڴ��е�����
                byte[] bufData = new byte[fileSize];
                status = fileTemp1.Read(bufData, 0, fileSize);
                if (status != DogStatus.StatusOk)
                {
                    errorString = string.Format("��¼�����ܹ�ʧ��:{0}", GetErrorMessage(status));
                    return false;
                }
                #endregion
                #region ���ü��ܹ�������������,��ȡ����
                byte[] bufTemp = new byte[encryptArr.Length];
                for (int i = 0; i < encryptArr.Length; i++)
                {
                    bufTemp[i] = encryptArr[i];
                }
                status = curDog.Decrypt(bufTemp);
                if (status != DogStatus.StatusOk)
                {
                    errorString = string.Format("��¼�����ܹ�ʧ��:{0}", GetErrorMessage(status));
                    return false;
                }
                #endregion
                #region ���ڴ��е��������������ݶԱ�,���һ������֤�ɹ�
                for (int i = 0; i < 12; i++)
                {
                    if (bufTemp[i] != bufData[10 + i])
                    {
                        return false;
                    }
                }
                #endregion
                return true;
            }
            catch (Exception e)
            {
                errorString = string.Format("��֤���ܹ������쳣:{0}", e.Message);
                return false;
            }
            finally
            {
                curDog.Logout();
            }
        }
        #endregion

        private void ThreadProcess()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(10000);
                    string errorString = "";
                    //�����ѯ���ζ����ܲ��ҵ����ܹ�,�����˳�
                    //ÿ�β�ѯ���ܹ�ʱ����Ϊ10��
                    for (int i = 0; i < 3; i++)
                    {
                        if (!CheckKey(out errorString))
                        {
                            if (EventLostDog != null)
                            {

                                EventLostDog(this, new DogEventArgs(i == 2, errorString));
                            }
                            //�Ҳ������ܹ��˳��߳�
                            if (i == 2)
                            {
                                return;
                            }
                            Thread.Sleep(10000);
                        }
                        //�ҵ����ܹ���ִ��ѭ��
                        else
                        {
                            break;
                        }
                    }
                }
                catch
                {

                }
            }
        }

        private Thread threadDog = null;
        /// <summary>
        /// �������ܹ�ɨ��
        /// </summary>
        /// <param name="errorString"></param>
        /// <returns></returns>
        public bool Run(out string errorString)
        {
            errorString = "";
            if (threadDog == null || threadDog.ThreadState != ThreadState.Running)
            {
                if (!Initialize(out errorString))
                {
                    return false;
                }
                threadDog = new Thread(() => ThreadProcess());
                threadDog.Start();
            }
            return true;
        }
        public event EventHandler EventLostDog;
        /// <summary>
        /// ֹͣ���ܹ�ɨ��
        /// </summary>
        public void Stop()
        {
            if (threadDog != null && threadDog.ThreadState == ThreadState.Running)
            {
                threadDog.Abort();
            }
        }
    }
}