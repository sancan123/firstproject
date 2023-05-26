using System;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;
using CLDC_Comm.Enum;

namespace CLDC_DataCore.Function
{
    /// <summary>
    /// 
    /// </summary>
    public class Common
    {
        /// <summary>
        /// ��Bool�ν��ת��Ϊ�ϸ�/���ϸ�
        /// </summary>
        /// <param name="Result"></param>
        /// <returns></returns>
        public static string ConverResult(bool Result)
        {
            return Result == true
                ? "�ϸ�"
                : "���ϸ�";
        }

        /// <summary>
        /// ���һ���ַ����Ƿ�Ϊ��
        /// </summary>
        /// <param name="str">Ҫ�����ַ���</param>
        /// <returns>bool</returns>
        public static bool IsEmpty(string str)
        {
            str = str.Trim();
            if (str.Length == 0 || str == "" || str == null || str == string.Empty)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// ��ʼ����������
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value"></param>
        public static void Memset(ref string[] array, string value)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = value;
        }
        /// <summary>
        /// ��ʼ����������
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value"></param>
        public static void Memset(ref bool[] array, bool value)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = value;
        }
        /// <summary>
        /// ��ʼ����������
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value"></param>
        public static void Memset(ref int[] array, int value)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = value;
        }

        /// <summary>
        /// ��ʼ������������
        /// </summary>
        /// <param name="array"></param>
        /// <param name="value"></param>
        public static void Memset(ref float[] array, float value)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = value;

        }
        /// <summary>
        /// ð������ʱ�α� ʱ��С����ǰ
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string SortMin(string array)
        {
            if (string.IsNullOrEmpty(array)) return "";
            string strRev = "";

            try
            {
                string[] arr = new string[array.Length / 6];

                for (int i = 0; i < array.Length / 6; i++)
                {
                    arr[i] = array.Substring(i * 6, 6);
                }

                for (int i = 0; i < arr.Length; i++)
                {
                    for (int j = i; j < arr.Length; j++)
                    {
                        if (int.Parse(arr[i].Substring(0, 4)) > int.Parse(arr[j].Substring(0, 4)))
                        {
                            string temp = arr[i];
                            arr[i] = arr[j];
                            arr[j] = temp;
                        }
                    }
                }

                for (int i = 0; i < arr.Length; i++)
                {
                    strRev += arr[i];
                }
            }
            catch (Exception)
            {
                strRev = array;
            }
            return strRev;
        }

        /// <summary>
        /// ð������ʱ�α� ʱ������ǰ
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static string SortMax(string array)
        {
            if (string.IsNullOrEmpty(array)) return "";
            string strRev = "";
            try
            {
                string[] arr = new string[array.Length / 6];

                for (int i = 0; i < array.Length / 6; i++)
                {
                    arr[i] = array.Substring(i * 6, 6);
                }

                for (int i = 0; i < arr.Length; i++)
                {
                    for (int j = i; j < arr.Length; j++)
                    {
                        if (int.Parse(arr[i].Substring(0, 4)) < int.Parse(arr[j].Substring(0, 4)))
                        {
                            string temp = arr[i];
                            arr[i] = arr[j];
                            arr[j] = temp;
                        }
                    }
                }

                for (int i = 0; i < arr.Length; i++)
                {
                    strRev += arr[i];
                }
            }
            catch (Exception)
            {
                strRev = array;
            }
            return strRev;
        }

        /// <summary>
        /// ���ʵ������
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string[] ConverFl(string[] value)
        {
            string[] ArrTmp = new string[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                ArrTmp[i] = ConverFl(value[i]);
            }
            return ArrTmp;
        }

        /// <summary>
        /// ���ʵ������
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConverFl(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length % 8 != 0) return value;
            string strRevData = "";

            for (int i = value.Length / 8 - 1; i > -1; i--)
            {
                strRevData += value.Substring(i * 8, 8);
            }
            return strRevData;
        }

        /// <summary>
        /// ���ݱ�����
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string[] ConverJtb(string[] value)
        {
            string[] ArrTmp = new string[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                ArrTmp[i] = ConverJtb(value[i]);
            }
            return ArrTmp;
        }

        /// <summary>
        /// ���ݱ�����
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConverJtb(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length != 140) return value;
            string strRevData = "";
            //����ֵ1-6
            strRevData = value.Substring(132, 8) + value.Substring(124, 8) + value.Substring(116, 8) + value.Substring(108, 8) + value.Substring(100, 8) + value.Substring(92, 8);
            //���ݵ��1-7
            strRevData += value.Substring(84, 8) + value.Substring(76, 8) + value.Substring(68, 8) + value.Substring(60, 8) + value.Substring(52, 8) + value.Substring(44, 8) + value.Substring(36, 8);
            //��1-6������
            strRevData += value.Substring(30, 6) + value.Substring(24, 6) + value.Substring(18, 6) + value.Substring(12, 6) + value.Substring(6, 6) + value.Substring(0, 6);

            return strRevData;
        }

        /// <summary>
        /// ���鷭ת
        /// </summary>
        /// <param name="dData"></param>
        /// <returns></returns>
        public static string StringReserve(string strData)
        {
            if (string.IsNullOrEmpty(strData)) return "";

            string tdata = "";

            for (int i = strData.Length / 2 - 1; i >= 0; i--)
            {
                tdata += strData.Substring(i * 2, 2);
            }
            return tdata;
        }



        public static Single[] GetPhiGlys(Cus_Clfs Clfs, int Glfx, Cus_PowerYuanJian Glyj, string strGlys, Cus_PowerPhase bln_Nxx)
        {
            strGlys = strGlys.Replace("-","");
            Single sngAngle;
            Single[] sng_Phi = new Single[7];

            int intFX;
            int intClfs;
            int m_intClfs;
            Single sngPhiTmp;


            intClfs = (int)Clfs;
            m_intClfs = intClfs;
            intFX = (int)Glfx;



            if (intClfs == 0)
            {
                if (intFX == 1 || intFX == 2)
                    intClfs = 0;
                else
                    intClfs = 1;
            }
            else if (intClfs == 1)
            {
                if (intFX == 1 || intFX == 2)
                    intClfs = 2;
                else
                    intClfs = 3;
            }
            else if (intClfs == 5)
            {
                if (intFX == 1 || intFX == 2)
                    intClfs = 0;
                else
                    intClfs = 1;
            }


            //��ѹ������λ
            sngAngle = GetGlysAngle(intClfs, strGlys);
            sngAngle = (int)sngAngle;
            sng_Phi[6] = sngAngle;
            if (m_intClfs == 0)
            {
                //----------------�������߽Ƕ�------------------------
                sng_Phi[0] = 0;           //Ua
                sng_Phi[1] = 240;         //Ub
                sng_Phi[2] = 120;         //Uc
                sng_Phi[3] = sng_Phi[0] - sngAngle;
                sng_Phi[4] = sng_Phi[1] - sngAngle;
                sng_Phi[5] = sng_Phi[2] - sngAngle;

                if (sng_Phi[3] > 360) sng_Phi[3] = sng_Phi[3] - 360;
                if (sng_Phi[3] < 0) sng_Phi[3] = sng_Phi[3] + 360;
                if (sng_Phi[4] > 360) sng_Phi[4] = sng_Phi[4] - 360;
                if (sng_Phi[4] < 0) sng_Phi[4] = sng_Phi[4] + 360;
                if (sng_Phi[5] > 360) sng_Phi[5] = sng_Phi[5] - 360;
                if (sng_Phi[5] < 0) sng_Phi[5] = sng_Phi[5] + 360;



                //����Ƿ���Ҫ�������Ƕȷ�����
                if (strGlys.IndexOf('-') == -1)
                {
                    if (intFX == 2 || intFX == 4)
                    {
                        sng_Phi[3] = sng_Phi[3] + 180;
                        sng_Phi[4] = sng_Phi[4] + 180;
                        sng_Phi[5] = sng_Phi[5] + 180;
                        sng_Phi[6] = sng_Phi[6] + 180;

                        if (sng_Phi[3] > 360) sng_Phi[3] = sng_Phi[3] - 360;
                        if (sng_Phi[4] > 360) sng_Phi[4] = sng_Phi[4] - 360;
                        if (sng_Phi[5] > 360) sng_Phi[5] = sng_Phi[5] - 360;
                        if (sng_Phi[6] > 360) sng_Phi[6] = sng_Phi[6] - 360;
                    }
                }

              

            }
            else if (m_intClfs == 1)
            {
                //---------------�������߽Ƕ�--------------------

                sng_Phi[0] = 0;           //Ua
                sng_Phi[2] = 120;         //Uc
                sng_Phi[3] = sng_Phi[0] - sngAngle;
                sng_Phi[5] = sng_Phi[2] - sngAngle;
                sng_Phi[0] = 30;
                sng_Phi[2] = 90;

                if (Glyj != Cus_PowerYuanJian.H)
                {
                    sng_Phi[3] = sng_Phi[0] - sngAngle;
                    sng_Phi[5] = sng_Phi[2] - sngAngle;
                }

                if (sng_Phi[3] > 360) sng_Phi[3] = sng_Phi[3] - 360;
                if (sng_Phi[3] < 0) sng_Phi[3] = sng_Phi[3] + 360;
                if (sng_Phi[5] > 360) sng_Phi[5] = sng_Phi[5] - 360;
                if (sng_Phi[5] < 0) sng_Phi[5] = sng_Phi[5] + 360;
                if (sng_Phi[6] > 360) sng_Phi[6] = sng_Phi[6] - 360;
                if (sng_Phi[6] < 0) sng_Phi[6] = sng_Phi[6] + 360;


                //����Ƿ���Ҫ�������Ƕȷ�����

                if (intFX == 2 || intFX == 4)
                {
                    sng_Phi[3] = sng_Phi[3] + 180;
                    sng_Phi[5] = sng_Phi[5] + 180;
                    sng_Phi[6] = sng_Phi[6] + 180;

                    if (sng_Phi[3] > 360) sng_Phi[3] = sng_Phi[3] - 360;
                    if (sng_Phi[5] > 360) sng_Phi[5] = sng_Phi[5] - 360;
                    if (sng_Phi[6] > 360) sng_Phi[6] = sng_Phi[6] - 360;
                }

            }
            else if (m_intClfs == 2)
            {
                //-----------��Ԫ������90���޹���Ƕ�------------------
                sng_Phi[0] = 0;           //Ua
                sng_Phi[1] = 240;         //Ub
                sng_Phi[2] = 120;         //Uc
                sng_Phi[3] = sng_Phi[0] - sngAngle;
                sng_Phi[4] = sng_Phi[1] - sngAngle;
                sng_Phi[5] = sng_Phi[2] - sngAngle;

                //sngPhiUab = 30;          //Uab
                //sngPhiUbc = 270;         //Ubc
                //sngPhiUca = 150;         //Uca

                if (sng_Phi[3] > 360) sng_Phi[3] = sng_Phi[3] - 360;
                if (sng_Phi[3] < 0) sng_Phi[3] = sng_Phi[3] + 360;
                if (sng_Phi[4] > 360) sng_Phi[4] = sng_Phi[4] - 360;
                if (sng_Phi[4] < 0) sng_Phi[4] = sng_Phi[4] + 360;
                if (sng_Phi[5] > 360) sng_Phi[5] = sng_Phi[5] - 360;
                if (sng_Phi[5] < 0) sng_Phi[5] = sng_Phi[5] + 360;
                if (sng_Phi[6] > 360) sng_Phi[6] = sng_Phi[6] - 360;
                if (sng_Phi[6] < 0) sng_Phi[6] = sng_Phi[6] + 360;
            }
            else
            {
                //-----------�����------------------
                sng_Phi[0] = 0;         //Ua
                sng_Phi[3] = sng_Phi[0] - sngAngle;

                if (sng_Phi[3] > 360) sng_Phi[3] = sng_Phi[3] - 360;
                if (sng_Phi[3] < 0) sng_Phi[3] = sng_Phi[3] + 360;

                //����Ƿ���Ҫ�������Ƕȷ�����                
                if (intFX == 2 || intFX == 4)
                {
                    sng_Phi[3] = sng_Phi[3] + 180;
                    sng_Phi[6] = sng_Phi[6] + 180;
                    if (sng_Phi[3] > 360) sng_Phi[3] = sng_Phi[3] - 360;
                    if (sng_Phi[6] > 360) sng_Phi[6] = sng_Phi[6] - 360;
                }

            }
            if (bln_Nxx == Cus_PowerPhase.����������)
            {
                sngPhiTmp = sng_Phi[0];
                sng_Phi[0] = sng_Phi[1];
                sng_Phi[1] = sngPhiTmp;
            }
            else if (bln_Nxx == Cus_PowerPhase.��ѹ������)
            {
                sngPhiTmp = sng_Phi[3];
                sng_Phi[3] = sng_Phi[4];
                sng_Phi[4] = sngPhiTmp;
            }
            else if (bln_Nxx == Cus_PowerPhase.������)
            {
                if (m_intClfs == 1)
                {
                    sng_Phi[1] = 0;
                    sng_Phi[2] = 330;
                    sngPhiTmp = sng_Phi[4];
                    sng_Phi[4] = sng_Phi[5];
                    sng_Phi[5] = sngPhiTmp;

                }
                else
                {
                    sngPhiTmp = sng_Phi[0];
                    sng_Phi[0] = sng_Phi[1];
                    sng_Phi[1] = sngPhiTmp;
                    sngPhiTmp = sng_Phi[3];
                    sng_Phi[3] = sng_Phi[4];
                    sng_Phi[4] = sngPhiTmp;
                }
            }
            return sng_Phi;
        }


        private static Single GetGlysAngle(int intClfs, string strGlys)
        {
            Double dbl_Pha = 0;
            String sLC;
            Double dbl_Xs;
            Single PI = 3.14159f;
            strGlys = strGlys.Trim();
            sLC = strGlys.Substring(strGlys.Length - 1, 1);
            if (sLC.ToUpper() == "C" || sLC.ToUpper() == "L")
                dbl_Xs = Double.Parse(strGlys.Substring(0, strGlys.Length - 1));
            else
                dbl_Xs = double.Parse(strGlys);


            if (intClfs == 0 || intClfs == 2)      //�й�
            {
                if (dbl_Xs > 0 && dbl_Xs <= 1)
                    dbl_Pha = Math.Atan(Math.Sqrt(1 - dbl_Xs * dbl_Xs) / dbl_Xs);
                else if (dbl_Xs < 0 && dbl_Xs >= -1)
                    dbl_Pha = Math.Atan(Math.Sqrt(1 - dbl_Xs * dbl_Xs) / dbl_Xs) + PI;
                else if (dbl_Xs == 0)
                    dbl_Pha = PI / 2;
            }
            else
            {
                if (dbl_Xs > -1 && dbl_Xs < 1)
                    dbl_Pha = Math.Atan(dbl_Xs / Math.Sqrt(1 - dbl_Xs * dbl_Xs));
                else if (dbl_Xs == -1)
                    dbl_Pha = -PI * 0.5f;
                else if (dbl_Xs == 1)
                    dbl_Pha = PI * 0.5f;
            }
            dbl_Pha = dbl_Pha * 180 / PI;


            if (intClfs == 2 && sLC.ToUpper() == "C")
                dbl_Pha = 360 - dbl_Pha;
            else if ((intClfs == 1 || intClfs == 3) && sLC.ToUpper() == "C")
                dbl_Pha = 360 - dbl_Pha - 180;
            else if (sLC.ToUpper() == "C")
                dbl_Pha = 360 - dbl_Pha;


            if (dbl_Pha < 0) dbl_Pha = 360 + dbl_Pha;
            if (dbl_Pha >= 360) dbl_Pha = dbl_Pha - (dbl_Pha / 360) * 360;
            dbl_Pha = Math.Round(dbl_Pha, 4);
            return (Single)dbl_Pha;
        }





        /// <summary>
        /// ����Ƕ� �������
        /// </summary>
        /// <param name="int_Clfs">������ʽ</param>
        /// <param name="str_Glys">��������</param>
        /// <param name="int_Element">������</param>
        /// <param name="bln_NXX">������</param>
        /// <returns>�������飬����Ԫ��Ϊ����ABC���ѹ�����Ƕ�</returns>
        public static Single[] GetPhiGlys2(int int_Clfs, string str_Glys, int int_Element, bool bln_NXX)
        {
            string str_CL = str_Glys.ToUpper().Substring(str_Glys.Length - 1, 1);
            Double dbl_XS = 0;
            if (str_CL == "C" || str_CL == "L")
                dbl_XS = Convert.ToDouble(str_Glys.Substring(0, str_Glys.Length - 1));
            else
                dbl_XS = Convert.ToDouble(str_Glys);
            Double dbl_Phase;

            if (int_Clfs == 1 || int_Clfs == 3 || int_Clfs == 6)
                dbl_Phase = Math.Asin(Math.Abs(dbl_XS));                              //�޹�����
            else
                dbl_Phase = Math.Acos(Math.Abs(dbl_XS));                              //�й�����

            dbl_Phase = dbl_Phase * 180 / Math.PI;      //�ǶȻ���
            if (dbl_XS < 0) dbl_Phase = 180 + dbl_Phase;         //����
            if (str_CL == "C") dbl_Phase = 360 - dbl_Phase;
            if (dbl_Phase < 0) dbl_Phase = 360 + dbl_Phase;

            Single sng_UIPhi = Convert.ToSingle(dbl_Phase);
            Single[] sng_Phi = new Single[6];

            if (bln_NXX)
            {
                sng_Phi[0] = 0;         //Ua
                sng_Phi[1] = 240;       //Ub
                sng_Phi[2] = 120;       //Uc
            }
            else
            {
                sng_Phi[0] = 0;         //Ua
                sng_Phi[1] = 120;       //Ub
                sng_Phi[2] = 240;       //Uc
            }


            sng_Phi[3] = sng_Phi[0] + sng_UIPhi;       //Ia
            sng_Phi[4] = sng_Phi[1] + sng_UIPhi;       //Ib
            sng_Phi[5] = sng_Phi[2] + sng_UIPhi;       //Ic

            if (int_Clfs == 2 || int_Clfs == 3)
            {
                sng_Phi[2] += 60;       //Uc
                sng_Phi[3] += 30;       //Ia
                sng_Phi[4] += 30;       //Ib
                sng_Phi[5] += 30;       //Ic
            }

            sng_Phi[3] %= 360;       //Ia
            sng_Phi[4] %= 360;       //Ib
            sng_Phi[5] %= 360;



            //0, 240, 120, 0, 240, 120
            //0, 240, 120, 180, 60, 300
            //0, 240, 120, 30, 270, 150
            //0, 240, 120, 210, 90, 330,

            return sng_Phi;
        }


        /// <summary>
        /// ͳ�Ƴ�������ΪTrue�ĸ���,���С�ڴ������ĸ�������Ϊtrue
        /// </summary>
        /// <param name="bResult"></param>
        /// <param name="iCount"></param>
        /// <returns></returns>
        public static bool GetResultCount(bool[] bResult, int iCount)
        {

            int Count = 0;
            for (int i = 0; i < bResult.Length; i++)
            {
                if (bResult[i])
                {
                    Count++;
                }
            }
            if (Count <= iCount)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 16����ת10���� �������ݻس��Ľ��ת��   ���磺000C350ת����500
        /// </summary>
        /// <param name="strHex">16�����ַ���</param>
        /// <returns></returns>
        public static string[] HexConverToDecimalism(string[] strHex)
        {
            string[] strDecimalism = new string[strHex.Length];
            for (int i = 0; i < strHex.Length; i++)
            {
                if (!string.IsNullOrEmpty(strHex[i]))
                {
                    strDecimalism[i] = string.Format((Convert.ToInt32(strHex[i], 16) / 100).ToString());
                }
                else
                {
                    strDecimalism[i] = "";
                }
            }
            return strDecimalism;
        }

        /// <summary>
        /// 16����ת10���� �������ݻس��Ľ��ת��   ���磺000C350ת����500
        /// </summary>
        /// <param name="strHex">16�����ַ���</param>
        /// <returns></returns>
        public static string HexConverToDecimalism(string strHex)
        {
            string strDecimalism = "";

            if (!string.IsNullOrEmpty(strHex))
            {
                strDecimalism = string.Format((Convert.ToInt32(strHex, 16) / 100).ToString());
            }
            return strDecimalism;
        }
        /// <summary>
        /// 16����ת2���� �������ݻس��Ľ��ת��   ���磺15ת����10101
        /// </summary>
        /// <param name="strHex">16�����ַ���</param>
        /// <returns></returns>
        public static string HexStrToBinStr(string strHex)
        {
            string strResult = string.Empty;

            foreach (char c in strHex)
            {
                int v = Convert.ToInt32(c.ToString(), 16);
                int v2 = int.Parse(Convert.ToString(v, 2));
                strResult += string.Format("{0:d4}", v2);
            }
            return strResult;
        }
        /// <summary>
        /// �ַ���ת10����  ������ͨ��ȡ�Ľ��ת��   ���磺00050000ת����500
        /// </summary>
        /// <param name="strHex"></param>
        /// <returns></returns>
        public static string[] StringConverToDecima(string[] strData)
        {
            string[] strDecimalism = new string[strData.Length];
            for (int i = 0; i < strData.Length; i++)
            {
                if (!string.IsNullOrEmpty(strData[i]))
                {
                    strDecimalism[i] = Math.Round((Convert.ToDecimal(strData[i]) / 100), 2).ToString();
                }
                else
                {
                    strDecimalism[i] = "";
                }
            }
            return strDecimalism;
        }

        /// <summary>
        /// �ַ���ת10��������        ���磺0048ת����48
        /// </summary>
        /// <param name="strHex">�ַ���</param>
        /// <returns></returns>
        public static string[] StringConverToIntger(string[] strData)
        {
            string[] strDecimalism = new string[strData.Length];
            for (int i = 0; i < strData.Length; i++)
            {
                if (!string.IsNullOrEmpty(strData[i]))
                {
                    strDecimalism[i] = (Convert.ToInt32(strData[i])).ToString();
                }
                else
                {
                    strDecimalism[i] = "";
                }
            }
            return strDecimalism;
        }

        /// <summary>
        /// �ַ���ת10����  ������ͨ��ȡ�ĵ���ת��   ���磺000799ת����0.799
        /// </summary>
        /// <param name="strHex"></param>
        /// <returns></returns>
        public static string[] StringConverToDecimaByIb(string[] strData)
        {
            string[] strDecimalism = new string[strData.Length];
            for (int i = 0; i < strData.Length; i++)
            {
                if (!string.IsNullOrEmpty(strData[i]))
                {
                    strDecimalism[i] = Math.Round((Convert.ToDecimal(strData[i]) / 1000), 3).ToString();
                }
                else
                {
                    strDecimalism[i] = "";
                }
            }
            return strDecimalism;
        }

        /// <summary>
        /// �ַ���ת10����  ������ͨ��ȡ�ĵ���ת��   
        /// </summary>
        /// <param name="strData"></param>
        /// <param name="dot"></param>
        /// <returns></returns>
        public static string[] StringConverToDecimaByIb(string[] strData, int dot)
        {
            string[] strDecimalism = new string[strData.Length];
            for (int i = 0; i < strData.Length; i++)
            {
                if (!string.IsNullOrEmpty(strData[i]))
                {
                    strDecimalism[i] = (Convert.ToSingle(strData[i]) / Convert.ToSingle(Math.Pow(10, dot))).ToString();
                }
                else
                {
                    strDecimalism[i] = "";
                }
            }
            return strDecimalism;
        }

        /// <summary>
        /// �ַ���ת10����  ������ͨ��ȡ��˲ʱ����ת��   ���磺000799ת����0.0799
        /// </summary>
        /// <param name="strHex"></param>
        /// <returns></returns>
        public static string[] StringConverToDecimaByGL(string[] strData)
        {
            string[] strDecimalism = new string[strData.Length];
            for (int i = 0; i < strData.Length; i++)
            {
                if (!string.IsNullOrEmpty(strData[i]))
                {

                    string strTmp = (Int32.Parse(strData[i].Substring(0, 2), System.Globalization.NumberStyles.HexNumber) + 51).ToString("x2").ToUpper();
                    string strTmp1 = strTmp.Substring(0, 1);
                    if (strTmp1 == "A" || strTmp1 == "B" || strTmp1 == "C" || strTmp1 == "D" || strTmp1 == "E" || strTmp1 == "F")
                    {
                        strDecimalism[i] = "-" + Math.Round((Convert.ToDecimal(strData[i].Substring(1, 5)) / 10000), 4).ToString();
                    }
                    else
                    {
                        strDecimalism[i] = Math.Round((Convert.ToDecimal(strData[i]) / 10000), 4).ToString();
                    }
                }
                else
                {
                    strDecimalism[i] = "";
                }
            }
            return strDecimalism;
        }
        /// <summary>
        /// ��ȡָ�����ݵ�С��λ��
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int GetPrecision(float number)
        {
            return GetPrecision(number.ToString());
        }
        /// <summary>
        /// ��ȡָ�����ݵ�С��λ��
        /// </summary>
        /// <param name="strNumber">�����ַ���</param>
        /// <returns></returns>
        public static int GetPrecision(string strNumber)
        {
            if (!Function.Number.IsNumeric(strNumber))
            {
                return 0;
            }
            int hzPrecision = strNumber.ToString().LastIndexOf('.');
            if (hzPrecision == -1)
            {
                //û��С���㣬����0
                hzPrecision = 0;
            }
            else
            {
                //��С����
                hzPrecision = strNumber.ToString().Length - hzPrecision - 1;
            }
            return hzPrecision;

        }
    }

}
