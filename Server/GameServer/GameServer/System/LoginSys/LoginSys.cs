using PENet;
using PEProtocol;

namespace GameServer
{
    class LoginSys : Singleton<LoginSys>
    {

        private CacheSvc cacheSvc;
        private TimerSvc timerSvc;
        public void Init()
        {
            PECommon.Log("LoginSystem Loading");
            cacheSvc = CacheSvc.Instance;
            timerSvc = TimerSvc.Instance;
        }

        public void ReqLogin(MsgPack pack)
        {
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspLogin,
               
            };
            ReqLogin _data = pack.msg.reqLogin;
            if(cacheSvc.IsAccOnLine(_data.acc))
            {
                msg.err = (int)ErrorCode.AccountIsOnline;
            }
            else
            {
                PlayerData _playerData = cacheSvc.GetPlayerData(_data.acc, _data.pas);
                if(_playerData == null)
                {
                    msg.err = (int)ErrorCode.WrongPass;
                }
                else
                {
                    int power = _playerData.power;
                    long nowTime = timerSvc.GetNowTime();
                    long millisecond = nowTime - _playerData.time;
                    int addPower = (int)(millisecond / (60 * 1000 * PECommon.PowerAddSpace)) * PECommon.PowerAddCount;
                    if(addPower > 0)
                    {
                        int powerMax = PECommon.GetPowerLimit(_playerData.lv);
                        if (_playerData.power < powerMax)
                        {
                            _playerData.power += addPower;
                            if (_playerData.power > powerMax)
                            {
                                _playerData.power = powerMax;
                            }
                        }
                    }

                    if(power != _playerData.power)
                    { 
                        cacheSvc.UpdatePlayerData(_playerData.id, _playerData);
                    }

                    msg.rspLogin = new RspLogin
                    {
                        playerData = _playerData
                    };

                    cacheSvc.AccOnLine(_data.acc, pack.session, _playerData);
                    BagSys.Instance.SetOnlineClientBagCache(pack.session, _playerData.bag);
                }
            }

            pack.session.SendMsg(msg);
        }

        public void ReqRename(MsgPack pack)
        {
            ReqRename data = pack.msg.reqRename;
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspRename
            };

            if(cacheSvc.IsNameExist(data.name))
            {
                msg.err = (int)ErrorCode.NameIsExist;
            }
            else
            {
                PlayerData playerData = cacheSvc.GetPlayerDataBySession(pack.session);
                playerData.name = data.name;
                playerData.modle = data.modle;
                if(!cacheSvc.UpdatePlayerData(playerData.id, playerData))
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
                else
                {
                    msg.rspRename = new RspRename
                    {
                        name = data.name,
                        modle = data.modle,
                    };

                }
            }

            pack.session.SendMsg(msg);
        }

        public void ClearOffLine(ServerSession session)
        {
            PlayerData pd = cacheSvc.GetPlayerDataBySession(session);
            if (pd != null)
            {
                pd.time = timerSvc.GetNowTime();

                if (!cacheSvc.UpdatePlayerData(pd.id, pd))
                {
                    PECommon.Log("Update Offline Time Error");
                }
            }
            cacheSvc.ReleaseCache(session);
            BagSys.Instance.ReleaseClientBagCache(session);
        }
    }
}
