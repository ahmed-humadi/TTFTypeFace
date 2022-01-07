using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueTypeFont.IO;

namespace TrueTypeFont.TTFTables
{
    public class TTFCMapTable
    {
        private string[] _subtables;
        public string[] Subtables
        {
            get { return _subtables; }
            set { _subtables = value; }
        }
        private TTFReader _reader;
        private long _cMapTableOffset;
        public long CMapTableOffset { get => _cMapTableOffset; }
        private ushort _version;
        public ushort Version { get => _version; }
        private ushort _noOfSubTables;
        public ushort NoOfSubTables { get => _noOfSubTables; }

        private CMapFormat4 _cMapFormat4;

        public CMapFormat4 CMapFormat4
        {
            get { return _cMapFormat4; }
            set { _cMapFormat4 = value; }
        }

        private CMapFormat12 _cMapFormat12;

        public CMapFormat12 CMapFormat12
        {
            get { return _cMapFormat12; }
            set { _cMapFormat12 = value; }
        }
        private CMapFormat14 _cMapFormat14;

        public CMapFormat14 CMapFormat14
        {
            get { return _cMapFormat14; }
            set { _cMapFormat14 = value; }
        }
        public TTFCMapTable(TTFReader reader)
        {
            this._reader = reader;
            this._cMapTableOffset = reader.Position;
            this.InitializingComponents();
            if (_subtables is null)
            { }
        }
        private void InitializingComponents()
        {
            long pos = 0L;
            this._version = this._reader.GetUInt16();

            //MessageBox.Show(_version.ToString());
            this._noOfSubTables = this._reader.GetUInt16();
            this._subtables = new string[this._noOfSubTables];
            //MessageBox.Show(_noOfSubTables.ToString());

            for (int i = 0; i < this._noOfSubTables; i++)
            {
                var platformID = this._reader.GetUInt16();
                var encodingID = this._reader.GetUInt16();
                var offset = this._reader.GetUInt32();
                this._subtables[i] = $"({platformID}, {encodingID})";
                //MessageBox.Show($"{platformID}   {platformSpecificID}");

                pos = this._reader.Position;

                if (platformID == 3)
                {
                    this._reader.Seek(offset + this._cMapTableOffset);

                    var cMapFormat = this._reader.GetUInt16();
                    if (cMapFormat == 4)
                    {
                        _cMapFormat4 = new CMapFormat4(this._reader);
                    }
                    if (cMapFormat == 12)
                    {
                        _cMapFormat12 = new CMapFormat12(this._reader);
                    }
                    this._reader.Seek(pos);
                }
                if (platformID == 0)
                {
                    this._reader.Seek(offset + this._cMapTableOffset);
                    var cMapFormat = this._reader.GetUInt16();
                    if (cMapFormat == 14)
                    {
                        _cMapFormat14 = new CMapFormat14(this._reader);
                    }
                    this._reader.Seek(pos);
                }
            }
        }
    }
}
