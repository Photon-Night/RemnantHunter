﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PEProtocol;

namespace Game.Event
{
    public class GameEventGroup : GameEventNode
    {
        private Dictionary<string, GameEventNode> childNode = new Dictionary<string, GameEventNode>();
        public GameEventGroup(string name) : base(name)
        {

        }
        public bool AddEvent(GameEventNode eventNode, string groupName = "Root")
        {           

            if (groupName == nodeName || groupName == null)
            {
                childNode[eventNode.NodeName] = eventNode;
                PECommon.Log($"{eventNode.NodeName} register");
                return true;
            }
            else
            {
                bool isFind = false;
                var e = childNode.GetEnumerator();
                while(e.MoveNext())
                {
                    if(e.Current.Value is GameEventGroup group)
                    {
                        if(group.AddEvent(eventNode, groupName))
                        {
                            isFind = true;
                            break;
                        }
                       
                    }
                }
                e.Dispose();

                return isFind;
            }           
        }
        public bool RemoveEvent(string eventName)
        {
            
            if(childNode.ContainsKey(eventName))
            {
                childNode[eventName].Enable = false;
                childNode.Remove(eventName);
                PECommon.Log($"{eventName} is removed");
                return true;
            }
            else
            {
                bool isFind = false;
                var e = childNode.GetEnumerator();
                while(e.MoveNext())
                {
                    var node = e.Current;
                    if(node.Value is GameEventGroup group)
                    {
                        if(group.RemoveEvent(eventName))
                        {
                            isFind = true;
                            break;
                        }
                    }
                }
                e.Dispose();

                return isFind;
            }
        }
        public bool Subscrible<T>(string eventName, EventDelegate<T> handler)
        {
            if(childNode.ContainsKey(eventName))
            {
                if(childNode[eventName] is GameEvent<T> gameEvent)
                {
                    gameEvent.Subscrible(handler);
                    return true;
                }
                else
                {
                    PECommon.Log($"{eventName} is not a GameEvent", PEProtocol.LogType.Error);
                    return false;
                }
            }
            else
            {
                bool isFind = false;
                var e = childNode.GetEnumerator();
                while(e.MoveNext())
                {
                    var node = e.Current;
                    if(node.Value is GameEventGroup group)
                    {
                        if(group.Subscrible<T>(eventName, handler))
                        {
                            isFind = true;
                            break;
                        }
                    }
                }

                e.Dispose();
                return isFind;
            }
        }
        public bool Unsubscrible<T>(string eventName, EventDelegate<T> handler)
        {
            if (childNode.ContainsKey(eventName))
            {
                if (childNode[eventName] is GameEvent<T> gameEvent)
                {
                    gameEvent.Unsubscrible(handler);
                    return true;
                }
                else
                {
                    PEProtocol.PECommon.Log($"{eventName} is not a GameEvent", PEProtocol.LogType.Error);
                    return false;
                }
            }
            else
            {
                bool isFind = false;
                var e = childNode.GetEnumerator();
                while (e.MoveNext())
                {
                    var node = e.Current;
                    if (node.Value is GameEventGroup group)
                    {
                        if (group.Unsubscrible<T>(eventName, handler))
                        {
                            isFind = true;
                            break;
                        }
                    }
                }

                e.Dispose();
                return isFind;
            }
        }
        public bool ClearSubscrible(string eventName)
        {           

            if(childNode.ContainsKey(eventName))
            {
                childNode[eventName].Enable = false;
                return true;
            }
            else
            {
                bool isFind = false;
                var e = childNode.GetEnumerator();
                while(e.MoveNext())
                {
                    var node = e.Current.Value;
                    if(node is GameEventGroup group)
                    {
                        if(group.ClearSubscrible(eventName))
                        {
                            isFind = true;
                            break;
                        }
                    }
                }

                e.Dispose();
                return isFind;
            }
        }        
        public bool Trigger<T>(string eventName, params T[] args)
        {
            if (!Enable)
                return false;

            if(childNode.ContainsKey(eventName))
            {
                if(childNode[eventName] is GameEvent<T> gameEvent)
                {
                    gameEvent.Tirgger(args);
                    PECommon.Log($"{eventName} Trigger");
                }
                else
                {
                    PECommon.Log($"{eventName} is not a Event", PEProtocol.LogType.Error);
                    return false;
                }

                return true;
            }
            else
            {
                bool isFind = false;
                var e = childNode.GetEnumerator();
                while(e.MoveNext())
                {
                    var node = e.Current;
                    if(node.Value is GameEventGroup group)
                    {
                        if(group.Trigger<T>(eventName, args))
                        {
                            isFind = true;
                            break;
                        }
                    }
                }

                e.Dispose();
                return isFind;
            }
        }
        protected override void OnDisable()
        {
            var e = childNode.GetEnumerator();
            while(e.MoveNext())
            {
                var node = e.Current.Value;
                node.Enable = false;
            }
        }
    }
}
