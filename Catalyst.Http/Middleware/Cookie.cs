namespace Catalyst.Http
{
    public class Cookie : ICallable
    {
        ICallable next = null;

        public Cookie(ICallable app)
        {
            next = app;
        }

        public IResponse Call(IRequest request)
        {      
            var response = next.Call(request);
            return response;
        }
    }

    public static class CookieExtensions
    {
        public static void SetCookie(this IRequest request, string name, int age)
        {
        }
    }
}
