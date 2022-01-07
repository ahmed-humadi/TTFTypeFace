using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TrueTypeFont.IO;

namespace TrueTypeFont.TTFTables
{
    public class TTFglyphTable
    {
        private TTFReader _reader;
        private long _glyfTableOffset;

        public long GlyfTableOffset
        {
            get { return _glyfTableOffset; }
            set { _glyfTableOffset = value; }
        }

        private Dictionary<uint, int> _locaOffset;
        public Dictionary<uint, int> LocaOffset
        {
            get { return _locaOffset; }
            set { _locaOffset = value; }
        }
        private short _indexToLocFormat;

        public short IndexToLocFormat
        {
            get { return _indexToLocFormat; }
            set { _indexToLocFormat = value; }
        }
        private TTFHeader _tTFHeader;

        public TTFHeader TTFHeader
        {
            get { return _tTFHeader; }
            set { _tTFHeader = value; }
        }

        private GlyphDescription _glyph;

        public GlyphDescription Glyph
        {
            get { return _glyph; }
            set { _glyph = value; }
        }

        public TTFglyphTable(TTFReader reader, long glyfTableOffset, short indexToLocFormat, Dictionary<uint, int> locaOffset)
        {
            this._reader = reader;
            this._glyfTableOffset = glyfTableOffset;
            this._indexToLocFormat = indexToLocFormat;
            this._locaOffset = locaOffset;
        }
        private TTFlocaTable _tTFloca;
        public TTFglyphTable(TTFReader reader, long glyfTableOffset, short indexToLocFormat, TTFlocaTable locaOffset)
        {
            this._reader = reader;
            this._glyfTableOffset = glyfTableOffset;
            this._indexToLocFormat = indexToLocFormat;
            this._tTFloca = locaOffset;
        }
        public GlyphDescription GetGlyphDescription(ushort glyphIndex)
        {
            GlyphDescription glyph = null;
            if (this._tTFloca is not null)
            {
                this._reader.Seek(this._glyfTableOffset + this._tTFloca.GetLocalOffset(glyphIndex));
                var numberOfContours = this._reader.GetInt16();
                var xMin = this._reader.GetInt16();
                var yMin = this._reader.GetInt16();
                var xMax = this._reader.GetInt16();
                var yMax = this._reader.GetInt16();

                if (!(xMin > this._tTFHeader.xMin && xMin < this._tTFHeader.xMax))
                    return null;
                else if (!(xMax > this._tTFHeader.xMin && xMax < this._tTFHeader.xMax))
                    return null;
                else if (!(yMin > this._tTFHeader.yMin && yMin < this._tTFHeader.yMax))
                    return null;
                else if (!(yMax > this._tTFHeader.yMin && yMax < this._tTFHeader.yMax))
                    return null;
                glyph = new GlyphDescription()
                {
                    NumberOfContours = numberOfContours,
                    xMin = xMin,
                    yMin = yMin,
                    xMax = xMax,
                    yMax = yMax,
                    OnCurve = new List<bool>(),
                    XCoord = new List<short>(),
                    YCoord = new List<short>()
                };
                if (numberOfContours == -1) // compound
                    this.GetCompoundGlyph(glyph);
                else
                    this.GetSimpleGlyph(glyph);

            }
            return glyph;
        }
        private void GetSimpleGlyph(GlyphDescription glyph)
        {
            byte ON_CURVE = 1;
            byte X_IS_BYTE = 2;
            byte Y_IS_BYTE = 4;
            byte REPEAT = 8;
            byte X_DELTA = 16;
            byte Y_DELTA = 32;

            for (int i = 0; i < glyph.NumberOfContours; i++)
            {
                var d = this._reader.GetUInt16();
                glyph.ContourEnds.Add(d);
            }
            // skip instructions
            var length = this._reader.GetUInt16();
            for (int i = 0; i < length; i++)
                this._reader.GetUInt8();
            if (glyph.NumberOfContours == 0)
                return;
            // max points
            var numPoints = glyph.ContourEnds.Max<ushort>() + 1;
            // read flags
            byte[] flags = new byte[numPoints];
            int c = 0;
            int repeatCount = 0;
            byte flag = 0;
            while (c < flags.Length)
            {
                if (repeatCount > 0)
                {
                    repeatCount--;
                }
                else
                {
                    flag = this._reader.GetUInt8();

                    if ((flag & REPEAT) != 0)
                    {
                        repeatCount = this._reader.GetUInt8();
                    }
                }
                glyph.OnCurve.Add((flag & ON_CURVE) == ON_CURVE);
                flags[c++] = flag;
            }
            // read coord
            List<short> ReadCoords(short byteFlag, short deltaFlag)
            {
                int x = 0;
                List<short> xs = new List<short>();

                for (int i = 0; i < numPoints; i++)
                {
                    int dx;
                    if ((flags[i] & byteFlag) != 0)
                    {
                        var b = this._reader.GetUInt8();
                        dx = ((flags[i] & deltaFlag) != 0) ? b : -b;
                    }
                    else
                    {
                        if ((deltaFlag & flags[i]) != 0)
                            dx = 0;
                        else
                            dx = this._reader.GetInt16();
                    }
                    x += dx;
                    xs.Add((short)x);
                }
                return xs;
            }
            glyph.XCoord = ReadCoords(X_IS_BYTE, X_DELTA);
            glyph.YCoord = ReadCoords(Y_IS_BYTE, Y_DELTA);
        }
        private void GetCompoundGlyph(GlyphDescription glyph)
        {
            // compound glyph masks
            ushort ArgsAreWords = 1;
            ushort ArgsAreXYValues = 2;
            ushort RountXYToGrid = 4;
            ushort WeHaveScale = 8;
            ushort Reserved = 16;
            ushort MoreComponents = 32;
            ushort WeHaveXAndYScale = 64;
            ushort WeHaveATwoByTwo = 128;
            ushort WeHaveInstructions = 256;
            ushort UseMyMetrics = 512;
            ushort OverLapCompound = 1024;
            ushort ScaledComponentOffset = 2048;
            ushort UnscaledComponentOffset = 4096;
            //
            ushort flags;
            ushort glyphIndex;
            do
            {
                flags = this._reader.GetUInt16();
                glyphIndex = this._reader.GetUInt16();
                short arg1;
                short arg2;
                short m00 = 1, m01 = 0, m10 = 0, m11 = 1;
                // [a b c d e f]
                Matrix matrix = Matrix.Identity;

                if ((flags & ArgsAreWords) != 0)
                {
                    arg1 = this._reader.GetInt16();
                    arg2 = this._reader.GetInt16();
                }
                else
                {
                    arg1 = (short)this._reader.GetUInt8();
                    arg2 = (short)this._reader.GetUInt8();
                }

                short dx; short dy;

                if ((flags & ArgsAreXYValues) != 0)
                {
                    //dx = arg1;
                    //dy = arg2;
                    matrix.OffsetX = arg1;
                    matrix.OffsetY = arg2;
                }
                else
                {
                    dx = 0;
                    dy = 0;
                    matrix.OffsetX = 0;
                    matrix.OffsetY = 0;
                }
                if ((flags & WeHaveScale) != 0)
                {
                    short scale = this._reader.GetF2Dot14();
                    m00 = scale;
                    m11 = scale;
                    matrix.M11 = scale;
                    matrix.M22 = scale;
                }
                else if ((flags & WeHaveXAndYScale) != 0)
                {
                    short xscale = this._reader.GetF2Dot14();
                    short yscale = this._reader.GetF2Dot14();
                    //m00 = xscale;
                    //m11 = yscale;
                    matrix.M11 = xscale;
                    matrix.M22 = yscale;
                }
                else if ((flags & WeHaveATwoByTwo) != 0)
                {
                    //m00 = this._reader.GetF2Dot14();
                    //m01 = this._reader.GetF2Dot14();
                    //m10 = this._reader.GetF2Dot14();
                    //m11 = this._reader.GetF2Dot14();
                    matrix.M11 = this._reader.GetF2Dot14();
                    matrix.M12 = this._reader.GetF2Dot14();
                    matrix.M21 = this._reader.GetF2Dot14();
                    matrix.M22 = this._reader.GetF2Dot14();
                }
                //
                long pos = this._reader.Position;

                GlyphDescription simlpGlyph = this.GetGlyphDescription(glyphIndex);


                if (simlpGlyph is not null)
                {
                    int pointOffset = glyph.OnCurve.Count;

                    for (int i = 0; i < simlpGlyph.ContourEnds.Count; i++)
                    {
                        glyph.ContourEnds.Add((ushort)(simlpGlyph.ContourEnds[i] + pointOffset));
                    }
                    for (int i = 0; i < simlpGlyph.XCoord.Count; i++)
                    {
                        short x = simlpGlyph.XCoord[i];
                        short y = simlpGlyph.YCoord[i];
                        //matrix.OffsetX = 0;
                        Transform transform = new MatrixTransform(matrix);
                        Point point = transform.Transform(new Point(x, y));
                        glyph.XCoord.Add((short)point.X);
                        glyph.YCoord.Add((short)point.Y);

                        glyph.OnCurve.Add(simlpGlyph.OnCurve[i]);

                    }
                }
                this._reader.Seek(pos);
                //
            }
            while ((flags & MoreComponents) != 0);

            glyph.NumberOfContours = (short)glyph.ContourEnds.Count;

            if ((flags & WeHaveInstructions) != 0)
            {
                // instrcutions
            }
        }
    }
}
