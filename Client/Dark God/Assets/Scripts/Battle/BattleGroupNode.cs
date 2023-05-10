using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Battle
{
    public class BattleGroupNode : IBattleNode
    {    
        public string NodeName { get; set; }
        public bool IsActive { get ; set; }

        private Dictionary<string, IBattleNode> childNode = new Dictionary<string, IBattleNode>();

        public BattleGroupNode() { }

        public BattleGroupNode(string NodeName)
        {           
            this.NodeName = NodeName;
        }
        public virtual void AddChildNode(IBattleNode node, string parentNodeName = "root")
        {
            if(parentNodeName == NodeName)
            {
                if (!childNode.ContainsKey(node.NodeName))
                {
                    childNode.Add(node.NodeName, node);
                }
            }
            else
            {
                var e = childNode.GetEnumerator();
                while(e.MoveNext())
                {
                    if(e.Current.Value is BattleGroupNode group)
                    group.AddChildNode(node, parentNodeName);
                }

                e.Dispose();
            }
            
        }

        public virtual void ReMoveChildNode(string nodeName, string parentNodeName)
        {
            if (parentNodeName == NodeName)
            {
                if(childNode.ContainsKey(nodeName))
                {
                    childNode[nodeName].OnRemoved();
                    childNode[nodeName] = null;
                    childNode.Remove(nodeName);
                }
            }
            else
            {
                var e = childNode.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.Value is BattleGroupNode group)
                        group.ReMoveChildNode(nodeName, parentNodeName);
                }

                e.Dispose();
            }
        }

        public void SetNodeActive(bool active, bool isChain = false)
        {
            IsActive = active;
            if (isChain)
            {
                var e = childNode.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.Value is BattleGroupNode group)
                        group.SetNodeActive(active, isChain);
                }
            }
        }

        public virtual void OnActive()
        {
            
        }

        public virtual void OnDisactive()
        {
            
        }

        public virtual void OnRemoved()
        {
            childNode.Clear();
        }
    }
}
