using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemRoot <T>: MonoSingleton<T> where T:MonoBehaviour
{
    protected ResService resSvc;
    protected AudioService audioSvc;

    public virtual void InitSystem()
    {
        resSvc = ResService.Instance;
        audioSvc = AudioService.Instance;
    }
}
