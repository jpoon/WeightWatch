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

namespace WeightWatch.Models
{
    using System;
    using System.Globalization;

    public enum MeasurementSystem
    {
        Imperial = 1,
        Metric = 2
    }

    public interface IMeasurementSystem
    {
        string Abbreviation { get; }
        Decimal ConvertTo(MeasurementSystem obj, Decimal weight);
    }

    class Imperial : IMeasurementSystem
    {
        public string Abbreviation
        {
            get { return "lbs"; }
        }

        public Decimal ConvertTo(MeasurementSystem system, Decimal weight)
        {
            switch (system)
            {
                case MeasurementSystem.Imperial:
                    return weight;
                case MeasurementSystem.Metric:
                    // 1 lb = 0.4536 kgs
                    return weight * 0.4536m;
                default:
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Measurement system of type {0} cannot be found",
                            Enum.GetName(typeof(MeasurementSystem), system)));
            }
        }
    }

    class Metric : IMeasurementSystem
    {
        public string Abbreviation
        {
            get { return "kgs"; }
        }

        public Decimal ConvertTo(MeasurementSystem system, Decimal weight)
        {
            switch (system)
            {
                case MeasurementSystem.Imperial:
                    // 1kg = 2.205 lbs
                    return weight * 2.205m;
                case MeasurementSystem.Metric:
                    return weight;
                default:
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Measurement system of type {0} cannot be found",
                            Enum.GetName(typeof(MeasurementSystem), system)));
            }
        }
    }

    public static class MeasurementFactory
    {
        public static IMeasurementSystem GetSystem(MeasurementSystem type)
        {
            IMeasurementSystem measurementSystem = null;
            switch (type)
            {
                case MeasurementSystem.Imperial:
                    measurementSystem = new Imperial();
                    break;
                case MeasurementSystem.Metric:
                    measurementSystem = new Metric();
                    break;
                default:
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Measurement system of type {0} cannot be found",
                            Enum.GetName(typeof(MeasurementSystem), type)));
            }

            return measurementSystem;
        }
    }
}
