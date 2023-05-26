using System;
using CLDC_Comm.BaseClass;
using CLDC_Comm.Enum;

namespace CLDC_VerifyAdapter.MulitThread
{
    public class MulitEncryptionWorkThreadManager : SingletonBase<MulitThreadManager>
    {
       
        public MulitEncryptionWorkThreadManager(int ChannelCount)
        {
            workThreads = new EncryptionWorkThread[ChannelCount];
        }
 

        /// <summary>
        /// 工作线程数组
        /// </summary>
        public EncryptionWorkThread[] workThreads = new EncryptionWorkThread[0];

 

        /// <summary>
        /// 停止所有工作线程
        /// </summary>
        public void Stop()
        {
            //首先发出停止指令
            foreach (EncryptionWorkThread workthread in workThreads)
            {
                workthread.Stop();
            }
            //等待所有工作线程都完成
            bool isAllThreaWorkDone = false;
            while (!isAllThreaWorkDone)
            {
                isAllThreaWorkDone = IsWorkDone();
            }

        }

        /// <summary>
        /// 等待所有线程工作完成
        /// </summary>
        public bool IsWorkDone()
        {
            bool isAllThreaWorkDone = true;

            foreach (EncryptionWorkThread workthread in workThreads)
            {
                if (workthread == null)
                    continue;
                isAllThreaWorkDone = workthread.IsWorkFinish();
                if (!isAllThreaWorkDone)
                {
                    break;
                }
            }
            if (isAllThreaWorkDone)
            {
                //Comm.MessageController.Instance.AddMessage("当前操作已经完成!", false);
            }
            return isAllThreaWorkDone;
        }
    }
}
