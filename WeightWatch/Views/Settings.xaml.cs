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
    using WeightWatch.Classes;
    using WeightWatch.Models;
    using GraphMode = WeightWatch.Models.ApplicationSettings.GraphMode;

    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();
            Loaded += Settings_Loaded;
        }

        void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            // Default Measurement
            Measurement_ListPicker.ItemsSource = Helpers.EnumToStringList(typeof(MeasurementSystem));
            Measurement_ListPicker.SelectionChanged += Measurement_ListPicker_SelectionChanged;

            var index = 0;
            foreach (var system in Helpers.EnumToStringList(typeof(MeasurementSystem)))
            {
                if (system == ApplicationSettings.DefaultMeasurementSystem.ToString())
                {
                    Measurement_ListPicker.SelectedIndex = index;
                    break;
                }
                index++;
            }

            // Default Graph
            Graph_ListPicker.ItemsSource = Helpers.EnumToStringList(typeof(GraphMode));
            Graph_ListPicker.SelectionChanged += Graph_ListPicker_SelectionChanged;

            index = 0;
            foreach (var system in Helpers.EnumToStringList(typeof(GraphMode)))
            {
                if (system == ApplicationSettings.DefaultGraphMode.ToString())
                {
                    Graph_ListPicker.SelectedIndex = index;
                    break;
                }
                index++;
            }
        }

        #region Event Handlers

        private void Measurement_ListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationSettings.DefaultMeasurementSystem = (MeasurementSystem)Enum.Parse(typeof(MeasurementSystem), (string)Measurement_ListPicker.SelectedItem, true);
        }

        private void Graph_ListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationSettings.DefaultGraphMode = (GraphMode)Enum.Parse(typeof(GraphMode), (string)Graph_ListPicker.SelectedItem, true);
        }

        #endregion
    }
}