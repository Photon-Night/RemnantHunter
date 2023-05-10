using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Common
{
    public class AnimMessage
    {
        private static int float_vertical;
        private static int float_horizontal;
        private static int bool_CanMove_p;
        private static int bool_onGround_p;
        private static int bool_sprint_p;
        private static int trigger_roll_p;
        private static int trigger_hit_p;
        private static int trigger_dodge_p;
        private static int trigger_combo_p;

        private static Dictionary<string, int> animArgHashDic;
        static AnimMessage()
        {
            animArgHashDic = new Dictionary<string, int>();

            float_vertical = Animator.StringToHash("vertical");
            float_horizontal = Animator.StringToHash("horizontal");
            bool_CanMove_p = Animator.StringToHash("CanMove");
            bool_onGround_p = Animator.StringToHash("onGround");
            bool_sprint_p = Animator.StringToHash("sprint");
            trigger_roll_p = Animator.StringToHash("roll");
            trigger_hit_p = Animator.StringToHash("hit");
            trigger_dodge_p = Animator.StringToHash("dodge");
            trigger_combo_p = Animator.StringToHash("combo");
        }

        public static int GetAnimArgHashCode(string name)
        {
            int code = -1;
            if(animArgHashDic.TryGetValue(name, out code))
            {
                return code;
            }

            return -1;
        }
        #region Player
        public int Float_Vertical
        {
            get
            {
                if (float_vertical == 0)
                {
                    float_vertical = Animator.StringToHash("vertical");
                }

                return float_vertical;
            }
        }

        public int Float_Horizontal
        {
            get
            {
                if (float_horizontal == 0)
                {
                    float_horizontal = Animator.StringToHash("horizontal");
                }

                return float_horizontal;
            }
        }

        public int Bool_CanMove_P
        {
            get
            {
                if (bool_CanMove_p == 0)
                {
                    bool_CanMove_p = Animator.StringToHash("CanMove");
                }
                return bool_CanMove_p;
            }
        }

        public int Bool_OnGround_P
        {
            get
            {
                if (bool_onGround_p == 0)
                {
                    bool_onGround_p = Animator.StringToHash("onGround");
                }
                return bool_onGround_p;
            }

        }

        public int Bool_Sprint_P
        {
            get
            {
                if (bool_sprint_p == 0)
                {
                    bool_sprint_p = Animator.StringToHash("sprint");
                }
                return bool_sprint_p;
            }

        }

        public int Trigger_Roll_P
        {
            get
            {
                if (trigger_roll_p == 0)
                {
                    trigger_roll_p = Animator.StringToHash("roll");
                }
                return trigger_roll_p;
            }

        }

        public int Trigger_Hit_P
        {
            get
            {
                if (trigger_hit_p == 0)
                {
                    trigger_hit_p = Animator.StringToHash("hit");
                }
                return trigger_hit_p;
            }

        }

        public int Trigger_Dodge_P
        {
            get
            {
                if (trigger_dodge_p == 0)
                {
                    trigger_dodge_p = Animator.StringToHash("dodge");
                }
                return trigger_dodge_p;
            }

        }

        public int Trigger_Combo_P
        {
            get
            {
                if (trigger_combo_p == 0)
                {
                    trigger_combo_p = Animator.StringToHash("combo");
                }
                return trigger_combo_p;
            }

        }
        #endregion


    }
}
