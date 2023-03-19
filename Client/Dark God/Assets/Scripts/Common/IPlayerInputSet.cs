using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Common
{
    interface IPlayerInputSet
    {
        public void Move(float ver, float hor);
        public void Attack();
        public void Jump();
        public void Sprint(bool isSprint);
        public void Combo();
        public void Roll();
    }
}
