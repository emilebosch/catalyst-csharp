using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Catalyst.Http
{
    public class Response : IResponse
    {
        public Response()
        {
            Headers = new Dictionary<string, string>();
        }
        public Dictionary<string, string> Headers { get; set; }
        public int Status { get; set; }
        public string Body { get; set; }
        public Stream Out { get; set; }
    }

    public class TextResponse : Response
    {
        public TextResponse(string text)
        {
            Out = new MemoryStream();
            var writer = new StreamWriter(Out);
            writer.Write(text);
            writer.Flush(); 
            Out.Seek(0, SeekOrigin.Begin);
        }
    }

    public class Request : IRequest
    {
        public Dictionary<string, string> Headers { get; set; }
        public Uri Path { get; set; }
		public string Method {get;set;}
   	    public Stream InputStream { get; set; }	
		public Dictionary<string,object> Data { get; set;}
		
		public Request ()
		{
			Headers = new Dictionary<string, string> ();
			Data = new Dictionary<string, object> ();
		}		
    }
}
