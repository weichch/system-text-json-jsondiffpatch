using System.Buffers;
using System.Text.Json.Nodes;

namespace System.Text.Json
{
    internal class JsonBytes : IBufferWriter<byte>, IDisposable
    {
        private const int DefaultBufferSize = 256;
        private byte[]? _buffer;
        private int _bufferHead;
        private bool _isDisposed;
        private readonly bool _writable;

        private static readonly JsonBytes Empty = new(false);

        private JsonBytes(bool writable)
        {
            _writable = writable;
        }

        public Utf8JsonReader GetReader() => _buffer is null
            ? default
            : new Utf8JsonReader(_buffer.AsSpan(0, _bufferHead));

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
            oldBuffer.AsSpan(0, _bufferHead).CopyTo(_buffer.AsSpan());
            oldBuffer.AsSpan(0, _bufferHead).Clear();
            ArrayPool<byte>.Shared.Return(oldBuffer);
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

            _buffer.AsSpan(0, _bufferHead).Clear();
            ArrayPool<byte>.Shared.Return(_buffer);
            _buffer = null;
            _bufferHead = 0;
        }

        public static JsonBytes FromNode(JsonNode? node, JsonSerializerOptions? serializerOptions)
        {
            if (node is null)
            {
                return Empty;
            }

            var jsonBytes = new JsonBytes(true);
            using var writer = new Utf8JsonWriter(jsonBytes);
            node.WriteTo(writer, serializerOptions);
            writer.Flush();
            return jsonBytes;
        }
    }
}
