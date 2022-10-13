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

    private EntityPlayer ep;
    public void InitManager(int mapId)
    {
        PECommon.Log("BattleManager Loading");
        resSvc = ResService.Instance;
        audioSvc = AudioService.Instance;

        skillMgr = gameObject.AddComponent<SkillManager>();
        skillMgr.InitManager();
        stateMgr = gameObject.AddComponent<StateManager>();
        stateMgr.InitManager();

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

    public void LoadPlayer(MapCfg mapData)
    {
        GameObject player = resSvc.LoadPrefab(PathDefine.AssissnBattlePlayerPrefab, true);
        player.transform.localPosition = mapData.playerBornPos;
        player.transform.localEulerAngles = mapData.playerBornRote;
        player.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);

        Camera.main.transform.position = mapData.mainCamPos;
        Camera.main.transform.localEulerAngles = mapData.mainCamRote;

        PlayerController pc = player.GetComponent<PlayerController>();
        pc.Init();

        ep = new EntityPlayer
        {
            stateMgr = stateMgr,
            controller = pc,
        };


       
    }

    public void ReqReleaseSkill(int index)
    {
        switch (index)
        {
            case 0:
                ReleaseNormalAttack();
                break;
            case 1:
                ReleaseSkill1();
                break;
            case 2:
                ReleaseSkill2();
                break;
            case 3:
                ReleaseSkill3();
                break;
        }
    }

    public void SetMoveDir(Vector2 dir)
    {
        PECommon.Log(dir + "");

        if(dir != Vector2.zero)
        {
            ep.Move();
        }
        else
        {
            ep.Idle();
        }
    }

    private void ReleaseSkill1()
    {
        PECommon.Log("skill1");
    }

    private void ReleaseSkill2()
    {
        PECommon.Log("skill2");
    }

    private void ReleaseSkill3()
    {
        PECommon.Log("skill3");
    }

    private void ReleaseNormalAttack()
    {
        PECommon.Log("normal");
    }
}

