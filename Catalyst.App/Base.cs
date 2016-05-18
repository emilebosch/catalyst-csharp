using System.Linq;
using Catalyst.Http;
using System.Collections.Generic;
using System;

namespace Catalyst.App
{
    public interface IMatcher
    {
        bool Match(IRequest request);
    }

    public interface IBase
    {
        Dictionary<IMatcher, Func<IRequest, object>> Routes { get; set; }
    }

    public class App : ICallable, IBase
    {
        public Dictionary<IMatcher,Func<IRequest,object>> Routes { get; set; }

		public void render(string name) 
		{
		}

		public void parm() {
		}	

		public void view() {
		}

        public App()
        {
            Routes = new Dictionary<IMatcher, Func<IRequest, object>>();
        }

        public void Get(string route, Func<IRequest, object> block)
        {
            Routes.Add(new DefaultMatcher(route,"GET"), block);
        }

        public void Post(string route, Func<IRequest, object> block)
        {
            Routes.Add(new DefaultMatcher(route, "POST"), block);
        }

        public void Delete(string route, Func<IRequest, object> block)
        {
            Routes.Add(new DefaultMatcher(route,"DELETE"), block);
        }

        public void Put(string route, Func<IRequest, object> block)
        {
            Routes.Add(new DefaultMatcher(route,"PUT"), block);
        }

        public IResponse Call (IRequest request)
		{
			var route = Routes.FirstOrDefault (a => a.Key.Match (request));
			if (route.Key == null) 
				throw new Exception ("No route found for request!");

			var response = route.Value (request);
            if (response is IResponse)
            {
                return (IResponse)response;
            }

			if(response is int) 
			{
				return new Response { Status = (int)response, Headers = { { "status", "ok" },{"content-type","text/html"} }, Body = response.ToString() };
			}

			return new Response { Status = 200, Headers = { { "status", "ok" },{"content-type","text/html"} }, Body = response.ToString() };
        }
    }

    public class DefaultMatcher : IMatcher 
    {
		public string Method;
        public DefaultMatcher(string route, string method)
        {
			Method=method;
            Route = route;
        }
        public string Route { get; set; }

        public bool Match(IRequest request)
        {
            return request.Path.AbsolutePath.Equals(Route, StringComparison.InvariantCultureIgnoreCase)
				&& request.Method==Method;
        }
    }
}
