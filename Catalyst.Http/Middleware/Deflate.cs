using System.IO;
using System.IO.Compression;

namespace Catalyst.Http
{
    public class Deflate : ICallable
    {
        ICallable next;

        public Deflate(ICallable app)
        {
            next = app;
        }

        public IResponse Call (IRequest req)
		{
			var response = next.Call (req);
			if (response.Out == null)
				return response;

			response.Headers.Add ("Content-Encoding", "gzip");

			//TODO: Fix me properly. I.e. Get the output stream from the request. 
            var tempStream = new MemoryStream();
            using (var gzipStream = new GZipStream(tempStream, CompressionMode.Compress, true))
            {
                response.Out.CopyTo(gzipStream);
            }

            tempStream.Seek(0, SeekOrigin.Begin);
            response.Out = tempStream;
            return response;
        }
    }
}
