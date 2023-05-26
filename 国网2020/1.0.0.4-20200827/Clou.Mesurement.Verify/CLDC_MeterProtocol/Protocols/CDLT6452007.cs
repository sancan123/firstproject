using CLDC_DataCore.Const;
using System;
//using ClInterface;
namespace CLDC_MeterProtocol.Protocols
{
    public class CDLT6452007 : CDLT645
    {

        #region -----------------接口成员-----------------------

        #region 密钥下装指令
        /// <summary>
        /// 密钥下装通用指令
        /// </summary>
        /// <param name="byt_Addr">地址</param>
        /// <param name="byt_Cmd">命令字</param>
        /// <param name="byt_Data">数据域</param>
        /// <param name="bln_Sequela">是否有后续帧</param>
        /// <param name="byt_RevDataF">返回帧数据域</param>
        /// <returns></returns>
        public override bool UpdateRemoteEncryptionCommand(byte byt_Cmd, byte[] byt_Data, ref bool bln_Sequela, ref byte[] byt_RevDataF)
        {
            //fjk 兼容0x1C 、 0x03命令this.protocolInfo.WriteClass
            byte[] byt_Data_T;
            if (byt_Cmd == 0x1C)
            {
                byt_Data_T = new byte[byt_Data.Length + 4];
                Array.Copy(BitConverter.GetBytes(Convert.ToInt32("0x" + this.protocolInfo.WritePassword2 + protocolInfo.WriteClass2, 16)), byt_Data_T, 4);
                Array.Copy(byt_Data, 0, byt_Data_T, 4, byt_Data.Length);
            }
            else
            {
                byt_Data_T = new byte[byt_Data.Length];
                Array.Copy(byt_Data, byt_Data_T, byt_Data.Length);
            }
            return this.ExeCommand(this.GetAddressByte(), byt_Cmd, byt_Data_T, ref bln_Sequela, ref  byt_RevDataF, 2, 6200, 2100);
        }

        /// <summary>
        /// 密钥下装通用指令 交互终端专用 注意：只能发一遍
        /// </summary>
        /// <param name="byt_Addr">地址</param>
        /// <param name="byt_Cmd">命令字</param>
        /// <param name="byt_Data">数据域</param>
        /// <param name="bln_Sequela">是否有后续帧</param>
        /// <param name="byt_RevDataF">返回帧数据域</param>
        /// <returns></returns>
        public override bool UpdateRemoteEncryptionCommandByTerminal(byte byt_Cmd, byte[] byt_Data, ref bool bln_Sequela, ref byte[] byt_RevDataF)
        {
            //fjk 兼容0x1C 、 0x03命令this.protocolInfo.WriteClass
            byte[] byt_Data_T;
            if (byt_Cmd == 0x1C)
            {
                byt_Data_T = new byte[byt_Data.Length + 4];
                Array.Copy(BitConverter.GetBytes(Convert.ToInt32("0x" + this.protocolInfo.WritePassword2 + protocolInfo.WriteClass2, 16)), byt_Data_T, 4);
                Array.Copy(byt_Data, 0, byt_Data_T, 4, byt_Data.Length);
            }
            else
            {
                byt_Data_T = new byte[byt_Data.Length];
                Array.Copy(byt_Data, byt_Data_T, byt_Data.Length);
            }
            return this.ExeCommand(this.GetAddressByte(), byt_Cmd, byt_Data_T, ref bln_Sequela, ref  byt_RevDataF, 1, 6200, 2100);
        }

        //读取冻结电量冻结数据模式字   int_PatternType  ：2,3,4,5,6
        public override bool ReadPatternWord(int int_type, int int_PatternType, ref string str_PatternWord)
        {
            string str_ID = "0400090" + int_PatternType.ToString();      //取出整点冻结数据模式字
            str_PatternWord = this.ReadData(str_ID, 1).ToString();
            return true;
        }

        //写入冻结电量冻结数据模式字   int_PatternType  ：2,3,4,5,6   data：
        public override bool WritePatternWord(int int_type, string data) // 5整点
        {
            string str_ID = "0400090" + int_type.ToString();      //取出整点冻结数据模式字
            bool str_PatternWord = this.WriteData(str_ID, 1, data);
            return str_PatternWord;
        }

        //读取冻结电量冻结起始时间、时间间隔 int_FreezeType 1 时间 2 时间间隔
        /// <summary>
        /// 时段表切换时间int_FreezeType 切换时间类型 1=两套时区表切换时间，2=两套日，3=两套费率电价切换时间，4=两套梯度切换时间,5=整点冻结起始时间 6=时间间隔
        /// </summary>
        /// <param name="int_FreezeType"></param>
        /// <param name="str_FreezeTime"></param>
        public override bool ReadFreezeTime(int int_FreezeType, ref string str_FreezeTime)
        {
            string str_ID = "";
            int length = 5;
            if (int_FreezeType == 1)
                str_ID = "04000106";
            else if (int_FreezeType == 2)
                str_ID = "04000107";
            else if (int_FreezeType == 3)
                str_ID = "04000108";
            else if (int_FreezeType == 4)
                str_ID = "04000109";
            else if (int_FreezeType == 5)
                str_ID = "04001201";
            else if (int_FreezeType == 6)
            {
                str_ID = "04001202";
                length = 1;
            }
            str_FreezeTime = this.ReadData(str_ID, length).ToString();

            //string str_ID = "0400120" + int_FreezeType.ToString();      //取出整点冻结数据模式字  1 时间 2 时间间隔
            //if (int_FreezeType==1)
            //    str_FreezeTime = this.ReadData(str_ID, 5, 0).ToString();
            //else if(int_FreezeType==2)
            //    str_FreezeTime = this.ReadData(str_ID, 1, 0).ToString();
            return true;
        }



        /// <summary>
        /// 切换时间
        /// </summary>
        /// <param name="int_FreezeType"></param>
        /// <param name="str_DateTime"> 时段表切换时间int_FreezeType 切换时间类型 1=两套时区表切换时间，2=两套日，3=两套费率电价切换时间，4=两套梯度切换时间,5=整点冻结起始时间 6=时间间隔</param>
        /// <returns></returns>
        public override bool WriteFreezeInterval(int int_FreezeType, string str_DateTime)
        {
            bool bln_Result = false;
            string str_ID = "";
            int length = 5;
            if (int_FreezeType == 1)
                str_ID = "04000106";
            else if (int_FreezeType == 2)
                str_ID = "04000107";
            else if (int_FreezeType == 3)
                str_ID = "04000108";
            else if (int_FreezeType == 4)
                str_ID = "04000109";
            else if (int_FreezeType == 5)
                str_ID = "04001201";
            else if (int_FreezeType == 6)
            {
                str_ID = "04001202";
                length = 1;
            }
            bln_Result = this.WriteData(str_ID, length, str_DateTime);
            return bln_Result;
        }
        #endregion

        /// <summary>
        /// 读取需量(分费率读取)
        /// </summary>
        /// <param name="int_Type">读取类型0=块读，1=分项读</param>
        /// <param name="ept_PDirect">功率类型</param>
        /// <param name="ett_TariffType">费率类型</param>
        /// <param name="sng_Demand">返回需量</param>
        /// <returns></returns>

        public override float ReadDemand(byte energyType, byte tariffType)
        {

            float sng_Demand = -1F;

            int int_PDirect = energyType;// (int)ept_PDirect;
            int int_TariffType = tariffType;// (int)ett_TariffType;
            if (int_PDirect < 0 || int_PDirect > 7)
            {
                //this.m_str_LostMessage = "读取需量指定功率类型超出范围[0-7]";
                return sng_Demand;
            }
            if (int_TariffType < 0 || int_TariffType > 4)
            {
                //this.m_str_LostMessage = "读取需量指定费率类型超出范围[0-4]";
                return sng_Demand;
            }

            bool bln_Block = false;
            if (ReadDemandType == 0) bln_Block = true;

            int int_Index = protocolInfo.TariffOrderType.IndexOf(int_TariffType.ToString()) + 1;      //按设置的费率排序取出需量

            string str_ID = this.GetDemandID(bln_Block, int_PDirect, int_Index);      //取出需量的标识符
            if (bln_Block)
            {
                string[] str_DemandArry = this.ReadDataBlock(str_ID, 8);

                if (str_DemandArry.Length > 0)
                {
                    if (int_TariffType == 0)        //总需量
                    {
                        if (str_DemandArry.Length > 0)      //解需量块有数据
                        {
                            sng_Demand = Convert.ToSingle(str_DemandArry[0].Substring(10, 6)) / 10000F;
                            //return true;
                        }
                        //else
                        //{
                        //    //this.m_str_LostMessage = "没有读到指定费率需量";
                        //    return false;
                        //}
                    }
                    else                                //尖峰平谷需量
                    {
                        if (int_Index < str_DemandArry.Length)
                        {
                            sng_Demand = Convert.ToSingle(str_DemandArry[int_Index].Substring(0, 8)) / 10000.0f;
                            //return true;
                        }
                        //else
                        //{
                        //    //this.m_str_LostMessage = "没有读到指定费率需量";
                        //    return false;
                        //}
                    }
                }
                //else
                //    return false;
            }
            else
            {
                string str_Demand = this.ReadData(str_ID, 8);
                if (str_Demand != string.Empty)
                {
                    sng_Demand = Convert.ToSingle(str_Demand.Substring(10, 6)) / 10000.0f;
                }
                //return bln_Result;
            }

            return sng_Demand;
        }


        /// <summary>
        /// 读取需量(所有费率读取)
        /// </summary>
        /// <param name="int_Type">读取类型0=块读，1=分项读</param>
        /// <param name="ept_PDirect">功率类型</param>
        /// <param name="sng_Demand">返回需量</param>
        /// <returns></returns>

