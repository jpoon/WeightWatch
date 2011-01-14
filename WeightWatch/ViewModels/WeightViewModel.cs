using System;
using WeightWatch.Models;

namespace WeightWatch
{
    public class WeightViewModel
    {
        WeightModel _data;
        float _weightInDefaultSystem;
        MeasurementSystem _defaultSystem;

        public WeightViewModel(WeightModel data)
        {
            _data = data;

            _defaultSystem = (MeasurementSystem)ApplicationSettings.DefaultMeasurementSystem;
            _weightInDefaultSystem = MeasurementFactory.GetSystem(_data.MeasurementUnit).ConvertTo(_defaultSystem, _data.Weight);
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
                return Math.Ceiling(_weightInDefaultSystem).ToString() + " " + MeasurementFactory.GetSystem(_defaultSystem).Abbreviation;
            }
        }

        public float Weight
        {
            get
            {
                return _weightInDefaultSystem;
            }
        }

        #endregion

    }
}