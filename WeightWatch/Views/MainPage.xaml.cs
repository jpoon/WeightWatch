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
    using System.Windows.Controls.DataVisualization.Charting;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using WeightWatch.Models;
    using WeightWatch.ViewModels;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;

    public partial class MainPage : PhoneApplicationPage
    {
        WeightListViewModel _viewModel;

        const int GRAPH_DEFAULT_MAX = 100;
        const int GRAPH_DEFAULT_MIN = 0;
        Uri DOWN_ARROW = new Uri("/WeightWatch;component/Images/downarrow.png", UriKind.Relative);
        Uri UP_ARROW = new Uri("/WeightWatch;component/Images/uparrow.png", UriKind.Relative);

        ReadOnlyCollection<String> NegativeComments = new ReadOnlyCollection<String>(
            new List<String>() { 
                "Hey Fatty!\r\nLay off the donuts! You've gained [DELTA_WEIGHT] since starting on [START_DATE].",
                "[DELTA_WEIGHT]!?!?!\r\nYou're packing on the pounds there buddy.",
                "Boom! Boom! Boom!\r\nHear that? That's the sound of [LAST_WEIGHT] walking into the room.",
                "You're so fat, that you make free willy look like a goldfish.",
                "[DELTA_WEIGHT]?! You're getting so fat that if I take a picture of you, it'll still be printing until next year.",
                "On [START_DATE] you were [START_WEIGHT], on [LAST_DATE] you were [LAST_WEIGHT].\r\nWhat the heck happened?",
                "How on earth did you manage to gain [DELTA_WEIGHT] between [START_DATE] and [LAST_DATE]?!",
                "At [LAST_WEIGHT], you're getting so fat to the point where if your beeper goes off, people will think you are backing up.",
                "You've gained [DELTA_WEIGHT]. Go hit the gym!",
            }
        );

        ReadOnlyCollection<String> PositiveComments = new ReadOnlyCollection<String>(
            new List<String>() { 
                "Wow! [DELTA_WEIGHT]!\r\nI am impressed",
                "What's your secret? You've lost [DELTA_WEIGHT]!",
                "Since starting on [START_DATE] at [START_WEIGHT], you've lost an amazing [DELTA_WEIGHT]! Keep it up!",
                "Before: [START_WEIGHT]\r\nAfter: [LAST_WEIGHT]\r\nDifference: [DELTA_WEIGHT]\r\n",
            }
        );

        public MainPage()
        {
            InitializeComponent();

            _viewModel = WeightListViewModel.GetInstance();
            this.DataContext = _viewModel;

            Loaded += new System.Windows.RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            SetupGraph();
            SetupSummary();

            weightLongListSelector.ItemsSource = _viewModel.WeightHistoryGroup;
        }

        private void SetupSummary()
        {
            WeightViewModel first = _viewModel.FirstWeightEntry;
            WeightViewModel last = _viewModel.LastWeightEntry;

            string message = String.Empty;
            string measurementSystemAbbr = MeasurementFactory.GetSystem(ApplicationSettings.DefaultMeasurementSystem).Abbreviation;
            if (first != null && last != null)
            {
                Decimal weightDelta = last.Weight - first.Weight;

                if (weightDelta > 0)
                {
                    summary_arrowImage.Source = new System.Windows.Media.Imaging.BitmapImage(UP_ARROW);
                    message = NegativeComments[new Random().Next(NegativeComments.Count)];
                }
                else
                {
                    summary_arrowImage.Source = new System.Windows.Media.Imaging.BitmapImage(DOWN_ARROW);
                    message = PositiveComments[new Random().Next(PositiveComments.Count)];
                }

                summary_weightTextBlock.Text = weightDelta.ToString("+#;-#;0");
                summary_systemTextBlock.Text = "[" + measurementSystemAbbr + "]";

                message = message.Replace("[DELTA_WEIGHT]", Math.Round(weightDelta).ToString() + " " + measurementSystemAbbr);
                message = message.Replace("[START_DATE]", first.DateStr);
                message = message.Replace("[START_WEIGHT]", first.WeightStr);
                message = message.Replace("[LAST_DATE]", last.DateStr);
                message = message.Replace("[LAST_WEIGHT]", last.WeightStr);
                summary_messageTextBlock.Text = message;
            }
        }

        #region Graph

        private void SetupGraph()
        {
            AreaSeries areaSeries = (AreaSeries)weightChart.Series[0];
            areaSeries.Refresh();

            DateTime startDate = new DateTime();
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
                default:
                    break;
            }
            foreach (IAxis axis in weightChart.Axes)
            {
                Type axisType = axis.GetType();
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
            MeasurementSystem defaultMeasurementSystem = ApplicationSettings.DefaultMeasurementSystem;
            string weightAbbrev = MeasurementFactory.GetSystem(defaultMeasurementSystem).Abbreviation;

            WeightListViewModel.WeightMinMax weightMinMax = _viewModel.GetMinMaxWeight(startDate, DateTime.Today);

            linearAxis.Title = "Weight (" + weightAbbrev + ")";

            if (weightMinMax.Min == null)
            {
                weightMinMax.Min = GRAPH_DEFAULT_MIN;
            }

            if (weightMinMax.Max == null)
            {
                weightMinMax.Max = GRAPH_DEFAULT_MAX;
            }

            double graphMinimum = Math.Floor((float)weightMinMax.Min / 10) * 10;
            double graphMaximum = Math.Ceiling((float)weightMinMax.Max / 10) * 10;

            linearAxis.Minimum = 0;

            linearAxis.Maximum = graphMaximum;
            linearAxis.Minimum = graphMinimum;

            linearAxis.Interval = Math.Floor((double)(linearAxis.Maximum - linearAxis.Minimum) / 10);
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
                default:
                    break;
            }
        }

        #endregion Graph

        #region Event Handlers

        private void AppBarIconClick_AddWeight(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/AddWeightPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void AppBarIconClick_Settings(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Settings.xaml", UriKind.RelativeOrAbsolute));
        }

        #endregion Event Handlers
    }
}