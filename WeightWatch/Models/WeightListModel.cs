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
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Runtime.Serialization;
    using WeightWatch.Classes;
    using System;

    public class WeightListModel : INotifyCollectionChanged
    {
        private static List<WeightModel> _weightList;
        public List<WeightModel> WeightList
        {
            get { return _weightList; }
        }

        public WeightListModel()
        {
            if (_weightList == null)
            {
                _weightList = IsoStorage.LoadFile();
            }
        }

        #region Public Methods

        public void Add(WeightModel data)
        {
            int index = WeightList.BinarySearch(data);
            if (index >= 0)
            {
                WeightModel oldItem = WeightList[index];
                WeightList.RemoveAt(index);

                WeightList.Add(data);
                WeightList.Sort();
                index = WeightList.BinarySearch(data);
                notifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, data, oldItem, index));
            }
            else
            {
                WeightList.Add(data);
                WeightList.Sort();
                index = WeightList.BinarySearch(data);
                notifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, data, index));
            }
        }

        public WeightModel Get(DateTime date)
        {
            WeightModel data = new WeightModel(0, date, MeasurementSystem.Imperial);
            int index = WeightList.BinarySearch(data);
            if (index >= 0)
            {
                return WeightList[index];
            }
            else
            {
                return null;
            }
        }

        public void Delete(WeightModel data)
        {
            int index = WeightList.BinarySearch(data);
            if (index >= 0)
            {
                WeightList.RemoveAt(index);
                WeightList.Sort();
                notifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, data, index));
            }
        }

        public void Save()
        {
            IsoStorage.Save(this.WeightList);
        }

        #endregion


        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private void notifyCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, args);
            }
        }
    }
}
