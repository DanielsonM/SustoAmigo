using SustoAmigo.Interfaces;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SustoAmigo.Configuracoes
{
    public class ConfiguracaoXml : IConfigService
    {
        private const string NOME_ARQUIVO_PADRAO = "Configuracao.xml";
        private const int INTERVALO_PADRAO = 10;
        private const int TEMPO_EXIBICAO_PADRAO = 5;
        private const int PORTA_PADRAO = 5000;
        private const string IP_SERVIDOR_PADRAO = "0.0.0.0";
        private const bool APENAS_SOM = false;
        private const bool APENAS_IMAGEM = false;

        private readonly IConfigService _configService;
        private readonly IMediaService _mediaService;

        private bool _emReproducao;

        private readonly string _pastaImagens = Path.Combine(Application.StartupPath, "Imagens");
        private readonly string _pastaSons = Path.Combine(Application.StartupPath, "Sons");

        private static readonly Lazy<ConfiguracaoXml> _i = new Lazy<ConfiguracaoXml>(() => new ConfiguracaoXml());
        private static string _caminhoArquivo = Path.Combine(Application.StartupPath, NOME_ARQUIVO_PADRAO);

        public static ConfiguracaoXml i => _i.Value;

        public int IntervaloExecucao { get; private set; }
        public int TempoExibicao { get; private set; }
        public bool ModoRede { get; private set; }
        public int Porta { get; private set; }
        public string IpServidor { get; private set; }
        public string ImagemSelecionada { get; private set; }
        public string SomSelecionado { get; private set; }
        public bool ReiniciarAoFechar { get; set; }
        public bool ApenasSom { get; set; }
        public bool ApenasImagem { get; set; }

        private ConfiguracaoXml() => Carregar();

        public void Salvar(bool booReiniciarAoFechar, int intervalo, int tempoExibicao, bool modoRede, int porta, string ipServidor, string imagem, string som, bool ApenasSom, bool ApenasImagem)
        {
            var doc = new XDocument(
                new XElement("Configuracao",
                 new XElement("ReiniciarAoFechar", intervalo),
                    new XElement("IntervaloExecucao", intervalo),
                    new XElement("TempoExibicao", tempoExibicao),
                    new XElement("ModoRede", modoRede),
                    new XElement("Porta", porta),
                    new XElement("IpServidor", ipServidor ?? string.Empty),
                    new XElement("ImagemSelecionada", imagem ?? string.Empty),
                    new XElement("SomSelecionado", som ?? string.Empty),
                    new XElement("ApenasSom", som ?? string.Empty),
                    new XElement("ApenasImagem", som ?? string.Empty)
                )
            );

            doc.Save(_caminhoArquivo);

            this.ReiniciarAoFechar = booReiniciarAoFechar;
            this.IntervaloExecucao = intervalo;
            this.TempoExibicao = tempoExibicao;
            this.ModoRede = modoRede;
            this.Porta = porta;
            this.IpServidor = ipServidor;
            this.ImagemSelecionada = imagem;
            this.SomSelecionado = som;
            this.ApenasSom = ApenasSom;
            this.ApenasImagem = ApenasImagem;
        }

        public void Carregar()
        {
            try
            {
                if (!File.Exists(_caminhoArquivo))
                {
                    CriarArquivoPadrao();
                }

                var doc = XDocument.Load(_caminhoArquivo);
                var root = doc.Root;

                ReiniciarAoFechar = ObterValorBooleano(root, "ReiniciarAoFechar", false);
                IntervaloExecucao = ObterValorInteiro(root, "IntervaloExecucao", INTERVALO_PADRAO);
                TempoExibicao = ObterValorInteiro(root, "TempoExibicao", TEMPO_EXIBICAO_PADRAO);
                ModoRede = ObterValorBooleano(root, "ModoRede", false);
                Porta = ObterValorInteiro(root, "Porta", PORTA_PADRAO);
                IpServidor = ObterValorString(root, "IpServidor", IP_SERVIDOR_PADRAO);
                ImagemSelecionada = ObterValorString(root, "ImagemSelecionada", string.Empty);
                SomSelecionado = ObterValorString(root, "SomSelecionado", string.Empty);
                ApenasSom = ObterValorBooleano(root, "ApenasSom", APENAS_SOM);
                ApenasImagem = ObterValorBooleano(root, "ApenasImagem", APENAS_IMAGEM);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar configurações: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                CriarArquivoPadrao();
                Carregar();
            }
        }

        public string ObterCaminhoSomSelecionado()
        {
            if (!string.IsNullOrEmpty(_configService.SomSelecionado))
            {
                var caminho = Path.Combine(_pastaSons, _configService.SomSelecionado);
                if (File.Exists(caminho))
                    return caminho;
            }

            if (Directory.Exists(_pastaSons))
            {
                var arquivos = Directory.GetFiles(_pastaSons, "*");
                return arquivos.Length > 0 ? arquivos[0] : null;
            }

            return null;
        }

        public void CriarPastasPadrao()
        {
            if (!Directory.Exists(_pastaImagens))
                Directory.CreateDirectory(_pastaImagens);

            if (!Directory.Exists(_pastaSons))
                Directory.CreateDirectory(_pastaSons);
        }

        private void CriarArquivoPadrao()
        {
            var doc = new XDocument(
                new XElement("Configuracao",
                    new XElement("ReiniciarAoFechar", false),
                    new XElement("IntervaloExecucao", INTERVALO_PADRAO),
                    new XElement("TempoExibicao", TEMPO_EXIBICAO_PADRAO),
                    new XElement("ModoRede", false),
                    new XElement("Porta", PORTA_PADRAO),
                    new XElement("IpServidor", IP_SERVIDOR_PADRAO),
                    new XElement("ImagemSelecionada", string.Empty),
                    new XElement("SomSelecionado", string.Empty),
                    new XElement("ApenasSom", ApenasSom),
                    new XElement("ApenasImagem", ApenasImagem)
                )
            );

            doc.Save(_caminhoArquivo);
        }

        public static int ObterValorInteiro(XElement root, string elemento, int valorPadrao)
        {
            var elem = root.Element(elemento);
            return elem != null && int.TryParse(elem.Value, out int valor) ? valor : valorPadrao;
        }

        private static bool ObterValorBooleano(XElement root, string elemento, bool valorPadrao)
        {
            var elem = root.Element(elemento);
            return elem != null && bool.TryParse(elem.Value, out bool valor) ? valor : valorPadrao;
        }

        private static string ObterValorString(XElement root, string elemento, string valorPadrao)
        {
            return root.Element(elemento)?.Value ?? valorPadrao;
        }
    }
}