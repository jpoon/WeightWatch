using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;

namespace WeightWatch.Models
{
    public class WeightListModel
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
                WeightList.RemoveAt(index);
            }
            WeightList.Add(data);
            WeightList.Sort();
        }

        public void Delete(DateTime date)
        {
            // ToDo: Implement
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
    }
}
