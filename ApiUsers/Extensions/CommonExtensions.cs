namespace ApiUsers.Extensions
{
    public static class CommonExtensions
    {
        public static bool HasPropertie(this Type type, string name)
            => type.GetProperties().Any(p => p.Name.Equals(name));

        public static byte[] ToByteArray(this Stream stream)
        {
            byte[] bytes;

            if (stream is MemoryStream memStream)
            {
                return memStream.ToArray();
            }

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            return bytes;
        }
    }
}
