using System;
//using ClInterface;
namespace CLDC_MeterProtocol.Protocols
{

    /// <summary>
    /// ElSTER(ABB)的ALPHA协议基类
    /// </summary>
    public class CAlpha:ProtocolBase
    {

        

        protected CAlphaFrame m_clf_Frame;
        //private string m_str_Setting = "1200,e,8,1";            //波特率
        //private ISerialport m_Ispt_com;                         //控制端口
        //protected byte[] m_byt_RevData;                           //返回数据
        //public string m_str_LostMessage = "";                   //操作失败信息
        //public bool m_bln_Enabled = true;                       //

        //protected string m_str_RxFrame = "";
        //protected string m_str_TxFrame = "";
        //int protocolInfo.FECount = 0;

        //private object m_obj_LockRev = new object();        //用于锁定互斥 


        public CAlpha()
        {
            //arrRecv = new byte[0];
            this.m_clf_Frame = new CAlphaFrame();
            


        }


        //public event Dge_EventRxFrame OnEventRxFrame;
        //public event Dge_EventTxFrame OnEventTxFrame;


        /// <summary>
        /// 误差计算板串口
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
        ///// 波特率
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
        ///// 返回帧
        ///// </summary>
        //public string RxFrame
        //{
        //    get { return this.m_str_RxFrame; }
        //}

        ///// <summary>
        ///// 下发帧
        ///// </summary>
        //public string TxFrame
        //{
        //    get { return this.m_str_TxFrame; }
        //}


        

        ///// <summary>
        ///// 下发帧的唤醒符个数
        ///// </summary>
        //public int FECount
        //{
        //    get { return this.protocolInfo.FECount; }
        //    set { this.protocolInfo.FECount = value; }
        //}



