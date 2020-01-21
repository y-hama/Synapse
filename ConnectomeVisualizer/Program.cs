using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConnectomeVisualizer
{
    static class Program
    {
        private static Forms.MainForm Form { get; set; }

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Connectome.Core.DataUpload += Core_DataUpload;

            Connectome.Core.Initialize();
            Connectome.Core.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run((Form = new Forms.MainForm()));
        }

        private static void Core_DataUpload(Connectome.Core.DataUploadEventArgs e)
        {
            Form.SetImage(Connectome.Visualize.Imaging(e, VisualizeObjects.CameraLocation, VisualizeObjects.ViewLocation, VisualizeObjects.ImageSize));
        }
    }
}
