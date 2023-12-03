namespace TL2BetaMiniLobby
{
    [Flags]
    public enum PacketFlags : byte
    {
        None = 0,
        Modded = 1 << 0
    }

    public class Packet
    {
        public LobbyOpcode Opcode { get; }
        public PacketFlags Flags { get; }
        public byte[] BasePayload { get; }
        public byte[] ModPayload { get; }

        public Packet(byte[] buffer)
        {
            using (MemoryStream stream = new(buffer))
            using (BinaryReader reader = new(stream))
            {
                Opcode = (LobbyOpcode)reader.ReadByte();
                Flags = (PacketFlags)reader.ReadByte();
                byte payloadSize = reader.ReadByte();
                BasePayload = reader.ReadBytes(payloadSize);

                // Packets that have 0x01 as their second byte contain an additional 256-byte payload
                // This happens only in modded games
                if (Flags.HasFlag(PacketFlags.Modded))
                    ModPayload = reader.ReadBytes(256);
            }
        }

        public Packet(LobbyOpcode opcode, byte[] data)
        {
            Opcode = opcode;
            Flags = PacketFlags.None;
            BasePayload = data;
        }

        public byte[] Serialize()
        {
            using (MemoryStream stream = new())
            using (BinaryWriter writer = new(stream))
            {
                writer.Write((byte)Opcode);
                writer.Write((byte)Flags);
                writer.Write((byte)BasePayload.Length);
                writer.Write(BasePayload);
                writer.Flush();
                return stream.ToArray();
            }
        }
    }
}
