namespace WeightWatch.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Runtime.Serialization;

    public class WeightListModel : INotifyCollectionChanged
    {
        public List<WeightModel> WeightList { get; private set; }

        private const string FILE_NAME = "weight.xml";

        static WeightListModel _instance = null;
        static readonly object _singletonLock = new object();

        private WeightListModel() { }

        /// <summary>
        /// WeightHistoryModel object instantiated once and shared thereafter (singleton pattern).
        /// </summary>
        public static WeightListModel GetInstance()
        {
            lock (_singletonLock)
            {
                if (_instance == null)
                {
                    _instance = WeightListModel.LoadFromFile();
                }
                return _instance;
            }
        }

        #region Public Methods

        public void Add(WeightModel data)
        {
            int index = WeightList.BinarySearch(data);
            if (index >= 0)
            {
                WeightModel oldItem = WeightList[index];
                WeightList.RemoveAt(index);
                WeightList.Add(data);
                notifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, data, oldItem, index));
            }
            else
            {
                WeightList.Add(data);
                WeightList.Sort();
                index = WeightList.BinarySearch(data);
                notifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, data, index));
            }
        }

        public void Delete(DateTime date)
        {
            // ToDo: Implement
            notifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
        }

        public void Persist()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(FILE_NAME, FileMode.Create, isf))
                {
                    DataContractSerializer dcs = new DataContractSerializer(typeof(List<WeightModel>));
                    dcs.WriteObject(stream, this.WeightList);
                }
            }
        }

        #endregion

        private static WeightListModel LoadFromFile()
        {
            List<WeightModel> dataList = null;
            using (IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream stream = isoStorage.OpenFile(FILE_NAME, FileMode.OpenOrCreate))
                {
                    if (stream.Length > 0)
                    {
                        DataContractSerializer dcs = new DataContractSerializer(typeof(List<WeightModel>));
                        dataList = dcs.ReadObject(stream) as List<WeightModel>;
                    }
                }
            }

            WeightListModel weightHistory = new WeightListModel();
            if (dataList == null)
            {
                weightHistory.WeightList = new List<WeightModel>();
            }
            else
            {
                weightHistory.WeightList = dataList;
            }

            return weightHistory;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private void notifyCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, args);
            }
        }
    }
}
