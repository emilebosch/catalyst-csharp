using System.Threading;
using System;
using System.Text;

namespace Catalyst.Http
{
    public class Logger : ICallable
    {
        ICallable next;
		
        public Logger(ICallable app)
        {
            next = app;
        }

        public IResponse Call (IRequest env)
		{
			try {
				var stopwatch = new System.Diagnostics.Stopwatch ();
				stopwatch.Start ();				
				var r = next.Call (env);
				stopwatch.Stop ();
				Console.WriteLine ("#{1} - {0} - {2}ms", env.Path.ToString () , Thread.CurrentThread.ManagedThreadId, stopwatch.ElapsedMilliseconds);
				return r;
            }
            catch (Exception exception)	
            {

				var message = RenderException(exception);				
                return new Response { Status = 500, Body = message, Headers = { { "Content-type", "text/html" },{ "Status", "Internal status error" } } };
            }
        }

		string RenderException (Exception exception)
		{
			var sb  =new StringBuilder();
			sb.AppendFormat("<h1>{0}</h1>",exception.Message);
			if(exception.InnerException!=null) 
			{
				sb.Append(exception.InnerException.Message);
			}
			sb.AppendFormat("<pre>{0}</pre>",exception.StackTrace);
			return sb.ToString();
		}
    }
}
