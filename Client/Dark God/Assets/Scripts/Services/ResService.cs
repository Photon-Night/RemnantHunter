using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResService : MonoSingleton<ResService>
{
    public void ServiceInit()
    {
        Debug.Log("ResService Loading");
    }

    private System.Action PrgCB = null;
    public void LoadSceneAsync(string sceneName, System.Action loaded = null)   
    {
        GameRoot.Instance.loadingWin.SetWinState();
        AsyncOperation sceneAsync = SceneManager.LoadSceneAsync(sceneName);
        PrgCB = () =>
        {
            float val = sceneAsync.progress;
            GameRoot.Instance.loadingWin.SetProgress(val);
            if(val == 1)
            {
                PrgCB = null;
                sceneAsync = null;
                GameRoot.Instance.loadingWin.SetWinState(false);
                if(loaded != null)
                {
                    loaded();
                }
            }
        };
    }

    private void Update()
    {
        if (PrgCB != null)
            PrgCB();
    }

    private Dictionary<string, AudioClip> adDic = new Dictionary<string, AudioClip>();
    public AudioClip LoadAudio(string path, bool cache = false)
    {
        AudioClip au = null;
        if(!adDic.TryGetValue(path, out au))
        {
            au = Resources.Load<AudioClip>(path);
            if(cache)
            {
                adDic.Add(path, au);
            }
        }
        return au;
    }
}
