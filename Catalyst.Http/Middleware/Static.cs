using System.IO;
using System;
using System.Threading;

namespace Catalyst.Http
{
    public class Static : ICallable
    {
        ICallable next;
		string baseDirectory;

        public Static (ICallable app)
		{
			Folder = "public";
			next = app;
			baseDirectory = AppDomain.CurrentDomain.BaseDirectory + "../";
        }

        public IResponse Call (IRequest env)
		{
			if (env.Path.AbsolutePath.StartsWith ("/" + Folder)) {
				var x = baseDirectory + env.Path.AbsolutePath;
                return new Response { Out = File.OpenRead(x), Status = 200 };
            }
            return next.Call(env);
        }

        public string Folder { get; set; }
    }
}
