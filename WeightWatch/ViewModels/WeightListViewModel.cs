﻿using System;
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
            public WeightMinMax(float? min, float? max)
                : this()
            {
                Min = min;
                Max = max;
            }

            private float? _min;
            public float? Min
            {
                get
                {
                    return _min;
                }
                set
                {
                    if (value < 0)
                    {
                        throw new ArgumentException("Minimum weight cannot be less than 0");
                    }
                    if (value > _max)
                    {
                        throw new ArgumentException("Minimum weight cannot be greater than maximum weight");
                    }
                    _min = value;
                }
            }

            private float? _max;
            public float? Max
            {
                get
                {
                    return _max;
                }
                set
                {
                    if (value < _min)
                    {
                        throw new ArgumentException("Maximum weight cannot be less than minimum weight");
                    }
                    _max = value;
                }
            }
        }

        WeightListModel _dataList;
        ApplicationSettings _appSettings = new ApplicationSettings();
        static WeightListViewModel _instance = null;
        static readonly object _singletonLock = new object();

        private WeightListViewModel()
        {
            WeightHistoryList = new ObservableCollection<WeightViewModel>();

            _dataList = WeightListModel.GetInstance();
            _dataList.WeightList.ForEach(x => WeightHistoryList.Add(new WeightViewModel(x)));

            _appSettings.PropertyChanged += new PropertyChangedEventHandler(ApplicationSettings_PropertyChanged);
        }

        /// <summary>
        /// WeightHistoryModel object instantiated once and shared thereafter (singleton pattern).
        /// </summary>
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
            WeightHistoryList.Add(new WeightViewModel(_model));
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

        void ApplicationSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InvokePropertyChanged("WeightHistoryList");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void InvokePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}