using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class UiController : MonoBehaviour
{
    public Dictionary<string, Dictionary<string, MonstersInfo>> monstersInfo;
    public Dictionary<string, Dictionary<int, PlaceholderTier>> order;
    public GameObject enemiesPlaceholders;
    public GameObject weapon;
    List<GameObject> enemies = new List<GameObject>();

    public Text popUp;
    public Button saveBtn;

    public Button btnSelected, next, previous;
    public GameObject testRoom;
    int monsterIndex = 0;
    List<string> tiers = new List<string>();

    string monsterSelectedName;
    // Use this for initialization
    void Start()
    {
        enemies = EnemyLibrary.instance.GetAllPossibleEnemiesPrefabs();
        monstersInfo = new Dictionary<string, Dictionary<string, MonstersInfo>>();
        order = new Dictionary<string, Dictionary<int, PlaceholderTier>>();
        InitializeList();
        InitializeDic();
        LoadFile();
        GetMonster();
    }

    private void InitializeList()
    {
        tiers.Add("default");
        tiers.Add("Tier1");
        tiers.Add("Tier2");
    }

    private void InitializeDic()
    {
        foreach (var item in enemies)
        {
            monstersInfo.Add(item.name, new Dictionary<string, MonstersInfo>());
            order.Add(item.name, new Dictionary<int, PlaceholderTier>());
        }
    }

    void GetMonster()
    {
        var enemPlaceholder = enemiesPlaceholders.GetComponentsInChildren<Image>();
        List<string> auxList = new List<string>();
        auxList.AddRange(tiers);
        foreach (var item in enemPlaceholder)
        {
            if (item.name == monsterSelectedName || item.name == "Enemy")
            {
                item.name = enemies[monsterIndex].name;
                item.sprite = enemies[monsterIndex].GetComponent<SpriteRenderer>().sprite;
                item.preserveAspect = true;
                if (!order[item.name].ContainsKey(item.GetComponent<PlaceholderTier>().id))
                {
                    PlaceholderTier save = new PlaceholderTier();
                    //gets random tier for monster
                    var tierName = auxList[UnityEngine.Random.Range(0, auxList.Count)];
                    item.GetComponent<PlaceholderTier>().tierName = tierName;
                    save.tierName = tierName;
                    save.id = item.GetComponent<PlaceholderTier>().id;
                    save.monsterName = item.name;
                    auxList.Remove(tierName);
                    //adds the monster to dic to save order
                    order[item.name].Add(item.GetComponent<PlaceholderTier>().id, save);
                }
                else
                {
                    item.GetComponent<PlaceholderTier>().tierName = order[item.name][item.GetComponent<PlaceholderTier>().id].tierName;
                    if (order[item.name][item.GetComponent<PlaceholderTier>().id].orderNumber != 0)
                    {
                        item.GetComponentInChildren<InputField>().text = order[item.name][item.GetComponent<PlaceholderTier>().id].orderNumber.ToString();
                        item.GetComponent<PlaceholderTier>().orderNumber = order[item.name][item.GetComponent<PlaceholderTier>().id].orderNumber;
                    }
                }
            }
        }
        monsterSelectedName = enemies[monsterIndex].name;
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
                ClearBullets();
                DestroyMonster();
                btnSelected.GetComponentInChildren<Text>().text = "Select";
                ColorBlock colors1 = btnSelected.colors;
                colors1.normalColor = new Color(1f, 1f, 1f);
                btnSelected.colors = colors1;
                btnSelected = btn;

            }
            var enemy = CreateMonster(btn.transform.parent);
            ResetPlayerWeapon();

            GetMonsterCharac(enemy);
        }
        else
        {
            //Unselect the same monster
            if (btnSelected == btn)
            {
                ClearBullets();
                DestroyMonster();
                btnSelected.GetComponentInChildren<Text>().text = "Select";
                ColorBlock colors = btnSelected.colors;
                colors.normalColor = new Color(1f, 1f, 1f);
                btnSelected.colors = colors;
                btnSelected = null;
            }

        }
    }

    void ResetPlayerWeapon()
    {
        GameObject.FindObjectOfType<PlayerShootTest>().DroppedThrowable();
        PlayerHealthSystemTest.instance.EnablePlayer();
        if (GameObject.Find("xmas tree 30 bullets test"))
            Destroy(GameObject.Find("xmas tree 30 bullets test"));
        if (GameObject.Find("thrown candyCane test(Clone)"))
            Destroy(GameObject.Find("thrown candyCane test(Clone)"));
        var w = Instantiate(weapon, GameObject.Find("playerTest").transform.position, Quaternion.identity);
        w.name = weapon.name;
    }

    void GetMonsterCharac(GameObject enemy)
    {
        var mInfo = monstersInfo[enemy.name][btnSelected.transform.parent.GetComponent<PlaceholderTier>().tierName];
        var enemyCharac = enemy.GetComponentInChildren<BulletSpawner>();
        enemyCharac.numberOfBullets = (int)mInfo.numberBullets;
        enemyCharac.bulletSpeed = (int)mInfo.bulletSpeed;
        enemyCharac.numberOfWaves = (int)mInfo.numberOfWaves;
        enemyCharac.secondsBetweenWaves = (int)mInfo.secBtwWaves;
        enemyCharac.secondsBetweenShots = mInfo.secBtwShots;
        enemyCharac.angleToShootInDegrees = mInfo.angleToShoot;
        enemy.GetComponent<Thesis.Enemy.EnemyControllerTest>().attackDistance = mInfo.attackDistance;
        enemy.GetComponent<Thesis.Enemy.EnemyMovementTest>().stoppingDistanceToPlayer = mInfo.stoppingDistance;
        enemy.GetComponent<Thesis.Enemy.EnemyMovementTest>().movementSpeed = mInfo.movementSpeed;
        enemy.GetComponentInChildren<Thesis.Enemy.EnemyShootTest>().timeToWaitBeforeShootingAgain = mInfo.attackSpeed;
    }

    void UnselectMonster()
    {
        if (btnSelected != null)
        {
            DestroyMonster();
            btnSelected.GetComponentInChildren<Text>().text = "Select";
            ColorBlock colors = btnSelected.colors;
            colors.normalColor = new Color(1f, 1f, 1f);
            btnSelected.colors = colors;
            btnSelected = null;
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
                obj.GetComponent<Thesis.Enemy.EnemyHealthSystemTest>().OnEnemyDie += UnselectMonster;
                obj.name = item.name;
                return obj;
            }
        }
        return null;
    }

    public void DestroyMonster()
    {
        if (testRoom.GetComponentInChildren<RoomManager>().GetEnemyHolder().transform.childCount > 0)
            Destroy(testRoom.GetComponentInChildren<RoomManager>().GetEnemyHolder().GetComponentInChildren<SpriteRenderer>().gameObject);
    }

    public void ChangeMonster(Button btn)
    {
        if (btn.name == "Next")
        {
            next.gameObject.SetActive(false);
            monsterIndex++;
            UnselectMonster();
            ClearInputField();
            GetMonster();
        }
        if (btn.name == "Previous")
        {
            monsterIndex--;
            UnselectMonster();
            ClearInputField();
            GetMonster();
        }
        if (monsterIndex > 0)
        {
            previous.gameObject.SetActive(true);
        }
        if (monsterIndex == 0)
        {
            previous.gameObject.SetActive(false);
        }


        if (monsterIndex == (enemies.Count - 1))
        {
            next.gameObject.SetActive(false);
        }
        if (monsterIndex < (enemies.Count - 1))
        {
            //next.gameObject.SetActive(true);
        }
    }

    void LoadFile()
    {
        string path = Application.dataPath + "/Resources/denis.json";
        string[] fileContent = File.ReadAllLines(path);
        foreach (var item in monstersInfo)
        {
            foreach (var str in fileContent)
            {
                MonstersInfo obj = JsonUtility.FromJson<MonstersInfo>(str);
                if (obj.monsterName == item.Key)
                {
                    item.Value.Add(obj.tier, obj);
                }
            }
        }
        // foreach (var item in monstersInfo)
        // {
        // 	Debug.Log(item.Key);
        //     foreach (var str in item.Value)
        //     {
        // 		Debug.Log(str.Key);
        //     }
        // }
    }

    public void SaveMonsterOrder(InputField input)
    {
        var enemPlaceholder = enemiesPlaceholders.GetComponentsInChildren<InputField>();
        int aux = 0;
        foreach (var item in enemPlaceholder)
        {
            if (item.text != "")
            {
                aux++;
            }
        }
        if (aux == 3)
            EnableArrowForNextMonster();
        if (input.text != "")
        {
            order[input.transform.parent.name][input.GetComponentInParent<PlaceholderTier>().id].orderNumber = Int32.Parse(input.text);
            if (Int32.Parse(input.text) > 3 || Int32.Parse(input.text) < 1)
            {
                input.text = "";
                StartCoroutine(EnablePopUp(input));
            }
        }
        CheckIfReadyToSave();
    }

    void CheckIfReadyToSave()
    {
        int count = 0;
        foreach (var item in order)
        {
            foreach (var enemy in item.Value)
            {
                if (enemy.Value.orderNumber != 0)
                {
                    count++;
                }
            }
        }
        if (count == (enemies.Count * 3))
        {
            saveBtn.gameObject.SetActive(true);
        }
    }

    void EnableArrowForNextMonster()
    {
        var enemPlaceholder = enemiesPlaceholders.GetComponentsInChildren<InputField>();
        List<string> aux = new List<string>();
        foreach (var item in enemPlaceholder)
        {
            if (aux.Contains(item.text))
            {
                var obj = Instantiate(popUp, enemPlaceholder[1].transform.position - new Vector3(0, 65, 0), Quaternion.identity, gameObject.transform);
                obj.fontSize = 40;
                obj.text = "Please order the monsters from 1 to 3.";
                obj.verticalOverflow = VerticalWrapMode.Overflow;
                obj.horizontalOverflow = HorizontalWrapMode.Overflow;
                obj.gameObject.SetActive(true);
                StartCoroutine(DisablePopUp(obj));
                break;
            }
            aux.Add(item.text);
        }
        if (aux.Count == 3)
        {
            GameObject.Find("ProgressBar").GetComponent<ProgressBar>().IncreaseProgress(3 / ((float)enemies.Count * 3));
        }
        if (aux.Count == 3 && monsterIndex < (enemies.Count - 1))
        {
            next.gameObject.SetActive(true);
        }
    }
    IEnumerator DisablePopUp(Text t)
    {
        yield return new WaitForSecondsRealtime(2);
        Destroy(t.gameObject);
    }

    IEnumerator EnablePopUp(InputField input)
    {
        popUp.gameObject.SetActive(true);
        popUp.transform.position = input.transform.position - new Vector3(0, 45, 0);
        popUp.fontSize = 20;
        popUp.text = "Insert a number between 1 - 3.";
        yield return new WaitForSecondsRealtime(1);
        popUp.gameObject.SetActive(false);
    }

    void ClearInputField()
    {
        var enemPlaceholder = enemiesPlaceholders.GetComponentsInChildren<InputField>();
        foreach (var item in enemPlaceholder)
        {
            item.text = "";
        }
    }

    void ClearBullets()
    {
        var bullets = GameObject.FindObjectsOfType<RegularBullet>();
        foreach (var item in bullets)
        {
            Destroy(item.gameObject);
        }
    }
}
