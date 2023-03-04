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

    private TalkCfg talkData;
    private string[] dialogArr;
    private int index;
    private int npcID = -1;
    
    private List<GameObject> btnOptions = new List<GameObject>();
    protected override void InitWin()   
    {
        base.InitWin();
        talkData = resSvc.GetNpcTalkRootData(npcID);
        string npcName = resSvc.GetNPCData(npcID).name;
        SetText(txtName, npcName);
        dialogArr = talkData.dialogArr;
        SetTalk();
    }
    private void SetTalk()
    {
        SetText(txtTalk, dialogArr[index]);
        if (index == dialogArr.Length - 1 && talkData.selectLst != null)
        {
            SetActive(btnNextTalk.gameObject, false);
            SetAnswer(talkData.ID, talkData.selectLst);
        }
    }

    public void InitTalkData(int npcID)
    {
        index = 0;
        this.npcID = npcID;
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
            for(int i = 0; i < btnOptions.Count; i++)
            {
                Destroy(btnOptions[i]);
            }

            btnOptions.Clear();
        }

        for(int i = 0; i < ansLst.Length; i++)
        {
            TalkCfg _data = resSvc.GetTalkData(talkID, ansLst[i]);
            GameObject go = resSvc.LoadPrefab(PathDefine.BtnOption); 
            go.transform.SetParent(optionContent);
            int nextIndex = _data.nextIndex;
            int nextTalkID = _data.nextTalkID;

            go.GetComponentInChildren<Text>().text = _data.dialogArr[0];

            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                audioSvc.PlayUIAudio(Message.UIClickBtn);

                index = 0;
                SetNextTalkData(nextTalkID, nextIndex);

                SetActive(optionContent, false);
                SetActive(btnNextTalk.gameObject);

                SetTalk();
            });

            btnOptions.Add(go);
        }

        SetActive(optionContent);
    }

    public void OnClickNextTalkBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        index += 1;

        if (index == dialogArr.Length)
        {
            NPCManager.Instance.InteractiveNpcFunction(talkData.actID, npcID);
            MainCitySystem.Instance.OnPlayerOverTalk(npcID);
            SetWinState(false);
        }
        else
        {
            SetTalk();
        }
    }
}


