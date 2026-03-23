using System.Collections;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public enum GameState
{
    Menus,
    CombatRoom,
    ItemRoom,
    RestRoom
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private GameState state = GameState.Menus;
    public GameState State { get { return state; } }

    //Attack Data
    [SerializeField] private GameObject AttackPrefab;
    [SerializeField] private GameObject FogAttackPrefab;
    private int LightAttackStringPositions = 7;
    private int LightAttackStringsActive = 3;
    private int HeavyAttackStringPositions = 12;
    private int HeavyAttackStringsActive = 6;
    [SerializeField] private float missThresh = .5f; //if attack total is less than this, it counts as a full miss
    [SerializeField] private float fullHitThresh = .8f; //if attack total is greater than this, it counts as a full hit

    public HealthBar ActiveHealthBar;

    //References to UIs to enable/disable
    //TODO: all placeholder UIs currently, this may be removed/adjusted
    [SerializeField] private GameObject EquipUI;
    [SerializeField] private GameObject GeneralUI;

    //Description management for inventory ui
    private Transform descObject;
    private TextMeshProUGUI descText;

    //Status popup for general ui
    private Transform recieveObject;
    private TextMeshProUGUI recieveText;

    //Used to temporarily "hold" an item to move between slots
    public Item selectedItem;
    public int selectedItemIndex;

    //Inventory slots
    //TODO: may adjust this setup later
    [SerializeField] private Button[] inventorySlots = new Button[4];

    // Delay Time between player input choices
    [Header("Delay Time")]
    [SerializeField] private float _delayTimeBetweenChoices = 1f;

    //Use bombs?
    private bool useBombs = false;


    //Persistent Data setup: an array of room datas to hold what is at what position
    private RoomData[] roomDatas;
    public int MapWidth;

    private Vector2Int lastVisitedRoom = new Vector2Int(-1, -1);
    private int lastVisitedRoomIndex = -1;
    public Vector2Int LastVisitedRoom { get { return lastVisitedRoom; } set
        {
            lastVisitedRoom = value;
            lastVisitedRoomIndex = value.x * value.y + value.x;
        }
    }

    public RoomManager LastVisitedRoomManager;

    // Data-Tracking
    public int TurnsPerCombat { get; set; } = 0;

    private void Awake()
    {
        //standard singleton
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SetupMapDimensions(3, 3); //TEMP
            SetupInvButtons();
        }
        //awake should never be called twice but. just in case!
        else if (instance != this)
        {
            Debug.LogWarning($"Destroying {name} due to the static instance of GameManager already being on {GameManager.instance.name}");
            //pass on our data on (the old gm will need it as it's loading into this scene)
            GameManager.instance.GeneralUI = GeneralUI;
            GameManager.instance.EquipUI = EquipUI;
            GameManager.instance.inventorySlots = inventorySlots;

            GameManager.instance.SetupInvButtons();

            Destroy(this.gameObject);
        }

        

    }


    //this will be how Orpheus and Eurydice talk
    public OrpheusController Orpheus;
    public EurydiceController Eurydice;

    public Enemy CurrentEnemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("started");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupMapDimensions(int xSize, int ySize)
    {
        if(roomDatas != null)
        {
            roomDatas = null; //I'm 99% sure I don't have to dispose of the array and GC will clean this...
        }

        roomDatas = new RoomData[xSize * ySize];
    }

    public RoomData GetRoomDataAtLoc(int x, int y)
    {
        if((y * x) + x >= roomDatas.Length)
        {
            throw new ArgumentOutOfRangeException($"Position {x}, {y} is out of range of Room Datas. must be within room datas of bounds {MapWidth}, {roomDatas.Length / MapWidth}");
        }

        return roomDatas[x * y + x];
    }

    public void SetRoomDataAtLoc(int x, int y, RoomData data)
    {
        if ((y * x) + x >= roomDatas.Length)
        {
            throw new ArgumentOutOfRangeException($"Position {x}, {y} is out of range of Room Datas. must be within room datas of bounds {MapWidth}, {roomDatas.Length / MapWidth}");
        }

        roomDatas[x * y + x] = data;
    }

    //Toggles inventory UI
    public void ToggleInventory()
    {
        EquipUI.SetActive(!EquipUI.activeSelf);
        GeneralUI.SetActive(!GeneralUI.activeSelf);
    }


    //Called when an inventory button is pressed
    //Either updates the selected item if null, or moves the selected item to the new spot
    //i refers to the slot of the array to check
    //First pass of this code is gonna be a mess im so sorry, inventories suck
    //There is absolutely a better way to indicate spots I'm just tired and can't think of it rn
    public void MoveItem(int i)
    {
        //Gets the description panel
        descObject = EquipUI.transform.Find("ItemDescription");
        descText = descObject.GetComponent<TextMeshProUGUI>();

        Debug.Log("moving");
        //No selected item, store the one in the corresponding array slot
        if (selectedItem == null)
        {
            //TODO: highlight button
            selectedItem = RetrieveItem(i);
            selectedItemIndex = i;
            Debug.Log("Selected item: " + selectedItem);

            //Adds the current item description if there's a selected item
            if (selectedItem != null)
            {
                descObject.gameObject.SetActive(true);
                descText.text = $"Selected Item: \n {selectedItem.name} \n {selectedItem.description}";
            }
            

            return;
        }

        //Moving spots

        //Equips selected item, get previous item in slot if applicable
        Item previousItem = EquipItem(selectedItem, i);

        UpdateLabel(i, selectedItem.name);


        //If there's an item passed back, set it to the first item's spot
        if (previousItem != null)
        {
            //Don't really need the return here
            EquipItem(previousItem, selectedItemIndex);
            UpdateLabel(selectedItemIndex, previousItem.name);
        }
        //Empty slot
        else
        {
            UpdateLabel(selectedItemIndex);
        }

        Debug.Log("Eurydice Item 1: " + Eurydice.EquippedItems[0] + "Item 2: " + Eurydice.EquippedItems[1]);
        Debug.Log("Orpheus Item 1: " + Orpheus.EquippedItems[0] + "Item 2: " + Orpheus.EquippedItems[1]);

        //Null out selected item and index
        //tbh index doesn't *need* to be wiped but just in case
        selectedItem = null;
        selectedItemIndex = -1;

        //Clearing out and disabling item description
        descObject.gameObject.SetActive(false);
        descText.text = "";

        //Reset button highlight

    }

    public void SetupInvButtons()
    {
        //Set up listeners on buttons
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] == null)
            {
                Debug.LogError($"Inventory Slot {i} Does not exist!");
                continue;
            }
            //Seems like passing in i directly is causing issues
            int index = i;
            inventorySlots[i].onClick.AddListener(() => MoveItem(index));
        }
    }


    //Helper for MoveItem, gets an item from an array based on button index
    //TODO: update this to remove some number hardcoding
    private Item RetrieveItem(int i)
    {
        //If index is >1, Eurydice array
        if (i > 1)
        {
            return Eurydice.EquippedItems[i - 2];
        }

        //Orpheus array
        else
        {
            Debug.Log(GameManager.instance.Orpheus);
            return GameManager.instance.Orpheus.EquippedItems[i];
        }
    }

    //Helper for MoveItem, runs equipItem on determined spot
    //TODO: update this to remove some number hardcoding
    private Item EquipItem(Item item, int i)
    {
        Item previousItem = null;

        //Removes item from any slots beforehand
        //TODO: update unequip to take an index?
        Eurydice.UnequipItem(item);
        Orpheus.UnequipItem(item);

        //If index is >1, Eurydice array
        if (i > 1)
        {
            previousItem = Eurydice.EquipItem(i - 2, item);
        }

        //Orpheus array
        else
        {
            previousItem = Orpheus.EquipItem(i, item);
        }

        
        return previousItem;
    }

    //Equips item in first empty slot
    public void AutoEquip(GameObject item)
    {

        //Merging the two arrays for checking purposes
        //Didn't think to do this with earlier stuff, will go back and clean up later

        Item[] fullInventory = Orpheus.EquippedItems.Concat(Eurydice.EquippedItems).ToArray();

        //Loops through the inventory slots to find the first empty one
        for (int i = 0; i < fullInventory.Length; i++)
        {
            Debug.Log(fullInventory[i]);
            //Empty spot found
            if (fullInventory[i] == null)
            {
                Debug.Log(i);
                //Place item in the corresponding inventory, need to access og arrays
                if (i <= 1)
                {
                    Orpheus.EquipItem(i, item.GetComponent<Item>());
                }
                else
                {
                    Eurydice.EquipItem(i - 2, item.GetComponent<Item>());
                }

                //Makes sure general ui is on
                if (!GeneralUI.activeSelf)
                {
                    ToggleInventory();
                }

                //Enables popup
                recieveObject = GeneralUI.transform.Find("ItemRecieve");
          
                recieveText = recieveObject.GetComponent<TextMeshProUGUI>();
                recieveObject.gameObject.SetActive(true);

                string itemName = item.GetComponent<Item>().name;
                UpdateLabel(i, itemName);
                recieveText.text = $"You recieved a {itemName}";

                //End the loop here
                break;
            }
        }
    }

    //Updates label for the inventory slot
    //TODO: I assume this'll probably get replaced with sprites down the line?
    private void UpdateLabel(int i, string name = "Empty")
    {
        inventorySlots[i].GetComponentInChildren<TextMeshProUGUI>().text = name;
    }


    // ============ GAME FLOW ============

    public IEnumerator RunCombat()
    {
        //combat setup
        int turn = 0; //whose turn is it? 0 - Eurydice, 1 - Orpheus, 2 - Enemy

        Debug.Log($"Orpheus is: {Orpheus.name}");
        Debug.Log($"Enemy is: {CurrentEnemy.name} | {CurrentEnemy.MoralePercent}");
        

        int waitFramesForHealthbar = 100;
        while (ActiveHealthBar == null && waitFramesForHealthbar > 0)
        {
            waitFramesForHealthbar--;
            if(waitFramesForHealthbar == 0)
            {
                Debug.LogError("Healthbar was not initialized within 100 frames! Cancelling!");
            }
            yield return null;
        }

        ActiveHealthBar.HaveEnemyHealth = true;
        ActiveHealthBar.SetHealthData(Orpheus.MoralePercent, Orpheus.MoraleDisplayPercent, CurrentEnemy.MoralePercent);

        //combat running
        while (Orpheus.IsAlive && CurrentEnemy.IsAlive)
        {

            GameManager.print($"Combat Data turn {turn}: Orpheus is at {Orpheus.Morale} and the enemy is at {CurrentEnemy.Morale}");
            switch(turn)
            {
                case 0: //E
                    //get input
                    yield return StartCoroutine(Eurydice.GetCombatChoice());
                    EurydiceDecision Edecision = Eurydice.CombatChoice;

                    //handle input
                    switch (Edecision)
                    {
                        case EurydiceDecision.Heal:
                            Orpheus.ChangeMorale(Eurydice.HealAmount);
                            break;

                        case EurydiceDecision.Laurel:
                            //find the item, use it
                            for(int i = 0; i < Eurydice.EquippedItems.Length; i++)
                            {
                                if (Eurydice.EquippedItems[i] is LaurelItem)
                                {
                                    Eurydice.EquippedItems[i].EurydiceAbility(Orpheus);
                                }
                            }
                            break;

                        case EurydiceDecision.Torch:
                            //SAME LOGIC as laurel, but torch. this is not DRY but idgaf
                            for (int i = 0; i < Eurydice.EquippedItems.Length; i++)
                            {
                                if (Eurydice.EquippedItems[i] is TorchItem)
                                {
                                    Eurydice.EquippedItems[i].EurydiceAbility(Orpheus);
                                }
                            }
                            break;
                    }

                    // Apply a delay time between Eurydice input choice and Orpheus input choice
                    yield return new WaitForSeconds(_delayTimeBetweenChoices);
                    break;

                case 1: //O
                    //get input
                    yield return StartCoroutine(Orpheus.GetCombatChoice());
                    OrpheusDecision Odecision = Orpheus.CombatChoice;

                    //handle input
                    GameObject attackPrefab;
                    switch(LastVisitedRoomManager.RoomData.Modifier)
                    {
                        case RoomModifier.Fog:
                            attackPrefab = FogAttackPrefab;
                            useBombs = false;
                            break;

                        case RoomModifier.Bomb:
                            attackPrefab = AttackPrefab;
                            useBombs = true;
                            break;

                        default:
                            attackPrefab = AttackPrefab;
                            useBombs = false;
                            break;
                    }
                    AttackController attack = Instantiate(attackPrefab).GetComponent<AttackController>();
                    List<int> activeStringIndexes = new List<int>();
                    List<TargetType> activeStrings = new List<TargetType>();
                    
                    //TODO: putting a todo here so i can ctrl f here. makin bombs
                    switch(Odecision)
                    {
                        case OrpheusDecision.LightAttack:
                            //get random numbers (unique)
                            for(int i = 0; i < LightAttackStringsActive; i++)
                            {
                                int index = UnityEngine.Random.Range(0, LightAttackStringPositions);
                                while(activeStringIndexes.Contains(index))
                                {
                                    index = UnityEngine.Random.Range(0, LightAttackStringPositions);
                                }
                                activeStringIndexes.Add(index);
                            }
                            //build bool array
                            for(int i = 0; i < LightAttackStringPositions; i++)
                            {
                                if (activeStringIndexes.Contains(i))
                                {
                                    
                                    //Get random number, if < 3, BOMB
                                    int rng = UnityEngine.Random.Range(0, 10);
                                    if (rng < 1 && useBombs)
                                    {
                                        activeStrings.Add(TargetType.Bomb);
                                    }
                                    else
                                    {
                                        activeStrings.Add(TargetType.Normal);
                                    }
                                } 
                                else
                                {
                                    activeStrings.Add(TargetType.None);
                                }
                                
                            }
                            //setup orpheus data for this attack
                            Orpheus.AttackDamage = 10;

                            //attack.
                            attack.SetupTargets(LightAttackStringPositions, activeStrings);
                            break;

                        case OrpheusDecision.HeavyAttack:
                            //get random numbers (unique)
                            for (int i = 0; i < HeavyAttackStringsActive; i++)
                            {
                                int index = UnityEngine.Random.Range(0, HeavyAttackStringPositions);
                                while (activeStringIndexes.Contains(index))
                                {
                                    index = UnityEngine.Random.Range(0, HeavyAttackStringPositions);
                                }
                                activeStringIndexes.Add(index);
                            }
                            //build bool array
                            for (int i = 0; i < HeavyAttackStringPositions; i++)
                            {
                                if (activeStringIndexes.Contains(i))
                                {
                                    //Get random number, if < 3, BOMB
                                    int rng = UnityEngine.Random.Range(0, 10);
                                    if (rng < 3 && useBombs)
                                    {
                                        activeStrings.Add(TargetType.Bomb);
                                    }
                                    else
                                    {
                                        activeStrings.Add(TargetType.Normal);
                                    }
                                }
                                else
                                {
                                    activeStrings.Add(TargetType.None);
                                }

                            }
                            //setup orpheus data for this attack
                            Orpheus.AttackDamage = 20;

                            
                            //attack
                            attack.SetupTargets(HeavyAttackStringPositions, activeStrings);
                            break;
                    } //end attack setup switch

                    //wait for attack to be done
                    yield return StartCoroutine(attack.MoveAttackIndicator());


                    //handle attack data
                    if(attack.PercentHit < missThresh)
                    {
                        //do nothing lol. attack for 0. failure!!
                    }
                    else
                    {
                        float attackEffectiveness = Mathf.Lerp(0, 1, (attack.PercentHit - missThresh) / (fullHitThresh - missThresh));
                        Orpheus.Attack(attackEffectiveness, CurrentEnemy);
                    }

                    //Hit bombs?
                    Debug.Log(attack.PercentHitBomb);
                    if(attack.PercentHitBomb == 0)
                    {
                        //bombs dodged, do nothing
                    }
                    else
                    {                      
                        //Percent amount of 20 for dmg calcs
                        float dmgTaken = 20 * attack.PercentHitBomb;

                        //Take damage
                        Orpheus.ChangeMorale(-dmgTaken);
                    }

                    //get rid of the attack display now
                    Destroy(attack.gameObject);

                    //deactivate any Eurydice effects!
                    foreach(Item i in Eurydice.EquippedItems)
                    {
                        if(i != null)
                        {
                            i.EurydiceEndAbility(Orpheus);
                        }
                    }
                    break;

                case 2: //Enemy
                    //ouch.
                    Orpheus.ChangeMorale(-CurrentEnemy.MoraleDamagePerTurn);
                    // At the end of each turn (enemy attacks), increment TurnsPerCombat for data-tracking
                    TurnsPerCombat++; 
                    yield return null; //chug along
                    break;

            }

            ActiveHealthBar.SetHealthData(Orpheus.MoralePercent, Orpheus.MoraleDisplayPercent, CurrentEnemy.MoralePercent);

            turn = (turn + 1) % 3;
        }

        //handle combat output
        if(Orpheus.IsAlive)
        {
            //enemy died

            //Add exp, can modify this if we want varying amounts
            //Handles level up too
            Orpheus.AddXP(2);

            //Updates scaling to account for level up
            ActiveHealthBar.ScaleHealthBG(Orpheus.MoraleDisplayPercent);

            //playtest temp code:
            Destroy(CurrentEnemy.gameObject);
            LastVisitedRoomManager.SetCompleted(true);
        } else
        {
            //Orpheus died:
            //game over!
            //RIP in Fart for playtest for now
        }




            yield return null;
    }

    public void StartCombat(Enemy e)
    {
        CurrentEnemy = e;

        StartCoroutine(RunCombat());
    }

    // Data-Tracking Helper Method
    public void SaveGame()
    {
        SaveData data = new SaveData();

        data.turnsPerCombat = TurnsPerCombat;

        string json = JsonUtility.ToJson(data, true);

        string path = Path.Combine(Application.persistentDataPath, "savefile.json");

        File.WriteAllText(path, json);
        Debug.Log("Saved to: " + path);
    }
}
