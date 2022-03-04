//From EddieLopesRJ. Thank you very much for this class file.
//Had to modify a few things to fix conflicts with custom Vector4 implementation & have Keyframes retain the bone ID for later.
using System;
using System.Collections.Generic;
using System.Numerics;
using static ThreeWorkTool.Resources.Wrappers.LMTM3AEntry;

enum BufferType
{
    singlevector3 = 1,
    singlerotationquat3,
    linearvector3,
    bilinearvector3_16bit,
    bilinearvector3_8bit,
    linearrotationquat4_14bit,
    bilinearrotationquat4_7bit
}

public class Vector4X
{
    public float[] data;
}

class Extremes
{
    public Vector4X min;
    public Vector4X max;
}

class BufferConversor
{
    public int bit_size;
    public int buffer_size;
    public int[] strides;
    public Func<BigInteger, float> convert;
    public Func<BigInteger, int> frames;

    //int bit_mask;

    public KeyFrame Process(BigInteger value, float[] extremes, int Boneid)
    {
        var frame_value = frames(value);

        Vector4X data = new Vector4X()
        {
            data = new float[4]
        };

        int pos = 0;
        foreach (var stride in strides)
        {
            var curr_value = (value >> stride) & ((1 << bit_size) - 1);
            Console.Write($"{curr_value} ");
            data.data[pos++] = convert(curr_value);
        }
        Console.WriteLine();

        if (extremes != null)
        {
            for (int i = 0; i < 4; i++)
            {
                data.data[i] = extremes[i + 4] + extremes[i] * data.data[i];
            }
        }

        return new KeyFrame()
        {
            data = data,
            frame = frame_value,
            BoneID = Boneid
        };
    }
}
/*
public class KeyFrame
{
    public Vector4X data;
    public int frame;
}
*/
class LMTM3ATrackBuffer
{
    static public IEnumerable<KeyFrame> Convert(int bufferType, byte[] buffer, float[] extremes, int BoneID)
    {
        BufferConversor conversor;

        Func<BigInteger, float> BI2Float = (bi) => { return System.BitConverter.ToSingle(BitConverter.GetBytes((int)bi), 0); };

        Func<int, Func<BigInteger, float>> BI2Unsigned = (bit_size) => (bi) => ((float)bi) / ((float)((1 << bit_size) - 1));

        Func<int, Func<BigInteger, float>> BI2Signed = (bit_size) => (bi) =>
              (int)(bi < (1 << 13) ? bi : (bi - (1 << 14))) / ((float)((1 << (12)) - 1));

        Func<int, Func<BigInteger, float>> BI2Unsigned2 = (bit_size) => (bi) =>
              ((uint)bi) / (float)((1 << 7) - 1);

        switch (bufferType)
        {
            case 1: //singlevector3
                conversor = new BufferConversor()
                {
                    buffer_size = 12,
                    bit_size = 32,
                    strides = new int[] { 0, 32, 64 },
                    convert = BI2Float,
                    frames = (val) => 1
                };
                break;
            case 2: //singlerotationquat3
                conversor = new BufferConversor()
                {
                    buffer_size = 12,
                    bit_size = 32,
                    strides = new int[] { 0, 32, 64 },
                    convert = BI2Float,
                    frames = (val) => 1
                };
                break;
            case 3: //linearvector3
                conversor = new BufferConversor()
                {
                    buffer_size = 16,
                    bit_size = 32,
                    strides = new int[] { 0, 32, 64, 96 },
                    convert = BI2Float,
                    frames = (val) => 1
                };
                break;
            case 4: //bilinearvector3_16bit
                conversor = new BufferConversor()
                {
                    buffer_size = 8,
                    bit_size = 16,
                    strides = new int[] { 0, 16, 32 },
                    convert = BI2Unsigned(16),
                    frames = (val) => (int)((val >> 48) & ((1 << 16) - 1))
                };
                break;
            case 5: //bilinearvector3_8bit
                conversor = new BufferConversor()
                {
                    buffer_size = 4,
                    bit_size = 8,
                    strides = new int[] { 0, 8, 16 },
                    convert = BI2Unsigned(8),
                    frames = (val) => (int)((val >> 24) & ((1 << 8) - 1))
                };
                break;
            case 6: //linearrotationquat4_14bit
                conversor = new BufferConversor()
                {
                    buffer_size = 8,
                    bit_size = 14,
                    strides = new int[] { 42, 28, 14, 0, },
                    convert = BI2Signed(14),
                    frames = (val) => (int)((val >> 56) & ((1 << 8) - 1))
                };
                break;
            case 7: //bilinearrotationquat4_7bit
                conversor = new BufferConversor()
                {
                    buffer_size = 4,
                    bit_size = 7,
                    strides = new int[] { 21, 14, 7, 0 },
                    convert = BI2Unsigned2(7),
                    frames = (val) => (int)((val >> 28) & 0xf)
                };
                break;
            default:
                throw new Exception("Unknown Buffer Type");
        }

        int pos = 0;

        while (pos != buffer.Length)
        {
            var segment = new ArraySegment<byte>(buffer, pos, conversor.buffer_size);
            var buffer_value = new BigInteger(segment.Array);
            yield return conversor.Process(buffer_value, extremes, BoneID);

            pos += conversor.buffer_size;
        }

        //yield return new KeyFrame();
    }
}