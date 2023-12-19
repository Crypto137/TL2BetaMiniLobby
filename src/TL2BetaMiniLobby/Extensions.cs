using System.Text;

namespace TL2BetaMiniLobby
{
    public static class Extensions
    {
        /// <summary>
        /// Converts a byte array to a hex string.
        /// </summary>
        public static string ToHexString(this byte[] byteArray)
        {
            return byteArray.Aggregate("", (current, b) => current + b.ToString("X2"));
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
            if (bytes.Length > 255) throw new Exception("String size exceeded 255 bytes.");

            writer.Write((byte)@string.Length);
            writer.Write(bytes);
        }
    }
}
