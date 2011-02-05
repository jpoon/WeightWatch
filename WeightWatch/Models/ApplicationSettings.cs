namespace WeightWatch.Models
{
    using WeightWatch.Classes;
    using System.IO.IsolatedStorage;

    public static class ApplicationSettings
    {
        public enum GraphMode
        {
            Week = 1,
            Month = 2,
            Year = 3
        };

        public static GraphMode DefaultGraphMode
        {
            get
            {
                GraphMode defaultGraphMode;
                if (!IsolatedStorageSettings.ApplicationSettings.TryGetValue("GraphMode", out defaultGraphMode))
                {
                    defaultGraphMode = GraphMode.Week;
                    IsolatedStorageSettings.ApplicationSettings.Add("GraphMode", defaultGraphMode);
                }

                return defaultGraphMode;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["GraphMode"] = value;
            }
        }

        public static MeasurementSystem DefaultMeasurementSystem
        {
            get
            {
                MeasurementSystem defaultMeasurementSystem;
                if (!IsolatedStorageSettings.ApplicationSettings.TryGetValue("MeasurementSystem", out defaultMeasurementSystem))
                {
                    defaultMeasurementSystem = MeasurementSystem.Imperial;
                    IsolatedStorageSettings.ApplicationSettings.Add("MeasurementSystem", defaultMeasurementSystem);
                }

                return defaultMeasurementSystem;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["MeasurementSystem"] = value;
            }
        }
    }
}
