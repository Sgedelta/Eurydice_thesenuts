using UnityEngine;

public class LaurelItem : Item
{
    //Percent modifier
    //TODO: split into separate modifiers for characters?
    [SerializeField] private float modifyAmount;

    //TODO: singleton behavior

    public override string name => "Laurel";

    public override void OrpheusEquip(OrpheusController control)
    {
        control.AttackDamage *= modifyAmount;
        control.DamageTaken *= modifyAmount;

    }
    public override void OrpheusUnequip(OrpheusController control)
    {
        control.AttackDamage /= modifyAmount;
        control.DamageTaken /= modifyAmount;
    }
    public override void EurydiceAbility(OrpheusController control)
    {
        //Set effect to onattack
    }

    public override void EurydiceEndAbility(OrpheusController control)
    {
        //Remove effect from onattack
    }
}
