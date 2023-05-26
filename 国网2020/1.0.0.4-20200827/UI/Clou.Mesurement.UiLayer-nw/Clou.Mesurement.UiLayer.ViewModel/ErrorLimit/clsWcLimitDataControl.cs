using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using Mesurement.UiLayer.DAL;

namespace Mesurement.UiLayer.ViewModel.ErrorLimit
{
    /// <summary>
    /// 在误差限获取模块化中专用
    /// </summary>
    public struct IDAndValue
    {
        /// <summary>
        /// 数据库中对应ID
        /// </summary>
        public long id;
        /// <summary>
        /// 数据库中对应值
        /// </summary>
        public string Value;
        public override string ToString()
        {
            return Value;
        }
    }

    public class clsWcLimitDataControl
    {
        #region -------------------------------获取一个项目的值返回用IDAndValue结构体--------------------------------
        /// <summary>
        /// 获取规程名称
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public IDAndValue getGuiChengValue(string Name)
        {
            string valueTemp = CodeDictionary.GetValueLayer2("WcLimitRule", Name);
            return new IDAndValue() { id = long.Parse(valueTemp), Value = Name } ;
        }
        /// <summary>
        /// 获取等级名称
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public IDAndValue getDjValue(string Name)
        {
            string valueTemp = CodeDictionary.GetValueLayer2("MeterAccuracyLevel", Name);
            return new IDAndValue() { id = long.Parse(valueTemp), Value = Name };
        }
        #endregion

        /// <summary>
        /// 获取误差限
        /// </summary>
        /// <param name="xIb">电流倍数</param>
        /// <param name="GuiChengName">规程名称</param>
        /// <param name="Dj">等级</param>
        /// <param name="Yj">元件</param>
        /// <param name="glys">功率因素</param>
        /// <param name="Hgq">互感器</param>
        /// <param name="YouGong">是否有功</param>
        /// <returns></returns>
        public static string Wcx(string xIb, string GuiChengName, string Dj, int Yj, string glys, bool Hgq, bool YouGong)
        {
            switch (GuiChengName.ToUpper())
            {
                case "JJG596-1999":
                    {
                        return getLimit_JJG596_1999(xIb, Dj, Yj, glys, Hgq);
                    }
                case "JJG307-1988":
                    {
                        return getGy(xIb, "JJG307-1988", Dj, Yj, glys, Hgq, YouGong);
                    }
                case "JJG307-2006":
                    {
                        return getGy(xIb, "JJG307-2006", Dj, Yj, glys, Hgq, YouGong);
                    }
                case "JJG596-2012":
                    {
                        return getLimit_JJG596_2012(xIb, Dj, Yj, glys, Hgq);
                    }
                default:
                    return Dj;
            }
        }

        private static string getLimit_JJG596_1999(string xIb, string Dj, int Yj, string glys, bool Hgq)
        {
            switch (Dj)
            {
                #region
                case "0.02":
                    return getdz002(xIb, Yj, glys, Hgq);
                case "0.05":
                    return getdz005(xIb, Yj, glys, Hgq);
                case "0.1":
                    return getdz01(xIb, Yj, glys, Hgq);
                case "0.2":
                    if ((int)Yj == 1)  ///合元
                    {
                        return getdz02(xIb, Yj, glys, Hgq);
                    }
                    else
                    {
                        if (glys == "1.0")
                            return "0.3";
                        else
                            return "0.4";
                    }
                case "0.5":
                    if ((int)Yj == 1)  ///合元
                    {
                        return getdz05(xIb, Yj, glys, Hgq);
                    }
                    else
                    {
                        if (glys == "1.0")
                            return "0.6";
                        else
                            return "1.0";
                    }
                case "1.0":
                    if ((int)Yj == 1)  ///合元
                    {
                        return getdz10(xIb, Yj, glys, Hgq);
                    }
                    else
                    {
                        if (glys == "1.0")
                            return "2.0";
                        else
                            return "2.0";
                    }
                case "2.0":
                    if ((int)Yj == 1)  ///合元
                    {
                        return getdz20(xIb, Yj, glys, Hgq);
                    }
                    else
                    {
                        if (glys == "1.0")
                            return "3.0";
                        else
                            return "3.0";
                    }
                case "3.0":
                    if ((int)Yj == 1)  ///合元
                    {
                        return getdz30(xIb, Yj, glys, Hgq);
                    }
                    else
                    {
                        if (glys == "1.0")
                            return "4.0";
                        else
                            return "4.0";
                    }
                default:
                    return Dj;

                #endregion
            }
        }

