using System;
using System.Collections.Generic;
using System.Text;
using CLDC_DataCore.Struct;
using CLDC_DataCore.Const;

namespace CLDC_DataCore.Struct
{
    /// <summary>
    /// 影响量检定方案项目
    /// </summary>
    [Serializable()]
    public struct  StPlan_Influence
    {
        
        /// <summary>
        /// 项目描述
        /// </summary>
        public string PrjName;
        /// <summary>
        /// 项目类型
        /// 100电压影响+15%"
        /// 101电压影响+10%"
        /// 102电压影响-10%"
        /// 103电压影响-20%"
        /// 104频率影响+2%"
        /// 105频率影响-2%"
        /// 106逆相序影响"        
        /// 107电压不平衡影响"
        /// 108电源电压影响115%
        /// 109电源电压影响110%
        /// 110电源电压影响90%
        /// 111电源电压影响80%
        /// 112电源电压影响70%
        /// 113电源电压影响60%
        /// 114电源电压影响50%
        /// 115电源电压影响40%
        /// 116电源电压影响30%
        /// 117电源电压影响20%
        /// 118电源电压影响10%
        /// 119电源电压影响0%
        /// 120电压电流线路中的谐波分量影响"      
        /// 121交流电流线路中次谐波分量影响"
        /// 122交流电流线路中奇次谐波分量影响"
        /// 123交流电流线路中直流和偶次谐波分量影响"
        /// </summary>
        public string PrjType;

        /// <summary>
        /// 功率方向
        /// </summary>
        public CLDC_Comm.Enum.Cus_PowerFangXiang PowerFangXiang;

        /// <summary>
        /// 功率因数，如：1.0 、0.5C 、-0.8L
        /// </summary>
        public string PowerYinSu;

