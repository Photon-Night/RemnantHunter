using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Game.UIWin
{
    public class BagUIItem : UIRoot
    {
        public Text txtCount;
        public Text txtName;
        public Image imgIcon;
        public Image imgFrame_Normal;
        public Image imgFrame_Select;
        public Image imgIsEquipped;

        public Toggle togItem;
        public void InitItem(int count, string name, string iconPath, ToggleGroup group, bool isEquipped = false)
        {
            resSvc = ResService.Instance;
            SetText(txtCount, count);
            SetText(txtName, name);
            SetSprite(imgIcon, iconPath);
            togItem.group = group;
            SetActive(imgIsEquipped, isEquipped);
        }

        public void SetCount(int num)
        {
            SetText(txtCount, num);
        }
    }
}
