using System;
using System.Collections.Generic;
using Catalyst.Http;
using Catalyst.Tilt;

namespace Catalyst.App
{
    public static class Extensions
    {
		private static Dictionary<string,object> viewCache = new Dictionary<string, object>();
		
		public static Response View (this IBase app, string name)
		{
			return app.View<object> (name, null);
		}
				
		public static Response  Redirect (this IBase app, string url)
		{
			return new Response {Status= 301, Headers = {{ "Location", url }} };	
		}
		
        public static Response View<T> (this IBase app, string name, T model)
		{
			object view = null;		

#if DEBUG
			viewCache.Clear();
#endif

			viewCache.TryGetValue (name, out view);		

			if (view == null) 
			{
				view = Tilt.Tilt.Create<T> (name);
				viewCache.Add (name, view);
			}

			return new Response { Status=200, Headers={{"content-type","text/html"}}, Body=((ITemplate<T>)(view)).Render(model)};
        }

        public static Response Render<T> (this IBase app, string name, IEnumerable<T> models)
		{
			var template = Tilt.Tilt.Create<T> (name);
			return new Response { Status=200, Headers={{"content-type","text/html"}}, Body= template.Render(models)};
        }

        public static IResponse File(this App app, string name) 
        {
            return null;
        }
    }
}
