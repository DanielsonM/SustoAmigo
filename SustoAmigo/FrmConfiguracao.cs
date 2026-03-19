using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WMPLib;

namespace SustoAmigo
{
    public partial class FrmConfiguracao : Form
    {
        #region Atributos

        private string _strPastaImagens;
        private string _strPastaSons;
        private bool _booTocando = false; // controla o estado atual
        private WindowsMediaPlayer _wmpPlayer;

        public bool booIniciadoPorConfiguracao = false;

        #endregion Atributos

        #region Construtores

        public FrmConfiguracao()
        {
            InitializeComponent();

            this.InicializarComponente();

            this._strPastaImagens = Path.Combine(Application.StartupPath, "Imagens");
            this._strPastaSons = Path.Combine(Application.StartupPath, "Sons");

            if (!Directory.Exists(this._strPastaImagens))
                Directory.CreateDirectory(this._strPastaImagens);

            if (!Directory.Exists(this._strPastaSons))
                Directory.CreateDirectory(this._strPastaSons);

            this.SetCampos();
        }

        #endregion Construtores

        #region Métodos

        private void SetCampos()
        {
            this.SetComboImagens();
            this.SetComboSons();
            this.SetDadosModo();

            if (!string.IsNullOrEmpty(ConfiguracaoXML.i.strImagemSelecionada) && this.cmbImagens.Items.Contains(ConfiguracaoXML.i.strImagemSelecionada))
                this.cmbImagens.SelectedItem = ConfiguracaoXML.i.strImagemSelecionada;

            if (!string.IsNullOrEmpty(ConfiguracaoXML.i.strSomSelecionado) && this.cmbSons.Items.Contains(ConfiguracaoXML.i.strSomSelecionado))
                this.cmbSons.SelectedItem = ConfiguracaoXML.i.strSomSelecionado;
        }

        private void SetDadosModo()
        {
            this.txtIpVitima.Text = ConfiguracaoXML.i.strIpServidor;
            this.txtPorta.Text = ConfiguracaoXML.i.intPorta.ToString();
            this.speIntervaloTempo.Value = ConfiguracaoXML.i.intIntervaloExecucao;
            this.speTempoExibicao.Value = ConfiguracaoXML.i.intTempoExibicao;
        }

        private void InicializarComponente()
        {
            if (!ConfiguracaoXML.i.booModoRede)
                this.rdbModoAutomatico.Checked = true;
            else if (ConfiguracaoXML.i.booModoRede)
                this.rdbModoRede.Checked = true;
            else
                this.rdbModoAutomatico.Checked = true;
        }

        private void ValidarModo()
        {
            if (this.rdbModoRede.Checked)
            {
                this.pnlModoRede.Visible = true;
                this.rdbModoRede.Checked = true;
                this.pnlModoAutomatico.Visible = false;
            }
            else
            {
                this.pnlModoRede.Visible = false;
                this.pnlModoAutomatico.Visible = true;
                this.rdbModoRede.Checked = false;
                this.rdbModoAutomatico.Checked = true;
            }
        }

        private void SetComboImagens()
        {
            this.cmbImagens.Items.Clear();

            foreach (string arquivo in Directory.GetFiles(_strPastaImagens))
            {
                this.cmbImagens.Items.Add(Path.GetFileName(arquivo));
            }
        }

        private void ExibirImagemSelecionada()
        {
            if (this.cmbImagens.SelectedItem != null)
            {
                string caminho = Path.Combine(this._strPastaImagens, this.cmbImagens.SelectedItem.ToString());

                this.pbxAmostra.SizeMode = PictureBoxSizeMode.Zoom;
                this.pbxAmostra.Image = Image.FromFile(caminho);
            }
        }

        private void SetComboSons()
        {
            this.cmbSons.Items.Clear();

            // Lista os formatos suportados
            string[] extensoes = new[] { "*.wav", "*.mp3", "*.wma", "*.aac", "*.flac" };

            foreach (string ext in extensoes)
            {
                foreach (string arquivo in Directory.GetFiles(_strPastaSons, ext))
                    this.cmbSons.Items.Add(Path.GetFileName(arquivo));
            }
        }

