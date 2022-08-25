﻿using PENet;
using PEProtocol;

namespace GameServer
{
    class LoginSys : Singleton<LoginSys>
    {

        private CacheSvc cacheSvc;
        public void Init()
        {
            PECommon.Log("LoginSystem Loading");
            cacheSvc = CacheSvc.Instance;
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
                    msg.rspLogin = new RspLogin
                    {
                        playerData = _playerData
                    };

                    cacheSvc.AccOnLine(_data.acc, pack.session, _playerData);
                }
            }

            pack.session.SendMsg(msg);
        }
    }
}