using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TalkWin : WinRoot
{
    public Text txtName;
    public Text txtTalk;
    public Image imgIcon;
    public Button btnNextTalk;

    public Transform optionContent;


    private PlayerData pd;
    private GuideCfg currentTaskData;
    private TalkCfg talkData;
    private string[] dialogArr;
    private int index;
    private System.Action<int> OnTalkOverEvent;
    protected override void InitWin()   
    {
        base.InitWin();
        pd = GameRoot.Instance.PlayerData;
        //currentTaskData = MainCitySystem.Instance.GetCurrentTaskData();
        //dialogArr = currentTaskData.dilogArr.Split('#');
        //index = 1;
        SetTalk();
    }
    public void SetTalkData(GuideCfg data)
    {
        currentTaskData = data;
        dialogArr = currentTaskData.dilogArr.Split('#');
        index = 1;
    }
    public void RegisterTalkOverEvent(System.Action<int> func)
    {
        OnTalkOverEvent = func;
    }
    private void SetTalk()
    {
        SetText(txtTalk, dialogArr[index]);
    }

    public void InitTalkData(int npcID)
    {
        TalkCfg data = resSvc.GetNpcTalkRootData(npcID);
        string npcName = resSvc.GetNPCData(data.entityID).name;
        SetText(txtName, npcName);
        dialogArr = talkData.dialogArr;
    }

    private void SetNextTalkData(int talkID, int index)
    {
        talkData = resSvc.GetTalkData(talkID, index);
        dialogArr = talkData.dialogArr;
    }

    private void SetAnswer(int talkID, int[] ansLst)
    {
        if(optionContent.childCount != 0)
        {
            for(int i = 0; i < optionContent.childCount; i++)
            {
                Transform go = optionContent.GetChild(i);
                Destroy(go);
            }
        }

        for(int i = 0; i < ansLst.Length; i++)
        {
            TalkCfg _data = resSvc.GetTalkData(talkID, ansLst[i]);
            GameObject go = resSvc.LoadPrefab(PathDefine.BtnOption); 
            go.transform.SetParent(optionContent);
            int nextIndex = _data.nextIndex;
            int nextTalkID = _data.nextTalkID;
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                audioSvc.PlayUIAudio(Message.UIClickBtn);
                SetNextTalkData(nextTalkID, nextIndex);
                SetTalk();
                SetActive(optionContent, false);
            });
        }
    }

    public void OnClickNextTalkBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        index += 1;
        if(index == dialogArr.Length)
        {
            index = 0;

            if (talkData.selectLst.Length == 0)
            {
                switch (talkData.actID)
                {
                    case 0:
                        break;
                    case 1:
                        TaskSystem.Instance.OpenNpcTaskWin(talkData.entityID);
                        break;

                }

                OnTalkOverEvent(currentTaskData.npcID);
                SetWinState(false);
            }
            else
            {
                SetActive(btnNextTalk.gameObject, false);
                SetAnswer(talkData.ID, talkData.selectLst);
            }

        }
        else
        SetTalk();

    }
}


