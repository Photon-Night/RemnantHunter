using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Battle
{
    public interface IBattleNode
    {       

        public string NodeName { get; set; }
        public bool IsActive { get; set; }
        public void OnActive();
        public void OnDisactive();

        public void OnRemoved();
    }
}
