using System.Text;

namespace TL2BetaMiniLobby.Messages.Definitions
{
    public class LobbyStartLoginMsg : LobbyMessage
    {
        public byte[] UnkBytes { get; private set; }    // Character data?
        public string Username { get; private set; }
        public string CharacterName { get; private set; }

        public LobbyStartLoginMsg(byte[] rawData) : base(rawData) { }

        public override void Parse(BinaryReader reader)
        {
            UnkBytes = reader.ReadBytes(16);
            Username = reader.ReadFixedString8();
            CharacterName = reader.ReadFixedString8();
        }

        public override void BuildString(StringBuilder sb)
        {
            base.BuildString(sb);
            sb.AppendLine($"{nameof(UnkBytes)}: {UnkBytes.ToHexString()}");
            sb.AppendLine($"{nameof(Username)}: {Username}");
            sb.AppendLine($"{nameof(CharacterName)}: {CharacterName}");
        }
    }
}
