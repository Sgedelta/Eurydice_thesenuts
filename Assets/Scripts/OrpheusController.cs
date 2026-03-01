using System;
using UnityEngine;
public class OrpheusController : MonoBehaviour, ICanEquip, ICanAttack
{
    public Item[] EquippedItems { get; set; } = new Item[2];
    [SerializeField] public Item TEMPITEM;
    public Action<float> OnAttack { get; set; } //to be used by Eurydice effects
    public float AttackSpeed { get; set; } = 1;
    public float AttackMissAllowance { get; set; } = 1;
    public float AttackPerfectAllowance { get; set; } = 1;
    public float AttackDamage { get; set; } = 1;

    public float DamageTaken { get; set; } = 1;

    //========= orpheus specific controls =========


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

    public void Attack(float effectiveness)
    {
        OnAttack.Invoke(effectiveness);
        throw new NotImplementedException();
    }
}
