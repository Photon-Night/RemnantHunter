using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoot : MonoSingleton<GameRoot>
{
    public LoadingWin loadingWin;



    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Game Start");
        Init();
    }

    private void Init()
    {
        //组件初始化加载
        ResService res = GetComponent<ResService>();
        res.ServiceInit();
        LoginSystem login = GetComponent<LoginSystem>();
        login.SystemInit();

        //
        login.OnLoginEnter();
    }
}
