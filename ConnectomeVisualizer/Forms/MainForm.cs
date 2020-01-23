using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConnectomeVisualizer.Forms
{
    public partial class MainForm : Form
    {
        private MouseButtons PressedButton { get; set; } = MouseButtons.None;

        public MainForm()
        {
            InitializeComponent();

        }

        delegate void SetImageDelegate(Connectome.Visualize.Image image);
        public void SetImage(Connectome.Visualize.Image image)
        {
            if (InvokeRequired)
            {
                try
                {
                    Invoke(new SetImageDelegate(SetImage), image);
                }
                catch (Exception) { }
                return;
            }
            else
            {
                if (image.Bitmap != null)
                {
                    pictureBox1.Image = image.Bitmap;
                    GC.Collect();
                }
                this.Text = image.CreateTimeSpan.TotalMilliseconds.ToString() + " " + Math.Round(image.ProcessTimeSpan.TotalMilliseconds / image.ProcessFrameCount, 2);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Visualization.ImageSize = 2 * Math.Min(pictureBox1.Width, pictureBox1.Height);
            if (Visualization.ImageSize > 0)
            {
                if (PressedButton == MouseButtons.Left)
                {
                    Visualization.Th -= 0.01;
                }
                else if (PressedButton == MouseButtons.Right)
                {
                    Visualization.Th += 0.01;
                }
                if (Visualization.Th > 2 * Math.PI)
                { Visualization.Th -= 2 * Math.PI; }
                if (Visualization.Th < 0)
                { Visualization.Th += 2 * Math.PI; }
                Visualization.CalcCameraPos();
                Visualization.RequestImage(SaveImageFlag.Checked);
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            PressedButton = e.Button;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            PressedButton = MouseButtons.None;
        }

        private void DrawEdgeFlag_CheckedChanged(object sender, EventArgs e)
        {
            Visualization.RequestDrawEdgeLine = DrawEdgeFlag.Checked;
        }

        private void DrawCellFlag_CheckedChanged(object sender, EventArgs e)
        {
            Visualization.RequestDrawCell = DrawCellFlag.Checked;
        }
    }
}
