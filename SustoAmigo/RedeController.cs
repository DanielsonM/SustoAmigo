using SustoAmigo.Configuracoes;
using SustoAmigo.Dto;
using SustoAmigo.Interfaces;
using SustoAmigo.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SustoAmigo
{
    public class RedeController : IDisposable
    {
        private readonly IUploadHandler _uploadHandler;
        private TcpListener _listener;
        private HttpListener _httpListener;
        private bool _disposed;

        public event Action OnReceberComando;

        private static readonly Lazy<RedeController> _i = new Lazy<RedeController>(() => new RedeController());

        public static RedeController i => _i.Value;

        public RedeController() : this(new UploadHandler())
        {
        }

        public RedeController(IUploadHandler uploadHandler)
        {
            _uploadHandler = uploadHandler;
        }

        private void ProcessarRequisicao(HttpListenerContext context)
        {
            var path = context.Request.Url.AbsolutePath.ToLower();

            if (path == "/config")
                ProcessarConfiguracao(context);
            else if (path.StartsWith("/upload"))
                ProcessarUpload(context, path);
            else if (path == "/comando")
                ProcessarComando(context);
            else
                Responder(context, 400, "Endpoint inválido");
        }

        private void ProcessarComando(HttpListenerContext context)
        {
            try
            {
                using (var reader = new StreamReader(context.Request.InputStream))
                {
                    var body = reader.ReadToEnd();
                    var json = JsonSerializer.Deserialize<Dictionary<string, string>>(body);

                    if (json != null && json.TryGetValue("acao", out var acao))
                    {
                        if (acao == "SUSTO")
                        {
                            OnReceberComando?.Invoke();
                            Responder(context, 200, "Comando SUSTO recebido!");
                            return;
                        }
                    }
                }

                Responder(context, 400, "Comando inválido");
            }
            catch (Exception ex)
            {
                Responder(context, 500, $"Erro ao processar comando: {ex.Message}");
            }
        }

        public void IniciarServidor(int porta)
        {
            Task.Run(() =>
            {
                try
                {
                    _listener = new TcpListener(IPAddress.Any, porta);
                    _listener.Start();

                    while (true)
                    {
                        var client = _listener.AcceptTcpClient();
                        using (client)
                        using (var stream = client.GetStream())
                        {
                            var buffer = new byte[1024];
                            var bytes = stream.Read(buffer, 0, buffer.Length);
                            var msg = Encoding.UTF8.GetString(buffer, 0, bytes);

                            if (msg == "SUSTO")
                                OnReceberComando?.Invoke();
                        }
                    }
                }
                catch (ObjectDisposedException)
                {
                    // Servidor foi parado
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro no servidor TCP: {ex.Message}");
                }
            });
        }

        public void IniciarServidorHttp(int porta)
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add($"http://+:{porta}/");
            _httpListener.Start();

            Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        var context = _httpListener.GetContext();
                        ProcessarRequisicao(context);
                    }
                }
                catch (HttpListenerException)
                {
                    // Listener foi parado
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro no servidor HTTP: {ex.Message}");
                }
            });
        }

        private void ProcessarConfiguracao(HttpListenerContext context)
        {
            try
            {
                using (var reader = new StreamReader(context.Request.InputStream))
                {
                    var body = reader.ReadToEnd();

                    if (string.IsNullOrWhiteSpace(body))
                    {
                        Responder(context, 400, "Nenhum JSON enviado");
                        return;
                    }

                    var valores = JsonSerializer.Deserialize<ConfigDto>(body);

                    ConfiguracaoXml.i.Salvar(
                        valores.booReiniciarAoFechar,
                        valores.IntervaloExecucao,
                        valores.TempoExibicao,
                        valores.ModoRede,
                        valores.Porta,
                        valores.IpServidor,
                        valores.ImagemSelecionada,
                        valores.SomSelecionado
                    );
                }

                Responder(context, 200, "Configuração atualizada com sucesso!");
            }
            catch (Exception ex)
            {
                Responder(context, 500, $"Erro ao salvar configuração: {ex.Message}");
            }
        }

        private void ProcessarUpload(HttpListenerContext context, string path)
        {
            var contentType = context.Request.ContentType ?? string.Empty;
            if (!contentType.StartsWith("multipart/form-data"))
            {
                Responder(context, 400, "Content-Type deve ser multipart/form-data");
                return;
            }

            var boundary = contentType.Split('=').LastOrDefault()?.Trim('"');
            if (string.IsNullOrEmpty(boundary))
            {
                Responder(context, 400, "Boundary não encontrado no Content-Type");
                return;
            }

            if (path.StartsWith("/upload/photo"))
            {
                var resultado = _uploadHandler.ProcessarUploadImagem(context.Request.InputStream, boundary);
                ProcessarResultadoUpload(context, resultado, "Imagem");
            }
            else if (path.StartsWith("/upload/audio"))
            {
                var resultado = _uploadHandler.ProcessarUploadSom(context.Request.InputStream, boundary);
                ProcessarResultadoUpload(context, resultado, "audio");
            }
            else
            {
                Responder(context, 400, "Endpoint de upload inválido. Use /upload/imagem ou /upload/audio");
            }
        }

        private void ProcessarResultadoUpload(HttpListenerContext context, (bool Sucesso, string CaminhoArquivo, string Erro) resultado, string tipo)
        {
            if (resultado.Sucesso)
            {
                Responder(context, 200, $"{tipo} salvo com sucesso: {resultado.CaminhoArquivo}");
            }
            else
            {
                Responder(context, 400, $"Erro no upload: {resultado.Erro}");
            }
        }

        private void Responder(HttpListenerContext context, int statusCode, string mensagem)
        {
            context.Response.StatusCode = statusCode;
            using (var writer = new StreamWriter(context.Response.OutputStream))
            {
                writer.Write(mensagem);
            }
            context.Response.Close();
        }

        public void EnviarComando(string ip, int porta, string comando)
        {
            using (var client = new TcpClient(ip, porta))
            using (var stream = client.GetStream())
            {
                var data = Encoding.UTF8.GetBytes(comando);
                stream.Write(data, 0, data.Length);
            }
        }

        public void PararServidor()
        {
            _listener?.Stop();
            _httpListener?.Stop();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    PararServidor();
                    _httpListener?.Close();
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