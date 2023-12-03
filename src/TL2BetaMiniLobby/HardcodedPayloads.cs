namespace TL2BetaMiniLobby
{
    public static class HardcodedPayloads
    {
        public static readonly byte[] NetConnectOkMsg               = Convert.FromHexString("0000000000000000010000000100000000FFFFFFFF");
        public static readonly byte[] LobbyLoginChallengeMsg        = Convert.FromHexString("0000");
        public static readonly byte[] LobbyLoginResult              = Convert.FromHexString("000000000011C500");
        public static readonly byte[] LobbyCreateGameResponseMsg    = Convert.FromHexString("00");
    }
}
