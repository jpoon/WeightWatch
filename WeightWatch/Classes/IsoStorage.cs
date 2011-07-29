namespace WeightWatch.Classes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Runtime.Serialization;
    using WeightWatch.Models;


    public class IsoStorage
    {
        private const string FILE_NAME = "weight.xml";

        public static List<WeightModel> LoadFile()
        {
            List<WeightModel> contents = null;
            using (IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream stream = isoStorage.OpenFile(FILE_NAME, FileMode.OpenOrCreate))
                {
                    if (stream.Length > 0)
                    {
                        DataContractSerializer dcs = new DataContractSerializer(typeof(List<WeightModel>));
                        contents = dcs.ReadObject(stream) as List<WeightModel>;
                    }
                }
            }

            return contents ?? new List<WeightModel>();
        }

        public static void Save(List<WeightModel> weightList)
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(FILE_NAME, FileMode.Create, isf))
                {
                    DataContractSerializer dcs = new DataContractSerializer(typeof(List<WeightModel>));
                    dcs.WriteObject(stream, weightList);
                }
            }
        }

    }
}
