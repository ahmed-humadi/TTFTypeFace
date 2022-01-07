using System.Collections.Generic;
using TrueTypeFont.IO;

namespace TrueTypeFont.TTFTables
{
    public class CMapFormat4
    {
        private Dictionary<int, ushort> _uint16CharCode2GID;
        public Dictionary<int, ushort> Uint16CharCode2GID
        {
            get
            {
                if (_uint16CharCode2GID is null)
                    return _uint16CharCode2GID = new Dictionary<int, ushort>();
                return _uint16CharCode2GID;
            }
        }
        private TTFReader _reader;
        private long idRangeOffsetsStart;
        public CMapFormat4(TTFReader reader)
        {
            this._reader = reader;
            this.InitializeComponents();
        }
        private ushort _length;
        private ushort _language;
        private ushort _segCountX2;
        private ushort _segCount;
        private ushort _searchRange;
        private ushort _entrySelector;
        private ushort _rangeShift;

        private ushort[] _endCode;
        private ushort _reservedPad;
        private ushort[] _startCode;

        private ushort[] _idDelta;
        private ushort[] _idRangeOffset;
        private void InitializeComponents()
        {
            this._length = this._reader.GetUInt16();
            //MessageBox.Show($"windows table length {_length}");
            this._language = this._reader.GetUInt16();
            //MessageBox.Show($"windows table language {language}");
            this._segCountX2 = this._reader.GetUInt16();
            this._segCount = (ushort)(this._segCountX2 >> 1);
            //MessageBox.Show($"windows table segCountX2 {_segCountX2}");
            this._searchRange = this._reader.GetUInt16();
            //MessageBox.Show($"windows table searchRange {searchRange}");
            this._entrySelector = this._reader.GetUInt16();
            //MessageBox.Show($"windows table entrySelector {entrySelector}");
            this._rangeShift = this._reader.GetUInt16();
            //MessageBox.Show($"windows table rangeShift {rangeShift}");
            // fill endCode array;
            this.FillendCode();
            this._reservedPad = this._reader.GetUInt16();
            //MessageBox.Show($"windows table reservedPad {_reservedPad}");
            // fill startCode array;
            this.FillstartCode();
            // fill idDelta array;
            this.FillidDelta();
            // fill idRangeOffset array;
            this.idRangeOffsetsStart = this._reader.Position;
            this.FillidRangeOffset();
            // fill glyphIndex dic;
            this.FillglyphIndexDictionary();
        }
        private void FillendCode()
        {

            this._endCode = new ushort[_segCount];
            for (int i = 0; i < _segCount; i++)
            {
                this._endCode[i] = this._reader.GetUInt16();
                //MessageBox.Show($"end Code = {this._endCode[i]:X4}");
            }
        }
        private void FillstartCode()
        {
            this._startCode = new ushort[_segCount];
            for (int i = 0; i < _segCount; i++)
            {
                this._startCode[i] = this._reader.GetUInt16();
                //MessageBox.Show($"start Code = {this._startCode[i]:X4}");
            }
        }
        private void FillidDelta()
        {
            this._idDelta = new ushort[_segCount];
            for (int i = 0; i < _segCount; i++)
            {
                this._idDelta[i] = this._reader.GetUInt16();
            }
        }
        private void FillidRangeOffset()
        {
            this._idRangeOffset = new ushort[_segCount];
            for (int i = 0; i < _segCount; i++)
            {
                this._idRangeOffset[i] = this._reader.GetUInt16();
            }
        }
        private void FillglyphIndexDictionary()
        {
            ushort glyphIndex;
            for (int i = 0; i < _segCount; i++)
            {
                ushort endCode = this._endCode[i];
                ushort startCode = this._startCode[i];
                ushort idDelta = this._idDelta[i];
                ushort idRangeOffset = this._idRangeOffset[i];
                for (int c = startCode; c <= endCode; c++)
                {
                    if (idRangeOffset != 0)
                    {
                        long startCodeOffset = (c - startCode) * 2;
                        long currentRangeOffset = i * 2;

                        long glyphIndexOffset = this.idRangeOffsetsStart + currentRangeOffset +
                                                (long)idRangeOffset + startCodeOffset;

                        this._reader.Seek(glyphIndexOffset);

                        glyphIndex = this._reader.GetUInt16();

                        if (glyphIndex != 0)
                        {
                            glyphIndex = (ushort)((glyphIndex + idDelta) & 0xffff);
                        }
                        //else
                        //{
                        //    glyphIndex = (ushort)((0 + idDelta) & 0xffff);
                        //}
                    }
                    else
                        glyphIndex = (ushort)((c + idDelta) & 0xffff);
                    this.Uint16CharCode2GID.Add(c, glyphIndex);
                    //MessageBox.Show($"Char Code = {c:X4} .... Glyf index = {glyphIndex}");
                }
            }
            //MessageBox.Show(CharCode2GID.Count.ToString());
            //MessageBox.Show(CharCode2GID.ContainsKey(
            //    int.Parse("818C", System.Globalization.NumberStyles.HexNumber)).ToString());

        }
    }
}