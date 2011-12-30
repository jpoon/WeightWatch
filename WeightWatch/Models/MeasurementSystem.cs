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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using WeightWatch.Classes;

    public enum MeasurementSystem
    {
        Imperial = 1,
        Metric = 2,
        Stone = 3
    }

    public interface IMeasurementSystem
    {
        MeasurementSystem MeasurementSystem { get; }
        string Abbreviation { get; }
        Double ConvertTo(MeasurementSystem obj, Double weight);
    }

    class Imperial : IMeasurementSystem
    {
        public MeasurementSystem MeasurementSystem
        {
            get { return MeasurementSystem.Imperial;  }
        }

        public string Abbreviation
        {
            get { return "lbs"; }
        }

        public Double ConvertTo(MeasurementSystem system, Double weight)
        {
            switch (system)
            {
                case MeasurementSystem.Imperial:
                    return weight;
                case MeasurementSystem.Metric:
                    // 1 lb = 0.4536 kgs
                    return weight * 0.4536;
                case MeasurementSystem.Stone:
                    // 1 stone = 14 lbs
                    return weight / 14;
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
        public MeasurementSystem MeasurementSystem
        {
            get { return MeasurementSystem.Metric; }
        }

        public string Abbreviation
        {
            get { return "kgs"; }
        }

        public Double ConvertTo(MeasurementSystem system, Double weight)
        {
            switch (system)
            {
                case MeasurementSystem.Imperial:
                    // 1kg = 2.205 lbs
                    return weight * 2.205;
                case MeasurementSystem.Metric:
                    return weight;
                case MeasurementSystem.Stone:
                    // 1kg = 0.157473044 stone
                    return weight * 0.157473044; 
                default:
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Measurement system of type {0} cannot be found",
                            Enum.GetName(typeof(MeasurementSystem), system)));
            }
        }
    }

    class Stone : IMeasurementSystem
    {
        public MeasurementSystem MeasurementSystem
        {
            get { return MeasurementSystem.Stone; }
        }

        public string Abbreviation
        {
            get { return "st"; }
        }

        public Double ConvertTo(MeasurementSystem system, Double weight)
        {
            switch (system)
            {
                case MeasurementSystem.Imperial:
                    return weight * 14;
                case MeasurementSystem.Metric:
                    return weight * 6.35029318;
                case MeasurementSystem.Stone:
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
        private static readonly Dictionary<MeasurementSystem, IMeasurementSystem> InstanceCache = new Dictionary<MeasurementSystem, IMeasurementSystem>();

        public static IMeasurementSystem Get(MeasurementSystem system)
        {
            if (!InstanceCache.ContainsKey(system))
            {
                var systemType = Type.GetType("WeightWatch.Models." + system);
                if (systemType == null)
                {
                    throw new ArgumentException("Unexpected Measurement System of " + system);
                }

                InstanceCache.Add(system, (IMeasurementSystem)Activator.CreateInstance(systemType));
            }

            return InstanceCache[system];
        }

        public static IMeasurementSystem Get(string abbreviation)
        {
            return Get().FirstOrDefault(system => system.Abbreviation.Equals(abbreviation));
        }

        public static IEnumerable<IMeasurementSystem> Get()
        {
            return Helpers.GetAllEnum<MeasurementSystem>().Select(Get);
        }
    }
}
