using System;
using System.Collections.Generic;
using TrueTypeFont.IO;

namespace TrueTypeFont.TTFTables
{
    public class TTFlocaTable
    {
        private TTFReader _reader;
        private ushort _numGlyphs;

        public ushort NumGlyphs
        {
            get { return _numGlyphs; }
            set { _numGlyphs = value; }
        }
        private short _indexToLocFormat;

        public short IndexToLocFormat
        {
            get { return _indexToLocFormat; }
            set { _indexToLocFormat = value; }
        }
        private Dictionary<uint, int> _locaOffset;

        public Dictionary<uint, int> LocaOffset => _locaOffset;
        private long _tableOffset;
        public TTFlocaTable(TTFReader reader, ushort numGlyphs, short indexToLocFormat)
        {
            this._reader = reader;
            this._tableOffset = this._reader.Position;
            this._numGlyphs = numGlyphs;
            this._indexToLocFormat = indexToLocFormat;
            this._locaOffset = new Dictionary<uint, int>();
        }
        public uint GetLocalOffset(ushort index)
        {
            if (this._indexToLocFormat == 0)
            {
                if (index < this._numGlyphs + 1)
                {
                    this._reader.Seek(this._tableOffset + (index * 2)); // 2 means two bytes

                    return (uint)(this._reader.GetUInt16() * 2);
                }
                else
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
            else
            {
                if (index < this._numGlyphs + 1)
                {
                    this._reader.Seek(this._tableOffset + (index * 4)); // 4 means four bytes

                    return this._reader.GetUInt32();
                }
                else
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
        private void InitiateComponents()
        {
            if (this._indexToLocFormat == 0)
            {
                for (uint i = 0; i < this._numGlyphs + 1; i++)
                {
                    this._locaOffset.Add(i, this._reader.GetUInt16() * 2);
                }
            }
            else
            {
                for (uint i = 0; i < this._numGlyphs + 1; i++)
                {
                    var offest = this._reader.GetUInt32();
                    this._locaOffset.Add(i, (int)offest);
                }
            }
        }
    }
}