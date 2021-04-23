using Ionic.Zlib;
using System.IO;

namespace ThreeWorkTool.Resources
{
    class Zlibber
    {
        //This is the closest match to the compression method the game uses.
        public static byte[] Compressor(byte[] decombuffer)
        {
            byte[] CompressedData;
            using (MemoryStream ms = new MemoryStream(decombuffer))
            {
                using (var raws = new MemoryStream())
                {
                    using (Stream compressor = new ZlibStream(raws, Ionic.Zlib.CompressionMode.Compress))
                    {
                        byte[] buffer = new byte[4096];
                        int n;
                        while ((n = ms.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            compressor.Write(buffer, 0, n);
                        }

                    }
                    CompressedData = raws.ToArray();
                    return CompressedData;
                }
            }

        }
    }
}
