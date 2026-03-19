using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SustoAmigo.Configuracoes;
using SustoAmigo.Interfaces;
using SustoAmigo.Services;

namespace SustoAmigo
{
    public partial class Principal : Form, IDisposable
    {
        private readonly IMediaService _mediaService;
        private readonly IConfigService _configuracao;
        private RedeController _redeController;
        private Timer _tmrRepeatTimer;
        private Timer _tmrResetTimer;
        private bool _disposed;

        public Principal() : this(new MediaService(), ConfiguracaoXml.Instancia) { }

        public Principal(IMediaService mediaService, IConfigService configuracao)
        {
            _mediaService = mediaService;
            _configuracao = configuracao;

            InitializeComponent();
            Iniciar();
        }

        private void Iniciar()
        {
            _redeController = new RedeController();

            if (_configuracao.ModoRede)
            {
                ConfigurarModoRede();
            }
            else
            {
                ConfigurarModoAutomatico();
            }
        }

        private void ConfigurarModoRede()
        {
            _redeController.OnReceberComando += () =>
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    Show();
                    Carregar();
                }));
            };

            _redeController.IniciarServidor(_configuracao.Porta);
        }

        private void ConfigurarModoAutomatico()
        {
            _tmrRepeatTimer = new Timer
            {
                Interval = _configuracao.IntervaloExecucao * 1000
            };
            _tmrRepeatTimer.Tick += RepeatTimer_Tick;
            _tmrRepeatTimer.Start();
        }

        private void RepeatTimer_Tick(object sender, EventArgs e)
        {
            Show();
            Carregar();
        }

        private async void Carregar()
        {
            _tmrResetTimer?.Stop();
            _tmrResetTimer?.Dispose();

            _mediaService.PararSom();

            var pastaUploads = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads");
            var pastaImagens = Path.Combine(Application.StartupPath, "Imagens");
            var pastaSons = Path.Combine(Application.StartupPath, "Sons");

            var caminhoImagem = await _mediaService.CarregarImagemAsync(
                pastaUploads, pastaImagens, _configuracao.ImagemSelecionada);

            var caminhoSom = await _mediaService.CarregarSomAsync(
                pastaUploads, pastaSons, _configuracao.SomSelecionado);

            ExibirImagem(caminhoImagem);
            _mediaService.ReproduzirSom(caminhoSom);

            _tmrResetTimer = new Timer();
            _tmrResetTimer.Interval = _configuracao.TempoExibicao * 1000;
            _tmrResetTimer.Tick += (s, ev) =>
            {
                _tmrResetTimer.Stop();
                _mediaService.PararSom();
                Hide();
            };
            _tmrResetTimer.Start();
        }

        private void ExibirImagem(string caminhoImagem)
        {
            if (string.IsNullOrEmpty(caminhoImagem) || !File.Exists(caminhoImagem))
            {
                picFoto.SizeMode = PictureBoxSizeMode.Zoom;
                picFoto.Image = Properties.Resources.imgSusto;
                picFoto.Visible = true;
                return;
            }

            using (var imagem = Image.FromFile(caminhoImagem))
            {
                picFoto.SizeMode = PictureBoxSizeMode.Zoom;
                picFoto.Image = new Bitmap(imagem);
                picFoto.Visible = true;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Hide();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _tmrRepeatTimer?.Stop();
            _tmrResetTimer?.Stop();
            _redeController?.PararServidor();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    components?.Dispose();
                    _tmrRepeatTimer?.Dispose();
                    _tmrResetTimer?.Dispose();
                    _redeController?.Dispose();
                    (_mediaService as IDisposable)?.Dispose();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}