using PropertyEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C4Editor.C4Types
{
    class C4Boundary : Item
    {
        public C4Boundary(Item item)
    {
        Command = item.Command;
        Text = item.Text;
        Level = item.Level;
        foreach (string str in item.Parameters)
        {
            Parameters.Add(str);
        }
    }

    [PropertyAttribute("Alias", "Text")]
    public string Alias
    {
        get
        {
            return GetValue(0);
        }
        set
        {
            SetValue(0, value, false);
        }
    }

    [PropertyAttribute("Label", "Text")]
    public string Label
    {
        get
        {
            return GetValue(1);
        }
        set
        {
            SetValue(1, value);
        }
    }

        [PropertyAttribute("Boundary Type", "Text")]
        public string BoundaryType
        {
            get
            {
                return GetValue(2);
            }
            set
            {
                SetValue(2, value);
            }
        }


        public string DisplayName
    {
        get { return "Enterprise Boundary"; }
    }
}
}
