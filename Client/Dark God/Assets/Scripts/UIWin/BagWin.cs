using Game.Bag;
using PEProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UIWin
{
    public class BagWin : WinRoot
    {
        public Toggle togPotion;
        public Toggle togEquipment;
        public Toggle togProp;
        public ToggleGroup togGroupitem;

        public Text txtItemName;
        public Text txtItemDes;
        public Text txtTitleFunc;
        public Text txtFuncNum;
        public Text txtTitleDuration;
        public Text txtDuration;

        public Image imgHadEquiped;

        public Button btnUse;
        public Button btnEquip;

        public Transform bagItemContent;
        public Transform rightPanel;

        private List<BagItemData> currentItemLst = new List<BagItemData>();
        private BagItemType currentType = BagItemType.Potion;
        private bool isInit = false;
        private BagItemData currentItemData;
        private BagUIItem currentUIItem;
        private BagUIItem currentEquippedWeapon;
        private BagUIItem currentEquippedShield;

        private bool changeByOpen = false;
        protected override void InitWin()
        {
            base.InitWin();
            if(!isInit)
            {
                togPotion.onValueChanged.AddListener((isOn) =>
                {
                    if(isOn)
                    {
                        currentType = BagItemType.Potion;
                        RefreshUI();
                    }
                });

                togEquipment.onValueChanged.AddListener((isOn) =>
                {
                    if (isOn)
                    {
                        currentType = BagItemType.Equipment;
                        RefreshUI();
                    }
                });

                togProp.onValueChanged.AddListener((isOn) =>
                {
                    if (isOn)
                    {
                        currentType = BagItemType.Prop;
                        RefreshUI();
                    }
                });

                isInit = true;
            }

            RefreshUI();
        }

        public void RefreshUI(int currentItemID = -1)
        {
            RefreshLeftPanel(currentItemID);
            RefreshRightPanel();
        }

        private void RefreshLeftPanel(int currentItemID = -1)
        {
            for (int i = 0; i < bagItemContent.childCount; i++)
            {
                Destroy(bagItemContent.GetChild(i).gameObject);
            }
            currentItemLst.Clear();

            BagSystem.Instance.GetBagItemLstByItemType(currentType, currentItemLst);

            if (currentItemLst.Count == 0) SetActive(rightPanel, false);
            else SetActive(rightPanel);

            for (int i = 0; i < currentItemLst.Count; i++)
            {
                var itemData = currentItemLst[i];
                var go = resSvc.LoadPrefab(PathDefine.BagItem, true);
                go.transform.SetParent(bagItemContent);
                BagUIItem uiItem = go.GetComponent<BagUIItem>();

                if (i == 0)
                {
                    currentItemData = itemData;
                    currentUIItem = uiItem;
                }

                if(currentItemID > 0 && currentItemID == itemData.cfg.ID)
                {
                    currentItemData = itemData;
                    currentUIItem = uiItem;
                }

                if (currentType == BagItemType.Equipment)
                {
                    bool isEquipped = BagSystem.Instance.isEquiped(currentItemLst[i].cfg.ID);
                    uiItem.InitItem(currentItemLst[i].count, currentItemLst[i].cfg.name, currentItemLst[i].cfg.iconPath, togGroupitem, isEquipped);
                    if(isEquipped)
                    {
                        switch (currentItemLst[i].cfg.equipmentType)
                        {
                            case EquipmentType.None:
                                break;
                            case EquipmentType.Weapon:
                                currentEquippedWeapon = uiItem;
                                break;
                            case EquipmentType.Shield:
                                currentEquippedShield = uiItem;
                                break;
                            default:
                                break;
                        }
                    }

                }
                else
                    uiItem.InitItem(currentItemLst[i].count, currentItemLst[i].cfg.name, currentItemLst[i].cfg.iconPath, togGroupitem);

                uiItem.togItem.onValueChanged.AddListener((isOn) =>
                {
                    if (isOn)
                    {
                        if (!changeByOpen)
                        {
                            audioSvc.PlayUIAudio(Message.UIClickBtn);
                        }
                        changeByOpen = false;
                        currentItemData = itemData;
                        currentUIItem = uiItem;
                        RefreshRightPanel();
                    }
                });
            }

            if(currentItemLst.Count > 0)
            {
                changeByOpen = true;
                
                if (currentUIItem.togItem.isOn)
                {
                    currentUIItem.togItem.onValueChanged.Invoke(true);
                }
                else
                {
                    currentUIItem.togItem.isOn = true;
                }
                
            }
        }

        private void RefreshRightPanel()
        {
            if (currentItemData == null) return;
            Debug.Log(currentItemData.cfg.funcType);
            switch (currentItemData.cfg.funcType)
            {
                case ItemFunction.Power:
                    SetText(txtTitleFunc, "体力");
                    break;
                case ItemFunction.Health:
                    SetText(txtTitleFunc, "生命");
                    break;
                case ItemFunction.ADAtk:
                    SetText(txtTitleFunc, "物攻");
                    break;
                case ItemFunction.Stamina:
                    SetText(txtTitleFunc, "耐力");
                    break;
                case ItemFunction.ADDef:
                    SetText(txtTitleFunc, "物防");
                    break;
                case ItemFunction.Dodge:
                    SetText(txtTitleFunc, "闪避");
                    break;
                case ItemFunction.APAtk:
                    SetText(txtTitleFunc, "魔攻");
                    break;
                case ItemFunction.APDef:
                    SetText(txtTitleFunc, "魔防");
                    break;
            }


            SetText(txtItemName, currentItemData.cfg.name);
            SetText(txtFuncNum, $"+{currentItemData.cfg.funcNum}");
            SetText(txtItemDes, currentItemData.cfg.des);

            if(currentItemData.cfg.duration > 0)
            {
                SetActive(txtTitleDuration);
                SetActive(txtDuration);
                SetText(txtDuration, $"{currentItemData.cfg.duration / 1000} s");
            }
            else
            {
                SetActive(txtTitleDuration, false);
                SetActive(txtDuration, false);
            }

            bool res = currentItemData.cfg.ItemType == BagItemType.Equipment;

            SetActive(btnUse, !res);
            SetActive(btnEquip, res && !BagSystem.Instance.isEquiped(currentItemData.cfg.ID));
            SetActive(imgHadEquiped, res && BagSystem.Instance.isEquiped(currentItemData.cfg.ID));

        }

        public void UpdateItemCount(int count)
        {
            currentUIItem.SetCount(count);
        }

        public void ChangeEquipment(int newEquipmentID)
        {
            var type = resSvc.GetGameItemCfg(newEquipmentID).equipmentType;
            switch (type)
            {
                case EquipmentType.None:
                    break;
                case EquipmentType.Weapon:
                    

                    break;
                case EquipmentType.Shield:
                    break;
                default:
                    break;
            }
        }

        public void OnClickUseBtn()
        {
            audioSvc.PlayUIAudio(Message.UIClickBtn);
            if (currentItemData.CanUse)
                BagSystem.Instance.UseProp(currentItemData);
            else
                GameRoot.AddTips($"{Message.Color(currentItemData.cfg.name, Message.ColorOrange)}只能在战斗中使用");
        }

        public void OnClickEquipBtn()
        {
            audioSvc.PlayUIAudio(Message.UIClickBtn);
            BagSystem.Instance.ChangeEquipment(currentItemData.cfg.ID, currentItemData.cfg.equipmentType);
        }

        public void OnClickCloseBtn()
        {
            audioSvc.PlayUIAudio(Message.UIClickBtn);
            SetWinState(false);
        }
    }
}
