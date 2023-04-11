using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Kaitai;
using ThreeWorkTool.Resources.Wrappers.ExtraNodes.Kaitai;
using static Kaitai.Lmt.Track;
using static Kaitai.Lmt;
using System.Diagnostics;

namespace ThreeWorkTool.Resources.Wrappers.ExtraNodes.Kaitai
{
    public static class LmtCodec
    {
        
                public static Vector4 Vec3to4(Vector3 d)
                {
                    Vector4 Vex4 = new Vector4();
                    Vex4.X = d.X;
                    Vex4.Y = d.Y;
                    Vex4.Z = d.Z;
                    Vex4.W = 1;
                    return Vex4;
                }

                public static Vector4 Vec3to4Q(Vector3 d)
                {

                    Vector4 Vex4 = new Vector4();
                    Vex4.X = d.X;
                    Vex4.Y = d.Y;
                    Vex4.Z = d.Z;
                    Vex4.W = Convert.ToSingle(Math.Sqrt((1 - Math.Pow(d.X, 2) - Math.Pow(d.Y, 2) - Math.Pow(d.Z, 2))));
                    return Vex4;

                }

                public static Vector3 Vec4to3(Vector4 d)
                {
                    Vector3 Vex3 = new Vector3();
                    Vex3.X = d.X;
                    Vex3.X = d.Y;
                    Vex3.X = d.Z;
                    return Vex3;
                }

