using System;
//using ClInterface;
namespace CLDC_MeterProtocol.Protocols
{
    public class ClA1600 : CAlpha, IMeterProtocol
    {


        //private string MeterAddress ="";                          //地址

        //private string protocolInfo.WritePassword = "00000000";            //写操作密码+密码等级
        //private string protocolInfo.ClearDemandPassword = "00000000";            //清需量密码+密码等级
        //private string protocolInfo.ClearDLPassword = "00000000";            //清电量密码+密码等级

        //private string protocolInfo.UserID = "000000";                 //用户代码
        //private int protocolInfo.VerifyPasswordType = 0;                //验证密码类型
        //private bool protocolInfo.DataFieldPassword = true;                 //数据域是否包含密码
        //private bool protocolInfo.BlockAddAA = false;                   //块写操作时是否在数据域后加AA


        //private string m_str_TariffOrderType = "1234";          //费率排序
        //private string protocolInfo.DateTimeFormat = "YYMMDDHHFFSS";   //日期格式
        //private int protocolInfo.SundayIndex = 0;                      //星期日的序号

        //private string m_str_ConfigFile = "";                   //配置文件


        public ClA1600()
        {




        }


        #region --------------------接口参数---------------------



        /// <summary>
        /// 通信测试
        /// </summary>
        /// <param name="int_Type"></param>
        /// <returns></returns>
        /// 


        //public override bool ComTest()
        //{
        //    //    return base.ComTest();
        //    //}
        //    //public bool ComTest(int int_Type)
        //    //{
        //    //    try
        //    //    {
        //    byte[] byt_Key = new byte[4];
        //    bool bln_Result = this.RequestComm(this.MeterAddress, ref byt_Key);        //请求握手
        //    if (bln_Result)
        //        bln_Result = VerifyPassword(this.protocolInfo.WritePassword, byt_Key);             //密码验证
        //    this.CloseComm();
        //    return bln_Result;
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    //this.m_str_LostMessage = e.ToString();
        //    //    return false;
        //    //}
        //}

        public override bool BroadcastTime(DateTime broadCaseTime)
        {
            //    return base.BroadcastTime(broadCaseTime);
            //}

            //public bool BroadcastTime(int int_Type, string str_DateTime)
            //{
            //    //this.m_str_LostMessage = "ABB不支持广播校时指令！";
            //   
            return false;
        }


        /// <summary>
        /// 读取电量
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="ept_DirectType">功率方向</param>
        /// <param name="ett_TariffType">费率</param>
        /// <param name="sng_Energy">返回电量</param>
        /// <returns></returns>