        public override float[] ReadDemand(byte energyType)
        {
            //    return base.ReadDemand(energyType);
            //}

            //public bool ReadDemand(int int_Type, enmPDirectType ept_PDirect, ref float[] sng_Demand)
            //{
            float[] sng_Demand = new float[0];
            //try
            //{
            int int_PDirect = energyType;// (int)ept_PDirect;
            if (int_PDirect < 0 || int_PDirect > 7)
            {
                //this.m_str_LostMessage = "读取需量指定功率类型超出范围[0-7]";
                return sng_Demand;
            }
            bool bln_Block = false;
            if (ReadDemandType == 0) bln_Block = true;

            if (bln_Block)
            {
                string str_ID = this.GetDemandID(bln_Block, int_PDirect, 0);      //取出需量的标识符
                string[] str_DemandArry = this.ReadDataBlock(str_ID, 8);
                if (str_DemandArry.Length > 0)
                {
                    if (str_DemandArry.Length > 0)
                    {
                        Array.Resize(ref sng_Demand, 5);
                        sng_Demand[0] = Convert.ToSingle(str_DemandArry[0].Substring(0, 8)) / 10000.0f;
                        for (int int_Inc = 1; int_Inc < str_DemandArry.Length; int_Inc++)
                        {
                            int int_Index = Convert.ToInt16(protocolInfo.TariffOrderType.Substring(int_Inc - 1, 1));      //按设置的费率排序取出需量
                            sng_Demand[int_Index] = Convert.ToSingle(str_DemandArry[int_Inc].Substring(0, 8)) / 10000.0f;
                        }
                        //return true;
                    }
                    //else
                    //{
                    //    //this.m_str_LostMessage = "没有读到指定需量块";
                    //    return false;
                    //}
                }
                //else
                //    return false;
            }
            else
            {
                Array.Resize(ref sng_Demand, 5);
                string str_Demand = "";
                string str_ID = GetDemandID(bln_Block, int_PDirect, 0);      //取出需量总的标识符
                str_Demand = this.ReadData(str_ID, 8);
                if (str_Demand != string.Empty)
                {
                    sng_Demand[0] = Convert.ToSingle(str_Demand.Substring(0, 8)) / 10000.0f;
                    for (int int_Inc = 1; int_Inc < 5; int_Inc++)
                    {
                        int int_Index = protocolInfo.TariffOrderType.IndexOf(int_Inc.ToString()) + 1;      //按设置的费率排序取出需量
                        str_ID = GetDemandID(bln_Block, int_PDirect, int_Index);      //取出需量费率的标识符
                        str_Demand = this.ReadData(str_ID, 8);
                        if (str_Demand != string.Empty)
                            sng_Demand[int_Inc] = Convert.ToSingle(str_Demand.Substring(0, 8)) / 10000.0f;
                        else
                            break;
                    }
                }
                //return bln_Result;
            }

            return sng_Demand;
            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
        }

        /// <summary>
        ///  读取需量(所有费率读取)
        /// </summary>
        /// <param name="energyType"></param>
        /// <param name="int_FreezeTimes"></param>
        /// <returns></returns>

        public override float[] ReadDemands(byte energyType, int int_FreezeTimes)
        {
            //    return base.ReadDemand(energyType);
            //}

            //public bool ReadDemand(int int_Type, enmPDirectType ept_PDirect, ref float[] sng_Demand)
            //{
            float[] sng_Demand = new float[0];
            //try
            //{
            int int_PDirect = energyType;// (int)ept_PDirect;
            if (int_PDirect < 0 || int_PDirect > 7)
            {
                //this.m_str_LostMessage = "读取需量指定功率类型超出范围[0-7]";
                return sng_Demand;
            }
            bool bln_Block = false;
            if (ReadDemandType == 0) bln_Block = true;

            if (bln_Block)
            {
                string str_ID = this.GetDemandID(bln_Block, int_PDirect, 0, int_FreezeTimes);      //取出需量的标识符
                string[] str_DemandArry = this.ReadDataBlock(str_ID, 8);
                if (str_DemandArry.Length > 0)
                {
                    if (str_DemandArry.Length > 0)
                    {
                        Array.Resize(ref sng_Demand, 5);
                        sng_Demand[0] = Convert.ToSingle(str_DemandArry[0].Substring(10, 6)) / 10000.0f;
                        for (int int_Inc = 1; int_Inc < str_DemandArry.Length; int_Inc++)
                        {
                            int int_Index = Convert.ToInt16(protocolInfo.TariffOrderType.Substring(int_Inc - 1, 1));      //按设置的费率排序取出需量
                            sng_Demand[int_Index] = Convert.ToSingle(str_DemandArry[int_Inc].Substring(10, 6)) / 10000.0f;
                        }
                        //return true;
                    }
                    //else
                    //{
                    //    //this.m_str_LostMessage = "没有读到指定需量块";
                    //    return false;
                    //}
                }
                //else
                //    return false;
            }
            else
            {
                Array.Resize(ref sng_Demand, 5);
                string str_Demand = "";
                string str_ID = GetDemandID(bln_Block, int_PDirect, 0, int_FreezeTimes);      //取出需量总的标识符
                str_Demand = this.ReadData(str_ID, 8);
                if (str_Demand != string.Empty)
                {
                    sng_Demand[0] = Convert.ToSingle(str_Demand.Substring(10, 6)) / 10000.0f;
                    for (int int_Inc = 1; int_Inc < 5; int_Inc++)
                    {
                        int int_Index = protocolInfo.TariffOrderType.IndexOf(int_Inc.ToString()) + 1;      //按设置的费率排序取出需量
                        str_ID = GetDemandID(bln_Block, int_PDirect, int_Index, int_FreezeTimes);      //取出需量费率的标识符
                        str_Demand = this.ReadData(str_ID, 8);
                        if (str_Demand != string.Empty)
                            sng_Demand[int_Inc] = Convert.ToSingle(str_Demand.Substring(10, 6)) / 10000.0f;
                        else
                            break;
                    }
                }
                //return bln_Result;
            }

            return sng_Demand;
            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
        }

    
        /// <summary>
        /// 读取电量(分费率读取)
        /// </summary>
        /// <param name="int_Type">读取类型 0=块读，1=分项读</param>
        /// <param name="ept_PDirect">功率类型</param>
        /// <param name="ett_TariffType">费率类型</param>
        /// <param name="sng_Energy">返回电量</param>
        /// <returns></returns>
        public override float ReadEnergy(byte energyType, byte tariffType)
        {

            //    return base.ReadEnergy(energyType);
            //}
            float sng_Energy = -1F;
            //public bool ReadEnergy(int int_Type, enmPDirectType ept_PDirect, enmTariffType ett_TariffType, ref float sng_Energy)
            //{
            //try
            //{
                int int_PDirect = energyType;// (int)ept_PDirect;
                int int_TariffType = tariffType;// (int)ett_TariffType;
                if (int_PDirect < 0 || int_PDirect > 7)
                {
                    //this.m_str_LostMessage = "读取电量指定功率类型超出范围[0-7]";
                    return sng_Energy;
                }
                if (int_TariffType < 0 || int_TariffType > 4)
                {
                    //this.m_str_LostMessage = "读取电量指定费率类型超出范围[0-4]";
                    return sng_Energy;
                }

                bool bln_Block = false;
                if (ReadEnergyType == 0) bln_Block = true;

                int int_Index = protocolInfo.TariffOrderType.IndexOf(int_TariffType.ToString()) + 1;      //按设置的费率排序取出电量

                string str_ID = GetEnergyID(bln_Block, int_PDirect, int_Index);      //取出电量的标识符
                if (bln_Block)
                {
                    //Single[] sng_EnergyArry =this.ReadDataBlock(str_ID, 4, 2);

                    //if (sng_EnergyArry.Length>0)
                    //{
                    //    if (int_TariffType == 0)        //总电量
                    //    {
                    //        if (sng_EnergyArry.Length > 0)      //解电量块有数据
                    //        {
                    //            sng_Energy = sng_EnergyArry[0];
                    //            //return true;
                    //        }
                    //        //else
                    //        //{
                    //        //    //this.m_str_LostMessage = "没有读到指定费率电量";
                    //        //    return false;
                    //        //}
                    //    }
                    //    else                                //尖峰平谷电量
                    //    {
                    //        if (int_Index < sng_EnergyArry.Length)
                    //        {
                    //            sng_Energy = sng_EnergyArry[int_Index];
                    //            //return true;
                    //        }
                    //        //else
                    //        //{
                    //        //    //this.m_str_LostMessage = "没有读到指定费率电量";
                    //        //    return false;
                    //        //}
                    //    }
                    //}
                    //else
                    //    return false;
                }
                else
                    return this.ReadData( str_ID, 4, 2);
            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
                return sng_Energy;
        }



        /// <summary>
        /// 读取电量(所有费率读取)
        /// </summary>
        /// <param name="int_Type">读取类型0=块读，1=分项读</param>
        /// <param name="ept_PDirect">功率类型</param>
        /// <param name="sng_Energy">返回电量</param>
        /// <returns></returns>

