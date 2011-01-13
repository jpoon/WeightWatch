using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace WeightWatch.Models
{
    [DataContract(Name = "WeightModel")]
    public class WeightModel : IComparable<WeightModel>
    {
        private float _weight;
        private DateTime _date;
        private MeasurementSystem _unit;

        [DataMember]
        public float Weight
        {
            get
            {
                return _weight;
            }
            set
            {
                _weight = value;
            }
        }

        [DataMember]
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
            }
        }

        [DataMember]
        public MeasurementSystem MeasurementUnit
        {
            get
            {
                return _unit;
            }
            set
            {
                _unit = value;
            }
        }

        public WeightModel(float weight, DateTime date, MeasurementSystem unit)
        {
            this._weight = weight;
            this._date = date;
            this._unit = unit;
        }

        public int CompareTo(WeightModel obj)
        {
            // reverse chronological order
            return -(this.Date.CompareTo(obj.Date));
        }

    }
}