        private static string getLimit_JJG596_2012(string xIb, string Dj, int Yj, string glys, bool Hgq)
        {
            switch (Dj)
            {
                #region
                case "0.02":
                    return getdz002(xIb, Yj, glys, Hgq);
                case "0.05":
                    return getdz005(xIb, Yj, glys, Hgq);
                case "0.1":
                    return getdz01(xIb, Yj, glys, Hgq);
                case "0.2":
                    if ((int)Yj == 1)  ///合元
                    {
                        return getdz02(xIb, Yj, glys, Hgq);
                    }
                    else
                    {
                        if (glys == "1.0")
                            return "0.3";
                        else
                            return "0.4";
                    }
                case "0.5":
                    if ((int)Yj == 1)  ///合元
                    {
                        return getdz05(xIb, Yj, glys, Hgq);
                    }
                    else
                    {
                        if (glys == "1.0")
                            return "0.6";
                        else
                            return "1.0";
                    }
                case "1.0":
                    if ((int)Yj == 1)  ///合元
                    {
                        return getdz10(xIb, Yj, glys, Hgq);
                    }
                    else
                    {
                        if (glys == "1.0")
                            return "2.0";
                        else
                            return "2.0";
                    }
                case "2.0":
                    if ((int)Yj == 1)  ///合元
                    {
                        return getdz20(xIb, Yj, glys, Hgq);
                    }
                    else
                    {
                        if (glys == "1.0")
                            return "3.0";
                        else
                            return "3.0";
                    }
                case "3.0":
                    if ((int)Yj == 1)  ///合元
                    {
                        return getdz30(xIb, Yj, glys, Hgq);
                    }
                    else
                    {
                        if (glys == "1.0")
                            return "4.0";
                        else
                            return "4.0";
                    }
                default:
                    return Dj;

                #endregion
            }
        }

