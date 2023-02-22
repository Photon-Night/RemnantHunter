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
        private DBMgr dbMgr = null;
        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            cfgSvc = CfgSvc.Instance;
            dbMgr = DBMgr.Instance;
            PECommon.Log("TaskSystem Loading");
        }

        public void ReqUpdateTaskInfo(MsgPack pack)
        {
            ReqUpdateTaskInfo data = pack.msg.reqUpdateTaskInfo;
            PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
            TaskDefine task = cfgSvc.GetTaskData(data.taskId);
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspUpdateTaskInfo,
            };
            if (data != null)
            {
                NTaskInfo info = new NTaskInfo
                {
                    taskID = data.taskId,
                    npcID = task.submitNpcID,
                    prg = data.prg,
                    taskState = data.newStatus
                };
                if (data.newStatus == PEProtocol.TaskStatus.Finished)
                {
                    if (data.prg >= task.targetCount)
                    {
                        pd.coin += task.coin;
                        PECommon.CalcExp(pd, task.exp);


                        if (dbMgr.UpdateTaskInfo(pd.id, info))
                        {
                            msg.rspUpdateTaskInfo = new RspUpdateTaskInfo
                            {
                                info = info,
                                coin = pd.coin ,
                                exp = pd.exp,
                                lv = pd.lv
                            };
                        }
                        else
                        {
                            msg.cmd = (int)ErrorCode.UpdateDBError;
                        }
                    }
                    else
                    {
                        msg.cmd = (int)ErrorCode.LackTargetCount;
                    }
                }
                else if (data.newStatus == PEProtocol.TaskStatus.InProgress)
                {
                    if(dbMgr.FindTaskInfo(pd.id, task.ID))
                    {
                        msg.cmd = (int)ErrorCode.ClientDataError;
                    }
                    else if (task.preTaskID == -1 || dbMgr.FindTaskInfo(pd.id, task.preTaskID))
                    {
                        if (dbMgr.InsertNewTaskInfo(pd.id, ref info))
                        {
                            msg.rspUpdateTaskInfo = new RspUpdateTaskInfo
                            {
                                info = info,
                            };
                        }
                        else
                        {
                            msg.cmd = (int)ErrorCode.UpdateDBError;
                        }
                    }
                    else
                    {
                        msg.cmd = (int)ErrorCode.LackPreTask;
                    }
                }
                else if (data.newStatus == PEProtocol.TaskStatus.Failed)
                {
                    if(dbMgr.DeleteTaskInfo(pd.id, info.taskID))
                    {
                        msg.rspUpdateTaskInfo = new RspUpdateTaskInfo
                        {
                            info = info
                        };
                    }
                    else
                    {
                        msg.cmd = (int)ErrorCode.UpdateDBError;
                    }
                }
            }

            pack.session.SendMsg(msg);
        }

        public void ReqUpdateTaskPrg(MsgPack pack)
        {
            ReqUpdateTaskPrg data = pack.msg.reqUpdateTaskPrg;
            PlayerData pd = CacheSvc.Instance.GetPlayerDataBySession(pack.session);
            NTaskInfo info = dbMgr.GetPlayerTaskInfo(pd.id, data.taskId);
            TaskDefine task = cfgSvc.GetTaskData(data.taskId);
            GameMsg msg = new GameMsg();
            if (info != null)
            {
                int count = data.count + info.prg;
                if(count > task.targetCount)
                {
                    count = task.targetCount;
                }
                if (dbMgr.UpdatePlayerTaskPrg(pd.id, data.taskId, count))
                {
                    if (count == task.targetCount)
                    {
                        NTaskInfo newInfo = new NTaskInfo
                        {
                            taskID = data.taskId,
                            npcID = task.submitNpcID,
                            prg = count,
                            taskState = PEProtocol.TaskStatus.Complated
                        };
                        if (dbMgr.UpdateTaskInfo(pd.id, newInfo))
                        {
                            msg.cmd = (int)CMD.RspUpdateTaskInfo;
                            msg.rspUpdateTaskInfo = new RspUpdateTaskInfo
                            {
                                info = new NTaskInfo
                                {
                                    taskID = data.taskId,
                                    npcID = task.submitNpcID,
                                    prg = data.count + info.prg,
                                    taskState = PEProtocol.TaskStatus.Complated
                                }
                            };
                        }
                        else
                        {
                            msg.cmd = (int)ErrorCode.UpdateDBError;
                        }                       
                    }
                    else
                    {
                        msg.cmd = (int)CMD.RspUpdateTaskPrg;
                        msg.rspUpdateTaskPrg = new RspUpdateTaskPrg
                        {
                            taskId = data.taskId,
                            prg = data.count + info.prg
                        };
                    }
                }
                else
                {
                    msg.cmd = (int)ErrorCode.UpdateDBError;
                }
                
            }
            else
            {
                msg.cmd = (int)ErrorCode.ClientDataError;
            }

            pack.session.SendMsg(msg);
        }


        //public void ReqTakeTaskReward(MsgPack pack)
        //{
        //    ReqTakeTaskReward data = pack.msg.reqTakeTaskReward;
        //    GameMsg msg = new GameMsg
        //    {
        //        cmd = (int)CMD.RspTakeTaskReward,
        //    };
        //    PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
        //    TaskRewardData trd = CalcTaskRewardData(pd, data.rid);
        //    TaskCfg tc = cfgSvc.GetTaskCfgData(data.rid);
        //
        //    if (tc.count == trd.prgs && !trd.taked)
        //    {
        //        pd.coin += tc.coin;
        //        PECommon.CalcExp(pd, tc.exp);
        //        trd.taked = true;
        //        CalcTaskArr(pd, trd);
        //
        //        if (!cacheSvc.UpdatePlayerData(pd.id, pd))
        //        {
        //            msg.err = (int)ErrorCode.UpdateDBError;
        //        }
        //        else
        //        {
        //            msg.rspTakeTaskReward = new RspTakeTaskReward
        //            {
        //                coin = pd.coin,
        //                exp = pd.exp,
        //                lv = pd.lv,
        //                taskArr = pd.task,
        //            };
        //
        //        }
        //    }
        //    else
        //    {
        //        msg.err = (int)ErrorCode.ClientDataError;
        //    }
        //    pack.session.SendMsg(msg);
        //}
        /// <summary>
        /// 从PlayerData中提取出对应id任务的完成信息
        /// </summary>
        /// <param name="pd"></param>
        /// <param name="rid"></param>
        /// <returns></returns>
        //private TaskRewardData CalcTaskRewardData(PlayerData pd, int rid)
        //{
        //    string[] _taskArr = pd.task;
        //    TaskRewardData trd = null;
        //    for (int i = 0; i < _taskArr.Length; i++)
        //    {
        //        string[] taskInfo = _taskArr[i].Split('|');
        //        if (int.Parse(taskInfo[0]) == rid)
        //        {
        //            trd = new TaskRewardData
        //            {
        //                ID = int.Parse(taskInfo[0]),
        //                prgs = int.Parse(taskInfo[1]),
        //                taked = taskInfo[2].Equals("1")
        //            };
        //
        //        }
        //    }
        //
        //    return trd;
        //}
        /// <summary>
        /// 修改PlayerData中task数组对应id任务的完成信息
        /// </summary>
        /// <param name="pd"></param>
        /// <param name="data"></param>
        //private void CalcTaskArr(PlayerData pd, TaskRewardData data)
        //{
        //    string[] taskArr = pd.task;
        //    string result = data.ID + "|" + data.prgs + "|" + (data.taked ? 1 : 0);
        //    int index = -1;
        //    for (int i = 0; i < taskArr.Length; i++)
        //    {
        //        string[] taskInfo = taskArr[i].Split('|');
        //        if(int.Parse(taskInfo[0]) == data.ID)
        //        {
        //            index = i;
        //            break;
        //        }
        //    }
        //    if(index != -1)
        //    pd.task[index] = result;
        //}

        //public void CalcTaskPrgs(PlayerData pd, int tid)
        //{
        //    TaskCfg tc = cfgSvc.GetTaskCfgData(tid);
        //    TaskRewardData trd = CalcTaskRewardData(pd, tid);
        //
        //    if (trd.prgs < tc.count)
        //    { 
        //        trd.prgs += 1;
        //        CalcTaskArr(pd, trd);
        //
        //        ServerSession session = cacheSvc.GetOnlineSessionByID(pd.id);
        //        GameMsg msg = new GameMsg
        //        {
        //            cmd = (int)CMD.PushTaskPrgs,
        //            pushTaskPrgs = new PushTaskPrgs
        //            {
        //                taskArr = pd.task,
        //            }
        //        };
        //
        //        session.SendMsg(msg);
        //    }
        //}
        //
        //public PushTaskPrgs GetTaskPrgs(PlayerData pd, int tid)
        //{
        //    TaskCfg tc = cfgSvc.GetTaskCfgData(tid);
        //    TaskRewardData trd = CalcTaskRewardData(pd, tid);
        //    PushTaskPrgs pushTaskPrgs = null;
        //    if (trd.prgs < tc.count)
        //    {
        //        trd.prgs += 1;
        //        CalcTaskArr(pd, trd);
        //        pushTaskPrgs = new PushTaskPrgs
        //        {
        //            taskArr = pd.task
        //        };
        //
        //        return pushTaskPrgs;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //
        //    
        //}
    }
}

