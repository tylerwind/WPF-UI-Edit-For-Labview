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
    }
}
