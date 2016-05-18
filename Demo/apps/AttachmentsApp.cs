using System;
using System.Collections.Generic;
using Catalyst.App;
using Catalyst.Http;
using Catalyst;
using System.Linq;
using System.IO;

namespace Demo
{
    public class AttachmentsApp : App 
	{
		public AttachmentsApp() 
		{
			Get("/new", request => 
			{
				return "upload a file";			
			});

			Post("/new", request => 
			{
				var file = new File ();
					
				foreach(var uploaded in request.GetFiles()) 
				{
					using (var stream = new MemoryStream())
				    {
						uploaded.Value.CopyTo(stream);	
						file.Name = uploaded.Filename.ToLower();
						file.ContentType = uploaded.ContentType.ToLower();
	      				file.Contents =  stream.ToArray();
						DemoApp.Db.Insert (file);
				    }
				}		
				return new Json(new { name = file.Name, id = file.Id, contenttype = file.ContentType, url = request.Url ("/file?id={0}", file.Id)});
			});

			Get("/file", request =>
			{
				var file = DemoApp.Db.Find<File>((int)request.Query("id"));
				if(file == null)
					return 404;

				return new Response { Status = 200, Out = new MemoryStream(file.Contents), Headers = {{"content-type", file.ContentType}} };
			});

			Get("/", request => 
			{
				return 
					new Tag("ul", DemoApp.Db.Table<File>().Select(file => 
						new Tag("li", new Tag("a", file.Name, href: request.Url ("/file?id={0}", file.Id)))));	
			});
		}
	}
}