using System.IO;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Web;

namespace Catalyst.Http
{	
	public class Multipart : ICallable 
	{
		ICallable next;
		
		public Multipart (ICallable call)
		{
			next = call;
		}
		
		public IResponse Call (IRequest env)
		{		
			if (!env.Headers.ContainsKey ("Content-Type"))
				return next.Call (env);
				
			var match = Regex.Match (env.Headers ["Content-Type"], "boundary=(.*)");
			if (match.Success) 
			{	
				using (var fs = File.Create (Path.GetTempFileName ())) 
				{			
					env.InputStream.CopyTo (fs);
					fs.Seek (0, SeekOrigin.Begin);
							
					var postdata = new NameValueCollection ();
					var parser = new MultipartParser (fs, match.Groups [1].Value);
					
					foreach (var boundary in parser.GetBoundaries()) 
					{
						if (boundary.ContentType != null)
							continue;
						
						using (var reader = new StreamReader(boundary.Value)) 
						{
							postdata.Add (boundary.Name, HttpUtility.UrlDecode (reader.ReadToEnd ()));
						}					
					}

					env.SetPostData (postdata);
					env.SetFiles (parser);
					fs.Seek (0, SeekOrigin.Begin);

					return next.Call (env);
				}	
			}
			return next.Call (env);
		}
	}
	
	public static class MultipartExtensions 
	{
		public static IEnumerable<MultipartBoundary> GetFiles (this IRequest request)
		{
			var parser = request.Data ["multipart"] as MultipartParser;		
			foreach (var p in parser.GetBoundaries()) 
			{
				if (p.ContentType != null)
					yield return p;
			}		
			yield break;
		}
		
		public static void SetFiles (this IRequest request, MultipartParser parser)
		{
			request.Data ["multipart"] = parser;	
		}
	}

}