using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Catalyst
{
	public class Model : IValidatable
	{
		protected Validator validator = new Validator();
		public virtual bool IsValid ()
		{
			return validator.IsValid();
		}

		public Dictionary<string, List<string>> Errors 
		{
			get { return validator.Errors; }
		}
	}


    public class Validator
    {
        Action<Validator> validator;
        Dictionary<string, List<string>> errors;
        public Dictionary<string, List<string>> Errors { get { return errors; } }

        public Validator(Action<Validator> onValidate = null)
        {
            errors = new Dictionary<string, List<string>>();
            validator = onValidate;
        }

        public bool IsValid()
        {
			if(validator!=null)
           		validator(this);
            return !Errors.Any();
        }

        public void ErrorIf(bool condition, string name, string errormessage, string id = null)
        {
            if (!condition)
				return;

            if (!errors.ContainsKey(name))
                errors.Add(name, new List<string>());
            errors[name].Add(errormessage);            
        }
    }

    public interface IValidatable
    {
        bool IsValid();
        Dictionary<string, List<string>> Errors { get; }
    }

    public static class ValidationExtensions
    {
        public static bool Between(this int target, int start, int end)
        {
            return (target >= start && target <= end);
        }

        public static bool IsNull(this object target)
        {
            return target == null;
        }

        public static bool IsNullOrEmpty(this string target)
        {
            return String.IsNullOrEmpty(target);
        }
    }

    public static class ModelExtensions 
    {
		public static bool Validate(this IValidatable target) 
		{
			bool valid=true;
			if(!target.IsValid())
				valid=false;

			foreach (var relation in Metadata.GetRelationsFor(target))
            {
 				var collectionField = target.GetType().GetField(relation.Name);
				var collection = collectionField.GetValue(target) as IEnumerable;

				foreach(var item in collection) 
				{
					var validatable = item as IValidatable;
					if(validatable!=null) 
						if(!validatable.Validate())
							valid=false;
				}
            }
			return valid;
		}

        public static void UpdateWithHash(this object target, SimpleHash hash)
        {
            hash.Update(target);
            foreach (var relation in Metadata.GetRelationsFor(target))
            {
                var relationHash = hash[relation.Name.ToLower()];
				if(relationHash==null)
					continue;
           
				var collectionField = target.GetType().GetField(relation.Name);
				var collection = collectionField.GetValue(target) as IEnumerable;

				var i = 0;
				foreach(var item in collection) 
				{
					var obj = relationHash[i++];
					item.UpdateWithHash(obj);
				}
            }
        }
    }
}
