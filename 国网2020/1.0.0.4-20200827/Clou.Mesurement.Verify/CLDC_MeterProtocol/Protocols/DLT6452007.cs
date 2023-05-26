using System;
using MeterProtocol.Packet.DLT645;
using MeterProtocol.Packet.DLT645.DLT6452007.In;
using MeterProtocol.Packet.DLT645.DLT6452007.Out;

namespace MeterProtocol.Protocols
{
    /// <summary>
    /// 2007协议
    /// </summary>
    class DLT6452007 : ProtocolBase
    {
        /// <summary>
        /// 通讯测试
        /// </summary>
        /// <returns></returns>
        public override bool ComTest()
        {
            byte[] byt_Data;
            switch (CommTestType)
            {
                case 1:
                    byt_Data = BitConverter.GetBytes(Convert.ToInt32("0x04000402", 16));
                    break;
                case 2:
                    byt_Data = BitConverter.GetBytes(Convert.ToInt32("0x00010000", 16));
                    break;

                default:
                    byt_Data = BitConverter.GetBytes(Convert.ToInt32("0x04000401", 16));
                    break;
            }
            DL645SendPacket sendPacket = new DL645SendPacket()
            {
                Address = MeterAddress,
                CmdCode = 0x11
            };
            sendPacket.Data.Add(byt_Data);
            DL645RecvPacket recvPacket = new DL645RecvPacket();
            if (SendData(sendPacket, recvPacket))
                return recvPacket.IsCmdCodeOk;
            return false;
        }

        /// <summary>
        /// 广播校时
        /// </summary>
        /// <param name="broadCaseTime"></param>
        /// <returns></returns>
        public override bool BroadcastTime(DateTime broadCaseTime)
        {
            string broadCastAddress = string.Empty;
            broadCastAddress = broadCastAddress.PadRight(12, '9');
            string strTime = broadCaseTime.ToString("yyMMDDHHMMSS");
            byte[] data = Util.Functions.String2BCDCode(strTime);
            Packet.DLT645.DL645SendPacket sendPacket = new Packet.DLT645.DL645SendPacket()
            {
                Address = broadCastAddress,
                CmdCode = 0x08,
                IsNeedReturn = false      // 广播校时不需要回复
            };
            sendPacket.Data.Add(data);
            Packet.DLT645.DL645RecvPacket recvPacket = null;
            return SendData(sendPacket, recvPacket);
        }

        /// <summary>
        /// 读取指定功率类型，指定费率的电量
        /// </summary>
        /// <param name="energyType"></param>
        /// <param name="tariffType"></param>
        /// <returns></returns>
        public override float ReadEnergy(byte energyType, byte tariffType)
        {
            bool bln_Block = (ReadEnergyType == 1);

            int int_Index = base.protocolInfo.TariffOrderType.IndexOf(tariffType.ToString()) + 1;      //按设置的费率排序取出电量

            string str_ID = GetEnergyID(bln_Block, energyType, tariffType);      //取出电量的标识符
            if (bln_Block)
            {
                //块读
                Single[] sng_EnergyArry = ReadDataBlock(str_ID, 4, 2);
                if (int_Index < sng_EnergyArry.Length)
                {
                    return sng_EnergyArry[int_Index];
                }
                else
                    return -1F;
            }
            else
                return this.ReadData(str_ID, 4, 2);
        }

