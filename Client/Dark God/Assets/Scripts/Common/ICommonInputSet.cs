using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Common
{
    interface ICommonInputSet
    {
        public void SetCamLock(bool state);

        public void SetInteraction();

        public void SetScrollInteraction(float axis);

        public void SetOpenMenu();

    }
}
