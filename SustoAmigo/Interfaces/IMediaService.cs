using System;
using System.IO;
using System.Threading.Tasks;

namespace SustoAmigo.Interfaces
{
    public interface IMediaService
    {
        Task<string> CarregarImagemAsync(string pastaUploads, string pastaPadrao, string imagemSelecionada);
        Task<string> CarregarSomAsync(string pastaUploads, string pastaPadrao, string somSelecionado);
        void ReproduzirSom(string caminhoSom);
        void PararSom();
    }
}
