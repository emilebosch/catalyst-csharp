using System.Net;
using System;
using System.IO;
using System.Linq;

namespace Catalyst.Http
{
    public class Server
    {
        static HttpListener listener = new HttpListener();
        static ICallable root = null;

        static public void Run (ICallable rootApp, params Uri[] uri)
		{
			root = rootApp;
			uri.ToList ().ForEach (url => listener.Prefixes.Add (url.ToString ()));

			listener.Start ();
            listener.BeginGetContext(ProcessCallback, null);
			Console.WriteLine (listener.IsListening);
        }
		
		static public void Run (ICallable rootApp, params string[] urls)
		{
			Run (rootApp, urls.ToList ().Select (s => new Uri (s)).ToArray ());
		}

        static private void ProcessCallback (IAsyncResult result)
		{
            try
            {
                var context = listener.EndGetContext(result);
                listener.BeginGetContext(ProcessCallback, null);
                Process(context);
            }
            catch (HttpListenerException)
            {
                return;
            }   
        }

        private static void Process (HttpListenerContext context)
		{
			var request = new Request { Path = context.Request.Url, Method=context.Request.HttpMethod };
			request.InputStream = context.Request.InputStream;
			request.Headers = new System.Collections.Generic.Dictionary<string, string> ();
	
			request.Data["query"]=context.Request.QueryString;

			foreach (var header in context.Request.Headers.AllKeys)
				request.Headers.Add (header, context.Request.Headers [header]);

			var catalystResponse = root.Call(request);
            context.Response.StatusCode = catalystResponse.Status; 
           
            //Response
            foreach (var header in catalystResponse.Headers)
                context.Response.AddHeader(header.Key, header.Value);

            if (catalystResponse.Out != null)
            {
                using (var stream = context.Response.OutputStream)
                {
                    catalystResponse.Out.CopyTo(stream);
                }
            } 
            else
            {
                using (var writer = new StreamWriter(context.Response.OutputStream))
                {
                    writer.Write(catalystResponse.Body);
                }
            }
        }
    }
}
