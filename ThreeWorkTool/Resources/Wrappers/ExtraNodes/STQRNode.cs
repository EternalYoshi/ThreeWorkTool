using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Utility;
using ThreeWorkTool.Resources.Archives;

namespace ThreeWorkTool.Resources.Wrappers.ExtraNodes
{
    public class STQRNode : DefaultWrapper
    {
        public int index;
        public int FileNameOffset;
        public int FileSize;
        public int Duration;
        public int Channels;
        public int SampleRate;
        public int LoopStart;
        public int LoopEnd;
        public string EntryTypeHash;
        public string FilePath;
        public int UnknownDataA;
        public int UnknownDataB;
        public byte[] UnknownDataC;


        [Category("STQR"), ReadOnlyAttribute(true)]
        public int Index
        {

            get
            {
                return index;
            }
            set
            {
                index = value;
            }
        }

        [Category("STQR"), ReadOnlyAttribute(false)]
        public int AudioDuration
        {

            get
            {
                return Duration;
            }
            set
            {
                Duration = value;
            }
        }

        [Category("STQR"), ReadOnlyAttribute(false)]
        public int ChannelCount
        {

            get
            {
                return Channels;
            }
            set
            {
                Channels = value;
            }
        }

        [Category("STQR"), ReadOnlyAttribute(false)]
        public int SamplingRate
        {

            get
            {
                return SampleRate;
            }
            set
            {
                SampleRate = value;
            }
        }

        [Category("STQR"), ReadOnlyAttribute(false)]
        public int StartOfLoop
        {

            get
            {
                return LoopStart;
            }
            set
            {
                LoopStart = value;
            }
        }

        [Category("STQR"), ReadOnlyAttribute(false)]
        public int EndOfLoop
        {

            get
            {
                return LoopEnd;
            }
            set
            {
                LoopEnd = value;
            }
        }

        [Category("STQR"), ReadOnlyAttribute(false)]
        public int ProjectedFileSize
        {

            get
            {
                return FileSize;
            }
            set
            {
                FileSize = value;
            }
        }

        [Category("STQR"), ReadOnlyAttribute(false)]
        public int ProjectedFileNamePos
        {

            get
            {
                return FileNameOffset;
            }
            set
            {
                FileNameOffset = value;
            }
        }

        [Category("STQR"), ReadOnlyAttribute(false)]
        public string SoundFileTypeHash
        {

            get
            {
                return EntryTypeHash;
            }
            set
            {
                EntryTypeHash = value;
            }
        }

        [Category("STQR"), ReadOnlyAttribute(false)]
        public string EntryFileName
        {

            get
            {
                return FilePath;
            }
            set
            {
                FilePath = value;
            }
        }

    }
}
