using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catalyst {
	public class Tag
	{
		List<Tag> innerTags = new List<Tag>();
		List<Tag> siblingTags = new List<Tag>();
		Dictionary<string,string> Attrs = new Dictionary<string, string>();
		string tag, content;

		public Tag(
			string tag, 
        	string content = null, 
			string name = null,
			string type = null,
			string href = null,
			bool? @checked = null,
			string title = null, 
			string value = null,
			string src = null,
			string @class = null,
			string @for = null,
			string id = null,
			string target = null,
			string alt = null,
			string placeholder = null, 
			string rel = null, 
			string min = null,
			string required = null, 
			string max = null, 
			string style = null, 
			string accesskey = null, 
			string contenteditable = null,
			string contextmenu = null, 
			string dir = null, 
			string draggable = null, 
			string dropzone = null, 
			string hidden = null,
			string lang = null, 
			string spellcheck = null, 
			string tabindex = null, 
			string autocomplete = null, 
			string novalidate = null, 
			string autofocus = null, 
			string form = null,
			string formaction = null, 
			string formenctype = null, 
			string formmethod = null, 
			string formnovalidate = null, 
			string formtarget = null, 
			string height = null, 
			string width = null, 
			string list = null,
			string label=null,
			bool? multiple = null,
			bool? selected = null,
			string pattern = null, 
			string step = null,
			IEnumerable<Tag> children=null,
			params Tag[] childTags) 
		{
			this.tag = tag;
			//Split shorthand

			if(this.tag[0]=='.'||this.tag[0]=='#') 
				this.tag = "div"+this.tag;
	
			var split = this.tag.Split (new char[] {'#'}, 2);
			if (split.Length > 1) 
			{
				this.tag = split [0];
				Attr("id", split [1]);
			}

			split = this.tag.Split (new char[] {'.'}, 2);
			if (split.Length > 1) 
			{	
				this.tag = split [0];
				Attr("class", split [1]);
			}

			this.content = content;

			Attr("name"			 	,name);
			Attr("type"			 	,type);
			Attr("for"			 	,@for);
			Attr("class"		 	,@class);
			Attr("href"				,href);
			Attr("target"			,target);
			Attr("value"			,value);
			Attr("rel"				,rel);
			Attr("src"				,src);
			Attr("alt"				,alt);
			Attr("title"			,title);
			Attr("style"			,style);
			Attr("id"				,id);
			Attr("label"			,label);
	
			if(@checked.HasValue)
				Attr("checked"		,@checked.Value?"checked":null);

			if(@multiple.HasValue)
				Attr("multiple"		,@multiple.Value?"multiple":null);

			if(@selected.HasValue)
				Attr("selected"		,@selected.Value?"selected":null);

			//HTML 5 global
			Attr("accesskey"		,accesskey);	
			Attr("contenteditable"	,contenteditable);	
			Attr("contextmenu"		,contextmenu);	
			Attr("dir"				,dir);	
			Attr("draggable"		,draggable);	
			Attr("dropzone"			,dropzone);	
			Attr("hidden"			,hidden);	
			Attr("lang"				,lang);	
			Attr("spellcheck"		,spellcheck);	
			Attr("tabindex"			,tabindex);

			//HTML 5 forms
			Attr("autocomplete"		,autocomplete);	
			Attr("novalidate"		,novalidate);	
			Attr("autofocus"		,autofocus);
			Attr("form"				,form);
			Attr("formaction"		,formaction);
			Attr("formenctype"		,formenctype);
			Attr("formmethod"		,formmethod);
			Attr("formnovalidate"	,formnovalidate);
			Attr("formtarget"		,formtarget);
			Attr("height"			,height);
			Attr("width"			,width);
			Attr("list"				,list);
			Attr("min"		 		,min);
			Attr("max"		 		,max);
			Attr("pattern"			,pattern);
			Attr("placeholder"		,placeholder);
			Attr("step"				,step);

			this.innerTags.AddRange(childTags);
			if(children!=null)
			this.innerTags.AddRange(children);
		}

		public Tag(string tag, params Tag[] children) : this(tag , childTags:children, content:null, name:null)
		{
		}

		public Tag(string tag, IEnumerable<Tag> children) : this(tag , childTags:children.ToArray(), content:null, name:null)
		{
		}

		public Tag Attr(string name, object value) 
		{
			if(value!=null) 
			{
				if(!Attrs.ContainsKey(name))
					Attrs.Add(name,value.ToString());
				else
					Attrs[name]=value.ToString();
			}
			return this;
		}

		public override string ToString ()
		{
			var tagHtml = new StringBuilder();
			tagHtml.AppendFormat("<{0}",tag);

			if(Attrs!=null && Attrs.Count!=0) 
				foreach(var attr in Attrs) 
					tagHtml.AppendFormat(" {0}='{1}'",attr.Key,attr.Value);

			if(innerTags.Count()==0 && content==null) 
			{
				tagHtml.Append("/>");
				foreach(var nextTag in siblingTags) 	
					tagHtml.Append(nextTag.ToString());	

				return tagHtml.ToString();
			}

			tagHtml.Append(">");

			if(content!=null)
				tagHtml.Append(content);

			foreach(var currentTag in innerTags) 
				tagHtml.Append(currentTag.ToString());

			tagHtml.AppendFormat("</{0}>",tag);
			foreach(var nextTag in siblingTags) 	
				tagHtml.Append(nextTag.ToString());	

			return tagHtml.ToString();
		}

		public static implicit operator Tag(string shorthand) 
   		{
			return new Tag(shorthand);
  		}

		 public static Tag operator +(Tag t1, Tag t2) 
		 {
			t1.siblingTags.Add(t2);
			return t1;
		 }
	}
}