        /// <summary>
        /// 读取指定功率方向下所有费率的电量
        /// </summary>
        /// <param name="energyType"></param>
        /// <returns></returns>
        public override float[] ReadEnergy(byte energyType)
        {
            float[] sng_Energy = new float[0];
            bool bln_Block = (ReadEnergyType == 1);

            if (bln_Block)
            {
                string str_ID = GetEnergyID(bln_Block, energyType, 0);      //取出电量的标识符
                Single[] sng_EnergyArry = ReadDataBlock(str_ID, 4, 2);
                // bool bln_Result = this.ReadData( str_ID, 4, 2, ref sng_EnergyArry);
                if (sng_EnergyArry.Length > 0)
                {

                    Array.Resize(ref sng_Energy, 5);
                    Array.Resize(ref sng_EnergyArry, 5);
                    sng_Energy[0] = sng_EnergyArry[0];
                    for (int int_Inc = 1; int_Inc < sng_EnergyArry.Length; int_Inc++)
                    {
                        int int_Index = Convert.ToInt16(protocolInfo.TariffOrderType.Substring(int_Inc - 1, 1));      //按设置的费率排序取出电量
                        sng_Energy[int_Index] = sng_EnergyArry[int_Inc];
                    }
                }
            }
            else
            {
                //Array.Resize(ref sng_Energy, 5);

                string str_ID = GetEnergyID(bln_Block, energyType, 0);      //取出电量总的标识符
                sng_Energy[0] = this.ReadData(str_ID, 4, 2);
                if (sng_Energy.Length != 5) Array.Resize(ref sng_Energy, 5);

                for (int int_Inc = 1; int_Inc < 5; int_Inc++)
                {
                    int int_Index = protocolInfo.TariffOrderType.IndexOf(int_Inc.ToString()) + 1;      //按设置的费率排序取出电量

                    str_ID = GetEnergyID(bln_Block, energyType, int_Index);      //取出电量总的标识符
                    sng_Energy[int_Inc] = ReadData(str_ID, 4, 2);
                    // bln_Result = this.ReadData( str_ID, 4, 2, ref sng_Energy[int_Inc]);
                    // if (!bln_Result) break;
                }
            }
            return sng_Energy;
        }

        private string GetEnergyID(bool bln_Block, byte energyType, int param3)
        {
            if (bln_Block)
                return "000" + Convert.ToString(energyType + 1) + "ff00";
            else
                return "000" + Convert.ToString(energyType + 1) + "0" + param3.ToString() + "00";

        }

        /// <summary>
        /// 读取指定功率类型，指定费率的需量
        /// </summary>
        /// <param name="energyType"></param>
        /// <param name="tariffType"></param>
        /// <returns>-1为读取失败</returns>
        public override float ReadDemand(byte energyType, byte tariffType)
        {
            float retValue = -1F;
            bool bln_Block = (ReadDemandType == 1);

            int int_Index = protocolInfo.TariffOrderType.IndexOf(tariffType.ToString()) + 1;      //按设置的费率排序取出需量

            string str_ID = GetDemandID(bln_Block, energyType, int_Index);      //取出需量的标识符
            if (bln_Block)
            {
                Single[] sng_DemandArry = this.ReadDataBlock(str_ID, 3, 4);

                if (sng_DemandArry.Length > 0)
                {
                    if (tariffType == 0)        //总需量
                    {
                        retValue = sng_DemandArry[0];
                    }
                    else                                //尖峰平谷需量
                    {
                        if (int_Index < sng_DemandArry.Length)
                        {
                            retValue = sng_DemandArry[int_Index];
                        }
                    }
                }
            }
            else
            {
                retValue = this.ReadData(str_ID, 3, 4);
            }
            return retValue;
        }

        public override float[] ReadDemand(byte energyType)
        {
            bool bln_Block = (ReadDemandType == 1);
            float[] sng_Demand = new float[0];
            if (bln_Block)
            {
                string str_ID = GetDemandID(bln_Block, energyType, 0);      //取出需量的标识符
                Single[] sng_DemandArry = this.ReadDataBlock(str_ID, 4, 2);
                if (sng_DemandArry.Length > 0)
                {
                    Array.Resize(ref sng_Demand, 5);
                    sng_Demand[0] = sng_DemandArry[0];
                    for (int int_Inc = 1; int_Inc < sng_DemandArry.Length; int_Inc++)
                    {
                        int int_Index = Convert.ToInt16(protocolInfo.TariffOrderType.Substring(int_Inc - 1, 1));      //按设置的费率排序取出需量
                        sng_Demand[int_Index] = sng_DemandArry[int_Inc];
                    }
                }
            }
            else
            {

                string str_ID = GetDemandID(bln_Block, energyType, 0);      //取出需量总的标识符
                sng_Demand[0] = this.ReadData(str_ID, 4, 2);

                for (int int_Inc = 1; int_Inc < 5; int_Inc++)
                {
                    int int_Index = protocolInfo.TariffOrderType.IndexOf(int_Inc.ToString()) + 1;      //按设置的费率排序取出需量

                    str_ID = GetDemandID(bln_Block, energyType, int_Index);      //取出需量费率的标识符
                    sng_Demand[int_Inc] = this.ReadData(str_ID, 4, 2);
                }
            }
            return sng_Demand;
        }

