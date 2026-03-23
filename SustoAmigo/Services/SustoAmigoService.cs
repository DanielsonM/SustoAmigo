using SustoAmigo.Configuracoes;
using SustoAmigo.Interfaces;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using Topshelf;

namespace SustoAmigo.Services
{
    public class SustoAmigoService : ServiceControl
    {
        private Principal _principal;
        private RedeController _redeController;
        private readonly IConfigService _configuracao;

        public SustoAmigoService()
        {
            _configuracao = ConfiguracaoXml.i;
        }

        public bool Start(HostControl hostControl)
        {
            try
            {
                _redeController = RedeController.i;

                if (_configuracao.ModoRede)
                {
                    string strArquivo = "Configuracao.xml";
                    string caminhoArquivo = Path.Combine(Application.StartupPath, strArquivo);

                    if (File.Exists(caminhoArquivo))
                    {
                        var doc = XDocument.Load(caminhoArquivo);
                        var root = doc.Root;
                        int porta = ConfiguracaoXml.ObterValorInteiro(root, "Porta", 5000);
                        _redeController.IniciarServidorHttp(porta);
                    }
                }

                _principal = new Principal(MediaService.i, _configuracao, _redeController);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Stop(HostControl hostControl)
        {
            try
            {
                _principal?.Invoke((MethodInvoker)(() =>
                {
                    _principal.Close();
                }));

                _redeController?.PararServidor();
                _redeController?.Dispose();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Pause(HostControl hostControl)
        {
            return true;
        }

        public bool Continue(HostControl hostControl)
        {
            return true;
        }
    }
}