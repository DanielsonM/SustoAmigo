using System;

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

        void Salvar(int intervalo, int tempoExibicao, bool modoRede, int porta, string ipServidor, string imagem, string som);
        void Carregar();
    }
}
