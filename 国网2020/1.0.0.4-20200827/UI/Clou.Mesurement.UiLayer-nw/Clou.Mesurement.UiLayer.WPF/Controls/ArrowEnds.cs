// ArrowEnds.cs by Charles Petzold, December 2007
using System;

namespace Mesurement.UiLayer.WPF.Controls
{
    /// ��ͷ����λ�õ�ö��
    /// <summary>
    /// ��ͷ����λ�õ�ö��
    /// </summary>
    [Flags]
    public enum ArrowEnds
    {
        /// û�м�ͷ
        /// <summary>
        /// û�м�ͷ
        /// </summary>
        None = 0,
        /// �ڿ�ʼ�ĵ�
        /// <summary>
        /// �ڿ�ʼ�ĵ�
        /// </summary>
        Start = 1,
        /// �ڿ�ʼ�ĵ�
        /// <summary>
        /// �ڿ�ʼ�ĵ�
        /// </summary>
        Begin = 1,
        /// �ڽ�����
        /// <summary>
        /// �ڽ�����
        /// </summary>
        End = 2,
        /// ��ͷ���м�ͷ
        /// <summary>
        /// ��ͷ���м�ͷ
        /// </summary>
        Both = 3
    }
}
