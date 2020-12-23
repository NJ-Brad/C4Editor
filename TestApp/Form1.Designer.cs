
namespace TestApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.c4EditorControl1 = new C4Editor.C4EditorControl();
            this.SuspendLayout();
            // 
            // c4EditorControl1
            // 
            this.c4EditorControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.c4EditorControl1.Location = new System.Drawing.Point(0, 0);
            this.c4EditorControl1.Name = "c4EditorControl1";
            this.c4EditorControl1.Size = new System.Drawing.Size(800, 450);
            this.c4EditorControl1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.c4EditorControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private C4Editor.C4EditorControl c4EditorControl1;
    }
}

