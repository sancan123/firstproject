using System;
using System.Collections.Generic;
using System.Text;
using Comm.BaseClass;

namespace VerifyAdapter.Helper
{
    class Rs485CallBackHelper:SingletonBase<Rs485CallBackHelper>
    {
       public  Rs485Helper Rs485Hleper {get;set;}

        #region -----------无参数，BOOL型返回代理回调-----------
        /// <summary>
        /// 无参数BOOL回调
        /// </summary>
        /// <returns></returns>
        public delegate bool CallBack();

       /// <summary>
        /// 无参BOOL型代理
        /// </summary>
        /// <param name="callBack">一个无参BOOL型返回值委托</param>
        /// <returns>全部返回为真，反之为假</returns>
        public bool CallBackWithNoPara(CallBack callBack)
        {
            if (Rs485Hleper == null) return false;
            for (int i = 0; i < Rs485Hleper.RetryTimes; i++)
            {
                if (!callBack())
                    continue;
                Rs485Hleper.WaitReturn();
                if (Rs485Hleper.IsAllReturn())
                    break;
            }
            return Rs485Hleper.IsAllReturn();
        }
        #endregion

        #region -----------一个INT参数，BOOL型返回回调-----------
        /// <summary>
        /// 一个INT参数回调
        /// </summary>
        /// <param name="pos">第一个参数，一般为表位号</param>
        /// <returns></returns>
        public delegate bool CallBackOnePara_Int(int pos);

        /// <summary>
        /// 一个INT型参数，BOOL值返回代理
        /// </summary>
        /// <param name="callBack">一个int型参数，BOOL值返回的委托</param>
        /// <param name="para">要传入的int类型参数</param>
        /// <returns>成功为真，反之为假</returns>
        public bool CallBackWithOnePara_Int(CallBackOnePara_Int callBack,int para)
        {
            if (Rs485Hleper == null) return false;
            for (int i = 0; i < Rs485Hleper.RetryTimes; i++)
            {
                if (!callBack(para))
                    continue;
                if (Rs485Hleper.IsAllReturn())
                    break;
            }
            return Rs485Hleper.IsAllReturn();
        }
        #endregion


    }
}
