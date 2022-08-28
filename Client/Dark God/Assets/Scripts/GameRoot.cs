using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocol;

public class GameRoot : MonoSingleton<GameRoot>
{
    public LoadingWin loadingWin;  
    public DynamicWin dynamicWin;

    // Start is called before the first frame update
    void Start()
    {
        PECommon.Log("Game Start");
        ClearUIRoot();
        Init();
    }

    private void Init()
    {
        //组件初始化加载
        ResService.Instance.ServiceInit();
        NetService.Instance.ServiceInit();
        AudioService.Instance.ServiceInit();

        MainCitySystem mainCitySys = GetComponent<MainCitySystem>();
        mainCitySys.InitSystem();

        LoginSystem loginSys = GetComponent<LoginSystem>();
        loginSys.InitSystem();
        loginSys.OnLoginEnter();
    }

     private void ClearUIRoot()
    {
        Transform canvas = this.transform.Find("Canvas");
        for(int i = 0; i < canvas.childCount; i++)
        {
            canvas.GetChild(i).gameObject.SetActive(false);
        }

        dynamicWin.SetWinState();
    }

    public static void AddTips(string tip)
    {
        Instance.dynamicWin.AddTips(tip);
    }

    private PlayerData playerData = null;
    public PlayerData PlayerData
    {
        get
        {
            return playerData;
        }
    }

    public void SetPlayerData(RspLogin data)
    {
        playerData = data.playerData;
    }

    public void SetPlayerName(string name)
    {
        PlayerData.name = name;
    }
}
