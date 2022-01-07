using System.Collections.Generic;
using TrueTypeFont.IO;

namespace TrueTypeFont.TTFTables
{
    public class CMapFormat12
    {
        private TTFReader _reader;
        private long _tableOffset;
        private Dictionary<uint, ushort> _charCode2GID;

        public Dictionary<uint, ushort> CharCode2GID
        {
            get
            {
                if (_charCode2GID is null)
                    _charCode2GID = new Dictionary<uint, ushort>();
                return _charCode2GID;
            }
            set { _charCode2GID = value; }
        }

        public CMapFormat12(TTFReader reader)
        {
            this._reader = reader;
            _tableOffset = this._reader.Position - 2;
            this.Initializing();
        }
        private void Initializing()
        {
            var reserved = this._reader.GetUInt16();

            var length = this._reader.GetUInt32();

            var language = this._reader.GetUInt32();

            var numGroups = this._reader.GetUInt32();

            for (int i = 0; i < numGroups; i++)
            {
                var startCharCode = this._reader.GetUInt32();
                var endCharCode = this._reader.GetUInt32();
                var startGlyphID = this._reader.GetUInt32();
                // fill dict with each group

                for (; startCharCode <= endCharCode;)
                {
                    CharCode2GID.Add(startCharCode++, (ushort)startGlyphID++);
                }
            }
            //MessageBox.Show(CharCode2GID.Values.Max().ToString());
            //MessageBox.Show(CharCode2GID.ContainsKey(
            //    uint.Parse("007E", System.Globalization.NumberStyles.HexNumber)).ToString());
        }
    }
}