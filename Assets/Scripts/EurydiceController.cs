using System;
using UnityEngine;

//for the definitions of ICanEquip and ICanAttack, see OrpheusController.cs
public class EurydiceController : MonoBehaviour, ICanEquip
{
    public Item[] EquippedItems { get; set; } = new Item[2];

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
        throw new System.NotImplementedException();
    }

    public bool IsEquipped(Item item)
    {
        throw new System.NotImplementedException();
    }

    public bool UnequpItem(Item item)
    {
        throw new System.NotImplementedException();
    }

}
