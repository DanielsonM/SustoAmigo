using System;
using System.IO;
using System.Linq;

namespace SustoAmigo.Interfaces
{
    public interface IUploadHandler
    {
        (bool Sucesso, string CaminhoArquivo, string Erro) ProcessarUploadImagem(Stream inputStream, string boundary);
        (bool Sucesso, string CaminhoArquivo, string Erro) ProcessarUploadSom(Stream inputStream, string boundary);
    }
}
