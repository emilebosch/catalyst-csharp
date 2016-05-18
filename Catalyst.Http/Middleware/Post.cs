using System;
using System.IO;
using System.Collections.Specialized;
using System.Web;


namespace Catalyst.Http
{
	public class Post : ICallable
	{
		ICallable next;
		
		public Post (ICallable callable)
		{
			next = callable;
		}
		
		public IResponse Call (IRequest env)
		{
			if (env.Headers.ContainsKey ("Content-Type") 
				&& env.Headers ["Content-Type"] == "application/x-www-form-urlencoded") {		

				using (var reader = new StreamReader(env.InputStream)) 
				{
					env.SetPostData (HttpUtility.ParseQueryString (reader.ReadToEnd ()));
				}			
			}	                                                                    	
			return next.Call (env);
		}
	}
	
	public static class PostDataExtensions 
	{
		public static NameValueCollection GetPost (this IRequest request)
		{
			return request.Data ["postdata"] as NameValueCollection;
		}	
		
		//TODO: ?? We are setting it here directly but no actual postdat is being submitted.
		//Should we fix this.
		public static void SetPostData (this IRequest request, NameValueCollection collection)
		{
			request.Data ["postdata"] = collection;
		}
	}
}

