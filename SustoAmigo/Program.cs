using SustoAmigo.Configuracoes;
using SustoAmigo.Interfaces;
using SustoAmigo.Services;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

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

                
                    var configuracao = ConfiguracaoXml.Instancia;
                    var redeController = new RedeController();

                    if (configuracao.ModoRede)
                    {
                        string strArquivo = "Configuracao.xml";
                        string caminhoArquivo = Path.Combine(Application.StartupPath, strArquivo);
                        var doc = XDocument.Load(caminhoArquivo);
                        var root = doc.Root;
                        int porta = ConfiguracaoXml.ObterValorInteiro(root, "Porta", 5000);
                        redeController.IniciarServidorHttp(porta);
                    }

                    Application.Run(new Principal(new MediaService(), configuracao, redeController));
                
            }
        }
    }
}