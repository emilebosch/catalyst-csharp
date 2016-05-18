using System;
using Catalyst.Http;
using Catalyst.App;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Catalyst
{

	public class Json : Response
    {
        public Json(object obj)
        {
			Status=200;
			Body = JsonConvert.SerializeObject(obj);
        }
    }

  public static class JsonExtensions
	{
	    public static IResponse Json(this IBase app, object obj) 
        {
			var r = JsonConvert.SerializeObject(obj);
			return new Response { Status=200, Headers={{"Content-type","text/json"}}, Body=r};	
        }
		
		public static T Json<T>(this IRequest request) 
        {
			using(var reader = new StreamReader(request.InputStream)) 
			{
				var json = reader.ReadToEnd();
				return JsonConvert.DeserializeObject<T>(json);
			}	
        }

		public static void WithJson(this IRequest req, object o) 
		{
			var r = JsonConvert.SerializeObject(o);
			req.InputStream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(r));	
		}
		
		public static T GetJson<T>(this IResponse req) 
		{	
			return JsonConvert.DeserializeObject<T>(req.Body);
		}
	
    }
}