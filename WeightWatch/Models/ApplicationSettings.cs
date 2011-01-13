using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using WeightWatch.Classes;

namespace WeightWatch.Models
{
    public class ApplicationSettings
    {
        public static MeasurementSystem DefaultMeasurementSystem
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
            }
        }
    }
}
