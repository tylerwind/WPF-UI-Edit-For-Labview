namespace PIDDesigner.Runtime.Models
{
    public class PipeConnection
    {
        public string PipeID { get; set; }
        public string SourceComponentID { get; set; }
        public string SourceAnchorName { get; set; }
        public string TargetComponentID { get; set; }
        public string TargetAnchorName { get; set; }
    }
}
