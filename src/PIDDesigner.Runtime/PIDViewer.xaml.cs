using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PIDDesigner.Runtime.Controls;
using PIDDesigner.Runtime.Models;

namespace PIDDesigner.Runtime
{
    /// <summary>
    /// This is the main control that LabVIEW will host.
    /// </summary>
    public partial class PIDViewer : UserControl
    {
        private Dictionary<string, IPIDComponent> _components = new Dictionary<string, IPIDComponent>();

        public PIDViewer()
        {
            InitializeComponent();
            
            // For testing: Add a sample pump so the user can see it works
            var testPump = new PumpControl();
            AddComponent(testPump, "TestPump_01", 100, 100);
        }

        /// <summary>
        /// LabVIEW calls this to update the entire UI.
        /// </summary>
        public void UpdateDeviceStates(DeviceState[] states)
        {
            if (states == null) return;
            
            // Use Dispatcher to ensure UI updates on the correct thread
            this.Dispatcher.Invoke(() =>
            {
                foreach (var state in states)
                {
                    if (_components.ContainsKey(state.ID))
                    {
                        _components[state.ID].UpdateData(state.Value, state.State);
                    }
                }
            });
        }

        // Helper to add a component (for testing or dynamic loading)
        public void AddComponent(UserControl control, string id, double x, double y)
        {
            if (control is IPIDComponent pidComp)
            {
                pidComp.ComponentID = id;
                _components[id] = pidComp;
                
                Canvas.SetLeft(control, x);
                Canvas.SetTop(control, y);
                MainCanvas.Children.Add(control);
            }
        }
    }
}
