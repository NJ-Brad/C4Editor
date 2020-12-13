using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C4Editor
{
    internal partial class ZoomSlider : UserControl
    {
        private double[] zoomFactor = { .25, .33, .50, .66, .80, 1, 1.25, 1.5, 2.0, 2.5, 3.0 };
        public ZoomSlider()
        {
            InitializeComponent();

            ContextMenuStrip cms = new ContextMenuStrip();
            cms.ShowImageMargin = false;
            cms.Items.Add("25%", null, (s,a) => { Changed(.25); splitButton1.Text = "25%"; trackBar1.Value = 0; });
            cms.Items.Add("33%", null, (s, a) => { Changed(.33); splitButton1.Text = "33%"; trackBar1.Value = 1; });
            cms.Items.Add("50%", null, (s, a) => { Changed(.50); splitButton1.Text = "50%"; trackBar1.Value = 2; });
            cms.Items.Add("66%", null, (s, a) => { Changed(.66); splitButton1.Text = "66%"; trackBar1.Value = 3; });
            cms.Items.Add("80%", null, (s, a) => { Changed(.80); splitButton1.Text = "80%"; trackBar1.Value = 4; });
            cms.Items.Add("100%", null, (s, a) => { Changed(1); splitButton1.Text = "100%"; trackBar1.Value = 5; });
            cms.Items.Add("125%", null, (s, a) => { Changed(1.25); splitButton1.Text = "125%"; trackBar1.Value = 6; });
            cms.Items.Add("150%", null, (s, a) => { Changed(1.5); splitButton1.Text = "150%"; trackBar1.Value = 7; });
            cms.Items.Add("200%", null, (s, a) => { Changed(2); splitButton1.Text = "200%"; trackBar1.Value = 8; });
            cms.Items.Add("250%", null, (s, a) => { Changed(2.5); splitButton1.Text = "250%"; trackBar1.Value = 9; });
            cms.Items.Add("300%", null, (s, a) => { Changed(3); splitButton1.Text = "300%"; trackBar1.Value = 10; });
            splitButton1.DropDownMenu = cms;

            splitButton1.Text = "100%";
        }

        public int Maximum { get { return trackBar1.Maximum; } }

        private void TrackBar1_Scroll(object sender, System.EventArgs e)
        {
            throw new System.NotImplementedException();
        }

        public delegate void ZoomChangedHandler(double zoomFactor);

        public ZoomChangedHandler ZoomChanged { get; set; }

        private void Changed(double zoomFactor)
        {
            if (ZoomChanged != null)
            {
                ZoomChanged(zoomFactor);
            }
        }

        private void zoomOut_Click(object sender, EventArgs e)
        {
            if (trackBar1.Value > trackBar1.Minimum)
            {
                trackBar1.Value--;
                splitButton1.Text = splitButton1.DropDownMenu.Items[trackBar1.Value].Text;
                Changed(zoomFactor[trackBar1.Value]);
            }
        }

        private void zoomIn_Click(object sender, EventArgs e)
        {
            if (trackBar1.Value < trackBar1.Maximum)
            {
                trackBar1.Value++;
                splitButton1.Text = splitButton1.DropDownMenu.Items[trackBar1.Value].Text;
                Changed(zoomFactor[trackBar1.Value]);
            }
        }

        public int Value
        {
            get
            {
                return trackBar1.Value;
            }
            set 
            {
                trackBar1.Value = value;
                splitButton1.Text = splitButton1.DropDownMenu.Items[trackBar1.Value].Text;
            }
        }

        private void splitButton1_DropDownMenuChanged(object sender, EventArgs e)
        {

        }
    }
}
