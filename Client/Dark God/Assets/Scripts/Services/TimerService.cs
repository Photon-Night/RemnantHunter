using PEProtocol;
using UnityEngine;
using System;

public class TimerService : MonoSingleton<TimerService>
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
}