        public override float[] ReadEnergys(byte energyType, int int_FreezeTimes)
        {
            //    return base.ReadEnergy(energyType, tariffType);
            //}
            float[] sng_Energy = new float[0];
            //public bool ReadEnergy(int int_Type, enmPDirectType ept_PDirect, ref )
            //{
            //try
            //{
            int int_PDirect = energyType;// (int)ept_PDirect;
            if (int_PDirect < 0 || int_PDirect > 8)
            {
                //this.m_str_LostMessage = "读取电量指定功率类型超出范围[0-7]";
                return sng_Energy;
            }
            bool bln_Block = false;
            if (ReadEnergyType == 0) bln_Block = true;

            if (bln_Block)
            {
                string str_ID = GetEnergyIDs(bln_Block, int_PDirect, 0, int_FreezeTimes);      //取出电量的标识符
                Single[] sng_EnergyArry = this.ReadDataBlock(str_ID, 4, 2);
                if (sng_EnergyArry.Length > 0)
                {
                    if (sng_EnergyArry.Length > 0)
                    {
                        Array.Resize(ref sng_Energy, 5);
                        sng_Energy[0] = sng_EnergyArry[0];
                        for (int int_Inc = 1; int_Inc < sng_EnergyArry.Length; int_Inc++)
                        {
                            int int_Index = Convert.ToInt16(protocolInfo.TariffOrderType.Substring(int_Inc - 1, 1));      //按设置的费率排序取出电量
                            sng_Energy[int_Index] = sng_EnergyArry[int_Inc];
                        }
                        //return true;
                    }
                    //    else
                    //    {
                    //        //this.m_str_LostMessage = "没有读到指定电量块";
                    //        return false;
                    //    }
                }
                //else
                //    return false;
            }
            else
            {
                Array.Resize(ref sng_Energy, 5);

                string str_ID = GetEnergyIDs(bln_Block, int_PDirect, 0, int_FreezeTimes);      //取出电量总的标识符
                sng_Energy[0] = this.ReadData(str_ID, 4, 2);
                if (sng_Energy[0] != -1)
                {
                    for (int int_Inc = 1; int_Inc < 5; int_Inc++)
                    {
                        int int_Index = protocolInfo.TariffOrderType.IndexOf(int_Inc.ToString()) + 1;      //按设置的费率排序取出电量

                        str_ID = GetEnergyIDs(bln_Block, int_PDirect, int_Index, int_FreezeTimes);      //取出电量总的标识符
                        sng_Energy[int_Inc] = this.ReadData(str_ID, 4, 2);
                        System.Threading.Thread.Sleep(250);
                        if (sng_Energy[int_Inc] == -1) break;
                    }
                }

                //return bln_Result;

            }
            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
            return sng_Energy;
        }

        /// <summary>
        /// 换算电量标识符
        /// </summary>
        /// <param name="bln_Block">块操作</param>
        /// <param name="int_PDirect">功率类型，0=组合有功 1=P+ 2=P- 3=Q+ 4=Q- 5=Q1 6=Q2 7=Q3 8=Q4</param>
        /// <param name="int_TariffType">费率类型，0=总，1=尖, 2=峰，3=平，4=谷</param>
        /// <param name="int_FreezeTimes">结算次数，为0时表示读取当前</param>
        /// <returns></returns>
        private string GetEnergyIDs(bool bln_Block, int int_PDirect, int int_TariffType, int int_FreezeTimes)
        {
            if (bln_Block)
                return "000" + Convert.ToString(int_PDirect) + "ff" + int_FreezeTimes.ToString("D2");
            else
                return "000" + Convert.ToString(int_PDirect) + "0" + int_TariffType.ToString() + int_FreezeTimes.ToString("D2");
        }

