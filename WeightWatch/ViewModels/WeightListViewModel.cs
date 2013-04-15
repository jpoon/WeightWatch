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
    using System.Globalization;
    using System.Linq;
    using WeightWatch.Classes;
    using WeightWatch.MeasurementSystem;
    using WeightWatch.Models;

    public class WeightListViewModel : INotifyPropertyChanged
    {
        private static WeightListModel _dataList;
        private static ObservableCollection<WeightViewModel> _weightHistoryList;
        private static bool _invalidateCache;

        public WeightListViewModel()
        {
            IsoStorage.WeightListDataModelPropertyChanged = (sender, args) =>
            {
                _invalidateCache = true;
            };

            if (_dataList == null || _invalidateCache)
            {
                _dataList = new WeightListModel();
                _dataList.CollectionChanged += DataListCollectionChanged;

                _invalidateCache = false;
            }

            _weightHistoryList = new ObservableCollection<WeightViewModel>();
            foreach (var item in _dataList.WeightList)
            {
                _weightHistoryList.Add(new WeightViewModel(item));
            }
        }

        #region Properties

        public ObservableCollection<WeightViewModel> WeightHistoryList
        {
            get 
            { 
                return _weightHistoryList;
            }
        }

        public ObservableCollection<WeightListGroup<WeightViewModel>> WeightHistoryGroup
        {
            get
            {
                var results = from item in _weightHistoryList
                              group item by item.DateStrMonthYear
                              into g
                              select new WeightListGroup<WeightViewModel>(g.Key, g);

                var weightHistoryGroup = new ObservableCollection<WeightListGroup<WeightViewModel>>();
                foreach (var result in results)
                {
                    weightHistoryGroup.Add(result);
                }

                return weightHistoryGroup;
            }
        }

        #endregion Properties

        #region Public Methods

        public void Delete(WeightViewModel data)
        {
            if (data == null || data.WeightModel == null)
            {
                throw new ArgumentNullException("data");
            }

            _dataList.Delete(data.WeightModel);
        }

        public WeightViewModel Get(DateTime date)
        {
            return new WeightViewModel(_dataList.Get(date));
        }

        public IEnumerable<WeightViewModel> Get(DateTime start, DateTime end)
        {
            return  from item in WeightHistoryList
                    where   item.Date >= start && 
                            item.Date <= end
                    select item;
        }

        public void Save(string weightStr, DateTime? date, MeasurementUnit unit)
        {
            if (date == null || !date.HasValue)
            {
                throw new ArgumentException("Please enter a non-empty date");
            }

            if (String.IsNullOrEmpty(weightStr))
            {
                throw new ArgumentException("Please enter a non-empty weight");
            }

            double weight;
            if (!Double.TryParse(weightStr, NumberStyles.Number, CultureInfo.CurrentCulture, out weight))
            {
                throw new ArgumentException("Please enter a valid weight");
            }

            var model = new WeightModel(weight, date.Value, unit);
            _dataList.Add(model);
        }

        #endregion

        #region Event Handlers

        public event PropertyChangedEventHandler PropertyChanged;
        private void InvokePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DataListCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    _weightHistoryList.Insert(e.NewStartingIndex, new WeightViewModel(e.NewItems[0] as WeightModel));
                    break;
                case NotifyCollectionChangedAction.Replace:
                    _weightHistoryList[e.NewStartingIndex] = new WeightViewModel(e.NewItems[0] as WeightModel);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    _weightHistoryList.RemoveAt(e.OldStartingIndex);
                    break;
            }
            InvokePropertyChanged("WeightHistoryGroup");
            InvokePropertyChanged("WeightHistoryList");
        }

        #endregion
    }
}