        /*
                public static object Unpack_bytes(object b)
                {
                    return b.@int / ((1 << b.Count - 2) - 1);
                }

                public static object Pack_bytes(int bitcount, float f)
                {
                    int bitmask2 = Convert.ToInt32(Math.Pow(2, bitcount) - 1);
                    int bitmask3 = Convert.ToInt32(Math.Pow(2, bitcount - 2) - 1);
                    int r = Convert.ToInt32(f * bitmask3) + Math.Pow(2, bitcount) & bitmask2;
                    return bitstring.Bits(Convert.ToUInt32(r), length: bitcount);
                }



                public static object Id(object d)
                {
                    return d;
                }

                public static object Buffer2bits(object d)
                {
                    return bitstring.Bits(bytes: d);
                }

                public static object Buffer2bits2(object d)
                {
                    return bitstring.Bits(bytes: d[: -1:]);
                }

                public static object Xwto4(Vector3 d)
                {
                    Vector4 Vex4 = new Vector4(d.X, 0, 0, d.Y);
                    return Vex4;

                }

                public static object Ywto4(Vector3 d)
                {
                    Vector4 Vex4 = new Vector4(0, d.X, 0, d.Y);
                    return Vex4;
                }

                public static object Zwto4(Vector3 d)
                {
                    Vector4 Vex4 = new Vector4(0, 0, d.X, d.Y);
                    return Vex4;
                }

                public static object Constant1(object b)
                {
                    return 1;
                }

                public static object Noop_frame(object b, object frame)
                {
                    return b;
                }

                public static object Get_frame(object start, object len)
                {
                    return b => b[start::(start + len)].uint;
                }

                public static object Put_frame(object start, object len)
                {
                    return (b, frame) => b[0::start] + bitstring.Bits(uint: frame, length: len) + b[start + len];
                }

                public static object Float_format = new Dictionary<object, object> {
                {
                    "unpack",
                    b => b.floatle},
                {
                    "pack",
                    (bitcount,f) => bitstring.Bits(floatle: f, length: bitcount)}};

                public static object Unsigned_format = new Dictionary<object, object> {
                {
                    "unpack",
                    b => (b.uint - 8) / (Math.Pow(2, b.Count) - 16)},
                {
                    "pack",
                    (bitcount,f) => bitstring.Bits(uint: uint(f * (Math.Pow(2, bitcount) - 16) + 8), length: bitcount)}};

                public static object Signed_format = new Dictionary<object, object> {
                {
                    "unpack",
                    unpack_bytes},
                {
                    "pack",
                    pack_bytes}};

                public static object Basic_preprocess = new Dictionary<object, object> {
                {
                    "unpack",
                    buffer2bits},
                {
                    "pack",
                    b => b.bytes}};

                public static object Invert_preprocess = new Dictionary<object, object> {
                {
                    "unpack",
                    buffer2bits2},
                {
                    "pack",
                    b => b.bytes[: - 1:]}};

                public static object Post_process_vec3 = new Dictionary<object, object> {
                {
                    "unpack",
                    vec3to4},
                {
                    "pack",
                    vec4to3}};

                public static object Post_process_vec3Q = new Dictionary<object, object> {
                {
                    "unpack",
                    vec3to4Q},
                {
                    "pack",
                    vec4to3}};

                public static object No_post_process = new Dictionary<object, object> {
                {
                    "unpack",
                    id},
                {
                    "pack",
                    id}};

                public static object No_frame = new Dictionary<object, object> {
                {
                    "unpack",
                    constant1},
                {
                    "pack",
                    noop_frame}};

                public static object Has_frame(object start, object end)
                {
                    return new Dictionary<object, object> {
                    {
                        "unpack",
                        get_frame(start, end)},
                    {
                        "pack",
                        put_frame(start, end)}};
                }

                public static object Constants = new Dictionary<object, object> {
                {
                    Lmt.Track.Compression.Singlevector3,
                    new Dictionary<object, object>
                    {
                        {
                            "buffer_size",12},
                        {
                            "bit_size",32},
                        {
                            "strides",
                            new List<int> {
                                0,32,64}},
                        {
                            "format",Float_format},
                        {
                            "frames",No_frame},
                        {
                            "preprocess",Basic_preprocess},
                        {
                            "postprocess",Post_process_vec3}}},
                {
                    Lmt.Track.Compression.Singlerotationquat3,
                    new Dictionary<object, object>
                    {
                        {
                            "buffer_size",12},
                        {
                            "bit_size",32},
                        {
                            "strides",
                            new List<int> {0,32,64}},
                        {
                            "format",Float_format},
                        {
                            "frames",No_frame},
                        {
                            "preprocess",Basic_preprocess},
                        {
                            "postprocess",Post_process_vec3Q}}},
                {
                    Lmt.Track.Compression.Linearvector3,
                    new Dictionary<object, object>
                    {
                        {
                            "buffer_size",16},
                        {
                            "bit_size",32},
                        {
                            "strides", new List<int> {0,32,64,96}},
                        {
                            "format",Float_format},
                        {
                            "frames",No_frame},
                        {
                            "preprocess",Basic_preprocess},
                        {
                            "postprocess",No_post_process}}},
                {
                    Lmt.Track.Compression.Bilinearvector3_16bit,
                    new Dictionary<object, object>
                    {
                        {
                            "buffer_size",8},
                        {
                            "bit_size",16},
                        {
                            "strides", new List<int> {48,32,16}},
                        {
                            "format",Unsigned_format},
                        {
                            "frames",Has_frame(0, 16)},
                        {
                            "preprocess",Invert_preprocess},
                        {
                            "postprocess",Post_process_vec3}}},
                {
                    Lmt.Track.Compression.Bilinearvector3_8bit,
                    new Dictionary<object, object>
                    {
                        {
                            "buffer_size",4},
                        {
                            "bit_size",8},
                        {
                            "strides",new List<int> {0,8,16}},
                        {
                            "format",Unsigned_format},
                        {
                            "frames",Has_frame(24, 8)},
                        {
                            "preprocess",Basic_preprocess},
                        {
                            "postprocess",Post_process_vec3}}},
                {
                    Lmt.Track.Compression.Linearrotationquat4_14bit,
                    new Dictionary<object, object>
                    {
                        {
                            "buffer_size",8},
                        {
                            "bit_size",14},
                        {
                            "strides",new List<int> {8,22,36,50}},
                        {
                            "format",Signed_format},
                        {
                            "frames",Has_frame(0, 8)},
                        {
                            "preprocess",Invert_preprocess},
                        {
                            "postprocess",No_post_process}}},
                {
                    Lmt.Track.Compression.Bilinearrotationquat4_7bit,
                    new Dictionary<object, object>
                    {
                        {
                            "buffer_size",4},
                        {
                            "bit_size",7},
                        {
                            "strides",new List<int> {4,11,18,25}},
                        {
                            "format",Unsigned_format},
                        {
                            "frames",Has_frame(0, 4)},
                        {
                            "preprocess",Invert_preprocess},
                        {
                            "postprocess",No_post_process}}}};

                public static IDictionary<string, object> NewConstant = new Dictionary<string, object>()
                {
                    Compression.Singlevector3,
                    {
                        {
                            "buffer_size",12},
                        {
                            "bit_size",32},
                        {
                            "strides",
                            new List<int> {
                                0,32,64}},
                        {
                            "format",Float_format},
                        {
                            "frames",No_frame},
                        {
                            "preprocess",Basic_preprocess},
                        {
                            "postprocess",Post_process_vec3}}},
                    {


                }

        public static Tuple<List<byte>, int> Process(object compressor, List<byte> buffer, Extreme extremes)
        {
            var strides = compressor["strides"];
            var bit_size = compressor["bit_size"];
            var preprocess = compressor["preprocess"];
            var format = compressor["format"];
            var postprocess = compressor["postprocess"];
            var frames_process = compressor["frames"];
            Debug.Assert(compressor["buffer_size"] == buffer.Count);
            var c2 = preprocess["unpack"](buffer);
            var bin_vec = (from s in strides
                           select c2[s::(s + bit_size)]).ToList();
            var vec = (from bv in bin_vec
                       select format["unpack"](bv)).ToList();
            //for bv,v in zip(bin_vec,vec):
            //    print(bv, v, end=' ')
            //print()
            var result = postprocess["unpack"](vec);
            var frames = frames_process["unpack"](c2);
            //### assert ####

            //### assert ####
            //print(bin_vec, end=' ')
            //print(result, end=' ')
            if (!(extremes == null))
            {
                result = np.add(extremes.max, np.multiply(extremes.min, result));
            }
            //print(result)
            return Tuple.Create(result, frames);
        }

        public static List<Tuple<List<byte>, int>> Process_Buffer(Compression codec, List<byte> buffer, Extreme extremes)
        {
            var compressor = Constants[Codec];
            int size = compressor[buffer.Count];
            if (!(buffer == null) && buffer.Count > 0)
            {
                return (from s in range(0, buffer.Count, size)
                        select Process(compressor, buffer[s::(s + size)], extremes)).ToList();
            }
            else
            {
                return null;
            }
        }

        public static object Generate(object compressor, object value, object frame, Extreme extremes)
        {
            var preprocess = compressor["preprocess"];
            var postprocess = compressor["postprocess"];
            var strides = compressor["strides"];
            var bit_size = compressor["bit_size"];
            var frames_process = compressor["frames"];
            var format = compressor["format"];
            //print('value', value)
            var unvec = postprocess["pack"](value);
            var temp = bitstring.Bits(length: compressor["buffer_size"] * 8);
            //print('unvec',[b for b in unvec])
            var temp_vec = (from b in unvec
                            select format["pack"](bit_size, b)).ToList();
            foreach (var _tup_1 in zip(strides, temp_vec))
            {
                var s = _tup_1.Item1;
                var v = _tup_1.Item2;
                temp = temp | bitstring.Bits(length: s) + v + bitstring.Bits(length: temp.Count - s - bit_size);
            }
            temp = frames_process["pack"](temp, frame);
            var unc = preprocess["pack"](temp);
            unc = bitstring.Bits(unc);
            //assert(unc==bitstring.Bits(bytes(buffer)))
            return unc;
        }

        public static object Generate_Buffer(Compression codec, object values, object frames)
        {
            //print('codec', codec)
            var compressor = constants[codec];
            object extremes = null;
            if (codec == Lmt.Track.Compression.bilinearrotationquat4_7bit || codec == Lmt.Track.Compression.bilinearvector3_8bit || codec == Lmt.Track.Compression.bilinearvector3_16bit)
            {
                extremes = Extreme();
                extremes.max = np.min(values, axis: 0);
                extremes.min = np.max(np.subtract(values, extremes.max), axis: 0);
                //print('extremes.min',extremes.min)
                //print('extremes.max',extremes.max)
                values = (from value in values
                          select (from _tup_1 in zip(np.subtract(value, extremes.max), extremes.min).Chop((a, b) => Tuple.Create(a, b))
                                  let a = _tup_1.Item1
                                  let b = _tup_1.Item2
                                  select safe_divide(a, b)).ToList()).ToList();
                //print(values)
            }
            Func<object, object, object> safe_divide = (a, b) =>
            {
                if (b == 0.0)
                {
                    return a;
                }
                else
                {
                    return a / b;
                }
            };
            var buffer = "".join((from _tup_2 in zip(values, frames).Chop((value, frame) => Tuple.Create(value, frame))
                                  let value = _tup_2.Item1
                                  let frame = _tup_2.Item2
                                  select generate(compressor, value, frame, extremes).bytes).ToList()).ToList();
            if (buffer.Count == 0)
            {
                buffer = null;
            }
            return Tuple.Create(buffer, extremes);
        }

        public static object Codec = new List<object>
                {
                Lmt.Track.Compression.Singlevector3,
                Lmt.Track.Compression.Singlerotationquat3,
                Lmt.Track.Compression.Linearvector3,
                Lmt.Track.Compression.Bilinearvector3_16bit,
                Lmt.Track.Compression.Bilinearvector3_8bit,
                Lmt.Track.Compression.Linearrotationquat4_14bit,
                Lmt.Track.Compression.Bilinearrotationquat4_7bit,
                Lmt.Track.Compression.Bilinearrotationquatxw_14bit,
                Lmt.Track.Compression.Bilinearrotationquatyw_14bit,
                Lmt.Track.Compression.Bilinearrotationquatzw_14bit,
                Lmt.Track.Compression.Bilinearrotationquat4_11bit,
                Lmt.Track.Compression.Bilinearrotationquat4_9bit
                };
                */

    }

}
