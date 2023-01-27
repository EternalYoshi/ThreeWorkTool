// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

using System.Collections.Generic;

namespace Kaitai
{
    public partial class Lmt : KaitaiStruct
    {
        public static Lmt FromFile(string fileName)
        {
            return new Lmt(new KaitaiStream(fileName));
        }

        public Lmt(KaitaiStream p__io, KaitaiStruct p__parent = null, Lmt p__root = null) : base(p__io)
        {
            m_parent = p__parent;
            m_root = p__root ?? this;
            _read();
        }
        private void _read()
        {
            _magic = m_io.ReadBytes(4);
            if (!((KaitaiStream.ByteArrayCompare(Magic, new byte[] { 76, 77, 84, 0 }) == 0)))
            {
                throw new ValidationNotEqualError(new byte[] { 76, 77, 84, 0 }, Magic, M_Io, "/seq/0");
            }
            _version = m_io.ReadU2le();
            _entrycount = m_io.ReadU2le();
            _entries = new List<Motion>((int) (Entrycount));
            for (var i = 0; i < Entrycount; i++)
            {
                _entries.Add(new Motion(m_io, this, m_root));
            }
        }
        public partial class Event : KaitaiStruct
        {
            public static Event FromFile(string fileName)
            {
                return new Event(new KaitaiStream(fileName));
            }

            public Event(KaitaiStream p__io, Lmt.Animevent p__parent = null, Lmt p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _runeventbit = m_io.ReadU4le();
                _numframes = m_io.ReadU4le();
            }
            private uint _runeventbit;
            private uint _numframes;
            private Lmt m_root;
            private Lmt.Animevent m_parent;
            public uint Runeventbit { get { return _runeventbit; } }
            public uint Numframes { get { return _numframes; } }
            public Lmt M_Root { get { return m_root; } }
            public Lmt.Animevent M_Parent { get { return m_parent; } }
        }
        public partial class Track : KaitaiStruct
        {
            public static Track FromFile(string fileName)
            {
                return new Track(new KaitaiStream(fileName));
            }


            public enum Compression
            {
                Singlevector3 = 1,
                Singlerotationquat3 = 2,
                Linearvector3 = 3,
                Bilinearvector3_16bit = 4,
                Bilinearvector3_8bit = 5,
                Linearrotationquat4_14bit = 6,
                Bilinearrotationquat4_7bit = 7,
                Bilinearrotationquatxw_14bit = 11,
                Bilinearrotationquatyw_14bit = 12,
                Bilinearrotationquatzw_14bit = 13,
                Bilinearrotationquat4_11bit = 14,
                Bilinearrotationquat4_9bit = 15,
            }

            public enum Tracktype
            {
                Localrotation = 0,
                Localposition = 1,
                Localscale = 2,
                Absoluterotation = 3,
                Absoluteposition = 4,
                Xpto = 5,
            }
            public Track(KaitaiStream p__io, Lmt.Animentry p__parent = null, Lmt p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_buffer = false;
                f_extremes = false;
                _read();
            }
            private void _read()
            {
                _buffertype = ((Compression) m_io.ReadS1());
                _tracktype = ((Tracktype) m_io.ReadS1());
                _bonetype = m_io.ReadU1();
                _boneid = m_io.ReadU1();
                _weight = m_io.ReadF4le();
                _buffersize = m_io.ReadS8le();
                _bufferptr = m_io.ReadS8le();
                _referencedata = new List<float>((int) (4));
                for (var i = 0; i < 4; i++)
                {
                    _referencedata.Add(m_io.ReadF4le());
                }
                _extremesptr = m_io.ReadS8le();
            }
            private bool f_buffer;
            private List<byte> _buffer;
            public List<byte> Buffer
            {
                get
                {
                    if (f_buffer)
                        return _buffer;
                    if (Bufferptr != 0) {
                        KaitaiStream io = M_Root.M_Io;
                        long _pos = io.Pos;
                        io.Seek(Bufferptr);
                        _buffer = new List<byte>((int) (Buffersize));
                        for (var i = 0; i < Buffersize; i++)
                        {
                            _buffer.Add(io.ReadU1());
                        }
                        io.Seek(_pos);
                        f_buffer = true;
                    }
                    return _buffer;
                }
            }
            private bool f_extremes;
            private Extreme _extremes;
            public Extreme Extremes
            {
                get
                {
                    if (f_extremes)
                        return _extremes;
                    if (Extremesptr != 0) {
                        KaitaiStream io = M_Root.M_Io;
                        long _pos = io.Pos;
                        io.Seek(Extremesptr);
                        _extremes = new Extreme(io, this, m_root);
                        io.Seek(_pos);
                        f_extremes = true;
                    }
                    return _extremes;
                }
            }
            private Compression _buffertype;
            private Tracktype _tracktype;
            private byte _bonetype;
            private byte _boneid;
            private float _weight;
            private long _buffersize;
            private long _bufferptr;
            private List<float> _referencedata;
            private long _extremesptr;
            private Lmt m_root;
            private Lmt.Animentry m_parent;
            public Compression Buffertype { get { return _buffertype; } }
            //public Tracktype Tracktype { get { return _tracktype; } }
            public byte Bonetype { get { return _bonetype; } }
            public byte Boneid { get { return _boneid; } }
            public float Weight { get { return _weight; } }
            public long Buffersize { get { return _buffersize; } }
            public long Bufferptr { get { return _bufferptr; } }
            public List<float> Referencedata { get { return _referencedata; } }
            public long Extremesptr { get { return _extremesptr; } }
            public Lmt M_Root { get { return m_root; } }
            public Lmt.Animentry M_Parent { get { return m_parent; } }
        }
        public partial class Motion : KaitaiStruct
        {
            public static Motion FromFile(string fileName)
            {
                return new Motion(new KaitaiStream(fileName));
            }

