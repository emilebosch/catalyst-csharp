using System.IO;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Web;

namespace Catalyst.Http
{	
	
    public class MultipartStream : Stream
    {
        private readonly Stream stream;
        private long start;
        private readonly long end;
        private long position;

        public MultipartStream(Stream stream, long start, long end)
        {
            this.stream = stream;
            this.start = start;
            position = start;
            this.end = end;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return (end - start); }
        }

        public override long Position
        {
            get { return position - start; }
            set { position = Seek(value, SeekOrigin.Begin); }
        }

        private long CalculateSubStreamRelativePosition(SeekOrigin origin, long offset)
        {
            var subStreamRelativePosition = 0L;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    subStreamRelativePosition = start + offset;
                    break;

                case SeekOrigin.Current:
                    subStreamRelativePosition = position + offset;
                    break;

                case SeekOrigin.End:
                    subStreamRelativePosition = end + offset;
                    break;
            }
            return subStreamRelativePosition;
        }

        public void PositionStartAtCurrentLocation()
        {
            start = stream.Position;
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (count > (end - position))
                count = (int)(end - position);
 
            if (count <= 0)
                return 0;
 
            stream.Position = position;

            var bytesReadFromStream = stream.Read(buffer, offset, count);
            RepositionAfterRead(bytesReadFromStream);
            return bytesReadFromStream;
        }

        public override int ReadByte()
        {
            if (position >= end)
                return -1;

            stream.Position = position;

            var byteReadFromStream = stream.ReadByte(); 
            RepositionAfterRead(1);

            return byteReadFromStream;
        }

        private void RepositionAfterRead(int bytesReadFromStream)
        {
            if (bytesReadFromStream == -1)
                position = end;
            else
                position += bytesReadFromStream;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            var subStreamRelativePosition = 
                CalculateSubStreamRelativePosition(origin, offset);

            ThrowExceptionIsPositionIsOutOfBounds(subStreamRelativePosition);
            return stream.Seek(subStreamRelativePosition, SeekOrigin.Begin);
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException();
        }

        private void ThrowExceptionIsPositionIsOutOfBounds(long subStreamRelativePosition)
        {
            if (subStreamRelativePosition < 0 || subStreamRelativePosition > end)
                throw new InvalidOperationException();
        }
    }

}