        internal static string GetDemandID(bool bln_Block, byte energyType, int tariffType)
        {
            if (bln_Block)
                return "010" + Convert.ToString(energyType + 1) + "ff00";
            else
                return "010" + Convert.ToString(energyType + 1) + "0" + tariffType.ToString() + "00";

        }


        public override DateTime ReadDateTime()
        {

            string dateTime = string.Empty;
            //分项读取
            //读取日期
            RequestReadDatePacket sendPacket = new RequestReadDatePacket()
            {
                Address = MeterAddress
            };

            RequestReadDateReplyPacket recvPacket = new RequestReadDateReplyPacket();
            if (SendData(sendPacket, recvPacket) && recvPacket.IsCmdCodeOk)
            {
                dateTime = recvPacket.DateString;
                //读取时间
                RequestReadTimePacket sendPacket2 = new RequestReadTimePacket()
                {
                    Address = MeterAddress,
                };
                RequestReadTimeReplayPacket recvPacket2 = new RequestReadTimeReplayPacket();
                if (SendData(sendPacket2, recvPacket2) && recvPacket2.IsCmdCodeOk)
                {
                    dateTime += recvPacket2.TimeString;
                    return DateTime.Parse(dateTime);
                }
            }

            return DateTime.Parse("19000101000000");

        }

