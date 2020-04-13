using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PopulateWithMonsters : MonoBehaviour
{
    public Dictionary<string, Dictionary<string, MonstersInfo>> monstersInfo;
    public Image image;
    List<GameObject> enemies = new List<GameObject>();
    public int canvasWidth, canvasHeight;
    int numberOfEnemies;
    Button btnSelected;
    public GameObject testRoom;
    public GameObject monsterCharac;
    public bool canChangeValue = false;
    public GameObject attackLine;
    public GameObject stopLine;

    Vector3 startingPosition;
    // Use this for initialization
    void Start()
    {
        canvasHeight = (int)GetComponent<RectTransform>().rect.height;
        canvasWidth = (int)GetComponent<RectTransform>().rect.width;
        enemies = EnemyLibrary.instance.GetAllPossibleEnemiesPrefabs();
        numberOfEnemies = enemies.Count;
        startingPosition = image.transform.position;
        monstersInfo = new Dictionary<string, Dictionary<string, MonstersInfo>>();
        InitializeDic();
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

    private void InitializeDic()
    {
        foreach (var item in enemies)
        {
            monstersInfo.Add(item.name, new Dictionary<string, MonstersInfo>());
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
            //btn becomes selected
            btn.GetComponentInChildren<Text>().text = "Selected";
            ColorBlock colors = btn.colors;
            colors.normalColor = btn.colors.highlightedColor;
            btn.colors = colors;

            //If othh btn is selected them unselect the selected btn
            if (btnSelected != btn)
            {
                DestroyMonster();
                btnSelected.GetComponentInChildren<Text>().text = "Select";
                ColorBlock colors1 = btnSelected.colors;
                colors1.normalColor = new Color(1f, 1f, 1f);
                btnSelected.colors = colors1;
                //if a tier is selected for a monster it becomes unselected
                GetComponent<TierMonsterBtn>().UnselectAllBtns(btnSelected.transform.parent);
                btnSelected = btn;

            }
            var enemy = CreateMonster(btn.transform.parent);
            GetMonsterCharac(enemy);
        }
        else
        {
            //Unselect the same monster
            if (btnSelected == btn)
            {
                DestroyMonster();
                btn.GetComponentInChildren<Text>().text = "Select";
                ColorBlock colors = btn.colors;
                colors.normalColor = new Color(1f, 1f, 1f);
                btn.colors = colors;
                btnSelected = null;
                ResetValuesToZero();
                GetComponent<TierMonsterBtn>().UnselectAllBtns(btn.transform.parent);
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
                obj.name = item.name;
                var lineA = Instantiate(attackLine, obj.transform.position, Quaternion.identity, obj.transform);
                var lineS = Instantiate(stopLine, obj.transform.position, Quaternion.identity, obj.transform);
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
                PopulatePlaceholder(item, 0, 30, enemyCharac.numberOfBullets, true);
            }
            if (item.name == "BulletSpeed")
            {
                PopulatePlaceholder(item, 0, 20, enemyCharac.bulletSpeed, false);
            }
            if (item.name == "NumberOfWaves")
            {
                PopulatePlaceholder(item, 0, 15, enemyCharac.numberOfWaves, true);
            }
            if (item.name == "SecBtwWaves")
            {
                PopulatePlaceholder(item, 0, 20, enemyCharac.secondsBetweenWaves, true);
            }
            if (item.name == "SecBtwShoots")
            {
                PopulatePlaceholder(item, 0, 1, enemyCharac.secondsBetweenShots, false);
            }
            if (item.name == "AngleToShoot")
            {
                PopulatePlaceholder(item, 0, 360, enemyCharac.angleToShootInDegrees, true);
            }
            if (item.name == "AttackDistance")
            {
                PopulatePlaceholder(item, 0, 20, enemy.GetComponent<Thesis.Enemy.EnemyControllerTest>().attackDistance, false);
                enemy.GetComponentInChildren<DrawAttackLine>().radius = enemy.GetComponent<Thesis.Enemy.EnemyControllerTest>().attackDistance;
            }
            if (item.name == "StoppingDistance")
            {
                PopulatePlaceholder(item, 0, 15, enemy.GetComponent<Thesis.Enemy.EnemyMovementTest>().stoppingDistanceToPlayer, false);
                enemy.GetComponentInChildren<DrawStopLine>().radius = enemy.GetComponent<Thesis.Enemy.EnemyMovementTest>().stoppingDistanceToPlayer;
            }
            if (item.name == "MovSpeed")
            {
                PopulatePlaceholder(item, 0, 15, enemy.GetComponent<Thesis.Enemy.EnemyMovementTest>().movementSpeed, false);
            }
            if (item.name == "AttackSpeed")
            {
                PopulatePlaceholder(item, 0, 15, enemy.GetComponentInChildren<Thesis.Enemy.EnemyShootTest>().timeToWaitBeforeShootingAgain, false);
            }
        }
        AddEnemyToDic(enemy);
        canChangeValue = true;
    }

    private void AddEnemyToDic(GameObject enemy)
    {
        var enemyCharac = enemy.GetComponentInChildren<BulletSpawner>();
        MonstersInfo mInfo = new MonstersInfo(enemy.name, "default", enemyCharac.numberOfBullets, enemyCharac.bulletSpeed, enemyCharac.numberOfWaves, enemyCharac.secondsBetweenWaves,
                                                    enemyCharac.secondsBetweenShots, enemyCharac.angleToShootInDegrees, enemy.GetComponent<Thesis.Enemy.EnemyControllerTest>().attackDistance,
                                                    enemy.GetComponent<Thesis.Enemy.EnemyMovementTest>().stoppingDistanceToPlayer, enemy.GetComponent<Thesis.Enemy.EnemyMovementTest>().movementSpeed,
                                                    enemy.GetComponentInChildren<Thesis.Enemy.EnemyShootTest>().timeToWaitBeforeShootingAgain);
        if (!monstersInfo[enemy.name].ContainsKey("default"))
        {
            monstersInfo[enemy.name].Add("default", mInfo);
        }
        else
        {
            //update monster info
            monstersInfo[enemy.name].Remove("default");
            monstersInfo[enemy.name].Add("default", mInfo);
        }
    }

    private void PopulatePlaceholder(RectTransform item, int min, int max, float value, bool wholeNumbers)
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
            var sliderValue = slider.GetComponentInChildren<Slider>().value;
            slider.transform.parent.transform.Find("Values").GetComponent<Text>().text = "Value: " + sliderValue;
            var enemySelected = testRoom.GetComponentInChildren<RoomManager>().GetEnemyHolder().GetComponentInChildren<BulletSpawner>();
            var characChanged = slider.transform.parent;
            if (characChanged.name == "NBullets")
            {
                enemySelected.numberOfBullets = (int)sliderValue;
            }
            if (characChanged.name == "BulletSpeed")
            {
                enemySelected.bulletSpeed = (int)sliderValue;
            }
            if (characChanged.name == "NumberOfWaves")
            {
                enemySelected.numberOfWaves = (int)sliderValue;
            }
            if (characChanged.name == "SecBtwWaves")
            {
                enemySelected.secondsBetweenWaves = (int)sliderValue;
            }
            if (characChanged.name == "SecBtwShoots")
            {
                enemySelected.secondsBetweenShots = sliderValue;
            }
            if (characChanged.name == "AngleToShoot")
            {
                enemySelected.angleToShootInDegrees = sliderValue;
            }
            if (characChanged.name == "AttackDistance")
            {
                enemySelected.transform.parent.GetComponent<Thesis.Enemy.EnemyControllerTest>().attackDistance = sliderValue;
                enemySelected.transform.parent.GetComponentInChildren<DrawAttackLine>().radius = sliderValue;
            }
            if (characChanged.name == "StoppingDistance")
            {
                enemySelected.transform.parent.GetComponent<Thesis.Enemy.EnemyMovementTest>().stoppingDistanceToPlayer = sliderValue;
                enemySelected.transform.parent.GetComponentInChildren<DrawStopLine>().radius = sliderValue;
            }
            if (characChanged.name == "MovSpeed")
            {
                enemySelected.transform.parent.GetComponent<Thesis.Enemy.EnemyMovementTest>().movementSpeed = sliderValue;
            }
            if (characChanged.name == "AttackSpeed")
            {
                enemySelected.transform.parent.GetComponentInChildren<Thesis.Enemy.EnemyShootTest>().timeToWaitBeforeShootingAgain = sliderValue;
            }
            AddEnemyToDic(enemySelected.transform.parent.gameObject);
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
                PopulatePlaceholder(item, 0, 1, 0, false);
            }
            if (item.name == "AngleToShoot")
            {
                PopulatePlaceholder(item, 0, 360, 0, true);
            }
        }

    }
}
