using Microsoft.VisualStudio.TestTools.UnitTesting;
using PIDDesigner.Runtime;
using PIDDesigner.Runtime.Models;

namespace PIDDesigner.Tests
{
    [TestClass]
    public class PIDRuntimeEngineTests
    {
        [TestMethod]
        public void UpdateDeviceStates_ReceivesArray_UpdatesInternalCache()
        {
            var engine = new PIDRuntimeEngine();
            var states = new[] { new DeviceState { ID = "P1", State = 1 } };
            engine.UpdateDeviceStates(states);
            Assert.AreEqual(1, engine.GetState("P1").State);
        }
    }
}
