using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace C4Editor
{
    public class Item
    {
        //public Item() { }

        //public Item(int level, string text)
        //{
        //    this.Level = level;
        //    this.Text = text;
        //    Parse();
        //}

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (Command.StartsWith("Rel"))
            {
                sb.Append("Rel(");
                sb.Append(Parameters[0]);

                if (Parameters.Count > 1)
                {
                    sb.Append($", {Parameters[1]}");
                }

                if (Parameters.Count > 2)
                {
                    sb.Append($", {Parameters[2]}");
                }
                if (Parameters.Count > 3)
                {
                    sb.Append($", {Parameters[3]}");
                }

                sb.Append(")");
            }
            else
            {
                sb.Append(Command);
                if (IsDatabase)
                {
                    sb.Append("Db");
                }
                if (IsExternal)
                {
                    sb.Append("_Ext");
                }

                sb.Append("(");

                sb.Append(Parameters[0]);

                if (Parameters.Count > 1)
                {
                    sb.Append($", {Parameters[1]}");
                }

                if (Parameters.Count > 2)
                {
                    sb.Append($", {Parameters[2]}");
                }
                if (Parameters.Count > 3)
                {
                    sb.Append($", {Parameters[3]}");
                }

                sb.Append(")");
            }

            return sb.ToString();
        }

        public int Level { get; set; }
        public string Text { get; set; } = "";
        public string Command { get; set; } = "";
        public bool IsDatabase { get; set; }
        public bool IsExternal { get; set; }

        public List<string> Parameters { get; set; } = new List<string>();

        //StringBuilder sb = new StringBuilder();
        //bool inQuote;

        //private void Parse()
        //{
        //    foreach (char character in Text)
        //    {
        //        switch (character)
        //        {
        //            case '"':
        //                inQuote = !inQuote;
        //                sb.Append(character);
        //                break;
        //            case '(':   // could be start of parameters
        //                if (!inQuote)
        //                {
        //                    if (string.IsNullOrEmpty(Command))
        //                    {
        //                        string initialCommand = sb.ToString().Trim();

        //                        // "fix" the command, so that it is easier to deal with
        //                        switch (initialCommand)
        //                        {
        //                            case "Person":
        //                                IsExternal = false;
        //                                IsDatabase = false;
        //                                Command = "Person";
        //                                break;
        //                            case "Person_Ext":
        //                                IsExternal = true;
        //                                IsDatabase = false;
        //                                Command = "Person";
        //                                break;
        //                            case "System":
        //                                IsExternal = false;
        //                                IsDatabase = false;
        //                                Command = "System";
        //                                break;
        //                            case "System_Ext":
        //                                IsExternal = true;
        //                                IsDatabase = false;
        //                                Command = "System";
        //                                break;
        //                            case "SystemDb":
        //                                IsExternal = false;
        //                                IsDatabase = true;
        //                                Command = "System";
        //                                break;
        //                            case "SystemDb_Ext":
        //                                IsExternal = true;
        //                                IsDatabase = true;
        //                                Command = "System";
        //                                break;
        //                            case "Container":
        //                                IsExternal = false;
        //                                IsDatabase = false;
        //                                Command = "Container";
        //                                break;
        //                            case "Container_Ext":
        //                                IsExternal = true;
        //                                IsDatabase = false;
        //                                Command = "Container";
        //                                break;
        //                            case "ContainerDb":
        //                                IsExternal = false;
        //                                IsDatabase = true;
        //                                Command = "Container";
        //                                break;
        //                            case "ContainerDb_Ext":
        //                                IsExternal = true;
        //                                IsDatabase = true;
        //                                Command = "Container";
        //                                break;
        //                            case "Component":
        //                                IsExternal = false;
        //                                IsDatabase = false;
        //                                Command = "Component";
        //                                break;
        //                            case "ComponentDb":
        //                                IsExternal = false;
        //                                IsDatabase = true;
        //                                Command = "Component";
        //                                break;
        //                            default:
        //                                Command = initialCommand;
        //                                break;

        //                        }

        //                        sb.Length = 0;
        //                    }
        //                }
        //                else
        //                {
        //                    sb.Append(character);
        //                }
        //                break;
        //            case ')':   // could be end of parameters
        //                if (inQuote)
        //                {
        //                    sb.Append(character);
        //                }
        //                break;
        //            case ',':   // parameter separator
        //                if (inQuote)
        //                {
        //                    sb.Append(character);
        //                }
        //                else
        //                {
        //                    Parameter();
        //                }
        //                break;
        //            default:
        //                sb.Append(character);
        //                break;
        //        }
        //    }
        //    if (sb.Length > 0)
        //    {
        //        Parameter();
        //    }
        //}

        //private void Parameter()
        //{
        //    if (sb.Length > 0)
        //    {
        //        Parameters.Add(sb.ToString().Trim());
        //        sb.Length = 0;
        //    }
        //}

        public string GetValue(int parameterNumber)
        {
            string rtnVal = string.Empty;

            if (Parameters.Count > parameterNumber)
            {
                rtnVal = Parameters[parameterNumber].Trim('\"').Replace("</size>\\n<size:$TECHN_FONT_SIZE>", "\r\n");
            }

            return rtnVal;
        }
        public void SetValue(int parameterNumber, string value, bool quote = true)
        {
            string newValue = value;

            if (quote)
            {
                newValue = value.Replace("\r\n", "</size>\\n<size:$TECHN_FONT_SIZE>");
                newValue = "\"" + newValue + "\"";
            }

            if (Parameters.Count > parameterNumber)
            {
                Parameters[parameterNumber] = newValue;
            }
            else
            {
                Parameters.Add(newValue);
            }
        }

    }

    // Why a helper class and not just methods?  Let's find out
    //public static class ItemHelpers
    //{
    //    public static string GetValue(this Item item, int parameterNumber)
    //    {
    //        string rtnVal = string.Empty;

    //        if (item.Parameters.Count > parameterNumber)
    //        {
    //            rtnVal = item.Parameters[parameterNumber].Trim('\"').Replace("</size>\\n<size:$TECHN_FONT_SIZE>", "\r\n");
    //        }

    //        return rtnVal;
    //    }
    //    public static void SetValue(this Item item, int parameterNumber, string value, bool quote = true)
    //    {
    //        string newValue = value;

    //        if (quote)
    //        {
    //            newValue = value.Replace("\r\n", "</size>\\n<size:$TECHN_FONT_SIZE>");
    //            newValue = "\"" + newValue + "\"";
    //        }

    //        if (item.Parameters.Count > parameterNumber)
    //        {
    //            item.Parameters[parameterNumber] = newValue;
    //        }
    //        else
    //        {
    //            item.Parameters.Add(newValue);
    //        }
    //    }
    //}
}
