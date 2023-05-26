using System;
using System.Collections.Generic;
using CLDC_DataCore.Model.DnbModel;
//using System.Windows.Forms;
namespace CLDC_DataCore
{

    [Serializable()]
    public class CusModel 
    {
        /// <summary>
        /// 电能表信息模型
        /// </summary>
        public DnbGroupInfo DnbData;
        /// <summary>
        /// 系统配置信息列表。用于系统模型携带系统必须配置信息。
        /// </summary>
        public Dictionary<string, string> SystemInfo;

        private int _Bws;

        private int _Taiid;



        /// <summary>
        /// 构造函数，构造表信息List
        /// </summary>
        /// <param name="Bws">电能台表位数</param>
        public CusModel(int Bws,int TaiID)
        {
            _Bws = Bws;
            _Taiid = TaiID;
            DnbData = new DnbGroupInfo(Bws,TaiID);

            CLDC_DataCore.SystemModel.Item.SystemConfigure _TmpSystem = new CLDC_DataCore.SystemModel.Item.SystemConfigure();
            _TmpSystem.Load();

            this.Load(_TmpSystem);
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~CusModel()
        {
            DnbData = null;
            SystemInfo=null;
        }

        
        /// <summary>
        /// 从本地序列化缓存中加载模型数据。如果加载失败，则返回一个全新的模型对象。
        /// </summary>
        public void Load()
        {
            //TODO:从临时库加载数据，现有模式保留优化。如果可以从临时库获取模型，则认为没有检完。反之创建新模型，当前方案也置空
            //DnbData = DnbData.LoadTmpData();
            //DataBase.clsDataManage clsDM = new DataBase.clsDataManage(Const.GlobalUnit.DBPathOfTempAccess,false);
            //DnbData.MeterGroup = clsDM.GetMeterListFromTempDB();
            if (DnbData == null || DnbData.MeterGroup.Count != _Bws)
            {
                //clsDM.DeleteTmp_MeterInfo(-1);
                DnbData = new DnbGroupInfo(_Bws, _Taiid);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TmpSystemInfo"></param>
        public void Load(CLDC_DataCore.SystemModel.Item.SystemConfigure TmpSystemInfo)
        {
            SystemInfo = new Dictionary<string, string>();

            foreach (string _Key in TmpSystemInfo.getKeyNames())
            {
                SystemInfo.Add(_Key, TmpSystemInfo.getItem(_Key).Value);
            }
        }
        /// <summary>
        /// 保存电能表数据到临时文件中
        /// </summary>
        public void Save()
        {
            
            //DnbData.Save();
        }


        ///// <summary>
        ///// 序列化存储未上传的电能表数据到未上传文件夹
        ///// </summary>
        ///// <param name="_DnbInfo"></param>
        //public static void SaveWaitUptoServerFile(DnbGroupInfo _DnbInfo)
        //{
        //    Function.File.WriteFileData(string.Format("\\{0}\\{1}.dat", CLDC_DataCore.Const.Variable.CONST_WAITUPDATE,DateTime.Now.ToString()), _DnbInfo.GetBytes());
        //}
    }

}
