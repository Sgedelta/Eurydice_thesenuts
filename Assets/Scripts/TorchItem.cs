using UnityEngine;

public class TorchItem : Item
{
    //Percent modifier 
    //TODO: split into separate modifiers for characters?
    [SerializeField] private float modifyAmount;

    public override string name => "Torch";

    public override void OrpheusEquip(OrpheusController control)
    {
        //Modifies attack hitbox via increasing perfect allowing and decreasing miss allowance 
        //TODO: CHECK THAT THIS FUNCTIONS AS ASSUMED
        control.AttackPerfectAllowance *= modifyAmount;
        control.AttackMissAllowance /= modifyAmount;

    }
    public override void OrpheusUnequip(OrpheusController control)
    {
        //Resets values of allowances
        control.AttackMissAllowance *= modifyAmount;
        control.AttackPerfectAllowance /= modifyAmount;
    }
    public override void EurydiceAbility(OrpheusController control)
    {
        //Assumes modify amount is < 1
        control.DamageTaken *= modifyAmount;
    }

    public override void EurydiceEndAbility(OrpheusController control)
    {
        control.DamageTaken /= modifyAmount;
    }
}