        public override float ReadEnergy(byte energyType, byte tariffType)
        {
            //    return base.ReadEnergy(energyType, tariffType);
            //}
            float sng_Energy = -1F;
            //public bool ReadEnergy(int int_Type, enmPDirectType ept_DirectType, enmTariffType ett_TariffType, ref float sng_Energy)
            //{
            //    try
            //    {
            //if (tariffType != enmTariffType.总
            //&& (ept_DirectType == enmPDirectType.第一象限
            //|| ept_DirectType == enmPDirectType.第二象限
            //|| ept_DirectType == enmPDirectType.第三象限
            //|| ept_DirectType == enmPDirectType.第四象限))
            //{
            //    //this.m_str_LostMessage = "Q1、Q2、Q3、Q4不支持分费率读取电量";
            //    return false;
            //}

            byte[] byt_Key = new byte[4];
            bool bln_Result = this.RequestComm(this.MeterAddress, ref byt_Key);        //请求握手
            if (bln_Result)
                bln_Result = VerifyPassword(this.protocolInfo.WritePassword, byt_Key);             //密码验证
            if (bln_Result)
            {
                byte[] byt_RevData = new byte[0];
                bln_Result = this.ReadClass(0x00, ref byt_RevData);
                if (bln_Result)
                {
                    int int_Dot = byt_RevData[11];
                    bln_Result = this.ReadClass(0x0B, ref byt_RevData);
                    if (bln_Result)
                    {
                        if(energyType<4)
                        //if (ept_DirectType == enmPDirectType.反向无功
                        // || ept_DirectType == enmPDirectType.正向无功
                        // || ept_DirectType == enmPDirectType.反向有功
                        // || ept_DirectType == enmPDirectType.正向有功)
                        {
                            int int_Block = energyType;// (int)ept_DirectType;
                            if(tariffType==0)
                           // if (ett_TariffType == enmTariffType.总)         //总电量
                            {
                                for (int int_Inc = 0; int_Inc < 4; int_Inc++)
                                {
                                    string str_Value = BitConverter.ToString(byt_RevData, int_Block * 84 + 21 * int_Inc, 7);
                                    str_Value = str_Value.Replace("-", "");
                                    sng_Energy += (float)Math.Round(double.Parse(str_Value) / Math.Pow(10, int_Dot + 6), 5);
                                }
                            }
                            else
                            {
                                int int_TariffNo = tariffType;// (int)ett_TariffType;

                                int int_TariffIndex = protocolInfo.TariffOrderType.IndexOf(int_TariffNo.ToString()) + 1;

                                string str_Value = BitConverter.ToString(byt_RevData, int_Block * 84 + 21 * int_TariffIndex, 7);
                                str_Value = str_Value.Replace("-", "");
                                sng_Energy = (float)Math.Round(double.Parse(str_Value) / Math.Pow(10, int_Dot + 6), 5);
                            }
                        }
                        else
                        {
                            int int_Block = 7 - energyType;// (int)ept_DirectType;
                            string str_Value = BitConverter.ToString(byt_RevData, 4 * 84 + 7 * int_Block, 7);
                            str_Value = str_Value.Replace("-", "");
                            sng_Energy = (float)Math.Round(double.Parse(str_Value) / Math.Pow(10, int_Dot + 6), 5);
                        }
                    }
                }
            }

            this.CloseComm();
            return sng_Energy;
            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}

        }


        /// <summary>
        /// 读取电量
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="ept_DirectType">功率方向</param>
        /// <param name="sng_Energy">返回电量</param>
        /// <returns></returns>
        /// 

        public override float[] ReadEnergy(byte energyType)
        {
            //    return base.ReadEnergy(energyType);
            //}
            //public bool ReadEnergy(int int_Type, enmPDirectType ept_DirectType, ref float[] sng_Energy)
            //{
            //    try
            //    {
            float[] sng_Energy = new float[0];

            byte[] byt_Key = new byte[4];
            bool bln_Result = this.RequestComm(this.MeterAddress, ref byt_Key);        //请求握手
            if (bln_Result)
                bln_Result = VerifyPassword(this.protocolInfo.WritePassword, byt_Key);             //密码验证
            if (bln_Result)
            {
                byte[] byt_RevData = new byte[0];
                bln_Result = this.ReadClass(0x00, ref byt_RevData);
                if (bln_Result)
                {
                    int int_Dot = byt_RevData[11];
                    bln_Result = this.ReadClass(0x0B, ref byt_RevData);
                    if (bln_Result)
                    {
                        sng_Energy = new float[5];
                        if(energyType<4)
                        //if (ept_DirectType == enmPDirectType.反向无功
                        // || ept_DirectType == enmPDirectType.正向无功
                        // || ept_DirectType == enmPDirectType.反向有功
                        // || ept_DirectType == enmPDirectType.正向有功)
                        {
                            int int_Block = energyType;// (int)ept_DirectType;
                            for (int int_Inc = 0; int_Inc < 4; int_Inc++)
                            {
                                int int_TariffIndex = protocolInfo.TariffOrderType.IndexOf(Convert.ToString(int_Inc + 1)) + 1;
                                string str_Value = BitConverter.ToString(byt_RevData, int_Block * 84 + 21 * int_TariffIndex, 7);
                                str_Value = str_Value.Replace("-", "");
                                sng_Energy[int_Inc + 1] = (float)Math.Round(double.Parse(str_Value) / Math.Pow(10, int_Dot + 6), 5);
                                sng_Energy[0] += sng_Energy[int_Inc + 1];
                            }
                        }
                        else
                        {
                            int int_Block = 7 - energyType;// (int)ept_DirectType;
                            string str_Value = BitConverter.ToString(byt_RevData, 4 * 84 + 7 * int_Block, 7);
                            str_Value = str_Value.Replace("-", "");
                            sng_Energy[0] = (float)Math.Round(double.Parse(str_Value) / Math.Pow(10, int_Dot + 6), 5);
                        }
                    }
                }
            }

            this.CloseComm();
            return sng_Energy;
            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
        }



