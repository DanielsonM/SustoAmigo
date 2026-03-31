using SustoAmigo.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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
            System.Diagnostics.Debug.WriteLine("[UploadHandler] ProcessarUploadSom INICIADO");
            System.Diagnostics.Debug.WriteLine($"[UploadHandler] Boundary: {boundary}");
            var resultado = ProcessarUpload(inputStream, boundary, EXTENSOES_SOM, "áudio");
            System.Diagnostics.Debug.WriteLine($"[UploadHandler] ProcessarUploadSom FINALIZADO - Sucesso: {resultado.Sucesso}, Erro: {resultado.Erro}");
            return resultado;
        }

        private (bool Sucesso, string CaminhoArquivo, string Erro) ProcessarUpload(
            Stream inputStream, string boundary, string[] extensoesPermitidas, string tipoArquivo)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[UploadHandler] ProcessarUpload INICIADO - Tipo: {tipoArquivo}");
                
                var memory = new MemoryStream();
                inputStream.CopyTo(memory);
                var dados = memory.ToArray();

                System.Diagnostics.Debug.WriteLine($"[UploadHandler] Dados recebidos: {dados.Length} bytes");

                if (!ExtrairDadosMultipart(dados, boundary, out var cabecalho, out var dadosArquivo, out var inicioDados, out var fimDados))
                {
                    System.Diagnostics.Debug.WriteLine("[UploadHandler] Falha ao extrair dados multipart");
                    return (false, null, "Formato multipart inválido");
                }

                System.Diagnostics.Debug.WriteLine($"[UploadHandler] Cabeçalho: {cabecalho}");

                var nomeArquivo = ExtrairNomeArquivo(cabecalho);
                System.Diagnostics.Debug.WriteLine($"[UploadHandler] Nome do arquivo extraído: {nomeArquivo}");
                
                if (string.IsNullOrEmpty(nomeArquivo))
                    return (false, null, "Nome do arquivo não encontrado");

                var extensao = Path.GetExtension(nomeArquivo).ToLower();
                System.Diagnostics.Debug.WriteLine($"[UploadHandler] Extensão: {extensao}");
                System.Diagnostics.Debug.WriteLine($"[UploadHandler] Extensões permitidas: {string.Join(", ", extensoesPermitidas)}");
                
                if (!extensoesPermitidas.Contains(extensao))
                {
                    System.Diagnostics.Debug.WriteLine($"[UploadHandler] Extensão não permitida!");
                    return (false, null, $"Formato de {tipoArquivo} não suportado");
                }

                var bytesArquivo = new byte[fimDados - inicioDados];
                Array.Copy(dados, inicioDados, bytesArquivo, 0, bytesArquivo.Length);
                System.Diagnostics.Debug.WriteLine($"[UploadHandler] Bytes do arquivo: {bytesArquivo.Length}");

                var caminhoArquivo = SalvarArquivo(bytesArquivo, extensao);
                System.Diagnostics.Debug.WriteLine($"[UploadHandler] Arquivo salvo em: {caminhoArquivo}");
                
                return (true, caminhoArquivo, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UploadHandler] EXCEÇÃO: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[UploadHandler] Stack: {ex.StackTrace}");
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

            // Encontrar o primeiro boundary
            int inicioBoundary = IndexOf(dados, boundaryBytes, 0);
            if (inicioBoundary < 0)
                return false;

            // Encontrar o próximo boundary (fim da parte do arquivo)
            int proximaBoundary = IndexOf(dados, boundaryBytes, inicioBoundary + boundaryBytes.Length);
            if (proximaBoundary < 0)
                return false;

            // Procurar fim do cabeçalho (\r\n\r\n)
            var headerEndPattern = Encoding.UTF8.GetBytes("\r\n\r\n");
            int fimCabecalho = IndexOf(dados, headerEndPattern, inicioBoundary);
            if (fimCabecalho < 0)
                return false;

            // Extrair cabeçalho
            cabecalho = Encoding.UTF8.GetString(dados, inicioBoundary + boundaryBytes.Length + 2, fimCabecalho - (inicioBoundary + boundaryBytes.Length + 2));

            // Dados começam logo após o cabeçalho
            inicioDados = fimCabecalho + headerEndPattern.Length;

            // Dados terminam antes do próximo boundary
            fimDados = proximaBoundary - 2; // remove \r\n antes do boundary

            // Extrair os bytes do arquivo
            int tamanho = fimDados - inicioDados;
            if (tamanho <= 0)
                return false;

            dadosArquivo = new byte[tamanho];
            Array.Copy(dados, inicioDados, dadosArquivo, 0, tamanho);

            return true;
        }

        // Função auxiliar para procurar sequência de bytes
        private int IndexOf(byte[] buffer, byte[] pattern, int startIndex = 0)
        {
            for (int i = startIndex; i <= buffer.Length - pattern.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (buffer[i + j] != pattern[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match) return i;
            }
            return -1;
        }


        private string ExtrairNomeArquivo(string cabecalho)
        {
            var match = Regex.Match(cabecalho, @"filename=""(?<nome>[^""]+)""");
            return match.Success ? match.Groups["nome"].Value : null;
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