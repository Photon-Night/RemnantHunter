using PEProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class BagSys : Singleton<BagSys>
    {
        private CacheSvc cacheSvc;
        private CfgSvc cfgSvc;

        private Dictionary<ServerSession, Dictionary<int, int>> clientBagDic = new Dictionary<ServerSession, Dictionary<int, int>>();
        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            cfgSvc = CfgSvc.Instance;
        }
        public void AddItem(ServerSession session, int itemID, int count)
        {
            
            
            if(clientBagDic.TryGetValue(session, out var bag))
            {
                if(bag.ContainsKey(itemID))
                {
                    bag[itemID] += count;                   
                }
                else
                {
                    bag[itemID] = count;
                }
            }
        }    
        
        public bool UpdateBagArrBySession(ServerSession session)
        {
            if (clientBagDic.TryGetValue(session, out var bag))
            {
                var pd = cacheSvc.GetPlayerDataBySession(session);
                var bagArr = pd.bag;
                StringBuilder builder = new StringBuilder();
                var e = bag.GetEnumerator();
                while (e.MoveNext())
                {
                    builder.Append($"{e.Current.Key}#{e.Current.Value}|");
                }
                PECommon.Log(builder.ToString());
                builder.Remove(builder.Length - 1, 1);
                pd.bag = builder.ToString().Split('|');
                return cacheSvc.UpdatePlayerData(pd.id, pd);

            }

            return false;
        }

        public void ReqUseProp(MsgPack pack)
        {
            var data = pack.msg.reqUseProp;
            var session = pack.session;
            var pd = cacheSvc.GetPlayerDataBySession(session);
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspUseProp
            };
            if(clientBagDic.TryGetValue(session, out var bagDic))
            {
                if(bagDic.ContainsKey(data.itemID))
                {
                    int count = bagDic[data.itemID];
                    if(count - 1 < 0)
                    {
                        msg.cmd = (int)ErrorCode.LackProp;
                    }
                    else
                    {
                        clientBagDic[session][data.itemID] -= 1;
                        for(int i = 0; i < pd.bag.Length; i++)
                        {
                            var itemInfo = pd.bag[i].Split('#');
                            if(data.itemID == int.Parse(itemInfo[0]))
                            {
                                pd.bag[i] = $"{data.itemID}#{count - 1}";
                            }
                        }

                        cacheSvc.UpdatePlayerData(pd.id, pd);

                        msg.rspUseProp = new RspUseProp
                        {
                            itemID = data.itemID,
                            count = count - 1,
                            bag = pd.bag,
                        };

                        
                    }
                }
                else
                {
                    msg.cmd = (int)ErrorCode.LackProp;
                }
            }
            else
            {
                msg.cmd = (int)ErrorCode.ServerDataError;
            }

            session.SendMsg(msg);
        }

        public void ReqChangeEquipent(MsgPack pack)
        {
            var session = pack.session;
            var data = pack.msg.reqChangeEquipment;
            var pd = cacheSvc.GetPlayerDataBySession(session);

            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspChangeEquipment,
            };
            if (clientBagDic.TryGetValue(session, out var bagDic))
            {
                if(bagDic.ContainsKey(data.itemID))
                {
                    if(bagDic[data.itemID] > 0)
                    {
                        msg.rspChangeEquipment = new RspChangeEquipment
                        {
                            itemID = data.itemID,
                            type = data.type,
                        };

                        var equipmentArr = pd.equipment.Split('|');
                        if(data.type == EquipmentType.Shield)
                        {
                            pd.equipment = $"{equipmentArr[0]}|{data.itemID}";
                            msg.rspChangeEquipment.equipmentStr = pd.equipment;
                            cacheSvc.UpdatePlayerData(pd.id, pd);
                        }
                        else if(data.type == EquipmentType.Weapon)
                        {
                            pd.equipment = $"{data.itemID}|{equipmentArr[1]}";
                            msg.rspChangeEquipment.equipmentStr = pd.equipment;
                            cacheSvc.UpdatePlayerData(pd.id, pd);
                        }
                        else
                        {
                            msg.cmd = (int)ErrorCode.ClientDataError;
                        }
                    }
                    else
                    {
                        msg.cmd = (int)ErrorCode.LackEquipment;
                    }
                }
            }
            else
            {
                msg.cmd = (int)ErrorCode.ServerDataError;
            }

            session.SendMsg(msg);
        }

        public void SetOnlineClientBagCache(ServerSession session, string[] bagArr)
        {
            if(!clientBagDic.ContainsKey(session))
            {
                clientBagDic[session] = new Dictionary<int, int>();
            }
            for(int i = 0; i < bagArr.Length; i++)
            {
                string[] itemInfo = bagArr[i].Split('#');
                clientBagDic[session].Add(int.Parse(itemInfo[0]), int.Parse(itemInfo[1]));
            }
        }

        public void ReleaseClientBagCache(ServerSession session)
        {
            if (clientBagDic.TryGetValue(session, out var bagDic))
            {
                bagDic.Clear();
                clientBagDic[session] = null;
                PECommon.Log($"SessionID_{session.sessionID} Bag Cache Release_True");
            }
        }
    }
}
