using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueTypeFont.IO;

namespace TrueTypeFont.TTFTables
{
    public class TTFmaxpTable
    {
        private TTFReader _reader;
        private ushort _numGlyphs;

        public ushort NumGlyphs
        {
            get { return _numGlyphs; }
            set { _numGlyphs = value; }
        }

        public TTFmaxpTable(TTFReader reader)
        {
            this._reader = reader;
            this.InitiateComponents();
        }
        private void InitiateComponents()
        {
            var version = this._reader.GetFixed();
            this._numGlyphs = this._reader.GetUInt16();
            var maxPoints = this._reader.GetUInt16();
            var maxContours = this._reader.GetUInt16();
            var maxCompositePoints = this._reader.GetUInt16();
            var maxCompositeContours = this._reader.GetUInt16();
            var maxZones = this._reader.GetUInt16();
            var maxTwilightPoints = this._reader.GetUInt16();
            var maxStorage = this._reader.GetUInt16();
            var maxFunctionDefs = this._reader.GetUInt16();
            var maxInstructionDefs = this._reader.GetUInt16();
            var maxStackElements = this._reader.GetUInt16();
            var maxSizeOfInstructions = this._reader.GetUInt16();
            var maxComponentElements = this._reader.GetUInt16();
            var maxComponentDepth = this._reader.GetUInt16();
        }
    }
}
