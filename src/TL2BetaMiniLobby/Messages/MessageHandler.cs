using TL2BetaMiniLobby.Messages.Definitions;

namespace TL2BetaMiniLobby.Messages
{
    public static class MessageHandler
    {
        /// <summary>
        /// Handles a message received from a client.
        /// </summary>
        public static bool HandleMessage(LobbyClient client, LobbyMessage message)
        {
            switch (message)
            {
                case NetConnectMsg netConnectMsg:                   OnNetConnectMsg(client, netConnectMsg); break;
                case NetDisconnectMsg netDisconnectMsg:             OnNetDisconnectMsg(client, netDisconnectMsg); break;
                case LobbyStartLoginMsg startLoginMsg:              OnStartLoginMsg(client, startLoginMsg); break;
                case LobbyLoginResponseMsg loginResponseMsg:        OnLoginResponseMsg(client, loginResponseMsg); break;
                case LobbyCreateGameServerMsg createGameServerMsg:  OnCreateGameServerMsg(client, createGameServerMsg); break;
                case LobbyKeepaliveMsg keepaliveMsg:                OnKeepaliveMsg(client, keepaliveMsg); break;

                default: return false;
            }

            return true;
        }

        private static void OnNetConnectMsg(LobbyClient client, NetConnectMsg message)
        {
            client.Send(new NetConnectOkMsg() { Field1 = 0x1, Field2 = 0x1, ClientKey = 0xFFFFFFFF });
        }

        private static void OnNetDisconnectMsg(LobbyClient client, NetDisconnectMsg message)
        {
            client.Disconnect();
        }

        private static void OnStartLoginMsg(LobbyClient client, LobbyStartLoginMsg message)
        {
            client.Username = message.Username;
            client.Send(new LobbyLoginChallengeMsg());
        }

        private static void OnLoginResponseMsg(LobbyClient client, LobbyLoginResponseMsg message)
        {
            Console.WriteLine($"{client.Username} logged in");
            client.Send(new LobbyLoginResultMsg());
        }

        private static void OnCreateGameServerMsg(LobbyClient client, LobbyCreateGameServerMsg message)
        {
            Console.WriteLine($"{client.Username} created a game server: {message.Name} (id 0x{message.GameServerId})");
            client.Send(new LobbyCreateGameResponseMsg() { Response = CreateGameResponse.Success });
        }

        private static void OnKeepaliveMsg(LobbyClient client, LobbyKeepaliveMsg message)
        {
            client.Send(new LobbyKeepaliveMsg());
        }
    }
}
