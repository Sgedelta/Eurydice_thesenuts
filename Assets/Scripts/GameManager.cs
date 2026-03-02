using System.Collections;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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

    //Used to temporarily "hold" an item to move between slots
    public Item selectedItem;
    public int selectedItemIndex;

    //Inventory slots
    //TODO: may adjust this setup later
    [SerializeField] private Button[] inventorySlots = new Button[4];

    private void Awake()
    {
        //standard singleton
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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

    public Enemy CurrentEnemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Set up listeners on buttons
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if(inventorySlots[i] == null)
            {
                Debug.LogError($"Inventory Slot {i} Does not exist!");
                continue;
            }
            //Seems like passing in i directly is causing issues
            int index = i;
            inventorySlots[i].onClick.AddListener(() => MoveItem(index));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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

        //No selected item, store the one in the corresponding array slot
        if (selectedItem == null)
        {
            //TODO: highlight button
            selectedItem = RetrieveItem(i);
            selectedItemIndex = i;
            Debug.Log("Selected item: " + selectedItem);
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

        //Reset button highlight

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
            return Orpheus.EquippedItems[i];
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

        //combat running
        while(Orpheus.IsAlive && CurrentEnemy.IsAlive)
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
                    break;

                case 1: //O
                    //get input
                    yield return StartCoroutine(Orpheus.GetCombatChoice());
                    OrpheusDecision Odecision = Orpheus.CombatChoice;

                    //handle input
                    AttackController attack = Instantiate(AttackPrefab).GetComponent<AttackController>();
                    List<int> activeStringIndexes = new List<int>();
                    List<bool> activeStrings = new List<bool>();
                    

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
                                    activeStrings.Add(true);
                                } 
                                else
                                {
                                    activeStrings.Add(false);
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
                                    activeStrings.Add(true);
                                }
                                else
                                {
                                    activeStrings.Add(false);
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
                    yield return null; //chug along
                    break;

            }

            ActiveHealthBar.SetHealthData(Orpheus.MoralePercent, CurrentEnemy.MoralePercent);

            turn = (turn + 1) % 3;
        }

        //handle combat output
        if(Orpheus.IsAlive)
        {
            //enemy died

            //playtest temp code:
            Enemy OldEnemy = CurrentEnemy;
            Instantiate(OldEnemy);
            Destroy(OldEnemy.gameObject);
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

        ActiveHealthBar.HaveEnemyHealth = true;

        StartCoroutine(RunCombat());
    }



}
