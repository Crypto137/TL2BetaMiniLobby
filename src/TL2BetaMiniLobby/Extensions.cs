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
        /// Reads a fixed-length string preceded by its length as a signed 8-bit integer.
        /// </summary>
        public static string ReadFixedString8(this BinaryReader reader)
        {
            byte length = reader.ReadByte();
            return Encoding.UTF8.GetString(reader.ReadBytes(length));
        }
    }
}
