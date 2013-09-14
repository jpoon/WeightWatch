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

namespace WeightWatch.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using WeightWatch.Classes;

    public class WeightListModel : INotifyCollectionChanged
    {
        private readonly List<WeightModel> _weightList;

        public WeightListModel()
        {
            _weightList = IsoStorage.Get();
        }

        public ReadOnlyCollection<WeightModel> WeightList
        {
            get
            {
                return new ReadOnlyCollection<WeightModel>(_weightList);
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #region Public Methods

        public void Add(WeightModel data)
        {
            var index = _weightList.IndexOf(this.Get(data.Date));
            if (index >= 0)
            {
                var oldItem = WeightList[index];
                _weightList[index] = data;
                NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, data, oldItem, index));
            }
            else
            {
                _weightList.Add(data);
                _weightList.Sort();
                index = _weightList.IndexOf(data);
                NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, data, index));
            }

            Save();
        }

        public WeightModel Get(DateTime date)
        {
            return _weightList.FirstOrDefault(c => c.Date.Equals(date));
        }

        public void Delete(DateTime date)
        {
            var model = this.Get(date);
            var index = _weightList.IndexOf(model);
            if (index >= 0)
            {
                _weightList.RemoveAt(index);
                NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, model, index));
            }

            Save();
        }

        #endregion

        private void NotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, args);
            }
        }

        private void Save()
        {
            _weightList.Sort();
            IsoStorage.Save(_weightList);
        }
    }
}
