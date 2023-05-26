using System;
//using ClInterface;
namespace CLDC_MeterProtocol.Protocols
{

    /// <summary>
    /// ElSTER(ABB)��ALPHAЭ�����
    /// </summary>
    public class CAlpha:ProtocolBase
    {

        

        protected CAlphaFrame m_clf_Frame;
        //private string m_str_Setting = "1200,e,8,1";            //������
        //private ISerialport m_Ispt_com;                         //���ƶ˿�
        //protected byte[] m_byt_RevData;                           //��������
        //public string m_str_LostMessage = "";                   //����ʧ����Ϣ
        //public bool m_bln_Enabled = true;                       //

        //protected string m_str_RxFrame = "";
        //protected string m_str_TxFrame = "";
        //int protocolInfo.FECount = 0;

        //private object m_obj_LockRev = new object();        //������������ 


        public CAlpha()
        {
            //arrRecv = new byte[0];
            this.m_clf_Frame = new CAlphaFrame();
            


        }


        //public event Dge_EventRxFrame OnEventRxFrame;
        //public event Dge_EventTxFrame OnEventTxFrame;


        /// <summary>
        /// ������崮��
        /// </summary>
        //public ISerialport ComPort
        //{
        //    get
        //    {
        //        return this.m_Ispt_com;
        //    }
        //    set
        //    {
        //        if (!value.Equals(this.m_Ispt_com))
        //        {
        //            if (this.m_Ispt_com != null)
        //            {
        //                this.m_Ispt_com.DataReceive -= new RevEventDelegete(m_Ispt_com_DataReceive);
        //            }
        //            this.m_Ispt_com = value;
        //            this.m_Ispt_com.DataReceive += new RevEventDelegete(m_Ispt_com_DataReceive);
        //        }
        //    }
        //}

        ///// <summary>
        ///// ������
        ///// </summary>
        //public string Setting
        //{
        //    get
        //    {
        //        return this.m_str_Setting;
        //    }
        //    set
        //    {
        //        this.m_str_Setting = value;
        //    }
        //}

        ///// <summary>
        ///// ����֡
        ///// </summary>
        //public string RxFrame
        //{
        //    get { return this.m_str_RxFrame; }
        //}

        ///// <summary>
        ///// �·�֡
        ///// </summary>
        //public string TxFrame
        //{
        //    get { return this.m_str_TxFrame; }
        //}


        

        ///// <summary>
        ///// �·�֡�Ļ��ѷ�����
        ///// </summary>
        //public int FECount
        //{
        //    get { return this.protocolInfo.FECount; }
        //    set { this.protocolInfo.FECount = value; }
        //}



        /// <summary>
        /// ����ͨ��
        /// </summary>
        /// <param name="str_Addr">��ַ</param>
        /// <param name="byt_Key">�����ܳ�</param>
        /// <returns></returns>
        protected bool RequestComm(string str_Addr,ref byte []byt_Key)
        {
            try
            {
                String str_Tmp = str_Addr;
                str_Tmp = str_Tmp.PadLeft(2, '0');
                byte[] byt_Data = new byte[1];

                byt_Data[0] = Convert.ToByte(str_Tmp.Substring(str_Tmp.Length - 2, 2));

                if (byt_Data[0] == 0)           //����ַ��0ʱ����Ϊ100
                    byt_Data[0] = 100;

                byte[] byt_Frame = this.m_clf_Frame.OrgFrame(CAlphaFrame.CST_BYT_CB_DAT,
                                                             CAlphaFrame.CST_BYT_FUN_WHO,
                                                             byt_Data);
                for (int int_Inc = 0; int_Inc < 10; int_Inc++)
                {
                    byte[] arrRecv = new byte[0];

                    bool bln_Result = SendData(byt_Frame, ref arrRecv);// SendFrame(byt_Frame, 400, 300);
                    if (bln_Result)
                    {
                        if (arrRecv.Length > 0)                                  //�Ƿ������ݷ���
                        {
                            if (this.m_clf_Frame.CheckFrame(arrRecv, ref byt_Data))
                            {
                                if (byt_Data.Length == 15)
                                {
                                    byt_Key = new byte[4];
                                    Array.Copy(byt_Data, 9, byt_Key, 0, 4);
                                    //this.m_str_LostMessage = "";
                                    return true;
                                }
                                //else
                                    //this.m_str_LostMessage = "����ʧ��";
                            }
                           // else
                                //this.m_str_LostMessage = "���ַ���֡������Ҫ��";
                        }
                        //else
                            //this.m_str_LostMessage = "����û�з�������";
                    }
                    //else
                        //this.m_str_LostMessage = "��������֡ʧ��";
                }
                return false;
            }
            catch (Exception e)
            {
                CLDC_DataCore.Const.GlobalUnit.g_MsgControl.OutMessage(e.Message, false);
                //this.m_str_LostMessage = e.ToString();
                return false;
            }
        }


