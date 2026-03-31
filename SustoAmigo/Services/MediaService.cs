using SustoAmigo.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WMPLib;

namespace SustoAmigo.Services
{
    public class MediaService : IMediaService, IDisposable
    {
        private const string PASTA_UPLOADS = "uploads";
        private static readonly string[] EXTENSOES_IMAGEM = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        private static readonly string[] EXTENSOES_SOM = { ".mp3", ".wav", ".wma", ".ogg", ".m4a" };

        private WindowsMediaPlayer _wmpPlayer;
        private string _arquivoTemporario;
        private bool _disposed;

        private static readonly Lazy<MediaService> _i = new Lazy<MediaService>(() => new MediaService());

        public static MediaService i => _i.Value;

        public async Task<string> CarregarImagemAsync(string pastaUploads, string pastaPadrao, string imagemSelecionada)
        {
            return await Task.Run(() =>
            {
                var arquivoImagem = ObterArquivoImagem(pastaUploads, pastaPadrao, imagemSelecionada);

                if (string.IsNullOrEmpty(arquivoImagem) || !File.Exists(arquivoImagem))
                {
                    return null;
                }

                return arquivoImagem;
            });
        }

        public async Task<string> CarregarSomAsync(string pastaUploads, string pastaPadrao, string somSelecionado)
        {
            return await Task.Run(() =>
            {
                var arquivoSom = ObterArquivoSom(pastaUploads, pastaPadrao, somSelecionado);
                return arquivoSom;
            });
        }

        public void ReproduzirSom(string caminhoSom)
        {
            PararSom();

            _wmpPlayer = new WindowsMediaPlayer();

            if (!string.IsNullOrEmpty(caminhoSom) && File.Exists(caminhoSom))
            {
                _wmpPlayer.URL = caminhoSom;
                _wmpPlayer.controls.play();
            }
            else
            {
                var arquivoTemp = ReproduzirSomPadrao();
                _wmpPlayer.URL = arquivoTemp;
                _wmpPlayer.controls.play();
            }
        }

        public void PararSom()
        {
            if (_wmpPlayer != null)
            {
                try
                {
                    _wmpPlayer.controls.stop();
                    _wmpPlayer = null;
                }
                catch
                {
                    // Ignora erros ao parar o player
                }
            }
        }

        private string ObterArquivoImagem(string pastaUploads, string pastaPadrao, string imagemSelecionada)
        {
            string arquivoImagem = null;

            if (Directory.Exists(pastaUploads))
            {
                var imagens = Directory.GetFiles(pastaUploads, "*.*")
                    .Where(f => EXTENSOES_IMAGEM.Contains(Path.GetExtension(f).ToLower()))
                    .OrderByDescending(f => File.GetCreationTime(f))  // Ordena por data de criação (mais recente primeiro)
                    .ToArray();

                if (imagens.Length > 0)
                {
                    // Se tiver imagem selecionada e ela existir, usa ela
                    if (!string.IsNullOrEmpty(imagemSelecionada))
                    {
                        var caminhoSelecionado = Path.Combine(pastaUploads, imagemSelecionada);
                        if (File.Exists(caminhoSelecionado))
                        {
                            arquivoImagem = caminhoSelecionado;
                        }
                        else
                        {
                            // Imagem selecionada não existe, pega a mais recente
                            arquivoImagem = imagens.First();
                        }
                    }
                    else
                    {
                        // Pega a imagem mais recente
                        arquivoImagem = imagens.First();
                    }
                }
            }

            if (string.IsNullOrEmpty(arquivoImagem) && Directory.Exists(pastaPadrao))
            {
                var imagensPadrao = Directory.GetFiles(pastaPadrao);
                if (imagensPadrao.Length > 0)
                {
                    arquivoImagem = !string.IsNullOrEmpty(imagemSelecionada)
                        ? Path.Combine(pastaPadrao, imagemSelecionada)
                        : imagensPadrao.First();

                    if (!File.Exists(arquivoImagem))
                        arquivoImagem = imagensPadrao.First();
                }
            }

            return arquivoImagem;
        }

        private string ObterArquivoSom(string pastaUploads, string pastaPadrao, string somSelecionado)
        {
            string arquivoSom = null;

            if (Directory.Exists(pastaUploads))
            {
                var sons = Directory.GetFiles(pastaUploads, "*.*")
                    .Where(f => EXTENSOES_SOM.Contains(Path.GetExtension(f).ToLower()))
                    .OrderByDescending(f => File.GetCreationTime(f))  // Ordena por data de criação (mais recente primeiro)
                    .ToArray();

                if (sons.Length > 0)
                {
                    // Se tiver som selecionado e ele existir, usa ele
                    if (!string.IsNullOrEmpty(somSelecionado))
                    {
                        var caminhoSelecionado = Path.Combine(pastaUploads, somSelecionado);
                        if (File.Exists(caminhoSelecionado))
                        {
                            arquivoSom = caminhoSelecionado;
                        }
                        else
                        {
                            // Som selecionado não existe, pega o mais recente
                            arquivoSom = sons.First();
                        }
                    }
                    else
                    {
                        // Pega o som mais recente
                        arquivoSom = sons.First();
                    }
                }
            }

            if (string.IsNullOrEmpty(arquivoSom) && Directory.Exists(pastaPadrao))
            {
                var sonsPadrao = Directory.GetFiles(pastaPadrao, "*");
                if (sonsPadrao.Length > 0)
                {
                    arquivoSom = !string.IsNullOrEmpty(somSelecionado)
                        ? Path.Combine(pastaPadrao, somSelecionado)
                        : sonsPadrao.FirstOrDefault();
                }
            }

            return arquivoSom;
        }

        private string ReproduzirSomPadrao()
        {
            _arquivoTemporario = Path.Combine(Path.GetTempPath(), "grito.wav");

            using (var resourceStream = Properties.Resources.Grito)
            using (var fileStream = new FileStream(_arquivoTemporario, FileMode.Create, FileAccess.Write))
            {
                resourceStream.CopyTo(fileStream);
            }

            return _arquivoTemporario;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    PararSom();

                    if (!string.IsNullOrEmpty(_arquivoTemporario) && File.Exists(_arquivoTemporario))
                    {
                        try
                        {
                            File.Delete(_arquivoTemporario);
                        }
                        catch
                        {
                            // Ignora erros ao deletar arquivo temporário
                        }
                    }
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}