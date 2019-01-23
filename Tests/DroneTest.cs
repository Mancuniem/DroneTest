using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusinessLogic;

namespace UnitTestProject1
{
    [TestClass]
    public class DroneTest
    {
        [TestMethod]
        public void TestDispatchStartCommand()
        {
            var drone = new Drone();

            Assert.AreEqual(false, drone.Active);

            var result = drone.DispatchCommand("S");

            Assert.AreEqual(true, drone.Active);
            Assert.AreEqual("Drone Started.", result);
        }
    }
}
