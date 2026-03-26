using SustoAmigo.Interfaces;
using SustoAmigo.Services;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SustoAmigo.Configuracoes
{
    public partial class FrmConfiguracao : Form
    {
        private readonly IConfigService _configService;
        private readonly IMediaService _mediaService;
        private readonly string _pastaImagens;
        private readonly string _pastaSons;
        private bool _emReproducao;

        public bool IniciadoPorConfiguracao { get; private set; }

        public FrmConfiguracao() : this(ConfiguracaoXml.i, new MediaService()) { }

        public FrmConfiguracao(IConfigService configService, IMediaService mediaService)
        {
            _configService = configService;
            _mediaService = mediaService;

            InitializeComponent();
            InicializarComponente();

            _pastaImagens = Path.Combine(Application.StartupPath, "Imagens");
            _pastaSons = Path.Combine(Application.StartupPath, "Sons");

            CriarPastasPadrao();
            CarregarConfiguracoes();
        }

        public void CriarPastasPadrao()
        {
            if (!Directory.Exists(_pastaImagens))
                Directory.CreateDirectory(_pastaImagens);

            if (!Directory.Exists(_pastaSons))
                Directory.CreateDirectory(_pastaSons);
        }

        private void InicializarComponente()
        {
            rdbModoAutomatico.Checked = !_configService.ModoRede;
            rdbModoRede.Checked = _configService.ModoRede;
        }

        private void CarregarConfiguracoes()
        {
            CarregarImagens();
            CarregarSons();
            CarregarDadosModo();
            SelecionarItensSalvos();
        }

        private void CarregarImagens()
        {
            cmbImagens.Items.Clear();

            if (Directory.Exists(_pastaImagens))
            {
                foreach (var arquivo in Directory.GetFiles(_pastaImagens))
                    cmbImagens.Items.Add(Path.GetFileName(arquivo));
            }
        }

        private void CarregarSons()
        {
            cmbSons.Items.Clear();

            if (Directory.Exists(_pastaSons))
            {
                var extensoes = new[] { "*.wav", "*.mp3", "*.wma", "*.aac", "*.flac" };
                foreach (var ext in extensoes)
                {
                    foreach (var arquivo in Directory.GetFiles(_pastaSons, ext))
                        cmbSons.Items.Add(Path.GetFileName(arquivo));
                }
            }
        }

        private void CarregarDadosModo()
        {
            txtIpVitima.Text = _configService.IpServidor;
            txtPorta.Text = _configService.Porta.ToString();
            speIntervaloTempo.Value = _configService.IntervaloExecucao;
            speTempoExibicao.Value = _configService.TempoExibicao;
        }

        private void SelecionarItensSalvos()
        {
            if (!string.IsNullOrEmpty(_configService.ImagemSelecionada) &&
                cmbImagens.Items.Contains(_configService.ImagemSelecionada))
            {
                cmbImagens.SelectedItem = _configService.ImagemSelecionada;
            }

            if (!string.IsNullOrEmpty(_configService.SomSelecionado) &&
                cmbSons.Items.Contains(_configService.SomSelecionado))
            {
                cmbSons.SelectedItem = _configService.SomSelecionado;
            }
        }

        private void ValidarModo()
        {
            var modoRede = rdbModoRede.Checked;
            pnlModoRede.Visible = modoRede;
            pnlModoAutomatico.Visible = !modoRede;
        }

        private void ExibirImagemSelecionada()
        {
            if (cmbImagens.SelectedItem == null)
                return;

            var caminho = Path.Combine(_pastaImagens, cmbImagens.SelectedItem.ToString());

            if (File.Exists(caminho))
            {
                using (var imagem = Image.FromFile(caminho))
                {
                    pbxAmostra.SizeMode = PictureBoxSizeMode.Zoom;
                    pbxAmostra.Image = new Bitmap(imagem);
                }
            }
        }

        private void ReproduzirSomSelecionado()
        {
            if (!_emReproducao)
            {
                var caminhoSom = ObterCaminhoSomSelecionado();

                if (!string.IsNullOrEmpty(caminhoSom))
                {
                    _mediaService.ReproduzirSom(caminhoSom);
                    btnPlayer.Image = Properties.Resources.pausa;
                    _emReproducao = true;
                }
                else
                {
                    MessageBox.Show("Nenhum arquivo de �udio encontrado.");
                }
            }
            else
            {
                _mediaService.PararSom();
                btnPlayer.Image = Properties.Resources.player;
                _emReproducao = false;
            }
        }

        private string ObterCaminhoSomSelecionado()
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

        private void SalvarConfiguracao()
        {
            var intervalo = Convert.ToInt32(speIntervaloTempo.Value);
            var tempoExibicao = Convert.ToInt32(speTempoExibicao.Value);
            var modoRede = rdbModoRede.Checked;

            int porta = 0;
            var ipServidor = string.Empty;

            if (modoRede)
            {
                if (string.IsNullOrEmpty(txtPorta.Text))
                    throw new Exception("Campo PORTA n�o pode estar vazio.");

                if (string.IsNullOrEmpty(txtIpVitima.Text))
                    throw new Exception("Campo IP n�o pode estar vazio.");

                porta = int.Parse(txtPorta.Text);
                ipServidor = txtIpVitima.Text.Replace(",", ".");
            }

            var imagem = cmbImagens.SelectedItem?.ToString();
            var som = cmbSons.SelectedItem?.ToString();

          //  _configService.Salvar(false, intervalo, tempoExibicao, modoRede, porta, ipServidor, imagem, som, apenasSom, ApenasImagem);

            MessageBox.Show("Configura��es salvas com sucesso!");
            IniciadoPorConfiguracao = true;
            Close();
        }

        private void btnCarregarImagem_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Arquivos de Imagem|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var destino = Path.Combine(_pastaImagens, Path.GetFileName(ofd.FileName));
                    File.Copy(ofd.FileName, destino, true);
                    CarregarImagens();
                }
            }
        }

        private void cmbImagens_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ExibirImagemSelecionada();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar imagem: {ex.Message}");
            }
        }

        private void btnCarregarSom_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Arquivos de �udio|*.wav;*.mp3;*.wma;*.aac;*.flac";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var destino = Path.Combine(_pastaSons, Path.GetFileName(ofd.FileName));
                    File.Copy(ofd.FileName, destino, true);
                    CarregarSons();
                }
            }
        }

        private void btnPlayer_Click(object sender, EventArgs e)
        {
            ReproduzirSomSelecionado();
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CmbSons_SelectedIndexChanged(object sender, EventArgs e) { }

        private void rdbModoRede_CheckedChanged(object sender, EventArgs e)
        {
            ValidarModo();
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            try
            {
                SalvarConfiguracao();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar configura��es: {ex.Message}");
            }
        }

        private void FrmConfiguracao_Load(object sender, EventArgs e)
        {
            ValidarModo();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _mediaService.PararSom();
            base.OnFormClosing(e);
        }

        private void txtIpVitima_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void txtIpVitima_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ',')
            {
                e.KeyChar = '.';
            }
        }
    }
}