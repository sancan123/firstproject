using System;
using System.Collections.Generic;
using System.Text;
using MeterProtocol.Packet.DLT645.DLT6451997.Out;
using MeterProtocol.Packet.DLT645.DLT6451997.In;
using MeterProtocol.Packet.DLT645;
using MeterProtocol.Packet.DLT645;

namespace MeterProtocol.Protocols
{
    /// <summary>
    /// 645-1997协议
    /// </summary>
    class DLT6451997 : ProtocolBase
    {
        /// <summary>
        /// 通讯测试
        /// </summary>
        /// <returns></returns>
        public override bool ComTest()
        {
            Packet.DLT645.DL645SendPacket sendPacket = new Packet.DLT645.DL645SendPacket()
            {
                Address = MeterAddress,
                CmdCode = 0x01,
            };
            byte[] byt_Data = new byte[2];
            //根据不同的通讯测试类型发送不同的标签符
            switch (CommTestType)
            {
                case 1:
                    byt_Data[0] = 0xc0;
                    byt_Data[1] = 0x32;
                    break;
                case 2:
                    byt_Data[0] = 0xc0;
                    byt_Data[1] = 0x33;
                    break;
                case 3:
                    byt_Data[0] = 0xc0;
                    byt_Data[1] = 0x34;
                    break;
                case 4:
                    byt_Data[0] = 0x90;
                    byt_Data[1] = 0x1f;
                    break;
                default:
                    byt_Data[0] = 0x90;
                    byt_Data[1] = 0x10;
                    break;
            }
            sendPacket.Data.Add(byt_Data);
            Packet.DLT645.DL645RecvPacket recvPacket = new Packet.DLT645.DL645RecvPacket();
            if (SendData(sendPacket, recvPacket))
            {
                return (recvPacket.CmdCode == (0x01 | 0x80));
            }
            return false;
        }

        /// <summary>
        /// 广播校时
        /// </summary>
        /// <param name="broadCaseTime">新时间</param>
        /// <returns>校时是否成功</returns>
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

            string str_ID = this.GetDemandID(bln_Block, energyType, int_Index);      //取出需量的标识符
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
                string str_ID = this.GetDemandID(bln_Block, energyType, 0);      //取出需量的标识符
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

        public override DateTime ReadDateTime()
        {
            if (ReadDateTimeType != 1)
            {
                Packet.DLT645.DL645SendPacket sendPacket = new Packet.DLT645.DLT6451997.Out.RequestReadDateTimePacket()
                {
                    Address = MeterAddress,
                    CmdCode = 0x01,
                    ReadType = ReadDateTimeType
                };
                Packet.DLT645.DLT6451997.In.RequestReadDateTimeReplayPacket recvPacket = new Packet.DLT645.DLT6451997.In.RequestReadDateTimeReplayPacket();
                recvPacket.DateTimeFormat = protocolInfo.DateTimeFormat;
                if (SendData(sendPacket, recvPacket))
                {
                    if (recvPacket.IsCmdCodeOk)
                    {
                        return recvPacket.MeterTime;
                    }
                }
            }
            else
            {
                string dateTime = string.Empty;
                //分项读取
                //读取日期
                RequestReadDatePacket sendPacket = new RequestReadDatePacket()
                {
                    CmdCode = 0x01,
                    Address = MeterAddress,
                };
                RequestReadDateReplayPacket recvPacket = new RequestReadDateReplayPacket();
                if (SendData(sendPacket, recvPacket) && recvPacket.IsCmdCodeOk)
                {
                    dateTime = recvPacket.DateString;
                    //读取时间
                    RequestReadTimePacket sendPacket2 = new RequestReadTimePacket()
                    {
                        Address = MeterAddress,
                        CmdCode = 0x0,
                    };
                    RequestReadTimeReplayPacket recvPacket2 = new RequestReadTimeReplayPacket();
                    if (SendData(sendPacket2, recvPacket2) && recvPacket2.IsCmdCodeOk)
                    {
                        dateTime += recvPacket2.TimeString;
                        return DateTime.Parse(dateTime);
                    }
                }
            }
            return DateTime.Parse("19000101000000");

        }

