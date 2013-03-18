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
    using System;
    using System.IO;
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

        public Settings()
        {
            InitializeComponent();
            Loaded += Settings_Loaded;
        }

        private void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            // Default Measurement
            Measurement_ListPicker.ItemsSource = Helpers.EnumToStringList(typeof(MeasurementUnit));
            Measurement_ListPicker.SelectionChanged += Measurement_ListPicker_SelectionChanged;

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
            ApplicationSettings.DefaultMeasurementSystem = (MeasurementUnit)Enum.Parse(typeof(MeasurementUnit), (string)Measurement_ListPicker.SelectedItem, true);
        }

        private void Graph_ListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationSettings.DefaultGraphMode = (GraphMode)Enum.Parse(typeof(GraphMode), (string)Graph_ListPicker.SelectedItem, true);
        }

        private void LiveIdSignIn_Button_SessionChanged(object sender, Microsoft.Live.Controls.LiveConnectSessionChangedEventArgs e)
        {
            if (e.Status == LiveConnectSessionStatus.Connected)
            {
                _skydrive = new Skydrive(e.Session);
                _skydrive.PropertyChanged += Skydrive_PropertyChanged;
                infoTextBlock.Text = "Accessing SkyDrive...";
            }
            else
            {
                infoTextBlock.Text = "Not signed in.";
            }
        }

        private void Skydrive_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Status"))
            {
                switch (this._skydrive.Status)
                {
                    case SkydriveStatus.GetFoldersPending:
                        break;
                    case SkydriveStatus.GetFilesPending:
                        dateTextBlock.Text = "Obtaining previous backup...";
                        break;
                    case SkydriveStatus.CreateFolderPending:
                        break;
                    case SkydriveStatus.UploadPending:
                        infoTextBlock.Text = "Uploading backup...";
                        dateTextBlock.Text = "";
                        break;
                    case SkydriveStatus.GetFoldersComplete:
                        break;
                    case SkydriveStatus.CreateFolderComplete:
                    case SkydriveStatus.GetFilesComplete:
                    case SkydriveStatus.UploadCompleted:
                        buttonBackup.IsEnabled = true;
                        infoTextBlock.Text = "Ready to backup.";

                        if (this._skydrive.LastBackUpDateTime != null)
                        {
                            buttonRestore.IsEnabled = true;
                            dateTextBlock.Text = "Last backup on " + this._skydrive.LastBackUpDateTime.ToString();
                        }
                        else
                        {
                            dateTextBlock.Text = "No previous backup available to restore.";
                        }
                        break;
                }
            }
        }

        private void buttonBackup_Click(object sender, RoutedEventArgs e)
        {
            if (_skydrive == null)
            {
                MessageBox.Show("You must sign in first.");
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to backup? This will overwrite your old backup file!", "Backup?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Stream stream = IsoStorage.GetStream();
                    var backupResponse = this._skydrive.Backup(stream, () => stream.Dispose());

                    if (!backupResponse)
                    {
                        MessageBox.Show("Error accessing IsolatedStorage. Please close the app and re-open it, and then try backing up again!", "Backup Failed", MessageBoxButton.OK);
                        infoTextBlock.Text = "Error. Close the app and start again.";
                        dateTextBlock.Text = "";
                    }
                }
            }

        }

        /*
        private void download()
        {
            if (fileID != null)
            {
                infoTextBlock.Text = "Downloading backup...";

                client.DownloadAsync(fileID + "/content");
                client.DownloadCompleted += new EventHandler<LiveDownloadCompletedEventArgs>(client_DownloadCompleted);
            }

            else
                MessageBox.Show("Backup file doesn't exist!", "Error", MessageBoxButton.OK);
        }

        void client_DownloadCompleted(object sender, LiveDownloadCompletedEventArgs e)
        {
            Stream stream = e.Result;

            /*
            //CHANGE "App.MyStorage.Restore" to a method of your own that takes a Stream, deserializes it, updates your local storage,
            //and then returns true if it was successful or false if not successful.
            if (IsoStorage.App.MyStorage.Restore(stream)) //CHANGE THIS
            {
                infoTextBlock.Text = "Restore completed!";
            }

            else
            {
                MessageBox.Show("Restore failed.", "Failure", MessageBoxButton.OK);
            }
        }*/
        private void buttonRestore_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (client == null || client.Session == null)
            {
                MessageBox.Show("You must sign in first.");
            }
            else
            {
                if (MessageBox.Show("Are you sure you want to restore your data? This will overwrite all your current items and settings in the app!", "Restore?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    download();
            }*/
        }

        /*
        //checks all the files in the folder to see if a backup file exists
        void getFiles_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            string fileID = null;
            List<object> data = (List<object>)e.Result["data"];

            dateTextBlock.Text = "Obtaining previous backup...";
            DateTimeOffset date = DateTime.MinValue;

            foreach (IDictionary<string, object> content in data)
            {
                if (((string)content["name"]).Equals(fileName))
                {
                    fileID = (string)content["id"];
                    try
                    {
                        date = DateTimeOffset.Parse(((string)content["updated_time"]).Substring(0, 19));
                    }

                    catch { }

                    break;
                }
            }

            if (fileID != null)
            {
                try
                {
                    dateTextBlock.Text = (date != DateTime.MinValue) ? "Last backup on " + date.Add(date.Offset).DateTime : "Last backup on: unknown";
                }

                catch
                {
                    dateTextBlock.Text = "Last backup on: unknown";
                }

                buttonRestore.IsEnabled = true; //enable restoring since the file exists
            }

            else
                dateTextBlock.Text = "No previous backup available to restore.";
        }
         */

        #endregion
    }
}