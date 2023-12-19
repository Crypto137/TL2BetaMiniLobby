using System.Net.Sockets;
using System.Reflection;
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

                    // Dump the message if needed
                    if (Program.MessageDumpMode)
                    {
                        Console.WriteLine(packet.Message.RawData.ToHexString());
                        SaveMessageToFile(packet);
                    }
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

        /// <summary>
        /// Saves a raw message to a file.
        /// </summary>
        private void SaveMessageToFile(Packet packet)
        {
            // Create packet directory if needed
            string root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string packetDir = Path.Combine(root, "DumpedMessages");
            if (Directory.Exists(packetDir) == false)
                Directory.CreateDirectory(packetDir);

            // Save message to a file
            string filePath = $"[{DateTime.Now:yyyy-dd-MM_HH.mm.ss.fff}] {packet.Opcode}.bin";
            File.WriteAllBytes(Path.Combine(packetDir, filePath), packet.Message.RawData);
        }
    }
}
