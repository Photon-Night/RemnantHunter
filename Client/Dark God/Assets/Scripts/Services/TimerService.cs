using PEProtocol;
using UnityEngine;
using System;
using Game.Common;

public class TimerService : MonoSingleton<TimerService>, IService
{

    private PETimer pt;
    public void ServiceInit()
    {
        PECommon.Log("TimeService Loading");
        pt = new PETimer();
        pt.SetLog((string info) =>
        {
            PECommon.Log(info);
        });

    }

    public void Update()
    {
        if(pt != null)
        pt.Update();
    }

    public int AddTimeTask(Action<int> callback, double delay, PETimeUnit timeUnit = PETimeUnit.Millisecond, int count = 1)
    {
        return pt.AddTimeTask(callback, delay, timeUnit, count);
    }

    public double GetCurrentTime()
    {
        return pt.GetMillisecondsTime();
    }

    public void DeleteTimeTask(int tid)
    {
        pt.DeleteTimeTask(tid);
    }

    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }
}
