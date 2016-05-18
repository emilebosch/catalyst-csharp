using System;
using XCSS3SE;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Catalyst.Http.Test
{
	public static class WebDriverExtensions 
	{
		public static void Visit (this WebDriver driver, string url, Action<IRequest> with=null)
		{
			driver.LastResponse = driver.Browser.Get (url, with);
			driver.Form.Clear ();
			
			if (driver.LastResponse.Status != 200 && driver.LastResponse.Status != 301) 
			{
				Console.WriteLine ("Visiting " + url + " gave status " + driver.LastResponse.Status);
				driver.Dump ();
			}
			
			driver.FollowRedirect ();
			driver.Document = new SharpQuery (driver.LastResponse.Body);
		}
		
		static void FollowRedirect (this WebDriver driver)
		{
			if (driver.LastResponse.Headers.ContainsKey ("Location")) 
				driver.Visit (driver.LastResponse.Headers["Location"]);
        }
		
		public static void Submit (this WebDriver driver, string text=null, Action<IRequest> with=null)
		{	
			var submit = driver.Document.Find ("input[type='submit']").FirstOrDefault ();
			Throw(submit==null,"Submit: Can't find a submit button");
			
			var node = submit;
			
			while (node !=null && node.Name !="form") 
			{
				node = node.ParentElement ();
			}
						
			string action = node.Attr ("action");
			//string method = node.Attr ("method");		
			
			//TODO: Try to find all the default values on the form and add it to the form collection
			
			//If the submit button has a name, add it to the form values
			if(submit.Attr ("name")!=null)
				driver.Form.Add (submit.Attr ("name"), submit.Attr ("value"));	
			
			driver.LastResponse = driver.Browser.Post (action, request =>
			{
				request.SetPostData (driver.Form);
				if (with != null)
					with (request);
			});		
			driver.Form.Clear ();
	
			if (driver.LastResponse.Status != 200 && driver.LastResponse.Status != 301) {
				Console.WriteLine ("Visiting " + action + " gave status " + driver.LastResponse.Status);
				driver.Dump ();
			}

			driver.FollowRedirect ();
			driver.Document = new SharpQuery (driver.LastResponse.Body);			
		}
		
		public static void Click (this WebDriver driver, string text)
		{
			var link = driver.Document.Find ("a").WithText (text).FirstOrDefault ();	
			Throw (link == null, "Click: Cannot find an a with the text {0} to click", text);
			driver.Visit (link.Attr ("href"));
		}

		static XmlElement FindElementByLabel (WebDriver driver, string labelname, string hint)
		{
			var label = driver.Document.Find ("label").WithText (labelname).FirstOrDefault ();			
			Throw (label == null, "{0}: Cannot find a label with the text '{1}'", hint, labelname);

			var refering = label.Attr ("for");
			var element = driver.Document.Find ("#" + refering).FirstOrDefault ();
			Throw (element == null, "{0}: Cannot find the element with id '{1}' where the label for '{2}' is refering to", 
			       hint, refering, labelname);	
			
			return element;
		}		

		public static void Pick (this WebDriver driver, string label) 
		{
			var element = FindElementByLabel (driver, label, "Pick");
			driver.Form.Add (element.Attr("name"), element.Attr("value"));
		}
		
		public static void Check (this WebDriver driver, string label) 
		{
			var element = FindElementByLabel (driver, label, "Check");
			driver.Form.Add (element.Attr("name"), element.Attr("value"));
		}
		
		public static void Fill (this WebDriver driver, string label, string with)
		{	
			var element = FindElementByLabel (driver, label, "Fill");		
			driver.Form.Add (element.Attr("name"), with);
		}
		
		public static void Select (this WebDriver driver, string label, string with)
		{
			var select = FindElementByLabel (driver, label, "Select");	
			Throw(select == null, "Cannot find the select where the label {0} is refering to or the label itself",label);	
			var option = select.SelectSingleNode("option[text()='"+with+"']");
			driver.Form.Add(select .Attr ("name"), option.Attributes["value"].Value);
		}
		
		public static void Text (this WebDriver driver, string contains=null, string doesntContain=null)
		{
			if (contains != null)
				Throw (!driver.LastResponse.Body.Contains (contains), "Text: Body text does not contain '{0}'", contains);
			
			if (doesntContain != null)
				Throw (driver.LastResponse.Body.Contains (doesntContain), "Text: Body text does contain '{0}', while it should not.", contains);
		}
		
		private static void Throw(bool condition, string message, params object[] x) 
		{
			if(condition)
				throw new Exception(String.Format(message,x));
		}
		
		public static void Dump (this WebDriver driver)
		{
			Console.WriteLine (driver.LastResponse.Body);	
		}
	}
}