        /// <summary>
        /// 读取需量
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="ept_DirectType">功率方向</param>
        /// <param name="ett_TariffType">费率类型</param>
        /// <param name="sng_Demand"></param>
        /// <returns></returns>

        public override float ReadDemand(byte energyType, byte tariffType)
        {
            //    return base.ReadDemand(energyType, tariffType);
            //}
            float sng_Demand = -1F;
            //public bool ReadDemand(int int_Type, enmPDirectType ept_DirectType, enmTariffType ett_TariffType, ref float sng_Demand)
            //{
            //    try
            //    {
            //if (ept_DirectType == enmPDirectType.第一象限
            // || ept_DirectType == enmPDirectType.第二象限
            // || ept_DirectType == enmPDirectType.第三象限
            // || ept_DirectType == enmPDirectType.第四象限)
            if (energyType > 3)
            {
                //this.m_str_LostMessage = "Q1、Q2、Q3、Q4不支持读取需量";
                return sng_Demand;
            }

            byte[] byt_Key = new byte[4];
            bool bln_Result = this.RequestComm(this.MeterAddress, ref byt_Key);        //请求握手
            if (bln_Result)
                bln_Result = VerifyPassword(this.protocolInfo.WritePassword, byt_Key);             //密码验证
            if (bln_Result)
            {
                byte[] byt_RevData = new byte[0];
                bln_Result = this.ReadClass(0x00, ref byt_RevData);
                if (bln_Result)
                {
                    int int_Dot = byt_RevData[12];
                    bln_Result = this.ReadClass(0x0B, ref byt_RevData);
                    if (bln_Result)
                    {

                        int int_Block = energyType;// (int)ept_DirectType;
                        if(tariffType==0)
                        //if (ett_TariffType == enmTariffType.总)         //总电量
                        {
                            for (int int_Inc = 0; int_Inc < 4; int_Inc++)
                            {
                                string str_Value = BitConverter.ToString(byt_RevData, int_Block * 84 + 21 * int_Inc + 7, 7);
                                str_Value = str_Value.Replace("-", "");
                                sng_Demand += (float)Math.Round(double.Parse(str_Value) / Math.Pow(10, int_Dot), 5);
                            }
                        }
                        else
                        {
                            int int_TariffNo = tariffType;// (int)ett_TariffType;
                            string str_Value = BitConverter.ToString(byt_RevData, int_Block * 84 + 21 * int_TariffNo + 7, 7);
                            str_Value = str_Value.Replace("-", "");
                            sng_Demand = (float)Math.Round(double.Parse(str_Value) / Math.Pow(10, int_Dot), 5);
                        }
                    }
                }
            }

            this.CloseComm();
            return sng_Demand;
            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
        }


