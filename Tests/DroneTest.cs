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
            //Arrange
            var drone = new Drone();

            //Assert
            Assert.AreEqual(false, drone.Active);

            //Act
            var result = drone.DispatchCommand("S");
            //Assert
            Assert.AreEqual(true, drone.Active);
            Assert.AreEqual("Drone Started.", result);
        }

        [TestMethod]
        public void TestDispatchReStartCommand()
        {
            //Arrange
            var drone = new Drone();

            //Act
            var result = drone.DispatchCommand("R");

            //Assert
            Assert.AreEqual(true, drone.Active);
            Assert.AreEqual("Drone restarted. Please set boundary before attempting any action commands.", result);
        }

        [TestMethod]
        public void TestDispatchToggleLightsCommand()
        {
            //Test that action command won't work if Boundary dimensions aren't set
            //Arrange
            var drone = new Drone();
            drone.BoundaryIsSet = false;

            //Act
            var result1 = drone.DispatchCommand("T");

            //Assert
            Assert.AreEqual("Please set boundary before attempting any action commands.", result1);



            //Test that action command will work if Boundary dimensions are set
            //Arrange
            drone.BoundaryIsSet = true;

            //Act
            var result2 = drone.DispatchCommand("T");

            //Assert
            Assert.AreEqual(true, drone.LightOn);
            Assert.AreEqual("Lights toggled.", result2);
        }
    }
}

