using System.Collections.Generic;
using System;
using System.IO;

namespace Catalyst.Http
{
    public interface IRequest 
    {
		string Method {get;set;}
        Dictionary<string, string> Headers { get; set; }
        Uri Path { get; set; }
		Stream InputStream { get; set;}
		Dictionary<string,object> Data {get;set;}
    }

    public interface IResponse
    {
        Dictionary<string, string> Headers { get; set; }
        Stream Out { get; set; }
        int Status { get; set; }
        string Body { get; set; }
    }

    public interface ICallable 
    {
        IResponse Call(IRequest env);
    }
}
