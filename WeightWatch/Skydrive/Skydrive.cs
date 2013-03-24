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

using System.Linq;

namespace WeightWatch.Skydrive
{
    using Microsoft.Live;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;

    public class Skydrive : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private SkydriveStatus _status = SkydriveStatus.None;
        public SkydriveStatus Status 
        { 
            get 
            { 
                return _status;
            }

            private set 
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        public DateTime? LastBackUpDateTime { get; private set; }

        private readonly LiveConnectClient _liveClient;
        private const string BackupFileName = "WeightWatch.xml";
        private string _backupFileId = string.Empty;
        private const string BackupFolderName = "WeightWatch"; //the folder name for backups
        private string _backupPathId = string.Empty; //the id of the folder name for backups
        private Action _backupCompleted;
        private Action<Stream> _downloadCompleted;

        public Skydrive(LiveConnectSession session)
        {
            _liveClient = new LiveConnectClient(session);

            this.Status = SkydriveStatus.GetFoldersPending;
            LastBackUpDateTime = null;

            _liveClient.GetCompleted += LiveClient_GetFoldersCompleted;
            _liveClient.GetAsync("me/skydrive/files?filter=folders,albums");
        }

        public bool Upload(Stream stream, Action backupCompleted = null)
        {
            var isSuccess = true;
            this.Status = SkydriveStatus.UploadPending;
            this._backupCompleted = backupCompleted;

            try
            {
                _liveClient.UploadCompleted += LiveClient_UploadCompleted;
                _liveClient.UploadAsync(_backupPathId, BackupFileName, stream, OverwriteOption.Overwrite, null);
            }
            catch (Exception e)
            {
                // log
                isSuccess = false;
            }

            return isSuccess;
        }

        public void Download(Action<Stream> downloadCompleted)
        {
            if (this._backupFileId == null)
            {
                throw new InvalidOperationException("Backup file does not exist");
            }

            this._downloadCompleted = downloadCompleted;
            this.Status = SkydriveStatus.DownloadPending;

            _liveClient.DownloadCompleted += LiveClient_DownloadCompleted;
            _liveClient.DownloadAsync(this._backupFileId + "/content");
        }

        protected void OnPropertyChanged(string name)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private void LiveClient_DownloadCompleted(object sender, LiveDownloadCompletedEventArgs e)
        {
            _liveClient.DownloadCompleted -= LiveClient_DownloadCompleted;
            this.Status = SkydriveStatus.DownloadCompleted;

            this._downloadCompleted.Invoke(e.Result);
        }

        #region Event Handlers

        private void LiveClient_UploadCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            _liveClient.UploadCompleted -= LiveClient_UploadCompleted;
            Console.WriteLine(e.RawResult);
            this.Status = SkydriveStatus.UploadCompleted;

            if (this._backupCompleted != null)
            {
                this._backupCompleted.Invoke();
            }

            this.Status = SkydriveStatus.GetFilesPending;
            _liveClient.GetCompleted += LiveClient_GetFilesCompleted;
            _liveClient.GetAsync(_backupPathId + "/files");
        }

        private void LiveClient_GetFoldersCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            _liveClient.GetCompleted -= LiveClient_GetFoldersCompleted;
            this.Status = SkydriveStatus.GetFoldersComplete;

            var folderData = (Dictionary<string, object>)e.Result;
            var folders = (List<object>)folderData["data"];

            foreach (var item in folders)
            {
                var folder = (Dictionary<string, object>)item;
                if (folder["name"].ToString() == BackupFolderName)
                {
                    _backupPathId = folder["id"].ToString();
                }
            }

            if (String.IsNullOrEmpty(_backupPathId))
            {
                // create folder
                var skyDriveFolderData = new Dictionary<string, object>();
                skyDriveFolderData.Add("name", BackupFolderName);

                this.Status = SkydriveStatus.CreateFolderPending;
                _liveClient.PostCompleted += LiveClient_CreateFolderCompleted;
                _liveClient.PostAsync("me/skydrive", skyDriveFolderData);
            }
            else
            {
                // otherwise check if the backup file is in the folder
                this.Status = SkydriveStatus.GetFilesPending;
                _liveClient.GetCompleted += LiveClient_GetFilesCompleted;
                _liveClient.GetAsync(_backupPathId + "/files");
            }
        }

        private void LiveClient_GetFilesCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            _liveClient.PostCompleted -= LiveClient_GetFilesCompleted;

            var data = (List<object>)e.Result["data"];

            DateTimeOffset date;
            foreach (var content in data.Cast<IDictionary<string, object>>().Where(content => ((string)content["name"]).Equals(BackupFileName)))
            {
                this._backupFileId = (string)content["id"];
                try
                {
                    date = DateTimeOffset.Parse(((string)content["updated_time"]).Substring(0, 19));
                    LastBackUpDateTime = date.Add(date.Offset).DateTime;
                }
                catch { }
                break;
            }

            this.Status = SkydriveStatus.GetFilesComplete;
        }

        private void LiveClient_CreateFolderCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            _liveClient.GetCompleted -= LiveClient_CreateFolderCompleted;

            if (e.Error == null)
            {
                var folder = (Dictionary<string, object>)e.Result;
                _backupPathId = folder["id"].ToString(); 
            }
            else
            {
                //MessageBox.Show(e.Error.Message);
            }

            this.Status = SkydriveStatus.CreateFolderComplete;
        }

        #endregion
    }
}
