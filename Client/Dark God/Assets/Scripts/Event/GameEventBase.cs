using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Event
{
    public abstract class GameEventNode
    {
        protected string nodeName;

        public GameEventNode(string name)
        {
            nodeName = name;
            enable = true;
        }
        public string NodeName
        {
            get { return nodeName; }
            protected set { nodeName = value; }
        }

        protected bool enable;
        public bool Enable
        {
            get { return enable; }
            set
            {
                enable = value; 
                if(!enable)
                {
                    OnDisable();
                }
            }
        }

        protected virtual void OnDisable() { }
    }
}
