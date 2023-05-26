using System;
using CLDC_MeterProtocol.Packet;
using CLDC_DataCore.Const;

namespace CLDC_MeterProtocol.Protocols
{

    public class CDLT645 : ProtocolBase
    {

        public CDLT645()
        {
        }

        /// <summary>
        /// ִ���������(�з���)
        /// </summary>
        /// <param name="byt_Addr">��ַ</param>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">������</param>
        /// <param name="bln_Sequela">�Ƿ��к���֡</param>
        /// <param name="byt_RevDataF">����֡������</param>
        /// <param name="int_WaitSecond">�ȴ�ʱ�䣨���룩,������������</param>
        /// <param name="int_BitSecond">�ֽڼ��ʱ�䣨���룩,������������</param>
        /// <returns></returns>
        public bool ExeCommandA(byte[] byt_Addr, byte byt_Cmd, byte[] byt_Data, ref bool bln_Sequela,
                               ref byte[] byt_RevDataF, int int_WaitSecond, int int_BitSecond)
        {
            //try
            //{
            byte[] byt_Frame = this.OrgFrame(byt_Addr, byt_Cmd, byt_Data);
            MeterProtocolRecvPacket recvPacket = new MeterProtocolRecvPacket();
            MeterProtocolSendPacket sendPacket = new MeterProtocolSendPacket()
            {
                SendData = byt_Frame,
                IsNeedReturn = (recvPacket != null)
            };

            bool bln_Result = base.SendData(sendPacket, recvPacket);//this.SendFrame(byt_Frame, int_WaitSecond, int_BitSecond);
            if (byt_Cmd == 0x11 && bln_Result == false)
            {
                bln_Result = base.SendData(sendPacket, recvPacket);
            }
            if (bln_Result)
            {
                var count = recvPacket.RecvData.Length;


                if (recvPacket.RecvData[0] == recvPacket.RecvData[count / 2] && CLDC_DataCore.Const.GlobalUnit.Flag_IsCarrier == true)
                {
                    var newarry = new Byte[count / 2];
                    for (int i = 0; i < count / 2; i++)
                    {
                        newarry[i] = recvPacket.RecvData[i];
                    }

                    if (CheckFrame(byt_Cmd, newarry, ref bln_Sequela, ref byt_RevDataF))
                        return true;
                }
                else
                {

                    if (CheckFrame(byt_Cmd, recvPacket.RecvData, ref bln_Sequela, ref byt_RevDataF))
                        return true;
                }
            }

            return false;
            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="byt_Addr">��ַ</param>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">������</param>
        /// <param name="bln_Sequela">�Ƿ��к���֡</param>
        /// <param name="byt_RevDataF">����֡������</param>
        /// <param name="int_WaitSecond">�ȴ�ʱ�䣨���룩,������������</param>
        /// <param name="int_BitSecond">�ֽڼ��ʱ�䣨���룩,������������</param>
        /// <returns></returns>
        public bool ExeCommandByMac(byte[] byt_Addr, byte byt_Cmd, byte[] byt_Data, ref bool bln_Sequela,
                               ref byte[] byt_RevDataF, int int_WaitSecond, int int_BitSecond)
        {
            //try
            //{
            byte[] byt_Frame = this.OrgFrame(byt_Addr, byt_Cmd, byt_Data);
            MeterProtocolRecvPacket recvPacket = new MeterProtocolRecvPacket();
            MeterProtocolSendPacket sendPacket = new MeterProtocolSendPacket()
            {
                SendData = byt_Frame,
                IsNeedReturn = (recvPacket != null)
            };

            bool bln_Result = base.SendData(sendPacket, recvPacket);//this.SendFrame(byt_Frame, int_WaitSecond, int_BitSecond);
            if (bln_Result)
            {
                if (CheckFrame(byt_Cmd, recvPacket.RecvData, ref bln_Sequela, ref byt_RevDataF))
                    return true;
            }
            return false;
            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
        }


        /// <summary>
        /// ִ���������(�з���)
        /// </summary>
        /// <param name="byt_Addr">��ַ</param>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">������</param>
        /// <param name="bln_Sequela">�Ƿ��к���֡</param>
        /// <param name="byt_RevDataF">����֡������</param>
        ///         /// <param name="byt_RevData">����֡����</param>
        /// <param name="int_WaitSecond">�ȴ�ʱ�䣨���룩,������������</param>
        /// <param name="int_BitSecond">�ֽڼ��ʱ�䣨���룩,������������</param>
        /// <returns></returns>
        public bool ExeCommand(byte[] byt_Addr, byte byt_Cmd, byte[] byt_Data, ref bool bln_Sequela,
                               ref byte[] byt_RevDataF,  int int_WaitSecond, int int_BitSecond,ref byte[] byt_RevData)
        {
            //try
            //{
            byte[] byt_Frame = this.OrgFrame(byt_Addr, byt_Cmd, byt_Data);
            MeterProtocolRecvPacket recvPacket = new MeterProtocolRecvPacket();
            MeterProtocolSendPacket sendPacket = new MeterProtocolSendPacket()
            {
                SendData = byt_Frame,
                IsNeedReturn = (recvPacket != null)
            };

            int retrytime = 2;
            for (int i = 0; i < retrytime; i++)
            {
                bool bln_Result = base.SendData(sendPacket, recvPacket);//this.SendFrame(byt_Frame, int_WaitSecond, int_BitSecond);
                //if (byt_Cmd == 0x11 && bln_Result == false)
                //{
                //    bln_Result = base.SendData(sendPacket, recvPacket);
                //}
                if (bln_Result)
                {
                    byt_RevData = recvPacket.RecvData;
                    if (CheckFrame(byt_Cmd, recvPacket.RecvData, ref bln_Sequela, ref byt_RevDataF))
                        return true;

                }
            }
            return false;
            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
        }
        /// <summary>
        /// ִ���������(�з���)
        /// </summary>
        /// <param name="byt_Addr">��ַ</param>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">������</param>
        /// <param name="bln_Sequela">�Ƿ��к���֡</param>
        /// <param name="byt_RevDataF">����֡������</param>
        /// <param name="int_WaitSecond">�ȴ�ʱ�䣨���룩,������������</param>
        /// <param name="int_BitSecond">�ֽڼ��ʱ�䣨���룩,������������</param>
        /// <returns></returns>
        public bool ExeCommand(byte[] byt_Addr, byte byt_Cmd, byte[] byt_Data, ref bool bln_Sequela,
                               ref byte[] byt_RevDataF, int int_WaitSecond, int int_BitSecond)
        {
            //try
            //{
            byte[] byt_Frame = this.OrgFrame(byt_Addr, byt_Cmd, byt_Data);
            MeterProtocolRecvPacket recvPacket = new MeterProtocolRecvPacket();
            MeterProtocolSendPacket sendPacket = new MeterProtocolSendPacket()
            {
                SendData = byt_Frame,
                IsNeedReturn = (recvPacket != null)
            };

            int retrytime = 2;
            for (int i = 0; i < retrytime; i++)
            {
                bool bln_Result = base.SendData(sendPacket, recvPacket);//this.SendFrame(byt_Frame, int_WaitSecond, int_BitSecond);
                //if (byt_Cmd == 0x11 && bln_Result == false)
                //{
                //    bln_Result = base.SendData(sendPacket, recvPacket);
                //}
                if (bln_Result)
                {
                    if (CheckFrame(byt_Cmd, recvPacket.RecvData, ref bln_Sequela, ref byt_RevDataF))
                        return true;

                }
            }
            return false;
            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
        }

        /// <summary>
        /// ִ���������(�з���)
        /// </summary>
        /// <param name="byt_Addr">��ַ</param>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">������</param>
        /// <param name="bln_Sequela">�Ƿ��к���֡</param>
        /// <param name="byt_RevDataF">����֡������</param>
        /// <param name="int_WaitSecond">�ȴ�ʱ�䣨���룩,������������</param>
        /// <param name="int_BitSecond">�ֽڼ��ʱ�䣨���룩,������������</param>
        /// <returns></returns>
        public bool ExeCommandByBlueTooth(byte[] byt_Addr, byte byt_Cmd, byte[] byt_Data, ref bool bln_Sequela,
                               ref byte[] byt_RevDataF)
        {
            //try
            //{
            byte[] byt_Frame = this.OrgFrame(byt_Addr, byt_Cmd, byt_Data);
            MeterProtocolRecvPacket recvPacket = new MeterProtocolRecvPacket();
            MeterProtocolSendPacket sendPacket = new MeterProtocolSendPacket()
            {
                SendData = byt_Frame,
                IsNeedReturn = (recvPacket != null)
            };

            int retrytime = 2;
            for (int i = 0; i < retrytime; i++)
            {
                bool bln_Result = base.SendData(sendPacket, recvPacket);//this.SendFrame(byt_Frame, int_WaitSecond, int_BitSecond);
                //if (byt_Cmd == 0x11 && bln_Result == false)
                //{
                //    bln_Result = base.SendData(sendPacket, recvPacket);
                //}
                if (bln_Result)
                {
                    if (CheckFrameByBlueTooth(byt_Cmd, recvPacket.RecvData, ref bln_Sequela, ref byt_RevDataF))
                        return true;

                }
            }
            return false;
            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
        }

        /// <summary>
        /// ִ���������(�з���)
        /// </summary>
        /// <param name="byt_Addr">��ַ</param>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">������</param>
        /// <param name="bln_Sequela">�Ƿ��к���֡</param>
        /// <param name="byt_RevDataF">����֡������</param>
        /// <param name="retrytime">���Դ���</param>
        /// <param name="int_WaitSecond">�ȴ�ʱ�䣨���룩,������������</param>
        /// <param name="int_BitSecond">�ֽڼ��ʱ�䣨���룩,������������</param>
        /// <returns></returns>
        public bool ExeCommand(byte[] byt_Addr, byte byt_Cmd, byte[] byt_Data, ref bool bln_Sequela,
                               ref byte[] byt_RevDataF, int retrytime, int int_WaitSecond, int int_BitSecond)
        {
            //try
            //{
            byte[] byt_Frame = this.OrgFrame(byt_Addr, byt_Cmd, byt_Data);
            MeterProtocolRecvPacket recvPacket = new MeterProtocolRecvPacket();
            MeterProtocolSendPacket sendPacket = new MeterProtocolSendPacket()
            {
                SendData = byt_Frame,
                //IsNeedReturn=(recvPacket!=null)
                IsNeedReturn = true
            };

            for (int i = 0; i < retrytime; i++)
            {
                bool bln_Result = base.SendData(sendPacket, recvPacket);//this.SendFrame(byt_Frame, int_WaitSecond, int_BitSecond);
                if (bln_Result)
                {
                    if (CheckFrame(byt_Cmd, recvPacket.RecvData, ref bln_Sequela, ref byt_RevDataF))
                        return true;
                }
            }
            return false;
            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
        }
       
        /// <summary>
        /// ִ���������(�з���)
        /// </summary>
        /// <param name="byt_Addr">��ַ</param>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">������</param>
        /// <param name="bln_Sequela">�Ƿ��к���֡</param>
        /// <param name="byt_RevAddr">����֡��ַ��</param>
        /// <param name="byt_RevDataF">����֡������</param>
        /// <param name="int_WaitSecond">�ȴ�ʱ�䣨���룩������������</param>
        /// <param name="int_BitSecond">�ֽڼ��ʱ�䣨���룩������������</param>
        /// <returns></returns>
        public bool ExeCommand(byte[] byt_Addr, byte byt_Cmd, byte[] byt_Data, ref bool bln_Sequela,
                               ref byte[] byt_RevAddr, ref byte[] byt_RevDataF, int int_WaitSecond,
                               int int_BitSecond)
        {
            //try
            //{
            byte[] byt_Frame = this.OrgFrame(byt_Addr, byt_Cmd, byt_Data);
            MeterProtocolRecvPacket recvPacket = new MeterProtocolRecvPacket();
            MeterProtocolSendPacket sendPacket = new MeterProtocolSendPacket()
            {
                SendData = byt_Frame,
            };

            bool bln_Result = SendData(sendPacket, recvPacket);//this.SendFrame(byt_Frame, int_WaitSecond, int_BitSecond);
            if (bln_Result)
            {
                if (CheckFrame(byt_Cmd, recvPacket.RecvData, ref bln_Sequela, ref byt_RevAddr, ref byt_RevDataF))
                    return true;
            }
            return false;
            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
        }


        /// <summary>
        /// ִ���������(�з���)
        /// </summary>
        /// <param name="by_sendata">��������</param>
        /// <param name="re">�������ݰ�</param>
        /// <param name="retrytime">���Դ���</param>
        /// <returns></returns>
        public bool ExeCommand(byte[] by_sendata, CLDC_DataCore.SocketModule.Packet.RecvPacket re, int retrytime)
        {

            MeterProtocolRecvPacket recvPacket = new MeterProtocolRecvPacket();
            MeterProtocolSendPacket sendPacket = new MeterProtocolSendPacket()
            {
                SendData = by_sendata,
                IsNeedReturn = true
            };

            for (int i = 0; i < retrytime; i++)
            {
                bool bln_Result = base.SendData(sendPacket, recvPacket);
                if (bln_Result)
                {
                    byte[] getData = recvPacket.RecvData;
                    return true;
                }
            }
            return false;

        }

        /// <summary>
        /// ִ���������(�޷���)
        /// </summary>
        /// <param name="byt_Addr">��ַ</param>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_Data">������</param>
        /// <returns></returns>
        public bool ExeCommand(byte[] byt_Addr, byte byt_Cmd, byte[] byt_Data)
        {
            //try
            //{
            byte[] byt_Frame = this.OrgFrame(byt_Addr, byt_Cmd, byt_Data);
            MeterProtocolSendPacket sendPacket = new MeterProtocolSendPacket()
            {
                SendData = byt_Frame,
            };
            MeterProtocolRecvPacket recvPacket = null;//�ظ�������ΪNULL������Ҫ�ظ�
            return SendData(sendPacket, recvPacket);
            //return SendFrame(byt_Frame, 600, 500);
            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
        }

        /// <summary>
        /// 645��֡
        /// </summary>
        /// <param name="byt_Addr">��ַ��</param>
        /// <param name="byt_Cmd">����</param>
        /// <param name="byt_Data">������</param>
        /// <returns></returns>
        private byte[] OrgFrame(byte[] byt_Addr, byte byt_Cmd, byte[] byt_Data)
        {

            byte byt_Len = (byte)byt_Data.Length;
            byte[] byt_Frame = new byte[byt_Len + 12 + this.protocolInfo.FECount];  //68H(1)+��ַ(6)+68H(1)+������(1)+Len(1)+Data(Len)+ChkSum(1)+16H(1)  
            if (this.protocolInfo.FECount > 0)
                for (int int_Inc = 0; int_Inc < this.protocolInfo.FECount; int_Inc++)
                    byt_Frame[int_Inc] = 0xFE;
            byt_Frame[this.protocolInfo.FECount + 0] = 0x68;
            Array.Copy(byt_Addr, 0, byt_Frame, protocolInfo.FECount + 1, 6);
            byt_Frame[this.protocolInfo.FECount + 7] = 0x68;
            byt_Frame[this.protocolInfo.FECount + 8] = byt_Cmd;
            byt_Frame[this.protocolInfo.FECount + 9] = byt_Len;

            for (int int_Inc = 0; int_Inc < byt_Len; int_Inc++)
                byt_Frame[this.protocolInfo.FECount + 10 + int_Inc] = Convert.ToByte((byt_Data[int_Inc] + 0x33) % 256);
            for (int int_Inc = 0; int_Inc < byt_Len + 10; int_Inc++)
            {
                if (GlobalUnit.IsErrChkSum)////�����У�飬add by wzs 20200417 begin
                {
                    byt_Frame[this.protocolInfo.FECount + 10 + byt_Len] += byt_Frame[this.protocolInfo.FECount + int_Inc+1];
                }
                else
                {
                    byt_Frame[this.protocolInfo.FECount + 10 + byt_Len] += byt_Frame[this.protocolInfo.FECount + int_Inc];
                }
            }
             
            byt_Frame[this.protocolInfo.FECount + 11 + byt_Len] = 0x16;
            //Console.WriteLine(BitConverter.ToString(byt_Frame));
            return byt_Frame;
        }



        /// <summary>
        /// ��������֡
        /// </summary>
        /// <param name="byt_Cmd">�·�������</param>
        /// <param name="byt_RevFrame">��������֡</param>
        /// <param name="bln_Sequela">�Ƿ��к���֡</param>
        /// <param name="byt_Addr">���ص�ַ��</param>
        /// <param name="byt_RevData">����������</param>
        /// <returns></returns>
        private bool CheckFrame(byte byt_Cmd, byte[] byt_RevFrame, ref bool bln_Sequela, ref byte[] byt_Addr, ref byte[] byt_RevData)
        {

            if (byt_RevFrame == null || byt_RevFrame.Length <= 0)
            {
                //this.m_str_LostMessage = "û�з������ݣ�";
                return false;
            }
            int int_Start = 0;
            int_Start = Array.IndexOf(byt_RevFrame, (byte)0x68);
            if (int_Start < 0 || int_Start > byt_RevFrame.Length || int_Start + 12 > byt_RevFrame.Length) //û��68��ͷ �����Ƿ��㹻һ֡ �Ƿ�����
            {
                //this.m_str_LostMessage = "����֡��������û��֡ͷ��[" + BitConverter.ToString(byt_RevFrame) + "]";
                return false;
            }

            if (byt_RevFrame[int_Start + 7] != 0x68)        //�Ҳ����ڶ���68
            {
                //this.m_str_LostMessage = "����֡��������[" + BitConverter.ToString(byt_RevFrame) + "]";
                return false;
            }
            int int_Len = byt_RevFrame[int_Start + 9];
            if (int_Start + 12 + int_Len != byt_RevFrame.Length)
            {
                //this.m_str_LostMessage = "���ݳ�����ʵ�ʳ��Ȳ�һ�£�[" + BitConverter.ToString(byt_RevFrame) + "]";
                return false;                //֡�ĳ����Ƿ���ʵ�ʳ���һ��
            }
            byte byt_Chksum = 0;
            for (int int_Inc = int_Start; int_Inc < int_Start + int_Len + 10; int_Inc++)
                byt_Chksum += byt_RevFrame[int_Inc];
            if (byt_RevFrame[int_Start + int_Len + 10] != byt_Chksum)       //У���벻��ȷ
            {
                //this.m_str_LostMessage = "����У���벻��ȷ��[" + BitConverter.ToString(byt_RevFrame) + "]";
                return false;
            }
            if (byt_RevFrame[int_Start + int_Len + 11] != 0x16)       //û��16����
            {
                //this.m_str_LostMessage = "����֡��������[" + BitConverter.ToString(byt_RevFrame) + "]";
                return false;
            }

            Array.Resize(ref byt_Addr, 6);
            Array.Copy(byt_RevFrame, int_Start + 1, byt_Addr, 0, 6);
            //cmd
            Array.Resize(ref byt_RevData, int_Len);    //�����򳤶�
            if (int_Len > 0)
            {
                Array.Copy(byt_RevFrame, int_Start + 10, byt_RevData, 0, int_Len);
                for (int int_Inc = 0; int_Inc < byt_RevData.Length; int_Inc++)
                    byt_RevData[int_Inc] -= 0x33;
            }

            if ((byt_RevFrame[int_Start + 8] & 0x1f) != byt_Cmd)
            {
                //this.m_str_LostMessage = "����֡�������·�֡��һ�£�[" + BitConverter.ToString(byt_RevFrame) + "]";
                return false;
            }

            //�Ƿ��к���֡
            if ((byt_RevFrame[int_Start + 8] & 0x20) == 0x20)
                bln_Sequela = true;
            else
                bln_Sequela = false;

            //�Ƿ񷵻ز����ɹ�     ��7Bit��1���Ƿ��أ���6bit��0=�ɹ���1=ʧ��
            if ((byt_RevFrame[int_Start + 8] & 0x80) == 0x80 && (byt_RevFrame[int_Start + 8] & 0x40) == 0x00)
                return true;
            else
            {
                //if (byt_RevData != null && byt_RevData.Length > 0)
                //this.m_str_LostMessage = GetErrorMsg6451997(byt_RevData[0])+"[" + BitConverter.ToString(byt_RevFrame) + "]";
                //else
                //this.m_str_LostMessage = "���ز���ʧ�ܣ�[" + BitConverter.ToString(byt_RevFrame) + "]";
                return false;
            }
        }



        /// <summary>
        /// ��������֡
        /// </summary>
        /// <param name="byt_Cmd">�·�������</param>
        /// <param name="byt_RevFrame">���֡</param>
        /// <param name="bln_Sequela">�Ƿ��к���֡</param>
        /// <param name="byt_RevData">����������</param>
        /// <returns></returns>
        private bool CheckFrame(byte byt_Cmd, byte[] byt_RevFrame, ref bool bln_Sequela, ref byte[] byt_RevData)
        {
            byte[] byt_Addr = new byte[6];
            return CheckFrame(byt_Cmd, byt_RevFrame, ref bln_Sequela, ref byt_Addr, ref  byt_RevData);
        }

        /// <summary>
        /// ��������֡
        /// </summary>
        /// <param name="byt_Cmd">�·�������</param>
        /// <param name="byt_RevFrame">���֡</param>
        /// <param name="bln_Sequela">�Ƿ��к���֡</param>
        /// <param name="byt_RevData">����������</param>
        /// <returns></returns>
        private bool CheckFrameByBlueTooth(byte byt_Cmd, byte[] byt_RevFrame, ref bool bln_Sequela, ref byte[] byt_RevData)
        {
            byte[] byt_Addr = new byte[6];
            return CheckFrameByBlueTooth(byt_Cmd, byt_RevFrame, ref bln_Sequela, ref byt_Addr, ref  byt_RevData);
        }

        /// <summary>
        /// ��������֡
        /// </summary>
        /// <param name="byt_Cmd">�·�������</param>
        /// <param name="byt_RevFrame">��������֡</param>
        /// <param name="bln_Sequela">�Ƿ��к���֡</param>
        /// <param name="byt_Addr">���ص�ַ��</param>
        /// <param name="byt_RevData">����������</param>
        /// <returns></returns>
        private bool CheckFrameByBlueTooth(byte byt_Cmd, byte[] byt_RevFrame, ref bool bln_Sequela, ref byte[] byt_Addr, ref byte[] byt_RevData)
        {

            if (byt_RevFrame == null || byt_RevFrame.Length <= 0)
            {
                //this.m_str_LostMessage = "û�з������ݣ�";
                return false;
            }
            int int_Start = 0;
            int_Start = Array.IndexOf(byt_RevFrame, (byte)0x68);
            if (int_Start < 0 || int_Start > byt_RevFrame.Length || int_Start + 12 > byt_RevFrame.Length) //û��68��ͷ �����Ƿ��㹻һ֡ �Ƿ�����
            {
                //this.m_str_LostMessage = "����֡��������û��֡ͷ��[" + BitConverter.ToString(byt_RevFrame) + "]";
                return false;
            }

            if (byt_RevFrame[int_Start + 7] != 0x68)        //�Ҳ����ڶ���68
            {
                //this.m_str_LostMessage = "����֡��������[" + BitConverter.ToString(byt_RevFrame) + "]";
                return false;
            }
            int int_Len = byt_RevFrame[int_Start + 9];
            if (int_Start + 12 + int_Len != byt_RevFrame.Length)
            {
                //this.m_str_LostMessage = "���ݳ�����ʵ�ʳ��Ȳ�һ�£�[" + BitConverter.ToString(byt_RevFrame) + "]";
                return false;                //֡�ĳ����Ƿ���ʵ�ʳ���һ��
            }
            byte byt_Chksum = 0;
            for (int int_Inc = int_Start; int_Inc < int_Start + int_Len + 10; int_Inc++)
                byt_Chksum += byt_RevFrame[int_Inc];
            if (byt_RevFrame[int_Start + int_Len + 10] != byt_Chksum)       //У���벻��ȷ
            {
                //this.m_str_LostMessage = "����У���벻��ȷ��[" + BitConverter.ToString(byt_RevFrame) + "]";
                return false;
            }
            if (byt_RevFrame[int_Start + int_Len + 11] != 0x16)       //û��16����
            {
                //this.m_str_LostMessage = "����֡��������[" + BitConverter.ToString(byt_RevFrame) + "]";
                return false;
            }

            Array.Resize(ref byt_Addr, 6);
            Array.Copy(byt_RevFrame, int_Start + 1, byt_Addr, 0, 6);
            //cmd
            Array.Resize(ref byt_RevData, int_Len);    //�����򳤶�
            if (int_Len > 0)
            {
                Array.Copy(byt_RevFrame, int_Start + 10, byt_RevData, 0, int_Len);
                for (int int_Inc = 0; int_Inc < byt_RevData.Length; int_Inc++)
                    byt_RevData[int_Inc] -= 0x33;
            }

            //if ((byt_RevFrame[int_Start + 8] & 0x1f) != byt_Cmd)
            //{
            //    //this.m_str_LostMessage = "����֡�������·�֡��һ�£�[" + BitConverter.ToString(byt_RevFrame) + "]";
            //    return false;
            //}

            //�Ƿ��к���֡
            //if ((byt_RevFrame[int_Start + 8] & 0x20) == 0x20)
            //    bln_Sequela = true;
            //else
            //    bln_Sequela = false;

            //�Ƿ񷵻ز����ɹ�     ��7Bit��1���Ƿ��أ���6bit��0=�ɹ���1=ʧ��
            if ((byt_RevFrame[int_Start + 8]) == 0x11)
                return true;
            else
            {
                //if (byt_RevData != null && byt_RevData.Length > 0)
                //this.m_str_LostMessage = GetErrorMsg6451997(byt_RevData[0])+"[" + BitConverter.ToString(byt_RevFrame) + "]";
                //else
                //this.m_str_LostMessage = "���ز���ʧ�ܣ�[" + BitConverter.ToString(byt_RevFrame) + "]";
                return false;
            }
        }

        public override string ReadData(string sendData)
        {
            string str_Value = "";
            MeterProtocolRecvPacket recvPacket = new MeterProtocolRecvPacket();
            bool bln_Result = this.ExeCommand(BitConverter.GetBytes(Convert.ToInt64(sendData, 16)), recvPacket, 3);
            if (bln_Result)
            {
                for (int i = 0; i < recvPacket.RecvData.Length; i++)
                {
                    str_Value += Convert.ToChar(recvPacket.RecvData[i]);
                }
            }
            return str_Value;
        }



    }
}
