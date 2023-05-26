namespace CLDC_DataCore.SystemModel
{
    /// <summary>
    /// 
    /// </summary>
    public class SystemInfo
    {
        /// <summary>
        /// 系统配置模型
        /// </summary>
        public Item.SystemConfigure SystemMode;//实验方法与依据、实验参数
        /// <summary>
        /// 用户信息集合
        /// </summary>
        public Item.csUserInfo UserGroup;
        /// <summary>
        /// 字典信息集合
        /// </summary>
        public Item.csDictionary ZiDianGroup;
        /// <summary>
        /// 功率因素字典集合
        /// </summary>
        public Item.csGlys GlysZiDian;
        /// <summary>
        /// 电流倍数字典
        /// </summary>
        public Item.csxIbDic xIbDic;
        /// <summary>
        /// 载波方案配置集合
        /// </summary>
        public Item.csCarrier ZaiBoInfo;
        /// <summary>
        /// 
        /// </summary>
        public Item.csDgnDic DgnDicInfo;
        /// <summary>
        /// 数据标识信息
        /// </summary>
        public Item.csDataFlag DataFlagInfo;
        /// <summary>
        /// 实验参数
        /// </summary>
        public Item.TestSetting testSetting;
        /// <summary>
        /// 构造函数
        /// </summary>
        public SystemInfo()
        {
            SystemMode = new Item.SystemConfigure();
            UserGroup = new Item.csUserInfo();
            ZiDianGroup = new Item.csDictionary();
            GlysZiDian = new Item.csGlys();
            xIbDic = new Item.csxIbDic();
            DgnDicInfo = new Item.csDgnDic();
            DataFlagInfo = new Item.csDataFlag();
            testSetting = new Item.TestSetting();
            ZaiBoInfo = new Item.csCarrier();
        }
        /// <summary>
        /// 
        /// </summary>
        ~SystemInfo()
        {
            UserGroup = null;
            SystemMode = null;
            ZiDianGroup = null;
            GlysZiDian = null;
            xIbDic = null;
            DgnDicInfo = null;
            DataFlagInfo = null;
            testSetting = null;
            ZaiBoInfo = null;
        }

        /// <summary>
        /// 系统登陆验证
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Pwd">密码</param>
        /// <param name="_UserInfo">输出用户信息结构体</param>
        /// <returns>返回验证成功或失败</returns>
        public bool CheckIn(string UserName, string Pwd, out Struct.StUserInfo _UserInfo)
        {
            return UserGroup.CheckIn(UserName, Pwd, out _UserInfo);
        }
        public void Load()
        {
           
            ZaiBoInfo.Load();
        
        }
     
    }
}
