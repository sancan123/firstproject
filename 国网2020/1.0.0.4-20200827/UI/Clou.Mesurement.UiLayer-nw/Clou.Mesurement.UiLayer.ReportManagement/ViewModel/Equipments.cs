using Mesurement.UiLayer.DAL;
using Mesurement.UiLayer.DAL.DataBaseView;
using Mesurement.UiLayer.ViewModel;
using Mesurement.UiLayer.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mesurement.UiLayer.DataManager.ViewModel
{
    /// <summary>
    /// 台体信息
    /// </summary>
    public class Equipments : ViewModelBase
    {
        private static Equipments instance;

        public static Equipments Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Equipments();
                }
                return instance;
            }
        }
        private TableDisplayModel displayModel = ResultViewHelper.GetTableDisplayModel("41", true);
        public void Initialize()
        {
            List<DynamicModel> modelList = DALManager.ApplicationDbDal.GetList("DSPTCH_EQUIP_INFO");
            for (int i = 0; i < modelList.Count; i++)
            {
                DynamicViewModel modelTemp = new DynamicViewModel(i);
                #region 解析台体信息
                DynamicModel modelEquip = modelList[i];
                for (int j = 0; j < displayModel.ColumnModelList.Count; j++)
                {
                    ColumnDisplayModel columnModel = displayModel.ColumnModelList[j];
                    string fieldValue = modelEquip.GetProperty(columnModel.Field) as string;
                    if (fieldValue == null)
                    {
                        fieldValue = "";
                    }
                    string[] displayNames = columnModel.DisplayName.Split('|');
                    string[] valueArray = fieldValue.Split('|');
                    for (int k = 0; k < displayNames.Length; k++)
                    {
                        if (valueArray.Length > k)
                        {
                            modelTemp.SetProperty(displayNames[k], valueArray[k]);
                        }
                        else
                        {
                            modelTemp.SetProperty(displayNames[k], "");
                        }
                    }
                }
                #endregion
                Models.Add(modelTemp);
            }
        }

        private AsyncObservableCollection<DynamicViewModel> models=new AsyncObservableCollection<DynamicViewModel>();
        /// <summary>
        /// 信息列表
        /// </summary>
        public AsyncObservableCollection<DynamicViewModel> Models
        {
            get { return models; }
            set { models = value; }
        }
        /// <summary>
        /// 创建新的台体信息
        /// </summary>
        /// <param name="equipmentNo"></param>
        public DynamicViewModel FindEquipInfo(string equipmentNo)
        {
            DynamicViewModel modelTemp = Models.FirstOrDefault(item => item.GetProperty(equipNoName) as string == equipmentNo);
            if (modelTemp != null)
            {
                return modelTemp;
            }
            modelTemp = new DynamicViewModel(0);
            for (int j = 0; j < displayModel.ColumnModelList.Count; j++)
            {
                ColumnDisplayModel columnModel = displayModel.ColumnModelList[j];
                if (columnModel.Field == "AVR_DEVICE_ID")
                {
                    modelTemp.SetProperty(columnModel.DisplayName, equipmentNo);
                    continue;
                }
                string[] displayNames = columnModel.DisplayName.Split('|');
                for (int k = 0; k < displayNames.Length; k++)
                {
                    modelTemp.SetProperty(displayNames[k], "");
                }
            }
            Models.Add(modelTemp);
            return modelTemp;
        }

        /// <summary>
        /// 保存台体信息
        /// </summary>
        /// <param name="modelTemp"></param>
        public void SaveEquipInfo(DynamicViewModel modelTemp)
        {
            DynamicModel modelDal = new DynamicModel();
            for (int j = 0; j < displayModel.ColumnModelList.Count; j++)
            {
                ColumnDisplayModel columnModel = displayModel.ColumnModelList[j];
                List<string> listTemp = new List<string>();
                string[] displayNames = columnModel.DisplayName.Split('|');
                for (int k = 0; k < displayNames.Length; k++)
                {
                    listTemp.Add(modelTemp.GetProperty(displayNames[k]) as string);
                }
                modelDal.SetProperty(columnModel.Field, string.Join("|", listTemp));
            }
            string equipmentNo = modelDal.GetProperty("AVR_DEVICE_ID") as string;
            if (string.IsNullOrEmpty(equipmentNo))
            {
                return;
            }
            string where = string.Format("AVR_DEVICE_ID='{0}'", equipmentNo);
            int countTemp = DALManager.ApplicationDbDal.GetCount("DSPTCH_EQUIP_INFO", where);
            if (countTemp == 0)
            {
                DALManager.ApplicationDbDal.Insert("DSPTCH_EQUIP_INFO", modelDal);
            }
            else
            {
                List<string> fieldsUpdate = modelDal.GetAllProperyName();
                fieldsUpdate.Remove("AVR_DEVICE_ID");
                DALManager.ApplicationDbDal.Update("DSPTCH_EQUIP_INFO", where, modelDal, fieldsUpdate);
            }
        }

        public List<string> GetNames()
        {
            List<string> nameList = new List<string>();
            for (int j = 0; j < displayModel.ColumnModelList.Count; j++)
            {
                ColumnDisplayModel columnModel = displayModel.ColumnModelList[j];
                string[] displayNames = columnModel.DisplayName.Split('|');
                nameList.AddRange(displayNames);
            }
            return nameList;
        }

        /// <summary>
        /// 获取台体编号的显示名称
        /// </summary>
        /// <returns></returns>
        private string equipNoName
        {
            get
            {
                ColumnDisplayModel columnModel = displayModel.ColumnModelList.Find(item => item.Field == "AVR_DEVICE_ID");
                return columnModel.DisplayName;
            }
        }
    }
}
