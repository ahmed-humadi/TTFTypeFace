using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueTypeFont.IO;

namespace TrueTypeFont.TTFTables
{
    public class TTFhmtxTable
    {
        private TTFReader _reader;
        private long _hmtxTableOffset;
        private ushort _numberOfHMetrics;
        private ushort _numGlyphs;
        private ushort _unitsPerEm;
        private Dictionary<ushort, float> _advanceWidth;
        public Dictionary<ushort, float> AdvanceWidth
        {
            get { return _advanceWidth; }
            set { _advanceWidth = value; }
        }
        private Dictionary<ushort, short> _lsb;
        public Dictionary<ushort, short> lsb
        {
            get { return _lsb; }
            set { _lsb = value; }
        }

        public TTFhmtxTable(TTFReader reader, ushort numberOfHMetrics, ushort numGlyphs, ushort unitsPerEm)
        {
            this._reader = reader;
            this._hmtxTableOffset = reader.Position;
            this._numberOfHMetrics = numberOfHMetrics;
            this._numGlyphs = numGlyphs;
            this._unitsPerEm = unitsPerEm;
            this.InitiateComponents();
        }
        private void InitiateComponents()
        {
            if (this._advanceWidth is null)
                this._advanceWidth = new Dictionary<ushort, float>();
            if (this._lsb is null)
                this._lsb = new Dictionary<ushort, short>();
            for (int i = 0; i < this._numberOfHMetrics; i++)
            {
                float width = (float)this._reader.GetUInt16() / (float)this._unitsPerEm;
                this._advanceWidth.Add((ushort)i, width);
                this._lsb.Add((ushort)i, this._reader.GetInt16());
            }
            float lastWidth = this._advanceWidth.Values.Last();
            for (int i = this._numberOfHMetrics; i <= (this._numGlyphs); i++)
            {
                this._lsb.Add((ushort)i, this._reader.GetFWord());
                this._advanceWidth.Add((ushort)i, lastWidth);
            }
        }
    }
}
