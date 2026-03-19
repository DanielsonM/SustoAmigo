using System.Drawing;
using System.Windows.Forms;

namespace SustoAmigo
{
    partial class FrmConfiguracao
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlPrincipal = new System.Windows.Forms.Panel();
            this.btnFechar = new System.Windows.Forms.Button();
            this.btnPlayer = new System.Windows.Forms.Button();
            this.btnIniciar = new System.Windows.Forms.Button();
            this.cmbSons = new System.Windows.Forms.ComboBox();
            this.btnCarregarSom = new System.Windows.Forms.Button();
            this.cmbImagens = new System.Windows.Forms.ComboBox();
            this.pnlModoRede = new System.Windows.Forms.Panel();
            this.txtPorta = new System.Windows.Forms.TextBox();
            this.lblIpVitima = new System.Windows.Forms.Label();
            this.lblPortaDeAcesso = new System.Windows.Forms.Label();
            this.txtIpVitima = new System.Windows.Forms.MaskedTextBox();
            this.btnCarregarImagem = new System.Windows.Forms.Button();
            this.pbxAmostra = new System.Windows.Forms.PictureBox();
            this.pnlModoAutomatico = new System.Windows.Forms.Panel();
            this.speTempoExibicao = new System.Windows.Forms.NumericUpDown();
            this.lblTempoExibicao = new System.Windows.Forms.Label();
            this.lblTempoIntervalo = new System.Windows.Forms.Label();
            this.speIntervaloTempo = new System.Windows.Forms.NumericUpDown();
            this.rdbModoAutomatico = new System.Windows.Forms.RadioButton();
            this.rdbModoRede = new System.Windows.Forms.RadioButton();
            this.pnlPrincipal.SuspendLayout();
            this.pnlModoRede.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxAmostra)).BeginInit();
            this.pnlModoAutomatico.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.speTempoExibicao)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.speIntervaloTempo)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlPrincipal
            // 
            this.pnlPrincipal.Controls.Add(this.btnFechar);
            this.pnlPrincipal.Controls.Add(this.btnPlayer);
            this.pnlPrincipal.Controls.Add(this.btnIniciar);
            this.pnlPrincipal.Controls.Add(this.cmbSons);
            this.pnlPrincipal.Controls.Add(this.btnCarregarSom);
            this.pnlPrincipal.Controls.Add(this.cmbImagens);
            this.pnlPrincipal.Controls.Add(this.pnlModoRede);
            this.pnlPrincipal.Controls.Add(this.btnCarregarImagem);
            this.pnlPrincipal.Controls.Add(this.pbxAmostra);
            this.pnlPrincipal.Controls.Add(this.pnlModoAutomatico);
            this.pnlPrincipal.Controls.Add(this.rdbModoAutomatico);
            this.pnlPrincipal.Controls.Add(this.rdbModoRede);
            this.pnlPrincipal.Location = new System.Drawing.Point(0, 0);
            this.pnlPrincipal.Name = "pnlPrincipal";
            this.pnlPrincipal.Size = new System.Drawing.Size(401, 300);
            this.pnlPrincipal.TabIndex = 0;
            // 
            // btnFechar
            // 
            this.btnFechar.FlatAppearance.BorderSize = 0;
            this.btnFechar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFechar.Image = global::SustoAmigo.Properties.Resources.fechar;
            this.btnFechar.Location = new System.Drawing.Point(377, 5);
            this.btnFechar.Name = "btnFechar";
            this.btnFechar.Size = new System.Drawing.Size(13, 17);
            this.btnFechar.TabIndex = 26;
            this.btnFechar.UseVisualStyleBackColor = true;
            this.btnFechar.Click += new System.EventHandler(this.btnFechar_Click);
            // 
            // btnPlayer
            // 
            this.btnPlayer.FlatAppearance.BorderSize = 0;
            this.btnPlayer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlayer.Image = global::SustoAmigo.Properties.Resources.player;
            this.btnPlayer.Location = new System.Drawing.Point(184, 238);
            this.btnPlayer.Name = "btnPlayer";
            this.btnPlayer.Size = new System.Drawing.Size(22, 18);
            this.btnPlayer.TabIndex = 27;
            this.btnPlayer.UseVisualStyleBackColor = true;
            this.btnPlayer.Click += new System.EventHandler(this.btnPlayer_Click);
            // 
            // btnIniciar
            // 
            this.btnIniciar.Location = new System.Drawing.Point(312, 272);
            this.btnIniciar.Name = "btnIniciar";
            this.btnIniciar.Size = new System.Drawing.Size(80, 20);
            this.btnIniciar.TabIndex = 25;
            this.btnIniciar.Text = "Iniciar";
            this.btnIniciar.UseVisualStyleBackColor = true;
            this.btnIniciar.Click += new System.EventHandler(this.btnIniciar_Click);
            // 
            // cmbSons
            // 
            this.cmbSons.FormattingEnabled = true;
            this.cmbSons.Location = new System.Drawing.Point(297, 238);
            this.cmbSons.Name = "cmbSons";
            this.cmbSons.Size = new System.Drawing.Size(95, 21);
            this.cmbSons.TabIndex = 23;
            this.cmbSons.SelectedIndexChanged += new System.EventHandler(this.CmbSons_SelectedIndexChanged);
            // 
            // btnCarregarSom
            // 
            this.btnCarregarSom.Location = new System.Drawing.Point(212, 238);
            this.btnCarregarSom.Name = "btnCarregarSom";
            this.btnCarregarSom.Size = new System.Drawing.Size(80, 20);
            this.btnCarregarSom.TabIndex = 22;
            this.btnCarregarSom.Text = "Carregar som";
            this.btnCarregarSom.UseVisualStyleBackColor = true;
            this.btnCarregarSom.Click += new System.EventHandler(this.btnCarregarSom_Click);
            // 
            // cmbImagens
            // 
            this.cmbImagens.FormattingEnabled = true;
            this.cmbImagens.Location = new System.Drawing.Point(297, 38);
            this.cmbImagens.Name = "cmbImagens";
            this.cmbImagens.Size = new System.Drawing.Size(95, 21);
            this.cmbImagens.TabIndex = 21;
            this.cmbImagens.SelectedIndexChanged += new System.EventHandler(this.cmbImagens_SelectedIndexChanged);
            // 
            // pnlModoRede
            // 
            this.pnlModoRede.Controls.Add(this.txtPorta);
            this.pnlModoRede.Controls.Add(this.lblIpVitima);
            this.pnlModoRede.Controls.Add(this.lblPortaDeAcesso);
            this.pnlModoRede.Controls.Add(this.txtIpVitima);
            this.pnlModoRede.Location = new System.Drawing.Point(2, 65);
            this.pnlModoRede.Name = "pnlModoRede";
            this.pnlModoRede.Size = new System.Drawing.Size(141, 117);
            this.pnlModoRede.TabIndex = 19;
            // 
            // txtPorta
            // 
            this.txtPorta.Location = new System.Drawing.Point(9, 32);
            this.txtPorta.Name = "txtPorta";
            this.txtPorta.Size = new System.Drawing.Size(103, 20);
            this.txtPorta.TabIndex = 17;
            // 
            // lblIpVitima
            // 
            this.lblIpVitima.AutoSize = true;
            this.lblIpVitima.Location = new System.Drawing.Point(9, 56);
            this.lblIpVitima.Name = "lblIpVitima";
            this.lblIpVitima.Size = new System.Drawing.Size(67, 13);
            this.lblIpVitima.TabIndex = 14;
            this.lblIpVitima.Text = "IP da vítima:";
            // 
            // lblPortaDeAcesso
            // 
            this.lblPortaDeAcesso.AutoSize = true;
            this.lblPortaDeAcesso.Location = new System.Drawing.Point(9, 18);
            this.lblPortaDeAcesso.Name = "lblPortaDeAcesso";
            this.lblPortaDeAcesso.Size = new System.Drawing.Size(87, 13);
            this.lblPortaDeAcesso.TabIndex = 15;
            this.lblPortaDeAcesso.Text = "Porta de acesso:";
            // 
            // txtIpVitima
            // 
            this.txtIpVitima.Font = new System.Drawing.Font("Arial", 9F);
            this.txtIpVitima.Location = new System.Drawing.Point(9, 72);
            this.txtIpVitima.Mask = "000.000.0.000";
            this.txtIpVitima.Name = "txtIpVitima";
            this.txtIpVitima.Size = new System.Drawing.Size(103, 21);
            this.txtIpVitima.TabIndex = 16;
            // 
            // btnCarregarImagem
            // 
            this.btnCarregarImagem.Location = new System.Drawing.Point(188, 38);
            this.btnCarregarImagem.Name = "btnCarregarImagem";
            this.btnCarregarImagem.Size = new System.Drawing.Size(104, 20);
            this.btnCarregarImagem.TabIndex = 0;
            this.btnCarregarImagem.Text = "Carregar imagem";
            this.btnCarregarImagem.UseVisualStyleBackColor = true;
            this.btnCarregarImagem.Click += new System.EventHandler(this.btnCarregarImagem_Click);
            // 
            // pbxAmostra
            // 
            this.pbxAmostra.Location = new System.Drawing.Point(188, 63);
            this.pbxAmostra.Name = "pbxAmostra";
            this.pbxAmostra.Size = new System.Drawing.Size(202, 170);
            this.pbxAmostra.TabIndex = 20;
            this.pbxAmostra.TabStop = false;
            // 
            // pnlModoAutomatico
            // 
            this.pnlModoAutomatico.Controls.Add(this.speTempoExibicao);
            this.pnlModoAutomatico.Controls.Add(this.lblTempoExibicao);
            this.pnlModoAutomatico.Controls.Add(this.lblTempoIntervalo);
            this.pnlModoAutomatico.Controls.Add(this.speIntervaloTempo);
            this.pnlModoAutomatico.Location = new System.Drawing.Point(1, 64);
            this.pnlModoAutomatico.Name = "pnlModoAutomatico";
            this.pnlModoAutomatico.Size = new System.Drawing.Size(141, 118);
            this.pnlModoAutomatico.TabIndex = 18;
            // 
            // speTempoExibicao
            // 
            this.speTempoExibicao.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.speTempoExibicao.Font = new System.Drawing.Font("Arial", 9F);
            this.speTempoExibicao.Location = new System.Drawing.Point(9, 34);
            this.speTempoExibicao.Name = "speTempoExibicao";
            this.speTempoExibicao.Size = new System.Drawing.Size(103, 21);
            this.speTempoExibicao.TabIndex = 7;
            this.speTempoExibicao.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblTempoExibicao
            // 
            this.lblTempoExibicao.AutoSize = true;
            this.lblTempoExibicao.Location = new System.Drawing.Point(9, 18);
            this.lblTempoExibicao.Name = "lblTempoExibicao";
            this.lblTempoExibicao.Size = new System.Drawing.Size(100, 13);
            this.lblTempoExibicao.TabIndex = 2;
            this.lblTempoExibicao.Text = "Tempo de exibição:";
            // 
            // lblTempoIntervalo
            // 
            this.lblTempoIntervalo.AutoSize = true;
            this.lblTempoIntervalo.Location = new System.Drawing.Point(9, 56);
            this.lblTempoIntervalo.Name = "lblTempoIntervalo";
            this.lblTempoIntervalo.Size = new System.Drawing.Size(101, 13);
            this.lblTempoIntervalo.TabIndex = 3;
            this.lblTempoIntervalo.Text = "Tempo de intervalo:";
            // 
            // speIntervaloTempo
            // 
            this.speIntervaloTempo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.speIntervaloTempo.Font = new System.Drawing.Font("Arial", 9F);
            this.speIntervaloTempo.Location = new System.Drawing.Point(9, 72);
            this.speIntervaloTempo.Name = "speIntervaloTempo";
            this.speIntervaloTempo.Size = new System.Drawing.Size(103, 21);
            this.speIntervaloTempo.TabIndex = 8;
            this.speIntervaloTempo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // rdbModoAutomatico
            // 
            this.rdbModoAutomatico.AutoSize = true;
            this.rdbModoAutomatico.Location = new System.Drawing.Point(13, 12);
            this.rdbModoAutomatico.Name = "rdbModoAutomatico";
            this.rdbModoAutomatico.Size = new System.Drawing.Size(107, 17);
            this.rdbModoAutomatico.TabIndex = 13;
            this.rdbModoAutomatico.TabStop = true;
            this.rdbModoAutomatico.Text = "Modo automático";
            this.rdbModoAutomatico.UseVisualStyleBackColor = true;
            // 
            // rdbModoRede
            // 
            this.rdbModoRede.AutoSize = true;
            this.rdbModoRede.Location = new System.Drawing.Point(126, 13);
            this.rdbModoRede.Name = "rdbModoRede";
            this.rdbModoRede.Size = new System.Drawing.Size(76, 17);
            this.rdbModoRede.TabIndex = 12;
            this.rdbModoRede.TabStop = true;
            this.rdbModoRede.Text = "Modo rede";
            this.rdbModoRede.UseVisualStyleBackColor = true;
            this.rdbModoRede.CheckedChanged += new System.EventHandler(this.rdbModoRede_CheckedChanged);
            // 
            // FrmConfiguracao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(401, 300);
            this.Controls.Add(this.pnlPrincipal);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmConfiguracao";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FrmConfiguracao_Load);
            this.pnlPrincipal.ResumeLayout(false);
            this.pnlPrincipal.PerformLayout();
            this.pnlModoRede.ResumeLayout(false);
            this.pnlModoRede.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxAmostra)).EndInit();
            this.pnlModoAutomatico.ResumeLayout(false);
            this.pnlModoAutomatico.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.speTempoExibicao)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.speIntervaloTempo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel pnlPrincipal;
        private Panel pnlModoRede;
        private TextBox txtPorta;
        private Label lblIpVitima;
        private Label lblPortaDeAcesso;
        private MaskedTextBox txtIpVitima;
        private Panel pnlModoAutomatico;
        private NumericUpDown speTempoExibicao;
        private Label lblTempoExibicao;
        private Label lblTempoIntervalo;
        private NumericUpDown speIntervaloTempo;
        private RadioButton rdbModoAutomatico;
        private RadioButton rdbModoRede;
        private PictureBox pbxAmostra;
        private ComboBox cmbImagens;
        private Button btnCarregarImagem;
        private ComboBox cmbSons;
        private Button btnCarregarSom;
        private Button btnIniciar;
        private Button btnFechar;
        private Button btnPlayer;
    }
}
