using System;

namespace CLDC_MeterProtocol.Protocols
{
    /// <summary>
    /// AlphaЭ��֡
    /// </summary>
    public class CAlphaFrame
    {

        #region -----------�ؼ��֣���������-------------------------
        /// <summary>
        /// ֡��ʼ��
        /// </summary>
        public const byte CST_BYT_STX = 0x02;           //֡��ʼ��

        /// <summary>
        /// �����ɹ�
        /// </summary>
        public const byte CST_BYT_ACK = 0x00;           //�����ɹ�

        //-----------NAK�ؼ��֣�ʧ�ܴ���---------------------------

        /// <summary>
        /// У�����
        /// </summary>
        public const byte CST_BYT_NAK_CRC = 0x01;       //CRCУ�����

        /// <summary>
        /// ͨ����������
        /// </summary>
        public const byte CST_BYT_NAK_CMM = 0x02;       //ͨ����������

        /// <summary>
        /// ���Ϸ�����
        /// </summary>
        public const byte CST_BYT_NAK_CMD = 0x03;       //���Ϸ�����

        /// <summary>
        /// ֡����
        /// </summary>
        public const byte CST_BYT_NAK_FRM = 0x04;       //֡����

        /// <summary>
        /// ��ʱ
        /// </summary>
        public const byte CST_BYT_NAK_OTM = 0x05;       //��ʱ

        /// <summary>
        /// ��Ч����
        /// </summary>
        public const byte CST_BYT_NAK_PSW = 0x06;       //��Ч����

        /// <summary>
        /// û�лش�
        /// </summary>
        public const byte CST_BYT_NAK_NAS = 0x07;       //û�лش�

        /// <summary>
        /// Cģʽ�ر�
        /// </summary>
        public const byte CST_BYT_NAK_CCL = 0x0E;       //Cģʽ�ر�

        //----------CB�ؼ���-----------------------------
        /// <summary>
        /// �����Ի�
        /// </summary>
        public const byte CST_BYT_CB_END = 0x80;        //�����Ի�

        /// <summary>
        /// ����
        /// </summary>
        public const byte CST_BYT_CB_CNE = 0x81;        //����

        /// <summary>
        /// �ٷ���һ���
        /// </summary>
        public const byte CST_BYT_CB_AGN = 0x82;        //�ٷ���һ���

        /// <summary>
        /// ���ܿ���
        /// </summary>
        public const byte CST_BYT_CB_ACE = 0x84;        //���ܿ���

        /// <summary>
        /// �������йز���
        /// </summary>
        public const byte CST_BYT_CB_DAT = 0x18;        //�������йز���

        /// <summary>
        /// �������йز���
        /// </summary>
        public const byte CST_BYT_CB_NDAT = 0x08;        //�������޹ز���

    
        /// <summary>
        /// �����
        /// </summary>
        public const byte CST_BYT_CB_CAS = 0x05;        //�����

        /// <summary>
        /// ����춨
        /// </summary>
        public const byte CST_BYT_FUN_PSW = 0x01;        //����춨

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public const byte CST_BYT_FUN_SET = 0x02;        //����ʱ��

        /// <summary>
        /// ����˭
        /// </summary>
        public const byte CST_BYT_FUN_WHO = 0x06;        //����˭

        /// <summary>
        /// �ؽ�����
        /// </summary>
        public const byte CST_BYT_FUN_BCK = 0x08;        //�ؽ�����

        /// <summary>
        /// ����ߴ�(֡���ݳ���)
        /// </summary>
        public const byte CST_BYT_FUN_PAK = 0x09;        //����ߴ�(֡���ݳ���)

        /// <summary>
        /// д������仺����
        /// </summary>
        public const byte CST_BYT_FUN_ERA = 0x0A;        //д������仺����

        /// <summary>
        /// ʱ��ͬ��
        /// </summary>
        public const byte CST_BYT_FUN_SYN = 0x0C;        //ʱ��ͬ��


        /// <summary>
        /// F2 ͨ�ų�ʱ�ż�ֵ ����Ϊ1�ֽڣ�6~255������Ϊ 0.5�룩
        /// </summary>
        public const byte CST_BYT_FUN_COST = 0xF2;        //ͨ�ų�ʱ�ż�ֵ



        #endregion

        public CAlphaFrame()
        {




        }

        /// <summary>
        /// ��֡������Ϣ��ʽ
        /// </summary>
        /// <returns></returns>
        public byte[] OrgFrame(byte byt_Cmd)
        {
            byte[] byt_Frame = new byte[4];
            byt_Frame[0]=CST_BYT_STX ;
            byt_Frame[1] = byt_Cmd;
            Array.Copy(GetCRC(byt_Frame, 0, 2), 0, byt_Frame, 2, 2);
            return byt_Frame;
        }


        /// <summary>
        /// ��֡������Ϣ��ʽ
        /// </summary>
        /// <returns></returns>
        public byte[] OrgFrame(byte byt_Cmd, byte[] byt_Data)
        {
            int int_Len = 4 + byt_Data.Length;
            byte[] byt_Frame = new byte[int_Len];
            byt_Frame[0] = CST_BYT_STX;
            byt_Frame[1] = byt_Cmd;
            Array.Copy(byt_Data, 0, byt_Frame, 2, byt_Data.Length);
            byte[] byt_Tmp = GetCRC(byt_Frame, 0, byt_Data.Length + 2);
            Array.Copy(byt_Tmp, 0, byt_Frame, byt_Data.Length + 2, 2);
            return byt_Frame;
        }


