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

            private MeasurementSystem _unit = ApplicationSettings.DefaultMeasurementSystem;
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

            WeightListViewModel.Save((Decimal)_newEntry.Weight, (DateTime)_newEntry.Date, _newEntry.MeasurementUnit);
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