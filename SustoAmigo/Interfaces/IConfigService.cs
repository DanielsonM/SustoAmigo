namespace SustoAmigo.Interfaces
{
    public interface IConfigService
    {
        int IntervaloExecucao { get; }
        int TempoExibicao { get; }
        bool ModoRede { get; }
        int Porta { get; }
        string IpServidor { get; }
        string ImagemSelecionada { get; }
        string SomSelecionado { get; }
        bool ReiniciarAoFechar { get; }
        bool ApenasSom { get; }
        bool ApenasImagem { get; }
        bool UsarMilesegundos { get; }

        void Salvar(bool booReiniciarAoFechar, int intervalo, int tempoExibicao, bool modoRede, int porta, string ipServidor, string imagem, string som, bool ApenasSom, bool ApenasImagem, bool UsarMilesegundos);

        void Carregar();
    }
}