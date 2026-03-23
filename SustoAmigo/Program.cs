using SustoAmigo.Configuracoes;
using SustoAmigo.Services;
using System;
using System.IO;
using System.Linq;
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
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var configuracao = ConfiguracaoXml.i;
            var redeController = new RedeController();

            if (configuracao.ModoRede)
            {
                string strArquivo = "Configuracao.xml";
                string caminhoArquivo = Path.Combine(Application.StartupPath, strArquivo);
                var doc = XDocument.Load(caminhoArquivo);
                var root = doc.Root;
                int porta = ConfiguracaoXml.ObterValorInteiro(root, "Porta", 5000);
                ConfiguracaoXml.i.CriarPastasPadrao();
                //ConfiguracaoXml.i.ObterCaminhoSomSelecionado();
                redeController.IniciarServidorHttp(porta);
            }

           // bool reiniciarAoFechar = !args.Contains("--no-restart");

            Application.Run(new Principal(new MediaService(), configuracao, redeController)
            {
               // ReiniciarAoFechar = reiniciarAoFechar
            });
        }
    }
}