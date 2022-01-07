using System.Collections.Generic;

namespace TrueTypeFont.TTFTables
{
    public class GlyphDescription
    {
        public GlyphDescription()
        {
        }

        private short _numberOfContours;
        public short NumberOfContours
        {
            get { return _numberOfContours; }
            set { _numberOfContours = value; }
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
        private string _type;
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
        private List<ushort> _contourEnds;
        public List<ushort> ContourEnds
        {
            get
            {
                if (_contourEnds is null)
                    return _contourEnds = new List<ushort>();
                return _contourEnds;
            }
        }
        private List<bool> _onCurve;
        public List<bool> OnCurve
        {
            get { return _onCurve; }
            set { _onCurve = value; }
        }
        private List<short> _xCoord;

        public List<short> XCoord
        {
            get { return _xCoord; }
            set { _xCoord = value; }
        }
        private List<short> _yCoord;

        public List<short> YCoord
        {
            get { return _yCoord; }
            set { _yCoord = value; }
        }
    }
}