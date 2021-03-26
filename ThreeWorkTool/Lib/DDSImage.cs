using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;

//Original by andburn. Modified for my own use.
namespace DDSReader
{
    public class DDSImage
    {
        private readonly Pfim.IImage _image;

        public byte[] Data
        {
            get
            {
                if (_image != null)
                    return _image.Data;
                else
                    return new byte[0];
            }
        }

        public DDSImage(string file)
        {
            _image = Pfim.Pfim.FromFile(file);
            Process();
        }

        public DDSImage(Stream stream)
        {
            if (stream == null)
                throw new Exception("DDSImage ctor: Stream is null");

            _image = Pfim.Dds.Create(stream, new Pfim.PfimConfig());
            Process();
        }

        public DDSImage(byte[] data)
        {
            if (data == null || data.Length <= 0)
                throw new Exception("DDSImage ctor: no data");

            _image = Pfim.Dds.Create(data, new Pfim.PfimConfig());
            Process();
        }

        public void Save(string file)
        {
            if (_image.Format == Pfim.ImageFormat.Rgba32)
            {

                Image<Rgba32> image = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(_image.Data, _image.Width, _image.Height);


                image.Save(file);


            }
            else if (_image.Format == Pfim.ImageFormat.Rgb24)
            {

                Image<Rgb24> image = SixLabors.ImageSharp.Image.LoadPixelData<Rgb24>(_image.Data, _image.Width, _image.Height);


                image.Save(file);


            }
            else
                throw new Exception("Unsupported pixel format (" + _image.Format + ")");
        }


        public void ToBitmap(string sfile, byte[] ddsdata)
        {

            if (_image.Format == Pfim.ImageFormat.Rgba32)
            {

                Image<Rgba32> image = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(_image.Data, _image.Width, _image.Height);


                image.Save(sfile);


            }
            else if (_image.Format == Pfim.ImageFormat.Rgb24)
            {

                Image<Rgb24> image = SixLabors.ImageSharp.Image.LoadPixelData<Rgb24>(_image.Data, _image.Width, _image.Height);


                image.Save(sfile);


            }
            else
                throw new Exception("Unsupported pixel format (" + _image.Format + ")");
        }

        public byte[] ToPNGArray(byte[] ddsdata)
        {
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

        public void ToPNG(string ddsdata)
        {
            if (_image.Format == Pfim.ImageFormat.Rgba32)
            {
                Image<Rgba32> image = SixLabors.ImageSharp.Image.LoadPixelData<Rgba32>(_image.Data, _image.Width, _image.Height);
                MemoryStream stream = new MemoryStream();
                image.SaveAsPng(ddsdata);
                byte[] OutPng = stream.ToArray();

                //return OutPng;
            }
            else if (_image.Format == Pfim.ImageFormat.Rgb24)
            {
                Image<Rgb24> image = SixLabors.ImageSharp.Image.LoadPixelData<Rgb24>(_image.Data, _image.Width, _image.Height);
                MemoryStream stream = new MemoryStream();
                image.SaveAsPng(ddsdata);
                byte[] OutPng = stream.ToArray();

                //return OutPng;
            }
            else throw new Exception("Unsupported pixel format (" + _image.Format + ")");
        }

        private void Process()
        {
            if (_image == null)
                throw new Exception("DDSImage image creation failed");

            if (_image.Compressed)
                _image.Decompress();
        }

    }
}