        /// <summary>
        /// 读取需量
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="ept_DirectType">功率方向</param>
        /// <param name="sng_Demand">返回需量</param>
        /// <returns></returns>
        /// 
        public override float[] ReadDemand(byte energyType)
        {
            //    return base.ReadDemand(energyType);
            //}
            //public bool ReadDemand(int int_Type, enmPDirectType ept_DirectType, ref float[] sng_Demand)
            //{
            //    try
            //    {
            float[] sng_Demand = new float[0];
            if (energyType > 3)
            {
                //if (ept_DirectType == enmPDirectType.第一象限
                // || ept_DirectType == enmPDirectType.第二象限
                // || ept_DirectType == enmPDirectType.第三象限
                // || ept_DirectType == enmPDirectType.第四象限)
                //{
                //this.m_str_LostMessage = "Q1、Q2、Q3、Q4不支持读取需量";
                return sng_Demand;
            }


            byte[] byt_Key = new byte[4];
            bool bln_Result = this.RequestComm(this.MeterAddress, ref byt_Key);        //请求握手
            if (bln_Result)
                bln_Result = VerifyPassword(this.protocolInfo.WritePassword, byt_Key);             //密码验证
            if (bln_Result)
            {
                byte[] byt_RevData = new byte[0];
                bln_Result = this.ReadClass(0x00, ref byt_RevData);
                if (bln_Result)
                {
                    int int_Dot = byt_RevData[12];
                    bln_Result = this.ReadClass(0x0B, ref byt_RevData);
                    if (bln_Result)
                    {
                        sng_Demand = new float[5];
                        int int_Block = energyType;// (int)ept_DirectType;
                        for (int int_Inc = 0; int_Inc < 4; int_Inc++)
                        {
                            string str_Value = BitConverter.ToString(byt_RevData, int_Block * 84 + 21 * int_Inc + 7, 3);
                            str_Value = str_Value.Replace("-", "");
                            sng_Demand[int_Inc + 1] = (float)Math.Round(double.Parse(str_Value) / Math.Pow(10, int_Dot), 5);
                            sng_Demand[0] += sng_Demand[int_Inc + 1];
                        }
                    }
                }
            }

            this.CloseComm();
            return sng_Demand;
            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
        }



        /// <summary>
        /// 读日期时间
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="str_DateTime">返回日期时间</param>
        /// <returns></returns>

        public override DateTime ReadDateTime()
        {
        //    return base.ReadDateTime();
        //}
            string str_DateTime = string.Empty;
        //public bool ReadDateTime(int int_Type, ref string str_DateTime)
        //{

            byte[] byt_Key = new byte[4];
            bool bln_Result = this.RequestComm(this.MeterAddress, ref byt_Key);        //请求握手
            if (bln_Result)
                bln_Result = VerifyPassword(this.protocolInfo.WritePassword, byt_Key);             //密码验证
            if (bln_Result)
            {
                byte[] byt_RevData = new byte[0];
                bln_Result = this.ReadClass(0x09, ref byt_RevData);                     //读取CLASS 9
                if (bln_Result)
                {
                    string str_Value = BitConverter.ToString(byt_RevData, 27, 6);      //取出第28个字节，共6个，为YY MM DD hh mm ss
                    str_Value = str_Value.Replace("-", "");
                    string[] str_Para = new string[] { "YY", "MM", "DD", "HH", "FF", "SS" };
                    for (int int_Inc = 0; int_Inc < 6; int_Inc++)
                    {
                        int int_Index = this.protocolInfo.DateTimeFormat.IndexOf(str_Para[int_Inc]);
                        str_DateTime += str_Value.Substring(int_Index, 2);
                        if (int_Inc < 2)
                            str_DateTime += "-";
                        else if (int_Inc == 2)
                            str_DateTime += " ";
                        else if (int_Inc != 5)
                            str_DateTime += ":";
                    }

                    //return true;
                }
            }

            return DateTime.Parse(str_DateTime);

            //return false;
        }


        public override string ReadAddress()
        {
            //    return base.ReadAddress();
            //}

            //public bool ReadAddress(int int_Type, ref string str_Address)
            //{
            //    //this.m_str_LostMessage = "ABB不支持读取地址指令！";
            return string.Empty;
        }


        //public override string[] ReadPeriodTime()
        //{
        //    //    return base.ReadPeriodTime();
        //    //}

        //    //public bool ReadPeriodTime(int int_Type, ref string[] str_PTime)
        //    //{
        //    //    //this.m_str_LostMessage = "ABB不支持读取时段！";
        //    return new string[0];
        //}

        public override string ReadData(string str_ID, int int_Len)
        {
            return base.ReadData(str_ID, int_Len);
        }

        public override float ReadData(string str_ID, int int_Len, int int_Dot)
        {
            return base.ReadData(str_ID, int_Len, int_Dot);
        }

        public override string[] ReadDataBlock(string str_ID, int int_Len)
        {
            return base.ReadDataBlock(str_ID, int_Len);
        }

