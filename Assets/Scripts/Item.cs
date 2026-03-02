using UnityEngine;

public abstract class Item : MonoBehaviour
{
    //Hi julia :) 

    //Pseudocode brainstorm
    /*
     * Make this a parent class, with child scripts for items
     * Overridable functions -- Eurydice effect and orpheus effect
     * Call said functions when needed
     * o -- equipitem/unequip item should run the functions to modify stats
     * e should -> ability/item selected -> call function -> function updates onattack
     */
    public abstract string name { get; }

    protected int ActiveCount = 0;
    protected int MaxActiveCount = 1;

    //Adds modifier
    public virtual void OrpheusEquip(OrpheusController control) { }
    //Removes modifier 
    public virtual void OrpheusUnequip(OrpheusController control) { }
    //Update onattack with data or do other stuff as needed
    public virtual void EurydiceAbility(OrpheusController control) 
    {
        if(ActiveCount >= MaxActiveCount)
        {
            throw new System.Exception("Item Activated Too Many Times!");
        }
        ActiveCount += 1;
    
    }
    //Called at end of turn, removes temporary effects
    public virtual void EurydiceEndAbility(OrpheusController control) 
    {
        if(ActiveCount <= 0)
        {
            throw new System.Exception("Item Deactivated when not active!");
        }
        ActiveCount -= 1;
    }

}
