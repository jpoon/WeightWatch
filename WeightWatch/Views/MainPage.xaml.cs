namespace WeightWatch.Views
{
    using System;
    using System.Windows.Controls.DataVisualization.Charting;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using WeightWatch.Models;
    using WeightWatch.ViewModels;

    public partial class MainPage : PhoneApplicationPage
    {
        WeightListViewModel _viewModel;

        const int GRAPH_DEFAULT_MAX = 100;
        const int GRAPH_DEFAULT_MIN = 0;
        Uri DOWN_ARROW = new Uri("/WeightWatch;component/Images/downarrow.png", UriKind.Relative);
        Uri UP_ARROW = new Uri("/WeightWatch;component/Images/uparrow.png", UriKind.Relative);

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

            if (first != null && last != null)
            {
                float weightDelta = last.Weight - first.Weight;
                if (weightDelta > 0)
                {
                    summary_arrowImage.Source = new System.Windows.Media.Imaging.BitmapImage(UP_ARROW);
                }
                else
                {
                    summary_arrowImage.Source = new System.Windows.Media.Imaging.BitmapImage(DOWN_ARROW);
                }
                summary_weightTextBlock.Text = weightDelta.ToString("+#;-#;0");
                summary_systemTextBlock.Text = "[" + MeasurementFactory.GetSystem(ApplicationSettings.DefaultMeasurementSystem).Abbreviation + "]";
                summary_messageTextBlock.Text = "Hey Fatty, \r\n lay off the donuts";
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