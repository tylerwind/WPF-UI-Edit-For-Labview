using System.Windows.Controls;

namespace PIDDesigner.Runtime.Controls
{
    public partial class ValveControl : UserControl, IPIDComponent
    {
        public string ComponentID { get; set; }
        public System.Collections.Generic.List<AnchorPoint> Anchors { get; } = new System.Collections.Generic.List<AnchorPoint>
        {
            new AnchorPoint { Name = "Inlet", OffsetX = 0, OffsetY = 30 },
            new AnchorPoint { Name = "Outlet", OffsetX = 60, OffsetY = 30 }
        };

        public ValveControl()
        {
            InitializeComponent();
        }

        public void UpdateData(double value, int state)
        {
            // State 1 = Open, 0 = Closed
            if (state == 1)
            {
                RunningIndicator.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                RunningIndicator.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
