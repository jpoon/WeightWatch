using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using WeightWatch.Classes;
using WeightWatch.Models;
using GraphMode = WeightWatch.Models.ApplicationSettings.GraphMode;

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
            Measurement_ListPicker.ItemsSource = Helpers.EnumToStringList(typeof(MeasurementSystem));
            Measurement_ListPicker.SelectionChanged += new SelectionChangedEventHandler(Measurement_ListPicker_SelectionChanged);

            int index = 0;
            foreach (string system in Helpers.EnumToStringList(typeof(MeasurementSystem)))
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

            // Default Graph
            Graph_ListPicker.ItemsSource = Helpers.EnumToStringList(typeof(GraphMode));
            Graph_ListPicker.SelectionChanged += new SelectionChangedEventHandler(Graph_ListPicker_SelectionChanged);

            index = 0;
            foreach (string system in Helpers.EnumToStringList(typeof(GraphMode)))
            {
                if (system == ApplicationSettings.DefaultGraphMode.ToString())
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
            ApplicationSettings.DefaultMeasurementSystem = (MeasurementSystem)Enum.Parse(typeof(MeasurementSystem), (string)Measurement_ListPicker.SelectedItem, true);
        }

        private void Graph_ListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationSettings.DefaultGraphMode = (GraphMode)Enum.Parse(typeof(GraphMode), (string)Graph_ListPicker.SelectedItem, true);
        }

        #endregion
    }
}