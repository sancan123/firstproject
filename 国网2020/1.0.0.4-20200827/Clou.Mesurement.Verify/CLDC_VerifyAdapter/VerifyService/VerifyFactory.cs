using System.Reflection;

namespace CLDC_VerifyAdapter.VerifyService
{
    /// 检定器工厂,解析检定服务
    /// <summary>
    /// 检定器工厂,解析检定服务
    /// </summary>
    internal class VerifyFactory
    {
        ///获取检定器
        /// <summary>
        /// 获取检定器
        /// </summary>
        /// <param name="className">检定项方法名称</param>
        /// <param name="verifyPara"></param>
        /// <returns></returns>
        public VerifyBase GetVerifyControl(string className, string verifyPara)
        {
            if (string.IsNullOrEmpty(className))
            {
                return null;
            }
            VerifyBase verifyControl = null;

            string fullName = string.Format("CLDC_VerifyAdapter.{0}", className);
            Assembly currentAssembly = Assembly.GetAssembly(this.GetType());
            //className, true, BindingFlags.CreateInstance, null, paras, null, null
            object objTemp = currentAssembly.CreateInstance(fullName, true, BindingFlags.CreateInstance, null, new object[] { verifyPara }, null, null);
            if (objTemp is VerifyBase)
            {
                verifyControl = (VerifyBase)objTemp;
            }
            return verifyControl;
        }
    }
}
