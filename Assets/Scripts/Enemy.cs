using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IHasMorale
{
    public float Morale { get; set; } = 100;
    public float MaxMorale { get; set; } = 100;

    public float MoralePercent { get { return Morale / MaxMorale; } }

    public bool IsAlive { get { return Morale > 0; } }

    public float MoraleDamagePerTurn = 12;


    public void ChangeMorale(float moraleChange)
    {
        Morale += moraleChange;
        Morale = Mathf.Clamp(Morale, 0, MaxMorale);
    }

    public void SetMorale(float morale)
    {
        Morale = morale;
        Morale = Mathf.Clamp(Morale, 0, MaxMorale);
    }

    private void Start()
    {
        StartCoroutine(SetupOnNextFrame());
    }

    private IEnumerator SetupOnNextFrame()
    {
        while(GameManager.instance == null && GameManager.instance.ActiveHealthBar == null)
        {
            yield return null;
        }
        
        //TEMP
        GameManager.instance.StartCombat(this);
    }

}
