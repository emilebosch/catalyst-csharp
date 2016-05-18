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
	
    public class MultipartBuffer
    {
        private readonly byte[] boundaryAsBytes;
        private readonly byte[] closingBoundaryAsBytes;
        private readonly byte[] buffer;
        private int position;

        public MultipartBuffer(byte[] boundaryAsBytes, byte[] closingBoundaryAsBytes)
        {
            this.boundaryAsBytes = boundaryAsBytes;
            this.closingBoundaryAsBytes = closingBoundaryAsBytes;
            buffer = new byte[boundaryAsBytes.Length];
        }

        public bool IsBoundary
        {
            get { return buffer.SequenceEqual(boundaryAsBytes); }
        }
 
		public bool IsClosingBoundary
        {
            get { return buffer.SequenceEqual(closingBoundaryAsBytes); }
        }

		public bool IsFull
        {
            get { return position.Equals(buffer.Length); }
        }

		public int Length
        {
            get { return buffer.Length; }
        }

		public void Reset()
        {
            position = 0;
        }

        public void Insert(byte value)
        {
            buffer[position++] = value;
        }
    }

}