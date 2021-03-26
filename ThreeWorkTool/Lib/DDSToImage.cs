using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;

namespace DDSReader
{
    public class DDSToImage
    {

        public byte[] Data;
        public string TargetOutput;
        public Pfim.IImage _image;


        public byte[] BuildDDS(byte[] filedata)
        {
            _image = Pfim.Dds.Create(filedata, new Pfim.PfimConfig());

            if (_image.Format == Pfim.ImageFormat.Rgba32)
            {
                Image<Rgba32> image = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(_image.Data, _image.Width, _image.Height);
                MemoryStream stream = new MemoryStream();
                image.SaveAsPng(stream);
                byte[] OutPng = stream.ToArray();

                return OutPng;
            }
            else if (_image.Format == Pfim.ImageFormat.Rgb24)
            {
                Image<Rgb24> image = SixLabors.ImageSharp.Image.LoadPixelData<Rgb24>(_image.Data, _image.Width, _image.Height);
                MemoryStream stream = new MemoryStream();
                image.SaveAsPng(stream);
                byte[] OutPng = stream.ToArray();

                return OutPng;
            }
            else throw new Exception("Unsupported pixel format (" + _image.Format + ")");
        }


    }
}
