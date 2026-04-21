using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using PIDDesigner.Runtime.Models;

namespace PIDDesigner.Runtime
{
    /// <summary>
    /// This is a WinForms wrapper for the WPF PIDViewer.
    /// It ensures LabVIEW's standard .NET Container can see and host the control.
    /// </summary>
    [ToolboxItem(true)]
    [DesignTimeVisible(true)]
    [Guid("A7E8D2C1-3B5F-4D9A-8E2B-5C1F7A8D2E9C")]
    [ComVisible(true)]
    public class PIDViewerHost : UserControl
    {
        private ElementHost _host;
        private PIDViewer _viewer;

        public PIDViewerHost()
        {
            _host = new ElementHost { Dock = DockStyle.Fill };
            _viewer = new PIDViewer();
            _host.Child = _viewer;
            this.Controls.Add(_host);
        }

        /// <summary>
        /// API for LabVIEW to update states.
        /// </summary>
        public void UpdateDeviceStates(DeviceState[] states)
        {
            _viewer.UpdateDeviceStates(states);
        }

        /// <summary>
        /// API for LabVIEW to load a design from XML.
        /// </summary>
        public void LoadConfiguration(string xmlPath)
        {
            _viewer.ImportConfig(xmlPath);
        }

        // Expose the viewer for more advanced access if needed
        public PIDViewer Viewer => _viewer;
    }
}
