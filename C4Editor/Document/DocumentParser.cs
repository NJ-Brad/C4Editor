using System.Collections.Generic;
using System.Text;

namespace C4Editor
{
    public class DocumentParser
    {
        public static Document Parse(string documentText)
        {
            Document rtnVal = new Document();

            bool inQuote = false;
            int level = 0;

            List<Item> items = new List<Item>();

            StringBuilder sb = new StringBuilder();

            foreach (char character in documentText)
            {
                switch (character)
                {
                    case '"':
                        inQuote = !inQuote;
                        sb.Append(character);
                        break;
                    case '{':
                        if (!inQuote)
                        {
                            // treat as newline
                            NewLine(ref items, level, ref sb);

                            level++;
                        }
                        else
                        {
                            sb.Append(character);
                        }
                        break;
                    case '}':
                        if (!inQuote)
                        {
                            level--;
                        }
                        else
                        {
                            sb.Append(character);
                        }
                        break;
                    case '\r':
                    case '\n':
                        NewLine(ref items, level, ref sb);
                        break;
                    default:
                        sb.Append(character);
                        break;
                }
            }

            if (sb.Length > 0)
            {
                NewLine(ref items, level, ref sb);
            }

            //System.Diagnostics.Debug.WriteLine(items.Count);

            foreach (Item item in items)
            {
                if (item.Text.StartsWith("@startuml"))
                {
                    if (item.Text.Length > 9)
                    {
                        rtnVal.DocumentName = item.Text.Substring(9).Trim();
                    }
                }
                else if (item.Text.StartsWith("!include"))
                {
                    if (item.Text.ToUpper().TrimEnd().EndsWith("C4_CONTEXT.PUML"))
                    {
                        rtnVal.DocumentType = "Context";
                    }
                    else if (item.Text.ToUpper().TrimEnd().EndsWith("C4_DEPLOYMENT.PUML"))
                    {
                        rtnVal.DocumentType = "Deployment";
                    }
                    else if (item.Text.ToUpper().TrimEnd().EndsWith("C4_COMPONENT.PUML"))
                    {
                        rtnVal.DocumentType = "Component";
                    }
                    else if (item.Text.ToUpper().TrimEnd().EndsWith("C4_CONTAINER.PUML"))
                    {
                        rtnVal.DocumentType = "Container";
                    }
                }
                else if (item.Text.StartsWith("title"))
                {
                    if (item.Text.Length > 5)
                    {
                        rtnVal.Title = item.Text.Substring(5).Trim();
                    }
                }
                else if (item.Text.StartsWith("LAYOUT_AS_SKETCH()"))
                {
                    rtnVal.Sketch = true;
                }
                else if (item.Text.StartsWith("LAYOUT_WITH_LEGEND()"))
                {
                    rtnVal.ShowLegend = true;
                }
                else if (item.Text.StartsWith("LAYOUT_TOP_DOWN()"))
                {
                    rtnVal.TopDown = true;
                }
                else if (item.Text.StartsWith("LAYOUT_LEFT_RIGHT()"))
                {
                    rtnVal.TopDown = false;
                }

                if ((!string.IsNullOrEmpty(item.Command)) && (item.Parameters.Count > 0))
                {
                    if ((item.Command.ToUpper().StartsWith("REL")) ||
                        (item.Command.ToUpper().StartsWith("INTERACT")))
                    {
                        rtnVal.Relations.Add(item);
                    }
                    else
                    {
                        rtnVal.Commands.Add(item);
                    }
                }
            }
            return rtnVal;
        }

        private static void NewLine(ref List<Item> items,
            int level,
            ref StringBuilder sb)
        {
            string itemText = sb.ToString().Trim();

            // only add the item, if there is text in it
            if (itemText.Length > 0)
            {
                //                items.Add(new Item(level, itemText));
                items.Add(ItemParser.Parse(level, itemText));

            }
            sb.Length = 0;
        }
    }
}
