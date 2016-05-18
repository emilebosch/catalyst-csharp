using System;
using XCSS3SE;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Catalyst.Http.Test
{
	public static class XmlElementExtensions 
	{
		public static IEnumerable<XmlElement> WithText (this IEnumerable<XmlElement> query, string name)
		{
			return query.Where(x=>x.InnerText==name);
		}
		
		public static string Attr (this XmlElement element, string name)
		{
			return element.GetAttribute(name);
		}
	}
}
