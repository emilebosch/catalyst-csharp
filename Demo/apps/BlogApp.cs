using System;
using SQLite;
using Catalyst.App;
using Catalyst.Http;
using Catalyst;

namespace Demo
{
    public class BlogApp : App
    {
        public BlogApp ()
		{	
			Get ("/new", request =>
			{
				return this.View ("views/new", new HelperModel<object> { Request = request});
			});	
			
			Post ("/create", request =>
			{	
				var product = new File (request.Params ("post"));			                                    				
				DemoApp.Db.Insert (product);	
				return this.Redirect (request.Url ("/"));
			});
			
			Get ("/", request =>
			{
				return this.View ("views/index", 
					new HelperModel<TableQuery<File>> { Request = request, Model = DemoApp.Db.Table<File> ()});
			});
        }
    }
}

