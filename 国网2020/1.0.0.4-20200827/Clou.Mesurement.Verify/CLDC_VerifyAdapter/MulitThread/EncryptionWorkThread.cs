using System;
using CLDC_DataCore;
using System.Threading;
using CLDC_Comm.Enum;
using CLDC_Encryption.CLEncryption.Interface;
using CLDC_DataCore.Struct;
using CLDC_DataCore.Const;

namespace CLDC_VerifyAdapter.MulitThread
{
    public class EncryptionWorkThread
    {
        private Thread workThread = null;                


        private bool runFlag = false;

        private bool workOverFlag = false;



        /// <summary>
        /// 停止当前工作任务
        /// </summary>
        public void Stop()
        {
            runFlag = true;
        }

        /// <summary>
        /// 工作线程是否完成
        /// </summary>
        /// <returns></returns>
        public bool IsWorkFinish()
        {
            return workOverFlag;
        }

        /// <summary>
        /// 按字节反转
        /// </summary>
        /// <param name="str_Keyinfo1"></param>
        /// <returns></returns>
        private static string DxString(string str_Keyinfo1)
        {
            int Len = str_Keyinfo1.Length / 2;
            string DxStr = "";
            for (int i = 0; i < Len; i++)
            {
                DxStr = str_Keyinfo1.Substring(i * 2, 2) + DxStr;
            }
            return DxStr;
        }        

    }
}
