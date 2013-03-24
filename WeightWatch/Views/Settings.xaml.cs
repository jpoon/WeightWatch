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
    using Microsoft.Live;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using WeightWatch.Classes;
    using WeightWatch.MeasurementSystem;
    using WeightWatch.Models;
    using WeightWatch.Skydrive;
    using GraphMode = WeightWatch.Models.ApplicationSettings.GraphMode;

    public partial class Settings : PhoneApplicationPage
    {
        private Skydrive _skydrive;
        private readonly ProgressIndicator _progressIndicator;

        public Settings()
        {
            InitializeComponent();
            Loaded += Settings_Loaded;

            this._progressIndicator = new ProgressIndicator
            {
                IsVisible = false,
                IsIndeterminate = false,
            };

            SystemTray.SetProgressIndicator(this, _progressIndicator);
        }

        private void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            // Default Measurement
            Measurement_ListPicker.ItemsSource = Helpers.EnumToStringList(typeof(MeasurementUnit));
            Measurement_ListPicker.SelectionChanged += MeasurementListPickerSelectionChanged;

            var index = 0;
            foreach (var system in Helpers.EnumToStringList(typeof(MeasurementUnit)))
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
            Graph_ListPicker.SelectionChanged += GraphListPickerSelectionChanged;

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

        private void MeasurementListPickerSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationSettings.DefaultMeasurementSystem = (MeasurementUnit)Enum.Parse(typeof(MeasurementUnit), (string)Measurement_ListPicker.SelectedItem, true);
        }

        private void GraphListPickerSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationSettings.DefaultGraphMode = (GraphMode)Enum.Parse(typeof(GraphMode), (string)Graph_ListPicker.SelectedItem, true);
        }

        private void LiveIdSignInButtonSessionChanged(object sender, Microsoft.Live.Controls.LiveConnectSessionChangedEventArgs e)
        {
            if (e.Status == LiveConnectSessionStatus.Connected)
            {
                _skydrive = new Skydrive(e.Session);
                _skydrive.PropertyChanged += SkydrivePropertyChanged;

                this._progressIndicator.Text = "Accessing SkyDrive...";
                this._progressIndicator.IsVisible = true;
            }
        }

        private void SkydrivePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Status"))
            {
                buttonBackup.IsEnabled = false;
                buttonRestore.IsEnabled = false;
                dateTextBlock.Text = String.Empty;

                switch (this._skydrive.Status)
                {
                    case SkydriveStatus.Error:
                        MessageBox.Show(this._skydrive.ErrorMessage);
                        break;
                    case SkydriveStatus.GetFoldersPending:
                    case SkydriveStatus.GetFilesPending:
                    case SkydriveStatus.CreateFolderPending:
                    case SkydriveStatus.UploadPending:
                    case SkydriveStatus.DownloadPending:
                    case SkydriveStatus.GetFoldersComplete:
                        break;
                    case SkydriveStatus.CreateFolderComplete:
                    case SkydriveStatus.GetFilesComplete:
                    case SkydriveStatus.UploadCompleted:
                    case SkydriveStatus.DownloadCompleted:
                        buttonBackup.IsEnabled = true;
                        this._progressIndicator.IsVisible = false;

                        if (this._skydrive.LastBackUpDateTime != null)
                        {
                            buttonRestore.IsEnabled = true;
                            dateTextBlock.Text = "Last backup on " + this._skydrive.LastBackUpDateTime;
                        }
                        else
                        {
                            dateTextBlock.Text = "No previous backup available to restore.";
                        }
                        break;
                }


            }
        }

        private void ButtonBackupClick(object sender, RoutedEventArgs e)
        {
            if (_skydrive == null)
            {
                MessageBox.Show("You must sign in first.");
            }
            else
            {
                if (MessageBox.Show("This will overwrite your old backup file.", "Backup?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    this._progressIndicator.Text = "Uploading backup...";
                    this._progressIndicator.IsVisible = true;

                    var stream = IsoStorage.GetStream();

                    this._skydrive.Upload(stream, stream.Dispose);
                }
            }
        }

        private void ButtonRestoreClick(object sender, RoutedEventArgs e)
        {
            if (_skydrive == null)
            {
                MessageBox.Show("You must sign in first.");
            }
            else
            {
                if (MessageBox.Show("This will overwrite your existing weight history.", "Restore?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    this._progressIndicator.Text = "Restoring backup...";
                    this._progressIndicator.IsVisible = true;

                    this._skydrive.Download(IsoStorage.Save);
                }
            }
        }

        #endregion
    }
}