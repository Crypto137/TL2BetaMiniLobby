using System.Net.Sockets;
using TL2BetaMiniLobby.Messages.Definitions;

namespace TL2BetaMiniLobby
{
    public class LobbyClient
    {
        private readonly byte[] _buffer = new byte[1024];
        private readonly LobbyServer _lobbyServer;
        private readonly TcpClient _tcpClient;

        public string Username { get; private set; } = "Unknown";

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
                    HandlePacket(new(_buffer));
                }
                catch
                {
                    break;
                }
            }
        }

        public void Disconnect()
        {
            Console.WriteLine($"{Username} disconnected");
            _tcpClient.Close();
            _lobbyServer.RemoveClient(this);
        }

        private void HandlePacket(Packet packet)
        {
            switch (packet.Opcode)
            {
                case LobbyOpcode.NetConnectMsg:
                    SendMessage(LobbyOpcode.NetConnectOkMsg, HardcodedPayloads.NetConnectOkMsg);
                    break;

                case LobbyOpcode.NetDisconnectMsg:
                    Disconnect();
                    break;

                case LobbyOpcode.LobbyStartLoginMsg:
                    LobbyStartLoginMsg startLoginMessage = new(packet.BasePayload);
                    Username = startLoginMessage.Username;

                    SendMessage(LobbyOpcode.LobbyLoginChallengeMsg, HardcodedPayloads.LobbyLoginChallengeMsg);
                    break;

                case LobbyOpcode.LobbyLoginResponseMsg:
                    Console.WriteLine($"{Username} logged in");
                    SendMessage(LobbyOpcode.LobbyLoginResultMsg, HardcodedPayloads.LobbyLoginResult);
                    break;

                case LobbyOpcode.LobbyCreateGameServerMsg:
                    Console.WriteLine($"{Username} created a game server");
                    SendMessage(LobbyOpcode.LobbyCreateGameResponseMsg, HardcodedPayloads.LobbyCreateGameResponseMsg);
                    break;

                case LobbyOpcode.LobbyKeepaliveMsg:
                    SendMessage(LobbyOpcode.LobbyKeepaliveMsg, Array.Empty<byte>());
                    break;

                default:
                    Console.WriteLine($"Unhandled message: {packet.Opcode}");
                    break;
            }
        }

        private void SendMessage(LobbyOpcode opcode, byte[] payload)
        {
            Packet packet = new(opcode, payload);
            _tcpClient.Client.Send(packet.Serialize());
        }
    }
}
