using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocol;

public class MainCitySystem : SystemRoot
{
    public static MainCitySystem Instance = null;
    public MainCityWin mainCityWin;
    private PlayerController pc = null;
    public override void InitSystem()
    {
        base.InitSystem();

        Instance = this;
        PECommon.Log("MainCitySystem Loading");
    }

    public void EnterMainCity()
    {
        MapCfg mapData = resSvc.GetMapCfgData(Message.MainCityMapID);

        resSvc.LoadSceneAsync(mapData.sceneName, () =>
        {
            PECommon.Log("Enter MainCity");

            //TODO 加载角色模型
            LoadPlayer(mapData);
            //加载主城ui
            mainCityWin.SetWinState();

            audioSvc.PlayerBGMusic(Message.BGMMainCity);
            
            //TODO 设置人物
        });
    }

    private void LoadPlayer(MapCfg mapData)
    {
        GameObject player = resSvc.LoadPrefab(PathDefine.AssissnCityPlayerPrefab, true);        
        player.transform.localEulerAngles = mapData.playerBornRote;
        player.transform.position = mapData.playerBornPos;
        player.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        Camera.main.transform.position = mapData.mainCamPos;
        Camera.main.transform.localEulerAngles = mapData.mainCamRote;

        pc = player.GetComponent<PlayerController>();
        pc.Init();
        
    }

    public void SetMoveDir(Vector2 dir)
    {
        if(dir == Vector2.zero)
        {
            pc.SetBlend(Message.BlendIdle);
        }
        else
        {
            pc.SetBlend(Message.BlendWalk);
        }
        pc.Dir = dir;
    }
}
