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
    public class MultipartBoundary
    {
        public MultipartBoundary(MultipartStream boundaryStream)
        {
            Value = boundaryStream;
            ExtractHeaders();
        }

        private const byte LF = (byte)'\n';
        private const byte CR = (byte)'\r';       
		public string ContentType { get; private set; }
        public string Filename { get; private set; }
        public string Name { get; private set; }
        public MultipartStream Value { get; private set; }

        private void ExtractHeaders()
        {
            while(true)
            {
                var header = ReadLine();
                if (string.IsNullOrEmpty(header))
                    break;

                if (header.StartsWith("Content-Disposition", StringComparison.CurrentCultureIgnoreCase))
                {
                    Name = Regex.Match(header, @"name=""(?<name>[^\""]*)", RegexOptions.IgnoreCase).Groups["name"].Value;
                    Filename = Regex.Match(header, @"filename=""(?<filename>[^\""]*)", RegexOptions.IgnoreCase).Groups["filename"].Value;
                }

                if (header.StartsWith("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                {
                    ContentType = header.Split(new[] { ' ' }).Last().Trim();
                }
            }

            Value.PositionStartAtCurrentLocation();
        }

        private string ReadLine()
        {
            var readBuffer = new StringBuilder();
            while (true)
            {
                var byteReadFromStream = Value.ReadByte();
                if (byteReadFromStream == -1)
                    return null; 
                if (byteReadFromStream.Equals(LF))
                    break;
                readBuffer.Append((char)byteReadFromStream);
            }
           	return readBuffer.ToString().Trim(new[] { (char)CR });
        }
    }

}