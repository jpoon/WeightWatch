using System;
using WeightWatch.Models;

namespace WeightWatch
{
    public class WeightViewModel
    {
        WeightModel _data;
        ApplicationSettings _appSettings;

        public WeightViewModel(WeightModel data)
        {
            _data = data;
            _appSettings = new ApplicationSettings();
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
                MeasurementSystem defaultSystem = _appSettings.DefaultMeasurementSystem;
                return Math.Ceiling(_data.Weight).ToString() + " " + MeasurementFactory.GetSystem(defaultSystem).Abbreviation;
            }
        }

        public float Weight
        {
            get
            {
                MeasurementSystem defaultSystem = _appSettings.DefaultMeasurementSystem;
                return MeasurementFactory.GetSystem(_data.MeasurementUnit).ConvertTo(defaultSystem, _data.Weight);
            }
        }

        #endregion

    }
}