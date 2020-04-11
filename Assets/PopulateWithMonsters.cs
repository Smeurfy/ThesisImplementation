using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PopulateWithMonsters : MonoBehaviour
{

    public Image image;
    List<GameObject> enemies = new List<GameObject>();
    public int canvasWidth, canvasHeight;
    int numberOfEnemies;
    Button btnSelected;
    public GameObject testRoom;
    public GameObject monsterCharac;
    public bool canChangeValue = false;

    Vector3 startingPosition;
    // Use this for initialization
    void Start()
    {
        canvasHeight = (int)GetComponent<RectTransform>().rect.height;
        canvasWidth = (int)GetComponent<RectTransform>().rect.width;
        enemies = EnemyLibrary.instance.GetAllPossibleEnemiesPrefabs();
        numberOfEnemies = enemies.Count;
        startingPosition = image.transform.position;

        Populate();
    }

    private void Populate()
    {
        var margin = canvasWidth / numberOfEnemies;
        for (int i = 0; i < numberOfEnemies; i++)
        {
            var obj = Instantiate(image, startingPosition, Quaternion.identity, gameObject.transform);
            obj.GetComponent<Image>().sprite = enemies[i].GetComponent<SpriteRenderer>().sprite;
            obj.GetComponent<Image>().preserveAspect = true;


            startingPosition += new Vector3(margin, 0, 0);
            obj.gameObject.SetActive(true);
        }
    }

    public void SelectMonster(Button btn)
    {
        if (btnSelected == null)
        {
            btnSelected = btn;
        }
        if (btn.GetComponentInChildren<Text>().text == "Select")
        {
            btn.GetComponentInChildren<Text>().text = "Selected";
            ColorBlock colors = btn.colors;
            colors.normalColor = btn.colors.highlightedColor;
            btn.colors = colors;

            //unselect the other btn
            if (btnSelected != btn)
            {
                DestroyMonster();
                btnSelected.GetComponentInChildren<Text>().text = "Select";
                ColorBlock colors1 = btnSelected.colors;
                colors1.normalColor = new Color(1f, 1f, 1f);
                btnSelected.colors = colors1;
                btnSelected = btn;
            }
            var enemy = CreateMonster(btn.transform.parent);
            GetMonsterCharac(enemy);
        }
        else
        {
            if (btnSelected == btn)
            {
                DestroyMonster();
                btn.GetComponentInChildren<Text>().text = "Select";
                ColorBlock colors = btn.colors;
                colors.normalColor = new Color(1f, 1f, 1f);
                btn.colors = colors;
                btnSelected = null;
                ResetValuesToZero();
            }

        }
    }

    private GameObject CreateMonster(Transform parent)
    {
        var enemyName = parent.GetComponent<Image>().sprite.name;
        foreach (var item in enemies)
        {
            if (item.GetComponent<SpriteRenderer>().sprite.name == enemyName)
            {
                var obj = Instantiate(item, testRoom.transform.position + new Vector3(3, 0, 0), Quaternion.identity, testRoom.GetComponentInChildren<RoomManager>().GetEnemyHolder());
                return obj;
            }
        }
        return null;
    }

    private void DestroyMonster()
    {
        Destroy(testRoom.GetComponentInChildren<RoomManager>().GetEnemyHolder().GetComponentInChildren<SpriteRenderer>().gameObject);
        canChangeValue = false;
    }

    private void GetMonsterCharac(GameObject enemy)
    {
        var enemyCharac = enemy.GetComponentInChildren<BulletSpawner>();
        var placeHolders = monsterCharac.GetComponentsInChildren<RectTransform>();
        foreach (var item in placeHolders)
        {
            if (item.name == "NBullets")
            {
                PopulatePlaceholder(item, 0, 100, enemyCharac.numberOfBullets, true);
            }
            if (item.name == "BulletSpeed")
            {
                PopulatePlaceholder(item, 0, 100, enemyCharac.bulletSpeed, true);
            }
            if (item.name == "NumberOfWaves")
            {
                PopulatePlaceholder(item, 0, 100, enemyCharac.numberOfWaves, true);
            }
            if (item.name == "SecBtwWaves")
            {
                PopulatePlaceholder(item, 0, 100, enemyCharac.secondsBetweenWaves, true);
            }
            if (item.name == "SecBtwShoots")
            {
                //To do incremento de 0.05
                PopulatePlaceholder(item, 0, 1,(int)enemyCharac.secondsBetweenShots, true);
            }
            if (item.name == "AngleToShoot")
            {
                PopulatePlaceholder(item, 0, 360, (int)enemyCharac.angleToShootInDegrees, true);
            }
        }
        canChangeValue = true;
    }

    private void PopulatePlaceholder(RectTransform item, int min, int max, int value, bool wholeNumbers)
    {
        item.GetComponentInChildren<Slider>().minValue = min;
        item.GetComponentInChildren<Slider>().maxValue = max;
        item.GetComponentInChildren<Slider>().value = value;
        item.GetComponentInChildren<Slider>().wholeNumbers = wholeNumbers;
        item.transform.Find("Values").GetComponent<Text>().text = "Value: " + value;
    }

    public void UpdateValue(Slider slider)
    {
        if (canChangeValue)
        {
            slider.transform.parent.transform.Find("Values").GetComponent<Text>().text = "Value: " + slider.GetComponentInChildren<Slider>().value;
            var enemySelected = testRoom.GetComponentInChildren<RoomManager>().GetEnemyHolder().GetComponentInChildren<SpriteRenderer>().gameObject;
        }
    }

    private void ResetValuesToZero()
    {
        var placeHolders = monsterCharac.GetComponentsInChildren<RectTransform>();
        foreach (var item in placeHolders)
        {
            if (item.name == "NBullets")
            {
                PopulatePlaceholder(item, 0, 100, 0, true);
            }
            if (item.name == "BulletSpeed")
            {
                PopulatePlaceholder(item, 0, 100, 0, true);
            }
            if (item.name == "NumberOfWaves")
            {
                PopulatePlaceholder(item, 0, 100, 0, true);
            }
            if (item.name == "SecBtwWaves")
            {
                PopulatePlaceholder(item, 0, 100, 0, true);
            }
            if (item.name == "SecBtwShoots")
            {
                //To do incremento de 0.05
                PopulatePlaceholder(item, 0, 1, 0, true);
            }
            if (item.name == "AngleToShoot")
            {
                PopulatePlaceholder(item, 0, 360, 0, true);
            }
        }

    }
}
