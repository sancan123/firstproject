using System.Collections.Generic;

namespace Mesurement.UiLayer.ViewModel.PortConfig
{
    /// <summary>
    /// 继电器类
    /// </summary>
    public class RelayGroup : ViewModelBase
    {
        private string id = "0";

        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        private string paraNo;

        public string ParaNo
        {
            get { return paraNo; }
            set { SetPropertyValue(value, ref paraNo, "ParaNo"); }
        }

        public void LoadRelayConfig(string stringRelay)
        {
            if (string.IsNullOrEmpty(stringRelay))
            {
                return;
            }
            string[] arrayRelay = stringRelay.Split('|');
            if (arrayRelay.Length > 10)
            {
                for (int i = 0; i < 10; i++)
                {
                    RelayModel.SetProperty(string.Format("继电器{0}", i + 1), arrayRelay[i + 1] == "闭合");
                }
            }
        }

        public RelayGroup()
        {
            for (int i = 0; i < 10; i++)
            {
                RelayModel.SetProperty(string.Format("继电器{0}", i + 1), false);
                RelayModel.PropertyChanged += RelayModel_PropertyChanged;
            }
        }

        private void RelayModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            FlagChanged = true;
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { SetPropertyValue(value, ref name, "Name"); }
        }

        private string groupId;
        /// <summary>
        /// 继电器组编号
        /// </summary>
        public string GroupId
        {
            get { return groupId; }
            set { SetPropertyValue(value, ref groupId, "GroupId"); }
        }

        private DynamicViewModel relayModel = new DynamicViewModel(1);

        public DynamicViewModel RelayModel
        {
            get { return relayModel; }
            set { SetPropertyValue(value, ref relayModel, "RelayModel"); }
        }

        private bool flagChanged;
        /// <summary>
        /// 修改标记
        /// </summary>
        public bool FlagChanged
        {
            get { return flagChanged; }
            set { SetPropertyValue(value, ref flagChanged, "FlagChanged"); }
        }
        public override string ToString()
        {
            List<string> listTemp = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                listTemp.Add(((bool)RelayModel.GetProperty(string.Format("继电器{0}", i + 1))) ? "闭合" : "断开");
            }
            return string.Format("{0}|{1}", GroupId, string.Join("|", listTemp));
        }
    }
}