        /// <summary>
        /// 清空电量
        /// </summary>
        ///  <param name="int_Type">类型 </param>
        /// <returns></returns>
        public override bool ClearEnergy()
        {
            //a)功能：清空电能表内电能量、最大需量及发生时间、冻结量、事件记录、负荷记录等数据
            //b)控制码：C=1AH
            //c)数据域长度：L=08H  password (4) + UserID(4)

            byte[] byt_Data = new byte[8];
            Array.Copy(BitConverter.GetBytes(Convert.ToInt32("0x" + this.protocolInfo.ClearDLPassword + this.protocolInfo.ClearDLClass, 16)), byt_Data, 4);
            string str_UserID = "00000000" + this.protocolInfo.UserID;
            Array.Copy(BitConverter.GetBytes(Convert.ToInt32("0x" + str_UserID.Substring(str_UserID.Length - 8), 16)), 0, byt_Data, 4, 4);
            bool bln_Sequela = false;
            byte[] byt_RevData = new byte[0];
            bool bln_Result = this.ExeCommand(this.GetAddressByte(), 0x1A, byt_Data, ref bln_Sequela, ref byt_RevData, 1200, 1100);
            return bln_Result;

        }
        /// <summary>
        /// 广播校时
        /// </summary>
        /// <param name="int_Type">测试类型</param>
        /// <param name="str_DateTime">日期时间</param>
        /// <returns></returns>
        public override bool BroadcastTime(DateTime broadCaseTime)
        {
            string str_DateTime = broadCaseTime.ToString("yyMMddHHmmss");

            try
            {
                //功能：强制从站与主站时间同步
                //b)控制码：C=08H
                //c)数据域长度：L=06H
                //d)数据域：YYMMDDhhmmss(年.月.日.时.分.秒)
                byte[] byt_GAddr=null;
                if(GlobalUnit.g_CommunType == CLDC_Comm.Enum.Cus_CommunType.通讯485)
                {
                    byt_GAddr = new byte[] { 0x99, 0x99, 0x99, 0x99, 0x99, 0x99 };
                }
                else if (GlobalUnit.g_CommunType == CLDC_Comm.Enum.Cus_CommunType.通讯蓝牙)
                {
                    byt_GAddr = this.GetAddressByte();
                }
                byte[] byt_Data = new byte[6];
                byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToInt64("0x" + str_DateTime, 16));
                Array.Copy(byt_Tmp, byt_Data, 6);
                bool bln_Result = this.ExeCommand(byt_GAddr, 0x08, byt_Data);
                return bln_Result;
            }
            catch (Exception e)
            {
                CLDC_DataCore.Const.GlobalUnit.g_MsgControl.OutMessage(e.Message, false);
                //this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 广播校时，点对点
        /// </summary>
        /// <param name="int_Type">测试类型</param>
        /// <param name="str_DateTime">日期时间</param>
        /// <returns></returns>
        public override bool BroadcastTimeByPoint(DateTime broadCaseTime)
        {
            string str_DateTime = broadCaseTime.ToString("yyMMddHHmmss");

            try
            {
                //功能：强制从站与主站时间同步
                //b)控制码：C=08H
                //c)数据域长度：L=06H
                //d)数据域：YYMMDDhhmmss(年.月.日.时.分.秒)

                byte[] byt_Data = new byte[6];
                byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToInt64("0x" + str_DateTime, 16));
                Array.Copy(byt_Tmp, byt_Data, 6);
                bool bln_Result = this.ExeCommand(this.GetAddressByte(), 0x08, byt_Data);
                return bln_Result;
            }
            catch (Exception e)
            {
                CLDC_DataCore.Const.GlobalUnit.g_MsgControl.OutMessage(e.Message, false);
                //this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        public override bool SetBreakRelayTime(int Time)
        {
            try
            {
                //功能：设置拉闸心跳帧，用于自保电功能
                //b)控制码：C=1EH
                //c)数据域长度：L=01H
                //d)数据域：06(06=21分)
                byte[] byt_GAddr = new byte[] { 0x99, 0x99, 0x99, 0x99, 0x99, 0x99 };
                byte[] byt_Data = new byte[1];
                byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToInt16("0x" + Time, 16));
                Array.Copy(byt_Tmp, byt_Data, 1);
                bool bln_Result = this.ExeCommand(byt_GAddr, 0x1E, byt_Data);
                return bln_Result;
            }
            catch (Exception e)
            {
                CLDC_DataCore.Const.GlobalUnit.g_MsgControl.OutMessage(e.Message, false);
                //this.m_str_LostMessage = e.ToString();
                return false;
            }
        }

        /// <summary>
        /// 读日期时间
        /// </summary>
        /// <param name="int_Type">读取类型 </param>
        /// <param name="str_DateTime">返回时间 yy-mm-dd hh:mm:ss</param>
        /// <returns></returns>

        public override DateTime ReadDateTime()
        {
            //    return base.ReadDateTime();
            //}
            string str_DateTime = string.Empty;
            //public bool ReadDateTime(int int_Type, ref string str_DateTime)
            //{
            //04 00	01 01
            //try
            //{
            bool bln_Sequela = false;
            byte[][] byt_ID = new byte[][]{new byte[]{0x01,0x01 ,0x00,0x04},
                                                   new byte[]{0x02,0x01 ,0x00,0x04}};
            //------------读日期
            byte[] byt_RevData = new byte[0];
            bool bln_Result = this.ExeCommand(this.GetAddressByte(), 0x11, byt_ID[0], ref bln_Sequela, ref byt_RevData, 1300, 1100);
            byte[] byt_DateTime = new byte[6];
            if (bln_Result)
            {
                if (byt_RevData[0] == byt_ID[0][0] && byt_RevData[1] == byt_ID[0][1]
                    && byt_RevData[2] == byt_ID[0][2] && byt_RevData[3] == byt_ID[0][3])
                {
                    byt_DateTime[2] = byt_RevData[5];
                    byt_DateTime[1] = byt_RevData[6];
                    byt_DateTime[0] = byt_RevData[7];

                    //----------读时间
                    byt_RevData = new byte[0];
                    bln_Result = this.ExeCommand(this.GetAddressByte(), 0x11, byt_ID[1], ref bln_Sequela, ref byt_RevData, 1400, 1100);
                    if (bln_Result)
                    {
                        if (byt_RevData[0] == byt_ID[1][0] && byt_RevData[1] == byt_ID[1][1]
                            && byt_RevData[2] == byt_ID[1][2] && byt_RevData[3] == byt_ID[1][3])
                        {
                            byt_DateTime[5] = byt_RevData[4];
                            byt_DateTime[4] = byt_RevData[5];
                            byt_DateTime[3] = byt_RevData[6];
                            string str_Tmp = BitConverter.ToString(byt_DateTime).Replace("-", "");
                            string[] str_Para = new string[] { "YY", "MM", "DD", "HH", "FF", "SS" };
                            string dateTimeFormat = protocolInfo.DateTimeFormat;
                            if (str_Tmp.Substring(0, 2) != "20") dateTimeFormat = dateTimeFormat.Replace("YYYY", "YY");
                            if (str_Tmp.Length < 14) dateTimeFormat = dateTimeFormat.Replace("WW", "");
                            for (int int_Inc = 0; int_Inc < 6; int_Inc++)
                            {
                                int int_Index = dateTimeFormat.IndexOf(str_Para[int_Inc]);
                                str_DateTime += str_Tmp.Substring(int_Index, 2);
                                if (int_Inc < 2)
                                    str_DateTime += "-";
                                else if (int_Inc == 2)
                                    str_DateTime += " ";
                                else if (int_Inc != 5)
                                    str_DateTime += ":";
                            }
                            //return true;
                        }
                        //else
                        //{
                        //    //this.m_str_LostMessage = "返回标识符不一致！";
                        //    return false;
                        //}
                    }
                    //else
                    //    return false;
                }
                //else
                //{
                //    //this.m_str_LostMessage = "返回标识符不一致！";
                //    return false;
                //}
            }
            else
            {
                str_DateTime = "01-01-01 00:00:00";
            }
            //    return false;
            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
            return DateTime.Parse(str_DateTime);
        }


        /// <summary>
        /// 读地址
        /// </summary>
        /// <param name="int_Type">类型，0=强制读取地址，1=探测读取地址</param>
        /// <param name="str_Address"></param>
        /// <returns></returns>

        public override string ReadAddress()
        {
            //    return base.ReadAddress();
            //}
            string str_Address = string.Empty;
            //public bool ReadAddress(int int_Type, ref string str_Address)
            //{
            //try
            //{
            bool bln_Result = CptReadAddress(ref str_Address);
            if (!bln_Result)
                bln_Result = this.DetectAddress(ref str_Address);
            //return bln_Result;
            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
            return str_Address;
        }



        /// <summary>
        /// 读取数据（数据型，数据项）
        /// </summary>
        /// <param name="str_ID">标识符,两个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="int_Dot">小数位</param>
        /// <param name="bln_Reverse">解释方式，true=高低位对调，false=高低位正常</param>
        /// <param name="sng_Value">返回数据</param>
        /// <returns></returns>

        public override float ReadData(string str_ID, int int_Len, int int_Dot)
        {
            
            float sng_Value = -1F;
            
            if (str_ID.Length != 8)
            {
                //this.m_str_LostMessage = "标识符不符合要求，不是4字节";
                //return false;
                return sng_Value;
            }
            byte[] byt_ID = BitConverter.GetBytes(Convert.ToInt32("0x" + str_ID, 16));
            byte[] byt_MyRevData = new byte[0];
            bool bln_Sequela = false;
            bool bln_Result = this.ExeCommandA(this.GetAddressByte(), 0x11, byt_ID, ref bln_Sequela, ref byt_MyRevData, 1500, 1100);
            if (bln_Result)
            {
                if (byt_MyRevData[0] == byt_ID[0] && byt_MyRevData[1] == byt_ID[1]
                    && byt_MyRevData[2] == byt_ID[2] && byt_MyRevData[3] == byt_ID[3])
                {
                    byte[] byt_TmpValue = new byte[int_Len];
                    Array.Copy(byt_MyRevData, 4, byt_TmpValue, 0, int_Len);
                    Array.Reverse(byt_TmpValue);   //倒序
                    string str_Tmp = "";
                    str_Tmp = BitConverter.ToString(byt_TmpValue, 0, int_Len);
                    str_Tmp = str_Tmp.Replace("-", "");
                    sng_Value = Convert.ToSingle(str_Tmp) / Convert.ToSingle(Math.Pow(10, int_Dot));
                    
                }
                
            }
            
            return sng_Value;
        }

        /// <summary>
        /// 读取数据（字符型，数据项）
        /// </summary>
        /// <param name="str_ID">标识符,2个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="revData">返回报文</param>
        /// <returns></returns>
        public override string ReadData(string str_ID, int int_Len, out string revData)
        {
            //    return base.ReadData(str_ID, int_Len);
            //}
            string str_Value = string.Empty;
            string str_revData = string.Empty;
            //public bool ReadData(int int_Type, string str_ID, int int_Len, ref string str_Value)
            //{
            //    try
            //    {
            if (str_ID.Length != 8)
            {
                //this.m_str_LostMessage = "标识符不符合要求，不是4字节";
                revData = str_revData;
                return str_Value;
            }
            byte[] byt_ID = BitConverter.GetBytes(Convert.ToInt32("0x" + str_ID, 16));
            byte[] byt_MyRevData = new byte[0];
            byte[] byt_MyRevDataOrig = new byte[0];
            bool bln_Sequela = false;
            bool bln_Result = this.ExeCommand(this.GetAddressByte(), 0x11, byt_ID, ref bln_Sequela, ref byt_MyRevData, 2000, 100, ref byt_MyRevDataOrig);
            str_revData = BitConverter.ToString(byt_MyRevDataOrig, 0, byt_MyRevDataOrig.Length);
            str_revData = str_revData.Replace("-", "");
            
            if (bln_Result)
            {
               
                if (byt_MyRevData[0] == byt_ID[0] && byt_MyRevData[1] == byt_ID[1]
                    && byt_MyRevData[2] == byt_ID[2] && byt_MyRevData[3] == byt_ID[3])
                {
                   
                  

                    if (int_Len > byt_MyRevData.Length - 4)
                        int_Len = byt_MyRevData.Length - 4;
                    byte[] byt_TmpValue = new byte[int_Len];
                    Array.Copy(byt_MyRevData, 4, byt_TmpValue, 0, int_Len);
                    Array.Reverse(byt_TmpValue);
                    str_Value = "";
                    str_Value = BitConverter.ToString(byt_TmpValue, 0, int_Len);
                    str_Value = str_Value.Replace("-", "");
                    //return true;
                }
                //else
                //{
                //    //this.m_str_LostMessage = "返回的标识符与下发指令不一致";
                //    return true;
                //}

            }
            //else
            //    return false;
            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
            revData = str_revData;
            return str_Value;
        }

        /// <summary>
        /// 读取数据（字符型，数据项）
        /// </summary>
        /// <param name="str_ID">标识符,两个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="bln_Reverse">解释方式，true=高低位对调，false=高低位正常</param>
        /// <param name="str_Value">返回数据</param>
        /// <returns></returns>
        public override string ReadData(string str_ID, int int_Len)
        {
            //    return base.ReadData(str_ID, int_Len);
            //}
            string str_Value = string.Empty;
            //public bool ReadData(int int_Type, string str_ID, int int_Len, ref string str_Value)
            //{
            //    try
            //    {
            if (str_ID.Length != 8)
            {
                //this.m_str_LostMessage = "标识符不符合要求，不是4字节";
                return str_Value;
            }
            byte[] byt_ID = BitConverter.GetBytes(Convert.ToInt32("0x" + str_ID, 16));
            byte[] byt_MyRevData = new byte[0];
            bool bln_Sequela = false;
            bool bln_Result = this.ExeCommand(this.GetAddressByte(), 0x11, byt_ID, ref bln_Sequela, ref byt_MyRevData, 2000, 100);
            if (bln_Result)
            {
                if (byt_MyRevData[0] == byt_ID[0] && byt_MyRevData[1] == byt_ID[1]
                    && byt_MyRevData[2] == byt_ID[2] && byt_MyRevData[3] == byt_ID[3])
                {
                    if (int_Len > byt_MyRevData.Length - 4)
                        int_Len = byt_MyRevData.Length - 4;
                    byte[] byt_TmpValue = new byte[int_Len];
                    Array.Copy(byt_MyRevData, 4, byt_TmpValue, 0, int_Len);
                    Array.Reverse(byt_TmpValue);
                    str_Value = "";
                    str_Value = BitConverter.ToString(byt_TmpValue, 0, int_Len);
                    str_Value = str_Value.Replace("-", "");
                    //return true;
                }
                //else
                //{
                //    //this.m_str_LostMessage = "返回的标识符与下发指令不一致";
                //    return true;
                //}

            }
            //else
            //    return false;
            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
            return str_Value;
        }




        /// <summary>
        /// 写日期时间
        /// </summary>
        /// <param name="int_Type">类型</param>
        /// <param name="str_DateTime">日期时间(YYMMDDhhmmss)</param>
        /// <returns></returns>

        public override bool WriteDateTime(string str_DateTime)
        {
            
            string str_Tmp = str_DateTime.Substring(0, 6) + "0" + Convert.ToString(GetWeekday(str_DateTime, this.protocolInfo.SundayIndex));
            bool bln_Result = this.WriteData("04000101", 4, str_Tmp);
            if (bln_Result)
            {
                str_Tmp = str_DateTime.Substring(6, 6);
                bln_Result = this.WriteData( "04000102", 3, str_Tmp);
            }

            return bln_Result;
            
        }

        /// <summary>
        /// 写费率1(字符型，数据项)
        /// </summary>
        /// <param name="str_ID">标识符</param>
        /// <param name="byt_Value">写入数据</param>
        /// <returns></returns>
        public override bool WriteRatesPrice(string str_ID, byte[] byt_Value)
        {

            if (str_ID.Length != 8)
            {
                //this.m_str_LostMessage = "标识符不符合要求，不是4字节";
                return false;
            }
            byte[] byt_ID = BitConverter.GetBytes(Convert.ToInt32("0x" + str_ID, 16));

            int int_DataLen = 12;                                //标识符4字节,用户代码4字节， 密码4字节
            int_DataLen += byt_Value.Length;                             //数据
            byte[] byt_Data = new byte[int_DataLen];
            Array.Copy(byt_ID, 0, byt_Data, 0, 4);

            byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToInt32("0x" + this.protocolInfo.WritePassword + protocolInfo.WriteClass, 16));
            Array.Copy(byt_Tmp, 0, byt_Data, 4, 4);

            string str_Tmp = this.protocolInfo.UserID;
            if (str_Tmp.Length > 8) str_Tmp = str_Tmp.Substring(0, 8);

            byt_Tmp = BitConverter.GetBytes(Convert.ToInt32("0x" + str_Tmp, 16));
            Array.Copy(byt_Tmp, 0, byt_Data, 8, 4);

            Array.Copy(byt_Value, 0, byt_Data, 12, byt_Value.Length);

            byte[] byt_MyRevData = new byte[0];
            bool bln_Sequela = false;
            return this.ExeCommand(this.GetAddressByte(), 0x14, byt_Data, ref bln_Sequela, ref byt_MyRevData, 1500, 1100);
        }
        /// <summary>
        /// 写数据(字符型，数据项)
        /// </summary>
        /// <param name="str_ID">标识符</param>
        /// <param name="byt_Value">写入数据</param>
        /// <returns></returns>

