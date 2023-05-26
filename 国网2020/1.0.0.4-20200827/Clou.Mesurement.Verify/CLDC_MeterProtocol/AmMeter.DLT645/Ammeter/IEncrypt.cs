/*-----------------------------------------------------------------------------------
 * Copyright(C) 2011 深圳市科陆软件有限公司电测事业部
 * 文件名: IEncrypt.cs
 * 文件功能描述: 数据加密函数定义
 * 创建标识: ShiHe 20110316
 * 修改标识:
 * 修改描述:
 *-----------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Text;

namespace CLDC_MeterProtocol.Ammeter
{
    /// <summary>
    /// 数据加密
    /// </summary>
    public interface IEncrypt
    {
        /// <summary>
        /// 数据加密
        /// </summary>
        /// <param name="sKeyState">密钥状态[0-生产密钥 1-交易密钥]</param>
        /// <param name="meterNum">表号</param>
        /// <param name="dataType">数据类型[0-控制数据 1-一类参数 2-二类参数]</param>
        /// <param name="syllabify">明文</param>
        /// <param name="cryptograph">密文</param>
        /// <param name="macAddr">MAC地址</param>
        /// <returns>True-加密成功；False-加密失败</returns>
        bool Encrypt(byte sKeyState, string meterNum, byte dataType, byte[] syllabify,
            out byte[] cryptograph, ref byte[] macAddr);

        bool Encrypt(byte sKeyState, string meterNum, byte dataType, byte[] syllabify, string DI2,
           out byte[] cryptograph, ref byte[] macAddr);
    }
}