        public override float[] ReadDataBlock(string str_ID, int int_Len, int int_Dot)
        {
            return base.ReadDataBlock(str_ID, int_Len, int_Dot);
        }
        //public bool ReadData(int int_Type, string str_ID, int int_Len, int int_Dot, ref float sng_Value)
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        //public bool ReadData(int int_Type, string str_ID, int int_Len, ref string str_Value)
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        //public bool ReadData(int int_Type, string str_ID, int int_Len, int int_Dot, ref float[] sng_Value)
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        //public bool ReadData(int int_Type, string str_ID, int int_Len, ref string[] str_Value)
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        //public override bool WriteAddress(string str_Address)
        //{
        //    //    return base.WriteAddress(str_Address);
        //    //}

        //    //public bool WriteAddress(int int_Type, string str_Address)
        //    //{
        //    //    //this.m_str_LostMessage = "ABB不支持写地址操作！";
        //    return false;
        //}


        /// <summary>
        /// 写日期时间
        /// </summary>
        /// <param name="int_Type">操作类型</param>
        /// <param name="str_DateTime">日期时间</param>
        /// <returns></returns>

        public override bool WriteDateTime(string str_DateTime)
        {
            //    return base.WriteDateTime(str_DateTime);
            //}
            //public bool WriteDateTime(int int_Type, string str_DateTime)
            //{
            //    try
            //    {
            byte[] byt_Key = new byte[4];
            bool bln_Result = this.RequestComm(this.MeterAddress, ref byt_Key);        //请求握手
            if (bln_Result)
                bln_Result = VerifyPassword(this.protocolInfo.WritePassword, byt_Key);             //密码验证
            if (bln_Result)
            {
                byte[] byt_Data = new byte[6];
                byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToInt64(str_DateTime, 16));
                Array.Copy(byt_Tmp, 2, byt_Data, 0, 6);
                byte[] byt_Frame = this.m_clf_Frame.OrgFrame(CAlphaFrame.CST_BYT_CB_DAT,
                                                                CAlphaFrame.CST_BYT_FUN_SET,
                                                                byt_Data);
                byte[] arrRecv = new byte[0];
                bln_Result = SendData(byt_Frame, ref arrRecv);// this.SendFrame(byt_Frame, 1200, 500);
                if (bln_Result)
                {
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
                            //this.m_str_LostMessage = "写日期时间操作失败";
                    }
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

        //public override bool WritePeriodTime(string[] str_PTime)
        //{
        //    //    return base.WritePeriodTime(str_PTime);
        //    //}

        //    //public bool WritePeriodTime(int int_Type, string[] str_PTime)
        //    //{
        //    //    //this.m_str_LostMessage = "ABB不支持设置时段操作！";
        //    return false;
        //}


        public override bool WriteData(string str_ID, byte[] byt_Value)
        {
            return base.WriteData(str_ID, byt_Value);
        }

        public override bool WriteDataByMac(string str_ID, byte[] byt_Value)
        {
            return base.WriteDataByMac(str_ID, byt_Value);
        }


        public override bool WriteData(string str_ID, int int_Len, string str_Value)
        {
            return base.WriteData(str_ID, int_Len, str_Value);
        }
        public override bool WriteData(string str_ID, int int_Len, string[] str_Value)
        {
            return base.WriteData(str_ID, int_Len, str_Value);
        }
        //public bool WriteData(int int_Type, string str_ID, byte[] byt_Value)
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        //public bool WriteData(int int_Type, string str_ID, int int_Len, string str_Value)
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        //public bool WriteData(int int_Type, string str_ID, int int_Len, int int_Dot, float sng_Value)
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        //public bool WriteData(int int_Type, string str_ID, int int_Len, string[] str_Value)
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        //public bool WriteData(int int_Type, string str_ID, int int_Len, int int_Dot, float[] sng_Value)
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        /// <summary>
        /// 清空需量
        /// </summary>
        /// <param name="int_Type"></param>
        /// <returns></returns>
        /// 
        public override bool ClearDemand()
        {
            //    return base.ClearDemand();
            //}
            //public bool ClearDemand(int int_Type)
            //{
            //    try
            //    {
            byte[] byt_Key = new byte[4];
            bool bln_Result = this.RequestComm(this.MeterAddress, ref byt_Key);        //请求握手
            if (bln_Result)
                bln_Result = VerifyPassword(this.protocolInfo.WritePassword, byt_Key);             //密码验证
            if (bln_Result)
            {
                byte[] byt_Frame = this.m_clf_Frame.OrgFrame(CAlphaFrame.CST_BYT_CB_NDAT, new byte[] { 0x01 });
                byte[] arrRecv = new byte[0];
                bln_Result = SendData(byt_Frame, ref arrRecv);// this.SendFrame(byt_Frame, 1200, 500);
                if (bln_Result)
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
                            //this.m_str_LostMessage = "清空需量操作失败";
                    }
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

        //public override bool ClearEnergy()
        //{
        //    //    return base.ClearEnergy();
        //    //}
        //    //public bool ClearEnergy(int int_Type)
        //    //{
        //    //    //this.m_str_LostMessage = "ABB不支持清空电量操作！";
        //    return false;
        //}
        ///// <summary>
        ///// 钱包初始化
        ///// </summary>
        /////  <param name="strEndata">密文 </param>
        ///// <returns></returns>
        //bool IMeterProtocol.InitPurse(string strEndata)
        //{
        //    throw new NotImplementedException();
        //}
        //public override bool ClearEventLog(string str_ID)
        //{
        //    //    return base.ClearEventLog(str_ID);
        //    //}
        //    //public bool ClearEventLog(int int_Type, string str_ID)
        //    //{
        //    //    //this.m_str_LostMessage = "ABB不支持清空事件记录操作！";
        //    return false;
        //}


        public override bool SetPulseCom(byte ecp_PulseType)
        {
            //    return base.SetPulseCom(ecp_PulseType);
            //}
            //public bool SetPulseCom(int int_Type, enmComPulseType ecp_PulseType)
            //{
            //    //this.m_str_LostMessage = "ABB不支持设置脉冲端口操作！";
            return true;
        }

        public override bool FreezeCmd(string str_DateHour)
        {
            //    return base.FreezeCmd(str_DateHour);
            //}
            //public bool FreezeCmd(int int_Type, string str_DateHour)
            //{
            //    //this.m_str_LostMessage = "ABB不支持冻结指令操作！";
            return false;
        }

        public override bool ChangePassword(int int_Class, string str_OldPws, string str_NewPsw)
        {
            //    return base.ChangePassword(int_Class, str_OldPws, str_NewPsw);
            //}
            //public bool ChangeSetting(int int_Type, string str_Setting)
            //{
            //    //this.m_str_LostMessage = "ABB不支持修改波特率操作！";
            return false;
        }

        public override bool ChangeSetting(string str_Setting)
        {
            //    return base.ChangeSetting(str_Setting);
            //}
            //public bool ChangePassword(int int_Type, int int_Class, string str_OldPws, string str_NewPsw)
            //{
            //    //this.m_str_LostMessage = "ABB不支持修改密码操作！";
            return false;
        }

        /// <summary>
        /// 密钥下装指令
        /// </summary>
        /// <param name="byt_Addr">地址</param>
        /// <param name="byt_Cmd">命令字</param>
        /// <param name="byt_Data">数据域</param>
        /// <param name="bln_Sequela">是否有后续帧</param>
        /// <param name="byt_RevDataF">返回帧数据域</param>
        /// <returns></returns>
        public override bool UpdateRemoteEncryptionCommand(byte byt_Cmd, byte[] byt_Data, ref bool bln_Sequela, ref byte[] byt_RevDataF)
        {
            bln_Sequela = false;
            byt_RevDataF = null;
            return false;
        }

        /// <summary>
        /// 密钥下装指令
        /// </summary>
        /// <param name="byt_Addr">地址</param>
        /// <param name="byt_Cmd">命令字</param>
        /// <param name="byt_Data">数据域</param>
        /// <param name="bln_Sequela">是否有后续帧</param>
        /// <param name="byt_RevDataF">返回帧数据域</param>
        /// <returns></returns>
        public override bool UpdateRemoteEncryptionCommandByTerminal(byte byt_Cmd, byte[] byt_Data, ref bool bln_Sequela, ref byte[] byt_RevDataF)
        {
            bln_Sequela = false;
            byt_RevDataF = null;
            return false;
        }

        #endregion


        #region IMeterProtocol 成员

        float[] IMeterProtocol.ReadEnergys(byte energyType, int int_FreezeTimes)
        {
            throw new NotImplementedException();
        }



        float[] IMeterProtocol.ReadDemands(byte energyType, int int_FreezeTimes)
        {
            throw new NotImplementedException();
        }


        float[] IMeterProtocol.ReadFreezeInterval(int int_Type, ref string str_FTime)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.WriteSwitchTime(int int_SwitchType, string str_Time)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.ReadSpecialEnergy(int int_type, int int_DLType, int int_Times, ref float[] flt_CurDL)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.ReadPatternWord(int int_type, int int_PatternType, ref string str_PatternWord)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.WriteFreezeInterval(int int_PatternType, string str_DateTime)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.ReadFreezeTime(int int_FreezeType, ref string str_FreezeTime)
        {
            throw new NotImplementedException();
        }

        #endregion
        #region IMeterProtocol 成员

        void IMeterProtocol.SetPortName(string szPortName)
        {
            throw new NotImplementedException();
        }

        void IMeterProtocol.SetMeterAddress(string szMeterAddress)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.SetProtocol(CLDC_DataCore.Model.DgnProtocol.DgnProtocolInfo protocol)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.ComTest()
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.BroadcastTime(DateTime broadCaseTime)
        {
            throw new NotImplementedException();
        }

        float IMeterProtocol.ReadEnergy(byte energyType, byte tariffType)
        {
            throw new NotImplementedException();
        }

        float[] IMeterProtocol.ReadEnergy(byte energyType)
        {
            throw new NotImplementedException();
        }

        float IMeterProtocol.ReadDemand(byte energyType, byte tariffType)
        {
            throw new NotImplementedException();
        }

        float[] IMeterProtocol.ReadDemand(byte energyType)
        {
            throw new NotImplementedException();
        }

        DateTime IMeterProtocol.ReadDateTime()
        {
            throw new NotImplementedException();
        }

        string IMeterProtocol.ReadAddress()
        {
            throw new NotImplementedException();
        }

        string[] IMeterProtocol.ReadPeriodTime()
        {
            throw new NotImplementedException();
        }

        float IMeterProtocol.ReadData(string str_ID, int int_Len, int int_Dot)
        {
            throw new NotImplementedException();
        }

        string IMeterProtocol.ReadData(string str_ID, int int_Len)
        {
            throw new NotImplementedException();
        }

        string IMeterProtocol.ReadData(string sendData)
        {
            throw new NotImplementedException();
        }

        float[] IMeterProtocol.ReadDataBlock(string str_ID, int int_Len, int int_Dot)
        {
            throw new NotImplementedException();
        }

        string[] IMeterProtocol.ReadDataBlock(string str_ID, int int_Len)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.WriteAddress(string str_Address)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.WriteDateTime(string str_DateTime)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.WritePeriodTime(string[] str_PTime)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.WriteRatesPrice(string str_ID, byte[] byt_Value)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.WriteData(string str_ID, byte[] byt_Value)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.WriteData(string str_ID, int int_Len, string str_Value)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.WriteData(string str_ID, int int_Len, int int_Dot, float sng_Value)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.WriteData(string str_ID, int int_Len, string[] str_Value)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.WriteData(string str_ID, int int_Len, int int_Dot, float[] sng_Value)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.ClearDemand()
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.ClearDemand(string str_Endata)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.ClearEnergy()
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.ClearEnergy(string str_Endata)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.ClearEventLog(string str_ID)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.SetPulseCom(byte ecp_PulseType)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.FreezeCmd(string str_DateHour)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.ChangeSetting(string str_Setting)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.ChangePassword(int int_Class, string str_OldPws, string str_NewPsw)
        {
            throw new NotImplementedException();
        }

        bool IMeterProtocol.UpdateRemoteEncryptionCommand(byte byt_Cmd, byte[] byt_Data, ref bool bln_Sequela, ref byte[] byt_RevDataF)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
