namespace ConnectomeVisualizer.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.panel1 = new System.Windows.Forms.Panel();
            this.DrawEdgeFlag = new System.Windows.Forms.CheckBox();
            this.SaveImageFlag = new System.Windows.Forms.CheckBox();
            this.DrawCellFlag = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 44);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(412, 397);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(412, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.DrawCellFlag);
            this.panel1.Controls.Add(this.DrawEdgeFlag);
            this.panel1.Controls.Add(this.SaveImageFlag);
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(412, 44);
            this.panel1.TabIndex = 2;
            // 
            // DrawEdgeFlag
            // 
            this.DrawEdgeFlag.AutoSize = true;
            this.DrawEdgeFlag.Location = new System.Drawing.Point(99, 25);
            this.DrawEdgeFlag.Name = "DrawEdgeFlag";
            this.DrawEdgeFlag.Size = new System.Drawing.Size(75, 16);
            this.DrawEdgeFlag.TabIndex = 3;
            this.DrawEdgeFlag.Text = "DrawEdge";
            this.DrawEdgeFlag.UseVisualStyleBackColor = true;
            this.DrawEdgeFlag.CheckedChanged += new System.EventHandler(this.DrawEdgeFlag_CheckedChanged);
            // 
            // SaveImageFlag
            // 
            this.SaveImageFlag.AutoSize = true;
            this.SaveImageFlag.Location = new System.Drawing.Point(3, 25);
            this.SaveImageFlag.Name = "SaveImageFlag";
            this.SaveImageFlag.Size = new System.Drawing.Size(90, 16);
            this.SaveImageFlag.TabIndex = 2;
            this.SaveImageFlag.Text = "SaveProcess";
            this.SaveImageFlag.UseVisualStyleBackColor = true;
            // 
            // DrawCellFlag
            // 
            this.DrawCellFlag.AutoSize = true;
            this.DrawCellFlag.Checked = true;
            this.DrawCellFlag.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DrawCellFlag.Location = new System.Drawing.Point(180, 25);
            this.DrawCellFlag.Name = "DrawCellFlag";
            this.DrawCellFlag.Size = new System.Drawing.Size(70, 16);
            this.DrawCellFlag.TabIndex = 4;
            this.DrawCellFlag.Text = "DrawCell";
            this.DrawCellFlag.UseVisualStyleBackColor = true;
            this.DrawCellFlag.CheckedChanged += new System.EventHandler(this.DrawCellFlag_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 441);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox SaveImageFlag;
        private System.Windows.Forms.CheckBox DrawEdgeFlag;
        private System.Windows.Forms.CheckBox DrawCellFlag;
    }
}