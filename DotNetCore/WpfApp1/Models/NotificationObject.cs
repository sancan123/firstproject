using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class NotificationObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RasiePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                //当事件处理程序不为null时，本身作为事件源，然后发送事件数据，通知属性已经发生了改变
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
