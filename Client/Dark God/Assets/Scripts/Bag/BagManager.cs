using PEProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Bag
{
    public class BagManager
    {
        private Dictionary<BagItemType, Dictionary<int, BagItemData>> bagItemDic = new Dictionary<BagItemType, Dictionary<int, BagItemData>>();
        
        private ResService resSvc;
        public BagManager()
        {
            resSvc = ResService.Instance;
        }    

        public BagManager(string[] itemArr)
        {
            resSvc = ResService.Instance;
            string[] itemData = null;
            for(int i = 0; i < itemArr.Length; i++)
            {
                itemData = itemArr[i].Split('#');
                if(itemData.Length == 2)
                {
                    int ID = int.Parse(itemData[0]);
                    int count = int.Parse(itemData[1]);

                    if (count == 0) continue;

                    var cfg = resSvc.GetGameItemCfg(ID);
                    if(cfg != null)
                    {                                              
                        if(!bagItemDic.ContainsKey(cfg.ItemType))
                        {
                            bagItemDic[cfg.ItemType] = new Dictionary<int, BagItemData>();
                        }

                        bagItemDic[cfg.ItemType].Add(cfg.ID, new BagItemData(cfg, count));
                        Debug.Log($"{cfg.name} {count}");
                    }
                    else
                    {
                        PECommon.Log($"Item_{ID} Not Find");
                        continue;
                    }
                }
            }
        }

        public void UpdateBagItemCount(BagItemType type, int ID, int count)
        {
            if(bagItemDic.TryGetValue(type, out var dic))
            {
                if (dic.TryGetValue(ID, out var data))
                {
                    if (count == 0)
                    {
                        dic[ID] = null;
                        dic.Remove(ID);
                    }
                    else
                        data.count = count;
                }
            }
        }

        public void AddBagItem(int itemID, int count)
        {
            var cfg = resSvc.GetGameItemCfg(itemID);
            AddBagItem(cfg, count);
        }

        public void AddBagItem(GameItemCfg cfg, int count)
        {
            if (count == 0) return;


            if (!bagItemDic.ContainsKey(cfg.ItemType))
            {
                bagItemDic[cfg.ItemType] = new Dictionary<int, BagItemData>();
            }

            if (bagItemDic[cfg.ItemType].TryGetValue(cfg.ID, out var item))
            {
                item.count += count;
            }
            else
            {
                bagItemDic[cfg.ItemType].Add(cfg.ID, new BagItemData(cfg, count));
            }

        }

        public void GetBagItemLstByItemType(BagItemType type, List<BagItemData> itemLst)
        {
            if (itemLst == null)
                itemLst = new List<BagItemData>();

            if(bagItemDic.TryGetValue(type, out var dic))
            {
                var e = dic.GetEnumerator();
                while(e.MoveNext())
                {
                    if(type == BagItemType.Equipment)
                    {
                        if(e.Current.Value.cfg.equipmentType == EquipmentType.Weapon)
                        {
                            itemLst.Insert(0, e.Current.Value);
                        }
                        else if(e.Current.Value.cfg.equipmentType == EquipmentType.Shield)
                        {
                            itemLst.Add(e.Current.Value);
                        }
                    }
                    else
                    {
                        itemLst.Add(e.Current.Value);
                    }
                }
            }
        }
    }
}
