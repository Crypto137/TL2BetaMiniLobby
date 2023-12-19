using TL2BetaMiniLobby.Messages;

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
        public LobbyMessage Message { get; }
        public byte[] ModPayload { get; }

        public Packet(byte[] buffer)
        {
            using (MemoryStream stream = new(buffer))
            using (BinaryReader reader = new(stream))
            {
                Opcode = (LobbyOpcode)reader.ReadByte();
                Flags = (PacketFlags)reader.ReadByte();

                // Read payload and parse message
                byte payloadSize = reader.ReadByte();
                byte[] payload = reader.ReadBytes(payloadSize);
                Message = MessageManager.ParseMessage(Opcode, payload);

                // Packets that have 0x01 as their second byte contain an additional 256-byte payload
                // This happens only in modded games
                if (Flags.HasFlag(PacketFlags.Modded))
                    ModPayload = reader.ReadBytes(256);
            }
        }

        public Packet(LobbyMessage message)
        {
            if (MessageManager.TryGetMessageOpcode(message, out LobbyOpcode opcode) == false)
                throw new("Attempted to create a packet for an undefined message.");

            Opcode = opcode;
            Flags = PacketFlags.None;
            Message = message;
        }

        public byte[] Serialize()
        {
            Message.Serialize();
            if (Message.RawData.Length > 255) throw new Exception("Message size exceeded 255 bytes.");

            using (MemoryStream stream = new())
            using (BinaryWriter writer = new(stream))
            {
                writer.Write((byte)Opcode);
                writer.Write((byte)Flags);
                writer.Write((byte)Message.RawData.Length);
                writer.Write(Message.RawData);
                writer.Flush();
                return stream.ToArray();
            }
        }
    }
}
