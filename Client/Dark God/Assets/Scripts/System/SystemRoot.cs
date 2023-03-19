using Game.Service;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemRoot <T>: MonoSingleton<T> where T:MonoBehaviour
{
    protected ResService resSvc;
    protected AudioService audioSvc;
    protected TimerService timeSvc;
    protected NetService netSvc;
    protected DirectorService dreSvc;
    public virtual void InitSystem()
    {
        resSvc = ResService.Instance;
        audioSvc = AudioService.Instance;
        timeSvc = TimerService.Instance;
        netSvc = NetService.Instance;
        dreSvc = DirectorService.Instance;
    }
}
