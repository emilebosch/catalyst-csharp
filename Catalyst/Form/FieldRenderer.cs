using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catalyst
{
    public static class FieldRenderer
    {
        static Dictionary<string, Func<Field, object, string, string>> renders =  new Dictionary<string, Func<Field, object, string, string>>();

		static string Id (string name)
		{
			var id = name
			.Replace ("[", "_").Replace ("]", "_").Replace("__","_").TrimEnd('_').ToLower();
			return id;
		}  
		
        static FieldRenderer()
        {
            renders.Add("text", (field, value, name) =>
            {
				return new Tag("input",  placeholder:field.Placeholder, type:"text",  name: name, id:Id(name),  
					value:value as string,  @class: field.Errors.Any() ? "error" : null).ToString();
            });

            renders.Add("date", (field, value, name) =>
            {
				var picker = new Tag("input", type:"date",  
					placeholder:field.Placeholder, name: name, id:Id(name),  value:((DateTime)value).ToString("yyyy-MM-dd"), @class: field.Errors.Any() ? "error" : null);

				picker += new Tag("button","Select a date");
				return picker.ToString();
           });

            renders.Add("numeric", (field, value, name) =>
            {
				return new Tag("input", type:"number",  
					placeholder:field.Placeholder, name: name, id:Id(name), value:value.ToString(), @class: field.Errors.Any() ? "error" : null).ToString();
            });

            renders.Add("check", (field, value, name) =>
            {
				return new Tag("input", type:"checkbox", 
					name: name, id:Id(name), @checked: (bool)value, @class: field.Errors.Any() ? "error" : null).ToString();
            });

            renders.Add("memo", (field, value, name) =>
            {
				return new Tag("textarea", "" + value,  placeholder:field.Placeholder, name: name,  id:Id(name),  @class: field.Errors.Any() ? "error" : null).ToString();
            });

            renders.Add("select", (field, value, name) =>
            {
                var selected = ConvertAndGetValues(ref field, value, name);
                var choice = (CollectionField)field;

				var @select = new Tag("select", 
				    @class: field.Errors.Any() ? "error" : null,                  		
					name:name,
                	multiple:choice.Multiple, 
                	children: choice.Collection.Select(o => new Tag("option", o.Value, selected:selected.Contains(o.Key))));

				return @select.ToString();
            });

       		renders.Add("location", (field, value, name) => 
            {
				var loc = value as Location;
				var location = new Tag("input", name: name+"[x]", type:"text", value: loc.Lattitude);
				location += new Tag("input",  name: name+"[y]", type:"text", value: loc.Longitude);	
				return location.ToString();
            });

            renders.Add("radio", (field, value, name) =>
            {
                var selected = ConvertAndGetValues(ref field, value, name);
                var choice = (CollectionField)field;

				var radios = new Tag("span", choice.Collection.Select(o => 
					new Tag("input", @checked:  selected.Contains(o.Key), name:name, type:"radio", value:o.Key ,id:Id(name + o.Key)) +
				    new Tag("label", o.Value, @for: Id(name + o.Key))));

				return radios.ToString();
            });
        }

        private static List<string> ConvertAndGetValues(ref Field field, object value, string name)
        {
            var selected = new List<string>();
            if (field is BooleanField)
            {
                field = new CollectionField { Name = name, Collection = { { "1", "Yes" }, { "0", "No" } } };
                if (value != null && (bool)value)
                    selected.Add("1");
                else
                    selected.Add("0");
            }
            else
            {
				if(value==null)
					return selected;

                if (value is Array)
                {
                    foreach (var en in (Array)value)
                        selected.Add(((int)en).ToString());
                }
                else
                {
                    selected.Add(((int)value).ToString());
                }
        }
            return selected;
        }

        private static string TryFindDefault(Field field)
        {
            var name = field.Name.ToLower();
            if (field is BooleanField)
                return "check";

            if (field is CollectionField)
                return ((new[] { "sex", "gender" }).Contains(name) ? "radio" : "select"); ;

            if (field is TextField)
                return ((new[] { 
					"description", "comments", "comment", "text", "body" }).Contains(name) ? "memo" : "text");

            if (field is DateField)
                return "date";

            if (field is NumericField)
                return "numeric";

            if (field is LocationField)
            {
                return "location";
            }
            return null;
        }

        public static string RenderInput(Field field, object value, string prefix, string with = null)
        {
            var name = GetFieldName(field, prefix);
            Func<Field, object, string, string> render = null;

            if (with != null)
                return renders[with](field, value, name);
            else
                render = renders[TryFindDefault(field)];
   
            if (render != null)
                return render(field, value, name);

			return string.Empty;
        }

        private static string GetFieldName(Field field, string prefix)
        {
            if (prefix != null)
                return "{0}[{1}]".Inject(prefix, field.Name).ToLower();
            return "{0}".Inject(field.Name).ToLower();
        }

        public static string RenderLabel(Field field, string prefix)
        {
            return "<div><b><label for='{0}'>{2}{1}</label></b></div>".Inject(Id(GetFieldName(field, prefix)),
                !String.IsNullOrEmpty(field.Label) ? field.Label : field.Name, field.Required ? "*" : string.Empty,
                field.Errors.Any() ? "error" : "");
        }

        public static string RenderHint(Field input)
        {
            return "<i>{0}</i>".Inject(input.Hint);
        }

        public static string RenderError(Field input) 
        {
            if (!input.Errors.Any())
                return string.Empty;
            
            var builder = new StringBuilder();
            builder.Append("<ul>");
            foreach (var a in input.Errors)
            {
                builder.AppendFormat("<li>{0}</li>", a);
            }
            builder.Append("</ul>");
            return builder.ToString();
        }
    }
}
