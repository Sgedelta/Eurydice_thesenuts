using System;
using UnityEngine;

public interface ICanEquip
{
    public Item[] EquippedItems { get; set; }

    /// <summary>
    /// Attempts to put the item in the slot of the inventory in the current index, and adjusts all internal flags as appropriate
    /// </summary>
    /// <param name="i">The index to put the item into, 0 based</param>
    /// <returns>The item that was in that index previously, or null</returns>
    public Item EquipItem(int i, Item item);

    /// <summary>
    /// Unequips the item listed, no matter the index. Returns true if the item was removed, false otherwise
    /// </summary>
    /// <param name="item">The item to remove</param>
    /// <returns>true if the item was removed, false otherwise</returns>
    public bool UnequpItem(Item item);

    /// <summary>
    /// Checks if the given item is currently equipped
    /// </summary>
    /// <param name="item">the item to check</param>
    /// <returns>true if the item is equipped, false otherwise</returns>
    public bool IsEquipped(Item item);

}

public interface ICanAttack
{
    /// <summary>
    /// An attack method, which takes an effectiveness (how accurately you hit the notes)
    /// </summary>
    /// <param name="effectiveness"></param>
    public void Attack(float effectiveness);

    /// <summary>
    /// A event that is called on attack. Takes the effectiveness passed into attack. 
    /// Should be run during each character's attack method for any rider effects -> need for Eurydice
    /// </summary>
    public System.Action<float> OnAttack { get; set;}
}




public class OrpheusController : MonoBehaviour, ICanEquip, ICanAttack
{
    public Item[] EquippedItems { get; set; }
    public Action<float> OnAttack { get; set; } //to be used by Eurydice effects

    //========= orpheus specific controls =========
    /// <summary>
    /// Scalar value applied to the base speed for the attack slider
    /// </summary>
    [SerializeField] private float attackSpeed = 1;

    /// <summary>
    /// Scalar value applied to the base acceptable hit range for attack slider
    /// </summary>
    [SerializeField] private float attackMissAllowance = 1;

    /// <summary>
    /// Scalar value applied to the base acceptable perfect hit range for attack slider
    /// </summary>
    [SerializeField] private float attackPerfectAllowance = 1; 

    /// <summary>
    /// Amount of Sympathy gained by an attack
    /// </summary>
    [SerializeField] private float attackDamage = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EquippedItems = new Item[2];
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

    public void Attack(float effectiveness)
    {
        OnAttack.Invoke(effectiveness);
        throw new NotImplementedException();
    }
}
