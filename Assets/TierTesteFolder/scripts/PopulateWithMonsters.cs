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
    public Button btnSelected;
    public GameObject testRoom;
    public GameObject monsterCharac;
    public bool canChangeValue = false;
    public GameObject attackLine;
    public GameObject stopLine;
    public Text tierName;

    MonstersInfo mInfo;

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

    public void Populate()
    {
        var margin = canvasWidth / numberOfEnemies;
        for (int i = 0; i < numberOfEnemies; i++)
        {
            var obj = Instantiate(image, startingPosition, Quaternion.identity, gameObject.transform);
            obj.name = enemies[i].name;
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

            //If other btn is selected them unselect the selected btn
            if (btnSelected != btn)
            {
                DestroyMonster();
                btnSelected.GetComponentInChildren<Text>().text = "Select";
                ColorBlock colors1 = btnSelected.colors;
                colors1.normalColor = new Color(1f, 1f, 1f);
                btnSelected.colors = colors1;
                //if a tier is selected for a monster it becomes unselected
                GetComponent<TierMonsterBtn>().UnselectAllBtns(btnSelected.transform.parent);
                GetComponent<TierMonsterBtn>().EnableTierButtonsForSelectedMonster(btnSelected.transform.parent, false);
                btnSelected = btn;

            }
            var enemy = CreateMonster(btn.transform.parent);
            GetMonsterCharac(enemy, "default");
            GetComponent<TierMonsterBtn>().EnableTierButtonsForSelectedMonster(btn.transform.parent, true);
            DisableSlidersBasedOnMonster(enemy);
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
                GetComponent<TierMonsterBtn>().EnableTierButtonsForSelectedMonster(btn.transform.parent, false);
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

    public void DestroyMonster()
    {
        if (testRoom.GetComponentInChildren<RoomManager>().GetEnemyHolder().transform.childCount > 0)
            Destroy(testRoom.GetComponentInChildren<RoomManager>().GetEnemyHolder().GetComponentInChildren<SpriteRenderer>().gameObject);
        canChangeValue = false;
    }

    public void GetMonsterCharac(GameObject enemy, string mName)
    {
        var placeHolders = monsterCharac.GetComponentsInChildren<RectTransform>();
        if (monstersInfo[enemy.name].ContainsKey(mName))
        {
            mInfo = monstersInfo[enemy.name][mName];
            enemy.GetComponent<Thesis.Enemy.EnemyMovementTest>().stoppingDistanceToPlayer = mInfo.stoppingDistance;
            enemy.GetComponent<Thesis.Enemy.EnemyMovementTest>().movementSpeed = mInfo.movementSpeed;
            if (enemy.name == "iceZombieTest")
            {

                enemy.GetComponent<IceZombieControllerTest>().timeBetweenAttacksInSecs = mInfo.timeBetweenAttacks;
                enemy.GetComponent<IceZombiePhysicalAttack>().DurationOfAttackInSecs = mInfo.durationOfAttacks;
                enemy.GetComponent<IceZombiePhysicalAttack>().attackFlightSpeed = mInfo.attackSpeed;
            }
            else
            {
                var enemyCharac = enemy.GetComponentInChildren<BulletSpawner>();
                enemyCharac.numberOfBullets = (int)mInfo.numberBullets;
                enemyCharac.bulletSpeed = (int)mInfo.bulletSpeed;
                enemyCharac.numberOfWaves = (int)mInfo.numberOfWaves;
                enemyCharac.secondsBetweenWaves = (int)mInfo.secBtwWaves;
                enemyCharac.secondsBetweenShots = mInfo.secBtwShots;
                enemyCharac.angleToShootInDegrees = mInfo.angleToShoot;
                enemy.GetComponent<Thesis.Enemy.EnemyControllerTest>().attackDistance = mInfo.attackDistance;
                enemy.GetComponentInChildren<Thesis.Enemy.EnemyShootTest>().timeToWaitBeforeShootingAgain = mInfo.attackSpeed;
            }
        }
        else
        {
            mInfo = AddEnemyToDic(enemy, mName);
        }
        tierName.text = ": " + mInfo.monsterName + " " + mInfo.tier;
        foreach (var item in placeHolders)
        {
            if (item.name == "NBullets")
            {
                PopulatePlaceholder(item, 0, 30, mInfo.numberBullets, true);
            }
            if (item.name == "BulletSpeed")
            {
                PopulatePlaceholder(item, 0, 20, mInfo.bulletSpeed, false);
            }
            if (item.name == "NumberOfWaves")
            {
                PopulatePlaceholder(item, 0, 15, mInfo.numberOfWaves, true);
            }
            if (item.name == "SecBtwWaves")
            {
                PopulatePlaceholder(item, 0, 20, mInfo.secBtwWaves, true);
            }
            if (item.name == "SecBtwShoots")
            {
                PopulatePlaceholder(item, 0, 1, mInfo.secBtwShots, false);
            }
            if (item.name == "AngleToShoot")
            {
                PopulatePlaceholder(item, 0, 360, mInfo.angleToShoot, true);
            }
            if (item.name == "AttackDistance")
            {
                PopulatePlaceholder(item, 0, 20, mInfo.attackDistance, false);
                enemy.GetComponentInChildren<DrawAttackLine>().radius = enemy.GetComponent<Thesis.Enemy.EnemyControllerTest>().attackDistance;
            }
            if (item.name == "AttackSpeed")
            {
                PopulatePlaceholder(item, 0, 15, mInfo.attackSpeed, false);
            }
            if (item.name == "StoppingDistance")
            {
                PopulatePlaceholder(item, 0, 15, mInfo.stoppingDistance, false);
                enemy.GetComponentInChildren<DrawStopLine>().radius = enemy.GetComponent<Thesis.Enemy.EnemyMovementTest>().stoppingDistanceToPlayer;
            }
            if (item.name == "MovSpeed")
            {
                PopulatePlaceholder(item, 0, 15, mInfo.movementSpeed, false);
            }
            if (item.name == "TBetweenAttacks")
            {
                PopulatePlaceholder(item, 0, 15, mInfo.timeBetweenAttacks, false);
            }
            if (item.name == "DurationAttack")
            {
                PopulatePlaceholder(item, 0, 15, mInfo.durationOfAttacks, false);
            }
        }
        canChangeValue = true;
    }

    private MonstersInfo AddEnemyToDic(GameObject enemy, string mName)
    {
        MonstersInfo monsterInfo = new MonstersInfo();
        if (enemy.name == "iceZombieTest")
        {
            monsterInfo = new MonstersInfo(enemy.name, mName, enemy.GetComponent<Thesis.Enemy.EnemyMovementTest>().stoppingDistanceToPlayer,
                                                enemy.GetComponent<Thesis.Enemy.EnemyMovementTest>().movementSpeed, enemy.GetComponent<IceZombieControllerTest>().timeBetweenAttacksInSecs,
                                                enemy.GetComponent<IceZombiePhysicalAttack>().DurationOfAttackInSecs, enemy.GetComponent<IceZombiePhysicalAttack>().attackFlightSpeed);
        }
        else
        {
            var enemyCharac = enemy.GetComponentInChildren<BulletSpawner>();
            monsterInfo = new MonstersInfo(enemy.name, mName, enemyCharac.numberOfBullets, enemyCharac.bulletSpeed, enemyCharac.numberOfWaves, enemyCharac.secondsBetweenWaves,
                                                  enemyCharac.secondsBetweenShots, enemyCharac.angleToShootInDegrees, enemy.GetComponent<Thesis.Enemy.EnemyControllerTest>().attackDistance,
                                                  enemy.GetComponent<Thesis.Enemy.EnemyMovementTest>().stoppingDistanceToPlayer, enemy.GetComponent<Thesis.Enemy.EnemyMovementTest>().movementSpeed,
                                                  enemy.GetComponentInChildren<Thesis.Enemy.EnemyShootTest>().timeToWaitBeforeShootingAgain);
        }
        if (!monstersInfo[enemy.name].ContainsKey(mName))
        {
            monstersInfo[enemy.name].Add(mName, monsterInfo);
        }
        else
        {
            //update monster info
            monstersInfo[enemy.name].Remove(mName);
            monstersInfo[enemy.name].Add(mName, monsterInfo);
        }
        return monsterInfo;
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
            var enemySelected = testRoom.GetComponentInChildren<RoomManager>().GetEnemyHolder().GetComponentInChildren<Thesis.Enemy.EnemyHealthSystemTest>().transform.gameObject;
            var characChanged = slider.transform.parent;

            if (characChanged.name == "StoppingDistance")
            {
                enemySelected.GetComponent<Thesis.Enemy.EnemyMovementTest>().stoppingDistanceToPlayer = sliderValue;
                enemySelected.GetComponentInChildren<DrawStopLine>().radius = sliderValue;
            }
            if (characChanged.name == "MovSpeed")
            {
                enemySelected.GetComponent<Thesis.Enemy.EnemyMovementTest>().movementSpeed = sliderValue;
            }
            if (enemySelected.name == "iceZombieTest")
            {
                if (characChanged.name == "AttackSpeed")
                {
                    enemySelected.GetComponent<IceZombiePhysicalAttack>().attackFlightSpeed = sliderValue;

                }
                if (characChanged.name == "TBetweenAttacks")
                {
                    enemySelected.GetComponent<IceZombieControllerTest>().timeBetweenAttacksInSecs = sliderValue;
                }
                if (characChanged.name == "DurationAttack")
                {
                    enemySelected.GetComponent<IceZombiePhysicalAttack>().DurationOfAttackInSecs = sliderValue;
                }
                AddEnemyToDic(enemySelected, mInfo.tier);
            }
            else
            {
                var bulletSpawner = testRoom.GetComponentInChildren<RoomManager>().GetEnemyHolder().GetComponentInChildren<BulletSpawner>();
                if (characChanged.name == "NBullets")
                {
                    bulletSpawner.numberOfBullets = (int)sliderValue;
                    bulletSpawner.UpdateInitialValues();
                }
                if (characChanged.name == "BulletSpeed")
                {
                    bulletSpawner.bulletSpeed = (int)sliderValue;
                }
                if (characChanged.name == "NumberOfWaves")
                {
                    bulletSpawner.numberOfWaves = (int)sliderValue;
                }
                if (characChanged.name == "SecBtwWaves")
                {
                    bulletSpawner.secondsBetweenWaves = (int)sliderValue;
                }
                if (characChanged.name == "SecBtwShoots")
                {
                    bulletSpawner.secondsBetweenShots = sliderValue;
                }
                if (characChanged.name == "AngleToShoot")
                {
                    bulletSpawner.angleToShootInDegrees = sliderValue;
                    bulletSpawner.UpdateInitialValues();
                }
                if (characChanged.name == "AttackDistance")
                {
                    bulletSpawner.transform.parent.GetComponent<Thesis.Enemy.EnemyControllerTest>().attackDistance = sliderValue;
                    bulletSpawner.transform.parent.GetComponentInChildren<DrawAttackLine>().radius = sliderValue;
                }
                if (characChanged.name == "AttackSpeed")
                {
                    bulletSpawner.transform.parent.GetComponentInChildren<Thesis.Enemy.EnemyShootTest>().timeToWaitBeforeShootingAgain = sliderValue;
                }
                AddEnemyToDic(bulletSpawner.transform.parent.gameObject, mInfo.tier);
            }






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
                PopulatePlaceholder(item, 0, 1, 0, false);
            }
            if (item.name == "AngleToShoot")
            {
                PopulatePlaceholder(item, 0, 360, 0, true);
            }
            if (item.name == "AttackDistance")
            {
                PopulatePlaceholder(item, 0, 20, 0, false);
            }
            if (item.name == "StoppingDistance")
            {
                PopulatePlaceholder(item, 0, 15, 0, false);
            }
            if (item.name == "MovSpeed")
            {
                PopulatePlaceholder(item, 0, 15, 0, false);
            }
            if (item.name == "AttackSpeed")
            {
                PopulatePlaceholder(item, 0, 15, 0, false);
            }
            if (item.name == "TBetweenAttacks")
            {
                PopulatePlaceholder(item, 0, 15, 0, false);
            }
            if (item.name == "DurationAttack")
            {
                PopulatePlaceholder(item, 0, 15, 0, false);
            }
        }
    }

    public void UnselectBtn()
    {
        var topRow = GetComponentsInChildren<Image>();
        foreach (var item in topRow)
        {
            foreach (var enemy in enemies)
            {
                if (item.transform.parent.name == enemy.name)
                {
                    var btn = item.gameObject.GetComponentInChildren<Button>();
                    btn.GetComponentInChildren<Text>().text = "Select";
                    ColorBlock colors = btn.colors;
                    colors.normalColor = new Color(1f, 1f, 1f);
                    btn.colors = colors;
                    btnSelected = null;
                    ResetValuesToZero();
                    GetComponent<TierMonsterBtn>().UnselectAllBtns(btn.transform.parent);
                    GetComponent<TierMonsterBtn>().EnableTierButtonsForSelectedMonster(btn.transform.parent, false);
                }
            }
        }
        tierName.text = "";
    }

    void DisableSlidersBasedOnMonster(GameObject enemy)
    {
        var placeHolders = monsterCharac.GetComponentsInChildren<RectTransform>();
        foreach (var item in placeHolders)
        {
            if (enemy.name == "iceZombieTest")
            {
                if (item.name == "NBullets" || item.name == "BulletSpeed" || item.name == "NumberOfWaves" || item.name == "SecBtwWaves" ||
                    item.name == "SecBtwShoots" || item.name == "AngleToShoot" || item.name == "AttackDistance")
                {
                    item.GetComponentInChildren<Slider>().interactable = false;
                }
                else
                {
                    if (item.name == "TBetweenAttacks" || item.name == "DurationAttack")
                    {
                        item.GetComponentInChildren<Slider>().interactable = true;
                    }
                }
            }
            else
            {
                if (item.name == "TBetweenAttacks" || item.name == "DurationAttack")
                {
                    item.GetComponentInChildren<Slider>().interactable = false;
                }
                else
                {
                    if (item.name == "NBullets" || item.name == "BulletSpeed" || item.name == "NumberOfWaves" || item.name == "SecBtwWaves" ||
                    item.name == "SecBtwShoots" || item.name == "AngleToShoot" || item.name == "AttackDistance")
                    {
                        item.GetComponentInChildren<Slider>().interactable = true;
                    }
                }
            }
        }
    }
}
