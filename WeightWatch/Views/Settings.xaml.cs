﻿using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using WeightWatch.Models;

namespace WeightWatch.Views
{
    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(Settings_Loaded);
        }

        #region Event Handlers

        void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            // Default Measurement
            Measurement_ListPicker.ItemsSource = MeasurementFactory.GetAllString();
            Measurement_ListPicker.SelectionChanged += new SelectionChangedEventHandler(Measurement_ListPicker_SelectionChanged);

            int index = 0;
            foreach (string system in MeasurementFactory.GetAllString())
            {
                if (system == ApplicationSettings.DefaultMeasurementSystem.ToString())
                {
                    Measurement_ListPicker.SelectedIndex = index;
                    break;
                }
                else
                {
                    index++;
                }
            }


            Graph_ListPicker.SelectionChanged += new SelectionChangedEventHandler(Graph_ListPicker_SelectionChanged);
        }


        private void Measurement_ListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationSettings.DefaultMeasurementSystem = (MeasurementSystem)Enum.Parse(typeof(MeasurementSystem), (string)Measurement_ListPicker.SelectedItem, true);
        }

        private void Graph_ListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #endregion
    }
}