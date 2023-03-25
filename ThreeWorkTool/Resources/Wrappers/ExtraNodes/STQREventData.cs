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
    public class STQREventData : DefaultWrapper
    {

        static int ENTRYSIZE = 0x98;
        public int index;
        public short EventEntryID;
        public short UnknownValue02;
        public int UnknownValue04;
        public int UnknownValue08;
        public int UnknownValue0C;
        public sbyte UnknownValue10;
        public sbyte UnknownValue11;
        public short UnknownValue12;
        public short UnknownValue14;
        public short UnknownValue16;
        public sbyte UnknownValue18;
        public sbyte UnknownValue19;
        public sbyte UnknownValue1A;
        public sbyte UnknownValue1B;
        public sbyte UnknownValue1C;
        public short UnknownValue1D;
        public sbyte UnknownValue1F;
        public short UnknownValue20;
        public short UnknownValue22;
        public float UnknownValue24;
        public float UnknownValue28;
        public int UnknownValue2C;
        public int UnknownValue30;
        public short UnknownValue34;
        public short UnknownValue36;
        public int UnknownValue38;
        public int UnknownValue3C;
        public int UnknownValue40;
        public int UnknownValue44;
        public int UnknownValue48;
        public int UnknownValue4C;
        public int UnknownValue50;
        public int UnknownValue54;
        public int UnknownValue58;
        public int PossibleEntryID;
        public int UnknownValue60;
        public float UnknownValue64;
        public int UnknownValue68;
        public int UnknownValue6C;
        public short UnknownValue70;
        public short UnknownValue72;
        public short UnknownValue74;
        public short UnknownValue76;
        public int UnknownValue78;
        public int UnknownValue7C;
        public int UnknownValue80;
        public int UnknownValue84;
        public int UnknownValue88;
        public int UnknownValue8C;
        public int UnknownValue90;
        public int UnknownValue94;

        #region STQREvent
        [Category("STQR Event Data"), ReadOnlyAttribute(true)]
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

        [Category("STQR Event Data"), ReadOnlyAttribute(true)]
        public short ID
        {

            get
            {
                return EventEntryID;
            }
            set
            {
                EventEntryID = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public short SomeValue02
        {

            get
            {
                return UnknownValue02;
            }
            set
            {
                UnknownValue02 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue04
        {

            get
            {
                return UnknownValue04;
            }
            set
            {
                UnknownValue04 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue08
        {

            get
            {
                return UnknownValue08;
            }
            set
            {
                UnknownValue08 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue0C
        {

            get
            {
                return UnknownValue0C;
            }
            set
            {
                UnknownValue0C = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public sbyte SomeValue10
        {

            get
            {
                return UnknownValue10;
            }
            set
            {
                UnknownValue10 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public sbyte SomeValue11
        {

            get
            {
                return UnknownValue11;
            }
            set
            {
                UnknownValue11 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public short SomeValue12
        {

            get
            {
                return UnknownValue12;
            }
            set
            {
                UnknownValue12 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public short SomeValue14
        {

            get
            {
                return UnknownValue14;
            }
            set
            {
                UnknownValue14 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public short SomeValue16
        {

            get
            {
                return UnknownValue16;
            }
            set
            {
                UnknownValue16 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public sbyte SomeValue18
        {

            get
            {
                return UnknownValue18;
            }
            set
            {
                UnknownValue18 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public sbyte SomeValue19
        {

            get
            {
                return UnknownValue19;
            }
            set
            {
                UnknownValue19 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public sbyte SomeValue1A
        {

            get
            {
                return UnknownValue1A;
            }
            set
            {
                UnknownValue1A = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public sbyte SomeValue1B
        {

            get
            {
                return UnknownValue1B;
            }
            set
            {
                UnknownValue1B = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public sbyte SomeValue1C
        {

            get
            {
                return UnknownValue1C;
            }
            set
            {
                UnknownValue1C = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public short SomeValue1D
        {

            get
            {
                return UnknownValue1D;
            }
            set
            {
                UnknownValue1D = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public sbyte SomeValue1F
        {

            get
            {
                return UnknownValue1F;
            }
            set
            {
                UnknownValue1F = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public short SomeValue20
        {

            get
            {
                return UnknownValue20;
            }
            set
            {
                UnknownValue20 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public short SomeValue22
        {

            get
            {
                return UnknownValue22;
            }
            set
            {
                UnknownValue22 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public float SomeValue24
        {

            get
            {
                return UnknownValue24;
            }
            set
            {
                UnknownValue24 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public float SomeValue28
        {

            get
            {
                return UnknownValue28;
            }
            set
            {
                UnknownValue28 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue2C
        {

            get
            {
                return UnknownValue2C;
            }
            set
            {
                UnknownValue2C = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue30
        {

            get
            {
                return UnknownValue30;
            }
            set
            {
                UnknownValue30 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public short SomeValue34
        {

            get
            {
                return UnknownValue34;
            }
            set
            {
                UnknownValue34 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public short SomeValue36
        {

            get
            {
                return UnknownValue36;
            }
            set
            {
                UnknownValue36 = value;
            }
        }


        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue38
        {

            get
            {
                return UnknownValue38;
            }
            set
            {
                UnknownValue38 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue3C
        {

            get
            {
                return UnknownValue3C;
            }
            set
            {
                UnknownValue3C = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue40
        {

            get
            {
                return UnknownValue40;
            }
            set
            {
                UnknownValue40 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue44
        {

            get
            {
                return UnknownValue44;
            }
            set
            {
                UnknownValue44 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue48
        {

            get
            {
                return UnknownValue48;
            }
            set
            {
                UnknownValue48 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue4C
        {

            get
            {
                return UnknownValue4C;
            }
            set
            {
                UnknownValue4C = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue50
        {

            get
            {
                return UnknownValue50;
            }
            set
            {
                UnknownValue50 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue54
        {

            get
            {
                return UnknownValue54;
            }
            set
            {
                UnknownValue54 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue58
        {

            get
            {
                return UnknownValue58;
            }
            set
            {
                UnknownValue58 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int MaybeCorrespondsWithEntryID
        {

            get
            {
                return PossibleEntryID;
            }
            set
            {
                PossibleEntryID = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue60
        {

            get
            {
                return UnknownValue60;
            }
            set
            {
                UnknownValue60 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public float SomeValue64
        {

            get
            {
                return UnknownValue64;
            }
            set
            {
                UnknownValue64 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue68
        {

            get
            {
                return UnknownValue68;
            }
            set
            {
                UnknownValue68 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue6C
        {

            get
            {
                return UnknownValue6C;
            }
            set
            {
                UnknownValue6C = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public short SomeValue70
        {

            get
            {
                return UnknownValue70;
            }
            set
            {
                UnknownValue70 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public short SomeValue72
        {

            get
            {
                return UnknownValue72;
            }
            set
            {
                UnknownValue72 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public short SomeValue74
        {

            get
            {
                return UnknownValue74;
            }
            set
            {
                UnknownValue74 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public short SomeValue76
        {

            get
            {
                return UnknownValue76;
            }
            set
            {
                UnknownValue76 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue78
        {

            get
            {
                return UnknownValue78;
            }
            set
            {
                UnknownValue78 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue7C
        {

            get
            {
                return UnknownValue7C;
            }
            set
            {
                UnknownValue7C = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue80
        {

            get
            {
                return UnknownValue80;
            }
            set
            {
                UnknownValue80 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue84
        {

            get
            {
                return UnknownValue84;
            }
            set
            {
                UnknownValue84 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue88
        {

            get
            {
                return UnknownValue88;
            }
            set
            {
                UnknownValue88 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue8C
        {

            get
            {
                return UnknownValue8C;
            }
            set
            {
                UnknownValue8C = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue90
        {

            get
            {
                return UnknownValue90;
            }
            set
            {
                UnknownValue90 = value;
            }
        }

        [Category("STQR Event Data"), ReadOnlyAttribute(false)]
        public int SomeValue94
        {

            get
            {
                return UnknownValue94;
            }
            set
            {
                UnknownValue94 = value;
            }
        }

        #endregion

    }
}