        #region 
        /// <summary>
        /// 电子式0.02级
        /// </summary>
        /// <param name="xIb">电流倍数</param>
        /// <param name="Yj">元件</param>
        /// <param name="glys">功率因素</param>
        /// <param name="Hgq">互感器</param>
        /// <returns></returns>
        private static string getdz002(string xIb, int Yj, string glys, bool Hgq)
        {
            string _WcLimit = "";
            if (xIb.ToLower() == "ib")
            {
                xIb = "1.0ib";
            }
            if ((int)Yj == 1)
            {
                if (glys == "1.0")
                {
                    _WcLimit = "0.02";
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.1F)
                        _WcLimit = "0.04";
                }
                else
                {
                    _WcLimit = "0.02";
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.5F)
                        _WcLimit = "0.03";
                    else if (xIb.ToLower().IndexOf("imax") >= 0 || xIb.ToLower().IndexOf("imax-ib") >= 0 || float.Parse(xIb.ToLower().Replace("ib", "")) > 0.1F)
                        _WcLimit = "0.03";
                    if (glys == "0.25L")
                        _WcLimit = "0.04";
                    else if (glys == "0.5C")
                        _WcLimit = "0.03";
                }
            }
            else
            {
                _WcLimit = "0.03";
                if (glys != "1.0" && (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.5F))
                    _WcLimit = "0.04";
            }
            return _WcLimit;

        }
        /// <summary>
        /// 电子式0.05级
        /// </summary>
        /// <param name="xIb"></param>
        /// <param name="Yj"></param>
        /// <param name="glys"></param>
        /// <param name="Hgq"></param>
        /// <returns></returns>
        private static string getdz005(string xIb, int Yj, string glys, bool Hgq)
        {
            string _WcLimit = "";
            if (xIb.ToLower() == "ib")
            {
                xIb = "1.0ib";
            }
            if ((int)Yj == 1)
            {
                if (glys == "1.0")
                {
                    _WcLimit = "0.05";
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.1F)
                        _WcLimit = "0.1";
                }
                else
                {
                    _WcLimit = "0.05";
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.2F)
                        _WcLimit = "0.15";
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.5F)
                        _WcLimit = "0.075";
                    if (xIb.ToLower().IndexOf("imax") >= 0 || xIb.ToLower().IndexOf("imax-ib") >= 0 || float.Parse(xIb.ToLower().Replace("ib", "")) > 0.1F)
                        _WcLimit = "0.075";
                    if (glys.ToUpper() == "0.5C")
                        _WcLimit = "0.1";
                    if (glys.ToUpper() == "0.25L")
                        _WcLimit = "0.15";
                }
            }
            else
            {
                _WcLimit = "0.075";
                if (glys != "1.0" && (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.5F))
                    _WcLimit = "0.1";
            }
            return _WcLimit;
        }
        /// <summary>
        /// 电子式0.1级
        /// </summary>
        /// <param name="xIb"></param>
        /// <param name="Yj"></param>
        /// <param name="glys"></param>
        /// <param name="Hgq"></param>
        /// <returns></returns>
        private static string getdz01(string xIb, int Yj, string glys, bool Hgq)
        {
            string _WcLimit = "";
            if (xIb.ToLower() == "ib")
            {
                xIb = "1.0ib";
            }
            if ((int)Yj == 1)
                if (glys == "1.0")
                {
                    _WcLimit = "0.1";
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.1F)
                        _WcLimit = "0.2";
                }
                else
                {
                    _WcLimit = "0.1";
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.2F)
                        _WcLimit = "0.3";
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.5F)
                        _WcLimit = "0.15";
                    if (xIb.ToLower().IndexOf("imax") >= 0 || xIb.ToLower().IndexOf("imax-ib") >= 0 || float.Parse(xIb.ToLower().Replace("ib", "")) > 0.1F)
                        _WcLimit = "0.15";
                    if (glys == "0.5C")
                        _WcLimit = "0.2";
                    if (glys == "0.25L")
                        _WcLimit = "0.3";
                }
            else
            {
                _WcLimit = "0.15";
                if (glys != "1.0" && (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.5F))
                    _WcLimit = "0.2";
            }
            return _WcLimit;
        }
        /// <summary>
        /// 电子式0.2级
        /// </summary>
        /// <param name="xIb"></param>
        /// <param name="Yj"></param>
        /// <param name="glys"></param>
        /// <param name="Hgq"></param>
        /// <returns></returns>
        private static string getdz02(string xIb, int Yj, string glys, bool Hgq)
        {
            string _WcLimit = "";
            if (xIb.ToLower() == "ib")
            {
                xIb = "1.0ib";
            }
            if (glys == "1.0")
                if (Hgq)
                {
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.05F)
                        _WcLimit = "0.4";
                    else
                        _WcLimit = "0.2";
                }
                else
                {
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.1F)
                        _WcLimit = "0.4";
                    else
                        _WcLimit = "0.2";
                }
            else if (glys.ToUpper() == "0.5C" || glys.ToUpper() == "0.25L")
                _WcLimit = "0.5";
            else
            {
                if (Hgq)
                {
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.1F)
                        _WcLimit = "0.5";
                    else
                        _WcLimit = "0.3";
                }
                else
                {
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.2F)
                        _WcLimit = "0.5";
                    else
                        _WcLimit = "0.3";
                }
            }

            return _WcLimit;
        }
        /// <summary>
        /// 电子式0.5级
        /// </summary>
        /// <param name="xIb"></param>
        /// <param name="Yj"></param>
        /// <param name="glys"></param>
        /// <param name="Hgq"></param>
        /// <returns></returns>
        private static string getdz05(string xIb, int Yj, string glys, bool Hgq)
        {
            string _WcLimit = "";
            if (xIb.ToLower() == "ib")
            {
                xIb = "1.0ib";
            }
            if (glys == "1.0")
                if (Hgq)
                {
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.05F)
                        _WcLimit = "1";
                    else
                        _WcLimit = "0.5";
                }
                else
                {
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.1F)
                        _WcLimit = "0.4";
                    else
                        _WcLimit = "0.2";
                }
            else if (glys.ToUpper() == "0.5C" || glys.ToUpper() == "0.25L")
                _WcLimit = "1";
            else
            {
                if (Hgq)
                {
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.1F)
                        _WcLimit = "1";
                    else
                        _WcLimit = "0.6";
                }
                else
                {
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.2F)
                        _WcLimit = "1";
                    else
                        _WcLimit = "0.6";
                }
            }

            return _WcLimit;
        }
        /// <summary>
        /// 电子式1级
        /// </summary>
        /// <param name="xIb"></param>
        /// <param name="Yj"></param>
        /// <param name="glys"></param>
        /// <param name="Hgq"></param>
        /// <returns></returns>
        private static string getdz10(string xIb, int Yj, string glys, bool Hgq)
        {
            string _WcLimit = "";
            if (xIb.ToLower() == "ib")
            {
                xIb = "1.0ib";
            }
            if (glys == "1.0")
                if (Hgq)
                {
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.05F)
                        _WcLimit = "1.5";
                    else
                        _WcLimit = "1.0";
                }
                else
                {
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.1F)
                        _WcLimit = "1.5";
                    else
                        _WcLimit = "1.0";
                }
            else if (glys.ToUpper() == "0.5C")
                _WcLimit = "2.5";
            else if (glys.ToUpper() == "0.25L")
                _WcLimit = "3.5";
            else
            {
                if (Hgq)
                {
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.1F)
                        _WcLimit = "1.5";
                    else
                        _WcLimit = "1.0";
                }
                else
                {
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.2F)
                        _WcLimit = "1.5";
                    else
                        _WcLimit = "1.0";
                }
            }

            return _WcLimit;
        }
        /// <summary>
        /// 电子式2级
        /// </summary>
        /// <param name="xIb"></param>
        /// <param name="Yj"></param>
        /// <param name="glys"></param>
        /// <param name="Hgq"></param>
        /// <returns></returns>
        private static string getdz20(string xIb, int Yj, string glys, bool Hgq)
        {
            string _WcLimit = "";
            if (xIb.ToLower() == "ib")
            {
                xIb = "1.0ib";
            }
            if (glys == "1.0")
                if (Hgq)
                {
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.05F)
                        _WcLimit = "2.5";
                    else
                        _WcLimit = "2.0";
                }
                else
                {
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.1F)
                        _WcLimit = "2.5";
                    else
                        _WcLimit = "2.0";
                }
            else
            {
                if (Hgq)
                {
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.1F)
                        _WcLimit = "2.5";
                    else
                        _WcLimit = "2.0";
                }
                else
                {
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.2F)
                        _WcLimit = "2.5";
                    else
                        _WcLimit = "2.0";
                }
            }

            return _WcLimit;
        }
        /// <summary>
        /// 电子式3级
        /// </summary>
        /// <param name="xIb"></param>
        /// <param name="Yj"></param>
        /// <param name="glys"></param>
        /// <param name="Hgq"></param>
        /// <returns></returns>
        private static string getdz30(string xIb, int Yj, string glys, bool Hgq)
        {
            string _WcLimit = "";
            if (xIb.ToLower() == "ib")
            {
                xIb = "1.0ib";
            }
            if (glys == "1.0")
                if (Hgq)
                {
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.05F)
                        _WcLimit = "3.5";
                    else
                        _WcLimit = "3.0";
                }
                else
                {
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.1F)
                        _WcLimit = "3.5";
                    else
                        _WcLimit = "3.0";
                }
            else
            {
                if (Hgq)
                {
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.1F)
                        _WcLimit = "3.5";
                    else
                        _WcLimit = "3.0";
                }
                else
                {
                    if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.2F)
                        _WcLimit = "3.5";
                    else
                        _WcLimit = "3.0";
                }
            }

            return _WcLimit;
        }

        /// <summary>
        /// 获取感应式电能表的误差限
        /// </summary>
        /// <param name="xIb">电流倍数</param>
        /// <param name="GuiChengName">规程名称</param>
        /// <param name="Dj">等级</param>
        /// <param name="Yj">元件</param>
        /// <param name="glys">功率因素</param>
        /// <param name="Hgq">互感器</param>
        /// <param name="YouGong">有无功</param>
        /// <returns></returns>
        private static string getGy(string xIb, string GuiChengName, string Dj, int Yj, string glys, bool Hgq, bool YouGong)
        {
            string _WcLimit = "";
            if (xIb.ToLower() == "0.1ib")
            { }
            if (xIb.ToLower() == "ib")
            {
                xIb = "1.0ib";
            }
            if (YouGong)        //有功
            {
                if ((int)Yj == 1)      //合元
                {
                    if (glys == "1.0")
                    {
                        _WcLimit = Dj;
                        if (GuiChengName.ToUpper() == "JJG307-2006" && Hgq)
                        {
                            if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.05F)
                                _WcLimit = (float.Parse(_WcLimit) + 0.5F).ToString();
                        }
                        else if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.1F)
                            _WcLimit = (float.Parse(_WcLimit) + 0.5F).ToString();
                    }
                    else if (glys.ToUpper() == "0.5C")
                    {
                        switch (Dj)
                        {
                            case "0.5":
                                {
                                    _WcLimit = "1.5";
                                    break;
                                }
                            case "1.0":
                                {
                                    _WcLimit = "2.5";
                                    break;
                                }
                            case "2.0":
                                {
                                    _WcLimit = "3.5";
                                    break;
                                }
                            default:
                                {
                                    _WcLimit = "3.5";
                                    break;
                                }
                        }
                    }
                    else if (glys.ToUpper() == "0.25L")
                    {
                        switch (Dj)
                        {
                            case "0.5":
                                {
                                    _WcLimit = "2.5";
                                    break;
                                }
                            case "1.0":
                                {
                                    _WcLimit = "3.5";
                                    break;
                                }
                            case "2.0":
                                {
                                    _WcLimit = "4.5";
                                    break;
                                }
                            default:
                                {
                                    _WcLimit = "4.5";
                                    break;
                                }
                        }
                    }
                    else
                    {
                        switch (Dj)
                        {
                            case "0.5":
                                {
                                    _WcLimit = "0.8";
                                    break;
                                }
                            case "1.0":
                                {
                                    _WcLimit = "1.0";
                                    break;
                                }
                            case "2.0":
                                {
                                    _WcLimit = "2.0";
                                    break;
                                }
                            default:
                                {
                                    _WcLimit = "2.0";
                                    break;
                                }
                        }
                        if (GuiChengName.ToUpper() == "JJG307-2006" && Hgq)
                        {
                            if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.1F)
                                _WcLimit = (float.Parse(_WcLimit) + 0.5F).ToString();
                        }
                        else if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.2F)
                            _WcLimit = (float.Parse(_WcLimit) + 0.5F).ToString();
                    }
                }
                else
                {
                    switch (Dj)
                    {
                        case "0.5":
                            {
                                _WcLimit = "1.5";
                                break;
                            }
                        case "1.0":
                            {
                                _WcLimit = "2.0";
                                break;
                            }
                        case "2.0":
                            {
                                _WcLimit = "2.0";
                                break;
                            }
                        default:
                            {
                                _WcLimit = "3.0";
                                break;
                            }

                    }
                    if (GuiChengName.ToUpper() == "JJG307-1988")
                        if (xIb.ToLower().IndexOf("imax") >= 0 || xIb.ToLower().IndexOf("imax-ib") >= 0 || float.Parse(xIb.ToLower().Replace("ib", "")) > 1F)
                            _WcLimit = (float.Parse(_WcLimit) + 1F).ToString();
                }
            }
            else//无功
            {
                if ((int)Yj == 1)           //合元
                {
                    if (glys == "1.0")
                    {
                        _WcLimit = Dj;
                        if (GuiChengName.ToUpper() == "JJG307-2006" && Hgq)
                        {
                            if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.1F)
                                _WcLimit = (float.Parse(_WcLimit) + 1.0F).ToString();
                        }
                        else if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.2F)
                            _WcLimit = (float.Parse(_WcLimit) + 1.0F).ToString();
                    }
                    else if (glys.ToUpper() == "0.25C" || glys.ToUpper() == "0.25L")
                    {
                        _WcLimit = (float.Parse(Dj) * 2F).ToString();
                    }
                    else
                    {
                        if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.5F)
                            _WcLimit = (float.Parse(Dj) + 2.0F).ToString();
                        else
                            _WcLimit = Dj;

                        if (GuiChengName.ToUpper() == "JJG307-2006")
                        {
                            if (Hgq)
                            {
                                if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.2F)
                                    _WcLimit = (float.Parse(_WcLimit) - 1F).ToString();
                                else if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.5F)
                                    _WcLimit = (float.Parse(_WcLimit) - 2F).ToString();
                            }
                            else if (xIb.ToLower().IndexOf("imax") == -1 && xIb.ToLower().IndexOf("imax-ib") == -1 && float.Parse(xIb.ToLower().Replace("ib", "")) < 0.5F)
                                _WcLimit = (float.Parse(_WcLimit) - 1F).ToString();
                        }
                    }
                }
                else
                    _WcLimit = (float.Parse(Dj) + 1.0F).ToString();
            }

            return _WcLimit;

        }
        #endregion

    }
}
