using UnityEngine;

public class Item : MonoBehaviour
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

    //Adds modifier
    public void OrpheusEquip() { }
    //Removes modifier 
    public void OrpheusUnequip() { }
    //Enable ability to be selected
    public void EurydiceEquip() { }
    //Disable ability from being selected
    public void EurydiceUnequip() { }
    //Update onattack with data or do other stuff as needed
    public void EurydiceAbility() { }

}
