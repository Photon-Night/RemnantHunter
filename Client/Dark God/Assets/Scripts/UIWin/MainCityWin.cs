using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCityWin : WinRoot
{
    public Text txtFight;
    public Text txtPower;
    public Image imgPowerPrg;
    public Text txtLevel;
    public Text txtName;
    public Text txtExpPrg;

    public Transform expPrgTrans;
    public Animation menuRootAnim;

    private bool menuRootState = true;

    protected override void InitWin()
    {
        base.InitWin();

        RefreshUI();
    }

    private void RefreshUI()
    {
        PlayerData pd = GameRoot.Instance.PlayerData;
        SetText(txtFight, PECommon.GetFightByProps(pd));
        SetText(txtPower, "体力" + pd.power + "/" + PECommon.GetPowerLimit(pd.lv));
        imgPowerPrg.fillAmount = pd.power * 1f / PECommon.GetPowerLimit(pd.lv);
        SetText(txtLevel, pd.lv);
        SetText(txtName, pd.name);

        
        int expPrgVal = (int)(pd.exp * 100f / PECommon.GetExpUpValByLv(pd.lv));
        Debug.Log(expPrgVal +" "+ pd.exp);
        SetText(txtExpPrg, expPrgVal + "%");

        //设置经验条显示。计算经验格排列自适应，计算经验显示
        int index = expPrgVal / 10;

        GridLayoutGroup gird = expPrgTrans.GetComponent<GridLayoutGroup>();

        float globalRate = 1f * Message.ScreenStandardHeight / Screen.height;
        float screenWidth = Screen.width * globalRate;
        float Width = (screenWidth - 223) / 10;

        gird.cellSize = new Vector2(Width, 19);

        for (int i = 0; i < expPrgTrans.childCount; i++)
        {
            Image img = expPrgTrans.GetChild(i).GetComponent<Image>();
            if (i < index)
            {
                img.fillAmount = 1;
            }
            else if (i == index)
            {
                img.fillAmount = expPrgVal % 10 * 1f / 10;
            }
            else
                img.fillAmount = 0;
        }

    }

    public void OnClickMenuRoot()
    {
        menuRootState = !menuRootState;
        AnimationClip clip;
        if(menuRootState)
        {
            clip = menuRootAnim.GetClip("MenuOpen");
        }
        else
        {
            clip = menuRootAnim.GetClip("MenuClose");
        }
        audioSvc.PlayUIAudio(Message.UIExtenBtn);
        menuRootAnim.Play(clip.name);
    }
}
