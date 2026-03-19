using System;
using System.Windows.Forms;
using SustoAmigo.Configuracoes;

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

            using (var frmConfiguracao = new FrmConfiguracao())
            {
                frmConfiguracao.ShowDialog();
                if (frmConfiguracao.IniciadoPorConfiguracao)
                    Application.Run(new Principal());
            }
        }
    }
}