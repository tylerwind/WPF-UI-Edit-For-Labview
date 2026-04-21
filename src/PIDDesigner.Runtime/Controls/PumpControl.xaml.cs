using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace PIDDesigner.Runtime.Controls
{
    public partial class PumpControl : UserControl, IPIDComponent
    {
        public string ComponentID { get; set; }
        private Storyboard _rotateAnim;

        public PumpControl()
        {
            InitializeComponent();
            // _rotateAnim = (Storyboard)this.Resources["RotateAnimation"]; // Removed for static image
        }

        public void UpdateData(double value, int state)
        {
            // State 1 = Running, 0 = Stopped
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
