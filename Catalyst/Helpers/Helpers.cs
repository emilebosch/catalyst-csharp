using System;
using System.Collections.Generic;
using Catalyst.Http;
using System.Collections.Specialized;

namespace Catalyst
{
    public static class Helpers
    {
        public static bool IsEmpty(this string str)
        {
            return String.IsNullOrEmpty(str);
        }

        public static string ToHuman(this DateTime time)
        {
            return time.ToString();
        }

        public static T Tap<T>(this T target, Action<T> t)
        {
            t(target);
            return target;
        }

        public static string Inject(this string f, params object[] p)
        {
            return String.Format(f, p);
        }

        public static T Add<T>(this ICollection<T> collection, T item)
        {
            collection.Add(item);
            return item;
        }

        public static Dictionary<string, string> ToDictionary(this IRequest r)
        {
			var namevalue = r.GetPost();
			var dictionary = new Dictionary<string, string>();
            foreach (var a in namevalue.AllKeys)
            {
                dictionary.Add(a, (string)namevalue[a]);
            }
            return dictionary;
        }

        public static SimpleHash Params(this IRequest r)
        {
            return SimpleHash.Parse(r.ToDictionary());
        }

		public static SimpleHash Query(this IRequest r)
        {
			var namevalue = r.Data["query"] as NameValueCollection;
			var dictionary = new Dictionary<string, string>();
            foreach (var a in namevalue.AllKeys)
            {
                dictionary.Add(a, (string)namevalue[a]);
            }
            return SimpleHash.Parse(dictionary);
        }
		public static SimpleHash Query(this IRequest r, string name)
        {
			return Query(r)[name];
        }
		
		 public static SimpleHash Params (this IRequest r, string name)
		{
			return SimpleHash.Parse (r.ToDictionary ()) [name] ?? new SimpleHash();
        }
    }


}
