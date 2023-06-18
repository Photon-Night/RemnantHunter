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
                txtBuy.text = "是否花费" + Message.Color("10钻石", Message.ColorRed) + "购买" + Message.Color("1000金币", Message.ColorOrange);
                break;
            case 1:
                txtBuy.text = "是否花费" + Message.Color("10钻石", Message.ColorRed) + "购买" + Message.Color("100体力", Message.ColorOrange);
                break;
        }
    }

}
