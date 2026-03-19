using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WMPLib; // importante: adicionar referência ao Windows Media Player

namespace SustoAmigo
{
    public partial class Principal : Form
    {
        #region Atributos

        private Timer _tmrRepeatTimer;
        private WindowsMediaPlayer _wmpPlayer;
        private RedeController _redeController;

        #endregion Atributos

        #region Construtores

        public Principal()
        {
            InitializeComponent();

            this.Iniciar();
        }

        #endregion Construtores

        #region Métodos

        private void Iniciar()
        {
            this._redeController = new RedeController();

            if (ConfiguracaoXML.i.booModoRede)
            {
                // Servidor
                this._redeController.OnReceberComando += () =>
                {
                    this.Invoke((MethodInvoker)(() =>
                    {
                        this.Show();
                        Carregar();
                    }));
                };

                this._redeController.IniciarServidor(ConfiguracaoXML.i.intPorta);
            }
            else
            {
                // Modo intervalo
                this._tmrRepeatTimer = new Timer();
                this._tmrRepeatTimer.Interval = ConfiguracaoXML.i.intIntervaloExecucao * 1000;
                this._tmrRepeatTimer.Tick += this.RepeatTimer_Tick;
                this._tmrRepeatTimer.Start();
            }
        }

        private void RepeatTimer_Tick(object sender, EventArgs e)
        {
            this.Show();
            this.Carregar();
        }

        private void Carregar()
        {
            this.CarregarSom();
            this.CarregarImagem();

            var tmrResetTimer = new Timer();
            tmrResetTimer.Interval = ConfiguracaoXML.i.intTempoExibicao * 1000;

            tmrResetTimer.Tick += (s, ev) =>
            {
                tmrResetTimer.Stop();
                this._wmpPlayer?.controls.stop();
                this.Hide();
            };

            tmrResetTimer.Start();
        }

        private void CarregarImagem()
        {
            string strPastaImagens = Path.Combine(Application.StartupPath, "Imagens");

            if (Directory.Exists(strPastaImagens) && Directory.GetFiles(strPastaImagens).Length > 0)
            {
                string strArquivoImagem = null;

                if (!string.IsNullOrEmpty(ConfiguracaoXML.i.strImagemSelecionada))
                {
                    strArquivoImagem = Path.Combine(strPastaImagens, ConfiguracaoXML.i.strImagemSelecionada);
                    if (!File.Exists(strArquivoImagem))
                    {
                        strArquivoImagem = Directory.GetFiles(strPastaImagens).First();
                    }
                }
                else
                {
                    strArquivoImagem = Directory.GetFiles(strPastaImagens).First();
                }

                this.picFoto.SizeMode = PictureBoxSizeMode.Zoom;
                this.picFoto.Image = Image.FromFile(strArquivoImagem);
                this.picFoto.Visible = true;
            }
            else
            {
                Properties.Resources.imgSusto.MakeTransparent(Color.White);
                this.picFoto.SizeMode = PictureBoxSizeMode.Zoom;
                this.picFoto.Image = Properties.Resources.imgSusto;
                this.picFoto.Visible = true;
            }
        }

        private void CarregarSom()
        {
            string strPastaSons = Path.Combine(Application.StartupPath, "Sons");
            string strArquivoSom = null;

            if (!string.IsNullOrEmpty(ConfiguracaoXML.i.strSomSelecionado))
            {
                strArquivoSom = Path.Combine(strPastaSons, ConfiguracaoXML.i.strSomSelecionado);
                if (!File.Exists(strArquivoSom))
                {
                    strArquivoSom = Directory.GetFiles(strPastaSons, "*").FirstOrDefault();
                }
            }
            else
            {
                strArquivoSom = Directory.GetFiles(strPastaSons, "*").FirstOrDefault();
            }

            this._wmpPlayer = new WindowsMediaPlayer();

            if (!string.IsNullOrEmpty(strArquivoSom))
            {
                this._wmpPlayer.URL = strArquivoSom;
                this._wmpPlayer.controls.play();
            }
            else
            {
                // fallback para recurso padrão (som embutido)
                string strTempFile = Path.Combine(Path.GetTempPath(), "grito.wav");

                using (var resourceStream = Properties.Resources.Grito)
                using (var fileStream = new FileStream(strTempFile, FileMode.Create, FileAccess.Write))
                {
                    resourceStream.CopyTo(fileStream); // copia o conteúdo do recurso para o arquivo
                }

                this._wmpPlayer.URL = strTempFile;
                this._wmpPlayer.controls.play();
            }
        }

        #endregion Métodos

        #region Eventos

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Hide();
        }

        #endregion Eventos
    }
}