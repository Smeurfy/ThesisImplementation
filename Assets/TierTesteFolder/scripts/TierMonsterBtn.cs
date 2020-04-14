using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TierMonsterBtn : MonoBehaviour
{
    Button btnSelected;
    public Text tierName;
	public Canvas canvas;

	PopulateWithMonsters ptn;

	void Start(){
		ptn = canvas.GetComponent<PopulateWithMonsters>();
	}
    public void TierSelect(Button btn)
    {
		var enemy = ptn.testRoom.GetComponentInChildren<RoomManager>().GetEnemyHolder().GetComponentInChildren<SpriteRenderer>().gameObject;
        if (btnSelected == null)
        {
            btnSelected = btn;
            ColorBlock colors = btn.colors;
            colors.normalColor = btn.colors.highlightedColor;
            btn.colors = colors;
            tierName.text = ": " + btn.transform.parent.transform.parent.name + " " + btn.name;
			ptn.GetMonsterCharac(enemy, btn.name);
        }
        else
        {
            if (btnSelected == btn)
            {
                ColorBlock colors1 = btn.colors;
                colors1.normalColor = new Color(1f, 1f, 1f);
                btn.colors = colors1;
                btnSelected = null;
                tierName.text = ": " + btn.transform.parent.transform.parent.name + " " + "default";
				ptn.GetMonsterCharac(enemy, "default");
            }
            else
            {
                //make btnSelected white
                ColorBlock colors1 = btnSelected.colors;
                colors1.normalColor = new Color(1f, 1f, 1f);
                btnSelected.colors = colors1;
                //make the new btn selected grey
                ColorBlock colors = btn.colors;
                colors.normalColor = btn.colors.highlightedColor;
                btn.colors = colors;
                //update the selected btn
                btnSelected = btn;
                tierName.text = ": " + btn.transform.parent.transform.parent.name + " " + btn.name;
				ptn.GetMonsterCharac(enemy, btn.name);
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

    public void EnableTierButtonsForSelectedMonster(Transform transform, bool bolean)
    {
        transform.Find("BtnTiers").gameObject.SetActive(bolean);
    }

}
