using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyWin : WinRoot
{

    public Text txtBuy;
    private int buyType;
    private PlayerData pd;
    protected override void InitWin()
    {
        pd = GameRoot.Instance.PlayerData;
        base.InitWin();
        RefreshUI();
    }

    public void SetBuyType(int type)
    {
        buyType = type;
    }

    public void OnClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        SetWinState(false);
    }

    public void OnClickBuyBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.ReqBuy
        };

        msg.reqBuy = new ReqBuy
        {
            buyType = buyType,
            diamond = 100
        };

        netSvc.SendMessage(msg);
    }

    public void RefreshUI()
    {
        switch (buyType)
        {
            case 0:
                txtBuy.text = "�Ƿ�Ҫ����" + Message.Color("10��ʯ", Message.ColorRed) + "����" + Message.Color("1000���", Message.ColorGreen);
                break;
            case 1:
                txtBuy.text = "�Ƿ�Ҫ����" + Message.Color("10��ʯ", Message.ColorRed) + "����" + Message.Color("100����", Message.ColorGreen);
                break;
        }
    }

}
