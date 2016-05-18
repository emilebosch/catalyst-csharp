using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Catalyst
{
    public static class FeatureHelper
    {
        public static void HasSelectedOption(this Feature f, string name)
        {
            var option = f.Page.Body["option:selected"].FirstOrDefault(a => a.Attribute["value"] == name);
            if (option == null) throw new ContentNotExpectedException(f.Page, "Can't find selected element with value: " + name);
        }

		public static void SelectOption(this Feature f, string labelname, string with = "")
        {
            var input = FindElementByLabel(f, labelname);
            f.Form.Add(input.Attribute["name"], with);
        }
		
        public static void Select(this Feature f, string name)
        {
            var radio = FindElementByLabel(f, name);
            var id = radio.Attribute["id"];

            var attrName = radio.TryGetAttribute("name");
            var attrValue = radio.TryGetAttribute("value");

            if (attrName == null)
                throw new ContentNotExpectedException(f.Page, "The element with id '" + id + "' has no name attribute");

            if (attrValue == null)
                throw new ContentNotExpectedException(f.Page, "The element with id '" + id + "' has no value attribute");

            f.Form.Add(radio.Attribute["name"], radio.Attribute["value"]);
        }
    }
}
