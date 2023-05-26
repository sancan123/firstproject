using Mesurement.UiLayer.DAL;
using Mesurement.UiLayer.Utility;
using Mesurement.UiLayer.ViewModel.InputPara;
using Mesurement.UiLayer.ViewModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mesurement.UiLayer.ViewModel.ConfigMore
{
    /// <summary>
    /// 协议数据模型
    /// </summary>
    public class ProtocolViewModel : ViewModelBase
    {
        public ProtocolViewModel()
        {
            PagerModel.PageSize = 30;
            PagerModel.EventUpdateData += PagerModel_EventUpdateData;
            SearchProtocol();
        }

        #region 协议数据标识
        private AsyncObservableCollection<DynamicViewModel> dataItems = new AsyncObservableCollection<DynamicViewModel>();
        /// <summary>
        /// 数据标识
        /// </summary>
        public AsyncObservableCollection<DynamicViewModel> DataItems
        {
            get { return dataItems; }
            set { SetPropertyValue(value, ref dataItems, "DataItems"); }
        }
        #endregion
        #region 分页控件
        /// <summary>
        /// 分页控件查找事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PagerModel_EventUpdateData(object sender, EventArgs e)
        {
            UpdateProtocol();
        }

        private DataPagerViewModel pagerModel = new DataPagerViewModel();
        /// <summary>
        /// 分页控件数据模型
        /// </summary>
        public DataPagerViewModel PagerModel
        {
            get { return pagerModel; }
            set { SetPropertyValue(value, ref pagerModel, "PagerModel"); }
        }
        #endregion
        #region 查询条件
        /// <summary>
        /// 查询条件
        /// </summary>
        private string Where
        {
            get { return string.Empty; }
        }
        #endregion

        #region 执行查询
        /// <summary>
        /// 更新表信息
        /// </summary>
        private void UpdateProtocol()
        {
            TaskManager.AddDataBaseAction(() =>
            {
                DataItems.Clear();
                string whereTemp = Where;
                List<DynamicModel> models = DALManager.ProtocolDal.GetPage("PROTOCOL_DLT645DICT", "PK_LNG_DLT_ID", PagerModel.PageSize, PagerModel.PageIndex, whereTemp, false);
                for (int j = 0; j < models.Count; j++)
                {
                    DataItems.Add(new DynamicViewModel(models[j], j + 1));
                }
            });
        }
        /// <summary>
        /// 查询数据标识
        /// </summary>
        public void SearchProtocol()
        {
            PagerModel.Total = DALManager.ProtocolDal.GetCount("PROTOCOL_DLT645DICT", Where);
        }
        #endregion
    }
}
