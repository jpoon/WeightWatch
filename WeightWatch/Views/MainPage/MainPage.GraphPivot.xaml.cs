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

namespace WeightWatch.Views
{
    using System;
    using System.Linq;
    using System.Windows.Controls.DataVisualization.Charting;
    using Microsoft.Phone.Controls;
    using WeightWatch.Models;


    public partial class MainPage : PhoneApplicationPage
    {
        const int GraphDefaultMax = 100;
        const int GraphDefaultMin = 0;
        const int GraphDefaultSpacing = 15;
        const int GraphDefaultResolution = 10;

        private void SetupGraphPivot()
        {
            var areaSeries = (AreaSeries)weightChart.Series[0];
            areaSeries.Refresh();

            var startDate = new DateTime();
            switch (ApplicationSettings.DefaultGraphMode)
            {
                case ApplicationSettings.GraphMode.Week:
                    startDate = DateTime.Today.AddDays(-6);
                    break;
                case ApplicationSettings.GraphMode.Month:
                    startDate = DateTime.Today.AddDays(-35);
                    break;
                case ApplicationSettings.GraphMode.Year:
                    startDate = DateTime.Today.AddMonths(-12);
                    break;
            }
            foreach (var axis in weightChart.Axes)
            {
                var axisType = axis.GetType();
                if (axisType == typeof(DateTimeAxis))
                {
                    SetupDateTimeAxis((DateTimeAxis)axis, startDate);
                }
                else if (axisType == typeof(LinearAxis))
                {
                    SetupLinearAxis((LinearAxis)axis, startDate);
                }
            }
        }

        private void SetupLinearAxis(LinearAxis linearAxis, DateTime startDate)
        {
            // Title
            var defaultMeasurementSystem = ApplicationSettings.DefaultMeasurementSystem;
            string weightAbbrev = MeasurementFactory.GetSystem(defaultMeasurementSystem).Abbreviation;
            linearAxis.Title = "Weight (" + weightAbbrev + ")";

            // Interval, Range
            var weightList = _viewModel.Get(startDate, DateTime.Today);

            double? weightRangeMin = null;
            double? weightRangeMax = null;
            foreach (var weight in weightList.Select(item => item.Weight))
            {
                if (!weightRangeMin.HasValue || weightRangeMin > weight) 
                {
                    weightRangeMin = weight;
                }

                if (!weightRangeMax.HasValue || weightRangeMax < weight)
                {
                    weightRangeMax = weight;
                }
            }

            weightRangeMin = weightRangeMin ?? GraphDefaultMin;
            weightRangeMax = weightRangeMax ?? GraphDefaultMax;
            
            double graphMinimum;
            double graphMaximum;
            if (weightRangeMin != weightRangeMax)
            {
                graphMinimum = Math.Floor((float)weightRangeMin / GraphDefaultResolution) * GraphDefaultResolution;
                graphMaximum = Math.Ceiling((float)weightRangeMax / GraphDefaultResolution) * GraphDefaultResolution;
            }
            else
            {
                var weightFloor = Math.Floor((float)weightRangeMax / GraphDefaultResolution) * GraphDefaultResolution;
                graphMaximum = weightFloor + GraphDefaultSpacing;
                graphMinimum = weightFloor - GraphDefaultSpacing;
            }

            if (graphMaximum < 0)
            {
                graphMaximum = 0;
            }

            linearAxis.Minimum = 0;
            linearAxis.Maximum = graphMaximum;
            linearAxis.Minimum = graphMinimum;

            linearAxis.Interval = Math.Floor((double)(linearAxis.Maximum - linearAxis.Minimum) / GraphDefaultResolution);
        }

        private static void SetupDateTimeAxis(DateTimeAxis dateTimeAxis, DateTime startDate)
        {
            dateTimeAxis.Minimum = startDate;
            dateTimeAxis.Maximum = DateTime.Today;
            switch (ApplicationSettings.DefaultGraphMode)
            {
                case ApplicationSettings.GraphMode.Week:
                    dateTimeAxis.IntervalType = DateTimeIntervalType.Days;
                    dateTimeAxis.Interval = 1;
                    break;
                case ApplicationSettings.GraphMode.Month:
                    dateTimeAxis.IntervalType = DateTimeIntervalType.Days;
                    dateTimeAxis.Interval = 6;
                    break;
                case ApplicationSettings.GraphMode.Year:
                    dateTimeAxis.IntervalType = DateTimeIntervalType.Months;
                    dateTimeAxis.Interval = 2;
                    break;
            }
        }
    }
}
