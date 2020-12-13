using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C4Editor
{
    public class DocumentBuilder
    {
        public static string Build(Document doc)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"@startuml {doc.DocumentName}");

            sb.AppendLine($"!include https://raw.githubusercontent.com/NJ-Brad/C4-PlantUML/master/C4_{doc.DocumentType}.puml");
            if (!string.IsNullOrEmpty(doc.Title))
            {
                sb.AppendLine($"title {doc.Title}");
            }
            if (doc.Sketch) {
                sb.AppendLine("LAYOUT_AS_SKETCH()");
            }
            if (doc.ShowLegend)
            {
                sb.AppendLine("LAYOUT_WITH_LEGEND()");
            }
            if (doc.TopDown)
            {
                sb.AppendLine("LAYOUT_TOP_DOWN()");
            }
            else
            {
                sb.AppendLine("LAYOUT_LEFT_RIGHT()");
            }

            // testing
            //sb.AppendLine(@"Node(Customer's_computer, ""Customer's computer"", ""Microsoft Windows or </size>\n<size:$TECHN_FONT_SIZE>Apple macOS""){");
            //sb.AppendLine("}");

            // build a line, but do NOT end it
            // if the next command is "deeper", end the previous line with "{\r\n", otherwise end the previous line with "\r\n"

            int previousLevel = -1;
            foreach (Item command in doc.Commands)
            {
                // handle previous line (if there is one)
                if (previousLevel != -1)
                {
                    if (previousLevel < command.Level)
                    {
                        // start of a group
                        sb.AppendLine("{");
                    }
                    else
                    {
                        // same level
                        sb.AppendLine();
                    }
                }

                // end any open groups
                if (command.Level < previousLevel)
                {
                    for (int i = previousLevel; i > command.Level; i--)
                    {
                        sb.AppendLine("}");
                    }
                }

                previousLevel = command.Level;

                sb.Append(command.ToString());
                //sb.Append(command.Command);
            }

            sb.AppendLine();
            // end any open groups
            for (int i = previousLevel; i > 0; i--)
            {
                sb.AppendLine("}");
            }

            foreach (Item item in doc.Relations)
            {
                sb.AppendLine(item.ToString());
            }



            sb.AppendLine("@enduml");

            return sb.ToString();
        }
    }
}
