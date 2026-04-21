using Microsoft.VisualStudio.TestTools.UnitTesting;
using PIDDesigner.Runtime.Models;

namespace PIDDesigner.Tests
{
    [TestClass]
    public class DeviceStateTests
    {
        [TestMethod]
        public void DeviceState_Initialization_SetsPropertiesCorrectly()
        {
            var state = new DeviceState { ID = "Pump1", Type = 1, Value = 50.5, State = 1 };
            Assert.AreEqual("Pump1", state.ID);
            Assert.AreEqual(1, state.Type);
            Assert.AreEqual(50.5, state.Value);
            Assert.AreEqual(1, state.State);
        }
    }
}