        /// <summary>
        ///  ��֡������Ϣ��ʽ
        /// </summary>
        /// <param name="byt_Cmd">������</param>
        /// <param name="byt_FunCmd">��������</param>
        /// <param name="byt_FillData">�����</param>
        /// <param name="byt_Data">����</param>
        /// <returns></returns>
        public byte[] OrgFrame(byte byt_Cmd, byte byt_FunCmd, byte[] byt_Data)
        {

            //��ʼ�� ���� ������ ����� ���ݳ��� ���� CRC���ֽ� CRC���ֽ�
            //02 18 06 00 01 01 89BE

            int int_Len = 7 + byt_Data.Length;
            byte[] byt_Frame = new byte[int_Len];
            byt_Frame[0] = CST_BYT_STX;
            byt_Frame[1] = byt_Cmd;
            byt_Frame[2] = byt_FunCmd;
            byt_Frame[3] = 0x00;
            byt_Frame[4] = (byte)byt_Data.Length;
            Array.Copy(byt_Data, 0, byt_Frame, 5, byt_Data.Length);

            byte[] byt_Tmp = GetCRC(byt_Frame, 0, byt_Data.Length + 5);

            Array.Copy(byt_Tmp, 0, byt_Frame, byt_Data.Length + 5, 2);
            return byt_Frame;
        }

        /// <summary>
        ///  ��֡��һ������05���ࣩ
        /// </summary>
        /// <param name="byt_Cmd"></param>
        /// <param name="byt_FillData"></param>
        /// <param name="byt_ClassID"></param>
        /// <param name="byt_AddrOffset"></param>
        /// <param name="byt_Data"></param>
        /// <returns></returns>
        public byte[] OrgFrame(byte byt_Cmd, byte byt_ClassID)
        {
            //��ʼ�� 05 ����� ���ݳ��ȸ��ֽ� ���ݳ��ȵ��ֽ� ��ַƫ�Ƹ��ֽ� ��ַƫ�Ƶ��ֽ� �� CRC���ֽ� CRC���ֽ�

            byte[] byt_Frame = new byte[10];
            byt_Frame[0] = CST_BYT_STX;
            byt_Frame[1] = byt_Cmd;
            byt_Frame[2] = 0x00;
            byt_Frame[3] = 0x00;        //���ݳ��ȸߡ����ֽ�ֻ���������ݵĶ�ȡ�����Ϊ�㣬���ʾ����ȱʡ
            byt_Frame[4] = 0x00;
            byt_Frame[5] = 0x00;
            byt_Frame[6] = 0x00;
            byt_Frame[7] = byt_ClassID;
            byte[] byt_Tmp = GetCRC(byt_Frame, 0, 8);
            Array.Copy(byt_Tmp, 0, byt_Frame, 8, 2);
            return byt_Frame;

        }


        /// <summary>
        /// ���֡�ϸ���
        /// </summary>
        /// <param name="byt_Frame">�����֡</param>
        /// <param name="byt_RevFrame">���غϸ�֡</param>
        /// <returns></returns>
        public bool CheckFrame(byte[] byt_Frame, ref byte[] byt_RevFrame)
        {
            int int_STXStart = Array.IndexOf(byt_Frame, CST_BYT_STX);
            if (int_STXStart < 0)       //û��֡��ʼ��
                return false;

            int int_Len = byt_Frame.Length - int_STXStart;

            byte[] byt_TmpCRC = GetCRC(byt_Frame, int_STXStart, int_Len - 2);

            if (byt_TmpCRC[0] == byt_Frame[byt_Frame.Length - 2]
                && byt_TmpCRC[1] == byt_Frame[byt_Frame.Length - 1])
            {
                byt_RevFrame = new byte[int_Len];
                Array.Copy(byt_Frame, int_STXStart, byt_RevFrame, 0, int_Len);
                return true;
            }
            else
                return false;
        }



        /// <summary>
        /// ����CRCУ����
        /// </summary>
        /// <param name="byt_Frame">ֵ֡</param>
        /// <param name="int_Start">��ʼλ</param>
        /// <param name="int_Len">���ݳ���</param>
        /// <returns></returns>
        private byte[] GetCRC(byte[] byt_Frame, int int_Start, int int_Len)
        {
            UInt16[] uht_Table = new UInt16[256];
            IniCRC(ref uht_Table);
            UInt16 uht_CRC = 0;
            for (int int_Inc = 0; int_Inc < int_Len; int_Inc++)
                uht_CRC = (UInt16)((uht_CRC << 8) ^ (uht_Table[(uht_CRC >> 8) ^ byt_Frame[int_Start + int_Inc]]));
            byte[] byt_Tmp = BitConverter.GetBytes(uht_CRC);
            Array.Reverse(byt_Tmp);
            return byt_Tmp;
        }

        /// <summary>
        /// ��ʼCRC����ֵ
        /// </summary>
        /// <param name="uht_Table"></param>
        private void IniCRC(ref UInt16[] uht_Table)
        {
            UInt16 uht_Tmp;
            UInt16 uht_Crc;
            for (int int_Inc = 0; int_Inc < 256; int_Inc++)
            {
                uht_Tmp = (ushort)(int_Inc << 8);
                uht_Crc = 0;
                for (int int_Inb = 0; int_Inb < 8; int_Inb++)
                {
                    if (((uht_Crc ^ uht_Tmp) & 0x8000) != 0)
                        uht_Crc = (ushort)((uht_Crc << 1) ^ 0x1021);
                    else
                        uht_Crc <<= 1;
                    uht_Tmp <<= 1;
                }
                uht_Table[int_Inc] = uht_Crc;
            }
        }

    }
}
