using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeightWatch.Models;

namespace WeightWatchUT
{
    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void AlwaysPass()
        {
            Assert.IsTrue(true, "method intended to always pass");
        }

        [TestMethod]
        [Description("This test always fails intentionally")]
        public void AlwaysFail()
        {
            Assert.IsFalse(true, "method intended to always fail");
        }

        [TestMethod]
        [Description("This test always fails intentionally")]
        public void DefaultMode()
        {
            ApplicationSettings.GraphMode graph = ApplicationSettings.DefaultGraphMode;
        }
    }
}
