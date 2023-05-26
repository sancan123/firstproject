using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models;

namespace WpfApp1.ViewModel
{
    public class MainWindowViewModels: NotificationObject
    {
        #region 数据属性
        private double input1;

        public double Input1
        {
            get { return input1; }
            set
            {
                input1 = value;
                this.RasiePropertyChanged("Input1");
            }
        }
        private double input2;
        public double Input2
        {
            get { return input2; }
            set
            {
                input2 = value;
                this.RasiePropertyChanged("Input2");
            }

        }
        private double result;
        public double Result
        {
            get { return result; }
            set
            {
                result = value;
                this.RasiePropertyChanged("Result");
            }
        }
        #endregion
        /// <summary>
        /// 创建加法命令属性(本质委托)
        /// </summary>
        public DelegateCommand AddCommand { get; set; }
        private void Add(object parameter)
        {
            this.Result = this.Input2 + this.Input1;
        }

        public MainWindowViewModels()
        {
            this.AddCommand = new DelegateCommand();
            this.AddCommand.ExecuteAction = new Action<object>(this.Add);
        }


    }
}
