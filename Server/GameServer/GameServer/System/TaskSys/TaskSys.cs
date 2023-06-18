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
                        
                        for(int i = 0; i < task.item.Length; i++)
                        {
                            var arr = task.item[i].Split('#');
                            BagSys.Instance.AddItem(pack.session, int.Parse(arr[0]), int.Parse(arr[1]));
                        }
                        
                        if (dbMgr.UpdateTaskInfo(pd.id, info) && BagSys.Instance.UpdateBagArrBySession(pack.session))
                        {
                            msg.rspUpdateTaskInfo = new RspUpdateTaskInfo
                            {
                                info = info,
                                coin = pd.coin ,
                                exp = pd.exp,
                                lv = pd.lv,
                                newBagArr  = pd.bag,
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



    }
}

