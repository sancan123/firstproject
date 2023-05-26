using System;
using System.Collections.Generic;
using System.Text;
using CLDC_DataCore.Struct;
using CLDC_DataCore.Const;

namespace CLDC_DataCore.Struct
{
    /// <summary>
    /// Ӱ�����춨������Ŀ
    /// </summary>
    [Serializable()]
    public struct  StPlan_Influence
    {
        
        /// <summary>
        /// ��Ŀ����
        /// </summary>
        public string PrjName;
        /// <summary>
        /// ��Ŀ����
        /// 100��ѹӰ��+15%"
        /// 101��ѹӰ��+10%"
        /// 102��ѹӰ��-10%"
        /// 103��ѹӰ��-20%"
        /// 104Ƶ��Ӱ��+2%"
        /// 105Ƶ��Ӱ��-2%"
        /// 106������Ӱ��"        
        /// 107��ѹ��ƽ��Ӱ��"
        /// 108��Դ��ѹӰ��115%
        /// 109��Դ��ѹӰ��110%
        /// 110��Դ��ѹӰ��90%
        /// 111��Դ��ѹӰ��80%
        /// 112��Դ��ѹӰ��70%
        /// 113��Դ��ѹӰ��60%
        /// 114��Դ��ѹӰ��50%
        /// 115��Դ��ѹӰ��40%
        /// 116��Դ��ѹӰ��30%
        /// 117��Դ��ѹӰ��20%
        /// 118��Դ��ѹӰ��10%
        /// 119��Դ��ѹӰ��0%
        /// 120��ѹ������·�е�г������Ӱ��"      
        /// 121����������·�д�г������Ӱ��"
        /// 122����������·�����г������Ӱ��"
        /// 123����������·��ֱ����ż��г������Ӱ��"
        /// </summary>
        public string PrjType;

        /// <summary>
        /// ���ʷ���
        /// </summary>
        public CLDC_Comm.Enum.Cus_PowerFangXiang PowerFangXiang;

        /// <summary>
        /// �����������磺1.0 ��0.5C ��-0.8L
        /// </summary>
        public string PowerYinSu;

        /// <summary>
        /// ��Ŀ��ţ�Ψһ��
        /// </summary>
        public string PrjNo
        {
            get
            {
                string strType = "";
                if (PrjType == "��ѹӰ��+15%")
                    strType = "100";
                else if (PrjType == "��ѹӰ��+10%")
                    strType = "101";
                else if (PrjType == "��ѹӰ��-10%")
                    strType = "102";
                else if (PrjType == "��ѹӰ��-20%")
                    strType = "103";
                else if (PrjType == "Ƶ��Ӱ��+2%")
                    strType = "104";
                else if (PrjType == "Ƶ��Ӱ��-2%")
                    strType = "105";
                else if (PrjType == "������Ӱ��")
                    strType = "106";
                else if (PrjType == "��ѹ��ƽ��Ӱ��")
                    strType = "107";
                else if (PrjType == "��Դ��ѹӰ��115%")
                    strType = "108";
                else if (PrjType == "��Դ��ѹӰ��110%")
                    strType = "109";
                else if (PrjType == "��Դ��ѹӰ��90%")
                    strType = "110";
                else if (PrjType == "��Դ��ѹӰ��80%")
                    strType = "111";
                else if (PrjType == "��Դ��ѹӰ��70%")
                    strType = "112";
                else if (PrjType == "��Դ��ѹӰ��60%")
                    strType = "113";
                else if (PrjType == "��Դ��ѹӰ��50%")
                    strType = "114";
                else if (PrjType == "��Դ��ѹӰ��40%")
                    strType = "115";
                else if (PrjType == "��Դ��ѹӰ��30%")
                    strType = "116";
                else if (PrjType == "��Դ��ѹӰ��20%")
                    strType = "117";
                else if (PrjType == "��Դ��ѹӰ��10%")
                    strType = "118";
                else if (PrjType == "��Դ��ѹӰ��0%")
                    strType = "119";
                else if (PrjType == "��ѹ������·�е�г������Ӱ��")
                    strType = "120";
                else if (PrjType == "����������·�д�г������Ӱ��")
                    strType = "121";
                else if (PrjType == "����������·�����г������Ӱ��")
                    strType = "122";
                else if (PrjType == "����������·��ֱ����ż��г������Ӱ��")
                    strType = "123";

                string str = strType + ((int)PowerFangXiang).ToString();
                str += GlobalUnit.g_SystemConfig.GlysZiDian.getGlysID(PowerYinSu);
                str += ((int)(xUa * 100)).ToString("D3");
                str += ((int)(xUb * 100)).ToString("D3");
                str += ((int)(xUc * 100)).ToString("D3");
                str += GlobalUnit.g_SystemConfig.xIbDic.getxIbID(Current);                
                str += ((int)PingLv * 10).ToString("D3");
                str += XiangXu.ToString();
                str += XieBo.ToString();
                return str;
            }
        }
        /// <summary>
        /// ��ѹ����(A��)
        /// </summary>
        public float xUa;

