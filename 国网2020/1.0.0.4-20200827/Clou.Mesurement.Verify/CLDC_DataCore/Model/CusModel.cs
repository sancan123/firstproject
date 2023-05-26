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
        /// ���ܱ���Ϣģ��
        /// </summary>
        public DnbGroupInfo DnbData;
        /// <summary>
        /// ϵͳ������Ϣ�б�����ϵͳģ��Я��ϵͳ����������Ϣ��
        /// </summary>
        public Dictionary<string, string> SystemInfo;

        private int _Bws;

        private int _Taiid;



        /// <summary>
        /// ���캯�����������ϢList
        /// </summary>
        /// <param name="Bws">����̨��λ��</param>
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
        /// ��������
        /// </summary>
        ~CusModel()
        {
            DnbData = null;
            SystemInfo=null;
        }

        
        /// <summary>
        /// �ӱ������л������м���ģ�����ݡ��������ʧ�ܣ��򷵻�һ��ȫ�µ�ģ�Ͷ���
        /// </summary>
        public void Load()
        {
            //TODO:����ʱ��������ݣ�����ģʽ�����Ż���������Դ���ʱ���ȡģ�ͣ�����Ϊû�м��ꡣ��֮������ģ�ͣ���ǰ����Ҳ�ÿ�
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
        /// ������ܱ����ݵ���ʱ�ļ���
        /// </summary>
        public void Save()
        {
            
            //DnbData.Save();
        }


        ///// <summary>
        ///// ���л��洢δ�ϴ��ĵ��ܱ����ݵ�δ�ϴ��ļ���
        ///// </summary>
        ///// <param name="_DnbInfo"></param>
        //public static void SaveWaitUptoServerFile(DnbGroupInfo _DnbInfo)
        //{
        //    Function.File.WriteFileData(string.Format("\\{0}\\{1}.dat", CLDC_DataCore.Const.Variable.CONST_WAITUPDATE,DateTime.Now.ToString()), _DnbInfo.GetBytes());
        //}
    }

}
