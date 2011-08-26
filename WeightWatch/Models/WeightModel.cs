﻿/*
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

namespace WeightWatch.Models
{
    using System;
    using System.Runtime.Serialization;

    [DataContract(Name = "WeightModel")]
    public class WeightModel : IComparable<WeightModel>
    {
        [DataMember]
        public double Weight { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public MeasurementSystem MeasurementUnit { get; set; }

        public WeightModel(Double weight, DateTime date, MeasurementSystem unit)
        {
            this.Weight = weight;
            this.Date = date;
            this.MeasurementUnit = unit;
        }

        public int CompareTo(WeightModel other)
        {
            if (other != null)
            {
                // reverse chronological order
                return -(this.Date.CompareTo(other.Date));
            }
            else
            {
                return 0;
            }
        }

    }
}