        /// <summary>
        /// ��ѹ������B�ࣩ
        /// </summary>
        public float xUb;

        /// <summary>
        /// ��ѹ������C�ࣩ
        /// </summary>
        public float xUc;

        public string Current;        

        /// <summary>
        /// ��ѹA����λ����ʱ���ã�
        /// </summary>
        public float fUa;

        /// <summary>
        /// ��ѹB����λ����ʱ���ã�
        /// </summary>
        public float fUb;
        /// <summary>
        /// ��ѹC����λ����ʱ���ã�
        /// </summary>
        public float fUc;

        /// <summary>
        /// ����A����λ����ʱ���ã�
        /// </summary>
        public float fIa;

        /// <summary>
        /// ����B����λ����ʱ���ã�
        /// </summary>
        public float fIb;
        /// <summary>
        /// ����C����λ����ʱ���ã�
        /// </summary>
        public float fIc;

        /// <summary>
        /// Ƶ��
        /// </summary>
        public float PingLv ;

        /// <summary>
        /// �������
        /// </summary>
        public float WuChaXian_Shang ;

        /// <summary>
        /// �������
        /// </summary>
        public float WuChaXian_Xia ;


        /// <summary>
        /// �������
        /// </summary>
        public float BianChaXian_Shang;

        /// <summary>
        /// ��������
        /// </summary>
        public float BianChaXian_Xia;

        /// <summary>
        /// ������
        /// </summary>
        public int WcCheckNumic;
        /// <summary>
        /// ��ʾ
        /// </summary>
        public bool bPrompt;
        /// <summary>
        /// Ȧ��
        /// </summary>
        public int LapCount;
        /// <summary>
        /// ����0-������1-������
        /// </summary>
        public int XiangXu;
        /// <summary>
        /// г��0-���ӣ�1-��
        /// </summary>
        public int XieBo;

        /// <summary>
        /// �����г�����򿴵�������г������
        /// </summary>
        public string XieBoFa;
        /// <summary>
        /// г�������б�
        /// </summary>
        public List<StXieBo> XieBoItem;


        /// <summary>
        /// ������ѹ�����ַ�����xUa|xUb|xUc��
        /// </summary>
        /// <param name="Ustring"></param>
        public void ExplainUString(string Ustring)
        {
            string[] xU = Ustring.Split('|');
            if (xU.Length != 3)
            {
                return;
            }
            this.xUa = float.Parse(xU[0]);
            this.xUb = float.Parse(xU[1]);
            this.xUc = float.Parse(xU[2]);
        }

        
        /// <summary>
        /// ��λ��(Ua,Ub,Uc|Ia,Ib,Ic)����ʱ���ã�
        /// </summary>
        /// <param name="XwString"></param>
        public void ExplainXwString(string XwString)
        {
            string[] Xw = XwString.Split('|');
            if (Xw.Length != 2) return;
            string[] XwU = Xw[0].Split(',');
            if (XwU.Length != 3) return;
            this.fUa = float.Parse(XwU[0]);
            this.fUb = float.Parse(XwU[1]);
            this.fUc = float.Parse(XwU[2]);
            string[] XwI = Xw[1].Split('|');
            if (XwI.Length != 3) return;
            this.fIa = float.Parse(XwI[0]);
            this.fIb = float.Parse(XwI[1]);
            this.fIc = float.Parse(XwI[2]);

        }
        /// <summary>
        /// ��������ޣ�����|���ߣ�
        /// </summary>
        /// <param name="WcxString"></param>
        public void ExplainWcx(string WcxString)
        {
            string[] Wcx = WcxString.Split('|');
            if (Wcx.Length != 2) return;
            this.WuChaXian_Shang = float.Parse(Wcx[0].Replace("+", ""));
            this.WuChaXian_Xia = float.Parse(Wcx[1].Replace("+", ""));
        }

        /// <summary>
        /// ���������ޣ�����|���ߣ�
        /// </summary>
        /// <param name="WcxString"></param>
        public void ExplainBcx(string WcxString)
        {
            string[] Wcx = WcxString.Split('|');
            if (Wcx.Length != 2) return;
            this.BianChaXian_Shang = float.Parse(Wcx[0].Replace("+", ""));
            this.BianChaXian_Xia = float.Parse(Wcx[1].Replace("+", ""));
        }

