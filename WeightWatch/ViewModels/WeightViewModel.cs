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
    using System.Globalization;
    using WeightWatch.Models;

    public class WeightViewModel
    {
        WeightModel _data;

        public WeightViewModel(WeightModel data)
        {
            _data = data;
        }

        #region Properties

        public string DateStr
        {
            get
            {
                return _data.Date.ToString("MMM. d, yyyy");
            }
        }

        public string DateStr_MonthYear
        {
            get
            {
                return _data.Date.ToString("MMM yyyy");
            }
        }

        public DateTime Date
        {
            get
            {
                return _data.Date;
            }
        }

        public string WeightStr
        {
            get
            {
                MeasurementSystem defaultSystem = ApplicationSettings.DefaultMeasurementSystem;
                return this.Weight.ToString("0.##", CultureInfo.InvariantCulture) + " " + MeasurementFactory.GetSystem(defaultSystem).Abbreviation;
            }
        }

        public Decimal Weight
        {
            get
            {
                MeasurementSystem defaultSystem = ApplicationSettings.DefaultMeasurementSystem;
                return MeasurementFactory.GetSystem(_data.MeasurementUnit).ConvertTo(defaultSystem, _data.Weight);
            }
        }

        #endregion
    }
}