using PropertyEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C4Editor.C4Types
{
    public class C4Relation : Item
    {
        public C4Relation(Item item)
        {
            Command = item.Command;
            Text = item.Text;
            Level = item.Level;
            foreach (string str in item.Parameters)
            {
                Parameters.Add(str);
            }
        }

        //            BuildAliasMap(treeView1.Nodes[1]);
        //            propertyTable1.AddProperty("Label", "Label", "Text");
        //            propertyTable1.AddProperty("Technology", "Technology*", "MultiLineText");

        [PropertyAttribute("From", "RuntimeChoice", "Items")]
        public string From
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

        [PropertyAttribute("To", "RuntimeChoice", "Items")]
        public string To
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

        [PropertyAttribute("Label", "Text")]
        public string Label
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

        [PropertyAttribute("Technology", "MultiLineText")]
        public string Technology
        {
            get
            {
                return GetValue(3);
            }
            set
            {
                SetValue(3, value);
            }
        }


        public string DisplayName
        {
            get { return "Container"; }
        }
    }
}
