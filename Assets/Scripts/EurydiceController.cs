using System;
using UnityEngine;

//for the definitions of ICanEquip and ICanAttack, see OrpheusController.cs
public class EurydiceController : MonoBehaviour, ICanEquip
{
    public Item[] EquippedItems { get; set; } = new Item[2];

    [SerializeField] public Item TEMPITEM;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public Item EquipItem(int i, Item item)
    {
        Item previousItem = EquippedItems[i];

        //NOTE: logic updated later might require running unequip here for the previous item
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

}
