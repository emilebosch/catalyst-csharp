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
	
	public class MultipartParser
    {
        private const byte LF = (byte)'\n';
        private readonly byte[] boundaryAsBytes;
        private readonly MultipartBuffer buffer;
        private readonly Stream requestStream;
        private readonly byte[] closingBoundaryAsBytes;

        public MultipartParser (Stream requestStream, string boundary)
		{
            this.requestStream = requestStream;
            boundaryAsBytes = GetBoundaryAsBytes(boundary, false);
            closingBoundaryAsBytes = GetBoundaryAsBytes(boundary, true);
            buffer = new MultipartBuffer(boundaryAsBytes, closingBoundaryAsBytes);
        }

        public IEnumerable<MultipartBoundary> GetBoundaries()
        {
            return (from boundaryStream in GetBoundarySubStreams()
                select new MultipartBoundary(boundaryStream)).ToList();
        }

        private IEnumerable<MultipartStream> GetBoundarySubStreams()
        {
            var boundarySubStreams = new List<MultipartStream>();
            var boundaryStart = GetNextBoundaryPosition();

            while (HasMoreBoundaries(boundaryStart))
            {
                var boundaryEnd = GetNextBoundaryPosition();
                boundarySubStreams.Add(new MultipartStream(requestStream, boundaryStart, GetActualEndOfBoundary(boundaryEnd)));
                boundaryStart = boundaryEnd;
            }
			
            return boundarySubStreams;
        }
		
        private bool HasMoreBoundaries(long boundaryPosition)
        {
            return boundaryPosition > -1 && !buffer.IsClosingBoundary;
        }

        private long GetActualEndOfBoundary(long boundaryEnd)
        {
            if (CheckIfFoundEndOfStream())        
                return requestStream.Position - (buffer.Length + 2);           
            return boundaryEnd - (buffer.Length + 2);
        }

        private bool CheckIfFoundEndOfStream()
        {
            return requestStream.Position.Equals(requestStream.Length);
        }

        private static byte[] GetBoundaryAsBytes(string boundary, bool closing)
        {
            var b = new StringBuilder();
            b.Append("--");
            b.Append(boundary);
            
            if(closing)
            {
                b.Append("--");
            }
            else
            {
                b.Append('\r');
                b.Append('\n');
            } 
            return Encoding.ASCII.GetBytes(b.ToString());;
        }

        private long GetNextBoundaryPosition()
        {
            buffer.Reset();
            while(true)
            {
                var byteReadFromStream = requestStream.ReadByte();
                if (byteReadFromStream == -1)
                    return -1;
  
                buffer.Insert((byte)byteReadFromStream);

                if (buffer.IsFull && (buffer.IsBoundary || buffer.IsClosingBoundary))
                    return requestStream.Position;

                if (byteReadFromStream.Equals(LF) || buffer.IsFull)
                {
                    buffer.Reset();
                }
            }
        }
    }

}