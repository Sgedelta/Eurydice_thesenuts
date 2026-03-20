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

    private Vector3 BarStartingPos;
    private Vector3 BarBGStartingPos;
    private float BarStartingScale;
    private float BarBGStartingScale;

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


        BarStartingPos = OBar.transform.localPosition;
        BarBGStartingPos = OBarBG.transform.localPosition;
        BarStartingScale = OBar.transform.localScale.x;
        //Multiplying by 2 to match the scale for the standard bar
        BarBGStartingScale = OBarBG.transform.localScale.x * 2;

        EnemyBar.transform.localPosition = BarStartingPos * -1;
        EnemyBar.transform.localScale = new Vector3(BarStartingScale, EnemyBar.transform.localScale.y, EnemyBar.transform.localScale.z);
        EnemyBar.SetActive(false);

        GameManager.instance.ActiveHealthBar = this;
    }
   
    //Updates health bg scaling, used for level up
    public void ScaleHealthBG(float OrpheusMaxPercent)
    {
        OBarBG.transform.localScale = new Vector2((BarBGStartingScale * OrpheusMaxPercent + 0.5f), OBarBG.transform.localScale.y);
        OBarBG.transform.localPosition = BarBGStartingPos + Vector3.left * BarBGStartingPos.x;
    }

    public void SetHealthData(float OPercentLeft, float OrpheusMaxPercent, float EnemyPercentLeft = -1)
    {   
        ScaleHealthBG(OrpheusMaxPercent);

        //scale to correct size
        OBar.transform.localScale = new Vector2((BarStartingScale * OrpheusMaxPercent) * OPercentLeft, OBar.transform.localScale.y);

        //set to correct position
        OBar.transform.localPosition = (BarStartingPos * OrpheusMaxPercent) + Vector3.left * (1 - OPercentLeft) * BarStartingPos.x;

        if(!haveEnemyHealth || EnemyPercentLeft == -1)
        {
            return;
        }

        //scale to correct size
        EnemyBar.transform.localScale = new Vector2(BarStartingScale * EnemyPercentLeft, EnemyBar.transform.localScale.y);

        //set to correct position
        EnemyBar.transform.localPosition = -BarStartingPos + Vector3.right * (1 - EnemyPercentLeft) * BarStartingPos.x;
    }



}

