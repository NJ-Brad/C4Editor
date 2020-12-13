namespace C4Editor
{
    public static class DisplayNames
    {
        public static string GetDisplayName(string plantName)
        {
            string rtnVal = plantName;  // return what came in, if not found
            switch (plantName)
            {
                case "Person":
                    rtnVal = "Person"; break;
                case "Person_Ext":
                    rtnVal = "Person (External)"; break;
                case "System":
                    rtnVal = "System"; break;
                case "System_Ext":
                    rtnVal = "System (External)"; break;
                case "SystemDb":
                    rtnVal = "System (Database)"; break;
                case "SystemDb_Ext":
                    rtnVal = "System (Database) (External)"; break;
                case "Enterprise_Boundary":
                    rtnVal = "Enterprise Boundary"; break;
                case "Rel":
                    rtnVal = "Relationship"; break;
                case "Container":
                    rtnVal = "Container"; break;
                case "Container_Ext":
                    rtnVal = "Container (External)"; break;
                case "ContainerDb":
                    rtnVal = "Container (Database)"; break;
                case "ContainerDb_Ext":
                    rtnVal = "Container (Database) (External)"; break;
                case "Container_Boundary":
                    rtnVal = "Container Boundary"; break;
                case "System_Boundary":
                    rtnVal = "System Boundary"; break;
                case "Component":
                    rtnVal = "Component"; break;
                case "ComponentDb":
                    rtnVal = "Component (Database)"; break;
                case "Node":
                    rtnVal = "Node"; break;
            }
            return rtnVal;
        }
    }
}
