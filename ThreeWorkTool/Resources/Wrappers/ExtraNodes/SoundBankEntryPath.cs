using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeWorkTool.Resources.Wrappers.ExtraNodes
{
    public class SoundBankEntryPath
    {
        public int index;
        public long Offset;
        public string SoundFilePath { get; set; }
        public string TypeHash { get; set; }
        public List<byte> OtherData;
        public const int OTHERDATASIZE = 0x4C;
        public int OriginalStringLength;
    }
}