        /// <summary>
        /// 请求通信
        /// </summary>
        /// <param name="str_Addr">地址</param>
        /// <param name="byt_Key">返回密匙</param>
        /// <returns></returns>
        protected bool RequestComm(string str_Addr,ref byte []byt_Key)
        {
            try
            {
                String str_Tmp = str_Addr;
                str_Tmp = str_Tmp.PadLeft(2, '0');
                byte[] byt_Data = new byte[1];

                byt_Data[0] = Convert.ToByte(str_Tmp.Substring(str_Tmp.Length - 2, 2));

                if (byt_Data[0] == 0)           //当地址是0时，则为100
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
                        if (arrRecv.Length > 0)                                  //是否有数据返回
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
                                    //this.m_str_LostMessage = "握手失败";
                            }
                           // else
                                //this.m_str_LostMessage = "握手返回帧不符合要求";
                        }
                        //else
                            //this.m_str_LostMessage = "握手没有返回数据";
                    }
                    //else
                        //this.m_str_LostMessage = "发送握手帧失败";
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
        /// 密码验证
        /// </summary>
        /// <param name="str_Password">密码</param>
        /// <param name="byt_Key">密匙</param>
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
                if (arrRecv.Length > 0)                                  //是否有数据返回
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
                            //this.m_str_LostMessage = "密码验证失败";
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
        /// 退出通信请求
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
        /// 读取类数据
        /// </summary>
        /// <param name="byt_Class">类编号</param>
        /// <param name="byt_RevData">返回数据</param>
        /// <returns></returns>
        protected bool ReadClass(byte byt_Class, ref byte[] byt_RevData)
        {

            byte[] byt_Frame = this.m_clf_Frame.OrgFrame(CAlphaFrame.CST_BYT_CB_CAS, byt_Class);
            byte[] arrRecv = new byte[0];

            bool bln_Result = SendData(byt_Frame, ref arrRecv);// this.SendFrame(byt_Frame, 1200, 400);
            if (arrRecv.Length > 0)                                  //是否有数据返回
            {
                byte[] byt_Data = new byte[0];
                if (this.m_clf_Frame.CheckFrame(arrRecv, ref byt_Data))
                {
                    if (byt_Data.Length >= 7)
                    {
                        if (byt_Data[1] == CAlphaFrame.CST_BYT_CB_CAS && byt_Data[2] == CAlphaFrame.CST_BYT_ACK)
                        {
                            bool bln_Continue =  ((byt_Data[4] & 0x80) != 0x80);         //是否有后续帧
                            int int_Len = byt_Data[4] & 0x7f;                           //帧的数据域长度
                            byt_RevData = new byte[int_Len];                            //数据域
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
            if (arrRecv.Length > 0)                                  //是否有数据返回
            {
                byte[] byt_Data = new byte[0];
                if (this.m_clf_Frame.CheckFrame(arrRecv, ref byt_Data))
                {
                    if (byt_Data[1] == CAlphaFrame.CST_BYT_CB_CNE && byt_Data[2] == CAlphaFrame.CST_BYT_ACK)
                    {
                        bln_Continue = ((byt_Data[4] & 0x80) != 0x80);         //是否有后续帧
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
        //        //this.m_Ispt_com.SendData(byt_Frame);                                //发送数据
        //        //Waiting(int_MinSecond, int_SpaceMSecond);                                                  //等待返回数据
                
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
            //1， NAK，CRC校验错
            //2， NAK，该功能为通信锁定
            //3， NAK，不合法的命令，同步或长度
            //4， NAK，帧错误
            //5， NAK，超时
            //6， NAK，无效口令
            //7， NAK，计算机无应答
            //E， NAK，IEC C模式通信闭锁

            switch (byt_Code)
            {
                case CAlphaFrame.CST_BYT_ACK:
                    return "";
                case CAlphaFrame.CST_BYT_NAK_CRC:
                    return "CRC校验错";
                case CAlphaFrame.CST_BYT_NAK_CMM:
                    return "该功能为通信锁定";
                case CAlphaFrame.CST_BYT_NAK_CMD:
                    return "不合法的命令，同步或长度";
                case CAlphaFrame.CST_BYT_NAK_FRM:
                    return "帧错误";
                case CAlphaFrame.CST_BYT_NAK_OTM:
                    return "超时";
                case CAlphaFrame.CST_BYT_NAK_PSW:
                    return "无效口令";
                case CAlphaFrame.CST_BYT_NAK_NAS:
                    return "计算机无应答";
                case CAlphaFrame.CST_BYT_NAK_CCL:
                    return "IEC C模式通信闭锁";
                default:
                    return "未知错误";
            }
        }




        #region-------------私有函数------------------------------

        /// <summary>
        /// 等待数据返回
        /// </summary>
        /// <param name="int_MinSecond">等待返回时间</param>
        /// <param name="int_SpaceMSecond">等待返回帧字节间隔时间</param>
        //private void Waiting(int int_MinSecond, int int_SpaceMSecond)
        //{
        //    try
        //    {
        //        int int_OldLen = 0;
        //        Stopwatch sth_Ticker = new Stopwatch();                     //等待计时，
        //        Stopwatch sth_SpaceTicker = new Stopwatch();                //
        //        sth_SpaceTicker.Start();
        //        sth_Ticker.Start();
        //        while (this.m_bln_Enabled )
        //        {
        //            System.Windows.Forms.Application.DoEvents();
        //            if (arrRecv.Length > int_OldLen)     //长度有改变
        //            {
        //                sth_SpaceTicker.Reset();
        //                int_OldLen = arrRecv.Length;
        //                sth_SpaceTicker.Start();                    //字节间计时重新开始
        //            }
        //            else        //如果长度有没有增加，与前次收到数据时间隔500毫秒则退出
        //            {
        //                if (arrRecv.Length > 0)      //已经收到一部分，则按字节间计时
        //                {
        //                    if (sth_SpaceTicker.ElapsedMilliseconds >= int_SpaceMSecond)
        //                        break;
        //                }
        //            }
        //            if (sth_Ticker.ElapsedMilliseconds >= int_MinSecond)        //总计时
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
        ///// 等待数据返回
        ///// </summary>
        ///// <param name="int_SpaceMSecond">等待返回帧字节间隔时间</param>
        //private void Waiting( int int_SpaceMSecond)
        //{
        //    try
        //    {
        //        int int_OldLen = 0;
        //        Stopwatch sth_Ticker = new Stopwatch();                     //等待计时，
        //        Stopwatch sth_SpaceTicker = new Stopwatch();                //
        //        sth_SpaceTicker.Start();
        //        sth_Ticker.Start();
        //        while (this.m_bln_Enabled )
        //        {
        //            System.Windows.Forms.Application.DoEvents();
        //            if (arrRecv.Length > int_OldLen)     //长度有改变
        //            {
        //                sth_SpaceTicker.Reset();
        //                int_OldLen = arrRecv.Length;
        //                sth_SpaceTicker.Start();                    //字节间计时重新开始
        //            }
        //            else        //如果长度有没有增加，与前次收到数据时间隔500毫秒则退出
        //            {
        //                if (arrRecv.Length > 0)      //已经收到一部分，则按字节间计时
        //                {
        //                    if (sth_SpaceTicker.ElapsedMilliseconds >= int_SpaceMSecond)
        //                        break;
        //                }
        //            }
        //            if (sth_Ticker.ElapsedMilliseconds >= 2000)        //总计时
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
        ///// 串口返回数据
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
        /// 密码加密运算
        /// </summary>
        /// <param name="byt_Password">密码</param>
        /// <param name="byt_Key">密匙</param>
        /// <returns></returns>
        public byte[] EncryptPassword(byte[] byt_Password, byte[] byt_Key)
        {
            //参照Alpha协议中的加密口令字计算源程序   根据密匙key与远程通讯口令来计算加密口令字
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

