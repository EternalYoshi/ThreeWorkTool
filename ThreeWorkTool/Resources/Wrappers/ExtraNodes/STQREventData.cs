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
        public int EventEntryID;
        public int UnknownValue04;
        public int UnknownValue08;
        public int UnknownValue0C;
        public int UnknownValue10;
        public int UnknownValue14;
        public int UnknownValue18;
        public int UnknownValue1C;
        public int UnknownValue20;
        public int UnknownValue24;
        public int UnknownValue28;
        public int UnknownValue2C;
        public int UnknownValue30;
        public int UnknownValue34;
        public int UnknownValue38;
        public int UnknownValue3C;
        public int UnknownValue40;
        public int UnknownValue44;
        public int UnknownValue48;
        public int UnknownValue4C;
        public int UnknownValue50;
        public int UnknownValue54;
        public int UnknownValue58;
        public int UnknownValue5C;
        public int UnknownValue60;
        public int UnknownValue64;
        public int UnknownValue68;
        public int UnknownValue6C;
        public int UnknownValue70;
        public int UnknownValue74;
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
        public int ID
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
        public int SomeValue10
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
        public int SomeValue14
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
        public int SomeValue18
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
        public int SomeValue1C
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
        public int SomeValue20
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
        public int SomeValue24
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
        public int SomeValue28
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
        public int SomeValue34
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
        public int SomeValue5C
        {

            get
            {
                return UnknownValue5C;
            }
            set
            {
                UnknownValue5C = value;
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
        public int SomeValue64
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
        public int SomeValue70
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
        public int SomeValue74
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
