using System;
using System.Collections.Generic;
using System.Reflection;

namespace WeightWatch.Models
{
    public enum MeasurementSystem
    {
        Imperial = 1,
        Metric = 2
    }

    public interface IMeasurementSystem
    {
        string Abbreviation { get; }
        float ConvertTo(MeasurementSystem obj, float weight);
    }

    class Imperial : IMeasurementSystem
    {
        public string Abbreviation
        {
            get { return "lbs"; }
        }

        public float ConvertTo(MeasurementSystem system, float weight)
        {
            switch (system)
            {
                case MeasurementSystem.Imperial:
                    return weight;
                case MeasurementSystem.Metric:
                    // 1 lb = 0.4536 kgs
                    return weight * 0.4536F;
                default:
                    throw new ArgumentException(string.Format("Measurement system of type {0} cannot be found", Enum.GetName(typeof(MeasurementSystem), system)));
            }
        }
    }

    class Metric : IMeasurementSystem
    {
        public string Abbreviation
        {
            get { return "kgs"; }
        }

        public float ConvertTo(MeasurementSystem system, float weight)
        {
            switch (system)
            {
                case MeasurementSystem.Imperial:
                    // 1kg = 2.205 lbs
                    return weight * 2.205F;
                case MeasurementSystem.Metric:
                    return weight;
                default:
                    throw new ArgumentException(string.Format("Measurement system of type {0} cannot be found", Enum.GetName(typeof(MeasurementSystem), system)));
            }
        }
    }

    public class MeasurementFactory
    {
        public const MeasurementSystem DefaultMeasurementUnit = MeasurementSystem.Imperial;

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
                    throw new ArgumentException(string.Format("Measurement system of type {0} cannot be found", Enum.GetName(typeof(MeasurementSystem), type)));
            }

            return measurementSystem;
        }

        public static List<MeasurementSystem> GetAllMeasurementSystem()
        {
            List<MeasurementSystem> list = new List<MeasurementSystem>();
            Type enumType = typeof(MeasurementSystem);
            FieldInfo[] enumDetail = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo item in enumDetail)
            {
                try
                {
                    list.Add((MeasurementSystem)Enum.Parse(enumType, item.Name, true));
                }
                catch (Exception) { }
            }
            return list;
        }
    }
}
