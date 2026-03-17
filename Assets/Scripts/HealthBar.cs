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

    private Vector3 BarStartingPos;
    private float BarStartingScale;

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
        BarStartingScale = OBar.transform.localScale.x;

        EnemyBar.transform.localPosition = BarStartingPos * -1;
        EnemyBar.transform.localScale = new Vector3(BarStartingScale, EnemyBar.transform.localScale.y, EnemyBar.transform.localScale.z);
        EnemyBar.SetActive(false);

        GameManager.instance.ActiveHealthBar = this;
    }



    public void SetHealthData(float OPercentLeft, float EnemyPercentLeft = -1)
    {
        //scale to correct size
        OBar.transform.localScale = new Vector2(BarStartingScale * OPercentLeft, OBar.transform.localScale.y);

        //set to correct position
        OBar.transform.localPosition = BarStartingPos + Vector3.left * (1 - OPercentLeft) * BarStartingPos.x;

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

