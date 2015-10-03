using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DirectoryJunctionProject.Model
{
    /// <summary>
    /// Contains state of application
    /// </summary>
    [XmlRoot]
    public class DirJunctionModel
    {
        [XmlElement]
        public string LinkName { get; set; }
        [XmlIgnore]
        public bool OutputReady { get; set; } = false;
        [XmlElement]
        public string LinkDirectoryPath { get; set; }
        [XmlElement]
        public string TargetPath { get; set; }
        [XmlIgnore]
        public string CmdLineFeedback { get; set; } = "";
    }
}
