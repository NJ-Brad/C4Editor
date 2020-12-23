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

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            c4EditorControl1.LoadFile(@"c:\autoany\context.puml");
            //c4EditorControl1.LoadFile(@"c:\autoany\component.puml");
            //c4EditorControl1.LoadFile(@"c:\autoany\container.puml");
            //c4EditorControl1.LoadFile(@"c:\autoany\deployment.puml");

            // dynamic will be for a future release
            //c4EditorControl1.LoadFile(@"c:\autoany\dynamic.puml");
        }
    }
}
