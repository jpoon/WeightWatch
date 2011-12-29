﻿/*
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
    using System.Text;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using WeightWatch.Classes;

    public class WeightListModel : INotifyCollectionChanged
    {
        const string CsvDelimiter = ",";

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

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #region Public Methods

        public void Add(WeightModel data)
        {
            var index = WeightList.BinarySearch(data);
            if (index >= 0)
            {
                var oldItem = WeightList[index];
                WeightList[index] = data;
                NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, data, oldItem, index));
            }
            else
            {
                WeightList.Add(data);
                WeightList.Sort();
                index = WeightList.BinarySearch(data);
                NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, data, index));
            }

            Save();
        }

        public WeightModel Get(DateTime date)
        {
            var data = new WeightModel(0, date, MeasurementSystem.Imperial);
            var index = WeightList.BinarySearch(data);
            return index >= 0 ? WeightList[index] : null;
        }

        public void Delete(WeightModel data)
        {
            var index = WeightList.BinarySearch(data);
            if (index >= 0)
            {
                WeightList.RemoveAt(index);
                WeightList.Sort();
                NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, data, index));
            }

            Save();
        }

        public String Export()
        {
            var csv = new StringBuilder();

            csv.AppendFormat("Date{0}Weight{0}Unit{0}",
                CsvDelimiter);
            csv.AppendLine();

            foreach (var model in WeightList)
            {
                csv.AppendFormat("{1}{0}{2}{0}{3}",
                    CsvDelimiter,
                    model.Date.ToShortDateString(),
                    model.Weight,
                    MeasurementFactory.GetSystem(model.MeasurementUnit).Abbreviation);
                csv.AppendLine();
            }
            return csv.ToString();
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
            IsoStorage.Save(WeightList);
        }
    }
}
