using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TL2BetaMiniLobby
{
    public class LobbyServer
    {
        private const string BindIP = "0.0.0.0";
        private const int Port = 4549;

        private readonly CancellationTokenSource _cts = new();

        private readonly HashSet<LobbyClient> _clientHashSet = new();
        private readonly object _clientLock = new();

        private readonly TcpListener _listener;

        public LobbyServer()
        {
            IPEndPoint endpoint = new(IPAddress.Parse(BindIP), Port);
            _listener = new(endpoint);
            _listener.Server.NoDelay = true;
            _listener.Server.LingerState = new(false, 0);
        }

        /// <summary>
        /// Starts listening and accepting client connections.
        /// </summary>
        public void Start()
        {
            _listener.Start();
            Task.Run(async () => await AcceptConnectionsAsync());
            Console.WriteLine($"LobbyServer is listening on {BindIP}:{Port}...");
        }

        /// <summary>
        /// Disconnects a client.
        /// </summary>
        /// <param name="client"></param>
        public void DisconnectClient(LobbyClient client)
        {
            client.TcpClient.Close();
            RemoveClient(client);
            Console.WriteLine($"{client.Username} disconnected");
        }

        /// <summary>
        /// Disconnects all clients.
        /// </summary>
        public void DisconnectAllClients()
        {
            lock (_clientLock)
            {
                foreach (LobbyClient client in _clientHashSet)
                {
                    if (client.TcpClient.Connected == false) continue;
                    client.TcpClient.Close();
                }
            }

            _clientHashSet.Clear();
        }

        /// <summary>
        /// Shuts the server down.
        /// </summary>
        public void Shutdown()
        {
            _cts.Cancel();              // Cancel async tasks
            _listener?.Stop();          // Stop listening
            DisconnectAllClients();     // Disconnect all clients
        }

        /// <summary>
        /// Retrieves server status.
        /// </summary>
        public string GetStatus()
        {
            StringBuilder sb = new();

            sb.Append($"{_clientHashSet.Count} client(s) online");

            // Add client usernames if anyone's online
            if (_clientHashSet.Count > 0)
            {
                sb.Append(": ");
                foreach (LobbyClient client in _clientHashSet)
                    sb.Append(client.Username).Append(", ");
                sb.Length -= 2; // Remove last comma and space
            }

            return sb.ToString();
        }

        /// <summary>
        /// Removes a connected client.
        /// </summary>
        private void RemoveClient(LobbyClient client)
        {
            lock (_clientLock) _clientHashSet.Remove(client);
        }

        /// <summary>
        /// Accepts client connections asynchronously.
        /// </summary>
        private async Task AcceptConnectionsAsync()
        {
            while (true)
            {
                try
                {
                    // Wait for a client connection
                    var tcpClient = await _listener.AcceptTcpClientAsync().WaitAsync(_cts.Token);

                    // Accept the client
                    Console.WriteLine($"Accepting connection from {tcpClient.Client.RemoteEndPoint}...");
                    LobbyClient lobbyClient = new(this, tcpClient);
                    lock (_clientLock) _clientHashSet.Add(lobbyClient);

                    // Start receiving data from the client
                    _ = Task.Run(async () => await lobbyClient.ReceiveDataAsync(_cts.Token));
                }
                catch (TaskCanceledException) { return; }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
        }
    }
}
