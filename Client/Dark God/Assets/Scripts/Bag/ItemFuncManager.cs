using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Bag
{
    public class ItemFuncManager
    {
        private Dictionary<ItemFunction, Action<float, float>> itemFuncDic = new Dictionary<ItemFunction, Action<float, float>>();
        public ItemFuncManager() { }
        
        public void InvokeItemFunction(ItemFunction funcType, float funcNum, float duration = 0f)
        {
            if(itemFuncDic.TryGetValue(funcType, out var action))
            {
                action(funcNum, duration);
            }
        }

        private void ItemFunction_Power(float funcNum, float duration)
        {

        }

        private void ItemFunction_Health(float functionNum, float duration)
        {
           
        }
      

    }
}
