using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleManager : MonoBehaviour
{
    ResService resSvc = null;
    AudioService audioSvc = null;

    MapManager mapMgr = null;
    SkillManager skillMgr = null;
    StateManager stateMgr = null;
    public void InitManager(int mapId)
    {
        PECommon.Log("BattleManager Loading");
        resSvc = ResService.Instance;
        audioSvc = AudioService.Instance;

        mapMgr = gameObject.AddComponent<MapManager>();
        mapMgr.InitManager();
        skillMgr = gameObject.AddComponent<SkillManager>();
        skillMgr.InitManager();
        

        MapCfg data = resSvc.GetMapCfgData(mapId);
        resSvc.LoadSceneAsync(data.sceneName, () =>
        {
            GameObject map = GameObject.FindGameObjectWithTag("MapRoot");

            mapMgr = map.gameObject.AddComponent<MapManager>();
            mapMgr.InitManager();

            map.transform.position = Vector3.zero;
            map.transform.localScale = Vector3.one;

            Camera.main.transform.position = data.mainCamPos;
            Camera.main.transform.localEulerAngles = data.mainCamRote;

            LoadPlayer(data);

            audioSvc.PlayBGMusic(Message.BGHuangYe);
        });
    }

    public void LoadPlayer(MapCfg data)
    {

    }



}

