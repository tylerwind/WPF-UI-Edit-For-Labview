using System.Windows;
using System.Windows.Controls;
using PIDDesigner.Runtime.Controls;

namespace PIDDesigner.App
{
    public partial class DesignerWindow : Window
    {
        private string _sourceId = null;
        private bool _isConnecting = false;

        public DesignerWindow()
        {
            InitializeComponent();
            ViewerControl.ComponentClicked += ViewerControl_ComponentClicked;
        }

        private void ViewerControl_ComponentClicked(string id)
        {
            if (!_isConnecting) return;

            if (_sourceId == null)
            {
                _sourceId = id;
                TxtStatus.Text = "Source: " + id + ". Click Target.";
            }
            else if (_sourceId != id)
            {
                string pipeId = "Pipe_" + System.Guid.NewGuid().ToString().Substring(0, 4);
                // Connect Outlet of Source to Inlet of Target
                ViewerControl.AddPipe(pipeId, _sourceId, "Outlet", id, "Inlet");
                
                _sourceId = null;
                _isConnecting = false;
                TxtStatus.Text = "Connected!";
            }
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            _isConnecting = true;
            _sourceId = null;
            TxtStatus.Text = "Select Source Component...";
        }

        private void BtnAddPump_Click(object sender, RoutedEventArgs e)
        {
            var pump = new PumpControl();
            string id = "Pump_" + System.Guid.NewGuid().ToString().Substring(0, 4);
            
            ViewerControl.AddComponent(pump, id, 100, 100);
        }

        private void BtnAddValve_Click(object sender, RoutedEventArgs e)
        {
            var valve = new ValveControl();
            string id = "Valve_" + System.Guid.NewGuid().ToString().Substring(0, 4);
            
            ViewerControl.AddComponent(valve, id, 200, 100);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "config.xml");
            ViewerControl.ExportConfig(path);
            MessageBox.Show("Design saved to: " + path);
        }
    }
}
