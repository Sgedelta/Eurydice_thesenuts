using System;
using System.Collections;
using UnityEngine;


public enum OrpheusDecision
{
    LightAttack,
    HeavyAttack
}

//for the definitions of interfaces, see Interfaces.cs
public class OrpheusController : MonoBehaviour, ICanEquip, ICanAttack, IHasMorale
{
    public Item[] EquippedItems { get; set; } = new Item[2];
    [SerializeField] public Item TEMPITEM;
    public Action<float> OnAttack { get; set; } //to be used by Eurydice effects
    public float AttackSpeed { get; set; } = 1;
    public float AttackMissAllowance { get; set; } = 1;
    public float AttackPerfectAllowance { get; set; } = 1;
    public float AttackDamage { get; set; } = 10;

    public float DamageTaken { get; set; } = 1;
    public float Morale { get; set; } = 100;
    public float MaxMorale { get; set; } = 100;

    public float MoralePercent { get { return Morale/MaxMorale; } }

    public bool IsAlive { get { return Morale > 0; } }


    public OrpheusDecision CombatChoice;
    private bool combatDecisionMade = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //DEBUG--REMOVE LATER
        EquipItem(0, TEMPITEM);
        Debug.Log("Orpheus Inventory:" + EquippedItems[0] + EquippedItems[1]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeMorale(float moraleChange)
    {
        Morale += moraleChange;
        Morale = Mathf.Clamp(Morale, 0, MaxMorale);
    }

    public void SetMorale(float morale)
    {
        Morale = morale;
        Morale = Mathf.Clamp(Morale, 0, MaxMorale);
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

        //Update stats 
        item.OrpheusEquip(this);

        return previousItem;
    }

    public bool IsEquipped(Item item)
    {
        for (int i = 0; i < EquippedItems.Length; i++) { 
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
                item.OrpheusUnequip(this);
                EquippedItems[i] = null;
                return true;
            }
        }

        //No matching item
        return false;
    }

    public void Attack(float effectiveness, IHasMorale target)
    {
        target.ChangeMorale(effectiveness * AttackDamage * -1);


        //OnAttack.Invoke(effectiveness); //update a bit later: idk if we use this lol -Sam
    }


    public IEnumerator GetCombatChoice()
    {
        //CHUD VERSION. This should follow this process:
        // - Show UI
        // - ON UI CLICK Update CombatChoice then update combatDecisionMade (through a diff method)
        // - Hide UI (attack will be made by GameManager)

        //Chud Random For Variation I Guess!
        //TODO: Remove and move to UI And actual player choice!!
        switch(UnityEngine.Random.Range(0, 3))
        {
            case 0: //one third of the time, do a heavy attack
                CombatChoice = OrpheusDecision.HeavyAttack;
                break;
            default: //the 2 thirds of the time do a light attack
                CombatChoice = OrpheusDecision.LightAttack;
                break;
        }

        combatDecisionMade = true; //TODO: REMOVE ONCE LOGIC FOR GAMEPLAY IS DONE

        while(!combatDecisionMade)
        {
            yield return null;
        }
        combatDecisionMade=false; //for next time

    }

    public void ChooseLightAttack()
    {
        CombatChoice = OrpheusDecision.LightAttack;
        combatDecisionMade = true;
    }

    public void ChooseHeavyAttack()
    {
        CombatChoice = OrpheusDecision.HeavyAttack;
        combatDecisionMade = true;
    }


}
