using PropertyEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C4Editor
{
    public class Document
    {
        [PropertyAttribute("Document Name", "Text")]
        public string DocumentName { get; set; }
        [PropertyAttribute("Document Type*", "Choice", "Context", "Container", "Component", "Dynamic (not yet)", "Deployment")]
        public string DocumentType { get; set; }
        [PropertyAttribute("Title", "Text")]
        public string Title { get; set; }

        [PropertyAttribute("Show as sketch", "Flag")]
        public bool Sketch { get; set; } = false;

        [PropertyAttribute("Show legend", "Flag")]
        public bool ShowLegend { get; set; } = false;

        [PropertyAttribute("Lay out Top-Down", "Flag")]
        public bool TopDown { get; set; } = false;

        public List<Item> Commands { get; set; } = new List<Item>();
        public List<Item> Relations { get; set; } = new List<Item>();
    }
}
