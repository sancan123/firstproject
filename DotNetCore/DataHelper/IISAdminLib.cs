using System;
using System.DirectoryServices;
using System.Collections;
using System.Text.RegularExpressions;
using System.Text;

namespace DataHelper
{
    //IIS管理类
    public class IISAdminLib
    {
        #region UserName,Password,HostName的定义
        public static string HostName
        {
            get
            {
                return hostName;
            }
            set
            {
                hostName = value;
            }
        }
        public static string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
            }
        }
        public static string Password
        {
            get
            {
                return password;
            }
            set
            {
                if (UserName.Length <= 1)
                {
                    throw new ArgumentException("还没有指定好用户名。请先指定用户名");
                }
                password = value;
            }
        }
        public static void RemoteConfig(string hostName, string userName, string password)
        {
            HostName = hostName;
            UserName = userName;
            Password = password;
        }
        private static string hostName = "localhost";
        private static string userName;
        private static string password;
        #endregion
        #region 根据路径构造Entry的方法
        /// <summary> 
        ///  根据是否有用户名来判断是否是远程服务器。 
        ///  然后再构造出不同的DirectoryEntry出来 
        /// </summary> 
        /// <param name="entPath">DirectoryEntry的路径</param> 
        /// <returns>返回的是DirectoryEntry实例</returns> 
        public static DirectoryEntry GetDirectoryEntry(string entPath)
        {
            DirectoryEntry ent;

            if (UserName == null)
            {
                ent = new DirectoryEntry(entPath);
            }
            else
            {
                //    ent = new DirectoryEntry(entPath, HostName+"\\"+UserName, Password, AuthenticationTypes.Secure); 
                //ent = new DirectoryEntry(entPath, UserName, Password, AuthenticationTypes.Secure);
                ent = new DirectoryEntry(entPath, "userName", "password");
                //ent = new DirectoryEntry(entPath, "sxcj", "Clousanxiangchoujian5!", AuthenticationTypes.Secure);
            }
            return ent;
        }
        #endregion
        #region 添加，删除网站的方法
        /// <summary> 
        ///  创建一个新的网站。根据传过来的信息进行配置 
        /// </summary> 
        /// <param name="siteInfo">存储的是新网站的信息</param> 
        public static void CreateNewWebSite(NewWebSiteInfo siteInfo)
        {
            if (!EnsureNewSiteEnavaible(siteInfo.BindString))
            {
                //throw new DuplicatedWebSiteException("已经有了这样的网站了。" + Environment.NewLine + siteInfo.BindString);
                return;
            }
            string entPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry rootEntry = GetDirectoryEntry(entPath);
            string newSiteNum = GetNewWebSiteID();
            DirectoryEntry newSiteEntry = rootEntry.Children.Add(newSiteNum, "IIsWebServer");
            newSiteEntry.CommitChanges();
            newSiteEntry.Properties["ServerBindings"].Value = siteInfo.BindString;
            newSiteEntry.Properties["ServerComment"].Value = siteInfo.CommentOfWebSite;
            newSiteEntry.Properties["AppPoolId"].Value = siteInfo.AppPoolIdOfWebSite;
            newSiteEntry.CommitChanges();
            DirectoryEntry vdEntry = newSiteEntry.Children.Add("root", "IIsWebVirtualDir");
            vdEntry.CommitChanges();
            vdEntry.Properties["Path"].Value = siteInfo.WebPath;
            vdEntry.CommitChanges();
        }
        /// <summary> 
        ///  删除一个网站。根据网站名称删除。 
        /// </summary> 
        /// <param name="siteName">网站名称</param> 
        public static void DeleteWebSiteByName(string siteName)
        {
            string siteNum = GetWebSiteNum(siteName);
            string siteEntPath = String.Format("IIS://{0}/w3svc/{1}", HostName, siteNum);
            DirectoryEntry siteEntry = GetDirectoryEntry(siteEntPath);
            string rootPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry rootEntry = GetDirectoryEntry(rootPath);
            rootEntry.Children.Remove(siteEntry);
            rootEntry.CommitChanges();
        }
        #endregion
        #region 添加，删除程序池
        /// <summary>
        /// 判断程序池是否存在
        /// </summary>
        /// <param name="AppPoolName">程序池名称</param>
        /// <returns>true存在 false不存在</returns>
        public static bool IsAppPoolName(string AppPoolName)
        {
            bool result = false;
            DirectoryEntry appPools = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");
            foreach (DirectoryEntry getdir in appPools.Children)
            {
                if (getdir.Name.Equals(AppPoolName))
                {
                    result = true;
                }
            }
            return result;
        }

        public static bool CreatAppPool(string AppPoolName)
        {
            if (!IsAppPoolName(AppPoolName))
            {
                DirectoryEntry newpool;
                DirectoryEntry appPools = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");
                newpool = appPools.Children.Add(AppPoolName, "IIsApplicationPool");
                newpool.CommitChanges();
            }
            return true;
        }

        /// <summary>
        /// 删除指定程序池
        /// </summary>
        /// <param name="AppPoolName">程序池名称</param>
        /// <returns>true删除成功 false删除失败</returns>
        public static bool DeleteAppPool(string AppPoolName)
        {
            bool result = false;
            DirectoryEntry appPools = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");
            foreach (DirectoryEntry getdir in appPools.Children)
            {
                if (getdir.Name.Equals(AppPoolName))
                {
                    try
                    {
                        getdir.DeleteTree();
                        result = true;
                    }
                    catch
                    {
                        result = false;
                    }
                }
            }
            return result;
        }
        #endregion
        #region Start和Stop网站的方法
        public static void StartWebSite(string siteName)
        {
            string siteNum = GetWebSiteNum(siteName);
            string siteEntPath = String.Format("IIS://{0}/w3svc/{1}", HostName, siteNum);
            DirectoryEntry siteEntry = GetDirectoryEntry(siteEntPath);
            siteEntry.Invoke("Start", new object[] { });
        }
        public static void StopWebSite(string siteName)
        {
            string siteNum = GetWebSiteNum(siteName);
            if (string.IsNullOrEmpty(siteNum)) return;
            string siteEntPath = String.Format("IIS://{0}/w3svc/{1}", HostName, siteNum);
            DirectoryEntry siteEntry = GetDirectoryEntry(siteEntPath);
            siteEntry.Invoke("Stop", new object[] { });
        }
        #endregion
        #region Start和Stop应用程序池的方法
        public static void StartAppPool(string appPoolName)
        {
            DirectoryEntry appPool = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");

            DirectoryEntry findPool = appPool.Children.Find(appPoolName, "IIsApplicationPool");

            findPool.Invoke("Start", null);

            appPool.CommitChanges();

            appPool.Close();
        }
        public static void StopAppPool(string appPoolName)
        {
            DirectoryEntry appPool = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");

            DirectoryEntry findPool = appPool.Children.Find(appPoolName, "IIsApplicationPool");

            findPool.Invoke("Stop", null);

            appPool.CommitChanges();

            appPool.Close();
        }
        #endregion
        #region 确认网站是否相同
        /// <summary> 
        ///  确定一个新的网站与现有的网站没有相同的。 
        ///  这样防止将非法的数据存放到IIS里面去 
        /// </summary> 
        /// <param name="bindStr">网站邦定信息</param> 
        /// <returns>真为可以创建，假为不可以创建</returns> 
        public static bool EnsureNewSiteEnavaible(string bindStr)
        {
            string entPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry ent = GetDirectoryEntry(entPath);
            foreach (DirectoryEntry child in ent.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    if (child.Properties["ServerBindings"].Value != null)
                    {
                        if (child.Properties["ServerBindings"].Value.ToString() == bindStr)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        #endregion
        #region 获取一个网站编号的方法
        /// <summary> 
        ///  获取一个网站的编号。根据网站的ServerBindings或者ServerComment来确定网站编号 
        /// </summary> 
        /// <param name="siteName"></param> 
        /// <returns>返回网站的编号</returns> 
        /// <exception cref="NotFoundWebSiteException">表示没有找到网站</exception> 
        public static string GetWebSiteNum(string siteName)
        {
            Regex regex = new Regex(siteName);
            string tmpStr;
            string entPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry ent = GetDirectoryEntry(entPath);
            foreach (DirectoryEntry child in ent.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    if (child.Properties["ServerBindings"].Value != null)
                    {
                        tmpStr = child.Properties["ServerBindings"].Value.ToString();
                        if (regex.Match(tmpStr).Success)
                        {
                            return child.Name;
                        }
                    }
                    if (child.Properties["ServerComment"].Value != null)
                    {
                        tmpStr = child.Properties["ServerComment"].Value.ToString();
                        if (regex.Match(tmpStr).Success)
                        {
                            return child.Name;
                        }
                    }
                }
            }
            //throw new NotFoundWebSiteException("没有找到我们想要的站点" + siteName);
            return "";
        }
        #endregion
        #region 获取新网站id的方法
        /// <summary> 
        ///  获取网站系统里面可以使用的最小的ID。 
        ///  这是因为每个网站都需要有一个唯一的编号，而且这个编号越小越好。 
        ///  这里面的算法经过了测试是没有问题的。 
        /// </summary> 
        /// <returns>最小的id</returns> 
        public static string GetNewWebSiteID()
        {
            ArrayList list = new ArrayList();
            string tmpStr;
            string entPath = String.Format("IIS://{0}/w3svc", HostName);
            DirectoryEntry ent = GetDirectoryEntry(entPath);
            foreach (DirectoryEntry child in ent.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    tmpStr = child.Name.ToString();
                    list.Add(Convert.ToInt32(tmpStr));
                }
            }
            list.Sort();
            int i = 1;
            foreach (int j in list)
            {
                if (i == j)
                {
                    i++;
                }
            }
            return i.ToString();
        }
        #endregion
    }
    #region 新网站信息结构体
    public struct NewWebSiteInfo
    {
        private string hostIP;   // The Hosts IP Address 
        private string portNum;   // The New Web Sites Port.generally is "80" 
        private string appPoolIdOfWebSite; // 应用程序池。一般为网站的网站名。
        private string commentOfWebSite;// 网站注释。一般也为网站的网站名。 
        private string webPath;   // 网站的主目录。例如"e:\tmp" 
        public NewWebSiteInfo(string hostIP, string portNum, string appPoolIdOfWebSite, string commentOfWebSite, string webPath)
        {
            this.hostIP = hostIP;
            this.portNum = portNum;
            this.appPoolIdOfWebSite = appPoolIdOfWebSite;
            this.commentOfWebSite = commentOfWebSite;
            this.webPath = webPath;
        }
        public string BindString
        {
            get
            {
                //return String.Format("{0}:{1}:{2}", hostIP, portNum, commentOfWebSite);
                return String.Format("{0}:{1}:{2}", hostIP, portNum, "");
            }
        }
        public string CommentOfWebSite
        {
            get
            {
                return commentOfWebSite;
            }
        }
        public string AppPoolIdOfWebSite
        {
            get
            {
                return appPoolIdOfWebSite;
            }
        }           
        public string WebPath
        {
            get
            {
                return webPath;
            }
        }
    }
    #endregion
}