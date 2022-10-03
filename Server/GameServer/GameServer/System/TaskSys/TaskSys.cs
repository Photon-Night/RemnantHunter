using PEProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class TaskSys : Singleton<TaskSys>
    {
        private CacheSvc cacheSvc = null;
        private CfgSvc cfgSvc = null;
        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            cfgSvc = CfgSvc.Instance;
            PECommon.Log("TaskSystem Loading");
        }


        public void ReqTakeTaskReward(MsgPack pack)
        {
            ReqTakeTaskReward data = pack.msg.reqTakeTaskReward;
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspTakeTaskReward,
            };
            PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
            TaskRewardData trd = CalcTaskRewardData(pd, data.rid);
            TaskCfg tc = cfgSvc.GetTaskCfgData(data.rid);

            if (tc.count == trd.prgs && !trd.taked)
            {
                pd.coin += tc.coin;
                PECommon.CalcExp(pd, tc.exp);
                trd.taked = true;
                CalcTaskArr(pd, trd);

                if (!cacheSvc.UpdatePlayerData(pd.id, pd))
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
                else
                {
                    msg.rspTakeTaskReward = new RspTakeTaskReward
                    {
                        coin = pd.coin,
                        exp = pd.exp,
                        lv = pd.lv,
                        taskArr = pd.task,
                    };

                }
            }
            else
            {
                msg.err = (int)ErrorCode.ClientDataError;
            }
            pack.session.SendMsg(msg);
        }
        /// <summary>
        /// 从PlayerData中提取出对应id任务的完成信息
        /// </summary>
        /// <param name="pd"></param>
        /// <param name="rid"></param>
        /// <returns></returns>
        private TaskRewardData CalcTaskRewardData(PlayerData pd, int rid)
        {
            string[] _taskArr = pd.task;
            TaskRewardData trd = null;
            for (int i = 0; i < _taskArr.Length; i++)
            {
                string[] taskInfo = _taskArr[i].Split('|');
                if (int.Parse(taskInfo[0]) == rid)
                {
                    trd = new TaskRewardData
                    {
                        ID = int.Parse(taskInfo[0]),
                        prgs = int.Parse(taskInfo[1]),
                        taked = taskInfo[2].Equals("1")
                    };

                }
            }

            return trd;
        }
        /// <summary>
        /// 修改PlayerData中task数组对应id任务的完成信息
        /// </summary>
        /// <param name="pd"></param>
        /// <param name="data"></param>
        private void CalcTaskArr(PlayerData pd, TaskRewardData data)
        {
            string[] taskArr = pd.task;
            string result = data.ID + "|" + data.prgs + "|" + (data.taked ? 1 : 0);
            int index = -1;
            for (int i = 0; i < taskArr.Length; i++)
            {
                string[] taskInfo = taskArr[i].Split('|');
                if(int.Parse(taskInfo[0]) == data.ID)
                {
                    index = i;
                    break;
                }
            }
            if(index != -1)
            pd.task[index] = result;
        }

        public void CalcTaskPrgs(PlayerData pd, int tid)
        {
            TaskCfg tc = cfgSvc.GetTaskCfgData(tid);
            TaskRewardData trd = CalcTaskRewardData(pd, tid);

            if (trd.prgs < tc.count)
            { 
                trd.prgs += 1;
                CalcTaskArr(pd, trd);

                ServerSession session = cacheSvc.GetOnlineSessionByID(pd.id);
                GameMsg msg = new GameMsg
                {
                    cmd = (int)CMD.PushTaskPrgs,
                    pushTaskPrgs = new PushTaskPrgs
                    {
                        taskArr = pd.task,
                    }
                };

                session.SendMsg(msg);
            }
        }

        public PushTaskPrgs GetTaskPrgs(PlayerData pd, int tid)
        {
            TaskCfg tc = cfgSvc.GetTaskCfgData(tid);
            TaskRewardData trd = CalcTaskRewardData(pd, tid);
            PushTaskPrgs pushTaskPrgs = null;
            if (trd.prgs < tc.count)
            {
                trd.prgs += 1;
                CalcTaskArr(pd, trd);
                pushTaskPrgs = new PushTaskPrgs
                {
                    taskArr = pd.task
                };

                return pushTaskPrgs;
            }
            else
            {
                return null;
            }

            
        }
    }
}

