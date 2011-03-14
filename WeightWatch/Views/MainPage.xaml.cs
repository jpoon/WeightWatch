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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Windows.Controls.DataVisualization.Charting;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using WeightWatch.Models;
    using WeightWatch.ViewModels;
    using WeightWatch.Classes;

    public partial class MainPage : PhoneApplicationPage
    {
        WeightListViewModel _viewModel;

        const int GRAPH_DEFAULT_MAX = 100;
        const int GRAPH_DEFAULT_MIN = 0;
        const int GRAPH_DEFAULT_SPACING = 15;

        Uri DOWN_ARROW = new Uri("/WeightWatch;component/Images/downarrow.png", UriKind.Relative);
        Uri UP_ARROW = new Uri("/WeightWatch;component/Images/uparrow.png", UriKind.Relative);
        Uri NO_CHANGE = new Uri("/WeightWatch;component/Images/nochange.png", UriKind.Relative);

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
        }

        private void SetupSummary()
        {
            summary_arrowImage.Source = new System.Windows.Media.Imaging.BitmapImage(NO_CHANGE);
            summary_weightTextBlock.Text = "0";
            summary_systemTextBlock.Text = "[" + MeasurementFactory.GetSystem(ApplicationSettings.DefaultMeasurementSystem).Abbreviation + "]";

            WeightViewModel first = _viewModel.FirstWeightEntry;
            WeightViewModel last = _viewModel.LastWeightEntry;

            if (first != null && last != null)
            {
                Decimal weightDelta = last.Weight - first.Weight;
                if (weightDelta > 0)
                {
                    summary_arrowImage.Source = new System.Windows.Media.Imaging.BitmapImage(UP_ARROW);
                }
                else if (weightDelta < 0)
                {
                    summary_arrowImage.Source = new System.Windows.Media.Imaging.BitmapImage(DOWN_ARROW);
                }

                summary_weightTextBlock.Text = weightDelta.ToString("+#.#;-#.#;0", CultureInfo.InvariantCulture);
                summary_messageTextBlock.Text = Message.GetMessage(first, last);
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

            double graphMinimum;
            double graphMaximum;
            if (weightMinMax.Min != weightMinMax.Max)
            {
                graphMinimum = Math.Floor((float)weightMinMax.Min / 10) * 10;
                graphMaximum = Math.Ceiling((float)weightMinMax.Max / 10) * 10;
            }
            else
            {
                double weightFloor = Math.Floor((float)weightMinMax.Max / 10) * 10;
                graphMaximum = weightFloor + GRAPH_DEFAULT_SPACING;
                graphMinimum = weightFloor - GRAPH_DEFAULT_SPACING;
            }

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

        private void DeleteMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            WeightViewModel item = (sender as MenuItem).DataContext as WeightViewModel;
            WeightListViewModel.Delete(item);

            // hack to refresh list
            weightLongListSelector.ItemsSource = null;
            weightLongListSelector.ItemsSource = _viewModel.WeightHistoryGroup;
        }

        #endregion Event Handlers

    }
}