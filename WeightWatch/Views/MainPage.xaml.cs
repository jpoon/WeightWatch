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
    using System.Globalization;
    using System.Windows.Controls.DataVisualization.Charting;
    using Microsoft.Phone.Controls;
    using WeightWatch.Classes;
    using WeightWatch.Models;
    using WeightWatch.ViewModels;

    public partial class MainPage : PhoneApplicationPage
    {
        WeightListViewModel _viewModel;

        const int GRAPH_DEFAULT_MAX = 100;
        const int GRAPH_DEFAULT_MIN = 0;
        const int GRAPH_DEFAULT_SPACING = 15;

        readonly Uri DOWN_ARROW = new Uri("/WeightWatch;component/Images/downarrow.png", UriKind.Relative);
        readonly Uri UP_ARROW = new Uri("/WeightWatch;component/Images/uparrow.png", UriKind.Relative);
        readonly Uri NO_CHANGE = new Uri("/WeightWatch;component/Images/nochange.png", UriKind.Relative);

        public MainPage()
        {
            InitializeComponent();

            Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            SetupGraph();
            SetupSummary();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var c = weightLongListSelector.SelectedItem;
            if (c == null)
            {
                _viewModel = new WeightListViewModel();
                DataContext = _viewModel;
            }
        }

        #region Summary

        private void SetupSummary()
        {
            summary_arrowImage.Source = new System.Windows.Media.Imaging.BitmapImage(NO_CHANGE);
            summary_weightTextBlock.Text = "0";
            summary_systemTextBlock.Text = "[" + MeasurementFactory.GetSystem(ApplicationSettings.DefaultMeasurementSystem).Abbreviation + "]";
            summary_messageTextBlock.Text =
                "Instructions:\n" +
                "(1) Add your daily weight\n" +
                "(2) Make a mistake? Tap and hold a weight entry on the 'Details' screen to delete\n";

            var first = _viewModel.FirstWeightEntry;
            var last = _viewModel.LastWeightEntry;

            if (first != null && last != null)
            {
                var weightDelta = last.Weight - first.Weight;
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

        #endregion

        #region Graph

        private void SetupGraph()
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
            var defaultMeasurementSystem = ApplicationSettings.DefaultMeasurementSystem;
            string weightAbbrev = MeasurementFactory.GetSystem(defaultMeasurementSystem).Abbreviation;

            var weightMinMax = _viewModel.GetMinMaxWeight(startDate, DateTime.Today);

            linearAxis.Title = "Weight (" + weightAbbrev + ")";

            weightMinMax.Min = weightMinMax.Min ?? GRAPH_DEFAULT_MIN;
            weightMinMax.Max = weightMinMax.Max ?? GRAPH_DEFAULT_MAX;

            double graphMinimum;
            double graphMaximum;
            if (weightMinMax.Min != weightMinMax.Max)
            {
                graphMinimum = Math.Floor((float)weightMinMax.Min / 10) * 10;
                graphMaximum = Math.Ceiling((float)weightMinMax.Max / 10) * 10;
            }
            else
            {
                var weightFloor = Math.Floor((float)weightMinMax.Max / 10) * 10;
                graphMaximum = weightFloor + GRAPH_DEFAULT_SPACING;
                graphMinimum = weightFloor - GRAPH_DEFAULT_SPACING;
            }

            if (graphMaximum < 0)
            {
                graphMaximum = 0;
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
            }
        }

        #endregion Graph

        #region Event Handlers

        private void AppBarIconClick_AddWeight(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/Views/AddWeightPage.xaml", UriKind.Relative)));
        }

        private void AppBarIconClick_Settings(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/Views/Settings.xaml", UriKind.Relative)));
        }

        private void AppBarIconClick_About(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/Views/About.xaml", UriKind.Relative)));
        }

        private void EditMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = (sender as MenuItem).DataContext as WeightViewModel;
            Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/Views/AddWeightPage.xaml?Date=" + item.DateStr, UriKind.Relative)));
        }

        private void DeleteMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = (sender as MenuItem).DataContext as WeightViewModel;
            WeightListViewModel.Delete(item);
        }

        #endregion Event Handlers

    }
}