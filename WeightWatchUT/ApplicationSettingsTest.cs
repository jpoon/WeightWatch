using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeightWatch.Models;
using System;

namespace WeightWatchUT
{
    [TestClass]
    public class ApplicationSettingsTest
    {
        [TestMethod]
        [Description("default graph mode")]
        public void DefaultGraphMode()
        {
            var graphMode = ApplicationSettings.DefaultGraphMode;
            Assert.AreEqual(graphMode, ApplicationSettings.GraphMode.Week);
        }

        [TestMethod]
        [Description("get/set graph mode")]
        public void GetSetGraphMode()
        {
            // Week
            ApplicationSettings.DefaultGraphMode = ApplicationSettings.GraphMode.Week;
            var graphMode = ApplicationSettings.DefaultGraphMode;
            Assert.AreEqual(graphMode, ApplicationSettings.GraphMode.Week);

            // Month
            ApplicationSettings.DefaultGraphMode = ApplicationSettings.GraphMode.Month;
            graphMode = ApplicationSettings.DefaultGraphMode;
            Assert.AreEqual(graphMode, ApplicationSettings.GraphMode.Month);

            // Year
            ApplicationSettings.DefaultGraphMode = ApplicationSettings.GraphMode.Year;
            graphMode = ApplicationSettings.DefaultGraphMode;
            Assert.AreEqual(graphMode, ApplicationSettings.GraphMode.Year);
        }

        [TestMethod]
        [Description("default measurement system")]
        public void DefaultMeasurementSystem()
        {
            var measurementSystem = ApplicationSettings.DefaultMeasurementSystem;
            Assert.AreEqual(measurementSystem, MeasurementSystem.Imperial);
        }

        [TestMethod]
        [Description("get/set measurement system")]
        public void GetSetMeasurementSystem()
        {
            // Imperial
            ApplicationSettings.DefaultMeasurementSystem = MeasurementSystem.Imperial;
            var measurementSystem = ApplicationSettings.DefaultMeasurementSystem;
            Assert.AreEqual(measurementSystem, MeasurementSystem.Imperial);

            // Metric
            ApplicationSettings.DefaultMeasurementSystem = MeasurementSystem.Metric;
            measurementSystem = ApplicationSettings.DefaultMeasurementSystem;
            Assert.AreEqual(measurementSystem, MeasurementSystem.Metric);
        }
    }
}
