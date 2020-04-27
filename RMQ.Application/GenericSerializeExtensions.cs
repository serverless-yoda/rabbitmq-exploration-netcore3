using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace RMQ.Application
{
    public static class GenericSerializeExtensions
    {
        public static byte[] Serialize<T>(this T obj)
        {
            if (obj is null)
            {
                return null;
            }

            using var ms = new MemoryStream();
            var bf = new BinaryFormatter();
            bf.Serialize(ms, obj);
            return Flattened(ms.ToArray());
        }


        public static T Deserialize<T>(this byte[] obj)
        {

            using var ms = new MemoryStream();
            var bf = new BinaryFormatter();
            var unflattened = Unflattened(obj);

            ms.Write(unflattened, 0, unflattened.Length);
            ms.Seek(0, SeekOrigin.Begin);

            return (T)bf.Deserialize(ms);
        }

        private static byte[] Unflattened(byte[] obj)
        {
            byte[] data;
            using (var outstream = new MemoryStream())
            {
                using (var instream = new MemoryStream(obj))
                {
                    using var zip = new GZipStream(instream, CompressionMode.Decompress);
                    zip.CopyTo(outstream);
                }
                data = outstream.ToArray();
            }
            return data;
        }

        private static byte[] Flattened(byte[] v)
        {
            byte[] data;
            using (var outstream = new MemoryStream())
            {
                using (var zip = new GZipStream(outstream, CompressionMode.Compress))
                {
                    zip.Write(v, 0, v.Length);
                }
                data = outstream.ToArray();
            }
            return data;
        }
    }
}
