using System;
using System.Text;

namespace Catalyst
{
    public class FormRenderer
    {
        public static string Render(SectionForm form, Form model)
        {
            var html = new StringBuilder();
            foreach (var section in form.Sections)
			{
                html.AppendLine("<div class='section'>");
                html.AppendLine("<h2>{0}</h2>".Inject(section.Title));
                html.AppendLine("<div class='notes'>{0}</div>".Inject(section.Description));
                foreach (var field in section.Fields)
                {
                    html.AppendLine(model.Label(field.Name));                   
					if (String.IsNullOrEmpty(field.With))

                        html.AppendLine(model.Input(field.Name));
                    else
                        html.AppendLine(model.Input(field.Name, with: field.With));

					html.AppendLine(model.Hint(field.Name));
                    html.AppendLine(model.Error(field.Name));
                }
                html.AppendLine("</div>");
            }
            return html.ToString();
        }

        public static string Render(Form model)
        {
            var html = new StringBuilder();
            foreach (var field in model.Fields)
            {
                html.AppendLine(model.Label(field.Name));
                html.AppendLine(model.Input(field.Name));
                html.AppendLine(model.Error(field.Name));
                html.AppendLine(model.Hint(field.Name));
            }
            return html.ToString();
        }
    }
}
