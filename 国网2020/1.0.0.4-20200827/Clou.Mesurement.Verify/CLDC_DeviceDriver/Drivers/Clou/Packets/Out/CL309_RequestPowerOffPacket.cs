using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_DeviceDriver.Drivers.Clou.V80.Packets.Out
{
    /// <summary>
    /// 控制源 请求包
    /// </summary>
    internal class CL309_RequestPowerOffPacket : Cl309SendPacket
    {
        public bool IsLink = true;

        //存储 电压电流
        public static float oldUa = 0.00F;
        public static float oldUb = 0.00F;
        public static float oldUc = 0.00F;
        public static float oldIa = 0.00F;
        public static float oldIb = 0.00F;
        public static float oldIc = 0.00F;
         

        private const int AskDat = 0xA0;  //
        private const int WrtDat = 0xA3; //
        private const int WrtAry = 0xA6;//
        private const int EchOk = 0x30;	//
        private const int EchErr = 0x33;	//
        private const int EchBsy = 0x35; //
        private const int EchInh = 0x36;	//

        private const int HeadPos = 0;
        private const int RxIDPos = 1;
        private const int TxIDPos = 2;
        private const int FlenPos = 3;
        private const int ComdPos = 4;
        private const int FRAME_ID = 0x81;

        private const int PagePos = 5;
        private const int GrpPos = 6;
        private const int AryPos = 6;
        private const int Start0Pos = 7;
        private const int Start1Pos = 8;
        private const int LenPos = 9;

        private const int Grp0Pos = 7;
        private const int Grp1Pos = 8;
        private const int Grp2Pos = 9;
        private const int Grp3Pos = 10;
        private const int Grp4Pos = 11;
        private const int Grp5Pos = 12;
        private const int Grp6Pos = 13;
        private const int Grp7Pos = 14;

        private const int LOCAL_ID = 0x05;
        private const int OBJ_ID = 0x01;


        private const int PAGE0 = 0;
        private const int PAGE1 = 1;
        private const int PAGE2 = 2;
        private const int PAGE3 = 3;
        private const int PAGE4 = 4;
        private const int PAGE5 = 5;
        private const int PAGE6 = 6;

        private const int GROUP0 = 0x01;
        private const int GROUP1 = 0x02;
        private const int GROUP2 = 0x04;
        private const int GROUP3 = 0x08;
        private const int GROUP4 = 0x10;
        private const int GROUP5 = 0x20;
        private const int GROUP6 = 0x40;
        private const int GROUP7 = 0x80;

        private const int DATA0 = 0x01;
        private const int DATA1 = 0x02;
        private const int DATA2 = 0x04;
        private const int DATA3 = 0x08;
        private const int DATA4 = 0x10;
        private const int DATA5 = 0x20;
        private const int DATA6 = 0x40;
        private const int DATA7 = 0x80;

        private const int COS_SIN_BEISHU = 10000;//COSIN放大倍数
        private const int JIAODU_BEISHU = 10000;//角度放大倍数
        private const int PINLV_BEISHU = 10000;//频率放大倍数


        private double m_UaXwValue = 0;//Ua相位角度
        private double m_UbXwValue = 240;//Ub相位角度
        private double m_UcXwValue = 120;//UC相位角度
        private double m_IaXwValue = 0;//IA相位角度
        private double m_IbXwValue = 240;
        private double m_IcXwValue = 120;

        private float _Ua;
        private float _Ub;
        private float _Uc;

        private float _Ia;
        private float _Ib;
        private float _Ic;

        private float _PhiUa;
        private float _PhiUb;
        private float _PhiUc;

        private float _PhiIa;
        private float _PhiIb;
        private float _PhiIc;

        private float _Hz;

        private int _iChange = 0x00;

        private bool _bCurrentDB;

        public CL309_RequestPowerOffPacket()
            : base()
        {}

        public void SetPara()
        {
            SetPara(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 50, 7, 0x00, false, false, false, false);
        }

        public void SetPara(float Ua, float Ub, float Uc, float Ia, float Ib, float Ic, float PhiUa, float PhiUb, float PhiUc,
            float PhiIa, float PhiIb, float PhiIc, float Hz, int iClfs, byte Xwkz, bool XieBo, bool bDBOut, bool bHuanJ, bool bBHuan)
        {            
            //Ia 是否变化， 
            if (oldIa == Ia)
            {
                _iChange += 0x00;
            }
            else
            {
                _iChange += DATA5;
                oldIa = Ia;
            }
            //Ib 是否变化， 
            if (oldIb == Ib)
            {
                _iChange += 0x00;
            }
            else
            {
                _iChange += DATA4;
                oldIb = Ib;
            }
            //Ic 是否变化， 
            if (oldIc == Ic)
            {
                _iChange += 0x00;
            }
            else
            {
                _iChange += DATA3;
                oldIc = Ic;
            }

            //Ua 是否变化， 
            if (oldUa == Ua)
            {
                _iChange += 0x00;
            }
            else
            {
                _iChange += DATA2;
                oldUa = Ua;
            }
            //Ub 是否变化， 
            if (oldUb == Ub)
            {
                _iChange += 0x00;
            }
            else
            {
                _iChange += DATA1;
                oldUb = Ub;
            }
            //Uc 是否变化， 
            if (oldUc == Uc)
            {
                _iChange += 0x00;
            }
            else
            {
                _iChange += DATA0;
                oldUc = Uc;
            }

            _Ua = Ua;
            _Ub = Ub;
            _Uc = Uc;

            _Ia = Ia;
            _Ib = Ib;
            _Ic = Ic;

            _PhiIa = PhiIa;
            _PhiIb = PhiIb;
            _PhiIc = PhiIc;
            _PhiUa = PhiUa;
            _PhiUb = PhiUb;
            _PhiUc = PhiUc;
            _Hz = Hz;

            _bCurrentDB = bDBOut;
        }

        public void SetPara(float U, float I, string str_Glys, float Hz, int iClfs, byte Xwkz, bool XieBo, bool bDBOut,
            bool bHuanJ, bool bBHuan, CLDC_Comm.Enum.Cus_PowerYuanJiang HABC)
        {
            float m_UaValue = 0f;
            float m_UbValue = 0f;
            float m_UcValue = 0f;
            float m_IaValue = 0f;
            float m_IbValue = 0f;
            float m_IcValue = 0f;
           
            //r_Glys = "-1.0";            
            switch (HABC)
            {

                case CLDC_Comm.Enum.Cus_PowerYuanJiang.H:
                    m_UaValue = U;
                    m_UbValue = U;
                    m_UcValue = U;
                    m_IaValue = I;
                    m_IbValue = I;
                    m_IcValue = I;
                    break;
                case CLDC_Comm.Enum.Cus_PowerYuanJiang.A:
                    m_UaValue = U;
                    m_UbValue = U;
                    m_UcValue = U;
                    m_IaValue = I;
                    m_IbValue = 0;
                    m_IcValue = 0;
                    break;
                case CLDC_Comm.Enum.Cus_PowerYuanJiang.B:
                    m_UaValue = U;
                    m_UbValue = U;
                    m_UcValue = U;
                    m_IaValue = 0;
                    m_IbValue = I;
                    m_IcValue = 0;
                    break;
                case CLDC_Comm.Enum.Cus_PowerYuanJiang.C:
                    m_UaValue = U;
                    m_UbValue = U;
                    m_UcValue = U;
                    m_IaValue = 0;
                    m_IbValue = 0;
                    m_IcValue = I;
                    break;
            }
            //m_UaXwValue = 0;
            //m_UbXwValue = 240;
            //m_UcXwValue = 120;
            //m_IaXwValue = 0;
            //m_IbXwValue = 240;
            //m_IcXwValue = 120;
            //m_FreqValue = 50;
           
            // 三相四线有功 = 0,
            //三相四线无功 = 1,
            //三相三线有功 = 2,
            //三相三线无功 = 3,
            //二元件跨相90 = 4,
            //二元件跨相60 = 5,
            //三元件跨相90 = 6,

            switch (iClfs)
            {

                case 0:  //三相四线有功表
                    m_UaXwValue = 0;
                    m_UbXwValue = 240;
                    m_UcXwValue = 120;
                    m_IaXwValue = 0;
                    m_IbXwValue = 240;
                    m_IcXwValue = 120;
                    break;
                case 2:  //三相三线有功表
                    m_UbValue = 0;
                    m_UaXwValue = 30;
                    m_UbXwValue = 0;
                    m_UcXwValue = 90;
                    m_IaXwValue = 0;
                    m_IbXwValue = 0;
                    m_IcXwValue = 120;
                    break;
                case 1: //三相四线真无功表(QT4)
                    m_UaXwValue = 0;
                    m_UbXwValue = 240;
                    m_UcXwValue = 120;
                    m_IaXwValue = 270;
                    m_IbXwValue = 150;
                    m_IcXwValue = 30;
                    break;
                case 3: //三相三线真无功表(Q32)
                    m_UbValue = 0;
                    m_UaXwValue = 30;
                    m_UbXwValue = 0;
                    m_UcXwValue = 90;

                    m_IaXwValue = 270;
                    m_IbXwValue = 0;
                    m_IcXwValue = 30;
                    break;
                case 6: //三元件跨相90无功表(Q33)
                    m_UaXwValue = 30;
                    m_UbXwValue = 270;
                    m_UcXwValue = 150;

                    m_IaXwValue = 270;
                    m_IbXwValue = 150;
                    m_IcXwValue = 30;
                    break;
                case 4: //二元件跨相90无功表(Q90)
                    m_UbValue = 0;
                    m_UaXwValue = 30;
                    m_UbXwValue = 0;
                    m_UcXwValue = 270;

                    m_IaXwValue = 270;
                    m_IbXwValue = 0;
                    m_IcXwValue = 30;
                    break;
                case 5: //二元件跨相60无功表(Q60)
                    m_UbValue = 0;
                    m_UaXwValue = 0;
                    m_UbXwValue = 0;
                    m_UcXwValue = 120;

                    m_IaXwValue = 270;
                    m_IbXwValue = 0;
                    m_IcXwValue = 30;
                    break;
                case 7: //单相表
                    m_UbValue = 0;
                    m_UcValue = 0;
                    m_UaXwValue = 0;
                    m_IaXwValue = 0;
                    m_UbXwValue = m_UaXwValue;
                    m_UcXwValue = m_UaXwValue;
                    m_IbXwValue = m_IaXwValue;
                    m_IcXwValue = m_IaXwValue;
                    break;
            }


            SetAcSourcePowerFactor(str_Glys , (byte)iClfs, false);

            SetPara(m_UaValue, m_UbValue, m_UcValue, m_IaValue, m_IbValue, m_IcValue, (float)m_UaXwValue, (float)m_UbXwValue, (float)m_UcXwValue,
            (float)m_IaXwValue, (float)m_IbXwValue, (float)m_IcXwValue, Hz, iClfs, Xwkz, XieBo, bDBOut, bHuanJ, bBHuan);

        }
        /// <summary>
        /// 设置源功率因数
        /// </summary>
        /// <param name="Glys">功率因数如(0.5L,1.0,-1,-0,0.5C)</param>
        /// <param name="jxfs">0-三相四线有功表PT4;1-三相三线有功表P32;2--三相四线真无功表(QT4);3--三相三线真无功表(Q32);4--三元件跨相90无功表(Q33);5--二元件跨相90无功表(Q90);6--二元件人工中点(60)无功表(Q60)</param>
        /// <returns></returns>
        public bool SetAcSourcePowerFactor(string Glys, byte jxfs, bool PH)
        {
            //jxfs 0-三相四线有功表；1-三相三线有功表;2--三相四线真无功表(QT4);3--三相三线真无功表(Q32);
            //4--三元件跨相90无功表(Q33);5--二元件跨相90无功表(Q90);6--二元件人工中点(60)无功表(Q60);
            
            double XwUa = 0;
            double XwUb = 0;
            double XwUc = 0;
            double XwIa = 0;
            double XwIb = 0;
            double XwIc = 0;
            string strGlys = "";
            string LC = "";
            double LcValue = 0;
            double Phi = 0;
            int n = 1;
            strGlys = Glys;

            if(jxfs==0)// 三相四线有功 = 0,
            {jxfs=0;}
            else if(jxfs==1)//三相四线无功 = 1,
            {jxfs=2;}
             else if(jxfs==2)//三相三线有功 = 2,
            {jxfs=1;}
             else if(jxfs==3)//三相三线无功 = 3,
            {jxfs=3;}
             else if(jxfs==4)//二元件跨相90 = 4,
            {jxfs=5;}
             else if(jxfs==5)//二元件跨相60 = 5,
            {jxfs=6;}
             else if(jxfs==6)//三元件跨相90 = 6,
            {jxfs=4;}
             else if(jxfs==7)
            { jxfs = 7; }

            
            if (Glys == "0") strGlys = "0L";
            if (Glys == "-0") strGlys = "0C";
            LC = GetUnit(strGlys);
            if (LC.Length > 0)
            {
                LcValue = Convert.ToDouble(strGlys.Replace(LC, ""));
            }
            else
            {
                LcValue = Convert.ToDouble(strGlys);
            }

            switch (jxfs)
            {
                case 0:  //三相四线有功表
                    Phi = Math.Round(Math.Acos(LcValue) * (180 / Math.PI), 5);
                    XwUa = 0;
                    XwUb = 240;
                    XwUc = 120;
                    if (LcValue > 0)
                    {
                        XwIa = 0;
                        XwIb = 240;
                        XwIc = 120;
                        Phi = 1 * Phi;

                    }
                    else if (LcValue < 0)
                    {
                        XwIa = 180;
                        XwIb = 60;
                        XwIc = 300;
                        Phi = -1 * Phi;
                    }
                    if (LC == "L")
                    {
                        Phi = 1 * Phi;
                        XwIa = XwUa - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = XwUb - Phi;
                        if (XwIb < 0) XwIb = XwIb + 360;
                        if (XwIb >= 360) XwIb = XwIb - 360;

                        XwIc = XwUc - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;

                    }
                    if (LC == "C")
                    {
                        Phi = -1 * Phi;
                        XwIa = XwUa - Phi;

                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = XwUb - Phi;
                        if (XwIb < 0) XwIb = XwIb + 360;
                        if (XwIb >= 360) XwIb = XwIb - 360;

                        XwIc = XwUc - Phi;

                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    break;
                case 1:  //三相三线有功表
                    Phi = Math.Round(Math.Acos(LcValue) * (180 / Math.PI), 5);
                    XwUa = 30;
                    XwUb = 0;
                    XwUc = 90;
                    if (LcValue > 0)
                    {
                        XwIa = 0;
                        XwIb = 0;
                        XwIc = 120;
                        Phi = 1 * Phi;
                    }
                    else if (LcValue < 0)
                    {

                        XwIa = 180;
                        XwIb = 0;
                        XwIc = 240;
                        Phi = -1 * Phi;
                    }
                    if (LC == "L")
                    {
                        Phi = 1 * Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + (360);
                        if (XwIa >= 360) XwIa = XwIa - (360);
                        XwIb = 0;
                        XwIc = 120 - Phi;

                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    if (LC == "C")
                    {
                        Phi = -1 * Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 0;

                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    if (PH == false)
                    {
                        XwIa = XwIa + 30;
                        XwIc = XwIc + 30;
                    }
                    break;
                case 2: //三相四线真无功表(QT4)
                    XwUa = 0;
                    XwUb = 240;
                    XwUc = 120;
                    if (LcValue > 0)
                    {
                        XwIa = 270;
                        XwIb = 150;
                        XwIc = 30;
                        n = 1;
                    }
                    else if (LcValue < 0)
                    {

                        XwIa = 90;
                        XwIb = 330;
                        XwIc = 210;
                        n = -1;
                    }
                    if (LC == "L")
                    {
                        if (n == -1) Phi = (-180) - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        else Phi = Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = Phi;
                        XwIa = XwUa - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = XwUb - Phi;
                        if (XwIb < 0) XwIb = XwIb + 360;
                        if (XwIb >= 360) XwIb = XwIb - 360;

                        XwIc = XwUc - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    if (LC == "C")
                    {
                        if (n == -1) Phi = (-180) - (180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5));
                        else Phi = 180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = n + Phi;
                        XwIa = XwUa - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = XwUb - Phi;
                        if (XwIb < 0) XwIb = XwIb + 360;
                        if (XwIb >= 360) XwIb = XwIb - 360;

                        XwIc = XwUc - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    break;
                case 3: //三相三线真无功表(Q32)
                    XwUa = 30;
                    XwUb = 0;
                    XwUc = 90;

                    if (LcValue > 0)
                    {
                        XwIa = 270;
                        XwIb = 0;
                        XwIc = 30;
                        n = 1;
                    }
                    else if (LcValue < 0)
                    {

                        XwIa = 90;
                        XwIb = 0;
                        XwIc = 210;
                        n = -1;
                    }

                    if (LC == "L")
                    {
                        if (n == -1) Phi = (-180) - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        else Phi = Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 0;


                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    if (LC == "C")
                    {
                        if (n == -1) Phi = (-180) - (180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5));
                        else Phi = 180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = n + Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 0;


                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    if (PH == false)
                    {
                        XwIa = XwIa + 30;
                        XwIc = XwIc - 30;
                    }
                    break;
                case 4: //三元件跨相90无功表(Q33)
                    XwUa = 30;
                    XwUb = 270;
                    XwUc = 150;
                    if (LcValue > 0)
                    {
                        XwIa = 270;
                        XwIb = 150;
                        XwIc = 30;
                        n = 1;
                    }
                    else if (LcValue < 0)
                    {

                        XwIa = 90;
                        XwIb = 330;
                        XwIc = 210;
                        n = -1;
                    }

                    if (LC == "L")
                    {
                        if (n == -1) Phi = (-180) - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        else Phi = Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 240 - Phi;
                        if (XwIb < 0) XwIb = XwIb + 360;
                        if (XwIb >= 360) XwIb = XwIb - 360;

                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    if (LC == "C")
                    {
                        if (n == -1) Phi = (-180) - (180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5));
                        else Phi = 180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = n + Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 240 - Phi;
                        if (XwIb < 0) XwIb = XwIb + 360;
                        if (XwIb >= 360) XwIb = XwIb - 360;

                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    break;
                case 5: //二元件跨相90无功表(Q90)
                    XwUa = 30;
                    XwUb = 0;
                    XwUc = 270;

                    if (LcValue > 0)
                    {
                        XwIa = 270;
                        XwIb = 0;
                        XwIc = 30;
                        n = 1;
                    }
                    else if (LcValue < 0)
                    {

                        XwIa = 90;
                        XwIb = 0;
                        XwIc = 210;
                        n = -1;
                    }

                    if (LC == "L")
                    {
                        if (n == -1) Phi = (-180) - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        else Phi = Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 0;


                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    if (LC == "C")
                    {
                        if (n == -1) Phi = (-180) - (180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5));
                        else Phi = 180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = n + Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 0;


                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    break;
                case 6: //二元件跨相60无功表(Q60)
                    XwUa = 0;
                    XwUb = 0;
                    XwUc = 120;

                    if (LcValue > 0)
                    {
                        XwIa = 270;
                        XwIb = 0;
                        XwIc = 30;
                        n = 1;
                    }
                    else if (LcValue < 0)
                    {

                        XwIa = 90;
                        XwIb = 0;
                        XwIc = 210;
                        n = -1;
                    }
                    if (LC == "L")
                    {
                        if (n == -1) Phi = (-180) - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        else Phi = Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 0;


                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    if (LC == "C")
                    {
                        if (n == -1) Phi = (-180) - (180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5));
                        else Phi = 180 - Math.Round(Math.Asin(LcValue) * (180 / Math.PI), 5);
                        //Phi = n + Phi;
                        XwIa = 0 - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                        XwIb = 0;


                        XwIc = 120 - Phi;
                        if (XwIc < 0) XwIc = XwIc + 360;
                        if (XwIc >= 360) XwIc = XwIc - 360;
                    }
                    break;
                case 7: //单相表
                    Phi = Math.Round(Math.Acos(LcValue) * (180 / Math.PI), 5);
                    XwUa = 0;
                    if (LcValue > 0)
                    {
                        XwIa = 0;
                        Phi = 1 * Phi;

                    }
                    else if (LcValue < 0)
                    {
                        XwIa = 180;
                        Phi = -1 * Phi;
                    }
                    if (LC == "L")
                    {
                        Phi = 1 * Phi;
                        XwIa = XwUa - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;

                    }
                    if (LC == "C")
                    {
                        Phi = -1 * Phi;
                        XwIa = XwUa - Phi;
                        if (XwIa < 0) XwIa = XwIa + 360;
                        if (XwIa >= 360) XwIa = XwIa - 360;
                    }
                    XwUb = XwUa;
                    XwUc = XwUa;
                    XwIb = XwIa;
                    XwIc = XwIa;
                    break;
            }
            //tmpOk = SetAcSourcePhasic(XwUa, XwUb, XwUc, XwIa, XwIb, XwIc);
            m_UaXwValue = XwUa;
            m_UbXwValue = XwUb;
            m_UcXwValue = XwUc;

            m_IaXwValue = XwIa;
            m_IbXwValue = XwIb;
            m_IcXwValue = XwIc;

            return true ;
        }

        private string GetUnit(string chrVal)  //得到量程的单位 //chrVal带单位的值如 15A
        {
            int i = 0;
            string cUnit = "";
            byte[] chrbytes = new byte[256];
            ASCIIEncoding ascii = new ASCIIEncoding();
            chrbytes = ascii.GetBytes(chrVal);
            for (i = 0; i < chrbytes.Length; ++i)
            {
                if (chrbytes[i] > 57)
                {
                    cUnit = chrVal.Substring(i);
                    break;
                }

            }
            return cUnit;
        }
        /*
         * 81 30 PCID 09 a0 02 02 40 CS
         */
        protected override byte[] GetBody()
        {
            int tmpvalue = 0;
            ByteBuffer buf = new ByteBuffer();
            buf.Initialize();

            byte[] byt_Value = new byte[67];

            byt_Value[0] = PAGE5;

            byt_Value[1] = GROUP1 + GROUP2 + GROUP6;

            //GROUP1 
            byt_Value[2] = DATA0 + DATA1 + DATA2 + DATA3 + DATA4 + DATA5; //len = 8          

            tmpvalue = (int)(_PhiUc * JIAODU_BEISHU);//电压角度 C-B-A
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 3, 4);

            tmpvalue = (int)(_PhiUb * JIAODU_BEISHU);//
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 7, 4);

            tmpvalue = (int)(_PhiUa * JIAODU_BEISHU);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 11, 4);

            tmpvalue = (int)(_PhiIc * JIAODU_BEISHU);//电流角度 C-B-A
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 15, 4);

            tmpvalue = (int)(_PhiIb * JIAODU_BEISHU);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 19, 4);

            tmpvalue = (int)(_PhiIa * JIAODU_BEISHU);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 23, 4);



            //GROUP2
            byt_Value[27] = DATA0 + DATA1 + DATA2 + DATA3 + DATA4 + DATA5 + DATA6 + DATA7; //len = 33

            tmpvalue = (int)(_Uc * 10000);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 28, 4);
            byt_Value[32] = (byte)Convert.ToSByte(-4);

            tmpvalue = (int)(_Ub * 10000);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 33, 4);
            byt_Value[37] = (byte)Convert.ToSByte(-4);

            tmpvalue = (int)(_Ua * 10000);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 38, 4);
            byt_Value[42] = (byte)Convert.ToSByte(-4);

            tmpvalue = (int)(_Ic * 1000000);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 43, 4);
            byt_Value[47] = (byte)Convert.ToSByte(-6);

            tmpvalue = (int)(_Ib * 1000000);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 48, 4);
            byt_Value[52] = (byte)Convert.ToSByte(-6);

            tmpvalue = (int)(_Ia * 1000000);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 53, 4);
            byt_Value[57] = (byte)Convert.ToSByte(-6);

            //

            tmpvalue = (int)(_Hz * 10000);
            Array.Copy(BitConverter.GetBytes(tmpvalue), 0, byt_Value, 58, 4); ; //

            byt_Value[62] = DATA0 + DATA1 + DATA2; //

            byt_Value[63] = DATA0 + DATA1 + DATA2;//

            byt_Value[64] = DATA0 + DATA1 + DATA2 + DATA3 + DATA4 + DATA5 + DATA6;
            //byt_Value[64] = //
            byt_Value[65] = Convert.ToByte(_iChange);// DATA0 + DATA1 + DATA2 + DATA3 + DATA4 + DATA5;// +DATA6; //

            if (_bCurrentDB)
            {
                byt_Value[66] = 0;//电流对标
            }
            else
            {
                byt_Value[66] = 0x0;//不对标
            }
            buf.Put(0xA3);
            buf.Put(byt_Value);
            return buf.ToByteArray();
        }
    }
}
