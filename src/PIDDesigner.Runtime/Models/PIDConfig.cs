using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace PIDDesigner.Runtime.Models
{
    [XmlRoot("PIDConfiguration")]
    public class PIDConfig
    {
        [XmlArray("Components")]
        [XmlArrayItem("Component")]
        public List<ComponentConfig> Components { get; set; } = new List<ComponentConfig>();

        [XmlArray("Connections")]
        [XmlArrayItem("Connection")]
        public List<PipeConfig> Connections { get; set; } = new List<PipeConfig>();
    }

    public class ComponentConfig
    {
        public string ID { get; set; }
        public string Type { get; set; } // e.g., "Pump"
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class PipeConfig
    {
        public string ID { get; set; }
        public string SourceID { get; set; }
        public string SourceAnchor { get; set; }
        public string TargetID { get; set; }
        public string TargetAnchor { get; set; }
    }
}
