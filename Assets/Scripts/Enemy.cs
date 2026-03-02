using UnityEngine;

public class Enemy : MonoBehaviour, IHasMorale
{
    public float Morale { get; set; } = 100;

    public bool IsAlive { get { return Morale > 0; } }

    public float MoraleDamagePerTurn = 5;


    public void ChangeMorale(float moraleChange)
    {
        Morale += moraleChange;
    }

    public void SetMorale(float morale)
    {
        Morale = morale;
    }

    private void Start()
    {
        //TEMP
        GameManager.instance.StartCombat(this);
    }

}
