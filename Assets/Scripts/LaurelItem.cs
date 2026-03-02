using UnityEngine;

public class LaurelItem : Item
{
    //Percent modifier
    //TODO: split into separate modifiers for characters?
    [SerializeField] private float modifyAmountOrpheus;
    [SerializeField] private float modifyAmountEurydice;

    //TODO: singleton behavior

    public override string name => "Laurel";

    public override void OrpheusEquip(OrpheusController control)
    {
        control.AttackDamage *= modifyAmountOrpheus;
        control.DamageTaken *= modifyAmountOrpheus;

    }
    public override void OrpheusUnequip(OrpheusController control)
    {
        control.AttackDamage /= modifyAmountOrpheus;
        control.DamageTaken /= modifyAmountOrpheus;
    }
    public override void EurydiceAbility(OrpheusController control)
    {
        //check for allowed activation
        try
        {
            base.EurydiceAbility(control);
        }
        catch (System.Exception e)
        {
            return;
        }


        control.AttackSpeed *= modifyAmountEurydice;
    }

    public override void EurydiceEndAbility(OrpheusController control)
    {
        //check for allowed deactivation
        try
        {
            base.EurydiceEndAbility(control);
        }
        catch (System.Exception e)
        {
            return;
        }

        control.AttackSpeed /= modifyAmountEurydice;
    }
}
