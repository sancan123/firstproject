using CLDC_Encryption.CLEncryption.Interface;

namespace CLDC_Encryption.CLEncryption
{

    public class EncryptionBase 
    {
        #region ----------公共事件----------


        /// <summary>
        /// 连接状态
        /// </summary>
        private bool m_Link = true;
        /// <summary>
        /// 连接状态
        /// </summary>
        public bool IsLink
        {
            set
            {
                m_Link = value;
            }
            get { return m_Link; }
        }

        ///认证状态 
        /// </summary>
        private static int[] m_status = new int[1];
        /// <summary>
        ///认证状态 
        /// </summary>
        public int[] Status
        {
            get { return m_status; }
            set { m_status = value; }
        }

        #endregion
    }
}
