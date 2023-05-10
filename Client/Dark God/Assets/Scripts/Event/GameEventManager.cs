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

        public static void InitManager()
        {
            root = new GameEventGroup(EventNode.Root);
            AddEventGroup(EventNode.Group_UI);
            AddEventGroup(EventNode.Group_Battle);
            AddEventGroup(EventNode.Group_NPC);
            AddEventGroup(EventNode.Group_Task);

            RegisterEvent<bool>(EventNode.Event_OnSetUIWinState, EventNode.Group_UI);
            RegisterEvent<int>(EventNode.Event_OnKillMonster, EventNode.Group_Battle);
            RegisterEvent<int>(EventNode.Event_OnOverTalk, EventNode.Group_NPC);
            RegisterEvent<int>(EventNode.Event_OnNPCTaskStatusChange, EventNode.Group_NPC);
            RegisterEvent<NPCCfg>(EventNode.Event_OnPlayerCloseToNpc, EventNode.Group_NPC);
            RegisterEvent<NPCCfg>(EventNode.Event_OnPlayerFarToNpc, EventNode.Group_NPC);
            RegisterEvent<int>(EventNode.Event_OnBattleEnd, EventNode.Group_Battle);
            RegisterEvent<int>(EventNode.Event_OnBattleStart, EventNode.Group_Battle);
            RegisterEvent<int>(EventNode.Event_OnChangeEquipment, EventNode.Group_UI);
            RegisterEvent<int>(EventNode.Event_OnGetBagItem, EventNode.Group_Task);
        }

        public static void RegisterEvent(GameEventNode eventBase, EventNode groupName)
        {
            if(!root.AddNode(eventBase, groupName))
            {
                PEProtocol.PECommon.Log($"{groupName} is not exited", PEProtocol.LogType.Error);
            }
        }
        public static void AddEventGroup(EventNode groupName, EventNode parentGroup = EventNode.Root)
        {
            GameEventGroup group = new GameEventGroup(groupName);
            root.AddNode(group, parentGroup);
        }
        public static void RegisterEvent<T>(EventNode eventName, EventNode groupName = EventNode.Root)
        {
            var eventNode = new GameEvent<T>(eventName);
            RegisterEvent(eventNode, groupName);
        }

        public static void RemoveEvent(EventNode eventName)
        {
            if(!root.RemoveNode(eventName))
            {
                PEProtocol.PECommon.Log($"{eventName} is not find", PEProtocol.LogType.Error);
            }
        }

        public static void SubscribeEvent<T>(EventNode eventName, EventDelegate<T> handler)
        {
            if(!root.Subscrible<T>(eventName, handler))
            {
                PEProtocol.PECommon.Log($"{eventName} is not find", PEProtocol.LogType.Error);
            }
        }

        public static void UnSubscribleEvent<T>(EventNode eventName, EventDelegate<T> handler)
        {
            if(!root.Unsubscrible<T>(eventName, handler))
            {
                PEProtocol.PECommon.Log($"{eventName} is not find", PEProtocol.LogType.Error);
            }
        }

        public static void TriggerEvent<T>(EventNode eventName, params T[] args)
        {
            if(!root.Trigger<T>(eventName, args))
            {
                PEProtocol.PECommon.Log($"{eventName} is not find or is not enable", PEProtocol.LogType.Error);
            }
        }

        public static void ClearSubscrible(EventNode nodeName)
        {
            if(!root.ClearSubscrible(nodeName))
            {
                PEProtocol.PECommon.Log($"{nodeName} is not find", PEProtocol.LogType.Error);
            }
        }
    }
}
