using System;
using System.Collections.Generic;
using System.Text;
using CLDC_DeviceDriver.Drivers.Geny.Packets.Out;
using CLDC_Comm.Enum;

namespace CLDC_DeviceDriver.Drivers.Geny.Packets.Out
{

    /// <summary>
    /// 设置某一相电源 连接方式
    /// </summary>
    internal class Geny_RequestPowerModePacket : GenySendPacket
    {
        /// <summary>
        /// a,b,c相
        /// </summary>
        public PhaseType PhaseType
        {
            get;
            set;
        }

        /// <summary>
        /// 该项电源工作类型
        /// </summary>
        public GenyPowerType PowerType
        {
            get;
            set;
        }

        /// <summary>
        /// 接线方式
        /// </summary>
        public Cus_Clfs CLF
        {
            get;
            set;
        }

        /// <summary>
        /// 感性，容性类型
        /// </summary>
        public LCType LCType
        {
            get;
            set;
        }

        /// <summary>
        /// 设置功率输出元件
        /// </summary>
        public Cus_PowerYuanJiang YuangJiang
        {
            get;
            set;
        }

        /// <summary>
        /// 功率方向
        /// </summary>
        public Cus_PowerFangXiang FangXiang
        {
            get;
            set;
        }


        public Geny_RequestPowerModePacket()
        { }

        /// <summary>
        /// 
        /// </summary>
        public Geny_RequestPowerModePacket(PhaseType phase, GenyPowerType powerType, Cus_Clfs clf, LCType lcType, Cus_PowerYuanJiang yuanJiang, Cus_PowerFangXiang fuangXiang)
            : base(GetDriverId(phase), 0x11)
        {
            this.PhaseType = phase;
            this.CLF = clf;
            this.LCType = lcType;
            this.YuangJiang = yuanJiang;
            this.FangXiang = fuangXiang;
        }

        /// <summary>
        /// 已重写，
        /// </summary>
        /// <returns></returns>
        protected override byte[] GetBody()
        {

            string szJxfs = string.Empty;
            byte[] buf = new byte[4];

            //正逆
            buf[0] = (byte)this.PowerType;
            buf[1] = 0;

            //接线类型
            switch (this.CLF)
            {
                case Cus_Clfs.单相:
                    {
                        szJxfs = "D";
                        if (this.YuangJiang == Cus_PowerYuanJiang.H || YuangJiang == CLDC_Comm.Enum.Cus_PowerYuanJiang.A)
                            szJxfs = "DA";
                        else if (YuangJiang == CLDC_Comm.Enum.Cus_PowerYuanJiang.B)
                            szJxfs = "DB";
                        else if (YuangJiang == CLDC_Comm.Enum.Cus_PowerYuanJiang.C)
                            szJxfs = "DC";
                    }
                    break;

                case Cus_Clfs.三相三线:                
                    {
                        if (this.FangXiang == CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功)
                            szJxfs = "3WZ";
                        else if (FangXiang == CLDC_Comm.Enum.Cus_PowerFangXiang.反向有功)
                            szJxfs = "3WF";
                        else if (FangXiang == CLDC_Comm.Enum.Cus_PowerFangXiang.正向无功)
                            szJxfs = "3RZ";
                        else //反向无功
                            szJxfs = "3RF";
                    }
                    break;

                case Cus_Clfs.三相四线:
                    {
                        if (this.FangXiang == CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功)
                            szJxfs = "4WZ";
                        else if (this.FangXiang == CLDC_Comm.Enum.Cus_PowerFangXiang.反向有功)
                            szJxfs = "4WF";
                        else if (this.FangXiang == CLDC_Comm.Enum.Cus_PowerFangXiang.正向无功)
                            szJxfs = "4RZ";
                        else //反向无功
                            szJxfs = "4RF";
                    }
                    break;
                default:
                    {
                        szJxfs = "D";
                        break;
                    }
            }


            //这些指令，应该被确认，确认后，删除该行注释
            if (szJxfs.StartsWith("4W"))
            {
                buf[2] = 0;
            }
            else if (szJxfs.StartsWith("3W"))
            {
                buf[2] = 1;
            }
            else if (szJxfs.StartsWith("4R"))
            {
                buf[2] = 2;
            }
            else if (szJxfs.StartsWith("3R"))
            {
                buf[2] = 3;
            }
            else if (szJxfs.StartsWith("D"))
            {
                buf[2] = 4;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            //感性，容性
            buf[3] = (byte)(this.LCType);
            return buf;
        }


        /// <summary>
        /// 根据 接线方式解析，应该设置的相
        /// </summary>
        /// <param name="clfs"></param>
        /// <returns></returns>
        public static PhaseType[] ParseYuanJiang(Cus_Clfs clfs, Cus_PowerYuanJiang yuangJian)
        {
            List<PhaseType> phases = new List<PhaseType>();

            if (yuangJian == Cus_PowerYuanJiang.Error || CLDC_Comm.GlobalUnit.IsDan)
            {
                phases.Add(PhaseType.A);
                return phases.ToArray();
            }
            switch(clfs)
            {
                case Cus_Clfs.单相:            
                    phases.Add(PhaseType.A);
                    break;
                case Cus_Clfs.三相三线:                         
                    phases.Add(PhaseType.A);                
                    phases.Add(PhaseType.C);
                    break;
                case Cus_Clfs.三相四线:                            
                    phases.Add(PhaseType.A);                
                    phases.Add(PhaseType.B);                
                    phases.Add(PhaseType.C);
                    break;
                default:            
                    phases.Add(PhaseType.A);
                    break;
            }

            return phases.ToArray();
        }
    }
}
