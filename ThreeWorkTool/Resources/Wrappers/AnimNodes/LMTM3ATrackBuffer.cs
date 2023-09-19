//From EddieLopesRJ. Thank you very much for this class file.
//Had to modify a few things to fix conflicts with custom Vector4 implementation & have Keyframes retain the bone ID for later.
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows;
using ThreeWorkTool.Resources.Utility;
using static ThreeWorkTool.Resources.Wrappers.LMTM3AEntry;

enum EBufferType
{
    singlevector3 = 1,
    singlerotationquat3 = 2,
    linearvector3 = 3,
    bilinearvector3_16bit = 4,
    bilinearvector3_8bit = 5,
    linearrotationquat4_14bit = 6,
    bilinearrotationquat4_7bit = 7,
    bilinearrotationquatxw_14bit = 11,
    bilinearrotationquatyw_14bit = 12,
    bilinearrotationquatzw_14bit = 13,
    bilinearrotationquat4_11bit = 14,
    bilinearrotationquat4_9bit = 15
}



/*
public class Vector4X
{
    public float[] data;
}
*/

public class Extremes
{
    public Vector4 min;
    public Vector4 max;
}

class BufferConversor
{
    public int bit_size;
    public int buffer_size;
    public int[] strides;
    public Func<BigInteger, float> convert;
    public Func<BigInteger, int> frames;
    public string format;


