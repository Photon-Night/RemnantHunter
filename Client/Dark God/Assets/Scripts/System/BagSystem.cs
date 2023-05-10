using Game.UIWin;
using System;
using System.Collections.Generic;
using PEProtocol;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Event;

namespace Game.Bag
{
    public class BagSystem : SystemRoot<BagSystem>
    {
        public BagWin bagWin;
        private BagManager bagMgr;

        public override void InitSystem()
        {
            base.InitSystem();
            PECommon.Log("BagSystem Init");
        }

        public void OpenBagWin()
        {
            bagWin.SetWinState();
        }

        public void InitBagManager(string[] itemArr)
        {
            bagMgr = new BagManager(itemArr);
        }

        public void GetBagItemLstByItemType(BagItemType type, List<BagItemData> itemLst)
        {
            bagMgr.GetBagItemLstByItemType(type, itemLst);
        }

        public void UseProp(int itemID)
        {
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqUseProp,
                reqUseProp = new ReqUseProp
                {
                    itemID = itemID,
                },
            };

            netSvc.SendMessage(msg);
        }

        public void RspUseProp(GameMsg msg)
        {
            var data = msg.rspUseProp;
            GameRoot.Instance.SetPlayerDataByUseProp(data);
            var cfg = resSvc.GetGameItemCfg(data.itemID);
            bagMgr.UpdateBagItemCount(cfg.ItemType, data.itemID, data.count);
            bagWin.RefreshUI(data.itemID);
            GameRoot.AddTips($"使用{Message.Color(cfg.name, Message.ColorOrange)}");
        }

        public void ChangeEquipment(int itemID, EquipmentType type)
        {
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqChangeEquipment,
                reqChangeEquipment = new ReqChangeEquipment
                {
                    itemID = itemID,
                    type = type,
                }
            };

            netSvc.SendMessage(msg);
        }

        public bool isEquiped(int itemID)
        {
            var pd = GameRoot.Instance.PlayerData;
            var equipmentArr = pd.equipment.Split('|');

            if (itemID == int.Parse(equipmentArr[0]) || itemID == int.Parse(equipmentArr[1])) return true;
            else return false;
        }

        public void RspChangeEquipment(GameMsg msg)
        {
            var data = msg.rspChangeEquipment;
            var cfg = resSvc.GetGameItemCfg(data.itemID);
            GameRoot.Instance.SetPlayerDataByChangeEquipment(data);

            if(BattleSystem.IsEnterBattle)
            {
                BattleSystem.Instance.ChangePlayerEquipment(data.itemID);
            }
            else
            {
                MainCitySystem.Instance.ChangePlayerEquipment(data.itemID);
            }

            bagWin.RefreshUI(data.itemID);
            GameRoot.AddTips($"{Message.Color(cfg.name, Message.ColorOrange)}已装备");
        }
    }
}
