using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using WeightWatch.Classes;
using WeightWatch.Models;

namespace WeightWatch
{
    public class WeightListViewModel : INotifyPropertyChanged
    {
        WeightListModel _dataList;

        static WeightListViewModel _instance = null;
        static readonly object _singletonLock = new object();

        private WeightListViewModel()
        {
            WeightHistoryList = new ObservableCollection<WeightViewModel>();

            _dataList = WeightListModel.GetInstance();
            _dataList.WeightList.ForEach(x => WeightHistoryList.Add(new WeightViewModel(x)));
            _dataList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_dataList_CollectionChanged);
        }

        public static WeightListViewModel GetInstance()
        {
            lock (_singletonLock)
            {
                if (_instance == null)
                {
                    _instance = new WeightListViewModel();
                }
                return _instance;
            }
        }

        #region properties

        public ObservableCollection<WeightViewModel> WeightHistoryList
        {
            get;
            set;
        }

        public IEnumerable<LongListSelectorGroup<WeightViewModel>> WeightHistoryGroup
        {
            get
            {
                return from item in WeightHistoryList
                       group item by item.DateStr_MonthYear into g
                       select new LongListSelectorGroup<WeightViewModel>(g.Key, g);
            }
        }

        #endregion properties

        #region Public Methods

        public void Save(float weight, DateTime date, MeasurementSystem unit)
        {
            WeightModel _model = new WeightModel(weight, date, unit);
            WeightListModel.GetInstance().add(_model);
            InvokePropertyChanged("WeightHistoryList");
            InvokePropertyChanged("WeightHistoryGroup");
        }

        #endregion

        #region Event Handlers

        void _dataList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            WeightHistoryList.Clear();
            _dataList.WeightList.ForEach(x => WeightHistoryList.Add(new WeightViewModel(x)));
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void InvokePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}