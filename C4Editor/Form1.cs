using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C4Editor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            newToolStripButton.DropDownItems.Clear();
            newToolStripButton.DropDownItems.Add("Context", null, (s, e) => { NewDocument("Context"); } );
            newToolStripButton.DropDownItems.Add("Container", null, (s, e) => { NewDocument("Container"); });
            newToolStripButton.DropDownItems.Add("Deployment", null, (s, e) => { NewDocument("Deployment"); });
            newToolStripButton.DropDownItems.Add("Component", null, (s, e) => { NewDocument("Component"); });

            //newToolStripButton.Click += (s, e) => { NewDocument(); };
            openToolStripButton.Click += (s, e) => { OpenDocument(); };
            saveToolStripButton.Click += (s, e) => { SaveDocument(); };

            toolStripButton1.Click += (s, e) => { RefreshDisplay(); };
            toolStripButton2.Click += (s, e) => { StartOver(); };

            cutToolStripButton.Click += (s, e) => { Cut(); };
            copyToolStripButton.Click += (s, e) => { Copy(); };
            pasteToolStripButton.Click += (s, e) => { Paste(); };

            moveUpButton.Click += (s, e) => { MoveUp(); };
            moveDownButton.Click += (s, e) => { MoveDown(); };

            autoSaveTimer = new Timer();
            autoSaveTimer.Tick += AutoSaveTimer_Tick;
            autoSaveTimer.Interval = 3000;
            autoSaveTimer.Enabled = false;
        }
        
        private void NewDocument(string documentType)
        {
            if (dirty)
            {
                if (MessageBox.Show("Save before closing?", "Unsaved changes", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    return;
                }
            }

            doc = new Document();
            doc.DocumentType = documentType;

            DocToTree(treeView1);

            propertyTable1.SetText("No object selected");
            propertyTable1.ClearProperties();

            zoomPanImageBox1.Image = null;
            textBox1.Text = string.Empty;

            Text = "C4 Editor - [No File]";
        }

        private void OpenDocument()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PUML files (*.puml)|*.puml|All Files (*.*)|*.*";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string docText = File.ReadAllText(ofd.FileName);
                doc = DocumentParser.Parse(docText);

                DocToTree(treeView1);

                propertyTable1.SetText("No object selected");
                propertyTable1.ClearProperties();

                RenderDocument(docText);

                textBox1.Text = docText;

                Text = $"C4 Editor - {Path.GetFileName(ofd.FileName)}";
                fileName = ofd.FileName;
            }

        }

        string fileName = "";

        private void SaveDocument()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PUML files (*.puml)|*.puml|All Files (*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                BuildDocument();

                string newDocText = DocumentBuilder.Build(doc);

                File.WriteAllText(sfd.FileName, newDocText);
                Text = $"C4 Editor - {Path.GetFileName(sfd.FileName)}";
                fileName = sfd.FileName;

                dirty = false;
            }
        }

        private void Cut()
        {
            Copy();
            TreeNode selectedNode = treeView1.SelectedNode;
            selectedNode.Parent.Nodes.Remove(selectedNode);
            DocumentChanged();
        }

        private void Copy()
        {
            TreeNode selectedNode = treeView1.SelectedNode;
            clipboardNode = new TreeNode(selectedNode.Text);
            clipboardNode.Tag = selectedNode.Tag;

            IterateTreeNodes(selectedNode, clipboardNode);
        }

        private void Paste()
        {
            TreeNode selectedNode = treeView1.SelectedNode;

            TreeNode newNode = new TreeNode(clipboardNode.Text);
            newNode.Tag = CopyFromExisting(clipboardNode.Tag as Item);
            treeView1.SelectedNode.Nodes.Add(newNode);

            treeView1.SelectedNode = newNode;

            IterateTreeNodes(clipboardNode, newNode, true);
            DocumentChanged();
        }

        private Item CopyFromExisting(Item existing)
        {
            Item rtnVal = new Item();
            if (existing != null)
            {
                rtnVal.Command = existing.Command;
                rtnVal.IsDatabase = existing.IsDatabase;
                rtnVal.IsExternal = existing.IsExternal;

                rtnVal.Parameters.Add(GetNewName(existing.Command));
                // need to fix this value later, to reduce confusion
                rtnVal.Parameters.Add(existing.Parameters[1]);

                if (existing.Parameters.Count > 2)
                {
                    rtnVal.Parameters.Add(existing.Parameters[2]);
                }

                if (existing.Parameters.Count > 3)
                {
                    rtnVal.Parameters.Add(existing.Parameters[3]);
                }
            }

            return rtnVal;
        }

        private void MoveUp()
        {
            int nodeIndex = GetIndex(treeView1.SelectedNode);

            // it has to be found
            if (nodeIndex != -1)
            {
                Cut();

                TreeNode newNode = new TreeNode(clipboardNode.Text);
                newNode.Tag = clipboardNode.Tag;
                treeView1.SelectedNode.Parent.Nodes.Insert((nodeIndex -1), newNode);

                IterateTreeNodes(clipboardNode, newNode);

                treeView1.SelectedNode = newNode;
            }
            DocumentChanged();
        }

        private void MoveDown()
        {
            int nodeIndex = GetIndex(treeView1.SelectedNode);

            // it has to be found
            if (nodeIndex != -1)
            {
                Cut();

                TreeNode newNode = new TreeNode(clipboardNode.Text);
                newNode.Tag = clipboardNode.Tag;
                treeView1.SelectedNode.Parent.Nodes.Insert((nodeIndex + 1), newNode);

                IterateTreeNodes(clipboardNode, newNode);

                treeView1.SelectedNode = newNode;
            }
            DocumentChanged();
        }

        private int GetIndex(TreeNode node)
        {
            int nodeIndex = -1;

            TreeNode parentNode = node.Parent;

            if (parentNode != null)
            {

                int counter = 0;
                foreach (TreeNode collectionNode in parentNode.Nodes)
                {
                    if (collectionNode == node)
                    {
                        nodeIndex = counter;
                        break;
                    }
                    counter++;
                }
            }
            return nodeIndex;
        }

        private void IterateTreeNodes(TreeNode originalNode, TreeNode rootNode, bool createNewItem = false)
        {
            foreach (TreeNode childNode in originalNode.Nodes)
            {
                TreeNode newNode = new TreeNode(childNode.Text);
                if (createNewItem)
                {
                    newNode.Tag = CopyFromExisting(childNode.Tag as Item);
                }
                else
                {
                    newNode.Tag = childNode.Tag;
                }

                rootNode.Nodes.Add(newNode);
                IterateTreeNodes(childNode, newNode, createNewItem);
            }
        }

        private void RefreshDisplay()
        {
            SaveItemData();

            BuildDocument();

            string newDocText2 = DocumentBuilder.Build(doc);

            RenderDocument(newDocText2);

            textBox1.Text = newDocText2;
        }

        private void StartOver()
        {
            doc = DocumentParser.Parse(textBox1.Text);

            DocToTree(treeView1);
        }

        private bool IsNodeInCorrectSection(TreeNode node, TreeNode sectionNode, bool parentAllowed = true)
        {
            bool rtnVal = false;

            if (node != null)
            {
                if (node == sectionNode)
                {
                    rtnVal = true;
                }
                else if (parentAllowed &&  (node.Parent != null))
                {
                    rtnVal = IsNodeInCorrectSection(node.Parent, sectionNode);
                }
            }

            return rtnVal;
        }

        private void ConfigureContextMenu(TreeNode node)
        {
            //if (button != null)
            //{
            //    button.DropDownItems.Clear();

            //    // model node
            //    if (IsNodeInCorrectSection(treeView1.SelectedNode, treeView1.Nodes[1]))
            //    {
            //        switch (doc.DocumentType)
            //        {
            //            case "Context":
            //                button.DropDownItems.Add("Person", null, (s, e) => { AddItem("Person"); });
            //                button.DropDownItems.Add("System", null, (s, e) => { AddItem("System"); });
            //                button.DropDownItems.Add("Enterprise Boundary", null, (s, e) => { AddItem("Enterprise_Boundary"); });
            //                break;
            //            case "Container":
            //                button.DropDownItems.Add("Container", null, (s, e) => { AddItem("Container"); });
            //                button.DropDownItems.Add("Container Boundary", null, (s, e) => { AddItem("Container_Boundary"); });
            //                button.DropDownItems.Add("System Boundary", null, (s, e) => { AddItem("System_Boundary"); });
            //                break;
            //            case "Component":
            //                button.DropDownItems.Add("Container Boundary", null, (s, e) => { AddItem("Container_Boundary"); });
            //                button.DropDownItems.Add("Component", null, (s, e) => { AddItem("Component"); });
            //                break;
            //            case "Dynamic":     // saving this for later
            //                break;
            //            case "Deployment":
            //                button.DropDownItems.Add("Node", null, (s, e) => { AddItem("Node"); });
            //                button.DropDownItems.Add("Container", null, (s, e) => { AddItem("Container"); });
            //                break;
            //        }
            //    }

            //    // relationships node
            //    else if (IsNodeInCorrectSection(treeView1.SelectedNode, treeView1.Nodes[2], false))
            //    {
            //        switch (doc.DocumentType)
            //        {
            //            case "Context":
            //            case "Deployment":
            //            case "Container":
            //            case "Component":
            //                button.DropDownItems.Add("Relationship", null, (s, e) => { AddItem("Relationship"); });
            //                break;
            //            case "Dynamic":     // saving this for later
            //                break;
            //        }
            //    }
            //    else
            //    {
            //        button.DropDownItems.Add("No options available for this node").Enabled = false; ;
            //    }
            //}
        }

        private void FillAddMenus(ToolStripDropDownItem item)
        {
            item.DropDownItems.Clear();

            // model node
            if (IsNodeInCorrectSection(treeView1.SelectedNode, treeView1.Nodes[1]))
            {
                switch (doc.DocumentType)
                {
                    case "Context":
                        item.DropDownItems.Add("Person", null, (s, e) => { AddItem("Person"); });
                        item.DropDownItems.Add("System", null, (s, e) => { AddItem("System"); });
                        item.DropDownItems.Add("Enterprise Boundary", null, (s, e) => { AddItem("Enterprise_Boundary"); });
                        break;
                    case "Container":
                        item.DropDownItems.Add("Container", null, (s, e) => { AddItem("Container"); });
                        item.DropDownItems.Add("Container Boundary", null, (s, e) => { AddItem("Container_Boundary"); });
                        item.DropDownItems.Add("System Boundary", null, (s, e) => { AddItem("System_Boundary"); });
                        break;
                    case "Component":
                        item.DropDownItems.Add("Container Boundary", null, (s, e) => { AddItem("Container_Boundary"); });
                        item.DropDownItems.Add("Component", null, (s, e) => { AddItem("Component"); });
                        break;
                    case "Dynamic":     // saving this for later
                        break;
                    case "Deployment":
                        item.DropDownItems.Add("Node", null, (s, e) => { AddItem("Node"); });
                        item.DropDownItems.Add("Container", null, (s, e) => { AddItem("Container"); });
                        break;
                }
            }

            // relationships node
            else if (IsNodeInCorrectSection(treeView1.SelectedNode, treeView1.Nodes[2], false))
            {
                switch (doc.DocumentType)
                {
                    case "Context":
                    case "Deployment":
                    case "Container":
                    case "Component":
                        item.DropDownItems.Add("Relationship", null, (s, e) => { AddItem("Relationship"); });
                        break;
                    case "Dynamic":     // saving this for later
                        break;
                }
            }
            else
            {
                item.DropDownItems.Add("No options available for this node").Enabled = false; ;
            }
        }


        private void AddItem(string itemType)
        {
            if (previousNode != null)
            {
                //Item item = new Item(-1, "") { Command = itemType };
                Item item = new Item() { Command = itemType };
                item.Parameters.Add(GetNewName(itemType));
                item.Parameters.Add($"New {DisplayNames.GetDisplayName(itemType)}");

                TreeNode treeNode = new TreeNode();
                treeNode.Text = item.GetValue(1);
                treeNode.Tag = item;
                previousNode.Nodes.Add(treeNode);

                treeView1.SelectedNode = treeNode;
            }
        }

        Dictionary<int, TreeNode> lastAtLevel = new Dictionary<int, TreeNode>();

        Document doc = null;

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            treeView1.ContextMenuStrip = new ContextMenuStrip { ShowImageMargin = false };

            ToolStripMenuItem tsmi = new ToolStripMenuItem { Name = "contextMenuCut", Text = "Cut" };
            tsmi.Click += (s, e) => { Cut(); };
            treeView1.ContextMenuStrip.Items.Add(tsmi);

            tsmi = new ToolStripMenuItem { Name = "contextMenuCopy", Text = "Copy" };
            tsmi.Click += (s, e) => { Copy(); };
            treeView1.ContextMenuStrip.Items.Add(tsmi);

            tsmi = new ToolStripMenuItem { Name = "contextMenuPaste", Text = "Paste" };
            tsmi.Click += (s, e) => { Paste(); };
            treeView1.ContextMenuStrip.Items.Add(tsmi);

            ToolStripSeparator tss = new ToolStripSeparator();
            treeView1.ContextMenuStrip.Items.Add(tss);

            tsmi = new ToolStripMenuItem { Name = "contextMenuMoveUp", Text = "Move Up" };
            tsmi.Click += (s, e) => { MoveUp(); };
            treeView1.ContextMenuStrip.Items.Add(tsmi);

            tsmi = new ToolStripMenuItem { Name = "contextMenuMoveDown", Text = "Move Down" };
            tsmi.Click += (s, e) => { MoveDown(); };
            treeView1.ContextMenuStrip.Items.Add(tsmi);

            tss = new ToolStripSeparator();
            treeView1.ContextMenuStrip.Items.Add(tss);

            tsmi = new ToolStripMenuItem { Name = "contextMenuAdd", Text = "Add" };
            treeView1.ContextMenuStrip.Items.Add(tsmi);

            propertyTable1.ProcessChange = (c) => { DocumentChanged(); };

            //NewDocument();
        }

        private ToolStripMenuItem GetMenuItem(string name)
        {
            ToolStripMenuItem rtnVal = null;

            foreach (ToolStripItem tsmi in treeView1.ContextMenuStrip.Items)
            {
                if (tsmi.Name == name)
                {
                    rtnVal = (ToolStripMenuItem)tsmi;
                    break;
                }
            }

            return rtnVal;
        }

        private void DocToTree(TreeView tree)
        {
            tree.Nodes.Clear();
            lastAtLevel.Clear();

            tree.Nodes.Add("Document").Tag = doc;
            TreeNode modelNode = tree.Nodes.Add("Model");
            TreeNode relationsNode = tree.Nodes.Add("Relations");

            lastAtLevel.Add(-1, modelNode);

            foreach (Item command in doc.Commands)
            {
                TreeNode parentNode = lastAtLevel[command.Level - 1];
                TreeNode treeNode = new TreeNode();
                if (command.Parameters.Count > 1)
                {
                    treeNode.Text = command.GetValue(1);
                }
                else
                {
                    treeNode.Text = command.Command;
                }
                treeNode.Tag = command;
                parentNode.Nodes.Add(treeNode);
                SetAtLevel(command.Level, treeNode);
            }

            foreach (Item relation in doc.Relations)
            {
                TreeNode treeNode = new TreeNode();
                treeNode.Text = $"{relation.Parameters[0]} --> {relation.Parameters[1]}";
                treeNode.Tag = relation;
                relationsNode.Nodes.Add(treeNode);
            }
        }

        private void SetAtLevel(int level, TreeNode node)
        {
            if (lastAtLevel.ContainsKey(level))
            {
                lastAtLevel[level] = node;
            }
            else
            {
                lastAtLevel.Add(level, node);
            }
        }

        private void SetProperties(string setName, bool includeExternal = false, bool includeDatabase = false)
        {
            propertyTable1.SetText("No object selected");
            propertyTable1.ClearProperties();

            if (includeExternal)
            {
                propertyTable1.AddProperty("IsExternal", "External", "Flag");
            }

            if (includeDatabase)
            {
                propertyTable1.AddProperty("IsDatabase", "Database", "Flag");
            }

            switch (setName)
            {
                case "AL":
                    //Alias *, label *
                    propertyTable1.AddProperty("Alias", "Alias*", "Text");
                    propertyTable1.AddProperty("Label", "Label*", "Text");
                    break;
                case "ALType":
                    //Alias *, label *, type
                    propertyTable1.AddProperty("Alias", "Alias*", "Text");
                    propertyTable1.AddProperty("Label", "Label*", "Text");
                    propertyTable1.AddProperty("Type", "Type", "Text");
                    break;
                case "ALD":
                    //Alias *, label *, description
                    propertyTable1.AddProperty("Alias", "Alias*", "Text");
                    propertyTable1.AddProperty("Label", "Label*", "Text");
                    propertyTable1.AddProperty("Description", "Description", "MultiLineText");
                    break;
                case "ALTechD":
                    //Alias *, label *, technology *, description
                    propertyTable1.AddProperty("Alias", "Alias*", "Text");
                    propertyTable1.AddProperty("Label", "Label*", "Text");
                    propertyTable1.AddProperty("Technology", "Technology*", "MultiLineText");
                    propertyTable1.AddProperty("Description", "Description", "MultiLineText");
                    break;
                case "ALTech":
                    //Alias *, label *, technology *
                    propertyTable1.AddProperty("Alias", "Alias*", "Text");
                    propertyTable1.AddProperty("Label", "Label*", "Text");
                    propertyTable1.AddProperty("Technology", "Technology*", "MultiLineText");
                    break;
                case "FTLTech":
                    //From *, to *, label, technology
                    //propertyTable1.AddProperty("From", "From*", "Text");
                    //propertyTable1.AddProperty("To", "To*", "Text");

                    BuildAliasMap(treeView1.Nodes[1]);
                    propertyTable1.AddProperty("From", "From*", "Choice", itemAliases);
                    propertyTable1.AddProperty("To", "To*", "Choice", itemAliases);
                    propertyTable1.AddProperty("Label", "Label", "Text");
                    propertyTable1.AddProperty("Technology", "Technology*", "MultiLineText");
                    break;
                case "Document":
                    propertyTable1.AddProperty("DocumentName", "Document Name*", "Text");
                    propertyTable1.AddProperty("DocumentType", "Document Type*", "Choice", "Context", "Container", "Component", "Dynamic (not yet)", "Deployment");
                    propertyTable1.AddProperty("Title", "Title", "Text");
                    propertyTable1.AddProperty("Sketch", "Show as sketch", "Flag");
                    propertyTable1.AddProperty("ShowLegend", "Show legend", "Flag");
                    propertyTable1.AddProperty("TopDown", "Lay out Top-Down", "Flag");
                    break;
            }
        }

        private void BuildDocument()
        {
            doc = treeView1.Nodes[0].Tag as Document;

            doc.Commands.Clear();
            BuildDocumentModel(treeView1.Nodes[1].Nodes);

            doc.Relations.Clear();
            BuildDocumentRelations(treeView1.Nodes[2].Nodes);
        }

        private void BuildDocumentModel(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes) 
            {
                Item item = node.Tag as Item;
                if (item != null) 
                {
                    item.Level = node.Level - 1;
                    doc.Commands.Add(item);

                    BuildDocumentModel(node.Nodes);
                }
            }
        }

        private void BuildDocumentRelations(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                Item item = node.Tag as Item;
                if (item != null)
                {
                    doc.Relations.Add(item);
                }
            }
        }

        private void RenderDocument(string rawText)
        {
            string code = Encoder.EncodeUrl(rawText);

            try
            {
                WebRenderer r = new WebRenderer();
                var bytes = r.Render(code);
                Image image = Image.FromStream(new MemoryStream(bytes));
                zoomPanImageBox1.Image = image;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private string GetNewName(string commandType)
        {
            BuildAliasMap(treeView1.Nodes[1]);

            int counter = 1;
            string counterString;
            string nameCandidate;

            do
            {
                counterString = counter.ToString().PadLeft(3, '0');
                nameCandidate = $"{commandType}_{counterString}";
                counter++;
            } while (itemAliases.ContainsKey(nameCandidate));

            return nameCandidate;
        }

        Dictionary<string, string> itemAliases = new Dictionary<string, string>();

        private void BuildAliasMap(TreeNode parentNode, bool firstNode = true)
        {
            if (firstNode)
            {
                itemAliases.Clear();
            }
            foreach (TreeNode node in parentNode.Nodes)
            {
                Item item = node.Tag as Item;
                if (item != null)
                {
                    itemAliases.Add(item.Parameters[0], item.Parameters[1].Trim('\"'));
                    if (node.Nodes.Count > 0)
                    {
                        BuildAliasMap(node, false);
                    }
                }
            }
        }

        TreeNode previousNode = null;

        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            SaveItemData();

            propertyTable1.SetText("No object selected");
            propertyTable1.ClearProperties();
        }

        bool dirty = false;

        Timer autoSaveTimer;

        private void AutoSaveTimer_Tick(object sender, EventArgs e)
        {
            autoSaveTimer.Stop();
            autoSaveTimer.Enabled = false;

            if (!string.IsNullOrEmpty(fileName))
            {
                BuildDocument();
                string newDocText = DocumentBuilder.Build(doc);

                File.WriteAllText(fileName, newDocText);

                dirty = false;
            }

            RefreshDisplay();
        }

        private void DocumentChanged()
        {
            dirty = true;
            autoSaveTimer.Enabled = true;
            autoSaveTimer.Stop();   // in case the user is still typing
            autoSaveTimer.Start();
        }

        private void SaveItemData()
        {
            if (previousNode != null)
            {
                Document doc = (previousNode.Tag as Document);
                if (doc != null)
                {
                    doc.DocumentName = propertyTable1.GetValue("DocumentName");
                    doc.DocumentType = propertyTable1.GetValue("DocumentType");
                    doc.Title = propertyTable1.GetValue("Title");
                    doc.Sketch = propertyTable1.GetValue("Sketch") == "Y";
                    doc.ShowLegend = propertyTable1.GetValue("ShowLegend") == "Y";
                    doc.TopDown = propertyTable1.GetValue("TopDown") == "Y";
                }

                Item item = (previousNode.Tag as Item);

                if (item != null)
                {
                    switch (item.Command)
                    {
                        case "Boundary":
                            item.SetValue(0, propertyTable1.GetValue("Alias"), false);
                            item.SetValue(1, propertyTable1.GetValue("Label"));
                            item.SetValue(2, propertyTable1.GetValue("Type"));
                            break;
                        case "System_Boundary":
                        case "Enterprise_Boundary":
                        case "Container_Boundary":
                            item.SetValue(0, propertyTable1.GetValue("Alias"), false);
                            item.SetValue(1, propertyTable1.GetValue("Label"));
                            break;
                        case "Relationship":
                        case "Interact":
                        case "Interact2":
                            item.SetValue(0, propertyTable1.GetValue("From"), false);
                            item.SetValue(1, propertyTable1.GetValue("To"), false);
                            item.SetValue(2, propertyTable1.GetValue("Label"));
                            item.SetValue(3, propertyTable1.GetValue("Technology"));
                            break;
                        case "Person":
                        case "Person_Ext":
                        case "System":
                        case "System_Ext":
                        case "SystemDb":
                        case "SystemDb_Ext":
                            item.IsExternal = propertyTable1.GetValue("IsExternal") == "Y";
                            item.IsDatabase = propertyTable1.GetValue("IsDatabase") == "Y";
                            item.SetValue(0, propertyTable1.GetValue("Alias"), false);
                            item.SetValue(1, propertyTable1.GetValue("Label"));
                            item.SetValue(2, propertyTable1.GetValue("Description"));
                            break;
                        case "Container":
                        case "Container_Ext":
                        case "ContainerDb":
                        case "ContainerDb_Ext":
                        case "Component":
                        case "ComponentDb":
                            item.IsExternal = propertyTable1.GetValue("IsExternal") == "Y";
                            item.IsDatabase = propertyTable1.GetValue("IsDatabase") == "Y";
                            item.SetValue(0, propertyTable1.GetValue("Alias"), false);
                            item.SetValue(1, propertyTable1.GetValue("Label"));
                            item.SetValue(2, propertyTable1.GetValue("Technology"));
                            item.SetValue(3, propertyTable1.GetValue("Description"));
                            break;
                        case "Node":
                            item.SetValue(0, propertyTable1.GetValue("Alias"), false);
                            item.SetValue(1, propertyTable1.GetValue("Label"));
                            item.SetValue(2, propertyTable1.GetValue("Technology"));
                            break;
                        case "Rel":
                            item.SetValue(0, propertyTable1.GetValue("From"), false);
                            item.SetValue(1, propertyTable1.GetValue("To"), false);
                            item.SetValue(2, propertyTable1.GetValue("Label"));
                            item.SetValue(3, propertyTable1.GetValue("Technology"));
                            break;
                        default:
                            MessageBox.Show($"{item.Command} is not handled");
                            break;
                    }
                    switch (item.Command)
                    {
                        case "Rel":
                            previousNode.Text = $"{item.Parameters[0]} --> {item.Parameters[1]}";
                            break;
                        default:
                            previousNode.Text = item.GetValue(1);
                            break;
                    }
                }
            }
        }

        private void EnableCCP(TreeNode node)
        {
            // Document
            if (IsNodeInCorrectSection(treeView1.SelectedNode, treeView1.Nodes[0]))
            {
                AddDropDownButton.Enabled = false;
                cutToolStripButton.Enabled = false;
                copyToolStripButton.Enabled = false;
                pasteToolStripButton.Enabled = false;

                GetMenuItem("contextMenuAdd").Enabled = false;

                GetMenuItem("contextMenuCut").Enabled = false;
                GetMenuItem("contextMenuCopy").Enabled = false;
                GetMenuItem("contextMenuPaste").Enabled = false;
            }
            else if (IsNodeInCorrectSection(treeView1.SelectedNode, treeView1.Nodes[1]))
            {
                AddDropDownButton.Enabled = true;
                GetMenuItem("contextMenuAdd").Enabled = true;

                if (treeView1.SelectedNode == treeView1.Nodes[1])
                {
                    cutToolStripButton.Enabled = false;
                    copyToolStripButton.Enabled = false;
                    GetMenuItem("contextMenuCut").Enabled = false;
                    GetMenuItem("contextMenuCopy").Enabled = false;
                }
                else
                {
                    cutToolStripButton.Enabled = true;
                    copyToolStripButton.Enabled = true;
                    GetMenuItem("contextMenuCut").Enabled = true;
                    GetMenuItem("contextMenuCopy").Enabled = true;
                }
                pasteToolStripButton.Enabled = ((clipboardNode!= null) && ((clipboardNode.Tag as Item).Command != "Rel"));
                GetMenuItem("contextMenuPaste").Enabled = ((clipboardNode != null) && ((clipboardNode.Tag as Item).Command != "Rel"));
            }
            else if (IsNodeInCorrectSection(treeView1.SelectedNode, treeView1.Nodes[2]))
            {
                if (treeView1.SelectedNode == treeView1.Nodes[2])
                {
                    AddDropDownButton.Enabled = true;       // no heirarchy - must be on the Relationships node
                    GetMenuItem("contextMenuAdd").Enabled = true;

                    cutToolStripButton.Enabled = false;
                    copyToolStripButton.Enabled = false;
                    GetMenuItem("contextMenuCut").Enabled = true;
                    GetMenuItem("contextMenuCopy").Enabled = true;
                }
                else
                {
                    AddDropDownButton.Enabled = false;      // no heirarchy - must be on the Relationships node
                    GetMenuItem("contextMenuAdd").Enabled = false;

                    cutToolStripButton.Enabled = true;
                    copyToolStripButton.Enabled = true;
                    GetMenuItem("contextMenuCut").Enabled = true;
                    GetMenuItem("contextMenuCopy").Enabled = true;
                }
                pasteToolStripButton.Enabled = ((clipboardNode != null) && ((clipboardNode.Tag as Item).Command == "Rel"));
            }
        }

        private void EnableUD(TreeNode node)
        {
            // Document
            if (IsNodeInCorrectSection(node, treeView1.Nodes[0]))
            {
                moveDownButton.Enabled = false;
                moveUpButton.Enabled = false;
                GetMenuItem("contextMenuMoveUp").Enabled = false;
                GetMenuItem("contextMenuMoveDown").Enabled = false;
            }
            else if (IsNodeInCorrectSection(node, treeView1.Nodes[1]))
            {
                if (treeView1.SelectedNode == treeView1.Nodes[1])
                {
                    moveDownButton.Enabled = false;
                    moveUpButton.Enabled = false;
                    GetMenuItem("contextMenuMoveUp").Enabled = false;
                    GetMenuItem("contextMenuMoveDown").Enabled = false;
                }
                else
                {
                    moveDownButton.Enabled = false;
                    moveUpButton.Enabled = false;
                    GetMenuItem("contextMenuMoveUp").Enabled = false;
                    GetMenuItem("contextMenuMoveDown").Enabled = false;

                    TreeNode parentNode = treeView1.SelectedNode.Parent;
                    if (parentNode != null)
                    {
                        int numNodes = parentNode.Nodes.Count;

                        int nodeIndex = GetIndex(node);

                        // it has to be found
                        if (nodeIndex != -1)
                        {
                            if (nodeIndex > 0)
                            {
                                moveUpButton.Enabled = true;
                                GetMenuItem("contextMenuMoveUp").Enabled = true;
                            }

                            if (nodeIndex < (numNodes - 1))
                            {
                                moveDownButton.Enabled = true;
                                GetMenuItem("contextMenuMoveDown").Enabled = true;
                            }
                        }
                    }
                }
            }
            else if (IsNodeInCorrectSection(node, treeView1.Nodes[2]))
            {
                moveDownButton.Enabled = false;
                moveUpButton.Enabled = false;
                GetMenuItem("contextMenuMoveUp").Enabled = false;
                GetMenuItem("contextMenuMoveDown").Enabled = false;
            }
        }

        TreeNode clipboardNode = null;

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ConfigureContextMenu(e.Node);

            EnableCCP(e.Node);
            EnableUD(e.Node);

            FillAddMenus(AddDropDownButton);
            FillAddMenus(GetMenuItem("contextMenuAdd"));

            Document doc = (e.Node.Tag as Document);
            if (doc != null)
            {
                // make this more descriptive later
                SetProperties("Document");
                propertyTable1.SetText("Document");

                propertyTable1.SetValue("DocumentName", doc.DocumentName);
                propertyTable1.SetValue("DocumentType", doc.DocumentType);
                propertyTable1.SetValue("Title", doc.Title);
                propertyTable1.SetValue("Sketch", doc.Sketch ? "Y" : "N");
                propertyTable1.SetValue("ShowLegend", doc.ShowLegend ? "Y" : "N");
                propertyTable1.SetValue("TopDown", doc.TopDown ? "Y" : "N");
            }

            Item item = (e.Node.Tag as Item);

            if (item != null)
            {
                switch (item.Command)
                {
                    case "Boundary":
                        SetProperties("ALType");
                        propertyTable1.SetValue("Alias", item.GetValue(0));
                        propertyTable1.SetValue("Label", item.GetValue(1));
                        propertyTable1.SetValue("Type", item.GetValue(2));
                        break;
                    case "System_Boundary":
                    case "Enterprise_Boundary":
                    case "Container_Boundary":
                        SetProperties("AL");
                        propertyTable1.SetValue("Alias", item.GetValue(0));
                        propertyTable1.SetValue("Label", item.GetValue(1));
                        break;
                    case "Relationship":
                    case "Interact":
                    case "Interact2":
                        SetProperties("FTLTech");
                        propertyTable1.SetValue("From", item.GetValue(0));
                        propertyTable1.SetValue("To", item.GetValue(1));
                        propertyTable1.SetValue("Label", item.GetValue(2));
                        propertyTable1.SetValue("Technology", item.GetValue(3));
                        break;
                    case "Person":
                    //case "Person_Ext":
                        SetProperties("ALD", includeExternal: true);
                        propertyTable1.SetValue("IsExternal", item.IsExternal ? "Y" : "N");
                        propertyTable1.SetValue("Alias", item.GetValue(0));
                        propertyTable1.SetValue("Label", item.GetValue(1));
                        propertyTable1.SetValue("Description", item.GetValue(2));
                        break;
                    case "System":
                    //case "System_Ext":
                    //case "SystemDb":
                    //case "SystemDb_Ext":
                        SetProperties("ALD", includeExternal: true, includeDatabase: true);
                        propertyTable1.SetValue("IsExternal", item.IsExternal ? "Y" : "N");
                        propertyTable1.SetValue("IsDatabase", item.IsDatabase ? "Y" : "N");
                        propertyTable1.SetValue("Alias", item.GetValue(0));
                        propertyTable1.SetValue("Label", item.GetValue(1));
                        propertyTable1.SetValue("Description", item.GetValue(2));
                        break;
                    case "Container":
                    //case "Container_Ext":
                    //case "ContainerDb":
                    //case "ContainerDb_Ext":
                        SetProperties("ALTechD", includeExternal: true, includeDatabase: true);
                        propertyTable1.SetValue("IsExternal", item.IsExternal ? "Y" : "N");
                        propertyTable1.SetValue("IsDatabase", item.IsDatabase ? "Y" : "N");
                        propertyTable1.SetValue("Alias", item.GetValue(0));
                        propertyTable1.SetValue("Label", item.GetValue(1));
                        propertyTable1.SetValue("Technology", item.GetValue(2));
                        propertyTable1.SetValue("Description", item.GetValue(3));
                        break;
                    case "Component":
                    //case "ComponentDb":
                        SetProperties("ALTechD", includeDatabase: true);
                        propertyTable1.SetValue("IsDatabase", item.IsDatabase ? "Y" : "N");
                        propertyTable1.SetValue("Alias", item.GetValue(0));
                        propertyTable1.SetValue("Label", item.GetValue(1));
                        propertyTable1.SetValue("Technology", item.GetValue(2));
                        propertyTable1.SetValue("Description", item.GetValue(3));
                        break;
                    case "Node":
                        SetProperties("ALTech");
                        propertyTable1.SetValue("Alias", item.GetValue(0));
                        propertyTable1.SetValue("Label", item.GetValue(1));
                        propertyTable1.SetValue("Technology", item.GetValue(2));
                        break;
                    case "Rel":
                        SetProperties("FTLTech");
                        propertyTable1.SetValue("From", item.GetValue(0));
                        propertyTable1.SetValue("To", item.GetValue(1));
                        propertyTable1.SetValue("Label", item.GetValue(2));
                        propertyTable1.SetValue("Technology", item.GetValue(3));
                        break;
                    default:
                        MessageBox.Show($"{item.Command} is not handled");
                        break;
                }
                propertyTable1.SetText(DisplayNames.GetDisplayName(item.Command));
            }

            previousNode = e.Node;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dirty)
            {
                if (MessageBox.Show("Save before closing?", "Unsaved changes", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
