using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Catalyst
{
    public class Form
    {
        public string Prefix { get; set; }
        public List<Field> Fields { get; set; }

        public Dictionary<string, object> Values { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; }

        public Form(object o, string prefix = null, int index = -1)
        {
            Prefix = prefix;

            if (index != -1)
            {
                Prefix += "[" + index + "]";
            }

            Fields = new List<Field>();
            Values = new Dictionary<string, object>();
            Errors = new Dictionary<string, List<string>>();

            GetMetadata(o.GetType());
            Populate(o);
        }

        public Form ChildForm(object o, string prefix = null, int index = -1)
        {
            return new Form(o, this.Prefix + "[" + prefix + "]", index);
        }

        public void GetMetadata(Type type, Func<PropertyInfo, Field> missing = null)
        {
            Fields = Metadata.GetFieldsFor(type, missing);
        }

        public void Populate(object target)
        {
            Values = Metadata.GetValuesFor(target);
            var validatable = target as IValidatable;
            if (validatable != null)
            {
                Errors = validatable.Errors;
                foreach (var error in Errors)
                {
                    var field = Field(error.Key);
                    field.Errors = error.Value;
                }
            }
        }

        public Field Field(string name)
        {
            return this.Fields.FirstOrDefault(field => field.Name == name);
        }

        public string Label(string name)
        {
            return FieldRenderer.RenderLabel(Field(name), Prefix);
        }

        public string Hint(string name, string with = null)
        {
            return FieldRenderer.RenderHint(Field(name));
        }

        public string Error(string name)
        {
            return FieldRenderer.RenderError(Field(name));
        }

        public string Input(string name, string with = null)
        {
            object val = null;
            Values.TryGetValue(name, out val);
            return FieldRenderer.RenderInput(Field(name), val, Prefix, with);
        }

        public string Render()
        {
			return FormRenderer.Render(this);
        }
    }

    public class Relation
    {
        public string Name { get; set; }
        public Type Type { get; set; }
    }    
}
