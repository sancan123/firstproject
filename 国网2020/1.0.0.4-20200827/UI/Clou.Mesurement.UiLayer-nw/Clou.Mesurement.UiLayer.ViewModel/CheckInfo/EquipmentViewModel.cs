using Mesurement.UiLayer.DAL.Config;

namespace Mesurement.UiLayer.ViewModel.CheckInfo
{
    /// 台体信息视图
    /// <summary>
    /// 台体信息视图
    /// </summary>
    public class EquipmentViewModel : ViewModelBase
    {
        private string equipmentType = ConfigHelper.Instance.EquipmentType;
        /// 台体类型
        /// <summary>
        /// 台体类型
        /// </summary>
        public string EquipmentType
        {
            get
            {
                return equipmentType; 
            }
            set
            {
                SetPropertyValue(value, ref equipmentType, "EquipmentType");
            }
        }

        private int meterCount = ConfigHelper.Instance.MeterCount;
        /// 表位数量
        /// <summary>
        /// 表位数量
        /// </summary>
        public int MeterCount
        {
            get { return meterCount; }
            set { SetPropertyValue(value, ref meterCount, "MeterCount"); }
        }

        /// <summary>
        /// 南网设备厂家
        /// </summary>
        private string southManufacturers = ConfigHelper.Instance.SouthManufacturers;
        public string SouthManufacturers
        {
            get { return southManufacturers; }
            set { SetPropertyValue(value, ref southManufacturers, "SouthManufacturers"); }
        }


        private string id = ConfigHelper.Instance.EquipmentNo;
        /// 台体编号
        /// <summary>
        /// 台体编号
        /// </summary>
        public string ID
        {
            get { return id; }
            set { SetPropertyValue(value, ref id, "ID"); }
        }

        private string textLogin;
        /// <summary>
        /// 登录时显示的文本
        /// </summary>
        public string TextLogin
        {
            get { return textLogin; }
            set { SetPropertyValue(value, ref textLogin, "TextLogin"); }
        }

    }
}
