using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Game.Event;

namespace Game.Test
{
    public class EventTest : MonoBehaviour
    {
        
        public void Start()
        {
            //Debug.Log("1");
            //GameEventManager.RegisterEvent<int>("event1");
            //GameEventManager.RegisterEvent<string>("event2");
            //GameEventManager.SubscribeEvent<int>("event1", Sub1);
            //GameEventManager.SubscribeEvent<string>("event2", Sub2);
            //
            //GameEventManager.AddEventGroup("Battle");
            //GameEventManager.RegisterEvent<int>("battleEvent1", "Battle");
            //GameEventManager.RegisterEvent<string>("battleEvent2", "Battle");
            //GameEventManager.SubscribeEvent<int>("battleEvent1", Battle1);
            //GameEventManager.SubscribeEvent<string>("battleEvent2", Battle2);
        }

        public void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    
            //    GameEventManager.TriggerEvent<int>("battleEvent1", 1, 1, 1, 1);
            //    GameEventManager.TriggerEvent<string>("battleEvent2", "yes", "amd");
            //}
            //else if (Input.GetKeyDown(KeyCode.C))
            //{
            //    GameEventManager.UnSubscribleEvent<int>("battleEvent1", Battle1);
            //}
            //else if(Input.GetKeyDown(KeyCode.A))
            //{
            //    GameEventManager.UnSubscribleEvent<string>("battleEvent2", Battle2);
            //}
            //else if(Input.GetKeyDown(KeyCode.B))
            //{
            //    GameEventManager.RemoveEvent("Battle");
            //    Debug.Log("remove Battle");
            //}
        }

        public void Sub1(params int[] args)
        {
            for(int i = 0; i < args.Length; i++)
            {
                Debug.Log($"sub1 : {args[i]}");
            }
        }

        public void Sub2(params string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                Debug.Log($"sub2 : {args[i]}");
            }
        }

        public void Battle1(params int[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                Debug.Log($"b1 : {args[i]}");
            }
        }
        public void Battle2(params string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                Debug.Log($"b2 : {args[i]}");
            }
        }
    }
}
