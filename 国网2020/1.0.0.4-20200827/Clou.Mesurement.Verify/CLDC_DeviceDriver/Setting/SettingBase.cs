using System;
using System.Collections.Generic;

namespace CLDC_DeviceDriver.Setting
{
    /// <summary>
    /// 配置信息
    /// </summary>
    public abstract class SettingBase 
    {
        private object objLock = new object();

        /// <summary>
        /// &lt;名称，值&gt;
        /// </summary>
        private Dictionary<string, string> DicValues = new Dictionary<string, string>();

        /// <summary>
        /// &lt;名称，描述&gt;
        /// </summary>
        private Dictionary<string, string> DicDescription = new Dictionary<string, string>();

        /// <summary>
        /// &lt;名称，颜色&gt;
        /// </summary>
        private Dictionary<string, string> DicColor = new Dictionary<string, string>();

        private string szFileName = string.Empty;

        /// <summary>
        /// 配置项目名称（关键字）
        /// </summary>
        public System.Collections.Generic.IEnumerable<string> Keys
        {
            get
            {
                return DicValues.Keys;
            }
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="szfname">保存配置信息的文件名称(INI配置文件)</param>
        public SettingBase(string szfname)
        {
            this.szFileName = szfname;
            this.InitItemValue();
            this.LoadFromFile();
        }

        /// <summary>
        /// 初始化数据，将需要使用的配置字段使用 AddDefault(string,string)函数添加进来。
        /// </summary>
        public abstract void InitItemValue();

        /// <summary>
        /// 添加一个默认配置项目
        /// </summary>
        /// <param name="szKey"></param>
        /// <param name="szValue"></param>
        public virtual void AddDefault(string szKey, string szValue,string szDescription,string szColor)
        {
            lock (objLock)
            {
                if (DicValues.ContainsKey(szKey))
                {
                    throw new Exception(string.Format("配置项目：{0} 已经存在",szKey));
                }
                this.DicValues.Add(szKey, szValue);
                this.DicDescription.Add(szKey, szDescription);
                this.DicColor.Add(szKey, szColor);
            }
        }

        /// <summary>
        /// 获取指定项目的值
        /// </summary>
        /// <param name="szKey"></param>
        /// <returns></returns>
        public virtual string GetValue(string szKey)
        {
            lock (objLock)
            {
                if (!DicValues.ContainsKey(szKey))
                {
                    throw new Exception(string.Format("配置项目：{0} 不存在", szKey));
                }
                return this.DicValues[szKey];
            }
        }

        /// <summary>
        /// 获取指定项目的描述
        /// </summary>
        /// <param name="szKey"></param>
        /// <returns></returns>
        public virtual string GetDescription(string szKey)
        {
            lock (objLock)
            {
                if (!DicDescription.ContainsKey(szKey))
                {
                    throw new Exception(string.Format("配置项目：{0} 不存在", szKey));
                }
                return this.DicDescription[szKey];
            }
        }
        /// <summary>
        /// 获取指定项目的背景色
        /// </summary>
        /// <param name="szKey"></param>
        /// <returns></returns>
        public virtual string GetColor(string szKey)
        {
            lock (objLock)
            {
                if (!DicColor.ContainsKey(szKey))
                {
                    throw new Exception(string.Format("配置项目：{0} 不存在", szKey));
                }
                return this.DicColor[szKey];
            }
        }
        /// <summary>
        /// 添加、修改 备注说明
        /// </summary>
        /// <param name="szkey"></param>
        /// <param name="szDescription"></param>
        public virtual void SetDescription(string szkey, string szDescription)
        {
            lock (objLock)
            {
                if (!DicDescription.ContainsKey(szkey))
                {
                    this.DicDescription.Add(szkey, szDescription);
                }
                else
                {
                    this.DicDescription[szkey] = szDescription;
                }
            }
        }
        /// <summary>
        /// 添加、修改 背景颜色
        /// </summary>
        /// <param name="szkey"></param>
        /// <param name="szDescription"></param>
        public virtual void SetColor(string szkey, string szColor)
        {
            lock (objLock)
            {
                if (!DicColor.ContainsKey(szkey))
                {
                    this.DicColor.Add(szkey, szColor);
                }
                else
                {
                    this.DicColor[szkey] = szColor;
                }
            }
        }

        /// <summary>
        /// 获取、设置配置数据
        /// </summary>
        /// <param name="szKey"></param>
        /// <returns></returns>
        public virtual string this[string szKey]
        {
            get
            {
                return GetValue(szKey);
            }
            set
            {
                lock (objLock)
                {
                    if (!DicValues.ContainsKey(szKey))
                    {
                        throw new Exception(string.Format("项目：{0} ，不存在!", szKey));
                    }
                    DicValues[szKey] = value;
                }
            }
        }

        /// <summary>
        /// 从文件加载
        /// </summary>
        /// <returns></returns>
        public virtual void LoadFromFile()
        {
            List<string> LstKeys = new List<string>();
            foreach (string szKey in this.Keys)
            {
                LstKeys.Add(szKey);
            }
            int lstCount=LstKeys.Count;
            for (int i = 0; i < lstCount; i++)
            {
                string szKey = LstKeys[i];
                string szValue = CLDC_DataCore.Function.File.ReadInIString(CLDC_DataCore.Function.File.GetPhyPath(szFileName), "Setting", szKey, "");
                string szDescription = CLDC_DataCore.Function.File.ReadInIString(CLDC_DataCore.Function.File.GetPhyPath(szFileName), "Setting", szKey + "_Description", "");
                string szColor = CLDC_DataCore.Function.File.ReadInIString(CLDC_DataCore.Function.File.GetPhyPath(szFileName), "Setting", szKey + "_Color", "");
                if (szValue != string.Empty)
                {
                    this[szKey] = szValue;
                    if (szDescription.Trim() != string.Empty)
                    {
                        this.SetDescription(szKey, szDescription);
                    }
                    if (szColor.Trim() != string.Empty)
                    {
                        this.SetColor(szKey, szColor);
                    }
                }
            }
        }

        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <returns></returns>
        public virtual void SaveToFile()
        {
            foreach (string szKey in this.Keys)
            {
                CLDC_DataCore.Function.File.WriteInIString(CLDC_DataCore.Function.File.GetPhyPath(szFileName), "Setting", szKey, this[szKey]);

                CLDC_DataCore.Function.File.WriteInIString(CLDC_DataCore.Function.File.GetPhyPath(szFileName), "Setting", szKey + "_Description", GetDescription(szKey));

                CLDC_DataCore.Function.File.WriteInIString(CLDC_DataCore.Function.File.GetPhyPath(szFileName), "Setting", szKey + "_Color", GetColor(szKey));
            }
        }

        /// <summary>
        /// 将原来的数据清除掉
        /// </summary>
        public virtual void ClearOld()
        {
            CLDC_DataCore.Function.File.RemoveFile(CLDC_DataCore.Function.File.GetPhyPath(szFileName));
        }


        
    }
}
