using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Xml.Serialization;
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
        private List<PipeConnection> _connections = new List<PipeConnection>();
        public event Action<string> ComponentClicked;

        private bool _isDragging = false;
        private Point _clickPosition;
        private UIElement _dragElement;

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
                
                // Enable dragging
                control.MouseLeftButtonDown += Component_MouseLeftButtonDown;
                control.MouseMove += Component_MouseMove;
                control.MouseLeftButtonUp += Component_MouseLeftButtonUp;
                control.Cursor = Cursors.Hand;

                MainCanvas.Children.Add(control);
            }
        }

        public void ExportConfig(string filePath)
        {
            var config = new PIDConfig();
            foreach (UIElement child in MainCanvas.Children)
            {
                if (child is IPIDComponent pidComp)
                {
                    config.Components.Add(new ComponentConfig
                    {
                        ID = pidComp.ComponentID,
                        Type = pidComp.GetType().Name,
                        X = Canvas.GetLeft(child),
                        Y = Canvas.GetTop(child)
                    });
                }
            }

            foreach (var conn in _connections)
            {
                config.Connections.Add(new PipeConfig
                {
                    ID = conn.PipeID,
                    SourceID = conn.SourceComponentID,
                    SourceAnchor = conn.SourceAnchorName,
                    TargetID = conn.TargetComponentID,
                    TargetAnchor = conn.TargetAnchorName
                });
            }

            XmlSerializer serializer = new XmlSerializer(typeof(PIDConfig));
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                serializer.Serialize(fs, config);
            }
        }

        public void ImportConfig(string filePath)
        {
            if (!File.Exists(filePath)) return;

            XmlSerializer serializer = new XmlSerializer(typeof(PIDConfig));
            PIDConfig config;
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                config = (PIDConfig)serializer.Deserialize(fs);
            }

            MainCanvas.Children.Clear();
            _components.Clear();

            foreach (var compConfig in config.Components)
            {
                UserControl control = null;
                if (compConfig.Type == "PumpControl")
                {
                    control = new PumpControl();
                }
                else if (compConfig.Type == "ValveControl")
                {
                    control = new ValveControl();
                }
                
                if (control != null)
                {
                    AddComponent(control, compConfig.ID, compConfig.X, compConfig.Y);
                }
            }

            foreach (var pipeConfig in config.Connections)
            {
                AddPipe(pipeConfig.ID, pipeConfig.SourceID, pipeConfig.SourceAnchor, pipeConfig.TargetID, pipeConfig.TargetAnchor);
            }
        }

        private void Component_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var comp = sender as IPIDComponent;
            if (comp != null)
            {
                ComponentClicked?.Invoke(comp.ComponentID);
            }

            _isDragging = true;
            _dragElement = sender as UIElement;
            _clickPosition = e.GetPosition(_dragElement);
            _dragElement.CaptureMouse();
            e.Handled = true;
        }

        private void Component_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _dragElement != null)
            {
                Point currentPos = e.GetPosition(MainCanvas);
                double left = currentPos.X - _clickPosition.X;
                double top = currentPos.Y - _clickPosition.Y;

                Canvas.SetLeft(_dragElement, left);
                Canvas.SetTop(_dragElement, top);

                // Update connected pipes
                UpdateConnectedPipes((_dragElement as IPIDComponent)?.ComponentID);
            }
        }

        private void UpdateConnectedPipes(string componentId)
        {
            if (string.IsNullOrEmpty(componentId)) return;

            foreach (var conn in _connections)
            {
                if (conn.SourceComponentID == componentId || conn.TargetComponentID == componentId)
                {
                    UpdatePipeGeometry(conn);
                }
            }
        }

        private void UpdatePipeGeometry(PipeConnection conn)
        {
            if (!_components.ContainsKey(conn.PipeID)) return;
            var pipe = _components[conn.PipeID] as PipeControl;
            if (pipe == null) return;

            if (_components.ContainsKey(conn.SourceComponentID) && _components.ContainsKey(conn.TargetComponentID))
            {
                var source = _components[conn.SourceComponentID] as UserControl;
                var target = _components[conn.TargetComponentID] as UserControl;
                
                var sourceComp = _components[conn.SourceComponentID];
                var targetComp = _components[conn.TargetComponentID];

                var sAnchor = sourceComp.Anchors.Find(a => a.Name == conn.SourceAnchorName);
                var tAnchor = targetComp.Anchors.Find(a => a.Name == conn.TargetAnchorName);

                if (sAnchor != null && tAnchor != null)
                {
                    Point start = new Point(Canvas.GetLeft(source) + sAnchor.OffsetX, Canvas.GetTop(source) + sAnchor.OffsetY);
                    Point end = new Point(Canvas.GetLeft(target) + tAnchor.OffsetX, Canvas.GetTop(target) + tAnchor.OffsetY);
                    pipe.SetPoints(start, end);
                }
            }
        }

        public void AddPipe(string id, string sourceId, string sAnchor, string targetId, string tAnchor)
        {
            var pipe = new PipeControl();
            var conn = new PipeConnection
            {
                PipeID = id,
                SourceComponentID = sourceId,
                SourceAnchorName = sAnchor,
                TargetComponentID = targetId,
                TargetAnchorName = tAnchor
            };

            _connections.Add(conn);
            AddComponent(pipe, id, 0, 0);
            UpdatePipeGeometry(conn);
        }

        private void Component_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            _dragElement?.ReleaseMouseCapture();
            _dragElement = null;
            e.Handled = true;
        }
    }
}
