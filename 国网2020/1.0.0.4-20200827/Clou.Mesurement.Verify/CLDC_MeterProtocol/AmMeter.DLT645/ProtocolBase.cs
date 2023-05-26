/*-----------------------------------------------------------------------------------
 * Copyright(C) 2011 深圳市科陆软件有限公司电测事业部
 * 文件名: ProtocolBase.cs
 * 文件功能描述: DLT645协议基类
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
    /// DLT协议基类
    /// </summary>
    public class ProtocolBase
    {
        #region ---- 协议格式 ----
        /// <summary>
        /// 第一个起始符
        /// </summary>
        public const int HEAD1_POS = 0;
        /// <summary>
        /// 第二个起始符
        /// </summary>
        public const int HEAD2_POS = 7;
        /// <summary>
        /// 控制码
        /// </summary>
        public const int CTL_POS = 8;
        /// <summary>
        /// 帧长度
        /// </summary>
        public const int FLEN_POS = 9;

        /// <summary>
        /// 起始符
        /// </summary>
        public const byte START_ID = 0x68;
        /// <summary>
        /// 结束符
        /// </summary>
        public const byte END_ID = 0x16;
        #endregion

        /// <summary>
        /// 获取帧数据
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="addrRegion">地址域</param>
        /// <param name="ctlCode">控制码</param>
        /// <param name="dataRegion">数据域</param>
        /// <returns>帧长度</returns>
        public int GetFrameData(out byte[] frameData, byte[] addrRegion, byte ctlCode, byte[] dataRegion) 
        {
            int frameLen =12 + dataRegion.Length;
            frameData = new byte[frameLen];
            frameData[HEAD1_POS] = START_ID;
            Array.Copy(addrRegion, 0, frameData, HEAD1_POS + 1, 6);
            frameData[HEAD2_POS] = START_ID;
            frameData[CTL_POS] = ctlCode;
            frameData[FLEN_POS] = (byte)dataRegion.Length;
            if (dataRegion.Length != 0)
                Array.Copy(dataRegion, 0, frameData, FLEN_POS + 1, dataRegion.Length);
            frameData[frameLen - 2] = GetCheckSum(frameData);
            frameData[frameLen - 1] = END_ID;
            return frameLen;
        }
        /// <summary>
        /// 解析帧数据
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="dataRegion">数据域</param>
        /// <param name="cltCode">控制码</param>
        /// <returns>数据域长度；解析失败则返回-1</returns>
        public int ParseFrameData(byte[] frameData, out byte[] dataRegion, ref byte cltCode)
        {
            int dataLen = frameData[FLEN_POS];
            dataRegion = new byte[dataLen];
            Array.Copy(frameData, FLEN_POS + 1, dataRegion, 0, dataLen);
            cltCode = frameData[CTL_POS];
            return dataLen;
        }

        /// <summary>
        /// 计算校验和
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <returns>校验和</returns>
        public static byte GetCheckSum(byte[] frameData) 
        {
            return GetCheckSum(frameData, 0, frameData.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataBuff"></param>
        /// <param name="strIndex"></param>
        /// <param name="frameLen"></param>
        /// <returns></returns>
        public static byte GetCheckSum(byte[] dataBuff, int strIndex, int frameLen)
        {
            byte chkSum = 0;
            int bytCount = frameLen - 2; //校验位和结束符不参与计算
            for (int i = 0; i < bytCount; i++)
            {
                chkSum += dataBuff[strIndex + i];
            }
            return chkSum;
        }

        /// <summary>
        /// 更新帧数据
        /// </summary>
        /// <param name="frameData">帧数据</param>
        /// <param name="addrRegion">地址域</param>
        public static void UpdateFrame(byte[] frameData, byte[] addrRegion) 
        {
            Array.Copy(addrRegion, 0, frameData, HEAD1_POS + 1, 6);
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
            s4_Start = Array.IndexOf(dataBuff, START_ID);
            if (s4_Start < 0)
                return noStartSign;
            if (s4_Start + 12 > dataBuff.Length)
                return noFullFrame;
            if (dataBuff[s4_Start + HEAD2_POS] == START_ID)
            {
                int s4_Len = dataBuff[s4_Start + FLEN_POS]; //帧长度
                if (s4_Start + s4_Len + 12 > dataBuff.Length)
                    return noFullFrame;
                byte byt_Chksum = GetCheckSum(dataBuff, s4_Start, s4_Len + 12); //校验和
                if (dataBuff[s4_Start + s4_Len + 10] == byt_Chksum)
                    if (dataBuff[s4_Start + s4_Len + 11] == END_ID) //结束符
                        return s4_Start;
            }
            return noFullFrame;
        }
    }
}
