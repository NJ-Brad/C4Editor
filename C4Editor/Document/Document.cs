using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C4Editor
{
    public class Document
    {
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }
        public string Title { get; set; }
        public bool Sketch { get; set; } = false;
        public bool ShowLegend { get; set; } = false;
        public bool TopDown { get; set; } = false;
        public List<Item> Commands { get; set; } = new List<Item>();
        public List<Item> Relations { get; set; } = new List<Item>();
    }
}
