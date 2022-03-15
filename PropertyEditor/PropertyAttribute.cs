using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyEditor
{
    public class PropertyAttribute : Attribute
    {
        public string Prompt { get; set; }
        public string PropertyType { get; set; }
        public string[] Choices { get; set; }

        public PropertyAttribute(string prompt, string propertyType, params string[] choices)
        {
            this.Prompt = prompt;
            this.PropertyType = propertyType;
            this.Choices = choices;
        }
    }
}