        public override bool WriteData(string str_ID, byte[] byt_Value)
        {
            //    return base.WriteData(str_ID, byt_Value);
            //}

            //public bool WriteData(int int_Type, string str_ID, byte[] byt_Value)
            //{
            //    try
            //    {
            if (str_ID.Length != 8)
            {
                //this.m_str_LostMessage = "标识符不符合要求，不是4字节";
                return false;
            }
            byte[] byt_ID = BitConverter.GetBytes(Convert.ToInt32("0x" + str_ID, 16));

            int int_DataLen = 12;                                //标识符4字节,用户代码4字节， 密码4字节
            int_DataLen += byt_Value.Length;                             //数据
            byte[] byt_Data = new byte[int_DataLen];
            Array.Copy(byt_ID, 0, byt_Data, 0, 4);

            byte[] byt_Tmp;
            int str_ID_type = CLDC_DataCore.Const.GlobalUnit.CheckStrIDType(str_ID);
            if (str_ID_type == 1)
            {
                byt_Tmp = BitConverter.GetBytes(Convert.ToInt32("0x" + this.protocolInfo.WritePassword + protocolInfo.WriteClass, 16));
            }
            else if (str_ID_type == 2)
            {
                byt_Tmp = BitConverter.GetBytes(Convert.ToInt32("0x" + this.protocolInfo.WritePassword2 + protocolInfo.WriteClass2, 16));
            }
            else
            {
                byt_Tmp = BitConverter.GetBytes(Convert.ToInt32("0x" + this.protocolInfo.WritePassword + this.protocolInfo.WriteClass, 16));
            }
            Array.Copy(byt_Tmp, 0, byt_Data, 4, 4);

            string str_Tmp = this.protocolInfo.UserID;
            if (str_Tmp.Length > 8) str_Tmp = str_Tmp.Substring(0, 8);

             byt_Tmp = BitConverter.GetBytes(Convert.ToInt32("0x" + str_Tmp, 16));
            Array.Copy(byt_Tmp, 0, byt_Data, 8, 4);

            Array.Copy(byt_Value, 0, byt_Data, 12, byt_Value.Length);

            byte[] byt_MyRevData = new byte[0];
            bool bln_Sequela = false;
            return this.ExeCommand(this.GetAddressByte(), 0x14, byt_Data, ref bln_Sequela, ref byt_MyRevData, 1500, 1100);

            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
        }

        public override bool WriteDataByMac(string str_ID, byte[] byt_Value)
        {
            //    return base.WriteData(str_ID, byt_Value);
            //}

            //public bool WriteData(int int_Type, string str_ID, byte[] byt_Value)
            //{
            //    try
            //    {
            if (str_ID.Length != 8)
            {
                //this.m_str_LostMessage = "标识符不符合要求，不是4字节";
                return false;
            }
            byte[] byt_ID = BitConverter.GetBytes(Convert.ToInt32("0x" + str_ID, 16));

            int int_DataLen = 12;                                //标识符4字节,用户代码4字节， 密码4字节
            int_DataLen += byt_Value.Length;                             //数据
            byte[] byt_Data = new byte[int_DataLen];
            Array.Copy(byt_ID, 0, byt_Data, 0, 4);

            byte[] byt_Tmp;
            int str_ID_type = CLDC_DataCore.Const.GlobalUnit.CheckStrIDType(str_ID);
            if (str_ID_type == 1)
            {
                byt_Tmp = BitConverter.GetBytes(Convert.ToInt32("0x" + this.protocolInfo.WritePassword + protocolInfo.WriteClass, 16));
            }
            else if (str_ID_type == 2)
            {
                byt_Tmp = BitConverter.GetBytes(Convert.ToInt32("0x" + this.protocolInfo.WritePassword2 + protocolInfo.WriteClass2, 16));
            }
            else
            {
                byt_Tmp = BitConverter.GetBytes(Convert.ToInt32("0x" + this.protocolInfo.WritePassword + this.protocolInfo.WriteClass, 16));
            }
            Array.Copy(byt_Tmp, 0, byt_Data, 4, 4);

            string str_Tmp = this.protocolInfo.UserID;
            if (str_Tmp.Length > 8) str_Tmp = str_Tmp.Substring(0, 8);

            byt_Tmp = BitConverter.GetBytes(Convert.ToInt32("0x" + str_Tmp, 16));
            Array.Copy(byt_Tmp, 0, byt_Data, 8, 4);

            Array.Copy(byt_Value, 0, byt_Data, 12, byt_Value.Length);

            byte[] byt_MyRevData = new byte[0];
            bool bln_Sequela = false;
            return this.ExeCommandByMac(this.GetAddressByte(), 0x14, byt_Data, ref bln_Sequela, ref byt_MyRevData, 1500, 1100);

            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
        }

        /// <summary>
        /// 写数据(数据型，数据项)
        /// </summary>
        /// <param name="str_ID">标识符</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="str_Value">写入数据</param>
        /// <returns></returns>

        public override bool WriteData(string str_ID, int int_Len, string str_Value)
        {
            
            byte[] byt_Data = new byte[int_Len];
            if (int_Len <= 8)                       //如果只写小于等于8字节的，可以直接转换
            {
                string str_Tmp = str_Value;
                if (str_Value.Length > int_Len * 2)
                    str_Tmp = str_Value.Substring(str_Value.Length - int_Len * 2);
                else if (str_Value.Length < int_Len * 2)
                    str_Tmp = "".PadLeft(int_Len * 2 - str_Value.Length, '0') + str_Value;
                byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToInt64("0x" + str_Tmp, 16));
                Array.Copy(byt_Tmp, byt_Data, int_Len);
            }
            else
                byt_Data = GetBytesArry(int_Len, str_Value, true);      //转换数据
            return WriteData( str_ID, byt_Data);
            
        }



        /// <summary>
        /// 写数据(字符型，数据块)
        /// </summary>
        /// <param name="str_ID">标识符,两个字节</param>
        /// <param name="int_Len">数据长度</param>
        /// <param name="str_Value">写入数据</param>
        /// <returns></returns>

        public override bool WriteData(string str_ID, int int_Len, string[] str_Value)
        {
            
            byte[] byt_Data = new byte[int_Len * str_Value.Length];
            for (int int_Inc = 0; int_Inc < str_Value.Length; int_Inc++)
            {
                byte[] byt_Tmp = new byte[0];
                if (int_Len <= 8)                       //如果只写小于等于8字节的，可以直接转换
                {
                    string str_Tmp = str_Value[int_Inc];
                    if (str_Tmp.Length > int_Len * 2)
                        str_Tmp = str_Tmp.Substring(str_Tmp.Length - int_Len * 2);
                    else if (str_Tmp.Length < int_Len * 2)
                        str_Tmp = "".PadLeft(int_Len * 2 - str_Value.Length, '0') + str_Tmp;
                    byt_Tmp = BitConverter.GetBytes(Convert.ToInt64("0x" + str_Tmp, 16));
                }
                else
                    byt_Tmp = GetBytesArry(int_Len, str_Value[int_Inc], true);      //转换数据

                Array.Copy(byt_Tmp, 0, byt_Data, int_Inc * int_Len, int_Len);

            }
            if (this.protocolInfo.BlockAddAA)
            {
                Array.Resize(ref byt_Data, byt_Data.Length + 1);
                byt_Data[byt_Data.Length - 1] = 0xaa;
            }
            return WriteData(str_ID, byt_Data);
            
        }

        
        /// <summary>
        /// 清空电量
        /// </summary>
        ///  <param name="strEndata">密文 </param>
        /// <returns></returns>
        public override bool ClearEnergy(string strEndata)
        {
            //a)功能：清空电能表内电能量、最大需量及发生时间、冻结量、事件记录、负荷记录等数据
            //b)控制码：C=1AH
            //c)数据域长度：L=08H  password (4) + UserID(4) + 密文(20)

            byte[] byt_Data = new byte[28];
            Array.Copy(BitConverter.GetBytes(Convert.ToInt32("0x" + this.protocolInfo.ClearDLPassword + this.protocolInfo.ClearDLClass, 16)), byt_Data, 4);
            string str_UserID = "00000000" + this.protocolInfo.UserID;
            Array.Copy(BitConverter.GetBytes(Convert.ToInt32("0x" + str_UserID.Substring(str_UserID.Length - 8), 16)), 0, byt_Data, 4, 4);
            Array.Copy(GetBytesArry(20, strEndata, false), 0, byt_Data, 8, 20);

            bool bln_Sequela = false;
            byte[] byt_RevData = new byte[0];
            bool bln_Result = this.ExeCommand(this.GetAddressByte(), 0x1A, byt_Data, ref bln_Sequela, ref byt_RevData, 1200, 1100);
            return bln_Result;

        }
        /// <summary>
        /// 钱包初始化
        /// </summary>
        ///  <param name="strEndata">密文 </param>
        /// <returns></returns>
        public override bool InitPurse(string strEndata)
        {
            //a)功能：钱包初始化            
            //c)数据域长度：L=08H  DI (4) + UserID(4) + 密文(16)

            byte[] byt_Data = new byte[24];
            Array.Copy(BitConverter.GetBytes(Convert.ToInt32("0x"+"070103FF", 16)), byt_Data, 4);
            string str_UserID = "00000000" + this.protocolInfo.UserID;
            Array.Copy(BitConverter.GetBytes(Convert.ToInt32("0x" + str_UserID.Substring(str_UserID.Length - 8), 16)), 0, byt_Data, 4, 4);
            Array.Copy(GetBytesArry(16, strEndata, false), 0, byt_Data, 8, 16);

            bool bln_Sequela = false;
            byte[] byt_RevData = new byte[0];
            bool bln_Result = this.ExeCommand(this.GetAddressByte(), 0x03, byt_Data, ref bln_Sequela, ref byt_RevData,2000,1000);
            return bln_Result;

        }
        //zhengrubin-20190920
        /// <summary>
        /// 设置多功能脉冲端子输出脉冲类型
        /// </summary>
        /// <param name="int_Type">操作类型  </param>
        /// <param name="ecp_PulseType">端子输出脉冲类型</param>
        /// <returns></returns>

