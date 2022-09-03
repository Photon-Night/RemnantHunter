using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocol;

public class MainCitySystem : SystemRoot
{
    public static MainCitySystem Instance = null;
    public MainCityWin mainCityWin;
    public InfoWin infoWin;

    private PlayerController pc = null;
    private Transform charShowCam = null;
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

            if(charShowCam != null)
            {
                charShowCam.gameObject.SetActive(false);
            }
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

    public void OpenInfoWin()
    {
        if(charShowCam == null)
        {
            charShowCam = GameObject.FindGameObjectWithTag("CharShowCam").transform;
        }
        charShowCam.localPosition = pc.transform.position + pc.transform.forward * 2.6f + new Vector3(0, 1.3f, 0);
        charShowCam.transform.localEulerAngles = new Vector3(0,180 + pc.transform.localEulerAngles.y, 0);
        charShowCam.localScale = Vector3.one;

        charShowCam.gameObject.SetActive(true);
        infoWin.SetWinState();
    }    

    public void CloseInfoWin()
    {
        if(charShowCam != null)
        {
            charShowCam.gameObject.SetActive(false);
        }
        infoWin.SetWinState(false);
    }

    private float startRoate = 0;
    public void SetStartRoate()
    {
        startRoate = pc.transform.localEulerAngles.y;
    }

    public void SetPlayerRoate(float roate)
    {
        pc.transform.localEulerAngles = new Vector3(0, startRoate + roate, 0);
    }
}
