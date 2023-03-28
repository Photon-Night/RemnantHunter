using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Event
{
    public delegate void EventDelegate<T>(params T[] args);
    public class GameEventManager
    {
        private static GameEventGroup root;

        static GameEventManager()
        {
            root = new GameEventGroup("Root");
        }

        public static void RegisterEvent(GameEventNode eventBase, string groupName = "Root")
        {
            if(!root.AddEvent(eventBase, groupName))
            {
                PEProtocol.PECommon.Log($"{groupName} is not exited", PEProtocol.LogType.Error);
            }
        }
        public static void AddEventGroup(string groupName, string parentGroup = "Root")
        {
            GameEventGroup group = new GameEventGroup(groupName);
            root.AddEvent(group, parentGroup);
        }
        public static void RegisterEvent<T>(string eventName, string groupName = "Root")
        {
            var eventNode = new GameEvent<T>(eventName);
            RegisterEvent(eventNode, groupName);
        }

        public static void RemoveEvent(string eventName)
        {
            if(!root.RemoveEvent(eventName))
            {
                PEProtocol.PECommon.Log($"{eventName} is not find", PEProtocol.LogType.Error);
            }
        }

        public static void SubscribeEvent<T>(string eventName, EventDelegate<T> handler)
        {
            if(!root.Subscrible<T>(eventName, handler))
            {
                PEProtocol.PECommon.Log($"{eventName} is not find", PEProtocol.LogType.Error);
            }
        }

        public static void UnSubscribleEvent<T>(string eventName, EventDelegate<T> handler)
        {
            if(!root.Unsubscrible<T>(eventName, handler))
            {
                PEProtocol.PECommon.Log($"{eventName} is not find", PEProtocol.LogType.Error);
            }
        }

        public static void TriggerEvent<T>(string eventName, params T[] args)
        {
            if(!root.Trigger<T>(eventName, args))
            {
                PEProtocol.PECommon.Log($"{eventName} is not find or is not enable", PEProtocol.LogType.Error);
            }
        }

        public static void ClearSubscrible(string nodeName)
        {
            if(!root.ClearSubscrible(nodeName))
            {
                PEProtocol.PECommon.Log($"{nodeName} is not find", PEProtocol.LogType.Error);
            }
        }
    }
}
