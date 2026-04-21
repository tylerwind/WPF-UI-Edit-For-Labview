namespace PIDDesigner.Runtime.Controls
{
    public interface IPIDComponent
    {
        /// <summary>
        /// Unique identifier mapping to the LabVIEW Cluster array.
        /// </summary>
        string ComponentID { get; set; }

        /// <summary>
        /// Update the visual state and value of the component.
        /// </summary>
        void UpdateData(double value, int state);

        /// <summary>
        /// Relative positions for pipe connections.
        /// </summary>
        System.Collections.Generic.List<AnchorPoint> Anchors { get; }
    }

    public class AnchorPoint
    {
        public string Name { get; set; }
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
    }
}