        private void ReproduzirrSomSelecionado()
        {
            if (!_booTocando)
            {
                string pastaSons = Path.Combine(Application.StartupPath, "Sons");
                string strArquivoSom = null;

                if (!string.IsNullOrEmpty(ConfiguracaoXML.i.strSomSelecionado))
                {
                    strArquivoSom = Path.Combine(pastaSons, ConfiguracaoXML.i.strSomSelecionado);
                    if (!File.Exists(strArquivoSom))
                    {
                        strArquivoSom = Directory.GetFiles(pastaSons, "*").FirstOrDefault();
                    }
                }
                else
                {
                    strArquivoSom = Directory.GetFiles(pastaSons, "*").FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(strArquivoSom))
                {
                    this._wmpPlayer = new WindowsMediaPlayer();
                    this._wmpPlayer.URL = strArquivoSom;
                    this._wmpPlayer.controls.play();
                }
                else
                {
                    MessageBox.Show("Nenhum arquivo de áudio encontrado.");
                }

                this.btnPlayer.Image = Properties.Resources.pausa;
                this._booTocando = true;
            }
            else
            {
                this._wmpPlayer?.controls.stop();

                this.btnPlayer.Image = Properties.Resources.player;
                this._booTocando = false;
            }
        }

        private void SalvarConfiguracao()
        {
            int intIntervalo = int.Parse(this.speIntervaloTempo.Value.ToString());
            int intTempoExibicao = int.Parse(this.speTempoExibicao.Value.ToString());
            bool booModoRede = this.rdbModoRede.Checked;

            int intPorta = 0;
            string strIpServidor = string.Empty;

            if (this.rdbModoRede.Checked)
            {
                if (this.txtPorta.Text == string.Empty)
                    throw new Exception("Campo PORTA não pode estar vazio.");

                if (txtIpVitima.Text == string.Empty)
                    throw new Exception("Campo IP não pode estar vazio.");

                intPorta = int.Parse(this.txtPorta.Text);
                strIpServidor = this.txtIpVitima.Text;
            }

            string strImagemSelecionada = this.cmbImagens.SelectedItem?.ToString();
            string strSomSelecionado = this.cmbSons.SelectedItem?.ToString();

            ConfiguracaoXML.i.Salvar(intIntervalo, intTempoExibicao, booModoRede, intPorta, strIpServidor, strImagemSelecionada, strSomSelecionado);

            MessageBox.Show("Configurações salvas com sucesso!");

            this.booIniciadoPorConfiguracao = true;
            this.Close();
        }

        #endregion Métodos

        #region Eventos

        private void btnCarregarImagem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Arquivos de Imagem|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string destino = Path.Combine(_strPastaImagens, Path.GetFileName(ofd.FileName));
                    File.Copy(ofd.FileName, destino, true);

                    this.SetComboImagens();
                }
            }
        }

        private void cmbImagens_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.ExibirImagemSelecionada();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar imagem: " + ex.Message);
            }
        }

        private void btnCarregarSom_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                // Aceita os formatos mais comuns
                ofd.Filter = "Arquivos de Áudio|*.wav;*.mp3;*.wma;*.aac;*.flac";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string destino = Path.Combine(_strPastaSons, Path.GetFileName(ofd.FileName));
                    File.Copy(ofd.FileName, destino, true);

                    this.SetComboSons();
                }
            }
        }

        private void btnPlayer_Click(object sender, EventArgs e)
        {
            ReproduzirrSomSelecionado();
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CmbSons_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void rdbModoRede_CheckedChanged(object sender, EventArgs e)
        {
            this.ValidarModo();
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            try
            {
                this.SalvarConfiguracao();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar configurações: " + ex.Message);
            }
        }

        private void FrmConfiguracao_Load(object sender, EventArgs e)
        {
            this.ValidarModo();
        }

        #endregion Eventos
    }
}