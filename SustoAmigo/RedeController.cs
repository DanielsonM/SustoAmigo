using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SustoAmigo
{
    public class RedeController
    {
        private TcpListener listener;

        public event Action OnReceberComando;

        public void IniciarServidor(int porta)
        {
            Task.Run(() =>
            {
                listener = new TcpListener(IPAddress.Any, porta);
                listener.Start();

                while (true)
                {
                    var client = listener.AcceptTcpClient();
                    var stream = client.GetStream();
                    byte[] buffer = new byte[1024];
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    string msg = Encoding.UTF8.GetString(buffer, 0, bytes);

                    if (msg == "SUSTO")
                    {
                        OnReceberComando?.Invoke();
                    }
                }
            });
        }

        public void EnviarComando(string ip, int porta, string comando)
        {
            using (TcpClient client = new TcpClient(ip, porta))
            {
                byte[] data = Encoding.UTF8.GetBytes(comando);
                client.GetStream().Write(data, 0, data.Length);
            }
        }
    }
}