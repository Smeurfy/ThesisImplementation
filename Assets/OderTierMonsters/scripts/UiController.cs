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

    public Dictionary<string, List<string>> logs;
    public GameObject enemiesPlaceholders;
    public GameObject weapon;
    public GameObject player;
    List<GameObject> enemies = new List<GameObject>();

    public Text popUp;
    public GameObject saveUI;

    public Button btnSelected, next, previous;
    public GameObject testRoom;
    int monsterIndex = 0;
    List<string> tiers = new List<string>();
    bool toggleSelected = false;

    DateTime dateTime = new DateTime();

    string monsterSelectedName;
    // Use this for initialization
    void Start()
    {
        enemies = EnemyLibrary.instance.GetAllPossibleEnemiesPrefabs();
        monstersInfo = new Dictionary<string, Dictionary<string, MonstersInfo>>();
        order = new Dictionary<string, Dictionary<int, PlaceholderTier>>();
        logs = new Dictionary<string, List<string>>();
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
            logs.Add(item.name, new List<string>());
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
                    save.mostDifferent = false;
                    auxList.Remove(tierName);
                    //adds the monster to dic to save order
                    order[item.name].Add(item.GetComponent<PlaceholderTier>().id, save);
                }
                else
                {
                    item.GetComponent<PlaceholderTier>().tierName = order[item.name][item.GetComponent<PlaceholderTier>().id].tierName;
                    if (order[item.name][item.GetComponent<PlaceholderTier>().id].orderNumber != 0)
                    {

                        Mute(item.GetComponentInChildren<InputField>().onValueChanged);
                        item.GetComponentInChildren<InputField>().text = order[item.name][item.GetComponent<PlaceholderTier>().id].orderNumber.ToString();
                        item.GetComponent<PlaceholderTier>().orderNumber = order[item.name][item.GetComponent<PlaceholderTier>().id].orderNumber;
                        Unmute(item.GetComponentInChildren<InputField>().onValueChanged);

                        if (order[item.name][item.GetComponent<PlaceholderTier>().id].mostDifferent)
                        {
                            Mute(item.GetComponentInChildren<Toggle>().onValueChanged);
                            item.GetComponent<PlaceholderTier>().mostDifferent = order[item.name][item.GetComponent<PlaceholderTier>().id].mostDifferent;
                            item.GetComponentInChildren<Toggle>().isOn = order[item.name][item.GetComponent<PlaceholderTier>().id].mostDifferent;
                            Unmute(item.GetComponentInChildren<Toggle>().onValueChanged);
                        }
                    }
                }

                //logs
                if (!logs[item.name].Contains(item.name))
                    logs[item.name].Add(item.name);
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
            //start timer
            if (dateTime.Year == 0001)
                dateTime = DateTime.Now;
            //btn becomes selected
            btn.GetComponentInChildren<Text>().text = "Selected";
            ColorBlock colors = btn.colors;
            colors.normalColor = new Color(0f, 0.7f, 0f);
            btn.colors = colors;

            //If other btn is clicked then unselect the selected btn
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
            //logs
            logs[enemy.name].Add("Select " + btn.GetComponentInParent<PlaceholderTier>().id);
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
        if (enemy.name == "iceZombieTest")
        {
            enemy.GetComponent<IceZombieControllerTest>().timeBetweenAttacksInSecs = mInfo.timeBetweenAttacks;
            enemy.GetComponent<IceZombiePhysicalAttack>().DurationOfAttackInSecs = mInfo.durationOfAttacks;
            enemy.GetComponent<IceZombiePhysicalAttack>().attackFlightSpeed = mInfo.attackSpeed;
            enemy.GetComponent<Thesis.Enemy.EnemyMovementTest>().stoppingDistanceToPlayer = mInfo.stoppingDistance;
            enemy.GetComponent<Thesis.Enemy.EnemyMovementTest>().movementSpeed = mInfo.movementSpeed;
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
            enemy.GetComponent<Thesis.Enemy.EnemyMovementTest>().stoppingDistanceToPlayer = mInfo.stoppingDistance;
            enemy.GetComponent<Thesis.Enemy.EnemyMovementTest>().movementSpeed = mInfo.movementSpeed;
            enemy.GetComponentInChildren<Thesis.Enemy.EnemyShootTest>().timeToWaitBeforeShootingAgain = mInfo.attackSpeed;
        }
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
            if (monsterIndex == 10)
            {
                SaveOrNot(true);
            }
            else
            {
                UnselectMonster();
                ClearInputField();
                GetMonster();
            }
            DateTime newDateTime = DateTime.Now;
            if ((newDateTime - dateTime).TotalSeconds < 5000)
                logs[enemies[(monsterIndex - 1)].name].Add("Time: " + (newDateTime - dateTime).TotalSeconds);
            dateTime = new DateTime();
        }
        if (btn.name == "Previous")
        {
            monsterIndex--;
            SaveOrNot(false);
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

        if (monsterIndex == enemies.Count)
        {
            next.gameObject.SetActive(false);
        }
        AssignToggleValue();
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
    }

    void LoadFile()
    {
        string path = Application.dataPath + "/StreamingAssets/denis.json";
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
        else
        {
            next.gameObject.SetActive(false);
        }
        if (input.text != "")
        {
            try
            {
                if (Int32.Parse(input.text) > 3 || Int32.Parse(input.text) < 1)
                {
                    input.text = "";
                    StartCoroutine(EnablePopUp(input));
                }
                else
                {
                    order[input.transform.parent.name][input.GetComponentInParent<PlaceholderTier>().id].orderNumber = Int32.Parse(input.text);
                    logs[input.transform.parent.name].Add("Order assigned: " + Int32.Parse(input.text) + " to monster of tier " + input.GetComponentInParent<PlaceholderTier>().tierName);
                }
            }
            catch (FormatException)
            {
                StartCoroutine(EnablePopUp(input));
            }
        }
    }

    void SaveOrNot(bool aux)
    {
        saveUI.SetActive(aux);
        testRoom.SetActive(!aux);
        player.SetActive(!aux);
        enemiesPlaceholders.SetActive(!aux);
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
                next.gameObject.SetActive(false);
                StartCoroutine(DisablePopUp(obj));
                break;
            }
            aux.Add(item.text);
        }
        if (toggleSelected && aux.Count == 3 && GameObject.Find("ProgressBar").GetComponentInChildren<Slider>().value <= GetValueBasedOnIndex())
        {
            GameObject.Find("ProgressBar").GetComponent<ProgressBar>().IncreaseProgress(3 / ((float)enemies.Count * 3));
        }
        if (toggleSelected && aux.Count == 3 && monsterIndex <= (enemies.Count - 1))
        {
            next.gameObject.SetActive(true);
        }
        else
        {
            next.gameObject.SetActive(false);
        }
    }

    float GetValueBasedOnIndex()
    {
        var enemyName = enemiesPlaceholders.GetComponentInChildren<InputField>().transform.parent.name;
        float enemiesCount = enemies.Count;
        for (int i = 0; i < enemiesCount; i++)
        {
            if (enemyName == enemies[i].name)
            {
                return (i / enemiesCount);
            }
        }
        return 0;

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

        var inputField = enemiesPlaceholders.GetComponentsInChildren<InputField>();
        foreach (var item in inputField)
        {
            Mute(item.GetComponentInChildren<InputField>().onValueChanged);
            item.text = "";
            Unmute(item.GetComponentInChildren<InputField>().onValueChanged);
        }
        var toggles = enemiesPlaceholders.GetComponentsInChildren<Toggle>();
        foreach (var item in toggles)
        {
            Mute(item.GetComponentInChildren<Toggle>().onValueChanged);
            item.isOn = false;
            item.GetComponentInParent<PlaceholderTier>().mostDifferent = false;
            Unmute(item.GetComponentInChildren<Toggle>().onValueChanged);
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

    public void Mute(UnityEngine.Events.UnityEventBase ev)
    {
        int count = ev.GetPersistentEventCount();
        for (int i = 0; i < count; i++)
        {
            ev.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.Off);
        }
    }

    public void Unmute(UnityEngine.Events.UnityEventBase ev)
    {
        int count = ev.GetPersistentEventCount();
        for (int i = 0; i < count; i++)
        {
            ev.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
        }
    }

    public void MostDifferentMonster(Toggle toggle)
    {
        if (!order[toggle.transform.parent.name][toggle.GetComponentInParent<PlaceholderTier>().id].mostDifferent)
        {
            order[toggle.transform.parent.name][toggle.GetComponentInParent<PlaceholderTier>().id].mostDifferent = true;
            // toggle.GetComponentInParent<PlaceholderTier>().mostDifferent = true;
            toggleSelected = true;
        }
        else
        {
            order[toggle.transform.parent.name][toggle.GetComponentInParent<PlaceholderTier>().id].mostDifferent = false;
            // toggle.GetComponentInParent<PlaceholderTier>().mostDifferent = false;
            toggleSelected = false;
        }
        EnableArrowForNextMonster();
    }

    void AssignToggleValue()
    {
        var tg = enemiesPlaceholders.GetComponentsInChildren<Toggle>();
        foreach (var item in tg)
        {
            Debug.Log(item.isOn);
            if (item.isOn)
            {
                toggleSelected = true;
                return;
            }
        }
        toggleSelected = false;
    }
}