    public KeyFrame Process(BigInteger value, byte[] buffer, float[] extremes, int bufferType, string format, int BoneID, BinaryReader bnr, int TrackType, int FrameCount, int BufferSize)
    {
        var frame_value = 0;

        var data = new float[4];
        int pos = 0;
        float[] vecs = new float[4];
        string TransType = "";
        //BigInteger bigin = new BigInteger(bnr.ReadBytes(buffer_size));
        var bin_vec = new uint[strides.Length];
        var strshorts = new string[strides.Length];
        byte[] tempArr = new byte[buffer_size];
        tempArr = bnr.ReadBytes(buffer_size);

        switch (bufferType)
        {
            case 1:
                break;
            case 2:
                frame_value = tempArr[(buffer_size - 1)];
                break;
            case 3:
                break;
            case 4:
                frame_value = tempArr[(buffer_size - 2)];
                break;
            case 5:
                frame_value = tempArr[(buffer_size - 1)];
                break;
            case 6:
                frame_value = tempArr[(buffer_size - 1)];
                break;
            case 7:
                byte bt = tempArr[buffer_size - 1];
                frame_value = bt >> 4;
                break;
            default:
                break;
        }

        Array.Reverse(tempArr);
        string BigString = "";
        BigString = ByteUtilitarian.ByteArrayToString(tempArr);

        //Converts To String To get the raw Binary.
        string binarystring = String.Join(String.Empty, BigString.Select
            (
                c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
            )
        );
        string SmallString = "";
        //Separates the needed bits from the long as heck binary string.
        for (int j = 0; j < strides.Length; j++)
        {
            SmallString = binarystring.Substring((strides[j]), bit_size);
            strshorts[j] = SmallString;
            bin_vec[j] = Convert.ToUInt32(SmallString, 2);
        }

        int[] bin_vecTC = new int[bin_vec.Length];

        //The "Unpacking" Part.
        switch (bufferType)
        {
            case 1:
                break;
            case 2:
                for (int k = 0; k < bin_vec.Length; k++)
                {
                    bin_vecTC[k] = (Int16)(bin_vec[k] << 2) / 4;
                    vecs[k] = Convert.ToSingle((decimal)(bin_vecTC[k]) / (decimal)((1 << (bit_size - 2)) - 1));
                }
                data[0] = vecs[0];
                data[1] = vecs[1];
                data[2] = vecs[2];
                data[3] = 1.0f;
                break;
            case 3:
                for (int k = 0; k < bin_vec.Length; k++)
                {
                    vecs[k] = Convert.ToSingle((decimal)(bin_vec[k] - 8) / (decimal)((Math.Pow(2, bit_size)) - 16));
                }
                data[0] = vecs[0];
                data[1] = vecs[1];
                data[2] = vecs[2];
                data[3] = 1.0f;
                break;
            case 4:
                for (int k = 0; k < bin_vec.Length; k++)
                {
                    vecs[k] = Convert.ToSingle((decimal)(bin_vec[k] - 8) / (decimal)((Math.Pow(2, bit_size)) - 16));
                }
                data[0] = vecs[0];
                data[1] = vecs[1];
                data[2] = vecs[2];
                data[3] = 1.0f;
                break;
            case 5:
                for (int k = 0; k < bin_vec.Length; k++)
                {
                    vecs[k] = Convert.ToSingle((decimal)(bin_vec[k] - 8) / (decimal)((Math.Pow(2, bit_size)) - 16));
                }
                data[0] = vecs[0];
                data[1] = vecs[1];
                data[2] = vecs[2];
                data[3] = 1.0f;
                break;
            case 6:
                for (int k = 0; k < bin_vec.Length; k++)
                {
                    bin_vecTC[k] = (Int16)(bin_vec[k] << 2) / 4;
                    vecs[k] = Convert.ToSingle((decimal)(bin_vecTC[k]) / (decimal)((1 << (bit_size - 2)) - 1));
                }
                data[0] = vecs[0];
                data[1] = vecs[1];
                data[2] = vecs[2];
                data[3] = 1.0f;
                break;
            case 7:
                for (int k = 0; k < bin_vec.Length; k++)
                {
                    vecs[k] = Convert.ToSingle((decimal)(bin_vec[k] - 8) / (decimal)((Math.Pow(2, bit_size)) - 16));
                }
                data[0] = vecs[0];
                data[1] = vecs[1];
                data[2] = vecs[2];
                data[3] = 1.0f;
                break;
            default:
                MessageBox.Show("There's an exotic buffer type in here.");
                break;
        }

        if (extremes != null && !(extremes.All(o => o == 0)))
        {
            for (int i = 0; i < 4; i++)
            {
                data[i] = extremes[i + 4] + extremes[i] * data[i];
            }
        }

        var ETrackType = (ETrackType)TrackType;
        var EBufferType = (EBufferType)bufferType;

        return new KeyFrame()
        {
            data = new Vector4(data[0], data[1], data[2], data[3]),
            TempFrameValue = frame_value,
            BoneID = BoneID,
            TrackType = ETrackType.ToString(),
            Buffertype = EBufferType.ToString(),
            KeyType = TransType
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
    static public IEnumerable<KeyFrame> Convert(int bufferType, byte[] buffer, int BoneID, float[] extremes, BinaryReader bnr, int TrackType, int AnimIndex, int BufferSize)
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
                    frames = (val) => 1,
                    format = "float_format"
                };
                break;
            case 2: //singlerotationquat3
                conversor = new BufferConversor()
                {
                    buffer_size = 8,
                    bit_size = 14,
                    strides = new int[] { 8, 22, 36, 50 },
                    convert = BI2Signed(14),
                    frames = (val) => (int)((val >> 56) & ((1 << 8) - 1)),
                    format = "signed_format"
                };
                break;
            case 3: //linearvector3
                conversor = new BufferConversor()
                {
                    buffer_size = 16,
                    bit_size = 32,
                    strides = new int[] { 0, 32, 64, 96 },
                    convert = BI2Float,
                    frames = (val) => 1,
                    format = "float_format"
                };
                break;
            case 4: //bilinearvector3_16bit
                conversor = new BufferConversor()
                {
                    buffer_size = 8,
                    bit_size = 16,
                    strides = new int[] { 48, 32, 16 },
                    convert = BI2Unsigned(16),
                    frames = (val) => (int)((val >> 48) & ((1 << 16) - 1)),
                    format = "unsigned_format"
                };
                break;
            case 5: //bilinearvector3_8bit
                conversor = new BufferConversor()
                {
                    buffer_size = 4,
                    bit_size = 8,
                    strides = new int[] { 24, 16, 8 },
                    convert = BI2Unsigned(8),
                    frames = (val) => (int)((val >> 24) & ((1 << 8) - 1)),
                    format = "unsigned_format"
                };
                break;
            case 6: //linearrotationquat4_14bit
                conversor = new BufferConversor()
                {
                    buffer_size = 8,
                    bit_size = 14,
                    strides = new int[] { 8, 22, 36, 50 },
                    convert = BI2Signed(14),
                    frames = (val) => (int)((val >> 56) & ((1 << 8) - 1)),
                    format = "signed_format"
                };
                break;
            case 7: //bilinearrotationquat4_7bit
                conversor = new BufferConversor()
                {
                    buffer_size = 4,
                    bit_size = 7,
                    strides = new int[] { 4, 11, 18, 25 },
                    convert = BI2Unsigned2(7),
                    frames = (val) => (int)((val >> 28) & 0xf),
                    format = "unsigned_format"
                };
                break;
            default:
                MessageBox.Show("Unknnown or Bogus Buffertype detected! The Buffertype: " + bufferType + "\n was detected in AnimationID" + AnimIndex + "\nThis track's going to be skipped when it comes to keyframes.", "Unknown Buffer Exception");
                throw new Exception("Unknown Buffer Type");
        }

        int pos = 0;
        int frame = 0;
        while (pos != buffer.Length)
        {
            //Time to check the buffer that gets passed into the below function.
            var segment = new ArraySegment<byte>(buffer, pos, conversor.buffer_size);
            var buffer_value = new BigInteger(segment.Array);
            var buffer_segment = new byte[conversor.buffer_size];
            Array.Copy(segment.Array, pos, buffer_segment, 0, buffer_segment.Length);
            yield return conversor.Process(buffer_value, buffer, extremes, bufferType, conversor.format, BoneID, bnr, TrackType, frame, BufferSize);
            pos += conversor.buffer_size;
        }

        //yield return new KeyFrame();
    }




}
