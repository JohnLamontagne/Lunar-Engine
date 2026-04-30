/** Copyright 2018 John Lamontagne https://www.rpgorigin.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/

using System;
using System.IO;
using System.Text;

namespace Lunar.Core.Net
{
    /// <summary>
    /// Transport-agnostic read/write buffer for serializing packet payloads.
    /// Wire format is fixed-width little-endian primitives plus length-prefixed UTF-8 strings,
    /// matching <see cref="System.IO.BinaryWriter"/>/<see cref="System.IO.BinaryReader"/> semantics.
    /// </summary>
    public sealed class Packet : IDisposable
    {
        private readonly MemoryStream _stream;
        private readonly BinaryWriter _writer;
        private readonly BinaryReader _reader;

        /// <summary>Construct an empty packet for writing.</summary>
        public Packet()
        {
            _stream = new MemoryStream();
            _writer = new BinaryWriter(_stream, Encoding.UTF8, leaveOpen: true);
            _reader = new BinaryReader(_stream, Encoding.UTF8, leaveOpen: true);
        }

        /// <summary>Construct a packet from existing wire bytes for reading. Position is set to 0.</summary>
        public Packet(byte[] data) : this(data, 0, data?.Length ?? 0) { }

        /// <summary>Construct a packet from a slice of wire bytes for reading. Position is set to 0.</summary>
        public Packet(byte[] data, int offset, int count)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            _stream = new MemoryStream();
            _stream.Write(data, offset, count);
            _stream.Position = 0;
            _writer = new BinaryWriter(_stream, Encoding.UTF8, leaveOpen: true);
            _reader = new BinaryReader(_stream, Encoding.UTF8, leaveOpen: true);
        }

        /// <summary>Current read/write cursor position in bytes.</summary>
        public long Position
        {
            get => _stream.Position;
            set => _stream.Position = value;
        }

        /// <summary>Total length of the buffer in bytes.</summary>
        public long LengthBytes => _stream.Length;

        public Packet Write(byte value) { _writer.Write(value); return this; }
        public Packet Write(sbyte value) { _writer.Write(value); return this; }
        public Packet Write(short value) { _writer.Write(value); return this; }
        public Packet Write(ushort value) { _writer.Write(value); return this; }
        public Packet Write(int value) { _writer.Write(value); return this; }
        public Packet Write(uint value) { _writer.Write(value); return this; }
        public Packet Write(long value) { _writer.Write(value); return this; }
        public Packet Write(ulong value) { _writer.Write(value); return this; }
        public Packet Write(float value) { _writer.Write(value); return this; }
        public Packet Write(double value) { _writer.Write(value); return this; }
        public Packet Write(bool value) { _writer.Write(value); return this; }
        public Packet Write(string value) { _writer.Write(value ?? string.Empty); return this; }
        public Packet Write(byte[] data) { _writer.Write(data); return this; }
        public Packet Write(byte[] data, int offset, int count) { _writer.Write(data, offset, count); return this; }

        /// <summary>Embed another packet's payload bytes inline (no length prefix).</summary>
        public Packet Write(Packet other)
        {
            if (other != null)
            {
                var bytes = other.ToArray();
                if (bytes.Length > 0)
                    _writer.Write(bytes);
            }
            return this;
        }

        public byte ReadByte() => _reader.ReadByte();
        public sbyte ReadSByte() => _reader.ReadSByte();
        public short ReadInt16() => _reader.ReadInt16();
        public ushort ReadUInt16() => _reader.ReadUInt16();
        public int ReadInt32() => _reader.ReadInt32();
        public uint ReadUInt32() => _reader.ReadUInt32();
        public long ReadInt64() => _reader.ReadInt64();
        public ulong ReadUInt64() => _reader.ReadUInt64();
        public float ReadFloat() => _reader.ReadSingle();
        public float ReadSingle() => _reader.ReadSingle();
        public double ReadDouble() => _reader.ReadDouble();
        public bool ReadBoolean() => _reader.ReadBoolean();
        public string ReadString() => _reader.ReadString();
        public byte[] ReadBytes(int count) => _reader.ReadBytes(count);

        /// <summary>Returns a copy of the packet's full payload bytes.</summary>
        public byte[] ToArray() => _stream.ToArray();

        public void Dispose()
        {
            _writer.Dispose();
            _reader.Dispose();
            _stream.Dispose();
        }
    }
}
