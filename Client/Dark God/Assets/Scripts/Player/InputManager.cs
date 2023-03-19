using Game.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Manager
{
    class InputManager : MonoSingleton<InputManager>
    {
        private IPlayerInputSet inputRoot;

        [Header("Inputs")]
        [SerializeField] private float vertical;
        [SerializeField] private float horizontal;

        [Header("PlayerInputStates")]        
        public bool sprint;     
        [HideInInspector]
        public bool jump;       
        [HideInInspector]
        public bool normalAttack;   
        [HideInInspector]
        public bool comboAttack;    
        public bool canMove;    
        [HideInInspector]
        public bool roll;

        private void Update()
        {
            GetInput();
            SetPlayerStates();
        }

        private void GetInput()
        {
            vertical = Input.GetAxis("Vertical");    //for getting vertical input.
            horizontal = Input.GetAxis("Horizontal");    //for getting horizontal input.
            sprint = Input.GetButton("SprintInput");     //for getting sprint input.
            jump = Input.GetButtonDown("Jump");      //for getting jump input.
            normalAttack = Input.GetButtonDown("Fire1"); //for getting normal attack input.
            comboAttack = Input.GetButtonDown("Fire2");    //for getting combo attack input.
            roll = Input.GetButtonDown("Fire3");     //for getting roll input.
        }

        private void SetPlayerStates()
        {
            if (inputRoot == null)
            {
                return;
            }

            if (jump)
            {
                inputRoot.Jump();
            }

            if (comboAttack)
            {
                inputRoot.Combo();
            }

            if (roll)
            {
                inputRoot.Roll();
            }

            if(normalAttack)
            {
                inputRoot.Attack();
            }


            inputRoot.Sprint(sprint);            
            inputRoot.Move(vertical, horizontal);
        }

        public void SetInputRoot(IPlayerInputSet root)
        {
            inputRoot = root;
        }

        public void CloseInput()
        {
            inputRoot = null;
        }

    }
}
