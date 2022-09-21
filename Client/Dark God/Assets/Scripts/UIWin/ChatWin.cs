using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PEProtocol;

public class ChatWin : WinRoot
{
    public Image imgWorld;
    public Image imgGuild;
    public Image imgFriend;
    public Text txtChat;
    public InputField iptChat;

    private int chatType;
    private List<string> chatLst = new List<string>();

    protected override void InitWin()
    {
        base.InitWin();
        chatType = 0;
    }


    public void OnClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        SetWinState(false);
    }

    public void RefreshUI()
    {
        if(chatType == 0)
        {
            string chatMsg = "";
            for (int i = 0; i < chatLst.Count; i++)
            {
                chatMsg += chatLst[i] + "\n";
            }
            SetText(txtChat, chatMsg);

            SetSprite(imgWorld, PathDefine.BtnType1);
            SetSprite(imgGuild, PathDefine.BtnType2);
            SetSprite(imgFriend, PathDefine.BtnType2);
        }

        else if(chatType == 1)
        {
            SetSprite(imgWorld, PathDefine.BtnType2);
            SetSprite(imgGuild, PathDefine.BtnType1);
            SetSprite(imgFriend, PathDefine.BtnType2);
        }

        else if(chatType == 2)
        {
            SetSprite(imgWorld, PathDefine.BtnType2);
            SetSprite(imgGuild, PathDefine.BtnType2);
            SetSprite(imgFriend, PathDefine.BtnType1);
        }
    }

    public void OmClickChangeChatTypeBtn(int type)
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        chatType = type;
        RefreshUI();
    }

    public void OnClickSendBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        if (iptChat.text != null && iptChat.text != "" && iptChat.text != " ")
        {
            if(iptChat.text.Length > 17)
            {
                GameRoot.AddTips("输入内容过长，请勿超过17字");
            }
            else
            {
                GameMsg msg = new GameMsg
                {
                    cmd = (int)CMD.SendChat
                };

                msg.sendChat = new SendChat
                {
                    chat = iptChat.text
                };

                netSvc.SendMessage(msg);

                iptChat.text = "";
            }
        }
        else
        {
            GameRoot.AddTips("请输入文字");
        }
    }
    public void AddChatMsg(PushChat data)
    {
        string chat = Message.Color(data.name, Message.ColorBlue) + ": " + data.chat;
        chatLst.Add(chat);
        if(chatLst.Count > 12)
        {
            chatLst.RemoveAt(0);
        }
        if (GetWinState())
        {
            RefreshUI();
        }
    }
}
