using System.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

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

    // Delay Time between player input choices
    [Header("Delay Time")]
    [SerializeField] private float _delayTimeBetweenChoices = 1f;

    //Use bombs?
    private bool useBombs = false;


    //Persistent Data setup: an array of room datas to hold what is at what position
    private RoomData[] roomDatas;
    public int MapWidth;
    public int MapIndex = 0;

    private Vector2Int lastVisitedRoom = new Vector2Int(-1, -1);
    private int lastVisitedRoomIndex = -1;
    public Vector2Int LastVisitedRoom { get { return lastVisitedRoom; } set
        {
            lastVisitedRoom = value;
            lastVisitedRoomIndex = value.x * value.y + value.x;
        }
    }

    public RoomManager LastVisitedRoomManager;

    //new data tracking
    public Dictionary<RoomModifier, Tuple<int, float>> DataTracker { get; set; } = new Dictionary<RoomModifier, Tuple<int, float>>(); 


    private void Awake()
    {
        //standard singleton
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SetupMapDimensions(3, 3); //TEMP
            
            if(!TryLoadSavedData()) 
            {
                //load some chud data so "analysis" works
                DataTracker.Add(RoomModifier.Fog, new Tuple<int, float>(0,0));
                DataTracker.Add(RoomModifier.Bomb, new Tuple<int, float>(0, 0));
                DataTracker.Add(RoomModifier.NONE, new Tuple<int, float>(0, 0));
            }

        }
        //awake should never be called twice but. just in case!
        else if (instance != this)
        {
            Debug.LogWarning($"Destroying {name} due to the static instance of GameManager already being on {GameManager.instance.name}");
            

            Destroy(this.gameObject);
        }
    }


    //this will be how Orpheus and Eurydice talk
    public OrpheusController Orpheus;
    public EurydiceController Eurydice;

    public HighlightableSprite ODisplay;
    public HighlightableSprite EDisplay;

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

        MapWidth = xSize;

        roomDatas = new RoomData[xSize * ySize];
    }

    public RoomData GetRoomDataAtLoc(int x, int y)
    {
        PrintAllRoomDatas();
        if((y * MapWidth) + x >= roomDatas.Length)
        {
            throw new ArgumentOutOfRangeException($"Position {x}, {y} is out of range of Room Datas. must be within room datas of bounds {MapWidth}, {roomDatas.Length / MapWidth}");
        }

        return roomDatas[(MapWidth * y) + x];
    }

    public void SetRoomDataAtLoc(int x, int y, RoomData data)
    {
        
        if ((y * MapWidth) + x >= roomDatas.Length)
        {
            throw new ArgumentOutOfRangeException($"Position {x}, {y} is out of range of Room Datas. must be within room datas of bounds {MapWidth}, {roomDatas.Length / MapWidth}");
        }

        roomDatas[(MapWidth * y) + x] = data;
    }

    public void PrintAllRoomDatas()
    {
        string data = $"ALL ROOM DATA: size {roomDatas.Length}\n";
        for (int i = 0; i < roomDatas.Length; i++)
        {
            if(roomDatas[i] != null)
            {
                data += roomDatas[i].ToString() + "\n";
            }
            else
            {
                data += $"room {i} is null! \n";
            }
            
        }
        Debug.Log(data);
    }

    // ============ GAME FLOW ============

    public IEnumerator RunCombat()
    {
        //combat setup
        int turn = 0; //whose turn is it? 0 - Eurydice, 1 - Orpheus, 2 - Enemy
        int totalRounds = 0;

        Debug.Log($"Orpheus is: {Orpheus.name}");
        Debug.Log($"Enemy is: {CurrentEnemy.name} | {CurrentEnemy.MoralePercent}");
        

        int waitFrames = 100;
        while (ActiveHealthBar == null && waitFrames > 0)
        {
            waitFrames--;
            if(waitFrames == 0)
            {
                Debug.LogError("Healthbar was not initialized within 100 frames! Cancelling!");
            }
            yield return null;
        }

        waitFrames = 100;

        while ((Orpheus.OrpheusCombatPanel == null || Eurydice.EurydiceCombatPanel == null) && waitFrames > 0)
        {
            //same as above but for combat panels to ensure they get loaded....
            waitFrames--;
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
                    totalRounds += 1;

                    //display
                    if(EDisplay != null)
                    {
                        EDisplay.SetHighlight(true);
                    }

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

                    //display
                    if (EDisplay != null)
                    {
                        EDisplay.SetHighlight(false);
                    }
                    break;

                case 1: //O
                    //display
                    if (ODisplay != null)
                    {
                        ODisplay.SetHighlight(true);
                    }

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
                        float lightAttackForgiveness = Odecision == OrpheusDecision.LightAttack ? .3f : 0f;
                        float attackEffectiveness = Mathf.Lerp(0, 1, (attack.PercentHit - missThresh + lightAttackForgiveness) / (fullHitThresh - missThresh));
                        Orpheus.Attack(attackEffectiveness, CurrentEnemy, Odecision);
                        if(Odecision == OrpheusDecision.LightAttack)
                        {
                            CurrentEnemy.DoTTriggers.Add(new DoTTrigger(attackEffectiveness * Orpheus.AttackDamage * .5f, 3));
                        }
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

                    //display
                    if (ODisplay != null)
                    {
                        ODisplay.SetHighlight(false);
                    }

                    break;

                case 2: //Enemy
                    //do DoT 
                    CurrentEnemy.TriggerAllDoTTriggers();
                    //ouch.
                    Orpheus.ChangeMorale(-UnityEngine.Random.Range(CurrentEnemy.MoraleDamagePerTurn.x, CurrentEnemy.MoraleDamagePerTurn.y));
                    // At the end of each turn (enemy attacks), increment TurnsPerCombat for data-tracking
                    
                    yield return null; //chug along
                    break;

            }

            ActiveHealthBar.SetHealthData(Orpheus.MoralePercent, Orpheus.MoraleDisplayPercent, CurrentEnemy.MoralePercent);

            turn = (turn + 1) % 3;
        }

        //handle saving data
        if(DataTracker.TryGetValue(LastVisitedRoomManager.RoomData.Modifier, out Tuple<int, float> data))
        {
            //make a new data and get the "unaveraged" (/scaled up) turn count, plus our new count, re-averaged. slight data loss here due to floating point, womp womp
            DataTracker[LastVisitedRoomManager.RoomData.Modifier] = new Tuple<int, float>(data.Item1 + 1, ((data.Item2 * data.Item1) + totalRounds) / ((float)data.Item1+1)); 
        }
        else
        {
            DataTracker.Add(LastVisitedRoomManager.RoomData.Modifier, new Tuple<int, float>(1, totalRounds));
        }

        SaveData();


        //handle combat output
        if (Orpheus.IsAlive)
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
            ActiveHealthBar.gameObject.SetActive(false); //hide healthbar - there's an issue with doors going on.
        }
        else
        {
            //Orpheus died:
            //game over!
            //RIP in Fart for playtest for now
            SceneManager.LoadScene("GameOver");
        }




            yield return null;
    }

    public void StartCombat(Enemy e)
    {
        CurrentEnemy = e;

        StartCoroutine(RunCombat());
    }

    public bool TryLoadSavedData()
    {
        string path = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/') + 1) + "savefile.json";
        
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path); 
            data = data.Substring(0, data.IndexOf('=')); //trim off process data
            SaveData parsedDat = (SaveData)JsonUtility.FromJson(data, typeof(SaveData));
            
            for(int i = 0; i < 3; i++)
            {
                //evil and disgusting and... functional. 
                RoomModifier modType = RoomModifier.NONE;
                switch(i)
                {
                    case 1:
                        modType = RoomModifier.Fog; break;
                    case 2:
                        modType= RoomModifier.Bomb; break;
                }
                DataTracker.Add(modType, new Tuple<int, float>(parsedDat.NumCombats[i], parsedDat.NumAvgRounds[i]));
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    // Data-Tracking Helper Method - overwrites file with new data
    public void SaveData()
    {
        SaveData data = new SaveData(DataTracker);

        string json = JsonUtility.ToJson(data, true);
        
        string path = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')+1) + "savefile.json";

        string outcome = "===ANALYSIS===\n";
        outcome += $"Normal Combat Average Turn Completion: {DataTracker[RoomModifier.NONE]}\nFog Combat Average Turn Completion: {DataTracker[RoomModifier.Fog]}\nBomb Combat Average Turn Completion: {DataTracker[RoomModifier.Bomb]}\n";
        if (DataTracker[RoomModifier.NONE].Item2 > DataTracker[RoomModifier.Fog].Item2 && DataTracker[RoomModifier.NONE].Item2 > DataTracker[RoomModifier.Bomb].Item2)
        {
            outcome += "Normal Combats have a higher average than both Fog and Bomb rooms, meaning that difficulty should be adjusted!";
        }
        else {
            if (DataTracker[RoomModifier.Bomb].Item2 >= DataTracker[RoomModifier.NONE].Item2)
            {
                outcome += $"Bomb combats are harder than normal combats by {DataTracker[RoomModifier.Bomb].Item2 - DataTracker[RoomModifier.NONE].Item2} turns on average\n";
            }
            if (DataTracker[RoomModifier.Fog].Item2 >= DataTracker[RoomModifier.NONE].Item2)
            {
                outcome += $"Fog combats are harder than normal combats by {DataTracker[RoomModifier.Fog].Item2 - DataTracker[RoomModifier.NONE].Item2} turns on average\n";
            }


        }

        File.WriteAllText(path, json + "\n" + outcome);
        Debug.Log("Saved to: " + path);
    }
}
