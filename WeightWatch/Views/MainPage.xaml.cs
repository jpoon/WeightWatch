using System;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using WeightWatch.Models;

namespace WeightWatch
{

    public partial class MainPage : PhoneApplicationPage
    {
        WeightListViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();

            Loaded += new System.Windows.RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _viewModel = new WeightListViewModel();
            this.DataContext = _viewModel;

            weightGraph.Series.Clear();

            // Graph Series
            AreaSeries series = new AreaSeries()
            {
                ItemsSource = _viewModel.WeightHistoryList,
                DependentValuePath = "Weight",
                IndependentValuePath = "Date",
                AnimationSequence = AnimationSequence.FirstToLast,
            };
            weightGraph.Series.Add(series);

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

            foreach (IAxis axis in weightGraph.Axes)
            {
                Type axisType = axis.GetType();
                if (axisType == typeof(DateTimeAxis))
                {
                    DateTimeAxis tmp = axis as DateTimeAxis;
                    tmp.IntervalType = DateTimeIntervalType.Days;
                    tmp.Minimum = startDate;
                    tmp.Maximum = DateTime.Today;

                    switch (ApplicationSettings.DefaultGraphMode)
                    {
                        case ApplicationSettings.GraphMode.Week:
                            tmp.Interval = 1;
                            break;
                        case ApplicationSettings.GraphMode.Month:
                            tmp.Interval = 6;
                            break;
                        case ApplicationSettings.GraphMode.Year:
                            tmp.IntervalType = DateTimeIntervalType.Months;
                            tmp.Interval = 2;
                            break;
                        default:
                            break;
                    }
                }
                else if (axisType == typeof(LinearAxis))
                {
                    string weightAbbrev = MeasurementFactory.GetSystem((MeasurementSystem)ApplicationSettings.DefaultMeasurementSystem).Abbreviation;

                    WeightListViewModel.WeightMinMax weightMinMax = _viewModel.GetMinMaxWeight(startDate, DateTime.Today);

                    LinearAxis tmp = axis as LinearAxis;
                    tmp.Title = "Weight (" + weightAbbrev + ")";
                    tmp.Minimum = weightMinMax.Min - 10;
                    tmp.Maximum = weightMinMax.Max + 10;
                    tmp.Interval = 10;
                }
            }
        }

        private void AppBarIconClick_AddWeight(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/AddWeightPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void AppBarIconClick_Settings(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Settings.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}