        public override string ReadAddress()
        {
            byte[][] byt_Data = new byte[][] { new byte[] { 0x04, 0x00,0x04,0x01},          //设备号
                                                 new byte[] { 0x04, 0x00,0x04,0x02} 
                                                };

            foreach (byte[] data in byt_Data)
            {
                DL645SendPacket sendPacket = new DL645SendPacket()
                {
                    Address = "AAAAAAAAAAAA",
                    CmdCode = 0x11
                };
                sendPacket.Data.Add(data);
                RequestReadAddressReplayPacket recvPacket = new RequestReadAddressReplayPacket();
                if (SendData(sendPacket, recvPacket))
                {
                    return recvPacket.Address;
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 读取时段
        /// </summary>
        /// <returns></returns>
        public override string[] ReadPeriodTime()
        {
            int periodCount = 0;
            string[] arrRet = new string[0];
            //if (ReadPeriodTimeType == 2 || ReadPeriodTimeType == 3)
            //{
            periodCount = (int)ReadData("04000203", 1, 0);//C312 读取时段数量
            //}


            string[] str_Data = this.ReadDataBlock("04010001", 3);

            if (periodCount > 0)
            {
                if (str_Data.Length >= periodCount)
                {
                    Array.Resize(ref arrRet, Convert.ToInt16(periodCount));
                    for (int int_Inc = 0; int_Inc < Convert.ToInt16(periodCount); int_Inc++)
                        if (ReadPeriodTimeType == 0)
                            arrRet[int_Inc] = str_Data[int_Inc];
                        else
                            arrRet[int_Inc] = str_Data[int_Inc].Substring(2, 4) + str_Data[int_Inc].Substring(0, 2);
                }
                else
                {
                    Array.Resize(ref arrRet, Convert.ToInt16(str_Data.Length));
                    for (int int_Inc = 0; int_Inc < str_Data.Length; int_Inc++)
                        if (ReadPeriodTimeType == 0)
                            arrRet[int_Inc] = str_Data[int_Inc];
                        else
                            arrRet[int_Inc] = str_Data[int_Inc].Substring(2, 4) + str_Data[int_Inc].Substring(0, 2);
                }
            }

            return arrRet;
        }

        /// <summary>
        /// 读取指定标识符的数据
        /// </summary>
        /// <param name="str_ID">标识符</param>
        /// <param name="int_Len">数据长</param>
        /// <returns>读取到的字符串</returns>
        public override string ReadData(string str_ID, int int_Len)
        {
            DL645SendPacket sendPacket = new DL645SendPacket()
            {
                CmdCode = 0x01,
                Address = MeterAddress
            };
            RequestReadDataReplayPacket recvPacket = new RequestReadDataReplayPacket()
            {
                Len = int_Len
            };

            byte[] byt_ID = BitConverter.GetBytes(Convert.ToInt32("0x" + str_ID, 16));
            sendPacket.Data.Add(byt_ID);
            if (SendData(sendPacket, recvPacket) && recvPacket.IsCmdCodeOk)
            {
                return recvPacket.ReadData;
            }
            return string.Empty;
        }

        /// <summary>
        /// 读取指定小数位的数据
        /// </summary>
        /// <param name="str_ID"></param>
        /// <param name="int_Len"></param>
        /// <param name="int_Dot"></param>
        /// <returns></returns>
        public override float ReadData(string str_ID, int int_Len, int int_Dot)
        {
            string tmpValue = ReadData(str_ID, int_Len);
            if (!string.IsNullOrEmpty(tmpValue))
            {
                return Convert.ToSingle(tmpValue) / Convert.ToSingle(Math.Pow(10, int_Dot));
            }
            return -1F;
        }


        public override string[] ReadDataBlock(string str_ID, int int_Len)
        {
            DL645SendPacket sendPacket = new DL645SendPacket()
            {
                CmdCode = 0x01,
                Address = MeterAddress
            };
            RequestReadDataBlockReplayPacket recvPacket = new RequestReadDataBlockReplayPacket()
            {
                Len = int_Len
            };

            byte[] byt_ID = BitConverter.GetBytes(Convert.ToInt32("0x" + str_ID, 16));
            sendPacket.Data.Add(byt_ID);
            if (SendData(sendPacket, recvPacket) && recvPacket.IsCmdCodeOk)
            {
                return recvPacket.ReadData;
            }
            return new string[0];
        }

        public override float[] ReadDataBlock(string str_ID, int int_Len, int int_Dot)
        {
            string[] arrRead = ReadDataBlock(str_ID, int_Len);
            float[] arrRet = new float[arrRead.Length];
            int pos = 0;
            foreach (string data in arrRead)
            {
                arrRet[pos] = Convert.ToSingle(data) / Convert.ToSingle(Math.Pow(10, int_Dot));
                pos++;
            }
            return arrRet;
        }

        /// <summary>
        /// 写日期时间
        /// </summary>
        /// <param name="str_DateTime"></param>
        /// <returns></returns>
        public override bool WriteDateTime(string str_DateTime)
        {
            string str_Tmp = str_DateTime.Substring(0, 6) + "0" + Convert.ToString(GetWeekday(str_DateTime, protocolInfo.SundayIndex));
            bool bln_Result = this.WriteData("04000101", 4, str_Tmp);
            if (bln_Result)
            {
                str_Tmp = str_DateTime.Substring(6, 6);
                bln_Result = this.WriteData("04000102", 3, str_Tmp);
            }

            return bln_Result;
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
        /// 写表地址
        /// </summary>
        /// <param name="str_Address"></param>
        /// <returns></returns>
        public override bool WriteAddress(string str_Address)
        {
            DL645SendPacket sendPacket = new DL645SendPacket()
            {
                Address = "AAAAAAAAAAAA",
                CmdCode = 0x15
            };
            str_Address = str_Address.PadLeft(12, '0');
            str_Address = str_Address.Substring(0, 12);
            sendPacket.Data.Add(Util.Functions.String2BCDCode(str_Address));
            DL645RecvPacket recvPacket = new DL645RecvPacket();
            if (SendData(sendPacket, recvPacket))
            {
                return recvPacket.IsCmdCodeOk;
            }
            return false;
        }

        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="str_ID"></param>
        /// <param name="byt_Value"></param>
        /// <returns></returns>
        public override bool WriteData(string str_ID, byte[] byt_Value)
        {
            DL645SendPacket sendPacket = new DL645SendPacket()
            {
                Address = MeterAddress,
                CmdCode = 0x14
            };
            sendPacket.Data.Add(Util.Functions.String2BCDCode(str_ID)); //标识
            sendPacket.Data.Add(Util.Functions.String2BCDCode(protocolInfo.UserID));//用户代码
            sendPacket.Data.Add(Util.Functions.String2BCDCode(protocolInfo.WritePassword));//密码
            sendPacket.Data.Add(byt_Value);
            DL645RecvPacket recvPacket = new DL645RecvPacket();
            return recvPacket.IsCmdCodeOk;

        }


        public override bool WriteData(string str_ID, int int_Len, int int_Dot, float sng_Value)
        {
            byte[] byt_Data = new byte[int_Len];
            string str_Tmp = Convert.ToInt64(sng_Value * Math.Pow(10, int_Dot)).ToString();
            byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToInt64("0x" + str_Tmp, 16));
            Array.Copy(byt_Tmp, 0, byt_Data, 0, int_Len);
            return WriteData(str_ID, byt_Data);
        }

        public override bool WriteData(string str_ID, int int_Len, int int_Dot, float[] sng_Value)
        {
            byte[] byt_Data = new byte[int_Len * sng_Value.Length];

            for (int int_Inc = 0; int_Inc < sng_Value.Length; int_Inc++)
            {
                string str_Tmp = Convert.ToInt64(sng_Value[int_Inc] * Math.Pow(10, int_Dot)).ToString();
                byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToInt64("0x" + str_Tmp, 16));
                Array.Copy(byt_Tmp, 0, byt_Data, int_Inc * int_Len, int_Len);
            }
            if (protocolInfo.BlockAddAA)
            {
                Array.Resize(ref byt_Data, byt_Data.Length + 1);
                byt_Data[byt_Data.Length - 1] = 0xaa;
            }
            return WriteData(str_ID, byt_Data);
        }

        public override bool WriteData(string str_ID, int int_Len, string str_Value)
        {
            byte[] byt_Data = new byte[int_Len];
            byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToInt64("0x" + str_Value, 16));
            Array.Copy(byt_Tmp, byt_Data, int_Len);
            return WriteData(str_ID, byt_Data);
        }
        public override bool WriteData(string str_ID, int int_Len, string[] str_Value)
        {
            byte[] byt_Data = new byte[int_Len * str_Value.Length];
            for (int int_Inc = 0; int_Inc < str_Value.Length; int_Inc++)
            {
                byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToInt64("0x" + str_Value[int_Inc], 16));
                Array.Copy(byt_Tmp, 0, byt_Data, int_Inc * int_Len, int_Len);
            }
            if (protocolInfo.BlockAddAA)
            {
                Array.Resize(ref byt_Data, byt_Data.Length + 1);
                byt_Data[byt_Data.Length - 1] = 0xAA;
            }
            return WriteData(str_ID, byt_Data);
        }
        public override bool ClearDemand()
        {
            DL645SendPacket sendPacket = new DL645SendPacket()
            {
                Address = MeterAddress,
                CmdCode = 0x19
            };
            DL645RecvPacket recvPacket = new DL645RecvPacket();

            //指令清空
            byte[] byt_Data = Util.Functions.String2BCDCode(protocolInfo.ClearDemandPassword);
            sendPacket.Data.Add(byt_Data);
            byt_Data = Util.Functions.String2BCDCode(protocolInfo.UserID);
            sendPacket.Data.Add(byt_Data);
            if (SendData(sendPacket, recvPacket))
            {
                return recvPacket.IsCmdCodeOk;
            }
            return false;

        }

