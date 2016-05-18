using System;
using System.Collections.Generic;

namespace Catalyst.Http
{
    public class WebBrowser
    {
        ICallable app;
		
        public WebBrowser(ICallable target)
        {
            app = target;
        }

        public IResponse Get(string path, Action<IRequest> request = null, Action<IResponse> response = null)
        {
            var req = new Request { Path = new Uri(path), Headers = new Dictionary<string, string>() };
			
			if(request!=null)
            	request(req);
           
			var rs = app.Call(req);          
            if (response != null)
                response(rs);
            return rs;
        }
		
		public IResponse Post(string path, Action<IRequest> request = null, Action<IResponse> response = null)
        {
            var req = new Request { Path = new Uri(path), Headers = new Dictionary<string, string>() };			
			if(request!=null)
            	request(req);
           
			var rs = app.Call(req);          
            if (response != null)
                response(rs);
            return rs;
        }
    }	
}