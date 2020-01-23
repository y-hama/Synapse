using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConnectomeVisualizer
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Connectome.Core.DataUpload += Visualization.Core_DataUpload;
            Connectome.Core.Initialize();
            Connectome.Core.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run((Visualization.Form = new Forms.MainForm()));
            Connectome.Core.Terminate();
        }
    }
}
