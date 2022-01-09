using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace TrueTypeFont.IO
{
    public class TTFReader : IDisposable
    {
        private BinaryReader _binaryReader;
        public long Position { get => this._binaryReader.BaseStream.Position; }
        public TTFReader(BinaryReader binaryReader)
        {
            this._binaryReader = binaryReader;
        }
        private byte[] Reverse(byte[] buffer)
        {
            Array.Reverse<byte>(buffer);
            return buffer;
        }
        public void Seek(long position)
        {
            if (!this._binaryReader.BaseStream.CanSeek)
                return;
            this._binaryReader.BaseStream.Position = position;
        }
        public void GoBack() => --this._binaryReader.BaseStream.Position;
        public void GoForword() => ++this._binaryReader.BaseStream.Position;
        public byte GetUInt8()
        {
            return this._binaryReader.ReadByte();
        }
        public ushort GetUInt16()
        {
            byte[] buffer = this.Reverse(this._binaryReader.ReadBytes(2));

            ushort number = 0;

            return buffer.Length < 2 ? number : BitConverter.ToUInt16(buffer);
        }
        public short GetInt16()
        {
            byte[] buffer = this.Reverse(this._binaryReader.ReadBytes(2));

            short number = 0;

            return buffer.Length < 2 ? number : BitConverter.ToInt16(buffer);
        }
        public short GetInt16SED()
        {
            byte[] buffer = this._binaryReader.ReadBytes(2);

            short number = 0;

            return buffer.Length < 2 ? number : BitConverter.ToInt16(buffer);
        }
        public uint GetUInt32()
        {
            byte[] buffer = this.Reverse(this._binaryReader.ReadBytes(4));

            uint number = 0;

            return buffer.Length < 4 ? number : BitConverter.ToUInt32(buffer);
        }
        public int GetInt32()
        {
            byte[] buffer = this.Reverse(this._binaryReader.ReadBytes(4));

            int number = 0;

            return buffer.Length < 4 ? number : BitConverter.ToInt32(buffer);
        }
        public long GetInt64()
        {
            byte[] buffer = this.Reverse(this._binaryReader.ReadBytes(8));

            long number = 0;

            return buffer.Length < 8 ? number : BitConverter.ToInt64(buffer);
        }
        public ulong GetUInt64()
        {
            byte[] buffer = this.Reverse(this._binaryReader.ReadBytes(8));

            ulong number = 0;

            return buffer.Length < 8 ? number : BitConverter.ToUInt64(buffer);
        }
        public short GetFWord()
        {
            return this.GetInt16();
        }
        public ushort GetUFWord()
        {
            return this.GetUInt16();
        }
        public ushort GetOffset16()
        {
            return this.GetUInt16();
        }
        public uint GetOffset32()
        {
            return this.GetUInt32();
        }
        public short GetF2Dot14()
        {
            return (short)(this.GetInt16() / (1 << 14));
        }
        public float GetFixed()
        {
            return this.GetInt32() / (1 << 16);
        }
        public string GetSting(int length)
        {
            string s = string.Empty;

            for (int i = 0; i < length; i++)
            {
                s += (char)this.GetUInt8();
            }
            return s;
        }
        public DateTime GetDate()
        {
            DateTime dateTime = new DateTime(1904, 1, 1);
            var macTime = this.GetInt64();
            var utcTime = macTime * 1000 + dateTime.Ticks;
            return new DateTime(macTime);
        }
        public void Dispose()
        {
            this._binaryReader.Dispose();
        }
    }
}