        /// <summary>
        /// 读取电表地址
        /// </summary>
        /// <returns>电表地址</returns>
        public override string ReadAddress()
        {
            byte[][] byt_Data = new byte[][] { new byte[] { 0xc0, 0x34},          //设备号
                                                 new byte[] { 0xc0 ,0x33} ,         //用户号
                                                 new byte[] { 0xc0,0x32 } ,         //表号
                                                 new byte[] { 0xe4,0x83 }          //江苏预付费表的通信地址
                                                };

            foreach (byte[] data in byt_Data)
            {
                RequestReadAddressPacket sendPacket = new RequestReadAddressPacket();
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
            if (ReadPeriodTimeType == 2 || ReadPeriodTimeType == 3)
            {
                periodCount = (int)ReadData("C312", 1, 0);//C312 读取时段数量
            }

            if (ReadPeriodTimeType == 1 || ReadPeriodTimeType == 3)
            {
                string[] str_Data = this.ReadDataBlock("C33F", 3);
                if (ReadPeriodTimeType == 1)
                    periodCount = str_Data.Length;

                if (periodCount > 0)
                {
                    if (str_Data.Length >= periodCount)
                    {
                        Array.Resize(ref arrRet, periodCount);
                        for (int int_Inc = 0; int_Inc < periodCount; int_Inc++)
                            arrRet[int_Inc] = str_Data[int_Inc];
                    }
                    else
                    {
                        Array.Resize(ref arrRet, str_Data.Length);
                        for (int int_Inc = 0; int_Inc < str_Data.Length; int_Inc++)
                            arrRet[int_Inc] = str_Data[int_Inc];
                    }
                }
            }
            else
            {
                string[] str_Tmp = new string[periodCount];
                for (int int_Inc = 0; int_Inc < periodCount; int_Inc++)
                {      //读取每个时段
                    arrRet[int_Inc] = this.ReadData("C3" + Convert.ToString(31 + int_Inc), 3);
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

            byte[] byt_ID = BitConverter.GetBytes(Convert.ToInt16("0x" + str_ID, 16));
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

            byte[] byt_ID = BitConverter.GetBytes(Convert.ToInt16("0x" + str_ID, 16));
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
            string str_Week = "0" + GetWeekday(str_DateTime, protocolInfo.SundayIndex).ToString();
            if (WriteDateTimeType == 1)      //分项目操作
            {
                byte[] byt_Data = BitConverter.GetBytes(Convert.ToInt32("0x" + str_DateTime.Substring(0, 6) + str_Week, 16));

                bool bln_Result = this.WriteData("C010", byt_Data);
                if (bln_Result)
                {
                    byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToInt32("0x" + str_DateTime.Substring(6, 6), 16));
                    byt_Data = new byte[3];
                    Array.Copy(byt_Tmp, byt_Data, 3);
                    bln_Result = this.WriteData("C011", byt_Data);
                    return bln_Result;
                }
                return false;
            }
            else
            {
                string str_ID = "C0";
                if (WriteDateTimeType == 2)
                    str_ID += "1F";
                else
                    str_ID += "12";

                byte[] byt_Data;
                if (WriteDateTimeType == 3)
                {
                    byt_Data = new byte[6];
                    byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToInt64("0x" + str_DateTime.Substring(0, 12), 16));
                    Array.Copy(byt_Tmp, byt_Data, 6);
                }
                else
                {
                    byt_Data = new byte[7];
                    byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToInt64("0x" + str_DateTime.Substring(6, 6)
                                                                                + str_DateTime.Substring(0, 6)
                                                                                + str_Week, 16));
                    Array.Copy(byt_Tmp, byt_Data, 7);
                }
                if (protocolInfo.BlockAddAA)
                {
                    Array.Resize(ref byt_Data, byt_Data.Length + 1);
                    byt_Data[byt_Data.Length - 1] = 0xaa;
                }
                return WriteData(str_ID, byt_Data);
            }
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
                Address = "999999999999",
                CmdCode = 0x0a
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

        public override bool WriteData(string str_ID, byte[] byt_Value)
        {
            byte[] byt_ID = BitConverter.GetBytes(Convert.ToInt16("0x" + str_ID, 16));
            if (protocolInfo.VerifyPasswordType > 0)       //需要密码验证
            {
                if (!VerifyPassword(protocolInfo.VerifyPasswordType, protocolInfo.WritePassword))    //验证是否通过
                {
                    return false;
                }
            }
            int int_DataLen = 2;                                        //标识符两字节
            if (protocolInfo.DataFieldPassword) int_DataLen += 4;       //是否带密码，4字节
            int_DataLen += byt_Value.Length;                             //数据
            byte[] byt_Data = new byte[int_DataLen];
            Array.Copy(byt_ID, 0, byt_Data, 0, 2);
            if (protocolInfo.DataFieldPassword)                //数据域包含密码
            {
                byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToInt32("0x" + protocolInfo.WritePassword, 16));
                Array.Copy(byt_Tmp, 0, byt_Data, 2, 4);
                Array.Copy(byt_Value, 0, byt_Data, 6, byt_Value.Length);
            }
            else
                Array.Copy(byt_Value, 0, byt_Data, 2, byt_Value.Length);

            DL645SendPacket sendPacket = new DL645SendPacket()
            {
                Address = MeterAddress,
                CmdCode = 0x04
            };
            sendPacket.Data.Add(byt_Data);
            DL645RecvPacket recvPacket = new DL645RecvPacket();
            return recvPacket.IsCmdCodeOk;
            //byte[] byt_MyRevData = new byte[0];
            //bool bln_Sequela = false;
            //return this.ExeCommand(this.m_byt_Address, 0x04, byt_Data, ref bln_Sequela, ref byt_MyRevData, 1500, 1100);

        }
        private bool VerifyPassword(int verifyType, string pwd)
        {
            byte[] cmdCode = new byte[] { 0x05, 0x08, 0x14, 0x0e, 0x04, 0x04 };
            DL645SendPacket sendPacket = new DL645SendPacket()
            {
                Address = MeterAddress,
                CmdCode = cmdCode[verifyType - 1]
            };
            byte[] byt_Data = null;
            if (verifyType == 1)
            {
                //科陆A型表
                byt_Data = new byte[5];              //密码(3)+密码等级(1)+编程有效时间(1)
                byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToInt32("0x" + pwd, 16));
                Array.Copy(byt_Tmp, 0, byt_Data, 2, 4);
                byt_Data[4] = 0x0A;
            }
            else if (verifyType == 2)
            {
                //浩宁达表验证方式
                byt_Data = new byte[7];              //密码(6)+密码等级(1)
                string str_Tmp = "00000000000000".Substring(0, 14 - pwd.Length) + pwd;
                byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToUInt64("0x" + str_Tmp, 16));
                Array.Copy(byt_Tmp, 0, byt_Data, 0, 7);
            }
            else if (verifyType == 3)
            {
                //恒星表验证方式
                byt_Data = new byte[4];              //密码(3字节)+密码等级(1字节)
                byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToUInt64("0x" + pwd, 16));
                Array.Copy(byt_Tmp, 0, byt_Data, 0, 4);

            }
            else if (verifyType == 4)
            {
                // 三星表验证方式 0x0E
                byt_Data = new byte[4];              //密码(3字节)+密码等级(1字节)
                byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToUInt64("0x" + pwd, 16));
                Array.Copy(byt_Tmp, 0, byt_Data, 0, 4);

            }
            else if (verifyType == 5)
            {
                //爱拓利验证方式
                byt_Data = new byte[6];              //密码(3字节)+密码等级(1字节)
                byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToUInt16("0xC212", 16));
                Array.Copy(byt_Tmp, 0, byt_Data, 0, 2);
                byt_Tmp = BitConverter.GetBytes(Convert.ToUInt64("0x" + pwd, 16));
                Array.Copy(byt_Tmp, 0, byt_Data, 2, 4);

            }
            else if (verifyType == 6)
            {
                //红相3000型645表验证方式 EF00
                byt_Data = new byte[12];              //标识符+用户名(6字节)+密码(3字节)+密码等级(1字节)
                byte[] byt_Tmp = BitConverter.GetBytes(Convert.ToUInt16("0xEF00", 16));
                Array.Copy(byt_Tmp, 0, byt_Data, 0, 2);
                string str_Tmp = "";
                if (protocolInfo.UserID.Length >= 12)
                    str_Tmp = protocolInfo.UserID.Substring(0, 12);
                else
                    str_Tmp = "0000000000000".Substring(0, 12 - protocolInfo.UserID.Length) + protocolInfo.UserID;

                byt_Tmp = BitConverter.GetBytes(Convert.ToUInt64("0x" + str_Tmp, 16));
                Array.Copy(byt_Tmp, 0, byt_Data, 2, 6);

                if (pwd.Length > 8)
                    str_Tmp = pwd.Substring(0, 8);
                else
                    str_Tmp = "0000000000000".Substring(0, 8 - pwd.Length) + pwd;

                byt_Tmp = BitConverter.GetBytes(Convert.ToUInt64("0x" + str_Tmp, 16));
                Array.Copy(byt_Tmp, 0, byt_Data, 8, 4);
            }
            sendPacket.Data.Add(byt_Data);
            DL645RecvPacket recvPakcet = new DL645RecvPacket();
            if (SendData(sendPacket, recvPakcet))
            {
                return recvPakcet.IsCmdCodeOk;
            }
            return false;
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
                Address = MeterAddress
            };
            DL645RecvPacket recvPacket = new DL645RecvPacket();
            if (ClearDemandType == 1)
            {
                //指令清空
                byte[] byt_Data = BitConverter.GetBytes(Convert.ToInt32("0x" + protocolInfo.ClearDemandPassword, 16));
                sendPacket.CmdCode = 0x10;
                sendPacket.Data.Add(byt_Data);
                if (SendData(sendPacket, recvPacket))
                {
                    return recvPacket.IsCmdCodeOk;
                }
                return false;
            }
            else
            {
                //抄表日清空
                string str_CBR = this.ReadData("C117", 2);
                if (str_CBR.Length != 4) str_CBR = "0100";  //默认1号0时
                DateTime dte_DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, Convert.ToInt16(str_CBR.Substring(0, 2)), Convert.ToInt16(str_CBR.Substring(2, 2)), 0, 0);
                dte_DateTime = dte_DateTime.AddSeconds(-5);
                string str_DateTime = dte_DateTime.ToString("yyMMddHHmmss");
                return WriteDateTime(str_DateTime);

            }
        }

        public override bool ClearEnergy()
        {
            DL645SendPacket sendPacket = new DL645SendPacket()
            {
                Address = MeterAddress,
                CmdCode = 0x04
            };
            byte[] byt_EPsw = BitConverter.GetBytes(Convert.ToUInt64("0x" + protocolInfo.ClearDLPassword, 16));
            byte[] byt_Data;
            switch (ClearEnergyType)
            {
                case 1:                     //C119  20字节  

                    byt_Data = new byte[22];
                    byt_Data[1] = 0x19;
                    byt_Data[0] = 0xc1;
                    sendPacket.Data.Add(byt_Data);
                    sendPacket.Data.Add(byt_EPsw);
                    break;
                case 2:                     //C119  16字节  

                    byt_Data = new byte[22];
                    byt_Data[1] = 0x19;
                    byt_Data[0] = 0xc1;
                    sendPacket.Data.Add(byt_Data);
                    sendPacket.Data.Add(byt_EPsw);
                    break;

                case 3:
                    sendPacket.CmdCode = 0x09;
                    sendPacket.Data.Add(byt_EPsw);

                    break;
                case 4:                     //江苏单相表
                case 5:
                    /* 2010-03-11 by Gqs 江苏单相表电量清零规则 ？？？*/
                    byt_Data = new byte[6];
                    byt_Data[1] = 0x19;
                    byt_Data[0] = 0xc1;
                    sendPacket.Data.Add(byt_Data);
                    sendPacket.Data.Add(byt_EPsw);

                    break;

                default:
                    break;
            }
            DL645RecvPacket recvPacket = new DL645RecvPacket();
            if (SendData(sendPacket, recvPacket) && recvPacket.IsCmdCodeOk)
                return true;
            else
                return false;
        }

        public override bool ClearEventLog(string str_ID)
        {
            return base.ClearEventLog(str_ID);
        }

        public override bool SetPulseCom(byte ecp_PulseType)
        {
            
            return false;
        }

        #region 私有成员
        /// <summary>
        /// 换算电量标识符
        /// </summary>
        /// <param name="bln_Block">块操作</param>
        /// <param name="int_PDirect">功率类型，0=P+ 1=P- 2=Q+ 3=Q- 4=Q1 5=Q2 6=Q3 7=Q4</param>
        /// <param name="int_TariffType">费率类型，0=总，1=尖,2=峰，3=平，4=谷</param>
        /// <returns></returns>
        private string GetEnergyID(bool bln_Block, int int_PDirect, int int_TariffType)
        {
            byte[] byt_ID = new byte[2];
            byt_ID[0] = Convert.ToByte(int_PDirect > 1 ? 0x91 : 0x90);
            if (bln_Block)
            {
                if (int_PDirect > 4)        //二三四限象没有按顺序
                {
                    if (int_PDirect == 5)
                        byt_ID[1] = 0x5f;
                    else if (int_PDirect == 6)
                        byt_ID[1] = 0x6f;
                    else
                        byt_ID[1] = 0x4f;
                }
                else
                    byt_ID[1] = Convert.ToByte(int_PDirect > 1 ? 0x1f + 0x10 * (int_PDirect - 2) : 0x1f + 0x10 * int_PDirect);
            }
            else
            {
                if (int_PDirect > 4)    //二三四限象没有按顺序
                {
                    if (int_PDirect == 5)
                        byt_ID[1] = 0x50;
                    else if (int_PDirect == 6)
                        byt_ID[1] = 0x60;
                    else
                        byt_ID[1] = 0x40;

                    byt_ID[1] += Convert.ToByte(int_TariffType);
                }
                else
                    byt_ID[1] = Convert.ToByte(int_PDirect > 1 ? 0x10 + 0x10 * (int_PDirect - 2) + int_TariffType : 0x10 + 0x10 * int_PDirect + int_TariffType);
            }
            return BitConverter.ToString(byt_ID).Replace("-", "");
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
            byte[] byt_ID = new byte[2];
            byt_ID[0] = Convert.ToByte(int_PDirect > 1 ? 0xA1 : 0xA0);
            if (bln_Block)
            {
                if (int_PDirect > 4)        //二三四限象没有按顺序
                {
                    if (int_PDirect == 5)
                        byt_ID[1] = 0x5f;
                    else if (int_PDirect == 6)
                        byt_ID[1] = 0x6f;
                    else
                        byt_ID[1] = 0x4f;
                }
                else
                    byt_ID[1] = Convert.ToByte(int_PDirect > 1 ? 0x1f + 0x10 * (int_PDirect - 2) : 0x1f + 0x10 * int_PDirect);
            }
            else
            {
                if (int_PDirect > 4)    //二三四限象没有按顺序
                {
                    if (int_PDirect == 5)
                        byt_ID[1] = 0x50;
                    else if (int_PDirect == 6)
                        byt_ID[1] = 0x60;
                    else
                        byt_ID[1] = 0x40;

                    byt_ID[1] += Convert.ToByte(int_TariffType);
                }
                else
                    byt_ID[1] = Convert.ToByte(int_PDirect > 1 ? 0x10 + 0x10 * (int_PDirect - 2) + int_TariffType : 0x10 + 0x10 * int_PDirect + int_TariffType);
            }
            return BitConverter.ToString(byt_ID).Replace("-", "");
        }


        /// <summary>
        /// 星期代码转换
        /// </summary>
        /// <param name="str_Date">日期</param>
        /// <param name="int_SundayIndex">星期日序号</param>
        /// <returns></returns>
        private int GetWeekday(string str_Date, int int_SundayIndex)
        {
            DateTime dte_DateTime = new DateTime(2000 + Convert.ToInt16(str_Date.Substring(0, 2)),
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

        #endregion
    }
}
