using Mesurement.UiLayer.DAL;
using Mesurement.UiLayer.Utility;
using Mesurement.UiLayer.Utility.Log;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;

namespace Mesurement.UiLayer.ViewModel.Schema
{
    /// 方案操作视图模型
    /// <summary>
    /// 方案操作视图模型
    /// </summary>
    public class SchemaOperationViewModel : ViewModelBase
    {
        public SchemaOperationViewModel()
        {
            LoadSchemas();
        }
        private DynamicViewModel selectedSchema=new DynamicViewModel(0);

        public DynamicViewModel SelectedSchema
        {
            get { return selectedSchema; }
            set { SetPropertyValue(value, ref selectedSchema, "SelectedSchema"); }
        }

        private ObservableCollection<DynamicViewModel> schemas = new ObservableCollection<DynamicViewModel>();
        /// <summary>
        /// 方案名称列表
        /// </summary>
        public ObservableCollection<DynamicViewModel> Schemas
        {
            get { return schemas; }
            set { SetPropertyValue(value, ref schemas, "Schemas"); }
        }
        /// 加载方案列表
        /// <summary>
        /// 加载方案列表
        /// </summary>
        private void LoadSchemas()
        {
            List<DynamicModel> models = DALManager.ApplicationDbDal.GetList("schema_info");
            Schemas.Clear();
            for (int i = 0; i < models.Count; i++)
            {
                Schemas.Add(new DynamicViewModel(models[i],i));
            }
        }
        /// 方案名校验
        /// <summary>
        /// 方案名校验
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        private bool CheckSchemaName(string schemaName)
        {
            //名称校验
            if (string.IsNullOrEmpty(schemaName))
            {
                LogManager.AddMessage("方案名无效,方案名不允许为空", EnumLogSource.用户操作日志, EnumLevel.WarningSpeech);
                return false;
            }
            if (!StringCheck.IsFileName(schemaName))
            {
                LogManager.AddMessage("方案名无效,方案名只允许为数字字母和下划线", EnumLogSource.用户操作日志, EnumLevel.WarningSpeech);
                return false;
            }
            //不允许重名
            if (DALManager.ApplicationDbDal.GetCount("schema_info", string.Format("schema_name ='{0}'", schemaName)) > 0)
            {
                LogManager.AddMessage("方案名无效,方案名已存在", EnumLogSource.用户操作日志, EnumLevel.WarningSpeech);
                return false;
            }
            return true;
        }
        /// 方案重命名
        /// <summary>
        /// 方案重命名
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameSchema()
        {
            if (!CheckSchemaName(newName))
            {
                return;
            }
            if (MessageBox.Show("确认对方案重命名?", "重命名", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string origionalName = SelectedSchema.GetProperty("SCHEMA_NAME") as string;
                DynamicModel model = new DynamicModel();
                model.SetProperty("SCHEMA_NAME", NewName);
                DALManager.ApplicationDbDal.Update("schema_info", string.Format("schema_name = '{0}'", origionalName), model, new List<string> { "SCHEMA_NAME" });
                LoadSchemas();

                DynamicViewModel modelTemp = EquipmentData.SchemaModels.Schemas.FirstOrDefault(item => item.GetProperty("SCHEMA_NAME") as string == origionalName);
                if (modelTemp != null)
                {
                    modelTemp.SetProperty("SCHEMA_NAME",NewName);
                }
            }
        }
        #region 添加方案
        private string newName;
        public string NewName
        {
            get { return newName; }
            set { SetPropertyValue(value, ref newName, "NewName"); }
        }
        private string schemaType;
        public string SchemaType
        {
            get { return schemaType; }
            set { SetPropertyValue(value, ref schemaType, "SchemaType"); }
        }

        /// <summary>
        /// 添加方案
        /// </summary>
        public void AddSchema()
        {
            if (!CheckSchemaName(NewName))
            {
                return;
            }
            if (string.IsNullOrEmpty(SchemaType))
            {
                LogManager.AddMessage("请选择新建的检定方案类型.", EnumLogSource.用户操作日志, EnumLevel.WarningSpeech);
                return;
            }
            DynamicModel model = new DynamicModel();
            model.SetProperty("SCHEMA_NAME", NewName);
            model.SetProperty("SCHEMA_TYPE", SchemaType);
            DALManager.ApplicationDbDal.Insert("schema_info", model);
            LoadSchemas();

            DynamicViewModel modelTemp = Schemas.FirstOrDefault(item => item.GetProperty("SCHEMA_NAME") as string == NewName);
            if (modelTemp != null)
            {
                EquipmentData.SchemaModels.Schemas.Add(modelTemp);
            }
        }
        #endregion
        #region 拷贝方案
        /// 拷贝方案
        /// <summary>
        /// 拷贝方案
        /// </summary>
        public void CopySchema()
        {
            if (!CheckSchemaName(NewName))
            {
                return;
            }
            #region 获取旧方案信息
            int oldId = (int)(SelectedSchema.GetProperty("ID"));
            List<DynamicModel> models = DALManager.ApplicationDbDal.GetList("SCHEMA_PARA_VALUE", string.Format("SCHEMA_ID={0}", oldId));
            #endregion
            #region 插入新方案导数据库
            int newId = 0;
            SelectedSchema.SetProperty("SCHEMA_NAME", NewName);
            DALManager.ApplicationDbDal.Insert("schema_info", SelectedSchema.GetDataSource());
            DynamicModel newModel = DALManager.ApplicationDbDal.GetByID("schema_info", string.Format("schema_name='{0}'",NewName));
            if (newModel != null)
            {
                newId = (int) (newModel.GetProperty("ID"));
                for (int i = 0; i < models.Count; i++)
                {
                    models[i].SetProperty("SCHEMA_ID", newId);
                    DALManager.ApplicationDbDal.Insert("SCHEMA_PARA_VALUE", models[i]);
                }
            }
            else
            {
                LogManager.AddMessage("方案拷贝失败!!!", EnumLogSource.用户操作日志, EnumLevel.ErrorSpeech);
            }
            #endregion
            LoadSchemas(); 
            
            DynamicViewModel modelTemp = Schemas.FirstOrDefault(item => item.GetProperty("SCHEMA_NAME") as string == NewName);
            if (modelTemp != null)
            {
                EquipmentData.SchemaModels.Schemas.Add(modelTemp);
            }
        }
        #endregion
        /// <summary>
        /// 删除方案
        /// </summary>
        public void DeleteSchema()
        {
            if (MessageBox.Show("方案很重要,请谨慎操作,确认要删除方案吗?", "删除方案", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                int oldId = (int)(SelectedSchema.GetProperty("ID"));
                DALManager.ApplicationDbDal.Delete("schema_info", string.Format("ID={0}", oldId));
                DALManager.ApplicationDbDal.Delete("schema_para_value", string.Format("schema_id={0}", oldId));
                LoadSchemas();

                DynamicViewModel modelTemp = EquipmentData.SchemaModels.Schemas.FirstOrDefault(item => (int)item.GetProperty("ID") == oldId);
                if (modelTemp != null)
                {
                    EquipmentData.SchemaModels.Schemas.Remove(modelTemp);
                }
            }
        }

        public void RefreshCurrrentSchema()
        {
            OnPropertyChanged("SelectedSchema");
        }
    }
}