        /// <summary>
        /// 清空电量
        /// </summary>
        /// <returns></returns>
        public bool ClearEnergy()
        {

            //a)功能：清空电能表内电能量、最大需量及发生时间、冻结量、事件记录、负荷记录等数据
            //b)控制码：C=1AH
            //c)数据域长度：L=08H  password (4) + UserID(4)
            DL645SendPacket sendPacket = new DL645SendPacket()
            {
                Address = MeterAddress,
                CmdCode = 0x1A
            };
            DL645RecvPacket recvPacket = new DL645RecvPacket();

            //指令清空
            byte[] byt_Data = Util.Functions.String2BCDCode(protocolInfo.ClearDemandPassword);
            sendPacket.Data.Add(byt_Data);
            byt_Data = Util.Functions.String2BCDCode(protocolInfo.UserID);
            sendPacket.Data.Add(byt_Data);
            if (SendData(sendPacket, recvPacket))
            {
                return recvPacket.IsCmdCodeOk;
            }
            return false;

        }

        /// <summary>
        /// 清空事件记录
        /// </summary>
        /// <param name="str_ID">事件清零内容 事件总清零=FFFFFFFF   分项事件清零=DI3DI2DI1FF</param>
        /// <returns></returns>
        public bool ClearEventLog(string str_ID)
        {

            //a)功能：清空电能表内存储的全部或某类事件记录数据。
            //b)控制码：C=1BH
            //c)数据域长度：L=0CH
            //1）事件总清零 PAOP0OP1OP2O＋C0C1C2C3＋FFFFFFFF；
            //2）分项事件清零 PAOP0OP1OP2O＋C0C1C2C3＋事件记录数据标识（DI0用FF表示）
            DL645SendPacket sendPacket = new DL645SendPacket()
            {
                Address = MeterAddress,
                CmdCode = 0x1B
            };
            DL645RecvPacket recvPacket = new DL645RecvPacket();

            //指令清空
            byte[] byt_Data = Util.Functions.String2BCDCode(protocolInfo.ClearDemandPassword);
            sendPacket.Data.Add(byt_Data);
            byt_Data = Util.Functions.String2BCDCode(protocolInfo.UserID);
            sendPacket.Data.Add(byt_Data);
            byt_Data = Util.Functions.String2BCDCode(str_ID);
            sendPacket.Data.Add(byt_Data);
            if (SendData(sendPacket, recvPacket))
            {
                return recvPacket.IsCmdCodeOk;
            }
            return false;



        }
        /// <summary>
        /// 冻结命令
        /// </summary>
        /// <param name="str_DateHour">冻结时间，MMDDhhmm(月.日.时.分)数据域99DDhhmm表示以月为周期定时冻结，9999hhmm表示以日为周期定时冻结，999999mm表示以小时为周期定时冻结，99999999为瞬时冻结。</param>
        /// <returns></returns>
        public bool FreezeCmd(string str_DateHour)
        {

            //a)功能：冻结电能表数据，冻结内容见冻结数据标识编码表。
            //b)控制码：C=16H
            //c)数据域长度：L=04H
            //d)数据域：MMDDhhmm(月.日.时.分)
            DL645SendPacket sendPacket = new DL645SendPacket()
            {
                Address = MeterAddress,
                CmdCode = 0x16
            };
            DL645RecvPacket recvPacket = new DL645RecvPacket();

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
            byte[] byt_Data = Util.Functions.String2BCDCode(str_Tmp);

            sendPacket.Data.Add(byt_Data);

            if (SendData(sendPacket, recvPacket))
            {
                return recvPacket.IsCmdCodeOk;
            }
            return false;


        }
        /// <summary>
        /// 设置多功能脉冲端子输出脉冲类型
        /// </summary>
        /// <param name="ecp_PulseType">端子输出脉冲类型</param>
        /// <returns></returns>
        public bool SetPulseCom(byte ecp_PulseType)
        {

            //a)功能：设置多功能端子输出信号类别
            //b)控制码：C=1CH
            //c)数据域长度：L=08H
            DL645SendPacket sendPacket = new DL645SendPacket()
            {
                Address = MeterAddress,
                CmdCode = 0x1C
            };
            DL645RecvPacket recvPacket = new DL645RecvPacket();
            sendPacket.Data.Add(new byte[] { ecp_PulseType });

            if (SendData(sendPacket, recvPacket))
            {
                return recvPacket.IsCmdCodeOk;
            }
            return false;
        }
    }
}
