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

            _viewModel = WeightListViewModel.GetInstance();
            this.DataContext = _viewModel;

            Loaded += new System.Windows.RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
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

            foreach (IAxis axis in weightGraph.Axes)
            {
                Type axisType = axis.GetType();
                if (axisType == typeof(DateTimeAxis))
                {
                    DateTimeAxis tmp = axis as DateTimeAxis;
                    tmp.IntervalType = DateTimeIntervalType.Days;
                    tmp.Interval = 1;
                    tmp.Minimum = DateTime.Today.AddHours(-24 * 5);
                    tmp.Maximum = DateTime.Today;
                }
                else if (axisType == typeof(LinearAxis))
                {
                    string weightAbbrev = MeasurementFactory.GetSystem((MeasurementSystem)ApplicationSettings.DefaultMeasurementSystem).Abbreviation;

                    LinearAxis tmp = axis as LinearAxis;
                    tmp.Title = "Weight (" + weightAbbrev + ")";
                    tmp.Minimum = 0;
                    tmp.Maximum = 100;
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