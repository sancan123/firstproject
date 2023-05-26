using System.Collections.Generic;
using CLDC_DataCore.Struct;
using System.Xml;
using CLDC_DataCore.DataBase;
using System.Windows.Forms;

namespace CLDC_DataCore.SystemModel.Item
{
    /// <summary>
    /// 
    /// </summary>
    public class csUserInfo
    {
        private Dictionary<string, StUserInfo> _UserInfo;
        /// <summary>
        /// 构造函数
        /// </summary>
        public csUserInfo()
        { 
            _UserInfo=new Dictionary<string,StUserInfo>();
        }
        /// <summary>
        /// 
        /// </summary>
        ~csUserInfo()
        {
            _UserInfo = null;
        }

        /// <summary>
        /// 检测用户是否存在
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <returns></returns>
        public bool FindUser(string UserName)
        {
            return _UserInfo.ContainsKey(UserName);
                
        }

        /// <summary>
        /// 系统登陆验证
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Pwd">密码</param>
        /// <param name="OutUserInfo">用户信息结构体</param>
        /// <returns>返回登陆成功或失败</returns>
        public bool CheckIn(string UserName, string Pwd, out StUserInfo OutUserInfo)
        {
            if (!_UserInfo.ContainsKey(UserName) || Pwd != _UserInfo[UserName].Pwd)
            {
                OutUserInfo = new StUserInfo();
                return false;
            }
            else
            {
                OutUserInfo = _UserInfo[UserName];
            }
            return true;

        }
    }
}
