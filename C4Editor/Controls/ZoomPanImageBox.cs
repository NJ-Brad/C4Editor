// from https://www.codeproject.com/Articles/12331/ImageBox-Control-with-Zoom-Pan-Capability
/* 
 * Developed by Shannon Young.  http://www.smallwisdom.com
 * Copyright 2005
 * 
 * You are welcome to use, edit, and redistribute this code.
 * 
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace C4Editor
{
	/// <summary>
	/// ZoomPanImageBox is a specialized ImageBox with Pan and Zoom control.
	/// </summary>
	internal class ZoomPanImageBox : System.Windows.Forms.UserControl
	{
		/// <summary>
		/// The zoom factor for this control.  Currently, it is hardcoded, 
		/// but perhaps a nice addition would be to set this?
		/// </summary>
		private double[] zoomFactor = { .25, .33, .50, .66, .80, 1, 1.25, 1.5, 2.0, 2.5, 3.0 };
		private System.Windows.Forms.Panel imagePanel;
		private System.Windows.Forms.PictureBox imgBox;
        private ZoomSlider zoomSlider1;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

		#region Construct, Dispose

		public ZoomPanImageBox()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Initialize anything not included in the designer
			init();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.imgBox = new System.Windows.Forms.PictureBox();
            this.imagePanel = new System.Windows.Forms.Panel();
            this.zoomSlider1 = new C4Editor.ZoomSlider();
            ((System.ComponentModel.ISupportInitialize)(this.imgBox)).BeginInit();
            this.imagePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // imgBox
            // 
            this.imgBox.Location = new System.Drawing.Point(0, 0);
            this.imgBox.Name = "imgBox";
            this.imgBox.Size = new System.Drawing.Size(200, 200);
            this.imgBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgBox.TabIndex = 6;
            this.imgBox.TabStop = false;
            // 
            // imagePanel
            // 
            this.imagePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.imagePanel.AutoScroll = true;
            this.imagePanel.Controls.Add(this.imgBox);
            this.imagePanel.Location = new System.Drawing.Point(0, 0);
            this.imagePanel.Name = "imagePanel";
            this.imagePanel.Size = new System.Drawing.Size(824, 574);
            this.imagePanel.TabIndex = 7;
            // 
            // zoomSlider1
            // 
            this.zoomSlider1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.zoomSlider1.Location = new System.Drawing.Point(561, 580);
            this.zoomSlider1.Name = "zoomSlider1";
            this.zoomSlider1.Size = new System.Drawing.Size(256, 28);
            this.zoomSlider1.TabIndex = 8;
            this.zoomSlider1.Value = 5;
            this.zoomSlider1.ZoomChanged = null;
            // 
            // ZoomPanImageBox
            // 
            this.Controls.Add(this.zoomSlider1);
            this.Controls.Add(this.imagePanel);
            this.Name = "ZoomPanImageBox";
            this.Size = new System.Drawing.Size(824, 608);
            ((System.ComponentModel.ISupportInitialize)(this.imgBox)).EndInit();
            this.imagePanel.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Initialization code goes here.
		/// </summary>
		private void init()
		{
			this.zoomSlider1.ZoomChanged += (v) => { setZoom(v); };
		}

		/// <summary>
		/// Image loaded into the box.
		/// </summary>
		[Browsable(true),
		Description("Image loaded into the box.")]
		public Image Image
		{
			get
			{
				return imgBox.Image;
			}
			set
			{
				// Set the image value
				imgBox.Image = value;

				// enable the zoom control if this is not a null image
				zoomSlider1.Enabled = (imgBox.Image != null);

				if (zoomSlider1.Enabled)
				{
					// reset zoom control
					zoomSlider1.Value = this.zoomSlider1.Maximum / 2;

					// Initially, the zoom factor is 100% so set the
					// ImageBox size equal to the Image size.
					if (imgBox.Image != null)
					{
						imgBox.Size = imgBox.Image.Size;
				//		setZoom();
					}
				}
				else
				{
					// If null image, then reset the imgBox size
					// to the size of the panel so that there are no
					// scroll bars.
					imgBox.Size = imagePanel.Size;
				}
			}
		}

		private void scrollZoom_Scroll(object sender, System.EventArgs e)
		{
			setZoom();
		}

		private void setZoom()
		{
			// The scrollZoom changed so reset the zoom factor
			// based on the scrollZoom TrackBar position.
			double newZoom = zoomFactor[zoomSlider1.Value];

			if (imgBox.Image != null)
			{
				// Set the ImageBox width and height to the new zoom
				// factor by multiplying the Image inside the Imagebox
				// by the new zoom factor.
				imgBox.Width = Convert.ToInt32(imgBox.Image.Width * newZoom);
				imgBox.Height = Convert.ToInt32(imgBox.Image.Height * newZoom);
			}
		}
		private void setZoom(double value)
		{
			double newZoom = value;

			if (imgBox.Image != null)
			{
				// Set the ImageBox width and height to the new zoom
				// factor by multiplying the Image inside the Imagebox
				// by the new zoom factor.
				imgBox.Width = Convert.ToInt32(imgBox.Image.Width * newZoom);
				imgBox.Height = Convert.ToInt32(imgBox.Image.Height * newZoom);
			}
		}
	}// end class
}// end namespace