        /// <summary>
        /// 项目编号（唯一）
        /// </summary>
        public string PrjNo
        {
            get
            {
                string strType = "";
                if (PrjType == "电压影响+15%")
                    strType = "100";
                else if (PrjType == "电压影响+10%")
                    strType = "101";
                else if (PrjType == "电压影响-10%")
                    strType = "102";
                else if (PrjType == "电压影响-20%")
                    strType = "103";
                else if (PrjType == "频率影响+2%")
                    strType = "104";
                else if (PrjType == "频率影响-2%")
                    strType = "105";
                else if (PrjType == "逆相序影响")
                    strType = "106";
                else if (PrjType == "电压不平衡影响")
                    strType = "107";
                else if (PrjType == "电源电压影响115%")
                    strType = "108";
                else if (PrjType == "电源电压影响110%")
                    strType = "109";
                else if (PrjType == "电源电压影响90%")
                    strType = "110";
                else if (PrjType == "电源电压影响80%")
                    strType = "111";
                else if (PrjType == "电源电压影响70%")
                    strType = "112";
                else if (PrjType == "电源电压影响60%")
                    strType = "113";
                else if (PrjType == "电源电压影响50%")
                    strType = "114";
                else if (PrjType == "电源电压影响40%")
                    strType = "115";
                else if (PrjType == "电源电压影响30%")
                    strType = "116";
                else if (PrjType == "电源电压影响20%")
                    strType = "117";
                else if (PrjType == "电源电压影响10%")
                    strType = "118";
                else if (PrjType == "电源电压影响0%")
                    strType = "119";
                else if (PrjType == "电压电流线路中的谐波分量影响")
                    strType = "120";
                else if (PrjType == "交流电流线路中次谐波分量影响")
                    strType = "121";
                else if (PrjType == "交流电流线路中奇次谐波分量影响")
                    strType = "122";
                else if (PrjType == "交流电流线路中直流和偶次谐波分量影响")
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
        /// 电压倍数(A相)
        /// </summary>
        public float xUa;

        /// <summary>
        /// 电压倍数（B相）
        /// </summary>
        public float xUb;

        /// <summary>
        /// 电压倍数（C相）
        /// </summary>
        public float xUc;

        public string Current;        

        /// <summary>
        /// 电压A相相位（暂时无用）
        /// </summary>
        public float fUa;

        /// <summary>
        /// 电压B相相位（暂时无用）
        /// </summary>
        public float fUb;
        /// <summary>
        /// 电压C相相位（暂时无用）
        /// </summary>
        public float fUc;

        /// <summary>
        /// 电流A相相位（暂时无用）
        /// </summary>
        public float fIa;

        /// <summary>
        /// 电流B相相位（暂时无用）
        /// </summary>
        public float fIb;
        /// <summary>
        /// 电流C相相位（暂时无用）
        /// </summary>
        public float fIc;

        /// <summary>
        /// 频率
        /// </summary>
        public float PingLv ;

        /// <summary>
        /// 误差上限
        /// </summary>
        public float WuChaXian_Shang ;

        /// <summary>
        /// 误差下限
        /// </summary>
        public float WuChaXian_Xia ;


        /// <summary>
        /// 变差上限
        /// </summary>
        public float BianChaXian_Shang;

        /// <summary>
        /// 变差差下限
        /// </summary>
        public float BianChaXian_Xia;

        /// <summary>
        /// 误差次数
        /// </summary>
        public int WcCheckNumic;
        /// <summary>
        /// 提示
        /// </summary>
        public bool bPrompt;
        /// <summary>
        /// 圈数
        /// </summary>
        public int LapCount;
        /// <summary>
        /// 相序0-正相序，1-逆相序
        /// </summary>
        public int XiangXu;
        /// <summary>
        /// 谐波0-不加，1-加
        /// </summary>
        public int XieBo;

        /// <summary>
        /// 如果加谐波，则看调用哪套谐波方案
        /// </summary>
        public string XieBoFa;
        /// <summary>
        /// 谐波参数列表
        /// </summary>
        public List<StXieBo> XieBoItem;


        /// <summary>
        /// 解析电压倍数字符串（xUa|xUb|xUc）
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
        /// 相位角(Ua,Ub,Uc|Ia,Ib,Ic)（暂时无用）
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
        /// 解析误差限（上线|下线）
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
        /// 解析变差差限（上线|下线）
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
        /// 组合电压倍数字符串
        /// </summary>
        /// <returns></returns>
        public string JoinxUString()
        {
            return string.Format("{0}|{1}|{2}", this.xUa.ToString(), this.xUb.ToString(), this.xUc.ToString());
        }
        
        /// <summary>
        /// 组合误差限字符串
        /// </summary>
        /// <returns></returns>
        public String JoinWcxString()
        {
            return string.Format("{0}|{1}", this.WuChaXian_Shang > 0 ? string.Format("+{0}", this.WuChaXian_Shang.ToString()) : this.WuChaXian_Shang.ToString()
                                          , this.WuChaXian_Xia > 0 ? string.Format("+{0}", this.WuChaXian_Xia.ToString()) : this.WuChaXian_Xia.ToString());
        }

        /// <summary>
        /// 组合变差限字符串
        /// </summary>
        /// <returns></returns>
        public String JoinBcxString()
        {
            return string.Format("{0}|{1}", this.BianChaXian_Shang > 0 ? string.Format("+{0}", this.BianChaXian_Shang.ToString()) : this.BianChaXian_Shang.ToString()
                                          , this.BianChaXian_Xia > 0 ? string.Format("+{0}", this.BianChaXian_Xia.ToString()) : this.BianChaXian_Xia.ToString());
        }

        /// <summary>
        /// 组合相位字符串（暂时无用）
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
        /// 特殊检定描述
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string _GlfxString;
            switch (PowerFangXiang)
            {
                case CLDC_Comm.Enum.Cus_PowerFangXiang.正向有功:
                    {
                        _GlfxString = "P+";
                        break;
                    }
                case CLDC_Comm.Enum.Cus_PowerFangXiang.反向有功:
                    {
                        _GlfxString = "P-";
                        break;
                    }
                case CLDC_Comm.Enum.Cus_PowerFangXiang.正向无功:
                    {
                        _GlfxString = "Q+";
                        break;
                    }
                case CLDC_Comm.Enum.Cus_PowerFangXiang.反向无功:
                    {
                        _GlfxString = "Q-";
                        break;
                    }
                case CLDC_Comm.Enum.Cus_PowerFangXiang.第一象限无功:
                    {
                        _GlfxString = "Q1";
                        break;
                    }
                case CLDC_Comm.Enum.Cus_PowerFangXiang.第二象限无功:
                    {
                        _GlfxString = "Q2";
                        break;
                    }
                case CLDC_Comm.Enum.Cus_PowerFangXiang.第三象限无功:
                    {
                        _GlfxString = "Q3";
                        break;
                    }
                case CLDC_Comm.Enum.Cus_PowerFangXiang.第四象限无功:
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
        /// 获取当前被检点的误差限
        /// </summary>
        /// <param name="WcLimitName">误差限名称</param>
        /// <param name="GuiChengName">规程名称</param>
        /// <param name="Dj">等级1.0(2.0)</param>
        /// <param name="Hgq">是否经互感器接入</param>
        public void SetWcx(string Dj)
        {
            string _Wcx = "";

            _Wcx = CLDC_DataCore.DataBase.clsWcLimitDataControl.getLimit_Effect(PrjType
                                                        , Dj                                                        
                                                        , this.PowerYinSu);
            this.SetWcx(float.Parse(_Wcx), float.Parse(string.Format("-{0}", _Wcx)));       //设置误差限

        }

        /// <summary>
        /// 设置误差限
        /// </summary>
        /// <param name="Max">上线</param>
        /// <param name="Min">下限</param>
        internal void SetWcx(float Max, float Min)
        {            
            this.WuChaXian_Shang = Max;
        
            this.WuChaXian_Xia = Min;
        }

    }


}
