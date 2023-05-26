/*-----------------------------------------------------------------------------------
 * Copyright(C) 2011 深圳市科陆软件有限公司电测事业部
 * 文件名: DLTFun.cs
 * 文件功能描述: 公共函数
 * 创建标识: ShiHe 20110316
 * 修改标识:
 * 修改描述:
 *-----------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_MeterProtocol.Ammeter.DLT645
{
    /// <summary>
    /// 公共函数
    /// </summary>
    public class DLTFun
    {
        /// <summary>
        /// 调整字符串大小
        /// </summary>
        /// <param name="origStr">待调字符串</param>
        /// <param name="desiredSize">调整的大小</param>
        /// <param name="isForward">是否前移</param>
        /// <returns>调整后的字符串</returns>
        public static string AdjustmentStrSize(string origStr, int desiredSize, bool isForward)
        {
            string strData = "";
            if (origStr.Length < desiredSize)
            {
                strData = origStr;
                for (int i = 0; i < desiredSize - origStr.Length; i++)
                {
                    if (isForward) //"123" => "0123"
                        strData = String.Intern("0") + strData;
                    else //"123" => "1230"
                        strData += String.Intern("0");
                }
            }
            else
            {
                if (isForward) //"0123" => "123"
                    strData = origStr.Substring(origStr.Length - desiredSize);
                else //"1230" => "123"
                    strData = origStr.Substring(0, desiredSize);
            }
            return strData;
        }

        /// <summary>
        /// 16进制字符串转字节数组
        ///     【"123" => {0x23,0x01}】
        /// </summary>
        /// <param name="strData">16进制字符串</param>
        /// <param name="aryLen">数组长度</param>
        /// <returns>字节数组</returns>
        public static byte[] HexStrToBytsData(string strData, int aryLen)
        {
            byte[] bytsData = new byte[aryLen];
            string tempStrData = AdjustmentStrSize(strData, aryLen * 2, true); //调整长度
            for (int i = 0; i < aryLen; i++)
            {
                bytsData[aryLen - 1 - i] = Convert.ToByte(tempStrData.Substring(i * 2, 2), 16);
            }
            return bytsData;
        }
        /// <summary>
        /// 16进制字符串转字节数组
        ///     【"123" => {0x01,0x23}】
        /// </summary>
        /// <param name="strData">16进制字符串</param>
        /// <param name="aryLen">数组长度</param>
        /// <returns>字节数组</returns>
        public static byte[] HexStrToBytsDataA(string strData, int aryLen)
        {
            byte[] bytsData = new byte[aryLen];
            string tempStrData = AdjustmentStrSize(strData, aryLen * 2, true); //调整字符串的长度
            for (int i = 0; i < aryLen; i++)
            {
                bytsData[i] = Convert.ToByte(tempStrData.Substring(i * 2, 2), 16);
            }
            return bytsData;
        }

        /// <summary>
        /// 字节数组转16进制字符串
        ///     【{0x23,0x01}=> "123"】
        /// </summary>
        /// <param name="bytsData">字节数组</param>
        /// <returns>16进制字符串</returns>
        public static string BytsToHexStrData(byte[] bytsData)
        {
            string strData = "";
            int aryCount = bytsData.Length;
            for (int i = 0; i < aryCount; i++)
                strData = bytsData[i].ToString("X2") + strData;
            return strData;
        }
        /// <summary>
        /// 字节数组转16进制字符串
        ///     【{0x23,0x01}=> "2301"】
        /// </summary>
        /// <param name="bytsData">字节数组</param>
        /// <returns>16进制字符串</returns>
        public static string BytsToHexStrDataA(byte[] bytsData) 
        {
            string strData = "";
            int aryCount = bytsData.Length;
            for (int i = 0; i < aryCount; i++)
                strData += bytsData[i].ToString("X2");
            return strData;
        }
        /// <summary>
        /// 调整数组的长度
        /// </summary>
        /// <param name="origArray">原数组</param>
        /// <param name="desiredSize">调整后的长度</param>
        /// <returns>调整后的数组</returns>
        public static Array Redim(Array origArray, int desiredSize)
        {
            Array newArray = null;
            if (origArray != null)
            {
                Type type = origArray.GetType().GetElementType(); //元素类型
                newArray = Array.CreateInstance(type, desiredSize);
                Array.Copy(origArray, 0, newArray, 0, Math.Min(origArray.Length, desiredSize));
            }
            return newArray;
        }

        /// <summary>
        /// 检定是否符合DLT协议
        /// </summary>
        /// <param name="dataBuff">数据缓存</param>
        /// <returns>符合则返回帧起始下标；否则返回错误编码</returns>
        public static int CheckFrame(byte[] dataBuff)
        {
            #region --错误编码--
            int noStartSign = -1; //搜索起始符失败
            int noFullFrame = -2; //不是完整帧
            int frameInfoErr = -3; //帧信息错误
            #endregion
            int s4_Start = 0;
            s4_Start = Array.IndexOf(dataBuff, ProtocolBase.START_ID);
            if (s4_Start < 0)
                return noStartSign;
            if (s4_Start + 12 > dataBuff.Length)
                return noFullFrame;
            if (dataBuff[s4_Start + ProtocolBase.HEAD2_POS] == ProtocolBase.START_ID)
            {
                int s4_Len = dataBuff[s4_Start + ProtocolBase.FLEN_POS]; //帧长度
                if (s4_Start + s4_Len + 12 > dataBuff.Length)
                    return noFullFrame;
                byte byt_Chksum = ProtocolBase.GetCheckSum(dataBuff, s4_Start, s4_Len + 12); //校验和
                if (dataBuff[s4_Start + s4_Len + 10] == byt_Chksum)
                    if (dataBuff[s4_Start + s4_Len + 11] == ProtocolBase.END_ID) //结束符
                        return s4_Start;
            }
            return noFullFrame;
        }
    }
}
