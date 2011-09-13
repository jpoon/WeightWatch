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
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Phone.Controls;
    using WeightWatch.Classes;
    using WeightWatch.Models;
    using WeightWatch.ViewModels;

    public partial class AddWeightPage : PhoneApplicationPage
    {
        public class WeightEntry
        {
            private Double? _weight = null;
            public Double? Weight
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

            private MeasurementSystem _unit = ApplicationSettings.DefaultMeasurementSystem;
            public MeasurementSystem MeasurementUnit
            {
                get { return _unit; }
                set { _unit = value; }
            }

            public string Validate()
            {
                String errorStr = String.Empty;

                if (_weight == null || _weight < 0)
                {
                    errorStr = "Please enter a valid weight";
                }
                else if (_weight > 9999)
                {
                    errorStr = "Seriously?! I doubt you weigh that much...";
                }
                else if (_date == null)
                {
                    errorStr = "Please enter a valid date";
                }

                return errorStr;
            }
        }

        readonly WeightEntry _newEntry;

        public AddWeightPage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(AddWeightPage_Loaded);

            _newEntry = new WeightEntry();
            this.DataContext = _newEntry;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string dateString;
            if (NavigationContext.QueryString.TryGetValue("Date", out dateString))
            {
                DateTime datetime;
                DateTime.TryParse(dateString, out datetime);
                weightDatePicker.Value = datetime;
                weightDatePicker.IsEnabled = false;

                WeightViewModel viewModel = WeightListViewModel.Get(datetime);
                weightTextBox.Text = viewModel.Weight.ToString();

                if (viewModel.WeightModel.MeasurementUnit == MeasurementSystem.Imperial)
                {
                    radioButton_lbs.IsChecked = true;
                }
                else
                {
                    radioButton_kgs.IsChecked = true;
                }
            }
        }

        void AddWeightPage_Loaded(object sender, RoutedEventArgs e)
        {
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
                            CultureInfo.InvariantCulture,
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

            String errorMessage = _newEntry.Validate();
            if (String.IsNullOrEmpty(errorMessage))
            {
                WeightListViewModel.Save((Double)_newEntry.Weight, (DateTime)_newEntry.Date, _newEntry.MeasurementUnit);
                GoBackOrMainMenu();
            }
            else
            {
                MessageBox.Show(errorMessage);
            }
        }

        private void AppBarIconButton_CancelClick(object sender, EventArgs e)
        {
            GoBackOrMainMenu();
        }

        private void GoBackOrMainMenu()
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
        }

        private void radioButtonChecked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            var rbContent = (string)rb.Content;
            foreach (var system in Helpers.GetAllEnum<MeasurementSystem>())
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