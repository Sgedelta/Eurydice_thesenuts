using System;
using System.Collections;
using UnityEngine;

//Note: we could do this with an interface and stuf but... ehhh.....prototype. I don't think we really need it
public enum EurydiceDecision
{
    Heal,
    Laurel,
    Torch
}


//for the definitions of ICanEquip and ICanAttack, see Interfaces.cs
public class EurydiceController : MonoBehaviour, ICanEquip
{
    public Item[] EquippedItems { get; set; } = new Item[2];

    [SerializeField] public Item TEMPITEM;

    public float HealAmount = 10;
    public EurydiceDecision CombatChoice = EurydiceDecision.Heal;
    private bool combatDecisionMade = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //DEBUG--REMOVE LATER
        //EquipItem(0, TEMPITEM);
        Debug.Log("Eurydice Inventory:" + EquippedItems[0] + EquippedItems[1]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public Item EquipItem(int i, Item item)
    {
        Item previousItem = EquippedItems[i];

        //Properly unequip previous item
        if (previousItem != null)
        {
            UnequipItem(previousItem);
        }

        EquippedItems[i] = item;

        return previousItem;

    }

    public bool IsEquipped(Item item)
    {
        for (int i = 0; i < EquippedItems.Length; i++)
        {
            if (EquippedItems[i] == item)
            {
                return true;
            }
        }

        return false;
    }

    public bool UnequipItem(Item item)
    {
        //Search array for matching item
        for (int i = 0; i < EquippedItems.Length; i++)
        {
            if (EquippedItems[i] == item)
            {
                EquippedItems[i] = null;
                return true;
            }
        }

        //No item to unequip
        return false;
    }


    public IEnumerator GetCombatChoice()
    {
        //CHUD RETURN. This should follow this process:
        // - Show UI (disabling Laurel/Torch as needed)
        // - ON UI CLICK: update CombatChoice, THEN update combatDecisionMade (via another method)
        // - Hide UI


        combatDecisionMade = true; //TODO: REMOVE ONCE THE LOGIC FOR UI IS IN

        while(!combatDecisionMade)
        {
            yield return null;
        }
        combatDecisionMade = false; //for next time

    }

    public void ChooseHeal()
    {
        CombatChoice = EurydiceDecision.Heal;
        combatDecisionMade = true;
    }

    public void ChooseLaurel()
    {
        CombatChoice = EurydiceDecision.Laurel;
        combatDecisionMade = true;
    }
    public void ChooseTorch()
    {
        CombatChoice = EurydiceDecision.Torch;
        combatDecisionMade = true;
    }


}
