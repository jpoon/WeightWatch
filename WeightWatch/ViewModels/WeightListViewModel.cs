/*
 * Copyright (C) 2011 by Jason Poon
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

namespace WeightWatch.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using WeightWatch.Classes;
    using WeightWatch.Models;

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
        static WeightListViewModel _instance = null;
        static readonly object _singletonLock = new object();

        private WeightListViewModel()
        {
            WeightHistoryList = new ObservableCollection<WeightViewModel>();

            _dataList = WeightListModel.GetInstance();
            _dataList.WeightList.ForEach(x => WeightHistoryList.Add(new WeightViewModel(x)));
            _dataList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_dataList_CollectionChanged);
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

        #region Properties

        public ObservableCollection<WeightViewModel> WeightHistoryList
        {
            get;
            private set;
        }

        public ObservableCollection<WeightListGroup<WeightViewModel>> WeightHistoryGroup
        {
            get
            {
                var results = from item in WeightHistoryList
                              group item by item.DateStr_MonthYear into g
                              select new WeightListGroup<WeightViewModel>(g.Key, g);

                ObservableCollection<WeightListGroup<WeightViewModel>> weightListGroup = new ObservableCollection<WeightListGroup<WeightViewModel>>();
                foreach (var result in results)
                {
                    weightListGroup.Add(result);
                }
                return weightListGroup;
            }
        }

        public WeightViewModel FirstWeightEntry
        {
            get
            {
                if (WeightHistoryList.Count > 0)
                {
                    return WeightHistoryList.LastOrDefault();
                }
                else
                {
                    return null;
                }
            }
        }

        public WeightViewModel LastWeightEntry
        {
            get
            {
                if (WeightHistoryList.Count > 0)
                {
                    return WeightHistoryList.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion Properties

        #region Public Methods

        public static void Delete(WeightViewModel data)
        {
            if (data == null || data.weightModel == null)
            {
                throw new ArgumentNullException("WeightViewModel");
            }

            WeightListModel.GetInstance().Delete(data.weightModel);
        }

        public static void Save(Decimal weight, DateTime date, MeasurementSystem unit)
        {
            WeightModel _model = new WeightModel(weight, date, unit);
            WeightListModel.GetInstance().Add(_model);
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void InvokePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void _dataList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    WeightHistoryList.Insert(e.NewStartingIndex, new WeightViewModel(e.NewItems[0] as WeightModel));
                    break;
                case NotifyCollectionChangedAction.Replace:
                    WeightHistoryList[e.NewStartingIndex] = new WeightViewModel(e.NewItems[0] as WeightModel);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    WeightHistoryList.RemoveAt(e.OldStartingIndex);
                    break;
            }
            InvokePropertyChanged("WeightHistoryGroup");
            InvokePropertyChanged("FirstWeightEntry");
            InvokePropertyChanged("LastWeightEntry");
        }

        #endregion
    }
}