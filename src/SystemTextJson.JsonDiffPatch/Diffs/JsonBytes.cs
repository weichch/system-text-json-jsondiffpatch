using System.Buffers;
using System.Text.Encodings.Web;
using System.Text.Json.Nodes;

namespace System.Text.Json.Diffs
{
    internal class JsonBytes : IBufferWriter<byte>, IEquatable<JsonBytes>, IDisposable
    {
        private const int DefaultBufferSize = 256;
        private byte[]? _buffer;
        private int _bufferHead;
        private bool _isDisposed;
        private readonly bool _writable;

        private static readonly JsonWriterOptions WriterOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Indented = false,
            SkipValidation = true
        };

        private static readonly JsonBytes Empty = new(false);

        private JsonBytes(bool writable)
        {
            _writable = writable;
        }

        void IBufferWriter<byte>.Advance(int count)
        {
            _bufferHead += count;
        }

        Memory<byte> IBufferWriter<byte>.GetMemory(int sizeHint)
        {
            EnsureBufferSize(sizeHint);
            return _buffer!.AsMemory(_bufferHead);
        }

        Span<byte> IBufferWriter<byte>.GetSpan(int sizeHint)
        {
            EnsureBufferSize(sizeHint);
            return _buffer!.AsSpan(sizeHint);
        }

        private void EnsureBufferSize(int sizeHint)
        {
            if (!_writable)
            {
                throw new InvalidOperationException("Buffer is not writable.");
            }

            if (_isDisposed)
            {
                throw new ObjectDisposedException("this");
            }

            _buffer ??= ArrayPool<byte>.Shared.Rent(Math.Max(sizeHint, DefaultBufferSize));

            if (_bufferHead + sizeHint <= _buffer.Length)
            {
                // We have enough space in buffer
                return;
            }

            // Resize buffer
            var addition = sizeHint - (_buffer.Length - _bufferHead);
            var oldBuffer = _buffer;
            // Rent a new buffer
            // If size of bytes requires is less than the default buffer size, use the default buffer size
            // to reduce future rent
            _buffer = ArrayPool<byte>.Shared.Rent(_buffer.Length + Math.Max(addition, DefaultBufferSize));
            // Copy the old buffer
            oldBuffer.AsSpan().CopyTo(_buffer.AsSpan(0, _bufferHead));
            ArrayPool<byte>.Shared.Return(oldBuffer);
        }

        public bool Equals(JsonBytes other)
        {
            if (_writable != other._writable)
            {
                return false;
            }

            if (_buffer == other._buffer)
            {
                return true;
            }

            if (_buffer is null || other._buffer is null)
            {
                return false;
            }

            return _buffer.AsSpan(0, _bufferHead) ==
                   other._buffer.AsSpan(0, other._bufferHead);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            if (_buffer is null)
            {
                return;
            }

            ArrayPool<byte>.Shared.Return(_buffer);
            _buffer = null;
            _bufferHead = 0;
        }

        public static JsonBytes FromNode(JsonNode? node)
        {
            if (node is null)
            {
                return Empty;
            }

            var jsonBytes = new JsonBytes(true);
            using var writer = new Utf8JsonWriter(jsonBytes, WriterOptions);
            node.WriteTo(writer);
            writer.Flush();
            return jsonBytes;
        }
    }
}
