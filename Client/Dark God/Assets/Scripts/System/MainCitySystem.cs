using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocol;

public class MainCitySystem : SystemRoot
{
    public static MainCitySystem Instance = null;
    public MainCityWin mainCityWin;
    public override void InitSystem()
    {
        base.InitSystem();

        Instance = this;
        PECommon.Log("MainCitySystem Loading");
    }

    public void EnterMainCity()
    {
        resSvc.LoadSceneAsync(Message.SceneMainCity, () =>
        {
            PECommon.Log("Enter MainCity");

            //TODO 加载角色模型

            //加载主城ui
            mainCityWin.SetWinState();

            audioSvc.PlayerBGMusic(Message.BGMMainCity);

            //TODO 设置人物
        });
    }
}
