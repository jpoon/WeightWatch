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

namespace WeightWatch.Classes
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Runtime.Serialization;
    using System.Xml.Linq;
    using WeightWatch.Models;

    public class IsoStorage
    {
        private const string WeightFile = "weight.xml";

        public static List<WeightModel> GetContent()
        {
            List<WeightModel> contents = null;
            using (var isoStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var stream = isoStorage.OpenFile(WeightFile, FileMode.OpenOrCreate))
                {
                    if (stream.Length > 0)
                    {
                        var dcs = new DataContractSerializer(typeof(List<WeightModel>));
                        contents = dcs.ReadObject(stream) as List<WeightModel>;
                    }
                }
            }

            return contents ?? new List<WeightModel>();
        }

        public static Stream GetStream()
        {
            Stream stream;
            using (var isoStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                stream = new IsolatedStorageFileStream(WeightFile, FileMode.OpenOrCreate, isoStorage);
            }
            return stream;
        }

        public static void Save(List<WeightModel> weightList)
        {
            using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var stream = new IsolatedStorageFileStream(WeightFile, FileMode.Create, isf))
                {
                    var dcs = new DataContractSerializer(typeof(List<WeightModel>));
                    dcs.WriteObject(stream, weightList);
                }
            }
        }

        public static void Save(Stream streamToSave)
        {
            using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var stream = new IsolatedStorageFileStream(WeightFile, FileMode.Create, isf))
                {
                    streamToSave.CopyTo(stream);
                }
            }
        }
    }
}
