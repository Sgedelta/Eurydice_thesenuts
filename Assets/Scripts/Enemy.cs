using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IHasMorale
{
    public float Morale { get; set; } = 1;
    public float MaxMorale { get; set; } = 1;

    public float MoralePercent { get { return Morale / MaxMorale; } }

    public bool IsAlive { get { return Morale > 0; } }

    public float MoraleDamagePerTurn = 12;

    public List<DoTTrigger> DoTTriggers;


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
        DoTTriggers = new List<DoTTrigger>();
        StartCoroutine(SetupOnNextFrame());
    }

    public void TriggerAllDoTTriggers()
    {
        //trigger damage, decrement turn, then remove if no more turns left
        for(int i = 0; i < DoTTriggers.Count; i++)
        {
            Morale -= DoTTriggers[i].Damage;
            DoTTriggers[i].TurnsLeft -= 1;
            if(DoTTriggers[i].TurnsLeft <= 0)
            {
                DoTTriggers.RemoveAt(i);
                i--;
            }
        }
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


public class DoTTrigger
{
    public float Damage;
    public int TurnsLeft;

    public DoTTrigger(float damage, int turnsLeft)
    {
        Damage = damage;
        TurnsLeft = turnsLeft;
    }
}
