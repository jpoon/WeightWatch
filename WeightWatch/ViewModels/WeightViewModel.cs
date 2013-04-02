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
    using WeightWatch.MeasurementSystem;

    public class WeightViewModel
    {
        public readonly WeightModel WeightModel;

        public WeightViewModel(WeightModel data)
        {
            WeightModel = data;
        }

        #region Properties

        public DateTime Date
        {
            get
            {
                return WeightModel.Date;
            }
        }

        public string DateStr
        {
            get
            {
                return WeightModel.Date.ToString("MMM. d, yyyy", CultureInfo.InvariantCulture);
            }
        }

        public string DateStrMonthYear
        {
            get
            {
                return WeightModel.Date.ToString("MMM yyyy", CultureInfo.InvariantCulture);
            }
        }

        public string WeightStr
        {
            get
            {
                var defaultSystem = ApplicationSettings.DefaultMeasurementSystem;
                return Weight.ToString("0.##", CultureInfo.InvariantCulture) + " " + MeasurementSystemFactory.Get(defaultSystem).Abbreviation;
            }
        }

        public Double Weight
        {
            get
            {
                var defaultSystem = ApplicationSettings.DefaultMeasurementSystem;
                return MeasurementSystemFactory.Get(WeightModel.MeasurementUnit).ConvertTo(defaultSystem, WeightModel.Weight);
            }
        }

        #endregion
    }
}