        public override bool SetPulseCom(byte ecp_PulseType)
        {

            //a)功能：设置多功能端子输出信号类别 00时钟，01需量 02 时段投切
            //b)控制码：C=1DH
            //c)数据域长度：L=01H
            int int_PulseType = (int)ecp_PulseType;
            bool bln_Sequela = false;
            byte[] byt_RevData = new byte[0];
            byte[] byt_Data = new byte[1];
            byt_Data[0] = Convert.ToByte(int_PulseType);
            bool bln_Result = this.ExeCommand(this.GetAddressByte(), 0x1D, byt_Data, ref bln_Sequela, ref byt_RevData, 1200, 1100);
            return bln_Result;

        }

        /// <summary>
        /// 蓝牙连接
        /// </summary>
        /// <returns></returns>
        public override bool ConnectBlueTooth(string strAddress_MAC)
        {

            //发送：68 AA AA AA AA AA AA 68 91 11 34 33 B3 41 32 32 32 32 32 32 3B 33 33 33 33 F3 34 23 16
            //a)功能：蓝牙连接
            //b)控制码：C=91H
            //c)数据域长度：L=11H
            //d)数据标识0E800001
            //e)数据域13字节=LocalMAC(蓝牙模块地址默认FF：6字节)+SlaveMAC(表MAC地址：6字节)+Tx Power(发射功率默认=01：1字节)

            //回复：68 AA AA AA AA AA AA 68 11 05 35 33 B3 41 34 72 16
            //a)功能：蓝牙连接
            //b)控制码：C=11H
            //c)数据域长度：L=05H
            //d)数据标识0E800002
            //e)数据域1字节：0=断开，1=连接


            byte[] byt_address = new byte[] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA};
            byte[] byt_RevData = new byte[0];
            byte[] byt_Data = new byte[17];
            bool bln_Sequela = false;

            Array.Copy(GetBytesArry(4, "0E800001", true), 0, byt_Data, 0, 4);
            Array.Copy(GetBytesArry(6, "FFFFFFFFFFFF", true), 0, byt_Data, 4, 6);
            Array.Copy(GetBytesArry(6, strAddress_MAC, true), 0, byt_Data, 10, 6);
            Array.Copy(GetBytesArry(1, "01", true), 0, byt_Data, 16, 1);

