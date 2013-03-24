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

        public string ErrorMessage { get; private set; }
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

            _liveClient.GetCompleted += LiveClientGetFoldersCompleted;
            _liveClient.GetAsync("me/skydrive/files?filter=folders,albums");
        }

        public void Upload(Stream stream, Action backupCompleted = null)
        {
            this.Status = SkydriveStatus.UploadPending;
            this._backupCompleted = backupCompleted;

            _liveClient.UploadCompleted += LiveClientUploadCompleted;
            _liveClient.UploadAsync(_backupPathId, BackupFileName, stream, OverwriteOption.Overwrite, null);
        }

        public void Download(Action<Stream> downloadCompleted)
        {
            if (this._backupFileId == null)
            {
                throw new InvalidOperationException("Backup file does not exist");
            }

            this._downloadCompleted = downloadCompleted;
            this.Status = SkydriveStatus.DownloadPending;

            _liveClient.DownloadCompleted += LiveClientDownloadCompleted;
            _liveClient.DownloadAsync(this._backupFileId + "/content");
        }

        private void SkydriveError(string errorMessage)
        {
            this.ErrorMessage = errorMessage;
            this.Status = SkydriveStatus.Error;
        }

        protected void OnPropertyChanged(string name)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #region Event Handlers

        private void LiveClientDownloadCompleted(object sender, LiveDownloadCompletedEventArgs e)
        {
            _liveClient.DownloadCompleted -= LiveClientDownloadCompleted;

            if (e.Error != null)
            {
                SkydriveError(e.Error.Message);
                return;
            }

            this.Status = SkydriveStatus.DownloadCompleted;

            this._downloadCompleted.Invoke(e.Result);
        }

        private void LiveClientUploadCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            _liveClient.UploadCompleted -= LiveClientUploadCompleted;

            if (e.Error != null)
            {
                SkydriveError(e.Error.Message);
                return;
            }

            this.Status = SkydriveStatus.UploadCompleted;

            if (this._backupCompleted != null)
            {
                this._backupCompleted.Invoke();
            }

            this.Status = SkydriveStatus.GetFilesPending;
            _liveClient.GetCompleted += LiveClientGetFilesCompleted;
            _liveClient.GetAsync(_backupPathId + "/files");
        }

        private void LiveClientGetFoldersCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            _liveClient.GetCompleted -= LiveClientGetFoldersCompleted;

            if (e.Error != null)
            {
                SkydriveError(e.Error.Message);
                return;
            }
            
            this.Status = SkydriveStatus.GetFoldersComplete;

            var folderData = (Dictionary<string, object>)e.Result;
            var folders = (List<object>)folderData["data"];

            foreach (var folder in folders.Cast<Dictionary<string, object>>().Where(folder => folder["name"].ToString() == BackupFolderName))
            {
                _backupPathId = folder["id"].ToString();
            }

            if (String.IsNullOrEmpty(_backupPathId))
            {
                // create folder
                var skyDriveFolderData = new Dictionary<string, object> { {"name", BackupFolderName }};

                this.Status = SkydriveStatus.CreateFolderPending;
                _liveClient.PostCompleted += LiveClientCreateFolderCompleted;
                _liveClient.PostAsync("me/skydrive", skyDriveFolderData);
            }
            else
            {
                // otherwise check if the backup file is in the folder
                this.Status = SkydriveStatus.GetFilesPending;
                _liveClient.GetCompleted += LiveClientGetFilesCompleted;
                _liveClient.GetAsync(_backupPathId + "/files");
            }
        }

        private void LiveClientGetFilesCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            _liveClient.PostCompleted -= LiveClientGetFilesCompleted;

            if (e.Error != null)
            {
                SkydriveError(e.Error.Message);
                return;
            }

            var data = (List<object>)e.Result["data"];

            foreach (var content in data.Cast<IDictionary<string, object>>().Where(content => ((string)content["name"]).Equals(BackupFileName)))
            {
                this._backupFileId = (string)content["id"];
                try
                {
                    var date = DateTimeOffset.Parse(((string)content["updated_time"]).Substring(0, 19));
                    LastBackUpDateTime = date.Add(date.Offset).DateTime;
                }
                catch { }
                break;
            }

            this.Status = SkydriveStatus.GetFilesComplete;
        }

        private void LiveClientCreateFolderCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            _liveClient.GetCompleted -= LiveClientCreateFolderCompleted;

            if (e.Error != null)
            {
                SkydriveError(e.Error.Message);
                return;
            }

            this.Status = SkydriveStatus.CreateFolderComplete;

            var folder = (Dictionary<string, object>)e.Result;
            _backupPathId = folder["id"].ToString(); 

        }

        #endregion
    }
}
