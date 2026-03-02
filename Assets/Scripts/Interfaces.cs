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
    public bool UnequipItem(Item item);

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
    /// Scalar value applied to the base speed for the attack slider
    /// </summary>
    public float AttackSpeed { get; set; }

    /// <summary>
    /// Scalar value applied to the base acceptable hit range for attack slider
    /// </summary>
    public float AttackMissAllowance { get; set; }

    /// <summary>
    /// Scalar value applied to the base acceptable perfect hit range for attack slider
    /// </summary>
    public float AttackPerfectAllowance { get; set; }

    /// <summary>
    /// Amount of Sympathy gained by an attack
    /// </summary>
    public float AttackDamage { get; set; }

    /// <summary>
    /// An attack method, which takes an effectiveness (how accurately you hit the notes) and something with Morale
    /// </summary>
    /// <param name="effectiveness"></param>
    public void Attack(float effectiveness, IHasMorale target);

    /// <summary>
    /// A event that is called on attack. Takes the effectiveness passed into attack. 
    /// Should be run during each character's attack method for any rider effects -> need for Eurydice
    /// </summary>
    public System.Action<float> OnAttack { get; set; }
}

public interface IHasMorale
{
    /// <summary>
    /// Morale, a representation of the willpower of the character, acts as health
    /// </summary>
    public float Morale { get; set; }

    public float MaxMorale { get; set; }

    /// <summary>
    /// returns if Morale is positive
    /// </summary>
    public bool IsAlive { get; }

    /// <summary>
    /// Change Morale by the given amount
    /// </summary>
    /// <param name="moraleChange"></param>
    public void ChangeMorale(float moraleChange);


    public void SetMorale(float morale);


}


