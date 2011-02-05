namespace WeightWatch.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Phone.Controls;
    using WeightWatch.Models;
    using WeightWatch.ViewModels;
    using WeightWatch.Classes;

    public partial class AddWeightPage : PhoneApplicationPage
    {
        public class WeightEntry
        {
            private float? _weight = null;
            public float? Weight
            {
                get { return _weight; }
                set { _weight = value; }
            }

            private DateTime? _date = DateTime.Today;
            public DateTime? Date
            {
                get { return _date; }
                set { _date = value; }
            }

            private MeasurementSystem _unit = MeasurementFactory.DefaultMeasurementUnit;
            public MeasurementSystem MeasurementUnit
            {
                get { return _unit; }
                set { _unit = value; }
            }
        }

        WeightEntry _newEntry;
        WeightListViewModel _weightListViewModel;

        public AddWeightPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(AddWeightPage_Loaded);

            _newEntry = new WeightEntry();
            _weightListViewModel = WeightListViewModel.GetInstance();
            this.DataContext = _newEntry;
        }

        void AddWeightPage_Loaded(object sender, RoutedEventArgs e)
        {
            // setup radio buttons
            radioButton_lbs.Content = MeasurementFactory.GetSystem(MeasurementSystem.Imperial).Abbreviation;
            radioButton_kgs.Content = MeasurementFactory.GetSystem(MeasurementSystem.Metric).Abbreviation;
            switch (_newEntry.MeasurementUnit)
            {
                case MeasurementSystem.Imperial:
                    radioButton_lbs.IsChecked = true;
                    break;
                case MeasurementSystem.Metric:
                    radioButton_kgs.IsChecked = true;
                    break;
                default:
                    throw new ArgumentException(
                        string.Format(
                            "Measurement system of type {0} cannot be found",
                            Enum.GetName(typeof(MeasurementSystem), _newEntry.MeasurementUnit))
                        );
            }

            weightTextBox.Focus();
        }

        private void AppBarIconButton_SaveClick(object sender, EventArgs e)
        {
            // Force update binding first
            var binding = weightTextBox.GetBindingExpression(TextBox.TextProperty);
            binding.UpdateSource();

            _weightListViewModel.Save((float)_newEntry.Weight, (DateTime)_newEntry.Date, _newEntry.MeasurementUnit);
            GoBackOrMainMenu();
        }

        private void AppBarIconButton_DeleteClick(object sender, EventArgs e)
        {
            GoBackOrMainMenu();
        }

        private void GoBackOrMainMenu()
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void radioButtonChecked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            string rbContent = (string)rb.Content;
            foreach (MeasurementSystem system in Helpers.GetAllEnum<MeasurementSystem>())
            {
                if (MeasurementFactory.GetSystem(system).Abbreviation.Equals(rbContent))
                {
                    _newEntry.MeasurementUnit = system;
                    break;
                }
            }
        }
    }
}