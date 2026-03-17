using System.Collections;
using UnityEngine;

public class CombatUITranslator : MonoBehaviour
{
    [SerializeField] private GameObject OCombatPanel;
    [SerializeField] private GameObject ECombatPanel;


    public void Start()
    {
        StartCoroutine(SetPanels());
    }

    public IEnumerator SetPanels()
    {
        while(GameManager.instance == null && GameManager.instance.Orpheus == null && GameManager.instance.Eurydice == null)
        {
            yield return null;
        }

        GameManager.instance.Orpheus.OrpheusCombatPanel = OCombatPanel;
        GameManager.instance.Eurydice.EurydiceCombatPanel = ECombatPanel;

        
    }

    public void ODoHeavyAttack()
    {
        if(GameManager.instance == null || GameManager.instance.Orpheus == null)
        {
            Debug.LogError("Game Manager or Orpheus was null! Cancelling Heavy Attack Input");
            return;
        }

        GameManager.instance.Orpheus.ChooseHeavyAttack();
    }

    public void ODoLightAttack()
    {
        if (GameManager.instance == null || GameManager.instance.Orpheus == null)
        {
            Debug.LogError("Game Manager or Orpheus was null! Cancelling Light Attack Input");
            return;
        }

        GameManager.instance.Orpheus.ChooseLightAttack();
    }

    public void EDoHeal()
    {
        if (GameManager.instance == null || GameManager.instance.Eurydice == null)
        {
            Debug.LogError("Game Manager or Eurydice was null! Cancelling Heal Input");
            return;
        }

        GameManager.instance.Eurydice.ChooseHeal();
    }

    public void EDoLaurel()
    {
        if (GameManager.instance == null || GameManager.instance.Eurydice == null)
        {
            Debug.LogError("Game Manager or Eurydice was null! Cancelling Laurel Input");
            return;
        }

        GameManager.instance.Eurydice.ChooseLaurel();

    }

    public void EDoTorch()
    {
        if (GameManager.instance == null || GameManager.instance.Eurydice == null)
        {
            Debug.LogError("Game Manager or Eurydice was null! Cancelling Torch Input");
            return;
        }

        GameManager.instance.Eurydice.ChooseTorch();

    }

}
