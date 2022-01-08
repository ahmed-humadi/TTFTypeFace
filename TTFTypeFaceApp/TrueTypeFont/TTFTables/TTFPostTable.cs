using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueTypeFont.IO;

namespace TrueTypeFont.TTFTables
{
    public class TTFPostTable
    {
        private TTFReader _reader;
        private long _glyfTableOffset;
        public long GlyfTableOffset
        {
            get { return _glyfTableOffset; }

        }
    }
}
