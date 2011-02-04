﻿namespace WeightWatch.ViewModels
{
    using System;
    using WeightWatch.Models;

    public class WeightViewModel
    {
        WeightModel _data;

        public WeightViewModel(WeightModel data)
        {
            _data = data;
        }

        #region Properties

        public string DateStr
        {
            get
            {
                return _data.Date.ToString("MMM. d, yyyy");
            }
        }

        public string DateStr_MonthYear
        {
            get
            {
                return _data.Date.ToString("MMM yyyy");
            }
        }

        public DateTime Date
        {
            get
            {
                return _data.Date;
            }
        }

        public string WeightStr
        {
            get
            {
                MeasurementSystem defaultSystem = ApplicationSettings.DefaultMeasurementSystem;
                return Math.Round(this.Weight).ToString() + " " + MeasurementFactory.GetSystem(defaultSystem).Abbreviation;
            }
        }

        public float Weight
        {
            get
            {
                MeasurementSystem defaultSystem = ApplicationSettings.DefaultMeasurementSystem;
                return MeasurementFactory.GetSystem(_data.MeasurementUnit).ConvertTo(defaultSystem, _data.Weight);
            }
        }

        #endregion
    }
}