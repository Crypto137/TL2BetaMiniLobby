using System.Net.Sockets;
using TL2BetaMiniLobby.Messages;

namespace TL2BetaMiniLobby
{
    /// <summary>
    /// Represents a client connected to a lobby server.
    /// </summary>
    public class LobbyClient
    {
        private readonly byte[] _buffer = new byte[1024];
        private readonly LobbyServer _lobbyServer;
        private readonly TcpClient _tcpClient;

        public string Username { get; set; } = "Unknown";

        public LobbyClient(LobbyServer server, TcpClient tcpClient)
        {
            _lobbyServer = server;
            _tcpClient = tcpClient;
        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    if (_tcpClient.Client == null) break;

                    _tcpClient.Client.Receive(_buffer, SocketFlags.None);

                    // Parse packet and try to handle its message
                    Packet packet = new(_buffer);
                    if (MessageHandler.HandleMessage(this, packet.Message) == false)
                        Console.WriteLine($"Unhandled message: {packet.Opcode}");
                }
                catch
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Disconnects this client from the server.
        /// </summary>
        public void Disconnect()
        {
            Console.WriteLine($"{Username} disconnected");
            _tcpClient.Close();
            _lobbyServer.RemoveClient(this);
        }

        /// <summary>
        /// Sends a message to the server.
        /// </summary>
        public void Send(LobbyMessage message)
        {
            Packet packet = new(message);
            _tcpClient.Client.Send(packet.Serialize());
        }
    }
}
