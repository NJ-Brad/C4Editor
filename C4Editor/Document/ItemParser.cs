﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C4Editor.C4Types;

namespace C4Editor
{
    public class ItemParser
    {
        StringBuilder sb = new StringBuilder();
        bool inQuote;

        public static Item Parse(int level, string text)
        {
            ItemParser p = new ItemParser();
            return p.ParseOperation(level, text);
        }

        public Item ParseOperation(int level, string text)
        {
            Item rtnVal = new Item();

            rtnVal.Level = level;
            rtnVal.Text = text;

            foreach (char character in text)
            {
                switch (character)
                {
                    case '"':
                        inQuote = !inQuote;
                        sb.Append(character);
                        break;
                    case '(':   // could be start of parameters
                        if (!inQuote)
                        {
                            if (string.IsNullOrEmpty(rtnVal.Command))
                            {
                                string initialCommand = sb.ToString().Trim();

                                // "fix" the command, so that it is easier to deal with
                                switch (initialCommand)
                                {
                                    case "Person":
                                        rtnVal.IsExternal = false;
                                        rtnVal.IsDatabase = false;
                                        rtnVal.Command = "Person";
                                        break;
                                    case "Person_Ext":
                                        rtnVal.IsExternal = true;
                                        rtnVal.IsDatabase = false;
                                        rtnVal.Command = "Person";
                                        break;
                                    case "System":
                                        rtnVal.IsExternal = false;
                                        rtnVal.IsDatabase = false;
                                        rtnVal.Command = "System";
                                        break;
                                    case "System_Ext":
                                        rtnVal.IsExternal = true;
                                        rtnVal.IsDatabase = false;
                                        rtnVal.Command = "System";
                                        break;
                                    case "SystemDb":
                                        rtnVal.IsExternal = false;
                                        rtnVal.IsDatabase = true;
                                        rtnVal.Command = "System";
                                        break;
                                    case "SystemDb_Ext":
                                        rtnVal.IsExternal = true;
                                        rtnVal.IsDatabase = true;
                                        rtnVal.Command = "System";
                                        break;
                                    case "Container":
                                        rtnVal.IsExternal = false;
                                        rtnVal.IsDatabase = false;
                                        rtnVal.Command = "Container";
                                        break;
                                    case "Container_Ext":
                                        rtnVal.IsExternal = true;
                                        rtnVal.IsDatabase = false;
                                        rtnVal.Command = "Container";
                                        break;
                                    case "ContainerDb":
                                        rtnVal.IsExternal = false;
                                        rtnVal.IsDatabase = true;
                                        rtnVal.Command = "Container";
                                        break;
                                    case "ContainerDb_Ext":
                                        rtnVal.IsExternal = true;
                                        rtnVal.IsDatabase = true;
                                        rtnVal.Command = "Container";
                                        break;
                                    case "Component":
                                        rtnVal.IsExternal = false;
                                        rtnVal.IsDatabase = false;
                                        rtnVal.Command = "Component";
                                        break;
                                    case "ComponentDb":
                                        rtnVal.IsExternal = false;
                                        rtnVal.IsDatabase = true;
                                        rtnVal.Command = "Component";
                                        break;
                                    case "Boundary":
                                        rtnVal.Command = "Boundary";
                                        break;
                                    case "System_Boundary":
                                        rtnVal.Command = "System_Boundary";
                                        break;
                                    case "Enterprise_Boundary":
                                        rtnVal.Command = "Enterprise_Boundary";
                                        break;
                                    case "Container_Boundary":
                                        rtnVal.Command = "Container_Boundary";
                                        break;
                                    default:
                                        rtnVal.Command = initialCommand;
                                        break;

                                }

                                sb.Length = 0;
                            }
                        }
                        else
                        {
                            sb.Append(character);
                        }
                        break;
                    case ')':   // could be end of parameters
                        if (inQuote)
                        {
                            sb.Append(character);
                        }
                        break;
                    case ',':   // parameter separator
                        if (inQuote)
                        {
                            sb.Append(character);
                        }
                        else
                        {
                            Parameter(ref rtnVal);
                        }
                        break;
                    default:
                        sb.Append(character);
                        break;
                }
            }
            if (sb.Length > 0)
            {
                Parameter(ref rtnVal);
            }

            rtnVal = ConvertToType(rtnVal);

            return rtnVal;
        }

        public static Item ConvertToType(Item original)
        {
            Item rtnVal = original;

            switch (original.Command)
            {
                case "Person":
                case "Person_Ext":
                    rtnVal = new C4Person(original);
                    break;
                case "System":
                case "System_Ext":
                case "SystemDb":
                case "SystemDb_Ext":
                    rtnVal = new C4System(original);
                    break;
                case "Container":
                case "ContainerDb":
                case "Container_Ext":
                case "ContainerDb_Ext":
                    rtnVal = new C4Container(original);
                    break;
                case "Node":
                    rtnVal = new C4Node(original);
                    break;
                case "Boundary":
                    rtnVal = new C4Boundary(original);
                    break;
                case "Enterprise_Boundary":
                    rtnVal = new C4EnterpriseBoundary(original);
                    break;
                case "System_Boundary":
                    rtnVal = new C4SystemBoundary(original);
                    break;
                case "Container_Boundary":
                    rtnVal = new C4ContainerBoundary(original);
                    break;
                case "Rel":
                    rtnVal = new C4Relation(original);
                    break;
                case "Component":
                case "ComponentDb":
                    rtnVal = new C4Component(original);
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine(original.Command);
                    break;
            }
            return rtnVal;
        }

        private void Parameter(ref Item item)
        {
            if (sb.Length > 0)
            {
                item.Parameters.Add(sb.ToString().Trim());
                sb.Length = 0;
            }
        }

    }
}
