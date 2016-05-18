using System;
using System.Collections.Generic;
using System.Text;
using NHaml;
using System.IO;
//using MarkdownSharp;

namespace Catalyst.Tilt
{
    // Markdown
//    public class MarkdownTemplate<X> : ITemplate<X>
//    {
//        Markdown markdown;
//
//        public void Compile(string file)
//        {
//            var dat = File.ReadAllText(file);
//            markdown = new Markdown(dat);
//        }
//
//        public string Render<T>(T obj)
//        {
//            return markdown.Parse();
//        }
//    }

    // Haml
    public class HamlTemplate<X> : ITemplate<X>
    {
        TemplateEngine engine = new TemplateEngine();
        CompiledTemplate template = null;

        public void Compile(string file)
        {
            template = engine.Compile(file, typeof(SimpleTemplate<X>));
        }

        public string Render<T>(T model)
        {
            var instance = template.CreateInstance() as SimpleTemplate<T>;
            instance.Model = model;

            var sw = new StringWriter(new StringBuilder());
            instance.Render(sw);
            return sw.ToString();
        }
    }

    public class SimpleTemplate<T> : Template
    {
        public T Model { get; set; }
    }

    // Interface

    public interface ITemplate<X>
    {
        void Compile(string file);
        string Render<T>(T obj);
    }

    public class Tilt
    {
        public static ITemplate<T> Create<T>(string file)
        {
            return LookupTemplate<T>(file);
        }

        private static ITemplate<T> LookupTemplate<T>(string name)
        {
            ITemplate<T> template = null;
            var files = Directory.EnumerateFiles(Directory.GetParent(name).ToString(), Path.GetFileName(name) + "*");
            foreach (var f in files)
            {
                var ext = Path.GetExtension(f);
                if (ext == ".haml")
                {
                    template = new HamlTemplate<T>();
                }
                else if (ext == ".md")
                {
                   // template = new MarkdownTemplate<T>();
                }
                if (template != null)
                {
                    template.Compile(f);
                    return template;
                }
            }
            return null;
        }
    }
}
