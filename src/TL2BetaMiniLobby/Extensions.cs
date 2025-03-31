using System.Text;

namespace TL2BetaMiniLobby
{
    public static class Extensions
    {
        /// <summary>
        /// Converts a byte array to a hex string.
        /// </summary>
        public static string ToHexString(this byte[] bytes)
        {
            return Convert.ToHexString(bytes);
        }

        /// <summary>
        /// Reads a fixed-length string preceded by its length as an unsigned 8-bit integer.
        /// </summary>
        public static string ReadFixedString8(this BinaryReader reader)
        {
            byte length = reader.ReadByte();
            return Encoding.UTF8.GetString(reader.ReadBytes(length));
        }

        /// <summary>
        /// Writes a fixed-length string preceded by its length as an unsigned 8-bit integer.
        /// </summary>
        public static void WriteFixedString8(this BinaryWriter writer, string @string)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(@string);
            if (bytes.Length > byte.MaxValue)
                throw new Exception($"String size exceeded {byte.MaxValue} bytes.");

            writer.Write((byte)@string.Length);
            writer.Write(bytes);
        }

        public static ushort ReadUInt16BigEndian(this BinaryReader reader)
        {
            ushort value = (ushort)(reader.ReadByte() << 8);
            value |= reader.ReadByte();
            return value;
        }

        public static void WriteUInt16BigEndian(this BinaryWriter writer, ushort value)
        {
            writer.Write((byte)(value >> 8));
            writer.Write((byte)(value & 0xFF));
        }
    }
}
