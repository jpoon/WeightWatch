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
    using System.IO.IsolatedStorage;
    using WeightWatch.MeasurementSystem;

    public static class ApplicationSettings
    {
        public enum GraphMode
        {
            Week = 1,
            Month = 2,
            Year = 3
        };

        public static GraphMode DefaultGraphMode
        {
            get
            {
                GraphMode defaultGraphMode;
                if (!IsolatedStorageSettings.ApplicationSettings.TryGetValue("GraphMode", out defaultGraphMode))
                {
                    defaultGraphMode = GraphMode.Week;
                    IsolatedStorageSettings.ApplicationSettings.Add("GraphMode", defaultGraphMode);
                }

                return defaultGraphMode;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["GraphMode"] = value;
            }
        }

        public static MeasurementUnit DefaultMeasurementSystem
        {
            get
            {
                MeasurementUnit defaultMeasurementSystem;
                if (!IsolatedStorageSettings.ApplicationSettings.TryGetValue("MeasurementSystem", out defaultMeasurementSystem))
                {
                    defaultMeasurementSystem = MeasurementUnit.Imperial;
                    IsolatedStorageSettings.ApplicationSettings.Add("MeasurementSystem", defaultMeasurementSystem);
                }

                return defaultMeasurementSystem;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["MeasurementSystem"] = value;
            }
        }
    }
}
