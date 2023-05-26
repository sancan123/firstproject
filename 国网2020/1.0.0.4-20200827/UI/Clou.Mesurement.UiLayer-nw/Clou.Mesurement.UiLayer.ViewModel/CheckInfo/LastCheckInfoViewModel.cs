using Mesurement.UiLayer.DAL;
using System.Collections.Generic;

namespace Mesurement.UiLayer.ViewModel.CheckInfo
{
    /// 程序退出时的状态
    /// 在程序开始运行时加载状态
    /// <summary>
    /// 程序退出时的状态
    /// 在程序开始运行时加载状态
    /// </summary>
    public class LastCheckInfoViewModel : ViewModelBase
    {
        private string equipmentNo;
        /// 台体编号
        /// <summary>
        /// 台体编号
        /// </summary>
        public string EquipmentNo
        {
            get { return equipmentNo; }
            set 
            { 
                SetPropertyValue(value, ref equipmentNo, "EquipmentNo"); 
            }
        }
        private int schemaId = 1;
        /// 方案编号
        /// <summary>
        /// 方案编号
        /// </summary>
        public int SchemaId
        {
            get { return schemaId; }
            set
            {
                if (value != schemaId)
                {
                    SetPropertyValue(value, ref schemaId, "SchemaId");
                    SaveCurrentCheckInfo();
                }
            }
        }
        private int checkIndex = -1;
        /// 检定点序号
        /// <summary>
        /// 检定点序号
        /// </summary>
        public int CheckIndex
        {
            get { return checkIndex; }
            set
            {
                if (value != checkIndex)
                {
                    SetPropertyValue(value, ref checkIndex, "CheckIndex");
                    SaveCurrentCheckInfo();
                }
            }
        }
        private string protectedVoltage;
        /// 保护电压
        /// <summary>
        /// 保护电压
        /// </summary>
        public string ProtectedVoltage
        {
            get { return protectedVoltage; }
            set
            {
                if (value != protectedVoltage)
                {
                    SetPropertyValue(value, ref protectedVoltage, "ProtectedVoltage");
                    SaveCurrentCheckInfo();
                }
            }
        }
        private string protectedCurrent;
        /// 保护电流
        /// <summary>
        /// 保护电流
        /// </summary>
        public string ProtectedCurrent
        {
            get { return protectedCurrent; }
            set
            {
                if (value != protectedCurrent)
                {
                    SetPropertyValue(value, ref protectedCurrent, "ProtectedCurrent");
                    SaveCurrentCheckInfo();
                }
            }
        }
        private string testPerson;
        /// 检验员
        /// <summary>
        /// 检验员
        /// </summary>
        public string TestPerson
        {
            get
            {
                if(User.UserViewModel.Instance.CurrentUser==null)
                {
                    return testPerson;
                }
                return User.UserViewModel.Instance.CurrentUser.GetProperty("chrUserName") as string;
            }
        }
        private string auditPerson;
        /// 核验员
        /// <summary>
        /// 核验员
        /// </summary>
        public string AuditPerson
        {
            get { return auditPerson; }
            set
            {
                if (value != auditPerson)
                {
                    SetPropertyValue(value, ref auditPerson, "AuditPerson");
                    SaveCurrentCheckInfo();
                }
            }
        }
        /// 加载数据库存储的最后一次检定信息
        /// <summary>
        /// 加载数据库存储的最后一次检定信息
        /// </summary>
        public void LoadLastCheckInfo()
        {
            List<DynamicModel> models = DALManager.ApplicationDbDal.GetList(EnumAppDbTable.LAST_STATE_INFO.ToString(), string.Format("AVR_DEVICE_ID='{0}'", EquipmentData.Equipment.ID));
            if (models == null || models.Count == 0)
            {
                checkIndex = -1;
                schemaId = 1;
                protectedCurrent = "80A";
                protectedVoltage = "264V";
                auditPerson = "Admin";
                testPerson = "Admin";
                equipmentNo = "001";
                SaveCurrentCheckInfo();
            }
            else
            {
                DynamicModel model = models[0];
                int.TryParse(model.GetProperty("AVR_CHECK_INDEX") as string, out checkIndex);
                int.TryParse(model.GetProperty("AVR_SCHEMA_ID") as string, out schemaId);
                auditPerson = model.GetProperty("AVR_AUDIT_PERSON") as string;
                testPerson = model.GetProperty("AVR_TEST_PERSON") as string;
                protectedCurrent = model.GetProperty("AVR_PROTECT_CURRENT") as string;
                protectedVoltage = model.GetProperty("AVR_PROTECT_VOLTAGE") as string;
                equipmentNo = model.GetProperty("AVR_DEVICE_ID") as string;
            }
        }
        /// 保存当前检定信息
        /// <summary>
        /// 保存当前检定信息
        /// </summary>
        public void SaveCurrentCheckInfo()
        {
            DynamicModel model = new DynamicModel();
            model.SetProperty("AVR_DEVICE_ID", EquipmentData.Equipment.ID);
            model.SetProperty("AVR_CHECK_INDEX", EquipmentData.Controller.Index);
            model.SetProperty("AVR_SCHEMA_ID", EquipmentData.Schema.SchemaId);
            model.SetProperty("AVR_AUDIT_PERSON", AuditPerson);
            model.SetProperty("AVR_TEST_PERSON", TestPerson);
            model.SetProperty("AVR_PROTECT_CURRENT", ProtectedCurrent);
            model.SetProperty("AVR_PROTECT_VOLTAGE", ProtectedVoltage);
            if (DALManager.ApplicationDbDal.GetCount(EnumAppDbTable.LAST_STATE_INFO.ToString(), string.Format("AVR_DEVICE_ID='{0}'", equipmentNo)) == 0)
            {
                DALManager.ApplicationDbDal.Insert(EnumAppDbTable.LAST_STATE_INFO.ToString(), model);
            }
            else
            {
                DALManager.ApplicationDbDal.Update(EnumAppDbTable.LAST_STATE_INFO.ToString(), string.Format("AVR_DEVICE_ID='{0}'", equipmentNo), model, new List<string> { "AVR_CHECK_INDEX", "AVR_SCHEMA_ID", "AVR_PROTECT_VOLTAGE", "AVR_PROTECT_CURRENT", "AVR_AUDIT_PERSON", "AVR_TEST_PERSON" });
            }
        }
    }
}
