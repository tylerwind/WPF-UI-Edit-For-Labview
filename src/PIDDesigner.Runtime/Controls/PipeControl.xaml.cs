using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PIDDesigner.Runtime.Controls
{
    public partial class PipeControl : UserControl, IPIDComponent
    {
        public string ComponentID { get; set; }
        public List<AnchorPoint> Anchors { get; } = new List<AnchorPoint>(); // Pipes don't usually have anchors

        private Point _startPoint;
        private Point _endPoint;

        public PipeControl()
        {
            InitializeComponent();
        }

        public void SetPoints(Point start, Point end)
        {
            _startPoint = start;
            _endPoint = end;
            UpdatePath();
        }

        private void UpdatePath()
        {
            PipePath.Points.Clear();
            PipePath.Points.Add(_startPoint);

            // Simple Orthogonal Routing (Z-shape)
            double midX = _startPoint.X + (_endPoint.X - _startPoint.X) / 2;
            PipePath.Points.Add(new Point(midX, _startPoint.Y));
            PipePath.Points.Add(new Point(midX, _endPoint.Y));
            
            PipePath.Points.Add(_endPoint);
        }

        public void UpdateData(double value, int state)
        {
            // state 1 = Active (Flow), 0 = Inactive
            PipePath.Stroke = (state == 1) ? Brushes.DodgerBlue : Brushes.LightGray;
        }
    }
}
