using System.Text;

namespace TL2BetaMiniLobby.Messages
{
    public class LobbyMessage
    {
        public byte[] RawData { get; }

        public LobbyMessage(byte[] rawData)
        {
            RawData = rawData;

            using (MemoryStream stream = new(rawData))
            using (BinaryReader reader = new(stream))
            {
                Parse(reader);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            BuildString(sb);
            return sb.ToString();
        }

        public virtual void Parse(BinaryReader reader) { }

        public virtual void Encode(BinaryWriter writer) => writer.Write(RawData);

        public virtual void BuildString(StringBuilder sb)
        {
            sb.AppendLine($"Raw Data: {RawData.ToHexString()}");
        }
    }
}
