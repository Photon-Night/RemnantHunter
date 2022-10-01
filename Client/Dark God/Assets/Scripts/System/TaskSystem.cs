using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskSystem : SystemRoot<TaskSystem>
{
    public override void InitSystem()
    {
        base.InitSystem();
        PECommon.Log("TaskSystem Loading");    
    }

   public void RspTakeTaskReward(GameMsg msg)
   {
        RspTakeTaskReward data = msg.rspTakeTaskReward;
        PlayerData pd = GameRoot.Instance.PlayerData;
        GameRoot.Instance.SetPlayerDataByTakeTaskReward(data);
        GameRoot.AddTips("任务完成");
        GameRoot.AddTips(Message.Color("金币 + " + (data.coin - pd.coin) + "经验 + " + (data.exp - pd.exp), Message.ColorBlue));
   }

}