        /// <summary>
        /// ������֤
        /// </summary>
        /// <param name="str_Password">����</param>
        /// <param name="byt_Key">�ܳ�</param>
        /// <returns></returns>
        protected bool VerifyPassword(string str_Password, byte[] byt_Key)
        {
            try
            {
                byte [] byt_Pswd=BitConverter.GetBytes(Convert.ToInt32(str_Password,16 ));

                
                byte[] byt_EPswd = this.EncryptPassword(byt_Pswd, byt_Key);
                byte[] byt_Frame = this.m_clf_Frame.OrgFrame(CAlphaFrame.CST_BYT_CB_DAT,
                                                             CAlphaFrame.CST_BYT_FUN_PSW,
                                                             byt_EPswd);
                byte[] arrRecv = new byte[0];

                bool bln_Result = SendData(byt_Frame, ref arrRecv);// this.SendFrame(byt_Frame, 800, 500);
                if (arrRecv.Length > 0)                                  //�Ƿ������ݷ���
                {
                    byte[] byt_Data = new byte[0];
                    if (this.m_clf_Frame.CheckFrame(arrRecv, ref byt_Data))
                    {
                        if (byt_Data.Length == 6)
                        {
                            if (byt_Data[2] == CAlphaFrame.CST_BYT_ACK)
                                return true;
                            //else
                                //this.m_str_LostMessage = GetNAKString(byt_Data[2]);
                        }
                        //else
                            //this.m_str_LostMessage = "������֤ʧ��";
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                CLDC_DataCore.Const.GlobalUnit.g_MsgControl.OutMessage(e.Message, false);

                //this.m_str_LostMessage = e.ToString();
                return false;
            }
        }


        /// <summary>
        /// �˳�ͨ������
        /// </summary>
        /// <returns></returns>
        protected bool CloseComm()
        {
            byte[] byt_Frame = new byte[] { 0x02, 0x80, 0xF7, 0xEA };
            for (int int_Inc = 0; int_Inc < 2; int_Inc++)
            {
                byte[] arrRecv = null;
                SendData(byt_Frame,ref arrRecv);               
               // bool bln_Result = SendFrame(byt_Frame, 600, 400);
            }
            return true;
        }


        /// <summary>
        /// ��ȡ������
        /// </summary>
        /// <param name="byt_Class">����</param>
        /// <param name="byt_RevData">��������</param>
        /// <returns></returns>
        protected bool ReadClass(byte byt_Class, ref byte[] byt_RevData)
        {

            byte[] byt_Frame = this.m_clf_Frame.OrgFrame(CAlphaFrame.CST_BYT_CB_CAS, byt_Class);
            byte[] arrRecv = new byte[0];

            bool bln_Result = SendData(byt_Frame, ref arrRecv);// this.SendFrame(byt_Frame, 1200, 400);
            if (arrRecv.Length > 0)                                  //�Ƿ������ݷ���
            {
                byte[] byt_Data = new byte[0];
                if (this.m_clf_Frame.CheckFrame(arrRecv, ref byt_Data))
                {
                    if (byt_Data.Length >= 7)
                    {
                        if (byt_Data[1] == CAlphaFrame.CST_BYT_CB_CAS && byt_Data[2] == CAlphaFrame.CST_BYT_ACK)
                        {
                            bool bln_Continue =  ((byt_Data[4] & 0x80) != 0x80);         //�Ƿ��к���֡
                            int int_Len = byt_Data[4] & 0x7f;                           //֡�������򳤶�
                            byt_RevData = new byte[int_Len];                            //������
                            Array.Copy(byt_Data, 5, byt_RevData, 0, int_Len);
                            if (!bln_Continue)
                                return true;

                            while (bln_Continue)
                            {
                                byte[] byt_TmpData = new byte[0];
                                bln_Result = ReadContinueData(ref byt_TmpData, ref bln_Continue);
                                if (bln_Result)
                                {
                                    int_Len = byt_TmpData.Length;
                                    int int_Old = byt_RevData.Length;
                                    if (int_Len > 0)
                                    {
                                        Array.Resize(ref byt_RevData, byt_RevData.Length + int_Len);
                                        Array.Copy(byt_TmpData, 0, byt_RevData, int_Old, int_Len);
                                    }
                                }
                                else
                                    return false;
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        protected bool ReadContinueData(ref byte[] byt_RevData, ref bool bln_Continue)
        {
            byte []byt_Frame = new byte[] { 0x02, 0x81, 0xE7, 0xCB };
            byte[] arrRecv = new byte[0];

            bool bln_Result = SendData(byt_Frame, ref arrRecv);// this.SendFrame(byt_Frame, 1200, 400);
            if (arrRecv.Length > 0)                                  //�Ƿ������ݷ���
            {
                byte[] byt_Data = new byte[0];
                if (this.m_clf_Frame.CheckFrame(arrRecv, ref byt_Data))
                {
                    if (byt_Data[1] == CAlphaFrame.CST_BYT_CB_CNE && byt_Data[2] == CAlphaFrame.CST_BYT_ACK)
                    {
                        bln_Continue = ((byt_Data[4] & 0x80) != 0x80);         //�Ƿ��к���֡
                        int int_Len = byt_Data[4] & 0x7f;
                        byt_RevData = new byte[int_Len];
                        Array.Copy(byt_Data, 5, byt_RevData, 0, int_Len);
                        return true;
                    }
                }
            }
            return false;
        }

        //protected bool SendFrame(byte[] byt_Frame, int int_MinSecond, int int_SpaceMSecond)
        //{
        //    try
        //    {
        //        if (this.m_Ispt_com.Setting != this.m_str_Setting)
        //            this.m_Ispt_com.Setting = this.m_str_Setting;

        //        //DisposeTxEvent DspTxFrame = new DisposeTxEvent(AcyDspTxFrame);
        //        //DspTxFrame(BitConverter.ToString(byt_Frame));

        //        //arrRecv = new byte[0];
        //        //this.m_Ispt_com.SendData(byt_Frame);                                //��������
        //        //Waiting(int_MinSecond, int_SpaceMSecond);                                                  //�ȴ���������
                
        //        //DisposeRxEvent DspRxFrame = new DisposeRxEvent(AcyDspRxFrame);
        //        //DspRxFrame(BitConverter.ToString(m_byt_RevData));
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        //this.m_str_LostMessage = e.Message;
        //        return false;
        //    }
        //}


        //private void AcyDspRxFrame(string str_Frame)
        //{
        //    this.m_str_RxFrame = str_Frame;
        //    if (this.OnEventRxFrame != null) this.OnEventRxFrame(str_Frame);
        //}

        //private void AcyDspTxFrame(string str_Frame)
        //{
        //    this.m_str_TxFrame = str_Frame;
        //    if (this.OnEventTxFrame != null) this.OnEventTxFrame(str_Frame);
        //}



        protected string GetNAKString(byte byt_Code)
        {
            //1�� NAK��CRCУ���
            //2�� NAK���ù���Ϊͨ������
            //3�� NAK�����Ϸ������ͬ���򳤶�
            //4�� NAK��֡����
            //5�� NAK����ʱ
            //6�� NAK����Ч����
            //7�� NAK���������Ӧ��
            //E�� NAK��IEC Cģʽͨ�ű���

            switch (byt_Code)
            {
                case CAlphaFrame.CST_BYT_ACK:
                    return "";
                case CAlphaFrame.CST_BYT_NAK_CRC:
                    return "CRCУ���";
                case CAlphaFrame.CST_BYT_NAK_CMM:
                    return "�ù���Ϊͨ������";
                case CAlphaFrame.CST_BYT_NAK_CMD:
                    return "���Ϸ������ͬ���򳤶�";
                case CAlphaFrame.CST_BYT_NAK_FRM:
                    return "֡����";
                case CAlphaFrame.CST_BYT_NAK_OTM:
                    return "��ʱ";
                case CAlphaFrame.CST_BYT_NAK_PSW:
                    return "��Ч����";
                case CAlphaFrame.CST_BYT_NAK_NAS:
                    return "�������Ӧ��";
                case CAlphaFrame.CST_BYT_NAK_CCL:
                    return "IEC Cģʽͨ�ű���";
                default:
                    return "δ֪����";
            }
        }




        #region-------------˽�к���------------------------------

        /// <summary>
        /// �ȴ����ݷ���
        /// </summary>
        /// <param name="int_MinSecond">�ȴ�����ʱ��</param>
        /// <param name="int_SpaceMSecond">�ȴ�����֡�ֽڼ��ʱ��</param>
        //private void Waiting(int int_MinSecond, int int_SpaceMSecond)
        //{
        //    try
        //    {
        //        int int_OldLen = 0;
        //        Stopwatch sth_Ticker = new Stopwatch();                     //�ȴ���ʱ��
        //        Stopwatch sth_SpaceTicker = new Stopwatch();                //
        //        sth_SpaceTicker.Start();
        //        sth_Ticker.Start();
        //        while (this.m_bln_Enabled )
        //        {
        //            System.Windows.Forms.Application.DoEvents();
        //            if (arrRecv.Length > int_OldLen)     //�����иı�
        //            {
        //                sth_SpaceTicker.Reset();
        //                int_OldLen = arrRecv.Length;
        //                sth_SpaceTicker.Start();                    //�ֽڼ��ʱ���¿�ʼ
        //            }
        //            else        //���������û�����ӣ���ǰ���յ�����ʱ���500�������˳�
        //            {
        //                if (arrRecv.Length > 0)      //�Ѿ��յ�һ���֣����ֽڼ��ʱ
        //                {
        //                    if (sth_SpaceTicker.ElapsedMilliseconds >= int_SpaceMSecond)
        //                        break;
        //                }
        //            }
        //            if (sth_Ticker.ElapsedMilliseconds >= int_MinSecond)        //�ܼ�ʱ
        //                break;
        //            System.Threading.Thread.Sleep(1);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        //this.m_str_LostMessage = e.Message;
        //    }
        //}


        ///// <summary>
        ///// �ȴ����ݷ���
        ///// </summary>
        ///// <param name="int_SpaceMSecond">�ȴ�����֡�ֽڼ��ʱ��</param>
        //private void Waiting( int int_SpaceMSecond)
        //{
        //    try
        //    {
        //        int int_OldLen = 0;
        //        Stopwatch sth_Ticker = new Stopwatch();                     //�ȴ���ʱ��
        //        Stopwatch sth_SpaceTicker = new Stopwatch();                //
        //        sth_SpaceTicker.Start();
        //        sth_Ticker.Start();
        //        while (this.m_bln_Enabled )
        //        {
        //            System.Windows.Forms.Application.DoEvents();
        //            if (arrRecv.Length > int_OldLen)     //�����иı�
        //            {
        //                sth_SpaceTicker.Reset();
        //                int_OldLen = arrRecv.Length;
        //                sth_SpaceTicker.Start();                    //�ֽڼ��ʱ���¿�ʼ
        //            }
        //            else        //���������û�����ӣ���ǰ���յ�����ʱ���500�������˳�
        //            {
        //                if (arrRecv.Length > 0)      //�Ѿ��յ�һ���֣����ֽڼ��ʱ
        //                {
        //                    if (sth_SpaceTicker.ElapsedMilliseconds >= int_SpaceMSecond)
        //                        break;
        //                }
        //            }
        //            if (sth_Ticker.ElapsedMilliseconds >= 2000)        //�ܼ�ʱ
        //                break;
        //            System.Threading.Thread.Sleep(1);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        //this.m_str_LostMessage = e.Message;
        //    }
        //}

        

      



        ///// <summary>
        ///// ���ڷ�������
        ///// </summary>
        ///// <param name="bData"></param>
        //private void m_Ispt_com_DataReceive(byte[] bData)
        //{
        //    lock (this.m_obj_LockRev)
        //    {
        //        int int_Len = bData.Length;
        //        int int_OldLen = arrRecv.Length;
        //        Array.Resize(ref arrRecv, int_Len + int_OldLen);
        //        Array.Copy(bData, 0, arrRecv, int_OldLen, int_Len);
        //    }
        //}



        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="byt_Password">����</param>
        /// <param name="byt_Key">�ܳ�</param>
        /// <returns></returns>
        public byte[] EncryptPassword(byte[] byt_Password, byte[] byt_Key)
        {
            //����AlphaЭ���еļ��ܿ����ּ���Դ����   �����ܳ�key��Զ��ͨѶ������������ܿ�����
            UInt32 unt_Psw = BitConverter.ToUInt32(byt_Password, 0);
            
            Array.Reverse(byt_Key);
            UInt32 unt_Key = BitConverter.ToUInt32(byt_Key, 0);
            unt_Key += 0xab41;
            byte[] byt_TmpKey = BitConverter.GetBytes(unt_Key);
            int int_Count = byt_TmpKey[0] + byt_TmpKey[1] + byt_TmpKey[2] + byt_TmpKey[3];
            int_Count &= 0x0f;
            int int_K = 0;
            int int_J = 0;
            while (int_Count >= 0)
            {
                if (byt_TmpKey[3] >= 0x80)
                    int_J = 1;
                else
                    int_J = 0;
                unt_Key = unt_Key << 1;
                unt_Key += (UInt32)int_K;
                int_K = int_J;
                unt_Psw ^= unt_Key;
                int_Count--;
                byt_TmpKey = BitConverter.GetBytes(unt_Key);
            }
            byte[] byt_TmpPsw = BitConverter.GetBytes(unt_Psw);
            Array.Reverse(byt_TmpPsw);
            return byt_TmpPsw;
        }




        #endregion






}


    }

