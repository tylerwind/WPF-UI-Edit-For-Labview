using System.Windows;
using System.Windows.Controls;
using PIDDesigner.Runtime.Controls;

namespace PIDDesigner.App
{
    public partial class DesignerWindow : Window
    {
        public DesignerWindow()
        {
            InitializeComponent();
        }

        private void BtnAddPump_Click(object sender, RoutedEventArgs e)
        {
            var pump = new PumpControl();
            pump.ComponentID = "Pump_" + MainCanvas.Children.Count;
            
            Canvas.SetLeft(pump, 100);
            Canvas.SetTop(pump, 100);
            
            MainCanvas.Children.Add(pump);
        }
    }
}
