using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AHK.TaskRunner
{
    public class LogReaderStream : Stream
    {
        private readonly Stream _stream;
        private int _remaining;
        private readonly byte[] _header = new byte[8];
        private readonly byte[] _buffer = new byte[BufferSize];

        const int BufferSize = 81920;

        public override bool CanRead => canRead;
        private bool canRead;

        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public LogReaderStream(Stream stream)
        {
            _stream = stream;
            canRead = true;
        }


        public override int Read(byte[] buffer, int offset, int count)
        {
            while (_remaining == 0)
            {
                for (var i = 0; i < _header.Length;)
                {
                    var n = _stream.Read(_header, i, _header.Length - i);
                    if (n == 0)
                    {
                        canRead = false;

                        if (i == 0)
                        {
                            // End of the stream.
                            return 0;
                        }

                        throw new EndOfStreamException();
                    }

                    i += n;
                }

                _remaining = (_header[4] << 24) |
                            (_header[5] << 16) |
                            (_header[6] << 8) |
                            _header[7];
            }

            var toRead = Math.Min(count, _remaining);
            var read = _stream.Read(_buffer, offset, toRead);
            if (read == 0)
            {
                canRead = false;
                return 0;
            }

            _remaining -= read;
            return read;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            while (_remaining == 0)
            {
                for (var i = 0; i < _header.Length;)
                {
                    var n = await _stream.ReadAsync(_header, i, _header.Length - i, cancellationToken).ConfigureAwait(false);
                    if (n == 0)
                    {
                        canRead = false;

                        if (i == 0)
                        {
                            // End of the stream.
                            return 0;
                        }

                        throw new EndOfStreamException();
                    }

                    i += n;
                }

                _remaining = (_header[4] << 24) |
                            (_header[5] << 16) |
                            (_header[6] << 8) |
                            _header[7];
            }

            var toRead = Math.Min(count, _remaining);
            var read = await _stream.ReadAsync(_buffer, offset, toRead, cancellationToken).ConfigureAwait(false);
            if (read == 0)
            {
                canRead = false;
                return 0;
            }

            _remaining -= read;
            return read;
        }

        protected override void Dispose(bool disposing) => _stream?.Dispose();

        public override void Flush() => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
