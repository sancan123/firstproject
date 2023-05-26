namespace Clou.Mesurement.UiLayer.ViewModel.Code
{
    /// 单个配置数据模型
    /// <summary>
    /// 单个配置数据模型
    /// </summary>
    public class CodeUnit : ViewModelBase
    {
        public int ID { get; set; }
        private string codeName;
        /// <summary>
        /// 配置名称
        /// </summary>
        public string CodeName
        {
            get { return codeName; }
            set
            {
                SetPropertyValue(value, ref codeName, "CodeName");
                ChangeFlag = true;
            }
        }
        private string codeValue;
        /// <summary>
        /// 配置的值
        /// </summary>
        public string CodeValue
        {
            get { return codeValue; }
            set
            {
                SetPropertyValue(value, ref codeValue, "CodeValue");
                ChangeFlag = true;
            }
        }
        /// <summary>
        /// 类别
        /// </summary>
        public string Category { get; set; }
        private bool changeFlag;

        public bool ChangeFlag
        {
            get { return changeFlag; }
            set { SetPropertyValue(value, ref changeFlag, "ChangeFlag"); }
        }
        private bool isValid = true;

        public bool IsValid
        {
            get { return isValid; }
            set
            {
                SetPropertyValue(value, ref isValid, "IsValid");
                ChangeFlag = true;
            }
        }

    }
}
