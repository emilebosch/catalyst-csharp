using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Catalyst
{
	public static class Metadata
    {
        public static List<Field> GetFieldsFor(Type type, Func<PropertyInfo, Field> missing = null)
        {
            var metafields = new List<Field>();
            foreach (var property in type.GetProperties())
            {
                var metafield = GetField(property.Name.ToLower(),property.Name, property.PropertyType);
                if (metafield != null) 
				{
					metafield.Placeholder = property.Name;
                    metafields.Add(metafield);
				}
            }

			foreach (var field in type.GetFields())
            {
				var metafield = GetField(field.Name.ToLower(),field.Name,field.FieldType);
                if (metafield != null) 
				{
					metafield.Placeholder = field.Name;
                    metafields.Add(metafield);
				}
            }
            return metafields;
        }

		public static Dictionary<string, object> GetValuesFor(object target)
        {
            var values = new Dictionary<string, object>();
            foreach (var property in target.GetType().GetProperties())
                values.Add(property.Name.ToLower(), property.GetValue(target, null));

			foreach (var field in target.GetType().GetFields())
                values.Add(field.Name.ToLower(), field.GetValue(target));

			return values;
        }

		static Field GetField (string name, string fieldLabel, Type fieldType)
		{
	        if (fieldType == typeof(DateTime))
            {
                return new DateField { Name = name, Label = fieldLabel };
            } else if (fieldType== typeof(string))
            {
                return new TextField { Name = name, Label = fieldLabel };
            }
            else if (fieldType == typeof(bool))
            {
                return new BooleanField { Name = name, Label = fieldLabel};
            }
            else if (fieldType == typeof(Location))
            {
                return new LocationField { Name = name, Label = fieldLabel};
            }
            else if (fieldType == typeof(int))
            {
                return new NumericField { Name = name, Label = fieldLabel };
            }
            else if (fieldType.IsEnum || (fieldType.IsArray && fieldType.GetElementType().IsEnum))
            {
                var elementtype = !fieldType.IsArray ? fieldType :fieldType.GetElementType();
                var names = Enum.GetNames(elementtype);
                var values = Enum.GetValues(elementtype);
                var options = new Collection();

				int i = 0;
                foreach (var o in values)
                    options.Add(((int)o).ToString(), names[i++].ToString());

                return new CollectionField { Name = name, Collection = options, Label = fieldLabel, Multiple = fieldType.IsArray };
            }
            return null;
		} 

		public static void Update (SimpleHash hash, object target)
		{
			foreach (var prop in target.GetType().GetProperties()) 
			{	
				var name = prop.Name.ToLower ();
				if (!hash.ContainsKey(name))
					continue;

				var proptype = prop.PropertyType;
				var value= GetValueFromHash(proptype,name, hash);

				if(value!=null)
			   		prop.SetValue(target, value, null);
            }

			foreach (var prop in target.GetType().GetFields()) 
			{	
				var name = prop.Name.ToLower ();
				if (!hash.ContainsKey(name))
					continue;

				var proptype = prop.FieldType;
				var value= GetValueFromHash(proptype,name, hash);
				if(value!=null)
			    	prop.SetValue(target, value);
            }
        }

		public static object GetValueFromHash(Type proptype, string name, SimpleHash hash) 
		{
			object value =null;
		    if (proptype == typeof(string))
            {
				value=(string)hash[name];
            } 
			else  
            if (proptype == typeof(int))
            {
				value=(int)hash[name];
            }
            else
            if (proptype == typeof(Location))
            {
				value = new Location { Lattitude = (string)hash[name]["x"], Longitude = (string)hash[name]["y"] };
            }
            else
            if (proptype == typeof(bool))
            {
				value = (bool)hash[name];
            } 
			else
            if (proptype == typeof(DateTime))
            {
				value = DateTime.Parse((string)hash[name]);
            } 
			else
            if (proptype.IsEnum)
            {
                value = Enum.Parse(proptype, (string)hash[name], true);
            } 
			else
            if (proptype.IsArray && proptype.GetElementType().IsEnum)
            {
                var items = (string)hash[name];
				var elementType = proptype.GetElementType(); 
                if (items != null)
                {
                    var found = items.ToLower().Split(',').Select(elm => Enum.Parse(elementType, elm, true)).ToArray();
                    var array = Array.CreateInstance(elementType, found.Length);               
					for (var i = 0; i < found.Length; i++)
                        array.SetValue(found[i], i);
					value = array;
                } 
            }
			return value;
		}

		public static Relation[] GetRelationsFor(object target)
        {
            var list = new List<Relation>();
            foreach (var relation in target.GetType().GetFields())
            {
                if (relation.FieldType.IsGenericType && 
                    relation.FieldType.GetGenericTypeDefinition() == typeof(ICollection<>))
                {   
                    list.Add(new Relation
                    {
                        Name = relation.Name,
                        Type = relation.FieldType.GetGenericArguments()[0]
                    });
                }
            }
            return list.ToArray();
        }
    }
    
}
