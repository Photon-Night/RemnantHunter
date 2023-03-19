using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.FSM
{
    public class StateChase : IState
    {
        public void OnEnter(EntityBase entity, params object[] args)
        {
            entity.CurrentAniState = AniState.Chase;
        }

        public void OnExit(EntityBase entity, params object[] args)
        {
            entity.CurrentAniState = AniState.None;
        }

        public void Process(EntityBase entity, params object[] args)
        {
            entity.MoveTo(entity.Target.GetPos());
        }
    }
}
