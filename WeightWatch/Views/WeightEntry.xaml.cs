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
    using Microsoft.Phone.Controls;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using WeightWatch.Classes;
    using WeightWatch.MeasurementSystem;
    using WeightWatch.Models;
    using WeightWatch.ViewModels;

    public partial class WeightEntry : PhoneApplicationPage
    {
        public class WeightEntryItem
        {
            public string Weight
            {
                get; set;
            }

            private DateTime? _date = DateTime.Today;
            public DateTime? Date
            {
                get { return _date; }
                set { _date = value; }
            }
        }

        private readonly WeightEntryItem _newEntry;
        private WeightListViewModel _weightListViewModel;

        public WeightEntry()
        {
            InitializeComponent();
            Loaded += WeightEntryLoaded;

            _newEntry = new WeightEntryItem();
            DataContext = _newEntry;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _weightListViewModel = new WeightListViewModel();

            var selectedSystem = ApplicationSettings.DefaultMeasurementSystem;

            string dateString;
            if (NavigationContext.QueryString.TryGetValue("Date", out dateString))
            {
                DateTime datetime;
                if (DateTime.TryParse(dateString, out datetime))
                {
                    var viewModel = _weightListViewModel.Get(datetime);
                    if (viewModel != null)
                    {
                        weightDatePicker.Value = datetime;
                        weightDatePicker.IsEnabled = false;

                        weightTextBox.Text = viewModel.Weight.ToString(CultureInfo.InvariantCulture);
                        selectedSystem = viewModel.WeightModel.MeasurementUnit;

                        PageTitle.Text = "edit";
                    }
                }
            }
            
            Measurement_ListPicker.ItemsSource = MeasurementSystemFactory.Get().Select(system => system.Abbreviation);

            var index = 0;
            foreach (var system in Helpers.GetAllEnum<MeasurementUnit>())
            {
                if (system.Equals(selectedSystem))
                {
                    Measurement_ListPicker.SelectedIndex = index;
                    break;
                }
                index++;
            }
        }

        private void WeightEntryLoaded(object sender, RoutedEventArgs e)
        {
            weightTextBox.Focus();
            weightTextBox.Select(weightTextBox.Text.Length, 0);
        }

        private void AppBarIconButtonSaveClick(object sender, EventArgs args)
        {
            // Force update binding first
            var binding = weightTextBox.GetBindingExpression(TextBox.TextProperty);
            if (binding != null) binding.UpdateSource();

            try
            {
                WeightListViewModel.Save(_newEntry.Weight, _newEntry.Date, MeasurementSystemFactory.Get((string) Measurement_ListPicker.SelectedItem).MeasurementSystem);
                GoToMainMenu();
            }
            catch (ArgumentException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void AppBarIconButtonCancelClick(object sender, EventArgs e)
        {
            GoToMainMenu();
        }

        private void GoToMainMenu()
        {
            NavigationService.Navigate(new Uri("/Views/MainPage.xaml", UriKind.Relative));
        }
    }
}