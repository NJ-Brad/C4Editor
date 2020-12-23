using PropertyEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C4Editor.C4Types
{
    class C4System : Item
    {
        public C4System(Item item)
        {
            Command = item.Command;
            Text = item.Text;
            Level = item.Level;
            foreach (string str in item.Parameters)
            {
                Parameters.Add(str);
            }
            IsExternal = item.IsExternal;
            IsDatabase = item.IsDatabase;
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

        [PropertyAttribute("Description", "Text")]
        public string Description
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

        [PropertyAttribute("External", "Flag")]
        public bool External
        {
            get
            {
                return IsExternal;
            }
            set
            {
                IsExternal = value;
            }
        }

        [PropertyAttribute("Database", "Flag")]
        public bool Database
        {
            get
            {
                return IsDatabase;
            }
            set
            {
                IsDatabase = value;
            }
        }

        public string DisplayName
        {
            get { return "Node"; }
        }

    }
}
