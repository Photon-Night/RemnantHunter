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
        private IPlayerInputSet playerInputRoot;
        private ICommonInputSet commonInputRoot;
        [Header("Inputs")]
        [SerializeField] private float vertical;
        [SerializeField] private float horizontal;
        [SerializeField] private float mouseAxis;

        [Header("PlayerInputStates")]        
        public bool sprint;     
        [HideInInspector]
        public bool jump;       
        [HideInInspector]
        public bool normalAttack;   
        [HideInInspector]
        public bool comboAttack;    
        public bool CanMove;    
        [HideInInspector]
        public bool roll;
        [HideInInspector]
        public bool unlockCam;
        public bool lockCam;
        public bool interaction;
        public bool openUI;
        public bool checkOut;
        private void Update()
        {
            GetInput();
            SetPlayerStates();
        }
        private void FixedUpdate()
        {
            
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
            lockCam = Input.GetKeyDown(KeyCode.LeftAlt);
            unlockCam = Input.GetKeyUp(KeyCode.LeftAlt);
            interaction = Input.GetKeyDown(KeyCode.F);
            mouseAxis = Input.GetAxis("Mouse ScrollWheel");
            openUI = Input.GetKeyDown(KeyCode.Q);
            checkOut = Input.GetKeyDown(KeyCode.Escape);
        }

        private void SetPlayerStates()
        {

            if (playerInputRoot == null)
            {
                return;
            }

            if(unlockCam)
            {
                commonInputRoot?.SetCamLock(false);
            }

            if(lockCam)
            {
                commonInputRoot?.SetCamLock(true);
            }

            playerInputRoot.Sprint(sprint);

            if (jump)
            {
                playerInputRoot.Jump();
            }
            else if (roll)
            {
                playerInputRoot.Roll();
            }

            if (comboAttack)
            {
                playerInputRoot.Combo();
            }


            if(normalAttack)
            {
                playerInputRoot.Attack();
            }

            if(interaction)
            {
                commonInputRoot?.SetInteraction();
            }

            if(mouseAxis != 0)
            {
                commonInputRoot?.SetScrollInteraction(mouseAxis);
            }

            if(openUI)
            {
                commonInputRoot?.SetOpenMenu();
            }

            if(checkOut)
            {
                commonInputRoot.CheckOut();
            }

            playerInputRoot.Move(vertical, horizontal);
        }

        public void SetPlayerInputRoot(IPlayerInputSet root)
        {
            playerInputRoot = root;
        }
        public void SetCommonInputRoot(ICommonInputSet root)
        {
            commonInputRoot = root;
        }
        public void CloseInput()
        {
            playerInputRoot = null;
            commonInputRoot = null;
        }

    }
}
