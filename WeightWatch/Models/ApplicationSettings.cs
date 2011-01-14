﻿using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using WeightWatch.Classes;

namespace WeightWatch.Models
{
    public class ApplicationSettings
    {
        public enum GraphMode
        {
            Week,
            Month,
            Year
        };

        public static GraphMode DefaultGraphMode
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
