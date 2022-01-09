using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.IO;
using TrueTypeFont.IO;
using TrueTypeFont.TTFTables;
namespace TrueTypeFont
{
    public class TTFTypeFace : IDisposable
    {
        private string[] _cMapSubtable;
        public string[] CMapSubtable
        {
            get { return _cMapSubtable; }
            set { _cMapSubtable = value; }
        }
        public ushort NumberOfGlyphs
        {
            get { return _tTFloca.NumGlyphs; }
        }
        public uint MagicNumber => _tTFHeader.MagicNumber;
        private Dictionary<int, ushort> _uint16charCode2GID;
        // lazy initialization
        public Dictionary<int, ushort> Uint16charCode2GID
        {
            get
            {
                if (this._uint16charCode2GID is null)
                {
                    if (this._tTFCMap is not null)
                    {
                        this._uint16charCode2GID = this._tTFCMap.CMapFormat4.Uint16CharCode2GID;
                        return _uint16charCode2GID;
                    }
                    else
                        throw new ArgumentNullException(nameof(this._tTFCMap));
                }
                return _uint16charCode2GID;
            }
        }
        private Dictionary<uint, ushort> _uint32charCode2GID;
        // lazy initialization
        public Dictionary<uint, ushort> Uint32CharCode2GID
        {
            get
            {
                if (this._uint32charCode2GID is null)
                {
                    if (this._tTFCMap is not null)
                    {
                        this._uint32charCode2GID = this._tTFCMap.CMapFormat12.CharCode2GID;
                        return _uint32charCode2GID;
                    }
                    else
                        throw new ArgumentNullException(nameof(this._tTFCMap));
                }
                return _uint32charCode2GID;
            }
        }
        // lazy initialization
        private Dictionary<ushort, float> _advanceWidthIndexedByGID;
        public Dictionary<ushort, float> AdvanceWidthIndexedByGID
        {
            get
            {
                if (this._advanceWidthIndexedByGID is null)
                {
                    if (_tTFhmtx is not null)
                    {
                        this._advanceWidthIndexedByGID = this._tTFhmtx.AdvanceWidth;
                        return _advanceWidthIndexedByGID;
                    }
                    else
                        throw new ArgumentNullException(nameof(this._tTFhmtx));
                }
                return _advanceWidthIndexedByGID;
            }
        }
        private Dictionary<ushort, short> _lsbIndexedByGID;
        public Dictionary<ushort, short> LsbIndexedByGID
        {
            get
            {
                if (this._lsbIndexedByGID is null)
                {
                    if (_tTFhmtx is not null)
                    {
                        this._lsbIndexedByGID = this._tTFhmtx.lsb;
                        return _lsbIndexedByGID;
                    }
                    else
                        throw new ArgumentNullException(nameof(this._tTFhmtx));
                }
                return _lsbIndexedByGID;
            }
        }
        private ushort _unitsPerEm;
        private MemoryStream _internalStream;
        private TTFReader _ttfReader;
        public TTFReader TtfReader
        {
            get
            {
                if (this._ttfReader is null)
                    _ttfReader = new TTFReader(new BinaryReader(this._internalStream));
                return _ttfReader;
            }
            set
            {
                _ttfReader = value;
            }
        }
        public TTFTypeFace(string fontname)
        {
            if (fontname == "MS-Gothic")
            {
                using (FileStream fileStream = new FileStream(@"F:\msgothic.ttc",
                    FileMode.Open, FileAccess.Read))
                {
                    byte[] data = new byte[fileStream.Length];
                    fileStream.Read(data);
                    this._internalStream = new MemoryStream(data);
                    this.InitializeFontTables();
                }
            }
            else if (fontname == "ArialMT")
            {
                using (FileStream fileStream = new FileStream(@"F:\arial.ttf",
                    FileMode.Open, FileAccess.Read))
                {
                    byte[] data = new byte[fileStream.Length];
                    fileStream.Read(data);
                    this._internalStream = new MemoryStream(data);
                    this.InitializeFontTables();
                }
            }
            else if (fontname == "Helvetica")
            {
                using (FileStream fileStream = new FileStream(@"G:\Helvetica-Font\Helvetica.ttf",
                    FileMode.Open, FileAccess.Read))
                {
                    byte[] data = new byte[fileStream.Length];
                    fileStream.Read(data);
                    this._internalStream = new MemoryStream(data);
                    this.InitializeFontTables();
                }
            }
            else if (fontname == "Helvetica-Bold")
            {
                using (FileStream fileStream = new FileStream(@"G:\Helvetica-Font\Helvetica-Bold.ttf",
                    FileMode.Open, FileAccess.Read))
                {
                    byte[] data = new byte[fileStream.Length];
                    fileStream.Read(data);
                    this._internalStream = new MemoryStream(data);
                    this.InitializeFontTables();
                }
            }
        }
        public TTFTypeFace(byte[] fontprogramData)
        {
            this._internalStream = new MemoryStream(fontprogramData);
            this.InitializeFontTables();
        }
        public TTFTypeFace(MemoryStream fontProgram)
        {
            this._internalStream = fontProgram;
            this.InitializeFontTables();
        }
        public TTFTypeFace()
        {
        }
        // true type font tables
        private TTFHeader _tTFHeader = null;
        private TTFCMapTable _tTFCMap = null;
        private TTFlocaTable _tTFloca = null;
        private TTFmaxpTable _tTFmaxp = null;
        private TTFglyphTable _tTFglyf = null;
        private TTFhheaTable _tTFhhea = null;
        private TTFhmtxTable _tTFhmtx = null;
        //private TTCHeader _tTCHeader = null;
        //public TTCHeaderTable TTCHeaderTable => this._tTCHeader;
        //
        public void InitializeFontTables()
        {
            if ((int)this.TtfReader.GetUInt8() == 116)
            {
                this.TtfReader.GoBack();
                //this._tTCHeader = new TTCHeaderTable(this.TtfReader);
            }
            else
            {
                this.TtfReader.GoBack();
                Dictionary<string, long> tablesOffset = new Dictionary<string, long>();
                var scale = this.TtfReader.GetUInt32(); // scalarType
                var numTables = this.TtfReader.GetUInt16();
                var g = this.TtfReader.GetUInt16(); // searchRange
                var d = this.TtfReader.GetUInt16(); // entrySelector
                var eh = this.TtfReader.GetUInt16(); // rangeShift
                for (int i = 0; i < numTables; i++)
                {
                    string tag = this.TtfReader.GetSting(4);
                    var checksum = this.TtfReader.GetUInt32();
                    var offset = this.TtfReader.GetUInt32();
                    var length = this.TtfReader.GetUInt32();
                    tablesOffset.Add(tag, offset);
                }
                if (tablesOffset.ContainsKey("head"))
                {
                    this.TtfReader.Seek(tablesOffset["head"]);
                    this._tTFHeader = new TTFHeader(this.TtfReader);
                    this._unitsPerEm = this._tTFHeader.UnitsPerEm;
                }
                if (tablesOffset.ContainsKey("maxp"))
                {
                    this.TtfReader.Seek(tablesOffset["maxp"]);
                    this._tTFmaxp = new TTFmaxpTable(this.TtfReader);
                }
                if (tablesOffset.ContainsKey("cmap"))
                {
                    this.TtfReader.Seek(tablesOffset["cmap"]);
                    this._tTFCMap = new TTFCMapTable(this.TtfReader);
                    this._cMapSubtable = this._tTFCMap.Subtables;
                }
                if (tablesOffset.ContainsKey("loca"))
                {
                    this.TtfReader.Seek(tablesOffset["loca"]);
                    this._tTFloca = new TTFlocaTable(this.TtfReader, _tTFmaxp.NumGlyphs, _tTFHeader.IndexToLocFormat);
                }
                if (tablesOffset.ContainsKey("glyf"))
                {
                    this.TtfReader.Seek(tablesOffset["glyf"]);
                    this._tTFglyf = new TTFglyphTable(this.TtfReader, tablesOffset["glyf"], _tTFHeader.IndexToLocFormat, _tTFloca);
                    this._tTFglyf.TTFHeader = this._tTFHeader;
                }
                if (tablesOffset.ContainsKey("hhea"))
                {
                    this.TtfReader.Seek(tablesOffset["hhea"]);
                    this._tTFhhea = new TTFhheaTable(this.TtfReader);
                }
                if (tablesOffset.ContainsKey("hmtx"))
                {
                    this.TtfReader.Seek(tablesOffset["hmtx"]);
                    this._tTFhmtx = new TTFhmtxTable(this.TtfReader, this._tTFhhea.NumberOfHMetrics, this._tTFmaxp.NumGlyphs, this._unitsPerEm);
                }

            }
        }
        private Dictionary<ushort, PathGeometry> _catchedGlyphs;
        public Dictionary<ushort, PathGeometry> CatchedGlyphs
        {
            get
            {
                if (_catchedGlyphs is null)
                    _catchedGlyphs = new Dictionary<ushort, PathGeometry>();
                return _catchedGlyphs;
            }
        }
        public PathGeometry GetGlyphOutline(ushort glyphIndex)
        {
            GlyphDescription glyph = null;
            if (this._tTFglyf is not null)
                glyph = this._tTFglyf.GetGlyphDescription(glyphIndex);
            else
                throw new ArgumentNullException(nameof(this._tTFglyf));
            if (glyph is null || glyphIndex == 0)
                return new PathGeometry(); // empty outline
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.FillRule = FillRule.EvenOdd;
            PathFigure pathFigure = null;
            if (glyph is not null)
            {
                double scale = 20.0 / (double)this._unitsPerEm;
                var p = 0;
                var c = 0;
                var first = 0;
                var contourStart = 0;
                Point point1 = new Point();
                Point pointPrev = new Point();
                for (; p < glyph.OnCurve.Count; p++)
                {
                    point1.X = glyph.XCoord[p] * scale;
                    point1.Y = glyph.YCoord[p] * scale;

                    if (first == 0)
                    {
                        pathFigure = new PathFigure() { IsClosed = true };
                        pathGeometry.Figures.Add(pathFigure);
                        pathFigure.StartPoint = point1;
                        first = 1;
                    }
                    else if (first == 1)
                    {
                        if (glyph.OnCurve[p])
                        {
                            LineSegment lineSegment = new LineSegment() { Point = point1 };
                            pathFigure.Segments.Add(lineSegment);
                        }
                        else
                            first = 2;
                    }
                    else
                    {
                        pointPrev.X = glyph.XCoord[p - 1] * scale;
                        pointPrev.Y = glyph.YCoord[p - 1] * scale;
                        if (glyph.OnCurve[p])
                        {
                            QuadraticBezierSegment bezierSegment =
                                new QuadraticBezierSegment(pointPrev, new Point(point1.X, point1.Y), false);
                            pathFigure.Segments.Add(bezierSegment);
                            first = 1;
                        }
                        else
                        {
                            QuadraticBezierSegment bezierSegment = new QuadraticBezierSegment(pointPrev,
                              new Point((pointPrev.X + point1.X) / 2.0, (pointPrev.Y + point1.Y) / 2.0), false);
                            pathFigure.Segments.Add(bezierSegment);
                        }
                    }
                    if (p == glyph.ContourEnds[c])
                    {
                        if (first == 2)
                        {
                            pointPrev = point1;

                            point1.X = glyph.XCoord[contourStart] * scale;
                            point1.Y = glyph.YCoord[contourStart] * scale;

                            if (glyph.OnCurve[p])
                            {
                                QuadraticBezierSegment bezierSegment = new QuadraticBezierSegment(pointPrev,
                                    new Point(point1.X, point1.Y), false);
                                pathFigure.Segments.Add(bezierSegment);
                                first = 1;
                            }
                            else
                            {
                                QuadraticBezierSegment bezierSegment = new QuadraticBezierSegment(pointPrev,
                              new Point((pointPrev.X + point1.X) / 2.0, (pointPrev.Y + point1.Y) / 2.0), false);
                                pathFigure.Segments.Add(bezierSegment);
                            }
                        }
                        contourStart = p + 1;
                        c += 1;
                        first = 0;
                    }
                }
            }
            if (AdvanceWidthIndexedByGID.ContainsKey(glyphIndex))
            {
                var aw = AdvanceWidthIndexedByGID[glyphIndex] * (float)_unitsPerEm;
                float max = glyph.xMax;
                float min = glyph.xMin;
                float lsb = LsbIndexedByGID[glyphIndex];
                float rsb = aw - (lsb + max - min);
                if (!((this._tTFHeader.Flags & 0x0020) != 0)) // bit 5 is set for variable fonts
                {
                    if ((this._tTFHeader.Flags & 0x0002) != 0) // bit 1 is set for same lsb and xmin
                    {
                        if (lsb != min)
                        {
                            pathGeometry.Clear();
                            if (!this.CatchedGlyphs.ContainsKey(glyphIndex))
                                this.CatchedGlyphs.Add(glyphIndex, pathGeometry);
                            return pathGeometry;
                        }
                    }
                }
                if (rsb < 0)
                {
                    if (Math.Abs(rsb) > Math.Abs(max - min) / 2.0f)
                    {
                        pathGeometry.Clear();
                        if (!this.CatchedGlyphs.ContainsKey(glyphIndex))
                            this.CatchedGlyphs.Add(glyphIndex, pathGeometry);
                        return pathGeometry;
                    }
                }
                if (rsb < this._tTFhhea.MinRightSideBearing
                    || lsb < this._tTFhhea.MinLeftSideBearing
                    || aw > this._tTFhhea.AdvanceWidthMax)
                {
                    pathGeometry.Clear();
                    if (!this.CatchedGlyphs.ContainsKey(glyphIndex))
                        this.CatchedGlyphs.Add(glyphIndex, pathGeometry);
                    return pathGeometry;
                }
                else
                {
                    if (!this.CatchedGlyphs.ContainsKey(glyphIndex))
                        this.CatchedGlyphs.Add(glyphIndex, pathGeometry);
                    return pathGeometry;
                }
            }
            else
            {
                if (!this.CatchedGlyphs.ContainsKey(glyphIndex))
                    this.CatchedGlyphs.Add(glyphIndex, pathGeometry);
                return pathGeometry;
            }
        }

        public void Dispose()
        {
            if(this._ttfReader is not null)
                this._ttfReader.Dispose();
        }
    }
}
