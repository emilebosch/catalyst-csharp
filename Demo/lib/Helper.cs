using System;
using Catalyst.Http;

namespace Demo
{	
	public interface IHelper 
	{
		IRequest Request { get; set;}	
	}
		
	public static class UrlHelpers 
	{
		public static string Url (this IHelper target, string url, params object[] obj)
		{
			return target.Request.Url (url, obj);	
		}	
		
		public static string Url(this IRequest target, string url, params object[] obj) 
		{
			if (target.Data.ContainsKey ("script_name")) {
				return String.Format(target.Data ["script_name"] + url,obj);
			}
			return String.Format(url,obj);	
		}
		
		public static string Link (this IHelper target, string url, string name)
		{
			return "<a href='"+url+"'>"+name+"</a>";
		}
	}

	public class HelperModel<T> : IHelper
	{
		public IRequest Request { get; set;}
		public T Model;
	}
}

