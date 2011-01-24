using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using WeightWatch.Models;
using WeightWatch.Classes;
using GraphMode = WeightWatch.Models.ApplicationSettings.GraphMode;

namespace WeightWatch.Views
{
    public partial class Settings : PhoneApplicationPage
    {
        ApplicationSettings _appSettings;

        public Settings()
        {
            InitializeComponent();

            _appSettings = new ApplicationSettings();

            this.Loaded += new RoutedEventHandler(Settings_Loaded);
        }

        #region Event Handlers

        void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            // Default Measurement
            Measurement_ListPicker.ItemsSource = Helpers.EnumToStringList(typeof(MeasurementSystem));
            Measurement_ListPicker.SelectionChanged += new SelectionChangedEventHandler(Measurement_ListPicker_SelectionChanged);

            int index = 0;
            foreach (string system in Helpers.EnumToStringList(typeof(MeasurementSystem)))
            {
                if (system == _appSettings.DefaultMeasurementSystem.ToString())
                {
                    Measurement_ListPicker.SelectedIndex = index;
                    break;
                }
                else
                {
                    index++;
                }
            }

            // Default Graph
            Graph_ListPicker.ItemsSource = Helpers.EnumToStringList(typeof(GraphMode));
            Graph_ListPicker.SelectionChanged += new SelectionChangedEventHandler(Graph_ListPicker_SelectionChanged);

            index = 0;
            foreach (string system in Helpers.EnumToStringList(typeof(GraphMode)))
            {
                if (system == _appSettings.DefaultGraphMode.ToString())
                {
                    Graph_ListPicker.SelectedIndex = index;
                    break;
                }
                else
                {
                    index++;
                }
            }
        }


        private void Measurement_ListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _appSettings.DefaultMeasurementSystem = (MeasurementSystem)Enum.Parse(typeof(MeasurementSystem), (string)Measurement_ListPicker.SelectedItem, true);
        }

        private void Graph_ListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _appSettings.DefaultGraphMode = (GraphMode)Enum.Parse(typeof(GraphMode), (string)Graph_ListPicker.SelectedItem, true);
        }

        #endregion
    }
}