            public Motion(KaitaiStream p__io, Lmt p__parent = null, Lmt p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_entry = false;
                _read();
            }
            private void _read()
            {
                _dataoffset = m_io.ReadU8le();
            }
            private bool f_entry;
            private Animentry _entry;
            public Animentry Entry
            {
                get
                {
                    if (f_entry)
                        return _entry;
                    if (Dataoffset != 0) {
                        KaitaiStream io = M_Root.M_Io;
                        long _pos = io.Pos;
                        io.Seek((long)Dataoffset);
                        _entry = new Animentry(io, this, m_root);
                        io.Seek(_pos);
                        f_entry = true;
                    }
                    return _entry;
                }
            }
            private ulong _dataoffset;
            private Lmt m_root;
            private Lmt m_parent;
            public ulong Dataoffset { get { return _dataoffset; } }
            public Lmt M_Root { get { return m_root; } }
            public Lmt M_Parent { get { return m_parent; } }
        }
        public partial class Animentry : KaitaiStruct
        {
            public static Animentry FromFile(string fileName)
            {
                return new Animentry(new KaitaiStream(fileName));
            }

            public Animentry(KaitaiStream p__io, Lmt.Motion p__parent = null, Lmt p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_tracklist = false;
                f_eventclasses = false;
                _read();
            }
            private void _read()
            {
                _trackptr = m_io.ReadU8le();
                _trackcount = m_io.ReadS4le();
                _numframes = m_io.ReadS4le();
                _loopframe = m_io.ReadS4le();
                _t1 = m_io.ReadS4le();
                if (!(T1 == 0))
                {
                    throw new ValidationNotEqualError(0, T1, M_Io, "/types/animentry/seq/4");
                }
                _t2 = m_io.ReadS4le();
                if (!(T2 == 0))
                {
                    throw new ValidationNotEqualError(0, T2, M_Io, "/types/animentry/seq/5");
                }
                _t3 = m_io.ReadS4le();
                if (!(T3 == 0))
                {
                    throw new ValidationNotEqualError(0, T3, M_Io, "/types/animentry/seq/6");
                }
                _endframeadditivesceneposition = new List<float>((int) (4));
                for (var i = 0; i < 4; i++)
                {
                    _endframeadditivesceneposition.Add(m_io.ReadF4le());
                }
                _endframeadditivescenerotation = new List<float>((int) (4));
                for (var i = 0; i < 4; i++)
                {
                    _endframeadditivescenerotation.Add(m_io.ReadF4le());
                }
                _flags = m_io.ReadS8le();
                _eventclassesptr = m_io.ReadS8le();
                _floattracksptr = m_io.ReadS8le();
            }
            private bool f_tracklist;
            private List<Track> _tracklist;
            public List<Track> Tracklist
            {
                get
                {
                    if (f_tracklist)
                        return _tracklist;
                    KaitaiStream io = M_Root.M_Io;
                    long _pos = io.Pos;
                    io.Seek((long)Trackptr);
                    _tracklist = new List<Track>((int) (Trackcount));
                    for (var i = 0; i < Trackcount; i++)
                    {
                        _tracklist.Add(new Track(io, this, m_root));
                    }
                    io.Seek(_pos);
                    f_tracklist = true;
                    return _tracklist;
                }
            }
            private bool f_eventclasses;
            private List<Animevent> _eventclasses;
            public List<Animevent> Eventclasses
            {
                get
                {
                    if (f_eventclasses)
                        return _eventclasses;
                    KaitaiStream io = M_Root.M_Io;
                    long _pos = io.Pos;
                    io.Seek(Eventclassesptr);
                    _eventclasses = new List<Animevent>((int) (4));
                    for (var i = 0; i < 4; i++)
                    {
                        _eventclasses.Add(new Animevent(io, this, m_root));
                    }
                    io.Seek(_pos);
                    f_eventclasses = true;
                    return _eventclasses;
                }
            }
            private ulong _trackptr;
            private int _trackcount;
            private int _numframes;
            private int _loopframe;
            private int _t1;
            private int _t2;
            private int _t3;
            private List<float> _endframeadditivesceneposition;
            private List<float> _endframeadditivescenerotation;
            private long _flags;
            private long _eventclassesptr;
            private long _floattracksptr;
            private Lmt m_root;
            private Lmt.Motion m_parent;
            public ulong Trackptr { get { return _trackptr; } }
            public int Trackcount { get { return _trackcount; } }
            public int Numframes { get { return _numframes; } }
            public int Loopframe { get { return _loopframe; } }
            public int T1 { get { return _t1; } }
            public int T2 { get { return _t2; } }
            public int T3 { get { return _t3; } }
            public List<float> Endframeadditivesceneposition { get { return _endframeadditivesceneposition; } }
            public List<float> Endframeadditivescenerotation { get { return _endframeadditivescenerotation; } }
            public long Flags { get { return _flags; } }
            public long Eventclassesptr { get { return _eventclassesptr; } }
            public long Floattracksptr { get { return _floattracksptr; } }
            public Lmt M_Root { get { return m_root; } }
            public Lmt.Motion M_Parent { get { return m_parent; } }
        }
        public partial class Animevent : KaitaiStruct
        {
            public static Animevent FromFile(string fileName)
            {
                return new Animevent(new KaitaiStream(fileName));
            }

