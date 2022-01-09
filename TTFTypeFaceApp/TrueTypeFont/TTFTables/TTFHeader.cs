using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueTypeFont.IO;

namespace TrueTypeFont.TTFTables
{
    public class TTFHeader
    {
        private TTFReader _reader;
        private long _headerTableOffset;
        private float _version;
        public float Version
        {
            get { return _version; }
            set { _version = value; }
        }
        private short _indexToLocFormat;

        public short IndexToLocFormat
        {
            get { return _indexToLocFormat; }
            set { _indexToLocFormat = value; }
        }
        private ushort _unitsPerEm;

        public ushort UnitsPerEm
        {
            get { return _unitsPerEm; }
            set { _unitsPerEm = value; }
        }

        public TTFHeader(TTFReader reader)
        {
            this._reader = reader;
            this._headerTableOffset = reader.Position;
            this.InitialComponents();
        }
        private short _xMin;

        public short xMin
        {
            get { return _xMin; }
            set { _xMin = value; }
        }
        private short _yMin;

        public short yMin
        {
            get { return _yMin; }
            set { _yMin = value; }
        }
        private short _xMax;

        public short xMax
        {
            get { return _xMax; }
            set { _xMax = value; }
        }
        private short _yMax;
        public short yMax
        {
            get { return _yMax; }
            set { _yMax = value; }
        }
        private ushort _flags;
        public ushort Flags
        {
            get { return _flags; }
            set { _flags = value; }
        }
        private uint _magicNumber;

        public uint MagicNumber
        {
            get { return _magicNumber; }
            set { _magicNumber = value; }
        }

        private void InitialComponents()
        {
            var majorVersion = _reader.GetFixed();
            //MessageBox.Show(majorVersion.ToString());
            var fontReVersion = _reader.GetFixed();
            //MessageBox.Show(fontReVersion.ToString());
            var checksumAdjustment = _reader.GetUInt32();
            //MessageBox.Show(checksumAdjustment.ToString());
            _magicNumber = _reader.GetUInt32();
            //MessageBox.Show(magicNumber.ToString(),"magicNumber");
            this._flags = _reader.GetUInt16();
            //MessageBox.Show(Convert.ToString(flags,toBase:2));
            this._unitsPerEm = _reader.GetUInt16();
            //MessageBox.Show(unitsPerEm.ToString());
            var created = _reader.GetDate();
            //MessageBox.Show(created.ToString());
            var modified = _reader.GetDate();
            //MessageBox.Show(modified.ToString());
            _xMin = _reader.GetFWord();
            //MessageBox.Show(xMin.ToString());
            _yMin = _reader.GetFWord();
            //MessageBox.Show(yMin.ToString());
            _xMax = _reader.GetFWord();
            //MessageBox.Show(xMax.ToString());
            _yMax = _reader.GetFWord();
            //MessageBox.Show(yMax.ToString());
            var macStyle = _reader.GetUInt16();
            //MessageBox.Show(macStyle.ToString());
            var lowestRecPPEM = _reader.GetUInt16();
            //MessageBox.Show(lowestRecPPEM.ToString());
            var fontDirectionHint = _reader.GetInt16();
            //MessageBox.Show(fontDirectionHint.ToString());
            this._indexToLocFormat = _reader.GetInt16();
            //MessageBox.Show(indexToLocFormat.ToString());
            var glyphDataFormat = _reader.GetInt16();
            //MessageBox.Show(glyphDataFormat.ToString());
        }
    }
}
