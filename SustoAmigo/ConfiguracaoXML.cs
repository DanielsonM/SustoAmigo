using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SustoAmigo
{
    public class ConfiguracaoXML
    {
        #region Atributos

        private static readonly Lazy<ConfiguracaoXML> _i = new Lazy<ConfiguracaoXML>(() => new ConfiguracaoXML());
        public static ConfiguracaoXML i => _i.Value;

        private static string _strCaminho = Path.Combine(Application.StartupPath, "ConfigurcaoXML.xml");

        public string strImagemSelecionada { get; private set; }
        public string strSomSelecionado { get; private set; }

        public int intIntervaloExecucao { get; private set; }
        public int intTempoExibicao { get; private set; }
        public bool booModoRede { get; private set; }
        public int intPorta { get; private set; }
        public string strIpServidor { get; private set; }

        #endregion Atributos

        #region Construtores

        private ConfiguracaoXML() => this.Carregar();

        #endregion Construtores

        #region Métodos

        public void Salvar(int intIntervalo, int intTempoExibicao, bool booModoRede, int intPorta, string strIpServidor, string strImagemSelecionada, string strSomSelecionado)
        {
            XDocument doc = new XDocument(
                new XElement("ConfigurcaoXML",
                    new XElement("IntervaloExecucao", intIntervalo),
                    new XElement("TempoExibicao", intTempoExibicao),
                    new XElement("ModoRede", booModoRede),
                    new XElement("Porta", intPorta),
                    new XElement("IpServidor", strIpServidor),
                    new XElement("ImagemSelecionada", strImagemSelecionada ?? string.Empty),
                    new XElement("SomSelecionado", strSomSelecionado ?? string.Empty)
                )
            );

            doc.Save(_strCaminho);

            this.intIntervaloExecucao = intIntervalo;
            this.intTempoExibicao = intTempoExibicao;
            this.booModoRede = booModoRede;
            this.intPorta = intPorta;
            this.strIpServidor = strIpServidor;
            this.strImagemSelecionada = strImagemSelecionada;
            this.strSomSelecionado = strSomSelecionado;
        }

        private void Carregar()
        {
            if (!File.Exists(_strCaminho))
                CriarPadrao();

            XDocument docCaminho = XDocument.Load(_strCaminho);

            this.intIntervaloExecucao = int.Parse(docCaminho.Root.Element("IntervaloExecucao").Value);
            this.intTempoExibicao = int.Parse(docCaminho.Root.Element("TempoExibicao").Value);
            this.booModoRede = bool.Parse(docCaminho.Root.Element("ModoRede").Value);
            this.intPorta = int.Parse(docCaminho.Root.Element("Porta").Value);
            this.strIpServidor = docCaminho.Root.Element("IpServidor").Value;

            var xetImagemElement = docCaminho.Root.Element("ImagemSelecionada");
            var xetSomElement = docCaminho.Root.Element("SomSelecionado");

            this.strImagemSelecionada = xetImagemElement != null ? xetImagemElement.Value : string.Empty;
            this.strSomSelecionado = xetSomElement != null ? xetSomElement.Value : string.Empty;
        }

        private static void CriarPadrao()
        {
            XDocument docXElement = new XDocument(
                new XElement("ConfigurcaoXML",
                    new XElement("IntervaloExecucao", 10),
                    new XElement("TempoExibicao", 5),
                    new XElement("ModoRede", false),
                    new XElement("Porta", 5000),
                    new XElement("IpServidor", "192.168.0.50"),
                    new XElement("ImagemSelecionada", string.Empty),
                    new XElement("SomSelecionado", string.Empty)
                )
            );

            docXElement.Save(_strCaminho);
        }

        #endregion Métodos
    }
}