            bool bln_Result = this.ExeCommandByBlueTooth(byt_address, 0x91, byt_Data, ref bln_Sequela, ref byt_RevData);
            if (bln_Result)
            {
                if (byt_RevData.Length >= 05)
                {
                    return byt_RevData[4] == 0x01 ? true : false;
                }
            }
            return false;
        }


        //读取冻结电量
        public override bool ReadSpecialEnergy(int bln_Block, int int_DLType, int int_Times, ref float[] flt_CurDL)
        {
            string str_ID = GetFreezeID(bln_Block, int_DLType, int_Times);      //取出电量的标识符
            float[] sng_EnergyArry = this.ReadDataBlock(str_ID, 4, 2);
            if (sng_EnergyArry.Length > 0)
            {
                flt_CurDL = sng_EnergyArry;
            }
            return true;
        }

        /// <param name="int_type">读取类型0=块读，1=分项读</param>
        /// <param name="int_DLType">读取类型 1=剩余电量，2=透支电量，3=(上1次)定时冻结正向有功电能,4=(上1次)日冻结正向有功电能
        private string GetFreezeID(int type, int int_DLType, int int_Times)
        {
            //0 为块  1 为分项
            string ID = "";
            if (int_DLType == 3)//定时冻结
                ID = "050001" + int_Times.ToString().PadLeft(2, '0');
            else if (int_DLType == 4)//日冻结
                ID = "050601" + int_Times.ToString().PadLeft(2, '0');
            else if (int_DLType == 5)//整点冻结
                ID = "050401" + int_Times.ToString().PadLeft(2, '0');
            else if (int_DLType == 6)//瞬时冻结
                ID = "050101" + int_Times.ToString().PadLeft(2, '0');
            else if (int_DLType == 7)//两套时区表切换
                ID = "050201" + int_Times.ToString().PadLeft(2, '0');
            else if (int_DLType == 8)//两套日时段表切换
                ID = "050301" + int_Times.ToString().PadLeft(2, '0');
            else if (int_DLType == 9)//两套费率电价切换
                ID = "050501" + int_Times.ToString().PadLeft(2, '0');
            else if (int_DLType == 10)//两套阶梯切换
                ID = "050501" + int_Times.ToString().PadLeft(2, '0');
            return ID;

        }

        /// <summary>
        /// 冻结命令
        /// </summary>
        /// <param name="int_Type">操作类型 0=广播冻结，1=普通冻结</param>
        /// <param name="str_DateHour">冻结时间，MMDDhhmm(月.日.时.分)数据域99DDhhmm表示以月为周期定时冻结，9999hhmm表示以日为周期定时冻结，999999mm表示以小时为周期定时冻结，99999999为瞬时冻结。</param>
        /// <returns></returns>
        public override bool FreezeCmd(string str_DateHour)
        {

            //a)功能：冻结电能表数据，冻结内容见冻结数据标识编码表。
            //b)控制码：C=16H
            //c)数据域长度：L=04H
            //d)数据域：MMDDhhmm(月.日.时.分)

            string str_Tmp = str_DateHour;
            if (str_DateHour.Length > 8)
                str_Tmp = str_DateHour.Substring(str_DateHour.Length - 8);
            else if (str_DateHour.Length < 8)
            {
                if (str_DateHour.Length % 2 == 0)
                    str_Tmp = str_Tmp.PadLeft(8 - str_DateHour.Length, '9');
                else
                    str_Tmp = str_Tmp.PadLeft(8 - str_DateHour.Length, '0');
            }
            byte[] byt_Data = BitConverter.GetBytes(Convert.ToInt32("0x" + str_Tmp, 16));
            //int_Type在协议中没有指定类型,原组件中默认为1
            //if (int_Type == 0)
            //{
            //    byte[] byt_GAddr = new byte[] { 0x99, 0x99, 0x99, 0x99, 0x99, 0x99 };
            //    return this.ExeCommand(byt_GAddr, 0x16, byt_Data);
            //}
            //else
            //{
            bool bln_Sequela = false;
            byte[] byt_RevData = new byte[0];
            return this.ExeCommand(this.GetAddressByte(), 0x16, byt_Data, ref bln_Sequela, ref byt_RevData, 1300, 1100);

        }
        /// <summary>
        /// 读取负荷记录（字符型，数据项）
        /// </summary>
        /// <param name="str_ID">标识符,两个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="bln_Reverse">解释方式，true=高低位对调，false=高低位正常</param>
        /// <param name="str_Value">返回数据</param>
        /// <returns></returns>
        public override string ReadData(string str_ID, int int_Len, string strItem)
        {

            string str_Value = string.Empty;

            if (str_ID.Length != 8)
            {
                //this.m_str_LostMessage = "标识符不符合要求，不是4字节";
                return str_Value;
            }
            byte[] byt_ID = BitConverter.GetBytes(Convert.ToInt32("0x" + str_ID, 16));
            byte[] byt_Tmp = GetBytesArry(6, strItem, true);
            byte[] byt_Read = new byte[byt_Tmp.Length + byt_ID.Length];
            byte[] byt_MyRevData = new byte[0];
            bool bln_Sequela = false;
            Array.Copy(byt_ID, 0, byt_Read, 0, byt_ID.Length);
            Array.Copy(byt_Tmp, 0, byt_Read, 4, byt_Tmp.Length);

            bool bln_Result = this.ExeCommand(this.GetAddressByte(), 0x11, byt_Read, ref bln_Sequela, ref byt_MyRevData, 2000, 100);
            if (bln_Result)
            {
                if (byt_MyRevData[0] == byt_ID[0] && byt_MyRevData[1] == byt_ID[1]
                    && byt_MyRevData[2] == byt_ID[2] && byt_MyRevData[3] == byt_ID[3])
                {
                    if (int_Len != byt_MyRevData.Length - 4)
                        int_Len = byt_MyRevData.Length - 4;
                    byte[] byt_TmpValue = new byte[int_Len];
                    Array.Copy(byt_MyRevData, 4, byt_TmpValue, 0, int_Len);
                    Array.Reverse(byt_TmpValue);
                    str_Value = "";
                    str_Value = BitConverter.ToString(byt_TmpValue, 0, int_Len);
                    str_Value = str_Value.Replace("-", "");
                    //return true;
                }


            }

            return str_Value;
        }

        /// <summary>
        /// 读取数据（数据型，数据块）
        /// </summary>
        /// <param name="str_ID">标识符,两个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="int_Dot">小数位</param>
        /// <param name="sng_Value">返回数据</param>
        /// <returns></returns>

        public override float[] ReadDataBlock(string str_ID, int int_Len, int int_Dot)
        {
            //    return base.ReadDataBlock(str_ID, int_Len, int_Dot);
            //}
            float[] sng_Value = new float[0];
            //public bool ReadData(int int_Type, string str_ID, int int_Len, int int_Dot, ref float[] sng_Value)
            //{
            //try
            //{
            if (str_ID.Length != 8)
            {
                //this.m_str_LostMessage = "标识符不符合要求，不是4字节";
                return sng_Value;// false;
            }
            byte[] byt_ID = BitConverter.GetBytes(Convert.ToInt32("0x" + str_ID, 16));
            byte[] byt_MyRevData = new byte[0];
            bool bln_Sequela = false;
            bool bln_Result = this.ExeCommand(this.GetAddressByte(), 0x11, byt_ID, ref bln_Sequela, ref byt_MyRevData, 1500, 1100);
            if (bln_Result)
            {

                if (byt_MyRevData[0] == byt_ID[0] && byt_MyRevData[1] == byt_ID[1]
                    && byt_MyRevData[2] == byt_ID[2] && byt_MyRevData[3] == byt_ID[3])
                {
                    byte[] byt_TmpValue = new byte[byt_MyRevData.Length - 4];
                    Array.Copy(byt_MyRevData, 4, byt_TmpValue, 0, byt_MyRevData.Length - 4);
                    Array.Reverse(byt_TmpValue);
                    string str_Tmp = BitConverter.ToString(byt_TmpValue, 0);
                    str_Tmp = str_Tmp.Replace("-", "");

                    str_Tmp = str_Tmp.TrimStart(new char[] { 'A' });        //去掉AA
                    str_Tmp = str_Tmp.TrimEnd(new char[] { 'A' });

                    if (str_Tmp.IndexOf("AA") > 0)      //防止各项数据项目用AA隔开
                    {
                        string[] str_Para = str_Tmp.Split(new string[] { "AA" }, StringSplitOptions.None);
                        if (int_Len * str_Para.Length * 2 + (str_Para.Length - 1) * 2 == str_Tmp.Length)
                        {
                            Array.Resize(ref sng_Value, str_Para.Length);
                            for (int int_Inc = 0; int_Inc < str_Para.Length; int_Inc++)
                                sng_Value[int_Inc] = Convert.ToSingle(str_Para[int_Inc]) / Convert.ToSingle(Math.Pow(10, int_Dot));
                            Array.Reverse(sng_Value);
                            //return true;
                        }
                    }

                    int int_Count = str_Tmp.Length / (int_Len * 2);
                    Array.Resize(ref sng_Value, int_Count);
                    for (int int_Inc = 0; int_Inc < int_Count; int_Inc++)
                        sng_Value[int_Inc] = Convert.ToSingle(str_Tmp.Substring(int_Inc * int_Len * 2, int_Len * 2)) / Convert.ToSingle(Math.Pow(10, int_Dot));
                    Array.Reverse(sng_Value);
                    //return true;
                }
                //else
                //{
                //    //this.m_str_LostMessage = "返回的标识符与下发指令不一致";
                //    return true;
                //}

            }
            //else
            //    return false;
            //}
            //catch (Exception e)
            //{
            //    //this.m_str_LostMessage = e.ToString();
            //    return false;
            //}
            return sng_Value;
        }

        /// <summary>
        /// 清空事件记录
        /// </summary>
        /// <param name="int_Type">操作类型</param>
        /// <param name="str_ID">事件清零内容 事件总清零=FFFFFFFF   分项事件清零=DI3DI2DI1FF</param>
        /// <returns></returns>

        public override bool ClearEventLog(string str_ID)
        {

            //a)功能：清空电能表内存储的全部或某类事件记录数据。
            //b)控制码：C=1BH
            //c)数据域长度：L=0CH
            //1）事件总清零 PAOP0OP1OP2O＋C0C1C2C3＋FFFFFFFF；
            //2）分项事件清零 PAOP0OP1OP2O＋C0C1C2C3＋事件记录数据标识（DI0用FF表示）

            byte[] byt_Data = new byte[12];
            Array.Copy(BitConverter.GetBytes(Convert.ToInt32("0x" + this.protocolInfo.ClearDLPassword + this.protocolInfo.ClearDLClass, 16)), byt_Data, 4);
            string str_Tmp = "00000000" + this.protocolInfo.UserID;
            Array.Copy(BitConverter.GetBytes(Convert.ToInt32("0x" + str_Tmp.Substring(str_Tmp.Length - 8), 16)), 0, byt_Data, 4, 4);
            str_Tmp = "00000000" + str_ID;
            Array.Copy(BitConverter.GetBytes(Convert.ToInt32("0x" + str_Tmp.Substring(str_Tmp.Length - 8), 16)), 0, byt_Data, 8, 4);
            bool bln_Sequela = false;
            byte[] byt_RevData = new byte[0];
            bool bln_Result = this.ExeCommand(this.GetAddressByte(), 0x1B, byt_Data, ref bln_Sequela, ref byt_RevData, 1200, 1100);
            return bln_Result;

        }

        /// <summary>
        /// 清空需量
        /// </summary>
        /// <param name="strEndata">密文</param>
        /// <returns></returns>
        public override bool ClearEventLog(string str_ID, string strEndata)
        {

            //a)功能：当前最大需量及发生时间数据清零
            //b)控制码：C=19H
            //c)数据域长度：L=08H  password (4) + UserID(4)
            byte[] byt_Data = new byte[28];

            Array.Copy(BitConverter.GetBytes(Convert.ToInt32("0x" + this.protocolInfo.ClearDemandPassword + this.protocolInfo.ClearDemandClass, 16)), byt_Data, 4);
            string str_UserID = "00000000" + this.protocolInfo.UserID;
            Array.Copy(BitConverter.GetBytes(Convert.ToInt32("0x" + str_UserID.Substring(str_UserID.Length - 8), 16)), 0, byt_Data, 4, 4);
            Array.Copy(GetBytesArry(20, strEndata, false), 0, byt_Data, 8, 20);

            bool bln_Sequela = false;
            byte[] byt_RevData = new byte[0];
            bool bln_Result = this.ExeCommand(this.GetAddressByte(), 0x1B, byt_Data, ref bln_Sequela, ref byt_RevData, 1200, 1100);
            return bln_Result;

        }


        #endregion

        #region --------------------私有函数---------------------------------





        /// <summary>
        /// 强制地址
        /// </summary>
        /// <param name="str_Address">返回地址</param>
        /// <returns></returns>
        private bool CptReadAddress(ref string str_Address)
        {
            try
            {
                //a)功能：请求读电能表通信地址，仅支持点对点通信。
                //b)地址域：AA…AAH
                //c)控制码：C=13H
                //d)数据域长度：L=00H
                byte[] byt_UAddr = new byte[] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA };
                byte[] byt_RevAddr = new byte[0];
                byte[] byt_RevData = new byte[0];
                bool bln_Sequela = false;
                bool bln_Result = this.ExeCommand(byt_UAddr, 0x13, new byte[0], ref bln_Sequela, ref byt_RevAddr,
                                                  ref byt_RevData, 1200, 1000);
                if (bln_Result)
                {
                    if (Array.Equals(byt_UAddr, byt_RevAddr))      //返回帧的地址域是否跟通用地址一样，一样则取数据域，不一样则就实际地址
                    {
                        Array.Reverse(byt_RevData);
                        string str_Addr = BitConverter.ToString(byt_RevData);
                        str_Address = str_Addr.Replace("-", "");
                        return true;
                    }
                    else     //返回帧的地址域不是广播地址,则是电能表实际地址
                    {
                        Array.Reverse(byt_RevAddr);
                        string str_Addr = BitConverter.ToString(byt_RevAddr);
                        str_Address = str_Addr.Replace("-", "");
                        return true;
                    }
                }
                return bln_Result;
            }
            catch (Exception e)
            {
                CLDC_DataCore.Const.GlobalUnit.g_MsgControl.OutMessage(e.Message, false);
                //this.m_str_LostMessage = e.ToString();
                return false;
            }
        }


        /// <summary>
        /// 探测地址
        /// </summary>
        /// <param name="str_Addr">返回地址</param>
        /// <returns></returns>
        private bool DetectAddress(ref string str_Addr)
        {

            try
            {
                //04 00	04 01
                byte[] byt_UAddr = new byte[] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA };
                byte[][] byt_Data = new byte[][] { new byte[] { 0x01,0x04,0x00,0x04 },          //设备号
                                                   new byte[] {0x01,0x04,0x00,0x04}  };        //表号

                for (int int_Inc = 0; int_Inc < byt_Data.Length; int_Inc++)
                {
                    byte[] byt_MyRevData = new byte[0];
                    byte[] byt_Addr = new byte[6];
                    bool bln_Sequela = false;
                    bool bln_Result = this.ExeCommand(byt_UAddr, 0x11, byt_Data[int_Inc], ref bln_Sequela, ref byt_Addr, ref byt_MyRevData, 1100, 1000);
                    if (bln_Result)
                    {
                        if (Array.Equals(byt_UAddr, byt_Addr))      //返帧的地址与通用地址一样,用读回的数据作为地址下发
                        {
                            if (byt_MyRevData.Length >= 10)
                            {
                                Array.Copy(byt_MyRevData, 4, byt_Addr, 0, 6);       //取出数据域中的数据作为地址，并下发指令，验证是否是地址
                                bln_Result = this.ExeCommand(byt_Addr, 0x11, byt_Data[int_Inc], ref bln_Sequela, ref byt_MyRevData, 1100, 1000);
                                if (bln_Result)
                                {
                                    str_Addr = BitConverter.ToString(byt_Addr).Replace("-", "");
                                    return true;
                                }
                            }
                        }
                        else       //如果跟通用地址不一样则就是电能表返回的实际地址
                        {
                            Array.Reverse(byt_Addr);
                            str_Addr = BitConverter.ToString(byt_Addr).Replace("-", "");
                            return true;
                        }
                    }
                }
                //this.m_str_LostMessage = "探测地址失败";
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
        /// 换算电量标识符
        /// </summary>
        /// <param name="bln_Block">块操作</param>
        /// <param name="int_PDirect">功率类型，0=P+ 1=P- 2=Q+ 3=Q- 4=Q1 5=Q2 6=Q3 7=Q4</param>
        /// <param name="int_TariffType">费率类型，0=总，1=尖, 2=峰，3=平，4=谷</param>
        /// <returns></returns>
        private string GetEnergyID(bool bln_Block, int int_PDirect, int int_TariffType)
        {
            if (bln_Block)
                return "000" + Convert.ToString(int_PDirect + 1) + "ff00";
            else
                return "000" + Convert.ToString(int_PDirect + 1) + "0" + int_TariffType.ToString() + "00";
        }





        /// <summary>
        /// 把任意16进制字符串转换为指定长度的byte数组
        /// </summary>
        /// <param name="int_Len">数组长度</param>
        /// <param name="str_Value">要转换的字符串</param>
        /// <param name="bln_Reverse">true翻转，false不翻转</param>
        /// <returns></returns>
        private byte[] GetBytesArry(int int_Len, string str_Value, bool bln_Reverse)
        {
            byte[] byt_Data = new byte[int_Len];
            string str_Tmp = str_Value;
            if (str_Value.Length > int_Len * 2)
                str_Tmp = str_Value.Substring(str_Value.Length - int_Len * 2);
            else if (str_Value.Length < int_Len * 2)
                str_Tmp = str_Value.PadLeft(int_Len * 2 - str_Value.Length, '0');
            if (bln_Reverse)
            {
                for (int int_Inc = 0; int_Inc < int_Len; int_Inc++)
                    byt_Data[int_Len - 1 - int_Inc] = Convert.ToByte(str_Tmp.Substring(int_Inc * 2, 2), 16);
            }
            else
            {
                for (int int_Inc = 0; int_Inc < int_Len; int_Inc++)
                    byt_Data[int_Inc] = Convert.ToByte(str_Tmp.Substring(int_Inc * 2, 2), 16);
            }
            return byt_Data;
        }


        /// <summary>
        /// 星期代码转换
        /// </summary>
        /// <param name="str_Date">日期</param>
        /// <param name="int_SundayIndex">星期日序号</param>
        /// <returns></returns>
        private int GetWeekday(string str_Date, int int_SundayIndex)
        {
            DateTime dte_DateTime = new DateTime(Convert.ToInt16(str_Date.Substring(0, 2)),
                                                 Convert.ToInt16(str_Date.Substring(2, 2)),
                                                 Convert.ToInt16(str_Date.Substring(4, 2)));

            int int_SysWeekday = (int)dte_DateTime.DayOfWeek;
            if (int_SundayIndex == 1)
                return int_SysWeekday + 1;
            else if (int_SundayIndex == 7)
            {
                if (int_SysWeekday == 0)
                    return 7;
                else
                    return int_SysWeekday;
            }
            else
                return int_SysWeekday;
        }
        /// <summary>
        /// 清空需量
        /// </summary>
        /// <param name="strEndata">密文</param>
        /// <returns></returns>
        public override bool ClearDemand(string strEndata)
        {

            //a)功能：当前最大需量及发生时间数据清零
            //b)控制码：C=19H
            //c)数据域长度：L=08H  password (4) + UserID(4)
            byte[] byt_Data = new byte[28];

            Array.Copy(BitConverter.GetBytes(Convert.ToInt32("0x" + this.protocolInfo.ClearDemandPassword + this.protocolInfo.ClearDemandClass, 16)), byt_Data, 4);
            string str_UserID = "00000000" + this.protocolInfo.UserID;
            Array.Copy(BitConverter.GetBytes(Convert.ToInt32("0x" + str_UserID.Substring(str_UserID.Length - 8), 16)), 0, byt_Data, 4, 4);
            Array.Copy(GetBytesArry(20, strEndata, false), 0, byt_Data, 8, 20);

            bool bln_Sequela = false;
            byte[] byt_RevData = new byte[0];
            bool bln_Result = this.ExeCommand(this.GetAddressByte(), 0x19, byt_Data, ref bln_Sequela, ref byt_RevData, 1200, 1100);
            return bln_Result;

        }

        /// <summary>
        /// 清空需量
        /// </summary>
        /// <returns></returns>
        public override bool ClearDemand()
        {

            //a)功能：当前最大需量及发生时间数据清零
            //b)控制码：C=19H
            //c)数据域长度：L=08H  password (4) + UserID(4)
            byte[] byt_Data = new byte[8];

            Array.Copy(BitConverter.GetBytes(Convert.ToInt32("0x" + this.protocolInfo.ClearDemandPassword + this.protocolInfo.ClearDemandClass, 16)), byt_Data, 4);
            string str_UserID = "00000000" + this.protocolInfo.UserID;
            Array.Copy(BitConverter.GetBytes(Convert.ToInt32("0x" + str_UserID.Substring(str_UserID.Length - 8), 16)), 0, byt_Data, 4, 4);
            bool bln_Sequela = false;
            byte[] byt_RevData = new byte[0];
            bool bln_Result = this.ExeCommand(this.GetAddressByte(), 0x19, byt_Data, ref bln_Sequela, ref byt_RevData, 1200, 1100);
            return bln_Result;

        }
        #endregion

        #region 读取需量

        /// <summary>
        /// 读取数据（字符型，数据块）
        /// </summary>
        /// <param name="str_ID">标识符,两个字节</param>
        /// <param name="int_Len">数据长度(字节数)</param>
        /// <param name="int_Dot">小数位</param>
        /// <param name="sng_Value">返回数据</param>
        /// <returns></returns>

        public override string[] ReadDataBlock(string str_ID, int int_Len)
        {

            string[] str_Value = new string[0];

            if (str_ID.Length != 8)
            {
                //this.m_str_LostMessage = "标识符不符合要求，不是4字节";
                return str_Value;
            }
            byte[] byt_ID = BitConverter.GetBytes(Convert.ToInt32("0x" + str_ID, 16));
            byte[] byt_MyRevData = new byte[0];
            bool bln_Sequela = false;
            bool bln_Result = this.ExeCommand(this.GetAddressByte(), 0x11, byt_ID, ref bln_Sequela, ref byt_MyRevData, 1500, 1100);
            if (bln_Result)
            {
                if (byt_MyRevData[0] == byt_ID[0] && byt_MyRevData[1] == byt_ID[1]
                    && byt_MyRevData[2] == byt_ID[2] && byt_MyRevData[3] == byt_ID[3])
                {
                    byte[] byt_TmpValue = new byte[byt_MyRevData.Length - 4];
                    Array.Copy(byt_MyRevData, 4, byt_TmpValue, 0, byt_MyRevData.Length - 4);
                    Array.Reverse(byt_TmpValue);
                    string str_Tmp = BitConverter.ToString(byt_TmpValue, 0);
                    str_Tmp = str_Tmp.Replace("-", "");

                    str_Tmp = str_Tmp.TrimStart(new char[] { 'A' });        //去掉AA
                    str_Tmp = str_Tmp.TrimEnd(new char[] { 'A' });

                    if (str_Tmp.IndexOf("AA") > 0)      //防止各项数据项目用AA隔开
                    {
                        string[] str_Para = str_Tmp.Split(new string[] { "AA" }, StringSplitOptions.None);
                        if (int_Len * str_Para.Length * 2 + (str_Para.Length - 1) * 2 == str_Tmp.Length)
                        {
                            Array.Resize(ref str_Value, str_Para.Length);
                            for (int int_Inc = 0; int_Inc < str_Para.Length; int_Inc++)
                                str_Value[int_Inc] = str_Para[int_Inc];
                            Array.Reverse(str_Value);

                        }
                    }

                    int int_Count = str_Tmp.Length / (int_Len * 2);
                    Array.Resize(ref str_Value, int_Count);
                    for (int int_Inc = 0; int_Inc < int_Count; int_Inc++)
                        str_Value[int_Inc] = str_Tmp.Substring(int_Inc * int_Len * 2, int_Len * 2);
                    Array.Reverse(str_Value);

                }

            }

            return str_Value;
        }

        /// <summary>
        /// 换算需量标识符
        /// </summary>
        /// <param name="bln_Block">块操作</param>
        /// <param name="int_PDirect">功率类型，0=P+ 1=P- 2=Q+ 3=Q- 4=Q1 5=Q2 6=Q3 7=Q4</param>
        /// <param name="int_TariffType">费率类型，0=总，1=尖,2=峰，3=平，4=谷</param>
        /// <returns></returns>
        private string GetDemandID(bool bln_Block, int int_PDirect, int int_TariffType, int int_FreezeTimes)
        {
            if (bln_Block)
                return "010" + Convert.ToString(int_PDirect + 1) + "ff" + int_FreezeTimes.ToString("D2");
            else
                return "010" + Convert.ToString(int_PDirect + 1) + "0" + int_TariffType.ToString() + int_FreezeTimes.ToString("D2");
        }

        /// <summary>
        /// 换算需量标识符
        /// </summary>
        /// <param name="bln_Block">块操作</param>
        /// <param name="int_PDirect">功率类型，0=P+ 1=P- 2=Q+ 3=Q- 4=Q1 5=Q2 6=Q3 7=Q4</param>
        /// <param name="int_TariffType">费率类型，0=总，1=尖,2=峰，3=平，4=谷</param>
        /// <returns></returns>
        private string GetDemandID(bool bln_Block, int int_PDirect, int int_TariffType)
        {
            if (bln_Block)
                return "010" + Convert.ToString(int_PDirect + 1) + "ff00";
            else
                return "010" + Convert.ToString(int_PDirect + 1) + "0" + int_TariffType.ToString() + "00";
        }

      

       

#endregion
    }
}
