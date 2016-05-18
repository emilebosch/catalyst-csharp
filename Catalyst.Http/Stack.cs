using System;
using System.Collections.Generic;
using System.Text;

namespace Catalyst.Http
{
    public class Stack : ICallable
    {
        private ICallable next = null;
            
        public Stack(Action<Stack> initializer)
        {
            initializer(this);
        }

        public void Run<T>() where T:class, ICallable
        {
            next = Activator.CreateInstance(typeof(T)) as T;
        }

        public void Use<T>(Action<T> initializer=null) where T : class, ICallable
        {
            var previous = next != null ? next : this;
            T app = null;
            try
            {
                app = Activator.CreateInstance(typeof(T), previous) as T;
            }
            catch (Exception) { }

            if (app == null)
            {
                app = Activator.CreateInstance(typeof(T)) as T;
            }

            if (initializer != null)
            {
                initializer(app);
            }
            next = app;
        }

        public IResponse Call(IRequest env)
        {
            return next.Call(env);
        }
    }
}
