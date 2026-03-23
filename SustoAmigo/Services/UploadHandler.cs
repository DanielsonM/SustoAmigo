using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SustoAmigo.Interfaces;

namespace SustoAmigo.Services
{
    public class UploadHandler : IUploadHandler
    {
        private static readonly string[] EXTENSOES_IMAGEM = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        private static readonly string[] EXTENSOES_SOM = { ".mp3", ".wav", ".wma", ".ogg", ".m4a" };
        private const string PASTA_UPLOADS = "uploads";

        public (bool Sucesso, string CaminhoArquivo, string Erro) ProcessarUploadImagem(Stream inputStream, string boundary)
        {
            return ProcessarUpload(inputStream, boundary, EXTENSOES_IMAGEM, "imagem");
        }

        public (bool Sucesso, string CaminhoArquivo, string Erro) ProcessarUploadSom(Stream inputStream, string boundary)
        {
            return ProcessarUpload(inputStream, boundary, EXTENSOES_SOM, "áudio");
        }

        private (bool Sucesso, string CaminhoArquivo, string Erro) ProcessarUpload(
            Stream inputStream, string boundary, string[] extensoesPermitidas, string tipoArquivo)
        {
            try
            {
                var memory = new MemoryStream();
                inputStream.CopyTo(memory);
                var dados = memory.ToArray();

                if (!ExtrairDadosMultipart(dados, boundary, out var cabecalho, out var dadosArquivo, out var inicioDados, out var fimDados))
                    return (false, null, "Formato multipart inválido");

                var nomeArquivo = ExtrairNomeArquivo(cabecalho);
                if (string.IsNullOrEmpty(nomeArquivo))
                    return (false, null, "Nome do arquivo não encontrado");

                var extensao = Path.GetExtension(nomeArquivo).ToLower();
                if (!extensoesPermitidas.Contains(extensao))
                    return (false, null, $"Formato de {tipoArquivo} não suportado");

                var bytesArquivo = new byte[fimDados - inicioDados];
                Array.Copy(dados, inicioDados, bytesArquivo, 0, bytesArquivo.Length);

                var caminhoArquivo = SalvarArquivo(bytesArquivo, extensao);
                return (true, caminhoArquivo, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Erro ao processar upload: {ex.Message}");
            }
        }

        private bool ExtrairDadosMultipart(byte[] dados, string boundary, out string cabecalho, out byte[] dadosArquivo, out int inicioDados, out int fimDados)
        {
            cabecalho = null;
            dadosArquivo = null;
            inicioDados = 0;
            fimDados = 0;

            var boundaryBytes = Encoding.UTF8.GetBytes($"--{boundary}");
            var inicioBoundary = Array.IndexOf(dados, boundaryBytes[0]);
            var proximaBoundary = Array.IndexOf(dados, boundaryBytes[0], inicioBoundary + 1);

            if (inicioBoundary < 0 || proximaBoundary < 0)
                return false;

            var fimCabecalho = Array.IndexOf(dados, (byte)'\r');
            if (fimCabecalho < 0)
                return false;

            cabecalho = Encoding.UTF8.GetString(dados, inicioBoundary, fimCabecalho - inicioBoundary);

            inicioDados = Array.IndexOf(dados, (byte)'\n', fimCabecalho) + 1;
            inicioDados = Array.IndexOf(dados, (byte)'\r', inicioDados) + 2;

            fimDados = Array.LastIndexOf(dados, boundaryBytes[0]);
            fimDados = Array.LastIndexOf(dados, (byte)'\n', fimDados - 2) - 1;

            return inicioDados < fimDados;
        }

        private string ExtrairNomeArquivo(string cabecalho)
        {
            var contentDisposition = cabecalho
                .Split(new[] { "\r\n" }, StringSplitOptions.None)
                .FirstOrDefault(l => l.Contains("Content-Disposition:"));

            if (string.IsNullOrEmpty(contentDisposition))
                return null;

            contentDisposition = contentDisposition
                .Replace("Content-Disposition: form-data;", "")
                .Trim();

            var match = Regex.Match(contentDisposition, "filename=\"([^\"]+)\"");
            return match.Success ? match.Groups[1].Value : null;
        }

        private string SalvarArquivo(byte[] bytesArquivo, string extensao)
        {
            var pastaUploads = Path.Combine(Application.StartupPath, PASTA_UPLOADS);
            if (!Directory.Exists(pastaUploads))
                Directory.CreateDirectory(pastaUploads);

            var nomeUnico = $"{Guid.NewGuid()}{extensao}";
            var caminhoArquivo = Path.Combine(pastaUploads, nomeUnico);
            File.WriteAllBytes(caminhoArquivo, bytesArquivo);

            System.Diagnostics.Debug.WriteLine($"[UploadHandler] Arquivo salvo: {caminhoArquivo}");
            System.Diagnostics.Debug.WriteLine($"[UploadHandler] pastaUploads: {pastaUploads}");
            System.Diagnostics.Debug.WriteLine($"[UploadHandler] nomeUnico: {nomeUnico}");

            return caminhoArquivo;
        }
    }
}
