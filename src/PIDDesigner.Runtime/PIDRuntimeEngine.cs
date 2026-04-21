using System.Collections.Generic;
using PIDDesigner.Runtime.Models;

namespace PIDDesigner.Runtime
{
    public class PIDRuntimeEngine
    {
        private Dictionary<string, DeviceState> _stateCache = new Dictionary<string, DeviceState>();

        public void UpdateDeviceStates(DeviceState[] states)
        {
            if (states == null) return;
            foreach (var state in states)
            {
                _stateCache[state.ID] = state;
                // Future: Raise event to update UI components here
            }
        }

        public DeviceState GetState(string id)
        {
            return _stateCache.ContainsKey(id) ? _stateCache[id] : default(DeviceState);
        }
    }
}
