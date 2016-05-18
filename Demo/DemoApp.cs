using System.IO;
using Catalyst.Http;
using Catalyst;
using System;
using Mono.Posix;
using Catalyst.Http.Test;
using Mono.Unix;
using Mono.Unix.Native;
using System.Threading;
using System.Linq;
using System.Text;

namespace Demo
{
    class DemoApp
    {	
        static void Main (string[] args)
		{		
			//Set up the database
			
			DemoApp.Db = new DemoDb (Path.GetTempFileName ());		
			DemoApp.Db.CreateTable<File> ();
			
			//Set up the stack
			
			var stack = new Stack (b => 
			{		
				b.Use<UrlMap> (m => 
				{
					m.Map ("/attachments", new AttachmentsApp ());
					m.Map ("/state", new StateApp ());
					m.Map ("/blog", new BlogApp ());
					m.Map ("/admin", new AdminApp ());
					m.Map( "/worklogs", new WorklogApp());
				});
				
				b.Use<AutoDoc> ();
				b.Use<Multipart> ();
				b.Use<Post> ();
				b.Use<Static> ();
				b.Use<Deflate> ();
				b.Use<Logger> ();
			});	
			
			//Run the server		
			Directory.SetCurrentDirectory (AppDomain.CurrentDomain.BaseDirectory + "../");	

			var hostname = System.Environment.GetEnvironmentVariable ("HOST") ?? "localhost";
			var port = System.Environment.GetEnvironmentVariable ("PORT") ?? "8080";	
			
			Server.Run (stack,  "http://" + hostname + ":" + port, "http://0.0.0.0:" + port);
			
			//Block for signals i.e. heroku sends sigterm when app needs to terminate

			int which = UnixSignal.WaitAny (new []
			{
   		    new UnixSignal (Signum.SIGINT),
  			  new UnixSignal (Signum.SIGTERM),
			}, -1);

			Console.WriteLine ("Terminated via signal " + which);
        }
		public static DemoDb Db;
    }
}
