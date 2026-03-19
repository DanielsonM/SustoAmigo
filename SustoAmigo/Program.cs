using System;
using System.Windows.Forms;

namespace SustoAmigo
{
    internal static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FrmConfiguracao frmConfiguracao = new FrmConfiguracao();
            frmConfiguracao.ShowDialog();

            if (frmConfiguracao.booIniciadoPorConfiguracao)
                Application.Run(new Principal());
        }
    }
}