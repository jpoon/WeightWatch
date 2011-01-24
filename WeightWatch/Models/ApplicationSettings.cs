using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using WeightWatch.Classes;
using System.ComponentModel;

namespace WeightWatch.Models
{
    public class ApplicationSettings : INotifyPropertyChanged
    {
        public enum GraphMode
        {
            Week = 1,
            Month = 2,
            Year = 3
        };

        public GraphMode DefaultGraphMode
        {
            get
            {
                if (ClientStorage.Instance["GraphMode"] == null)
                {
                    ClientStorage.Instance["GraphMode"] = GraphMode.Week;
                }

                return (GraphMode)ClientStorage.Instance["GraphMode"];
            }
            set
            {
                ClientStorage.Instance["GraphMode"] = value;
            }
        }

        public MeasurementSystem DefaultMeasurementSystem
        {
            get
            {
                if (ClientStorage.Instance["MeasurementSystem"] == null)
                {
                    ClientStorage.Instance["MeasurementSystem"] = MeasurementSystem.Imperial;
                }

                return (MeasurementSystem)ClientStorage.Instance["MeasurementSystem"];
            }
            set
            {
                ClientStorage.Instance["MeasurementSystem"] = value;
                NotifyPropertyChanged("DefaultMeasurementSystem");
            }
        }

        #region Event Handlers

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }
}
