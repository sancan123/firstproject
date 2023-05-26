using Mesurement.UiLayer.DAL;
using Mesurement.UiLayer.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mesurement.UiLayer.ViewModel.ErrorLimit
{
    /// <summary>
    /// 误差限单元格
    /// </summary>
    public class ErrorLimitCell:ViewModelBase
    {
        private string limitValue;
        /// <summary>
        /// 误差限值
        /// </summary>
        public string LimitValue
        {
            get { return limitValue; }
            set
            {
                SetPropertyValue(value, ref limitValue, "LimitValue"); 
            }
        }
        private bool changeFlag=false;
        /// <summary>
        /// 修改标记
        /// </summary>
        public bool ChangeFlag
        {
            get { return changeFlag; }
            set { SetPropertyValue(value, ref changeFlag, "ChangeFlag"); }
        }

        protected internal override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == "LimitValue")
            {
                ChangeFlag = true;
            }
        }

        private bool flagNoValue=false;
        /// <summary>
        /// 数据库中没有相关的值
        /// </summary>
        public bool FlagNoValue
        {
            get { return flagNoValue; }
            set { SetPropertyValue(value, ref flagNoValue, "FlagNoValue"); }
        }
        
    }
}
