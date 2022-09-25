using PEProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class PowerSys : Singleton<PowerSys>
    {
        private CacheSvc cacheSvc = null;
        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            TimerSvc.Instance.AddTimeTask(CalcPowerAdd, PECommon.PowerAddSpace, PETimeUnit.Minute, 0);
            PECommon.Log("PowerSystem Loading");
        }

        private void CalcPowerAdd(int tid)
        {
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.PushPower
            };
            msg.pushPower = new PushPower();

            Dictionary<ServerSession, PlayerData> onlineDic = cacheSvc.GetOnlineCache();

            var e = onlineDic.GetEnumerator();
            while(e.MoveNext())
            {
                PlayerData _data = e.Current.Value;
                ServerSession _session = e.Current.Key;
                int powerMax = PECommon.GetPowerLimit(_data.lv);

                if (_data.power >= powerMax)
                {
                    continue;
                }
                else
                {
                    _data.power += PECommon.PowerAddCount;
                    if(_data.power > powerMax)
                    {
                        _data.power = powerMax;
                    }

                    if(!cacheSvc.UpdatePlayerData(_data.id ,_data))
                    {
                        msg.err = (int)ErrorCode.UpdateDBError;
                    }
                    else
                    {
                        msg.pushPower.power = _data.power;
                    }

                    _session.SendMsg(msg);
                }
            }
        }
    }
}
