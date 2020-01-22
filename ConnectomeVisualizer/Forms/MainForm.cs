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
        public MainForm()
        {
            InitializeComponent();

        }

        delegate void SetImageDelegate(Connectome.Visualize.Image image);
        public void SetImage(Connectome.Visualize.Image image)
        {
            if (InvokeRequired)
            {
                Invoke(new SetImageDelegate(SetImage), image);
                return;
            }
            else
            {
                if (image.Bitmap != null)
                {
                    pictureBox1.Image = image.Bitmap;
                    GC.Collect();
                }
                this.Text = image.CreateTimeSpan.TotalMilliseconds.ToString();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Connectome.Core.Terminate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            VisualizeObjects.ImageSize = Math.Min(pictureBox1.Width, pictureBox1.Height);
            if (VisualizeObjects.ImageSize > 0)
            {
                VisualizeObjects.Th += 0.01;
                if (VisualizeObjects.Th > 2 * Math.PI)
                { VisualizeObjects.Th -= 2 * Math.PI; }
                VisualizeObjects.CalcCameraPos();
                Connectome.Core.RequestLatestState();
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {


            timer1.Enabled = true;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {

            timer1.Enabled = false;
        }
    }
}