        /// <summary>
        /// ��ϵ�ѹ�����ַ���
        /// </summary>
        /// <returns></returns>
        public string JoinxUString()
        {
            return string.Format("{0}|{1}|{2}", this.xUa.ToString(), this.xUb.ToString(), this.xUc.ToString());
        }
        
        /// <summary>
        /// ���������ַ���
        /// </summary>
        /// <returns></returns>
        public String JoinWcxString()
        {
            return string.Format("{0}|{1}", this.WuChaXian_Shang > 0 ? string.Format("+{0}", this.WuChaXian_Shang.ToString()) : this.WuChaXian_Shang.ToString()
                                          , this.WuChaXian_Xia > 0 ? string.Format("+{0}", this.WuChaXian_Xia.ToString()) : this.WuChaXian_Xia.ToString());
        }

        /// <summary>
        /// ��ϱ�����ַ���
        /// </summary>
        /// <returns></returns>
        public String JoinBcxString()
        {
            return string.Format("{0}|{1}", this.BianChaXian_Shang > 0 ? string.Format("+{0}", this.BianChaXian_Shang.ToString()) : this.BianChaXian_Shang.ToString()
                                          , this.BianChaXian_Xia > 0 ? string.Format("+{0}", this.BianChaXian_Xia.ToString()) : this.BianChaXian_Xia.ToString());
        }

        /// <summary>
        /// �����λ�ַ�������ʱ���ã�
        /// </summary>
        /// <returns></returns>
        public string JoinXwString()
        {
            return string.Format("{0},{1},{2}|{3},{4},{5}", 
                                    this.fUa.ToString(), 
                                    this.fUb.ToString(), 
                                    this.fUc.ToString(), 
                                    this.fIa.ToString(), 
                                    this.fIb.ToString(), 
                                    this.fIc.ToString());
        }




        /// <summary>
        /// ����춨����
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string _GlfxString;
            switch (PowerFangXiang)
            {
                case CLDC_Comm.Enum.Cus_PowerFangXiang.�����й�:
                    {
                        _GlfxString = "P+";
                        break;
                    }
                case CLDC_Comm.Enum.Cus_PowerFangXiang.�����й�:
                    {
                        _GlfxString = "P-";
                        break;
                    }
                case CLDC_Comm.Enum.Cus_PowerFangXiang.�����޹�:
                    {
                        _GlfxString = "Q+";
                        break;
                    }
                case CLDC_Comm.Enum.Cus_PowerFangXiang.�����޹�:
                    {
                        _GlfxString = "Q-";
                        break;
                    }
                case CLDC_Comm.Enum.Cus_PowerFangXiang.��һ�����޹�:
                    {
                        _GlfxString = "Q1";
                        break;
                    }
                case CLDC_Comm.Enum.Cus_PowerFangXiang.�ڶ������޹�:
                    {
                        _GlfxString = "Q2";
                        break;
                    }
                case CLDC_Comm.Enum.Cus_PowerFangXiang.���������޹�:
                    {
                        _GlfxString = "Q3";
                        break;
                    }
                case CLDC_Comm.Enum.Cus_PowerFangXiang.���������޹�:
                    {
                        _GlfxString = "Q4";
                        break;
                    }
                default:
                    {
                        _GlfxString = "P+";
                        break;
                    }
            }
            

            return string.Format("{0} {1},{2},{3},{4:f}Hz"
                                , PrjType
                                , _GlfxString
                                , PowerYinSu
                                , Current
                                , PingLv                                
                                );
        }

        // / <summary>
        /// ��ȡ��ǰ�����������
        /// </summary>
        /// <param name="WcLimitName">���������</param>
        /// <param name="GuiChengName">�������</param>
        /// <param name="Dj">�ȼ�1.0(2.0)</param>
        /// <param name="Hgq">�Ƿ񾭻���������</param>
        public void SetWcx(string Dj)
        {
            string _Wcx = "";

            _Wcx = CLDC_DataCore.DataBase.clsWcLimitDataControl.getLimit_Effect(PrjType
                                                        , Dj                                                        
                                                        , this.PowerYinSu);
            this.SetWcx(float.Parse(_Wcx), float.Parse(string.Format("-{0}", _Wcx)));       //���������

        }

        /// <summary>
        /// ���������
        /// </summary>
        /// <param name="Max">����</param>
        /// <param name="Min">����</param>
        internal void SetWcx(float Max, float Min)
        {            
            this.WuChaXian_Shang = Max;
        
            this.WuChaXian_Xia = Min;
        }

    }


}
