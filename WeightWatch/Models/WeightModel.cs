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
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.Serialization;

    [DataContract(Name = "WeightModel")]
    public class WeightModel : IComparable<WeightModel>, INotifyPropertyChanged
    {
        private double _weight;
        private DateTime _date;
        private MeasurementSystem _measurementSystem;

        public event PropertyChangedEventHandler PropertyChanged;

        [DataMember]
        public double Weight
        {
            get
            {
                return _weight;
            }

            set
            {
                _weight = value;
                OnPropertyChanged("Weight");
            }
        }

        [DataMember]
        public DateTime Date
        {
            get
            {
                return _date;
            }

            set
            {
                _date = value;
                OnPropertyChanged("Date");
            }
        }

        [DataMember]
        public MeasurementSystem MeasurementUnit
        {
            get
            {
                return _measurementSystem;
            }

            set
            {
                _measurementSystem = value;
                OnPropertyChanged("MeasurementSystem");
            }
        }

        public WeightModel(string weightStr, DateTime date, MeasurementSystem unit)
        {
            if (date == null)
            {
                throw new ArgumentException("Please enter a valid date");
            }

            if (String.IsNullOrEmpty(weightStr))
            {
                throw new ArgumentException("Please enter a valid weight");
            }

            double weight;
            if (!Double.TryParse(weightStr, NumberStyles.Number, CultureInfo.CurrentCulture, out weight))
            {
                throw new ArgumentException("Please enter a valid weight");
            }
            else if (weight > 9999)
            {
                throw new ArgumentException("Please enter a valid weight");
            }

            Weight = weight;
            Date = date;
            MeasurementUnit = unit;
        }

        public int CompareTo(WeightModel other)
        {
            if (other != null)
            {
                // reverse chronological order
                return -(Date.CompareTo(other.Date));
            }
            return 0;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }
}
