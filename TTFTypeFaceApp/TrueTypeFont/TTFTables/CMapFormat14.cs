using System;
using System.Collections.Generic;
using TrueTypeFont.IO;

namespace TrueTypeFont.TTFTables
{
    public class CMapFormat14
    {
        private TTFReader _reader;
        private uint _numVarSelectorRecords;
        private long _tableOffset;
        public uint numVarSelectorRecords
        {
            get { return _numVarSelectorRecords; }
            set { _numVarSelectorRecords = value; }
        }
        private Dictionary<int, ushort> _unicode2GID;
        public Dictionary<int, ushort> Unicode2GID
        {
            get
            {
                if (_unicode2GID is null)
                    _unicode2GID = new Dictionary<int, ushort>();
                return _unicode2GID;
            }
        }

        public CMapFormat14(TTFReader reader)
        {
            this._reader = reader;
            _tableOffset = this._reader.Position - 2;
            this.InitializeComponents();
        }
        private void InitializeComponents()
        {
            var length = this._reader.GetUInt32();
            this._numVarSelectorRecords = this._reader.GetUInt32();
            for (int i = 0; i < this._numVarSelectorRecords; i++)
            {
                var varSelector1 = this._reader.GetUInt8();
                var varSelector2 = this._reader.GetUInt8();
                var varSelector3 = this._reader.GetUInt8();

                string selectorChar = Convert.ToHexString(new byte[]
                {
                    varSelector1,
                    varSelector2,
                    varSelector3
                });

                var defaultUVSOffset = this._reader.GetUInt32();
                var nonDefaultUVSOffset = this._reader.GetUInt32();

                // NonDefult UVS Table

                long oldPos = this._reader.Position;

                this._reader.Seek((_tableOffset + nonDefaultUVSOffset));

                var numUVSMappings = this._reader.GetUInt32();

                for (int j = 0; j < numUVSMappings; j++)
                {
                    var BaseunicodeValue1 = this._reader.GetUInt8();
                    var BaseunicodeValue2 = this._reader.GetUInt8();
                    var BaseunicodeValue3 = this._reader.GetUInt8();

                    string unicodeValue = Convert.ToHexString(new byte[]
                    {
                        BaseunicodeValue1,
                        BaseunicodeValue2,
                        BaseunicodeValue3
                    });

                    var glyphID = this._reader.GetUInt16();

                    this.Unicode2GID.Add(int.Parse(unicodeValue, System.Globalization.NumberStyles.HexNumber),
                        glyphID);
                }
                this._reader.Seek((oldPos));
            }
        }
    }
}