using CLDC_DataCore;
using CLDC_DataCore.Const;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace CLDC_DeviceDriver.Drivers.Clou.DllPackage
{
   public abstract class ABase : Exception
    {
        protected Assembly ambly = null;
        protected object ob = null;
        protected Type ty = null;
        protected bool asmblyRst = false;
        public abstract string DisPlayName
        {
            get;
        }


        protected  int MethodInvoke(string methodName, object[] objPara)
        {
            object rst = 1;
            if (asmblyRst)
            {
                try
                {
                    MethodInfo mthd = ty.GetMethod(methodName);
                    if (mthd == null)
                    {
                        CLDC_DataCore.MessageController.Instance.AddMessage(string.Format("设备操作失败:设备名:{0},方法名:{1}", DisPlayName, methodName), 7, 2);
                        return 1;
                    }
                    else
                    {
                        rst = mthd.Invoke(ob, objPara);
                    }
                }
                catch (Exception e)
                {
                    CLDC_DataCore.MessageController.Instance.AddMessage(string.Format("设备操作失败:设备名:{0},方法名:{1},异常信息:{2}", DisPlayName, methodName, e.Message), 7, 2);
                    //throw e;
                }
            }
            if (rst == null)
                rst = 0;
            return (int)rst;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="className"></param>
        public ABase(string fileName, string className,bool isLocalAssemly)
        {
            try
            {
                if (isLocalAssemly)
                {
                    return;
                }
                string filePath = "";
                if (GlobalUnit.DeviceManufacturers == CLDC_Comm.Enum.Cus_DeviceManufacturers.科陆)
                {
                    filePath = string.Format(@"{0}\DllFile\DeviceManufacturers\Clou\{1}.dll", Application.StartupPath, fileName);
                }
                else if (GlobalUnit.DeviceManufacturers == CLDC_Comm.Enum.Cus_DeviceManufacturers.涵普)
                {
                    filePath = string.Format(@"{0}\DllFile\DeviceManufacturers\HyHpu\{1}.dll", Application.StartupPath, fileName);
                }
                else if (GlobalUnit.DeviceManufacturers == CLDC_Comm.Enum.Cus_DeviceManufacturers.格宁)
                {
                    filePath = string.Format(@"{0}\DllFile\DeviceManufacturers\GeNing\{1}.dll", Application.StartupPath, fileName);
                }
                else
                {
                    filePath = string.Format(@"{0}\DllFile\DeviceManufacturers\Clou\{1}.dll", Application.StartupPath, fileName);
                }

                //从本地加载程序集
                if (isLocalAssemly)
                {
                    filePath=fileName;
                }
                ambly = Assembly.LoadFile(filePath);
                ob = ambly.CreateInstance(string.Format("{0}.{1}", fileName, className));
                if (ob == null)
                {
                    //throw new Exception("创建\"" + className + "\"的实例失败。");
                    MessageController.Instance.AddMessage("创建\"" + DisPlayName + "\"的实例失败。", 7, 2);
                }
                else
                {
                    asmblyRst = true;
                    MessageController.Instance.AddMessage("创建\"" + DisPlayName + "\"成功。", 7);
                }
            }
            catch (Exception e)
            {
                asmblyRst = false;
                //throw new Exception("找不到" + DisPlayName + "驱动文件:" + fileName, e); 
                CLDC_DataCore.MessageController.Instance.AddMessage(string.Format("找不到{0}的驱动文件{1}.dll,{2}", DisPlayName, fileName, e.Message), 7, 2);
            }
            ty = ob.GetType();

        }

        public virtual int InitSetting(int ComNumber, int Parameter, int MaxWaitTme, int WaitSencondsPerByte, string IP, int RemotePort, int LocalStartPort)
        {
            object[] paras = new object[] { ComNumber, MaxWaitTme, WaitSencondsPerByte, IP, RemotePort, LocalStartPort };
            int temp = MethodInvoke("InitSetting", paras);
            if (temp == 0)
            {
                MessageController.Instance.AddMessage(string.Format("初始化{0},设备地址:{1}_{2}_{3},端口:{4}", DisPlayName, IP, RemotePort, LocalStartPort, ComNumber), 7);
            }
            else
            {
                MessageController.Instance.AddMessage(string.Format("初始化{0}失败!!", DisPlayName), 7, 2);
            }
            return temp;
        }

        public virtual int Connect()
        {
            object[] paras = null;
            int temp = MethodInvoke("Connect", paras);
            if (temp == 0)
            {
                MessageController.Instance.AddMessage(string.Format("设备:{0} 连接成功", DisPlayName), 7);
            }
            else
            {
                MessageController.Instance.AddMessage(string.Format("设备:{0} 连接失败", DisPlayName), 7, 2);
            }
            return temp;
        }

        public virtual int DisConnect()
        {
            object[] paras = null;
            int temp = MethodInvoke("DisConnect", paras);
            if (temp == 0)
            {
                MessageController.Instance.AddMessage(string.Format("设备:{0} 断开连接成功", DisPlayName), 7);
            }
            else
            {
                MessageController.Instance.AddMessage(string.Format("设备:{0} 断开连接失败", DisPlayName), 7, 2);
            }
            return temp;
        }
    }
}
