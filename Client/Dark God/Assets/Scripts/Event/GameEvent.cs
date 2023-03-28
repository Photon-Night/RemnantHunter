using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Event
{   
    public class GameEvent<T> : GameEventNode
    {
        private  EventDelegate<T> handlers;

        public GameEvent(string name) : base(name)
        {
            handlers = null;
        }

        public void Subscrible(EventDelegate<T> handler)
        {
            handlers += handler;
        }

        public void Unsubscrible(EventDelegate<T> handler)
        {
            handlers -= handler;
        }

        public void UnsubscribleAll()
        {
            var handlerLst = handlers.GetInvocationList();
            foreach (var handlerItem in handlerLst)
            {
                handlers -= handlerItem as EventDelegate<T>;
            }
        }

        protected override void OnDisable()
        {
            UnsubscribleAll();
        }
        public void Tirgger(params T[] args)
        {
            if(!Enable)
            {
                return;
            }

            handlers?.Invoke(args);
        }
    }
}
