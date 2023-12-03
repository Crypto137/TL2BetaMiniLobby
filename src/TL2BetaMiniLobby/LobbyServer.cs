using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TL2BetaMiniLobby
{
    public class LobbyServer
    {
        private const string BindIP = "0.0.0.0";
        private const int Port = 4549;

        private readonly List<LobbyClient> _clientList = new();
        private readonly TcpListener _listener;

        public LobbyServer()
        {
            IPEndPoint endpoint = new(IPAddress.Parse(BindIP), Port);
            _listener = new(endpoint);
            _listener.Server.NoDelay = true;
            _listener.Server.LingerState = new(false, 0);
        }

        public async void Run()
        {
            _listener.Start();
            Console.WriteLine($"LobbyServer is listening on {BindIP}:{Port}...");

            while (true)
            {
                var tcpClient = await _listener.AcceptTcpClientAsync();
                Console.WriteLine($"Accepting connection from {tcpClient.Client.RemoteEndPoint}...");
                LobbyClient lobbyClient = new(this, tcpClient);
                _clientList.Add(lobbyClient);
                new Thread(() => lobbyClient.Run()).Start();
            }
        }

        public void RemoveClient(LobbyClient client)
        {
            _clientList.Remove(client);
        }

        public void Shutdown()
        {
            while (_clientList.Count > 0)
                _clientList.First().Disconnect();
        }

        public string GetStatus()
        {
            StringBuilder sb = new();

            sb.Append($"{_clientList.Count} client(s) online");

            // Add client usernames if anyone's online
            if (_clientList.Count > 0)
            {
                sb.Append(": ");
                foreach (LobbyClient client in _clientList)
                    sb.Append(client.Username).Append(", ");
                sb.Length -= 2; // Remove last comma and space
            }

            return sb.ToString();
        }
    }
}
