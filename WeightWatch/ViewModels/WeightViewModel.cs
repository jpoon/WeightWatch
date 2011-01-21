using System;
using WeightWatch.Models;

namespace WeightWatch
{
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
                MeasurementSystem defaultSystem = (MeasurementSystem)ApplicationSettings.DefaultMeasurementSystem;
                return Math.Ceiling(this.Weight).ToString() + " " + MeasurementFactory.GetSystem(defaultSystem).Abbreviation;
            }
        }

        public float Weight
        {
            get
            {
                MeasurementSystem defaultSystem = (MeasurementSystem)ApplicationSettings.DefaultMeasurementSystem;
                return MeasurementFactory.GetSystem(_data.MeasurementUnit).ConvertTo(defaultSystem, _data.Weight);
            }
        }

        #endregion

    }
}