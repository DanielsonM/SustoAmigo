using SustoAmigo.Interfaces;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SustoAmigo
{
    public partial class Principal : Form
    {
        private const int MILISSEGUNDO_POR_SEGUNDO = 1000;

        private readonly IMediaService _mediaService;
        private readonly IConfigService _configuracao;
        private readonly RedeController _redeController;
        private Timer _timerRepeticao;
        private Timer _timerReset;
        private string _arquivoSomTemporario;
        private bool _disposed;
        public bool ReiniciarAoFechar { get; set; }

        public Principal(IMediaService mediaService, IConfigService configuracao, RedeController redeController = null)
        {
            _mediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));
            _configuracao = configuracao ?? throw new ArgumentNullException(nameof(configuracao));
            _redeController = redeController;

            InitializeComponent();
            Iniciar();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Hide();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timerRepeticao?.Stop();
            LimparTimerReset();
            LimparArquivoSomTemporario();
            _redeController?.PararServidor();
            base.OnFormClosing(e);

            if (ReiniciarAoFechar)
                System.Diagnostics.Process.Start(Application.ExecutablePath);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    components?.Dispose();
                    _timerRepeticao?.Dispose();
                    _timerReset?.Dispose();
                    _redeController?.Dispose();
                    (_mediaService as IDisposable)?.Dispose();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }

        private void Iniciar()
        {
            if (_configuracao.ModoRede)
                ConfigurarModoRede();
            else
                ConfigurarModoAutomatico();
        }

        private void ConfigurarModoRede()
        {
            if (_redeController == null)
                throw new InvalidOperationException("RedeController � necess�rio para o modo rede.");

            _redeController.OnReceberComando += AoReceberComando;
            _redeController.IniciarServidor(_configuracao.Porta);
        }

        private void AoReceberComando()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)ExecutarSusto);
            }
            else
            {
                ExecutarSusto();
            }
        }

        private void ConfigurarModoAutomatico()
        {
            _timerRepeticao = new Timer
            {
                Interval = _configuracao.IntervaloExecucao * MILISSEGUNDO_POR_SEGUNDO
            };
            _timerRepeticao.Tick += TimerRepeticao_Tick;
            _timerRepeticao.Start();
        }

        private void TimerRepeticao_Tick(object sender, EventArgs e)
        {
            ExecutarSusto();
        }

        private void IniciarTimerReset()
        {
            _timerReset = new Timer();
            _timerReset.Interval = _configuracao.TempoExibicao * MILISSEGUNDO_POR_SEGUNDO;
            _timerReset.Tick += TimerReset_Tick;
            _timerReset.Start();
        }

        private void LimparTimerReset()
        {
            if (_timerReset != null)
            {
                _timerReset.Stop();
                _timerReset.Dispose();
                _timerReset = null;
            }
        }

        private async void ExecutarSusto()
        {
            try
            {
                LimparTimerReset();
                _mediaService.PararSom();
                LimparArquivoSomTemporario();

                var pastas = ObterPastas();
                var caminhoImagem = await _mediaService.CarregarImagemAsync(
                    pastas.Uploads, pastas.Imagens, _configuracao.ImagemSelecionada);

                var caminhoSom = await _mediaService.CarregarSomAsync(
                    pastas.Uploads, pastas.Sons, _configuracao.SomSelecionado);

                ReproduzirSomComArquivoTemporario(caminhoSom, pastas.Sons);

                ExibirImagem(caminhoImagem);

                IniciarTimerReset();

                this.ShowDialog();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao executar susto: {ex.Message}");
            }
        }

        private (string Uploads, string Imagens, string Sons) ObterPastas()
        {
            return (
                Uploads: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads"),
                Imagens: Path.Combine(Application.StartupPath, "Imagens"),
                Sons: Path.Combine(Application.StartupPath, "Sons")
            );
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

        private void ReproduzirSomComArquivoTemporario(string caminhoSom, string pastaSons)
        {
            string arquivoSomFinal = ObterArquivoSomValido(caminhoSom, pastaSons);
            _arquivoSomTemporario = CriarArquivoTemporario(arquivoSomFinal);
            _mediaService.ReproduzirSom(_arquivoSomTemporario);
        }

        private string ObterArquivoSomValido(string caminhoSom, string pastaSons)
        {
            if (!string.IsNullOrEmpty(caminhoSom) && File.Exists(caminhoSom))
            {
                return caminhoSom;
            }

            return CriarOuObterSomPadrao(pastaSons);
        }

        private string CriarOuObterSomPadrao(string pastaSons)
        {
            var somPadrao = Path.Combine(pastaSons, "Grito.wav");

            if (!File.Exists(somPadrao))
            {
                if (!Directory.Exists(pastaSons))
                    Directory.CreateDirectory(pastaSons);

                using (var resourceStream = Properties.Resources.Grito)
                using (var fs = new FileStream(somPadrao, FileMode.Create, FileAccess.Write))
                {
                    resourceStream.CopyTo(fs);
                }
            }

            return somPadrao;
        }

        private string CriarArquivoTemporario(string arquivoOrigem)
        {
            var tempSom = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".wav");
            File.Copy(arquivoOrigem, tempSom, true);
            return tempSom;
        }

        private void LimparArquivoSomTemporario()
        {
            if (!string.IsNullOrEmpty(_arquivoSomTemporario) && File.Exists(_arquivoSomTemporario))
            {
                try
                {
                    File.Delete(_arquivoSomTemporario);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao deletar arquivo tempor�rio: {ex.Message}");
                }
            }
        }

        private void TimerReset_Tick(object sender, EventArgs e)
        {
            LimparTimerReset();
            _mediaService.PararSom();
            Hide();
        }
    }
}