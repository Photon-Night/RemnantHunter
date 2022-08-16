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
    public void LoadSceneAsync(string sceneName)
    {
        GameRoot.Instance.loadingWin.InitWin();
        GameRoot.Instance.loadingWin.gameObject.SetActive(true);
        AsyncOperation sceneAsync = SceneManager.LoadSceneAsync(sceneName);
        PrgCB = () =>
        {
            float val = sceneAsync.progress;
            GameRoot.Instance.loadingWin.SetProgress(val);
            if(val == 1)
            {
                PrgCB = null;
                sceneAsync = null;
                GameRoot.Instance.loadingWin.gameObject.SetActive(false);
            }
        };
    }

    private void Update()
    {
        if (PrgCB != null)
            PrgCB();
    }
}
