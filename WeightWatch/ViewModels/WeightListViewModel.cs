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
        public struct WeightMinMax
        {
            static float DEFAULT_MIN = 0;
            static float DEFAULT_MAX = 100;

            public WeightMinMax(float? min, float? max)
                : this()
            {
                Min = min;
                Max = max;
            }

            private float _min;
            public float? Min
            {
                get
                {
                    return _min;
                }

                private set
                {
                    if (value == null)
                    {
                        _min = DEFAULT_MIN;
                    }
                    else
                    {
                        _min = (float)value;
                    }
                }
            }

            private float _max;
            public float? Max
            {
                get
                {
                    return _max;
                }

                private set
                {
                    if (value == null)
                    {
                        _max = DEFAULT_MAX;
                    }
                    else
                    {
                        _max = (float)value;
                    }
                }
            }
        }

        WeightListModel _dataList;

        public WeightListViewModel()
        {
            WeightHistoryList = new ObservableCollection<WeightViewModel>();

            _dataList = WeightListModel.GetInstance();
            _dataList.WeightList.ForEach(x => WeightHistoryList.Add(new WeightViewModel(x)));
            _dataList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_dataList_CollectionChanged);
        }

        #region properties

        public ObservableCollection<WeightViewModel> WeightHistoryList
        {
            get;
            private set;
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
            WeightListModel.GetInstance().Add(_model);
            InvokePropertyChanged("WeightHistoryList");
            InvokePropertyChanged("WeightHistoryGroup");
        }

        public WeightMinMax GetMinMaxWeight(DateTime start, DateTime end)
        {
            var resultSet =
                from item in WeightHistoryList
                where item.Date >= start && item.Date <= end
                select item;

            float? min =
                (from item in resultSet
                 select (float?)item.Weight).Min();

            float? max =
                (from item in resultSet
                 select (float?)item.Weight).Max();

            return new WeightMinMax(min, max);
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