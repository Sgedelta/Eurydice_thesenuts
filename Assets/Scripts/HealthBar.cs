using System.Collections;
using UnityEngine;

public class HealthBar : MonoBehaviour
{

    private bool haveEnemyHealth = false;
    public bool HaveEnemyHealth { get { return haveEnemyHealth; } set {
            haveEnemyHealth = value;
            EnemyBar.SetActive(value);
        } }

    [SerializeField] GameObject OBar;
    [SerializeField] GameObject EnemyBar;
    [SerializeField] GameObject OBarBG;
    [SerializeField] GameObject EnemyBarBG;

    [SerializeField] Vector2 OXScale = new Vector2(2, 7.1f);
    [SerializeField] int OMaxLevel = 7;


    private void Start()
    {
        StartCoroutine(SetupOnceGMIsGood());

    }

    public IEnumerator SetupOnceGMIsGood()
    {
        while(GameManager.instance == null)
        {
            yield return null;
        }

        GameManager.instance.ActiveHealthBar = this;
    }
   
    //Updates health bg scaling, used for level up
    public void ScaleHealthBG(float OrpheusMaxPercent)
    {
        OBarBG.transform.localScale = new Vector2(Mathf.Lerp(OXScale.x, OXScale.y, OrpheusMaxPercent), OBarBG.transform.localScale.y);
    }

    public void SetHealthData(float OPercentLeft, float OrpheusMaxPercent, float EnemyPercentLeft = -1)
    {   
        ScaleHealthBG(OrpheusMaxPercent);

        //scale to correct size
        OBar.transform.localScale = new Vector2(OBarBG.transform.localScale.x * OPercentLeft, OBar.transform.localScale.y);

        if(!haveEnemyHealth || EnemyPercentLeft == -1)
        {
            return;
        }

        //scale to correct size
        EnemyBar.transform.localScale = new Vector2(EnemyBarBG.transform.localScale.x * EnemyPercentLeft, EnemyBar.transform.localScale.y);

       
    }



}

