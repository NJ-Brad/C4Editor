using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string fileName = "";

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Plant-UML file (*.puml)|*.puml|All Files (*.*)|*.*";
            ofd.CheckFileExists = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                fileName = ofd.FileName;
                c4EditorControl1.LoadFile(ofd.FileName);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //if (string.IsNullOrEmpty(fileName))
            //{
            //    SaveAs();
            //}
            //else
            //{
                Save();
            //}
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void SaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Markdown Files (*.md)|*.md|All Files (*.*)|*.*";
            sfd.OverwritePrompt = true;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileName = sfd.FileName;
                Save();
                //File.WriteAllText(fileName, richTextBox1.Text);
            }
        }

        private void Save()
        {
            c4EditorControl1.SaveDocument();
            //File.WriteAllText(fileName, richTextBox1.Text);
        }
    }
}
