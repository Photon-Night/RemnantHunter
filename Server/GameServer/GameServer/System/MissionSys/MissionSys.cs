using PEProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class MissionSys:Singleton<MissionSys>
    {
        private CacheSvc cacheSvc = null;
        private CfgSvc cfgSvc = null;

        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            cfgSvc = CfgSvc.Instance;

            PECommon.Log("MissionSystem Loading");
        }


        public void ReqMissionEnter(MsgPack pack)
        {
            PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
            ReqMissionEnter data = pack.msg.reqMissionEnter;
            int power = cfgSvc.GetMapCfg(data.mid).power;

            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspMissionEnter,
            };

            if(pd.power - power < 0)
            {
                msg.err = (int)ErrorCode.LackPower;
            }
            else if(pd.mission < data.mid)
            {
                msg.err = (int)ErrorCode.ClientDataError;
            }
            else 
            {
                pd.power -= power;

                if (cacheSvc.UpdatePlayerData(pd.id, pd))
                {
                    msg.rspMissionEnter = new RspMissionEnter
                    {
                        mid = data.mid,
                        power = pd.power,
                    };
                }
                else
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }


            }
            pack.session.SendMsg(msg);
        }

        public void ReqFBFightEnd(MsgPack pack)
        {
            ReqFBFightEnd data = pack.msg.reqFBFightEnd;

            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspFBFightEnd
            };

            //校验战斗是否合法
            if (data.win)
            {
                if (data.costtime > 0 && data.resthp > 0)
                {
                    //根据副本ID获取相应奖励
                    MapCfg rd = cfgSvc.GetMapCfg(data.fbid);
                    PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);

                    //任务进度数据更新
                    TaskSys.Instance.CalcTaskPrgs(pd, 2);

                    pd.coin += rd.coin;
                    pd.crystal += rd.crystal;
                    PECommon.CalcExp(pd, rd.exp);

                    if (pd.mission == data.fbid)
                    {
                        pd.mission += 1;
                    }

                    if (!cacheSvc.UpdatePlayerData(pd.id, pd))
                    {
                        msg.err = (int)ErrorCode.UpdateDBError;
                    }
                    else
                    {
                        RspFBFightEnd rspFBFight = new RspFBFightEnd
                        {
                            win = data.win,
                            fbid = data.fbid,
                            resthp = data.resthp,
                            costtime = data.costtime,

                            coin = pd.coin,
                            lv = pd.lv,
                            exp = pd.exp,
                            crystal = pd.crystal,
                            mission = pd.mission
                        };

                        msg.rspFBFightEnd = rspFBFight;
                    }
                }

            }
            else
            {
                msg.err = (int)ErrorCode.ClientDataError;
            }

            pack.session.SendMsg(msg);
        }
    }
}
