
#region FileHeader And Descriptions
// ***************************************************************
//  ExceptionEx   date: 10/26/2009 By Niaoked
//  -------------------------------------------------------------
//  Description:
//  检定器异常类。用于检定器内部抛出错误
//  -------------------------------------------------------------
//  Copyright (C) 2009 -CdClou All Rights Reserved
// ***************************************************************
// Modify Log:
// 10/26/2009 09-42-57    Created
// ***************************************************************
#endregion
using System;
using System.Collections.Generic;
using CLDC_Comm.Enum;
namespace CLDC_Comm.MessageArgs
{
    /// <summary>
    /// 
    /// </summary>
    public class ExceptionEx : Exception
    {
        /// <summary>
        /// 错误编号
        /// </summary>
        public int ErrorID = 0;

        /// <summary>
        /// 错误描述，通过错误ID
        /// </summary>
        private static Dictionary<int, string> m_lstErrors = new Dictionary<int, string>();
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ErrID">错误ID，与系统错误对照表对应。</param>
        public ExceptionEx(int ErrID)
            : base()
        {
            ErrorID = ErrID;
        }
        /// <summary>
        /// 获取错误描述
        /// </summary>
        public override string Message
        {
            get
            {
                if (m_lstErrors.ContainsKey(ErrorID))
                {
                    return m_lstErrors[ErrorID];
                }
                else
                {
                    return String.Format("暂无错误描述：错误代码：{0}", ErrorID);
                }
            }
        }

    }
}
