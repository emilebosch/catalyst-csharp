using System;

namespace Catalyst.Http
{
    public class BasicAuthorization : ICallable
    {
        ICallable next;
		public Func<string, string, bool> Authorize = null;
        public string Path { get; set; }
		
        public BasicAuthorization(ICallable app)
        {
            next = app;
        }

        public IResponse Call(IRequest env)
        {
            if (!env.Path.AbsolutePath.Contains(Path))
				  return next.Call(env);
            
            //Check if we have an authorize header
            if (env.Headers.ContainsKey("Authorization"))
            { 
                //Check if the username & password match -> Server replies with Basic Base64(User:Password)
                var auth = env.Headers["Authorization"].Split(new[] { ' ' }, 2);

                var encoded = System.Text.ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(auth[1]));
                var usernamepassword = encoded.Split(new []{':'}, 2);

                if (Authorize(usernamepassword[0], usernamepassword[1]))
                {
                    return next.Call(env);
                }              
            }
            return new Response { Headers = { { "WWW-Authenticate", "Basic realm=\"Authorize\"" } }, Status = 401, Body = "Unauthorized" };
        }
    }

    public static class BasicAuthorizationExtensions 
    {
        public static void WithAuthorization(this IRequest request, string name, string password)
        {
            request.Headers.Add("Authorization","Basic "+System.Convert.ToBase64String
				(System.Text.ASCIIEncoding.ASCII.GetBytes(name + ":" + password)));
        }
    }   
}
