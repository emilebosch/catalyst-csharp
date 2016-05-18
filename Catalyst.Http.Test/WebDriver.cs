using System;
using System.Collections.Specialized;
using XCSS3SE;

namespace Catalyst.Http.Test
{
	public class WebDriver
	{
		public WebBrowser Browser; 
		public IResponse LastResponse;
		public SharpQuery Document;
		public NameValueCollection Form = new NameValueCollection();
		
		public WebDriver (ICallable app)
		{
			Browser = new WebBrowser(app);	
		}
		
		public void OnRequest (Action<IRequest> request)
		{	
		}	
	}
	
}
