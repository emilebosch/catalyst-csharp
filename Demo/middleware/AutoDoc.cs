using Catalyst.Http;
using System;
using Catalyst.Tilt;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace Demo
{
    public class AutoDoc : ICallable
    {
        ICallable next = null;
        public string Folder { get; set; }
		public Dictionary<string,ITemplate<object>> cache = new Dictionary<string,ITemplate<object>>();

        public AutoDoc(ICallable app)
        {
            Folder = "/docs";
            next = app;
        }

        public IResponse Call (IRequest env)
		{
			var name = env.Path.AbsolutePath;
			if (name.StartsWith (Folder)) 
			{
				if(name==Folder+"/")
					name = Folder+"/index";

				var file = AppDomain.CurrentDomain.BaseDirectory +"../"+ name;
				
				ITemplate<object> temp;

#if DEBUG
				cache.Clear();
#endif
				cache.TryGetValue (file, out temp);
				
				if(temp==null) 
				{
	                temp = Tilt.Create<object>(file);
	                if (temp == null)
	                    throw new Exception("No document found by name");
	
					cache.Add(file, temp);
				}

                var document = RenderDocument(temp, name);
				return new TextResponse(document) { Status=200, Headers= {{"content-type","text/html"}}};
            }
            return next.Call(env);
        }

        private string RenderDocument (ITemplate<object> template, string name)
		{
			var html = new StringBuilder ();
			html.Append ("<link rel='stylesheet' type='text/css' href='/public/doc.css'/>");
			html.Append ("<div class='nav'><ul>");
			foreach (var file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory+"../"+Folder))
            {
				var x = Path.GetFileNameWithoutExtension(file);
                html.AppendFormat("<li><a href='{0}'>{1}</a></li>",Folder+"/"+x , x);
            }
            html.AppendFormat("</ul></div><div class='doc'>{0}</div>", template.Render<object>(null));
            return html.ToString();
        }
    }
}
