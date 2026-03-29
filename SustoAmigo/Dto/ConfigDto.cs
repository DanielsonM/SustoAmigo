namespace SustoAmigo.Dto
{
    public class ConfigDto
    {
        public int IntervaloExecucao { get; set; }
        public int TempoExibicao { get; set; }
        public bool ModoRede { get; set; }
        public int Porta { get; set; }
        public string IpServidor { get; set; }
        public string ImagemSelecionada { get; set; }
        public string SomSelecionado { get; set; }
        public bool booReiniciarAoFechar { get; set; }
        public bool ApenasSom { get; set; }
        public bool ApenasImagem { get; set; }
        public bool UsarMilesegundos { get; set; }
    }
}