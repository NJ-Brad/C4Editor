using PropertyEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C4Editor.C4Types
{
    public class C4Person : Item
    {
        public C4Person(Item item)
        {
            Command = item.Command;
            Text = item.Text;
            Level = item.Level;
            foreach (string str in item.Parameters)
            {
                Parameters.Add(str);
            }

            IsExternal = item.IsExternal;
        }



        //            propertyTable1.SetValue("IsExternal", item.IsExternal ? "Y" : "N");
        //            propertyTable1.SetValue("Alias", item.GetValue(0));
        //            propertyTable1.SetValue("Label", item.GetValue(1));
        //            propertyTable1.SetValue("Description", item.GetValue(2));

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

        public string DisplayName
        {
            get { return "Person"; }
        }

    }
}
