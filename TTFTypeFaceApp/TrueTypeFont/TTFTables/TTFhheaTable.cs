using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueTypeFont.IO;

namespace TrueTypeFont.TTFTables
{
    public class TTFhheaTable
    {
        private TTFReader _reader;
        private long _hheaTableOffset;
        private ushort _numberOfHMetrics;
        public ushort NumberOfHMetrics
        {
            get { return _numberOfHMetrics; }
            set { _numberOfHMetrics = value; }
        }
        private ushort _advanceWidthMax;
        public ushort AdvanceWidthMax
        {
            get { return _advanceWidthMax; }
            set { _advanceWidthMax = value; }
        }
        private short _minLeftSideBearing;

        public short MinLeftSideBearing
        {
            get { return _minLeftSideBearing; }
            set { _minLeftSideBearing = value; }
        }
        private short _minRightSideBearing;

        public short MinRightSideBearing
        {
            get { return _minRightSideBearing; }
            set { _minRightSideBearing = value; }
        }

        public TTFhheaTable(TTFReader reader)
        {
            this._reader = reader;
            this._hheaTableOffset = reader.Position;
            this.InitiateComponents();
        }
        private void InitiateComponents()
        {
            var majorVersion = this._reader.GetFixed();
            //var minorVersion = this._reader.GetUInt16();

            var ascender = this._reader.GetFWord();
            var descender = this._reader.GetFWord();
            var lineGap = this._reader.GetFWord();
            this._advanceWidthMax = this._reader.GetUFWord();
            this._minLeftSideBearing = this._reader.GetFWord();
            this._minRightSideBearing = this._reader.GetFWord();

            var xMaxExtent = this._reader.GetFWord();
            var caretSlopeRise = this._reader.GetInt16();
            var caretSlopeRun = this._reader.GetInt16();
            var caretOffset = this._reader.GetFWord();
            // reserved
            var reserved0 = this._reader.GetInt16();
            var reserved1 = this._reader.GetInt16();
            var reserved2 = this._reader.GetInt16();
            var reserved3 = this._reader.GetInt16();
            //
            var metricDataFormat = this._reader.GetInt16();
            this._numberOfHMetrics = this._reader.GetUInt16();
        }
    }
}
