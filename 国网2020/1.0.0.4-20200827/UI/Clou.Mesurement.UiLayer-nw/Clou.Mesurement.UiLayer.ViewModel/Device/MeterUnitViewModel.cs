namespace Mesurement.UiLayer.ViewModel.Device
{
    /// 表状态数据模型
    /// <summary>
    /// 表状态数据模型
    /// </summary>
    public class MeterUnitViewModel : ViewModelBase
    {
        private bool isSelected;
        /// 选中标记,用来控制表位压接等等操作
        /// <summary>
        /// 选中标记,用来控制表位压接等等操作
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set { SetPropertyValue(value, ref isSelected, "IsSelected"); }
        }

        #region 表的位置相关变量
        private bool haveMeter;
        /// 表位有表
        /// <summary>
        /// 表位有表
        /// </summary>
        public bool HaveMeter
        {
            get { return haveMeter; }
            set { SetPropertyValue(value, ref haveMeter, "HaveMeter"); }
        }
        private bool isTop;
        /// 电机在上位
        /// <summary>
        /// 电机在上位
        /// </summary>
        public bool IsTop
        {
            get { return isTop; }
            set { SetPropertyValue(value, ref isTop, "IsTop"); }
        }
        private bool isBottom;
        /// 电机在下位
        /// <summary>
        /// 电机在下位
        /// </summary>
        public bool IsBottom
        {
            get { return isBottom; }
            set { SetPropertyValue(value, ref isBottom, "IsBottom"); }
        }
        private bool errorPosition;
        /// 表位置错误
        /// <summary>
        /// 表位置错误
        /// </summary>
        public bool ErrorPosition
        {
            get { return errorPosition; }
            set { SetPropertyValue(value, ref errorPosition, "ErrorPosition"); }
        }
        #endregion

        private bool isOverFlow;
        /// <summary>
        /// 误差超出范围
        /// </summary>
        public bool IsOverFlow
        {
            get { return isOverFlow; }
            set { SetPropertyValue(value, ref isOverFlow, "IsOverFlow"); }
        }

        private string textScreen;
        /// 误差板显示文本
        /// <summary>
        /// 误差板显示文本
        /// </summary>
        public string TextScreen
        {
            get { return textScreen; }
            set { SetPropertyValue(value, ref textScreen, "TextScreen"); }
        }
        /// 将误差板读回的表位状态进行转换
        /// <summary>
        /// 将误差板读回的表位状态进行转换
        /// </summary>
        /// <param name="stringMeterUnit"></param>
        public void ConvertUnitStatus(string stringMeterUnit)
        {
            if (stringMeterUnit == "222")
            {
                ErrorPosition = false;
            }
            else if(stringMeterUnit.Length==3)
            {
                #region 解析表的上下位
                if (stringMeterUnit[0]=='0')
                {
                    IsTop=true;
                }
                else if(stringMeterUnit[0]=='1')
                {
                    IsTop=false;
                }
                if (stringMeterUnit[1] == '0')
                {
                    IsBottom = true;
                }
                else if (stringMeterUnit[1] == '1')
                {
                    IsBottom = false;
                }

                HaveMeter = stringMeterUnit[2] == '1' ? true : false;
                #endregion
            }
        }
    }
}