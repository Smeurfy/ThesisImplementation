using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TierMonsterBtn : MonoBehaviour
{
    Button btnSelected;

    public void TierSelect(Button btn)
    {
        var monsterSelected = btn.transform.parent.Find("Button").GetComponentInChildren<Text>().text;
        //enable button only if monster selected
        if (monsterSelected == "Selected")
        {
            if (btnSelected == null)
            {
                btnSelected = btn;
            }
            ColorBlock colors = btn.colors;
            colors.normalColor = btn.colors.highlightedColor;
            btn.colors = colors;

            //unselect the other btn
            if (btnSelected != btn)
            {
                ColorBlock colors1 = btnSelected.colors;
                colors1.normalColor = new Color(1f, 1f, 1f);
                btnSelected.colors = colors1;
                btnSelected = btn;
            }
        }
    }

    public void UnselectAllBtns(Transform trans)
    {
        foreach (var item in trans.GetComponentsInChildren<Button>())
        {
            ColorBlock colors1 = item.colors;
            colors1.normalColor = new Color(1f, 1f, 1f);
            item.colors = colors1;
        }
    }
}
