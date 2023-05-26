﻿using Mesurement.UiLayer.DAL.DataBaseView;
using Mesurement.UiLayer.DataManager.ViewModel.Mark;
using Mesurement.UiLayer.ViewModel;
using Mesurement.UiLayer.ViewModel.Model;
using System;
using System.Threading;

namespace Mesurement.UiLayer.DataManager.Mark.ViewModel
{
    /// <summary>
    /// 台体信息书签制作器
    /// </summary>
    class EquipmentBookmarkMaker : ViewModelBase
    {
        public EquipmentBookmarkMaker()
        {
            new Thread(() =>
            {
                TableDisplayModel displayModel = ResultViewHelper.GetTableDisplayModel("41",true);
                if (displayModel != null)
                {
                    for (int i = 0; i < displayModel.ColumnModelList.Count; i++)
                    {
                        string[] arrayDisplayName = displayModel.ColumnModelList[i].DisplayName.Split('|');
                        for (int j = 0; j < arrayDisplayName.Length; j++)
                        {
                            if (!string.IsNullOrEmpty(arrayDisplayName[j]))
                            {
                                resultNames.Add(arrayDisplayName[j].Trim());
                            }
                        }
                    }
                }
                OnPropertyChanged("ResultNames");
            }).Start();
        }

        #region 结论列表
        private string resultName;
        /// <summary>
        /// 结论名称
        /// </summary>
        public string ResultName
        {
            get { return resultName; }
            set
            {
                SetPropertyValue(value, ref resultName, "ResultName");
                OnPropertyChanged("BookmarkName");
                OnPropertyChanged("EnableAdd");
            }
        }

        private AsyncObservableCollection<string> resultNames = new AsyncObservableCollection<string>();
        /// <summary>
        /// 结论列表
        /// </summary>
        public AsyncObservableCollection<string> ResultNames
        {
            get
            {
                return resultNames;
            }
            set { SetPropertyValue(value, ref resultNames, "ResultNames"); }
        }
        #endregion
        private string bookmarkName;
        /// <summary>
        /// 书签名称
        /// </summary>
        public string BookmarkName
        {
            get
            {
                if (string.IsNullOrEmpty(ResultName))
                {
                    bookmarkName = "";
                }
                else
                {
                    bookmarkName = string.Format("EquipmentInfoVX{0}VX{1}",  ResultName,Format);
                }
                return bookmarkName;
            }
        }
        private EnumFormat format = EnumFormat.无;

        public EnumFormat Format
        {
            get { return format; }
            set
            {
                SetPropertyValue(value, ref format, "Format");
                OnPropertyChanged("BookmarkName");
                OnPropertyChanged("EnableAdd");
            }
        }
        /// <summary>
        /// 允许添加书签
        /// </summary>
        public bool EnableAdd
        {
            get
            {
                return !string.IsNullOrEmpty(bookmarkName);
            }
        }

        public event EventHandler EventAddBookmark;
        /// <summary>
        /// 添加书签
        /// </summary>
        public void AddBookmark()
        {
            if (EventAddBookmark != null)
            {
                EventAddBookmark(BookmarkName, null);
            }
        }
    }
}
