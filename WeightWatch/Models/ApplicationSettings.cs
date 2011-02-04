namespace WeightWatch.Models
{
    using WeightWatch.Classes;

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
                if (ClientStorage.Instance["GraphMode"] == null)
                {
                    ClientStorage.Instance["GraphMode"] = GraphMode.Week;
                }

                return (GraphMode)ClientStorage.Instance["GraphMode"];
            }
            set
            {
                ClientStorage.Instance["GraphMode"] = value;
            }
        }

        public static MeasurementSystem DefaultMeasurementSystem
        {
            get
            {
                if (ClientStorage.Instance["MeasurementSystem"] == null)
                {
                    ClientStorage.Instance["MeasurementSystem"] = MeasurementSystem.Imperial;
                }

                return (MeasurementSystem)ClientStorage.Instance["MeasurementSystem"];
            }
            set
            {
                ClientStorage.Instance["MeasurementSystem"] = value;
            }
        }
    }
}