            public Animevent(KaitaiStream p__io, Lmt.Animentry p__parent = null, Lmt p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                f_events = false;
                _read();
            }
            private void _read()
            {
                _eventremaps = new List<short>((int) (32));
                for (var i = 0; i < 32; i++)
                {
                    _eventremaps.Add(m_io.ReadS2le());
                }
                _numevents = m_io.ReadU8le();
                _eventsptr = m_io.ReadU8le();
            }
            private bool f_events;
            private List<Event> _events;
            public List<Event> Events
            {
                get
                {
                    if (f_events)
                        return _events;
                    KaitaiStream io = M_Root.M_Io;
                    long _pos = io.Pos;
                    io.Seek((long)Eventsptr);
                    _events = new List<Event>((int) (Numevents));
                    for (var i = 0; i < (long)Numevents; i++)
                    {
                        _events.Add(new Event(io, this, m_root));
                    }
                    io.Seek(_pos);
                    f_events = true;
                    return _events;
                }
            }
            private List<short> _eventremaps;
            private ulong _numevents;
            private ulong _eventsptr;
            private Lmt m_root;
            private Lmt.Animentry m_parent;
            public List<short> Eventremaps { get { return _eventremaps; } }
            public ulong Numevents { get { return _numevents; } }
            public ulong Eventsptr { get { return _eventsptr; } }
            public Lmt M_Root { get { return m_root; } }
            public Lmt.Animentry M_Parent { get { return m_parent; } }
        }
        public partial class Extreme : KaitaiStruct
        {
            public static Extreme FromFile(string fileName)
            {
                return new Extreme(new KaitaiStream(fileName));
            }

            public Extreme(KaitaiStream p__io, Lmt.Track p__parent = null, Lmt p__root = null) : base(p__io)
            {
                m_parent = p__parent;
                m_root = p__root;
                _read();
            }
            private void _read()
            {
                _min = new List<float>((int) (4));
                for (var i = 0; i < 4; i++)
                {
                    _min.Add(m_io.ReadF4le());
                }
                _max = new List<float>((int) (4));
                for (var i = 0; i < 4; i++)
                {
                    _max.Add(m_io.ReadF4le());
                }
            }
            private List<float> _min;
            private List<float> _max;
            private Lmt m_root;
            private Lmt.Track m_parent;
            public List<float> Min { get { return _min; } }
            public List<float> Max { get { return _max; } }
            public Lmt M_Root { get { return m_root; } }
            public Lmt.Track M_Parent { get { return m_parent; } }
        }
        private byte[] _magic;
        private ushort _version;
        private ushort _entrycount;
        private List<Motion> _entries;
        private Lmt m_root;
        private KaitaiStruct m_parent;
        public byte[] Magic { get { return _magic; } }
        public ushort Version { get { return _version; } }
        public ushort Entrycount { get { return _entrycount; } }
        public List<Motion> Entries { get { return _entries; } }
        public Lmt M_Root { get { return m_root; } }
        public KaitaiStruct M_Parent { get { return m_parent; } }
    }
}
