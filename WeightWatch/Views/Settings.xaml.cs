﻿namespace WeightWatch.Views
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Tasks;
    using WeightWatch.Classes;
    using WeightWatch.Models;
    using GraphMode = WeightWatch.Models.ApplicationSettings.GraphMode;

    public partial class Settings : PhoneApplicationPage
    {
        private AssemblyName _asmName;

        public Settings()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Settings_Loaded);

            _asmName = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
        }

        void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            versionTextBlock.Text = "Version: " + _asmName.Version.ToString();

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

        #region Event Handlers

        private void Measurement_ListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationSettings.DefaultMeasurementSystem = (MeasurementSystem)Enum.Parse(typeof(MeasurementSystem), (string)Measurement_ListPicker.SelectedItem, true);
        }

        private void Graph_ListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationSettings.DefaultGraphMode = (GraphMode)Enum.Parse(typeof(GraphMode), (string)Graph_ListPicker.SelectedItem, true);
        }

        private void sendFeedBackButton_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask email = new EmailComposeTask();
            email.To = App.FeedbackEmail;
            email.Body = "Version: " + _asmName.Version.ToString();
            email.Show();
        }

        private void githubButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask browser = new WebBrowserTask();
            browser.URL = Uri.EscapeDataString(App.ContributeUrl);
            browser.Show();
        }

        private void reviewButton_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask review = new MarketplaceReviewTask();
            review.Show();
        }

        #endregion
    }
}