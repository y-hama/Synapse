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

        public void SetImage(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                pictureBox1.Image = bitmap;
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
                VisualizeObjects.Th += 0.05;
                if (VisualizeObjects.Th > 2 * Math.PI)
                { VisualizeObjects.Th -= 2 * Math.PI; }
                VisualizeObjects.CalcCameraPos();
                Connectome.Core.RequestLatestState();
            }
        }
    }
}
