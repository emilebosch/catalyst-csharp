using System;
using System.Collections.Generic;
using System.Text;

namespace Catalyst.Http
{
    public class UrlMap :  ICallable
    {
        Dictionary<string, ICallable> mappings = new Dictionary<string, ICallable>();

        public IResponse Call (IRequest env)
		{
			foreach (var mapping in mappings) 
			{
				if (!env.Path.AbsolutePath.StartsWith (mapping.Key)) 
					continue;
				
				var b = new UriBuilder (env.Path);
				b.Path = env.Path.AbsolutePath.Remove (0, mapping.Key.Length);
				env.Path = b.Uri;
				env.Data ["script_name"] = mapping.Key;
                return mapping.Value.Call(env); 
                
            }
			return new Response { Status=404 };
        }

        public void Map(string map, ICallable app)
        {
            mappings.Add(map, app);
        }
     }
}
