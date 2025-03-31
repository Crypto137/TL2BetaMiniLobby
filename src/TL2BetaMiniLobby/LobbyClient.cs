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
        private readonly byte[] _receiveBuffer = new byte[1024];
        private readonly LobbyServer _server;

        public TcpClient TcpClient { get; }
        public string Username { get; set; } = string.Empty;

        public LobbyClient(LobbyServer server, TcpClient tcpClient)
        {
            _server = server;
            TcpClient = tcpClient;
        }

        /// <summary>
        /// Receives and handles data from the connected client asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task ReceiveDataAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    if (TcpClient.Client == null)
                        break;

                    // Receive data asynchronously
                    await TcpClient.Client.ReceiveAsync(_receiveBuffer, SocketFlags.None).WaitAsync(cancellationToken);

                    // Parse packet and try to handle its message
                    Packet packet = new(_receiveBuffer);
                    if (MessageHandler.HandleMessage(this, packet.Message) == false)
                        Logger.Log($"Unhandled message: {packet.Opcode}");

                    // Dump the message if needed
                    if (Program.MessageDumpMode)
                    {
                        Logger.Log(packet.Message.RawData.ToHexString());
                        SaveMessageToFile(packet);
                    }
                }
                catch (TaskCanceledException)
                {
                    return;
                }
                catch (Exception e)
                {
                    Logger.Log(e.Message);
                    Disconnect();
                }
            }
        }

        /// <summary>
        /// Disconnects this client from the server.
        /// </summary>
        public void Disconnect()
        {
            _server.DisconnectClient(this);
        }

        /// <summary>
        /// Sends a message to the server.
        /// </summary>
        public void Send(LobbyMessage message)
        {
            Packet packet = new(message);
            TcpClient.Client.Send(packet.Serialize());
        }

        /// <summary>
        /// Saves a raw message to a file.
        /// </summary>
        private void SaveMessageToFile(Packet packet)
        {
            string root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string packetDir = Path.Combine(root, "DumpedMessages");
            if (Directory.Exists(packetDir) == false)
                Directory.CreateDirectory(packetDir);

            string filePath = $"[{DateTime.Now:yyyy-dd-MM_HH.mm.ss.fff}] {packet.Opcode}.bin";
            File.WriteAllBytes(Path.Combine(packetDir, filePath), packet.Message.RawData);
        }
    }
}
