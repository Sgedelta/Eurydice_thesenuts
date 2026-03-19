using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AttackController : MonoBehaviour
{
    [SerializeField] private GameObject InStart;
    [SerializeField] private GameObject InEnd;
    [SerializeField] private float stringActiveZone = .7f;

    [SerializeField] private GameObject InDisplay;
    [SerializeField] private GameObject StringDisplay;

    [SerializeField] private float baseSpeed = .1f;
    [SerializeField] private float baseMissAllowance = .8f;
    [SerializeField] private float basePerfectAllowance = .5f;

    private List<GameObject> stringDisplayInstances;
    private List<Vector2> targetLocs;
    private List<float> targetHits;
    public float PercentHit
    {
        get {
            float ret = 0;

            for (int i = 0; i < targetHits.Count; i++)
            {
                if (targetHits[i] > 0)
                {
                    ret += targetHits[i] / targetHits.Count;
                }
            }

            return ret;
        }
    }

    private float attackCurrentPercent = 0;
    private Vector2 attackCurrPos = Vector2.zero;


    private void Awake()
    {
        targetLocs = new List<Vector2>();
        targetHits = new List<float>();
        stringDisplayInstances = new List<GameObject>();
    }

    void Start()
    {

    }

    private void Update()
    {
        InDisplay.transform.position = attackCurrPos;

        // I *could* set up the input package.. but lets wait for a meeting, so chud polling will do
        if(Input.GetKeyDown(KeyCode.Space)) //TODO: at least replace this with an actionm later...
        {
            Vector2 StartToEnd = InEnd.transform.position - InStart.transform.position;
            //get attack pos projected onto path vector
            float attackOnPath = Vector2.Dot(attackCurrPos, StartToEnd) / StartToEnd.magnitude;


            //now get hit and perfect sizes (assumed: all strings have same size, change implementation otherwise)
            float hitError = baseMissAllowance * (GameManager.instance.Orpheus != null ? GameManager.instance.Orpheus.AttackMissAllowance : 1) / 2;
            float perfectError = basePerfectAllowance * (GameManager.instance.Orpheus != null ? GameManager.instance.Orpheus.AttackPerfectAllowance : 1) / 2;

            //loop thru all targets, checking hits. Once one is found, input is done being processed.
            for(int i = 0; i < stringDisplayInstances.Count; i++)
            {
                //if we've tried this string... don't!
                if (targetHits[i] != -1)
                {
                    continue;
                }

                //get string project on path
                float stringOnPath = Vector2.Dot(stringDisplayInstances[i].transform.position, StartToEnd) / StartToEnd.magnitude;
                //Debug.Log($"attackOnPath: {attackOnPath} and string {i} is {stringOnPath}");

                //calculate absolute diff
                float hitDist = Math.Abs(attackOnPath - stringOnPath);
                if(hitDist <= perfectError)
                {
                    targetHits[i] = 1;
                    StartCoroutine(DisplayAttackHit());
                    break;
                } 
                else if(hitDist <= hitError) 
                {
                    targetHits[i] = (hitDist - perfectError) / (hitError - perfectError);
                    StartCoroutine(DisplayAttackPartialHit(targetHits[i]));
                    break;
                }
            }
        }
    }

    public void SetupAttackData(float attackSpeed)
    {
        baseSpeed = attackSpeed;
    }

    public void SetupTargets(int stringNum)
    {
        List<bool> activeStrings = new List<bool>();
        for (int i = 0; i < stringNum; i++)
        {
            activeStrings.Add(true); //TODO: Change to Enum Normal
        }
        SetupTargets(stringNum, activeStrings);
    }

    public void SetupTargets(int stringNum, List<bool> activeStrings) //< change list bool here to list ENUM -> None, Normal, Bomb
    {
        if(activeStrings == null || activeStrings.Count != stringNum)
        {
            throw new ArgumentException("activeStrings is null or its count does not match provided stringNum!");
        }

        //setup backend
        targetLocs.Clear();
        targetHits.Clear();

        for(int i = 0; i < stringNum; i++)
        {
            //if the string is not active, skip it!
            if (!activeStrings[i]) //TODO: Check if None Enum
            {
                continue;
            }
            //setup a lerp value (1 being a special case)
            float t = .5f;
            if (stringNum != 1)
            {
                //get a "pure" value
                t = i / (float)(stringNum - 1);
                //resacle to the active zone
                t = Mathf.Lerp((1 - stringActiveZone) / 2, 1 - ((1 - stringActiveZone) / 2), t); //I heard you like lerps, so I lerped your lerp so you can lerp while you lerp
            }

            //calculate a new position, add it to targetLocs
            //TODO: ADD A BOMB????
            targetLocs.Add(Vector2.Lerp(InStart.transform.position, InEnd.transform.position, t));
            targetHits.Add(-1f); //add a miss in logs, for now
        }


        //setup frontend
        for(int i = 0;i < stringDisplayInstances.Count; i++)
        {
            Destroy(stringDisplayInstances[i]);
        }
        stringDisplayInstances.Clear();

        for (int i = 0; i < targetLocs.Count; i++)
        {
            GameObject newString = Instantiate(StringDisplay, transform);
            newString.transform.position = targetLocs[i];

            //if we wanted we could do a fun little math thing here and rotate them, but we'd need another point to make it look accurate and not parallel
            //which would also make the last part harder and... ugh.
            //so not for now!
            

            newString.SetActive(true);
            stringDisplayInstances.Add(newString);
        }

    }

    public IEnumerator MoveAttackIndicator()
    {
        while(attackCurrentPercent < 1)
        {
            attackCurrPos = Vector2.Lerp(InStart.transform.position, InEnd.transform.position, attackCurrentPercent);
            attackCurrentPercent = Mathf.Lerp(0, 1, attackCurrentPercent + (baseSpeed * Time.deltaTime
                * (GameManager.instance.Orpheus != null ? GameManager.instance.Orpheus.AttackSpeed : 1))
                );
            yield return new WaitForEndOfFrame();
        }


    }


    public IEnumerator DisplayAttackHit()
    {
        //bit of a chud version for playtesting...
        InDisplay.GetComponent<SpriteRenderer>().color = Color.green;
        yield return new WaitForSeconds(.2f);
        InDisplay.GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    public IEnumerator DisplayAttackPartialHit(float effectiveness)
    {
        //bit of a chud version for playtesting...
        InDisplay.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(.2f);
        InDisplay.GetComponent<SpriteRenderer>().color = Color.yellow;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(InStart.transform.position, "Toolbar Plus");

        Gizmos.DrawIcon(InEnd.transform.position, "Toolbar Minus");

        if(stringDisplayInstances == null)
        {
            //frame 1 error protection
            return;
        }

        Gizmos.color = Color.yellow;

        for (int i = 0; i < stringDisplayInstances.Count; i++) 
        {
            Gizmos.matrix = stringDisplayInstances[i].transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(baseMissAllowance * (GameManager.instance.Orpheus != null ? GameManager.instance.Orpheus.AttackMissAllowance : 1) 
                / stringDisplayInstances[i].transform.localScale.x,
                1, 1) );
        }

        Gizmos.color = Color.green;

        for (int i = 0; i < stringDisplayInstances.Count; i++)
        {
            Gizmos.matrix = stringDisplayInstances[i].transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(basePerfectAllowance * (GameManager.instance.Orpheus != null ? GameManager.instance.Orpheus.AttackPerfectAllowance : 1)
                / stringDisplayInstances[i].transform.localScale.x,
                1, 1));
        }

        Gizmos.matrix = Matrix4x4.identity;
        
    }
}
